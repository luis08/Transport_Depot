using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;


namespace Transport_Depot_WCF.PaymentPosting
{
  class PaymentPostingDAL
  {
    private static Dictionary<string, string> settings = new Dictionary<string, string>();

    public static string GetSetting(string settingName)
    {
      var settingValue = string.Empty;
      if (settings.ContainsKey(settingName))
      {
        settingValue = settings[settingName];
      }
      else
      {
        settingValue = System.Configuration.ConfigurationManager.AppSettings[settingName];
        settings.Add(settingName, settingValue);
      }
      return settingValue;
    }

    internal void Post(PaymentRequestModel model)
    {
      if (!this.PostPaymentsSucceeded(model))
      {
        throw new Exception();
      }
    }

    internal string GetCheckNumber(ACHFileIdentifierModel paymentModel)
    {
      if (paymentModel == null)
      {
        throw new ArgumentException("payment model is null");
      }
      return string.Format("ACH_{0:yyyyMMdd}_{1}", paymentModel.FileDateTime, paymentModel.FileIDModifier);
    }

    internal bool CheckExists(string checkNumber)
    {
      var existsQuery = @"
        SELECT * FROM [CheckRegister]
        WHERE [cCheckNumber] = @CheckNumber
      ";
      using (var cn = new OleDbConnection(this.SettlementsConnectionString))
      using (var cmd = new OleDbCommand(existsQuery, cn))
      {
        cmd.Parameters.AddWithValue("@CheckNumber", checkNumber);
        cn.Open();
        var reader = cmd.ExecuteReader();
        var isEmpty = reader.Read();
        reader.Close();
        return isEmpty;
      }
    }

    internal IEnumerable<CheckExistenceTestModel> GetCheckTestModels(IEnumerable<ACHFileIdentifierModel> models)
    {

      var existenceModels = models.Select(m => new CheckExistenceTestModel
      {
        FileIdentifier = m,
        CheckNumber = this.GetCheckNumber(m),
        Exists = false
      });

      var checksXmlString = new XDocument(new XElement("checksRequested",
        existenceModels.Select(m => new XElement("check", m.CheckNumber)))).ToString();
      
      var checkQuery = @"
        DECLARE @ChecksXml XML
        SET @ChecksXml = @ChecksXmlString

        ;WITH [RequestedChecks] AS
        (
	        SELECT C.f.value('.', 'varchar(15)') AS [Check_Number]
	        FROM @ChecksXml.nodes('//check') C(f)
        )
        SELECT  [R].[Check_Number]
              , CASE
                  WHEN ( [I].[Check_Number_K] IS NULL ) THEN 0
                  ELSE 1 
                END AS [Ignore] 
              , CASE
		          WHEN ( [E].[cCheckNumber] IS NULL ) THEN 0
		          ELSE 1
		        END AS [Exists]
        FROM [RequestedChecks] [R]
          LEFT JOIN [Truckwin_TDPD_Access]...[CheckRegister] [E]
	        ON [E].[cCheckNumber]  = [R].[Check_Number]
          LEFT JOIN [Web_Payments].[dbo].[Ignore_Check] [I]
            ON [R].[Check_Number] = [I].[Check_Number_K]
        ";
      var checks = new List<string>();
      var checksTable = new DataTable();
      using (var cn = new SqlConnection(this.WebPaymentsSQLConnectionString))
      using (var cmd = new SqlCommand(checkQuery, cn))
      using (var adapter = new SqlDataAdapter(cmd))
      {
        cmd.Parameters.AddWithValue("@ChecksXmlString", checksXmlString);
        adapter.Fill(checksTable);
      }
      var newModels = checksTable.AsEnumerable()
        .Select(c => new
        {
          CheckNumber = c.Field<string>("Check_Number"),
          Exists = c.Field<int>("Exists").Equals(1),
          Ignore = c.Field<int>("Ignore").Equals(1)
        }).Join(existenceModels, c => c.CheckNumber, m => m.CheckNumber, (c, m) =>
          new CheckExistenceTestModel
          {
            FileIdentifier = m.FileIdentifier,
            CheckNumber = m.CheckNumber,
            Exists = c.Exists,
            Ignore = c.Ignore
          }).ToArray();

      return newModels;
    }

    internal void IgnoreCheck(ACHFileIdentifierModel paymentModel)
    {
      var ignoreCheckQuery = @"
        INSERT INTO [dbo].[Ignore_Check] ( [Check_Number_K] )  
        SELECT @CheckNumber AS [Check_Number_K]
        WHERE @CheckNumber
         NOT IN ( SELECT [Check_Number_K] FROM [dbo].[Ignore_Check] )";
      var checkNumber = this.GetCheckNumber(paymentModel);
      using (var cn = new SqlConnection(this.WebPaymentsSQLConnectionString))
      using (var cmd = new SqlCommand(ignoreCheckQuery, cn))
      {
        cmd.Parameters.AddWithValue("@CheckNumber", checkNumber);
        cn.Open();
        cmd.ExecuteNonQuery();
      }
    }

    internal void DontIgnoreCheck(ACHFileIdentifierModel paymentModel)
    {
      var ignoreCheckQuery = "DELETE FROM [dbo].[Ignore_Check] WHERE [Check_Number_K] = @CheckNumber ";
      var checkNumber = this.GetCheckNumber(paymentModel);

      using (var cn = new SqlConnection(this.WebPaymentsSQLConnectionString))
      using (var cmd = new SqlCommand(ignoreCheckQuery, cn))
      {
        cmd.Parameters.AddWithValue("@CheckNumber", checkNumber);
        cn.Open();
        cmd.ExecuteNonQuery();
      }
    }


    private bool PostPaymentsSucceeded(PaymentRequestModel model)
    {
      int[] paymentIds = this.GetPaymentIds(model.FileIdentifier);
      using (var cn = new OleDbConnection(this.SettlementsConnectionString))
      {
        OleDbTransaction achPostTransaction = null;
        try
        {
          var checkDate = DateTime.Now;
          var checkNumber = this.GetCheckNumber(model.FileIdentifier);

          cn.Open();
          achPostTransaction = cn.BeginTransaction();

          foreach (var paymentId in paymentIds)
          {
            this.SetCheckNumberAndDateOnTrips(cn, paymentId, achPostTransaction, checkDate, checkNumber);
            this.SetCheckNumberAndDateOnAdvances(cn, paymentId, achPostTransaction, checkDate, checkNumber);
          }

          this.AppendToRsPayableHistory(cn, achPostTransaction, checkNumber);
          this.AppendToRsPayment(cn, achPostTransaction, checkNumber);
          this.AppendToCheckRegister(cn, achPostTransaction, checkNumber, model.GlAccounts.SettlementAccount, model.GlAccounts.ACHCashAccount);
          this.AppendToGL(cn, achPostTransaction, checkNumber, checkDate, model.GlAccounts.SettlementAccount, model.GlAccounts.ACHCashAccount);
          this.UpdateRsPayableBalance(cn, checkNumber, achPostTransaction);
          this.DeleteRsPayableEntries(cn, achPostTransaction, checkNumber);

          achPostTransaction.Commit();
        }
        catch (Exception e)
        {
          if (achPostTransaction != null)
          { achPostTransaction.Rollback(); }
          return false;
        }
      }
      return true;
    }

    private void UpdateRsPayableBalance(OleDbConnection cn, string checkNumber, OleDbTransaction transaction)
    {
      var rsUpdateBalanceCommand = this.GetPostCommand(cn, transaction, "sp_Rs_Posting_Rs_Payable_Balance_Set");
      rsUpdateBalanceCommand.Parameters.AddWithValue("Check_Number", checkNumber);
      rsUpdateBalanceCommand.ExecuteNonQuery();
    }

    private int[] GetPaymentIds(ACHFileIdentifierModel model)
    {
      var paymentIdsDataTable = new DataTable();
      var query = @"
          SELECT [For_Payment_Scheduled_ID] FROM [Web_Payments].[dbo].[ACH_Lessor_Payment]
          WHERE ( [For_File_Creation_Date_K] = CONVERT( DATE, @FileDate ) )
            AND ( [For_File_ID_Modifier_K] = @FileIDModifier )
      ";
      using( var cn = new SqlConnection(this.WebPaymentsSQLConnectionString))
      using(var cmd = new SqlCommand(query, cn))
      using(var adapter = new SqlDataAdapter(cmd))
      {
        cmd.Parameters.AddWithValue("@FileDate", model.FileDateTime);
        cmd.Parameters.AddWithValue("@FileIDModifier", model.FileIDModifier);
        adapter.Fill(paymentIdsDataTable);
      }
      var paymentIds = paymentIdsDataTable.AsEnumerable()
        .Select(c => c.Field<int>("For_Payment_Scheduled_ID")).ToArray();
      return paymentIds;
    }

    private string GetNextGLTransactionId(OleDbConnection cn, OleDbTransaction achTransaction, DateTime checkDate)
    {
      var getNextTransactionCommand = this.GetPostCommand(cn, achTransaction, "sp_Get_Max_Gl_Transaction_Id_For_Date");
      getNextTransactionCommand.Parameters.AddWithValue("Module_Date", checkDate.ToShortDateString());
      var lastTransactionId = getNextTransactionCommand.ExecuteScalar();
      if ((lastTransactionId == null) || (lastTransactionId.Equals(DBNull.Value)))
      {
        return string.Format("{0:D4}{1:D2}00001", checkDate.Year, checkDate.Month);
      }
      var next = Convert.ToInt64(lastTransactionId) + 1;
      return next.ToString();
    }

    private void DeleteRsPayableEntries(OleDbConnection cn, OleDbTransaction transaction, string checkNumber)
    {
      var rsPayableDeleteCmd = this.GetPostCommand(cn, transaction, "sp_Rs_Scheduled_RsPayable_For_Check_Number_Delete");
      rsPayableDeleteCmd.Parameters.AddWithValue("Check_Number", checkNumber);
      rsPayableDeleteCmd.ExecuteNonQuery();
    }

    private void UpdateTransactionIdInGL(OleDbConnection cn, OleDbTransaction transaction, string checkNumber, DateTime checkDate, GLAccountModel settlementAccount, GLAccountModel achCashAccount)
    {
      var appendToGLCommand = this.GetPostCommand(cn, transaction, "sp_Rs_Posting_GL_TransactionId_Update_For_Check");
      appendToGLCommand.Parameters.AddWithValue("Check_Number", checkNumber);
      appendToGLCommand.Parameters.AddWithValue("Module_Date", checkDate.ToShortDateString());
      appendToGLCommand.Parameters.AddWithValue("Settlement_Dept", settlementAccount.Department);
      appendToGLCommand.Parameters.AddWithValue("Settlement_Acct", settlementAccount.Account);
      appendToGLCommand.Parameters.AddWithValue("ACH_Bank_Dept", achCashAccount.Department);
      appendToGLCommand.Parameters.AddWithValue("ACH_Bank_Acct", achCashAccount.Account);
      appendToGLCommand.ExecuteNonQuery();
    }

    private void AppendToGL(OleDbConnection cn, OleDbTransaction achPostTransaction, string checkNumber, DateTime checkDate, GLAccountModel settlementAccount, GLAccountModel achCashAccount)
    {
      var nextTransactionId = this.GetNextGLTransactionId(cn, achPostTransaction, checkDate);
      var appendToGLCommand = this.GetPostCommand(cn, achPostTransaction, "sp_Rs_Posting_GL_Entry_Append_For_Check");
      appendToGLCommand.Parameters.AddWithValue("Check_Number", checkNumber);
      appendToGLCommand.Parameters.AddWithValue("Transaction_ID", nextTransactionId);
      appendToGLCommand.Parameters.AddWithValue("Settlement_Dept", settlementAccount.Department);
      appendToGLCommand.Parameters.AddWithValue("Settlement_Acct", settlementAccount.Account);
      appendToGLCommand.Parameters.AddWithValue("ACH_Bank_Dept", achCashAccount.Department);
      appendToGLCommand.Parameters.AddWithValue("ACH_Bank_Acct", achCashAccount.Account);
      appendToGLCommand.ExecuteNonQuery();
    }

    private void AppendToCheckRegister(OleDbConnection cn, OleDbTransaction transaction, string checkNumber, GLAccountModel settlementAccount, GLAccountModel achCashAccount)
    {
      var appendCheckCommand = this.GetPostCommand(cn, transaction, "sp_Rs_Posting_CheckRegister_Append");
      appendCheckCommand.Parameters.AddWithValue("Check_Number", checkNumber);
      appendCheckCommand.Parameters.AddWithValue("Settlement_Dept", settlementAccount.Department);
      appendCheckCommand.Parameters.AddWithValue("Settlement_Acct", settlementAccount.Account);
      appendCheckCommand.Parameters.AddWithValue("ACH_Bank_Dept", achCashAccount.Department);
      appendCheckCommand.Parameters.AddWithValue("ACH_Bank_Acct", achCashAccount.Account);
      appendCheckCommand.ExecuteNonQuery();
    }

    private void AppendToRsPayment(OleDbConnection cn, OleDbTransaction transaction, string checkNumber)
    {
      var rsPaymentAppendCmd = this.GetPostCommand(cn, transaction, "sp_Rs_Posting_RsPayment_For_Check_Append");
      rsPaymentAppendCmd.Parameters.AddWithValue("Check_Number", checkNumber);
      rsPaymentAppendCmd.ExecuteNonQuery();
    }

    private void AppendToRsPayableHistory(OleDbConnection cn, OleDbTransaction transaction, string checkNumber)
    {
      var appendToHistoryCmd = this.GetPostCommand(cn, transaction, "sp_Rs_Scheduled_RsPayable_Append_To_History");
      appendToHistoryCmd.Parameters.AddWithValue("Check_Number", checkNumber);
      appendToHistoryCmd.ExecuteNonQuery();
    }

    private void SetCheckNumberAndDateOnAdvances(OleDbConnection cn, int paymentId, OleDbTransaction transaction, DateTime checkDate, string checkNumber)
    {
      var advancesCmd = this.GetPostCommand(cn, transaction, "sp_Rs_Scheduled_RsPayable_Advances_Set_Check_For_Payment_ID");
      advancesCmd.Parameters.AddWithValue("ForScheduledPaymentID", paymentId);
      advancesCmd.Parameters.AddWithValue("Check_Number", checkNumber);
      advancesCmd.Parameters.AddWithValue("Check_Date", checkDate.ToShortDateString());
      advancesCmd.ExecuteNonQuery();
    }

    private void SetCheckNumberAndDateOnTrips(OleDbConnection cn, int paymentId, OleDbTransaction transaction, DateTime checkDate, string checkNumber)
    {
      var tripsCmd = this.GetPostCommand(cn, transaction, "sp_Rs_Scheduled_RsPayable_Trips_Set_Check_For_Payment_ID");
      var param = new OleDbParameter("ForScheduledPaymentID", paymentId);
      param.DbType = System.Data.DbType.Int32;
      tripsCmd.Parameters.Add(param);
      tripsCmd.Parameters.AddWithValue("Check_Number", checkNumber);
      tripsCmd.Parameters.AddWithValue("Check_Date", checkDate.ToShortDateString());
      tripsCmd.ExecuteNonQuery();
    }

    private OleDbCommand GetPostCommand(OleDbConnection cn, OleDbTransaction transaction, string commandText)
    {
      return new OleDbCommand
      {
        Connection = cn,
        Transaction = transaction,
        CommandType = System.Data.CommandType.StoredProcedure,
        CommandText = commandText
      };
    }

    private IEnumerable<string> GetIgnoreChecks()
    { 
      var ignoredCheckNumbersTable = new DataTable();
      using( var cn = new SqlConnection(this.WebPaymentsSQLConnectionString))
      using(var cmd = new SqlCommand("SELECT * FROM [Ignore_Check]", cn))
      using(var adapter = new SqlDataAdapter(cmd))
      {
        adapter.Fill(ignoredCheckNumbersTable);
      }
      var checks = ignoredCheckNumbersTable.AsEnumerable()
        .Select(c => c.Field<string>("Check_Number_K")).ToList();
      return checks;
    }

    private string GetConnectionString( string name )
    {
      return System.Configuration.ConfigurationManager.ConnectionStrings[name].ConnectionString;
    }

    public string SettlementsConnectionString
    {
      get
      {
        var connectionString = GetConnectionString(SETTLEMENT_CONNECTION_STRING_KEY);
        return connectionString;
      }
    }
    public string WebPaymentsSQLConnectionString
    {
      get
      {
        var connectionString = GetConnectionString(WEB_PAYMENTS_CONNECTION_STRING);
        return connectionString;
      }
    }

    private const string SETTLEMENT_CONNECTION_STRING_KEY = "SettlementsConnectionString";
    private const string WEB_PAYMENTS_CONNECTION_STRING = "WebPaymentsConnectionString";


  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using WebPayments.ACHElements;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Globalization;


namespace DAL
{

  public class WebPaymentsDAL
  {
    public IEnumerable<DispatcherPayable> GetDispatcherPayables()
    {
      var cn = new SqlConnection(this.GetConnectionString());
      return GetDispatchers(cn);
    }

    private IEnumerable<DispatcherPayable> GetDispatchers(SqlConnection cn)
    {

      var query = @"
        SELECT [D].[cInitial] AS [Initials], 
               [DA].[VendorID]
        FROM [Truckwin_TDPD_Access]...[Dispatcher] [D]
        LEFT JOIN [dbo].[DispatcherAgent] [DA]
          ON [D].[cInitial] = [DA].[Initials]
      ";
      var connectionString = this.GetConnectionString();
      var tbl = new DataTable();

      using (var cmd = new SqlCommand(query, cn))
      using (var adapter = new SqlDataAdapter(cmd))
      {
        adapter.Fill(tbl);
      }
      if (tbl.Rows.Count == 0)
      { return new List<DispatcherPayable>(); }

      var dispatchers = tbl.AsEnumerable().Select(d => new DispatcherPayable
      {
        Initials =d.Field<string>("Initials"),
        VendorId = d.Field<string>("VendorID")
      });
      return dispatchers;
    }

    public DispatcherPayablesModel GetDispatcherPayableModel()
    {
      var model = new DispatcherPayablesModel();
      using( var cn = new SqlConnection(this.GetConnectionString()))
      {
        model.Dispatchers = this.GetDispatchers(cn).ToList();
        model.Initials = this.GetDispatcherInitials(cn).ToList();
        model.Vendors = this.GetDispatcherVendors(cn).ToList();
      }
      return model;
    }
    private IEnumerable<string> GetDispatcherInitials(SqlConnection cn)
    {
      var fieldName = "Initials";
      var query = "SELECT [cInitial] AS [Initials] FROM [Truckwin_TDPD_Access]...[Dispatcher] ORDER BY [cInitial]";
      return GetStrings(cn, fieldName, query);

    }

    private IEnumerable<string> GetDispatcherVendors(SqlConnection cn)
    {
      var fieldName = "VendorId";
      var query = "SELECT [cId] AS [VendorId] FROM [Truckwin_TDPD_Access]...[ApVendor] WHERE [cVendorType] = 'Agent' ORDER BY [cId]";
      return GetStrings(cn, fieldName, query);
    }

    private IEnumerable<string> GetStrings(SqlConnection cn, string fieldName, string query)
    {
      var tbl = new DataTable();
      using (var cmd = new SqlCommand(query, cn))
      {
        tbl = this.FetchDataTable(cmd);

      }
      if (tbl.Rows.Count == 0)
      {
        return new List<string>();
      }

      return tbl.AsEnumerable().Select(v => v.Field<string>(fieldName));
    }

    private DataTable FetchDataTable(SqlCommand cmd)
    {
      var tbl = new DataTable();
      using (var adapter = new SqlDataAdapter(cmd))
      {
        adapter.Fill(tbl);
      }
      return tbl;
    }

    
    internal WebPaymentsDue GetWebPaymentsDue()
    {
      var settlementsDue = this.GetUnPostedSettlements();
      var payrollDue = new List<PayableSalaryBatch>();
      var unpaidSettlements = this.RemovePaidSettlements(settlementsDue.Select(s => s.Id));
      var unpaidPayroll = this.RemovePaidPayrollBatches(payrollDue.Select(p => p.BatchId));
      var payablePayroll = payrollDue.Join(unpaidSettlements, s => s.BatchId, u => u, (s, u) => s);
      var payableSettlements = settlementsDue.Join(unpaidSettlements, s => s.Id, u => u, (s, u) => s);
      var payments_due = new WebPaymentsDue
      {
        Payroll = payablePayroll,
        Settlements = payableSettlements
      };

      return payments_due;
    }

    internal string GetSavedACHFileText(DateTime fileCreationDate, char fileIdModifier)
    {
      var fileTextQuery = @"
          SELECT [F].[File_Text] AS [FileText] 
          FROM [dbo].[ACH_Payment_File] [F]
          WHERE [F].[Creation_Date_K] = @FileCreationDate
            AND [F].[File_ID_Modifier_K] = @FileIdModifier
      ";
      var dataSet = new DataSet();
      using (var cn = new SqlConnection(this.GetACHFileConnectionString()))
      {
        using (var cmd = cn.CreateCommand())
        {
          cmd.CommandType = CommandType.Text;
          cmd.CommandText = fileTextQuery;
          cmd.Parameters.AddWithValue("@FileCreationDate", fileCreationDate);
          cmd.Parameters.AddWithValue("@FileIdModifier", fileIdModifier.ToString());
          using (var adapter = new SqlDataAdapter(cmd))
          {
            adapter.Fill(dataSet);
          }
        }
      }
      if (IsSingleTableDataSetEmpty(dataSet))
      {
        return string.Empty;
      }
      return dataSet.Tables[0].Rows[0]["FileText"].ToString();
    }

    internal IEnumerable<int> RemovePaidSettlements(IEnumerable<int> pendingSettlementPaymentIds)
    {
      XDocument paymentIdsXml = new XDocument(
        new XElement("paymentIds",
          pendingSettlementPaymentIds.Select(p =>
            new XElement("settlementScheduledId", p))));

      var storedProcedure = "sp_Select_Unpaid_Settlements";

      var dataSet = new DataSet();
      using (var cn = new SqlConnection(this.GetACHFileConnectionString()))
      {
        using (var cmd = cn.CreateCommand())
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = storedProcedure;
          cmd.Parameters.AddWithValue("@settlementIdsXml", paymentIdsXml.ToString());
          using (var adapter = new SqlDataAdapter(cmd))
          {
            adapter.Fill(dataSet);
          }
        }
      }
      if (IsSingleTableDataSetEmpty(dataSet))
      {
        return new int[] { };
      }
      return dataSet.Tables[0].AsEnumerable()
        .Select(p => p.Field<int>("PaymentId"));
    }

    internal IEnumerable<int> RemovePaidPayrollBatches(IEnumerable<int> pendingPayrollBatchIds)
    {
      XDocument payrollBatchIdsXml = new XDocument(
        new XElement("payrollBatches",
          pendingPayrollBatchIds.Select(p =>
            new XElement("batchId", p))));

      var storedProcedure = "sp_Select_Unpaid_PayrollBatches";

      var dataSet = new DataSet();
      using (var cn = new SqlConnection(this.GetACHFileConnectionString()))
      {
        using (var cmd = cn.CreateCommand())
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = storedProcedure;
          cmd.Parameters.AddWithValue("@payrollBatchesXml", payrollBatchIdsXml.ToString());
          using (var adapter = new SqlDataAdapter(cmd))
          {
            adapter.Fill(dataSet);
          }
        }
      }
      if (IsSingleTableDataSetEmpty(dataSet))
      {
        return new int[] { };
      }
      return dataSet.Tables[0].AsEnumerable()
        .Select(p => p.Field<int>("PaymentId"));
    }

    internal char GetNextFileIdModifier(DateTime forDate)
    {
      var nextIdModifier = '\0';

      using (var cn = new SqlConnection(this.GetACHFileConnectionString()))
      {
        nextIdModifier = this.GetNextFileIdModifier(forDate, cn);
      }

      return nextIdModifier;
    }

    internal void Save(ACHFile achFile)
    {
      if (achFile.CompanyBatchRecords.Count() == 0)
      {
        return;
      }
      var stored_procedure = "[dbo].[sp_append_ach_file]";
      
      using (var cn = new SqlConnection(this.GetACHFileConnectionString()))
      {
        if (cn.State != ConnectionState.Open)
        {
          cn.Open();
        }
        var achFileInXml = this.GetACHFileXml(achFile);
        WebPaymentsDebugger.DebugWrite(achFileInXml.ToString());
        using (var cmd = cn.CreateCommand())
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = stored_procedure;
          cmd.Parameters.AddWithValue("@achXml", achFileInXml.ToString());
          cmd.ExecuteNonQuery();
        }
      }
    }

    internal Dictionary<string, PayeeDFIAccount> GetPayeeAccounts()
    {
      var accountsDataSet = new DataSet();
      using (var cn = new SqlConnection(this.GetACHFileConnectionString()))
      {
        using (var cmd = cn.CreateCommand())
        {
          cmd.CommandText = @"
            SELECT PayeeID, LessorID AS TruckwinId, DFIIdentification, AccountNumber FROM [dbo].[Lessor_Payee_Banking_Information]
            UNION
            SELECT PayeeID, EmployeeID AS TruckwinId, DFIIdentification, AccountNumber FROM [dbo].[Employee_Payee_Banking_Information]";
          cmd.CommandType = CommandType.Text;
          using (var adapter = new SqlDataAdapter(cmd))
          {
            adapter.Fill(accountsDataSet);
          }
        }
      }
      if (IsSingleTableDataSetEmpty(accountsDataSet))
      {
        return new Dictionary<string,PayeeDFIAccount>();
      }
      var accounts = accountsDataSet.Tables[0].AsEnumerable()
        .Select(p => new PayeeDFIAccount
        {
          EmployeeID = p.Field<string>("TruckwinId"),
          DFIBankAccount = p.Field<string>("AccountNumber"),
          DFIRoutingIdentification = p.Field<string>("DFIIdentification")
        }).ToDictionary(p=>p.EmployeeID, p=>p);
      return accounts;
    }

    internal IEnumerable<ACHPaymentDisplayer> GetPaidACHFileDisplayers()
    {
      var paidACHDataSet = new DataSet();
      using (var cn = new SqlConnection(this.GetACHFileConnectionString()))
      {
        using (var cmd = cn.CreateCommand())
        {
          cmd.CommandType = CommandType.Text;
          cmd.CommandText = PaidACHFileDisplayersQuery;
          var achFileDataAdapter = new SqlDataAdapter(cmd);
          achFileDataAdapter.Fill(paidACHDataSet);
        }
      }

      if (IsSingleTableDataSetEmpty(paidACHDataSet))
      {
        return new List<ACHPaymentDisplayer>();
      }
      return paidACHDataSet.Tables[0].AsEnumerable()
        .Select(f => new ACHPaymentDisplayer
        {
          CreationDate = f.Field<DateTime>("CreationDate"),
          FileIDModifier = Convert.ToChar(f.Field<string>("FileIDModifier")),
          CreationTime = string.Concat(f.Field<string>("CreationTime").Substring(0, 2), ":", f.Field<string>("CreationTime").Substring(2, 2)),
          PayrollEmployeeCount = f.Field<int>("PaidEmployeeCount"),
          SettlementCount = f.Field<int>("SettlementCount")
        });
    }

    internal IEnumerable<PaidLessorSettlementViewModel> GetPaidLessorSettlements(DateTime fileCreationDate, char fileIdModifier)
    {
      var paidSettlementsDataSet = new DataSet();
      var paidSettlementsQuery = @"
          SELECT [P].[For_Batch_Number_K]       AS ACHBatchNumber
                ,[B].[Standard_Entry_Class_Code] AS TransactionType
                ,[P].[Trace_Number_Sequence_K]  AS TraceNumberSequence
                ,[For_Lessor_ID] AS LessorId
                ,[For_Payment_Scheduled_ID]  AS PaymentId
                ,[Amount]
            FROM [dbo].[ACH_Lessor_Payment] [P]
              INNER JOIN  [dbo].[ACH_Payment_Batch] [B]
                ON   [P].[For_File_Creation_Date_K] = [B].[For_File_Creation_Date_K]
                AND  [P].[For_File_ID_Modifier_K] = [B].[For_File_ID_Modifier_K]
                AND  [P].[For_Batch_Number_K] = [B].[Batch_Number_K]
            WHERE [P].[For_File_Creation_Date_K] = @FileCreationDate
              AND [P].[For_File_ID_Modifier_K]   = @FileIDModifier
      ";
      using (var cn = new SqlConnection(this.GetACHFileConnectionString()))
      {
        using (var cmd = cn.CreateCommand())
        {
          cmd.CommandType = CommandType.Text;
          cmd.CommandText = paidSettlementsQuery;
          cmd.Parameters.AddWithValue("@FileCreationDate", fileCreationDate);
          cmd.Parameters.AddWithValue("@FileIDModifier", fileIdModifier);
          var paidSettlementsDataAdapter = new SqlDataAdapter(cmd);
          paidSettlementsDataAdapter.Fill(paidSettlementsDataSet);
        }
      }
      if (IsSingleTableDataSetEmpty(paidSettlementsDataSet))
      {
        return new List<PaidLessorSettlementViewModel>();
      }

      return paidSettlementsDataSet.Tables[0].AsEnumerable()
        .Select(p => new PaidLessorSettlementViewModel
        {
          ACHBatchNumber = p.Field<int>("ACHBatchNumber"),
          TraceNumberSequence = p.Field<string>("TraceNumberSequence"),
          LessorId = p.Field<string>("LessorId"),
          PaymentId = p.Field<int>("PaymentId"),
          TransactionType = p.Field<string>("TransactionType"),
          Amount = p.Field<decimal>("Amount")
        });
    }

    internal IEnumerable<PaidPayrollItem> GetPaidPayrollItems(DateTime fileCreationDate, char fileIdModifier)
    {
      var paidPayrollItemsQuery = @"
          SELECT [P].[For_Batch_Number_K]       AS ACHBatchNumber
                ,[B].[Standard_Entry_Class_Code] AS TransactionType
                ,[P].[Trace_Number_Sequence_K]  AS TraceNumberSequence
                ,[P].[For_Employee_ID]          AS EmployeeId
                ,[P].[For_Batch_ID]             AS PayrollBatchNumber
                ,[P].[Amount]                   AS Amount
            FROM [dbo].[ACH_Payroll_Payment] [P]
              INNER JOIN  [dbo].[ACH_Payment_Batch] [B]
                ON   [P].[For_File_Creation_Date_K] = [B].[For_File_Creation_Date_K]
                AND  [P].[For_File_ID_Modifier_K] = [B].[For_File_ID_Modifier_K]
                AND  [P].[For_Batch_Number_K] = [B].[Batch_Number_K]
            WHERE [P].[For_File_Creation_Date_K] = @FileCreationDate
              AND [P].[For_File_ID_Modifier_K]   = @FileIDModifier
         
      ";
      var paidPayrollItemsDataSet = new DataSet();
      using (var cn = new SqlConnection(this.GetACHFileConnectionString()))
      {
        using (var cmd = cn.CreateCommand())
        {
          cmd.CommandType = CommandType.Text;
          cmd.CommandText = paidPayrollItemsQuery;
          cmd.Parameters.AddWithValue("@FileCreationDate", fileCreationDate);
          cmd.Parameters.AddWithValue("@FileIDModifier", fileIdModifier);
          var paidPayrollDataAdapter = new SqlDataAdapter(cmd);
          paidPayrollDataAdapter.Fill(paidPayrollItemsDataSet);
        }
      }
      if (IsSingleTableDataSetEmpty(paidPayrollItemsDataSet))
      {
        return new List<PaidPayrollItem>();
      }

      return paidPayrollItemsDataSet.Tables[0].AsEnumerable()
        .Select(p => new PaidPayrollItem
        {
          ACHBatchNumber = p.Field<int>("ACHBatchNumber"),
          TraceNumberSequence = p.Field<string>("TraceNumberSequence"),
          EmployeeId = p.Field<string>("EmployeeId"),
          PayrollBatchNumber = p.Field<int>("PayrollBatchNumber"),
          TransactionType = p.Field<string>("TransactionType"),
          Amount = p.Field<decimal>("Amount")
        });
    }

    private IEnumerable<PayableSettlement> GetUnPostedSettlements()
    {
      var storedProcedure = "sp_Rs_Get_Web_Payments_Pending";
      var dataSet = new DataSet();
      using (var cn = new SqlConnection(this.GetACHFileConnectionString()))
      {
        using (var cmd = cn.CreateCommand())
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = storedProcedure;
          using (var adapter = new SqlDataAdapter(cmd))
          {
            adapter.Fill(dataSet);
          }
        }
      }
      IEnumerable<PayableSettlement> payable_settlements = new LinkedList<PayableSettlement>();
      if (this.IsSingleTableDataSetEmpty(dataSet))
      {
        return payable_settlements;
      }
      payable_settlements = dataSet.Tables[0].AsEnumerable()
        .Select(s => new PayableSettlement
        {
          Id = s.Field<int>("Payment_ID"),
          LessorId = s.Field<string>("Lessor_ID"),
          LessorName = s.Field<string>("Lessor_Name"),
          IsCompany = s.IsNull("Is_Company") ? false : (s.Field<System.Int16>("Is_Company") != 0),
          AcceptedDate = s.Field<DateTime>("Accepted_Date"),
          Amount = s.Field<decimal>("Balance_Due")
        }).ToList();
      return payable_settlements;
    }

    private string GetACHFileXml(ACHFile achFile)
    {
      var file = achFile;
      var achXdocument = new XDocument(
        new XElement("achFile",
        new XAttribute("creationDate", file.FileHeaderRecord.FileCreationDateTime.ToString("yyyyMMdd")),
        new XAttribute("fileIdModifier", file.FileHeaderRecord.FileIDModifier),
        new XAttribute("creationTime", file.FileHeaderRecord.FileCreationTime),
        new XAttribute("immediateDestination", file.FileHeaderRecord.ImmediateDestination),
        new XAttribute("fileText", file.ToString()),
        new XElement("batches",
          new XAttribute("type", "payment"),
          file.CompanyBatchRecords
            .Where(b => b.EntryDetailRecords.Any(d => d.PaymentType == ACHPaymentType.Settlement))
            .Select(b => new XElement("batch",
              new XAttribute("number", b.HeaderRecord.BatchNumber),
              new XAttribute("standardEntryClassCode", b.HeaderRecord.StandardEntryClassCode),
              new XAttribute("companyEntryDescription", b.HeaderRecord.CompanyEntryDescription),
              new XAttribute("effectiveEntryDate", this.ParseACHDateYYMMDD(b.HeaderRecord.EffectiveEntryDate)),
              b.EntryDetailRecords
                .Where(d => d.PaymentType == ACHPaymentType.Settlement)
                .Select(d => new XElement("entryDetail",
                new XAttribute("traceNumber", d.TraceNumber),
                new XAttribute("lessorId", ACHFileBuilder.GetLessorId(d.ReceiverIdentificationNumber)),
                new XAttribute("settlementScheduledId", ACHFileBuilder.GetTransactionId(d.ReceiverIdentificationNumber, d.DiscretionaryData)),
                new XAttribute("amount", d.Amount))))))));
      //b.EntryDetailRecords.Select(d => new XElement(d.PaymentType == ACHPaymentType.Payroll ? "payrollPayment" : "lessorPayment",
      //  new XAttribute("traceNumberSequence", d.TraceNumber),
      //  new XAttribute(d.PaymentType == ACHPaymentType.Payroll ? "employeeId" : "lessorId", d.ReceiverIdentificationNumber),
      //  new XAttribute("amount", d.Amount))))))));
      return achXdocument.ToString();
    }

    private void SaveFileControl(FileControl fileControl, SqlConnection cn)
    {
      throw new NotImplementedException();
    }

    private DateTime ParseACHDateYYMMDD(string yymmdd)
    {
      var dateTime = DateTime.ParseExact(yymmdd,
          "yyMMdd",
          CultureInfo.InvariantCulture,
          DateTimeStyles.None);
      return dateTime;
    }

    private char GetNextFileIdModifier(DateTime forDate, SqlConnection cn)
    {
      var nextIdModifier = '\0';
      var lastCharIdModifier = '\0';
      var dataSet = new DataSet();
      using (var adapter = new SqlDataAdapter
      {
        SelectCommand = new SqlCommand
        {
          Connection = cn,
          CommandType = CommandType.Text,
          CommandText = @"
                          SELECT TOP 1 File_ID_Modifier_K AS File_ID_Modifier 
                          FROM ACH_Payment_File 
                          WHERE Creation_Date_K = @FileCreationDate 
                          ORDER BY File_ID_Modifier_K DESC      "
        }

      })
      {
        adapter.SelectCommand.Parameters.AddWithValue("@FileCreationDate", forDate.Date);
        adapter.Fill(dataSet);
        if (this.IsSingleTableDataSetEmpty(dataSet))
        { return 'A'; }
      }

      lastCharIdModifier = Convert.ToChar(dataSet.Tables[0].Rows[0]["File_ID_Modifier"]);

      int lastCharId = (int)lastCharIdModifier;
      nextIdModifier = (char)(++lastCharId);
      return nextIdModifier;
    }

    private string GetACHFileConnectionString()
    {
      return System.Configuration.ConfigurationManager
      .ConnectionStrings["ACHFileStorageConnectionString"].ConnectionString;
    }

    private string GetConnectionString()
    {
      return System.Configuration.ConfigurationManager
      .ConnectionStrings["AccessReplacementConnectionString"].ConnectionString;
    }

    private bool IsSingleTableDataSetEmpty(DataSet data_set)
    {
      if (data_set == null)
      { return true; }
      if (data_set.Tables == null)
      { return true; }
      if (data_set.Tables.Count == 0)
      { return true; }
      if (data_set.Tables[0].Rows.Count == 0)
      { return true; }
      return false;
    }

    public IEnumerable<PayeeDTO> GetEmployeePayees(bool activeOnly)
    {
      return this.GetPayees(EmployeePayeesQuery, activeOnly);
    }

    public IEnumerable<PayeeDTO> GetLessorPayees(bool activeOnly)
    {
      return this.GetPayees(LessorPayeesQuery, activeOnly);
    }



    private IEnumerable<PayeeDTO> GetPayees(string sourceQuery, bool activeOnly)
    {
      var dataSet = new DataSet();
      using (var cn = new SqlConnection(this.GetACHFileConnectionString()))
      {
        using (var cmd = cn.CreateCommand())
        {
          cmd.CommandType = CommandType.Text;
          cmd.CommandText = sourceQuery;
          cmd.Parameters.AddWithValue("@ActiveOnly", activeOnly);
          using (var adapter = new SqlDataAdapter(cmd))
          {
            adapter.Fill(dataSet);
          }
        }
      }
      if (IsSingleTableDataSetEmpty(dataSet))
      { return new List<PayeeDTO>(); }
      return dataSet.Tables[0].AsEnumerable()
        .Select(p => new PayeeDTO
        {
          PayeeId = p.Field<int>("PayeeId"),
          TruckwinId = p.Field<string>("TruckwinId"),
          DFIIdentification = p.Field<string>("DFIIdentification"),
          AccountNumber = p.Field<string>("AccountNumber")
        });
    }

    public void SaveEmployee(PayeeDTO employee)
    {
      var storedProcedure = "[dbo].[sp_Update_Employee_Payee]";
      var parameterName = "@EmployeeID";
      this.SavePayee(employee, storedProcedure, parameterName);
    }


    public void SaveLessor(PayeeDTO lessor)
    {
      var storedProcedure = "[dbo].[sp_Update_Lessor_Payee]";
      var parameterName = "@LessorID";
      this.SavePayee(lessor, storedProcedure, parameterName);
    }

    private void SavePayee(PayeeDTO payee, string storedProcedure, string typeParameterName)
    {
      using (var cn = new SqlConnection(this.GetACHFileConnectionString()))
      {
        cn.Open();
        using (var cmd = cn.CreateCommand())
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = storedProcedure;
          cmd.Parameters.AddWithValue(typeParameterName, payee.TruckwinId);
          cmd.Parameters.AddWithValue("@DFIIdentification", payee.DFIIdentification);
          cmd.Parameters.AddWithValue("@AccountNumber", payee.AccountNumber);
          cmd.ExecuteNonQuery();
        }
      }
    }

    public void DeleteLessor(int payeeId)
    {
      this.DeletePayee(payeeId, "sp_Delete_Lessor");
    }

    public void DeleteEmployee(int payeeId)
    {
      this.DeletePayee(payeeId, "sp_Delete_Employee");
    }

    public TruckWinPayees GetTruckWinPayeesNoPayee()
    {
      return new TruckWinPayees
      {
        Employees = this.GetTruckwinIds("sp_Get_Employees_No_Payee"),
        Lessors = this.GetTruckwinIds("sp_Get_Lessors_No_Payee")
      };
    }

    public TruckWinPayees GetTruckWinPayees()
    {
      var lessors = this.GetTruckwinIds("sp_Get_Lessors");
      var employees = this.GetTruckwinIds("sp_Get_Employees");
      
      return new TruckWinPayees
      {
        Employees =employees,
        Lessors = lessors 
      };
    }

    private IEnumerable<string> GetTruckwinIds(string storedProcedure)
    {
      var dataSet = new DataSet();
      using (var cn = new SqlConnection(this.GetACHFileConnectionString()))
      {
        using (var cmd = cn.CreateCommand())
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = storedProcedure;
          using (var adapter = new SqlDataAdapter(cmd))
          {
            adapter.Fill(dataSet);
          }
        }
      }
      if (IsSingleTableDataSetEmpty(dataSet))
      {
        return new List<string>();
      }
      return dataSet.Tables[0].AsEnumerable()
        .Select(p => p.Field<string>(0));
    }


    private void DeletePayee(int payeeId, string storedProcedure)
    {
      using (var cn = new SqlConnection(this.GetACHFileConnectionString()))
      {
        cn.Open();
        using (var cmd = cn.CreateCommand())
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = storedProcedure;
          cmd.Parameters.AddWithValue("@PayeeID", payeeId);
          cmd.ExecuteNonQuery();
        }
      }
    }

    private const string PaidACHFileDisplayersQuery = @"
        ;WITH Settlements 
        AS
        (
          SELECT [S].[For_File_Creation_Date_K],
                 [S].[For_File_ID_Modifier_K],
                 COUNT(*) AS [SettlementCount]
          FROM [dbo].[ACH_Lessor_Payment] [S]
          GROUP BY [S].[For_File_Creation_Date_K], [S].[For_File_ID_Modifier_K] 
        ),
        PayrolPayments AS 
        (
          SELECT [P].[For_File_Creation_Date_K],
                 [P].[For_File_ID_Modifier_K],
                 COUNT(*) AS [PaidEmployeeCount]
          FROM [dbo].[ACH_Payroll_Payment] [P]
          GROUP BY [P].[For_File_Creation_Date_K], [P].[For_File_ID_Modifier_K] 
        )

        SELECT [F].[Creation_Date_K] AS [CreationDate],
               [F].[Creation_Time]   AS [CreationTime],
               [F].[File_ID_Modifier_K] AS [FileIDModifier],
               CASE 
                WHEN [S].[SettlementCount] IS NULL THEN 0
                ELSE [S].[SettlementCount]  
               END AS [SettlementCount],
               CASE
                WHEN [P].[PaidEmployeeCount] IS NULL THEN 0
                ELSE [P].[PaidEmployeeCount] 
               END AS [PaidEmployeeCount]
        FROM [dbo].[ACH_Payment_File] [F]
          LEFT JOIN [Settlements] [S]
            ON [F].[Creation_Date_K] = [S].[For_File_Creation_Date_K]
              AND [F].[File_ID_Modifier_K] = [S].[For_File_ID_Modifier_K]
          LEFT JOIN [PayrolPayments] [P]
            ON [F].[Creation_Date_K] = [P].[For_File_Creation_Date_K]
              AND [F].[File_ID_Modifier_K] = [P].[For_File_ID_Modifier_K]
    ";

    private const string LessorPayeesQuery = @"
      SELECT [PayeeId]
            ,[LessorID] AS [TruckWinId]
            ,[DFIIdentification]
            ,[AccountNumber]
      FROM [dbo].[Lessor_Payee_Banking_Information_Active]
      WHERE @ActiveOnly = 1

      UNION

      SELECT [PayeeId]
            ,[LessorID] AS [TruckWinId]
            ,[DFIIdentification]
            ,[AccountNumber]
      FROM [dbo].[Lessor_Payee_Banking_Information]
      WHERE @ActiveOnly = 0
    ";

    private const string EmployeePayeesQuery = @"
      SELECT [PayeeId], 
             [EmployeeID] AS [TruckwinId],
             [DFIIdentification],
             [AccountNumber]
      FROM [dbo].[Employee_Payee_Banking_Information_Active] 
      WHERE @ActiveOnly = 1 

      UNION

      SELECT [PayeeId], 
             [EmployeeID] AS [TruckwinId],
             [DFIIdentification],
             [AccountNumber]
      FROM [dbo].[Employee_Payee_Banking_Information]
      WHERE @ActiveOnly = 0
    ";
  }
}
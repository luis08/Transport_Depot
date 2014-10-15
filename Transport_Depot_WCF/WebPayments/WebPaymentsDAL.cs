using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;

namespace Transport_Depot_WCF.WebPayments
{
  class WebPaymentsDAL
  {



    private OleDbConnection Connection
    {
      get { return new OleDbConnection(this.ConnectionString); }
    }

    private string ConnectionString
    {
      get
      {
        //return ConfigurationSettings.AppSettings[SETTLEMENT_CONNECTION_STRING].ToString(); 
        return CONNECTION_STRING;
      }
    }
    private const string CONNECTION_STRING = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Transport_Depot\Transport_Depot_WCF\App_Data\Web_App_DB.mdb;Persist Security Info=True";

    internal WebPaymentsDue GetWebPaymentsDue()
    {
      using (var cn = this.Connection)
      {
        return new WebPaymentsDue
        {
          SettlementsDue = this.GetSettlementsDue( cn ),
          PayrollDue = this.GetPayrollDue( cn )
        };
      }
    }

    private IEnumerable<PayableSalaryBatch> GetPayrollDue(OleDbConnection cn)
    {
      return new List<PayableSalaryBatch>();
    }

    private IEnumerable<PayableSettlement> GetSettlementsDue(OleDbConnection cn)
    {
      var adapter = new OleDbDataAdapter
      {
        SelectCommand = new OleDbCommand
        {
          Connection = cn,
          CommandType = System.Data.CommandType.TableDirect,
          CommandText = "vw_Wp_Ready_To_Pay"
        }
      };

      var data_set = new DataSet();
      adapter.Fill(data_set);
      
      var payable_settlements = new LinkedList<PayableSettlement>();
      if (this.IsDataSetEmpty(data_set))
      { return payable_settlements; }

      
      foreach (DataRow record in data_set.Tables[0].Rows)
      {
        payable_settlements.AddLast(new PayableSettlement
        {
          LessorId = record["Lessor_ID"].ToString(),
          IsCompany = this.ParseMSAccessBolean(record["Is_Company"].ToString()),
          PaymentId = int.Parse(record["Payment_ID"].ToString()), 
          AcceptedDate = this.ParseDate(record["Accepted_Date"].ToString()),
          BalanceDue = this.ParseDecimal(record["Balance_Due"].ToString())
        });
      }
      return payable_settlements;
    }

    private DateTime ParseDate(string date_string)
    {
      var parsed_date = DateTime.MinValue;
      if (!DateTime.TryParse(date_string, out parsed_date))
      { return DateTime.MinValue; }
      return parsed_date;
    }

    private bool ParseMSAccessBolean(string boolean_string)
    {
      var int_boolean = 0;
      boolean_string = boolean_string.Trim();

      if (!int.TryParse(boolean_string, out int_boolean))
      {
        throw new InvalidCastException(
            string.Format("Cannot convert '{0}' to boolean", boolean_string));

      }
      return int_boolean == 0 ? false : true;
    }

    private decimal ParseDecimal(string decimal_string)
    {
      decimal dec = 0.0m;

      if (!decimal.TryParse(decimal_string, out dec))
      { return 0.0m; }

      return dec;
    }



    private bool IsDataSetEmpty(DataSet data_set)
    {
      if (data_set == null)
      { return true; }
      if (data_set.Tables == null)
      { return true; }
      if (data_set.Tables.Count == 0)
      { return true; }
      if (data_set.Tables[0].Rows == null)
      { return true; }
      if (data_set.Tables[0].Rows.Count == 0)
      { return true; }
      return false;
    }
  }
}

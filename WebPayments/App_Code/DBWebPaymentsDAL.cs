using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using WebPayments.ACHElements;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Globalization;

public class DBWebPaymentsDAL : IWebPaymentsDAL
{
  public WebPaymentsDue GetWebPaymentsDue()
  {
    var settlementsDue = this.GetUnPostedSettlements();
    var payrollDue = new List<PayableSalaryBatch>();

    var webPaymentsDAL = new DAL.WebPaymentsDAL();
    var unpaidSettlements = webPaymentsDAL.RemovePaidSettlements(settlementsDue.Select(s => s.Id));
    var unpaidPayroll = webPaymentsDAL.RemovePaidPayrollBatches(payrollDue.Select(p => p.BatchId));
    var payablePayroll = payrollDue.Join(unpaidSettlements, s => s.BatchId, u => u, (s, u) => s);
    var payableSettlements = settlementsDue.Join(unpaidSettlements, s => s.Id, u => u, (s, u) => s);
    var payments_due = new WebPaymentsDue
    {
      Payroll = payablePayroll,
      Settlements = payableSettlements
    };

    return payments_due;
  }

  public void Save(ACHFile file)
  {
    throw new NotImplementedException();
  }

  private IEnumerable<PayableSettlement> GetUnPostedSettlements()
  {
    var adapter = new OleDbDataAdapter
    {
      SelectCommand = new OleDbCommand
      {
        Connection = this.CurrentConnection,
        CommandType = System.Data.CommandType.StoredProcedure,
        CommandText = "sp_Rs_Get_Web_Payments_Pending" // vw_Rs_Payment_Scheduled_Balance_Total"
      }
    };
    var dataset = new DataSet();
    adapter.Fill(dataset);

    this.CurrentConnection.Close();

    IEnumerable<PayableSettlement> payable_settlements = new LinkedList<PayableSettlement>();
    if (IsDataSetEmpty(dataset))
    {
      return payable_settlements;
    }

    payable_settlements = dataset.Tables[0].AsEnumerable()
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

  public TruckWinPayees GetTruckWinPayees()
  {
    return new TruckWinPayees
    {
      Employees = this.GetEmployees().ToList(),
      Lessors = this.GetLessors().ToList()
    };
  }

  private IEnumerable<string> GetEmployees()
  {
    var adapter = new OleDbDataAdapter
    {
      SelectCommand = new OleDbCommand
      {
        Connection = this.CurrentConnection,
        CommandType = System.Data.CommandType.Text,
        CommandText = "SELECT [cEmployeeId] AS [EmployeeID]  FROM [PrEmployee]  ORDER BY [cEmployeeId]" // vw_Rs_Payment_Scheduled_Balance_Total"
      }
    };
    var employeeTable = new DataTable();
    adapter.Fill(employeeTable);
    if (employeeTable == null)
    {
      return new LinkedList<string>();
    }
    return employeeTable
      .AsEnumerable()
      .Select(p => p.Field<string>("EmployeeID"));
  }

  private IEnumerable<string> GetLessors()
  {
    var adapter = new OleDbDataAdapter
    {
      SelectCommand = new OleDbCommand
      {
        Connection = this.CurrentConnection,
        CommandType = System.Data.CommandType.Text,
        CommandText = "SELECT [cId] AS [LessorID] From [RsLessor] ORDER BY [cId]"
      }
    };
    var lessorTable = new DataTable();
    adapter.Fill(lessorTable);
    if (lessorTable == null)
    {
      return new LinkedList<string>();
    }
    return lessorTable
      .AsEnumerable()
      .Select(p => p.Field<string>("LessorID"));
  }



  private OleDbConnection CurrentConnection
  {
    get
    {

      if (this._cn == null)
      {
        this._cn = new OleDbConnection(this.ConnectionString);
      }
      if (this._cn.State != System.Data.ConnectionState.Open)
      {
        try
        {
          this._cn.Open();
        }
        catch (Exception e)
        {
          using (var writer = new System.IO.StreamWriter(@"C:\Transport_Depot\WebPayments\App_Code\errors.txt"))
          {
            writer.WriteLine(e.Message);
            if (e.InnerException != null)
            {
              writer.WriteLine(e.InnerException.Message);
            }
            writer.WriteLine(System.IO.File.Exists(@"WebPayments"));
            writer.WriteLine(System.Security.Principal.WindowsIdentity.GetCurrent().Name);
          }
        }
      }
      return this._cn;
    }
  }

  private OleDbConnection _cn;

  private string ConnectionString
  {
    get { return CONNECTION_STRING; }
  }

  //TODO: Use connection string in web.config
  private const string CONNECTION_STRING = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=App_Data/WebPayments.mdb;Persist Security Info=True";// @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Transport_Depot\Transport_Depot_WCF\App_Data\Web_App_DB.mdb;";

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

  private string GetACHFileConnectionString()
  {
    return System.Configuration.ConfigurationManager
    .ConnectionStrings["ACHFileStorageConnectionString"].ConnectionString;

  }
}


using System.Collections.Generic;
using TransportDepot.Models.DB;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TransportDepot.Models.AR;


namespace TransportDepot.Data.DB
{
  public class AgingDataSource
  {
    private DataSource _dataSource = new DataSource();
    public IEnumerable<CustomerAging> GetAging()
    {
      var agingDataTable = new DataTable();
      var fullQuery = string.Concat(
        ";WITH ",
        AgingQueries.CTE,
        Environment.NewLine,
        "    SELECT * FROM [Aging]");
      using (var cmd = new SqlCommand(fullQuery))
      {
        agingDataTable = this._dataSource.FetchCommand(cmd);
      }

      var agingCustomers = agingDataTable.AsEnumerable()
        .Select(c => new CustomerAging
        {
          CustomerID = c.Field<string>("CustomerID"),
          InvoiceNumber = c.Field<string>("Invoice_Number"),
          InvoiceDate = c.Field<DateTime>("Invoice_Date"),
          TripNumber = c.Field<string>("Trip_Number"),
          CustomerReference = c.Field<string>("Customer_Reference"),
          BalanceDue = c.Field<decimal>("Balance_Due"),
        }).ToList();
      return agingCustomers;
    }

  }
}

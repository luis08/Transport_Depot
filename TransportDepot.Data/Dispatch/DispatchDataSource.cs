using System;
using System.Collections.Generic;
using System.Linq;
using TransportDepot.Models.Dispatch;
using System.Data.SqlClient;
using System.Data;

namespace TransportDepot.Data.Dispatch
{
  public class DispatchDataSource : IDataSource
  {
    private Utilities _utilities = new Utilities();
    public string ConnectionString
    {
      get { return this._utilities.ConnectionString; }
    }

    public IEnumerable<Dispatcher> GetDispatchers()
    {
      var dt = new DataTable();
      using (var cmd = new SqlCommand(Queries.Dispatchers))
      {
        var ds = new DataSource();
        dt = ds.FetchDataTable(cmd);
      }
      var dispatchers = dt.AsEnumerable()
        .Select(d => new Dispatcher
        {
          VendorId = d.Field<string>("VendorID"),
          Initials = d.Field<string>("Initials"),
          Name = d.Field<string>("Name")
        });
      return dispatchers.ToList();
    }

    public IEnumerable<DateTime> GetAllCommissionDates()
    {
      var dates = GetCommissionDates().Select(d=>d.CommissionPaymentDate).Distinct();
      return dates;
    }


    public IEnumerable<DateTime> GetAllCommissionDates(string dispatcherId)
    {
      var commissionDates = this.GetCommissionDates()
        .Where(d => d.DispatcherId.Equals(dispatcherId, StringComparison.OrdinalIgnoreCase))
        .Select(d=>d.CommissionPaymentDate)
        .OrderBy(d => d);
      return commissionDates;
    }

    public IEnumerable<DispatcherCommission> GetDispatcherCommissions(DateTime dueDate)
    {
      var tbl = new DataTable();
      var ds = new DataSource();
      var qry = string.Concat(Queries.CommissionsCTE,
        Environment.NewLine,
        @"
          SELECT * 
          FROM [AllCommissions] 
          WHERE ( [CommissionPaymentDate] = @CommissionPaymentDate )
        ");
      using (var cmd = new SqlCommand(qry))
      {
        cmd.Parameters.AddWithValue("@CommissionPaymentDate", dueDate);
        tbl = ds.FetchDataTable(cmd);
      }

      var commissions = this.GetCommissions(tbl);
      return commissions;
    }


    public IEnumerable<DispatcherCommission> GetDispatcherCommissions(string dispatcherId, DateTime dueDate)
    {
      var tbl = new DataTable();
      var ds = new DataSource();
      var qry = string.Concat(Queries.CommissionsCTE,
        Environment.NewLine,
        @"
        SELECT * 
        FROM [AllCommissions] 
        WHERE ( [CommissionPaymentDate] = @CommissionPaymentDate )
          AND ( [DispatcherId] = @DispatcherId ) 
      ");
      using (var cmd = new SqlCommand(qry))
      {
        cmd.Parameters.AddWithValue("@CommissionPaymentDate", dueDate);
        cmd.Parameters.AddWithValue("@DispatcherId", dispatcherId);
        tbl = ds.FetchDataTable(cmd);
      }

      var commissions = this.GetCommissions(tbl);
      return commissions;
    }

    public IEnumerable<DispatcherCommission> GetUnpaidCommissions()
    {
      var tbl = new DataTable();
      var ds = new DataSource();
      var qry = string.Concat(Queries.CommissionsCTE,
        Environment.NewLine,
        @"
        SELECT * 
        FROM [UnpaidCommissions] 
      ");
      using (var cmd = new SqlCommand(qry))
      {
        tbl = ds.FetchDataTable(cmd);
      }

      var commissions = this.GetCommissions(tbl);
      return commissions;
    }
    public IEnumerable<DriverContact> GetDriverContacts(bool activeOnly)
    {
      var contactsTable = new DataTable();
      using(var cn = new SqlConnection(this.ConnectionString))
      using (var cmd = new SqlCommand(DriverContactsQuery, cn))
      using(var adapter = new SqlDataAdapter(cmd))
      {
        adapter.Fill(contactsTable);
      }
      if (this._utilities.IsEmpty(contactsTable))
      {
        return new List<DriverContact>();
      }
      var contacts = contactsTable.AsEnumerable()
        .Where( c=> (!activeOnly) || ( activeOnly && c.Field<bool>("Active") ))
        .Select(c => new DriverContact
        {
          Id = c.Field<string>("ID"),
          Name = c.Field<string>("Name"),
          Card = c.Field<string>("Card"),
          CellPhone = c.Field<string>("Cell_Phone"),
          HomePhone = c.Field<string>("Home_Phone"),
          LessorPhone = c.Field<string>("Lessor_Phone"),
          LessorName = c.Field<string>("Lessor_Name")
        });
      return contacts;
    }

    public IEnumerable<MovingFreightTrip> GetMovingFreight()
    {
      var tbl = new DataTable();
      var ds = new DataSource();

      using (var cmd = new SqlCommand(Queries.MovingFreight))
      {
        tbl = ds.FetchDataTable(cmd);
      }
      try
      {
        var movingFreight = tbl.AsEnumerable()
          .Select(t => new MovingFreightTrip
          {
            Customer = GetCustomer(t),
            CustomerId = t.Field<string>("CustomerID"),
            Drivers = GetDrivers(t),
            TripNumber = t.Field<string>("TripNumber"),
            Tractor = t.Field<string>("TractorID"),
            Trailer = t.Field<string>("TrailerID"),
            From = this.GetFrom(t),
            To = this.GetTo(t)
          });
        return movingFreight;
      }
      catch (Exception e)
      {
        return new MovingFreightTrip[] { new MovingFreightTrip { Customer = new Models.Business.Company { Name = "Luis " + tbl.Rows.Count.ToString(), Address = e.Message } } };
      }
      
    }

    private DriverContact[] GetDrivers(DataRow t)
    {
      return new DriverContact[] 
          {
            this.GetDriver(t, 1),
            this.GetDriver(t,2)
          };
    }

    private static Models.Business.Company GetCustomer(DataRow t)
    {
      return new Models.Business.Company
      {
        Name = t.Field<string>("Customer_Name"),
        Phone = t.Field<string>("Customer_Phone")
      };
    }

    private MovingFreightStop GetFrom(DataRow t)
    {
      return new MovingFreightStop
      {
        City = t.Field<string>("FromCity"),
        State = t.Field<string>("FromState"),
        DateTime = t.Field<DateTime>("Pickup")
      };
    }

    private MovingFreightStop GetTo(DataRow t)
    {
      return new MovingFreightStop
      {
        City = t.Field<string>("ToCity"),
        State = t.Field<string>("ToState"),
        DateTime = t.Field<DateTime>("Delivery")
      };
    }

    private DriverContact GetDriver(DataRow t, int p)
    {
      var firstNameField = string.Format("Driver{0}_FirstName", p);
      var lastNameField = string.Format("Driver{0}_lastName", p);
      var phoneField = string.Format("Driver{0}_Phone", p);
      var cardField = string.Format("Driver{0}_Card", p);
      
      var driver = new DriverContact
      {
        Name = t.Field<string>(firstNameField) + " " + t.Field<string>(lastNameField),
        CellPhone = t.Field<string>(phoneField)
      };
      if (p == 1)
      {
        driver.Card = t.Field<string>(cardField);
      }
      return driver;
    }


    private IEnumerable<DispatcherCommissionDate> GetCommissionDates()
    {
      var tbl = new DataTable();
      var ds = new DataSource();
      var qry = string.Concat(Queries.CommissionsCTE,
        Environment.NewLine,
        @"
        SELECT [CommissionPaymentDate], [DispatcherId]
        FROM [AllCommissions] 
        GROUP BY [CommissionPaymentDate], [DispatcherId]
      ");
      using (var cmd = new SqlCommand(qry))
      {
        tbl = ds.FetchDataTable(cmd);
      }

      var commissionDates = tbl.AsEnumerable()
        .Select(d => new DispatcherCommissionDate
        { 
          DispatcherId = d.Field<string>("DispatcherId"),
          CommissionPaymentDate = d.Field<DateTime>("CommissionPaymentDate")
        }).ToList();
      return commissionDates;
    }

    private IEnumerable<DispatcherCommission> GetCommissions(DataTable tbl)
    {
      var commissions = tbl.AsEnumerable().Select(c => new DispatcherCommission
      {
        ApInvoiceNumber = c.Field<string>("APInvoiceNumber"),
        DispatcherId = c.Field<string>("DispatcherId"),
        CommissionTotal = c.Field<decimal>("CommissionTotal"),
        TripNumber = c.Field<string>("TripNumber"),
        ArInvoiceNumber = c.Field<string>("ArInvoiceNumber"),
        BillDate = c.Field<DateTime>("BillDate"),
        CommissionPayableDate = c.Field<DateTime>("CommissionPaymentDate"),
        LessorId = c.Field<string>("LessorId"),
        CommissionDescription= c.Field<string>("CommisionDescription"),
        Lane = this.GetLane(c),
        CustomerName = c.Field<string>("CustomerName"),
        InvoiceAmount = c.Field<decimal>("InvoiceAmount")
      }).ToList();
      return commissions;
    }

    private string GetLane(DataRow c)
    {
      return string.Concat(c.Field<string>("ShipState").Trim(),
        " - ",
        c.Field<string>("ShipState").Trim());
    }


    private const string DriverContactsQuery = @"
        ;WITH [Drivers] AS
        (
          SELECT [D].[cDriverId] AS [ID]
           , RTRIM(LTRIM((COALESCE([E].[cLast], '')))) + ' , ' + COALESCE([E].[cFirst], '') AS [Name]
           , COALESCE( [E].[cCellularPager], '' ) AS [Cell_Phone]
           , COALESCE( [E].[cHomePhone], '' )     AS [Home_Phone]
           , COALESCE( [D].[cFuelCardNumber1], '' ) AS [Card]
           , COALESCE( [L].[cName], '' ) AS [Lessor_Name]
           , COALESCE( [L].[cPhone], '' ) AS [Lessor_Phone]
           , [D].[bAssigned] AS [Has_Trip]
           , COALESCE( [D].[cLicenseNumber], '' ) AS [Drivers_License]
          FROM [Truckwin_TDPD_Access]...[DriverInfo] [D]
            INNER JOIN [Truckwin_TDPD_Access]...[PrEmployee] [E]
              ON [D].[cDriverID] = [E].[cEmployeeId]
            INNER JOIN [Truckwin_TDPD_Access]...[RsLessor] [L]
              ON [D].[cLessorID] = [L].[cId]
          WHERE ( [E].[bActive] != 0 )
        )

        SELECT * 
        FROM [Drivers] 
        ORDER BY [Name]
    ";


  }


}

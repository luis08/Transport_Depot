﻿using System;
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
        TractorId = c.Field<string>("TractorId"),
        CommissionDescription= c.Field<string>("CommisionDescription"),
        Lane = this.GetLane(c),
        CustomerName = c.Field<string>("CustomerName")
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

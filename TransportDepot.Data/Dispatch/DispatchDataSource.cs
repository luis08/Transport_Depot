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

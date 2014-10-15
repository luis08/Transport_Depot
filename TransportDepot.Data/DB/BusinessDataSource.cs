using System;
using System.Collections.Generic;
using System.Linq;
using TransportDepot.Models.Business;
using System.Data.SqlClient;
using System.Data;

namespace TransportDepot.Data.DB
{
  public class BusinessDataSource
  {
    private DataSource _dataSource = new DataSource();
    public Company GetCompany()
    {
      var companyTable = new DataTable();
      using (var cmd = new SqlCommand(CompanyQuery))
      {
        companyTable = this._dataSource.FetchCommand(cmd);
      }
      if (companyTable.Rows.Count == 0)
      {
        throw new InvalidOperationException("Cannot read the master information table");
      }
      var companies = companyTable.AsEnumerable()
        .Select(c => new Company
        {
          TaxId = c.Field<string>("TaxID"),
          Name = c.Field<string>("Name"),
          Address = c.Field<string>("Address"),
          City = c.Field<string>("City"),
          State = c.Field<string>("State"),
          ZipCode = c.Field<string>("Zip"),
          Country = c.Field<string>("Country"),
          Phone = c.Field<string>("Phone"),
          Fax = c.Field<string>("Fax"),
          MCNumber = c.Field<string>("MC_Number")
        });
      return companies.First();
    }

    private const string CompanyQuery = @"
      SELECT
            [cFederalId] AS [TaxID]
         ,  [cCompanyName] AS [Name]
         ,  [cAddress] AS [Address]
         ,  [cCity] AS [City]
         ,  [cState] AS [State]
         ,  [cZip] AS [Zip]
         ,  [cCountry] AS [Country]
         ,  [cPhone] AS [Phone]
         ,  [cFax] AS [Fax]
         ,  [cFHWANo] AS [MC_Number]
      FROM [Truckwin_TDPD_Access]...[MasterInfo] 
    ";
  }
}

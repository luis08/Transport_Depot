
using TransportDepot.Models.DB;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Linq;
using System.Data;
using TransportDepot.Models.Business;

namespace TransportDepot.Data.DB
{
  public class CustomerDataSource
  {
    private DataSource _dataSource = new DataSource();
    private Utilities _utilities = new Utilities();

    public IEnumerable<Customer> GetCustomers(IEnumerable<string> ids)
    {
      ids = ids.Distinct();

      var customersDataTable = new DataTable();
      using (var cmd = new SqlCommand(CustomerQueries.ForIds))
      {
        var idsXml = this.GetCustomersXml(ids);
        cmd.Parameters.AddWithValue("@CustomerIdsXmlString", idsXml.ToString());
        customersDataTable = this._dataSource.FetchCommand(cmd);
      }

      var customers = this.GetCustomersFromTable(customersDataTable);
      return customers;
    }

    public IEnumerable<CustomerMenuItem> GetCustomerMenuItems()
    {
      var customersDataTable = new DataTable();
      using (var cmd = new SqlCommand(CustomerQueries.ForMenuItems))
      {
        customersDataTable = this._dataSource.FetchCommand(cmd);
      }

      var customers = customersDataTable.AsEnumerable()
        .Select(c => new CustomerMenuItem
        {
          Id = c.Field<string>("ID"),
          Name = c.Field<string>("Customer_Name")
        }).ToList();
      return customers;
    }

    public IEnumerable<Customer> GetCustomersPageByName(int pageNumber, int itemsPerPage)
    {
      var customersDataTable = new DataTable();
      var pager = this._dataSource.GetPager(pageNumber, itemsPerPage);

      using (var cmd = new SqlCommand(CustomerQueries.PagedByName))
      {
        cmd.Parameters.AddWithValue("@FromRow", pager.FromRow);
        cmd.Parameters.AddWithValue("@ToRow", pager.ToRow);
        customersDataTable = this._dataSource.FetchCommand(cmd);
      }

      var customers = this.GetCustomersFromTable(customersDataTable);
      return customers;
    }

    public IEnumerable<Customer> GetCustomersPageById(int pageNumber, int itemsPerPage)
    {
      var customersDataTable = new DataTable();
      var pager = this._dataSource.GetPager(pageNumber, itemsPerPage);

      using (var cmd = new SqlCommand(CustomerQueries.PagedById))
      {
        cmd.Parameters.AddWithValue("@FromRow", pager.FromRow);
        cmd.Parameters.AddWithValue("@ToRow", pager.ToRow);
        customersDataTable = this._dataSource.FetchCommand(cmd);
      }

      var customers = this.GetCustomersFromTable(customersDataTable);
      return customers;
    }

    private IEnumerable<Customer> GetCustomersFromTable(DataTable customersDataTable)
    {

      var customersFromTable = customersDataTable.AsEnumerable()
        .Select(c => GetCustomer(c as DataRow)).ToList();
      return customersFromTable;
    }

    private Customer GetCustomer(DataRow c)
    {
      Customer customer = new Customer();

      customer.ID = c.Field<string>("ID");
      customer.CustomerName = c.Field<string>("Customer_Name");
      customer.IsBillTo = c.Field<bool>("Is_BillTo");
      customer.IsOrigin = c.Field<bool>("Is_Origin");
      customer.IsDestination = c.Field<bool>("Is_Destination");
      customer.IsInbound = c.Field<bool>("Is_Inbound");
      customer.IsOutbound = c.Field<bool>("Is_Outbound");
      customer.IsContract = c.Field<bool>("Is_Contract");
      customer.StreetAddress = c.Field<string>("Street_Address");
      customer.City = c.Field<string>("City");
      customer.State = c.Field<string>("State");
      customer.Zip = c.Field<string>("Zip");
      customer.Country = c.Field<string>("Country");
      customer.Phone = c.Field<string>("Phone");
      customer.Fax = c.Field<string>("Fax");
      customer.ContactPersons = c.Field<string>("Contact_Persons");
      customer.OfficeHours = c.Field<string>("Office_Hours");
      customer.FirmNote = c.Field<string>("Firm_Note");
      customer.HasRateByDestinationState = c.Field<bool>("Has_Rate_By_Destination_State");
      customer.HasRateByDestinationCity = c.Field<bool>("Has_Rate_By_Destination_City");
      customer.HasRateByOriginState = c.Field<bool>("Has_Rate_By_Origin_State");
      customer.HasRateByOriginCity = c.Field<bool>("Has_Rate_By_Origin_City");
      customer.HasChargeGST = c.Field<bool>("Has_ChargeGST");
      customer.CreditLimitAmount = c.Field<decimal>("Credit_Limit");
      customer.UsesCanadianFunds = c.Field<bool>("Uses_Canadian_Funds");
      customer.UsesEDI = c.Field<bool>("Uses_EDI");
      customer.IsConsolidated = c.Field<bool>("Is_Consolidated");
      customer.DriverInstructions = c.Field<string>("Driver_Instructions");
      customer.Location = this._utilities.CoalesceString(c, "Location");
      customer.StopRate = c.IsNull("Stop_Rate") ? decimal.Zero : c.Field<decimal>("Stop_Rate");
      customer.RequiresPONumber = c.Field<bool>("Requires_PO_Number");
      customer.CreditLimitDayCount = c.Field<int>("Credit_Limit_Days");

      customer.CustomsBrokerID = c.Field<string>("Customs_Broker_ID");
      customer.IsInactive = c.Field<bool>("Is_Inactive");
      customer.HasWebAccess = c.Field<bool>("Has_Web_Access");
      customer.WebPassword = c.Field<string>("Web_Password");
      customer.Email = c.Field<string>("Email");
      customer.CustomField = c.Field<string>("Custom_Field");
      customer.RequiresBOLNumber = c.Field<bool>("Requires_BOL_Number");
      return customer;
    }

    private XDocument GetCustomersXml(IEnumerable<string> ids)
    {
      var doc = new XDocument(new XElement("customers",
        ids.Select(i => new XElement("id", i))));
      return doc;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Data.DB;
using TransportDepot.Models.Business;

namespace TransportDepot.Business
{
  public class BusinessService: IBusinessService
  {
    private BusinessDataSource _dataSource = new BusinessDataSource();
    public Models.Business.Company GetCompany()
    {
      var company = this._dataSource.GetCompany();
      return company;
    }


    public IEnumerable<CustomerMenuItem> GetCustomerMenuItems()
    {
      var ds = new CustomerDataSource();
      var customers = ds.GetCustomerMenuItems()
        .Select(c => new CustomerMenuItem
        {
          Id = c.Id,
          Name = string.Format("{0} ({1}}", 
            c.Name, c.Id )
        });
      return customers;
    }
  }
}

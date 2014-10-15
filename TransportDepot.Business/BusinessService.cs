using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Data.DB;

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
  }
}

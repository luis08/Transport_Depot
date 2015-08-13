using System.Linq;
using TransportDepot.Data.Dispatch;
using TransportDepot.Data.DB;
using TransportDepot.Models.Dispatch;
using TransportDepot.Models.Utilities;
using TransportDepot.Dispatch.CompanySetup;
using TransportDepot.Utilities.Email;
namespace TransportDepot.Dispatch
{
  public class DispatchService: IDispatchService
  {
    private DispatchDataSource _datasource = new DispatchDataSource();

    public System.Collections.Generic.IEnumerable<Models.Dispatch.DispatcherCommission> GetUnpaidCommissions()
    {
      var commissions = this._datasource.GetUnpaidCommissions();
      return commissions;
    }

    public System.Collections.Generic.IEnumerable<System.DateTime> GetCommissionDates(string dispatcherId)
    {
      var dates = this._datasource.GetAllCommissionDates(dispatcherId);
      return dates;
    }

    public System.Collections.Generic.IEnumerable<Models.Dispatch.DispatcherCommission> GetCommissions(Models.Dispatch.DispatcherCommissionDate request)
    {
      if (string.IsNullOrEmpty(request.DispatcherId) ||
          string.IsNullOrEmpty(request.DispatcherId.Trim()))
      {
        var commissions = this._datasource.GetDispatcherCommissions(request.CommissionPaymentDate);
        return commissions;
      }
      else
      {
        var commissions = this._datasource.GetDispatcherCommissions(request.CommissionPaymentDate)
          .Where(d => d.DispatcherId.Equals(request.DispatcherId, System.StringComparison.OrdinalIgnoreCase));
        return commissions;
      }
    }

    public System.Collections.Generic.IEnumerable<System.DateTime> GetAllCommissionDates()
    {
      var dates = this._datasource.GetAllCommissionDates();
      return dates;
    }

    public System.Collections.Generic.IEnumerable<Models.Dispatch.Dispatcher> GetDispatchers()
    {
      var dispatchers = this._datasource.GetDispatchers().OrderBy(d=>d.Name);
      return dispatchers;
    }

    public System.Collections.Generic.IEnumerable<Models.Dispatch.MovingFreightTrip> GetMovingFreight()
    {
      var movingFreight = this._datasource.GetMovingFreight();
      return movingFreight;
    }

    public void SendCompanySetup(Models.Dispatch.CompanySetupRequest request)
    {
      var ds = new CustomerDataSource();
      var bds = new BusinessDataSource();
      var customer = ds.GetCustomers(new string[] { request.ClientId }).FirstOrDefault();
      var company = bds.GetCompany();
      var emailBuilder = new CompanySetupEmailBuilder(request, company, customer);
      var emailService = new EmailService();
      var model = emailBuilder.GetEmailModel();
      emailService.Send(model);
      
    }

    public System.Collections.Generic.IEnumerable<DriverContact> GetDriverContacts()
    {
      return this._datasource.GetDriverContacts(false);
    }
  }
}

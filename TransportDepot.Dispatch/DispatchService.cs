using System.Linq;
using TransportDepot.Data.Dispatch;
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
      var commissions = this._datasource.GetDispatcherCommissions(request.CommissionPaymentDate)
        .Where(d=>d.DispatcherId.Equals(request.DispatcherId, System.StringComparison.OrdinalIgnoreCase));
      return commissions;
    }


    public System.Collections.Generic.IEnumerable<System.DateTime> GetAllCommissionDates()
    {
      var dates = this._datasource.GetAllCommissionDates();
      return dates;
    }
  }
}

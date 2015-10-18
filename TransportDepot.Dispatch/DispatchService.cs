using System.Linq;
using TransportDepot.Data.Dispatch;
using TransportDepot.Data.DB;
using TransportDepot.Models.Dispatch;
using TransportDepot.Models.Utilities;
using TransportDepot.Dispatch.CompanySetup;
using TransportDepot.Utilities.Email;
using System.Collections.Generic;
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

    public System.Collections.Generic.IEnumerable<QueuedTractor> GetQueuedTractors()
    {
      return new List<QueuedTractor>();
      var dbDataSource = new DBDataSource();
      var queuedTractors = this._datasource.GetQueuedTractors();
      var allTractors = dbDataSource.GetTractors();
      var dispatchers = this.GetDispatchers().ToDictionary(d => d.Initials, d => d);
      Dictionary<string, string> lastDispatcher = this._datasource.GetLastTractorDispatcher();
      var missingTractors = allTractors.Select(t=>t.Id).Except(queuedTractors.Select(t=>t.TractorId));
      DispatchQueue unassignedQ = this._datasource.GetUnassignedQueue();
      var tractorsMissingQueue = missingTractors.Select(t => new QueuedTractor
      {
        TractorId = t,
        DispatcherInitials = lastDispatcher.ContainsKey(t) ? 
          dispatchers[t].Initials 
          : dispatchers.First().Value.Initials,
        KeepWhenLoaded = false,
        QueueId = unassignedQ.Id,
        Trailer = string.Empty
      });

      var allQueuedTractors = queuedTractors.Union(tractorsMissingQueue);
      return allQueuedTractors;
    }


    public void Queue(int queueId, IEnumerable<string> tractorIds)
    {
      var qtDataSource = new QueuedTractorDataSource();
      qtDataSource.Queue(queueId, tractorIds);
    }

    public void DeQueue(IEnumerable<int> queuedTractorIds)
    {
      var qtDataSource = new QueuedTractorDataSource();
      qtDataSource.DeQueue(queuedTractorIds);
    }

    public void UpdateQueuedTractor(QueuedTractor queuedTractor)
    {
      var qtDataSource = new QueuedTractorDataSource();
      qtDataSource.Update(queuedTractor);
    }

    public void Book( string tractorId, string customerId)
    {
      throw new System.NotImplementedException();
    }

    public void DeleteBooking(IEnumerable<int> bookingIds)
    {
      throw new System.NotImplementedException();
    }


    public IEnumerable<DispatchQueue> GetQueues()
    {
      var queueDataSource = new TransportDepot.Data.Dispatch.QueuingDataSource();
      var queues = queueDataSource.GetQueues().ToList();
      return queues;
    }

    public void CreateQueue(DispatchQueue queue)
    {
      var queueDataSource = new TransportDepot.Data.Dispatch.QueuingDataSource();
      queueDataSource.Create(queue);
    }

    public void UpdateQueue(DispatchQueue queue)
    {
      var queueDataSource = new TransportDepot.Data.Dispatch.QueuingDataSource();
      queueDataSource.Update(queue);
    }

    public void DeleteQueue(int id)
    {
      var queueDataSource = new TransportDepot.Data.Dispatch.QueuingDataSource();
      queueDataSource.Delete(id);
    }
  }
}

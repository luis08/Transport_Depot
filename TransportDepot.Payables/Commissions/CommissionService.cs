using System.Collections.Generic;
using TransportDepot.Models.Payables.Commissions;
using System.ServiceModel;
using System.Collections;
using System.Linq;

namespace TransportDepot.Payables.Commissions
{
  public class CommissionService : ICommissionService
  {
    public IEnumerable<InvoiceCommission> GetCommissions(IEnumerable<CommissionRequest> requests)
    {
      if (requests == null)
      {
        throw new FaultException("Cannot process a null request");
      }

      var commissions = new List<InvoiceCommission>();

      requests.ToList().ForEach(r =>
      {
        commissions.AddRange(GetCommissions(r));
      });
      return commissions;
    }

    private IEnumerable<InvoiceCommission> GetCommissions(CommissionRequest r)
    {
      Dictionary<string, IEnumerable<Span>> spans = GroupAndSort(r);
      var truckCommissions = new List<InvoiceCommission>();
      foreach (var kvp in spans)
      {
        var calc = new CommissionCalculator
        {
          AgentId = r.AgentId,
          TractorId = kvp.Key,
          Spans = kvp.Value
        };

        truckCommissions.AddRange(calc.GetCommissions());
      }
      return truckCommissions;
    }


    private Dictionary<string, IEnumerable<Span>> GroupAndSort(CommissionRequest request)
    {
      var allSpans = request.OffDutySpans.Union(request.Trips).GroupBy(s=>s.TractorId).ToDictionary(s=>s.Key, s=>s.AsEnumerable<Span>());
      return allSpans;  
    }
  }
}

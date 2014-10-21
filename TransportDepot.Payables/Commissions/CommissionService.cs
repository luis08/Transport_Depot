using System.Collections.Generic;
using TransportDepot.Models.Payables.Commissions;
using System.ServiceModel;
using System.Linq;
using TransportDepot.Data.DB;

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


    public IEnumerable<CommissionCandidate> GetCandidates()
    {
      var dataSource = new AgentCommissionDataSource();
      var candicates = dataSource.GetCommissionCandidates();
      return candicates;
    }

    public IEnumerable<InvoiceCommission> GetAllCommissions()
    {
     
      var candidates = this.GetCandidates();
      var requests = candidates.GroupBy(c => c.AgentId  )
        .Select(r => new CommissionRequest
        {
          OffDutySpans = new List<Span>(),
          AgentId = r.Key,
          Trips = r.Select(t=> new TripSpan{
            TractorId = t.TractorId,
            StartDate = t.StartDate,
            EndDate = t.EndDate,
            StartLocation = t.StartLocation,
            EndLocation = t.EndLocatioin,
            InvoiceNumber = t.InvoiceNumber,
            InvoiceAmout = t.InvoiceAmount,
            TripNumber = t.TripNumber
          })
        });

      var commissions = this.GetCommissions(requests);
      return commissions;
    }


    public void SaveNewCommisions()
    {
      var commissions = this.GetAllCommissions();
      if (commissions == null)
      {
        return;
      }
      else if (commissions.Count() == 0)
      {
        return;
      }
      var dataSource = new AgentCommissionDataSource();
      dataSource.Save(commissions);
    }
  }
}

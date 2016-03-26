using System.Collections.Generic;
using TransportDepot.Models.Payables.Commissions;
using System.ServiceModel;
using System.Linq;
using TransportDepot.Data.DB;
using System;

namespace TransportDepot.Payables.Commissions
{
  public class CommissionService : ICommissionService
  {
    private AgentCommissionDataSource _dataSource = new AgentCommissionDataSource();

    private const string CycleStartDayKey = "TransportDepot.Payables.Commissions.NewCommission.CycleStartDay";

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

      var tripInvoiceMappings = this._dataSource.GetTripInvoiceMappings(
        commissions.Select(t => t.InvoiceNumber));

      this.IgnoreDupes(commissions, tripInvoiceMappings);
      this.SetDueDates(commissions, tripInvoiceMappings);

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

    private void SetDueDates(IEnumerable<InvoiceCommission> commissions, IEnumerable<TripInvoiceMapping> map)
    {
      var periodStart = GetPeriodStart();

      commissions.Join(map, c => c.InvoiceNumber, m => m.InvoiceNumber,
        (c, m) => new { Map = m, Commission = c }).ToList()
        .ForEach(mc =>
        {
          var billDate = mc.Map.BillDate;
          if (billDate.Day < periodStart)
          {
            mc.Commission.DueDate = new DateTime(billDate.Year, billDate.Month, periodStart);
          }
          else
          {
            var dueDate = new DateTime(billDate.Year, billDate.Month, periodStart).AddMonths(1);
            mc.Commission.DueDate = dueDate;
          }
        });
    }

    private static int GetPeriodStart()
    {
      var periodStartString = System.Configuration.ConfigurationManager.AppSettings[CycleStartDayKey];

      var periodStart = 1;
      if (!int.TryParse(periodStartString, out periodStart))
      {
        periodStart = 15;
      }
      else if (periodStart > 28)
      {
        periodStart = 28;
      }
      else if (periodStart < 1)
      {
        periodStart = 1;
      }
      return periodStart;
    }


    private void IgnoreDupes(IEnumerable<InvoiceCommission> commissions, IEnumerable<TripInvoiceMapping> mappings)
    {
      Action<InvoiceCommission> zeroOutComission = (c) =>
        {
          c.Percent = 0.0m;
        };
      /* Partials */
      commissions.GroupBy(t => t.InvoiceNumber)
        .Where(g => g.Count() > 1)
        .SelectMany(g => g)
        .ToList().ForEach(i => zeroOutComission(i));
      /* One invoice with more than one trip */
      commissions.GroupBy(t => t.TripNumber)
        .Where(g => g.Count() > 1)
        .SelectMany(g => g)
        .ToList().ForEach(i => zeroOutComission(i));
    }


    private Dictionary<string, IEnumerable<Span>> GroupAndSort(CommissionRequest request)
    {
      var allSpans = request.NonCommissionSpans.Union(request.Trips).GroupBy(s => s.LessorId).ToDictionary(s => s.Key, s => s.AsEnumerable<Span>());
      return allSpans;
    }


    public IEnumerable<CommissionCandidate> GetCandidates()
    {
      var candicates = this._dataSource.GetCommissionCandidates();
      return candicates;
    }

    public IEnumerable<InvoiceCommission> GetAllCommissions()
    {
      var candidates = this.GetCandidates();

      var previousSpans = GetPreviousSpans(candidates);


      var requests = GetRequests(new RequestBuilderBucket
      {
        Candidates = candidates,
        LessorHomes = this._dataSource.GetLessorHomes(candidates.Select(c => c.TripNumber)),
        PreviousTrips = previousSpans
      });

      var commissions = this.GetCommissions(requests);
      return commissions;
    }

    private IEnumerable<PreviousTrip> GetPreviousSpans(IEnumerable<CommissionCandidate> candidates)
    {
      var previousSpans = this._dataSource.GetPreviousTrips(candidates);
      return previousSpans;
    }

    public IEnumerable<CommissionRequest> GetRequests(RequestBuilderBucket bucket)
    {
      var tractorHomes = bucket.LessorHomes.ToDictionary(h => h.LessorId, h => h);
      var nonCommissionSpans = bucket.PreviousTrips
        .GroupBy(t => t.TripNumber, (k, g) => g.OrderBy(e => e.TripNumber).First())
        .ToDictionary(s => s.TripNumber, s => s);
      var requests = GetRequests(bucket, tractorHomes, nonCommissionSpans);
      return requests;
    }

    private static IEnumerable<CommissionRequest> GetRequests(RequestBuilderBucket bucket, Dictionary<string, LessorHome> tractorHomes, Dictionary<string, PreviousTrip> nonCommissionSpans)
    {
      var requests = bucket.Candidates.GroupBy(c => c.AgentId)
      .Select(r => new CommissionRequest
      {
        NonCommissionSpans = new List<Span>(),
        AgentId = r.Key,
        Trips = r.Select(t => new TripSpan
        {
          LessorId = t.LessorId,
          StartDate = t.StartDate,
          EndDate = t.EndDate,
          StartLocation = t.StartLocation,
          EndLocation = t.EndLocatioin,
          InvoiceNumber = t.InvoiceNumber,
          InvoiceAmout = t.InvoiceAmount,
          TripNumber = t.TripNumber,
          TractorHome = tractorHomes.ContainsKey(t.LessorId) ?
                          tractorHomes[t.LessorId].Location : null,
          PreviousSpan = nonCommissionSpans.ContainsKey(t.TripNumber) ?
                          nonCommissionSpans[t.TripNumber].PreviuosSpan : null,
          LessorRevenue = t.LessorRevenue

        }).ToArray()
      });
      return requests;
    }


    public void SaveNewCommisions()
    {
      var commissions = this.GetAllCommissions();
      this.SaveCommissions(commissions);
    }

    public void SaveCommissions(IEnumerable<InvoiceCommission> commissions)
    {

      if (commissions == null)
      {
        return;
      }
      else if (commissions.Count() == 0)
      {
        return;
      }
      try
      {
        this._dataSource.Save(commissions);
      }
      catch (Exception e)
      {
        TransportDepot.Data.Utilities.WriteAppend("Error in CommissionService \n-------------------------------\n" + 
          e.Message + "\n---------------------\n" + e.StackTrace);
      }
    }


    public IEnumerable<PreviousTrip> GetPreviousTrips(IEnumerable<CommissionCandidate> candidates)
    {
      return this._dataSource.GetPreviousTrips(candidates);
    }


    public IEnumerable<LessorHome> GetLessorHomes(IEnumerable<string> tripIds)
    {
      return this._dataSource.GetLessorHomes(tripIds);
    }
  }
}

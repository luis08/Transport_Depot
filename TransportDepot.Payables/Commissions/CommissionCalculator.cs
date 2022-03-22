
using TransportDepot.Models.Payables.Commissions;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

namespace TransportDepot.Payables.Commissions
{
  class CommissionCalculator
  {
    public string TractorId { get; set; }
    public string AgentId { get; set; }
    /// <summary>
    /// LINQ to objects preserves order
    /// http://msdn.microsoft.com/en-us/library/dd460677%28v=vs.110%29.aspx
    /// </summary>
    public IEnumerable<Span> Spans { get; set; }

    
    internal IEnumerable<InvoiceCommission> GetCommissions()
    {
      this.Spans = this.Spans.OrderBy(s => s.StartDate);

      var commissions = new List<InvoiceCommission>();
      var tripSpans = this.Spans.Select(s => GetTripSpan(s));
      var duped = tripSpans.GroupBy(s => s.TripNumber).Select(ts => new { 
        TripNumber = ts.Key,
        Count = ts.Count()
      }).Where(ts => ts.Count > 1).Select(ts => ts.TripNumber);
      foreach(var span in tripSpans)
      {
        var commission = GetCommission(span);
        if (span != null && ! duped.Contains(span.TripNumber))
        {
          commissions.Add(commission);
        }
      }
      return commissions;
    }

    private InvoiceCommission GetCommission(TripSpan tripSpan)
    {
      
      var commission = CreateInvoiceCommission(tripSpan);
      var timeSpan = tripSpan.StartDate - tripSpan.PreviousSpan.EndDate;
      
      if (tripSpan.PreviousSpan == null)
      {
        commission.Percent = 0.5m;
        return commission;
      }

      if (TripStartsAtTractorHome(tripSpan))
      {
        commission = GetHomeCommission(commission, timeSpan);
      }
      else if (timeSpan.TotalDays < 1)
      {
        commission.Percent = 1.0m;
      }
      else if (timeSpan.TotalDays < 2)
      {
        commission.Percent = 0.75m;
      }
      else
      {
        commission.Percent = 0.5m;
      }
      if (decimal.Subtract( tripSpan.InvoiceAmout, tripSpan.LessorRevenue) < decimal.Zero)
      {
        commission.Percent = 0.5m;
      }
      return commission;
    }

    private bool TripStartsAtTractorHome(TripSpan tripSpan)
    {
      if (tripSpan.TractorHome == null) return false;
      return tripSpan.StartLocation.State.Equals(tripSpan.TractorHome.State);
    }

    private InvoiceCommission CreateInvoiceCommission(TripSpan tripSpan)
    {
      var commission = new InvoiceCommission
      {
        AgentId = this.AgentId,
        LessorId = this.TractorId,
        InvoiceNumber = tripSpan.InvoiceNumber,
        InvoiceAmount = tripSpan.InvoiceAmout,
        TripNumber = tripSpan.TripNumber,
        Description = GetDescription(tripSpan)
      };
      return commission;
    }

    private string GetDescription(TripSpan tripSpan)
    {
      var locationDescription = string.Empty;
      if (tripSpan.StartLocation == null)
      {
        locationDescription = "Not Specified";
      }
      else if (string.IsNullOrEmpty(tripSpan.StartLocation.State))
      {
        locationDescription = "Blank";
      }
      else
      {
        locationDescription = tripSpan.StartLocation.State;
      }
      return string.Format("A/R Invoice: {0}  Tractor: {1}  Start Location: {2}",
                tripSpan.InvoiceNumber, this.TractorId,
                locationDescription);
    }

    private TripSpan GetTripSpan(Span span)
    {
      if (!(span is TripSpan))
      {
        throw new InvalidOperationException("it is not a trip span");
      }
      var tripSpan = span as TripSpan;
      if (tripSpan == null)
      {
        throw new InvalidOperationException("Trip Span is null");
      }
      return tripSpan;
    }

    private InvoiceCommission GetHomeCommission(InvoiceCommission commission, TimeSpan timeSpan)
    {
      if (timeSpan.TotalDays < 2)
      {
        commission.Percent = 1.0m;
      } 
      else 
      { 
        commission.Percent = 0.5m;
      }
      return commission;
    }

    private int GetDayDifference(Span span, TripSpan tripSpan)
    {
      var unloaded = new DateTime(span.EndDate.Year, span.EndDate.Month, span.EndDate.Day);
      var loaded = new DateTime(tripSpan.StartDate.Year, tripSpan.StartDate.Month, tripSpan.StartDate.Day);
      var diff = loaded - unloaded;
      return diff.Days;
    }
  }
}


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
      
      foreach(var span in this.Spans)
      {
        var commission = GetCommission(span);
        if (span != null)
        {
          commissions.Add(commission);
        }

      }
      return commissions;
    }

    private InvoiceCommission GetCommission(Span span)
    {
      var tripSpan = GetTripSpan(span);
      var commission = CreateInvoiceCommission(tripSpan);
      
      if (span.PreviousSpan == null)
      {
        commission = GetHomeCommission(span, tripSpan, commission);
        return commission;
      }

      var timeSpan = span.StartDate - span.PreviousSpan.EndDate;
      
        
      if (timeSpan.TotalDays < 1)
      {
        commission.Percent = 1.0m;
        return commission;
      }
      else if (timeSpan.TotalDays < 2)
      {
        commission.Percent = 0.75m;
        return commission;
      }
      else
      {
        commission.Percent = 0.5m;
      }
      
      return commission;
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

    private InvoiceCommission GetHomeCommission(Span span, TripSpan tripSpan, InvoiceCommission commission)
    {
      commission.Percent = 0.5m;
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

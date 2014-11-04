
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
      if(( tripSpan.TractorHome != null )
        && (tripSpan.TractorHome.State.Equals(span.StartLocation.State, System.StringComparison.OrdinalIgnoreCase)))
      {
        commission.Percent = 0.5m;
        return commission;
      }
      var timeSpan = span.PreviousSpan.EndDate - span.StartDate;

      if (timeSpan.TotalDays <= 1)
      {
        commission.Percent = 1.0m;
        return commission;
      }
      commission.Percent = decimal.Zero;
      return commission;
    }

    private InvoiceCommission CreateInvoiceCommission(TripSpan tripSpan)
    {
      var commission = new InvoiceCommission
      {
        AgentId = this.AgentId,
        TractorId = this.TractorId,
        InvoiceNumber = tripSpan.InvoiceNumber,
        InvoiceAmount = tripSpan.InvoiceAmout,
        TripNumber = tripSpan.TripNumber,
        Description = string.Format("A/R Invoice: {0}  Tractor: {1} ",
          tripSpan.InvoiceNumber, this.TractorId)

      };
      return commission;
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
      if (tripSpan.TractorHome == null)
      {
        commission.Percent = 1.0m;
      }
      else if (string.IsNullOrEmpty(tripSpan.TractorHome.State))
      {
        commission.Percent = 1.0m;
      }
      else if (tripSpan.TractorHome.State.Equals(span.StartLocation.State, System.StringComparison.OrdinalIgnoreCase))
      {
        commission.Percent = 0.5m;
      }
      else
      {
        commission.Percent = 1.0m;
      }
      return commission;
    }
  }
}

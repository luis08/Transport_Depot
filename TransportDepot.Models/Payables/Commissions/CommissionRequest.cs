
using System.Collections.Generic;
namespace TransportDepot.Models.Payables.Commissions
{
  public class CommissionRequest
  {
    public string AgentId { get; set; }
    public string VendorType { get; set; }
    public IEnumerable<Span> NonCommissionSpans { get; set; }
    public IEnumerable<TripSpan> Trips { get; set; }
  }
}

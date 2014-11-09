
namespace TransportDepot.Models.Payables.Commissions
{
  public class TripSpan:Span
  {
    public string InvoiceNumber { get; set; }
    public decimal InvoiceAmout { get; set; }
    public string TripNumber { get; set; }
    public decimal LessorRevenue { get; set; }
    public Location TractorHome { get; set; }
  }
}


namespace TransportDepot.Models.Payables.Commissions
{
  public class InvoiceCommission
  {
    public string AgentId { get; set; }
    public string TractorId { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal Percent { get; set; }
    public decimal InvoiceAmount { get; set; }
  }
}

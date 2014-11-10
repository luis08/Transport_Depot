

using System;
namespace TransportDepot.Models.Factoring
{
  public class FactoringPayment
  {
    public string Type { get; set; }
    public string InvoiceNumber { get; set; }
    public int Schedule { get; set; }
    public string CheckNumber { get; set; }
    public string Debtor { get; set; }
    public DateTime EffectiveDate { get; set; }
    public decimal Amount { get; set; }
  }
}

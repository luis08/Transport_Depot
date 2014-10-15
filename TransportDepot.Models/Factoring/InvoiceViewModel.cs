using System;

namespace TransportDepot.Models.Factoring
{
  public class InvoiceViewModel
  {
    public string Number { get; set; }
    public DateTime Date { get; set; }
    public string CustomerName { get; set; }
    public decimal Amount { get; set; }
  }
}

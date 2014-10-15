
using System;
namespace TransportDepot.Models.AR
{
  public class AgingInvoice
  {
    public string Number { get; set; }
    public string CustomerId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string CustomerReference { get; set; }
    public decimal Amount { get; set; }
    public int Aging { get; set; }
    public bool IsOverdue { get; set; }
  }
}

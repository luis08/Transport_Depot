using System;

namespace TransportDepot.Models.Payables.Commissions
{
  public class TripInvoiceMapping
  {
    public string TripNumber { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime BillDate { get; set; }
  }
}

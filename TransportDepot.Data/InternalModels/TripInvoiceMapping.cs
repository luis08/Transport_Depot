using System;

namespace TransportDepot.Data.InternalModels
{
  class TripInvoiceMapping
  {
    public string TripNumber { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime BillDate { get; set; }
  }
}

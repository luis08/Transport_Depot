using System;

namespace TransportDepot.Models.AR
{
  public class CollectionsReportInvoice
  {
    public string InvoiceNumber { get; set; }
    public DateTime Date { get; set; }
    public int Aging { get; set; }
    public string Reference { get; set; }
    public decimal Current { get; set; }
    public decimal Late { get; set; }
    public decimal OverThirty { get; set; }
    public decimal OverSixty { get; set; }
  }
}

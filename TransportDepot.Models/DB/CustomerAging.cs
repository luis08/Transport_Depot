using System;
namespace TransportDepot.Models.DB
{
  public class CustomerAging
  {
    public string CustomerID { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string TripNumber { get; set; }
    public string CustomerReference { get; set; }
    public decimal BalanceDue { get; set; }
  }
}

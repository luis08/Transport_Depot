using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Models.AR
{
  public class CustomerAgingSummary
  {
    public string CustomerId { get; set; }
    public string CustomerName { get; set; }
    public decimal Balance { get; set; }
    public decimal OverdueBalance { get; set; }
    public int MaxAging { get; set; }
    public int CustomerCreditDayCount { get; set; }
    public int TotalInvoiceCount { get; set; }
    public int OverdueInvoiceCount { get; set; }
  }
}

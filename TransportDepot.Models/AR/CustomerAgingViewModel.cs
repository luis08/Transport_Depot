using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Models.AR
{
  public class CustomerAgingViewModel
  {
    public string CustomerId { get; set; }
    public string CustomerName { get; set; }
    public decimal Balance { get; set; }
    public int MaxAging { get; set; }
    public int InvoiceCount { get; set; }
  }
}

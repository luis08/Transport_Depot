using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Models.Dispatch
{
  public class DispatcherCommission
  {
    public string ApInvoiceNumber { get; set; }
    public string DispatcherId { get; set; }
    public decimal CommissionTotal { get; set; }
    public string TripNumber { get; set; }
    public string ArInvoiceNumber { get; set; }
    public decimal InvoiceAmount { get; set; }
    public DateTime BillDate { get; set; }
    public DateTime CommissionPayableDate { get; set; }
    public string TractorId { get; set; }
    public string CommissionDescription { get; set; }
    public string Lane { get; set; }
    public string CustomerName { get; set; }
  }
}

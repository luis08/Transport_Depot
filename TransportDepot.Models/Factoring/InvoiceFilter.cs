using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Models.Factoring
{
  public class InvoiceFilter
  {
    public string FromInvoiceNumber { get; set; }
    public string ToInvoiceNumber { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public bool OnlyWithoutSchedule { get; set; }
    public int RowsPerPage { get; set; }
    public int PageNumber { get; set; }
  }
  
}

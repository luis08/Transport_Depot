
using System.Collections.Generic;
namespace TransportDepot.Models.Factoring
{
  public class InvoiceSet
  {
    public IEnumerable<InvoiceViewModel> Invoices { get; set; }
    public int PageNumber { get; set; }
    public int RowsPerPage { get; set; }
    public int TotalPages { get; set; }
  }
}

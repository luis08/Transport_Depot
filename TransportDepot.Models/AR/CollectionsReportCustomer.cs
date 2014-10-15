
using TransportDepot.Models.DB;
using System.Collections.Generic;
namespace TransportDepot.Models.AR
{
  public class CollectionsReportCustomer
  {
    public Customer Customer { get; set; }
    public IEnumerable<CollectionsReportInvoice> Invoices { get; set; }
  }
}

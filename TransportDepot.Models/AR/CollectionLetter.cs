
using System.Collections.Generic;
using TransportDepot.Models.DB;
namespace TransportDepot.Models.AR
{
  public class CollectionLetter
  {
    public Customer Customer { get; set; }
    public IEnumerable<AgingInvoice> Invoices { get; set; }
  }
}

using System;
using System.Collections.Generic;

namespace TransportDepot.Models.Factoring
{
  public class ScheduleViewModel
  {
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public Guid User { get; set; }
    public IEnumerable<InvoiceViewModel> Invoices { get; set; }
  }
}

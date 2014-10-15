using System;
using System.Collections.Generic;

namespace TransportDepot.Models.Factoring
{
  public class NewScheduleViewModel
  {
    public DateTime Date { get; set; }
    public Guid User { get; set; }
    public IEnumerable<string> InvoiceNumbers { get; set; }
  }
}

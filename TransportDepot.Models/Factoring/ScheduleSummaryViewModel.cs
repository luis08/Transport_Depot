using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Models.Factoring
{
  public class ScheduleSummaryViewModel
  {
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
  }
}

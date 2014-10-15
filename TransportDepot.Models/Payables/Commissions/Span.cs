using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Models.Payables.Commissions
{
  public class Span
  {
    public string TractorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Location StartLocation { get; set; }
    public Location EndLocation { get; set; }
    public Span PreviousSpan { get; set; }
  }
}

using System;
using System.Collections.Generic;

namespace TransportDepot.Models.Safety
{
  public class SafetyCard
  {
    public string Title { get; set; }
    public string Type { get; set; }
    public IEnumerable<SafetyCardDetail> Details { get; set; }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Models.Dispatch
{
  public class DispatchQueue
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Comments { get; set; }
    public int GeoLocationId { get; set; }
  }
}


using System;
using System.Collections.Generic;
namespace TransportDepot.Models.Reports
{
  public class MaintenanceFilter
  {
    public IEnumerable<string> VehicleIds { get; set; }
    public string Type { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public string Description { get; set; }
  }
}

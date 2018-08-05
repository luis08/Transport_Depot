
using System.Collections.Generic;
using System;

namespace TransportDepot.Models.Safety
{
  public class VehicleMaintenanceRequest
  {
    public string VehicleId { get; set; }
    public IEnumerable<DateTime> Dates { get; set; }
  }
}

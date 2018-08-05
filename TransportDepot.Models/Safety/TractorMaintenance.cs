using System;

namespace TransportDepot.Models.Safety
{
  public class TractorMaintenance
  {
    public Tractor Tractor { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; }
    public string TypeId { get; set; }
    public int Mileage { get; set; }
    public string Description { get; set; }
  }
}

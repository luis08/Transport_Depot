using System;

namespace TransportDepot.Models.Safety
{
  public class TrailerMaintenance
  {
    public Trailer Trailer { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; }
    public string TypeId { get; set; }
    public string Description { get; set; }
  }
}

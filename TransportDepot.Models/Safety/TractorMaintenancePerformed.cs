using System;

namespace TransportDepot.Models.Safety
{
  public class TractorMaintenancePerformed
  {
    public string TractorId { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
  }
}

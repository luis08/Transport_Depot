
using System.Collections.Generic;
namespace TransportDepot.Models.Dispatch
{
  public class QueuedTractor
  {
    public int Id { get; set; }
    public string TractorId { get; set; }
    public string Trailer { get; set; }
    public int QueueId { get; set; }
    public string DispatcherInitials { get; set; }
    public bool KeepWhenLoaded { get; set; }
  }
}

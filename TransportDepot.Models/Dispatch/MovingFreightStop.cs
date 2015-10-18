using System;
namespace TransportDepot.Models.Dispatch
{
  public class MovingFreightStop
  {
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public GeoLocation GoogleMapLocation { get; set; }
    public DateTime DateTime { get; set; }
  }
}

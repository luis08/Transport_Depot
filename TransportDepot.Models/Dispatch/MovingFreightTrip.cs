using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Models.Business;

namespace TransportDepot.Models.Dispatch
{
  public class MovingFreightTrip
  {
    public string TripNumber { get; set; }
    public string Tractor { get; set; }
    public string Trailer { get; set; }
    public IEnumerable<DriverContact> Drivers { get; set; }
    public MovingFreightStop From { get; set; }
    public MovingFreightStop To { get; set; }
    public Company Customer { get; set; }
    public string CustomerId { get; set; }
  }
}

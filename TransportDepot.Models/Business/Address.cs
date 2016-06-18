using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Models.Business
{
  public class Address
  {
    public String StreetAddress { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string ZipCode { get; set; }

    public string Phone { get; set; }
  }
}

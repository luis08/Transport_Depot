using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Models.Business
{
  public class Company
  {
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string Phone { get; set; }
    public string Fax { get; set; }

    public string TaxId { get; set; }

    public string MCNumber { get; set; }

    public string Country { get; set; }
  }
}

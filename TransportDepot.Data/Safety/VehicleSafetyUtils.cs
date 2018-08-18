using System.Xml.Linq;
using System.Linq;
using TransportDepot.Models.Reports;
using System.Collections.Generic;

namespace TransportDepot.Data.Safety
{
  class VehicleSafetyUtils
  {
    private Utilities _utilities = new Utilities();


    internal XDocument GetTractorFilterXml(MaintenanceFilter filter)
    {
      var element = GetVehicleFilterElement(filter);
      if (filter.VehicleIds != null)
      {
        element.Add(filter.VehicleIds.Select(tractorId => new XElement("tractorId", tractorId)));
      }
      return new XDocument(element);
    }

    internal XDocument GetTrailerFilterXml(MaintenanceFilter filter)
    {
      var element = GetVehicleFilterElement(filter);
      if (filter.VehicleIds != null)
      {
        element.Add(filter.VehicleIds.Select(trailerId => new XElement("trailerId", trailerId)));
      }
      return new XDocument(element);
    }

        private XElement GetVehicleFilterElement(MaintenanceFilter filter)
    {
      XElement element = new XElement("filter");
      this._utilities.AddAttributeIfExists(element, "type", filter.Type);

      this._utilities.AddAttributeIfExists(element, "description", filter.Description);

      if (filter.From != null)
      {
        this._utilities.AddAttributeIfExists(element, "from", filter.From.ToShortDateString());
      }

      if (filter.To != null)
      {
        this._utilities.AddAttributeIfExists(element, "to", filter.To.ToShortDateString());
      }

      return element;
    }
  }
}

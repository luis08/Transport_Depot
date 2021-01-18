using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Safety
{
  class SafetyCardService : ISafetyCardService
  {
    private ISafetyService _safetyService = new SafetyService();
    private SafetyCardPicker _picker = new SafetyCardPicker();

    public IEnumerable<Models.Safety.SafetyCard> GetCards()
    {
      var tractors = this._safetyService.GetTractors(true);
      var drivers = this._safetyService.GetAllDrivers(true);
      var trailers = this._safetyService.GetTrailers(true);

      var tractorCards = this._picker.Make(tractors);
      var driverCards = this._picker.Make(drivers);
      var trailercards = this._picker.Make(trailers);
      throw new NotImplementedException();
    }
  }
}

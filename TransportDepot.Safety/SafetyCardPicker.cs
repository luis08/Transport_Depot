using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Models.Safety;

namespace TransportDepot.Safety
{
  internal class SafetyCardPicker
  {
    public IEnumerable<SafetyCard> Make(IEnumerable<Tractor> tractors)
    {
      var tractor = tractors.First();
      Func<Tractor, bool> tractorFunction = TractorDetails;
      var details = GetDetails(tractorFunction);
      throw new NotImplementedException();
    }

    internal IEnumerable<SafetyCard> Make(IEnumerable<Driver> drivers)
    {
      throw new NotImplementedException();
    }

    internal IEnumerable<SafetyCard> Make(IEnumerable<Trailer> trailers)
    {
      throw new NotImplementedException();
    }

    IEnumerable<SafetyCardDetail> GetDetails<T>(Func<T, bool> fn)
    {
      return null;
    }

    private bool TractorDetails(Tractor tractor)
    {
      return false;
    }
  }
}

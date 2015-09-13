
using TransportDepot.Models.Dispatch;
using System.Collections.Generic;
using System.Linq;

namespace TransportDepot.Data.Dispatch
{
  class Dispatchers
  {
    public Dispatchers( IEnumerable<Dispatcher> dispatchers )
    {
      this._dispatchers = dispatchers.ToDictionary(d=>d.Initials, d => d);
    }
    private Dictionary<string, Dispatcher> _dispatchers;

    public Dispatcher GetDispatcher(string initials)
    {
      if (string.IsNullOrEmpty(initials))
      {
        initials = "[Unassigned]";
        return new Dispatcher
        {
          Initials = initials,
          Name = initials,
          VendorId = initials
        };
      }

      if (this._dispatchers.ContainsKey(initials))
        return this._dispatchers[initials];

      return new Dispatcher
      {
        Initials = "N/A",
        VendorId = "N/A",
        Name = string.Format("Invalid: '{0}'", initials)
      };
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Models.Payables.Commissions;

namespace DriverAp
{
  class Program
  {
    static void Main(string[] args)
    {
      var s = new TransportDepot.Payables.Commissions.CommissionService();

      var cands = s.GetCandidates();
      var str = cands.Select(c => c.TripNumber).ToList().Aggregate((i, j) => string.Concat(i, ", ", j));
      s.SaveNewCommisions();
    }
  }
}

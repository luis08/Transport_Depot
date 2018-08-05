using System;
using System.Collections.Generic;
using System.Linq;
using TransportDepot.Models.Safety;
using TransportDepot.Data.Safety;

namespace TransportDepot.Reports.Safety.Maintenance
{
  class TractorMaintenancePendingService
  {
    private IEnumerable<TractorMaintenance> _performed;
    private DateTime _from;
    private DateTime _to;
    private ArgumentUtils _argumentUtils = new ArgumentUtils();

    public TractorMaintenancePendingService(IEnumerable<TractorMaintenance> performed, DateTime from , DateTime to)
    {
      this._argumentUtils.CheckNotDefault(from);
      this._argumentUtils.CheckNotDefault(to);
      this._argumentUtils.CheckDateRangeNotEqual(from, to);
      this._performed = performed;
      this._from = from;
      this._to = to;
    }

    public IEnumerable<DateTime> GetMonthsPending()
    {
      var monthsPending = new List<DateTime>();
      var monthsWithMaintenance = GetPerformedDictionary();
      var startDate = new DateTime(this._from.Year, this._from.Month, 1);
      var enddate = new DateTime(this._to.Year, this._to.Month, 1);

      for (var current = startDate; current < enddate; current = current.AddMonths(1))
      {
        if (!monthsWithMaintenance.ContainsKey(current))
        {
          monthsPending.Add(current);
        }
      }

      return monthsPending;
    }


    private Dictionary<DateTime, List<TractorMaintenance>> GetPerformedDictionary()
    {
      var performedMonths = new Dictionary<DateTime, List<TractorMaintenance>>();
      return this._performed.OrderBy(p => p.Date).Select(m => new TractorMaintenance
        {
          Date = new DateTime(m.Date.Year, m.Date.Month, 1),
          Tractor = m.Tractor,
          Type = m.Type,
          Description = m.Description,
          TypeId = m.TypeId
        }).GroupBy(m => m.Date).ToDictionary(g => g.Key, g => g.ToList());
    }
  }
}

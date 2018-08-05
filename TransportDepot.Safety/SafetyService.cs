using System;
using System.Collections.Generic;
using TransportDepot.Data.Safety;
using System.Linq;
using TransportDepot.Models.Safety;
using TransportDepot.Models.Business;
using TransportDepot.Models.Reports;
using TransportDepot.Data;

namespace TransportDepot.Safety
{
  public class SafetyService:ISafetyService
  {
    private SafetyDataSource _dataSource = new SafetyDataSource();
    private ArgumentUtils _argumentUtils = new ArgumentUtils();
    private Utilities _utilities = new Utilities();

    public IEnumerable<Models.Safety.Driver> GetDrivers(string[] ids)
    {
      return this._dataSource.GetDrivers(ids);
    }

    public IEnumerable<Models.Safety.Driver> GetAllDrivers(bool activeOnly)
    {
      return this._dataSource.GetDrivers(false);
    }

    public void UpdateDrivers(IEnumerable<Models.Safety.Driver> drivers)
    {
      this._dataSource.UpdateSafetyDrivers(drivers);
    }

    public void TrackDrivers(string[] ids)
    {
      this._dataSource.TrackDrivers(ids);
    }

    public void UnTrackDrivers(string[] ids)
    {
      this._dataSource.UntrackDrivers(ids);
    }

    public IEnumerable<Models.Safety.Tractor> GetTractors(bool activeOnly)
    {
      var safetyTractors = this._dataSource.GetTractors(activeOnly);
      return safetyTractors;
    }

    public IEnumerable<Models.Safety.Tractor> GetTractors(IEnumerable<string> ids)
    {
      var safetyTractors = this._dataSource.GetTractors(false);
      return safetyTractors.Join(ids, 
        t=>t.Id, id=>id, (t,id)=>t);
    }

    public void UdpateTractor(Models.Safety.Tractor tractor)
    {
      this._dataSource.UpdateTractor(tractor);
    }

    public IEnumerable<Models.UI.OptionModel> GetUntrackedDriversOptions()
    {
      return this._dataSource.GetUntrackedDriverOptions();
    }

    public IEnumerable<Models.Safety.Trailer> GetTrailers(bool activeOnly)
    {
      return this._dataSource.GetTrailers(activeOnly);
    }

    public IEnumerable<Models.Safety.Trailer> GetTrailers(IEnumerable<string> ids)
    {
      var safetyTrailers = this._dataSource.GetTrailers(false);
      return safetyTrailers.Join(ids,
        t => t.Id, id => id, (t, id) => t);
    }

    public void UdpateTrailer(Models.Safety.Trailer trailer)
    {
      this._dataSource.UpdateTrailer(trailer);
    }

    public void Append(TractorMaintenancePerformed maintenance)
    {
      this._dataSource.Append(maintenance);
    }

    public void AppendTrailer(TrailerMaintenancePerformed maintenance)
    {
      this._dataSource.Append(maintenance);
    }


    public List<SimpleItem> GetMaintenanceTypes()
    {
      return this._dataSource.GetMaintenanceTypes();
    }

    public IEnumerable<TractorMaintenance> GetTractorMaintenance(MaintenanceFilter filter)
    {
      CheckFilter(filter);
      return this._dataSource.GetTractorMaintenance(filter);
    }

    public IEnumerable<TrailerMaintenance> GetTrailerMaintenance(MaintenanceFilter filter)
    {
      CheckFilter(filter);
      return this._dataSource.GetTrailerMaintenance(filter);
    }

    private void CheckFilter(MaintenanceFilter filter)
    {
      this._argumentUtils.CheckNotNull(filter);
      this._argumentUtils.AssertTrue(!(this._argumentUtils.IsDefault(filter.From)
        && this._argumentUtils.IsDefault(filter.From)), "Cannot take arguments 'To' and 'From' be the default value.");
    }
  }
}

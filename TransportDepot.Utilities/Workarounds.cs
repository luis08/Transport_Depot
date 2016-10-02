
using TransportDepot.Data.Misc;
using System;
namespace TransportDepot.Utilities
{
  public class Workarounds : TransportDepot.Utilities.IWorkarounds
  {
    private DataSource _dataSource = new DataSource();
    
    private const string BlankSpace = " ";
    public void PrepareToOpen()
    {
      if (IsReadyToWork())
      {
        _dataSource.CopyEmployeeTable();
        _dataSource.CopyTractorTable();
        _dataSource.CopyBillingHistoryTable();
        _dataSource.CopyArEntryTable();
        _dataSource.CopyRsPayableTable();
        _dataSource.CopyRsPayableHistoryTable();
        _dataSource.CopyGlEntryTable();
        _dataSource.CopyApPayableHistoryTable();
        _dataSource.CopyTripNumberTable();
        _dataSource.DeleteEmployees();
        _dataSource.DeleteTractors();
        _dataSource.DeleteBillingHistory();
        _dataSource.DeleteArEntry();
        _dataSource.DeleteRsPayable();
        _dataSource.DeleteRsPayableHistory();
        _dataSource.DeleteGlEntry();
        _dataSource.DeleteApPayableHistory();
        _dataSource.DeleteTripNumber();
      }
    }

    public void PrepareToWork()
    {
      if (IsReadyToOpen())
      {
        _dataSource.RestoreEmployeesFromTempFile();
        _dataSource.RestoreTractorsFromTempFile();
        _dataSource.RestoreBillingHistoryFromTempFile();
        _dataSource.RestoreArEntryFromTempFile();
        _dataSource.RestoreRsPayableFromTempFile();
        _dataSource.RestoreRsPayableHistoryFromTempFile();
        _dataSource.RestoreGlEntryFromTempFile();
        _dataSource.RestoreApPayableHistoryFromTempFile();
        _dataSource.RestoreTripNumberFromTempFile();
      }
    }

    public bool IsReadyToOpen()
    {
      var employeeThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.Employees.ReadyToOpenCountThreshold");
      var employeeCount = _dataSource.GetEmployeeCount();
      var truckThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.Trucks.ReadyToOpenCountThreshold");
      var truckCount = _dataSource.GetTractorCount();
      var billingHistoryThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.BillingHistory.ReadyToOpenCountThreshold");
      var billingHistoryCount = _dataSource.GetArEntryCount();
      var arEntryThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.ArEntry.ReadyToOpenCountThreshold");
      var arEntryCount = _dataSource.GetBillingHistoryCount();
      var rsPayableThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.RsPayable.ReadyToOpenCountThreshold");
      var rsPayableCount = _dataSource.GetRsPayableCount();
      var rsPayableHistoryThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.RsPayableHistory.ReadyToOpenCountThreshold");
      var rsPayableHistoryCount = _dataSource.GetRsPayableHistoryCount();
      var glEntryThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.GlEntry.ReadyToOpenCountThreshold");
      var glEntryCount = _dataSource.GetGlEntryCount();
      var apPayableHistoryThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.ApPayableHistory.ReadyToOpenCountThreshold");
      var apPayableHistoryCount = _dataSource.GetApPayableHistoryCount();
      var tripNumberThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.TripNumber.ReadyToOpenCountThreshold");
      var tripNumberCount = _dataSource.GetTripNumberCount();
      var returnVal = (truckCount <= truckThreshold)
        && (employeeCount <= employeeThreshold)
        && (billingHistoryCount <= billingHistoryThreshold)
        && (arEntryCount <= arEntryThreshold)
        && (rsPayableCount <= rsPayableThreshold)
        && (rsPayableHistoryCount <= rsPayableHistoryThreshold)
        && (glEntryCount <= glEntryThreshold)
        && (apPayableHistoryCount <= apPayableHistoryThreshold)
        && (tripNumberCount <= tripNumberThreshold);
      return returnVal;
    }

    public bool IsReadyToWork()
    {
      var employeeThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.Employees.ReadyToWorkCountThreshold");
      var employeeCount = _dataSource.GetEmployeeCount();
      var truckThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.Employees.ReadyToWorkCountThreshold");
      var truckCount = _dataSource.GetTractorCount();
      var billingHistoryThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.BillingHistory.ReadyToWorkCountThreshold");
      var billingHistoryCount = _dataSource.GetBillingHistoryCount();
      var arEntryThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.ArEntry.ReadyToWorkCountThreshold");
      var arEntryCount = _dataSource.GetBillingHistoryCount();
      var rsPayableThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.RsPayable.ReadyToOpenCountThreshold");
      var rsPayableCount = _dataSource.GetRsPayableCount();
      var rsPayableHistoryThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.RsPayableHistory.ReadyToOpenCountThreshold");
      var rsPayableHistoryCount = _dataSource.GetRsPayableHistoryCount();
      var glEntryThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.GlEntry.ReadyToOpenCountThreshold");
      var glEntryCount = _dataSource.GetRsPayableHistoryCount();
      var apPayableHistoryThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.ApPayableHistory.ReadyToOpenCountThreshold");
      var apPayableHistoryCount = _dataSource.GetApPayableHistoryCount();
      var tripNumberThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.TripNumber.ReadyToOpenCountThreshold");
      var tripNumberCount = _dataSource.GetTripNumberCount();
      var returnVal = (truckCount > truckThreshold) 
        && (employeeCount > employeeThreshold) 
        && (billingHistoryCount > billingHistoryThreshold) 
        && (arEntryCount > arEntryThreshold)
        && (rsPayableCount > rsPayableThreshold) 
        && (rsPayableHistoryCount > rsPayableHistoryThreshold)
        && (glEntryCount > glEntryThreshold)
        && (apPayableHistoryCount > apPayableHistoryThreshold)
        && (tripNumberCount > tripNumberThreshold);
      return returnVal;
    }

    public void AppendDriver(string id, string firstName)
    {
      if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(firstName))
      {
        throw new ArgumentException("blank parameters received");
      }
      if (id.Length > 15)
      {
        throw new ArgumentException("Id cannot be more than 15 characters");
      }
      ValidateIdNoSpaces(id);
      _dataSource.AppendDriver(id, firstName);
    }

    public void AppendTractor(string id)
    {
      if (string.IsNullOrEmpty(id))
      {
        throw new ArgumentException("blank parameters received");
      }
      if (id.Length > 8)
      {
        throw new ArgumentException("Id cannot be more than 15 characters");
      }
      ValidateIdNoSpaces(id);
      _dataSource.AppendTractor(id);
    }
    private void ValidateIdNoSpaces(string id)
    {
      if(id.Contains(BlankSpace))
      {
        throw new ArgumentException("Ids canot contain spaces");
      }
    }
  }
}

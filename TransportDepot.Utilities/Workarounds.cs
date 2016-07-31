
using TransportDepot.Data.Misc;
using System;
namespace TransportDepot.Utilities
{
  public class Workarounds : TransportDepot.Utilities.IWorkarounds
  {
    private DataSource dataSource = new DataSource();
    public void PrepareToOpen()
    {
      if (IsReadyToWork())
      {
        dataSource.CopyEmployeeTable();
        dataSource.CopyTractorTable();
        dataSource.DeleteEmployees();
        dataSource.DeleteTractors();
      }
    }

    public void PrepareToWork()
    {
      if (IsReadyToOpen())
      {
        dataSource.RestoreEmployeesFromTempFile();
        dataSource.RestoreTractosFromTempFile();
      }
    }

    public bool IsReadyToOpen()
    {
      var employeeThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.Employees.ReadyToOpenCountThreshold");
      var employeeCount = dataSource.GetEmployeeCount();
      var truckThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.Trucks.ReadyToOpenCountThreshold");
      var truckCount = dataSource.GetTractorCount();
      return (truckCount <= truckThreshold) && (employeeCount <= employeeThreshold);
    }

    public bool IsReadyToWork()
    {
      var employeeThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.Employees.ReadyToWorkCountThreshold");
      var employeeCount = dataSource.GetEmployeeCount();
      var truckThreshold = Utilities.GetIntSetting("TransportDepot.Utilities.Employees.ReadyToWorkCountThreshold");
      var truckCount = dataSource.GetTractorCount();
      return (truckCount > truckThreshold) && (employeeCount > employeeThreshold);
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
      dataSource.AppendDriver(id, firstName);
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
      dataSource.AppendTractor(id);
    }
  }
}

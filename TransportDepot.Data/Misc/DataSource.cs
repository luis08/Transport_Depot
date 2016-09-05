
using System.Data.SqlClient;
using System.Linq;
using System.Data;
using System;

namespace TransportDepot.Data.Misc
{
  public class DataSource
  {
    Data.DataSource _dataSource = new Data.DataSource();
    Utilities _utilities = new Utilities();

    public void CopyTractorTable()
    {
      Save("Tractor");
    }

    public void RestoreTractorsFromTempFile()
    {
      Execute(WorkaroundsQueries.InsertTractors);
    }

    public void DeleteTractors()
    {
      Execute("DELETE FROM [dbo].[Tractor]");
    }

    public void CopyEmployeeTable()
    {
      Save("PrEmployee");
    }

    public void RestoreEmployeesFromTempFile()
    {
      Execute(WorkaroundsQueries.InsertEmployees);
    }

    public void DeleteEmployees()
    {
      Execute("DELETE FROM [dbo].[PrEmployee]");
    }

    public void CopyBillingHistoryTable()
    {
      Save("BillingHistory");
    }

    public void RestoreBillingHistoryFromTempFile()
    {
      Execute(WorkaroundsQueries.InsertBillingHistory);
    }

    public void DeleteBillingHistory()
    {
      Execute("DELETE FROM [dbo].[BillingHistory]");
    }

    public void CopyArEntryTable()
    {
      Save("ArEntry");
    }

    public void RestoreArEntryFromTempFile()
    {
      Execute(WorkaroundsQueries.InsertArEntry);
    }

    public void DeleteArEntry()
    {
      Execute("DELETE FROM [dbo].[ArEntry]");
    }

    public void CopyRsPayableTable()
    {
      Save("RsPayable");
    }

    public void RestoreRsPayableFromTempFile()
    {
      Execute(WorkaroundsQueries.InsertRsPayable);
    }

    public void DeleteRsPayable()
    {
      Execute("DELETE FROM [dbo].[RsPayable]");
    }

    public void CopyRsPayableHistoryTable()
    {
      Save("RsPayableHistory");
    }

    public void RestoreRsPayableHistoryFromTempFile()
    {
      Execute(WorkaroundsQueries.InsertRsPayableHistory);
    }

    public void DeleteRsPayableHistory()
    {
      Execute("DELETE FROM [dbo].[RsPayableHistory]");
    }

    public void CopyGlEntryTable()
    {
      Save("GlEntry");
    }

    public void RestoreGlEntryFromTempFile()
    {
      Execute(WorkaroundsQueries.InsertGlEntry);
    }

    public void DeleteGlEntry()
    {
      Execute("DELETE FROM [dbo].[GlEntry]");
    }

    public void AppendTractor(string id)
    {
      var query = "INSERT INTO [dbo].[Tractor] (cTractorId) VALUES (@TractorId)";
      using (var cmd = new SqlCommand(query))
      {
        cmd.Parameters.AddWithValue("@TractorId", id);
        Execute(cmd);
      }
    }

    public void AppendDriver(string driverId, string driverFirstName)
    {
      using (var cn = new SqlConnection(_dataSource.ConnectionString))
      {
        cn.Open();
        var transaction = cn.BeginTransaction("AppendDriverTransaction");
        try
        {
          AppendEmployee(driverId, driverFirstName, cn, transaction);
          AppendDriver(driverId, cn, transaction);
          transaction.Commit();
        }
        catch (Exception)
        {
          transaction.Rollback();
          throw;
        }
      }

    }

    public int GetTractorCount()
    {
      return this.GetCount("Tractor");
    }

    public int GetEmployeeCount()
    {
      return this.GetCount("PrEmployee");
    }

    public int GetBillingHistoryCount()
    {
      return this.GetCount("BillingHistory");
    }

    public int GetArEntryCount()
    {
      return this.GetCount("BillingHistory");
    }

    public int GetRsPayableCount()
    {
      return this.GetCount("RsPayable");
    }

    public int GetRsPayableHistoryCount()
    {
      return this.GetCount("RsPayableHistory");
    }

    public int GetGlEntryCount()
    {
      return this.GetCount("GlEntry");
    }

    private void AppendDriver(string driverId, SqlConnection cn, SqlTransaction transaction)
    {
      var driverQuery = "INSERT INTO [dbo].[DriverInfo] (cDriverId) VALUES (@DriverId)";
      using (var cmd = new SqlCommand(driverQuery))
      {
        cmd.Transaction = transaction;
        cmd.Parameters.AddWithValue("@DriverId", driverId);
        Execute(cmd, cn);
      }
    }

    private void AppendEmployee(string driverId, string driverFirstName, SqlConnection cn, SqlTransaction transaction)
    {
      var employeeQuery = "INSERT INTO [dbo].[PrEmployee] (cEmployeeId, cFirst) VALUES (@EmployeeId, @FirstName)";
      using (var cmd = new SqlCommand(employeeQuery))
      {
        cmd.Transaction = transaction;
        cmd.Parameters.AddWithValue("@EmployeeId", driverId);
        cmd.Parameters.AddWithValue("@FirstName", driverFirstName);
        Execute(cmd, cn);
      }
    }

    private int GetCount(string tableName)
    {
      var query = string.Format("SELECT COUNT(*) AS RecordCount FROM [dbo].[{0}]", tableName);
      var table = _dataSource.FetchDataTable(new SqlCommand(query));
      if (_utilities.IsEmpty(table))
      {
        return 0;
      }
      else
      {
        var records = table.AsEnumerable().First().Field<int>("RecordCount");
        return records;
      }
    }

    private void Save(string tableName)
    {
      var query = string.Format(WorkaroundsQueries.BackupTable, tableName);
      Execute(query);
    }

    private void Execute(string query)
    {
      var sd = new Data.DataSource();
      using (var cmd = new SqlCommand(query))
      {
        Execute(cmd);
      }
    }

    private void Execute(SqlCommand cmd)
    {
      using (var cn = new SqlConnection(_dataSource.ConnectionString))
      {
        Execute(cmd, cn);
      }
    }

    private static void Execute(SqlCommand cmd, SqlConnection cn)
    {
      cmd.Connection = cn;
      if (!cn.State.Equals(ConnectionState.Open))
      {
        cn.Open();
      }
      cmd.ExecuteNonQuery();
    }
  }
}

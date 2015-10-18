using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Models.Dispatch;
using System.Data.SqlClient;
using System.Data;

namespace TransportDepot.Data.Dispatch
{
  public class QueuingDataSource
  {
    private DB.DataSource _dbDataSource = new DB.DataSource();
    private Utilities _dbUtil = new Utilities();


    
    public IEnumerable<DispatchQueue> GetQueues()
    {
      var query = "SELECT [Id], [Name] , [Comments], [GeoLocationId] FROM [DispatchQueue]";
      var tbl = new DataTable();
      
      using(var cmd = new SqlCommand(query))
      {
        var ds = new DataSource();
        tbl = ds.FetchDataTable(cmd);
      }

      if (_dbUtil.IsEmpty(tbl)) return new List<DispatchQueue>();
      var queues = tbl.AsEnumerable().Select(q => new DispatchQueue
      {
        Id = q.Field<int>("Id"),
        Name = q.Field<string>("Name"),
        Comments = q.Field<string>("Comments"),
        GeoLocationId = _dbUtil.ParseGeoLocationId(q["GeoLocationId"])
      });
      return queues;
    }

    public void Create(DispatchQueue queue)
    {
      var insertQuery = @"
        INSERT INTO [dbo].[DispatchQueue] ( [Name], [Comments], [GeoLocationId] )
                                    VALUES( @Name, @Comments, @GeoLocationId    )";
      try
      {
        queue.GeoLocationId = _dbUtil.ParseGeoLocationId(queue.GeoLocationId);
        using (var cn = new SqlConnection(this._dbDataSource.ConnectionString))
        using (var cmd = new SqlCommand(insertQuery, cn))
        {
          cmd.Parameters.AddWithValue("@Name", queue.Name);
          cmd.Parameters.AddWithValue("@Comments", queue.Comments);
          cmd.Parameters.AddWithValue("@GeoLocationId", queue.GeoLocationId);
          cn.Open();
          cmd.ExecuteNonQuery();
        }
      }
      catch (Exception e)
      {
        var errors = new string[]
        {
          e.Message, e.StackTrace
        };
        _dbUtil.LogError(this, errors);

      }
    }

    public void Update(DispatchQueue queue)
    {
      var updateQuery = @"
      UPDATE [dbo].[DispatchQueue]
        SET [Name] = @Name
          , [Comments] = @Comments
          , [GeoLocationId] = @GeoLocationId
        WHERE [Id] = @Id
      ";
      try
      {
        queue.GeoLocationId = _dbUtil.ParseGeoLocationId(queue.GeoLocationId);

        var affectedRecords = 0;
        using (var cn = new SqlConnection(this._dbDataSource.ConnectionString))
        using (var cmd = new SqlCommand(updateQuery, cn))
        {
          cmd.Parameters.AddWithValue("@Id", queue.Id);
          cmd.Parameters.AddWithValue("@Name", queue.Name);
          cmd.Parameters.AddWithValue("@Comments", queue.Comments);
          cmd.Parameters.AddWithValue("@GeoLocationId", queue.GeoLocationId);
          cn.Open();
          affectedRecords = cmd.ExecuteNonQuery();
        }
      }
      catch (Exception e)
      {
        var errors = new string[]
        {
          e.Message, e.StackTrace
        };
        _dbUtil.LogError(this, errors);
      }
    }

    public void Delete(int id)
    {
      var updateQuery = @"
      DELETE FROM [dbo].[DispatchQueue]
      WHERE [Id] = @Id ";
      try
      {
        var affectedRecords = 0;
        using (var cn = new SqlConnection(this._dbDataSource.ConnectionString))
        using (var cmd = new SqlCommand(updateQuery, cn))
        {
          cmd.Parameters.AddWithValue("@Id", id);
          cn.Open();
          affectedRecords = cmd.ExecuteNonQuery();
        }
      }
      catch (Exception e)
      {
        var errors = new string[]
        {
          e.Message, e.StackTrace
        };
        _dbUtil.LogError(this, errors);
      }
    }
  }
}

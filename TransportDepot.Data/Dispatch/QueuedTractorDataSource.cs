
using TransportDepot.Models.Dispatch;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Data.SqlClient;
using System;

namespace TransportDepot.Data.Dispatch
{
  public class QueuedTractorDataSource
  {
    private DataSource _dataSource = new DataSource();
    public bool Queue(int queueId, IEnumerable<string> tractorIds)
    {
      if( tractorIds == null ) throw new ArgumentException("TractorIds is null");
      if( !tractorIds.Any()) throw new ArgumentException("No Tractors to queue supplied");

      XDocument tractorIdsXml = this.GetTractorIdsXml(queueId, tractorIds);
      var queueSql = @"
        DECLARE @queuedTractorsXml XML
        SET @queuedTractorsXml = CAST( @queuedTractorsXmlString AS XML )
        
        ;WITH [QT] AS
        (

          SELECT   [T].[C].value('../@id', 'int') AS [DispatchQueueId]
                 , [T].[C].value('.', 'varchar(12)') AS [TractorId]
          FROM @queuedTractorsXml.nodes('//tractorId') AS T(C)
        )

        INSERT INTO [dbo].[QueuedTractor]( [DispatchQueue], [TractorId] )
        SELECT [DispatchQueueId], [TractorId] 
        FROM [QT] AS [NQ]
        WHERE NOT EXISTS --A record in QueuedTractor lacking a match in dequeued tractor for the same queue
        (
          SELECT * 
          FROM [DispatchQueue] [Q]
          WHERE ( [TractorId] = [NQ].[TractorId] )
            AND NOT EXISTS
            (
              SELECT * 
              FROM [dbo].[DeQueuedTractor] [DQ]
              WHERE ([Q].[Id] = [DQ].[Id] )
            )
        )
      ";
      var recordsAffected = 0;
      try
      {
        using (var cn = new SqlConnection(_dataSource.ConnectionString))
        using (var cmd = new SqlCommand(queueSql, cn))
        {
          cmd.Parameters.AddWithValue("@queuedTractorsXmlString", tractorIdsXml.ToString());
          recordsAffected = cmd.ExecuteNonQuery();
        }
      }
      catch (Exception e)
      {
        return false;
      }
      return recordsAffected == tractorIds.Count();
    }

    private XDocument GetTractorIdsXml(int queueId, IEnumerable<string> tractorIds)
    {
      var xml = new XDocument(new XElement("queue",
        new XAttribute("id", queueId),
        tractorIds.Select(t => new XElement("tractorId", t))));
      return xml;
    }

    public bool DeQueue(IEnumerable<int> queuedTractorIds)
    {
      if (queuedTractorIds == null) throw new ArgumentException("Queued Tractor identifiers is null");
      if (!queuedTractorIds.Any()) throw new ArgumentException("Queued Tractor identifiers supplied");
      var xml = new XDocument("queuedTractorIds",
        queuedTractorIds.Select(t => new XElement("id", t))); 
      var queueSql = @"

        ;WITH [QT] AS
        (

          SELECT [T].[C].value('.', 'int') AS [QueuedTractorId]
          FROM @queuedTractorIds.nodes('//id') AS T(C)
        )

        INSERT INTO [dbo].[DeQueuedTractor]( [Id] )
        SELECT [QueuedTractorId]
        FROM [QT] 
      ";
      var recordsAffected = 0;
      try
      {
        using (var cn = new SqlConnection(_dataSource.ConnectionString))
        using (var cmd = new SqlCommand(queueSql, cn))
        {
          cmd.Parameters.AddWithValue("@queuedTractorsXmlString", xml.ToString());
          recordsAffected = cmd.ExecuteNonQuery();
        }
      }
      catch (Exception e)
      {
        return false;
      }
      return recordsAffected > 0; 
    }

    public bool Update(QueuedTractor queuedTractor)
    {
      var query = @"
        UPDATE [dbo].[QueuedTractor]
          SET [QueueId] = @QueueId 
            , [DispatcherInitials] = @DispatcherInitials 
            , [KeepWhenLoaded] = @KeepWhenLoaded 
        WHERE [Id] = @Id
      ";
      var recordsAffected = 0;
      try
      {
        using (var cn = new SqlConnection(_dataSource.ConnectionString))
        using (var cmd = new SqlCommand(query, cn))
        {
          cmd.Parameters.AddWithValue("@Id", queuedTractor.Id);
          cmd.Parameters.AddWithValue("@QueueId", queuedTractor.QueueId);
          cmd.Parameters.AddWithValue("@DispatcherInitials", queuedTractor.DispatcherInitials);
          cmd.Parameters.AddWithValue("@KeepWhenLoaded", queuedTractor.KeepWhenLoaded);
          recordsAffected = cmd.ExecuteNonQuery();
        }
      }
      catch (Exception e)
      {
        return false;
      }
      return recordsAffected == 1; 
    }
  }
}

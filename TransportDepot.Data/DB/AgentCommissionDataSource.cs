using System;
using System.Collections.Generic;
using System.Linq;
using TransportDepot.Models.Payables.Commissions;
using System.Data.SqlClient;
using System.Data;

namespace TransportDepot.Data.DB
{
  public class AgentCommissionDataSource
  {
    public IEnumerable<CommissionCandidate> GetCommissionCandidates()
    {
      var candidatesTable = new DataTable();
      var dataSource = new DataSource();
      using (var cmd = new SqlCommand(Queries.Candidates))
      {
        candidatesTable = dataSource.FetchCommand(cmd);
      }
      if (candidatesTable.Rows.Count == 0)
      {
        return new List<CommissionCandidate>();
      }
      var candidates = candidatesTable.AsEnumerable().Select(c => new CommissionCandidate
      {
        TractorId = c.Field<string>("TractorId"),
        StartDate = c.Field<DateTime>("StartDate"),
        EndDate = c.Field<DateTime>("EndDate"),
        StartLocation = new Location { State = c.Field<string>("StartState") },
        EndLocatioin = new Location { State = c.Field<string>("EndState") },
        InvoiceNumber = c.Field<string>("InvoiceNumber"),
        InvoiceAmount = c.Field<decimal>("InvoiceAmount"),
        TripNumber = c.Field<string>("TripNumber"),
        AgentId = c.Field<string>("DispatcherID")
      });
      return candidates;
    }

    static class Queries
    {
      public static string Candidates
      {
        get
        {
          return @"
              DECLARE @LastBillDate DATETIME
              SELECT @LastBillDate = MAX( [dBillDate] ) 
              FROM [Truckwin_TDPD_Access]...[BillingHistory] [BH]
                INNER JOIN [dbo].[Paid_Invoice_Commission] [C]
                  ON ( [BH].[cProNumber] = [C].[ArInvoiceNumber] )
              SELECT TOP 100 [cTractorID]           AS [TractorId]
                           , [BH].[dShipDate]       AS [StartDate]
                           , [BH].[dTripFinishDate] AS [EndDate]
                           , [BH].[cOrigState]      AS [StartState]
                           , [BH].[cDestState]      AS [EndState]
                           , [BH].[cProNumber]      AS [InvoiceNumber]
                           , [BH].[cTripNumber]     AS [TripNumber]
                           , [R].[cuSubMainRevenue] AS [InvoiceAmount]
                           , [R].[cAgent1Id]        AS [DispatcherID]
              FROM [Truckwin_TDPD_Access]...[Revenue] [R]
                INNER JOIN [Truckwin_TDPD_Access]...[BillingHistory] [BH]
                  ON ( [R].[cProNumber] = [BH].[cProNumber] )
                INNER JOIN [Truckwin_TDPD_Access]...[AssignedToHist] [ATH]
                  ON ( [BH].[cTripNumber] = [ATH].[cTripNumber] )      
              WHERE ( CAST( [bAgent1Mark] AS BIT ) != 0 )
                AND NOT EXISTS
                (
                  SELECT * 
                  FROM [dbo].[Paid_Invoice_Commission] [C]
                  WHERE ( [C].[ArInvoiceNumber] = [BH].[cProNumber] )
                )
                AND ( COALESCE( [cTractorID], '' ) != '' )
                AND ( [dBillDate] > @LastBillDate )
                
          ";
        }
      }
    }
  }
}

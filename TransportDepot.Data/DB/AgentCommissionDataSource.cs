using System;
using System.Collections.Generic;
using System.Linq;
using TransportDepot.Models.Payables.Commissions;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;

namespace TransportDepot.Data.DB
{
  public class AgentCommissionDataSource
  {
    private const string DecimalToStringFormat = "G";

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
        LessorId = c.Field<string>("LessorId"),
        StartDate = c.Field<DateTime>("StartDate"),
        EndDate = c.Field<DateTime>("EndDate"),
        StartLocation = new Location { State = c.Field<string>("StartState") },
        EndLocatioin = new Location { State = c.Field<string>("EndState") },
        InvoiceNumber = c.Field<string>("InvoiceNumber"),
        InvoiceAmount = c.Field<decimal>("InvoiceAmount"),
        LessorRevenue = c.Field<decimal>("LessorRevenue"),
        TripNumber = c.Field<string>("TripNumber"),
        AgentId = c.Field<string>("DispatcherID")
      });
      return candidates;
    }

    public void Save(IEnumerable<InvoiceCommission> commissions)
    {
      var xml = this.GetCommissionsXml(commissions);
      var dataSource = new DataSource();
      var insertedCommissions =this.TrackedCommissionPaid(xml, dataSource, commissions.Count());
      if (insertedCommissions == 0)
      {
        throw new InvalidOperationException("no commissions inserted");
      }
      else if (insertedCommissions != commissions.Count())
      {
        throw new InvalidOperationException("The number of commissions is invalid");
      }
      {
        this.SaveApInvoice(xml, dataSource);
      }
      
    }

    public IEnumerable<LessorHome> GetLessorHomes(IEnumerable<string> tripIds)
    {
      var xml = new XDocument(new XElement("trips",
        tripIds.Select(t => new XElement("trip", t))));
      var tractorHomes = new DataTable();
      using (var cmd = new SqlCommand
      {
        CommandText = Queries.LessorHome,
        CommandType = CommandType.Text
      })
      {
        var dataSource = new DataSource();
        cmd.Parameters.AddWithValue("@TripsXmlString", xml.ToString());
        tractorHomes = dataSource.FetchCommand(cmd);
      }

      if (tractorHomes.Rows.Count == 0)
      {
        return new List<LessorHome>();
      }
      var homes = tractorHomes.AsEnumerable()
        .ToDictionary(l => l.Field<string>("TractorID"), l =>
        {
          var location =l.Field<string>("State");
          if (string.IsNullOrEmpty(location))
          {
            return null;
          }
          return new Location { State = location };
        });
      return homes.Select(h=> new LessorHome{
        LessorId = h.Key,
        Location = h.Value
      }).ToList();
    }

    public IEnumerable<PreviousTrip> GetPreviousTrips(IEnumerable<CommissionCandidate> candidates)
    {
      var candidatesXml = new XDocument(new XElement("trips",
        candidates.Select(c => new XElement("trip", c.TripNumber,
          new XAttribute("lessorId", c.LessorId)))));

      var tbl = new DataTable();
      var dataSource = new DataSource();
      using (var cmd = new SqlCommand(Queries.PreviousSpans))
      {
        cmd.Parameters.AddWithValue("@TripsXmlString", candidatesXml.ToString());
        tbl = dataSource.FetchCommand(cmd);
      }
      if (tbl.Rows.Count.Equals(0))
      { return new List<PreviousTrip>(); }

      
      var spans = tbl.AsEnumerable()
        .Select(s => new PreviousTrip
        {
          LessorId = s.Field<string>("LessorID"),
          TripNumber = s.Field<string>("TripNumber"),
          PreviuosSpan = new Span
          {
            StartDate = s.Field<DateTime>("PreviousStartDate"),
            EndDate = s.Field<DateTime>("PreviousEndDate"),
            StartLocation = this.GetLocation(s, "PreviousStartState"),
            EndLocation = this.GetLocation(s, "PreviousEndState"),
            LessorId = s.Field<string>("LessorID"), 
            PreviousSpan = null
          }
        });

      return spans;
    }

    private Location GetLocation(DataRow s, string fieldName)
    {
      if (s.IsNull(fieldName))
      {
        return null;
      }
      return new Location { State = s.Field<string>(fieldName) };


    }

    private int TrackedCommissionPaid(XDocument commissionsXml, DataSource dataSource, int commissionCount)
    {
      var recordsAffected = 0;
      using(var cmd = new SqlCommand
      {
        CommandText = Queries.TrackCommissionPaid,
        CommandType = CommandType.Text
      })
      {
        cmd.Parameters.AddWithValue("@CommissionsXmlString", commissionsXml.ToString());
        recordsAffected = dataSource.ExecuteNonQuery(cmd);
        return recordsAffected;
      }
    }

    private void SaveApInvoice(XDocument commissionsXml, DataSource dataSource)
    {
      using (var cmd = new SqlCommand
      {
        CommandText = Queries.SaveCommissionAPInvoices,
        CommandType = CommandType.Text
      })
      {
        cmd.Parameters.AddWithValue("@CommissionsXmlString", commissionsXml.ToString());
        dataSource.ExecuteNonQuery(cmd);
      }
    }

    private XDocument GetCommissionsXml(IEnumerable<InvoiceCommission> commissions)
    {
      
      var glDepartment = this.GetSetting("TransportDepot.Payables.Commissions.NewCommission.GLDepartment");
      var glAccount = this.GetSetting("TransportDepot.Payables.Commissions.NewCommission.GLAccount");
      
      var reference = "Dispatch Com";//varchar 12
      var invoiceDate = DateTime.Today.ToShortDateString();
      var xml = new XDocument(new XElement("commissions",
        new XAttribute("glDepartment", glDepartment),
        new XAttribute("glAccount", glAccount),
        new XAttribute("invoiceDate", invoiceDate),
        new XAttribute("reference", reference),
        commissions.Select(c => new XElement("commission",
          new XAttribute("agent", c.AgentId),
          new XAttribute("apInvoiceNumber", c.TripNumber),
          new XAttribute("arInvoiceNumber", c.InvoiceNumber),
          new XAttribute("arInvoiceAmount", c.InvoiceAmount.ToString(DecimalToStringFormat)),
          new XAttribute("percentCommission", decimal.Divide( c.Percent, 100m )),
          new XAttribute("commissionTotal", decimal.Round(decimal.Multiply( 
            decimal.Multiply(c.Percent, 0.01m), 
            c.InvoiceAmount), 2).ToString() ),
          new XAttribute("tractorId", c.LessorId),
          new XAttribute("mInvoiceDescription", this.GetInvoiceDescription(c)),
          new XAttribute("dueDate", c.DueDate.ToShortDateString())
          ))));
      return xml;
    }

    private string GetInvoiceDescription(InvoiceCommission c)
    {
      return string.Format("Pro: {0} - {1:P} of {2:G}  Lessor: {3}", c.InvoiceNumber, 
         decimal.Divide(c.Percent, 100.0m) , 
         c.InvoiceAmount, c.LessorId);
    }

    private string GetSetting(string settingName)
    {
      
      if (settingName == null)
      {
        var message = string.Format("Setting '{0}' must be defined in the config file", settingName);
        throw new InvalidOperationException(message);
      }
      string settingValue = System.Configuration.ConfigurationManager.AppSettings[settingName];
      return settingValue.Trim(); 
    }

    public IEnumerable<TripInvoiceMapping> GetTripInvoiceMappings(IEnumerable<string> invoiceNumbers)
    {
      var dataSource = new DataSource();
      var invoicesXml = new XDocument( new XElement("invoices", 
        invoiceNumbers.Select(i=> new XElement("invoice", i))));
      
      var cmd = new SqlCommand
      {
        CommandText = Queries.InvoiceMappings,
        CommandType = CommandType.Text
      };
      cmd.Parameters.AddWithValue("@InvoicesString", invoicesXml.ToString());
      var mapTable = dataSource.FetchCommand(cmd);

      if (DataSource.IsEmpty(mapTable))
      {
        return new List<TripInvoiceMapping>();
      }
      var map = mapTable.AsEnumerable()
        .Select(t => new TripInvoiceMapping
        {
          TripNumber = t.Field<string>("TripNumber"),
          InvoiceNumber = t.Field<string>("InvoiceNumber"),
          BillDate = t.Field<DateTime>("BillDate")
        });
      
      return map;
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
              FROM [dbo].[BillingHistory] [BH]
                INNER JOIN [dbo].[Paid_Invoice_Commission] [C]
                  ON ( [BH].[cProNumber] = [C].[ArInvoiceNumber] )
              SELECT         [ATH].[cLessorId1]     AS [LessorID]
                           , [BH].[dShipDate]       AS [StartDate]
                           , [BH].[dTripFinishDate] AS [EndDate]
                           , [BH].[cOrigState]      AS [StartState]
                           , [BH].[cDestState]      AS [EndState]
                           , [BH].[cProNumber]      AS [InvoiceNumber]
                           , [BH].[cTripNumber]     AS [TripNumber]
                           , [R].[cuSubMainRevenue] AS [InvoiceAmount]
                           , [ATH].[cuLessor1Rate]  AS [LessorRevenue]
                           , [DA].[VendorID]        AS [DispatcherID]
              FROM [dbo].[Revenue] [R]
                INNER JOIN [dbo].[BillingHistory] [BH]
                  ON ( [R].[cProNumber] = [BH].[cProNumber] )
                INNER JOIN [dbo].[AssignedToHist] [ATH]
                  ON ( [BH].[cTripNumber] = [ATH].[cTripNumber] )      
                INNER JOIN [DispatcherAgent] [DA] 
                  ON ([DA].[Initials] = [BH].[cDispatcherInit] )
              WHERE NOT EXISTS
                (
                  SELECT * 
                  FROM [dbo].[Paid_Invoice_Commission] [C]
                  WHERE ( [C].[ArInvoiceNumber] = [BH].[cProNumber] )
                )
                AND ( [dBillDate] > '5-27-2015'  )
              --  AND ( [dBillDate] > COALESCE( @LastBillDate, '1-1-2001' ) )
              
                
          ";
        }
      }

      public static string PreviousSpans 
      {
        get
        {
          return @"
  
            DECLARE @TripsXml XML
            SET @TripsXml = @TripsXmlString

            ;WITH [LessorHistory]  AS
            (
              SELECT [AH].[cLessorId1]      AS [LessorID] 
                   , [AH].[cTripNumber]     AS [TripNumber]
                   , [BH].[dShipDate]       AS [StartDate]
                   , [BH].[dTripFinishDate] AS [EndDate]
                   , [BH].[cOrigState]      AS [StartState]
                   , [BH].[cDestState]      AS [EndState]
              FROM  [dbo].[BillingHistory] [BH]
              INNER JOIN [dbo].[AssignedToHist] [AH]
                ON [BH].[cTripNumber] = [AH].[cTripNumber]
            ), [Originals] AS
            (
              SELECT [T].[C].value('.', 'varchar(20)') AS [TripNumber]
                   , [T].[C].value('./@lessorId', 'varchar(20)' ) AS [LessorID]
              FROM @TripsXml.nodes('//trip') AS [T](C)
            )

            SELECT 
                  [P].[TripNumber] AS [PreviousTripNumber]
                , [P].[StartDate]  AS [PreviousStartDate]
                , [P].[EndDate]    AS [PreviousEndDate]
                , [P].[StartState] AS [PreviousStartState]
                , [P].[EndState]   AS [PreviousEndState]
                , [LH].* 
            FROM [LessorHistory] [LH]
            CROSS APPLY
            (
              SELECT TOP 1 [LessorID]
                         , [TripNumber]
                         , [StartDate] 
                         , [EndDate]
                         , [StartState]
                         , [EndState]
              FROM [LessorHistory] [LHI]
              WHERE ( [LHI].[EndDate] < [LH].[StartDate] )
                AND ( [LHI].[LessorID] = [LH].[LessorID] )
              ORDER BY [EndDate] DESC
            )  AS [P] 
            WHERE EXISTS
            (
              SELECT *
              FROM [Originals] [O]
              WHERE ( [O].[LessorID]   = [LH].[LessorID] )
               AND  ( [O].[TripNumber] = [LH].[TripNumber] )
  
            )  
          ";
        }
      }

      public static string SaveCommissionAPInvoices
      {
        get
        {
          return @"
          /*  Needs parameter @CommissionsXmlString  */

          DECLARE @Commissions XML
                
          SELECT @Commissions = CAST(@CommissionsXmlString AS XML)

          /* Invoice Deetails */
          ;WITH [NewCommissions] AS
          (
            SELECT [T].[c].value('./@agent', 'varchar(30)') AS [cVendorId]
                 , [T].[c].value('./@apInvoiceNumber', 'varchar(30)') AS [cInvoiceNo]
                 , 1 AS [niDetailNumber] 
                 , [T].[c].value('./@mInvoiceDescription', 'varchar(200)') AS [mInvoiceDesc]  
                 , [T].[c].value('../@glDepartment', 'varchar(30)') AS [cGlDept]
                 , [T].[c].value('../@glAccount', 'varchar(30)') AS [cGlAcct]
                 , 0 AS [b1099] 
                 , [T].[c].value('./@percentCommission', 'smallmoney') AS [cuQuantity]
                 , [T].[c].value('./@arInvoiceAmount', 'smallmoney') AS [cuUnitCost]
            FROM @commissions.nodes('//commission') AS T(c)
          )

          INSERT INTO [dbo].[ApInvoiceDetail]
          (
                [cVendorId] 
              , [cInvoiceNumber] 
              , [niDetailNumber] 
              , [mDescription] 
              , [cGlDept] 
              , [cGlAcct] 
              , [b1099] 
              , [cuQuantity] 
              , [cuUnitCost] 
          )

          SELECT [cVendorId] 
               , [cInvoiceNo] AS [cInvoiceNumber] 
               , [niDetailNumber] 
               , [mInvoiceDesc]
               , [cGlDept] 
               , [cGlAcct] 
               , [b1099] 
               , [cuQuantity] 
               , [cuUnitCost] 
          FROM [NewCommissions]


          ;WITH [NewCommissions] AS
          (
            SELECT [T].[c].value('./@apInvoiceNumber', 'varchar(30)') AS [cInvoiceNo]
                 , [T].[c].value('./@agent', 'varchar(30)') AS [cVendorId]
                 , [T].[c].value('../@invoiceDate', 'varchar(30)') AS [dInvoiceDate]
                 , [T].[c].value('./@arInvoiceNumber', 'varchar(30)') AS [ArInvoiceNumber]
                 , [T].[c].value('./@arInvoiceAmount', 'smallmoney') AS [ArInvoiceAmount]
                 , [T].[c].value('./@commissionTotal', 'smallmoney') AS [cuInvoiceTotal]
                 , [T].[c].value('./@commissionTotal', 'smallmoney') AS [cuBalanceDue]
                 , [T].[c].value('./@tractorId', 'varchar(30)') AS [cTractorId]
                 , [T].[c].value('./@mInvoiceDescription', 'varchar(200)') AS [mInvoiceDesc]  
                 , '' AS [cTermsCode] 
                 , [T].[c].value('./@dueDate', 'datetime') AS [dDueDate]  
                 , [T].[c].value('../@invoiceDate', 'varchar(30)') AS [dDiscountDate] 
                 , 0.0 AS [cuDiscountAmt]
                 , 0.0 AS [cuGSTAmt]
                 , 0.0 AS [cuPSTAmt]
                 , [T].[c].value('../@glDepartment', 'varchar(30)') AS [cGlDept]
                 , [T].[c].value('../@glAccount', 'varchar(30)') AS [cGlAcct]
                 , 0 AS [bSelected]
            FROM @commissions.nodes('//commission') AS T(c)
          )
          
          INSERT INTO [dbo].[ApInvoice]
          (
                [cInvoiceNo] 
              , [cVendorId] 
              , [dInvoiceDate] 
              , [cuInvoiceTotal] 
              , [cuBalanceDue] 
              , [mInvoiceDesc] 
              , [cTermsCode] 
              , [dDueDate] 
              , [dDiscountDate] 
              , [cuDiscountAmt] 
              , [cuGSTAmt] 
              , [cuPSTAmt] 
              , [bSelected] 
          )

          SELECT
                [cInvoiceNo] 
              , [cVendorId] 
              , [dInvoiceDate] 
              , [cuInvoiceTotal] 
              , [cuBalanceDue] 
              , [mInvoiceDesc] 
              , [cTermsCode] 
              , [dDueDate] 
              , [dDiscountDate] 
              , [cuDiscountAmt] 
              , [cuGSTAmt] 
              , [cuPSTAmt] 
              , [bSelected] 
          FROM [NewCommissions]       


        ";
        }
      }

      public static string InvoiceMappings
      {
        get
        {
          return @"
            DECLARE @Invoices XML
            SET @Invoices = CAST( @InvoicesString AS XML )

            ;WITH [Invoices] AS
            (
              SELECT [T].[C].value('.', 'varchar(20)') AS [InvoiceNumber]
              FROM @Invoices.nodes('//invoice') AS T(C)
            )
            SELECT [cTripNumber] AS [TripNumber]
                 , [cProNumber] AS [InvoiceNumber]
                 , MAX( [dBillDate] ) AS [BillDate]
            FROM [dbo].[BillingHistory] [BH]
              INNER JOIN [Invoices] [I]
                ON ( [I].[InvoiceNumber] = [BH].[cProNumber] )
            GROUP BY [cTripNumber]
                 , [cProNumber]
             
          ";
        }
      }

      public static string TrackCommissionPaid
      {
        get
        {
          return @"
          DECLARE @Commissions XML
          SET @Commissions = CAST( @CommissionsXmlString AS XML )


          INSERT INTO [dbo].[Paid_Invoice_Commission]
                   ([ApInvoiceNumber]
                   ,[ArInvoiceNumber]
                   ,[DueDate])

          SELECT [T].[C].value('./@apInvoiceNumber', 'varchar(20)') AS [ApInvoiceNumber]
              ,  [T].[C].value('./@arInvoiceNumber', 'varchar(20)') AS [ArInvoiceNumber]
              ,  [T].[C].value('./@dueDate', 'varchar(20)')         AS [DueDate]
          FROM @Commissions.nodes('//commission') AS T(C) 
      ";
        }
      }

      public static string LessorHome
      {
        get
        {
          return @"

            DECLARE @Trips XML
            SET @Trips = CAST( @TripsXmlString AS XML )

            ;WITH [Trips] AS
            (
              SELECT T.C.value('.', 'varchar(20)') AS [TripNumber]
              FROM @Trips.nodes('//trip') AS T(C)
            ), [Tractors] AS
            (
              SELECT [ATH].[cTractorID] AS [TractorID]
                   , CASE 
                        WHEN ( [SqlL].[Home_State] IS NOT NULL ) THEN [SqlL].[Home_State]
                        WHEN ( [L].[cState]   IS NOT NULL ) THEN [L].[cState]
                        ELSE ''
                     END AS [State]
              FROM [dbo].[AssignedToHist] [ATH]
                LEFT JOIN [dbo].[Lessor] [SqlL]
                  ON [ATH].[cLessorId1] = [SqlL].[LessorID]
                LEFT JOIN [dbo].[RsLessor] [L]
                  ON [ATH].[cLessorId1] = [L].[cId]
              WHERE EXISTS
              (
                SELECT * 
                FROM [Trips] [T]
                WHERE [T].[TripNumber] = [ATH].[cTripNumber]
              )
            )

            SELECT [TractorID], [State]
            FROM [Tractors]
            WHERE ( COALESCE( [TractorID], '' ) <> '' )
            GROUP BY [TractorID], [State]
        ";
        }
      }
    
    }
  }
}

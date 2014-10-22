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

    public void Save(IEnumerable<InvoiceCommission> commissions)
    {
      var xml = this.GetCommissionsXml(commissions);
      xml.Save(@"c:\transport_depot\commissions.xml");
      
    }

    private XDocument GetCommissionsXml(IEnumerable<InvoiceCommission> commissions)
    {
      var glDeparment = this.GetSetting("TransportDepot.Payables.Commissions.NewCommission.GLDepartment");
      var glAccount = this.GetSetting("TransportDepot.Payables.Commissions.NewCommission.GLAccount");
      var dueDates = this.GetCommissionDueDates(commissions.Select(c=>c.InvoiceNumber));
      var xml = new XDocument(new XElement("commissions",
        commissions.Select(c => new XElement("commission",
          new XAttribute("agent", c.AgentId),
          new XAttribute("arInvoiceNumber", c.InvoiceNumber),
          new XAttribute("arInvoiceAmount", c.InvoiceAmount.ToString()),
          new XAttribute("commissionTotal", decimal.Multiply( 
            decimal.Multiply(c.Percent, 0.01m), c.InvoiceAmount).ToString() ),
          new XAttribute("tractorId", c.TractorId),
          new XAttribute("mInvoiceDescription", this.GetInvoiceDescription(c)),
          new XAttribute("dueDate", dueDates[c.InvoiceNumber].ToShortDateString()),
          new XAttribute("glDepartment", glDeparment),
          new XAttribute("glAccount", glAccount)))));
      return xml;
    }

    private string GetInvoiceDescription(InvoiceCommission c)
    {
      return string.Format("Pro: {0}  Truck: {1}", c.InvoiceNumber, c.TractorId);
    }

    private string GetSetting(string settingName)
    {
      string settingValue = System.Configuration.ConfigurationManager.AppSettings[settingName];
      if (settingName == null)
      {
        return string.Empty;
      }
      return settingValue.Trim(); 
    }

    private Dictionary<string, DateTime> GetCommissionDueDates(IEnumerable<string> invoiceNumbers)
    {
      var invoiceNumbersXml = new XDocument("invoices",
        invoiceNumbers.Select(invNum => new XElement("invoice", invNum)));

      var tbl = new DataTable();
      var dict = tbl.AsEnumerable()
        .ToDictionary(i => i.Field<string>("InvoiceNumber"), i => i.Field<DateTime>("BillDate"));
      return dict;
    }

    public IEnumerable<TripInvoiceMapping> GetTripInvoiceMappings(IEnumerable<string> invoiceNumbers)
    {
      
      var dataSource = new DataSource();
      var invoicesXml = new XDocument( "invoices", 
        invoiceNumbers.Select(i=> new XElement("invoice", i)));

      var cmd = new SqlCommand
      {
        CommandText = Queries.InvoiceMappings,
        CommandType = CommandType.Text
      };
      cmd.Parameters.AddWithValue("@InvoincesString", invoicesXml.ToString());
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

      public static string SaveCommissions
      {
        get
        {
          return string.Empty;
        }
      }

      public static string InvoiceMappings
      {
        get
        {
          return @"
            DECLARE @Invoinces XML
            SET @Invoices = CAST( @InvoicesString AS XML )

            ;WITH [Invoices] AS
            (
              SELECT [T].[C].value('.', 'varchar(20)') AS [InvoiceNumber]
              FROM @Invoices.nodes('//invoice') AS T(C)
            )
            SELECT [cTripNumber] AS [TripNumber]
                 , [cProNumber] AS [InvoiceNumber]
                 , MAX( [dBillDate] ) AS [BillDate]
            FROM [Truckwin_TDPD_Access]...[BillingHistory] [BH]
              INNER JOIN [Invoices] [I]
                ON ( [I].[InvoiceNumber] = [BH].[cProNumber] )
            GROUP BY [cTripNumber]
                 , [cProNumber]
             
          ";
        }
      }
    }

    
  }
}

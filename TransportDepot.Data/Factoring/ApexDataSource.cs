using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Models.Factoring;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Data;

namespace TransportDepot.Data.Factoring
{
  public class ApexDataSource
  {
    public IEnumerable<string> GetExistingPayments(IEnumerable<FactoringPayment> payments)
    {
      var paymentsXml = GetPaymentsXml(payments);
      var ds = new DataSource();
      IEnumerable<string> dupes = new List<string>();
      using (var cn = new SqlConnection(ds.ConnectionString))
      {
        dupes = this.GetDuplicateInvoices(cn, paymentsXml);
      }
      if (dupes.Count() == 0)
      {
        return new List<string>();
      }
      return dupes;
    }
    public IEnumerable<string> Save(IEnumerable<FactoringPayment> payments)
    {
      
      var paymentsXml = GetPaymentsXml(payments);
      
      var ds  = new DataSource();
      IEnumerable<string> dupes = new List<string>();
      using(var cn = new SqlConnection(ds.ConnectionString))
      {
        dupes = this.GetDuplicateInvoices(cn, paymentsXml);
        payments = payments.Where(p => !dupes.Contains(p.InvoiceNumber));
        this.SavePayments(cn, payments);
      }
      
      return dupes;
    }

    private void SavePayments(SqlConnection cn, IEnumerable<FactoringPayment> payments)
    {
      var paymentsXml = this.GetPaymentsXml(payments);
      var saveCount = 0;
      using (var cmd = new SqlCommand(SavePaymentsSql, cn))
      {
        cmd.Parameters.AddWithValue("@ApexPaymentsString", paymentsXml.ToString());
        if (cn.State != ConnectionState.Open)
        {
          cn.Open();
        }
        saveCount = cmd.ExecuteNonQuery();
      }
    }
    
    private XDocument GetPaymentsXml(IEnumerable<FactoringPayment> payments)
    {
      var glDeparmtment = this.GetSetting("TransportDepot.AccountsReceivable.Apex.GLDeparment");
      var glAccount = this.GetSetting("TransportDepot.AccountsReceivable.Apex.GLAccount");
      var glArAccount = this.GetSetting("TransportDepot.AccountsReceivable.Apex.GLArAccount");
      var paymentsXml = new XDocument(new XElement("apexPayments",
        new XAttribute("glDepartment", glDeparmtment),
        new XAttribute("glAccount", glAccount),
        new XAttribute("glArAccount", glArAccount),
        payments.Select(p => new XElement("payment",
          new XAttribute("invoiceNumber", p.InvoiceNumber),
          new XAttribute("amount", p.Amount),
          new XAttribute("effectiveDate", p.EffectiveDate.ToShortDateString()),
          new XAttribute("comments",  this.GetComments(p))))));
      return paymentsXml;
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


    private string GetComments(FactoringPayment p)
    {
      var schedule = string.Empty;
      if (p.Schedule < 1)
      {
        schedule = string.Format("['{0}' - No Schedule]", p.Type);
      }
      else
      {
        schedule = p.Schedule.ToString();
      }
      return string.Format("Schedule {0} - Automatically Imported {1}",
        schedule,
        DateTime.Today.ToString("MM/dd/yyyy"));
    }

    

    private IEnumerable<string> GetDuplicateInvoices(SqlConnection cn, XDocument paymentsXml)
    {
      var invoiceTable = new DataTable();
      using (var cmd = new SqlCommand(DuplicatesSql, cn))
      using(var adapter = new SqlDataAdapter(cmd))
      {
        cmd.Parameters.AddWithValue("@ApexPaymentsString", paymentsXml.ToString());
        adapter.Fill(invoiceTable);  
      }
      var invoiceDupes = invoiceTable.AsEnumerable()
        .Select(i => i.Field<string>("InvoiceNumber")).ToList();
      return invoiceDupes;
    }

    private const string DuplicatesSql = @"

            DECLARE @ApexPayments xml
            SET @ApexPayments = @ApexPaymentsString

            ;WITH [Payments] AS
            (
              SELECT [T].[c].value('./@invoiceNumber', 'varchar(20)') AS [InvoiceNumber]
                  ,  [T].[c].value('./@effectiveDate', 'datetime')    AS [EffectiveDate]
                  ,  [T].[c].value('./@amount', 'smallmoney')         AS [Amount]
                  ,  [T].[c].value('../@glAccount', 'varchar(20)')    AS [GLAccount]
                  ,  [T].[c].value('../@glArAccount', 'varchar(20)')  AS [GLArAccount]
                  ,  [T].[c].value('../@glDepartment', 'varchar(20)') AS [GLDepartment]
                  ,  [T].[c].value('./@comments', 'varchar(20)')      AS [Comments]
              FROM @ApexPayments.nodes('//payment') AS T(C)
            )

                        
            SELECT [P].[InvoiceNumber] /* (object) */
            FROM [Payments] AS [P] LEFT JOIN [dbo].[ArAging]  AS [G] 
              ON ( [P].[InvoiceNumber] = G.cPronumber )
            WHERE ( ( [G].[cProNumber] IS NULL ) OR ( [P].[Amount] != [G].[cuBalanceDue] ) )
              OR EXISTS
              (
                SELECT * 
                FROM [dbo].[ArPayment] [A]
                WHERE ( [A].[cProNumber] = [P].[InvoiceNumber] )
              )
            ";

    private const string SavePaymentsSql = @"

            DECLARE @ApexPayments xml
            SET @ApexPayments = @ApexPaymentsString

           ;WITH [Payments] AS
            (
              SELECT [T].[c].value('./@invoiceNumber', 'varchar(20)') AS [InvoiceNumber]
                  ,  [T].[c].value('./@effectiveDate', 'datetime')    AS [EffectiveDate]
                  ,  [T].[c].value('./@amount', 'smallmoney')         AS [Amount]
                  ,  [T].[c].value('../@glAccount', 'varchar(20)')    AS [GLAccount]
                  ,  [T].[c].value('../@glArAccount', 'varchar(20)')  AS [GLArAccount]
                  ,  [T].[c].value('../@glDepartment', 'varchar(20)') AS [GLDepartment]
                  ,  [T].[c].value('./@comments', 'varchar(1000)')    AS [Comments]
              FROM @ApexPayments.nodes('//payment') AS T(C)
            )

            INSERT INTO [dbo].[ArPayment] 
            ( 
                  [cPronumber]       
                , [niNumber]         
                , [cCustomerId]
                , [dDate]
                , [cuAmount]
                , [cDept]
                , [cAcct]
                , [mDescription]
                , [cArAcct]
                , [cReference]
                , [bPosted]
                , [cTripNumber]
                , [cPayAdj]
                , [nlOrderNumber]
                , [bACHTransfer] 
            ) 

            
            SELECT 
                  [P].[InvoiceNumber]      AS [cProNumber] /* (object) */
                , 1 AS [niNumber]      
                , [G].[cCustomerId]   /* (ArAging or BillingHistory) */
                , [P].[EffectiveDate]      AS [dDate]    
                , [P].[Amount]             AS [cuAmount] 
                , [P].[GLDepartment]       AS [cDept]    
                , [P].[GLAccount]          AS [cAcct]
                , [P].[Comments]           AS [mDescription]
                , [P].[GLArAccount]        AS [cArAcct]
                , 'Apex Dwnld'             AS [cReference] /* (object) Schedule Number */
                , 0                        AS [bPosted] 
                , [G].[cTripNumber]         
                , 'P'                      AS [cPayAdj]
                , 0                        AS [nlOrderNumber] 
                , 0                        AS bACHTransfer  
            FROM [Payments] AS [P] INNER JOIN [dbo].[ArAging]  AS [G] 
              ON ( [P].[InvoiceNumber] = G.cPronumber )
            WHERE ( [P].[Amount] = [G].[cuBalanceDue] ) 
              AND NOT EXISTS
              (
                SELECT * 
                FROM [dbo].[ArPayment] [A]
                WHERE ( [A].[cProNumber] = [P].[InvoiceNumber] )
              )            
    ";
  }
}

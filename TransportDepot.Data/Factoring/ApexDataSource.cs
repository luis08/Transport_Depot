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
      
      var fileName = string.Format(@"C:\Test\ApexPayments\payments_{0:yyyyMMdd_mmss}.xml", DateTime.Now);
      var paymentsXml = GetPaymentsXml(payments);
      
      var ds  = new DataSource();
      IEnumerable<string> dupes = new List<string>();
      using(var cn = new SqlConnection(ds.ConnectionString))
      {
        dupes = this.GetDuplicateInvoices(cn, paymentsXml);
        payments = payments.Where(p => !dupes.Contains(p.InvoiceNumber));
        this.SavePayments(cn, payments);
      }
      paymentsXml.Save(fileName);
      return dupes;
    }

    private void SavePayments(SqlConnection cn, IEnumerable<FactoringPayment> payments)
    {
      var paymentsXml = this.GetPaymentsXml(payments);
      using (var cmd = new SqlCommand(SavePaymentsSql, cn))
      {
        cmd.Parameters.AddWithValue("@ApexPaymentsString", paymentsXml.ToString());
        if (cn.State != ConnectionState.Open)
        {
          cn.Open();
        }
        cmd.ExecuteNonQuery();
      }
    }

    private XDocument GetPaymentsXml(IEnumerable<FactoringPayment> payments)
    {
      var paymentsXml = new XDocument(new XElement("apexPayments",
        payments.Select(p => new XElement("payment",
          new XAttribute("invoiceNumber", p.InvoiceNumber),
          new XAttribute("amount", p.Amount),
          new XAttribute("effectiveDate", p.EffectiveDate.ToShortDateString()),
          new XAttribute("comments", string.Format("Schedule: {0}", p.Schedule))))));
      return paymentsXml;
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
            FROM [Payments] AS [P] LEFT JOIN [Truckwin_TDPD_Access]...[ArAging]  AS [G] 
              ON ( [P].[InvoiceNumber] = G.cPronumber )
            WHERE ( ( [G].[cProNumber] IS NULL ) OR ( [P].[Amount] = [G].[cuBalanceDue] ) )

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
                  ,  [T].[c].value('./@comments', 'varchar(20)')      AS [Comments]
              FROM @ApexPayments.nodes('//payment') AS T(C)
            )

            INSERT INTO [Truckwin_TDPD_Access]...[ArPayment] 
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
                  [P].[InvoiceNumber] /* (object) */
                , 1 AS [niNumber]      
                , [G].[cCustomerId]   /* (ArAging or BillingHistory) */
                , [P].[EffectiveDate]      AS [dDate]    
                , [P].[Amount]             AS [cuAmount] 
                , [P].[GLDepartment]       AS [cDept]    
                , [P].[GLAccount]          AS [cAcct]
                , 'Imported Automatically' AS [mDescription]
                , [P].[GLArAccount]        AS [cArAcct]
                , [P].[Comments]           AS [cReference] /* (object) Schedule Number */
                , 0                        AS [bPosted] 
                , [G].[cTripNumber]         
                , 'P'                      AS [cPayAdj]
                , 0                        AS [nlOrderNumber] 
                , 0                        AS bACHTransfer  
            FROM [Payments] AS [P] INNER JOIN [Truckwin_TDPD_Access]...[ArAging]  AS [G] 
              ON ( [P].[InvoiceNumber] = G.cPronumber )
            WHERE ( [P].[Amount] = [G].[cuBalanceDue] ) 
            
    ";
  }
}

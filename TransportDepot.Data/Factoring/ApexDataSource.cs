using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Models.Factoring;

namespace TransportDepot.Data.Factoring
{
  public class ApexDataSource
  {
    public void Save(IEnumerable<FactoringPayment> payments)
    {

    }


    private const string SavePaymentsSql = @"
            DECLARE @InsertedInvoices TABLE 
            (
              [cProNumber] VARCHAR( 50 ) NOT NULL,
              [cuAmount]   SMALLMONEY NOT NULL DEFAULT 0.0
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
            OUTPUT INSERTED.[cProNumber]
                 , INSERTED.[cuAmount]
            INTO @InsertedInvoices
            INNER JOIN [Truckwin_TDPD_Access]...[ArAging] AS [G] 
              ON ( INSERTED.[cProNumber] = [G].[cPronumber] )
            SELECT 
                  [P].[Invoice_Number] (object)
                , 1 AS [niNumber]      
                , [G].[cCustomerId]    (ArAging or BillingHistory)
                , Effective_Date       (object)
                , P.Invoice_Amount     (object)
                , cDept                Fixed value
                , cAcct                Fixed value
                , 'Imported Automatically' AS mDescription
                , cArAcct              Fixed value
                , cReference           
                , 0 AS bPosted
                , G.ctripnumber         Billing History
                , 'P' AS cPayAdj
                , 0 AS nlOrderNumber 
                , 0 AS bACHTransfer  
            FROM [Truckwin_TDPD_Access]...[Apex_Payment] AS [P] INNER JOIN [ArAging] AS [G] 
              ON ( P.Invoice_Number = G.cPronumber )
            WHERE ( [P].[Invoice_Amount] = [G].[cuBalanceDue] ) 
    ";
  }
}

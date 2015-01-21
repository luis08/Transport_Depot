
namespace TransportDepot.Data.Dispatch
{
  static class Queries
  {
    public static string Dispatchers
    {
      get
      {
        return @"

              SELECT [DA].[Initials]
                   , [DA].[VendorID]
                   , [V].[cName] AS [Name]
              FROM [Truckwin_TDPD_Access]...[ApVendor] [V]  
                INNER JOIN [dbo].[DispatcherAgent] [DA]
                  ON ( [DA].[VendorID] = [V].[cId] )
              ";
      }
    }
    public static string CommissionsCTE
    {
      get 
      {
        return @"
              ;WITH [PaidCommissions] AS
              (
                SELECT 
                    [BH].[cProNumber]      AS [ArInvoiceNumber]
                  , [PH].[cVendorID]       AS [DispatcherId]
                  , [PH].[cuInvoiceTotal]  AS [CommissionTotal]
                  , [ATH].[cTripnumber]    AS [TripNumber]
                  , [PH].[cInvoiceNo]      AS [APInvoiceNumber]
                  , [BH].[dBillDate]       AS [BillDate]
                  , [PH].[dCrntPymtDate]   AS [CommissionPaymentDate]
                  , [ATH].[cLessorId1]     AS [LessorID]
                  , [PH].[mInvoiceDesc]    AS [CommisionDescription]
                  , [BH].[cOrigState]      AS [ShipState]
                  , [BH].[cDestState]      AS [UnloadState]
                  , [C].[cName]            AS [CustomerName]
                  , [R].[cuSubMainRevenue] AS [InvoiceAmount]
                FROM [Truckwin_TDPD_Access]...[BillingHistory]           [BH]  
                  INNER JOIN [Truckwin_TDPD_Access]...[Customer]         [C]   ON ( [BH].[cCustomerID] = [C].[cID] )
                  LEFT JOIN  [Truckwin_TDPD_Access]...[AssignedToHist]   [ATH] ON ( [BH].[cTripnumber] = [ATH].[cTripnumber] )
                  INNER JOIN [Truckwin_TDPD_Access]...[ApPayableHistory] [PH]  ON ( [PH].[cInvoiceNo]  = [BH].[cTripNumber] )
                  INNER JOIN [Truckwin_TDPD_Access]...[Revenue]          [R]   ON ( [R].[cProNumber] = [BH].[cProNumber] )
                WHERE ([PH].[bReverse]  = 0 )  
              ), [UnpaidCommissions] AS
              (
                SELECT 
                    [BH].[cProNumber]      AS [ArInvoiceNumber]
                  , [PH].[cVendorID]       AS [DispatcherId]
                  , [PH].[cuInvoiceTotal]  AS [CommissionTotal]
                  , [ATH].[cTripnumber]    AS [TripNumber]
                  , [PH].[cInvoiceNo]      AS [APInvoiceNumber]
                  , [BH].[dBillDate]       AS [BillDate]
                  , [PH].[dDueDate]        AS [CommissionPaymentDate]
                  , [ATH].[cLessorId1]     AS [LessorID]
                  , [PH].[mInvoiceDesc]    AS [CommisionDescription]
                  , [BH].[cOrigState]      AS [ShipState]
                  , [BH].[cDestState]      AS [UnloadState]
                  , [C].[cName]            AS [CustomerName]
                  , [R].[cuSubMainRevenue] AS [InvoiceAmount]
                FROM [Truckwin_TDPD_Access]...[BillingHistory]           [BH]  
                  INNER JOIN [Truckwin_TDPD_Access]...[Customer]         [C]   ON ( [BH].[cCustomerID] = [C].[cID] )
                  LEFT JOIN  [Truckwin_TDPD_Access]...[AssignedToHist]   [ATH] ON ( [BH].[cTripnumber] = [ATH].[cTripnumber] )
                  INNER JOIN [Truckwin_TDPD_Access]...[ApInvoice]        [PH]  ON ( [PH].[cInvoiceNo]  = [BH].[cTripNumber] )
                  INNER JOIN [Truckwin_TDPD_Access]...[Revenue]          [R]   ON ( [R].[cProNumber] = [BH].[cProNumber] )
              ), [AllCommissions] AS
              (
                  SELECT * 
                  FROM [UnpaidCommissions]
                  UNION ALL
                  SELECT * 
                  FROM [PaidCommissions]
              )


            ";
      }
    }
  }
}

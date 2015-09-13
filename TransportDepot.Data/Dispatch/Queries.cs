
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
              FROM [dbo].[ApVendor] [V]  
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
                FROM [dbo].[BillingHistory]           [BH]  
                  INNER JOIN [dbo].[Customer]         [C]   ON ( [BH].[cCustomerID] = [C].[cID] )
                  LEFT JOIN  [dbo].[AssignedToHist]   [ATH] ON ( [BH].[cTripnumber] = [ATH].[cTripnumber] )
                  INNER JOIN [dbo].[ApPayableHistory] [PH]  ON ( [PH].[cInvoiceNo]  = [BH].[cTripNumber] )
                  INNER JOIN [dbo].[Revenue]          [R]   ON ( [R].[cProNumber] = [BH].[cProNumber] )
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
                FROM [dbo].[BillingHistory]           [BH]  
                  INNER JOIN [dbo].[Customer]         [C]   ON ( [BH].[cCustomerID] = [C].[cID] )
                  LEFT JOIN  [dbo].[AssignedToHist]   [ATH] ON ( [BH].[cTripnumber] = [ATH].[cTripnumber] )
                  INNER JOIN [dbo].[ApInvoice]        [PH]  ON ( [PH].[cInvoiceNo]  = [BH].[cTripNumber] )
                  INNER JOIN [dbo].[Revenue]          [R]   ON ( [R].[cProNumber] = [BH].[cProNumber] )
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
    public static string MovingFreight
    {
      get
      {
        return @"

        ;WITH [MovingFreight] AS
        (
          SELECT 
                   [AT].[cTripnumber] AS [TripNumber]
                 , CASE 
                      WHEN ( [bLessorTrip] != 0 ) THEN [T].[cTractorId]
                      ELSE ( [AT].[cCarrierID] )
                   END AS [TractorID]
                 , [AT].[cTrailer1Id] AS [TrailerID]
                 , [AT].[cDriver1Id]  AS [Driver1_EmployeeID]
                 , [AT].[cDriver2Id]  AS [Driver2_EmployeeID]
                 ,  [E].[cFirst] AS [Driver1_FirstName] 
                 ,  [E].[cLast]  AS [Driver1_LastName] 
                 ,  [E].[cCellularPager] AS [Driver1_Phone]
                 , [E2].[cFirst] AS [Driver2_FirstName] 
                 , [E2].[cLast]  AS [Driver2_LastName] 
                 , [E2].[cCellularPager] AS [Driver2_Phone]
                 , [TN].[cOrigAddress] AS [FromAddress]
                 , [TN].[cOrigCity]  AS [FromCity]
                 , [TN].[cOrigState] AS [FromState]
                 , [TN].[cOrigZip]   AS [FromZip]
                 , [TN].[cDestAddress] AS [ToAddress]
                 , [TN].[cDestCity]  AS [ToCity]
                 , [TN].[cDestState] AS [ToState]
                 , [TN].[cDestZip]   AS [ToZip]
                 , [TN].[dShipDate]  AS [PickUp]
                 , [TN].[dDeliveryDate] AS [Delivery]
                 , [TN].[cDestCity] AS [City]
                 , [TN].[cCustomerID] AS [CustomerID]
                 ,  [E].[cEmployeeId] AS [Driver1ID]
                 , [TN].[cDispatcherInit] AS [DispatcherInitials]
          FROM [dbo].[AssignedTo] [AT]
            LEFT JOIN  [dbo].[PrEmployee] AS [E]  ON [AT].[cDriver1Id] = [E].[cEmployeeId]
            LEFT JOIN  [dbo].[PrEmployee] AS [E2] ON [AT].[cDriver2Id] = [E2].[cEmployeeId]
            LEFT JOIN  [dbo].[Tractor]    AS [T]  ON [AT].[cTractorId] = [T].[cTractorId]
            INNER JOIN [dbo].[TripNumber] AS [TN] ON [AT].[cTripnumber] = SUBSTRING( [TN].[cTripnumber], 1, 9 )
        )


        SELECT   [M].[TripNumber]
               , [M].[CustomerID]
               , [M].[TractorID]
               , [M].[TrailerID]
               , [M].[Driver1_FirstName]
               , [M].[Driver1_LastName]
               , [M].[Driver2_FirstName]
               , [M].[Driver2_LastName]       
               , [M].[PickUp]
               , [M].[FromAddress]
               , [M].[FromCity] 
               , [M].[FromState] 
               , [M].[FromZip]
               , [M].[ToAddress]
               , [M].[ToCity]
               , [M].[ToState]
               , [M].[ToZip]           
               , [M].[Delivery]
               , [M].[City]
               , [C].[cName]  AS [Customer_Name]
               , [C].[cPhone] AS [Customer_Phone]
               , [M].[Driver1_Phone]
               , [M].[Driver2_Phone]
               , [M].[DispatcherInitials]
               , [DI].[cFuelCardNumber1] [Driver1_Card]
        FROM [MovingFreight] AS [M]
            INNER JOIN [dbo].[Customer]   AS [C]  ON [M].[CustomerID] = [C].[cID]
            INNER JOIN [dbo].[DriverInfo] AS [DI] ON [M].[Driver1_EmployeeID] = [DI].[cDriverID] 

        ";
      }
    }
  }
}

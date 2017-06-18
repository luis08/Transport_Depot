using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Data.DB
{
  static class AgingQueries
  {
    public static string CTE = 
    @" [Aging] AS 
      (
        SELECT
              [cCustomerId]  AS [CustomerID]
            , [cPronumber]  AS [Invoice_Number]
            , [dDateOfInvoice]  AS [Invoice_Date]
            , [ctripnumber]  AS [Trip_Number]
            , COALESCE([cCustomerRef], '')  AS [Customer_Reference]
            , [cuBalanceDue]  AS [Balance_Due]
        FROM [dbo].[ArAging] [A]
      )
    ";

    public static string AgingForCustomerId
    {
      get
      {
        var topQuery = @"
        DECLARE @CustomerIds XML
        SET @CustomerIds = CAST( @CustomerIdsXmlString AS XML )
        
        ;WITH [CustomerIds] AS
        (
          SELECT C.R.value('.', 'varchar(8)') As [ID]
          FROM @CustomerIds.nodes('//id') AS [C](R)
        )";
        var bottomQuery = @"
        SELECT [A].*
        FROM [Aging] [A]
           INNER JOIN  [CustomerIds] [I] 
             ON ( [A].[CustomerID] = [I].[ID] )
        ";
        return string.Concat(
          string.Concat(topQuery, "        , ", AgingQueries.CTE),
          Environment.NewLine,
          bottomQuery);
      }
    }
  }
}

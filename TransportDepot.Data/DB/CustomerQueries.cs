using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Data.DB
{
  static class CustomerQueries
  {
    static string CustomersCTE = @"[Customers] AS
      (
          SELECT
               [cID] AS [ID]
              ,[cName] AS [Customer_Name]
              ,[bBillTo] AS [Is_BillTo]
              ,[bOrigin] AS [Is_Origin]
              ,[bDestination] AS [Is_Destination]
              ,[bInbound] AS [Is_Inbound]
              ,[bOutBound] AS [Is_Outbound]
              ,[bContract] AS [Is_Contract]
              ,[cAddress] AS [Street_Address]
              ,[cCity] AS [City]
              ,[cStateRegion] AS [State]
              ,[cZipPostal] AS [Zip]
              ,[cCountry] AS [Country]
              ,[cPhone] AS [Phone]
              ,[cFax] AS [Fax]
              ,[cContactPersons] AS [Contact_Persons]
              ,[cOfficeHour] AS [Office_Hours]
              ,[cFirmNote] AS [Firm_Note]
              ,[bRateByDestState] AS [Has_Rate_By_Destination_State]
              ,[bRateByDestCity] AS [Has_Rate_By_Destination_City]
              ,[bRateByOrigState] AS [Has_Rate_By_Origin_State]
              ,[bRateByOrigCity] AS [Has_Rate_By_Origin_City]
              ,[bChargeGST] AS [Has_ChargeGST]
              ,[cuCreditLimit] AS [Credit_Limit]
              ,[bCanadianFunds] AS [Uses_Canadian_Funds]
              ,[bEDI] AS [Uses_EDI]
              ,[bConsolidated] AS [Is_Consolidated]
              ,[mDriverInstruct] AS [Driver_Instructions]
              ,COALESCE( [cLocation], '' )  AS [Location]
              ,[cuStopRate] AS [Stop_Rate]
              ,[bRequirePONumber] AS [Requires_PO_Number]
              ,CAST( COALESCE( [niCreditLimitDays], 0 ) As INT ) AS [Credit_Limit_Days]
              ,[cCustomsBrokerID] AS [Customs_Broker_ID]
              ,[bInactive] AS [Is_Inactive]
              ,[bWebAccess] AS [Has_Web_Access]
              ,[cWebPassword] AS [Web_Password]
              ,[cEmail] AS [Email]
              ,[cCustomField] AS [Custom_Field]
              ,[bRequireBOLNumber] AS [Requires_BOL_Number]
        FROM [dbo].[Customer] [C]
      )
      
    ";

    public static string ForMenuItems
    {
      get { return "SELECT [ID], ,[cName] AS [Customer_Name] FROM [dbo].[Customer] [C] WHERE ( [bInactive] = 0  )"; }
    }


    public static string PagedById
    {
      get
      {
        var bottomQuery = @"

        SELECT * 
        FROM
        (
          SELECT ROW_NUMBER() OVER (ORDER BY [ID]) AS [RowNumber]
                , *
          FROM [Customers]
        ) AS [C]
        WHERE [C].[RowNumber] BETWEEN @FromRow AND @ToRow
        
      ";
        var fullQuery = string.Concat(
          "; WITH ", CustomerQueries.CustomersCTE,
          Environment.NewLine,
          bottomQuery);
        return fullQuery;
      }
    }

    public static string PagedByName
    {
      get
      {
        var bottomQuery = @"

        SELECT * 
        FROM
        (
          SELECT ROW_NUMBER() OVER (ORDER BY [Customer_Name]) AS [RowNumber]
                , *
          FROM [Customers]
        ) AS [C]
        WHERE [C].[RowNumber] BETWEEN @FromRow AND @ToRow
        
      ";
        var fullQuery = string.Concat(
          "    ;WITH ", CustomerQueries.CustomersCTE,
          Environment.NewLine,
          bottomQuery);
        return fullQuery;
      }
    }


    public static string ForIds
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
        SELECT [C].*
        FROM [Customers] [C]
           INNER JOIN  [CustomerIds] [I] 
             ON ( [C].[ID] = [I].[ID] )
        ";
        return string.Concat(
          string.Concat(topQuery, "        , ", CustomerQueries.CustomersCTE),
          Environment.NewLine,
          bottomQuery);
      }
    }
  }
}

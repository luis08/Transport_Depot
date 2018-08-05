using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Data.DB
{
  static class TractorQueries
  {
    public static string UpdateTractors = @"
      UPDATE [dbo].[Tractor] 
        SET
           [bActive] = @Active
         , [cComment] = @Comments
         , [bTractorAssigned] = @HasTripAssigned
         , [dInspectionDate] = @InspectionDue
         , [bLessorTruck] = @IsLessorTruck
         , [cLessorOwner] = @LessorOwnerName
         , [cLicensePlate] = @LicensePlate
         , [cInsuranceName] = @InsuranceName
         , [cMake] = @Make
         , [cModel] = @Model
         , [cType] = @Type
         , [cTitle] = @Unit
         , [cSerial] = @VIN
         , [cYear] = @Year
      WHERE ( [cTractorId] = @Id )
      
      UPDATE [dbo].[Tractor_Qualification]
        SET 
           [Has_W9] = @HasW9
         , [Lease_Agreement_Due] = @LeaseAgreementDue
         , [Registration_Expiration]= @RegistrationExpiration
         , [Insurance_Policy_Expiration] = @InsuranceExpiration
      WHERE ( [ID] = @Id )
         
    ";
    public static string Tractors = @"
      SELECT
           [T].[bActive] AS [Active]
         , [T].[cComment] AS [Comments]
         , [T].[bTractorAssigned] AS [HasTripAssigned]
         , [Q].[Has_W9] AS [HasW9]
         , [T].[cTractorId] AS [Id]
         , [T].[dInspectionDate] AS [InspectionDue]
         , [T].[bLessorTruck] AS [IsLessorTruck]
         , [Q].[Lease_Agreement_Due] AS [LeaseAgreementDue]
         , COALESCE( [L].[cName], '' ) AS [LessorOwnerName]
         , [T].[cLicensePlate] AS [LicensePlate]
         , COALESCE( [M].[Last_Maintenance], '20010101' )  AS [LastMaintenance]
         , [T].[cMake] AS [Make]
         , [T].[cModel] AS [Model]
         , [Q].[Registration_Expiration] AS [RegistrationExpiration]
         , [Q].[Insurance_Policy_Expiration] AS [InsuranceExpiration]
         , [T].[cInsuranceName] AS [InsuranceName]
         , [T].[cType] AS [Type]
         , [T].[cTitle] AS [Unit]
         , [T].[cSerial] AS [VIN]
         , [T].[cYear] AS [Year]
      FROM [dbo].[Tractor] AS [T]
       INNER JOIN [dbo].[Tractor_Qualification] AS [Q]
         ON ( [T].[cTractorID] = [Q].[ID] )
       LEFT JOIN [dbo].[RsLessor] [L]
         ON ( [T].[cLessorOwner] = [L].[cName] )
       LEFT JOIN 
       ( 
          SELECT [cTractorID] AS [ID]
               , MAX( COALESCE( [dDateDone], '20010101' ) )AS [Last_Maintenance]
          FROM [dbo].[MaintenanceTractorLog] 
          GROUP BY [cTractorID] 
       ) AS [M] ON ( [M].[ID] = [T].[cTractorID] )
    ";

    public static string TractorSanityQuery = @"
      INSERT INTO [dbo].[Tractor_Qualification] 
      ( 
          [ID] 
        , [Self_Insured] 
        , [Has_W9]
        , [Lease_Agreement_Due]
        , [Registration_Expiration]
        , [Insurance_Policy_Expiration]
        , [DOT_Inspection_Expiration]
        , [Active]
      )
      SELECT [cTractorID] AS [ID]
           , 0 AS [Self_Insured]
           , 0 AS [Has_W9]
           , dbo.DefaultDate() AS [Lease_Agreement_Due]
           , dbo.DefaultDate() AS [Registration_Expiration]
           , dbo.DefaultDate() AS [Insurance_Policy_Expiration]
           , dbo.DefaultDate() AS [DOT_Inspection_Expiration]
           , 0 AS [Active]
      FROM [dbo].[Tractor] AS [T]
      WHERE NOT EXISTS
      (
        SELECT * 
        FROM [dbo].[Tractor_Qualification] [T1]
        WHERE [T1].[ID] = [T].[cTractorID]
      )
    ";

    public static string TractorMaintenance = @"
      DECLARE @filter XML
      SET @filter = CAST(@FilterString AS XML)

      ;WITH [Tractors] AS (
          SELECT [T].[C].value('.', 'varchar(8)') AS [TractorId]
          FROM @filter.nodes('//tractorId') AS T(c)
      ), [filter] AS 
      (
	      SELECT [T].[c].value('./@type', 'varchar(5)') AS [type],
		         [T].[c].value('./@from', 'datetime') AS [from],
		         [T].[c].value('./@to', 'datetime') AS [to],
		         [T].[c].value('./@description', 'varchar(300)') AS [description]
	      FROM @filter.nodes('//filter') AS T(c)
      )

      SELECT [M].[Id]
         , [M].[TractorId]
         , [T].[cSerial] AS [VIN]
         , [T].[cTitle] AS [Unit]
         , [M].[Type]
         , [C].[cDescription] AS [StandardDescription]
	       , [M].[PerformedDate]
	       , [M].[Mileage]
         , [M].[Description]
      FROM [dbo].[TractorMaintenance] [M]
          INNER JOIN [dbo].[MaintenanceTractorCode] [C] ON [M].[Type] = [C].[cCode]
          INNER JOIN [dbo].[Tractor] [T] ON [M].[TractorId] = [T].[cTractorId]
          CROSS JOIN [filter] [F]
      WHERE ([f].[type] IS NULL OR ([M].[Type] = [F].[type]))
        AND ([f].[from] IS NULL OR ([M].[PerformedDate] >= [f].[from]))
        AND ([f].[to] IS NULL OR ([M].[PerformedDate] < [f].[to]))
        AND (NOT EXISTS(SELECT * FROM [Tractors]) OR ([M].[TractorId] IN (SELECT [tractorId] FROM [tractors])))
        AND ([f].[description] IS NULL OR ([M].[Description] LIKE '%' + [f].[description] + '%'))
    ";
  }
}

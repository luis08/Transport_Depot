

namespace TransportDepot.Data.DB
{
  static class TrailerQueries
  {
    public static string UpdateTrailers = @"
      UPDATE [dbo].[Trailer] 
        SET
           [bActive] = @Active
         , [cComment] = @Comments
         , [bTrailerAssigned] = @HasTripAssigned
         , [dInspectionDate] = @InspectionDue
         , [bLessorTrailer] = @IsLessorTrailer
         , [cOwnerName] = @LessorOwnerName
         , [cLicensePlate] = @LicensePlate
         , [cMake] = @Make
         , [cModel] = @Model
         , [cType] = @Type
         , [cTitle] = @Unit
         , [cSerialNumber] = @VIN
         , [cYear] = @Year
      WHERE ( [cTrailerId] = @Id )
      
      UPDATE [dbo].[Trailer_Qualification]
        SET [Registration_Expiration]= @RegistrationExpiration
      WHERE ( [ID] = @Id )
    ";

    public static string Trailers = @"
      SELECT
           [T].[cTrailerId] AS [Id]  
         , [T].[bActive] AS [Active]
         , [T].[cComment] AS [Comments]
         , [T].[bTrailerAssigned] AS [HasTripAssigned]
         , [T].[dInspectionDate] AS [InspectionDue]
         , [T].[bLessorTrailer] AS [IsLessorTrailer]
         , COALESCE( [L].[cName], '' ) AS [LessorOwnerName]
         , [T].[cLicensePlate] AS [LicensePlate]
         , COALESCE( [M].[Last_Maintenance], '20010101' )  AS [LastMaintenance]
         , [T].[cMake] AS [Make]
         , [T].[cModel] AS [Model]
         , [Q].[Registration_Expiration] AS [RegistrationExpiration]
         , [T].[cType] AS [Type]
         , [T].[cTitle] AS [Unit]
         , [T].[cSerialNumber] AS [VIN]
         , [T].[cYear] AS [Year]
      FROM [dbo].[Trailer] AS [T]
       INNER JOIN [dbo].[Trailer_Qualification] AS [Q]
         ON ( [T].[cTrailerID] = [Q].[ID] )
       LEFT JOIN [dbo].[RsLessor] [L]
         ON ( [T].[cOwnerName] = [L].[cName] )
       LEFT JOIN 
       ( 
          SELECT [cTrailerID] AS [ID]
               , MAX( COALESCE( [dDateDone], '20010101' ) )AS [Last_Maintenance]
          FROM [dbo].[MaintenanceTrailerLog] 
          GROUP BY [cTrailerID] 
       ) AS [M] ON ( [M].[ID] = [T].[cTrailerID] )
    ";


    public static string TrailerSanityQuery = @"
      INSERT INTO [dbo].[Trailer_Qualification] ( [ID], [Registration_Expiration] )
      SELECT [cTrailerID] AS [ID], [dbo].[DefaultDate]() AS [Registration_Expiration]
      FROM [dbo].[Trailer] AS [T]
      WHERE NOT EXISTS
      (
        SELECT * 
        FROM [dbo].[Trailer_Qualification] [T1]
        WHERE [T1].[ID] = [T].[cTrailerID]
      )
    ";

    public static string TrailerMaintenance = @"
      DECLARE @filter XML
      SET @filter = CAST(@FilterString AS XML)

      ;WITH [Trailers] AS (
          SELECT [T].[C].value('.', 'varchar(8)') AS [TrailerId]
          FROM @filter.nodes('//trailerId') AS T(c)
      ), [filter] AS 
      (
	      SELECT [T].[c].value('./@type', 'varchar(5)') AS [type],
		         [T].[c].value('./@trailerId', 'varchar(8)') AS [trailerId],
		         [T].[c].value('./@from', 'datetime') AS [from],
		         [T].[c].value('./@to', 'datetime') AS [to],
		         [T].[c].value('./@description', 'varchar(300)') AS [description]
	      FROM @filter.nodes('//filter') AS T(c)
      )

      SELECT [M].[Id]
         , [T].[cSerialNumber] AS [VIN]
         , [T].[cTitle] AS [Unit]
	       , [M].[Type]
         , [C].[cDescription]
	       , [M].[PerformedDate]
	       , [M].[trailerId]
	       , [M].[Description]
      FROM [dbo].[TrailerMaintenance] [M]
          INNER JOIN [dbo].[MaintenanceTrailerCode] [C] ON [M].[Type] = [C].[cCode]
          INNER JOIN [dbo].[Trailer] [T] ON [M].[TrailerId] = [T].[cTrailerid]
          CROSS JOIN [filter] [F]
      WHERE ([f].[type] IS NULL OR ([M].[Type] = [F].[type]))
        AND ([f].[from] IS NULL OR ([M].[PerformedDate] >= [f].[from]))
        AND ([f].[to] IS NULL OR ([M].[PerformedDate] < [f].[to]))
        AND (NOT EXISTS(SELECT * FROM [Trailers]) OR ([M].[TrailerId] IN (SELECT [trailerId] FROM [Trailers])))
        AND ([f].[description] IS NULL OR ([M].[Description] LIKE '%' + [f].[description] + '%'))
    ";
  }
}


namespace TransportDepot.Data.Safety
{
  internal class TrailerQueries
  {
    public static string TrailerSafety = @"
      DECLARE @TrailerSafetyXml XML
      SET @TrailerSafetyXml = CAST( @TrailerSafetyXmlString AS XML ) 

      
      ;WITH [Trailers] AS (
          SELECT [T].[C].value('./@id', 'varchar(8)') AS [ID]
          FROM @TrailerSafetyXml.nodes('//trailer') AS T(c)
      )
      SELECT [T].[ID]
            ,[Registration_Expiration]
            ,[DOT_Inspection_Expiration]
      FROM [Trans].[dbo].[trailer_qualification] [Q]
      INNER JOIN [Trailers] [T] 
        ON [T].[ID] = [Q].[ID]

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

    public static string UpdateTrailers = @"
      UPDATE [dbo].[Trailer] 
        SET
           [bActive] = @Active
         , [cComment] = @Comments
         , [bTrailerAssigned] = @HasTripAssigned
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
           ,[DOT_Inspection_Expiration] = @InspectionDue
      WHERE ( [ID] = @Id )
    ";

  }
}

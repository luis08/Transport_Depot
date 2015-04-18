

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
      INSERT INTO [TDPD].[dbo].[Trailer_Qualification] ( [ID] )
      SELECT [cTrailerID] AS [ID]
      FROM [dbo].[Trailer] AS [T]
      WHERE NOT EXISTS
      (
        SELECT * 
        FROM [TDPD].[dbo].[Trailer_Qualification] [T1]
        WHERE [T1].[ID] = [T].[cTrailerID]
      )
    ";
  }
}


namespace TransportDepot.Data.Safety
{
  internal static class Queries
  {
    public static string ConfirmDriverExistence = @"
          DECLARE @TrackedDriversXml XML
          SET @TrackedDriversXml = CAST( @TrackedDriversXmlString AS XML )

          DECLARE @Drivers TABLE( [ID] VARCHAR(12) )

          INSERT INTO @Drivers
          SELECT Drvs.col.value('.', 'varchar(12)') AS [ID]
          FROM @TrackedDriversXml.nodes('//driver') Drvs(col)
          
          INSERT INTO [dbo].[Driver_Trackable] ( [ID] )
          SELECT [ID] FROM @Drivers [D]
          WHERE NOT EXISTS
          (
            SELECT * 
            FROM [dbo].[Driver_Trackable] [T]
            WHERE ( [T].[ID] = [D].[ID] )
          )

          INSERT INTO [dbo].[Driver_Qualification] ( [ID] )
          SELECT [ID] FROM @Drivers [D]
          WHERE NOT EXISTS
          (
            SELECT * 
            FROM [dbo].[Driver_Qualification] [Q]
            WHERE ( [Q].[ID] = [D].[ID] )
          )
      ";

    public static string DriversUntrack = @"
          DECLARE @UnTrackedDriversXml XML
          SET @UnTrackedDriversXml = CAST( @UnTrackedDriversXmlString AS XML )

          ;WITH [DriversToUntrack] AS
          (
            SELECT Drvs.col.value('.', 'varchar(12)') AS [ID]
            FROM @UnTrackedDriversXml.nodes('//driver') Drvs(col)
          )
          
          DELETE 
          FROM [dbo].[Driver_Trackable] 
          WHERE [ID] IN ( SELECT [ID] FROM [DriversToUntrack] )
      ";

    public static string UpdateDriverSafety = @"

        DECLARE @DriverSafetyXml XML
        SET @DriverSafetyXml = CAST( @DriverSafetyXmlString AS XML ) 

        DECLARE @DriverSafety TABLE
        (
           [ID] VARCHAR(12)
          ,[Active] BIT 
          ,[TrackDriver] BIT 
          ,[Has_Application] BIT 
          ,[Has_SODH] BIT 
          ,[Has_Drug_Test] BIT 
          ,[MVR_Expiration] DATETIME 
          ,[Has_Police_Record_Report] BIT 
          ,[Has_Previous_Employer_Form] BIT 
          ,[Has_Social_Security_Card] BIT 
          ,[Has_Driver_Agreements] BIT 
          ,[Has_W9] BIT 
          ,[Physical_Expiration] DATETIME 
          ,[Last_Valid_Log_Date] DATETIME 
          ,[Drivers_License_Expiration] DATETIME 
          ,[Annual_Violations_Cert_Expiration] DATETIME 
          ,[Comments] VARCHAR(MAX)
        )

        INSERT INTO @DriverSafety
        (
           [ID] 
          ,[Active] 
          ,[TrackDriver]  
          ,[Has_Application]  
          ,[Has_SODH]  
          ,[Has_Drug_Test]  
          ,[MVR_Expiration]  
          ,[Has_Police_Record_Report]  
          ,[Has_Previous_Employer_Form]  
          ,[Has_Social_Security_Card]  
          ,[Has_Driver_Agreements]  
          ,[Has_W9]  
          ,[Physical_Expiration]  
          ,[Last_Valid_Log_Date]  
          ,[Drivers_License_Expiration]  
          ,[Annual_Violations_Cert_Expiration]  
          ,[Comments] 
        )
        SELECT
              SD.col.value( './@id', 'VARCHAR(12)' ) AS [ID] 
            , SD.col.value( './@active', 'BIT' ) AS [Active]
            , SD.col.value( './@trackDriver', 'BIT' ) AS [TrackDriver]
            , SD.col.value( './@application', 'BIT' ) AS [Has_Application]
            , SD.col.value( './@onDutyHours', 'BIT' ) AS [Has_SODH]
            , SD.col.value( './@drugTest', 'BIT' ) AS [Has_Drug_Test]
            , SD.col.value( './@mVRExpiration', 'DATETIME' ) AS [MVR_Expiration]
            , SD.col.value( './@policeReport', 'BIT' ) AS [Has_Police_Record_Report]
            , SD.col.value( './@previousEmployerForm', 'BIT' ) AS [Has_Previous_Employer_Form]
            , SD.col.value( './@socialSecurity', 'BIT' ) AS [Has_Social_Security_Card]
            , SD.col.value( './@agreement', 'BIT' ) AS [Has_Driver_Agreements]
            , SD.col.value( './@w9', 'BIT' ) AS  [Has_W9]
            , SD.col.value( './@physicalExamExpiration', 'DATETIME' ) AS [Physical_Expiration]
            , SD.col.value( './@lastValidLogDate', 'DATETIME' ) AS [Last_Valid_Log_Date]
            , SD.col.value( './@driversLicenseExpiration', 'DATETIME' ) AS [Drivers_License_Expiration]
            , SD.col.value( './@annualCertification', 'DATETIME' ) AS [Annual_Violations_Cert_Expiration]
            , SD.col.value( './@comments', 'VARCHAR(MAX)' ) AS [Comments]
        FROM @DriverSafetyXml.nodes('//driver') AS SD(col)

        UPDATE [dbo].[DriverInfo]
          SET [dLicenseExpDate] = [S].[Drivers_License_Expiration]
            , [dPhysicalExpDate] = [S].[Physical_Expiration]
        FROM [dbo].[DriverInfo] [D]
          INNER JOIN @DriverSafety [S]
            ON ( [S].[ID] = [D].[cDriverID] )

        UPDATE [dbo].[PrEmployee]
          SET [bActive] = [S].[Active]
        FROM [dbo].[PrEmployee] [E]
          INNER JOIN @DriverSafety [S]
            ON ( [S].[ID] = [E].[cEmployeeId] )

        UPDATE [dbo].[Driver_Qualification]
            SET [Has_Application] = [S].[Has_Application]
              ,[Has_SODH] = [S].[Has_SODH]
              ,[Has_Drug_Test] = [S].[Has_Drug_Test]
              ,[Has_Police_Record_Report] = [S].[Has_Police_Record_Report] 
              ,[Has_Previous_Employer_Form] = [S].[Has_Previous_Employer_Form]
              ,[Has_Social_Security_Card] = [S].[Has_Social_Security_Card]
              ,[Has_Driver_Agreements] = [S].[Has_Driver_Agreements]
              ,[Has_W9] = [S].[Has_W9] 
              ,[MVR_Expiration] = [S].[MVR_Expiration]
              ,[Annual_Violations_Cert_Expiration] = [S].[Annual_Violations_Cert_Expiration]
              ,[Last_Valid_Log_Date] = [S].[Last_Valid_Log_Date]
              ,[Comments] = [S].[Comments]
        FROM [dbo].[Driver_Qualification] [Q]
          INNER JOIN @DriverSafety [S]
            ON ( [S].[ID] = [Q].[ID] )
      ";

    public static string DriversUntracked = @"
        SELECT   [E].[cEmployeeID] AS [ID]
                ,[E].[cLast] AS [Last_Name] 
                ,[E].[cFirst] AS [First_Name]
        FROM [dbo].[PrEmployee] [E]
        WHERE NOT EXISTS
        (
          SELECT * 
          FROM [dbo].[Driver_Trackable] [T]
          WHERE ( [T].[ID] = [E].[cEmployeeID] )
        ) AND ( COALESCE( [E].[bIncludeInDriverList], 0 ) != 0 )
          AND ( COALESCE( [E].[bActive], 0 ) != 0 )
        ";

    public static string DriverSafety = @"
        DECLARE @DriverSafetyXml XML
        SET @DriverSafetyXml = CAST( @DriverSafetyXmlString AS XML )
        
        ;WITH [Drivers] AS
        (
          SELECT Drvs.col.value('.', 'varchar(12)') AS [ID]
          FROM @DriverSafetyXml.nodes('//driver') Drvs(col)
        )
        
        SELECT   [Q].[ID]
                ,[E].[cLast] AS [Last_Name] 
                ,[E].[cFirst] AS [First_Name]
                ,[Q].[Has_Application]
                ,[Q].[Has_SODH]
                ,[Q].[Has_Drug_Test]
                ,[Q].[Has_Police_Record_Report]
                ,[Q].[Has_Previous_Employer_Form]
                ,[Q].[Has_Social_Security_Card]
                ,[Q].[Has_Driver_Agreements]
                ,[Q].[Has_W9]
                ,[Q].[MVR_Expiration]
                ,[Q].[Annual_Violations_Cert_Expiration]
                ,[Q].[Last_Valid_Log_Date]
                ,[Q].[Comments]
                ,[D].[dLicenseExpDate] AS [Drivers_License_Expiration]
                ,[D].[dPhysicalExpDate] AS [Physical_Expiration]
                ,[E].[bActive] AS [Active]
         
        FROM [dbo].[Driver_Qualification] [Q]
          INNER JOIN [dbo].[Driver_Trackable] [T]
            ON ( [Q].[ID] = [T].[ID] )
          INNER JOIN [dbo].[DriverInfo] [D]
            ON ( [Q].[ID] = [D].[cDriverID] )
          INNER JOIN [dbo].[PrEmployee] [E]
            ON ( [Q].[ID] = [E].[cEmployeeId] )
          INNER JOIN [Drivers] [SD]
            ON ( [Q].[ID] = [SD].[ID] )
      ";
  }
}

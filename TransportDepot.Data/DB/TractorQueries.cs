
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
      INSERT INTO [dbo].[Tractor_Qualification] ( [ID] )
      SELECT [cTractorID] AS [ID]
      FROM [dbo].[Tractor] AS [T]
      WHERE NOT EXISTS
      (
        SELECT * 
        FROM [dbo].[Tractor_Qualification] [T1]
        WHERE [T1].[ID] = [T].[cTractorID]
      )
    ";

    public static string LessorUpdate = @"
  
      DECLARE @lessorsXml XML
      SET @lessorsXml = @lessorsXmlString

      UPDATE [L]
      SET
            [cVendorType]           = [R].[C].value('./@VendorType', 'nvarchar(20)') 
          , [cName]                 = [R].[C].value('./@Name', 'nvarchar(40)') 
          , [cAddress]              = [R].[C].value('./@Address', 'nvarchar(160)') 
          , [cCity]                 = [R].[C].value('./@City', 'nvarchar(22)') 
          , [cState]                = [R].[C].value('./@State', 'nvarchar(2)') 
          , [cZip]                  = [R].[C].value('./@Zip', 'nvarchar(10)') 
          , [cCountry]              = [R].[C].value('./@Country', 'nvarchar(6)') 
          , [cPhone]                = [R].[C].value('./@Phone', 'nvarchar(40)') 
          , [cFax]                  = [R].[C].value('./@Fax', 'nvarchar(20)') 
          , [mContactPerson]        = [R].[C].value('./@ContactPerson', 'nvarchar(100)') 
          , [mOfficeHour]           = [R].[C].value('./@OfficeHours', 'ntext') 
          , [mComment]              = [R].[C].value('./@Comment', 'ntext') 
          , [b1099]                 = [R].[C].value('./@Has1099', 'bit') 
          , [ni1099Box]             = [R].[C].value('./@BoxNumberFor1099', 'smallint') 
          , [cFederalID]            = [R].[C].value('./@TaxId', 'nvarchar(11)') 
          , [cRsAcct]               = [R].[C].value('./@GlRsAccount', 'nvarchar(5)') 
          , [cExpDept]              = [R].[C].value('./@GlRsExpenseDepartment', 'nvarchar(3)') 
          , [cExpAcct]              = [R].[C].value('./@GlRsExpenseAccount', 'nvarchar(5)') 
          , [cMethod]               = [R].[C].value('./@Method', 'nvarchar(2)') 
          , [cuRate]                = [R].[C].value('./@Rate', 'money') 
          , [cu1099adj]             = [R].[C].value('./@Adjustments1099', 'money') 
          , [bDifferentAddress]     = [R].[C].value('./@HasDifferentAddress', 'bit') 
          , [cPaymentName]          = [R].[C].value('./@PaymentName', 'nvarchar(40)') 
          , [cPaymentAddress]       = [R].[C].value('./@PaymentAddress', 'nvarchar(160)') 
          , [cPaymentCity]          = [R].[C].value('./@PaymentCity', 'nvarchar(22)') 
          , [cPaymentState]         = [R].[C].value('./@PaymentState', 'nvarchar(2)') 
          , [cPaymentZip]           = [R].[C].value('./@PaymentZip', 'nvarchar(10)') 
          , [cPaymentCountry]       = [R].[C].value('./@PaymentCountry', 'nvarchar(6)') 
          , [bCanadianFunds]        = [R].[C].value('./@UsesCanadianFunds', 'bit') 
          , [dInsuranceExpiration]  = [R].[C].value('./@InsuranceExpiration', 'datetime') 
          , [bCarrier]              = [R].[C].value('./@IsCarrier', 'bit') 
          , [cFHWANo]               = [R].[C].value('./@McNumber', 'nvarchar(20)') 
          , [niDueDays]             = [R].[C].value('./@DueDays', 'smallint') 
          , [cuDeadheadRate]        = [R].[C].value('./@DeadheadRate', 'money') 

      FROM [dbo].[RsLessor] [L]
        INNER JOIN @lessorsXml.nodes('//lessor') AS [R]([C])
          ON [L].[cId] = [R].[C].value('./@id', 'varchar(12)')
    ";
  }
}

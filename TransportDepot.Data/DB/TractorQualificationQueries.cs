
namespace TransportDepot.Data.DB
{
  static class TractorQualificationQueries
  {
    public static string Select = @"

        SELECT [TQ].[ID]                            AS [Id]
            ,  [TQ].[Has_W9]                        AS [HasW9]
            ,  [TQ].[Lease_Agreement_Due]           AS [LeaseAgreementDue]
            ,  [TQ].[Registration_Expiration]       AS [RegistrationExpiration]
            ,  [TQ].[DOT_Inspection_Expiration]     AS [DotInspectionExpiration]
            ,  [TQ].[Safety_Comments]               AS [SafetyComments]
            ,  [TQ].[Unit_Number]                   AS [UnitNumber]
            ,  [TQ].[VIN]                           AS [Vin]
            ,  [TQ].[LessorId]                      AS [LessorId]
            ,  [TQ].[Active]                        AS [IsActive]
            ,  [TQ].[Make]                          AS [Make]
            ,  [TQ].[Model]                         AS [Model]
            ,  [TQ].[Year]                          AS [Year]
            ,  [TQ].[License_Plate]                 AS [LicensePlate]
            ,  [TQ].[Insurance_Company]             AS [InsuranceCompany]
            ,  [TQ].[Self_Insured]                  AS [IsSelfInsured]
        FROM [dbo].[Tractor_Qualification] [TQ]
    ";
  }
}

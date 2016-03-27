
namespace TransportDepot.Data.DB
{
  static class LessorQueries
  {
    public static string Select = @"
      SELECT
             [L].[cVendorType]            AS [VendorType]
          ,  [L].[cId]                    AS [Id]
          ,  [L].[cName]                  AS [Name]
          ,  [L].[cAddress]               AS [Address]
          ,  [L].[cCity]                  AS [City]
          ,  [L].[cState]                 AS [State]
          ,  [L].[cZip]                   AS [Zip]
          ,  [L].[cCountry]               AS [Country]
          ,  [L].[cPhone]                 AS [Phone]
          ,  [L].[cFax]                   AS [Fax]
          ,  [L].[mContactPerson]         AS [ContactPerson]
          ,  [L].[mOfficeHour]            AS [OfficeHours]
          ,  [L].[mComment]               AS [Comment]
          ,  [L].[b1099]                  AS [Has1099]
          ,  [L].[ni1099Box]              AS [BoxNumberFor1099]
          ,  [L].[cFederalID]             AS [TaxId]
          ,  [L].[cRsAcct]                AS [GlRsAccount]
          ,  [L].[cExpDept]               AS [GlRsExpenseDepartment]
          ,  [L].[cExpAcct]               AS [GlRsExpenseAccount]
          ,  [L].[cMethod]                AS [Method]
          ,  [L].[cuRate]                 AS [Rate]
          ,  [L].[cu1099adj]              AS [Adjustments1099]
          ,  [L].[bDifferentAddress]      AS [HasDifferentAddress]
          ,  [L].[cPaymentName]           AS [PaymentName]
          ,  [L].[cPaymentAddress]        AS [PaymentAddress]
          ,  [L].[cPaymentCity]           AS [PaymentCity]
          ,  [L].[cPaymentState]          AS [PaymentState]
          ,  [L].[cPaymentZip]            AS [PaymentZip]
          ,  [L].[cPaymentCountry]        AS [PaymentCountry]
          ,  [L].[bCanadianFunds]         AS [UsesCanadianFunds]
          ,  [L].[dInsuranceExpiration]   AS [InsuranceExpiration]
          ,  [L].[bCarrier]               AS [IsCarrier]
          ,  [L].[cFHWANo]                AS [McNumber]
          ,  [L].[niDueDays]              AS [DueDays]
          ,  [L].[cuDeadheadRate]         AS [DeadheadRate]

      FROM [dbo].[RsLessor] [L]
    ";
  }
}

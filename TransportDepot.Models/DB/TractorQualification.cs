
using System;
namespace TransportDepot.Models.DB
{
  /// <summary>
  /// The insurance expiration is already in the lessor table, which we use instead.
  /// </summary>
  public class TractorQualification
  {
    public string Id { get; set; }
    public bool HasW9 { get; set; }
    public DateTime LeaseAgreementDue { get; set; }
    public DateTime RegistrationExpiration { get; set; }
    public DateTime DotInspectionExpiration { get; set; }
    public string SafetyComments { get; set; }
    public string UnitNumber { get; set; }
    public string Vin { get; set; }
    public string LessorId { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public string Year { get; set; }
    public string LicensePlate { get; set; }
    public string InsuranceCompany { get; set; }
    public bool IsSelfInsured { get; set; }
    public bool IsActive { get; set; }

  }
}

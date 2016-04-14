using System;

namespace TransportDepot.Models.Safety
{
  public class Tractor
  {
    public string Id { get; set; }
    public string Unit { get; set; }
    public string VIN { get; set; }
    public string LessorOwnerName { get; set; }
    public DateTime LeaseAgreementDue { get; set; }
    public DateTime LastMaintenance { get; set; }
    public DateTime RegistrationExpiration { get; set; }
    public DateTime InspectionDue { get; set; }
    public DateTime InsuranceExpiration { get; set; }
    public bool HasW9 { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public string Year { get; set; }
    public string LicensePlate { get; set; }
    public string InsuranceCompany { get; set; }
    public bool IsSelfInsured { get; set; }
    public string Comments { get; set; }
    public bool IsActive { get; set; }
  }
}

using System;

namespace TransportDepot.Models.Safety
{
  public class Tractor
  {
    public string Id { get; set; }
    public string Unit { get; set; }
    public string VIN { get; set; }
    public string Name { get; set; }
    public string LessorOwnerName { get; set; }
    public DateTime LeaseAgreementDue { get; set; }
    public DateTime LastMaintenance { get; set; }
    public DateTime RegistrationExpiration { get; set; }
    public DateTime InspectionDue { get; set; }
    public string InsuranceName { get; set; }
    public DateTime InsuranceExpiration { get; set; }
    public bool HasW9 { get; set; }
    public string Comments { get; set; }
  }
}

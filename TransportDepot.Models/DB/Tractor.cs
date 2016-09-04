using System;

namespace TransportDepot.Models.DB
{
  public class Tractor
  {
    public string Id { get; set; }
    public bool Active { get; set; }
    public string Comments { get; set; }
    public bool HasTripAssigned { get; set; }
    public bool HasW9 { get; set; }
    public DateTime InspectionDue { get; set; }
    public bool IsLessorTruck { get; set; }
    public DateTime LeaseAgreementDue { get; set; }
    public string LessorOwnerName { get; set; }
    public string LicensePlate { get; set; }
    public DateTime LastMaintenance { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public DateTime RegistrationExpiration { get; set; }
    public string InsuranceName { get; set; }
    public DateTime InsuranceExpiration { get; set; }
    public string Type { get; set; }
    public string Unit { get; set; }
    public string VIN { get; set; }
    public string Year { get; set; }
  }
}

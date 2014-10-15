
using System;
namespace TransportDepot.Models.Safety
{
  public class Driver
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime AnnualCertificationOfViolations { get; set; }
    public bool Active { get; set; }
    public bool TrackDriver { get; set; }
    public bool Application { get; set; }
    public bool OnDutyHours { get; set; }
    public bool DrugTest { get; set; }
    public DateTime MVRExpiration { get; set; }
    public bool PoliceReport { get; set; }
    public bool PreviousEmployerForm { get; set; }
    public bool SocialSecurity { get; set; }
    public bool Agreement { get; set; }
    public bool W9 { get; set; }
    public DateTime PhysicalExamExpiration { get; set; }
    public DateTime LastValidLogDate { get; set; }
    public DateTime DriversLicenseExpiration { get; set; }
    public string Comments { get; set; }
  }
}

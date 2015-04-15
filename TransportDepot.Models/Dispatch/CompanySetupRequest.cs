using System;

namespace TransportDepot.Models.Dispatch
{
  public class CompanySetupRequest
  {
    public string ClientId { get; set; }
    public DateTime Date { get; set; }
    public string ContactName { get; set; }
    public string Email { get; set; }
    public string Comments { get; set; }
    public string EmployeeName { get; set; }
    public string EmployeeEmail { get; set; }
  }
}


namespace TransportDepot.Models.DB
{
  public class Customer
  {
    public string ID { get; set; }
    public string CustomerName { get; set; }
    public bool IsBillTo { get; set; }
    public bool IsOrigin { get; set; }
    public bool IsDestination { get; set; }
    public bool IsInbound { get; set; }
    public bool IsOutbound { get; set; }
    public bool IsContract { get; set; }
    public string StreetAddress { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string Country { get; set; }
    public string Phone { get; set; }
    public string Fax { get; set; }
    public string ContactPersons { get; set; }
    public string OfficeHours { get; set; }
    public string FirmNote { get; set; }
    public bool HasRateByDestinationState { get; set; }
    public bool HasRateByDestinationCity { get; set; }
    public bool HasRateByOriginState { get; set; }
    public bool HasRateByOriginCity { get; set; }
    public bool HasChargeGST { get; set; }
    public decimal CreditLimitAmount { get; set; }
    public bool UsesCanadianFunds { get; set; }
    public bool UsesEDI { get; set; }
    public bool IsConsolidated { get; set; }
    public string DriverInstructions { get; set; }
    public string Location { get; set; }
    public decimal StopRate { get; set; }
    public bool RequiresPONumber { get; set; }
    public int CreditLimitDayCount { get; set; }
    public string CustomsBrokerID { get; set; }
    public bool IsInactive { get; set; }
    public bool HasWebAccess { get; set; }
    public string WebPassword { get; set; }
    public string Email { get; set; }
    public string CustomField { get; set; }
    public bool RequiresBOLNumber { get; set; }
  }
}

using System;

namespace TransportDepot.Models.Business
{
  public class Lessor
  {
    public string VendorType { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string Country { get; set; }
    public string Phone { get; set; }
    public string Fax { get; set; }
    public string ContactPerson { get; set; }
    public string OfficeHours { get; set; }
    public string Comment { get; set; }
    public bool Has1099 { get; set; }
    public int BoxNumberFor1099 { get; set; }
    public string TaxId { get; set; }
    public string GlRsAccount { get; set; }
    public string GlRsExpenseDepartment { get; set; }
    public string GlRsExpenseAccount { get; set; }
    public string Method { get; set; }
    public decimal Rate { get; set; }
    public decimal Adjustments1099 { get; set; }
    public bool HasDifferentAddress { get; set; }
    public string PaymentName { get; set; }
    public string PaymentAddress { get; set; }
    public string PaymentCity { get; set; }
    public string PaymentState { get; set; }
    public string PaymentZip { get; set; }
    public string PaymentCountry { get; set; }
    public bool UsesCanadianFunds { get; set; }
    public DateTime InsuranceExpiration { get; set; }
    public bool IsCarrier { get; set; }
    public string McNumber { get; set; }
    public int DueDays { get; set; }
    public decimal DeadheadRate { get; set; }
  }
}

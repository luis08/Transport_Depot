
using System.Runtime.Serialization;
namespace TransportDepot.Models.Payables.Commissions
{
  [DataContract]
  public class TripSpan:Span
  {
    [DataMember] public string InvoiceNumber { get; set; }
    [DataMember]
    public decimal InvoiceAmout { get; set; }
    [DataMember]
    public string TripNumber { get; set; }
    [DataMember]
    public decimal LessorRevenue { get; set; }
    [DataMember]
    public Location TractorHome { get; set; }
  }
}

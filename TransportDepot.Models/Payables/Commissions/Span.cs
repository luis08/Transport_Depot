using System;
using System.Runtime.Serialization;

namespace TransportDepot.Models.Payables.Commissions
{
  [DataContract, KnownType(typeof(TripSpan))]
  public class Span
  {
    [DataMember]
    public string LessorId { get; set; }
    [DataMember]
    public DateTime StartDate { get; set; }
    [DataMember]
    public DateTime EndDate { get; set; }
    [DataMember]
    public Location StartLocation { get; set; }
    [DataMember]
    public Location EndLocation { get; set; }
    [DataMember]
    public Span PreviousSpan { get; set; }
  }
}

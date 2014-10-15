using System.Runtime.Serialization;

namespace Transport_Depot_WCF.ACH
{
  [DataContract]
  class LessorPayment:ACHTransaction
  {
    [DataMember]
    public string LessorId { get; set; }
    [DataMember]
    public int PaymentId { get; set; }
  }
}

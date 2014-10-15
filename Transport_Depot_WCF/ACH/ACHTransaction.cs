
using System.Runtime.Serialization;
namespace Transport_Depot_WCF.ACH
{
  [DataContract]
  class ACHTransaction
  {
    [DataMember]
    public int ACHBatchNumber { get; set; }
    [DataMember]
    public string TraceNumberSequence { get; set; }
    [DataMember]
    public string RoutingNumber { get; set; }
    [DataMember]
    public string AccountNumber { get; set; }
    [DataMember]
    public string TransactionType { get; set; }
    [DataMember]
    public decimal Amount { get; set; }
  }
}

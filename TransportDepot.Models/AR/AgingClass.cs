using System.ServiceModel;
using System.Runtime.Serialization;
namespace TransportDepot.Models.AR
{
  [DataContract]
  public enum AgingClass
  {
    [EnumMember]
    QuickPay = 0,
    [EnumMember]
    Factored = 1,
    [EnumMember]
    NoCredit = 2
  }
}

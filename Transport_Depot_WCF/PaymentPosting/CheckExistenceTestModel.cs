using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.PaymentPosting
{
  [DataContract]
  class CheckExistenceTestModel
  {
    [DataMember]
    public ACHFileIdentifierModel FileIdentifier { get; set; }
    [DataMember]
    public string CheckNumber { get; set; }
    [DataMember]
    public bool Exists { get; set; }
    [DataMember]
    public bool Ignore { get; set; }
  }
}

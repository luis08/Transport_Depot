using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.PaymentPosting
{
  [DataContract]
  class ACHFileIdentifierModel
  {
    [DataMember]
    public DateTime FileDateTime { get; set; }

    [DataMember]
    public char FileIDModifier { get; set; }
  }
}

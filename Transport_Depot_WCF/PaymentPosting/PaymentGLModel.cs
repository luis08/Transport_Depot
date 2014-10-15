using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.PaymentPosting
{
  [DataContract]
  class PaymentGlModel
  {
    [DataMember]
    public GLAccountModel SettlementAccount { get; set; }

    [DataMember]
    public GLAccountModel ACHCashAccount { get; set; }
  }
}

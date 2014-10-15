using System;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.Settlements
{
    [DataContract]
    class UnpostedSettlementsPending
    {
        [DataMember]
        public string Problem { get; set; }
    }
}

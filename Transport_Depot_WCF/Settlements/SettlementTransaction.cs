using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.Settlements
{
    [DataContract]
    public class SettlementTransaction
    {
        [DataMember]
        public DateTime ActivityDate { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}

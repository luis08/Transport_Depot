using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.Settlements
{
    [DataContract]
    public class SettlementTractor
    {
        [DataMember]
        public string TractorID { get; set; }
    }
}

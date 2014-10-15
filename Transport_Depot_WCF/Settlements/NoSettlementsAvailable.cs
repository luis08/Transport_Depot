using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.Settlements
{
    [DataContract]
    class NoSettlementsAvailable
    {
        [DataMember]
        public string Problem { get; set; }
    }
}

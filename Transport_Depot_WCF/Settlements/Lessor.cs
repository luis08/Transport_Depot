using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF
{
    [DataContract]
    public class Lessor
    {
        [DataMember]
        public string LessorId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Settlements.Address Address { get; set; }
    }
}

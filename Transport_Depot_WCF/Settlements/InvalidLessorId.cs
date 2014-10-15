using System;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.Settlements
{
    [DataContract]
    public class InvalidLessorId
    {
        [DataMember]
        public string Problem { get; set; }
    }
}

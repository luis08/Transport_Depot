using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.Settlements
{
    [DataContract]
    public class Advance:SettlementTransaction
    {
        public Advance():base( )
        { this.TripNumber = string.Empty; }

        [DataMember]
        public string TripNumber { get; set; }

        [DataMember]
        public string AdvanceDate { get; set; }
        
        [DataMember]
        public string Reference { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public bool IsSelectedForPayment { get; set; }
    }
}

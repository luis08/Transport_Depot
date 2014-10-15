using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.Settlements
{
    [DataContract(Name="State")]
    public enum SettlementState
    {
        [EnumMember]
        NotAccepted,
        [EnumMember]
        PendingPost,
        [EnumMember]
        Paid
    }
    
    
    [DataContract]
    public class LessorSettlement
    {
        public LessorSettlement()
        {
            this.Trips = new List<SettlementTrip>();
            this.Advances = new List<Advance>();
            this.Tractor = new SettlementTractor();
            this.Lessor = new Lessor();
            this.State = SettlementState.NotAccepted; 
        }

        [DataMember]
        public SettlementState State { get; set; }

        [DataMember]
        public SettlementTractor Tractor{ get; set; }

        [DataMember]
        public List<SettlementTrip> Trips { get; set; }

        [DataMember]
        public List<Advance> Advances { get; set; }

        [DataMember]
        public Lessor Lessor { get; set; }


        [DataMember]
        public string Message { get; set; }
    }
}

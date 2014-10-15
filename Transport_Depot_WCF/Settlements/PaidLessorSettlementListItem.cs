using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.Settlements
{
    [DataContract]
    public class PaidLessorSettlementListItem
    {
        [DataMember]
        public int ScheduledPaymentId { get; set; }

        [DataMember]
        public DateTime AcceptedDate { get; set; }

        [DataMember]
        public DateTime InvoiceDate { get; set; }

        [DataMember]
        public SettlementState State { get; set; }

        [DataMember]
        public decimal Total { get; set; }
    }
}

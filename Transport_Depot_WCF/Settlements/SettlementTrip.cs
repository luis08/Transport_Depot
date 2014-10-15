using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.Settlements
{
    [DataContract]
    public class SettlementTrip
    {
        public SettlementTrip( )
        { 
            this.Number = string.Empty;
            this.RevenueItems = new List<TripRevenueItem>();
            this.Deductions = new List<Deduction>();
            this.IsSelectedForPayment = false;
        }

        [DataMember]
        public DateTime InvoiceDate { get; set; }

        [DataMember]
        public DateTime ActivityDate { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public IEnumerable<TripRevenueItem> RevenueItems { get; set; }

        [DataMember]
        public IEnumerable<Deduction> Deductions { get; set; }

        [DataMember]
        public bool IsSelectedForPayment { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.Settlements
{
    /// <summary>
    /// These objects represent each revenue item in one trip (they all start with 'A')
    /// </summary>
    [DataContract]
    public class TripRevenueItem :SettlementTransaction
    {
        [DataMember]
        public decimal RevenueTotals { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.Settlements
{
    [DataContract]
    public class TractorRevenueItem: SettlementTransaction
    {
        public TractorRevenueItem()
            : base()
        {
            this.TotalRevenue = decimal.Zero;
        }

        public decimal TotalRevenue { get; set; }
    }
}

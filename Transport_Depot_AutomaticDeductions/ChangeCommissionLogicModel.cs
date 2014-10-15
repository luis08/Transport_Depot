using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace Transport_Depot_AutomaticDeductions
{
    [DataContract]
    public class ChangeCommissionLogicModel
    {
        [DataMember]
        public IEnumerable<decimal> CommissionRates
        {
            get;
            set;
        }

        [DataMember]
        public IEnumerable<ChangeCommission> Deductions
        {
            get;
            set;
        }
    }
}

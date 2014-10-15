using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.Settlements
{   
    [DataContract]
    public class Deduction
    {
        [DataMember]
        public string Description { get; set; }
        
        [DataMember]
        public decimal Rate { get; set; }
        
        [DataMember]
        public decimal Amount { get; set; }

        
        public decimal TotalDeduction
        { get { return decimal.Multiply(this.Rate, this.Amount); } }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Globalization;

namespace Transport_Depot_AutomaticDeductions
{
    [DataContract]
    public class LessorDefaults
    {
        [DataMember]
        public string LessorId
        {
            get;
            set;
        }

        [DataMember]
        public decimal Commission
        {
            get;
            set;
        }

        [DataMember]
        public IEnumerable<decimal> CommissionRates
        {
            get;
            set;
        }

        [DataMember]
        public decimal Liability
        {
            get;
            set;
        }

        [DataMember]
        public IEnumerable<decimal> LiabilityRates
        {
            get;
            set;
        }

        [DataMember]
        public decimal Cargo
        {
            get;
            set;
        }


        [DataMember]
        public IEnumerable<decimal> CargoRates
        {
            get;
            set;
        }
    }
}

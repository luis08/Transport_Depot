using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_AutomaticDeductions
{
    [DataContract]
    public class DefaultTypeAndValue
    {
        [DataMember]
        public string DefaultType
        {
            get;
            set;
        }

        [DataMember]
        public decimal DefaultValue
        {
            get;
            set;
        }
    }
}

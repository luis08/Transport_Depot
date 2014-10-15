using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Globalization;

namespace Transport_Depot_AutomaticDeductions
{
    [DataContract]
    public class DisplayRates
    {
        public DisplayRates(decimal rate)
        {
            if (rate < 0)
            {
                decimal val = rate * -1;
                this.TextField = val.ToString("C2");
            }
            else
            {
                this.TextField = string.Format(CultureInfo.InvariantCulture, "{0:#0.##%}", rate); 
            }
            this.ValueField = rate;
        }

        [DataMember]
        public string TextField
        {
            get;
            set;
        }

        [DataMember]
        public decimal ValueField
        {
            get;
            set;
        }
    }
}

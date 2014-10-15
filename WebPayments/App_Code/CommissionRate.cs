using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    [Serializable]
    public class CommissionRate
    {
        private decimal _rateID;

        public CommissionRate(decimal rate)
        {
            this.RateId = rate;
            if (rate < 1)
            {
                decimal displayRate = rate * 100;
                decimal fRate = Math.Floor(displayRate);
                if (displayRate != fRate)
                {
                    this.RateText = double.Parse(displayRate + "") + "%";
                }
                else
                {
                    this.RateText = Convert.ToInt32(displayRate) + "%";
                }
            }
            else
            {
                this.RateText = "$" + rate;
            }
            //int decimalPlaces = BitConverter.GetBytes(decimal.GetBits(rate)[3])[2];
        }

        public decimal RateId
        {
            get { return _rateID; }
            set
            {
                _rateID = value;
            }
        }

        public string RateText
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            CommissionRate other = obj as CommissionRate;
            if (other != null)
            {
                return other.RateId == other.RateId;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutomaticDeductionsService;
using System.Globalization;

    [Serializable]
    public class ChangeCommissionViewModel : ChangeCommission
    {
        private CommissionRate _commissionRate;

        public ChangeCommissionViewModel()
        {
        }

        public ChangeCommissionViewModel(ChangeCommission d, IEnumerable<CommissionRate> rates)
        {
            this.Update = true;
            this.LessorId = d.LessorId;
            this.TripNumber = d.TripNumber;
            this.Commission = d.Commission;
            this.ChangeCommissionDefault = false;
            this.LiabilityInsurance = d.LiabilityInsurance;
            this.CargoInsurance = d.CargoInsurance;
            foreach (CommissionRate r in rates)
            {
                if (r.RateId == this.Commission)
                {
                    this.DisplayRate = r;
                    break;
                }
            }
            if (this.DisplayRate == null)
            {
                CommissionRate nr = new CommissionRate(this.Commission);
                this.DisplayRate = nr;
                List<CommissionRate> new_rates = new List<CommissionRate>();
                foreach (CommissionRate r in rates)
                {
                    new_rates.Add(r);
                }
                new_rates.Add(nr);
                this.CommissionRates = new_rates;
            }
            else this.CommissionRates = rates;
        }

        public bool Update
        {
            get;
            set;
        }

        public CommissionRate DisplayRate
        {
            get
            {
                return _commissionRate;
            }
            set
            {
                if (value == null)
                    return;
                _commissionRate = value;
                this.Commission = _commissionRate.RateId;
            }
        }

        public IEnumerable<CommissionRate> CommissionRates
        {
            get;
            set;
        }

        public string LiabilityField
        {
            get 
            {
                if (this.LiabilityInsurance < 0)
                {
                    decimal val = this.LiabilityInsurance * -1;
                    return val.ToString("C2");
                }
                return string.Format(CultureInfo.InvariantCulture, "{0:#0.##%}", this.LiabilityInsurance); 
            }
            set { }
        }

        public string CargoField
        {
            get 
            {
                if (this.CargoInsurance < 0)
                {
                    decimal val = this.CargoInsurance * -1;
                    return val.ToString("C2");
                }
                return string.Format(CultureInfo.InvariantCulture, "{0:#0.##%}", this.CargoInsurance); 
            }
            set { }
        }
    }

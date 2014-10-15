using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Globalization;

namespace TransportDepot.AutomaticLessorDeductions
{
  [Serializable]
  public class DisplayRate
  {
      public DisplayRate(decimal rate)
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
          this.ValueField = rate.ToString("#0.00");
      }

      public string TextField
      {
          get;
          set;
      }

      public string ValueField
      {
          get;
          set;
      }
  }
}

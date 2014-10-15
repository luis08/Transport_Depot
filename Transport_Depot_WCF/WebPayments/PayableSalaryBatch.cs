using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.WebPayments
{
  [DataContract]
  public class PayableSalaryBatch
  {
    [DataMember]
    int BatchNumber;

    [DataMember]
    DateTime PayrollDate;


  }
}

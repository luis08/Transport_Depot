using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transport_Depot_WCF.WebPayments;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF
{
  [DataContract]
  public class WebPaymentsDue
  {
    [DataMember]
    public IEnumerable<PayableSettlement> SettlementsDue {get; set;}

    [DataMember]
    public IEnumerable<PayableSalaryBatch> PayrollDue { get; set; }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.Settlements
{
  [DataContract]
  public class PaidLessorSettlement: LessorSettlement
  {
    [DataMember]
    public int ScheduledPaymentID { get; set; }

    [DataMember]
    public DateTime ShceduledDate { get; set; }
  }
}

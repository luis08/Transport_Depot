using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.WebPayments
{
  [DataContract]
  public class PayableSettlement
  {
    [DataMember]
    public int PaymentId { get; set; }

    [DataMember]
    public string LessorId { get; set; }

    [DataMember]
    public bool IsCompany { get; set; }

    [DataMember]
    public DateTime AcceptedDate { get; set; }

    [DataMember]
    public decimal BalanceDue { get; set; }
  }
}

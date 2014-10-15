using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transport_Depot_WCF.Settlements;
using System.Runtime.Serialization;
using Transport_Depot_WCF.WebPayments;
using System.ServiceModel;

namespace Transport_Depot_WCF
{
  [ServiceContract]
  public interface IWebPaymentsService
  {
    [OperationContract]
    WebPaymentsDue GetWebPaymentsDue();
  }
}

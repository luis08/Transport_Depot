using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transport_Depot_WCF.WebPayments;

namespace Transport_Depot_WCF
{
  class WebPaymentService: IWebPaymentsService
  {
    public WebPaymentsDue GetWebPaymentsDue()
    {
      var dal = new WebPaymentsDAL();

      return dal.GetWebPaymentsDue();
    }
  }
}

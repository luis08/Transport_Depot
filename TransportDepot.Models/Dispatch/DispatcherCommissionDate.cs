using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Models.Dispatch
{
  public class DispatcherCommissionDate
  {
    public string DispatcherId { get; set; }
    public DateTime CommissionPaymentDate { get; set; }
  }
}

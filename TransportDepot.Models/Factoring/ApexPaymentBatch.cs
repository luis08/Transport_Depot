using System.Collections.Generic;

namespace TransportDepot.Models.Factoring
{
  public class ApexPaymentBatch
  {
    public IEnumerable<FactoringPayment> ValidPayments { get; set; }
    public IEnumerable<FactoringPayment> InvalidPayments { get; set; }
  }
}

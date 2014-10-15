using System.Collections.Generic;
using TransportDepot.Models.Payables.Commissions;

namespace TransportDepot.Payables.Commissions
{
  interface ICommissionService
  {
    IEnumerable<InvoiceCommission> GetCommissions(IEnumerable<CommissionRequest> request);
  }
}

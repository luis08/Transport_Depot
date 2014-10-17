using System.Collections.Generic;
using TransportDepot.Models.Payables.Commissions;
using System.ServiceModel;

namespace TransportDepot.Payables.Commissions
{
  [ServiceContract(Namespace="http://www.transportdepot.net/Payables")]
  public interface ICommissionService
  {
    [OperationContract]
    IEnumerable<InvoiceCommission> GetAllCommissions();
    [OperationContract]
    IEnumerable<InvoiceCommission> GetCommissions(IEnumerable<CommissionRequest> request);
    [OperationContract]
    IEnumerable<CommissionCandidate> GetCandidates();
  }
}

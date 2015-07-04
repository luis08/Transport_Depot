using System.Collections.Generic;
using TransportDepot.Models.Payables.Commissions;
using System.ServiceModel;

namespace TransportDepot.Payables.Commissions
{
  [ServiceContract(Namespace="http://www.transportdepot.net/Payables")]
  [ServiceKnownType(typeof(TripSpan))]
  public interface ICommissionService
  {
    [OperationContract]
    IEnumerable<InvoiceCommission> GetAllCommissions();
    [OperationContract]
    void SaveCommissions(IEnumerable<InvoiceCommission> commissions);
    [OperationContract]
    IEnumerable<InvoiceCommission> GetCommissions(IEnumerable<CommissionRequest> request);
    [OperationContract]
    IEnumerable<CommissionCandidate> GetCandidates();

    [OperationContract]
    IEnumerable<PreviousTrip> GetPreviousTrips(IEnumerable<CommissionCandidate> candidates);
    [OperationContract]
    IEnumerable<LessorHome> GetLessorHomes(IEnumerable<string> tripIds);
    [OperationContract]
    IEnumerable<CommissionRequest> GetRequests(RequestBuilderBucket bucket);
    [OperationContract]
    void SaveNewCommisions();
  }
}

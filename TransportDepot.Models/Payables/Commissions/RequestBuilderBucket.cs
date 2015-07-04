

using System.Collections.Generic;
namespace TransportDepot.Models.Payables.Commissions
{
  public class RequestBuilderBucket
  {
    public IEnumerable<CommissionCandidate> Candidates { get; set; }
    public IEnumerable<PreviousTrip> PreviousTrips { get; set; }
    public IEnumerable<LessorHome> LessorHomes { get; set; }
  }
}

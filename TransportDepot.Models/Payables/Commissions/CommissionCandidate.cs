using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Models.Payables.Commissions
{
  public class CommissionCandidate
  {
    public string TractorId { get; set; }
    public string AgentId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Location StartLocation { get; set; }
    public Location EndLocatioin { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal InvoiceAmount { get; set; }
    public string TripNumber { get; set; }
  }
}

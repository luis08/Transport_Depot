
using System;
namespace TransportDepot.Models.Payables.Commissions
{
  public class InvoiceCommission
  {
    public string AgentId { get; set; }
    public string LessorId { get; set; }
    public string InvoiceNumber { get; set; }
    public string TripNumber { get; set; }
    public decimal Percent { get; set; }
    public decimal InvoiceAmount { get; set; }
    public DateTime DueDate { get; set; }
    public string Description { get; set; }
  }
}

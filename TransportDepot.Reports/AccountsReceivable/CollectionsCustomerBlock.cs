
using System.Collections.Generic;
using TransportDepot.Models.AR;
namespace TransportDepot.Reports.AccountsReceivable
{
  public class CollectionsCustomerBlock
  {
    public string CustomerName { get; set; }
    public string CustomerPhone { get; set; }
    public int CreditPeriod { get; set; }
    public decimal Balance { get; set; }
    public string Email { get; set; }
    public decimal BalanceCurrent { get; set; }
    public decimal BalanceLate { get; set; }
    public decimal BalanceOver30 { get; set; }
    public bool LastCustomerBlock { get; set; }
    public IEnumerable<CollectionsReportInvoice> Invoices { get; set; }
  }
}

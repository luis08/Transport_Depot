using System;

namespace ACHWebPayments.ACHLogic
{
  public class EntryDetailTransaction
  {
    public WebPayments.ACHElements.ACHPaymentType Type { get; set; }
    public int TransactionId { get; set; }
    public string ReceiverName { get; set; }
    public string ReceiverDFIIdentification { get; set; }
    public string DFIAccountNumber { get; set; }
    public string ReceiverIdentficationNumber { get; set; }
    public decimal Amount { get; set; }
  }
}
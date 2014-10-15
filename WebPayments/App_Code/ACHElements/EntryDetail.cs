using System;
using System.Text;

namespace WebPayments.ACHElements
{
  public enum ACHPaymentType { Settlement, Payroll, Offset };
  public class EntryDetail
  {
    public string RecordTypeCode { get; set; }
    public string TransactionCode { get; set; }
    public string ReceivingDFIIdentification { get; set; }
    public string CheckDigit{ get; set; }
    public string DFIAccountNumber { get; set; }
    public string Amount { get; set; }
    public string ReceiverIdentificationNumber { get; set; }
    public string ReceiverName { get; set; }
    public string DiscretionaryData { get; set; }
    public string AddendaRecordIndicator { get; set; }
    public string TraceNumber { get; set; }
    public ACHPaymentType PaymentType  { get; set; }
    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append(this.RecordTypeCode)
        .Append(this.TransactionCode)
        .Append(this.ReceivingDFIIdentification)
        .Append(this.CheckDigit)
        .Append(this.DFIAccountNumber)
        .Append(this.Amount)
        .Append(this.ReceiverIdentificationNumber)
        .Append(this.ReceiverName)
        .Append(this.DiscretionaryData)
        .Append(this.AddendaRecordIndicator)
        .Append(this.TraceNumber);
      
      return sb.ToString();
    }
  }
}
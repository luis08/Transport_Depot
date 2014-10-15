using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WebPayments.ACHElements
{
  public class CompanyBatchControl
  {
    public string RecordTypeCode { get; set; }
    public string ServiceClassCode { get; set; }
    public string EntryAddendaCount { get; set; }
    public string EntryHash { get; set; }
    public string TotalDebitEntryDollarAmount { get; set; }
    public string TotalCreditEntryDollarAmount { get; set; }
    public string CompanyIdentification { get; set; }
    public string MessageAuthenticationCode { get; set; }
    public string Reserved { get; set; }
    public string OriginatingDFIBankIdentification { get; set; }
    public string BatchNumber { get; set; }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append(this.RecordTypeCode)
        .Append(this.ServiceClassCode)
        .Append(this.EntryAddendaCount)
        .Append(this.EntryHash)
        .Append(this.TotalDebitEntryDollarAmount)
        .Append(this.TotalCreditEntryDollarAmount)
        .Append(this.CompanyIdentification)
        .Append(this.MessageAuthenticationCode)
        .Append(this.Reserved)
        .Append(this.OriginatingDFIBankIdentification)
        .Append(this.BatchNumber);
      return sb.ToString();
    }
  }
}
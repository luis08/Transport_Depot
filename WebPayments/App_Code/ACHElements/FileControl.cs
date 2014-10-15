using System;
using System.Collections.Generic;
using System.Text;

namespace WebPayments.ACHElements
{
  public class FileControl
  {

    public string RecordTypeCode { get; set; }
    public string BatchCount { get; set; }
    public string BlockCount { get; set; }
    public string EntryAddendaRecordCount { get; set; }
    public string EntryHash { get; set; }
    public string TotalDebitEntryDollarAmountInFile { get; set; }
    public string TotalCreditEntryDollarAmountInFile { get; set; }
    public string Reserved { get; set; }
    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append(this.RecordTypeCode)
        .Append(this.BatchCount)
        .Append(this.BlockCount)
        .Append(this.EntryAddendaRecordCount)
        .Append(this.EntryHash)
        .Append(this.TotalDebitEntryDollarAmountInFile)
        .Append(this.TotalCreditEntryDollarAmountInFile)
        .Append(this.Reserved);
      
      return sb.ToString();
    }
  }
}
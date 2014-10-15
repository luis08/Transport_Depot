using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WebPayments.ACHElements
{
  public class CompanyBatchHeader
  {
    public string RecordTypeCode { get; set; }
    public string ServiceClassCode { get; set; }
    public string CompanyName { get; set; }
    public string CompanyDiscretionaryData { get; set; }
    public string CompanyIdentification { get; set; }
    public string StandardEntryClassCode { get; set; } //PPD
    public string CompanyEntryDescription { get; set; }
    public string CompanyDescriptiveDate { get; set; }
    public string EffectiveEntryDate { get; set; }
    public DateTime EffectiveEntryDateValue { get; set; }
    public string Reserved { get; set; }
    public string OriginatorStatusCode { get; set; }
    public string OriginatingDFIBankIdentification { get; set; }
    public string BatchNumber { get; set; }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append(this.RecordTypeCode)
        .Append(this.ServiceClassCode)
        .Append(this.CompanyName)
        .Append(this.CompanyDiscretionaryData)
        .Append(this.CompanyIdentification)
        .Append(this.StandardEntryClassCode)
        .Append(this.CompanyEntryDescription)
        .Append(this.CompanyDescriptiveDate)
        .Append(this.EffectiveEntryDate)
        .Append(this.Reserved)
        .Append(this.OriginatorStatusCode)
        .Append(this.OriginatingDFIBankIdentification)
        .Append(this.BatchNumber);
      return sb.ToString();
    }
  }
}

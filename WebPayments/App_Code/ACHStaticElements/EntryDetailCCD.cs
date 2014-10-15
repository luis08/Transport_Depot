using System;
using System.Text;
using System.Configuration;

namespace WebPayments.ACHStaticElements
{
  public class EntryDetailCCD : ConfigurationSection
  {
    [ConfigurationProperty("recordTypeCode")]
    //[StringValidator(MinLength = 1, MaxLength = 1)]
    public string RecordTypeCode
    {
      get { return this["recordTypeCode"].ToString(); }
    }

    [ConfigurationProperty("transactionCode")]
    //[StringValidator(MinLength = 2, MaxLength = 2)]
    public string TransactionCode
    {
      get { return this["transactionCode"].ToString(); }
    }

    [ConfigurationProperty("discretionaryData")]
    //[StringValidator(MinLength = 2, MaxLength = 2)]
    public string DiscretionaryData
    {
      get { return this["discretionaryData"].ToString(); }
    }

    [ConfigurationProperty("addendaRecordIndicator")]
    //[StringValidator(MinLength = 1, MaxLength = 1)]
    public string AddendaRecordIndicator
    {
      get { return this["addendaRecordIndicator"].ToString(); }
    }
  }
}

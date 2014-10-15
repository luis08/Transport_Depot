using System;
using System.Configuration;

namespace WebPayments.ACHStaticElements
{
  
  public class EntryDetailOffset: ConfigurationSection
  {
    [ConfigurationProperty("recordTypeCode")]
    public string RecordTypeCode 
    {
      get { return this["recordTypeCode"].ToString(); }
    }

    [ConfigurationProperty("transactionCode")]
    public string TransactionCode
    {
      get { return this["transactionCode"].ToString(); }
    }

    [ConfigurationProperty("receivingDFIIdentification")]
    public string ReceivingDFIIdentification
    {
      get { return this["receivingDFIIdentification"].ToString(); }
    }

    [ConfigurationProperty("dfiAccountNumber")]
    public string DFIAccountNumber
    {
      get { return this["dfiAccountNumber"].ToString(); }
    }

    [ConfigurationProperty("identificationNumber")]
    public string IdentificationNumber
    {
      get { return this["identificationNumber"].ToString(); }
    }

    [ConfigurationProperty("receiverName")]
    public string ReceiverName
    {
      get { return this["receiverName"].ToString(); }
    }

    [ConfigurationProperty("discretionaryData")]
    public string DiscretionaryData
    {
      get { return this["discretionaryData"].ToString(); }
    }

    [ConfigurationProperty("addendaRecordIndicator")]
    public string AddendaRecordIndicator
    {
      get { return this["addendaRecordIndicator"].ToString(); }
    }
  }
}
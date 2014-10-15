using System;
using System.Text;
using System.Configuration;

namespace WebPayments.ACHStaticElements
{
  public class CompanyBatchHeader : ConfigurationSection
  {
    [ConfigurationProperty("recordTypeCode")]
    public string RecordTypeCode
    {
      get { return this["recordTypeCode"].ToString(); }
    }

    [ConfigurationProperty("serviceClassCode")]
    //[StringValidator(MinLength = 3, MaxLength = 3)]
    public string ServiceClassCode
    {
      get { return this["serviceClassCode"].ToString(); }
    }


    [ConfigurationProperty("reserved")]
    //[StringValidator(MinLength = 6, MaxLength = 6)]
    public string Reserved
    {
      get { return this["reserved"].ToString(); }
    }


    [ConfigurationProperty("originatorStatusCode")]
    //[StringValidator(MinLength = 8, MaxLength = 8)]
    public string OriginatorStatusCode
    {
      get { return this["originatorStatusCode"].ToString(); }
    }

    [ConfigurationProperty("originatingDFIBankIdentification")]
    //[StringValidator(MinLength=8, MaxLength=8)]
    public string OriginatorDFIBankIdentification
    {
      get { return this["originatingDFIBankIdentification"].ToString(); }
    }
  }
}

using System;
using System.Text;
using System.Configuration;

namespace WebPayments.ACHStaticElements
{
  public class CompanyBatchControl : ConfigurationSection
  {
    [ConfigurationProperty("recordTypeCode")]
    //[StringValidator(MinLength = 1, MaxLength = 1)]
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

    [ConfigurationProperty("messageAuthenticationCode")] 
    public string MessageAuthenticationCode

    {
      get { return this["messageAuthenticationCode"].ToString(); }
    }

    [ConfigurationProperty("reserved")] 
    public string Reserved

    {
      get { return this["reserved"].ToString(); }
    }

    [ConfigurationProperty("originatingDFIBankIdentification")] 
    public string OriginatingDFIBankIdentification

    {
      get { return this["originatingDFIBankIdentification"].ToString(); }
    }

  }
}

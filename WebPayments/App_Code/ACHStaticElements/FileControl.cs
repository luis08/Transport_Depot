using System;
using System.Text;
using System.Configuration;

namespace WebPayments.ACHStaticElements
{
  public class FileControl : ConfigurationSection
  {
    [ConfigurationProperty("recordTypeCode")]
    public string RecordTypeCode
    {
      get { return this["recordTypeCode"].ToString(); }
    }

    [ConfigurationProperty("reserved")]
    public string Reserved
    {
      get { return this["reserved"].ToString(); }
    }
  }
}

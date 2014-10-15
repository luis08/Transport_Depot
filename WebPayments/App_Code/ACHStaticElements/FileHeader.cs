using System;
using System.Configuration;

namespace WebPayments.ACHStaticElements
{
  public class FileHeader : ConfigurationSection
  {
    [ConfigurationProperty("recordTypeCode")]
    public String RecordTypeCode
    {
      get { return this["recordTypeCode"].ToString(); }
    }
    
    [ConfigurationProperty("priorityCode")]
    public string PriorityCode 
    {
      get { return this["priorityCode"].ToString(); } 
    }

    
    [ConfigurationProperty("immediateDestination")]
    public string ImmediateDestination
    {
      get { return this["immediateDestination"].ToString(); }
    }

    public string ImmediateOrigin
    {
      get { return this.CompanyIdentification; }
    }

    [ConfigurationProperty("companyIdentificaton")]
    public string CompanyIdentification 
    {
      get { return this["companyIdentificaton"].ToString(); }
    }

    [ConfigurationProperty("recordSize")]
    public string RecordSize 
    {
      get { return this["recordSize"].ToString(); }
    }

    [ConfigurationProperty("blockingFactor")]
    public string BlockingFactor 
    {
      get { return this["blockingFactor"].ToString(); }
    }

    [ConfigurationProperty("formatCode")]
    public string FormatCode
    {
      get { return this["formatCode"].ToString(); }
    }

    [ConfigurationProperty("immediateDestinationName")]
    public string ImmediateDestinationName 
    {
      get { return this["immediateDestinationName"].ToString(); }
    }

    [ConfigurationProperty("immediateOriginName")]
    public string ImmediateOriginName
    {
      get { return this["immediateOriginName"].ToString(); }
    }
  }
}

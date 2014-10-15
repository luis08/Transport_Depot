using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for OceanBankFields
/// </summary>
public class OceanBankFields
{
  public string OriginalReceivingDFIBankIdentification;
  public string OriginalReceivingDFIAccountNumber;

  public string AccountNumber
  {
    get 
    {
      return this.OriginalReceivingDFIAccountNumber.PadRight(17);
        //string.Format("01{0}{1}", this.OriginalReceivingDFIAccountNumber, new string(' ', 5)); 
    }
  }

  public string ReceivingDFIBankIdentification
  {
    get { return this.OriginalReceivingDFIBankIdentification.Substring(0, 8); }
  }

  public string CheckDigit
  {
    get { return this.OriginalReceivingDFIBankIdentification.Substring(8,1); }
  }
}

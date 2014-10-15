using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ACHPaymentDisplayer
/// </summary>
public class ACHPaymentDisplayer
{
  public ACHPaymentDisplayer()
  {
    //
    // TODO: Add constructor logic here
    //
  }

  public DateTime CreationDate { get; set; }
  public string CreationTime { get; set; }
  public char FileIDModifier { get; set; }
  public int SettlementCount { get; set; }
  public int PayrollEmployeeCount { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PaidLessorSettlement
/// </summary>
public class PaidLessorSettlementViewModel: PaidACHTransaction
{
  public string LessorId { get; set; }
  public int PaymentId { get; set; }
}
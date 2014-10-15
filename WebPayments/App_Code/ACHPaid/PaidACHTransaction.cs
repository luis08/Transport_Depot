using System;


public class PaidACHTransaction
{
  public int ACHBatchNumber { get; set; }
  public string TraceNumberSequence { get; set; }
  public string RoutingNumber { get; set; }
  public string AccountNumber { get; set; }
  public string TransactionType { get; set; }
  public decimal Amount { get; set; }
}
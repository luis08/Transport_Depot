using System;

public class PaidPayrollItem: PaidACHTransaction
{
  public int PayrollBatchNumber { get; set; }
  public string EmployeeId { get; set; }
}
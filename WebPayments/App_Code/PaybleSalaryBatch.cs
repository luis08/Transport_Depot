using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PayableSalaryBatch
{
  public int BatchId { get; set; }
  public IEnumerable<PayablePayrollItem> Payments { get; set; }
}

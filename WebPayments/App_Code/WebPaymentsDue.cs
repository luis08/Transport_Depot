using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for WebPaymentsDue
/// </summary>
public class WebPaymentsDue
{
	public WebPaymentsDue()
	{
		
	}

  public IEnumerable<PayableSettlement> Settlements { get; set; }
  public IEnumerable<PayableSalaryBatch> Payroll { get; set; }
}
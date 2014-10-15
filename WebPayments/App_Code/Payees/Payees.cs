using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Payees
/// </summary>
public class Payees
{
  public IEnumerable<PayeeDTO> Employees { get; set; }
  public IEnumerable<PayeeDTO> Lessors { get; set; }
}
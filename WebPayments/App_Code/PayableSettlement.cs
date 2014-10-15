using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PayableSettlement
{
  public int Id { get; set; }
  public string LessorId { get; set; }
  public string LessorName { get; set; }
  public bool IsCompany { get; set; }
  public DateTime AcceptedDate { get; set; }
  public DateTime InvoiceDate { get; set; }
  public decimal Amount { get; set; }
}

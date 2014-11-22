using System.Collections.Generic;
public class DispatcherPayablesModel
{
  public IEnumerable<DispatcherPayable> Dispatchers { get; set; }
  public IEnumerable<string> Initials { get; set; }
  public IEnumerable<string> Vendors { get; set; }
}
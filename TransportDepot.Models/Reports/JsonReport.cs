
using System.Collections.Generic;
namespace TransportDepot.Models.Reports
{
  public class JsonReport
  {
    public string Name { get; set; }
    public string Title { get; set; }
    public IEnumerable<Field> Fields { get; set; }
    public string[][] data { get; set; }
  }
}

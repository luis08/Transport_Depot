
using System.Collections.Generic;
using System.Xml.Linq;
namespace TransportDepot.Models.Reports
{
  public class JsonReportData
  {
    public IEnumerable<Field> Fields { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public XDocument Data { get; set; }
  }
}

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using TransportDepot.Models.Reports;
using TransportDepot.Data.Reports;

namespace TransportDepot.Reports.Json
{
  class JsonReportService: IJsonReportService
  {
    public JsonReport GetReport(Models.Reports.JsonReportRequest request)
    {
      const string path = @"C:\Sites\debug_wcf222.txt";
      try
      {
        var report = this.GetReport(request.ReportName);
        using (System.IO.StreamWriter sw = System.IO.File.AppendText(path))
        {
          sw.WriteLine("Returned from GetReport ");
        }
        return report;
      }
      catch (Exception e)
      {
        return new JsonReport { Name = "An Error: " + e.Message };
      }
    }

    private JsonReport GetReport(string reportName)
    {
      var dataSource = new JsonReportDataSource();
      var dataReport = dataSource.GetReport(reportName);

      JsonReport report = new JsonReport
        {
          Title = dataReport.Title,
          Fields = dataReport.Fields.ToArray(),
          Name = dataReport.Name,
          data = new string[0,0]
        };
      
      if (dataReport.Data == null) return report;

      report.data = new string [ dataReport.Data.Root.Nodes().Count(), dataReport.Fields.Count()];

      var dataFields = report.Fields.OrderBy(f=>f.index).ToList();
      var rows = dataReport.Data.Root.Elements("result").ToArray();

      for(var rw = 0; rw < rows.Length; rw++)
      {
        dataFields.ForEach(f => report.data[rw, f.index] = rows[rw].Attribute(f.name).Value);
      }
        
      return report;
    }
  }
}

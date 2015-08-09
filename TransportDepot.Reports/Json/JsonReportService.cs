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
        var report = this.GetReport(request.ReportName);
        return report;
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
          data = new string[0][]
        };
      
      if (dataReport.Data == null) return report;
      
      report.data = new string [ dataReport.Data.Root.Nodes().Count()][];

      var dataFields = report.Fields.OrderBy(f=>f.index).ToList();
      var rows = dataReport.Data.Root.Elements("result").ToArray();
      
      for(var rw = 0; rw < rows.Length; rw++)
      {
        report.data[rw] = dataFields.Select(f => 
          rows[rw].Attribute(f.name).Value).ToArray();
      }
        
      return report;
    }


    public IEnumerable<JsonReportSpecs> GetReportSpecs()
    {
      var dataSource = new JsonReportDataSource();
      return dataSource.GetReports();
    }
  }
}

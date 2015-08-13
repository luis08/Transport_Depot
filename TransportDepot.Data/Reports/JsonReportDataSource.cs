
using TransportDepot.Models.Reports;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System;

namespace TransportDepot.Data.Reports
{
  public class JsonReportDataSource
  {
    public IEnumerable<JsonReportSpecs> GetReports()
    {
      var query = "SELECT * FROM [CustomReport]";

      var dataTable = new DataTable();
      using (var cmd = new SqlCommand(query))
      {
        var dataSource = new DataSource();
        dataTable = dataSource.FetchDataTable(cmd);
      }
      var reports = dataTable.AsEnumerable().Select(r => new JsonReportSpecs 
      {
        Name = r.Field<string>("Name"),
        Title = r.Field<string>("Title"),
        Comments =r.Field<string>("Comments")
      }).ToList();

      return reports;
    }

    public JsonReportData GetReport(string name)
    {
      var dataTable = GetDataTable(name);
      if (dataTable.Rows.Count == 0) return null;
      var dataRow = dataTable.Rows[0];
      var reportData = new JsonReportData
      {
        Name = name,
        Title = dataRow.Field<string>("Title"),
        Fields = this.ReadFields(dataRow)
      };

      reportData.Data = GetQueryResults(dataRow, reportData.Fields);

      return reportData;
    }

    private DataTable GetDataTable(string name)
    {
      var ds = new DataSource();
      var query = "SELECT * FROM [CustomReport] WHERE [Name] = @Name";
      var dataTable = new DataTable();
      using (var cmd = new SqlCommand(query))
      {
        cmd.Parameters.AddWithValue("@Name", name);
        dataTable = ds.FetchDataTable(cmd);
      }

      return dataTable;
    }

    private System.Xml.Linq.XDocument GetQueryResults(DataRow queryDataRow, IEnumerable<Field> fields)
    {
      var dataTable = new DataTable();
      using (var cmd = new SqlCommand(queryDataRow.Field<string>("Query")))
      {
        var dataSource = new DataSource();
        dataTable = dataSource.FetchDataTable(cmd);
      }
      if (dataTable.Rows.Count == 0) return null;
      var root = new XElement("results");
      var xml = new XDocument();
     
      for (var i = 0; i < dataTable.Rows.Count; i++)
      {
        var rw = dataTable.Rows[i];
        var ele = new XElement("result");
        fields.ToList().ForEach(f =>
        {
          ele.Add(new XAttribute(f.name, this.GetValue(rw[f.name], f.type)));
        });
        root.Add(ele);
      }

      xml.Add(root);
      return xml;
    }

    private string GetValue(object fieldValue, string fieldTypeName)
    {
      switch (fieldTypeName.ToLower())
      {
        case "string":
          return fieldValue.ToString();
        case "bool":
          return fieldValue.ToString().Equals("0") ? "No" : "Yes";
        case "decimal":
          return decimal.Parse(fieldValue.ToString()).ToString("C");
        case "int":
          return fieldValue.ToString();
        case "datetime":
          if( DBNull.Value.Equals(fieldValue) ) return "[NULL]";
          return Utilities.ParseDateTime(fieldValue).Value.ToShortDateString();
        default:
          throw new InvalidOperationException("Unaccounted type: " + fieldTypeName);
      }
    }

    private System.Collections.Generic.IEnumerable<Field> ReadFields(DataRow r)
    {
      var xmlString = r["Fields"].ToString();
      var xdocument = XDocument.Parse(xmlString);

      var fields = xdocument.Root.Elements("field").Select(e => new Field
        {
          name = e.Attribute("name").Value,
          index = int.Parse(e.Attribute("index").Value),
          headerText = e.Attribute("headerText").Value,
          type = e.Attribute("type").Value 
        });
      return fields;
    }
  }
}

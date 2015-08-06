
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
    public JsonReportData GetReport(string name)
    {
      var dataTable = GetDataTable(name);
      if( dataTable.Rows.Count == 0 ) return null;
      var dataRow = dataTable.Rows[0];
      var reportData =  new JsonReportData
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

    private System.Xml.Linq.XDocument GetQueryResults(DataRow r, IEnumerable<Field> fields)
    {
      var dataTable = new DataTable();
      using(var cmd = new SqlCommand(r.Field<string>("Query")))
      {
        var dataSource = new DataSource();
        dataTable = dataSource.FetchDataTable(cmd);
      }
      if (dataTable.Rows.Count == 0) return null;

      var xml = new XDocument("results",
      dataTable.AsEnumerable().Select(rw => new XElement("result",
        fields.Select(f => new XAttribute(f.name, this.GetValue(rw[f.name], r.Field<string>("type")))))));
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
          return DateTime.Parse(fieldValue.ToString()).ToShortDateString();
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
          headerText = e.Attribute("headerText").Value
        });
      return fields;
    }
  }
}

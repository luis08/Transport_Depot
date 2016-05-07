using System;
using System.Data;
using System.Linq;
using System.Text;

namespace TransportDepot.Data
{
  public class Utilities
  {
    private DateTime DefaultDate = DateTime.Today;
    private bool DefaultBool = false;
    public static readonly string DateTimeMaskMilliseconds = "hh_mm_ss_fff";
    private string _errorPath = string.Empty;
    private static string _debugPath = System.Configuration.ConfigurationManager.AppSettings["TransportDepot.DebugPath"];
    public Utilities()
    {
      var appSetting = System.Configuration.ConfigurationManager.AppSettings["ErrorLogPath"];
      _errorPath =  string.IsNullOrEmpty(appSetting) ? string.Empty : appSetting;
    }
    public static int DefaultCommandTimeout = 240;
    public string ConnectionString
    {
      get
      {
        var conStr = System.Configuration.ConfigurationManager.ConnectionStrings["AccessReplacementConnectionString"].ConnectionString;
        return conStr;
      }
    }

    public string CmdToString(System.Data.SqlClient.SqlCommand cmd)
    {
      if (cmd == null) return "NULL";
      var parameters = string.Empty;

      var builder = new StringBuilder("Sql Command")
        .AppendLine()
        .AppendLine(string.Format("  Type: {0}" , cmd.CommandType.ToString()))
        .AppendLine()
        .AppendLine(this.GetParametersString(cmd.Parameters))
        .AppendLine()
        .AppendLine(cmd.CommandText);

      return builder.ToString();
    }

    private string GetParametersString(System.Data.SqlClient.SqlParameterCollection parms)
    {
      
      var noParameters ="    No Parameters";
      if (parms == null) return noParameters;
      if (parms.Count.Equals(0)) return noParameters;

      var builder = new StringBuilder("    Parameters");
        for (var i = 0; i < parms.Count; i++)
        {
          var val = string.Empty;

          if (parms[i].Value == null) val = "NULL";
          else if (parms[i].Value == DBNull.Value) val = "DBNull";
          else val = parms[i].Value.ToString();
          builder.AppendLine(string.Format("    {0} = [{1}]   Type: {2}",
            parms[i].ParameterName,
            val,
            parms[i].TypeName));
        }
        return builder.ToString();
    }

    public void LogError(object o, string[] data)
    {
      data = data.Union(new string[] 
      { 
        string.Empty, 
        string.Empty, 
        new string('*', 100),
        "Error in: " + (o == null ? "NULL" : o.GetType().ToString())}).ToArray();
      WriteAppend(this._errorPath, data);
    }

    public string CoalesceString(DataRow row, string fieldName)
    {
      if (row == null) return string.Empty;
      if (row[fieldName] == null) return string.Empty;
      return row[fieldName].ToString();
    }

    public DateTime ParseDate(string input)
    {
      input = this.CleanString(input);
      if (input.Equals(string.Empty))
      {
        return DefaultDate;
      }
      var dateTime = DefaultDate;
      if (!DateTime.TryParse(input, out dateTime))
      {
        return DefaultDate;
      }
      return dateTime;
    }

    public bool ParseBool(string input)
    {
      input = this.CleanString(input);
      if (input.Equals(string.Empty)) return DefaultBool;

      if (input.Equals("0")) return false;
      else if (input.Equals("1")) return true;
      else if (input.Equals("N", StringComparison.OrdinalIgnoreCase)) return false;
      else if (input.Equals("Y", StringComparison.OrdinalIgnoreCase)) return true;
      else if (input.Equals("F", StringComparison.OrdinalIgnoreCase)) return false;
      else if (input.Equals("T", StringComparison.OrdinalIgnoreCase)) return true;

      return input.Length > 0;
    }

    public DateTime CoalesceDateTime(DataRow row, string fieldName)
    {
      if (row == null) return DefaultDate;
      if (row[fieldName] == DBNull.Value) return DefaultDate;
      if (row[fieldName] == null) return DefaultDate;
      return Convert.ToDateTime(row[fieldName]);
    }

    private string CleanString(string input)
    {
      if (input == null) return string.Empty;
      
      input = input.Trim();
      
      return input;
    }

    public static void WriteAppend(string path, string[] data)
    {
      using (var writer = new System.IO.StreamWriter(path, true))
      {
        writer.WriteLine(string.Join(Environment.NewLine, data));
      }
    }

    public static void Write(string path, string[] data)
    {
      using (var writer = new System.IO.StreamWriter(path))
      {
        writer.WriteLine(string.Join(Environment.NewLine, data));
      }
    }
    public static void Write(string path, string data)
    {
      using (var writer = new System.IO.StreamWriter(path))
      {
        writer.WriteLine(data);
      }
    }
    public static void WriteAppend(string path, string data)
    {
      using (var writer = new System.IO.StreamWriter(path, true))
      {
        writer.WriteLine(data);
      }
    }

    public static void WriteAppend( string data)
    {
      using (var writer = new System.IO.StreamWriter(_debugPath, true))
      {
        writer.WriteLine(data);
      }
    } 

    internal bool IsEmpty(DataTable dataTable)
    {
      if (dataTable == null)
      { return true; }
      else if (dataTable.Rows == null)
      { return true; }
      else if (dataTable.Rows.Count.Equals(0))
      { return true; }
      return false;
    }

    internal DateTime Coalesce(DateTime dateTime)
    {
      if (dateTime == null)
      {
        return DateTime.MinValue;
      }
      return dateTime;
    }
    public static string Clean(string str)
    {
      if (string.IsNullOrEmpty(str)) { return string.Empty; }
      str = str.Trim();
      return str;
    }

    public static DateTime? ParseDateTime(object o)
    {
      if (DBNull.Value.Equals(o)) return null;
      return ParseDateTime(o.ToString());
    }

    public static DateTime? ParseDateTime(string dateTime)
    {
      if (string.IsNullOrEmpty(dateTime))
      {
        return null;
      }
      var parsedDateTime = DateTime.Today;

      if (!DateTime.TryParse(dateTime, out parsedDateTime))
      {
        return null;
      }
      return parsedDateTime;
    }

    public static decimal ParseDecimalOrZero(string amount)
    {
      if (string.IsNullOrEmpty(amount))
      {
        return decimal.Zero;
      }
      var decimalAmount = decimal.Zero;
      if (!decimal.TryParse(amount, out decimalAmount))
      {
        return decimal.Zero;
      }
      return decimalAmount;
    }


    const int NullGeoLocationId = -1;

    public int ParseGeoLocationId(object geoLocationIdObj)
    {
      if (geoLocationIdObj == null) return NullGeoLocationId;

      var locationString = geoLocationIdObj.ToString();
      var geoLocationId = NullGeoLocationId;
      if (!int.TryParse(locationString, out geoLocationId)) return NullGeoLocationId;
      if (geoLocationId < 1) return NullGeoLocationId;
      return geoLocationId;
    }
  }
}
 
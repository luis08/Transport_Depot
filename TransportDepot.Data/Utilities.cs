﻿using System;
using System.Data;
using System.Linq;

namespace TransportDepot.Data
{
  public class Utilities
  {
    private DateTime DefaultDate = DateTime.Today;
    private bool DefaultBool = false;
    public static readonly string DateTimeMaskMilliseconds = "hh_mm_ss_fff";

    public string ConnectionString
    {
      get
      {
        var conStr = System.Configuration.ConfigurationManager.ConnectionStrings["AccessReplacementConnectionString"].ConnectionString;
        return conStr;
      }
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
  }
}
 
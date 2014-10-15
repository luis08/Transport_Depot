using System;
using System.Text;
using System.IO;
using System.Data.OleDb;
using System.Web.Management;

namespace Transport_Depot_WCF.Helpers
{
  public static class WCFDebugger
  {
    public static void DebugWrite(string text)
    {

      using (StreamWriter sw = File.AppendText(path))
      {
        sw.WriteLine(text);
      }
    }
    private const string path = @"C:\Transport_Depot\debug_wcf.txt";

    public static void LogToDB(string text)
    {
      var cnString = System.Configuration.ConfigurationManager.ConnectionStrings["LogConnectionString"].ConnectionString;
      using( var cn = new OleDbConnection(cnString))
      using (var cmd = new OleDbCommand("INSERT INTO [Log] ( [Text] ) VALUES (@TheText) ", cn))
      {
        cmd.Parameters.AddWithValue("@TheText", text);
        cn.Open();
        cmd.ExecuteNonQuery();
      }
    }

    public static void LogToEvent(string text, object sender)
    {
      var obj = new TransportDepotWcfAuditEvent(text, sender, WebEventCodes.WebExtendedBase + 1);
      obj.Raise();
    }
  }
  
}

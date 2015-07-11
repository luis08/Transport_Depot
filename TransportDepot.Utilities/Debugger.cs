using System.IO;
using System;
namespace TransportDepot.Utilities
{
  public static class Debugger
  {
    private const string path = @"C:\Sites\debug_wcf.txt";
    private const int DebugDelimiterTextWidth = 100;
    public static void WriteDebugHeader(string headerText)
    {
      headerText = string.IsNullOrEmpty(headerText) ? "headerText was null or empty" : headerText;
      Write(new string('*', DebugDelimiterTextWidth));
      var fullHeaderText = string.Format("** {0: yyyy-MM-dd hh:mm:ss.fff} Debug Section Start: {1} *******", DateTime.Now, headerText);
      fullHeaderText = string.Concat(fullHeaderText,
        new string('*', Math.Abs( DebugDelimiterTextWidth - fullHeaderText.Length)));
      Write(fullHeaderText);
      Write(new string('*', DebugDelimiterTextWidth));
      Write(Environment.NewLine);
      Write(Environment.NewLine);
    }

    public static void WriteDebugFooter(string footerText)
    {
      Write(new string('*', DebugDelimiterTextWidth));
      var fullHeaderText = string.Format("** {0: yyyy-MM-dd hh:mm:ss.fff} Debug Section END: {1} *******", DateTime.Now, footerText);
      fullHeaderText = string.Concat(fullHeaderText,
        new string('*', DebugDelimiterTextWidth - fullHeaderText.Length));
      Write(fullHeaderText);
      Write(new string('*', DebugDelimiterTextWidth));
      Write(new string('*', DebugDelimiterTextWidth));
      Write(new string('*', DebugDelimiterTextWidth));
      Write(Environment.NewLine);
      Write(Environment.NewLine);
    }

    public static void Write(string text)
    {
      using (StreamWriter sw = File.AppendText(path))
      {
        sw.WriteLine(text);
      }
    }
  }
}

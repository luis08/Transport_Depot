using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TransportDepot.Data
{
  static class Debugger
  {
    private const string path = @"C:\Sites\debug_wcf.txt";
    private const int DebugDelimiterTextWidth = 200;
    public static void WriteDebugHeader(string headerText)
    {
      Write(new string('*', DebugDelimiterTextWidth));
      var fullHeaderText = string.Format("** {0: yyyy-MM-dd hh:mm:ss.fff} Debug Section Start: {1} *******", DateTime.Now, headerText);
      fullHeaderText = string.Concat(fullHeaderText,
        new string('*', DebugDelimiterTextWidth - fullHeaderText.Length));
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

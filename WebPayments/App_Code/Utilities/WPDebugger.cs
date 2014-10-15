using System;
using System.IO;

public static class WPDebugger
{
  public static void DebugPrintStart()
  {
    WPDebugger.Print(Environment.NewLine);
    WPDebugger.Print(new string('*', 100));
    WPDebugger.Print(string.Format("Time: {0}", DateTime.Now.ToString("yyyymmdddd hh:mm:ss.fff")));
    WPDebugger.Print(string.Empty);
  }
  public static void DebugPrintEnd()
  {
    WPDebugger.Print("---------------------------------------------------------");
    WPDebugger.Print("End Debug");
    WPDebugger.Print(new string('*', 100));
  }

  public static void DebugPrint(string text)
  {
    WPDebugger.Print(text);
  }
  private static void Print(string text)
  { 
    var path = @"C:\transport_depot\debug.txt";
    using (var writer = new StreamWriter(path, true))
    {
      writer.WriteLine(text);
    }
  }
}
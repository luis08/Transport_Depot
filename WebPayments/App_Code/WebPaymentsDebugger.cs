using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

/// <summary>
/// Summary description for WebPaymentsDebugger
/// </summary>
public static class WebPaymentsDebugger
{
  public static void UseAnotherFile()
  { 
  
  }

  public static void DebugWrite(string text)
  {
    using (StreamWriter sw = File.AppendText(path))
    {
      sw.WriteLine(text);
    }	
    
  }
  private const string path = @"C:\Transport_Depot\debug.txt";
}
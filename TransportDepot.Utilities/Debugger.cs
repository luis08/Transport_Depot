
using System.IO;
namespace TransportDepot.Utilities
{
  public static class Debugger
  {
    private const string path = @"C:\Transport_Depot\debug_wcf.txt";
    public static void Write(string text)
    {
      using (StreamWriter sw = File.AppendText(path))
      {
        sw.WriteLine(text);
      }
    }
  }
}

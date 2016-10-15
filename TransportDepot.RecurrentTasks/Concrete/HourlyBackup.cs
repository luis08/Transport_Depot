
using System.IO;
using System.Configuration;
using System;
using System.Linq;

namespace TransportDepot.RecurrentTasks.Concrete
{
  class HourlyBackup: ICommand
  {
    public void Execute()
    {
      if (ShouldBackup())
      {
        Console.WriteLine("Skipping backup");
        PerformBackup();
      }
    }

    private void PerformBackup()
    {
      string[] filePaths = GetFilePaths();
      string targetDirectory = GetTargetFolder();
      Directory.CreateDirectory(targetDirectory);
      foreach (var file in filePaths)
      {
        var targetFileName = Path.Combine(targetDirectory, Path.GetFileName(file));
        Console.WriteLine("Backing up: '" + file + "'   To: '" + targetFileName + "'");
        File.Copy(file, targetFileName);
        Console.WriteLine("Done");
      }
    }

    private bool ShouldBackup()
    {
      int dow = (int)DateTime.Now.DayOfWeek;
      var daysToExecute = ConfigurationManager.AppSettings["HourlyBackup.DaysEffective"].Split(',').Select(d=>int.Parse(d));

      return daysToExecute.Contains(dow);
    }

    private string GetTargetFolder()
    {
      var prefix = ConfigurationManager.AppSettings["HourlyBackup.TargetFolder"];
      var folderName = string.Format("{0}{1}", prefix, DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss")) ;
      return folderName;
    }

    private string[] GetFilePaths()
    {
      return ConfigurationManager.AppSettings["HourlyBackup.SourceFiles"].Split(',');
    }
  }
}

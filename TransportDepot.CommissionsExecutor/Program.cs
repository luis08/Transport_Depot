using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TransportDepot.CommissionsExecutor.CommissionsReference;
using System.ServiceModel;

namespace TransportDepot.CommissionsExecutor
{
  class Program
  {
    static void Main(string[] args)
    {
      CreateCommissions();
    }

    private static string CreateCommissions()
    {
      var timeStamp = DateTime.Now;
      var errors = string.Empty;
      using (var svc = new CommissionsReference.CommissionServiceClient())
      {
        try
        {
          Console.WriteLine("Saving new commissions");
          svc.SaveNewCommisions();
          Console.WriteLine("Done Saving new commissions");
        }
        catch (TimeoutException t)
        {
          errors = "Timeout Exception: " + t.Message;
        }
        catch (FaultException f)
        {
          errors = "Fault Exception:  " + f.Message;
        }
        catch (CommunicationException c)
        {
          errors = "Communication Exception: " + c.Message;
        }
        catch (Exception e)
        {
          errors = "Exception: " + e.Message;
        }


      }
      return errors;
    }

    private static void Test()
    {
      var timeStamp = DateTime.Now;
      using (var svc = new CommissionsReference.CommissionServiceClient())
      {
        var candidates = svc.GetCandidates();

        Log(candidates, timeStamp);
        var commissionsFromCandidates = svc.GetAllCommissions();
        Log(commissionsFromCandidates, timeStamp);
      }
    }

    private static void Log(CommissionsReference.InvoiceCommission[] commissions, DateTime timeStamp)
    {
      var doc = GetCommissionsXml(commissions);
      var fileName = GetFullFilePath("Invoice_Commissions", timeStamp);
      doc.Save(fileName);
    }

    private static void Log(CommissionsReference.CommissionCandidate[] candidates, DateTime timeStamp)
    {
      var doc = new XDocument(new XElement("candidates",
        candidates.Select(c => new XElement("candidate",
          new XAttribute("agent", c.AgentId),
          new XAttribute("tripNumber", c.TripNumber),
          new XAttribute("invoiceNumber", c.InvoiceNumber),
          new XAttribute("startDate", c.StartDate),
          new XAttribute("endDate", c.EndDate),
          new XAttribute("invoiceAmount", c.InvoiceAmount),
          new XAttribute("startLocationState", c.StartLocation.State),
          new XAttribute("endLocationState", c.EndLocatioin.State),
          new XAttribute("tractorId", string.IsNullOrEmpty( c.TractorId ) ? "NULL" : c.TractorId )
          ))));
      var fileName = GetFullFilePath("Commission_Candidates", timeStamp);
      doc.Save(fileName);
    }

    private static XDocument GetCommissionsXml(IEnumerable<InvoiceCommission> commissions)
    {

      var glDeparment = "DEPT";
      var glAccount = "ACCT";

      var xml = new XDocument(new XElement("commissions",
        commissions.Select(c => new XElement("commission",
          new XAttribute("agent", c.AgentId),
          new XAttribute("apInvoiceNumber", c.TripNumber),
          new XAttribute("arInvoiceNumber", c.InvoiceNumber),
          new XAttribute("arInvoiceAmount", c.InvoiceAmount.ToString()),
          new XAttribute("commissionTotal", decimal.Multiply(
            decimal.Multiply(c.Percent, 0.01m), c.InvoiceAmount).ToString()),
          new XAttribute("tractorId", string.IsNullOrEmpty( c.TractorId) ? "NULL" : c.TractorId),
          new XAttribute("mInvoiceDescription", GetInvoiceDescription(c)),
          new XAttribute("dueDate", c.DueDate.ToShortDateString()),
          new XAttribute("glDepartment", glDeparment),
          new XAttribute("glAccount", glAccount)))));
      return xml;
    }

    private static string GetInvoiceDescription(InvoiceCommission c)
    {
      return string.Format("Pro: {0}  Truck: {1}", c.InvoiceNumber, string.IsNullOrEmpty(c.TractorId) ? "NULL" : c.TractorId );
    }

    private static string GetFullFilePath(string fileName, DateTime timeStamp)
    {
      var folder = System.Configuration.ConfigurationManager.AppSettings["DebugFolderPath"].Trim();
      if (folder.Length == 0)
      {
        throw new InvalidOperationException("Need appsetting DebugFolderPath ");
      }
      if (!System.IO.Directory.Exists(folder))
      {
        System.IO.Directory.CreateDirectory(folder);
      }

      if (!folder[folder.Length - 1].Equals('\\'))
      { folder = folder + '\\';  }

      return string.Format("{0}{1}_{2}.txt", folder, fileName,
        timeStamp.ToString("yyyyMMdd_HHmmss_fff"));
    }

  }
}

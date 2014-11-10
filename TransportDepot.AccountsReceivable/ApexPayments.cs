using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using TransportDepot.Models.Factoring;
using TransportDepot.Utilities;
using TransportDepot.Utilities.Csv;
using System.IO;


namespace TransportDepot.AccountsReceivable
{
  public class ApexPayments : IApexPaymentsService
  {

    private Dictionary<string, int> fieldIndexes = new Dictionary<string, int>();
    private List<string> transactionTypes =
      new List<string>(System.Configuration.ConfigurationManager.AppSettings["TransportDepot.AccountsReceivable.ApexPayments.TransactionTypes"].Split('|'));
    public System.Collections.Generic.IEnumerable<FactoringPayment> ParsePayments(string path)
    {

      var svc = new CsvUtilities();
      var parsedCsv = svc.ReadCsv(path);
      return GetPayments(parsedCsv);
    }



    public IEnumerable<FactoringPayment> ReadCsv(string fileName, Stream stream)
    {
      
      var svc = new CsvUtilities();
      var parsedCsv = svc.ReadCsv(stream);
      return GetPayments(parsedCsv);
    }

    private IEnumerable<FactoringPayment> GetPayments(IEnumerable<string[]> parsedCsv)
    {
      var headers = parsedCsv.First();
      var data = parsedCsv.Skip(1);
      this.FindFields(headers);

      var payments = data.Select(r => GetApexPayment(r))
        .Where(p => p != null)
        .Where(p => this.transactionTypes.Contains(p.Type));
      Save(payments);
      return payments;
    }
    public void SavePayments(IEnumerable<FactoringPayment> payments)
    {
      throw new NotImplementedException();
      /*
       * Public Sub AppendTruckWin(ByVal cDept As String, ByVal cAcct As String, ByVal cArAcct As String, ByVal cReference As String)
            Dim sql As String
    
            sql = "INSERT INTO ArPayment ( cPronumber, niNumber, cCustomerId, dDate, cuAmount, cDept, cAcct, mDescription, cArAcct, cReference, bPosted, cTripNumber, cPayAdj, nlOrderNumber, bACHTransfer ) " & _
                  "SELECT P.Invoice_Number, 1 AS niNumber, G.cCustomerId, P.Effective_Date, P.Invoice_Amount, " & _
                  "'" & cDept & "' AS cDept, " & _
                  "'" & cAcct & "' AS cAcct, " & _
                  "'Imported Automatically' AS mDescription, " & _
                  "'" & cArAcct & "' AS cArAcct, " & _
                  "'" & cReference & "' AS cReference, " & _
                  "0 AS bPosted, G.ctripnumber, 'P' AS cPayAdj, 0 AS nlOrderNumber, 0 AS bACHTransfer " & _
                  "FROM Apex_Payment AS P INNER JOIN ArAging AS G ON P.Invoice_Number = G.cPronumber " & _
                  "WHERE ( P.Invoice_Amount = [G].[cuBalanceDue] ) AND Not P.Exclude "
    
            DoCmd.RunSQL sql
            DoCmd.RunSQL "DELETE * FROM Apex_Payment A WHERE Not A.Exclude"
        End Sub

       * */
    }

    private FactoringPayment GetApexPayment(string[] csvRow)
    {
      var scheduleIdString = csvRow[this.fieldIndexes["Schedule"]];
      var scheduleId = -1;
      if (!int.TryParse(scheduleIdString, out scheduleId))
      {
        return null;
      }
      var paymentAmount = 0.0m;
      var amountString = csvRow[this.fieldIndexes["Amount"]].Replace("$", "");
      if (!decimal.TryParse(amountString, out paymentAmount))
      {
        return null;
      }
      var effectiveDate = DateTime.Today;
      if (!DateTime.TryParse(csvRow[this.fieldIndexes["EffectiveDate"]], out effectiveDate))
      {
        return null;
      }

      var payment = new FactoringPayment
      {
        Type = csvRow[this.fieldIndexes["Type"]],
        InvoiceNumber = csvRow[this.fieldIndexes["InvoiceNumber"]],
        Schedule = scheduleId,
        CheckNumber = csvRow[this.fieldIndexes["CheckNumber"]],
        Amount = paymentAmount,
        Debtor = csvRow[this.fieldIndexes["Debtor"]],
        EffecitveDate = effectiveDate
      };
      return payment;
    }

    private void FindFields(string[] headers)
    {
      for (var i = 0; i < headers.Length; i++)
      {
        var h = headers[i];
        if (h.LastIndexOf("Type", StringComparison.OrdinalIgnoreCase) >= 0)
        { this.fieldIndexes.Add("Type", i); }
        else if (h.LastIndexOf("Sched.", StringComparison.OrdinalIgnoreCase) >= 0)
        { this.fieldIndexes.Add("Schedule", i); }
        else if (h.LastIndexOf("Check", StringComparison.OrdinalIgnoreCase) >= 0)
        { this.fieldIndexes.Add("CheckNumber", i); }
        else if (h.LastIndexOf("Eff Date", StringComparison.OrdinalIgnoreCase) >= 0)
        { this.fieldIndexes.Add("EffectiveDate", i); }
        else if (h.LastIndexOf("Invoice", StringComparison.OrdinalIgnoreCase) >= 0)
        { this.fieldIndexes.Add("InvoiceNumber", i); }
        else if (h.LastIndexOf("Inv Am", StringComparison.OrdinalIgnoreCase) >= 0)
        { this.fieldIndexes.Add("Amount", i); }
        else if (h.LastIndexOf("Debtor", StringComparison.OrdinalIgnoreCase) >= 0)
        { this.fieldIndexes.Add("Debtor", i); }
      }
    }

    private void Save(IEnumerable<FactoringPayment> payments)
    {
      var glAccount = "glAccount";
      var glDepartment = "glDepartment";
      var glArAccount = "glArAccount";
      var reference = "This is just reference";
      var description = string.Format("Automatically Imported {0}", DateTime.Now.ToString("MM/dd/yyyy h:mm tt"));

      var xmlDoc = new XDocument(new XElement("apexPayments",
        new XAttribute("glDepartment", glDepartment),
        new XAttribute("glAccount", glAccount),
        new XAttribute("glArAccount", glArAccount),
        new XAttribute("reference", reference),

        payments.Select(p => new XElement("apexPayment",
          new XAttribute("invoiceNumber", p.InvoiceNumber),
          new XAttribute("effectiveDate", p.EffecitveDate),
          new XAttribute("invoiceAmount", p.Amount),
          new XAttribute("description", string.Format("Schedule {0} - {1}", p.Schedule, description)
            )))));

      xmlDoc.Save(@"C:\Projects\WCF_Tests\WCF.Test.Upload\0_Test_Files\t.xml");

    }
  }
}

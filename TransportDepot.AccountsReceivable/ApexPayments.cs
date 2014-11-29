using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using TransportDepot.Models.Factoring;
using TransportDepot.Utilities;
using TransportDepot.Utilities.Csv;
using System.IO;
using TransportDepot.Data.Factoring;


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
        .Where(p => p != null);

      Save(payments);
      return payments;
    }

    public IEnumerable<string> GetExistingPayments(IEnumerable<FactoringPayment> payments)
    {
      var dataSource = new ApexDataSource();
      return dataSource.GetExistingPayments(payments);
    }

    public void SavePayments(IEnumerable<FactoringPayment> payments)
    {
      var dataSource = new ApexDataSource();
      dataSource.Save(payments);
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
      string paymentType = this.CleanString(csvRow[this.fieldIndexes["Type"]]);
      if (!this.transactionTypes.Contains(paymentType))
      { 
        return null; 
      }
      string invoiceNumber = this.GetInvoiceNumber(csvRow);

      var payment = new FactoringPayment
      {
        Type = paymentType,
        InvoiceNumber = invoiceNumber,
        Schedule = scheduleId,
        CheckNumber = csvRow[this.fieldIndexes["CheckNumber"]],
        Amount = paymentAmount,
        Debtor = csvRow[this.fieldIndexes["Debtor"]],
        EffectiveDate = effectiveDate
      };
      return payment;
    }

    private string GetInvoiceNumber(string[] csvRow)
    {
      var invoiceNumber = this.CleanString( csvRow[this.fieldIndexes["InvoiceNumber"]] );
      invoiceNumber = this.PadInvoiceNumber(invoiceNumber);
      string paymentType = this.CleanString(csvRow[this.fieldIndexes["Type"]]);
      
      if (string.IsNullOrEmpty(invoiceNumber))
      {
        return string.Empty;
      }


      if (this.transactionTypes.Contains(paymentType))
      {
        return invoiceNumber;
      }
      return string.Empty;
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
          new XAttribute("effectiveDate", p.EffectiveDate),
          new XAttribute("invoiceAmount", p.Amount),
          new XAttribute("description", string.Format("Schedule {0} - {1}", p.Schedule, description)
            )))));

      xmlDoc.Save(@"C:\Projects\WCF_Tests\WCF.Test.Upload\0_Test_Files\t.xml");

    }

    private string CleanString(string str)
    {
      if (string.IsNullOrEmpty(str)) { return string.Empty; }
      str = str.Trim();
      return str;
    }
    private string PadInvoiceNumber(string invoiceNumber)
    {
      var padding = System.Configuration.ConfigurationManager.AppSettings["Factoring.InvoiceLeftFill"] ?? string.Empty;
      var lengthDifference = padding.Length - invoiceNumber.Length;
      if (padding.Length == 0)
      { return invoiceNumber; }
      else if (lengthDifference < 1)
      { return invoiceNumber; }

      var newInvoiceNumber = string.Format("{0}{1}",
        padding.Substring(0, lengthDifference),
        invoiceNumber);
      return newInvoiceNumber;
    }
  }
}

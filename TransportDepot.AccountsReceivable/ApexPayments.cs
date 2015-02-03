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

    private Dictionary<string, int> _fieldIndexes = new Dictionary<string, int>();
    private List<string> transactionTypes = new List<string>();
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
      var payments = new Dictionary<string, FactoringPayment>();
      data.ToList().ForEach(csvRow =>
      {
        var payment = GetApexPayment(csvRow);
        if (payment != null)
        {
          if (payments.ContainsKey(payment.InvoiceNumber))
          {
            payments.Remove(payment.InvoiceNumber);
          }
          else
          {
            payments.Add(payment.InvoiceNumber, payment);
          }
        }
      });

      Save(payments.Values);
      return payments.Values;
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
      var builder = new ApexPaymentBuilder
      {
        Amount = csvRow[this._fieldIndexes["Amount"]].Replace("$", ""),
        CheckNumber = csvRow[this._fieldIndexes["CheckNumber"]],
        Debtor = csvRow[this._fieldIndexes["Debtor"]],
        EffectiveDate = csvRow[this._fieldIndexes["EffectiveDate"]],
        InvoiceNumber = csvRow[this._fieldIndexes["InvoiceNumber"]],
        Schedule = csvRow[this._fieldIndexes["Schedule"]],
        Type = csvRow[this._fieldIndexes["Type"]],
        ReserveChange = csvRow[this._fieldIndexes["Reserve Change"]].Replace("$", "")
      };

      var payment = builder.GetPayment();
      return payment;
    }


    private void FindFields(string[] headers)
    {
      for (var i = 0; i < headers.Length; i++)
      {
        var h = headers[i];
        if (h.LastIndexOf("Type", StringComparison.OrdinalIgnoreCase) >= 0)
        { this._fieldIndexes.Add("Type", i); }
        else if (h.LastIndexOf("Sched.", StringComparison.OrdinalIgnoreCase) >= 0)
        { this._fieldIndexes.Add("Schedule", i); }
        else if (h.LastIndexOf("Check", StringComparison.OrdinalIgnoreCase) >= 0)
        { this._fieldIndexes.Add("CheckNumber", i); }
        else if (h.LastIndexOf("Eff Date", StringComparison.OrdinalIgnoreCase) >= 0)
        { this._fieldIndexes.Add("EffectiveDate", i); }
        else if (h.LastIndexOf("Invoice", StringComparison.OrdinalIgnoreCase) >= 0)
        { this._fieldIndexes.Add("InvoiceNumber", i); }
        else if (h.LastIndexOf("Inv Am", StringComparison.OrdinalIgnoreCase) >= 0)
        { this._fieldIndexes.Add("Amount", i); }
        else if (h.LastIndexOf("Debtor", StringComparison.OrdinalIgnoreCase) >= 0)
        { this._fieldIndexes.Add("Debtor", i); }
        else if (h.LastIndexOf("Debtor", StringComparison.OrdinalIgnoreCase) >= 0)
        { this._fieldIndexes.Add("Debtor", i); }
        else if ((h.LastIndexOf("Reserve", StringComparison.OrdinalIgnoreCase) >= 0) 
          && (h.LastIndexOf("Change", StringComparison.OrdinalIgnoreCase) >= 0) )
        { this._fieldIndexes.Add("Reserve Change", i); }
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
  }
}

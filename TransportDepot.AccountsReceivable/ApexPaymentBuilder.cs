using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Models.Factoring;

namespace TransportDepot.AccountsReceivable
{
  class ApexPaymentBuilder
  {
    private List<string> _transactionTypes =
      new List<string>(System.Configuration.ConfigurationManager.AppSettings["TransportDepot.AccountsReceivable.ApexPayments.TransactionTypes"].Split('|'));
    private string Padding = System.Configuration.ConfigurationManager.AppSettings["Factoring.InvoiceLeftFill"] ?? string.Empty;

    public string Schedule { get; set; }
    public string InvoiceNumber { get; set; }
    public string Type { get; set; }
    public string Amount { get; set; }
    public string EffectiveDate { get; set; }
    public string CheckNumber { get; set; }
    public string Debtor { get; set; }
    public string ReserveChange { get; set; }

    public FactoringPayment GetPayment()
    {
      this.CleanStrings();
      if (!this.IsUsable())
      {
        return null;
      }
      else if (!this._transactionTypes.Contains(this.Type))
      {
        return null;
      }
      else if (this.IsNonFactoredType())
      {
        return this.GetNonFactoredPayment();
      }
      else
      {
        return this.GetFactoredPayement();
      }
    }

    private bool IsNonFactoredType()
    {
      return this.Type.Equals("Non Factored", StringComparison.OrdinalIgnoreCase);
    }

    private FactoringPayment GetFactoredPayement()
    {
      var scheduleId = int.MinValue;
      if (!int.TryParse(this.Schedule, out scheduleId))
      {
        return null;
      }
      var invoiceNumber = this.GetFactoredInvoiceNumber();
      if (string.IsNullOrEmpty(invoiceNumber))
      {
        return null;
      }
      var parsedAmount = TransportDepot.Data.Utilities.ParseDecimalOrZero(this.Amount);


      return this.GetPayment(scheduleId, this.InvoiceNumber, parsedAmount);
    }

    private FactoringPayment GetNonFactoredPayment()
    {
      var zeros = this.Padding;
      if (!string.IsNullOrEmpty(this.InvoiceNumber))
      {
        return null;
      }
      var scheduleId = 0;
      var invoiceLabelIdx = this.Debtor.IndexOf("Inv #:", StringComparison.OrdinalIgnoreCase);
      if (invoiceLabelIdx < 0)
      {
        return null;
      }
      var invoiceNumber = this.Debtor.Substring(invoiceLabelIdx + 7, zeros.Length);

      var parsedAmount = TransportDepot.Data.Utilities.ParseDecimalOrZero(this.ReserveChange);

      var debtorIdx = this.Debtor.IndexOf("Debtor Name: ", StringComparison.OrdinalIgnoreCase) + 13;
      var debtorEndIdx = this.Debtor.IndexOf(", Inv #", StringComparison.OrdinalIgnoreCase);
      this.Debtor = this.Debtor.Substring(debtorIdx, debtorEndIdx - debtorIdx);
      if (parsedAmount.Equals(decimal.Zero))
      {
        return null;
      }
      var payment = this.GetPayment(scheduleId, invoiceNumber, parsedAmount);
      return payment;
    }

    private FactoringPayment GetPayment(int scheduleId, string invoiceNumber, decimal amount)
    {
      
      var nullableEffectiveDate = TransportDepot.Data.Utilities.ParseDateTime(this.EffectiveDate);
      if (!nullableEffectiveDate.HasValue)
      {
        return null;
      }

      var payment = new FactoringPayment
      {
        Type = this.Type,
        InvoiceNumber = invoiceNumber,
        Schedule = scheduleId,
        Amount = amount,
        EffectiveDate = nullableEffectiveDate.Value,
        CheckNumber = this.CheckNumber,
        Debtor = this.Debtor
      };
      return payment;
    }

    private void CleanStrings()
    {
      this.Schedule = TransportDepot.Data.Utilities.Clean(this.Schedule);
      this.InvoiceNumber = TransportDepot.Data.Utilities.Clean(this.InvoiceNumber);
      this.Type = TransportDepot.Data.Utilities.Clean(this.Type);
      this.Amount = TransportDepot.Data.Utilities.Clean(this.Amount);
      this.EffectiveDate = TransportDepot.Data.Utilities.Clean(this.EffectiveDate);
      this.CheckNumber = TransportDepot.Data.Utilities.Clean(this.CheckNumber);
      this.Debtor = TransportDepot.Data.Utilities.Clean(this.Debtor);
    }

    private string GetFactoredInvoiceNumber()
    {
      if (string.IsNullOrEmpty(this.InvoiceNumber))
      {
        return string.Empty;
      }
      var invoiceNumber = this.PadInvoiceNumber(this.InvoiceNumber);
      return invoiceNumber;
    }


    private string PadInvoiceNumber(string invoiceNumber)
    {
      string padding = Padding;
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

    private bool IsUsable()
    {
      if (string.IsNullOrEmpty(this.Type))
      {
        return false;
      }
      else if (string.IsNullOrEmpty(this.Amount) &&
        string.IsNullOrEmpty(this.ReserveChange))
      {
        return false;
      }

      if (string.IsNullOrEmpty(this.EffectiveDate))
      {
        return false;
      }

      return true;
    }
  }
}

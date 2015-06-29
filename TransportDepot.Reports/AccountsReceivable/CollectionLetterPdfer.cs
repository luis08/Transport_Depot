using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Models.AR;
using System.IO;
using MigraDoc.Rendering;
using TransportDepot.Models.Business;
using TransportDepot.Models.DB;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel;

namespace TransportDepot.Reports.AccountsReceivable
{
  public class CollectionLetterPdfer: ARPdfLetter
  {
    private const char BoxChar = (char)0x2B1C;
    private const string CurrencyFormat = "#,##0.00";
    private string BoxCharacter { get { return new string(BoxChar, 1); } }
    private readonly InvoiceRowCell[] ColumnFormats =
      {
        new InvoiceRowCell
        {
          HeaderText = "Invoice",
          Width = 2.25,
          HorizontalAlignment = ParagraphAlignment.Left,
          VerticalAllingment = VerticalAlignment.Center
        },
        new InvoiceRowCell
        {
          HeaderText = "Date",
          Width = 2.0,
          HorizontalAlignment = ParagraphAlignment.Left,
          VerticalAllingment = VerticalAlignment.Center
        },
        new InvoiceRowCell
        {
          HeaderText = "Age",
          Width = 1.0,
          HorizontalAlignment = ParagraphAlignment.Center,
          VerticalAllingment = VerticalAlignment.Center
        },
        new InvoiceRowCell
        {
          HeaderText = "Your Reference",
          Width = 4.0,
          HorizontalAlignment = ParagraphAlignment.Left,
          VerticalAllingment = VerticalAlignment.Center
        },
        new InvoiceRowCell
        {
          HeaderText = "Late",
          Width = 2.25,
          HorizontalAlignment = ParagraphAlignment.Right,
          VerticalAllingment = VerticalAlignment.Center
        },
        new InvoiceRowCell
        {
          HeaderText = "Over 30",
          Width = 2.25,
          HorizontalAlignment = ParagraphAlignment.Right,
          VerticalAllingment = VerticalAlignment.Center
        },
        new InvoiceRowCell
        {
          HeaderText = "Paid",
          Width = 1.0,
          HorizontalAlignment = ParagraphAlignment.Center,
          VerticalAllingment = VerticalAlignment.Center
        },
        new InvoiceRowCell
        {
          HeaderText = "Comments",
          Width = 4.25,
          HorizontalAlignment = ParagraphAlignment.Left,
          VerticalAllingment = VerticalAlignment.Center
        }
      };

    public CollectionLetterPdfer(Company company, Customer customer) : base(company, customer) { }
    public IEnumerable<AgingInvoice> Invoices { get; set; }

    public override PdfSharp.Pdf.PdfDocument GetPdf()
    {
      this.AddLetterText();
      this.AddCustomerCreditFrame();
      this.AddInvoiceSchedule();
      var renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
      renderer.Document = this.Document;
      renderer.RenderDocument();

      var pdf = renderer.PdfDocument;
      return pdf;
    }

    private void AddInvoiceSchedule()
    {
      var invoices = this.Invoices
        .Where(i => (i.Aging > 30) || (i.Aging >= this.Customer.CreditLimitDayCount))
        .ToArray();

      var invoiceCount = invoices.Count();
      var data = new string[invoiceCount, 8];
      var boldCells = new bool[invoiceCount, 8];
      
      for(var rwIdx = 0; rwIdx < invoiceCount; rwIdx ++)
      {
        var invoice = invoices[rwIdx];
        data[rwIdx, 0] = invoice.Number;
        boldCells[rwIdx, 0] = false;
        data[rwIdx, 1] = invoice.InvoiceDate.ToShortDateString();
        boldCells[rwIdx, 1] = false;
        data[rwIdx, 2] = invoice.Aging.ToString();
        boldCells[rwIdx, 2] = false;
        data[rwIdx, 3] = invoice.CustomerReference;
        boldCells[rwIdx, 3] = false;
        data[rwIdx, 4] = this.GetLate(invoice);
        boldCells[rwIdx, 4] = true;
        data[rwIdx, 5] = this.GetOver30(invoice);
        boldCells[rwIdx, 5] = true;
        data[rwIdx, 6] = this.BoxCharacter;
        boldCells[rwIdx, 6] = true;
        data[rwIdx, 7] = string.Empty;
        boldCells[rwIdx, 4] = false;
      }

      var tableMaker = new InvoiceTableMaker
      {
        Data = data,
        BoldCell = boldCells,
        RowHeight = "0.5cm",
        Section = this.Section,
        ColumnFormats = this.ColumnFormats,
        TableTopPage1 = "14cm",
        Page1RowCount = 25,
        TableTopPage2Plus = "4cm",
        Page2PlusRowCount = 40,
        Page2PlusTitle = "Overdue Invoice Schedule"
      };
      tableMaker.AddSchedules();
    }
    
    private void AddCustomerCreditFrame()
    {
      var customerLabelFrame = this.Section.AddTextFrame();
      customerLabelFrame.Height = "1.0cm";
      customerLabelFrame.Width = "10cm";
      customerLabelFrame.Left = "8cm";
      customerLabelFrame.RelativeHorizontal = RelativeHorizontal.Margin;
      customerLabelFrame.RelativeVertical = RelativeVertical.Page;
      customerLabelFrame.Top = "4.5cm";
      
      var customerBalance = string.Format("Total Overdue Balance: ${0}",
        this.Invoices.Sum(i => i.Amount).ToString(CurrencyFormat));
      var customerCreditLimit = string.Format("Credit Limit: ${0}",
        this.Customer.CreditLimitAmount.ToString(CurrencyFormat));
      var customerCreditLimitDayCount = string.Format("Credit Period: {0} days",
        this.Customer.CreditLimitDayCount);
      var creditParagraph = customerLabelFrame.AddParagraph(
        string.Concat(customerBalance,
          Environment.NewLine,
          customerCreditLimit,
          Environment.NewLine,
          customerCreditLimitDayCount));
      creditParagraph.Format.Font.Bold = true;
    }

    private string GetInvoiceTotal(AgingInvoice invoice)
    {
      var amountString = invoice.Amount.ToString(CurrencyFormat);
      return amountString;
    }
    private string GetOver30(AgingInvoice invoice)
    {
      var amountString = this.GetInvoiceTotal(invoice);
      if (invoice.Aging <= 30)
      {
        return decimal.Zero.ToString(CurrencyFormat);
      }
      return amountString;
    }

    private string GetLate(AgingInvoice invoice)
    {
      var amountString = this.GetInvoiceTotal(invoice);
      if (invoice.Aging > 30)
      {
        return decimal.Zero.ToString(CurrencyFormat);
      }
      if (invoice.Aging >= this.Customer.CreditLimitDayCount)
      {
        return amountString;
      }
      return decimal.Zero.ToString(CurrencyFormat);
    }

    private void AddLetterText()
    {
      var letterText = GetSetting("TransportDepot.Reports.AccountsReceivable.CollectionLetterText");
      
      var frame = this.Section.AddTextFrame();
      frame.Width = "18cm";
      frame.Left = ShapePosition.Left;
      frame.RelativeHorizontal = RelativeHorizontal.Margin;
      frame.RelativeVertical = RelativeVertical.Page;
      frame.Top = "7.6cm";
      frame.AddParagraph(string.Empty);
      frame.AddParagraph(letterText);
      frame.AddParagraph(string.Empty);
      var signature = this.GetSetting("TransportDepot.Reports.AccountsReceivable.CollectionsSignature");
      frame.AddParagraph(signature);
    }

    private string GetSetting(string key)
    {
      var settingValue = System.Configuration.ConfigurationManager.AppSettings[key];
      settingValue = settingValue.Replace("\\n", Environment.NewLine);
      return settingValue;
    }

  }
}

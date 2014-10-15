

using System;
using System.Collections.Generic;
using MigraDoc.DocumentObjectModel;
using PdfSharp.Pdf;
using MigraDoc.Rendering;
using MigraDoc.DocumentObjectModel.Shapes;
using System.Linq;
using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using TransportDepot.Models.Factoring;


namespace TransportDepot.Factoring
{
  class ApexAssingmentSheet: IApexSchedule
  {
    private PdfDocument _document = null;
    public string PdfPath { get; set; }
    public int ScheduleId { get; set; }
    public DateTime Date { get; set; }
    public string ClientNumber { get; set; }
    public string ClientName { get; set; }
    public string ClientAddress1 { get; set; }
    public string ClientAddress2 { get; set; }
    public string ClientWatts { get; set; }
    public string ClientLocalPhone { get; set; }
    public string ClientFax { get; set; }
    public string UserFullName { get; set; }
    public IEnumerable<InvoiceViewModel> Invoices { get; set; }

    public PdfDocument GetAssingmentSheet()
    {
      this.InitializeDocumment();
      return this._document;
    }

    private void InitializeDocumment()
    {
      this._document = new PdfDocument();
      // Open the document to import pages from it.
      PdfDocument inputDocument = PdfReader.Open(this.PdfPath, PdfDocumentOpenMode.Import);

      // Iterate pages
      int count = inputDocument.PageCount;
      for (int idx = 0; idx < count; idx++)
      {
        // Get the page from the external document...
        PdfPage page = inputDocument.Pages[idx];
        // ...and add it to the output document.
        this._document.AddPage(page);
      }
      
      this._document.Info.Title = "Apex Assignment Sheet";
      this._document.Info.Subject = ".";
      this._document.Info.Author = this.ClientName;

      
      var graphics = XGraphics.FromPdfPage(this._document.Pages[0]);
      this.SetCompanyName(graphics);
      this.SetCliendId(graphics);
      this.SetScheduleNumber(graphics);
      this.SetScheduleDate(graphics);
      this.SetScheduleTotal(graphics);
      this.SetUserName(graphics);
      this.SetUserTitle(graphics);

    }

    private void SetCompanyName(XGraphics graphics)
    {
      var titleLabel = new SingleLineLabel
      {
        Text = this.ClientName,
        Left = 285,
        Top = 515,
        Width = 300.0,
        Height = 30.0,
        Alignment = XStringFormats.TopLeft,
        BoldFont = false,
        FontSize = 8
      };
      this.AddLabel(titleLabel, graphics);
    }

    private void SetUserTitle(XGraphics graphics)
    {
      var titleLabel = new SingleLineLabel
      {
        Text = string.Empty,
        Left = 285,
        Top = 658,
        Width = 300.0,
        Height = 30.0,
        Alignment = XStringFormats.TopLeft,
        BoldFont = false,
        FontSize = 8
      };
      this.AddLabel(titleLabel, graphics);
    }

    private void SetUserName(XGraphics graphics)
    {
      var titleLabel = new SingleLineLabel
      {
        Text = this.UserFullName,
        Left = 285.0,
        Top = 564,
        Width = 300.0,
        Height = 30.0,
        Alignment = XStringFormats.TopLeft,
        BoldFont = false,
        FontSize = 8
      };
      this.AddLabel(titleLabel, graphics);
    }

    private void SetScheduleTotal(XGraphics graphics)
    {
      var total = this.Invoices.Sum(i => i.Amount).ToString("#,##0.00");

      var scheduleTotalLabel = new SingleLineLabel
      {
        Text = total,
        Left = 260,
        Top = 340,
        Width = 120,
        Height = 15,
        Alignment = XStringFormats.TopLeft,
        BoldFont = false,
        FontSize = 8
      };
      this.AddLabel(scheduleTotalLabel, graphics);
      scheduleTotalLabel = new SingleLineLabel
      {
        Text = total,
        Left = 165,
        Top = 515,
        Width = 90.0,
        Height = 15,
        Alignment = XStringFormats.TopLeft,
        BoldFont = false,
        FontSize = 8
      };
      this.AddLabel(scheduleTotalLabel, graphics);
    }

    private void SetScheduleDate(XGraphics graphics)
    {
      var scheduleDateLabel = new SingleLineLabel
      {
        Text = this.Date.ToShortDateString(),
        Left = 485,
        Top = 245,
        Width = 90.0,
        Height = 15,
        Alignment = XStringFormats.TopLeft,
        BoldFont = false,
        FontSize = 8
      };
      this.AddLabel(scheduleDateLabel, graphics);
    }

    private void SetScheduleNumber(XGraphics graphics)
    {
      var scheduleNumberLabel = new SingleLineLabel
      {
        Text = this.ScheduleId.ToString(),
        Left = 310,
        Top = 245,
        Width = 90.0,
        Height = 15,
        Alignment = XStringFormats.TopLeft,
        BoldFont = false,
        FontSize = 8
      };
      this.AddLabel(scheduleNumberLabel, graphics);
    }

    private void SetCliendId(XGraphics graphics)
    {
      var clientIdLabel = new SingleLineLabel
      {
        Text = this.ClientNumber,
        Left = 100,
        Top = 245,
        Width = 90.0,
        Height = 15,
        Alignment = XStringFormats.TopLeft,
        BoldFont = false,
        FontSize = 8
      };
      this.AddLabel(clientIdLabel, graphics);
    }

    private void AddLabel(SingleLineLabel label, XGraphics graphics)
    {

      var font = new XFont("Verdana", label.FontSize,
        label.BoldFont ? XFontStyle.Bold : XFontStyle.Regular);
      var frame = new XRect(
          label.Left,
          label.Top,
          label.Width,
          label.Height);
      graphics.DrawString(label.Text,
        font,
        XBrushes.Black,
        frame,
        label.Alignment);
    }
  }
}

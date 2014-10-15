using System.Linq;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using System;
using System.Collections.Generic;
using PdfSharp.Pdf;
using MigraDoc.Rendering;
using TransportDepot.Models.Factoring;
namespace TransportDepot.Factoring
{

  class ApexSchedule:IApexSchedule
  {
    private Document _document = null;
    private int _pageNumber = 0;

    public int ScheduleId { get; set; }
    public DateTime Date { get; set; }
    public string ClientNumber { get; set; }
    public string ClientName { get; set; }
    public string ClientAddress1 { get; set; }
    public string ClientAddress2 { get; set; }
    public string ClientWatts { get; set; }
    public string ClientLocalPhone { get; set; }
    public string ClientFax { get; set; }

    public IEnumerable<InvoiceViewModel> Invoices { get; set; }
    
    public PdfDocument GetInvoiceSchedule()
    {
      this.InitializeDocumment();
      PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
      renderer.Document = this._document;
      renderer.RenderDocument();

      return renderer.PdfDocument;
    }

    private void InitializeDocumment()
    {
      this._document = new Document();
      this._document.Info.Title = "Apex Schedule ";
      this._document.Info.Subject = ".";
      this._document.Info.Author = this.ClientName;
      
      this.DefineStyles();
      var section = this._document.AddSection();
      this._pageNumber++;
      section.PageSetup.LeftMargin = "1.5cm";
      this.AddCompanyName(section);
      this.AddDocumentTitle(section);

      this.AddInvoices(section);
      this.AddFooter(section);
    }

    private void AddScheduleInfo(Section section)
    {
      var scheduleFrame = section.AddTextFrame();
      scheduleFrame.Top = "1.3cm";
      scheduleFrame.Width = "5.8cm";
      scheduleFrame.Left = "13.0cm";
      scheduleFrame.Height = "4.5cm";
      scheduleFrame.RelativeHorizontal = RelativeHorizontal.Margin;
      scheduleFrame.RelativeVertical = RelativeVertical.Page;
      var scheduleTable = scheduleFrame.AddTable();
      scheduleTable.Style = "Table";
      scheduleTable.Borders.Color = Colors.Transparent;
      scheduleTable.Borders.Width = 0;
      scheduleTable.Rows.LeftIndent = 0;

      var lblColumn = scheduleTable.AddColumn("2.5cm");
      var dataColumn = scheduleTable.AddColumn("3cm");

      Action<string, string> addRow = (lbl, val) =>
        {
          var row = scheduleTable.AddRow();
          row.Cells[0].AddParagraph(lbl);
          row.Cells[0].Format.Font.Bold = true;
          row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
          row.Cells[1].Format.Font.Bold = false;
          row.Cells[1].AddParagraph(val);
          row.Cells[1].Format.Alignment = ParagraphAlignment.Left;

          row.Cells[0].VerticalAlignment =
            row.Cells[1].VerticalAlignment = VerticalAlignment.Center;
          row.Cells[0].Format.Font.Size =
            row.Cells[1].Format.Font.Size = 9;
          row.Cells[0].Format.Font.Name =
            row.Cells[1].Format.Font.Name = "Verdana";
        };
      addRow("Client", this.ClientNumber);
      addRow("Schedule", this.ScheduleId.ToString());
      addRow("Date", this.Date.ToShortDateString());
      addRow("Page", this._pageNumber.ToString());
    }

    private void AddFooter(Section section)
    {
      Paragraph paragraph = section.Footers.Primary.AddParagraph();
      paragraph.AddText( string.Format("{0} - {1} {2}", 
        this.ClientName,
        this.ClientAddress1, this.ClientAddress2));
      paragraph.Format.Font.Size = 7;
      paragraph.Format.Alignment = ParagraphAlignment.Center;
      paragraph.Format.Borders.Top.Color = Colors.Black;
    }

    private void AddInvoices(Section section)
    {
      var invoiceTable = GetInvoiceTableWithHeader(section);
      var total = decimal.Zero;
      var invoiceCount = 0;
      this.Invoices.ToList().ForEach(i =>
        {
          var invoiceRow = invoiceTable.AddRow();
          invoiceRow.Height = "0.7cm";
          invoiceRow.Format.Font.Bold = false;
          invoiceRow.Format.Font.Size = 8;
          invoiceRow.Cells[0].AddParagraph(i.Date.ToShortDateString());
          invoiceRow.Cells[1].AddParagraph(i.Number);
          invoiceRow.Cells[2].AddParagraph(i.CustomerName);
          invoiceRow.Cells[3].AddParagraph(i.Amount.ToString("C"));
          invoiceRow.Cells[3].Format.Alignment = ParagraphAlignment.Right;
          invoiceRow.Cells[3].Format.RightIndent = "0.3cm";

          invoiceRow.Cells[0].VerticalAlignment =
            invoiceRow.Cells[1].VerticalAlignment =
            invoiceRow.Cells[2].VerticalAlignment =
            invoiceRow.Cells[3].VerticalAlignment = VerticalAlignment.Center;
          total = decimal.Add(total, i.Amount);
          if (++invoiceCount % 32 == 0)
          {
            section.AddPageBreak();
            this._pageNumber++;
            invoiceTable = this.GetInvoiceTableWithHeader(section);
          }
        });
      var totalRow = invoiceTable.AddRow();
      totalRow.Height = "1.5cm";
      totalRow.Format.Font.Bold = true;
      totalRow.Format.Font.Size = 8;
      totalRow.VerticalAlignment = VerticalAlignment.Bottom;
      totalRow.Cells[2].AddParagraph("Schedule Total");
      totalRow.Cells[2].Format.Alignment = ParagraphAlignment.Right;
      totalRow.Cells[2].Format.RightIndent = "0.3cm";
      
      totalRow.Cells[3].AddParagraph(total.ToString("C"));
      totalRow.Cells[3].Format.Alignment = ParagraphAlignment.Right;
      totalRow.Cells[3].Format.RightIndent = "0.3cm";
    }

    private Table GetInvoiceTableWithHeader(Section section)
    {
      this.AddCompanyAddress(section);
      this.AddScheduleInfo(section);
      this.AddPhones(section);
      var tableFrame = section.AddTextFrame();
      tableFrame.Top = "4cm";
      tableFrame.RelativeVertical = RelativeVertical.Page;

      var invoiceTable = tableFrame.AddTable();

      invoiceTable.Style = "Table";
      invoiceTable.Borders.Color = Colors.Transparent;
      invoiceTable.Borders.Width = 0;
      invoiceTable.Format.Font.Size = 8;
      this.AddInvoiceColumns(invoiceTable);
      this.AddHeaderRow(invoiceTable);

      return invoiceTable;
    }

    private void AddHeaderRow(Table invoiceTable)
    {
      var headerRow = invoiceTable.AddRow();
      headerRow.HeadingFormat = true;
      headerRow.Format.Font.Bold = true;
      headerRow.Shading.Color = Colors.LightGray;
      headerRow.Cells[0].AddParagraph("Bill Date");
      headerRow.Cells[1].AddParagraph("Invoice");
      headerRow.Cells[2].AddParagraph("Customer Name");
      headerRow.Cells[3].AddParagraph("Amount");
      headerRow.Format.Font.Size = 9;

    }

    private void AddInvoiceColumns(Table invoiceTable)
    {
      var dateColumn = invoiceTable.AddColumn("3cm");
      
      var invoiceColumn = invoiceTable.AddColumn("3cm");
      var customerColumn = invoiceTable.AddColumn("8cm");
      var totalColumn = invoiceTable.AddColumn("4cm");
      totalColumn.Format.Alignment = ParagraphAlignment.Left;

      dateColumn.Format.Alignment =
        invoiceColumn.Format.Alignment = 
        totalColumn.Format.Alignment = ParagraphAlignment.Center;
    }

    private void AddPhones(Section section)
    {
      this.AddPhone(section, "Watts:", this.ClientWatts, "4.6cm");
      this.AddPhone(section, @"Local:", this.ClientLocalPhone, "9.0cm");
      this.AddPhone(section, "Fax:", this.ClientFax, "13.5cm");
    }

    private void AddPhone(Section section, string phoneLabel, string phoneNumber, string left)
    {
      var phoneFrame = section.AddTextFrame();
      phoneFrame.Height = "2.0cm";
      phoneFrame.Width = "4.5cm";
      phoneFrame.Top = "3.0cm";
      phoneFrame.Left = left;
      phoneFrame.MarginTop = "0.15cm";
      phoneFrame.RelativeHorizontal = RelativeHorizontal.Margin;
      phoneFrame.RelativeVertical = RelativeVertical.Page;
      var phoneTable = phoneFrame.AddTable();
      phoneTable.Style = "Table";
      phoneTable.Borders.Color = Colors.Transparent;
      phoneTable.Borders.Width = 0.0;
      phoneTable.Rows.LeftIndent = 0;
      var labelColumn = phoneTable.AddColumn("1.5cm");
      labelColumn.Format.Alignment = ParagraphAlignment.Right;
      var phoneColumn = phoneTable.AddColumn("3.0cm");
      phoneColumn.Format.Alignment = ParagraphAlignment.Left;

      var phoneRow = phoneTable.AddRow();

      phoneRow.HeadingFormat = false;
      phoneRow.Cells[0].AddParagraph(phoneLabel);
      phoneRow.Cells[0].Format.Font.Bold = true;
  
      phoneRow.Cells[0].Format.Alignment = ParagraphAlignment.Right;
      phoneRow.Cells[1].AddParagraph(phoneNumber);
      phoneRow.Cells[1].Format.Font.Bold = false;

      phoneRow.Cells[1].Format.Alignment = ParagraphAlignment.Left;

      phoneRow.Cells[0].Format.Font.Size =
        phoneRow.Cells[1].Format.Font.Size = 8;
      phoneRow.Cells[0].VerticalAlignment =
        phoneRow.Cells[1].VerticalAlignment = VerticalAlignment.Center;
      phoneRow.Cells[0].Format.Font.Name =
        phoneRow.Cells[1].Format.Font.Name = "Verdana";
    }

    private void AddCompanyAddress(Section section)
    {
      var addressFrame = section.AddTextFrame();
      addressFrame.Height = "2.0cm";
      addressFrame.Width = "5cm";
      addressFrame.Top = "3.0cm";
      addressFrame.Left = "0.0cm";
      addressFrame.RelativeVertical = RelativeVertical.Page;
      addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
      var addressParagraph = addressFrame.AddParagraph();
      addressParagraph.AddText(this.ClientAddress1);
      addressParagraph.AddLineBreak();
      addressParagraph.AddText(this.ClientAddress2);
      addressParagraph.Format.Font.Size = 8;
    }

    private void AddDocumentTitle(Section section)
    {
      var p = section.Headers.Primary.AddParagraph();
      p.AddText("Assingment Sheet Schedule");
      p.Format.Alignment = ParagraphAlignment.Center;
      p.Format.Font.Bold = true;
      p.Format.Font.Size = 12;
    }

    private void AddCompanyName(Section section)
    {
      var hdrParagraph = section.Headers.Primary.AddParagraph();
      hdrParagraph.AddText(this.ClientName);
      hdrParagraph.Format.Font.Name = "Verdana";
      hdrParagraph.Format.Font.Size = 15;
      hdrParagraph.Format.Alignment = ParagraphAlignment.Center;
      hdrParagraph.Format.Font.Bold = true;
    }
    void DefineStyles()
    {
      // Get the predefined style Normal.
      Style style = this._document.Styles["Normal"];
      // Because all styles are derived from Normal, the next line changes the 
      // font of the whole document. Or, more exactly, it changes the font of
      // all styles and paragraphs that do not redefine the font.
      style.Font.Name = "Verdana";

      style = this._document.Styles[StyleNames.Heading1];
      style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Center);

      style = this._document.Styles[StyleNames.Footer];
      style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

      // Create a new style called Table based on style Normal
      style = this._document.Styles.AddStyle("Table", "Normal");
      style.Font.Name = "Verdana";
      style.Font.Size = 9;

      // Create a new style called Reference based on style Normal
      style = this._document.Styles.AddStyle("Reference", "Normal");
      style.ParagraphFormat.SpaceBefore = "5mm";
      style.ParagraphFormat.SpaceAfter = "5mm";
      style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
    }


  }
}

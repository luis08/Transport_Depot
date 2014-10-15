
using System.Collections.Generic;
using TransportDepot.Models.AR;
using TransportDepot.Models.Business;
using PdfSharp.Pdf;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel;
using System.Linq;
using MigraDoc.Rendering;
using System;


namespace TransportDepot.Reports.AccountsReceivable
{
  internal class CollectionsReportPdfer
  {
    private const string GridWidth = "23cm";
    private readonly double GridHeightInCm = 17;
    public string Type { get; set; }
    public Company Company { get; set; }
    public double FontSize { get; set; }
    private Table _table;
    public IEnumerable<CollectionsCustomerBlock> CustomerBlocks { get; set; }

    public PdfDocument GetPdf()
    {
      var document = new Document();
      this.DefineStyles(document);

      var currentSection = document.AddSection();

      currentSection.PageSetup.LeftMargin = "1.5cm";
      currentSection.PageSetup.RightMargin = "1.5cm";
      currentSection.PageSetup.TopMargin = "1cm";
      currentSection.PageSetup.BottomMargin = "1cm";
      currentSection.PageSetup.Orientation = Orientation.Landscape;

      var headerParagraph = currentSection.Headers.Primary.AddParagraph(string.Format("{0} Collections Report - {1}",  
        this.Company.Name,
        this.Type ?? string.Empty));
      headerParagraph.Format.Font.Bold = true;
      headerParagraph.Format.Font.Size = 10;

      var footerParagraph = currentSection.Footers.Primary.AddParagraph();
      footerParagraph.AddText("Page ");
      footerParagraph.AddPageField();
      footerParagraph.Format.Alignment = ParagraphAlignment.Right;
      footerParagraph.Format.Font.Size = this.FontSize;
      this._table = this.GetReportTable(currentSection);
      this.PdfCustomerBlocks();
      var renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
      renderer.Document = document;
      renderer.RenderDocument();

      return renderer.PdfDocument;
    }

    private Table GetReportTable(Section currentSection)
    {
      var tableFrame = currentSection.AddTextFrame();
      tableFrame.Top = "1.8cm";
      tableFrame.RelativeVertical = MigraDoc.DocumentObjectModel.Shapes.RelativeVertical.Page;
      var invoiceTable = tableFrame.AddTable();
      
      invoiceTable.Format.Font.Size = this.FontSize;
      invoiceTable.Format.Font.Name = "Verdana";
      invoiceTable.AddColumn(GridWidth);
      
      var headerRow = invoiceTable.AddRow();
      headerRow.HeadingFormat = false;
      headerRow.Format.Shading.Color = Colors.LightGray;
      var headerTableFrame = headerRow.Cells[0].AddTextFrame();
      headerTableFrame.Height = CustomerBlockPdfer.HeaderRowHeight + .02;
      var headerTable = headerTableFrame.AddTable();
      headerTable.AddColumn(CustomerBlockPdfer.ColumnWidths.Dummy);
      headerTable.AddColumn(CustomerBlockPdfer.ColumnWidths.InvoiceNumber);
      headerTable.AddColumn(CustomerBlockPdfer.ColumnWidths.Date);
      headerTable.AddColumn(CustomerBlockPdfer.ColumnWidths.Age);
      headerTable.AddColumn(CustomerBlockPdfer.ColumnWidths.Reference);
      headerTable.AddColumn(CustomerBlockPdfer.ColumnWidths.Current);
      headerTable.AddColumn(CustomerBlockPdfer.ColumnWidths.Late);
      headerTable.AddColumn(CustomerBlockPdfer.ColumnWidths.Over30);
      
      var headerRw = headerTable.AddRow();
      headerRw.HeadingFormat = true;
      headerRw.Format.Shading.Color = Colors.Transparent;
      headerRw.Cells[0].AddParagraph("Customer");
      headerRw.Cells[0].Format.Alignment = ParagraphAlignment.Left;
      headerRw.Cells[1].AddParagraph("Invoice");
      headerRw.Cells[1].Format.Alignment = ParagraphAlignment.Center;
      headerRw.Cells[2].AddParagraph("Date");
      headerRw.Cells[2].Format.Alignment = ParagraphAlignment.Center;
      headerRw.Cells[3].AddParagraph("Aging");
      headerRw.Cells[3].Format.Alignment = ParagraphAlignment.Center;
      headerRw.Cells[4].AddParagraph("Customer Reference");
      headerRw.Cells[4].Format.Alignment = ParagraphAlignment.Left;
      headerRw.Cells[5].AddParagraph("Current");
      headerRw.Cells[5].Format.Alignment = ParagraphAlignment.Right;
      headerRw.Cells[6].AddParagraph("Late");
      headerRw.Cells[6].Format.Alignment = ParagraphAlignment.Right;
      headerRw.Cells[7].AddParagraph("Over 30");
      headerRw.Cells[7].Format.Alignment = ParagraphAlignment.Right;
      
      headerRw.Format.Font.Size = this.FontSize;
      headerRw.Format.Font.Bold = true;
      headerRw.Height = CustomerBlockPdfer.HeaderRowHeight;
      headerRw.Borders.Width = 0;
      invoiceTable.AddRow();
      return invoiceTable;
    }

    private void PdfCustomerBlocks()
    {
      var pageHeightRemaining = GridHeightInCm;
      LinkedList<CustomerBlockPdfer> customerBlockPdfers = new LinkedList<CustomerBlockPdfer>();

      this.CustomerBlocks.ToList().ForEach(b =>
      {
        pageHeightRemaining = this.AddCustomerBlockPdfers(customerBlockPdfers, b, pageHeightRemaining);
      });
      customerBlockPdfers.ToList().ForEach(b => b.Render());
    }

    private double AddCustomerBlockPdfers(LinkedList<CustomerBlockPdfer> blockPdfers, CollectionsCustomerBlock block, double pageHeightRemaining)
    {
    
      var pageBreakThreshold = CustomerBlockPdfer.HeaderRowHeight + 2 * CustomerBlockPdfer.InvoiceRowHeight + 0.02;
      var totalIncludedInvoiceCountForBlock = 0;
      var originalInvoiceCount = block.Invoices.Count();
      
      CustomerBlockPdfer blockPdfer = null;
      do
      {
        if (pageHeightRemaining < pageBreakThreshold)
        {
          this._table.Section.AddPageBreak();
          this._table = this.GetReportTable(this._table.Section);
          pageHeightRemaining = GridHeightInCm;
        }
        var nonDetailBlockHeight = CustomerBlockPdfer.HeaderRowHeight
          + CustomerBlockPdfer.InvoiceRowHeight * (block.LastCustomerBlock ? 2 : 1);
        var displayableInvoiceCount = (int)Math.Floor((pageHeightRemaining - nonDetailBlockHeight) / CustomerBlockPdfer.InvoiceRowHeight);
        var invoicesToInclude = block.Invoices.Skip(totalIncludedInvoiceCountForBlock).Take(displayableInvoiceCount);
        totalIncludedInvoiceCountForBlock += invoicesToInclude.Count();
        var thisBlockInvoiceCount = invoicesToInclude.Count();
        pageHeightRemaining = pageHeightRemaining - (nonDetailBlockHeight + CustomerBlockPdfer.InvoiceRowHeight * thisBlockInvoiceCount);


        blockPdfer = new CustomerBlockPdfer
        {
          Block = new CollectionsCustomerBlock
          {
            CustomerName = block.CustomerName,
            CustomerPhone = block.CustomerPhone,
            CreditPeriod = block.CreditPeriod,
            Email = block.Email,
            Balance = block.Balance,
            BalanceCurrent = block.BalanceCurrent,
            BalanceLate = block.BalanceLate,
            BalanceOver30 = block.BalanceOver30,
            Invoices = invoicesToInclude,
            LastCustomerBlock = false
          },
          FontSize = this.FontSize,
          Table = this._table,
          TotalCustomerInvoiceCount = originalInvoiceCount
        };
        blockPdfers.AddLast(blockPdfer);

      } while (totalIncludedInvoiceCountForBlock < originalInvoiceCount);
      if (blockPdfer.Block!= null)
      {
        blockPdfer.Block.LastCustomerBlock = true;
      }
      return pageHeightRemaining;
    }

    private void DefineStyles(Document document)
    {
      // Get the predefined style Normal.
      Style style = document.Styles["Normal"];
      // Because all styles are derived from Normal, the next line changes the 
      // font of the whole document. Or, more exactly, it changes the font of
      // all styles and paragraphs that do not redefine the font.
      style.Font.Name = "Verdana";

      style = document.Styles[StyleNames.Heading1];
      style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Center);

      style = document.Styles[StyleNames.Footer];
      style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

      // Create a new style called Table based on style Normal
      style = document.Styles.AddStyle("Table", "Normal");
      style.Font.Name = "Verdana";


      // Create a new style called Reference based on style Normal
      style = document.Styles.AddStyle("Reference", "Normal");
      style.ParagraphFormat.SpaceBefore = "5mm";
      style.ParagraphFormat.SpaceAfter = "5mm";
      style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
    }

  }
}

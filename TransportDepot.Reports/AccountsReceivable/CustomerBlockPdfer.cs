using System.Collections.Generic;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel;
using System.Linq;
using TransportDepot.Reports.Generic;
using MigraDoc.DocumentObjectModel.Shapes;

namespace TransportDepot.Reports.AccountsReceivable
{
  internal class CustomerBlockPdfer
  {
    internal static class ColumnWidths
    {
      public static string Dummy { get { return "5cm"; } }

      public static string InvoiceNumber { get { return "2cm"; } }

      public static string Date { get { return "2cm"; } }

      public static string Age { get { return "1.25cm"; } }

      public static string Reference { get { return "6cm"; } }

      public static string Current { get { return "2.25cm"; } }

      public static string Late { get { return "2.25cm"; } }

      public static string Over30 { get { return "2.25cm"; } }
    }
    public static readonly double InvoiceRowHeight = 0.5;
    public static readonly double HeaderRowHeight = 0.68;
    public CollectionsCustomerBlock Block { get; set; }

    public Table Table { get; set; }
    public double FontSize { get; set; }
    public int TotalCustomerInvoiceCount { get; set; }
    public void Render()
    {
      this.RenderHeader();
      var invoiceRow = this.Table.AddRow();
      var invoiceFrame = invoiceRow.Cells[0].AddTextFrame();
      double tableHeight = this.Block.Invoices.Count() * InvoiceRowHeight + 
        (this.Block.LastCustomerBlock ? InvoiceRowHeight /*footer*/ : 0.0);
      invoiceFrame.Height = string.Format("{0}cm", tableHeight);
      var table = this.GetInvoiceTable(invoiceFrame);
      table.Format.Font.Size = this.FontSize;
      this.RenderInvoices(table);
      if (this.Block.LastCustomerBlock)
      {
        this.RenderFooter(table);
      }
      if (table.Rows.Count > 0)
      {
        invoiceFrame.Height = table.Rows.Count * table.Rows[0].Height ;
      }
      
    }

    private void RenderFooter(Table invoiceTable)
    {
      var rw = invoiceTable.AddRow();
      rw.Format.Font.Bold = true; 
      rw.Cells[0].AddParagraph(string.Format("Invoice Count: {0} -- Total Balance: ${1}",
        this.TotalCustomerInvoiceCount,
        this.Block.Balance.ToString(DataFormats.Currency)));
      rw.Cells[0].MergeRight = 3;
      rw.Cells[4].AddParagraph(string.Empty);
      rw.Cells[5].AddParagraph(this.Block.BalanceCurrent.ToString(DataFormats.Currency));
      rw.Cells[6].AddParagraph(this.Block.BalanceLate.ToString(DataFormats.Currency));
      rw.Cells[7].AddParagraph(this.Block.BalanceOver30.ToString(DataFormats.Currency));
      rw.Cells[5].Format.Borders.Top.Color =
        rw.Cells[6].Format.Borders.Top.Color =
        rw.Cells[7].Format.Borders.Top.Color = Colors.Black;
      rw.Cells[4].Format.Alignment = ParagraphAlignment.Right;
    }

    /// <summary>
    /// Outer table
    /// </summary>
    private void RenderHeader()
    {
      var customerRow =this.Table.AddRow();
      customerRow.Borders.Top.Color = Colors.Black;
      customerRow.Borders.Top.Width = 0.5;
      var customerCell = customerRow.Cells[0];
      
      
      var customerParagraph = customerCell.AddParagraph(this.Block.CustomerName);
      customerParagraph.Format.Font.Bold = true;
      customerParagraph.Format.Borders.Width = 0;
      var line2Paragraph = customerCell.AddParagraph();
      
      line2Paragraph.AddFormattedText("Ph: ", TextFormat.Bold);
      line2Paragraph.AddText(this.Block.CustomerPhone);
      line2Paragraph.AddFormattedText("  Terms: ", TextFormat.Bold);
      line2Paragraph.AddText(this.Block.CreditPeriod.ToString());
    }

    /// <summary>
    /// Renders invoices in nested table
    /// </summary>
    private void RenderInvoices(Table invoiceTable)
    {

      this.Block.Invoices.ToList().ForEach(i =>
        {
          var rw = invoiceTable.AddRow();
          rw.Height = string.Format("{0}cm", InvoiceRowHeight);
          rw.TopPadding = rw.BottomPadding = 0;

          rw.Cells[0].AddParagraph(string.Empty);
          rw.Cells[1].AddParagraph(i.InvoiceNumber);
          rw.Cells[2].AddParagraph(i.Date.ToShortDateString());
          rw.Cells[3].AddParagraph(i.Aging.ToString());
          rw.Cells[4].AddParagraph(i.Reference);
          rw.Cells[5].AddParagraph(i.Current.ToString(DataFormats.Currency));
          rw.Cells[6].AddParagraph(i.Late.ToString(DataFormats.Currency));
          rw.Cells[7].AddParagraph(i.OverThirty.ToString(DataFormats.Currency));
        });

    }

    private MigraDoc.DocumentObjectModel.Tables.Table GetInvoiceTable(TextFrame invoiceFrame)
    {
      var invoiceTable = invoiceFrame.AddTable();
      invoiceTable.Borders.Width = 0.0;
      
      var dummyColumn = invoiceTable.AddColumn(ColumnWidths.Dummy);
      var invoiceNumberCol = invoiceTable.AddColumn(ColumnWidths.InvoiceNumber);
      var invoiceDateCol = invoiceTable.AddColumn(ColumnWidths.Date);
      var invoiceAgeCol = invoiceTable.AddColumn(ColumnWidths.Age);
      var customerRefCol = invoiceTable.AddColumn(ColumnWidths.Reference);
      var currentCol = invoiceTable.AddColumn(ColumnWidths.Current);
      var lateCol = invoiceTable.AddColumn(ColumnWidths.Late);
      var over30Col = invoiceTable.AddColumn(ColumnWidths.Over30);
      
      invoiceNumberCol.Format.Alignment =
        invoiceDateCol.Format.Alignment =
        invoiceAgeCol.Format.Alignment = ParagraphAlignment.Center;
      currentCol.Format.Alignment =
        lateCol.Format.Alignment =
        over30Col.Format.Alignment = ParagraphAlignment.Right;


      return invoiceTable;
    }



  }
}

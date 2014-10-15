using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;

namespace TransportDepot.Reports.AccountsReceivable
{
  internal class InvoiceRowCell
  {
    public string HeaderText { get; set; }
    public ParagraphAlignment HorizontalAlignment { get; set; }
    public VerticalAlignment VerticalAllingment { get; set; }
    public double Width { get; set; }
  }
}

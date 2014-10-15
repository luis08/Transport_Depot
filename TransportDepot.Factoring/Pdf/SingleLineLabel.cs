
using MigraDoc.DocumentObjectModel;
using PdfSharp.Drawing;

namespace TransportDepot.Factoring
{
  class SingleLineLabel
  {
    public string Text { get; set; }
    public double Left { get; set; }
    public double Top { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public XStringFormat Alignment { get; set; }
    public bool BoldFont { get; set; }
    public int FontSize { get; set; }
  }
}

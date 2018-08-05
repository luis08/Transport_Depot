using System;
using System.IO;
using System.Linq;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;

namespace TransportDepot.Reports.Generic
{
  public class GenericReport
  {
    
    
    private string RowHeightString
    {
      get { return string.Format("{0:0.0#}cm", this.RowHeight); }
    }

    public GenericReport()
    { 
      this.RowHeight = 0.5; 
      this.DetailFontSize = 9; 
      this.RowsPerPage = 30;
      this.Orientation = MigraDoc.DocumentObjectModel.Orientation.Portrait;
    }

    public double RowHeight { get; set; }
    public int RowsPerPage { get; set; }

    public double DetailFontSize { get; set; }

    public enum CellHorizontalAlignment
    {
      Left, Center, Right
    }
    public enum CellVerticalAlignment
    {
      Top, Middle, Bottom
    }

    public string ReportTitle { get; set; }
    public string FooterLeft { get; set; }
    public string FooterCenter { get; set; }
    public string HeaderLeft { get; set; }
    public string[] HeaderRight { get; set; }
    public double[] ColumnWidths { get; set; }

    /// <summary>
    /// The first row contains header labels.
    /// </summary>
    public string[,] ReportData { get; set; }

    public bool[,] HighLightFields { get; set; }
    public CellHorizontalAlignment[] HorizontalAlignment { get; set; }
    public CellVerticalAlignment[] VerticalAlignment { get; set; }
    public MigraDoc.DocumentObjectModel.Orientation Orientation { get; set; }

    public int ColumnCount
    {
      get
      {
        if (this.ReportData == null)
        {
          throw new InvalidOperationException("No data");
        }
        else if (this.HorizontalAlignment.Length == 0)
        {
          throw new InvalidOperationException("No Horizontal Alignment specified!");
        }
        else if (this.VerticalAlignment.Length == 0)
        {
          throw new InvalidOperationException("No Vertical Alignment specified");
        }

        var dataColCnt = this.ReportData.GetLength(1);
        var verAlignLen = this.VerticalAlignment.Length;
        var horAlignLen = this.HorizontalAlignment.Length;

        if (dataColCnt.Equals(verAlignLen).Equals(horAlignLen))
        {
          return dataColCnt;
        }
        throw new InvalidOperationException("Column counts do not match");
      }
    }

    public Stream GetDocumentStream()
    {
      var pdf = GetPdf();
      var stream = new MemoryStream();
      pdf.Save(stream, false);
      stream.Position = 0;
      return stream;
    }

    public PdfDocument GetPdf()
    {
      this.ValidateArrays();
      var document = this.GetMigraDoc();
      var renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
      renderer.Document = document;
      renderer.RenderDocument();

      var pdf = renderer.PdfDocument;
      return pdf;
    }

    private void ValidateArrays()
    {
      var rows = this.ReportData.GetLength(0);
      var cols = this.ReportData.GetLength(1);

      if (this.HighlightSpecified())
      {
        if( this.HighLightFields.GetLength(0)!= rows)
        {
          throw new InvalidOperationException("Data Rows do not match the Highlight Rows");
        }
        else if( this.HighLightFields.GetLength(1)!= cols)
        {
          throw new InvalidOperationException("Data Columns do not match the Highlight Columns");
        }
      }
      if (this.HorizontalAlignment.Length != cols)
      {
        throw new InvalidOperationException(
          string.Format("Data Columns do not match the Horizontal Alignment columns.  Data Columns: {0}     Horizontal Aligment Columns {1}",
          cols, this.HorizontalAlignment.Length));
      }
      else if (this.VerticalAlignment.Length != cols)
      {
        throw new InvalidOperationException("Data Columns do not match the Vertical Alingment columns");
      }

    }

    private Document GetMigraDoc()
    {
      var document = new Document();
      this.DefineStyles(document);

      var currentSection = document.AddSection();

      currentSection.PageSetup.LeftMargin = "1.5cm";
      currentSection.PageSetup.RightMargin = "1.5cm";
      currentSection.PageSetup.Orientation = this.Orientation;
      
      this.AddHeader(currentSection);
      this.AddFooter(currentSection);
      this.AddBody(currentSection);

      return document;
    }

    private void AddFooter(Section currentSection)
    {
      currentSection.PageSetup.StartingNumber = 1;
      var tbl = currentSection.Footers.Primary.AddTable();
      tbl.Style = "Table";

      tbl.Borders.Top.Width = 1;
      var leftCol = tbl.AddColumn(Formats.Landscape.FooterLeftWidth);
      var centerCol = tbl.AddColumn(Formats.Landscape.FooterCenterWidth);
      var rightCol = tbl.AddColumn(Formats.Landscape.FooterRightWidth);

      var rw = tbl.AddRow();
      rw.Cells[0].AddParagraph(this.FooterLeft);
      rw.Cells[0].Format.Alignment = ParagraphAlignment.Left;
      rw.Cells[0].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
      rw.Cells[1].AddParagraph(this.FooterCenter);
      rw.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
      rw.Cells[1].Format.Alignment = ParagraphAlignment.Center;
      
      rw.Cells[2].Format.Alignment = ParagraphAlignment.Right;
      rw.Cells[2].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
      var paragraph = rw.Cells[2].AddParagraph(); ;
      paragraph.AddTab();
      paragraph.AddPageField();
      
    }

    private bool HighlightSpecified()
    {
      if (this.HighLightFields == null)
      {
        return false;
      }
      return true;
    }

    private void AddBody(Section currentSection)
    {
      Color highLightColor = this.GetHighlightColor();
      var tbl = this.GetTable(currentSection);
      var totalRows =this.ReportData.GetLength(0);
      for (int rwIdx = 1; rwIdx < totalRows; rwIdx++)
      {
        var rw = tbl.AddRow();
        rw.Height = RowHeightString;
        if (rwIdx % 2 == 0)
        {
          rw.Shading.Color = new Color(240, 240, 240);
        }
        for (int colIdx = 0; colIdx < this.ReportData.GetLength(1); colIdx++)
        {
          var textContent = string.IsNullOrEmpty(this.ReportData[rwIdx, colIdx]) ?
            string.Empty:
            this.ReportData[rwIdx, colIdx];
          rw.Cells[colIdx].AddParagraph(textContent);
          rw.Cells[colIdx].Format.Alignment = this.GetHorizontalAlignment(colIdx);
          rw.Cells[colIdx].VerticalAlignment = this.GetVerticalAlignment(colIdx);
          if (this.HighlightSpecified() && this.HighLightFields[rwIdx, colIdx])
          {
            rw.Cells[colIdx].Format.Font.Bold = true;
            rw.Cells[colIdx].Format.Shading.Color = highLightColor;
          }
        }
        if ((rwIdx % RowsPerPage == 0)
          && rwIdx < (totalRows -1))
        {
          currentSection.AddPageBreak();
          tbl = this.GetTable(currentSection);
        }
      }
    }

    private Color GetHighlightColor()
    {
      var settingValue = this.GetSetting("TransportDepot.Reports.Safety.GenericReport.HighlightRGB");
      var defaultColor = new Color(255, 255, 0);
      if (settingValue.Trim().Equals(string.Empty))
      {
        return defaultColor;
      }
      var settingsArray = settingValue.Split(',');
      if (settingValue.Length != 3)
      {
        return defaultColor;
      }

      Func<string, byte> parseRgbElement = (c) =>
        {
          byte color = 0;
          if (!byte.TryParse(c, out color))
          {
            return 255;
          }
          return color;
        };

      var red = parseRgbElement(settingsArray[0]);
      var blue = parseRgbElement(settingsArray[1]);
      var green = parseRgbElement(settingsArray[2]);
      return new Color(red, blue, green);
    }

    private Table GetTable(Section currentSection)
    {
      MigraDoc.DocumentObjectModel.Tables.Row rw = null;
      var tableFrame = currentSection.AddTextFrame();
      tableFrame.Top = "3cm";
      tableFrame.RelativeVertical = MigraDoc.DocumentObjectModel.Shapes.RelativeVertical.Page;
      var tbl = tableFrame.AddTable();
      tbl.Style = "Table";
      tbl.Borders.Width = 0;
      for (int i = 0; i < this.ColumnWidths.Length; i++)
      {
        tbl.AddColumn(string.Format("{0}cm", this.ColumnWidths[i]));
      }
      rw = tbl.AddRow();
      rw.HeadingFormat = true;
      rw.Shading.Color = new Color(192, 192, 192);
      rw.Format.Font.Bold = true;
      for (int i = 0; i < this.ReportData.GetLength(1); i++)
      {
        var str = this.ReportData[0, i];
        rw.Cells[i].AddParagraph(str ?? string.Empty);
        rw.Cells[i].Format.Alignment = this.GetHorizontalAlignment(i);
        rw.Cells[i].VerticalAlignment = this.GetVerticalAlignment(i);
      }
      return tbl;
    }

    private MigraDoc.DocumentObjectModel.Tables.VerticalAlignment GetVerticalAlignment(int colIdx)
    {
      var cellVerticalAlignment = this.VerticalAlignment[colIdx];
      switch (cellVerticalAlignment)
      {
        case CellVerticalAlignment.Top:
          return MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Top;
        case CellVerticalAlignment.Middle:
          return MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
        case CellVerticalAlignment.Bottom:
          return MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Bottom;
      }
      throw new ArgumentException("Invalid vertical alignment: " + cellVerticalAlignment);
    }

    private ParagraphAlignment GetHorizontalAlignment(int colIdx)
    {
      var cellHorizontalAlignment = this.HorizontalAlignment[colIdx];
      switch (cellHorizontalAlignment)
      {
        case CellHorizontalAlignment.Left:
          return ParagraphAlignment.Left;
        case CellHorizontalAlignment.Center:
          return ParagraphAlignment.Center;
        case CellHorizontalAlignment.Right:
          return ParagraphAlignment.Right;
      }
      throw new ArgumentException("Invalid horizontal alignment: " + cellHorizontalAlignment);
    }


    private void AddHeader(Section currentSection)
    {
      var tbl = currentSection.Headers.Primary.AddTable();
      tbl.Style = "Table";

      var leftCol = tbl.AddColumn( Formats.GetHeaderLeftWidth(this.Orientation));
      var centerCol = tbl.AddColumn(Formats.GetHeaderCenterWidth(this.Orientation));
      var rightCol = tbl.AddColumn(Formats.GetHeaderRightWidth(this.Orientation));

      var rw = tbl.AddRow();
      rw.Cells[0].AddParagraph(string.Empty);
      rw.Cells[0].Format.Alignment = ParagraphAlignment.Left;
      rw.Cells[0].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
      rw.Cells[0].Format.Font.Size = 14;
      rw.Cells[0].Format.Font.Bold = true;
      rw.Cells[0].AddParagraph(this.HeaderLeft ?? string.Empty);
      rw.Cells[1].AddParagraph(this.ReportTitle ?? string.Empty);
      rw.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
      rw.Cells[1].Format.Alignment = ParagraphAlignment.Center;
      rw.Cells[1].Format.Font.Size = 14;
      rw.Cells[1].Format.Font.Bold = true;
      rw.Cells[2].Format.Font.Bold = true;
      this.HeaderRight.ToList().ForEach(h =>
      {
        rw.Cells[2].AddParagraph(h);
        rw.Cells[2].Format.Font.Size = 10;

      });

      rw.Cells[2].Format.Alignment = ParagraphAlignment.Right;
      rw.Cells[2].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
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
      style.Font.Size = this.DetailFontSize;

      // Create a new style called Reference based on style Normal
      style = document.Styles.AddStyle("Reference", "Normal");
      style.ParagraphFormat.SpaceBefore = "5mm";
      style.ParagraphFormat.SpaceAfter = "5mm";
      style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
    }

    private string GetSetting(string key)
    {
      string value = System.Configuration.ConfigurationManager.AppSettings[key] ?? string.Empty;
      return value;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using TransportDepot.Models.AR;
using MigraDoc.DocumentObjectModel.Shapes;

namespace TransportDepot.Reports.AccountsReceivable
{
  class InvoiceTableMaker
  {
    private Table _table;


    public int Page1RowCount { get; set; }
    public int Page2PlusRowCount { get; set; }
    public string[,] Data { get; set; }
    public bool[,] BoldCell { get; set; }
    public InvoiceRowCell[] ColumnFormats { get; set; }
    public string RowHeight { get; set; }
    public string TableTopPage1 { get; set; }
    public string TableTopPage2Plus { get; set; }
    public Section Section { get; set; }
    public string Page2PlusTitle { get; set; }

    public void AddSchedules()
    {
      this._table = this.GetTable(this.TableTopPage1);

      for (int rwIdx = 0; rwIdx < this.Data.GetLength(0); rwIdx++)
      {
        var rw = this._table.AddRow();
        rw.Height = this.RowHeight;
        if (rwIdx % 2 == 0)
        {
          rw.Shading.Color = new Color(240, 240, 240);
        }
        this.FormatRow(rw, rwIdx);
        this.SetTablePage(rwIdx);
      }
    }

    private void FormatRow(MigraDoc.DocumentObjectModel.Tables.Row rw, int rwIdx)
    {

      for (int colIdx = 0; colIdx < this.Data.GetLength(1); colIdx++)
      {
        rw.Cells[colIdx].AddParagraph(this.Data[rwIdx, colIdx]);
        rw.Cells[colIdx].Format.Alignment = this.ColumnFormats[colIdx].HorizontalAlignment;
        rw.Cells[colIdx].VerticalAlignment = this.ColumnFormats[colIdx].VerticalAllingment;
        rw.Cells[colIdx].Format.Font.Bold = this.BoldCell[rwIdx, colIdx];
      }
    }

    private void SetTablePage(int rwIdx)
    {
      if (rwIdx < this.Page1RowCount)
      {
        return;
      }
      else if (rwIdx.Equals(this.Page1RowCount))
      {
        this.Section.AddPageBreak();
        this._table = this.GetTable(this.TableTopPage2Plus);
      }
      var additionalPagesIdx = rwIdx - this.Page1RowCount;
      if (additionalPagesIdx % this.Page2PlusRowCount == 0)
      {
        this.AddPage2PlusTitle();
        if (additionalPagesIdx >= this.Page1RowCount + this.Page2PlusRowCount)
        {
          this.Section.AddPageBreak();
          this._table = this.GetTable(this.TableTopPage2Plus);
        }
      }
    }

    private void AddPage2PlusTitle()
    {
      var titleFrame = this.Section.AddTextFrame();
      titleFrame.Height = "1.0cm";
      titleFrame.Width = "6cm";
      titleFrame.Left = ShapePosition.Center;
      titleFrame.RelativeHorizontal = RelativeHorizontal.Margin;
      titleFrame.RelativeVertical = RelativeVertical.Page;
      titleFrame.Top = "3.0cm";
      var titleParagraph = titleFrame.AddParagraph(this.Page2PlusTitle);
      titleParagraph.Format.Font.Bold = true;
    }

    private Table GetTable(string tableTop)
    {
      MigraDoc.DocumentObjectModel.Tables.Row rw = null;
      var tableFrame = this.Section.AddTextFrame();
      tableFrame.Top = tableTop;
      tableFrame.RelativeVertical = MigraDoc.DocumentObjectModel.Shapes.RelativeVertical.Page;
      var tbl = tableFrame.AddTable();
      tbl.Borders.Width = 0;
      for (int i = 0; i < this.ColumnFormats.Length; i++)
      {
        var columnFormat = this.ColumnFormats[i];
        var column = tbl.AddColumn(string.Format("{0}cm", columnFormat.Width));
        column.Format.Alignment = columnFormat.HorizontalAlignment;
      }
      rw = tbl.AddRow();
      rw.HeadingFormat = true;
      rw.Shading.Color = new Color(192, 192, 192);
      rw.Format.Font.Bold = true;
      for (int i = 0; i < this.ColumnFormats.Count(); i++)
      {
        rw.Cells[i].AddParagraph(this.ColumnFormats[i].HeaderText);
        rw.Cells[i].Format.Alignment = this.ColumnFormats[i].HorizontalAlignment;
        rw.Cells[i].VerticalAlignment = this.ColumnFormats[i].VerticalAllingment;
      }
      return tbl;
    }
  }
}

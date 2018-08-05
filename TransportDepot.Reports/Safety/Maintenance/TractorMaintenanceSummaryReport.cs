
using System.Collections.Generic;
using System;
using TransportDepot.Data;
using TransportDepot.Models.Safety;
using System.Linq;
using TransportDepot.Data.Safety;
using System.IO;
using TransportDepot.Reports.Generic;
using TransportDepot.Data.DB;
using PdfSharp.Pdf;

namespace TransportDepot.Reports.Safety.Maintenance
{
  class TractorMaintenanceSummaryReport
  {
    private ArgumentUtils _argumentUtils = new ArgumentUtils();
    private readonly double[] _columnWidths = { 2.5, 2.5, 12 };
    private IEnumerable<TractorMaintenance> _data;
    private DateTime _from;
    private DateTime _to;
    private Tractor _tractor;
    private const int HeaderLineCount = 4;

    public TractorMaintenanceSummaryReport(IEnumerable<TractorMaintenance> data, DateTime from, DateTime to)
    {
      this._argumentUtils.CheckNotEmpty(data);
      this._argumentUtils.CheckNotDefault(from);
      this._argumentUtils.CheckNotDefault(to);

      this._from = from;
      this._to = to;
      this._data = data;
      this._tractor = data.First().Tractor;
    }

    public Stream GetReport()
    {
      var report = GetGenericReport();
      return report.GetDocumentStream();
    }

    public PdfDocument GetPdf()
    {
      var report = GetGenericReport();
      return report.GetPdf();
    }

    private GenericReport GetGenericReport()
    {
      var report = new GenericReport
      {
        HeaderRight = new string[] { "" },
        FooterLeft = "",
        FooterCenter = "",
        ColumnWidths = this._columnWidths,
        Orientation = MigraDoc.DocumentObjectModel.Orientation.Portrait,
        VerticalAlignment = this.GetVerticalAlignment(),
        HorizontalAlignment = this.GetHorizontalAlignment(),
        ReportData = this.GetReportData(),
        RowHeight = 1.0,
        RowsPerPage = 46
      };

      return report;
    }

    private string[,] GetReportData()
    {
      var arrayHeight = HeaderLineCount + this._data.Count() + 1;

      var data = new string[arrayHeight, this._columnWidths.Length];
      this.SetHeaderInfo(data);
      var nextRow = HeaderLineCount + 1;
      foreach (var m in this._data)
      {
        data[nextRow, 0] = m.Date.ToShortDateString();
        data[nextRow, 1] = m.Type;
        data[nextRow, 2] = m.Description;
        nextRow++;
      }
      return data;
    }

    private void SetHeaderInfo(string[,] data)
    {
      data[0, 0] = "Tractor";
      data[0, 1] = this._tractor.Unit;
      data[1, 0] = "VIN";
      data[1, 1] = this._tractor.VIN;
      data[2, 0] = "Tractor id";
      data[2, 1] = this._tractor.Id;
      data[2, 2] = string.Concat("Date Range: ", this.GetDatePerformed());

      data[4, 0] = "Date";
      data[4, 1] = "Code";
      data[4, 2] = "Description";
    }

    private string GetDatePerformed()
    {
      return this._from.ToShortDateString() + " to " + this._to.ToShortDateString();
    }

    private GenericReport.CellVerticalAlignment[] GetVerticalAlignment()
    {
      return new GenericReport.CellVerticalAlignment[]
      {
        GenericReport.CellVerticalAlignment.Middle,
        GenericReport.CellVerticalAlignment.Middle,
        GenericReport.CellVerticalAlignment.Middle
      };
    }

    private GenericReport.CellHorizontalAlignment[] GetHorizontalAlignment()
    {
      return new GenericReport.CellHorizontalAlignment[]
      {
        GenericReport.CellHorizontalAlignment.Left,
        GenericReport.CellHorizontalAlignment.Left,
        GenericReport.CellHorizontalAlignment.Left
      };
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Data.Safety;
using TransportDepot.Data;
using TransportDepot.Data.DB;
using TransportDepot.Models.Safety;
using System.IO;
using TransportDepot.Reports.Generic;
using PdfSharp.Pdf;

namespace TransportDepot.Reports.Safety.Maintenance
{
  class TrailerMaintenanceSummaryReport
  {
    private ArgumentUtils _argumentUtils = new ArgumentUtils();
    private readonly double[] _columnWidths = { 2.5, 2.5, 12 };
    private IEnumerable<TrailerMaintenance> _data;
    private Utilities _utilities = new Utilities();
    private BusinessDataSource _dataSource = new BusinessDataSource();
    private DateTime _from;
    private DateTime _to;
    private Trailer _trailer;
    private const int HeaderLineCount = 4;

    public TrailerMaintenanceSummaryReport(IEnumerable<TrailerMaintenance> data, DateTime from, DateTime to)
    {
      this._argumentUtils.CheckNotEmpty(data);
      this._argumentUtils.CheckNotDefault(from);
      this._argumentUtils.CheckNotDefault(to);

      this._from = from;
      this._to = to;
      this._data = data;
      this._trailer = data.First().Trailer;
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
        HeaderLeft = this._dataSource.GetCompany().Name,
        ReportTitle = string.Concat("Preventive", Environment.NewLine, "Maintenance Report"),
        HeaderRight = new string[] { "" },
        FooterLeft = "",
        FooterCenter = "",
        ColumnWidths = this._columnWidths,
        Orientation = MigraDoc.DocumentObjectModel.Orientation.Portrait,
        VerticalAlignment = this.GetVerticalAlignment(),
        HorizontalAlignment = this.GetHorizontalAlignment(),
        ReportData = this.GetReportData(),
        RowHeight = 1.0
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
      data[0, 0] = "Trailer";
      data[0, 1] = this._trailer.Unit;
      data[1, 0] = "VIN";
      data[1, 1] = this._trailer.VIN;
      data[2, 0] = "Trailer id";
      data[2, 1] = this._trailer.Id;
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

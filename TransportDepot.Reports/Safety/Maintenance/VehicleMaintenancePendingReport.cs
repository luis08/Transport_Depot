using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TransportDepot.Reports.Generic;
using TransportDepot.Models.Safety;
using TransportDepot.Data;
using TransportDepot.Data.Safety;

namespace TransportDepot.Reports.Safety.Maintenance
{
  class VehicleMaintenancePendingReport
  {
    private string[,] _reportData = null;
    private readonly double[] _columnWidths = { 2, 6, 6 };
    private Dictionary<Tractor, IEnumerable<DateTime>> _data;
    private Utilities _utilities = new Utilities();
    private ArgumentUtils _argumentUtils = new ArgumentUtils();

    private static class ColumnIndexes
    {
      public static int Name = 0;
      public static int MonthPending = 1;
      public static int MaintenanceType = 2;
      public static int Description = 3;
    }

    public VehicleMaintenancePendingReport(Dictionary<Tractor, IEnumerable<DateTime>> data, DateTime from, DateTime to)
    {
      this._argumentUtils.CheckNotNull(data);
      this._argumentUtils.CheckNotDefault(from);
      this._argumentUtils.CheckNotDefault(to);
      this._argumentUtils.CheckDateRangeNotEqual(from, to);

      this._data = data;
      this.From = from;
      this.To = to;
    }

    public DateTime From { get; set; }
    public DateTime To { get; set; }


    public Stream GetReport()
    {
      int arrayHeight = this._data.Values.Select(v => v.Count()).Sum() + this._data.Keys.Count * 2 + 3;
      this._reportData = new string[arrayHeight, this._columnWidths.Length];
      var report = new GenericReport
      {
        FooterLeft = DateTime.Now.ToString("MM/dd/yyyy H:mm"),
        ReportTitle = string.Empty,
        ReportData = this._reportData,
        HorizontalAlignment = this.GetHorizontalAlignment(),
        VerticalAlignment = this.GetVerticalAlignment(),
        HeaderRight = new string[] {},
        FooterCenter = DateTime.Now.ToString("t"),
        Orientation = MigraDoc.DocumentObjectModel.Orientation.Portrait,
        ColumnWidths = this._columnWidths,
        RowsPerPage = 46
      };
      PopulateColumnHeaders();
      var rowNumber = 1;
      foreach(var kvp in this._data)
      {
        foreach (var m in kvp.Value)
        {
          rowNumber++;
          this._reportData[rowNumber, ColumnIndexes.Name] = kvp.Key.Name;
          this._reportData[rowNumber, ColumnIndexes.MonthPending] = m.Date.ToString("MMMM yyyy");
        }
        if (kvp.Value.Count() > 0)
        {
          rowNumber += 2;
        }
      }
      this._utilities.WriteAppend(new string[] 
      { 
        "Total rows used:", rowNumber.ToString()
      });
      
      return report.GetDocumentStream();
    }

    private void PopulateColumnHeaders()
    {
      this._reportData[0, ColumnIndexes.Name] = "Vehicle";
      this._reportData[0, ColumnIndexes.MonthPending] = "Month Pending";
    }

    private GenericReport.CellHorizontalAlignment[] GetHorizontalAlignment()
    {
      var hAl = new GenericReport.CellHorizontalAlignment[]
      {
        GenericReport.CellHorizontalAlignment.Left,
        GenericReport.CellHorizontalAlignment.Left,
        GenericReport.CellHorizontalAlignment.Left
      };
      return hAl;
    }

    private GenericReport.CellVerticalAlignment[] GetVerticalAlignment()
    {
      var vAl = new GenericReport.CellVerticalAlignment[]
      {
        GenericReport.CellVerticalAlignment.Middle,
        GenericReport.CellVerticalAlignment.Middle,
        GenericReport.CellVerticalAlignment.Middle
      };
      return vAl;
    }
  }
}

using System;
using System.IO;
using System.Linq;
using TransportDepot.Models.Business;
using TransportDepot.Models.Dispatch;
using TransportDepot.Reports.Generic;

namespace TransportDepot.Reports.Dispatch
{
  class MovingFreightReport
  {
    private string[,] _reportData = null;
    private MovingFreightTrip[] _trips;
    private Company _company;
    private readonly double[] ColumnWidths = { 2.0, 2.7, 3.0, 2.0, 2.8, 6.4, 8.1 };

    private static class ColumnIndexes
    {
      public static int TruckTrip = 0;
      public static int TrailerDriverPhone = 1;
      public static int DriverNameCard = 2;
      public static int LineHaulPickup = 3;
      public static int Arrival = 4;
      public static int Customer = 5;
      public static int Notes = 6;
    }

    public MovingFreightReport(Company company, MovingFreightTrip [] trips )
    {
      this._company = company;
      this._trips = trips;
    }
    
    internal Stream GetMovingFreightStream()
    {
      this.PopulateReport();
      var verticalAlingment = this.GetVerticalAlignment();
      var horizontalAlignment = this.GetHorizontalAlignment();

      var report = new GenericReport
      {
        ReportTitle = string.Format("{0} Moving Freight", this._company.Name),
        DetailFontSize = 7.2,
        ColumnWidths = ColumnWidths,
        HeaderRight = new string[] {"", ""},
        FooterCenter = "",
        FooterLeft = DateTime.Now.ToString("MM/dd/yyyy H:mm"),
        HorizontalAlignment = horizontalAlignment,
        VerticalAlignment = verticalAlingment,
        RowHeight = 1.51,
        RowsPerPage = 10,
        Orientation = MigraDoc.DocumentObjectModel.Orientation.Landscape,
        ReportData = this._reportData
      };
      var pdfStream = report.GetDocumentStream();
      pdfStream.Position = 0;
      return pdfStream;
    }

    private void PopulateReport()
    {
      ValidateTrips();

      this._reportData = new string[this._trips.Length + 1, this.ColumnWidths.Length];

      PopulateHeaderColumnTitles();

      if (this._trips.Count() == 0) return;

      for (int tripIdx = 0; tripIdx < this._trips.Length; tripIdx++)
      {
        PopulateTrip(tripIdx, this._trips[tripIdx]);
      }
    }

    private void PopulateHeaderColumnTitles()
    {
      this._reportData[0, ColumnIndexes.TruckTrip] = "Truck/Trip";
      this._reportData[0, ColumnIndexes.TrailerDriverPhone] = "Trailer/Phone";
      this._reportData[0, ColumnIndexes.DriverNameCard] = "Driver/Card";
      this._reportData[0, ColumnIndexes.LineHaulPickup] = "Line/Pickup";
      this._reportData[0, ColumnIndexes.Arrival] = "Arrival";
      this._reportData[0, ColumnIndexes.Customer] = "Customer";
      this._reportData[0, ColumnIndexes.Notes] = "Location, Date & Time";
    }

    private void PopulateTrip(int tripIdx, MovingFreightTrip trip)
    {
      var driver = trip.Drivers.FirstOrDefault();
      var rowNumber = tripIdx + 1;
      this._reportData[rowNumber, ColumnIndexes.TruckTrip] =
        this.GetInSeparateLines(trip.Tractor, trip.TripNumber);
      this._reportData[rowNumber, ColumnIndexes.TrailerDriverPhone] =
        this.GetInSeparateLines(trip.Trailer, driver.CellPhone);
      this._reportData[rowNumber, ColumnIndexes.DriverNameCard] =
        this.GetInSeparateLines(driver.Name, driver.Card);
      this._reportData[rowNumber, ColumnIndexes.LineHaulPickup] =
        this.GetInSeparateLines(string.Format("{0} - {1}", trip.From.State, trip.To.State),
          this.GetDate(trip.From.DateTime));
      this._reportData[rowNumber, ColumnIndexes.Arrival] =
        this.GetInSeparateLines(this.GetDate(trip.To.DateTime), trip.To.City);
      this._reportData[rowNumber, ColumnIndexes.Customer] =
        this.GetInSeparateLines(trip.Customer.Name, trip.Customer.Phone);
      this._reportData[rowNumber, ColumnIndexes.Notes] = string.Empty;
    }

    private string GetDate(DateTime dateTime)
    {
      return dateTime.ToString("ddd MM d");
    }

    private string GetInSeparateLines(string line1, string line2)
    {
      return string.Concat(line1, Environment.NewLine, line2);
    }

    private void ValidateTrips()
    {
      if (this._trips == null) throw new InvalidOperationException("No trips defined");
    }

    private GenericReport.CellHorizontalAlignment[] GetHorizontalAlignment()
    {
      var hAl = new GenericReport.CellHorizontalAlignment[]
      {
        GenericReport.CellHorizontalAlignment.Left,
        GenericReport.CellHorizontalAlignment.Left,
        GenericReport.CellHorizontalAlignment.Left,
        GenericReport.CellHorizontalAlignment.Left,
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
        GenericReport.CellVerticalAlignment.Middle,
        GenericReport.CellVerticalAlignment.Middle,
        GenericReport.CellVerticalAlignment.Middle,
        GenericReport.CellVerticalAlignment.Middle,
        GenericReport.CellVerticalAlignment.Middle
      };
      return vAl;
    }
    
  }
}

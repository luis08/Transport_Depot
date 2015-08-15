using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Models.Business;
using TransportDepot.Models.Dispatch;
using System.IO;
using TransportDepot.Reports.Generic;

namespace TransportDepot.Reports.Dispatch
{
  class DriverContactsReport
  {
    private string[,] _reportData = null;
    private DriverContact[] _drivers;
    private Company _company;
    private readonly double[] ColumnWidths = { 7.0, 4.0, 4.0, 5.0, 4.0 };
    private bool[,] _hightlightFields;

    private static class ColumnIndexes
    {
      public static int Name = 0;
      public static int CellPhone = 1;
      public static int HomePhone = 2;
      public static int FuelCard = 3;
      public static int LessorPhone = 4;
    }
    
    public DriverContactsReport(Company company, DriverContact[] drivers)
    {
      this._company = company;
      this._drivers = drivers;
    }

    public void Initialize()
    { 
      this.PopulateReport();
    }

    internal Stream GetReportStream()
    {
      
      var verticalAlingment = this.GetVerticalAlignment();
      var horizontalAlignment = this.GetHorizontalAlignment();
      this.PopulateReport();

      var report = new GenericReport
      {
        ReportTitle = string.Format("{0} Driver Contacts ", this._company.Name),
        DetailFontSize = 7.2,
        ColumnWidths = new double[] { 6.0, 3.0, 3.0, 3.0, 3.0 },
        HeaderRight = new string[] { "", "" },
        FooterCenter = DateTime.Now.ToString("t"),
        FooterLeft = DateTime.Now.ToString("MM/dd/yyyy H:mm"),
        HorizontalAlignment = this.GetHorizontalAlignment(),
        VerticalAlignment = this.GetVerticalAlignment(),
        ReportData = this._reportData,
        HighLightFields = this._hightlightFields,
        Orientation = MigraDoc.DocumentObjectModel.Orientation.Portrait
      };
      var pdfStream = report.GetDocumentStream();
      pdfStream.Position = 0;
      return pdfStream;
    }

    private void PopulateReport()
    {
      ValidateDrivers();

      this._reportData = new string[this._drivers.Length + 1, this.ColumnWidths.Length];

      PopulateHeaderColumnTitles();

      if (this._drivers.Count() == 0) return;

      for (int tripIdx = 0; tripIdx < this._drivers.Length; tripIdx++)
      {
        PopulateDriver(tripIdx, this._drivers[tripIdx]);
      }
    }

    private void PopulateHeaderColumnTitles()
    {
      this._reportData[0, ColumnIndexes.Name] = "Name";
      this._reportData[0, ColumnIndexes.CellPhone] = "Cell";
      this._reportData[0, ColumnIndexes.HomePhone] = "Home";
      this._reportData[0, ColumnIndexes.FuelCard] = "Fuel Card";
      this._reportData[0, ColumnIndexes.LessorPhone] = "Lessor Phone";
    }

    private void PopulateDriver(int tripIdx, DriverContact driver)
    {
      var rowNumber = tripIdx + 1;
      this._reportData[rowNumber, ColumnIndexes.Name] =
        driver.Name;
      this._reportData[rowNumber, ColumnIndexes.CellPhone] =
        driver.CellPhone;
      this._reportData[rowNumber, ColumnIndexes.HomePhone] =
        driver.HomePhone;
      this._reportData[rowNumber, ColumnIndexes.FuelCard] =
        driver.Card;
      this._reportData[rowNumber, ColumnIndexes.LessorPhone] =
        driver.LessorPhone;
    }

    private string GetDate(DateTime dateTime)
    {
      return dateTime.ToString("ddd MM d");
    }


    private void ValidateDrivers()
    {
      if (this._drivers == null) throw new InvalidOperationException("No drivers defined");
    }

    private GenericReport.CellHorizontalAlignment[] GetHorizontalAlignment()
    {
      var hAl = new GenericReport.CellHorizontalAlignment[]
      {
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
        GenericReport.CellVerticalAlignment.Middle
      };
      return vAl;
    }
    
  }
}

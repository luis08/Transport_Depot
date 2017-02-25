using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Reports.Generic;
using TransportDepot.Models.Safety;
using System.IO;


namespace TransportDepot.Reports.Safety
{
  class DriverySafetyReport
  {
    private string[,] _reportData = null;
    private bool[,] _hightlightFields = null;

    public string CompanyName { get; set; }

    public Stream GetDriverSafety()
    {
      this.PopulateReport();
      var hAl = GetHorizontalAlignment();
      var vAl = GetVerticalAlignment();
      var rpt = new GenericReport
      {
        ReportTitle = this.CompanyName + Environment.NewLine + "Driver Safety",
        ColumnWidths = new double[] { 3.4, 1.3, 1.3, 1.3, 1.3, 1.3, 1.3, 1.3, 1.3, 2.6, 2.6, 2.6, 2.6, 2.6 },
        HeaderRight = new string[] { "", "" },
        FooterCenter = "Driver Safety Report",
        FooterLeft = DateTime.Now.ToString("MM/dd/yyyy H:mm"),
        HorizontalAlignment = hAl,
        VerticalAlignment = vAl,
        ReportData = this._reportData,
        HighLightFields = this._hightlightFields,
        Orientation = MigraDoc.DocumentObjectModel.Orientation.Landscape
      };
      var pdfStream = rpt.GetDocumentStream();
      pdfStream.Position = 0;
      return pdfStream;
    }
    private void PopulateReport()
    {
      Func<bool, string> blnStr = (b) =>
      {
        return b ? ((char)0x221A).ToString() : string.Empty;
      };

      IEnumerable<Driver> drivers = null;
      var safetyService = new TransportDepot.Safety.SafetyService();
      drivers = safetyService.GetAllDrivers(false).OrderBy(t=>t.Id);
      
      string[,] rptData = new string[drivers.Count() + 1, 14];
      this._hightlightFields = new bool[drivers.Count() + 1, 14];

      var driverArray = drivers.ToArray();
      var col = 0;
      rptData[0, col++] = "Id";
      rptData[0, col++] = "App";
      rptData[0, col++] = "ODH";
      rptData[0, col++] = "Drug";
      rptData[0, col++] = "Pol";
      rptData[0, col++] = "Empl";
      rptData[0, col++] = "SS";
      rptData[0, col++] = "Agr";
      rptData[0, col++] = "W9";
      rptData[0, col++] = "Physical";
      rptData[0, col++] = "Last Log";
      rptData[0, col++] = "License";
      rptData[0, col++] = "Cert. Viol";
      rptData[0, col++] = "MVR";

      for (int i = 0; i < driverArray.Length; i++)
      {
        col = 0;
        rptData[i + 1, col++] = driverArray[i].Id;
        rptData[i + 1, col++] = blnStr(driverArray[i].Application);
        rptData[i + 1, col++] = blnStr(driverArray[i].OnDutyHours);
        rptData[i + 1, col++] = blnStr(driverArray[i].DrugTest);
        rptData[i + 1, col++] = blnStr(driverArray[i].PoliceReport);
        rptData[i + 1, col++] = blnStr(driverArray[i].PreviousEmployerForm);
        rptData[i + 1, col++] = blnStr(driverArray[i].SocialSecurity);
        rptData[i + 1, col++] = blnStr(driverArray[i].Agreement);
        rptData[i + 1, col++] = blnStr(driverArray[i].W9);
        
        rptData[i + 1, col] = driverArray[i].PhysicalExamExpiration.ToShortDateString();
        this._hightlightFields[i + 1, col++] = (driverArray[i].PhysicalExamExpiration.AddDays(this.PhysicalHighlightDayCountOffset) <= DateTime.Today);
        
        rptData[i + 1, col] = driverArray[i].LastValidLogDate.ToShortDateString();
        this._hightlightFields[i + 1, col++] = (driverArray[i].LastValidLogDate.AddDays(this.LogHighlightDayCountOffset) <= DateTime.Today);
        
        rptData[i + 1, col] = driverArray[i].DriversLicenseExpiration.ToShortDateString();
        this._hightlightFields[i + 1, col++] = (driverArray[i].DriversLicenseExpiration.AddDays(this.DefaultHighlightDayCountOffset) <= DateTime.Today);

        rptData[i + 1, col] = driverArray[i].AnnualCertificationOfViolations.ToShortDateString();
        this._hightlightFields[i + 1, col++] = (driverArray[i].AnnualCertificationOfViolations.AddDays(this.COVHighlightDayCountOffset) <= DateTime.Today);
        
        rptData[i + 1, col] = driverArray[i].MVRExpiration.ToShortDateString();
        this._hightlightFields[i + 1, col++] = (driverArray[i].MVRExpiration.AddDays(this.DefaultHighlightDayCountOffset) <= DateTime.Today);
      }
      this._reportData = rptData;
    }

    private int PhysicalHighlightDayCountOffset
    {
      get
      {
        
        var highlightOffsetStr = System.Configuration.ConfigurationManager.AppSettings["TransportDepot.Reports.Safety.DriverSafety.PhysicalDayCountOffset"] ?? string.Empty;
        return this.GetOffset(highlightOffsetStr);
      }
    }
    
    private int LogHighlightDayCountOffset
    {
      get
      {
        
        var highlightOffsetStr = System.Configuration.ConfigurationManager.AppSettings["TransportDepot.Reports.Safety.DriverSafety.LogDateCountOffset"] ?? string.Empty;
        return this.GetOffset(highlightOffsetStr); 
      }
    }
    
    private int COVHighlightDayCountOffset
    {
      get
      {

        var highlightOffsetStr = System.Configuration.ConfigurationManager.AppSettings["TransportDepot.Reports.Safety.DriverSafety.COVDayCountOffset"] ?? string.Empty;
        return this.GetOffset(highlightOffsetStr); 
      }
    }
      private int DefaultHighlightDayCountOffset
    {
      get
      {

        var highlightOffsetStr = System.Configuration.ConfigurationManager.AppSettings["TransportDepot.Reports.Safety.DriverSafety.DefaultDayCountOffset"] ?? string.Empty;
        return this.GetOffset(highlightOffsetStr);
      }
    }

    private int GetOffset(string highlightOffsetStr)
    {
      int defaultHighlightOffset = 30;
      var highlightOffset = defaultHighlightOffset;
      if (!int.TryParse(highlightOffsetStr, out highlightOffset))
      {
        return defaultHighlightOffset;
      }
      return highlightOffset;
    }
    private int HighlightDayCountOffset
    {
      get
      {
        int defaultHighlightOffset = 30;
        var highlightOffsetStr = System.Configuration.ConfigurationManager.AppSettings["TransportDepot.Reports.Safety.HighlightDayCountOffset"] ?? string.Empty;
        var highlightOffset = defaultHighlightOffset;
        if (!int.TryParse(highlightOffsetStr, out highlightOffset))
        {
          return defaultHighlightOffset;
        }
        return highlightOffset;
      }
    }

    private GenericReport.CellHorizontalAlignment[] GetHorizontalAlignment()
    {
      var hAl = new GenericReport.CellHorizontalAlignment[]
      {
        GenericReport.CellHorizontalAlignment.Left,
        GenericReport.CellHorizontalAlignment.Center,
        GenericReport.CellHorizontalAlignment.Center,
        GenericReport.CellHorizontalAlignment.Center,
        GenericReport.CellHorizontalAlignment.Center,
        GenericReport.CellHorizontalAlignment.Center,
        GenericReport.CellHorizontalAlignment.Center,
        GenericReport.CellHorizontalAlignment.Center,
        GenericReport.CellHorizontalAlignment.Center,
        GenericReport.CellHorizontalAlignment.Center,
        GenericReport.CellHorizontalAlignment.Center,
        GenericReport.CellHorizontalAlignment.Center,
        GenericReport.CellHorizontalAlignment.Center,
        GenericReport.CellHorizontalAlignment.Center
      };
      return hAl;
    }

    private GenericReport.CellVerticalAlignment[] GetVerticalAlignment()
    {
      var vAl = new GenericReport.CellVerticalAlignment[]
      {
        GenericReport.CellVerticalAlignment.Top,
        GenericReport.CellVerticalAlignment.Top,
        GenericReport.CellVerticalAlignment.Top,
        GenericReport.CellVerticalAlignment.Top,
        GenericReport.CellVerticalAlignment.Top,
        GenericReport.CellVerticalAlignment.Top,
        GenericReport.CellVerticalAlignment.Top,
        GenericReport.CellVerticalAlignment.Top,
        GenericReport.CellVerticalAlignment.Top,
        GenericReport.CellVerticalAlignment.Top,
        GenericReport.CellVerticalAlignment.Top,
        GenericReport.CellVerticalAlignment.Top,
        GenericReport.CellVerticalAlignment.Top,
        GenericReport.CellVerticalAlignment.Top
      };
      return vAl;
    }
  }
}

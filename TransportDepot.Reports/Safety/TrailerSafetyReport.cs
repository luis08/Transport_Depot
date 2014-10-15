using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Models.Safety;
using TransportDepot.Reports.Generic;

namespace TransportDepot.Reports.Safety
{
  class TrailerSafetyReport
  {
    private string[,] _reportData = null;
    private bool[,] _hightlightFields = null;


    internal System.IO.Stream GetTrailerSafety()
    {
      this.PopulateReport();
      var hAl = GetHorizontalAlignment();
      var vAl = GetVerticalAlignment();
      var rpt = new GenericReport
      {
        ReportTitle = "Transport Depot Produce & Dry" + Environment.NewLine + "Trailer Safety",
        ColumnWidths = new double[] { 13.0, 3.0, 3.0, 3.0, 3.0 },
        HeaderRight = new string[] { "", "" },
        FooterCenter = this.GetFooterText(),
        FooterLeft = DateTime.Now.ToString("MM/dd/yyyy H:mm"),
        HorizontalAlignment = hAl,
        VerticalAlignment = vAl,
        ReportData = this._reportData,
        HighLightFields = this._hightlightFields
      };
      var pdfStream = rpt.GetDocumentStream();
      pdfStream.Position = 0;
      return pdfStream;
    }

    private string GetFooterText()
    {
      var footer = string.Empty;
      footer = string.Format("Trailer Safety");
      return footer;
    }

    private void PopulateReport()
    {
      Func<bool, string> blnStr = (b) =>
      {
        return b ? ((char)0x221A).ToString() : string.Empty;
      };


      IEnumerable<Trailer> trailers = null;
      var safetyService = new TransportDepot.Safety.SafetyService();
      trailers = safetyService.GetTrailers(true).OrderBy(t=>t.Id);
      
      this._reportData = new string[trailers.Count() + 1, 5];
      this._hightlightFields = new bool[trailers.Count() + 1, 5];

      var trailerArray = trailers.ToArray();
      var col = 0;
      this._reportData[0, col++] = "Id";
      this._reportData[0, col++] = "Unit";
      this._reportData[0, col++] = "Inspection";
      this._reportData[0, col++] = "Maintenance";
      this._reportData[0, col++] = "Registration";

      Action<int, int, int, DateTime> setDate = (rw, column, offset, dateValue)=>
        {
          this._reportData[rw,column] = dateValue.ToShortDateString();
          this._hightlightFields[rw,column] = (dateValue.AddDays(offset) <= DateTime.Today);
        };
      var defaultOffset = this.DefaultHighlightDayCountOffset;
      for (int i = 0; i < trailerArray.Length; i++)
      {
        col = 0;
        var lessorName = trailerArray[i].LessorOwnerName.Trim();
        var lessorId = trailerArray[i].Id;

        if (lessorName.Length == 0)
        {
          lessorName = "[NL]";
          this._hightlightFields[i + 1, col] = true;
        }
        else 
        {
          var lessorNameLength = lessorName.Length < 30 ? lessorName.Length : 20;
          lessorName = lessorName.Substring(0,lessorNameLength);
        }

        var vinNumber = trailerArray[i].VIN.Trim();
        if( vinNumber.Length == 0)
        {
          this._hightlightFields[i + 1, col] = true;
          vinNumber = "NO VIN";
        }
        this._reportData[i + 1, col++] = string.Format("{0} ({1}) {2}",
            trailerArray[i].Id,
            vinNumber, lessorName);
        this._reportData[i + 1, col++] = trailerArray[i].Unit;
        setDate(i + 1, col++, defaultOffset, trailerArray[i].InspectionDue);
        setDate(i + 1, col++, this.MaintenanceHighlightCountOffset, trailerArray[i].LastMaintenance);
        setDate(i + 1, col++, this.RegistrationHighlightDayCountOffset, trailerArray[i].RegistrationExpiration);
      }
    }

    private string GetSetting(string key)
    {
      var stringValue = System.Configuration.ConfigurationManager.AppSettings[key] ?? string.Empty;
      return stringValue;
    }

    private int MaintenanceHighlightCountOffset
    {
      get
      {
        var highlightOffsetStr = this.GetSetting("TransportDepot.Reports.Safety.TractorSafety.MaintenanceCountOffset");
        return this.GetOffset(highlightOffsetStr);
      }
    }
    private int RegistrationHighlightDayCountOffset
    {
      get
      {
        var highlightOffsetStr = this.GetSetting("TransportDepot.Reports.Safety.TractorSafety.RegistrationDayCountOffset");
        return this.GetOffset(highlightOffsetStr);
      }
    }
    private int DefaultHighlightDayCountOffset
    {
      get
      {
        var highlightOffsetStr = this.GetSetting("TransportDepot.Reports.Safety.TractorSafety.DefaultDayCountOffset");
        return this.GetOffset(highlightOffsetStr);
      }
    }
    private int HighlightDayCountOffset
    {
      get
      {
        var highlightOffsetStr = this.GetSetting( "TransportDepot.Reports.Safety.HighlightDayCountOffset");
        return this.GetOffset(highlightOffsetStr);
      }
    }
    private int GetOffset(string highlightOffsetStr)
    {
      int defaultHighlightOffset = -30;
      var highlightOffset = defaultHighlightOffset;
      if (!int.TryParse(highlightOffsetStr, out highlightOffset))
      {
        return defaultHighlightOffset;
      }
      return highlightOffset;
    }
    private GenericReport.CellHorizontalAlignment[] GetHorizontalAlignment()
    {
      var hAl = new GenericReport.CellHorizontalAlignment[]
      {
        GenericReport.CellHorizontalAlignment.Left,
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
        GenericReport.CellVerticalAlignment.Top
      };
          return vAl;
    }
  }
}

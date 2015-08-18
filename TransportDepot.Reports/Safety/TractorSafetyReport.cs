using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Models.Safety;
using TransportDepot.Reports.Generic;

namespace TransportDepot.Reports.Safety
{
  class TractorSafetyReport
  {
    private string[,] _reportData = null;
    private bool[,] _hightlightFields = null;


    internal System.IO.Stream GetTractorSafety()
    {
      this.PopulateReport();
      var hAl = GetHorizontalAlignment();
      var vAl = GetVerticalAlignment();
      var rpt = new GenericReport
      {
        ReportTitle = "Transport Depot Produce & Dry" + Environment.NewLine + "Tractor Safety",
        ColumnWidths = new double[] { 8.0, 3.0, 3.0, 3.0, 3.0, 3.0, 3.0, 1.3 },
        HeaderRight = new string[] { "", "" },
        FooterCenter = this.GetFooterText(),
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

    private string GetFooterText()
    {
      var footer = string.Empty;
      footer = string.Format("Tractor Safety");
      return footer;
    }

    private void PopulateReport()
    {
      Func<bool, string> blnStr = (b) =>
      {
        return b ? ((char)0x221A).ToString() : string.Empty;
      };


      IEnumerable<Tractor> tractors = null;
      var safetyService = new TransportDepot.Safety.SafetyService();
      tractors = safetyService.GetTractors(true).OrderBy(t=>t.Id);
      
      this._reportData = new string[tractors.Count() + 1, 8];
      this._hightlightFields = new bool[tractors.Count() + 1, 8];

      var tractorArray = tractors.ToArray();
      var col = 0;
      this._reportData[0, col++] = "Id";
      this._reportData[0, col++] = "Unit";
      this._reportData[0, col++] = "Inspection";
      this._reportData[0, col++] = "Lease Agrmt.";
      this._reportData[0, col++] = "Maintenance";
      this._reportData[0, col++] = "Registration";
      this._reportData[0, col++] = "Insurance Exp";
      this._reportData[0, col++] = "W9";

      Action<int, int, int, DateTime> setDate = (rw, column, offset, dateValue)=>
        {
          this._reportData[rw,column] = dateValue.ToShortDateString();
          this._hightlightFields[rw,column] = (dateValue.AddDays(offset) <= DateTime.Today);
        };
      var defaultOffset = this.DefaultHighlightDayCountOffset;
      for (int i = 0; i < tractorArray.Length; i++)
      {
        col = 0;
        var lessorName = tractorArray[i].LessorOwnerName.Trim();
        var lessorId = tractorArray[i].Id;

        if (lessorName.Trim().Length == 0)
        {
          lessorName = "[NL]";
          this._hightlightFields[i + 1, col] = true;
        }
        else { lessorName = string.Empty; }
        var vinNumber = tractorArray[i].VIN.Trim();
        if( vinNumber.Length == 0)
        {
          this._hightlightFields[i + 1, col] = true;
          vinNumber = "NO VIN";
        }
        this._reportData[i + 1, col++] = string.Format("{0} ({1}) {2}",
            tractorArray[i].Id,
            vinNumber, lessorName);
        this._reportData[i + 1, col++] = tractorArray[i].Unit;
        setDate(i + 1, col++, defaultOffset, tractorArray[i].InspectionDue);
        setDate(i + 1, col++, defaultOffset, tractorArray[i].LeaseAgreementDue);
        setDate(i + 1, col++, this.MaintenanceHighlightCountOffset, tractorArray[i].LastMaintenance);
        setDate(i + 1, col++, this.RegistrationHighlightDayCountOffset, tractorArray[i].RegistrationExpiration);
        setDate(i + 1, col++, defaultOffset, tractorArray[i].InsuranceExpiration);
        this._reportData[i + 1, col++] = blnStr( tractorArray[i].HasW9 );
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
        GenericReport.CellHorizontalAlignment.Center,
        GenericReport.CellHorizontalAlignment.Center,
        GenericReport.CellHorizontalAlignment.Center,
        GenericReport.CellHorizontalAlignment.Center,
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
      };
          return vAl;
    }
  }
}


using TransportDepot.Reports.Generic;
using System.ServiceModel.Web;
using System.IO;
using TransportDepot.Models.Reports;
using System;
using System.Collections.Generic;
using TransportDepot.Data.Safety;
using TransportDepot.Reports.Safety.Maintenance;
using System.Linq;
using TransportDepot.Models.Safety;
using PdfSharp.Pdf;

namespace TransportDepot.Reports.Safety
{
  class SafetyService:ISafetyService
  {
    private SafetyDataSource _safetyDataSource = new SafetyDataSource();
    private TransportDepot.Data.Utilities _utilities = new TransportDepot.Data.Utilities();
    private ArgumentUtils _argumentUtils = new ArgumentUtils();
    private PdfReportUtilities _pdfUtils = new PdfReportUtilities();

    public System.IO.Stream GetDriverSafety()
    {
      var ds = new TransportDepot.Data.DB.BusinessDataSource();
      var companyName = ds.GetCompany().Name;
      var tractorReportGenerator = new DriverySafetyReport 

      { 
        CompanyName = companyName
      };
      var report = tractorReportGenerator.GetDriverSafety();
      this.SetReturnTypePdf();
      return report;
    }

    public System.IO.Stream GetTractorSafety()
    {
      var ds = new TransportDepot.Data.DB.BusinessDataSource();
      var companyName = ds.GetCompany().Name;
      var tractorReportGenerator = new TractorSafetyReport 
      { 
        CompanyName = companyName
      };
      System.IO.Stream report = tractorReportGenerator.GetTractorSafety();
      this.SetReturnTypePdf();
      return report;
    }

    private void SetReturnTypePdf()
    {
      WebOperationContext.Current.OutgoingResponse.ContentType = "application/pdf";
    }

    public System.IO.Stream GetTrailerSafety()
    {
      var ds = new TransportDepot.Data.DB.BusinessDataSource();
      var companyName = ds.GetCompany().Name;
      var trailerReportGenerator = new TrailerSafetyReport 
      { 
        CompanyName = companyName
      };
      System.IO.Stream report = trailerReportGenerator.GetTrailerSafety();
      this.SetReturnTypePdf();
      return report;
    }

    public Stream GetTractorMaintenancePending(DateTime from, DateTime to)
    {
      var filter = new MaintenanceFilter 
      { 
        From = from,
        To = to
      };
      var maintenance = this._safetyDataSource.GetTractorMaintenance(filter);
      var maintenancePerformed = maintenance.GroupBy(t => t.Tractor.Id);
      Dictionary<Tractor, IEnumerable<DateTime>> data = new Dictionary<Tractor, IEnumerable<DateTime>>();
      foreach(var t in maintenancePerformed)
      {
        if(t.Count() > 0)
        {
          var pendingService = new TractorMaintenancePendingService(t.ToList(), filter.From, filter.To);
          var tractor = t.First().Tractor;
          var pendingDates = pendingService.GetMonthsPending();
          data.Add(tractor, pendingDates);
        }
      }
      this._utilities.WriteAppend(new string[] 
      { 
        "Tractors Count in service: ", data.Keys.Count().ToString(),
        "Maintenance Count in service: " , data.Values.Count().ToString()
      });
      if (data.Count() > 0)
      {
        var report = new VehicleMaintenancePendingReport(data, filter.From, filter.To);
        var fileName = "pendingMaintenance.pdf";
        this._pdfUtils.SetResponseToPdf(fileName);
        return report.GetReport();
      }
      return null;
    }

    public Stream GetTrailerMaintenancePending(DateTime from, DateTime to)
    {
      var filter = new MaintenanceFilter
      {
        From = from,
        To = to
      };
      var maintenance = this._safetyDataSource.GetTrailerMaintenance(filter);
      var maintenancePerformed = maintenance.GroupBy(t => t.Trailer.Id);
      var data = new Dictionary<Trailer, IEnumerable<DateTime>>();
      foreach (var t in maintenancePerformed)
      {
        if (t.Count() > 0)
        {
          var pendingService = new TrailerMaintenancePendingService(t.ToList(), filter.From, filter.To);
          var trailer = t.First().Trailer;
          var pendingDates = pendingService.GetMonthsPending();
          data.Add(trailer, pendingDates);
        }
      }

      if (data.Count() > 0)
      {
        var report = new TrailerMaintenancePendingReport(data, filter.From, filter.To);
        var fileName = "pendingMaintenance.pdf";
        this._pdfUtils.SetResponseToPdf(fileName);
        return report.GetReport();
      }
      return null;
    }

    public Stream GetTractorMaintenanceSummary(string tractorIds, DateTime from, DateTime to)
    {
      this._argumentUtils.CheckNotEmpty(tractorIds);
      this._argumentUtils.CheckDateRangeNotEqual(from, to);
      var tractorIdArray = tractorIds.Split(',').OrderBy(t => t);
      
      var pdfs = new List<PdfDocument>();
      foreach (var tractorId in tractorIdArray)
      {
        for (var d = from; d <= to; d = d.AddMonths(1))
        {
          var pdf = this.GetTractorReportPdf(tractorId, d.Year, d.Month);
          if (pdf != null)
          {
            pdfs.Add(pdf);
          }
        }
      }
      var fullReport = this._pdfUtils.Merge(pdfs);
      this._pdfUtils.SetResponseToPdf("tractor_maintenance_summaries.pdf");
      if (fullReport.PageCount.Equals(0))
      { return null; }
      return this._pdfUtils.ToStream(fullReport);
    }

    public Stream GetTrailerMaintenanceSummary(string trailerIds, DateTime from, DateTime to)
    {
      this._argumentUtils.CheckNotEmpty(trailerIds);
      this._argumentUtils.CheckDateRangeNotEqual(from, to);
      var trailerIdArray = trailerIds.Split(',');
      
      var pdfs = new List<PdfDocument>();
      foreach(var trailerId in trailerIdArray)
      {
        for (var d = from; d <= to; d = d.AddMonths(1))
        {
          var pdf = this.GetTrailerReportPdf(trailerId, d.Year, d.Month);
          if(pdf != null)
          {
            pdfs.Add(pdf);
          }
        }
      }
      var fullReport = this._pdfUtils.Merge(pdfs);
      this._pdfUtils.SetResponseToPdf("trailer_maintenance_summaries.pdf");
      if (fullReport.PageCount.Equals(0))
      { return null; }
      return this._pdfUtils.ToStream(fullReport);
    }

    private PdfDocument GetTractorReportPdf(string tractorId, int year, int month)
    {
      var starting = new DateTime(year, month, 1);
      var before = starting.AddMonths(1);
      var filter = new MaintenanceFilter
      {
        From = starting,
        To = before,
        VehicleIds = new string[] { tractorId }
      };
      var maintenance = this._safetyDataSource.GetTractorMaintenance(filter);

      if (maintenance.Count() > 0)
      {
        var report = new TractorMaintenanceSummaryReport(maintenance, starting, before.AddDays(-1));
        return report.GetPdf();
      }
      return null;
    }
    
    private PdfDocument GetTrailerReportPdf(string trailerId, int year, int month)
    {
      var starting = new DateTime(year, month, 1);
      var before = starting.AddMonths(1);
      var filter = new MaintenanceFilter
      {
        From = starting,
        To = before,
        VehicleIds = new string[] { trailerId }
      };
      var maintenance = this._safetyDataSource.GetTrailerMaintenance(filter);
      if (maintenance.Count() > 0)
      {
        var report = new TrailerMaintenanceSummaryReport(maintenance, starting, before.AddDays(-1));
        return report.GetPdf();
      }
      return null;
    }
  }
}

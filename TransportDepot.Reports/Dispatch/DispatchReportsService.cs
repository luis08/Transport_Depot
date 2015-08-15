
using TransportDepot.Models.Dispatch;
using System.Collections.Generic;
using System.Linq;
using TransportDepot.Dispatch;

namespace TransportDepot.Reports.Dispatch
{
  public class DispatchReportsService:IDispatchReports
  {
    private PdfReportUtilities _utilities = new PdfReportUtilities();
    private DispatchService _dispatchService = new DispatchService();
    private Business.BusinessService _businessService = new Business.BusinessService();

    public System.IO.Stream GetMovingFreight()
    {
      var company = this._businessService.GetCompany();
      var movingFreight = this.Coalesce(this._dispatchService.GetMovingFreight());
      var report = new MovingFreightReport(company, movingFreight);
      var reportStream = report.GetMovingFreightStream();
      this._utilities.SetResponseToPdf();
      return reportStream;
    }

    public System.IO.Stream GetDriverContacts()
    {
      var company = this._businessService.GetCompany();
      var drivers = this._dispatchService.GetDriverContacts();
      var report = new DriverContactsReport(company, drivers.ToArray());
      var pdfStream = report.GetReportStream();
      this._utilities.SetResponseToPdf();
      return pdfStream;
    }

    private MovingFreightTrip[] Coalesce(IEnumerable<MovingFreightTrip> movingFreight)
    {
      var defaultMovingFreight = new MovingFreightTrip[0];
      if (movingFreight == null) return defaultMovingFreight;
      if (movingFreight.Count().Equals(0)) return defaultMovingFreight;

      return movingFreight.ToArray();
    }
  }
}

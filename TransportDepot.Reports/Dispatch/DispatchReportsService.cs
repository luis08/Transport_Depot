
using TransportDepot.Models.Dispatch;
using System.Collections.Generic;
using System.Linq;

namespace TransportDepot.Reports.Dispatch
{
  public class DispatchReportsService:IDispatchReports
  {
    private PdfReportUtilities _utilities = new PdfReportUtilities();
    public System.IO.Stream GetMovingFreight()
    {

      var dispatchSvc = new TransportDepot.Dispatch.DispatchService();
      var businessSvc = new Business.BusinessService();
      var company = businessSvc.GetCompany();
      var movingFreight = this.Coalesce(dispatchSvc.GetMovingFreight());
      var report = new MovingFreightReport(company, movingFreight);
      var reportStream = report.GetMovingFreightStream();
      this._utilities.SetResponseToPdf();
      return reportStream;
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

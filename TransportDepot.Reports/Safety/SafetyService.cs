
using TransportDepot.Reports.Generic;
using System.ServiceModel.Web;
namespace TransportDepot.Reports.Safety
{
  class SafetyService:ISafetyService
  {
    
    public System.IO.Stream GetDriverSafety()
    {
      var tractorReportGenerator = new DriverySafetyReport();
      var report = tractorReportGenerator.GetDriverSafety();
      this.SetReturnTypePdf();
      return report;
    }

    public System.IO.Stream GetTractorSafety()
    {
      var tractorReportGenerator = new TractorSafetyReport();
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
      var trailerReportGenerator = new TrailerSafetyReport();
      System.IO.Stream report = trailerReportGenerator.GetTrailerSafety();
      this.SetReturnTypePdf();
      return report;
    }
  }
}

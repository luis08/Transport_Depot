
using TransportDepot.Reports.Generic;
using System.ServiceModel.Web;
namespace TransportDepot.Reports.Safety
{
  class SafetyService:ISafetyService
  {
    
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
  }
}

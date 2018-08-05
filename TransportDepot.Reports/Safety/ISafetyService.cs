
using System.ServiceModel;
using System.IO;
using System.ServiceModel.Web;
using TransportDepot.Models.Reports;
using System.Collections.Generic;
using System;
namespace TransportDepot.Reports.Safety
{
  [ServiceContract(Namespace = "http://transportdepot.net/reports/safety")]
  interface ISafetyService
  {
    [OperationContract]
    [WebGet(UriTemplate="DriverSafetyReport")]
    Stream GetDriverSafety();

    [OperationContract]
    [WebGet(UriTemplate = "TractorSafetyReport")]
    Stream GetTractorSafety();

    [OperationContract]
    [WebGet(UriTemplate = "TrailerSafetyReport")]
    Stream GetTrailerSafety();

    [OperationContract]
    [WebGet(UriTemplate = "TractorMaintenance/pending?from={from}&to={to}")]
    Stream GetTractorMaintenancePending(DateTime from, DateTime to);

    [OperationContract]
    [WebGet(UriTemplate = "TrailerMaintenance/pending?from={from}&to={to}")]
    Stream GetTrailerMaintenancePending(DateTime from, DateTime to);

    [OperationContract]
    [WebGet(UriTemplate = "TractorMaintenance?ids={tractorIds}&from={from}&to={to}")]
    Stream GetTractorMaintenanceSummary(string tractorIds, DateTime from, DateTime to);

    [OperationContract]
    [WebGet(UriTemplate = "TrailerMaintenance?ids={trailerIds}&from={from}&to={to}")]
    Stream GetTrailerMaintenanceSummary(string trailerIds, DateTime from, DateTime to);
  }
}

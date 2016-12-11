﻿using System.ServiceModel;
using System.ServiceModel.Web;
using TransportDepot.Models.Safety;
using System.Collections.Generic;
using TransportDepot.Models.UI;
using TransportDepot.Models.Business;
namespace TransportDepot.Safety
{

  [ServiceContract(Namespace = "http://transportdepot.net/safety")]
  interface ISafetyService
  {
    [OperationContract]
    [WebInvoke(Method = "POST")]
    IEnumerable<Driver> GetDrivers(string[] ids);
   
    [OperationContract]
    [WebInvoke(Method = "POST")]
    IEnumerable<Driver> GetAllDrivers(bool activeOnly);

    [OperationContract]
    [WebInvoke(Method = "POST")]
    IEnumerable<OptionModel> GetUntrackedDriversOptions();

    [OperationContract]
    [WebInvoke(Method = "POST")]
    void UpdateDrivers(IEnumerable<Driver> drivers);

    [OperationContract]
    [WebInvoke(Method = "POST")]
    void TrackDrivers(string[] ids);

    [OperationContract]
    [WebInvoke(Method = "POST")]
    void UnTrackDrivers(string[] ids);

    [OperationContract]
    [WebInvoke(Method = "POST")]
    IEnumerable<Tractor> GetTractors(bool activeOnly);

    [OperationContract(Name="GetTractorsById")]
    [WebInvoke(Method = "POST")]
    IEnumerable<Tractor> GetTractors(IEnumerable<string> ids);

    [OperationContract]
    [WebInvoke(Method = "POST")]
    void UdpateTractor(Tractor tractor);

    [OperationContract]
    [WebInvoke(Method = "POST")]
    IEnumerable<Trailer> GetTrailers(bool activeOnly);

    [OperationContract(Name = "GetTrailersById")]
    [WebInvoke(Method = "POST")]
    IEnumerable<Trailer> GetTrailers(IEnumerable<string> ids);

    [OperationContract]
    [WebInvoke(Method = "POST")]
    void UdpateTrailer(Trailer trailer);

    [OperationContract]
    [WebInvoke(UriTemplate = "/tractor-maintenance", Method = "POST")]
    void Append(TractorMaintenancePerformed maintenance);

    [OperationContract]
    [WebGet(UriTemplate = "/tractor-maintenance/types")]
    List<SimpleItem> GetMaintenanceTypes();
  }
}

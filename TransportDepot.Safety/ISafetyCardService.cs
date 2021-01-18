using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using TransportDepot.Models.Safety;
using System.Collections.Generic;

namespace TransportDepot.Safety
{
  [ServiceContract(Namespace = "http://transportdepot.net/safety/cards")]
  interface ISafetyCardService
  {
    [OperationContract]
    [WebGet]
    IEnumerable<SafetyCard> GetCards();
  }
}

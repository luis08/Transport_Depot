
using System.ServiceModel;
using System.ServiceModel.Web;
using Transport_Depot_WCF.PaymentPosting;
using System.Collections.Generic;
namespace Transport_Depot_WCF
{
  [ServiceContract]

  interface IPaymentPostingService
  {
    [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
      ResponseFormat = WebMessageFormat.Json,
      RequestFormat = WebMessageFormat.Json)]
    [OperationContract]
    void Post(PaymentRequestModel model);

    [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Bare,
          ResponseFormat = WebMessageFormat.Json,
          RequestFormat = WebMessageFormat.Json)]
    [OperationContract]
    string GetCheckNumber(ACHFileIdentifierModel model);

    [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
          ResponseFormat = WebMessageFormat.Json,
          RequestFormat = WebMessageFormat.Json)]
    [OperationContract]
    string GetExistingCheckNumber(ACHFileIdentifierModel model);

    [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
          ResponseFormat = WebMessageFormat.Json,
          RequestFormat = WebMessageFormat.Json)]
    [OperationContract]
    IEnumerable<CheckExistenceTestModel> GetCheckExistenceModels(ACHFileIdentifierModel[] models);

    [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
          ResponseFormat = WebMessageFormat.Json,
          RequestFormat = WebMessageFormat.Json)]
    [OperationContract]
    void IgnoreCheck(ACHFileIdentifierModel model);

    [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
          ResponseFormat = WebMessageFormat.Json,
          RequestFormat = WebMessageFormat.Json)]
    [OperationContract]
    void DontIgnoreCheck(ACHFileIdentifierModel model);

    [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
      ResponseFormat = WebMessageFormat.Json,
      RequestFormat = WebMessageFormat.Json)]
    [OperationContract]
    PaymentGlModel GetDefaultPaymentAccounts();
  }
}

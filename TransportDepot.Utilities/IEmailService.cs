using System.ServiceModel;
using TransportDepot.Models.Utilities;

///Not finished
namespace TransportDepot.Utilities
{
  [ServiceContract]
  public interface IEmailService
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <returns>int
    /// When Positive: The identifier of the queued email to send.  You can use it to check the queue later on.
    /// When Negative: Error sending email.
    ///   -1 : No recipients found in the 'To' collection.
    /// </returns>
    [OperationContract]
    int Send(EmailModel email);

    [OperationContract]
    int GetQueueId(int emailId);
  }
}

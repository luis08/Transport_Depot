
using System.Web.Management;
namespace Transport_Depot_WCF
{
  public class TransportDepotWcfAuditEvent : WebAuditEvent
  {
    public TransportDepotWcfAuditEvent(string msg, object eventSource, int eventCode)
      : base(msg, eventSource, eventCode) { }
    public TransportDepotWcfAuditEvent(string msg, object eventSource, int eventCode, int eventDetailCode)
      : base(msg, eventSource, eventCode, eventDetailCode) { }
    public override void FormatCustomEventDetails(WebEventFormatter formatter)
    {
      base.FormatCustomEventDetails(formatter);

      // Display some custom event message
      formatter.AppendLine("Transport Depot WCF Event");
    }
  }
}
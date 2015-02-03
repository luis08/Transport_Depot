
namespace TransportDepot.Models.Utilities
{
  public class EmailModel
  {
    public string[] To { get; set; }
    public string[] CC { get; set; }
    public string[] BCC { get; set; }
    public bool IsHtml { get; set; }
    public FileAttachment[] Attachments { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
  }
}

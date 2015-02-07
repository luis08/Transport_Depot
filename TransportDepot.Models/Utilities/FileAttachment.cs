using System.IO;

namespace TransportDepot.Models.Utilities
{

  public class FileAttachment
  {
    public string FileContent { get; set; }
    public MemoryStream Stream { get; set; }
  }
}

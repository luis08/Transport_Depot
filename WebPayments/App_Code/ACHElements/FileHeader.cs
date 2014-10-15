using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;


namespace WebPayments.ACHElements
{
  /// <summary>
  /// Summary description for FileHeader
  /// </summary>
  public class FileHeader
  {
    
    public string RecordTypeCode { get; set; }
    public string PriorityCode { get; set; }
    public string ImmediateDestination { get; set; }
    public string ImmediateOrigin { get; set; }
    public DateTime FileCreationDateTime { get; set; }
    public string FileCreationDate { get; set; }
    public string FileCreationTime { get; set; }
    public string FileIDModifier { get; set; }
    public string RecordSize { get; set; }
    public string BlockingFactor { get; set; }
    public string FormatCode { get; set; }
    public string ImmediateDestinationName { get; set; }
    public string ImmediateOriginName { get; set; }
    public string ReferenceCode { get; set; }

    public override string ToString()
    {
      var sb = new StringBuilder( );
      sb.Append(this.RecordTypeCode)
        .Append(this.PriorityCode)
        .Append(this.ImmediateDestination)
        .Append(this.ImmediateOrigin)
        .Append(this.FileCreationDate)
        .Append(this.FileCreationTime)
        .AppendFormat(this.FileIDModifier.ToUpper())
        .Append( this.RecordSize ) 
        .Append(this.BlockingFactor)
        .Append(this.FormatCode)
        .Append(this.ImmediateDestinationName)
        .Append(this.ImmediateOriginName)
        .Append(this.ReferenceCode);
      return sb.ToString();
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebPayments.ACHElements
{
  public class ACHFile
  {

    public FileHeader FileHeaderRecord { get; set; }
    public IEnumerable<CompanyBatch> CompanyBatchRecords { get; set; }
    public FileControl FileControlRecord { get; set; }
    public string BlockPadding { get; set; }
    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.AppendLine(this.FileHeaderRecord.ToString());

      this.CompanyBatchRecords.ToList()
        .ForEach(c => sb.AppendLine(c.ToString()));

      sb.AppendLine(this.FileControlRecord.ToString());

      if (this.BlockPadding.Length > 5)
      {
        sb.Append(this.BlockPadding);
      }
      
      return sb.ToString();
    }
  }
}
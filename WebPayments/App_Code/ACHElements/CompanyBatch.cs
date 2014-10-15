using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace WebPayments.ACHElements
{

  public class CompanyBatch
  {
    public CompanyBatchHeader HeaderRecord { get; set; }
    public IEnumerable<EntryDetail> EntryDetailRecords { get; set; }
    public CompanyBatchControl ControlRecord { get; set; }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.AppendLine(this.HeaderRecord.ToString());
      this.EntryDetailRecords.ToList()
        .ForEach(d=> sb.AppendLine(d.ToString() ));
      sb.Append(this.ControlRecord.ToString());

      return sb.ToString();
    }
  
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Models.Factoring
{
  public class ScheduleSetViewModel
  {
    public int PageNumber { get; set; }
    public int RowsPerPage { get; set; }
    public int TotalScheduleCount { get; set; }
    public IEnumerable<ScheduleSummaryViewModel> Schedules { get; set; }
  }
}

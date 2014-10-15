using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Models.Factoring
{
  public class ScheduleFilter
  {
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int FromSchedule { get; set; }
    public int ToSchedule { get; set; }

    public int PageNumber { get; set; }

    public int RowsPerPage { get; set; }
  }
}

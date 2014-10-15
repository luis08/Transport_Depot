using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FactoringServiceReference;

namespace WebPayments.Factoring
{
  /// <summary>
  /// Summary description for FactoringServiceDataSource
  /// </summary>
  public class FactoringServiceDataSource
  {
    private ScheduleSetViewModel _model = null;

    public IEnumerable<ScheduleSummaryViewModel> GetSchedules(DateTime fromDate, DateTime toDate, int fromSchedule, int toSchedule, int maxRows, int startRowIndex)
    {
      if (this._model == null)
      {
        this.SetModel(fromDate, toDate, fromSchedule, toSchedule, maxRows, startRowIndex);
      }
      return this._model.Schedules;
    }

    public int GetScheduleCount(DateTime fromDate, DateTime toDate, int fromSchedule, int toSchedule)
    {

      if (this._model == null)
      {
        this.SetModel(fromDate, toDate, fromSchedule, toSchedule, 1, int.MaxValue);
      }
      return this._model.TotalScheduleCount;
    }

    private void SetModel(DateTime fromDate, DateTime toDate, int fromSchedule, int toSchedule, int maxRows, int startRowIndex)
    {
      using (var svc = new FactoringServiceReference.FactoringServiceClient())
      {
        var scheduleFilter = this.GetFilter(fromDate, toDate, fromSchedule, toSchedule, maxRows, startRowIndex);
        this._model = svc.GetSchedules(scheduleFilter);
      }
    }

    private ScheduleFilter GetFilter(DateTime fromDate, DateTime toDate, int fromSchedule, int toSchedule, int maxRows, int startRowIndex)
    {
      int fullPageNumber = Convert.ToInt32( Math.Floor((double)( startRowIndex / maxRows )));
      var currentPageNumber = fullPageNumber;
      if( ( startRowIndex + 1 ) % maxRows != 0 )
      {
        currentPageNumber++;
      }
      var scheduleFilter = new ScheduleFilter
      {
        FromDate = this.CleanDate(fromDate),
        ToDate = this.CleanDate(toDate),
        FromSchedule = fromSchedule,
        ToSchedule = toSchedule,
        PageNumber = currentPageNumber,
        RowsPerPage = maxRows
      };
      return scheduleFilter;
    }
    private DateTime CleanDate(DateTime input)
    {
      if (input.Equals(DateTime.MinValue))
      {
        input = new DateTime(1753, 1, 1);
      }
      return input;
    }
  }
}
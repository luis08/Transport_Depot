using System.Collections.Generic;
using TransportDepot.Data.Factoring;
using TransportDepot.Models.Factoring;
using System.Linq;
using System;


namespace TransportDepot.Factoring
{
  class FactoringService : IFactoringService, IAjaxFactoringService
  {
    private FactoringDataSource _dataSource = new FactoringDataSource();

    public ScheduleSetViewModel GetSchedules(ScheduleFilter filter)
    {
      var normalizer = new FilterNormalizer
      {
        FromDate = filter.FromDate,
        ToDate = filter.ToDate,
        PageNumber = filter.PageNumber,
        RowsPerPage = filter.RowsPerPage
      };
      normalizer.Normalize();
      
      filter.RowsPerPage = normalizer.RowsPerPage;
      filter.PageNumber = normalizer.PageNumber;
      filter.FromDate = normalizer.FromDate;
      filter.ToDate = normalizer.ToDate;

      filter = this.NormalizeScheduleFilter(filter);
      
      var models  = this._dataSource.GetSchedules(filter);
      var count = models.Count();

      if (count.Equals(0))
      {
        return new ScheduleSetViewModel
        {
          PageNumber = 0,
          RowsPerPage = filter.RowsPerPage,
          TotalScheduleCount = 0,
          Schedules = new List<ScheduleSummaryViewModel>(),
          
        };
      }

      var lastFullPageNumber = Convert.ToInt32(count / filter.RowsPerPage);
      var lastPageNumber = (lastFullPageNumber * filter.RowsPerPage) < count ? lastFullPageNumber + 1 : lastFullPageNumber;
      if (lastPageNumber < filter.PageNumber)
      {
        filter.PageNumber = lastPageNumber;
      }
      var schedules = models.Skip((filter.PageNumber - 1) * filter.RowsPerPage).Take(filter.RowsPerPage)
        .ToList();

      var model = new ScheduleSetViewModel
      {
        PageNumber = filter.PageNumber,
        RowsPerPage = filter.RowsPerPage,
        TotalScheduleCount = count,
        Schedules = schedules
      };
      return model;
    }

    public Models.Factoring.ScheduleViewModel GetSchedule(int id)
    {
      ScheduleViewModel model = this._dataSource.GetSchedule(id);
      return model;
    }

    public void SaveSchedule(Models.Factoring.ScheduleViewModel schedule)
    {
      this._dataSource.Save(schedule);
    }

    public void DeleteSchedule(int id)
    {
      this._dataSource.DeleteSchedule(id);
    }

    public int CreateSchedule(NewScheduleViewModel schedule)
    {
      var newScheduleId = this._dataSource.Create(schedule);
      return newScheduleId;
    }

    public InvoiceSet GetInvoices(InvoiceFilter filter)
    {
      if (filter == null)
      {
        throw new ArgumentException("filter is null");
      }
      else if( filter.FromDate == null )
      {
        throw new ArgumentException("filter.FromDate is null");
      }
      else if (filter.ToDate == null)
      {
        throw new ArgumentException("filter.ToDate is null");
      }
      else if (filter.FromInvoiceNumber == null)
      {
        throw new ArgumentException("filter.FromInvoiceNumber is null");
      }
      else if (filter.ToInvoiceNumber == null)
      {
        throw new ArgumentException("filter.ToInvoiceNumber is null");
      }
      
      return this.GetUnpostedInvoices(filter);
    }

    public InvoiceSet GetUnpostedInvoices(InvoiceFilter filter)
    {
      var normalizer = new FilterNormalizer
      {
        FromDate = filter.FromDate,
        ToDate = filter.ToDate,
        PageNumber = filter.PageNumber,
        RowsPerPage = filter.RowsPerPage
      };
      normalizer.Normalize();
      filter.RowsPerPage = normalizer.RowsPerPage;
      filter.PageNumber = normalizer.PageNumber;
      filter.FromDate = normalizer.FromDate;
      filter.ToDate = normalizer.ToDate;
      filter = this.NormalizeInvoiceFilter(filter);
      var invoices = this._dataSource.GetUnPostedInvoices(filter);
      var count = invoices.Count();
      if (count.Equals(0))
      {
        return new InvoiceSet
        {
          Invoices = new List<InvoiceViewModel>(),
          PageNumber = 0,
          TotalPages = 0,
          RowsPerPage = filter.RowsPerPage
        };
      }
      var lastFullPageNumber = Convert.ToInt32(count / filter.RowsPerPage);
      var lastPageNumber = (lastFullPageNumber * filter.RowsPerPage) < count ? lastFullPageNumber + 1 : lastFullPageNumber;
      if (lastPageNumber < filter.PageNumber)
      {
        filter.PageNumber = lastPageNumber;
      }

      var page = invoices.Skip((filter.PageNumber - 1) * filter.RowsPerPage).Take(filter.RowsPerPage)
        .ToList();

      var invoiceSet = new InvoiceSet
      {
        Invoices = page,
        PageNumber = filter.PageNumber,
        RowsPerPage = filter.RowsPerPage,
        TotalPages = lastPageNumber
      };
      return invoiceSet;
    }
 

    private ScheduleFilter NormalizeScheduleFilter(ScheduleFilter filter)
    {

      if (filter.FromSchedule.Equals(0) && filter.ToSchedule.Equals(0))
      {
        filter.ToSchedule = int.MaxValue;
      }
      if (filter.FromSchedule > filter.ToSchedule)
      {
        var fromSchedule = filter.FromSchedule;
        filter.FromSchedule = filter.ToSchedule;
        filter.ToSchedule = fromSchedule;
      }
      return filter;
    }

    private InvoiceFilter NormalizeInvoiceFilter(InvoiceFilter filter)
    {
      
      filter.FromInvoiceNumber = this.PadInvoiceNumber( this.Cleanup(filter.FromInvoiceNumber, string.Empty) );
      filter.ToInvoiceNumber = this.PadInvoiceNumber( this.Cleanup(filter.ToInvoiceNumber, new string('Z', 18)));
      if (string.Compare(filter.FromInvoiceNumber, filter.ToInvoiceNumber) > 0)
      {
        var fromInvoice = filter.FromInvoiceNumber;
        filter.FromInvoiceNumber = filter.ToInvoiceNumber;
        filter.ToInvoiceNumber = fromInvoice;
      }
      return filter;
    }

    private string PadInvoiceNumber(string invoiceNumber)
    {
      var padding = System.Configuration.ConfigurationManager.AppSettings["Factoring.InvoiceLeftFill"] ?? string.Empty;
      var lengthDifference = padding.Length - invoiceNumber.Length;
      if (padding.Length == 0)
      { return invoiceNumber; }
      else if ( lengthDifference < 1 )
      { return invoiceNumber; }
      
      var newInvoiceNumber = string.Format("{0}{1}",
        padding.Substring(0, lengthDifference),
        invoiceNumber);
      return newInvoiceNumber;
    }
    private string Cleanup(string p, string nullValue)
    {
      p = (p ?? string.Empty).Trim();
      if (p.Equals(string.Empty))
      {
        return nullValue;
      }
      return p;
    }
    
    private const int DefaultRowsPerPage = 10;
  }
}

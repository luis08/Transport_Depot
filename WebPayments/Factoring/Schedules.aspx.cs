using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FactoringServiceReference;

public partial class Factoring_Schedules : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    
    if (this.IsPostBack)
    {
      this.DataBind();
    }
  }



  private ScheduleSetViewModel GetModel()
  {
    ScheduleSetViewModel model = null;

    using (var svc = new FactoringServiceReference.FactoringServiceClient())
    {
      model = svc.GetSchedules(this.ScheduleFilter);
    }
    return model;
  }

  protected void PopulateSchedules(object sender, EventArgs e)
  {

  }

  private const string FilterKey = "ScheduleFilterViewStateKey";
  private ScheduleFilter ScheduleFilter
  {
    get
    {
      var viewStateFilter = this.ViewState[FilterKey];
      if (viewStateFilter == null)
      {
        throw new InvalidOperationException("Filter is not initialized");
      }
      return viewStateFilter as ScheduleFilter;
    }
    set
    {
      if (value == null)
      {
        throw new InvalidOperationException("ScheduleFilter cannot be null");
      }
      this.ViewState[FilterKey] = value;
    }
  }


  protected void ApplyFiltersButton_Click(object sender, EventArgs e)
  {
    var pager = this.SchedulesListView.FindControl("SchedulesDataPager") as DataPager;
    this.SchedulesListView.DataBind();
  }

  protected void Page_LoadComplete(object sender, EventArgs e)
  {

    
  }
  protected void ItemsPerPageDropDown_SelectedIndexChanged(object sender, EventArgs e)
  {
    //var filter = this.GetScheduleFilter();
    //this.ScheduleFilter = filter;
    var pager = this.SchedulesListView.FindControl("SchedulesDataPager") as DataPager;
    var rowsPerpage = int.Parse((sender as DropDownList).SelectedValue);
    pager.SetPageProperties(pager.StartRowIndex, rowsPerpage, true);
  }

  private ScheduleFilter GetScheduleFilter()
  {
    var filter = new ScheduleFilter
    {
      FromDate = this.FromDate,
      ToDate = this.ToDate,
      FromSchedule = this.FromSchedule,
      ToSchedule = this.ToSchedule,
      PageNumber = 1,
      RowsPerPage = this.RowsPerPage
    };
    return filter;
  }

  private const int DefaultInvoicesPerPage = 15;
  private int RowsPerPage
  {
    get
    {
      
      var parsedRowsPerPage = DefaultInvoicesPerPage;

      if (!int.TryParse(this.ItemsPerPageDropDown.SelectedValue, out parsedRowsPerPage))
      {
        return DefaultInvoicesPerPage;
      }
      return parsedRowsPerPage;
    }
  }

  private DateTime FromDate
  {
    get
    {
      var defaultFromDate = DateTime.MinValue;
      if (this.UseDatesCheckbox.Checked)
      {
        return this.ParseDate(this.FromDateInput.Value, defaultFromDate);
      }
      return defaultFromDate;
    }
  }



  private DateTime ToDate
  {
    get
    {
      var defaultToDate = DateTime.MaxValue;
      if (this.UseDatesCheckbox.Checked)
      {
        return this.ParseDate(this.ToDateInput.Value, defaultToDate);
      }
      else
      {
        return defaultToDate;
      }
    }
  }

  private DateTime ParseDate(string date, DateTime defaultValue)
  {
    var parsedDate = DateTime.Today;
    if (!DateTime.TryParse(date, out parsedDate))
    {
      return defaultValue;
    }
    return parsedDate;
  }

  private int FromSchedule
  {
    get
    {
      int defaultFromSchedule = 1;
      if (this.UseScheduleNumberCheckbox.Checked)
      {
        return this.ParseInt(this.FromScheduleInput.Value, defaultFromSchedule);
      }
      return defaultFromSchedule;
    }
  }

  private int ToSchedule
  {
    get
    {
      int defaultToSchedule = int.MaxValue;
      if (this.UseScheduleNumberCheckbox.Checked)
      {
        return this.ParseInt(this.ToScheduleInput.Value, defaultToSchedule);
      }
      return defaultToSchedule;
    }
  }

  private int ParseInt(string intValue, int defaultValue)
  {
    var parsedInt = 0;
    if (!int.TryParse(intValue, out parsedInt))
    {
      return defaultValue;
    }
    return parsedInt;
  }


  protected void SchedulesObjectDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
  {
    e.InputParameters["fromDate"] = this.FromDate;
    e.InputParameters["toDate"] = this.ToDate;
    e.InputParameters["fromSchedule"] = this.FromSchedule;
    e.InputParameters["toSchedule"] = this.ToSchedule;
  }
  protected void SchedulesListView_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
  {
    var pager = this.SchedulesListView.FindControl("SchedulesDataPager") as DataPager;
    pager.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
  }
}
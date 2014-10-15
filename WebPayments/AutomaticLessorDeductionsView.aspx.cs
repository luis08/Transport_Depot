using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutomaticDeductionsService;
using System.Drawing;

public partial class AutomaticLessorDeductionsView : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!IsPostBack)
    {
      this.GetCommissionsToChange();
    }
    else
    {
      this.SaveCommissionsToChange();
    }
  }

  protected void Page_PreRender(object sender, EventArgs e)
  {
    // Save PageArrayList before the page is rendered.
    ViewState.Add("viewList", LessorsGridView.DataSource);
  }

  protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
  {
    //Checking whether the Row is Data Row
    if (e.Row.RowType == DataControlRowType.DataRow)
    {
      //Finding the Dropdown control.
      Control rateControl = e.Row.FindControl("rateSelector");
      if (rateControl != null)
      {
        DropDownList dd = rateControl as DropDownList;
        var dataSource = e.Row.DataItem as ChangeCommissionViewModel;
        if (dd != null && dataSource != null)
        {
          dd.DataTextField = "RateText";
          dd.DataValueField = "RateId";
          dd.DataSource = dataSource.CommissionRates;
          dd.SelectedValue = dataSource.Commission.ToString();
          dd.DataBind();
        }
      }
    }
  }

  private void GetCommissionsToChange()
  {
    List<ChangeCommissionViewModel> viewList = new List<ChangeCommissionViewModel>();
    label_message.Text = "";
    try
    {
      AutomaticDeductionsClient context = new AutomaticDeductionsClient();
      var deductions = context.GetCommissionsToChange();
      List<CommissionRate> rates = new List<CommissionRate>();
      foreach (decimal r in deductions.CommissionRates)
      {
        var rate = new CommissionRate(r);
        rates.Add(rate);
      }
      foreach (ChangeCommission d in deductions.Deductions)
      {
        var m = new ChangeCommissionViewModel(d, rates);
        viewList.Add(m);
      }
    }
    catch (Exception e)
    {
      label_message.Text = "There was an error accessing your data. Please try again!";
      label_message.ForeColor = Color.FromName("#FF0606");
    }
    LessorsGridView.DataSource = viewList;
    LessorsGridView.BorderWidth = 0;
    LessorsGridView.DataBind();
  }

  private void SaveCommissionsToChange()
  {
    List<ChangeCommissionViewModel> list = ViewState["viewList"] as List<ChangeCommissionViewModel>;
    label_message.Text = "";
    for (int i = 0; i < LessorsGridView.Rows.Count; i++)
    {
      GridViewRow row = LessorsGridView.Rows[i];
      var selector = row.FindControl("updateSelector") as CheckBox;
      var changeCommision = row.FindControl("changeCommission") as CheckBox;
      var rateSelector = row.FindControl("rateSelector") as DropDownList;
      var lessorId = row.Cells[1].Text;
      var tripNumber = row.Cells[2].Text;
      if (list != null)
      {
        // The elements of LessorsGridView[i] should be equal to LessorsGridView[i]
        ChangeCommissionViewModel model = i < list.Count ? model = list[i] : null;
        if (model != null && !(model.LessorId.Equals(lessorId, StringComparison.CurrentCultureIgnoreCase) &&
                model.TripNumber.Equals(tripNumber, StringComparison.CurrentCultureIgnoreCase)))
        {
          model = null;
          //i.e. list[i] != LessorsGridView[i];
        }
        if (model == null) // if the elements of LessorsGridView[i] are not equal to LessorsGridView[i], find the right one
        {
          foreach (ChangeCommissionViewModel m in list)
          {
            if (m.LessorId.Equals(lessorId, StringComparison.CurrentCultureIgnoreCase) &&
                    m.TripNumber.Equals(tripNumber, StringComparison.CurrentCultureIgnoreCase))
            {
              model = m;
              break;
            }
          }
        }
        if (model != null) // just in case
        {
          model.Update = selector.Checked;
          model.Commission = Convert.ToDecimal(rateSelector.SelectedValue);
          model.ChangeCommissionDefault = changeCommision.Checked;
        }
        model = null;
      }
    }
    //change to catch exception if an error happened
    bool Error = false;
    try
    {
      AutomaticDeductionsClient context = new AutomaticDeductionsClient();
      ChangeCommissionLogicModel ad = new ChangeCommissionLogicModel();
      var toSubmit = new List<ChangeCommission>();
      foreach (ChangeCommissionViewModel vm in list.Where(u => u.Update))
      {
        ChangeCommission c = new ChangeCommission();
        c.CargoInsurance = vm.CargoInsurance;
        c.ChangeCommissionDefault = vm.ChangeCommissionDefault;
        c.Commission = vm.Commission;
        c.LessorId = vm.LessorId;
        c.LiabilityInsurance = vm.LiabilityInsurance;
        c.TripNumber = vm.TripNumber;
        toSubmit.Add(c);
      }
      context.SaveCommissionsToChange(toSubmit.ToArray());
    }
    catch (Exception e)
    {
      label_message.Text = "There was a problem making changes. Please try again!";
      label_message.ForeColor = Color.FromName("#FF0606");
      LessorsGridView.DataSource = list;
      LessorsGridView.DataBind();
      Error = true;
    }
    if (!Error)
    {
      label_message.Text = "Changes saved successfully!";
      this.GetCommissionsToChange();
    }
  }

  private List<ChangeCommissionViewModel> GetTemp()
  {
    List<ChangeCommissionViewModel> list = new List<ChangeCommissionViewModel>();
    List<CommissionRate> rates = new List<CommissionRate>();
    for (int i = 1; i <= 3; i++)
    {
      CommissionRate r = new CommissionRate(Convert.ToDecimal("" + i / 10.0));
      rates.Add(r);
    }
    for (int i = 0; i < 10; i++)
    {
      ChangeCommission cc = new ChangeCommission();
      cc.LessorId = "LessorID_" + i;
      cc.TripNumber = "Trip_" + i;
      cc.LiabilityInsurance = Convert.ToDecimal("" + 0.2);
      cc.CargoInsurance = Convert.ToDecimal("" + 0.4);
      cc.ChangeCommissionDefault = false;
      cc.Commission = Convert.ToDecimal("" + 0.3);
      ChangeCommissionViewModel m = new ChangeCommissionViewModel(cc, rates);
      list.Add(m);
    }

    return list;
  }
}
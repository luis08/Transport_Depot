using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AutomaticDeductionsService;
using SettlementsService;
using System.Drawing;
using TransportDepot.AutomaticLessorDeductions;

public partial class ChangeDefaults : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    Page.EnableViewState = true;
    if (!Page.IsPostBack)
    {
      opacity_div.Visible = add_default.Visible = false;
      this.GetLessors();
    }
  }

  protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
  {
    //Checking whether the Row is Data Row
    if (e.Row.RowType == DataControlRowType.DataRow)
    {
      var dataSource = e.Row.DataItem as LessorDefaults;

      //Finding the Commission Dropdown control. Bind it to a list of dilplay rates
      Control commissionControl = e.Row.FindControl("commissionSelector");
      if (commissionControl != null)
      {
        DropDownList dd = commissionControl as DropDownList;
        if (dd != null && dataSource != null)
        {
          dd.DataTextField = "TextField";
          dd.DataValueField = "ValueField";
          List<DisplayRate> rates = new List<DisplayRate>();
          foreach (decimal val in dataSource.CommissionRates) // change to decimal
          {
            DisplayRate rate = new DisplayRate(val); // change to just val
            rates.Add(rate);
          }
          dd.DataSource = rates;
          dd.SelectedValue = dataSource.Commission.ToString("#0.00");
          dd.DataBind();
        }
      }

      //Finding the Liability Dropdown control.
      Control liabilityControl = e.Row.FindControl("liabilitySelector");
      if (liabilityControl != null)
      {
        DropDownList dd = liabilityControl as DropDownList;
        if (dd != null && dataSource != null)
        {
          dd.DataTextField = "TextField";
          dd.DataValueField = "ValueField";
          List<DisplayRate> rates = new List<DisplayRate>();
          foreach (decimal val in dataSource.LiabilityRates) // change to decimal
          {
            DisplayRate rate = new DisplayRate(val); // change to just val
            rates.Add(rate);
          }
          dd.DataSource = rates;
          dd.SelectedValue = dataSource.Liability.ToString("#0.00");
          dd.DataBind();
        }
      }

      //Finding the Cargo Dropdown control.
      Control cargoControl = e.Row.FindControl("cargoSelector");
      if (cargoControl != null)
      {
        DropDownList dd = cargoControl as DropDownList;
        if (dd != null && dataSource != null)
        {
          dd.DataTextField = "TextField";
          dd.DataValueField = "ValueField";
          List<DisplayRate> rates = new List<DisplayRate>();
          foreach (decimal val in dataSource.CargoRates) // change to decimal
          {
            DisplayRate rate = new DisplayRate(val); // change to just val
            rates.Add(rate);
          }
          dd.DataSource = rates;
          dd.SelectedValue = dataSource.Cargo.ToString("#0.00");
          dd.DataBind();
        }
      }
    }
  }

  internal void Change_Lessor(object sender, EventArgs e)
  {
    this.GetLessorDefaults();
  }

  protected void Save_Defaults(object sender, EventArgs e)
  {
    var ad = new AutomaticDeductionsClient();
    foreach (GridViewRow row in LessorsDefaultGridView.Rows)
    {
      try
      {
        var lessor_id = row.Cells[0].Text;
        var commissionList = row.FindControl("commissionSelector") as DropDownList;
        var liabilityList = row.FindControl("liabilitySelector") as DropDownList;
        var cargoList = row.FindControl("cargoSelector") as DropDownList;
        decimal commissionToChange = Convert.ToDecimal(commissionList.SelectedValue);
        decimal liabilityToChange = Convert.ToDecimal(liabilityList.SelectedValue);
        decimal cargoToChange = Convert.ToDecimal(cargoList.SelectedValue);
        LessorDefaults ld = new LessorDefaults();
        ld.LessorId = lessor_id;
        ld.Commission = commissionToChange;
        ld.Liability = liabilityToChange;
        ld.Cargo = cargoToChange;
        ad.SaveLessorsDefaults(ld);
        lbMessage.ForeColor = Color.FromName("#3D7DDE");
        lbMessage.Text = "Defaults were saved successfully!";
      }
      catch (Exception ex)
      {
        lbMessage.Text = "There was an error while saving lessor. Please try again.";
      }
    }
  }

  protected void Add_Defaults_Open(object sender, EventArgs e)
  {
    add_default_message.Text = "";
    opacity_div.Visible = add_default.Visible = true;
    var ad = new AutomaticDeductionsClient();
    DropDownListAddDefault.DataTextField = "DefaultType";
    DropDownListAddDefault.DataValueField = "DefaultType";
    try
    {
      DropDownListAddDefault.DataSource = ad.GetDefaultTypes();
      DropDownListAddDefault.DataBind();
    }
    catch (Exception ex)
    {
      DropDownListAddDefault.DataSource = new List<DefaultTypeAndValue>();
      DropDownListAddDefault.DataBind();
      add_default_message.Text = "There was an error while accessing your defaults. Please try again!";
      return;
    }
  }

  protected void Add_Default_Dialog_Clicked(object sender, EventArgs e)
  {
    add_default_message.Text = "";
    var ad = new AutomaticDeductionsClient();
    try
    {
      decimal val = Convert.ToDecimal(Default_Val_TextBox.Text) / Convert.ToDecimal("100.00");
      DefaultTypeAndValue type = new DefaultTypeAndValue();
      type.DefaultType = DropDownListAddDefault.SelectedValue;
      type.DefaultValue = val;
      ad.AddDefaultValue(type);
    }
    catch (FormatException ex)
    {
      add_default_message.Text = "Incorrect format for the value provided!";
      return;
    }
    catch (OverflowException ex)
    {
      add_default_message.Text = "The value provided is too large!";
      return;
    }
    catch (Exception ex)
    {
      add_default_message.Text = "There was a problem while adding the default type!";
      return;
    }
    this.GetLessorDefaults();
    opacity_div.Visible = add_default.Visible = false;
  }

  protected void Close_Add_Default_Dialog(object sender, EventArgs e)
  {
    add_default_message.Text = "";
    opacity_div.Visible = add_default.Visible = false;
  }

  private void GetLessors()
  {
    var ad = new AutomaticDeductionsClient();
    var settlementClient = new SettlementServiceClient();
    var lessors =  new Lessor[0];
    lbMessage.Text = "";
    try
    {
      lessors = settlementClient.GetAllLessors();
    }
    catch (Exception ex)
    {
      lbMessage.Text = "There was an error accessing lessors. Please try again.";
      work_area.Visible = false;
      return;
    }
    LessorIDs.DataTextField = "LessorId";
    LessorIDs.DataValueField = "LessorId";
    LessorIDs.DataSource = lessors;
    LessorIDs.DataBind();
    this.GetLessorDefaults();
  }

  private void GetLessorDefaults()
  {
    List<LessorDefaults> defaults = new List<LessorDefaults>();
    lbMessage.Text = "";
    try
    {
      var ad = new AutomaticDeductionsClient();
      if (LessorIDs != null && !string.IsNullOrWhiteSpace(LessorIDs.SelectedValue))
      {
        var lessor_id = LessorIDs.SelectedValue;
        LessorDefaults lDefaults = ad.GetLessorDefaults(lessor_id);
        defaults.Add(lDefaults);
        LessorsDefaultGridView.DataSource = defaults;
        LessorsDefaultGridView.DataBind();
      }
      else throw new Exception();
    }
    catch (Exception ex)
    {
      save_defaults.Enabled = false;
      lbMessage.Text = "There was a problem accessing " + LessorIDs.SelectedValue + "'s defaults. Please refress the page.";
      inner_work_area.Visible = false;
      return;
    }
    LessorsDefaultGridView.DataSource = defaults;
    LessorsDefaultGridView.DataBind();
  }
}

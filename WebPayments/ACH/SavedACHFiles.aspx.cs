using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ACH_SavedACHFiles : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!IsPostBack)
    {
      this.PopulatePayments();
    }
    this.AJAXCallDomainInput.Value = Request.Url.Host;
  }


  private void PopulatePayments()
  {
    var controller = new ACHController();
    this.ACHPaymentsListView.DataSource = controller.GetPaidACHFiles().ToList();
    this.ACHPaymentsListView.DataBind();
  }

  protected void ACHPaymentsListView_OnItemCommand(object sender, ListViewCommandEventArgs e)
  {
    var dataItem = (ListViewDataItem)e.Item;
    var keys = ACHPaymentsListView.DataKeys[dataItem.DisplayIndex].Values;
    Context.Items["CreationDate"] = keys["CreationDate"].ToString();
    Context.Items["FileIDModifier"] = keys["FileIDModifier"].ToString();
    if (string.Equals(e.CommandName, "OpenSettlements"))
    {
      Server.Transfer("PaidSettlements.aspx");
    }
    else if (string.Equals(e.CommandName, "OpenPayrollItems"))
    {
      Server.Transfer("PaidPayrollBatches.aspx");
    }
  }
  protected void ACHPaymentsListView_DataBound(object sender, EventArgs e)
  {
    ListViewUtilities.HidePagerOnPage1("SaveACHFilesPagerTop", this.ACHPaymentsListView);
    ListViewUtilities.HidePagerOnPage1("SavedACHFilesPagerBottom", this.ACHPaymentsListView);
  }
}
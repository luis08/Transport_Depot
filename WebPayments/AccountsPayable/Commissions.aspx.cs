using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AccountsPayable_Commissions : System.Web.UI.Page
{
  private IEnumerable<string> _initials;
  private IEnumerable<string> _vendorIds;

  protected void Page_Load(object sender, EventArgs e)
  {
    this.Populate();
  }

  private void Populate()
  {
    var dal = new DAL.WebPaymentsDAL();
    var model = dal.GetDispatcherPayableModel();

    this._vendorIds = model.Vendors.Union(new string[]{""}).OrderBy(v => v); ;
    this._initials = model.Initials.Union(new string[] { "" }).OrderBy(i => i);
    this.DispatcherListView.DataSource = model.Dispatchers.OrderBy(d=>d.Initials);
    this.DispatcherListView.DataBind();
  }
  protected void DispatcherListView_ItemDataBound(object sender, ListViewItemEventArgs e)
  {
    if (e.Item.ItemType != ListViewItemType.DataItem)
    {
      return;
    }

    var rowView = e.Item.DataItem as DispatcherPayable;
    string initials = rowView.Initials; 
    string vendorId = rowView.VendorId;

    var vendorIdDropDown = e.Item.FindControl("VendorIdDropDown") as DropDownList;
    vendorIdDropDown.DataSource = this._vendorIds;
    vendorIdDropDown.DataBind();
    vendorIdDropDown.SelectedValue = vendorId;
  }
}
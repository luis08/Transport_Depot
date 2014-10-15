using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ACH_PaidPayrollBatches : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!IsPostBack)
    {
      this.ReadSessionVariables();
      this.PopulatePayrollItems();
      this.PopulateHeader();
    }
  }

  private void PopulateHeader()
  {
    this.FileCreationDateLabel.Text = this._fileCreationDate.ToShortDateString();
    this.FileIdModifierLabel.Text = this._fileIdModifier.ToString();
  }

  private void PopulatePayrollItems()
  {
    var controller = new ACHController();
    IEnumerable<PaidPayrollItem> paidPayrollItems =
      controller.GetPaidPayrollItems(this._fileCreationDate, this._fileIdModifier)
      .ToList();
    this.PaidPayrollItemsListView.DataSource = paidPayrollItems;
    this.PaidPayrollItemsListView.DataBind();
  }

  private void ReadSessionVariables()
  {
    var fileCreationDateObject = Context.Items["CreationDate"];
    var fileIdModifierObject = Context.Items["FileIDModifier"];
    try
    {
      this._fileCreationDate = Convert.ToDateTime(fileCreationDateObject);
      this._fileIdModifier = Convert.ToChar(fileIdModifierObject);
    }
    catch (Exception)
    {
      Server.Transfer("SavedACHFiles.aspx");
    }
  }

  private DateTime _fileCreationDate;
  private char _fileIdModifier;
  protected void PaidPayrollItemsListView_DataBound(object sender, EventArgs e)
  {
    ListViewUtilities.HidePagerOnPage1("PaidPayrollItemsListViewTop", this.PaidPayrollItemsListView);
    ListViewUtilities.HidePagerOnPage1("PaidPayrollItemsListViewBottom", this.PaidPayrollItemsListView);
  }
}
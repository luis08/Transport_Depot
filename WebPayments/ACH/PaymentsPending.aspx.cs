using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using DAL;

public partial class ACH_PaymentsPending : System.Web.UI.Page
{
  private HashSet<int> settlementPaymentIds;
  private HashSet<int> payrolBatches;
  private TruckWinPayees _truckwinPayees;
  private HashSet<string> _lessorsWithPayee;
  private HashSet<string> _employeesWithPayee;

  protected void Page_Load(object sender, EventArgs e)
  {
    WebFormFileAttacher.AddCss("/WebPayments/Styles/PaymentsPending.aspx.css", this.Page);
    WebFormFileAttacher.AddJavaScript("/WebPayments/Scripts/PaymentsPending.aspx.js", this.Page);
    this.payrolBatches = new HashSet<int>();
    this.settlementPaymentIds = new HashSet<int>();
    if (!this.IsPostBack)
    {
      this.PopulatePaymentsDue();
    }
    var fileDate = ACHController.GetFileDateTime();
    this.ACHFileDateCalendar.SelectedDate = new DateTime(fileDate.Year, fileDate.Month, fileDate.Day);
    this.ACHFileDateCalendar.VisibleDate = this.ACHFileDateCalendar.SelectedDate;
  }

  private void PopulatePaymentsDue()
  {
    this.LoadValidationStructures();
    var controller = this.GetController();
    var payments_due = controller.GetWebPaymentsDue();
    this.SettlementPaymentsPending.DataSource = payments_due.Settlements.ToList();
    var showNewFileOptions = ((payments_due.Settlements.Count() != 0) || (payments_due.Payroll.Count() != 0));

    this.ACHFileDateCalendar.Visible = showNewFileOptions;
    if (!showNewFileOptions)
    {
      this.GoToACHFilesButton.Text = "View Older Files";
    }
    this.SettlementPaymentsPending.DataBind();
  }

  private void LoadValidationStructures()
  {
    var payeeDAL = new WebPaymentsDAL();

    this._truckwinPayees = payeeDAL.GetTruckWinPayees();
    this._employeesWithPayee = new HashSet<string>(payeeDAL.GetEmployeePayees(false).Select(e => e.TruckwinId));
    this._lessorsWithPayee = new HashSet<string>(payeeDAL.GetLessorPayees(false).Select(l => l.TruckwinId));
  }

  private ACHController GetController()
  {
    return new ACHController();
  }

  protected void GoToACHFilesButton_Click(object sender, EventArgs e)
  {
    try
    {
      this.ReadCheckedSettlements();
      this.SaveNewACHFile();
      HttpContext.Current.Items["ScheduledSettlementPayments"] = this.settlementPaymentIds.ToArray();
      HttpContext.Current.Items["ScheduledSalaryBatchIds"] = this.payrolBatches.ToArray();

      Server.Transfer("SavedACHFiles.aspx");
    }
    catch (Exception ex)
    {
      this.ErrorLabel.Text =
        string.Format("There was an error saving the file: '{0}'", ex.Message + Environment.NewLine + ex.StackTrace);
    }
  }

  private void SaveNewACHFile()
  {
    var controller = new ACHController();
    WebPaymentsDebugger.DebugWrite("PaymentsPending.aspx - PayrollBatches " + payrolBatches.Count.ToString());
    WebPaymentsDebugger.DebugWrite("PaymentsPending.aspx - settlementPaymentIds " + settlementPaymentIds.Count.ToString());
    controller.SaveACHFile(this.payrolBatches.ToArray(), this.settlementPaymentIds.ToArray(), this.ACHFileDateCalendar.SelectedDate);
  }

  private void ReadCheckedSettlements()
  {
    this.settlementPaymentIds = new HashSet<int>();
    foreach (ListViewItem itemRow in this.SettlementPaymentsPending.Items)
    {
      var thisCheckBox = (CheckBox)itemRow.FindControl("SelectPaymentCheckBox");
      var paymentIdLabel = (Label)itemRow.FindControl("PaymentIdLabel");
      var paymentId = int.Parse(paymentIdLabel.Text);

      if (thisCheckBox.Checked)
      {
        this.settlementPaymentIds.Add(paymentId);
      }
      else
      {
        this.settlementPaymentIds.Remove(paymentId);
      }
    }
  }
  protected void ACHFileDateCalendar_DayRender(object sender, DayRenderEventArgs e)
  {
    /*
    e.Day.IsSelectable = ! (e.Day.IsWeekend );
    if (e.Day.Date.ToShortDateString() == this.ACHFileDateCalendar.SelectedDate.ToShortDateString())
    { this.ACHFileDateCalendar.VisibleDate = e.Day.Date; }
     * */
  }
  protected void SettlementPaymentsPending_ItemDataBound(object sender, ListViewItemEventArgs e)
  {
    var lessorIdLabel = e.Item.FindControl("LessorIdLabel") as Label;
    var selectPaymentCheckBox = e.Item.FindControl("SelectPaymentCheckBox") as CheckBox;
    if (lessorIdLabel == null)
    { return; }

    var lessorId = lessorIdLabel.Text;

    if (!this._truckwinPayees.Lessors.Contains(lessorId))
    {
      lessorIdLabel.CssClass = "wrong-data";
      selectPaymentCheckBox.Enabled = false;
    }
    else if (!this._lessorsWithPayee.Contains(lessorId))
    {
      lessorIdLabel.CssClass = "missing-data";
      selectPaymentCheckBox.Enabled = false;
    }
  }
  protected void SettlementPaymentsPending_DataBound(object sender, EventArgs e)
  {
    ListViewUtilities.HidePagerOnPage1("PaymentsPendingPagerTop", this.SettlementPaymentsPending);
    ListViewUtilities.HidePagerOnPage1("PaymentsPendingPagerBottom", this.SettlementPaymentsPending);
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAL;
using System.Web.UI.HtmlControls;

public partial class ACH_Payees : System.Web.UI.Page
{
  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);

    this.AddStyleSheet();
    this.AddJavaScript();
  }
  protected void Page_Load(object sender, EventArgs e)
  {

    if (!this.IsPostBack)
    {
      this.BindPayees();
    }
    

    this.MessageLabel.Text = string.Empty;

    this.SetupPayeeType();
  }

  private void SetupPayeeType()
  {
    if (this.LessorsRadioButton.Checked)
    {
      this.PayeeEditContainer.InnerText = "New Lessor";
      this.PayeeTypeKeyHiddenInput.Value = ClientValidatorKeys.PayeeTypeValue.LessorId;
    }
    else if (this.EmployeesRadioButton.Checked)
    {
      this.PayeeEditContainer.InnerText = "New Employee";
      this.PayeeTypeKeyHiddenInput.Value = ClientValidatorKeys.PayeeTypeValue.EmployeeId;
    }
    this.IdKeyHiddenInput.Value = ClientValidatorKeys.Id;
  }

  private void AddJavaScript()
  {
    WebFormFileAttacher.AddJavaScript("/WebPayments/Scripts/Payees.aspx.js", this.Page);
  }

  private void AddStyleSheet()
  {
    WebFormFileAttacher.AddCss("/WebPayments/Styles/Payees.aspx.css", this.Page);
  }

  private void BindPayees()
  {
    var newPayees = new List<string>();
    IEnumerable<PayeeDTO> payees;
    var dal = new WebPaymentsDAL();
    var noPayeeTruckwinids = dal.GetTruckWinPayeesNoPayee();
    
    if( this.EmployeesRadioButton.Checked )
    {
      payees = dal.GetEmployeePayees(true).ToList();
      newPayees= noPayeeTruckwinids.Employees.ToList();
    }
    else if (this.LessorsRadioButton.Checked)
    {
      payees =dal.GetLessorPayees(true).ToList();
      newPayees = noPayeeTruckwinids.Lessors.ToList();
    }
    else
    { 
      this.LessorsRadioButton.Checked = true;
      this.BindPayees();
      return;
    }
    
    this.PayeeListView.DataSource = payees;
    this.PayeeListView.DataBind();

    this.NewPayeesDropDown.DataSource = newPayees;
    this.NewPayeesDropDown.DataBind();
  }
  protected void SaveEditedPayeeLinkButton_Click(object sender, EventArgs e)
  {
    this.Validate();
    if (!this.IsValid)
    { return;  }

    var payee = GetPayee();

    var dal = new WebPaymentsDAL();

    if (this.LessorsRadioButton.Checked)
    {
      dal.SaveLessor(payee);
    }
    else if (this.EmployeesRadioButton.Checked)
    {
      dal.SaveEmployee(payee);
    }
    this.BindPayees();
    this.ClearEditedPayee();
  }

  private void ClearEditedPayee()
  {
    this.EditPayeeIdHiddenField.Value =
      this.EditTruckWinId.Value =
      this.EditDFIIdentification.Text =
      this.EditAccountNumber.Text = string.Empty;
  }

  private PayeeDTO GetPayee()
  {
    var payee = new PayeeDTO
    {
      PayeeId = this.GetPayeeId(),
      TruckwinId = this.GetTruckwinId(),
      DFIIdentification = this.EditDFIIdentification.Text,
      AccountNumber = this.EditAccountNumber.Text
    };
    return payee;
  }

  private string GetTruckwinId()
  {
    if ( this.SaveInput.Checked)
    {
      return this.EditTruckWinId.Value;
    }
    else if( this.AddInput.Checked)
    {
      return this.NewPayeesDropDown.Text;
    }
    throw new ArgumentException("Invalid state");
  }

  private int GetPayeeId()
  {
    int noPayeeId = -1;
    int payeeId = noPayeeId;
    if( int.TryParse(this.EditPayeeIdHiddenField.Value, out payeeId))
    {
      return noPayeeId;  
    }
    return payeeId;
  }



  protected void PayeeListView_ItemDeleting(object sender, ListViewDeleteEventArgs e)
  {
    var dal = new WebPaymentsDAL();
    var payeeIdHiddenField = PayeeListView.Items[e.ItemIndex].FindControl("PayeeIDHiddenField") as HiddenField;
    var payeeId = 0;
    if (payeeIdHiddenField == null)
    {
      return;
    }
    if (!int.TryParse(payeeIdHiddenField.Value, out payeeId))
    {
      return;
    }
    if (this.LessorsRadioButton.Checked)
    {
      dal.DeleteLessor(payeeId);
    }
    else if (this.EmployeesRadioButton.Checked)
    {
      dal.DeleteEmployee(payeeId);
    }
    this.BindPayees();
    this.ClearEditedPayee();
  }


  protected void LessorEmployee_CheckedChanged(object sender, EventArgs e)
  {
    this.BindPayees();
  }
  protected void PayeeListView_DataBound(object sender, EventArgs e)
  {
    ListViewUtilities.HidePagerOnPage1("PayeesPagerTop", this.PayeeListView);
    ListViewUtilities.HidePagerOnPage1("PayeesPagerBottom", this.PayeeListView);
  }
}
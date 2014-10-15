using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ACH_ACHFilePreview : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
      this.PopulateSettlements();
    }

    private void PopulateSettlements()
    {
      var paymentIds = this.GetPaymentIds();
      var payrollBatchIds = new int[]{};
      var controller = new ACHController();
      var achFile = controller.GetACHFilePreview(payrollBatchIds, paymentIds);
      //this.FileTextBox.Text = achFile.ToString();
      string fileName = string.Format("ACH_File_{0}_{1}.txt",
        achFile.FileHeaderRecord.FileCreationDate,
        achFile.FileHeaderRecord.FileIDModifier);

      var response = HttpContext.Current.Response;

      response.ClearContent();
      response.Clear();
      response.ContentType = "text/plain";
      response.AddHeader("Content-Disposition", string.Format("attachment; filename={0};", fileName));
    }

    private int[] GetPaymentIds()
    { 
      var paymentIds =
        this.Request
        .Form[this.Request.Form.AllKeys.Where(p => p.LastIndexOf("ettlementIdsHidden") > 0).FirstOrDefault()]
        .ToString()
        .Split(',')
        .Select(s => int.Parse(s)).ToArray();
      return paymentIds;
    }
}
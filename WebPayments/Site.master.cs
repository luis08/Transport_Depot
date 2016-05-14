using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SiteMaster : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
      if (Page.User.Identity.IsAuthenticated)
      {
        this.SetupAuthenticatedMenu();
      }
    }


    private void SetupAuthenticatedMenu()
    {
      this.NavigationMenu.Items.Add(new MenuItem
      {
        NavigateUrl = "~/ACH/PaymentsPending.aspx",
        Text = "Payments Pending"
      });
      this.NavigationMenu.Items.Add(new MenuItem
      {
        NavigateUrl = "~/ACH/Payees.aspx",
        Text = "Payees"
      });
    }
    public String WebsitePath {
      get { return Request.ApplicationPath + "/" + System.Configuration.ConfigurationManager.AppSettings["TransportDepot.Paths.Website"]; } 
    }
    public String ServicePath {
      get { return Request.ApplicationPath + "/" + System.Configuration.ConfigurationManager.AppSettings["TransportDepot.Paths.Service"]; } 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Configuration;
public partial class Test : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    TestForSection();
  }
  private void TestForSection()
  {
    var config = WebConfigurationManager.OpenWebConfiguration(null);
    var cnt = config.AppSettings.Settings.Count;
    if (cnt == 0)
      config.AppSettings.Settings.Add("theKey", "thevalue");

    var i = 9;
  }
}
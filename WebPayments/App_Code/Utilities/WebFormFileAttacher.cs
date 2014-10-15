using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
/// <summary>
/// Summary description for WebFormFileAttacher
/// </summary>
public class WebFormFileAttacher
{
  public Page Page { get; set; }

  public static void AddCss(string path, Page page)
  {
    Literal cssFile = new Literal() { Text = @"<link href=""" + page.ResolveUrl(path) + @""" type=""text/css"" rel=""stylesheet"" />" };
    page.Header.Controls.Add(cssFile);
  }

  public static void AddJavaScript(string path, Page page)
  {
    Literal javascriptFile = new Literal() { Text = @"<script src=""" + page.ResolveUrl(path) + @""" type=""text/javascript"" ></script>" };
    page.Header.Controls.Add(javascriptFile);
  }

  public void AddStyleSheet(string sheetPath)
  {
    var cssSheetLink = new HtmlGenericControl
    {
      TagName = "style",
      InnerHtml = string.Format("@import \"{0}\";", sheetPath)
    };
    cssSheetLink.Attributes.Add("type", "text/css");
    this.Page.Header.Controls.Add(cssSheetLink);
  }


  private void AddJavaScript()
  {
    HtmlGenericControl js = new HtmlGenericControl("script");
    js.Attributes["type"] = "text/javascript";
    js.Attributes["src"] = "/Scripts/Vehicles.aspx.js";
    this.Page.Header.Controls.Add(js);
  }
  [Serializable]
  class UnassignedPageException : ArgumentNullException
  {
    public UnassignedPageException(string message)
      : base(message)
    { }
  }
}

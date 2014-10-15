using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for ListViewDisplayUtilities
/// </summary>
public static class ListViewUtilities
{
  public static void HidePagerOnPage1( string pagerId, ListView listView )
  {
    var pager = listView.FindControl(pagerId) as DataPager;
    if (pager != null)
    {
      pager.Visible = pager.Visible = (pager.PageSize < pager.TotalRowCount);
    }
  }
}
<%@ WebHandler Language="C#" Class="ACHFileGenerator" %>

using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
public class ACHFileGenerator : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
{

  public void ProcessRequest(HttpContext context)
  {
    /*
    if (PaymentsFound(context))
    {
      DownloadTheFile(context);
    }
    */
    if (this.SessionParametersExist(context))
    {
      this.RenderTheACHFile(context);
    }
    context.Response.End();
    
  }

  private void DownloadTheFile(HttpContext context)
  {
    var controller = new ACHController();
    var achFile = controller.GetACHFilePreview(this.scheduledSalaryBatchIds, this.scheduledSettlementPayments);
    var fileName = string.Format("ACHPayments_{0}_{1}.txt",
      achFile.FileHeaderRecord.FileCreationDateTime.ToString("yyyy_MM_dd"),
      achFile.FileHeaderRecord.FileIDModifier);
    var contentDisposition = string.Format("attachment; filename={0}", fileName);
    context.Response.ContentType = "text/plain";
    context.Response.AddHeader("Content-Disposition", contentDisposition);
    context.Response.Write(achFile.ToString());
    context.Response.Flush();
  }

  private void RenderTheACHFile(HttpContext context)
  {
    var controller = new ACHController();
    var achFileText = controller.GetSavedACHFileText(this._fileCreationDate, this._fileIdModifier);
    var fileName = string.Format("ACHPayments_{0}_{1}.txt",
      this._fileCreationDate.ToString("yyyy_MM_dd"),
      this._fileIdModifier);
    var contentDisposition = string.Format("attachment; filename={0}", fileName);
    context.Response.ContentType = "text/plain";
    context.Response.AddHeader("Content-Disposition", contentDisposition);
    context.Response.Write(achFileText);
    context.Response.Flush();
  }
  


  public bool IsReusable
  {
    get
    {
      return false;
    }
  }

  private int[] scheduledSettlementPayments;
  private int[] scheduledSalaryBatchIds;
  private DateTime _fileCreationDate;
  private char _fileIdModifier;
  private bool PaymentsFound(HttpContext context)
  {
    this.scheduledSettlementPayments = (int[])(context.Session["ScheduledSettlementPayments"] ?? new int[] { });
    this.scheduledSalaryBatchIds = (int[])(context.Session["ScheduledSalaryBatchIds"] ?? new int[] { });
    context.Session.Remove("ScheduledSettlementPayments");
    context.Session.Remove("ScheduledSalaryBatchIds");
    var paymentsFound = (scheduledSalaryBatchIds.Length > 0) || (scheduledSettlementPayments.Length > 0);

    return paymentsFound;

  }
  private bool SessionParametersExist(HttpContext context)
  {
    var fileCreationDateObject = context.Request.QueryString[ACHController.DownloadFileCreationDateKey];
    var fileIdModifierObject = context.Request.QueryString[ACHController.DownloadFileIdModiferKey];
    
    if (fileCreationDateObject == null)
    { return false; }
    else if (fileIdModifierObject == null)
    { return false; }
    else if (!DateTime.TryParseExact(fileCreationDateObject.ToString(), "yyyyMMdd", new CultureInfo("en-US"), DateTimeStyles.AssumeLocal, out this._fileCreationDate))
    { return false; }
    else if (!char.TryParse(fileIdModifierObject.ToString(), out this._fileIdModifier))
    { return false; }
    
    return true;
  }
}
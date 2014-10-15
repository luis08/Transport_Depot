<%@ WebHandler Language="C#" Class="ClientValidator" %>

using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
public class ClientValidator : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
{

  public void ProcessRequest(HttpContext context)
  {
    context.Response.ContentType = "text/plain";
    context.Response.Write(this.GetResponse(context));
    
    context.Response.End();
  }

  public bool IsReusable
  {
    get
    {
      return false;
    }
  }

  private const string ErrorToReturn = "<<NOT_FOUND>>";
  private string GetResponse(HttpContext context)
  {
    var dal = new DBWebPaymentsDAL();
    var truckwinIds = dal.GetTruckWinPayees();
    var idRequested = this.ReadIdFromQueryString(context).ToUpper();
    
    
    if (this.RequestedLessors(context))
    {
      var foundLessor = truckwinIds.Lessors.Where(l => l.Equals(idRequested, StringComparison.OrdinalIgnoreCase));
      
      return foundLessor.Count() > 0 ? foundLessor.First() 
         : ErrorToReturn;
    }
    else if (this.RequestedEmployees(context))
    {
      var foundEmployee = truckwinIds.Employees.Where(e => e.Equals(idRequested, StringComparison.OrdinalIgnoreCase));
      return foundEmployee.Count() > 0 ? foundEmployee.First() : ErrorToReturn;
    }
    return ErrorToReturn;
  }

  private string ReadIdFromQueryString(HttpContext context)
  {
    var queryStringId = context.Request.QueryString[ClientValidatorKeys.Id];
    if (queryStringId == null)
    {
      return string.Empty;
    }
    return queryStringId.ToString();
  }

  private bool RequestedLessors(HttpContext context)
  {
    var queryStringPayeeType = context.Request.QueryString[ClientValidatorKeys.PayeeType];
    if (queryStringPayeeType == null)
    { return false; }
    else if (queryStringPayeeType.ToString() == ClientValidatorKeys.PayeeTypeValue.LessorId)
    { return true; }
    return false;
  }

  private bool RequestedEmployees(HttpContext context)
  {
    var queryStringPayeeType = context.Request.QueryString[ClientValidatorKeys.PayeeType];
    if (queryStringPayeeType == null)
    { return false; }
    else if (queryStringPayeeType.ToString() == ClientValidatorKeys.PayeeTypeValue.EmployeeId)
    { return true; }
    return false;
  }
  
  
}
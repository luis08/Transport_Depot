using TransportDepot.Models.Dispatch;
using TransportDepot.Models.Business;
using TransportDepot.Models.DB;
using TransportDepot.Models.Utilities;
using System;

namespace TransportDepot.Dispatch.CompanySetup
{
  class CompanySetupEmailBuilder
  {
    private CompanySetupRequest _request;
    private Company _company;
    private Customer _customer;
    public CompanySetupEmailBuilder(CompanySetupRequest request, Company company, Customer customer)
    {
      this._request = request;
      this._company = company;
    }

    public EmailModel GetEmailModel()
    {
      var b = this.GetEmailHtml();
      var emailModel = new EmailModel
      {
        To = new string[] { _request.Email },
        Subject = _company.Name + " Company Setup",
        IsHtml = true,
        Body = this.GetEmailHtml()
      };
      return emailModel;
    }
    private string GetEmailHtml()
    {
      this._customer = new Customer
      {
        CustomerName = "El bravo trucking", 
        Phone = "333.333.3333",
        Fax = "444.444.4444"
      };
      this._company = new Company
      {
        Name = "Fast Transport"
      };
      var html = this.GetTemplate();
      html = html.Replace("{{Date}}", this._request.Date.ToShortDateString())
        .Replace("{{To}}", _customer.CustomerName)
        .Replace("{{Attn}}", _request.ContactName)
        .Replace("{{Email}}", _request.Email)
        .Replace("{{Phone}}", _customer.Phone)
        .Replace("{{Fax}}", _customer.Fax)
        .Replace("{{Comments}}", _request.Comments)
        .Replace("{{From}}", _request.EmployeeName)
        .Replace("{{FromEmail}}", _request.EmployeeEmail);
        

      return html;
    }

    private string GetTemplate()
    {
      var directory = AppDomain.CurrentDomain.BaseDirectory;
      var fullPath = string.Format(@"{0}\Templates\company_setup_email_template.html", directory);
      var template = System.IO.File.ReadAllText(fullPath);
      return template;
    }

  }
}

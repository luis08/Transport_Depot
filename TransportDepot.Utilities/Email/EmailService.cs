using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.ComponentModel;
using System.Net;

namespace TransportDepot.Utilities.Email
{
  public class EmailService: IEmailService
  {
    public int GetQueueId(int emailId)
    {
      throw new NotImplementedException();
    }
    private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
      var j = 1;
    }
    public int Send(Models.Utilities.EmailModel email)
    {
      //http://customer.comcast.com/help-and-support/internet/email-client-programs-with-xfinity-email/
      if(this.HasNoRecipients(email)) return -1;
      SmtpClient client = GetSMTPClient();
      client.Credentials = this.GetClientCredentials(client);

      var postMan = GetPostman();

      MailMessage message = new MailMessage
        {
          From = postMan,
          Subject = email.Subject,
          Body = email.Body,
          IsBodyHtml = email.IsHtml,
          BodyEncoding = System.Text.Encoding.UTF8,
          SubjectEncoding = System.Text.Encoding.UTF8
        };
      email.To.ToList().ForEach(e => message.To.Add(e));
      client.SendCompleted += SendCompletedCallback;

      client.SendAsync(message, "no token required" );
      
      message.Dispose();
      
      return 1; 
    }

    private static MailAddress GetPostman()
    {
      var postMan = new MailAddress(
        Utilities.GetConfigSetting("SMTP.Postman.Email"),
        Utilities.GetConfigSetting("SMTP.Postman.DisplayName"),
        System.Text.Encoding.UTF8);
      return postMan;
    }

    private NetworkCredential GetClientCredentials(SmtpClient client)
    {
      var credentials = new System.Net.NetworkCredential(
        Utilities.GetConfigSetting("SMTP.UserName"),
        Utilities.GetConfigSetting("SMTP.Password"));
      return credentials;
    }

    private SmtpClient GetSMTPClient()
    {
      var portNumber = Utilities.GetConfigSetting("SMTP.Port");
      var intPortNumber = 0;
      
      if(string.IsNullOrEmpty(portNumber) || (! int.TryParse(portNumber, out intPortNumber)))
      {
        throw new InvalidOperationException("Must specify smtp.port in the Config file");
      }

      var host = Utilities.GetConfigSetting("SMTP.Host");
      if( string.IsNullOrEmpty(host) ) throw new InvalidOperationException("Must specify smtp.host in config file");

      var client = new SmtpClient(host, intPortNumber);
      return client;
    }

  

    

    private bool HasNoRecipients(Models.Utilities.EmailModel email)
    {
      if (email == null) return true;
      if (email.To == null) return true;

      var nonEmptyEmails = email.To.Where(e => !string.IsNullOrEmpty(e))
        .Where(e=> EmailUtilities.IsValidEmailAddress(e))
        .ToList();

      return nonEmptyEmails.Count < 1;
    }



    
  }
}

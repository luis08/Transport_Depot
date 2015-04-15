using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.ComponentModel;

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

      // Command line argument must the the SMTP host.
      SmtpClient client = new SmtpClient(this.Host, 587);
      client.Credentials = new System.Net.NetworkCredential("jantran@transportdepot.comcastbiz.net", "jantran1963");
      // Specify the e-mail sender. 
      // Create a mailing address that includes a UTF8 character 
      // in the display name.
      MailAddress from = new MailAddress("mail@transportdepot.net", 
         "Postman",
      System.Text.Encoding.UTF8);

      // Set destinations for the e-mail message.
      // Specify the message content.
      MailMessage message = new MailMessage();
      email.To.ToList().ForEach(e => message.To.Add(e));
      message.From = from;
      message.Body = email.Body;
      // Include some non-ASCII characters in body and subject. 
      message.IsBodyHtml = email.IsHtml;
      message.BodyEncoding = System.Text.Encoding.UTF8;
      message.Subject = email.Subject;
      message.SubjectEncoding = System.Text.Encoding.UTF8;
      // Set the method that is called back when the send operation ends.
      //client.SendCompleted += new
      //SendCompletedEventHandler(SendCompletedCallback);

      // The userState can be any object that allows your callback  
      // method to identify this send operation. 
      // For this example, the userToken is a string constant. 
      string userState = "test message1";
      //client.SendAsync(message, userState);
      client.Send(message);
      Console.WriteLine("Sending message... press c to cancel mail. Press any other key to exit.");
      
      // If the user canceled the send, and mail hasn't been sent yet, 
      // then cancel the pending operation. 
      
      // Clean up.
      message.Dispose();
      Console.WriteLine("Goodbye.");
      return 1; 
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



    private string Host { get { return "smtp.w14d.comcast.net"; } }
  }
}

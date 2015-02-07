using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Utilities.Email
{
  public class EmailService: IEmailService
  {
    public int GetQueueId(int emailId)
    {
      throw new NotImplementedException();
    }

    public int Send(Models.Utilities.EmailModel email)
    {
      if(this.HasNoRecipients(email)) return -1;
            
      throw new NotImplementedException();
      
    }

  

    

    private bool HasNoRecipients(Models.Utilities.EmailModel email)
    {
      if (email == null) return true;
      if (email.To == null) return true;

      var nonEmptyEmails = email.To.Where(e => !string.IsNullOrEmpty(e))
        .Where(e=> EmailUtilities.IsValidEmailAddress(e))
        .ToList();

      return nonEmptyEmails.Count > 0;
    }

  
  }
}

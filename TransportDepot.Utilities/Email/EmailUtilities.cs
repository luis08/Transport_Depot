
using System;
using System.Text.RegularExpressions;
namespace TransportDepot.Utilities.Email
{
  public class EmailUtilities
  {
    /// <summary>
    /// Checks for valid email address format.  Suggested in sample by Microsoft
    /// 2/4/2015  7:15 AM
    /// https://msdn.microsoft.com/en-us/library/01escwtf(v=vs.110).aspx
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static bool IsValidEmailAddress(string email)
    {
      return Regex.IsMatch(email,
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                RegexOptions.IgnoreCase);
    }
  }
}


using System;
using System.Collections.Generic;
using System.Linq;

namespace TransportDepot.Data.Safety
{
  public class ArgumentUtils
  {
    public void CheckNotNull(object o)
    {
      if (o == null)
      {
        throw new ArgumentNullException();
      }
    }

    public void CheckNotEmpty(IEnumerable<object> collection)
    {
      CheckNotNull(collection);
      if (collection.Count() == 0)
      {
        throw new ArgumentException("Collection cannot be null");
      }
    }

    public void CheckNotEmpty(string str)
    {
      CheckNotNull(str);
      if (str.Length == 0)
      {
        throw new ArgumentException("TractorId cannot be null");
      }
    }

    public bool IsDefault(DateTime dateTime)
    {
      return dateTime.Equals(DateTime.MinValue);
    }

    public void CheckNotDefault(DateTime dateTime)
    {
      if (dateTime.Equals(DateTime.MinValue))
      {
        throw new ArgumentException("Date Cannot be default value");
      }
    }

    public void CheckDateRangeNotEqual(DateTime from, DateTime to)
    {
      if (from.Equals(to))
      {
        throw new ArgumentException("From and To Dates cannot be the same");
      }
    }

    public void AssertTrue(bool boolean, string message)
    {
      if (boolean)
      {
        throw new ArgumentException(message);
      }
    }

    public IEnumerable<DateTime> parseDates(string dates)
    {
      CheckNotEmpty(dates);
      var dateStrings = dates.Split(',');
      
      return dateStrings.Select(d => 
      {
        DateTime validDate;
        if (!DateTime.TryParse(d, out validDate))
        {
          throw new ArgumentException("Cannot parse date");
        }
        return validDate;
      });
    }
  }
}

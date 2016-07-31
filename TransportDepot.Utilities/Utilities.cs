using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Utilities
{
  static class Utilities
  {
    public static int InvalidIntValue = int.MaxValue;
    public static string GetConfigSetting(string key)
    {
      if (string.IsNullOrEmpty(key)) return string.Empty;
      var settingValue = System.Configuration.ConfigurationManager.AppSettings[key];
      
      if (string.IsNullOrEmpty(settingValue)) return string.Empty;

      return settingValue.Trim();
    }

    public static int GetIntSetting(string key)
    {
      var value = GetConfigSetting(key);
      var intValue = -1;
      var invalidInt = int.MaxValue;
      return int.TryParse(value, out intValue) ? intValue : invalidInt;
    }
  }
}

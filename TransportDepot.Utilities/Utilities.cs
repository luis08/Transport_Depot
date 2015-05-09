using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Utilities
{
  static class Utilities
  {
    public static string GetConfigSetting(string key)
    {
      if (string.IsNullOrEmpty(key)) return string.Empty;
      var settingValue = System.Configuration.ConfigurationManager.AppSettings[key];
      
      if (string.IsNullOrEmpty(settingValue)) return string.Empty;

      return settingValue.Trim();
    }
  }
}

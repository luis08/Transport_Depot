using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.Data
{
  interface IDataSource
  {
    string ConnectionString { get; }
  }

  class DataSource : IDataSource
  {
    public string ConnectionString
    {
      get
      {
        var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AccessReplacementConnectionString"].ConnectionString;
        return connectionString;
      }  
    }
  }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

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

    public DataTable FetchDataTable(SqlCommand cmd)
    {
      var tbl = new DataTable();
      using(var cn = new SqlConnection(this.ConnectionString))
      using (var adapter = new SqlDataAdapter(cmd))
      {
        cmd.Connection = cn;
        adapter.Fill(tbl);
        return tbl;
      }
    }
  }

}

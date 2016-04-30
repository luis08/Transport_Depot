using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TransportDepot.Data.DB
{
  class DataSource:IDataSource
  {

    public string ConnectionString
    {
      get 
      {
        var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AccessReplacementConnectionString"].ConnectionString;
        return connectionString;      
      }
    }

    internal int ExecuteNonQuery(SqlCommand cmd)
    {
      using (var cn = new SqlConnection(this.ConnectionString))
      {
        cmd.Connection = cn;
        cmd.CommandTimeout = Utilities.DefaultCommandTimeout;
        if (cn.State != ConnectionState.Open)
          cn.Open();
        return cmd.ExecuteNonQuery();
      }
    }

    internal DataTable FetchCommand(SqlCommand cmd)
    {
      var dataTable = new DataTable();
      using (var cn = new SqlConnection(this.ConnectionString))
      using(var adapter = new SqlDataAdapter(cmd))
      {
        cmd.Connection = cn;
        cmd.CommandTimeout = Utilities.DefaultCommandTimeout;
        adapter.Fill(dataTable);
      }
      return dataTable;
    }

    internal SqlPage GetPager(int fromPage, int itemsPerPage)
    {
      return new SqlPage
      {
        ItemsPerPage = itemsPerPage,
        PageNumber = fromPage
      };
    }

    internal static bool IsEmpty(DataTable mapTable)
    {
      if (mapTable == null)
      { return true; }
      return mapTable.Rows.Count == 0;
    }

  }
}

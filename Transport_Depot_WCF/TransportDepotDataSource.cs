using System.Linq;
using System.Data;

namespace Transport_Depot_WCF
{
  class TransportDepotDataSource
  {
    protected string GetACHFileConnectionString()
    {
      return System.Configuration.ConfigurationManager
      .ConnectionStrings["ACHFileStorageConnectionString"].ConnectionString;
    }

    protected string GetTransportDepotAccessConnectionString()
    {
      return System.Configuration.ConfigurationManager
      .ConnectionStrings["TransportDepot_Mdb_ConnectionString"].ConnectionString;
    }

    protected string GetTransportDepotWebAppConnectionString()
    {
      return System.Configuration.ConfigurationManager
      .ConnectionStrings["TransportDepot_Web_App_Mdb_ConnectionString"].ConnectionString;
    }
    
    protected bool IsEmpty(DataTable dataTable)
    {
      if (dataTable == null)
      { return true; }
      if (dataTable.Rows  == null)
      { return true; }
      if (dataTable.Rows.Count == 0)
      { return true; }
      return false;
    }

    protected bool IsEmpty( DataSet dataSet )
    {
      if( dataSet == null )
      { return true; }
      else if( dataSet.Tables == null )
      {return true;}

      foreach (DataTable table in dataSet.Tables)
      {
        if (this.IsEmpty(table))
        { return true; }
      }

      return false;
    }
  }
}



namespace TransportDepot.Data
{
  class SqlPage
  {
    public int PageNumber { get; set; }
    public int ItemsPerPage { get; set; }
    public int FromRow 
    {
      get
      {
        return this.ItemsPerPage * this.PageNumber;
      }
    }
    public int ToRow
    {
      get
      {
        return this.FromRow + this.ItemsPerPage;
      }
    }
  }
}

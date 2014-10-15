using System;
namespace TransportDepot.Factoring
{
  class FilterNormalizer
  {
    public int PageNumber { get; set; }
    public int RowsPerPage { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public void Normalize()
    {
      if (this.FromDate > this.ToDate)
      {
        var fromDate = this.FromDate;
        this.FromDate = this.ToDate;
        this.ToDate = fromDate;
      }

      if (this.PageNumber < 1)
      { this.PageNumber = 1; }
      if (this.RowsPerPage < 1)
      { this.RowsPerPage = DefaultRowsPerPage; }
    }
    private const int DefaultRowsPerPage = 10;
  }

}
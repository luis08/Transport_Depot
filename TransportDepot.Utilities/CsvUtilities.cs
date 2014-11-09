using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TransportDepot.Utilities.Csv;

namespace TransportDepot.Utilities
{
  public class CsvUtilities: ICsvService
  {
    public IEnumerable<string[]> ReadCsv(Stream csv)
    {
      using (var reader = new CsvFileReader(csv))
      {
        return ReadCsv(reader);
      }
    }
    public IEnumerable<string[]> ReadCsv(string path)
    {
      using (var reader = new CsvFileReader(path))
      {
        return ReadCsv(reader);
      }
    }

    private IEnumerable<string[]> ReadCsv(CsvFileReader reader)
    {
      var rows = new LinkedList<string[]>();
      var columns = new List<string>();
      var maxCol = 0;
      while (reader.ReadRow(columns))
      {
        rows.AddLast(columns.ToArray());
        maxCol = Math.Max(maxCol, columns.Count);
      }
      return rows;
    }





    public IEnumerable<string[]> ReadCsvFromUri(string uri, Stream stream)
    {
      //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
      //req.Method = "GET";

      //req.MediaType = "HTTP/1.1";

      //HttpWebResponse response = (HttpWebResponse)req.GetResponse();
      //var reqStream = response.GetResponseStream();
      return ReadCsv(stream);
    }


    public IEnumerable<string[]> ReadCsvRequest(CsvRequest request)
    {
      throw new NotImplementedException();
    }
  }
}

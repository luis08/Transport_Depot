
using PdfSharp.Pdf;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfSharp.Pdf.IO;
using System.ServiceModel.Web;

namespace TransportDepot.Reports
{
  class PdfReportUtilities
  {
    public PdfDocument Merge(IEnumerable<PdfDocument> docs)
    {
      if (docs == null)
      {
        return new PdfDocument();
      }
      
      var mergedDoc = new PdfDocument();
      docs.ToList().ForEach(pdf =>
        {
          var stream = new MemoryStream();
          pdf.Save(stream, false);
          var importableDoc = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
          for (int i = 0; i < importableDoc.Pages.Count; i++)
          {
            var page = importableDoc.Pages[i];
            mergedDoc.AddPage(page);
          }
        });
      return mergedDoc;
    }

    public Stream ToStream(PdfDocument pdf)
    {
      var stream = new MemoryStream();
      pdf.Save(stream, false);
      stream.Position = 0;
      return stream;
    }

    public void SetResponseToPdf()
    {
      WebOperationContext.Current.OutgoingResponse.ContentType = "application/pdf";
    }
    
    public void SetResponseToPdf(string fileName)
    {
      WebOperationContext.Current.OutgoingResponse.ContentType = "application/pdf";
      WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-disposition", "inline; filename=" + fileName);
    }
  }
}

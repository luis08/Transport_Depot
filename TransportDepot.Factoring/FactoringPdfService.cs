

using TransportDepot.Models.Factoring;
using System;
using PdfSharp.Pdf;
using System.IO;
using PdfSharp.Pdf.IO;
using System.Linq;
using System.ServiceModel.Web;

namespace TransportDepot.Factoring
{
  class FactoringPdfService:IFactoringPdfService
  {
    public System.IO.Stream GetSchedulePdf(int id)
    {
      var factoringService = new FactoringService();

      var model = factoringService.GetSchedule(id);
      var schedule = this.GetSchedulePdf(model);
      var assingmentSheet = this.GetAssingmentSheetPdf(model);

      var schedulePdf = schedule.GetInvoiceSchedule();
      var assingmentSheetPdf = assingmentSheet.GetAssingmentSheet();

      var fullDoc = new PdfDocument();
      Action<PdfDocument> addDocument = (doc) =>
      {
        var stream = new MemoryStream();
        doc.Save(stream, false);
        var importableDoc = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
        for (int i = 0; i < importableDoc.Pages.Count; i++)
        {
          var page = importableDoc.Pages[i];
          fullDoc.AddPage(page);
        }
      };
      addDocument(assingmentSheetPdf);
      addDocument(schedulePdf);
      var finalDocStream = new MemoryStream();
      fullDoc.Save(finalDocStream, false);
      finalDocStream.Position = 0;
      WebOperationContext.Current.OutgoingResponse.ContentType = "application/pdf";
      return finalDocStream;
    }

    private ApexAssingmentSheet GetAssingmentSheetPdf(ScheduleViewModel model)
    {
      string settingsPath = System.Configuration.ConfigurationManager.AppSettings["Factoring.ApexAssingmentSheetPath"];
      string defaultUserName = System.Configuration.ConfigurationManager.AppSettings["Factoring.DefaultUserFullName"];
      string apexClientNumber = System.Configuration.ConfigurationManager.AppSettings["Factoring.ApexClientNumber"];
      var path = System.Web.Hosting.HostingEnvironment.MapPath(settingsPath);
      var sheet = new ApexAssingmentSheet
      {
        PdfPath = path,
        ScheduleId = model.Id,
        ClientName = System.Configuration.ConfigurationManager.AppSettings["Factoring.ApexClientName"],
        ClientAddress1 = System.Configuration.ConfigurationManager.AppSettings["Factoring.ApexClientAddress1"],
        ClientAddress2 = System.Configuration.ConfigurationManager.AppSettings["Factoring.ApexClientAddress2"],
        ClientWatts = System.Configuration.ConfigurationManager.AppSettings["Factoring.ApexClientWatts"],
        ClientLocalPhone = System.Configuration.ConfigurationManager.AppSettings["Factoring.ApexClientLocalPhone"],
        ClientFax = System.Configuration.ConfigurationManager.AppSettings["Factoring.ApexClientFax"],
        ClientNumber = apexClientNumber,
        UserFullName = defaultUserName,
        Date = model.Date,
        Invoices = model.Invoices.OrderBy(n => n.Number)
      };
      return sheet;
    }

    private ApexSchedule GetSchedulePdf(ScheduleViewModel model)
    {
      string defaultUserName = System.Configuration.ConfigurationManager.AppSettings["Factoring.DefaultUserFullName"];
      string apexClientNumber = System.Configuration.ConfigurationManager.AppSettings["Factoring.ApexClientNumber"];
      
      var schedule = new ApexSchedule
      {
        ScheduleId = model.Id,
        ClientName = System.Configuration.ConfigurationManager.AppSettings["Factoring.ApexClientName"],
        ClientAddress1 = System.Configuration.ConfigurationManager.AppSettings["Factoring.ApexClientAddress1"],
        ClientAddress2 = System.Configuration.ConfigurationManager.AppSettings["Factoring.ApexClientAddress2"],
        ClientWatts = System.Configuration.ConfigurationManager.AppSettings["Factoring.ApexClientWatts"],
        ClientLocalPhone = System.Configuration.ConfigurationManager.AppSettings["Factoring.ApexClientLocalPhone"],
        ClientFax = System.Configuration.ConfigurationManager.AppSettings["Factoring.ApexClientFax"],
        ClientNumber = apexClientNumber,
        Date = model.Date,
        Invoices = model.Invoices.OrderBy(n => n.Number)
      };
      return schedule;
    }
  }
}

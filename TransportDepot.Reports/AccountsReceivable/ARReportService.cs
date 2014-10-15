using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TransportDepot.Business;
using TransportDepot.Models.AR;
using PdfSharp.Pdf;
using TransportDepot.Models.Business;
using System.ServiceModel.Web;
using TransportDepot.AccountsReceivable;

namespace TransportDepot.Reports.AccountsReceivable
{
  public class ARReportService : IARReportService
  {
    private PdfReportUtilities _utilities = new PdfReportUtilities();
    private AccountsReceivableService _arService = new AccountsReceivableService();
    public Stream GetCollectionLettersForCustomers(string ids)
    {
      WebOperationContext.Current.OutgoingResponse.ContentType = "application/pdf";
      var parsedIds = this.ParseIds(ids);
      if (parsedIds.Count().Equals(0))
      {
        return new MemoryStream();
      }
      var company = this.GetCompany();
      var collectionLetters = this.GetCollectionLetters(parsedIds);
      var model = this.GetCollectionLettersStream(collectionLetters, company);

      return model;
    }

    public Stream GetCollectionsReport(string type, string sort)
    {
      var reportStream = new MemoryStream();
      this._utilities.SetResponseToPdf();
      IEnumerable<CollectionsReportCustomer> reportData = null;
      var reportTitle = string.Empty;

      if (type.Equals("QuickPay", StringComparison.OrdinalIgnoreCase))
      {
        reportTitle = "QuickPay";
        reportData = this._arService.GetAgingDetails(0);
      }
      else if (type.Equals("Factored", StringComparison.OrdinalIgnoreCase))
      {
        reportTitle = "Factored";
        reportData = this._arService.GetAgingDetails(1);
      }
      else if (type.Equals("All", StringComparison.OrdinalIgnoreCase))
      {
        reportTitle = "All Clients";
        reportData = this._arService.GetAllAgingDetails();
      }


      var blocks = this.GetCollections(reportData);
      blocks = this.Sort(blocks, sort);
      var company = this.GetCompany();
      var pdfer = new CollectionsReportPdfer
      {
        Company = company,
        Type = reportTitle,
        FontSize = 8.0,
        CustomerBlocks = blocks
      };
      var pdf = pdfer.GetPdf();

      pdf.Save(reportStream, false);
      reportStream.Position = 0;
      return reportStream;

    }

    public Stream GetFinalLettersForCustomers(string ids)
    {
      this._utilities.SetResponseToPdf();
      var parsedIds = this.ParseIds(ids);
      if (parsedIds.Count().Equals(0))
      {
        return new MemoryStream();
      }
      var finalLetters = GetCollectionLetters(parsedIds);
      var company = this.GetCompany();
      var model = this.GetFinalLettersStream(finalLetters, company);
      return model;
    }


    public Stream GetCollectionsReportAll()
    {
      this._utilities.SetResponseToPdf();
      var aging = this._arService.GetAllAgingDetails();
      var reportStream = this.GetCollectionsReport(aging, "AllClients", "");
      return reportStream;
    }

    public Stream GetCollectionsReportFactored()
    {
      this._utilities.SetResponseToPdf();
      var aging = this._arService.GetAgingDetails(1);
      var reportStream = this.GetCollectionsReport(aging, "Factored", "");
      return reportStream; ;
    }

    public Stream GetCollectionsReportQuickPay()
    {
      this._utilities.SetResponseToPdf();
      var aging = this._arService.GetAgingDetails(0);
      var reportStream = this.GetCollectionsReport(aging, "QuickPay", "");
      return reportStream; ;
    }

    private Company GetCompany()
    {
      var businessSvc = new BusinessService();
      var company = businessSvc.GetCompany();
      return company;
    }


    private Stream GetCollectionsReport(IEnumerable<CollectionsReportCustomer> aging, string type, string sort)
    {
      var blocks = this.GetCollections(aging);
      var company = this.GetCompany();

      blocks = this.Sort(blocks, sort);
      var pdfer = new CollectionsReportPdfer
      {
        Company = company,
        Type = type,
        FontSize = 8.0,
        CustomerBlocks = blocks
      };
      var pdf = pdfer.GetPdf();
      var stream = new MemoryStream();
      pdf.Save(stream, false);
      stream.Position = 0;
      return stream;
    }

    private IEnumerable<CollectionsCustomerBlock> Sort(IEnumerable<CollectionsCustomerBlock> blocks, string sort)
    {
      if (sort.Equals("CustomerName", StringComparison.OrdinalIgnoreCase))
      {
        blocks = blocks.OrderBy(b => b.CustomerName, StringComparer.OrdinalIgnoreCase);
      }
      else if (sort.Equals("Balance", StringComparison.OrdinalIgnoreCase))
      {
        blocks = blocks.OrderByDescending(b => b.Balance);
      }
      else if (sort.Equals("Aging", StringComparison.OrdinalIgnoreCase))
      {
        blocks = blocks.OrderByDescending(b => b.Invoices.Max(i => i.Aging));
      }
      return blocks;
    }
    private IEnumerable<CollectionsCustomerBlock> GetCollections(IEnumerable<CollectionsReportCustomer> reportData)
    {
      var blocks = reportData.Select(a =>
      {
        var block = new CollectionsCustomerBlock
        {
          CustomerName = a.Customer.CustomerName,
          CustomerPhone = a.Customer.Phone,
          CreditPeriod = a.Customer.CreditLimitDayCount,
          Invoices = a.Invoices.Select(i => new TransportDepot.Models.AR.CollectionsReportInvoice
          {
            InvoiceNumber = i.InvoiceNumber,
            Date = i.Date,
            Reference = i.Reference,
            Current = i.Current,
            Late = i.Late,
            OverThirty = i.OverThirty,
            OverSixty = i.OverSixty,
            Aging = i.Aging
          }).ToList(),
          BalanceCurrent = a.Invoices.Sum(i => i.Current),
          BalanceLate = a.Invoices.Sum(i => i.Late),
          BalanceOver30 = a.Invoices.Sum(i => i.OverThirty),
          Email = a.Customer.Email
        };

        block.Balance = decimal.Add(decimal.Add(block.BalanceCurrent,
          block.BalanceLate),
          block.BalanceOver30);
        return block;
      });
      return blocks;
    }

    private IEnumerable<CollectionLetter> GetCollectionLetters(IEnumerable<string> parsedIds)
    {
      var collectionLetters = this._arService.GetCollectionLetters(parsedIds);
      return collectionLetters;
    }

    private Stream GetFinalLettersStream(IEnumerable<CollectionLetter> letters, Company company)
    {
      var pdfs = this.GetFinalLetterPdfs(letters, company);
      var mergedPdf = this._utilities.Merge(pdfs);

      var stream = new MemoryStream();
      mergedPdf.Save(stream, false);
      stream.Position = 0;
      return stream;
    }

    private Stream GetCollectionLettersStream(IEnumerable<CollectionLetter> letters, Company company)
    {
      var pdfs = this.GetCollectionLetterPdfs(letters, company);
      var mergedPdf = this._utilities.Merge(pdfs);
      var stream = new MemoryStream();

      mergedPdf.Save(stream, false);
      stream.Position = 0;
      return stream;
    }

    private List<PdfDocument> GetCollectionLetterPdfs(IEnumerable<CollectionLetter> letters, Company company)
    {
      var pdfs = new List<PdfDocument>();
      letters.ToList().ForEach(l =>
      {
        var collectionLetterPdfer = new CollectionLetterPdfer(company, l.Customer)
        {
          Invoices = l.Invoices
        };
        pdfs.Add(collectionLetterPdfer.GetPdf());
      });

      return pdfs;
    }

    private List<PdfDocument> GetFinalLetterPdfs(IEnumerable<CollectionLetter> letters, Company company)
    {
      var pdfs = new List<PdfDocument>();
      letters.ToList().ForEach(l =>
      {
        var collectionLetterPdfer = new FinalLetterPdfer
          (company, l.Customer)
        {
          Invoices = l.Invoices
        };
        pdfs.Add(collectionLetterPdfer.GetPdf());
      });

      return pdfs;
    }

    private IEnumerable<string> ParseIds(string ids)
    {
      var noIds = new string[] { };
      if (string.IsNullOrEmpty(ids))
      { return noIds; }
      else if (ids.Trim().Equals(string.Empty))
      { return noIds; }
      return ids.Split('|');
    }
  }
}

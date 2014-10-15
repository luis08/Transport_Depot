using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfSharp.Pdf;
using MigraDoc.DocumentObjectModel;
using TransportDepot.Models.Business;
using MigraDoc.DocumentObjectModel.Shapes;
using PdfSharp.Drawing;
using TransportDepot.Models.DB;

namespace TransportDepot.Reports.AccountsReceivable
{
  public abstract class ARPdfLetter
  {
    private Section _section;
    private Document _document;

    protected Section Section
    {
      get { return this._section; }
    }

    protected Document Document
    {
      get { return this._document; }
    }

    protected static class Constants
    {
      public static int CoverPage1InvoiceCount = 10;
      public static int AdditionalScheduleInvoiceCount = 30;
      public static string RowHeight = "0.5cm";
      public static string HorizontalMargin = "1.5cm";
    }

    public string CompanyName { get; set; }
    public string StreetAddress1 { get; set; }
    public string StreetAddress2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string Country { get; set; }
    public string Phone { get; set; }
    public string Fax { get; set; }
    public string Email { get; set; }
    public Customer Customer { get; set; }

    public abstract PdfDocument GetPdf();

    public ARPdfLetter(Company company, Customer customer)
    {
      this.Customer = customer;
      this.CompanyName = company.Name;
      this.StreetAddress1 = company.Address;
      this.City = company.City;
      this.State = company.State;
      this.Zip = company.ZipCode;
      this.Phone = company.Phone;
      this.Fax = company.Fax;

      this._document = new Document();
      this.DefineStyles();
      this._section = this._document.AddSection();
      this.AddHeader();
      this.AddFooter();
      this.AddCustomerContactInformation();
      this.StyleCurrentSection();
    }

    private void AddFooter()
    {
      this.Section.PageSetup.StartingNumber = 1;
      var tbl = this.Section.Footers.Primary.AddTable();
      tbl.Style = "Table";

      tbl.Borders.Top.Width = 1;
      var leftCol = tbl.AddColumn("4cm");
      var centerCol = tbl.AddColumn("11cm");
      var rightCol = tbl.AddColumn("4cm");

      var rw = tbl.AddRow();
      rw.Cells[0].AddParagraph("Attn: Accounts Payable");
      rw.Cells[0].Format.Alignment = ParagraphAlignment.Left;
      rw.Cells[0].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
      rw.Cells[1].AddParagraph(this.Customer.CustomerName);
      rw.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
      rw.Cells[1].Format.Alignment = ParagraphAlignment.Center;

      rw.Cells[2].Format.Alignment = ParagraphAlignment.Right;
      rw.Cells[2].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
      var paragraph = rw.Cells[2].AddParagraph(); ;
      paragraph.AddTab();
      paragraph.AddPageField();
    }

    private void AddCustomerContactInformation()
    {
      this.AddCustomerAddressAndSalutation();
      this.AddCustomerFaxAndPhone();
    }

    private void AddCustomerFaxAndPhone()
    {
      var customerLabelFrame = this.Section.AddTextFrame();
      customerLabelFrame.Height = "1.0cm";
      customerLabelFrame.Width = "6cm";
      customerLabelFrame.Left = "9cm";
      customerLabelFrame.RelativeHorizontal = RelativeHorizontal.Margin;
      customerLabelFrame.RelativeVertical = RelativeVertical.Page;
      customerLabelFrame.Top = "6.5cm";

      var customerPhones = string.Concat(
        string.Format("Phone: {0}", this.Customer.Phone),
        Environment.NewLine,
        string.Format ("Fax: {0}", this.Customer.Fax));

      customerLabelFrame.AddParagraph(customerPhones);
    }

    private void AddCustomerAddressAndSalutation()
    {
      var customerLabelFrame = this.Section.AddTextFrame();
      customerLabelFrame.Height = "1.0cm";
      customerLabelFrame.Width = "13cm";
      customerLabelFrame.Left = ShapePosition.Left;
      customerLabelFrame.RelativeHorizontal = RelativeHorizontal.Margin;
      customerLabelFrame.RelativeVertical = RelativeVertical.Page;
      customerLabelFrame.Top = "4.5cm";

      customerLabelFrame.AddParagraph(DateTime.Now.ToLongDateString());
      customerLabelFrame.AddParagraph(string.Empty);
      var attnLabel = customerLabelFrame.AddParagraph("Attn: Accounts Payable");
      attnLabel.Format.Font.Bold = true;
      customerLabelFrame.AddParagraph(string.Empty);

      var customerContact = string.Concat(
        this.Customer.CustomerName, Environment.NewLine,
        this.Customer.StreetAddress, Environment.NewLine,
        this.GetCSZ(this.Customer.City,
          this.Customer.State,
          this.Customer.Zip));

      customerLabelFrame.AddParagraph(customerContact);
    }

    private void AddHeader()
    {
      var companyName = this.Section.Headers.Primary.AddParagraph(this.CompanyName);
      companyName.Format.Font.Bold = true;
      companyName.Format.Font.Size = 13;
      var addressFrame = this.Section.Headers.Primary.AddTextFrame();
      addressFrame.Height = "3.0cm";
      addressFrame.Width = "7.0cm";
      addressFrame.Left = ShapePosition.Left;
      addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
      addressFrame.RelativeVertical = RelativeVertical.Page;
      addressFrame.Top = "1.9cm";
      addressFrame.AddParagraph(this.StreetAddress1);

      addressFrame.AddParagraph(this.GetCSZ(this.City, this.State, this.Zip));
      addressFrame.AddParagraph(
        string.Format("Phone: {0}", this.Phone));
      addressFrame.AddParagraph(
        string.Format("Fax: {0}", this.Fax));

      var respondFrame = this.Section.Headers.Primary.AddTextFrame();
      respondFrame.Height = "4.0cm";
      respondFrame.Width = "5.0cm";
      respondFrame.Left = ShapePosition.Right;
      respondFrame.RelativeHorizontal = RelativeHorizontal.Margin;
      respondFrame.RelativeVertical = RelativeVertical.Page;
      respondFrame.Top = "1.7cm";

      var respondText = string.Concat(
        "Please Respond", Environment.NewLine,
        "By Fax", Environment.NewLine,
        this.Fax);

      var para = respondFrame.AddParagraph(respondText);
      para.Format.Alignment = ParagraphAlignment.Center;

      para.Format.Font.Size = 12;
      para.Format.Font.Bold = true;
      para.Format.Borders.Width = 1;

    }

    private string GetCSZ(string city, string state, string zip)
    {
      var csz = string.Format("{0}, {1} - {2}",
        city, state, zip);
      return csz;
    }

    private void DefineStyles()
    {
      Style style = this._document.Styles["Normal"];
      style.Font.Name = "Verdana";

      style = this._document.Styles[StyleNames.Heading1];
      style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Center);

      style = this._document.Styles[StyleNames.Footer];
      style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

      // Create a new style called Table based on style Normal
      style = this._document.Styles.AddStyle("Table", "Normal");
      style.Font.Name = "Verdana";
      style.Font.Size = 9;

      // Create a new style called Reference based on style Normal
      style = this._document.Styles.AddStyle("Reference", "Normal");
      style.ParagraphFormat.SpaceBefore = "5mm";
      style.ParagraphFormat.SpaceAfter = "5mm";
      style.ParagraphFormat.Font.Size = 11;
      style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
    }

    private void StyleCurrentSection()
    {
      this._section.PageSetup.LeftMargin =
        this._section.PageSetup.RightMargin = Constants.HorizontalMargin;
      this._section.PageSetup.Orientation = Orientation.Portrait;
    }
  }
}

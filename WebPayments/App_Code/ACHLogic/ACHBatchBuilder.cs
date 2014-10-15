using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebPayments.ACHElements;
using System.Configuration;

/// <summary>
/// Summary description for ACHBatchBuilder
/// </summary>
/// 
namespace ACHWebPayments.ACHLogic
{

  public class ACHBatchBuilder : ACHBuilder
  {
    public ACHBatchBuilder()
    {
      this._entry_transaction_detail_items = new LinkedList<EntryDetailTransaction>();
      this._entry_details = new LinkedList<EntryDetail>();
    }

    public string StandardEntryClass
    {
      get { return this._standard_entry_class; }
      set
      {
        var trimmed_value = value.Trim().ToUpper();
        if ((trimmed_value != "PPD") && (trimmed_value != "CCD"))
        {
          throw new ArgumentException(
            string.Format("Invalid Standard Entry Class {0}  Values can only be 'PPD' and 'CCD'.", trimmed_value));
        }
        this._standard_entry_class = trimmed_value;
      }
    }
    
    public CompanyBatch CompanyBatch { get { return this._batch; } }
    public string CompanyName { get; set; }
    public string CompanyDiscretionaryData { get; set; }
    public string CompanyEntryDescription { get; set; }
    public DateTime CompanyDescriptiveDate { get; set; }
    public DateTime EffectiveEntryDate { get; set; }
    public string OriginatorDFIBankIdentification { get; set; }
    public int BatchNumber { get; set; }
    public decimal TotalDebitEntryDecimalValue { get; set; }
    public decimal TotalCreditEntryDollarValue { get; set; }
    public string CompanyIdentification { get; set; }
    
    public void Build()
    {
      this.TotalCreditEntryDollarValue =
        this.TotalDebitEntryDecimalValue =
        decimal.Zero;

      var batchHeaderRecord = this.GetBatchHeader();
      this.CreateDetailEntries();
      this._batch = new CompanyBatch
      {
        HeaderRecord = batchHeaderRecord,
        EntryDetailRecords = this._entry_details,
        ControlRecord = this.GetBatchControl()
      };

    }

    public void AddCreditEntryDetail(EntryDetailTransaction entry_detail)
    {
      this._entry_transaction_detail_items.AddLast(entry_detail);
      this.TotalCreditEntryDollarValue = decimal.Add(
        this.TotalCreditEntryDollarValue,
        entry_detail.Amount);

    }





    private CompanyBatchControl GetBatchControl()
    {
      var templateControl = ConfigurationManager.GetSection("webPaymentsGroup/companyBatchControl") as
        WebPayments.ACHStaticElements.CompanyBatchControl;
      var controlRecord = new CompanyBatchControl
      {
        RecordTypeCode = templateControl.RecordTypeCode,
        ServiceClassCode = templateControl.ServiceClassCode,
        EntryAddendaCount = this.ACHIntToString(this._entry_details.Count, 6),
        EntryHash = this.ACHStandardizeString(this.GetEntryHash(), 10, ACHBuilder.EMPTY_CHAR, '0'),
        TotalDebitEntryDollarAmount = this.ACHDecimalToString(this.TotalDebitEntryDecimalValue, 12),
        TotalCreditEntryDollarAmount = this.ACHDecimalToString(this.TotalCreditEntryDollarValue, 12),
        CompanyIdentification = this.CompanyIdentification,
        MessageAuthenticationCode = templateControl.MessageAuthenticationCode,
        Reserved = templateControl.Reserved,
        OriginatingDFIBankIdentification = templateControl.OriginatingDFIBankIdentification,
        BatchNumber = this.ACHIntToString(this.BatchNumber, 7)
      };

      return controlRecord;
    }

    private string GetEntryHash()
    {
      UInt64 hash = 0;
      this._entry_details.ToList().ForEach(d => hash += UInt64.Parse(d.ReceivingDFIIdentification));

      var entryHash = string.Format("{0:D20}", hash).Substring(11);
      return entryHash;
    }

    private CompanyBatchHeader GetBatchHeader()
    {
      var templateHeader = ConfigurationManager.GetSection("webPaymentsGroup/companyBatchHeader") as
        WebPayments.ACHStaticElements.CompanyBatchHeader;

      var headerRecord = new CompanyBatchHeader
      {
        RecordTypeCode = templateHeader.RecordTypeCode,
        ServiceClassCode = templateHeader.ServiceClassCode,
        CompanyName = this.ACHStandardizeString(this.CompanyName, 16, ' ', ACHBuilder.EMPTY_CHAR),
        CompanyDiscretionaryData = this.ACHStandardizeString(this.CompanyDiscretionaryData, 20, ' ', ACHBuilder.EMPTY_CHAR),
        CompanyIdentification = this.CompanyIdentification,
        StandardEntryClassCode = this.StandardEntryClass,
        CompanyEntryDescription = this.ACHStandardizeString(this.CompanyEntryDescription, 10, ' ', ACHBuilder.EMPTY_CHAR),
        CompanyDescriptiveDate = this.ACHDateYYMMDD(this.CompanyDescriptiveDate),
        EffectiveEntryDate = this.ACHDateYYMMDD(this.EffectiveEntryDate),
        Reserved = templateHeader.Reserved,
        OriginatorStatusCode = templateHeader.OriginatorStatusCode,
        OriginatingDFIBankIdentification = templateHeader.OriginatorDFIBankIdentification,
        BatchNumber = this.ACHIntToString(this.BatchNumber, 7)
      };
      return headerRecord;
    }

    private void CreateDetailEntries()
    {
      if (this.StandardEntryClass == "CCD")
      {
        this.CreateDetailCCDEntries();
      }
      else
      {
        this.CreateDetailPPDEntries();
      }

      var offsetRecord = this.GetBatchOffsetDetailRecord();
      this._entry_details.AddLast( offsetRecord );
    }

    private void CreateDetailCCDEntries()
    {
      var templateDetailEntry = ConfigurationManager.GetSection("webPaymentsGroup/entryDetailCCD") as
        WebPayments.ACHStaticElements.EntryDetailCCD;
      this._entry_details = new LinkedList<EntryDetail>();

      foreach (var ccd in this._entry_transaction_detail_items)
      {
        this.TotalCreditEntryDollarValue = decimal.Add(this.TotalCreditEntryDollarValue, ccd.Amount);
        var obFields = this.GetOceanBankFields(
          ccd.DFIAccountNumber,
          ccd.ReceiverDFIIdentification);
        this._entry_details.AddLast(new EntryDetail
        {
          PaymentType =  ccd.Type,
          RecordTypeCode = templateDetailEntry.RecordTypeCode,
          TransactionCode = templateDetailEntry.TransactionCode,
          ReceivingDFIIdentification = obFields.ReceivingDFIBankIdentification,
          CheckDigit = obFields.CheckDigit,
          DFIAccountNumber = obFields.AccountNumber,
          Amount = this.ACHDecimalToString(ccd.Amount, 10),
          ReceiverIdentificationNumber = ACHFileBuilder.GetReceiverIdentification(ccd.ReceiverIdentficationNumber, ccd.TransactionId),
          ReceiverName = this.ACHStandardizeString(ccd.ReceiverName, 22, ' ', ACHBuilder.EMPTY_CHAR),
          DiscretionaryData = ACHFileBuilder.GetReceiverDiscretionaryData(ccd.TransactionId),
          AddendaRecordIndicator = templateDetailEntry.AddendaRecordIndicator,
          TraceNumber = this.GetTraceNumber()
        });
      }
    }

    private void CreateDetailPPDEntries()
    {
      var templateDetailEntry = ConfigurationManager.GetSection("webPaymentsGroup/entryDetailPPD") as
        WebPayments.ACHStaticElements.EntryDetailPPD;

      this._entry_details = new LinkedList<EntryDetail>();

      foreach (var ppd in this._entry_transaction_detail_items)
      {
        var obFields = this.GetOceanBankFields(
          ppd.DFIAccountNumber,
          ppd.ReceiverDFIIdentification);
        this.TotalCreditEntryDollarValue = decimal.Add(this.TotalCreditEntryDollarValue, ppd.Amount);
        this._entry_details.AddLast(new EntryDetail
          {
            PaymentType = ppd.Type,
            RecordTypeCode = templateDetailEntry.RecordTypeCode,
            TransactionCode = templateDetailEntry.TransactionCode,
            ReceivingDFIIdentification = obFields.ReceivingDFIBankIdentification,
            CheckDigit = obFields.CheckDigit,
            DFIAccountNumber = obFields.AccountNumber,
            Amount = this.ACHDecimalToString(ppd.Amount, 10),
            ReceiverIdentificationNumber = ACHFileBuilder.GetReceiverIdentification(ppd.ReceiverIdentficationNumber, ppd.TransactionId),
            ReceiverName = this.ACHStandardizeString(ppd.ReceiverName, 22, ' ', ACHBuilder.EMPTY_CHAR),
            DiscretionaryData = ACHFileBuilder.GetReceiverDiscretionaryData(ppd.TransactionId),
            AddendaRecordIndicator = templateDetailEntry.AddendaRecordIndicator,
            TraceNumber = this.GetTraceNumber()
          });
      }

    }

    private EntryDetail GetBatchOffsetDetailRecord()
    {
      var templateDetailEntry = ConfigurationManager.GetSection("webPaymentsGroup/entryDetailOffset") as
        WebPayments.ACHStaticElements.EntryDetailOffset;
      
      var obFields = this.GetOceanBankFields(
        templateDetailEntry.DFIAccountNumber,
        templateDetailEntry.ReceivingDFIIdentification);
      var offsetDebitAmount = this.TotalCreditEntryDollarValue;
      this.TotalDebitEntryDecimalValue = offsetDebitAmount;
      var offsetDetailRecord = new EntryDetail
      {
        PaymentType =  WebPayments.ACHElements.ACHPaymentType.Offset,
        RecordTypeCode = templateDetailEntry.RecordTypeCode,
        TransactionCode = templateDetailEntry.TransactionCode,
        ReceivingDFIIdentification = obFields.ReceivingDFIBankIdentification,
        CheckDigit = obFields.CheckDigit,
        DFIAccountNumber = obFields.AccountNumber,
        Amount = this.ACHDecimalToString( offsetDebitAmount, 10 ),
        ReceiverIdentificationNumber = this.ACHStandardizeString(templateDetailEntry.IdentificationNumber, 15, ' ', ACHBuilder.EMPTY_CHAR),
        ReceiverName = this.ACHStandardizeString(templateDetailEntry.ReceiverName, 22, ' ', ACHBuilder.EMPTY_CHAR ),
        DiscretionaryData = templateDetailEntry.DiscretionaryData,
        AddendaRecordIndicator = templateDetailEntry.AddendaRecordIndicator,
        TraceNumber = this.GetTraceNumber()
      };
      
      return offsetDetailRecord;
    }

    private string GetTraceNumber()
    {
      return string.Concat(
            this.OriginatorDFIBankIdentification.Substring(0, 8),
            this.ACHIntToString(this._entry_details.Count + 1, 7));
    }

    private OceanBankFields GetOceanBankFields(string accountNumber, string dfiIdentification)
    {
      return new OceanBankFields
      {
        OriginalReceivingDFIAccountNumber = accountNumber,
        OriginalReceivingDFIBankIdentification = dfiIdentification
      };
    }

    private CompanyBatch _batch;
    private LinkedList<EntryDetailTransaction> _entry_transaction_detail_items;
    private LinkedList<EntryDetail> _entry_details;
    private string _standard_entry_class;
  }
}
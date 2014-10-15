using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebPayments.ACHElements;
using System.Configuration;
using System.Text.RegularExpressions;

class ACHFileBuilder: ACHBuilder
{


  #region FileHeader Properties Supplied set from Controller
  public string ReferenceCode{ get; set; }
  public char FileIDModifier { get; set; }
  public DateTime FileDateTime { get; set; }
  public Dictionary<string, PayeeDFIAccount> PayeeAccounts { get; set; }
  #endregion FileHeader Properties Supplied

  public ACHFile GetFile()
  {
    var fileHeader = this.GetFileHeader();
    var companyBatchRecords =this.GetCompanyBatchRecords();
    this.CalculateFileAggregates(companyBatchRecords);
    ACHFile achFile = new ACHFile
    {
      FileHeaderRecord = fileHeader,
      CompanyBatchRecords = companyBatchRecords,
      FileControlRecord = this.GetFileControlRecord(companyBatchRecords),
      BlockPadding = string.Empty
    };

    this.SetBlockProperties(achFile);
    return achFile;
  }




  public static string GetLessorId(string receiverIdentification)
  {
    if (receiverIdentification.Length != 15)
    {
      throw new ArgumentException(
        string.Format("receiverIdentification Length must be 12. Received {0} for {1}",
          receiverIdentification.Length,
          receiverIdentification));
    }
    return receiverIdentification.Substring(0, 12);
  }
  public static int GetTransactionId(string receiverIdentification, string discretionaryData)
  {
    var transactionIdRadix = string.Concat(receiverIdentification.Substring(12, 3), discretionaryData);
    var transactionId = GetTransactionIdFromRadix(transactionIdRadix);
    return transactionId;
  }
  public static string GetReceiverIdentification(string lessorId, int transactionId)
  {
    var formattedReceiverId = lessorId.Trim().PadRight(12);
    var paymentIdRadix = ACHFileBuilder.GetPaymentIdRadix(transactionId);
    var transactionIdReceiverIdSubstring = paymentIdRadix.Substring(0,3);
    var formattedValue = string.Concat( formattedReceiverId, transactionIdReceiverIdSubstring);
    return formattedValue;
  }
  public static string GetReceiverDiscretionaryData(int transactionId)
  {
    var rightTwoDigits = GetPaymentIdRadix(transactionId).PadLeft(5, '0').Substring(3, 2);
    return rightTwoDigits;
  }
  private static string GetPaymentIdRadix(int paymentId)
  {
    return Convert.ToString(paymentId, 16).PadLeft(5, '0').ToUpper();
  }
  private static int GetTransactionIdFromRadix(string transactionId)
  {
    return Convert.ToInt32(transactionId, 16);
  }


  private void SetBlockProperties(ACHFile file)
  { 
    var fileString = file.ToString();
    
    var recordSize =int.Parse( this.templateHeader.RecordSize );
    var blockingFactor = int.Parse(this.templateHeader.BlockingFactor);
    var requiredBlockSize = blockingFactor * recordSize ;
    var fileStringLength = fileString.Replace(Environment.NewLine, string.Empty).Length;
    
    var fileStringRecordRemainder = fileStringLength % recordSize;
    var recordCount = (int)fileStringLength / recordSize;
    var paddingRecordNeededCount = fileStringLength < requiredBlockSize ? requiredBlockSize - fileStringLength : fileStringLength % requiredBlockSize;
    
    if( fileStringRecordRemainder != 0)
    {
      throw new Exception("Invalid File Size");
    }
    if (paddingRecordNeededCount % recordSize != 0)
    {
      throw new Exception("Invalid File Size");
    }

    var fillerRecordCount = paddingRecordNeededCount / recordSize;
    var blockCount = (int)(recordCount + fillerRecordCount) / blockingFactor;
    var fillerString = string.Empty;
    var fillerRowString = new string('9', recordSize);
    StringBuilder sb = new StringBuilder();

    for (int i = 0; i < fillerRecordCount; i++)
    {
      sb.AppendLine(fillerRowString);
    }

    file.BlockPadding =  sb.ToString();
    file.FileControlRecord.BlockCount = string.Format("{0:D6}", blockCount);
  }



  private FileHeader GetFileHeader()
  {

    this.templateHeader = ConfigurationManager.GetSection("webPaymentsGroup/fileHeader") as
      WebPayments.ACHStaticElements.FileHeader;
    var realHeader = new FileHeader
    {
      RecordTypeCode = templateHeader.RecordTypeCode,
      PriorityCode = templateHeader.PriorityCode,
      ImmediateDestination = templateHeader.ImmediateDestination,
      ImmediateOrigin = templateHeader.ImmediateOrigin,
      FileCreationDateTime = this.FileDateTime,
      FileCreationDate = this.ACHDateYYMMDD(this.FileDateTime),
      FileCreationTime = this.ACHTimeHHMM(this.FileDateTime),
      FileIDModifier = this.FileIDModifier.ToString().ToUpper(),
      RecordSize = templateHeader.RecordSize,
      BlockingFactor = templateHeader.BlockingFactor,
      FormatCode = templateHeader.FormatCode,
      ImmediateDestinationName = this.ACHStandardizeString( templateHeader.ImmediateDestinationName, 23, ' ', ACHBuilder.EMPTY_CHAR),
      ImmediateOriginName = this.ACHStandardizeString( templateHeader.ImmediateOriginName,23, ' ', ACHBuilder.EMPTY_CHAR),
      ReferenceCode = this.ReferenceCode.ToUpper()
    };
    return realHeader;
  }

  private IEnumerable<CompanyBatch> GetCompanyBatchRecords()
  {
    int batchId = 1; 
    this.SplitPPDAndCCDSettlements();
    var companyBatches = new LinkedList<CompanyBatch>();

    //Add an Company Batch per truckwin payroll batch
    this.PayrollBatches.ToList()
      .ForEach(b=> companyBatches.AddLast(this.GetPPDPayrollBatch(b.BatchId, batchId++, b.Payments)));

    //At most we will have one CCD and one PPD Settlement Batch per file --> no need to loop
    var settlementBatches = this.GetSettlementBatches(batchId);
    settlementBatches.ToList().ForEach(b => companyBatches.AddLast(b));
    return companyBatches;
  }

  private IEnumerable<CompanyBatch> GetSettlementBatches(int batchId)
  {
    var companyBatches = new LinkedList<CompanyBatch>(); 
    if (this._ccd_settlements_payable.Count() > 0)
    {
      companyBatches.AddLast(this.GetSettlementBatch(batchId++, this._ccd_settlements_payable, "CCD"));
    }
    if (this._ppd_settlements_payable.Count() > 0)
    {
      companyBatches.AddLast(this.GetSettlementBatch(batchId++, this._ppd_settlements_payable, "PPD"));
    }
    return companyBatches;
  }

  private string GetOriginatorDFIBankIdentification()
  {
    return this.templateHeader.ImmediateDestination.Substring(2, 8);
  }

  private CompanyBatch GetPPDPayrollBatch(int payrollBatchId, int companyBatchId, IEnumerable<PayablePayrollItem> payments )
  {
    var batchCompanyDiscretionaryData = this.ACHStandardizeString(
      string.Format("Payroll BatchID: {0}", payrollBatchId), 20, ' ', ACHBuilder.EMPTY_CHAR);
    var batchBuilder = new ACHWebPayments.ACHLogic.ACHBatchBuilder
    { 
      CompanyName = this.templateHeader.ImmediateOriginName,
      CompanyDiscretionaryData = batchCompanyDiscretionaryData,
      CompanyIdentification = this.templateHeader.CompanyIdentification,
      StandardEntryClass = "PPD",
      CompanyEntryDescription = this.ACHStandardizeString("PAYROLL", 10, ' ', ACHBuilder.EMPTY_CHAR),
      CompanyDescriptiveDate = this.FileDateTime,
      EffectiveEntryDate = this.FileDateTime,
      OriginatorDFIBankIdentification = this.GetOriginatorDFIBankIdentification(),
      BatchNumber = payrollBatchId
    };

    payments.ToList()
      .ForEach(p=> batchBuilder.AddCreditEntryDetail( new ACHWebPayments.ACHLogic.EntryDetailTransaction
      {
        Type = ACHPaymentType.Payroll,
        ReceiverName = string.Format("{0}, {1}", p.LastName, p.FirstName).ToUpper(),
        ReceiverDFIIdentification = this.PayeeAccounts[ p.EmployeeId ].DFIRoutingIdentification,
        DFIAccountNumber = this.PayeeAccounts[p.EmployeeId].DFIBankAccount,
        ReceiverIdentficationNumber = this.ACHStandardizeString( p.EmployeeId, 12, ' ', ACHBuilder.EMPTY_CHAR ),
        TransactionId = payrollBatchId,
        Amount = p.Amount
      }));
    batchBuilder.Build();
    return batchBuilder.CompanyBatch;
  }

  private CompanyBatch GetSettlementBatch(int batchId, IEnumerable<PayableSettlement> settlements, string ppdOrCcd )
  { 
    var batchCompanyDiscretionaryData = this.ACHStandardizeString(
      string.Format("Settl Pay:{0}", this.FileDateTime.ToShortDateString()), 20, ' ', ACHBuilder.EMPTY_CHAR);
    var batchBuilder = new ACHWebPayments.ACHLogic.ACHBatchBuilder
    {
      CompanyName = this.templateHeader.ImmediateOriginName,
      CompanyDiscretionaryData = batchCompanyDiscretionaryData,
      CompanyIdentification = this.templateHeader.CompanyIdentification,
      StandardEntryClass = ppdOrCcd,
      CompanyEntryDescription = this.ACHStandardizeString("PAYMENT", 10, ' ', ACHBuilder.EMPTY_CHAR),
      CompanyDescriptiveDate = this.FileDateTime,
      EffectiveEntryDate = this.FileDateTime,
      OriginatorDFIBankIdentification = this.GetOriginatorDFIBankIdentification(),
      BatchNumber = batchId
    };
    settlements.ToList().ForEach(s=> batchBuilder.AddCreditEntryDetail( new ACHWebPayments.ACHLogic.EntryDetailTransaction
    {
      Type = ACHPaymentType.Settlement,
      ReceiverName = s.LessorName,
      ReceiverDFIIdentification = this.PayeeAccounts[s.LessorId].DFIRoutingIdentification,
      DFIAccountNumber = this.PayeeAccounts[s.LessorId].DFIBankAccount,
      ReceiverIdentficationNumber = this.ACHStandardizeString( s.LessorId, 12, ' ', ACHBuilder.EMPTY_CHAR ),
      TransactionId = s.Id,
      Amount = s.Amount
    }));
    batchBuilder.Build();
    return batchBuilder.CompanyBatch;
  }

  private FileControl GetFileControlRecord(IEnumerable<CompanyBatch> companyBatchRecords)
  {
    var templateControl = ConfigurationManager.GetSection("webPaymentsGroup/fileControl") as WebPayments.ACHStaticElements.FileControl;

    var entryAddendaCount = companyBatchRecords.SelectMany(b => b.EntryDetailRecords).Count();
    
    var realControl = new FileControl
    {
      RecordTypeCode = templateControl.RecordTypeCode,
      BatchCount = this.ACHIntToString(this._batch_count, 6),
      BlockCount = new string(' ',6 ), 
      EntryAddendaRecordCount = this.ACHIntToString(entryAddendaCount, 8),
      EntryHash = this.ACHIntToString(this._entry_hash, 10),
      TotalDebitEntryDollarAmountInFile =  this.ACHDecimalToString(this._total_debit_in_file, 12),
      TotalCreditEntryDollarAmountInFile = this.ACHDecimalToString(this._total_credit_in_file, 12),
      Reserved = templateControl.Reserved
    };
    return realControl;
  }

  private void CalculateFileAggregates(IEnumerable<CompanyBatch> companyBatchRecords)
  {
    companyBatchRecords.ToList()
      .ForEach(b => AddToAggregates(b));
        
  }

  private void AddToAggregates(CompanyBatch batch)
  {
    this._batch_count++;
    this._total_credit_in_file = decimal.Add(this._total_credit_in_file, 
      decimal.Parse(batch.ControlRecord.TotalCreditEntryDollarAmount));
    this._total_debit_in_file = decimal.Add(this._total_debit_in_file,
      decimal.Parse(batch.ControlRecord.TotalDebitEntryDollarAmount));
    batch.EntryDetailRecords.ToList()
      .ForEach(e => this._entry_hash += ulong.Parse(e.ReceivingDFIIdentification));
  }

  private void SplitPPDAndCCDSettlements()
  {
    this._ccd_settlements_payable =
      this.Settlements.Where(p => p.IsCompany);
    this._ppd_settlements_payable =
      this.Settlements.Where(p => (!p.IsCompany));
  }

  public IEnumerable<PayableSettlement> Settlements { get; set; }
  public IEnumerable<PayableSalaryBatch> PayrollBatches { get; set; }

  private int _batch_count;
  private ulong _entry_hash;
  private decimal _total_debit_in_file;
  private decimal _total_credit_in_file;
  private IEnumerable<PayableSettlement> _ppd_settlements_payable;
  private IEnumerable<PayableSettlement> _ccd_settlements_payable;
  private WebPayments.ACHStaticElements.FileHeader templateHeader;
}

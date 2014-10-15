using System;
using System.Linq;
using WebPayments.ACHElements;
using System.Collections.Generic;
/// <summary>
/// Summary description for ACHController
/// </summary>
public class ACHController
{
  public static DateTime GetFileDateTime()
  {
    var now = DateTime.Now;
    if (now.DayOfWeek == DayOfWeek.Saturday)
    {
      return DateTime.Today.AddDays(2);
    }
    else if (now.DayOfWeek == DayOfWeek.Sunday)
    {
      return DateTime.Today.AddDays(1);
    }
    return now;
  }

  public string GetSavedACHFileText(DateTime fileCreationDate, char fileIdModifier)
  {
    var dal = new DAL.WebPaymentsDAL();
    return dal.GetSavedACHFileText(fileCreationDate, fileIdModifier);
  }

  public WebPaymentsDue GetWebPaymentsDue()
  { 
    var dal = new  DAL.WebPaymentsDAL( );
    return dal.GetWebPaymentsDue();
  }
	
	public void SaveACHFile(int [] payrollBatchIds, int[] scheduledPaymentIds, DateTime fileDateTime )
  {
    var fileDateTimeBeginningOfDay = new DateTime(fileDateTime.Year, fileDateTime.Month, fileDateTime.Day);
    var paymentsDAL = new DAL.WebPaymentsDAL();

    if (fileDateTimeBeginningOfDay == DateTime.Today)
    {
      fileDateTime = DateTime.Now;
    }
    
    var ach_file = this.GetACHFile(payrollBatchIds, scheduledPaymentIds, fileDateTime);
    
    paymentsDAL.Save( ach_file );
  }

  public ACHFile GetACHFilePreview(int[] payrollBatchIds, int[] scheduledPaymentIds)
  {
    ACHFile ach_file = this.GetACHFile(payrollBatchIds, scheduledPaymentIds);
    return ach_file;
  }

  public IEnumerable<ACHPaymentDisplayer> GetPaidACHFiles()
  {
    var dal = new DAL.WebPaymentsDAL();
    return dal.GetPaidACHFileDisplayers()
      .OrderByDescending(p=>p.CreationDate)
      .ThenByDescending(p=>p.FileIDModifier);
  }

  private ACHFile GetACHFile(int[] payrollBatchIds, int[] scheduledPaymentIds, DateTime fileDateTime)
  {
    var web_payments_dal = new DAL.WebPaymentsDAL();
    var web_payments_due = web_payments_dal.GetWebPaymentsDue();
    var accounts = web_payments_dal.GetPayeeAccounts();
    var nextFileIdModifier = web_payments_dal.GetNextFileIdModifier(fileDateTime);

    var settlements_selected = web_payments_due.Settlements
      .Join(scheduledPaymentIds, p => p.Id, q => q, (p, q) => p);
    var payroll_selected = web_payments_due.Payroll
      .Join(payrollBatchIds, p => p.BatchId, q => q, (p, q) => p);


    var ach_file_builder = new ACHFileBuilder
    {
      FileIDModifier = nextFileIdModifier,
      FileDateTime = fileDateTime,
      Settlements = settlements_selected,
      PayrollBatches = payroll_selected,
      ReferenceCode = "        ",
      PayeeAccounts = accounts
    };

    ACHFile ach_file = ach_file_builder.GetFile();
    return ach_file;  
  }
  private ACHFile GetACHFile(int[] payrollBatchIds, int[] scheduledPaymentIds)
  {
    var nextFileDateTime = ACHController.GetFileDateTime();
    return this.GetACHFile(payrollBatchIds, scheduledPaymentIds, nextFileDateTime);
  }


  public IEnumerable<PaidLessorSettlementViewModel> GetPaidLessorSettlements(DateTime fileCreationDate, char fileIdModifier)
  {
    var dal = new DAL.WebPaymentsDAL();
    return dal.GetPaidLessorSettlements(fileCreationDate, fileIdModifier)
      .OrderBy(p=>p.TransactionType).ThenBy(p=>p.LessorId);
  }

  public IEnumerable<PaidPayrollItem> GetPaidPayrollItems(DateTime fileCreationDate, char fileIdModifier)
  {
    var dal = new DAL.WebPaymentsDAL();
    return dal.GetPaidPayrollItems(fileCreationDate, fileIdModifier)
      .OrderBy(p=>p.TransactionType).ThenBy(p=>p.EmployeeId);
  }

  public static readonly string DownloadFileCreationDateKey = "FileCreationDate";
  public static readonly string DownloadFileIdModiferKey = "FileIdModifier";
}
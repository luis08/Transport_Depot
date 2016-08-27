﻿
namespace TransportDepot.Data.Misc
{
  static class WorkaroundsQueries
  {
    public static string InsertTractors = @"
      INSERT INTO [dbo].[Tractor]
      (
             [T].[cTractorId]
          ,  [T].[cSerial]
          ,  [T].[cTitle]
          ,  [T].[bLessorTruck]
          ,  [T].[cLessorOwner]
          ,  [T].[bTractorAssigned]
          ,  [T].[cDriver]
          ,  [T].[cModel]
          ,  [T].[cType]
          ,  [T].[cMake]
          ,  [T].[cYear]
          ,  [T].[dDateBought]
          ,  [T].[cuCost]
          ,  [T].[cuFMV]
          ,  [T].[nlOdometerStart]
          ,  [T].[nlOdometerEnding]
          ,  [T].[bActive]
          ,  [T].[cComment]
          ,  [T].[cLocation]
          ,  [T].[cLicensePlate]
          ,  [T].[cInsuranceName]
          ,  [T].[cInsurancePolicy]
          ,  [T].[cWeight]
          ,  [T].[cHeight]
          ,  [T].[bScheduleToMaintenance]
          ,  [T].[cTripNumber]
          ,  [T].[cLastRecordedCity]
          ,  [T].[cLastRecordedState]
          ,  [T].[dLastRecordedDateTime]
          ,  [T].[cTireSize]
          ,  [T].[dInspectionDate]
      )
      SELECT
             [T].[cTractorId]
          ,  [T].[cSerial]
          ,  [T].[cTitle]
          ,  [T].[bLessorTruck]
          ,  [T].[cLessorOwner]
          ,  [T].[bTractorAssigned]
          ,  [T].[cDriver]
          ,  [T].[cModel]
          ,  [T].[cType]
          ,  [T].[cMake]
          ,  [T].[cYear]
          ,  [T].[dDateBought]
          ,  [T].[cuCost]
          ,  [T].[cuFMV]
          ,  [T].[nlOdometerStart]
          ,  [T].[nlOdometerEnding]
          ,  [T].[bActive]
          ,  [T].[cComment]
          ,  [T].[cLocation]
          ,  [T].[cLicensePlate]
          ,  [T].[cInsuranceName]
          ,  [T].[cInsurancePolicy]
          ,  [T].[cWeight]
          ,  [T].[cHeight]
          ,  [T].[bScheduleToMaintenance]
          ,  [T].[cTripNumber]
          ,  [T].[cLastRecordedCity]
          ,  [T].[cLastRecordedState]
          ,  [T].[dLastRecordedDateTime]
          ,  [T].[cTireSize]
          ,  [T].[dInspectionDate]
      FROM [dbo].[TractorBackup] T
    ";

    public static string InsertEmployees = @"
      INSERT INTO [dbo].[PrEmployee]
      (
            [cEmployeeId]
          , [cGroup]
          , [cSSN]
          , [cMaritalStatus]
          , [cFirst]
          , [cMiddle]
          , [cLast]
          , [cAddress]
          , [cCity]
          , [cState]
          , [cZip]
          , [cHomePhone]
          , [cCellularPager]
          , [dDateHired]
          , [dDateTerminated]
          , [bActive]
          , [bIncludeInPayroll]
          , [bIncludeInDriverList]
          , [dDateOfBirth]
          , [mEmergency]
          , [mOtherComment]
          , [niPayPeriodPerYear]
          , [bExemptFedUnEmp]
          , [bExemptIncomeTaxWH]
          , [bFICAss]
          , [bFICAMedicare]
          , [cuAdditionalFedWH]
          , [cUnEmpTaxState]
          , [mAdditionalInformation]
          , [bStatutory]
          , [bDeceased]
          , [bPensionPlan]
          , [bLegalRep]
          , [bDeferredCompensation]
          , [niFedExempt]
          , [cFedTable]
          , [cStCode1]
          , [niStExempt1]
          , [cStTable1]
      )

      SELECT 
            [cEmployeeId]
          , [cGroup]
          , [cSSN]
          , [cMaritalStatus]
          , [cFirst]
          , [cMiddle]
          , [cLast]
          , [cAddress]
          , [cCity]
          , [cState]
          , [cZip]
          , [cHomePhone]
          , [cCellularPager]
          , [dDateHired]
          , [dDateTerminated]
          , [bActive]
          , [bIncludeInPayroll]
          , [bIncludeInDriverList]
          , [dDateOfBirth]
          , [mEmergency]
          , [mOtherComment]
          , [niPayPeriodPerYear]
          , [bExemptFedUnEmp]
          , [bExemptIncomeTaxWH]
          , [bFICAss]
          , [bFICAMedicare]
          , [cuAdditionalFedWH]
          , [cUnEmpTaxState]
          , [mAdditionalInformation]
          , [bStatutory]
          , [bDeceased]
          , [bPensionPlan]
          , [bLegalRep]
          , [bDeferredCompensation]
          , [niFedExempt]
          , [cFedTable]
          , [cStCode1]
          , [niStExempt1]
          , [cStTable1]
      FROM [dbo].[PrEmployeeBackup]
    ";

    public static string InsertBillingHistory = @"
      INSERT INTO [Trans].[dbo].[BillingHistory]
      (
            [nlOrderNumber]
          , [cDispatcherInit]
          , [cBOL]
          , [cTripnumber]
          , [cPronumber]
          , [bAssigned]
          , [bFinishTrip]
          , [dTripFinishDate]
          , [dBillDate]
          , [bPosted]
          , [cCustomerID]
          , [dCallerDate]
          , [cloadDescriptionCode]
          , [cOrigID]
          , [cOrigName]
          , [cOrigAddress]
          , [cOrigCity]
          , [cOrigState]
          , [cOrigZip]
          , [cOrigTel]
          , [dShipDate]
          , [cDestID]
          , [cDestName]
          , [cDestAddress]
          , [cDestCity]
          , [cDestState]
          , [cDestZip]
          , [cDestTel]
          , [dDeliveryDate]
          , [mTripInstruction]
          , [bSplit]
          , [cPONumber]
          , [mTempDirections1]
          , [mTempDirections2]
          , [mTempDirections3]
          , [cCustomerPU]
          , [bPrintout]
          , [cuSplitRevenue]
          , [bPrevReversed]
          , [cConsPronumber]
          , [cCustomerRef]
          , [cBillComment]
          , [cTempName]
          , [bPartialLoad]
          , [nbOrigPalletsIn]
          , [nbOrigPalletsOut]
          , [nbDestPalletsIn]
          , [nbDestPalletsOut]
          , [cCustomsBroker]
          , [niPriority]
          , [bPaperworkReceived]
          , [dPaperworkDate]
          , [bClaimsPending]
          , [bShipperNet]
          , [cSNImportID]
          , [nlSNImportOrder]
          , [cSNImportOffice]
          , [cSalesPersonInit]
          , [bBobtail]
          , [cCompleteInit]
          , [bUnloaded]
          , [dUnloadedDate]
          , [bDoNotPost]
      )
      SELECT
            [nlOrderNumber]
          , [cDispatcherInit]
          , [cBOL]
          , [cTripnumber]
          , [cPronumber]
          , [bAssigned]
          , [bFinishTrip]
          , [dTripFinishDate]
          , [dBillDate]
          , [bPosted]
          , [cCustomerID]
          , [dCallerDate]
          , [cloadDescriptionCode]
          , [cOrigID]
          , [cOrigName]
          , [cOrigAddress]
          , [cOrigCity]
          , [cOrigState]
          , [cOrigZip]
          , [cOrigTel]
          , [dShipDate]
          , [cDestID]
          , [cDestName]
          , [cDestAddress]
          , [cDestCity]
          , [cDestState]
          , [cDestZip]
          , [cDestTel]
          , [dDeliveryDate]
          , [mTripInstruction]
          , [bSplit]
          , [cPONumber]
          , [mTempDirections1]
          , [mTempDirections2]
          , [mTempDirections3]
          , [cCustomerPU]
          , [bPrintout]
          , [cuSplitRevenue]
          , [bPrevReversed]
          , [cConsPronumber]
          , [cCustomerRef]
          , [cBillComment]
          , [cTempName]
          , [bPartialLoad]
          , [nbOrigPalletsIn]
          , [nbOrigPalletsOut]
          , [nbDestPalletsIn]
          , [nbDestPalletsOut]
          , [cCustomsBroker]
          , [niPriority]
          , [bPaperworkReceived]
          , [dPaperworkDate]
          , [bClaimsPending]
          , [bShipperNet]
          , [cSNImportID]
          , [nlSNImportOrder]
          , [cSNImportOffice]
          , [cSalesPersonInit]
          , [bBobtail]
          , [cCompleteInit]
          , [bUnloaded]
          , [dUnloadedDate]
          , [bDoNotPost]
      FROM [dbo].[BillingHistoryBackup]
    ";

    public static string InsertArEntry = @"
      INSERT INTO [Trans].[dbo].[ArEntry]
      (
	        [cCustomerId]
          , [cPronumber]
          , [cuAmount]
          , [bFullyPaid]
          , [dDateOfInvoice]
          , [cArAcct]
          , [cTripNumber]
          , [bCanadianFunds]
          , [cShipperRef]
          , [nlOrderNumber]
          , [bPrepaid]
          , [cuCrntPymtAmt]
          , [dCrntPymtDate]
          , [cCrntCheckNo]
          , [cuBalanceDue]
      )
	      SELECT 
	        [AE].[cCustomerId]
          , [AE].[cPronumber]
          , [AE].[cuAmount]
          , [AE].[bFullyPaid]
          , [AE].[dDateOfInvoice]
          , [AE].[cArAcct]
          , [AE].[cTripNumber]
          , [AE].[bCanadianFunds]
          , [AE].[cShipperRef]
          , [AE].[nlOrderNumber]
          , [AE].[bPrepaid]
          , [AE].[cuCrntPymtAmt]
          , [AE].[dCrntPymtDate]
          , [AE].[cCrntCheckNo]
          , [AE].[cuBalanceDue]

      FROM  [dbo].[ArEntryBackup] [AE]
    ";

    public static string BackupTable = @"
      IF OBJECT_ID('dbo.{0}Backup', 'U') IS NOT NULL 
        DROP TABLE [dbo].[{0}Backup]; 
      SELECT * INTO [dbo].[{0}Backup] FROM [dbo].[{0}]
  ";

    public static string InsertRsPayable = @"
        SET IDENTITY_INSERT [dbo].[RsPayable] ON; 
        INSERT INTO [dbo].[RsPayable]
        (
              [cTripnumber]
            , [cProNumber]
            , [cLessorId]
            , [cTractor]
            , [dInvoiceDate]
            , [cGlRsAcct]
            , [cInvoiceDesc]
            , [dActivityDate]
            , [cu1099Amt]
            , [cuSettlement]
            , [cStatus]
            , [bPosted]
            , [bPrinted]
            , [bReverse]
            , [dDueDate]
            , [cCrntCheckNo]
            , [dCrntPymtDate]
            , [bCarrier]
            , [cCarrierInvoiceNo]
            , [bClaimsPending]
            , [cuBalanceDue]
            , [cuAmountToPay]
            , [bReadyToPay]
            , [cTransactionID]
            , [bPaperworkReceived]
            , [dPaperworkDate]
            , [COUNTER]
        )

        SELECT
              [P].[cTripnumber]
            , [P].[cProNumber]
            , [P].[cLessorId]
            , [P].[cTractor]
            , [P].[dInvoiceDate]
            , [P].[cGlRsAcct]
            , [P].[cInvoiceDesc]
            , [P].[dActivityDate]
            , [P].[cu1099Amt]
            , [P].[cuSettlement]
            , [P].[cStatus]
            , [P].[bPosted]
            , [P].[bPrinted]
            , [P].[bReverse]
            , [P].[dDueDate]
            , [P].[cCrntCheckNo]
            , [P].[dCrntPymtDate]
            , [P].[bCarrier]
            , [P].[cCarrierInvoiceNo]
            , [P].[bClaimsPending]
            , [P].[cuBalanceDue]
            , [P].[cuAmountToPay]
            , [P].[bReadyToPay]
            , [P].[cTransactionID]
            , [P].[bPaperworkReceived]
            , [P].[dPaperworkDate]
            , [P].[COUNTER]
        FROM [dbo].[RsPayableBackup] [P]

        SET IDENTITY_INSERT [dbo].[RsPayable] OFF;  

    ";

    public static string InsertRsPayableHistory = @"
 
        SET IDENTITY_INSERT [dbo].[RsPayableHistory] ON;  
        INSERT INTO [dbo].[RsPayableHistory]
        (
              [cTripnumber]
            , [cProNumber]
            , [cLessorId]
            , [cTractor]
            , [dInvoiceDate]
            , [cGlRsAcct]
            , [cInvoiceDesc]
            , [dActivityDate]
            , [cu1099Amt]
            , [cuSettlement]
            , [cStatus]
            , [bPrinted]
            , [bReverse]
            , [dDueDate]
            , [cCrntCheckNo]
            , [dCrntPymtDate]
            , [bCarrier]
            , [cCarrierInvoiceNo]
            , [bClaimsPending]
            , [cTransactionID]
            , [bPaperworkReceived]
            , [dPaperworkDate]
            , [COUNTER]
        )
        SELECT 
              [H].[cTripnumber]
            , [H].[cProNumber]
            , [H].[cLessorId]
            , [H].[cTractor]
            , [H].[dInvoiceDate]
            , [H].[cGlRsAcct]
            , [H].[cInvoiceDesc]
            , [H].[dActivityDate]
            , [H].[cu1099Amt]
            , [H].[cuSettlement]
            , [H].[cStatus]
            , [H].[bPrinted]
            , [H].[bReverse]
            , [H].[dDueDate]
            , [H].[cCrntCheckNo]
            , [H].[dCrntPymtDate]
            , [H].[bCarrier]
            , [H].[cCarrierInvoiceNo]
            , [H].[bClaimsPending]
            , [H].[cTransactionID]
            , [H].[bPaperworkReceived]
            , [H].[dPaperworkDate]
            , [H].[COUNTER]
        FROM [dbo].[RsPayableHistoryBackup] [H]
        SET IDENTITY_INSERT [dbo].[RsPayableHistory] OFF;     

    ";
  }
}

using System;
using System.Collections.Generic;
using Transport_Depot_WCF.ACH;
using System.Data;
using System.Data.SqlClient;

using Transport_Depot_WCF.PaymentPosting;
namespace Transport_Depot_WCF
{
  class WebPaymentsService : TransportDepotDataSource, IWebPaymentsService
  {
    internal IEnumerable<LessorPayment> GetPaidLessorSettlements(ACHFileIdentifierModel model)
    {
      var paidSettlementsDataTable = new DataTable();
      var paidSettlementsQuery = @"
          SELECT [P].[For_Batch_Number_K]       AS ACHBatchNumber
                ,[B].[Standard_Entry_Class_Code] AS TransactionType
                ,[P].[Trace_Number_Sequence_K]  AS TraceNumberSequence
                ,[For_Lessor_ID] AS LessorId
                ,[For_Payment_Scheduled_ID]  AS PaymentId
                ,[Amount]
            FROM [dbo].[ACH_Lessor_Payment] [P]
              INNER JOIN  [dbo].[ACH_Payment_Batch] [B]
                ON   [P].[For_File_Creation_Date_K] = [B].[For_File_Creation_Date_K]
                AND  [P].[For_File_ID_Modifier_K] = [B].[For_File_ID_Modifier_K]
                AND  [P].[For_Batch_Number_K] = [B].[Batch_Number_K]
            WHERE [P].[For_File_Creation_Date_K] = @FileCreationDate
              AND [P].[For_File_ID_Modifier_K]   = @FileIDModifier
      ";
      using (var cn = new SqlConnection(this.GetACHFileConnectionString()))
      using (var cmd = cn.CreateCommand())
      {
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = paidSettlementsQuery;
        cmd.Parameters.AddWithValue("@FileCreationDate", model.FileDateTime);
        cmd.Parameters.AddWithValue("@FileIDModifier", model.FileIDModifier);
        var paidSettlementsDataAdapter = new SqlDataAdapter(cmd);
        paidSettlementsDataAdapter.Fill(paidSettlementsDataTable);
      }
      if (this.IsEmpty(paidSettlementsDataTable))
      {
        return new List<LessorPayment>();
      }

      return paidSettlementsDataTable.AsEnumerable()
        .Select(p => new LessorPayment
        {
          ACHBatchNumber = p.Field<int>("ACHBatchNumber"),
          TraceNumberSequence = p.Field<string>("TraceNumberSequence"),
          LessorId = p.Field<string>("LessorId"),
          PaymentId = p.Field<int>("PaymentId"),
          TransactionType = p.Field<string>("TransactionType"),
          Amount = p.Field<decimal>("Amount")
        });
    }



  }
}

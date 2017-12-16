using TransportDepot.Models.Factoring;
using System.Collections.Generic;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace TransportDepot.Data.Factoring
{
  public class FactoringDataSource : IDataSource
  {
    public IEnumerable<ScheduleSummaryViewModel> GetSchedules(ScheduleFilter filter)
    {
      var schedulesTable = new DataTable();
      using (var cmd = new SqlCommand(Queries.SchedulesQuery))
      {
        cmd.Parameters.AddWithValue("@FromDate", filter.FromDate);
        cmd.Parameters.AddWithValue("@ToDate", filter.ToDate);
        cmd.Parameters.AddWithValue("@FromScheduleID", filter.FromSchedule);
        cmd.Parameters.AddWithValue("@ToScheduleID", filter.ToSchedule);
        schedulesTable = this.Fetch(cmd);
      }
      var schedules = this.GetSchedulesFromTable(schedulesTable);
      
      return schedules;
    }

    public IEnumerable<InvoiceViewModel> GetUnPostedInvoices(InvoiceFilter filter)
    {
      var invoicesTable = new DataTable();
      using (var cmd = new SqlCommand(Queries.InvoicesFiltered))
      {
        cmd.Parameters.AddWithValue("@FromDate", filter.FromDate);
        cmd.Parameters.AddWithValue("@ToDate", filter.ToDate);
        cmd.Parameters.AddWithValue("@FromInvoice", filter.FromInvoiceNumber);
        cmd.Parameters.AddWithValue("@ToInvoice", filter.ToInvoiceNumber);
        cmd.Parameters.AddWithValue("@OnlyWithoutSchedule", filter.OnlyWithoutSchedule ? 1 : 0);
        invoicesTable = this.Fetch(cmd);
      }
      var invoices = invoicesTable.AsEnumerable()
        .Select(i => new InvoiceViewModel
        {
          Number = i.Field<string>("Invoice_Number"),
          CustomerName = i.Field<string>("Customer_Name"),
          Date = i.Field<DateTime>("Date"),
          Amount = i.IsNull("Amount") ? decimal.Zero : i.Field<decimal>("Amount")
        });
      return invoices;
    }





    public ScheduleViewModel GetSchedule(int id)
    {
      var schedulesTable = new DataTable();
      using (var cmd = new SqlCommand(Queries.ScheduleQuery))
      {
        cmd.Parameters.AddWithValue("@ScheduleID", id);
        schedulesTable = this.Fetch(cmd);
      }
      if (schedulesTable.Rows.Count == 0)
      {
        throw new KeyNotFoundException(
          string.Format("ScheduleID not found [{0}]", id));
      }
      var schedule = schedulesTable.AsEnumerable()
        .Select(s => new ScheduleViewModel
        {
          Id = s.Field<int>("ID"),
          Date = s.Field<DateTime>("Date"),
          User = s.IsNull("Done_By") ? Guid.Empty : s.Field<Guid>("Done_By"),
          Invoices = this.GetInvoices(s.Field<int>("ID"))
        }).First();

      return schedule;
    }

    public int Create(NewScheduleViewModel schedule)
    {
      using (var cn = new SqlConnection(this.ConnectionString))
      {
        var scheduleId = this.AppendSchedule(schedule, cn);
        var newSchedule = new ScheduleViewModel
        {
          Id = scheduleId,
          Date = schedule.Date,
          User = this.GetUserId(),
          Invoices = schedule.InvoiceNumbers.Select(i=> new InvoiceViewModel
          {
            Number = i
          })
        };
        this.SaveInvoices(newSchedule, cn);
        return scheduleId;
      }
    }

    private Guid GetUserId()
    {
      return Guid.Empty;
    }

    public void Save(ScheduleViewModel schedule)
    {
      using (var cn = new SqlConnection(this.ConnectionString))
      {
        cn.Open();

        if (schedule.Id > 0)
        {
          this.UpdateSchedule(schedule, cn);
          this.SaveInvoices(schedule, cn);
        }
        else
        {
          throw new InvalidOperationException("ScheduleID must be greater than zero.");
        }
      }
    }

    public void DeleteSchedule(int id)
    {
      using (var cn = new SqlConnection(this.ConnectionString))
      {
        if (id < 1)
        {
          throw new InvalidOperationException("ScheduleID must be greater than zero");
        }
        cn.Open();
        this.ClearScheduleOnInvoices(id, cn);
        using (var cmd = new SqlCommand(Queries.DeleteSchedule, cn))
        {
          cmd.Parameters.AddWithValue("@ScheduleId", id);
          cmd.ExecuteNonQuery();
        }
      }
    }

    private IEnumerable<ScheduleSummaryViewModel> GetSchedulesFromTable(DataTable schedulesTable)
    {
      if (schedulesTable.Rows.Count == 0)
      {
        return new List<ScheduleSummaryViewModel>();
      }
      var schedules = schedulesTable.AsEnumerable()
        .Select(s => new ScheduleSummaryViewModel
        {
          Id = s.Field<int>("ID"),
          Date = s.Field<DateTime>("Date"),
          Amount = s.Field<decimal>("Amount")
        }).ToList();
      return schedules;
    }

    private void ClearScheduleOnInvoices(int scheduleId, SqlConnection cn)
    {
      var invoices = this.GetInvoices(scheduleId);
      var invoicesXml = this.GetInvoicesXml(invoices);

      using (var cmd = new SqlCommand(Queries.ClearInvoicesFromSchedule, cn))
      {
        cmd.Parameters.AddWithValue("@InvoicesXmlString", invoicesXml.ToString());
        cmd.ExecuteNonQuery();
      }
    }

    private int AppendSchedule(NewScheduleViewModel schedule, SqlConnection cn)
    {
      int newScheduleId = int.MinValue;

      using (var cmd = new SqlCommand(Queries.CreateSchedule, cn))
      {
        cmd.Parameters.AddWithValue("@Date", schedule.Date);
        cmd.Parameters.AddWithValue("@DoneBy", schedule.User);
        if (cn.State == ConnectionState.Closed)
        {
          cn.Open();
        }
        newScheduleId = Convert.ToInt32(cmd.ExecuteScalar());
      }
      if (newScheduleId.Equals(int.MinValue))
      {
        throw new ApplicationException("Unable to append Schedule");
      }
      return newScheduleId;
    }

    private void UpdateSchedule(ScheduleViewModel schedule, SqlConnection cn)
    {
      using (var cmd = new SqlCommand(Queries.UpdateSchedule, cn))
      {
        cmd.Parameters.AddWithValue("@ScheduleID", schedule.Id);
        cmd.Parameters.AddWithValue("@Date", schedule.Date);
        cmd.Parameters.AddWithValue("@DoneBy", schedule.User);
        cmd.ExecuteNonQuery();
      }
    }

    private void SaveInvoices(ScheduleViewModel schedule, SqlConnection cn)
    {
      var invoicesXml = GetInvoicesXml(schedule.Invoices);
      var rowsAffected = int.MinValue;
      using (var cmd = new SqlCommand(Queries.UpdateInvoicesQuery, cn))
      {
        cmd.Parameters.AddWithValue("@ScheduleId", schedule.Id);
        cmd.Parameters.AddWithValue("@InvoicesXmlString", invoicesXml.ToString());
        if (cn.State != ConnectionState.Open)
        {
          cn.Open();
        }
        rowsAffected = cmd.ExecuteNonQuery();
      }
      if (rowsAffected.Equals(0))
      {
        throw new ApplicationException("No invoices affected.  The invoice numbers may be invalid" + Environment.NewLine + invoicesXml.ToString());
      }
    }

    private XDocument GetInvoicesXml(IEnumerable<InvoiceViewModel> invoices)
    {
      var invoicesXml = new XDocument(
        new XElement("invoices",
          invoices.Select(i => new XElement("invoice",
            new XAttribute("number", i.Number)))));
      return invoicesXml;
    }


    private IEnumerable<InvoiceViewModel> GetInvoices(int scheduleId)
    {
      var invoiceQuery = Queries.InvoicesQuery + @"
          WHERE ( [S].[ID] = @ScheduleID )
          ORDER BY [BH].[cProNumber] 
      ";
      var invoiceTable = new DataTable();
      using (var cmd = new SqlCommand(invoiceQuery))
      {
        cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);
        invoiceTable = this.Fetch(cmd);
      }
      if (invoiceTable.Rows.Count == 0)
      {
        return new List<InvoiceViewModel>();
      }
      var invoices = invoiceTable.AsEnumerable()
        .Select(i => new InvoiceViewModel
        {
          Number = i.Field<string>("Invoice_Number"),
          Date = i.Field<DateTime>("Date"),
          CustomerName = i.Field<string>("Customer_Name"),
          Amount = i.Field<decimal>("Amount")
        }).ToList();
      return invoices;
    }

    private IEnumerable<InvoiceViewModel> GetUnpostedInvoices()
    {
      var invoiceQuery = Queries.InvoicesQuery + @"
        WHERE [bPosted] = 0 
        ORDER BY [BH].[cProNumber] 
      ";
      var invoiceTable = new DataTable();
      using (var cmd = new SqlCommand())
      {
        invoiceTable = this.Fetch(cmd);
      }
      if (invoiceTable.Rows.Count == 0)
      {
        return new List<InvoiceViewModel>();
      }
      var invoices = invoiceTable.AsEnumerable()
        .Select(i => new InvoiceViewModel
        {
          Number = i.Field<string>("Invoice_Number"),
          Date = i.Field<DateTime>("Date"),
          CustomerName = i.Field<string>("Customer_Name"),
          Amount = i.Field<decimal>("Amount")
        });
      return invoices;
    }
    private DataTable Fetch(SqlCommand cmd)
    {
      var dataTable = new DataTable();
      using (var cn = new SqlConnection(this.ConnectionString))
      using (var adapter = new SqlDataAdapter(cmd))
      {
        cmd.Connection = cn;
        adapter.Fill(dataTable);
      }
      return dataTable;
    }
    public string ConnectionString
    {
      get
      {
        var conStr = System.Configuration.ConfigurationManager.ConnectionStrings["AccessReplacementConnectionString"].ConnectionString;
        return conStr;
      }
    }

    private static class Queries
    {
      public static string InvoicesQuery = @"
          SELECT [BH].[cPronumber] AS [Invoice_Number]
               , [BH].[dBillDate] AS [Date]
               , [C].[cID] AS [Customer_ID]
               , [C].[cName] AS [Customer_Name]
               , [C].[niCreditLimitDays] AS [Credit_Limit_Days]
               , [R].[cuTotalRevenue]    AS [Amount]
               , [S].[ID] AS [ScheduleID]
               , [BH].[bPosted] AS [Is_Posted]
          FROM [dbo].[BillingHistory] [BH] 
            INNER JOIN [dbo].[Customer] [C]
              ON ( [BH].[cCustomerID] = [C].[cID] )
            INNER JOIN [dbo].[Revenue] [R]
              ON ( [BH].[cPronumber] = [R].[cProNumber] )
            INNER JOIN [dbo].[Factoring_Schedule] [S]
              ON ( [BH].[nlSNImportOrder] = [S].[ID] )
      ";
      public static string UpdateInvoicesQuery = @"
          /*
             Parameters required
              @InvoicesXmlString 
              @ScheduleId ( if = 0 ==> deletes from schedule )
          */

          DECLARE @Invoices XML
          SET @Invoices = CAST( @InvoicesXmlString AS XML )

          ;WITH [Schedule_Invoices] AS
          (
            SELECT invoice.value('./@number', 'varchar(18)') AS [Invoice_Number]
            FROM @Invoices.nodes('//invoice') AS T(invoice)
          ), [Target_Invoices] AS
          (
            SELECT [BH].[cProNumber] 
                 , [SI].[Invoice_Number] AS [Source_cProNumber]
            FROM [dbo].[BillingHistory] [BH]
              LEFT JOIN [Schedule_Invoices] [SI]
                ON ( [BH].[cProNumber] = [SI].[Invoice_Number] )
            WHERE ( [BH].[nlSNImportOrder] = @ScheduleId )
               OR ( [SI].[Invoice_Number] IS NOT NULL )
          )

          UPDATE [dbo].[BillingHistory] 
            SET [nlSNImportOrder] = CASE
						                          WHEN ( [Source_cProNumber] IS NOT NULL ) THEN @ScheduleId
						                          ELSE 0      	
                                    END
          FROM [dbo].[BillingHistory] [Target]
          INNER JOIN [Target_Invoices] [Source]
            ON ( [Target].[cProNumber] = [Source].[cProNumber] )

      ";

      public static string SchedulesQuery = @"
        ;WITH [Revenue] AS
        (
            SELECT [BH].[cProNumber] AS [Invoice_Number] 
                 , [BH].[nlSNImportOrder] AS [ScheduleID]
                 , SUM( [R].[cuTotalRevenue] ) AS [Amount]
            FROM [dbo].[BillingHistory] AS [BH]
              INNER JOIN [dbo].[Revenue] [R]
                ON  ( [BH].[cProNumber] = [R].[cProNumber] )
            GROUP BY [BH].[cProNumber]
                    ,[BH].[nlSNImportOrder]
        )

        SELECT [S].[ID] 
             , [S].[Date] 
             , [S].[Done_By] 
             , SUM( COALESCE( [R].[Amount], 0.0 ) ) AS [Amount] 
        FROM [dbo].[Factoring_Schedule] [S]
          LEFT JOIN [Revenue] [R]
            ON [R].[ScheduleID] = [S].[ID]
        WHERE ( [ID] >= @FromScheduleID ) 
          AND ( [ID] <= @ToScheduleID ) 
          AND ( [Date] >= @FromDate ) 
          AND ( [Date] <= @ToDate ) 
        GROUP BY [S].[ID] 
             , [S].[Date] 
             , [S].[Done_By] 
        ORDER BY [ID] DESC; 

        ";
      public static string InvoicesFiltered = @"

    ;WITH [LessBillingHistory] AS 
    (
      SELECT [nlOrderNumber], [bPosted], [cProNumber], [dBillDate], [nlSNImportOrder], [cCustomerID]
      FROM [dbo].[BillingHistory] [BH]
      WHERE ( [BH].[bPosted] != 0 ) 
        AND ( [BH].[cProNumber] >= @FromInvoice )
        AND ( [BH].[cProNumber] <= @ToInvoice )
        AND ( [BH].[dBillDate] >= @FromDate )
        AND ( [BH].[dBillDate] <= @ToDate )
        AND ( 
              ( ( @OnlyWithoutSchedule = 1 ) AND ( ( [BH].[nlSNImportOrder] = 0  ) OR ( [BH].[nlSNImportOrder] IS NULL ) ) )  
              OR ( COALESCE( @OnlyWithoutSchedule, 0 ) = 0  ) 
            ) 
    ), [InvoicesPaid] AS 
    ( 
      SELECT [cPronumber] AS [InvoiceNumber]
           , SUM([cuAmount]) AS [TotalPaid] 
      FROM [dbo].[ArPayment]
      GROUP BY [cPronumber]
    ), [LessRevenue] AS
    (
      SELECT [BH].[nlOrderNumber], [BH].[cProNumber], [dBillDate], [nlSNImportOrder], [cCustomerID]
      , SUM( [R].[cuTotalRevenue] ) AS [Amount]
      FROM [LessBillingHistory] [BH]
      INNER JOIN [dbo].[Revenue] [R]
        ON ( [BH].[nlOrderNumber] = [R].[nlOrderNumber] )
      GROUP BY [BH].[nlOrderNumber]
              ,[BH].[cProNumber]
              ,[BH].[dBillDate]
              ,[BH].[nlSNImportOrder]
              ,[BH].[cCustomerID]
    )
    
    SELECT [R].[nlOrderNumber] AS [Order_Number]
          ,[R].[cProNumber] AS [Invoice_Number]
          ,[R].[dBillDate] AS [Date]
          ,[C].[cName] AS [Customer_Name]
          ,[C].[cID] AS [CustomerID]
          ,[C].[niCreditLimitDays]
          ,[R].[Amount]
          FROM [LessRevenue] [R]
          INNER JOIN [dbo].[Customer] [C]
            ON ( [R].[cCustomerID] = [C].[cID] )
          WHERE ( [C].[niCreditLimitDays] = 31 )
          AND NOT EXISTS
          (
            SELECT * 
            FROM [InvoicesPaid] [I]
            WHERE ( [I].[InvoiceNumber] = [R].[cProNumber] )
              AND ( [I].[TotalPaid] > 0 )
          )
        ";


      public static string ScheduleQuery = @"
        SELECT [ID] 
             , [Date] 
             , [Done_By] 
        FROM [dbo].[Factoring_Schedule] 
        WHERE [ID] = @ScheduleID
      ";

      public static string UpdateSchedule = @"
	        UPDATE [dbo].[Factoring_Schedule]
	          SET	[Date]   =  @Date
	            , [Done_By] = @DoneBy
          WHERE [ID]      = @ScheduleID
      ";

      public static string CreateSchedule = @"
        DECLARE @LastScheduleId INT
        BEGIN TRANSACTION
	        SELECT @LastScheduleId = MAX(ID) + 1 
	        FROM [dbo].[Factoring_Schedule] [S]
	        WITH (TABLOCK)

          INSERT INTO [dbo].[Factoring_Schedule] WITH (TABLOCK)
          (
	            [ID]
	          , [Date]
	          , [Done_By]
          )
          SELECT @LastScheduleId AS [ID]
               , @Date AS [Date]
	             , @DoneBy AS [Done_By]

        COMMIT TRANSACTION
        SELECT @LastScheduleId
      ";
      public static string DeleteSchedule = @"
          DELETE FROM [dbo].[Factoring_Schedule]
          WHERE [ID] = @ScheduleId";

      public static string ClearInvoicesFromSchedule = @"
          /*
             Parameters required
              @InvoicesXmlString 
          */


          DECLARE @Invoices XML
          SET @Invoices = CAST( @InvoicesXmlString AS XML )

          ;WITH [Schedule_Invoices] AS
          (
            SELECT invoice.value('./@number', 'varchar(18)') AS [Invoice_Number]
            FROM @Invoices.nodes('//invoice') AS T(invoice)
          )
          
          UPDATE [dbo].[BillingHistory] 
              SET [nlSNImportOrder] = 0
          FROM [dbo].[BillingHistory] [Target]
            INNER JOIN [Schedule_Invoices] [Source]
              ON ( [Target].[cProNumber] = [Source].[Invoice_Number] )
      ";
    }
  }
}

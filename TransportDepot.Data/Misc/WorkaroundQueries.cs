
namespace TransportDepot.Data.Misc
{
  class WorkaroundQueries
  {
    public static string PointToEmpty = @"
		PRINT 'Starting Point to Empty';

		BEGIN TRANSACTION

			GO	

			ALTER VIEW [dbo].[ApPayableHistory]
			AS
			SELECT * FROM [dbo].[ApPayableHistory_Empty];

			GO

			PRINT '[ApPayableHistory] Done';

			GO
		 
			ALTER VIEW [dbo].[ArEntry]
			AS
			SELECT * FROM [dbo].[ArEntry_Empty]

			GO

			PRINT '[ArEntry] Done';

			GO

			ALTER VIEW [dbo].[BillingHistory]
			AS
			SELECT * FROM [dbo].[BillingHistory_Empty]

			GO

			PRINT '[BillingHistory] Done';

			GO

			ALTER VIEW [dbo].[GlEntry]
			AS
			SELECT * FROM [dbo].[GlEntry_Empty]

			GO

			PRINT '[GlEntry] Done';

			GO
			  
			ALTER VIEW [dbo].[PrEmployee]
			AS
			SELECT * FROM [dbo].[PrEmployee_Empty]

			GO

			PRINT '[PrEmployee] Done';

			GO
				 
			ALTER VIEW [dbo].[RsPayable]
			AS
			SELECT * FROM [dbo].[RsPayable_Empty]

			GO

			PRINT '[RsPayable] Done';

			GO
					 
			ALTER VIEW [dbo].[RsPayableHistory]
			AS
			SELECT * FROM [dbo].[RsPayableHistory_Empty]

			GO

			PRINT '[RsPayableHistory] Done';

			GO
					 
			ALTER VIEW [dbo].[Tractor]
			AS
			SELECT * FROM [dbo].[Tractor_Empty]

			GO

			PRINT '[Tractor] Done';

			GO
			  
			ALTER VIEW [dbo].[TripNumber]
			AS
			SELECT * FROM [dbo].[TripNumber_Empty]

			GO

			PRINT '[TripNumber] Done';

		IF @@TRANCOUNT > 0
			COMMIT TRANSACTION;

	";

    public static string PointToData = @"


		BEGIN TRANSACTION		        
      
			PRINT 'Starting Point to Data';

			GO

			ALTER VIEW [dbo].[ApPayableHistory]
			AS
			SELECT * FROM [dbo].[ApPayableHistory_Data]

			GO

			PRINT '[ApPayableHistory] Done';

			GO
   
			ALTER VIEW [dbo].[ArEntry]
			AS
			SELECT * FROM [dbo].[ArEntry_Data]

			GO

			PRINT '[ArEntry] Done';

			GO

			ALTER VIEW [dbo].[BillingHistory]
			AS
			SELECT * FROM [dbo].[BillingHistory_Data]

			GO

			PRINT '[BillingHistory] Done';

			GO

			ALTER VIEW [dbo].[GlEntry]
			AS
			SELECT * FROM [dbo].[GlEntry_Data]

			GO

			PRINT '[GlEntry] Done';

			GO
        
			ALTER VIEW [dbo].[PrEmployee]
			AS
			SELECT * FROM [dbo].[PrEmployee_Data]

			GO

			PRINT '[PrEmployee] Done';

			GO
           
			ALTER VIEW [dbo].[RsPayable]
			AS
			SELECT * FROM [dbo].[RsPayable_Data]

			GO

			PRINT '[RsPayable] Done';

			GO
               
			ALTER VIEW [dbo].[RsPayableHistory]
			AS
			SELECT * FROM [dbo].[RsPayableHistory_Data]

			GO

			PRINT '[RsPayableHistory] Done';

			GO
               
			ALTER VIEW [dbo].[Tractor]
			AS
			SELECT * FROM [dbo].[Tractor_Data]

			GO

			PRINT '[Tractor] Done';

			GO
        
			ALTER VIEW [dbo].[TripNumber]
			AS
			SELECT * FROM [dbo].[TripNumber_Data]

			GO

			PRINT '[TripNumber] Done';

			GO
	
		IF @@TRANCOUNT > 0
			COMMIT TRANSACTION;

    ";
  }
}

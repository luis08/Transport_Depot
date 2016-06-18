using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;

namespace Transport_Depot_AutomaticDeductions
{
  class DataSource
  {
    private static class UsedDeductions
    {
      private static Dictionary<string, int> settings = new Dictionary<string, int>( StringComparer.OrdinalIgnoreCase );

      internal static int GetSetting(string settingName)
      {
        var settingValue = 0;
        if (settings.ContainsKey(settingName))
        {
          settingValue = settings[settingName];
        }
        else
        {
          settingValue = int.Parse(System.Configuration.ConfigurationManager.AppSettings[settingName]);
          settings.Add(settingName, settingValue);
        }
        return settingValue;
      }
      
      public static string CommissionTypeIdName { get { return "CommissionRateTypeID"; } }
      public static string LiabilityTypeIdName { get { return "LiabilityInsuranceRateTypeID"; } }
      public static string CargoTypeIdName { get { return "CargoInsuranceRateTypeID"; } }

      public static int CommissionTypeId
      {
        get
        {
          return GetSetting(UsedDeductions.CommissionTypeIdName);
        }
      }
      public static int LiabilityTypeId
      {
        get
        {
          return GetSetting("LiabilityInsuranceRateTypeID");
        }
      }
      public static int CargoTypeId
      {
        get
        {
          return GetSetting("CargoInsuranceRateTypeID");
        }
      }
    }

    public ChangeCommissionLogicModel GetCommissionLogicModel()
    {
      var model = new ChangeCommissionLogicModel();

      var dataSet = new DataSet();
      using (var cn = new SqlConnection(this.ConnectionString))
      using (var cmd = new SqlCommand(Queries.LessorRatesAll, cn))
      using (var adapter = new SqlDataAdapter(cmd))
      {
        cmd.Parameters.AddWithValue("@CommissionRateTypeID", UsedDeductions.CommissionTypeId);
        adapter.Fill(dataSet);
      }
      var commmissionRates = dataSet.Tables[1].AsEnumerable()
          .Select(r => r.Field<decimal>("Rate")).ToList();
      var lessorData = dataSet.Tables[0].AsEnumerable()
        .Select(r => new
        {
          LessorID = r.Field<string>("Lessor_ID"),
          TripNumber = r.Field<string>("Trip_Number"),
          RateTypeId = r.Field<int>("Rate_Type_ID"),
          Rate = r.Field<decimal>("Rate")
        }).ToList();
      var grouped = lessorData.GroupBy(p => new { p.LessorID, p.TripNumber }).ToList();
      var commissions = new List<ChangeCommission>();
      var lessorId = string.Empty;
      var trip = string.Empty;
      var lessorChangeCommissions = new Dictionary<string, Dictionary<string, ChangeCommission>>();
      foreach (var itm in lessorData)
      {
        if (!lessorChangeCommissions.ContainsKey(itm.LessorID))
        {
          lessorChangeCommissions.Add(itm.LessorID, new Dictionary<string, ChangeCommission>());
        }

        if (!lessorChangeCommissions[itm.LessorID].ContainsKey(itm.TripNumber))
        {
          lessorChangeCommissions[itm.LessorID][itm.TripNumber] = new ChangeCommission
              {
                LessorId = itm.LessorID,
                TripNumber = itm.TripNumber
              };
        }
        var changeCommission = lessorChangeCommissions[itm.LessorID][itm.TripNumber];

        if (itm.RateTypeId == UsedDeductions.CommissionTypeId)
        {
          changeCommission.Commission = itm.Rate;
        }
        else if (itm.RateTypeId == UsedDeductions.LiabilityTypeId)
        {
          changeCommission.LiabilityInsurance = itm.Rate;
        }
        else if (itm.RateTypeId == UsedDeductions.CargoTypeId)
        {
          changeCommission.CargoInsurance = itm.Rate;
        }
        else
        {
          throw new InvalidOperationException("Cannot convert to Commission, Liability or Cargo");
        }
      }

      model.CommissionRates = commmissionRates;
      model.Deductions = lessorChangeCommissions.Values.SelectMany(p => p.Values).ToList();
      return model;
    }

    internal LessorDefaults GetLessorDefaults(string lessorId)
    {
      var dataSet = new DataSet();
      using (var cn = new SqlConnection(this.ConnectionString))
      using (var cmd = new SqlCommand(Queries.LessorRatesSpecific, cn))
      using (var adapter = new SqlDataAdapter(cmd))
      {
        cmd.Parameters.AddWithValue("@LessorID", lessorId);
        adapter.Fill(dataSet);
      }
      var lessorRatesTable = dataSet.Tables[0];
      var rateTypesTable = dataSet.Tables[1];
      Dictionary<int, IEnumerable<decimal>> rates = this.GetDisplayRates(rateTypesTable);
      LessorDefaults lessorDefaults = this.GetLessorRates(lessorRatesTable);

      lessorDefaults.LessorId = lessorId;
      lessorDefaults.CommissionRates = rates[UsedDeductions.CommissionTypeId].ToList();
      lessorDefaults.LiabilityRates = rates[UsedDeductions.LiabilityTypeId].ToList();
      lessorDefaults.CargoRates = rates[UsedDeductions.CargoTypeId].ToList();
      return lessorDefaults;
    }

    internal void Save(LessorDefaults lessorDefaults)
    {
      using (var cn = new SqlConnection(this.ConnectionString))
      using (var cmd = new SqlCommand(Queries.InsertLessorDefaults, cn))
      {
        cmd.Parameters.AddWithValue("@Commission", lessorDefaults.Commission);
        cmd.Parameters.AddWithValue("@CommissionTypeID", UsedDeductions.CommissionTypeId);
        cmd.Parameters.AddWithValue("@Liability", lessorDefaults.Liability);
        cmd.Parameters.AddWithValue("@LiabilityTypeID", UsedDeductions.LiabilityTypeId);
        cmd.Parameters.AddWithValue("@Cargo", lessorDefaults.Cargo);
        cmd.Parameters.AddWithValue("@CargoTypeID", UsedDeductions.CargoTypeId);
        cmd.Parameters.AddWithValue("@LessorID", lessorDefaults.LessorId);
        cn.Open();
        cmd.ExecuteNonQuery();
      }
    }

    internal void Save(DefaultTypeAndValue aDefaults)
    {
      var rateTypeId = 0;
      if (aDefaults.DefaultType.Equals("Commission", StringComparison.InvariantCultureIgnoreCase))
      { rateTypeId = UsedDeductions.GetSetting( UsedDeductions.CommissionTypeIdName ); }
      else if (aDefaults.DefaultType.Equals("Liability", StringComparison.InvariantCultureIgnoreCase))
      { rateTypeId = UsedDeductions.GetSetting( UsedDeductions.LiabilityTypeIdName ); }
      else if (aDefaults.DefaultType.Equals("Cargo", StringComparison.InvariantCultureIgnoreCase))
      { rateTypeId = UsedDeductions.GetSetting( UsedDeductions.CargoTypeIdName ); }
      using (var cn = new SqlConnection(this.ConnectionString))
      using (var cmd = new SqlCommand(Queries.InsertRateDefault, cn))
      {
        
        cmd.Parameters.AddWithValue("@RateTypeID", rateTypeId);
        cmd.Parameters.AddWithValue("@Rate", aDefaults.DefaultValue);
        cn.Open();
        cmd.ExecuteNonQuery();
      }
    }

    private LessorDefaults GetLessorRates(DataTable lessorRatesTable)
    {
      var lessorRatesEnumerable = lessorRatesTable.AsEnumerable();
      var commissionRate = lessorRatesEnumerable.Where(r => r.Field<int>("Rate_Type_ID").Equals(UsedDeductions.CommissionTypeId)).First().Field<decimal>("Rate");
      var liabilityRate = lessorRatesEnumerable.Where(r => r.Field<int>("Rate_Type_ID").Equals(UsedDeductions.LiabilityTypeId)).First().Field<decimal>("Rate");
      var cargoRate = lessorRatesEnumerable.Where(r => r.Field<int>("Rate_Type_ID").Equals(UsedDeductions.CargoTypeId)).First().Field<decimal>("Rate");
      return new LessorDefaults
      {
        Commission = commissionRate,
        Liability = liabilityRate,
        Cargo = cargoRate
      };
    }


    private Dictionary<int, IEnumerable<decimal>> GetDisplayRates(DataTable rateTypesTable)
    {
      var enumerableRatesTable = rateTypesTable.AsEnumerable();
      var commisionRates = enumerableRatesTable.Where(r => r.Field<int>("Rate_Type_ID").Equals(UsedDeductions.CommissionTypeId))
                          .Select(r => r.Field<decimal>("Rate")).ToList();
      var liabilityRates = enumerableRatesTable.Where(r => r.Field<int>("Rate_Type_ID").Equals(UsedDeductions.LiabilityTypeId))
                          .Select(r => r.Field<decimal>("Rate")).ToList();

      var cargoRates = enumerableRatesTable.Where(r => r.Field<int>("Rate_Type_ID").Equals(UsedDeductions.CargoTypeId))
                .Select(r => r.Field<decimal>("Rate")).ToList();

      return new Dictionary<int, IEnumerable<decimal>>
      {
        { UsedDeductions.CommissionTypeId,  commisionRates },
        { UsedDeductions.LiabilityTypeId, liabilityRates },
        { UsedDeductions.CargoTypeId, cargoRates }
      };
    }

    internal void SaveCommissions(IEnumerable<ChangeCommission> automaticDeductions)
    {
      var deductionsXml = GetDeductionsXml(automaticDeductions);
      using (var cn = new SqlConnection(this.ConnectionString))
      using (var cmd = new SqlCommand(Queries.InsertDeductions, cn))
      {
        cmd.Parameters.AddWithValue("@DeductionsXml", deductionsXml.ToString());
        cn.Open();
        cmd.ExecuteNonQuery();
      }
    }

    private XDocument GetDeductionsXml(IEnumerable<ChangeCommission> automaticDeductions)
    {
      var deductionsXml = new XDocument(new XElement("deductions",
        automaticDeductions.GroupBy(l => l.LessorId)
        .Select(l => new XElement("lessor",
          new XAttribute("id", l.Key),
          l.Select(d =>
            new XElement("trip",
              new XAttribute("id", d.TripNumber),
            new XElement("rate",
             new XAttribute("rateTypeId", UsedDeductions.CommissionTypeId),
             new XAttribute("default", d.ChangeCommissionDefault), d.Commission),
            new XElement("rate",
              new XAttribute("rateTypeId", UsedDeductions.CargoTypeId), d.CargoInsurance),
            new XElement("rate",
              new XAttribute("rateTypeId", UsedDeductions.LiabilityTypeId), d.LiabilityInsurance)))))));
      return deductionsXml;
    }



    private string ConnectionString
    {
      get
      {
        var conStr = System.Configuration.ConfigurationManager.ConnectionStrings["AccessReplacementConnectionString"].ConnectionString;
        return conStr;
      }
    }

    protected static class Queries
    {
      public static string LessorRatesCommon
      {
        get
        {
          return @"

            ;WITH [Lessor_Rates] AS
            (
	            SELECT [L].[Lessor_ID]
	                  ,[T].[ID] As [Rate_Type_ID]
		              ,[T].[Name] AS [Rate_Name]
		              ,[R].[Rate]
		              ,[T].[Gl_Department]
		              ,[T].[Gl_Account]
	            FROM [dbo].[Lessor_Deduction_Rate] [L]
	             INNER JOIN [dbo].[Automatic_Deduction_Rate] [R]
	               ON [L].[Deduction_Rate_ID] = [R].[ID]
	             INNER JOIN [dbo].[Automatic_Deduction_Type] [T]
	               ON [R].[Type_ID] = [T].[ID]
            )

          ";
        }
      }
      public static string LessorRatesSpecific
      {
        get
        {
          return string.Concat(Queries.LessorRatesCommon,
            @"
            ,[RateCounts] AS
			      (
				      SELECT [R].[Type_ID]
					       , [R].[Rate]
					       , COUNT(*) AS RateCount
				      FROM dbo.Lessor_Deduction_Rate [L]
				        INNER JOIN [dbo].[Automatic_Deduction_Rate] [R]
				         ON [R].[ID] = [L].[Deduction_Rate_ID]
				      GROUP BY [R].[Type_ID], [R].[Rate]
			      ), [MostCommon] AS
			      (
			         SELECT [RC].[Type_ID]
					       ,MAX( [RC].[RateCount] ) AS [MaxCount]
			         FROM [RateCounts] [RC]
			         GROUP BY [RC].[Type_ID]
					 
			      )			

                  SELECT [R].[Rate_Type_ID]
                        ,[R].[Rate_Name]
                        ,[R].[Rate]
                  FROM [Lessor_Rates] [R]
                  WHERE [R].[Lessor_ID] = @LessorID
			
			      UNION
			
			      SELECT [RC].[Type_ID] AS [Rate_Type_ID]
			           , [T].[Name] AS [Rate_Name]
				       , [RC].[Rate] 
			      FROM [RateCounts] [RC]
			        INNER JOIN [Automatic_Deduction_Type] [T]
			          ON [RC].[Type_ID] = [T].[ID]
			      WHERE EXISTS
			      (
			        SELECT * 
			        FROM [MostCommon] [MC]
			        WHERE [RC].[RateCount] = [MC].[MaxCount]
			      ) 
			      AND NOT EXISTS
			      (
			        SELECT * 
			        FROM [Lessor_Rates] [R]
			        WHERE [R].[Rate_Type_ID] = [RC].[Type_ID]
			          AND [R].[Lessor_ID] = @LessorID
			      )


					  SELECT [R].[Type_ID] AS [Rate_Type_ID]
						   , [T].[Name] AS [Rate_Type_Name]
						   , [R].[Rate]
					  FROM [dbo].[Automatic_Deduction_Rate] [R]
					    INNER JOIN [dbo].[Automatic_Deduction_Type] [T]
						  ON [R].[Type_ID] = [T].[ID]

            ");
        }
      }
      public static string LessorRatesAll
      {
        get
        {
          return string.Concat(Queries.LessorRatesCommon, @"
            , [Trips] AS
            (
	            SELECT [E].[cTripNumber] AS [Trip_Number]
		             , [E].[cLessorID] AS [Lessor_ID]
	            FROM [dbo].[RsEntry] [E] 
	              LEFT JOIN [dbo].[RsDeduction] [D]
		            ON  ( [E].[cTripNumber] = [D].[cTripNumber]  )
		            AND ( [E].[cLessorID] = [D].[cLessorID] )
	            WHERE ( [E].[cTripNumber] IS NOT NULL ) 
	              AND ( [E].[cLessorID] IS NOT NULL )
	              AND ( [E].[cTripNumber] LIKE 'A%' )
	              AND ( [E].[cLessorID] <> '' )
	              AND ( [D].[cTripNumber] IS NULL )
	              AND ( [D].[cLessorID] IS NULL )
            )

            SELECT [T].[Lessor_ID]
                  ,[T].[Trip_Number]
                  ,[R].[Rate_Type_ID]
                  ,[R].[Rate_Name]
                  ,[R].[Rate]
            FROM [Lessor_Rates] [R]
              INNER JOIN [Trips] [T]
                ON [R].[Lessor_ID] = [T].[Lessor_ID]
        
            SELECT [R].[Rate]
            FROM [dbo].[Automatic_Deduction_Rate] [R]
            WHERE [R].[Type_ID] = @CommissionRateTypeID
          ");
        }
      }
      public static string InsertDeductions
      {
        get
        {
          return @"
            DECLARE @xml XML
            SET @xml = CAST( @DeductionsXml AS XML )
            ;WITH [Deductions] AS
            (
              SELECT  T.c.value('../../@id', 'varchar(12)') AS [Lessor_ID]
                    , T.c.value('../@id', 'varchar(12)') AS [Trip_Number]
                    , T.c.value('./@rateTypeId', 'varchar(20)') AS [Deduction_Type_ID]
                    , T.c.value('.', 'decimal(5,2)') AS [Deduction_Rate]
              FROM @xml.nodes('//lessor/trip/rate') T(c)
            ), [Trips] AS
            (
	            SELECT  [E].[cTripNumber] AS [Trip_Number]
		              , [E].[cLessorID] AS [Lessor_ID]
                      , [E].[cuSettlement] AS [Revenue]
	            FROM [dbo].[RsEntry] [E] 
	              LEFT JOIN [dbo].[RsDeduction] [D]
		            ON  ( [E].[cTripNumber] = [D].[cTripNumber]  )
		            AND ( [E].[cLessorID] = [D].[cLessorID] )
	            WHERE ( [E].[cTripNumber] IS NOT NULL ) 
	              AND ( [E].[cLessorID] IS NOT NULL )
	              AND ( [E].[cTripNumber] LIKE 'A%' )
	              AND ( [E].[cLessorID] <> '' )
	              AND ( [D].[cTripNumber] IS NULL )
	              AND ( [D].[cLessorID] IS NULL )
            )


            INSERT INTO [dbo].[RsDeduction]
                       ([cLessorId]
                       ,[cTripnumber]
                       ,[cDescription]
                       ,[b1099]
                       ,[cuUnit]
                       ,[cuRate]
                       ,[cuAmount]
                       ,[cGlDept]
                       ,[cGlAcct])

            SELECT [D].[Lessor_ID] AS [cLessorId]
                 , [D].[Trip_Number] AS [cTripNumber]
                 , [RT].[Name] AS [cDescription]
                 , 0 AS [b1099]
                 , [D].[Deduction_Rate] AS [cuUnit]
                 , [T].[Revenue] AS [cuRate]
                 , CAST( ROUND([D].[Deduction_Rate] * [T].[Revenue], 2) AS DECIMAL(5,2) ) AS [cuAmount]
                 , [RT].[Gl_Department] AS [cGlDept]
                 , [RT].[Gl_Account] AS [cGlAcct]
              FROM [Deductions] [D]
              INNER JOIN [dbo].[Automatic_Deduction_Type] [RT]
                ON [D].[Deduction_Type_ID] = [RT].[ID]
              INNER JOIN [Trips] [T] 
                ON [T].[Trip_Number] = [D].[Trip_Number]
            WHERE [D].[Deduction_Rate] > 0  

            ORDER BY [D].[Lessor_ID]
                   , [D].[Trip_Number]


            /*************************************************************/
            /* Update Defaults */
            /*************************************************************/
            ;WITH [Deductions] AS
            (
              SELECT  T.c.value('../../@id', 'varchar(12)') AS [Lessor_ID]
                    , T.c.value('../@id', 'varchar(12)') AS [Trip_Number]
                    , T.c.value('./@rateTypeId', 'varchar(20)') AS [Deduction_Type_ID]
                    , T.c.value('.', 'decimal(5,2)') AS [Rate]
                    , COALESCE( T.c.value('./@default', 'bit'), 0 ) AS [SetDefault]
              FROM @xml.nodes('//lessor/trip/rate') T(c)
            ), [DefaultsToUpdate] AS
            (
              SELECT * FROM [Deductions] WHERE [SetDefault] = 1
            ), [AffectedRates] AS
            (
              SELECT [DU].[Lessor_ID]

                   , [ADR].[ID] AS [New_Rate_ID]
                   , [ADR].[Type_ID]
              FROM [dbo].[Automatic_Deduction_Rate] [ADR]
              INNER JOIN [DefaultsToUpdate] [DU]
                ON ( [DU].[Deduction_Type_ID] = [ADR].[Type_ID] )
                AND ( [DU].[Rate] = [ADR].[Rate] )
            ), [Changes] AS
            (
              SELECT [OLD].[Lessor_ID]
                   , [OLD].[Deduction_Rate_ID] AS [Old_Deduction_Rate_ID]
                   , [NEW].[New_Rate_ID]
              FROM [dbo].[Lessor_Deduction_Rate] [OLD]
                INNER JOIN [dbo].[Automatic_Deduction_Rate] [ADR]
                  ON ( [ADR].[ID] = [OLD].[Deduction_Rate_ID] )
                INNER JOIN [AffectedRates] [NEW]
                  ON  ( [NEW].[Lessor_ID] = [OLD].[Lessor_ID] )
                  AND ( [NEW].[Type_ID] = [ADR].[Type_ID] )
             )

            UPDATE [dbo].[Lessor_Deduction_Rate] 
              SET [Deduction_Rate_ID] = [C].[New_Rate_ID]
            FROM [dbo].[Lessor_Deduction_Rate] [R]
             INNER JOIN [Changes] [C]
               ON  ( [C].[Lessor_ID] = [R].[Lessor_ID] )
               AND ( [C].[Old_Deduction_Rate_ID] = [R].[Deduction_Rate_ID] )

          ";
        }
      }

      public static string InsertLessorDefaults
      {
        get
        {
          return @"
            BEGIN TRANSACTION;

            BEGIN TRY
  
              DELETE 
              FROM [dbo].[Lessor_Deduction_Rate] 
              WHERE [Lessor_ID] = @LessorID
  
              ;WITH [NewRates] AS 
              (
                  SELECT [ID], [Type_ID], [Rate] 
                  FROM [dbo].[Automatic_Deduction_Rate] [R]
                  WHERE ( [TYPE_ID] = @CommissionTypeID )
                    AND ( [Rate]    = @Commission )
                  UNION
                  SELECT [ID], [Type_ID], [Rate] 
                  FROM [dbo].[Automatic_Deduction_Rate] [R]
                  WHERE ( [TYPE_ID] = @LiabilityTypeID )
                    AND ( [Rate]    = @Liability )
                  UNION
                  SELECT [ID], [Type_ID], [Rate] 
                  FROM [dbo].[Automatic_Deduction_Rate] [R]
                  WHERE ( [TYPE_ID] = @CargoTypeID )
                    AND ( [Rate]    = @Cargo )
              )
  
              INSERT INTO [dbo].[Lessor_Deduction_Rate] ( [Lessor_ID], [Deduction_Rate_ID] )
              SELECT @LessorID, [ID] AS [Deduction_Rate_ID]
              FROM [NewRates] 
  
            END TRY
            BEGIN CATCH
              IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            END CATCH;

            IF @@TRANCOUNT > 0
              COMMIT TRANSACTION;

          ";
        }
      }

      public static string InsertRateDefault 
      {
        get
        {
          return @"
            ;WITH [NewRates] AS
            (
              SELECT @RateTypeID AS [Type_ID], @Rate AS [Rate]
            )

            INSERT INTO [dbo].[Automatic_Deduction_Rate] ( [Type_ID], [Rate] )
            SELECT [Type_ID], [Rate]
            FROM [NewRates]
            WHERE NOT EXISTS
            (
              SELECT * FROM [dbo].[Automatic_Deduction_Rate] 
              WHERE ( [TYPE_ID] = @RateTypeID  )
                AND ( [Rate]    = @Rate )
            )
          ";
        }
      }
    }
  }
}

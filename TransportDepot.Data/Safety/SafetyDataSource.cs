using TransportDepot.Models;
using System.Collections.Generic;
using TransportDepot.Models.Safety;
using System;
using System.Xml.Linq;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using TransportDepot.Data.DB;
using TransportDepot.Models.Business;
using TransportDepot.Models.Reports;

namespace TransportDepot.Data.Safety
{
  public class SafetyDataSource : IDataSource
  {
    private SafetyEntityFactory _factory = new SafetyEntityFactory();
    private Utilities _utilities = new Utilities();
    private VehicleSafetyUtils _vehicleSafetyUtils = new VehicleSafetyUtils();
    public string ConnectionString
    {
      get
      {
        var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AccessReplacementConnectionString"].ConnectionString;
        return connectionString;
      }
    }

    public IEnumerable<Driver> GetDrivers(bool activeOnly)
    {
      var driversTable = new DataTable();
      using (var cn = new SqlConnection(this.ConnectionString))
      {
        var driverIdsXml = this.GetAllDriverIdsXml(cn);
        var cmd = new SqlCommand(Queries.DriverSafety, cn);
        var drivers = this.GetDrivers(cn, driverIdsXml.ToString());
        return drivers;  
      }
    }

    public IEnumerable<Driver> GetDrivers(IEnumerable<string> driverIds)
    {
      using (var cn = new SqlConnection(this.ConnectionString))
      {
        var driverIdsXml = this.GetDriverIdsXml(driverIds);
        var drivers = this.GetDrivers(cn, driverIdsXml.ToString());
        return drivers;
      }
    }
    
    public void UpdateSafetyDrivers(IEnumerable<Driver> safetyDrivers)
    {

      var xml = new XDocument(
        new XElement("safetyDrivers",
          safetyDrivers.Select(d => 
            new XElement("driver",
              new XAttribute("id", d.Id),
              new XAttribute("annualCertification", d.AnnualCertificationOfViolations.ToShortDateString()),
              new XAttribute("active", this.BoolToInt(d.Active)),
              new XAttribute("trackDriver", this.BoolToInt(d.TrackDriver)),
              new XAttribute("application", this.BoolToInt(d.Application)),
              new XAttribute("onDutyHours", this.BoolToInt(d.OnDutyHours)),
              new XAttribute("drugTest", this.BoolToInt(d.DrugTest)),
              new XAttribute("mVRExpiration", d.MVRExpiration.ToShortDateString()),
              new XAttribute("policeReport", this.BoolToInt(d.PoliceReport)),
              new XAttribute("previousEmployerForm", this.BoolToInt(d.PreviousEmployerForm)),
              new XAttribute("socialSecurity", this.BoolToInt(d.SocialSecurity)),
              new XAttribute("agreement", this.BoolToInt(d.Agreement)),
              new XAttribute("w9", this.BoolToInt(d.W9)),
              new XAttribute("physicalExamExpiration", d.PhysicalExamExpiration.ToShortDateString()),
              new XAttribute("lastValidLogDate", d.LastValidLogDate.ToShortDateString()),
              new XAttribute("driversLicenseExpiration", d.DriversLicenseExpiration.ToShortDateString()),
              new XAttribute("comments", d.Comments ?? string.Empty)))));
      
      using(var cn = new SqlConnection(this.ConnectionString))
      using (var cmd = new SqlCommand(Queries.UpdateDriverSafety, cn))
      {
        cmd.Parameters.AddWithValue("@DriverSafetyXmlString", xml.ToString());
        this.SaveTrackedDrivers(safetyDrivers.Select(d => d.Id), cn);
        if (cn.State == ConnectionState.Closed)
        {
          cn.Open();
        }
        cmd.ExecuteNonQuery();
      }
    }

    public void TrackDrivers(IEnumerable<string> ids)
    {
      using (var cn = new SqlConnection(this.ConnectionString))
      {
        this.SaveTrackedDrivers(ids, cn);
      }
    }

    public void UntrackDrivers(string[] ids)
    {
      var xml = this.GetDriverIdsXml(ids);
      using(var cn = new SqlConnection(this.ConnectionString))
      using (var cmd = new SqlCommand(Queries.DriversUntrack, cn))
      {
        cmd.Parameters.AddWithValue("@UnTrackedDriversXmlString", xml.ToString());
        if (cn.State == System.Data.ConnectionState.Closed)
        {
          cn.Open();
        }
        cmd.ExecuteNonQuery();
      }
    }

    public void SaveTrackedDrivers(IEnumerable<string> ids, SqlConnection cn)
    {
      var xml = this.GetDriverIdsXml(ids);

      using (var cmd = new SqlCommand(Queries.ConfirmDriverExistence, cn))
      {
        cmd.Parameters.AddWithValue("@TrackedDriversXmlString", xml.ToString());
        if (cn.State == System.Data.ConnectionState.Closed)
        {
          cn.Open();
        }
        cmd.ExecuteNonQuery();
      }
    }

    public IEnumerable<Tractor> GetTractors(bool activeOnly)
    {
      var generalDataSource = new DBDataSource();
      var tractors = generalDataSource.GetTractors();
      if (activeOnly)
      {
        tractors = tractors.Where(t => t.Active);
      }
      var safetyTractors = tractors.Select(t => this._factory.MakeTractor(t));
      return safetyTractors;
    }

    public void UpdateTractor(Tractor tractor)
    {
      var generalDataSource = new DBDataSource();
      var existingTractors = generalDataSource.GetTractors()
          .Where(t=>t.Id.Equals(tractor.Id, StringComparison.OrdinalIgnoreCase));
      if (existingTractors.Count().Equals(0))
      {
        throw new InvalidOperationException(string.Format("Invalid Truck ID: {0}", tractor.Id));
      }

      var tractorToUpdate = existingTractors.First();
      tractorToUpdate.Comments = tractor.Comments;
      tractorToUpdate.InspectionDue = tractor.InspectionDue;
      tractorToUpdate.InsuranceExpiration = tractor.InsuranceExpiration;
      tractorToUpdate.InsuranceName = tractor.InsuranceName;
      tractorToUpdate.LeaseAgreementDue = tractor.LeaseAgreementDue;
      tractorToUpdate.RegistrationExpiration = tractor.RegistrationExpiration;
      tractorToUpdate.Unit = tractor.Unit;
      tractorToUpdate.HasW9 = tractor.HasW9;
      tractorToUpdate.LessorOwnerName = tractor.LessorOwnerName;
      generalDataSource.UpdateTractor(tractorToUpdate);
    }

    public IEnumerable<Trailer> GetTrailers(bool activeOnly)
    {
      var generalDataSource = new DBDataSource();
      IEnumerable<Models.DB.Trailer> trailers = generalDataSource.GetTrailers();
      if (activeOnly)
      {
        trailers = trailers.Where(t => t.Active);
      }
      IEnumerable<Trailer> safetyTrailers = trailers.Select(t => this._factory.MakeTrailer(t));
      return safetyTrailers;
    }

    public IEnumerable<Models.UI.OptionModel> GetUntrackedDriverOptions()
    {
      var untrackedTable = new DataTable();
      using(var cn = new SqlConnection(this.ConnectionString))
      using(var cmd = new SqlCommand(Queries.DriversUntracked, cn))
      using (var adapter = new SqlDataAdapter(cmd))
      {
        adapter.Fill(untrackedTable);
      }
      if (untrackedTable.Rows.Count.Equals(0))
      {
        return new List<Models.UI.OptionModel>();
      }
      var untrackedDrivers = untrackedTable.AsEnumerable()
        .Select(d => new Models.UI.OptionModel
        {
          Id = d.Field<string>("ID"),
          Name = this.FormatName(d.Field<string>("Last_Name"), d.Field<string>("First_Name"))
        });
      return untrackedDrivers;
    }

    public void UpdateTrailer(Trailer trailer)
    {
      var generalDataSource = new DBDataSource();
      var existingTrailers = generalDataSource.GetTrailers()
          .Where(t => t.Id.Equals(trailer.Id, StringComparison.OrdinalIgnoreCase));
      if (existingTrailers.Count().Equals(0))
      {
        throw new InvalidOperationException(string.Format("Invalid Truck ID: {0}", trailer.Id));
      }

      var trailerToUpdate = existingTrailers.First();
      trailerToUpdate.Comments = trailer.Comments;
      trailerToUpdate.InspectionDue = trailer.InspectionDue;
      trailerToUpdate.RegistrationExpiration = trailer.RegistrationExpiration;
      trailerToUpdate.Unit = trailer.Unit;
      trailerToUpdate.LessorOwnerName = trailer.LessorOwnerName;
      generalDataSource.UpdateTrailer(trailerToUpdate); 
    }

    public bool Append(TrailerMaintenancePerformed maintenance)
    {
      var recordsAffected = 0;
      using (var cn = new SqlConnection(this.ConnectionString))
      using (var cmd = new SqlCommand(Queries.TrailerMaintenanceInsert, cn))
      {
        cmd.Parameters.AddWithValue("@TrailerId", maintenance.TrailerId);
        cmd.Parameters.AddWithValue("@DateDone", maintenance.Date);
        cmd.Parameters.AddWithValue("@Type", maintenance.Type);
        cmd.Parameters.AddWithValue("@Mileage", maintenance.Mileage);
        cmd.Parameters.AddWithValue("@Description", maintenance.Description);

        if (cn.State == ConnectionState.Closed)
        {
          cn.Open();
        }
        recordsAffected = cmd.ExecuteNonQuery();
        return recordsAffected > 0;
      }
    }

    public bool Append(TractorMaintenancePerformed maintenance)
    {
      var recordsAffected = 0;
      using (var cn = new SqlConnection(this.ConnectionString))
      using (var cmd = new SqlCommand(Queries.TractorMaintenanceInsert, cn))
      {
        cmd.Parameters.AddWithValue("@TractorId", maintenance.TractorId);
        cmd.Parameters.AddWithValue("@DateDone", maintenance.Date);
        cmd.Parameters.AddWithValue("@Type", maintenance.Type);
        cmd.Parameters.AddWithValue("@Mileage", maintenance.Mileage);
        cmd.Parameters.AddWithValue("@Description", maintenance.Description);
        
        if (cn.State == ConnectionState.Closed)
        {
          cn.Open();
        }
        recordsAffected = cmd.ExecuteNonQuery();
        return recordsAffected > 0;
      }
    }

    public List<SimpleItem> GetMaintenanceTypes()
    {
      var query = "SELECT * FROM [dbo].[MaintenanceTractorCode]";
      var tractorMaintenanceTypes = new List<SimpleItem>();
      var dataTable = new DataTable();
      using(var cmd = new SqlCommand(query))
      {
        var db = new TransportDepot.Data.DB.DataSource();
        dataTable = db.FetchCommand(cmd);
      }
      tractorMaintenanceTypes = dataTable.AsEnumerable().Select(p=> new SimpleItem {
        Id = p.Field<string>("cCode"), 
        Name = p.Field<string>("cDescription")
      }).ToList();
      return tractorMaintenanceTypes;
    }


    public IEnumerable<TractorMaintenance> GetTractorMaintenance(MaintenanceFilter filter)
    {
      XDocument xDocument = this._vehicleSafetyUtils.GetTractorFilterXml(filter);
      DataTable dataTable = new DataTable();
      using (var cmd = new SqlCommand(TractorQueries.TractorMaintenance))
      {
        cmd.Parameters.AddWithValue("@FilterString", xDocument.ToString());
        var db = new TransportDepot.Data.DB.DataSource();
        dataTable = db.FetchCommand(cmd);
      }
      return dataTable.AsEnumerable().Select(m => new TractorMaintenance
      {
        Tractor = this._factory.MakeTractor(m),
        Date = this._utilities.CoalesceDateTime(m, "PerformedDate"),
        Type = this._utilities.CoalesceString(m, "Type"),
        TypeId = this._utilities.CoalesceString(m, "Type"),
        Mileage = this._utilities.CoalesceInt(m, "Mileage"),
        Description = this._utilities.CoalesceString(m, "Description")
      });
    }

    public IEnumerable<TrailerMaintenance> GetTrailerMaintenance(MaintenanceFilter filter)
    {
      var xDocument = this._vehicleSafetyUtils.GetTrailerFilterXml(filter);
      DataTable dataTable = new DataTable();
      using (var cmd = new SqlCommand(TrailerQueries.TrailerMaintenance))
      {
        cmd.Parameters.AddWithValue("@FilterString", xDocument.ToString());
        var db = new TransportDepot.Data.DB.DataSource();
        dataTable = db.FetchCommand(cmd);
      }

      return dataTable.AsEnumerable().Select(m => new TrailerMaintenance
      {
        Trailer = this._factory.MakeTrailer(m),
        Date = this._utilities.CoalesceDateTime(m, "PerformedDate"),
        Type = this._utilities.CoalesceString(m, "Type"),
        TypeId = this._utilities.CoalesceString(m, "Type"),
        Description = this._utilities.CoalesceString(m, "Description") 
      });
    }
      
    private List<Driver> GetDrivers(SqlConnection cn, string driversXml)
    {
      var driversTable = new DataTable();
      using (var cmd = new SqlCommand(Queries.DriverSafety, cn))
      {
        cmd.Parameters.AddWithValue("@DriverSafetyXmlString", driversXml);
        var adapter = new SqlDataAdapter(cmd);
        adapter.Fill(driversTable);
      }
      var drivers = driversTable.AsEnumerable().Select(d => new Driver
      {
        Id = d.Field<string>("ID"),
        Name = this.FormatName(d.Field<string>("Last_Name") ?? string.Empty, d.Field<string>("First_Name") ?? string.Empty),
        Active = d.Field<bool>("Active"),
        AnnualCertificationOfViolations = d.IsNull("Annual_Violations_Cert_Expiration") ? DateTime.Today : d.Field<DateTime>("Annual_Violations_Cert_Expiration") ,
        TrackDriver = true,
        Application = d.Field<bool>("Has_Application"),
        OnDutyHours = d.Field<bool>("Has_SODH"),
        DrugTest = d.Field<bool>("Has_Drug_Test"),
        MVRExpiration = d.IsNull("MVR_Expiration") ? DateTime.Today : d.Field<DateTime>("MVR_Expiration"),
        PoliceReport = d.Field<bool>("Has_Police_Record_Report"),
        PreviousEmployerForm = d.Field<bool>("Has_Previous_Employer_Form"),
        SocialSecurity = d.Field<bool>("Has_Social_Security_Card"),
        Agreement = d.Field<bool>("Has_Driver_Agreements"),
        W9 = d.Field<bool>("Has_W9"),
        PhysicalExamExpiration = d.IsNull("Physical_Expiration") ? DateTime.Today : d.Field<DateTime>("Physical_Expiration"),
        LastValidLogDate = d.IsNull("Last_Valid_Log_Date") ? DateTime.Today : d.Field<DateTime>("Last_Valid_Log_Date"),
        DriversLicenseExpiration = d.IsNull("Drivers_License_Expiration") ? DateTime.Today : d.Field<DateTime>("Drivers_License_Expiration"),
        Comments = d.IsNull("Comments") ? string.Empty : d.Field<string>("Comments")
      }).ToList();
      return drivers;
    }

    private string FormatName(string last, string first)
    {
      if ((last = last.Trim()).Equals(string.Empty))
      {
        last = "[Empty Last Name]";
      }
      if (((first = first.Trim()).Equals(string.Empty)))
      {
        first = "[Empty First Name]";
      }
      return string.Format("{0}, {1}", last, first);
    }
    
    private XDocument GetAllDriverIdsXml(SqlConnection cn)
    {
      var allDriverIdsQuery = @"
        SELECT [cDriverID] AS [ID] 
        FROM [dbo].[DriverInfo] [D]
      ";
      var tbl = new DataTable();
      using (var cmd = new SqlCommand(allDriverIdsQuery,cn))
      using (var adapter = new SqlDataAdapter(cmd))
      {
        adapter.Fill(tbl);
      }
      var driverIds = tbl.AsEnumerable()
        .Select(d => d.Field<string>("ID")).ToList();
      var xml = this.GetDriverIdsXml(driverIds);
      return xml;

    }

    private XDocument GetDriverIdsXml(List<string> driverIds)
    {
      var xml = new XDocument(new XElement("drivers",
        driverIds.Select(id => new XElement("driver", id))));
      return xml;
    }

    private XDocument GetDriverIdsXml(IEnumerable<string> ids)
    {
      var xml = new XDocument(
        new XElement("trackedDrivers",
          ids.Select(d => new XElement("driver", d))));
      return xml;
    }
    
    private int BoolToInt(bool value)
    {
      return value ? 1 : 0;
    }
    
    private bool IntToBool(int i)
    {
      return i != 0;
    }
    
    private static class Queries
    {
      public static string ConfirmDriverExistence = @"
          DECLARE @TrackedDriversXml XML
          SET @TrackedDriversXml = CAST( @TrackedDriversXmlString AS XML )

          DECLARE @Drivers TABLE( [ID] VARCHAR(12) )

          INSERT INTO @Drivers
          SELECT Drvs.col.value('.', 'varchar(12)') AS [ID]
          FROM @TrackedDriversXml.nodes('//driver') Drvs(col)
          
          INSERT INTO [dbo].[Driver_Trackable] ( [ID] )
          SELECT [ID] FROM @Drivers [D]
          WHERE NOT EXISTS
          (
            SELECT * 
            FROM [dbo].[Driver_Trackable] [T]
            WHERE ( [T].[ID] = [D].[ID] )
          )

          INSERT INTO [dbo].[Driver_Qualification] ( [ID] )
          SELECT [ID] FROM @Drivers [D]
          WHERE NOT EXISTS
          (
            SELECT * 
            FROM [dbo].[Driver_Qualification] [Q]
            WHERE ( [Q].[ID] = [D].[ID] )
          )
      ";

      public static string DriversUntrack = @"
          DECLARE @UnTrackedDriversXml XML
          SET @UnTrackedDriversXml = CAST( @UnTrackedDriversXmlString AS XML )

          ;WITH [DriversToUntrack] AS
          (
            SELECT Drvs.col.value('.', 'varchar(12)') AS [ID]
            FROM @UnTrackedDriversXml.nodes('//driver') Drvs(col)
          )
          
          DELETE 
          FROM [dbo].[Driver_Trackable] 
          WHERE [ID] IN ( SELECT [ID] FROM [DriversToUntrack] )
      ";

      public static string UpdateDriverSafety = @"

        DECLARE @DriverSafetyXml XML
        SET @DriverSafetyXml = CAST( @DriverSafetyXmlString AS XML ) 

        DECLARE @DriverSafety TABLE
        (
           [ID] VARCHAR(12)
          ,[Active] BIT 
          ,[TrackDriver] BIT 
          ,[Has_Application] BIT 
          ,[Has_SODH] BIT 
          ,[Has_Drug_Test] BIT 
          ,[MVR_Expiration] DATETIME 
          ,[Has_Police_Record_Report] BIT 
          ,[Has_Previous_Employer_Form] BIT 
          ,[Has_Social_Security_Card] BIT 
          ,[Has_Driver_Agreements] BIT 
          ,[Has_W9] BIT 
          ,[Physical_Expiration] DATETIME 
          ,[Last_Valid_Log_Date] DATETIME 
          ,[Drivers_License_Expiration] DATETIME 
          ,[Annual_Violations_Cert_Expiration] DATETIME 
          ,[Comments] VARCHAR(MAX)
        )

        INSERT INTO @DriverSafety
        (
           [ID] 
          ,[Active] 
          ,[TrackDriver]  
          ,[Has_Application]  
          ,[Has_SODH]  
          ,[Has_Drug_Test]  
          ,[MVR_Expiration]  
          ,[Has_Police_Record_Report]  
          ,[Has_Previous_Employer_Form]  
          ,[Has_Social_Security_Card]  
          ,[Has_Driver_Agreements]  
          ,[Has_W9]  
          ,[Physical_Expiration]  
          ,[Last_Valid_Log_Date]  
          ,[Drivers_License_Expiration]  
          ,[Annual_Violations_Cert_Expiration]  
          ,[Comments] 
        )
        SELECT
              SD.col.value( './@id', 'VARCHAR(12)' ) AS [ID] 
            , SD.col.value( './@active', 'BIT' ) AS [Active]
            , SD.col.value( './@trackDriver', 'BIT' ) AS [TrackDriver]
            , SD.col.value( './@application', 'BIT' ) AS [Has_Application]
            , SD.col.value( './@onDutyHours', 'BIT' ) AS [Has_SODH]
            , SD.col.value( './@drugTest', 'BIT' ) AS [Has_Drug_Test]
            , SD.col.value( './@mVRExpiration', 'DATETIME' ) AS [MVR_Expiration]
            , SD.col.value( './@policeReport', 'BIT' ) AS [Has_Police_Record_Report]
            , SD.col.value( './@previousEmployerForm', 'BIT' ) AS [Has_Previous_Employer_Form]
            , SD.col.value( './@socialSecurity', 'BIT' ) AS [Has_Social_Security_Card]
            , SD.col.value( './@agreement', 'BIT' ) AS [Has_Driver_Agreements]
            , SD.col.value( './@w9', 'BIT' ) AS  [Has_W9]
            , SD.col.value( './@physicalExamExpiration', 'DATETIME' ) AS [Physical_Expiration]
            , SD.col.value( './@lastValidLogDate', 'DATETIME' ) AS [Last_Valid_Log_Date]
            , SD.col.value( './@driversLicenseExpiration', 'DATETIME' ) AS [Drivers_License_Expiration]
            , SD.col.value( './@annualCertification', 'DATETIME' ) AS [Annual_Violations_Cert_Expiration]
            , SD.col.value( './@comments', 'VARCHAR(MAX)' ) AS [Comments]
        FROM @DriverSafetyXml.nodes('//driver') AS SD(col)

        UPDATE [dbo].[DriverInfo]
          SET [dLicenseExpDate] = [S].[Drivers_License_Expiration]
            , [dPhysicalExpDate] = [S].[Physical_Expiration]
        FROM [dbo].[DriverInfo] [D]
          INNER JOIN @DriverSafety [S]
            ON ( [S].[ID] = [D].[cDriverID] )

        UPDATE [dbo].[PrEmployee]
          SET [bActive] = [S].[Active]
        FROM [dbo].[PrEmployee] [E]
          INNER JOIN @DriverSafety [S]
            ON ( [S].[ID] = [E].[cEmployeeId] )

        UPDATE [dbo].[Driver_Qualification]
            SET [Has_Application] = [S].[Has_Application]
              ,[Has_SODH] = [S].[Has_SODH]
              ,[Has_Drug_Test] = [S].[Has_Drug_Test]
              ,[Has_Police_Record_Report] = [S].[Has_Police_Record_Report] 
              ,[Has_Previous_Employer_Form] = [S].[Has_Previous_Employer_Form]
              ,[Has_Social_Security_Card] = [S].[Has_Social_Security_Card]
              ,[Has_Driver_Agreements] = [S].[Has_Driver_Agreements]
              ,[Has_W9] = [S].[Has_W9] 
              ,[MVR_Expiration] = [S].[MVR_Expiration]
              ,[Annual_Violations_Cert_Expiration] = [S].[Annual_Violations_Cert_Expiration]
              ,[Last_Valid_Log_Date] = [S].[Last_Valid_Log_Date]
              ,[Comments] = [S].[Comments]
        FROM [dbo].[Driver_Qualification] [Q]
          INNER JOIN @DriverSafety [S]
            ON ( [S].[ID] = [Q].[ID] )
      ";

      public static string DriversUntracked = @"
        SELECT   [E].[cEmployeeID] AS [ID]
                ,[E].[cLast] AS [Last_Name] 
                ,[E].[cFirst] AS [First_Name]
        FROM [dbo].[PrEmployee] [E]
        WHERE NOT EXISTS
        (
          SELECT * 
          FROM [dbo].[Driver_Trackable] [T]
          WHERE ( [T].[ID] = [E].[cEmployeeID] )
        ) AND ( COALESCE( [E].[bIncludeInDriverList], 0 ) != 0 )
          AND ( COALESCE( [E].[bActive], 0 ) != 0 )
        ";

      public static string DriverSafety = @"
        DECLARE @DriverSafetyXml XML
        SET @DriverSafetyXml = CAST( @DriverSafetyXmlString AS XML )
        
        ;WITH [Drivers] AS
        (
          SELECT Drvs.col.value('.', 'varchar(12)') AS [ID]
          FROM @DriverSafetyXml.nodes('//driver') Drvs(col)
        )
        
        SELECT   [Q].[ID]
                ,[E].[cLast] AS [Last_Name] 
                ,[E].[cFirst] AS [First_Name]
                ,[Q].[Has_Application]
                ,[Q].[Has_SODH]
                ,[Q].[Has_Drug_Test]
                ,[Q].[Has_Police_Record_Report]
                ,[Q].[Has_Previous_Employer_Form]
                ,[Q].[Has_Social_Security_Card]
                ,[Q].[Has_Driver_Agreements]
                ,[Q].[Has_W9]
                ,[Q].[MVR_Expiration]
                ,[Q].[Annual_Violations_Cert_Expiration]
                ,[Q].[Last_Valid_Log_Date]
                ,[Q].[Comments]
                ,[D].[dLicenseExpDate] AS [Drivers_License_Expiration]
                ,[D].[dPhysicalExpDate] AS [Physical_Expiration]
                ,[E].[bActive] AS [Active]
         
        FROM [dbo].[Driver_Qualification] [Q]
          INNER JOIN [dbo].[Driver_Trackable] [T]
            ON ( [Q].[ID] = [T].[ID] )
          INNER JOIN [dbo].[DriverInfo] [D]
            ON ( [Q].[ID] = [D].[cDriverID] )
          INNER JOIN [dbo].[PrEmployee] [E]
            ON ( [Q].[ID] = [E].[cEmployeeId] )
          INNER JOIN [Drivers] [SD]
            ON ( [Q].[ID] = [SD].[ID] )
      ";
      public static string TractorMaintenanceInsert = @"
      INSERT INTO [dbo].[TractorMaintenance] ( [TractorId], [Type], [PerformedDate], [Mileage], [Description]) 
                                      VALUES ( @TractorId, @Type, @DateDone, @Mileage, @Description )
    ";

      public static string TrailerMaintenanceInsert = @"
      INSERT INTO [dbo].[TrailerMaintenance] ( [TrailerId], [Type], [PerformedDate], [Mileage], [Description]) 
                                      VALUES ( @TrailerId, @Type, @DateDone, @Mileage, @Description )
    ";
    }
  }
}

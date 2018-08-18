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
    private TransportDepot.Data.DB.DataSource _db = new TransportDepot.Data.DB.DataSource();

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
        var cmd = new SqlCommand(SafetyQueries.DriverSafety, cn);
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
      using (var cmd = new SqlCommand(SafetyQueries.UpdateDriverSafety, cn))
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
      using (var cmd = new SqlCommand(SafetyQueries.DriversUntrack, cn))
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

      using (var cmd = new SqlCommand(SafetyQueries.ConfirmDriverExistence, cn))
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
      var trailerIds = trailers.Select(t => t.Id);
      var safetyTrailerDataRows = this.GetSafetyTrailers(trailerIds);
      
      var filter = new MaintenanceFilter
      {
        From = Utilities.DBMinDate,
        To = Utilities.DBMaxDate,
        VehicleIds = trailerIds
      };
      var trailerMaintenance = this.GetTrailerMaintenance(filter).GroupBy(t => t.Trailer.Id.ToUpper());
      var maxMaint = trailerMaintenance.ToDictionary(t => t.Key, t => t.Max(l => l.Date));

      var safetyTrailers = trailers.Select(t => 
      { 
        var trailer = this._factory.MakeTrailer(safetyTrailerDataRows[t.Id.ToUpper()], t);
        if (maxMaint.ContainsKey(trailer.Id))
        {
          trailer.LastMaintenance = maxMaint[trailer.Id];
          this._utilities.WriteAppend("Assigned to " + trailer.Id + " " + maxMaint[trailer.Id].ToShortDateString());
          this._utilities.WriteAppend("In fact, look: " + trailer.Id + "  Maint: " + trailer.LastMaintenance);
        }
        else
        {
          trailer.LastMaintenance = Utilities.DBMinDate;
          this._utilities.WriteAppend("This was assigned: " + trailer.Id + "  Maint: " + trailer.LastMaintenance);
        }
        return trailer;
      });

      return safetyTrailers;
    }

    public IEnumerable<Models.UI.OptionModel> GetUntrackedDriverOptions()
    {
      var untrackedTable = new DataTable();
      using(var cn = new SqlConnection(this.ConnectionString))
      using (var cmd = new SqlCommand(SafetyQueries.DriversUntracked, cn))
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
          Name = this._utilities.FormatName(d.Field<string>("Last_Name"), d.Field<string>("First_Name"))
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
        throw new InvalidOperationException(string.Format("Invalid Trailer ID: {0}", trailer.Id));
      }
      var trailerIds = new string[] { trailer.Id };
      var safetyTrailerRow = this.GetSafetyTrailers(trailerIds)[trailer.Id.ToUpper()];            
      
      var trailerToUpdate = existingTrailers.First();
      var filter = new MaintenanceFilter
      {
        From = Utilities.DBMinDate,
        To = Utilities.DBMaxDate,
        VehicleIds = new string[] { trailerToUpdate.Id }
      };
      var safetyTrailer = this._factory.MakeTrailer(safetyTrailerRow, trailerToUpdate);

      trailerToUpdate.Unit = trailer.Unit;
      trailerToUpdate.LessorOwnerName = trailer.LessorOwnerName;
      trailerToUpdate.LastMaintenance = this.GetLastTrailerMaintenance(new string[] {trailerToUpdate.Id});
      trailerToUpdate.RegistrationExpiration = trailer.RegistrationExpiration;
      trailerToUpdate.Comments = trailer.Comments;
      trailerToUpdate.InspectionDue = trailer.InspectionDue;
      UpdateTrailer(trailerToUpdate); 
    }

    internal void UpdateTrailer(Models.DB.Trailer trailer)
    {
      using (var cn = new SqlConnection(this.ConnectionString))
      using (var cmd = new SqlCommand(TrailerQueries.UpdateTrailers, cn))
      {
        cmd.Parameters.AddWithValue("@Active", trailer.Active);
        cmd.Parameters.AddWithValue("@Comments", trailer.Comments);
        cmd.Parameters.AddWithValue("@HasTripAssigned", trailer.HasTripAssigned);
        cmd.Parameters.AddWithValue("@InspectionDue", trailer.InspectionDue);
        cmd.Parameters.AddWithValue("@IsLessorTrailer", trailer.IsLessorTrailer);
        cmd.Parameters.AddWithValue("@LessorOwnerName", trailer.LessorOwnerName);
        cmd.Parameters.AddWithValue("@LicensePlate", trailer.LicensePlate);
        cmd.Parameters.AddWithValue("@LastMaintenance", trailer.LastMaintenance);
        cmd.Parameters.AddWithValue("@Make", trailer.Make);
        cmd.Parameters.AddWithValue("@Model", trailer.Model);
        cmd.Parameters.AddWithValue("@RegistrationExpiration", trailer.RegistrationExpiration);
        cmd.Parameters.AddWithValue("@Type", trailer.Type);
        cmd.Parameters.AddWithValue("@Unit", trailer.Unit);
        cmd.Parameters.AddWithValue("@VIN", trailer.VIN);
        cmd.Parameters.AddWithValue("@Year", trailer.Year);
        cmd.Parameters.AddWithValue("@Id", trailer.Id);

        if (cn.State == ConnectionState.Closed)
        {
          cn.Open();
        }
        cmd.ExecuteNonQuery();
      }
    }

    public bool Append(TrailerMaintenancePerformed maintenance)
    {
      var recordsAffected = 0;
      using (var cn = new SqlConnection(this.ConnectionString))
      using (var cmd = new SqlCommand(SafetyQueries.TrailerMaintenanceInsert, cn))
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
      using (var cmd = new SqlCommand(SafetyQueries.TractorMaintenanceInsert, cn))
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
      using (var cmd = new SqlCommand(TransportDepot.Data.DB.TractorQueries.TractorMaintenance))
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
      using (var cmd = new SqlCommand(SafetyQueries.DriverSafety, cn))
      {
        cmd.Parameters.AddWithValue("@DriverSafetyXmlString", driversXml);
        var adapter = new SqlDataAdapter(cmd);
        adapter.Fill(driversTable);
      }
      var drivers = driversTable.AsEnumerable().Select(d => new Driver
      {
        Id = d.Field<string>("ID"),
        Name = this._utilities.FormatName(d.Field<string>("Last_Name") ?? string.Empty, d.Field<string>("First_Name") ?? string.Empty),
        Active = d.Field<bool>("Active"),
        AnnualCertificationOfViolations = this._utilities.CoalesceToMin(d, "Annual_Violations_Cert_Expiration"),
        TrackDriver = true,
        Application = d.Field<bool>("Has_Application"),
        OnDutyHours = d.Field<bool>("Has_SODH"),
        DrugTest = d.Field<bool>("Has_Drug_Test"),
        MVRExpiration = this._utilities.CoalesceToMin(d, "MVR_Expiration"),
        PoliceReport = d.Field<bool>("Has_Police_Record_Report"),
        PreviousEmployerForm = d.Field<bool>("Has_Previous_Employer_Form"),
        SocialSecurity = d.Field<bool>("Has_Social_Security_Card"),
        Agreement = d.Field<bool>("Has_Driver_Agreements"),
        W9 = d.Field<bool>("Has_W9"),
        PhysicalExamExpiration = this._utilities.CoalesceToMin(d, "Physical_Expiration"),
        LastValidLogDate = this._utilities.CoalesceToMin(d, "Last_Valid_Log_Date"),
        DriversLicenseExpiration = this._utilities.CoalesceToMin(d, "Drivers_License_Expiration"),
        Comments = this._utilities.CoalesceString(d, "Comments")
      }).ToList();
      return drivers;
    }

    private DateTime GetLastTrailerMaintenance(string [] trailerIds)
    {
      MaintenanceFilter filter = new MaintenanceFilter 
      { 
        From = Utilities.DBMinDate,
        To = Utilities.DBMaxDate,
        VehicleIds = trailerIds
      };

      var trailerMaintenance = this.GetTrailerMaintenance(filter) ;
      if (trailerMaintenance.Count().Equals(0))
      {
        return Utilities.DBMinDate;
      }

      return trailerMaintenance.Max(f => f.Date);
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
    private Dictionary<string, DataRow> GetSafetyTrailers(IEnumerable<string> ids)
    {
      var dataTable = new DataTable();
      var xml = new XDocument(new XElement("trailers", ids.Select(id => new XElement("trailer", new XAttribute("id", id)))));

      using (var cmd = new SqlCommand(TrailerQueries.TrailerSafety))
      {
        cmd.Parameters.AddWithValue("@TrailerSafetyXmlString", xml.ToString());
        dataTable = this._db.FetchCommand(cmd);
      }
      return dataTable.AsEnumerable().ToDictionary(t => this._utilities.CoalesceString(t, "ID").ToUpper(), t => t);
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
  }
}

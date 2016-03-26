using TransportDepot.Models;
using System.Collections.Generic;
using TransportDepot.Models.Safety;
using System;
using System.Xml.Linq;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using TransportDepot.Data.DB;
using TransportDepot.Data;

namespace TransportDepot.Data.Safety
{
  public class SafetyDataSource : IDataSource
  {
    private Utilities _utilities = new Utilities();
    private SafetyEntityFactory _factory = new SafetyEntityFactory();
    public string ConnectionString
    {
      get
      {
        return this._utilities.ConnectionString;
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
    

  }


}

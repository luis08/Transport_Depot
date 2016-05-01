using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Models.DB;
using System.Data.SqlClient;
using System.Data;
using TransportDepot.Models.Business;
using System.Xml.Linq;

namespace TransportDepot.Data.DB
{
  public class DBDataSource:IDataSource
  {
    private Utilities _utilities = new Utilities();
    DataSource _ds = new DataSource();
    public string ConnectionString
    {
      get
      {
        return this._utilities.ConnectionString;
      }
    }

    public IEnumerable<TractorQualification> GetTractorQualifications()
    {
      DataTable tractorQualTable = new DataTable();
      using (var cmd = new SqlCommand(TractorQualificationQueries.Select))
      {
        tractorQualTable = this._ds.FetchCommand(cmd);
      }
      if (this._utilities.IsEmpty(tractorQualTable))
      {
        return new List<TractorQualification>();
      }
      return tractorQualTable.AsEnumerable()
        .Select(rw => new TractorQualification
        {
          Id = this._utilities.CoalesceString(rw, "Id"),
          HasW9 = this._utilities.CoalesceBool(rw, "HasW9"),
          LeaseAgreementDue = this._utilities.CoalesceDateTime(rw, "LeaseAgreementDue"),
          RegistrationExpiration = this._utilities.CoalesceDateTime(rw, "RegistrationExpiration"),
          DotInspectionExpiration = this._utilities.CoalesceDateTime(rw, "DotInspectionExpiration"),
          SafetyComments = this._utilities.CoalesceString(rw, "SafetyComments"),
          UnitNumber = this._utilities.CoalesceString(rw, "UnitNumber"),
          Vin = this._utilities.CoalesceString(rw, "Vin"),
          LessorId = this._utilities.CoalesceString(rw, "LessorId"),
          IsActive = this._utilities.CoalesceBool(rw, "IsActive"),
          InsuranceCompany = this._utilities.CoalesceString(rw, "InsuranceCompany"),
          IsSelfInsured = this._utilities.CoalesceBool(rw, "IsSelfInsured"),
          Make = this._utilities.CoalesceString(rw, "Make"),
          Model = this._utilities.CoalesceString(rw, "Model"),
          Year = this._utilities.CoalesceString(rw, "Year"),
          LicensePlate = this._utilities.CoalesceString(rw, "LicensePlate")
        });
    }

    public IEnumerable<Lessor> GetAllLessors() 
    {
      DataTable lessorsTable = new DataTable();
      using (var cmd = new SqlCommand(LessorQueries.Select))
      {
        lessorsTable = this._ds.FetchCommand(cmd);
      }
      if (this._utilities.IsEmpty(lessorsTable))
      {
        return new List<Lessor>();
      }
      return lessorsTable.AsEnumerable()
        .Select(rw => new Lessor
        {
          VendorType = this._utilities.CoalesceString( rw, "VendorType"),
          Id = this._utilities.CoalesceString(rw, "Id"),
          Name = this._utilities.CoalesceString(rw, "Name"),
          Address = this._utilities.CoalesceString(rw, "Address"),
          City = this._utilities.CoalesceString(rw, "City"),
          State = this._utilities.CoalesceString(rw, "State"),
          Zip = this._utilities.CoalesceString(rw, "Zip"),
          Country = this._utilities.CoalesceString(rw, "Country"),
          Phone = this._utilities.CoalesceString(rw, "Phone"),
          Fax = this._utilities.CoalesceString(rw, "Fax"),
          ContactPerson = this._utilities.CoalesceString(rw, "ContactPerson"),
          OfficeHours = this._utilities.CoalesceString(rw, "OfficeHours"),
          Comment = this._utilities.CoalesceString(rw, "Comment"),
          Has1099 = this._utilities.CoalesceBool(rw, "Has1099"),
          BoxNumberFor1099 = this._utilities.CoalesceInt(rw, "BoxNumberFor1099"),
          TaxId = this._utilities.CoalesceString(rw, "TaxId"),
          GlRsAccount = this._utilities.CoalesceString(rw, "GlRsAccount"),
          GlRsExpenseDepartment = this._utilities.CoalesceString(rw, "GlRsExpenseDepartment"),
          GlRsExpenseAccount = this._utilities.CoalesceString(rw, "GlRsExpenseAccount"),
          Method = this._utilities.CoalesceString(rw, "Method"),
          Rate = this._utilities.CoalesceDecimal(rw, "Rate"),
          Adjustments1099 = this._utilities.CoalesceDecimal(rw, "Adjustments1099"),
          HasDifferentAddress = this._utilities.CoalesceBool(rw, "HasDifferentAddress"),
          PaymentName = this._utilities.CoalesceString(rw, "PaymentName"),
          PaymentAddress = this._utilities.CoalesceString(rw, "PaymentAddress"),
          PaymentCity = this._utilities.CoalesceString(rw, "PaymentCity"),
          PaymentState = this._utilities.CoalesceString(rw, "PaymentState"),
          PaymentZip = this._utilities.CoalesceString(rw, "PaymentZip"),
          PaymentCountry = this._utilities.CoalesceString(rw, "PaymentCountry"),
          UsesCanadianFunds = this._utilities.CoalesceBool(rw, "UsesCanadianFunds"),
          InsuranceExpiration = this._utilities.CoalesceDateTime(rw, "InsuranceExpiration"),
          IsCarrier = this._utilities.CoalesceBool(rw, "IsCarrier"),
          McNumber = this._utilities.CoalesceString(rw, "McNumber"),
          DueDays = this._utilities.CoalesceInt(rw, "DueDays"),
          DeadheadRate = this._utilities.CoalesceDecimal(rw, "DeadheadRate"), 

        }).ToList<Lessor>();

    }
    public IEnumerable<Tractor> GetTractors()
    {
      var tractorsTable = new DataTable();
      using (var cn = new SqlConnection(this.ConnectionString))
      using (var cmd = new SqlCommand(TractorQueries.Tractors, cn))
      using (var adapter = new SqlDataAdapter(cmd))
      {
        this.RunTractorSanityChecks(cn);
        adapter.Fill(tractorsTable);
      }

      if (tractorsTable.Rows.Count == 0)
      {
        return new List<Tractor>();
      }
      var tractors = new List<Tractor>();
      foreach (DataRow rw in tractorsTable.Rows)
      {
        tractors.Add(this.GetTractor(rw));
      }
      
      return tractors;
    }

    internal void UpdateTrailer(Trailer trailer)
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

    private void RunTractorSanityChecks(SqlConnection cn)
    {
      using (var cmd = new SqlCommand(TractorQueries.TractorSanityQuery, cn))
      {
        if (cn.State == ConnectionState.Closed)
        {
          cn.Open();
        }
        cmd.ExecuteNonQuery();
      }
    }

    public void UpdateTractor(Tractor tractor)
    {
      using(var cn = new SqlConnection(this.ConnectionString))
      using (var cmd = new SqlCommand(TractorQueries.UpdateTractors, cn))
      {
        cmd.Parameters.AddWithValue("@Active", tractor.Active);
        cmd.Parameters.AddWithValue("@Comments", tractor.Comments);
        cmd.Parameters.AddWithValue("@HasTripAssigned", tractor.HasTripAssigned);
        cmd.Parameters.AddWithValue("@HasW9", tractor.HasW9);
        cmd.Parameters.AddWithValue("@InspectionDue", tractor.InspectionDue);
        cmd.Parameters.AddWithValue("@IsLessorTruck", tractor.IsLessorTruck);
        cmd.Parameters.AddWithValue("@LeaseAgreementDue", tractor.LeaseAgreementDue);
        cmd.Parameters.AddWithValue("@LessorOwnerName", tractor.LessorOwnerName);
        cmd.Parameters.AddWithValue("@LicensePlate", tractor.LicensePlate);
        cmd.Parameters.AddWithValue("@LastMaintenance", tractor.LastMaintenance);
        cmd.Parameters.AddWithValue("@Make", tractor.Make);
        cmd.Parameters.AddWithValue("@Model", tractor.Model);
        cmd.Parameters.AddWithValue("@InsuranceExpiration", tractor.InsuranceExpiration);
        cmd.Parameters.AddWithValue("@RegistrationExpiration", tractor.RegistrationExpiration);
        cmd.Parameters.AddWithValue("@Type", tractor.Type);
        cmd.Parameters.AddWithValue("@Unit", tractor.Unit);
        cmd.Parameters.AddWithValue("@VIN", tractor.VIN);
        cmd.Parameters.AddWithValue("@Year", tractor.Year);
        cmd.Parameters.AddWithValue("@Id", tractor.Id);
        
        if (cn.State == ConnectionState.Closed)
        {
          cn.Open();
        }
        cmd.ExecuteNonQuery();
      }
    }

    private Tractor GetTractor(DataRow t)
    {
      var tractor = new Tractor();
        tractor.Id = t.Field<string>("Id");
        tractor.Active = t.Field<bool>("Active");
        tractor.Comments = this._utilities.CoalesceString(t, "Comments");
        tractor.HasTripAssigned = t.Field<bool>("HasTripAssigned");
        tractor.HasW9 = t.Field<bool>("HasW9");
        tractor.InspectionDue = this._utilities.CoalesceDateTime(t, "InspectionDue");
        tractor.IsLessorTruck = t.Field<bool>("IsLessorTruck");
        tractor.LeaseAgreementDue = t.Field<DateTime>("LeaseAgreementDue");
        tractor.LessorOwnerName = this._utilities.CoalesceString(t, "LessorOwnerName");
        tractor.LicensePlate = this._utilities.CoalesceString(t, "LicensePlate");
        tractor.LastMaintenance = t.Field<DateTime>("LastMaintenance");
        tractor.InsuranceExpiration = t.Field<DateTime>("InsuranceExpiration");
        tractor.Make = this._utilities.CoalesceString(t, "Make");
        tractor.Model = this._utilities.CoalesceString(t, "Model");
        tractor.RegistrationExpiration = t.Field<DateTime>("RegistrationExpiration");
        tractor.Type =  this._utilities.CoalesceString(t, "Type");
        tractor.Unit = this._utilities.CoalesceString(t, "Unit");
        tractor.VIN = this._utilities.CoalesceString(t, "VIN");
        tractor.Year = this._utilities.CoalesceString(t, "Year");
      return tractor;
    }

    internal IEnumerable<Trailer> GetTrailers()
    {
      var trailersTable = new DataTable();
      using (var cn = new SqlConnection(this.ConnectionString))
      using (var cmd = new SqlCommand(TrailerQueries.Trailers, cn))
      using (var adapter = new SqlDataAdapter(cmd))
      {
        this.RunTrailerSanityChecks(cn);
        adapter.Fill(trailersTable);
      }

      if (trailersTable.Rows.Count == 0)
      {
        return new List<Trailer>();
      }
      var trailers = new List<Trailer>();
      foreach (DataRow rw in trailersTable.Rows)
      {
        trailers.Add(this.GetTrailer(rw));
      }

      return trailers;
    }
    internal void UpdateLessors(IEnumerable<Lessor> lessors)
    {
      var lessorsXml = GetLessorsXml(lessors);
      lessorsXml.ToString();

      var recordsAffected = 0;

      using (var cn = new SqlConnection(this.ConnectionString))
      using (var cmd = new SqlCommand(TractorQueries.LessorUpdate, cn))
      {
        cmd.Parameters.AddWithValue("lessorsString", lessorsXml.ToString());
        cn.Open();

        recordsAffected = cmd.ExecuteNonQuery();
      }
      if (recordsAffected == 0)
      {
        throw new KeyNotFoundException("Tractor not found");
      }
    }

    internal void UpdateTractorQualifications(IEnumerable<TractorQualification> tqs)
    {
      var recordsAffected = 0;
      
      using (var cn = new SqlConnection(this.ConnectionString))
      using(var cmd = new SqlCommand(TractorQueries.LessorUpdate, cn))
      {
        cn.Open();
        recordsAffected = cmd.ExecuteNonQuery();
      }
      if (recordsAffected == 0)
      {
        throw new KeyNotFoundException("Tractor not found");
      }
    }
    private XDocument GetLessorsXml(IEnumerable<Lessor> lessors)
    {
      var lessorXml = new XDocument(new XElement("lessors",
        lessors.Select(lessor => new XElement("lessor",
          new XAttribute("VendorType", lessor.VendorType),
          new XAttribute("Id", lessor.Id),
          new XAttribute("Name", lessor.Name),
          new XAttribute("Address", lessor.Address),
          new XAttribute("City", lessor.City),
          new XAttribute("State", lessor.State),
          new XAttribute("Zip", lessor.Zip),
          new XAttribute("Country", lessor.Country),
          new XAttribute("Phone", lessor.Phone),
          new XAttribute("Fax", lessor.Fax),
          new XAttribute("ContactPerson", lessor.ContactPerson),
          new XAttribute("OfficeHours", lessor.OfficeHours),
          new XAttribute("Comment", lessor.Comment),
          new XAttribute("Has1099", lessor.Has1099),
          new XAttribute("BoxNumberFor1099", lessor.BoxNumberFor1099),
          new XAttribute("TaxId", lessor.TaxId),
          new XAttribute("GlRsAccount", lessor.GlRsAccount),
          new XAttribute("GlRsExpenseDepartment", lessor.GlRsExpenseDepartment),
          new XAttribute("GlRsExpenseAccount", lessor.GlRsExpenseAccount),
          new XAttribute("Method", lessor.Method),
          new XAttribute("Rate", lessor.Rate),
          new XAttribute("Adjustments1099", lessor.Adjustments1099),
          new XAttribute("HasDifferentAddress", lessor.HasDifferentAddress),
          new XAttribute("PaymentName", lessor.PaymentName),
          new XAttribute("PaymentAddress", lessor.PaymentAddress),
          new XAttribute("PaymentCity", lessor.PaymentCity),
          new XAttribute("PaymentState", lessor.PaymentState),
          new XAttribute("PaymentZip", lessor.PaymentZip),
          new XAttribute("PaymentCountry", lessor.PaymentCountry),
          new XAttribute("UsesCanadianFunds", lessor.UsesCanadianFunds),
          new XAttribute("InsuranceExpiration", lessor.InsuranceExpiration),
          new XAttribute("IsCarrier", lessor.IsCarrier),
          new XAttribute("McNumber", lessor.McNumber),
          new XAttribute("DueDays", lessor.DueDays),
          new XAttribute("DeadheadRate", lessor.DeadheadRate)))));
      return lessorXml;
    }
    private Trailer GetTrailer(DataRow t)
    {
      var trailer = new Trailer();
      trailer.Id = t.Field<string>("Id");
      trailer.Active = t.Field<bool>("Active");
      trailer.Comments = this._utilities.CoalesceString(t, "Comments");
      trailer.HasTripAssigned = t.Field<bool>("HasTripAssigned");
      trailer.InspectionDue = this._utilities.CoalesceDateTime(t, "InspectionDue");
      trailer.IsLessorTrailer = t.Field<bool>("IsLessorTrailer");
      trailer.LessorOwnerName = this._utilities.CoalesceString(t, "LessorOwnerName");
      trailer.LicensePlate = this._utilities.CoalesceString(t, "LicensePlate");
      trailer.LastMaintenance = t.Field<DateTime>("LastMaintenance");
      trailer.Make = this._utilities.CoalesceString(t, "Make");
      trailer.Model = this._utilities.CoalesceString(t, "Model");
      trailer.RegistrationExpiration = t.Field<DateTime>("RegistrationExpiration");
      trailer.Type = this._utilities.CoalesceString(t, "Type");
      trailer.Unit = this._utilities.CoalesceString(t, "Unit");
      trailer.VIN = this._utilities.CoalesceString(t, "VIN");
      trailer.Year = this._utilities.CoalesceString(t, "Year");
      return trailer;
    }

    private void RunTrailerSanityChecks(SqlConnection cn)
    {
      using (var cmd = new SqlCommand(TrailerQueries.TrailerSanityQuery, cn))
      {
        if (cn.State == ConnectionState.Closed)
        {
          cn.Open();
        }
        cmd.ExecuteNonQuery();
      }
    }
  }
}

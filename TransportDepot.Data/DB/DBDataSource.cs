﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Models.DB;
using System.Data.SqlClient;
using System.Data;

namespace TransportDepot.Data.DB
{
  public class DBDataSource:IDataSource
  {
    private Utilities _utilities = new Utilities();
    
    public string ConnectionString
    {
      get
      {
        var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AccessReplacementConnectionString"].ConnectionString;
        return connectionString;
      }
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
      var tractors = new List<Tractor>();// tractorsTable.AsEnumerable().Select(t => this.GetTractor(t));
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

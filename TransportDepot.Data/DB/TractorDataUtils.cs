using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TransportDepot.Models.DB;

namespace TransportDepot.Data.DB
{
  class TractorDataUtils
  {
    private Utilities _utilities = new Utilities();

    public Tractor GetTractor(DataRow t)
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
      tractor.InsuranceName = t.Field<string>("InsuranceName");
      tractor.InsuranceExpiration = t.Field<DateTime>("InsuranceExpiration");
      tractor.Make = this._utilities.CoalesceString(t, "Make");
      tractor.Model = this._utilities.CoalesceString(t, "Model");
      tractor.RegistrationExpiration = t.Field<DateTime>("RegistrationExpiration");
      tractor.Type = this._utilities.CoalesceString(t, "Type");
      tractor.Unit = this._utilities.CoalesceString(t, "Unit");
      tractor.VIN = this._utilities.CoalesceString(t, "VIN");
      tractor.Year = this._utilities.CoalesceString(t, "Year");
      return tractor;
    }
  }
}

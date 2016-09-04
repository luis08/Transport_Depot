using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Models.Safety;

namespace TransportDepot.Data.Safety
{
  class SafetyEntityFactory
  {
    internal Tractor MakeTractor(Models.DB.Tractor t)
    {
      return new Tractor
      {
        Id = t.Id,
        Unit = t.Unit,
        LessorOwnerName = t.LessorOwnerName,
        Comments = t.Comments,
        InspectionDue = t.InspectionDue,
        InsuranceName = t.InsuranceName,
        LeaseAgreementDue = t.LeaseAgreementDue,
        LastMaintenance = t.LastMaintenance,
        RegistrationExpiration = t.RegistrationExpiration,
        HasW9 = t.HasW9, 
        InsuranceExpiration = t.InsuranceExpiration,
        VIN = t.VIN
      };
    }


    internal Trailer MakeTrailer(Models.DB.Trailer t)
    {
      return new Trailer
      {
        Id = t.Id,
        Unit = t.Unit,
        VIN = t.VIN,
        LessorOwnerName = t.LessorOwnerName,
        LastMaintenance = t.LastMaintenance,
        RegistrationExpiration = t.RegistrationExpiration,
        InspectionDue = t.InspectionDue,
        Comments = t.Comments
      };
    }
  }
}

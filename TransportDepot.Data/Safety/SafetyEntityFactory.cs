using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Models.Safety;
using TransportDepot.Models.Business;

namespace TransportDepot.Data.Safety
{
  class SafetyEntityFactory
  {
    
    
    internal TransportDepot.Models.DB.TractorQualification MakeQualificationTractor(Tractor t, string lessorId)
    {
      var tq = new TransportDepot.Models.DB.TractorQualification 
      {
        Id = t.Id,
        UnitNumber = t.Unit,
        LessorId = lessorId,
        DotInspectionExpiration = t.InspectionDue,
        SafetyComments = t.Comments,
        LeaseAgreementDue = t.LeaseAgreementDue,
        RegistrationExpiration = t.RegistrationExpiration,
        Vin = t.VIN,
        Make = t.Make,
        Model = t.Model,
        LicensePlate = t.LicensePlate,
        HasW9 = t.HasW9,
        InsuranceCompany = t.InsuranceCompany,
        IsSelfInsured = t.IsSelfInsured,
        IsActive = t.IsActive
      };
      return tq;
    }

    //TODO: DateTime
    internal Tractor MakeTractor(Models.Business.Lessor l, Models.DB.TractorQualification tq)
    {
      return new Tractor
      {
        Id = l.Id,
        Unit = tq.UnitNumber,
        LessorOwnerName = l.Name,
        Comments = tq.SafetyComments,
        InspectionDue = tq.DotInspectionExpiration,
        LeaseAgreementDue = tq.LeaseAgreementDue,
        LastMaintenance = DateTime.Now,
        RegistrationExpiration = tq.RegistrationExpiration,
        HasW9 = tq.HasW9, 
        InsuranceExpiration = l.InsuranceExpiration,
        VIN = tq.Vin,
        Make = tq.Make,
        Model = tq.Model, 
        Year = tq.Year,
        LicensePlate = tq.LicensePlate,
        InsuranceCompany = tq.InsuranceCompany,
        IsSelfInsured = tq.IsSelfInsured
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

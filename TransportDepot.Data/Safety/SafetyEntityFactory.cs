using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Models.Safety;

namespace TransportDepot.Data.Safety
{
  class SafetyEntityFactory
  {
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
        VIN = tq.Vin
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

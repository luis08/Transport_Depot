using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_AutomaticDeductions
{
  [DataContract]
  public class ChangeCommission
  {
    [DataMember]
    public string LessorId
    {
      get;
      set;
    }

    [DataMember]
    public string TripNumber
    {
      get;
      set;
    }

    [DataMember]
    public decimal Commission
    {
      get;
      set;
    }

    [DataMember]
    public bool ChangeCommissionDefault
    {
      get;
      set;
    }

    [DataMember]
    public decimal CargoInsurance
    { 
      get; 
      set; 
    }
    [DataMember]
    public decimal LiabilityInsurance 
    { 
      get; 
      set; 
    }
  }
}

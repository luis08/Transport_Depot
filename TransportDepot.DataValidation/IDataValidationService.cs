using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Models.DataValidation;
using System.ServiceModel;

namespace TransportDepot.DataValidation
{
  [ServiceContract]
  public interface IDataValidationService
  {
    [OperationContract]
    ValidationResponse Validate(string name);
    
    [OperationContract]
    IEnumerable<ValidationResponse> ValidateMany(IEnumerable<string> names);
  }
}

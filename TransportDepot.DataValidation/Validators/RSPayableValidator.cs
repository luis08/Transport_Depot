using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.DataValidation.Validators
{
  class RSPayableValidator:IValidator
  {
    public string Name { get; set; }

    public bool IsValid { get; set; }

    public string Message { get; set; }

    public void Validate()
    {
      throw new NotImplementedException();
    }
  }
}

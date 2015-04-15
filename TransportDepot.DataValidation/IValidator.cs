using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransportDepot.DataValidation
{
  interface IValidator
  {
    string Name { get; set; }
    bool IsValid { get; set; }
    string Message { get; set; }
    void Validate();
  }
}

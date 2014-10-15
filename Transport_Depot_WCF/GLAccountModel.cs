using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF
{
  [DataContract]
  public class GLAccountModel
  {
    [DataMember]
    public string Department { get; set; }
    
    [DataMember]
    public string Account { get; set; }
  }
}

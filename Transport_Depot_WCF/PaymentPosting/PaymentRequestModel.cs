﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Transport_Depot_WCF.PaymentPosting
{
  [DataContract]
  class PaymentRequestModel 
  {
    [DataMember]
    public ACHFileIdentifierModel FileIdentifier {get; set;}
    [DataMember]
    public PaymentGlModel GlAccounts { get; set; }
  }
}

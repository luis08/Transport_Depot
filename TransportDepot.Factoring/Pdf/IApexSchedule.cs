using System;
using System.Collections.Generic;
using TransportDepot.Models.Factoring;



namespace TransportDepot.Factoring
{
  interface IApexSchedule
  {
    int ScheduleId { get; set; }
    DateTime Date { get; set; }
    string ClientNumber { get; set; }
    string ClientName { get; set; }
    string ClientAddress1 { get; set; }
    string ClientAddress2 { get; set; }
    string ClientWatts { get; set; }
    string ClientLocalPhone { get; set; }
    string ClientFax { get; set; }
    IEnumerable<InvoiceViewModel> Invoices { get; set; }

  }
}

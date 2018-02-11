using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Data.DB;
using TransportDepot.Models.AR;
using TransportDepot.Models.DB;

namespace TransportDepot.AccountsReceivable
{
  public class AccountsReceivableService : IAccountsReceivableService
  {
    private AgingDataSource _agingData = new AgingDataSource();
    private CustomerDataSource _customerData = new CustomerDataSource();
    private ARUtilities _utilities = new ARUtilities();
    private Dictionary<string, Models.DB.Customer> _customersDictionary = null;

    public IEnumerable<Models.DB.CustomerAging> GetAging()
    {
      var agingData = this._agingData.GetAging();
      return agingData;
    }

    public IEnumerable<Models.DB.CustomerAging> GetAgingForCustomers(IEnumerable<string> ids)
    {
      var agingdata = this.GetAging().Join(ids, ag => ag.CustomerID.ToUpperInvariant(),
        customerId => customerId.ToUpperInvariant(), (a, c) => a);
      return agingdata.ToList();
    }

    public IEnumerable<Models.AR.CustomerAgingSummary> GetAgingSummary()
    {
      var allAging = this.GetAllCustomerInvoices();
      var customerIds = allAging.Select(a => a.CustomerId).Distinct();
      var customers = this.GetCustomersDictionary(customerIds);
      Customer customer = null;

      var groupedAging = allAging.GroupBy(a =>
      {
        customer = this._utilities.GetCustomer(a.CustomerId, customers);
        return new
        {
          CustomerId = a.CustomerId,
          CustomerName = customer.CustomerName,
          CreditLimitDayCount = customer.CreditLimitDayCount
        };
      }).Select(b => new CustomerAgingSummary
        {
          CustomerId = b.Key.CustomerId,
          CustomerName = b.Key.CustomerName,
          CustomerCreditDayCount = b.Key.CreditLimitDayCount,
          Balance = b.Sum(a => a.Amount),
          MaxAging = b.Max(a => a.Aging),
          OverdueBalance = b.Where(a => a.IsOverdue).Sum(c => c.Amount),
          TotalInvoiceCount = b.Count(),
          OverdueInvoiceCount = b.Where(c => c.IsOverdue).Count(),
        }).ToList();

      return groupedAging;
    }

    public IEnumerable<CollectionsReportCustomer> GetAllAgingDetails()
    {
      var allAging = this.GetAllCustomerInvoices();
      var customerIds = allAging.Select(a => a.CustomerId).Distinct();
      var customers = this.GetCustomersDictionary(customerIds);
      var agingDetails= this.GetCollectionsReportCustomers(allAging, customers);

      return agingDetails;
    }

    public IEnumerable<CollectionsReportCustomer> GetAgingDetails(int agingClass)
    {
      var allAging = this.GetAllCustomerInvoices();
      var customerIds = allAging.Select(a => a.CustomerId).Distinct();
      var customers = this.GetCustomersDictionary(customerIds)
         .Where(c=>this._utilities.GetAgingClass( c.Value.CreditLimitDayCount ) == (AgingClass) agingClass)
         .ToDictionary(c=>c.Key, c=>c.Value);

      return this.GetCollectionsReportCustomers(allAging, customers);
    }

    private IEnumerable<CollectionsReportCustomer> GetCollectionsReportCustomers(IEnumerable<AgingInvoice> allAging, Dictionary<string, Customer> customers)
    {
      var agingDetails = allAging.Where(a => customers.ContainsKey(a.CustomerId)).ToList()
        .GroupBy(c => c.CustomerId)
        .Select(d =>
        {
          var customer = this._utilities.GetCustomer(d.Key, customers);
          var customerAgingClass = this._utilities.GetAgingClass(customer.CreditLimitDayCount);
          return new CollectionsReportCustomer
          {
            Customer = customer,
            Invoices = d.Select(i => new CollectionsReportInvoice
            {
              Aging = i.Aging,
              InvoiceNumber = i.Number,
              Reference = i.CustomerReference,
              Date = i.InvoiceDate,
              Current = this._utilities.GetCurrent(i, customer),
              OverThirty = this._utilities.GetOver30(i, customer),
              Late = this._utilities.GetLate(i, customer),
              OverSixty = this._utilities.GetOver60(i, customer)
            }).ToList()
          };
        }).ToList();
      
      return agingDetails;
    }

    private Dictionary<string, Models.DB.Customer> GetCustomersDictionary(IEnumerable<string> customerIds)
    {
      if (this._customersDictionary != null)
      {
        return this._customersDictionary;
      }
      var upperDistinctCustomerIds = customerIds.Select(c => c.ToUpper()).Distinct();
      var customers = this._customerData.GetCustomers(upperDistinctCustomerIds);
      this._customersDictionary = customers.ToDictionary(c => c.ID, c => c);

      return this._customersDictionary;
    }

    public IEnumerable<Models.AR.CustomerAgingSummary> GetAgingSummaryForCustomers(IEnumerable<string> ids)
    {
      var selectedAging = this.GetAgingSummary()
        .Join(ids, a => a.CustomerId, i => i, (a, i) => a)
        .ToList();
      return selectedAging;
    }

    public IEnumerable<AgingInvoice> GetCustomerInvoices(IEnumerable<string> ids)
    {
      var allAging = this.GetAllCustomerInvoices();
      var clientsAging = allAging.Join(ids, a => a.CustomerId,
        i => i, (a, i) => a);

      return clientsAging.ToList();
    }

    public IEnumerable<AgingInvoice> GetAllCustomerInvoices()
    {
      var allAging = this.GetAging();
      var usedCustomerIds = allAging.Select(a => a.CustomerID);
      var customers = this.GetCustomersDictionary(usedCustomerIds);

      var invoices = allAging
        .Select(i => new AgingInvoice
        {
          Number = i.InvoiceNumber,
          CustomerId = i.CustomerID,
          CustomerReference = i.CustomerReference,
          InvoiceDate = i.InvoiceDate,
          Amount = i.BalanceDue,
          Aging = this._utilities.GetAging(i),
          IsOverdue = this._utilities.IsOverdue(
            customers.ContainsKey(i.CustomerID) ? customers[i.CustomerID] : null, i)
        }).ToList();

      return invoices;
    }
    
    public IEnumerable<CollectionLetter> GetCollectionLetters(IEnumerable<string> customerIds)
    {
      var customersDictionary = this.GetCustomersDictionary(customerIds);

      var invoices = this.GetCustomerInvoices(customerIds).Where(i => i.IsOverdue);
      var invoiceDictionary = invoices.GroupBy(c => c.CustomerId)
        .ToDictionary(p => p.Key, q => q, StringComparer.OrdinalIgnoreCase);

      var collectionLetters = invoiceDictionary.Select(i => new CollectionLetter
        {
          Customer = this._utilities.GetCustomer(i.Key, customersDictionary),
          Invoices = i.Value
        }).ToList();
      return collectionLetters;
    }
    
    public IEnumerable<CustomerAgingSummary> GetOverdueAgingSummary()
    {
      var overdueAging = this.GetAgingSummary()
        .Where(a => a.MaxAging > a.CustomerCreditDayCount).ToList();

      return overdueAging;
    }

    public IEnumerable<CustomerAgingSummary> GetAgingSummaryExceedingDays(int dayCount)
    {
      var agingExceedingDays = this.GetAgingSummary()
        .Where(a => a.MaxAging > dayCount).ToList();
      return agingExceedingDays;
    }
  }
}

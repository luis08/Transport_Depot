using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportDepot.Models.DB;
using TransportDepot.Models.AR;

namespace TransportDepot.AccountsReceivable
{
  public class ARUtilities
  {
    private int FactoringCreditPeriod
    {
      get;
      set;
    }

    public ARUtilities()
    {
      var creditPeriod = System.Configuration.ConfigurationManager.AppSettings["TransportDepot.AccountsReceivable.FactoringCreditPeriod"];

      this.FactoringCreditPeriod = int.Parse(creditPeriod);
    }


    public bool IsOverdue(Customer customer, CustomerAging invoice)
    {
      if (customer == null)
      {
        return false;
      }
      var aging = this.GetAging(invoice);
      return aging > customer.CreditLimitDayCount;
    }

    public int GetAging(CustomerAging invoice)
    {
      return (DateTime.Today - invoice.InvoiceDate).Days;
    }

    public Customer GetCustomer(string id, Dictionary<string, Customer> customersDictionary)
    {
      if (customersDictionary.ContainsKey(id))
      {
        return customersDictionary[id];
      }
      var bogusCustomer = new TransportDepot.Models.DB.Customer
      {
        ID = id,
        CustomerName = string.Format("ID not found: {0}", id),
        StreetAddress = string.Empty,
        City = string.Empty,
        State = string.Empty,
        Zip = string.Empty,
        Phone = string.Empty,
        Fax = string.Empty,
        CreditLimitAmount = decimal.Zero,
        CreditLimitDayCount = 0
      };
      return bogusCustomer;
    }


    internal AgingClass GetAgingClass(int aging)
    {
      if (aging <= 0)
      {
        return AgingClass.NoCredit;
      }
      if (aging.Equals(this.FactoringCreditPeriod))
      {
        return AgingClass.Factored;
      }
      return AgingClass.QuickPay;
    }

    internal decimal GetCurrent(AgingInvoice invoice, Customer customer)
    {
      var isCurrent = invoice.Aging <= customer.CreditLimitDayCount;
      if (isCurrent)
      {
        return invoice.Amount;
      }
      return decimal.Zero;
    }

    internal decimal GetOver30(AgingInvoice invoice, Customer customer)
    {
      var isOver30 = invoice.Aging > 30;
      if (isOver30)
      {
        return invoice.Amount;
      }
      return decimal.Zero;
    }

    internal decimal GetLate(AgingInvoice invoice, Customer customer)
    {
      var isLate = (invoice.Aging > customer.CreditLimitDayCount)
        && ( invoice.Aging <= 30);
      if (isLate)
      {
        return invoice.Amount;
      }
      return decimal.Zero;
    }

    internal decimal GetOver60(AgingInvoice invoice, Customer customer)
    {
      var isOver60 = invoice.Aging > 60;
      if (isOver60)
      {
        return invoice.Amount;
      }
      return decimal.Zero;
    }
  }
}

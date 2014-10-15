using System;
using WebPayments.ACHElements;
/// <summary>
/// Summary description for IWebPaymentsDAL
/// </summary>
public interface IWebPaymentsDAL
{
  WebPaymentsDue GetWebPaymentsDue();
  void Save( ACHFile file );
}

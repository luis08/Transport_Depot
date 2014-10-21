using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transport_Depot_WCF.PaymentPosting;
using System.ServiceModel.Activation;
using Transport_Depot_WCF.Helpers;

namespace Transport_Depot_WCF
{
  class AccessPaymentPostingService: IPaymentPostingService
  {
    public void Post(PaymentRequestModel model)
    {
      
      try
      {
        var dal = new PaymentPostingDAL();
        dal.Post(model);
      }
      catch (Exception e)
      { 
        
      }
    }

    public string GetCheckNumber(ACHFileIdentifierModel model)
    {
      try
      {
        var dal = new PaymentPostingDAL();
        var checkNumber = dal.GetCheckNumber(model);
        return checkNumber;
      }
      catch (Exception e)
      {
        throw;
      }
    }


    public string GetExistingCheckNumber(ACHFileIdentifierModel model)
    {
      var dal = new PaymentPostingDAL();
      var checkNumber = dal.GetCheckNumber(model);
      bool exists = dal.CheckExists(checkNumber);
      if (exists)
      { return checkNumber; }
      return string.Empty;
    }


    public IEnumerable<CheckExistenceTestModel> GetCheckExistenceModels(ACHFileIdentifierModel[] models)
    {
      var dal = new PaymentPostingDAL();
      IEnumerable<CheckExistenceTestModel>  viewModel = new List<CheckExistenceTestModel>();
        viewModel = dal.GetCheckTestModels(models);
        
      return viewModel;
    }


    public void IgnoreCheck(ACHFileIdentifierModel model)
    {
      if (model == null)
      { throw new ArgumentException("model is null - cannot ignore check"); }
      var dal = new PaymentPostingDAL();
      dal.IgnoreCheck(model);
    }

    public void DontIgnoreCheck(ACHFileIdentifierModel model)
    {
      var dal = new PaymentPostingDAL();
      dal.DontIgnoreCheck(model);
    }


    public PaymentGlModel GetDefaultPaymentAccounts()
    {
      var paymentGlModel = new PaymentGlModel
      {
        ACHCashAccount = new GLAccountModel
        {
          Department = PaymentPostingDAL.GetSetting("DefaultPostCashDepartment"),
          Account = PaymentPostingDAL.GetSetting("DefaultPostCashAccount")
        },
        SettlementAccount = new GLAccountModel
        {
          Department = PaymentPostingDAL.GetSetting("DefaultPostSettlementDepartment"),
          Account = PaymentPostingDAL.GetSetting("DefaultPostSettlementAccount")
        }
      };
      return paymentGlModel;
    }

    private string GetSetting(string settingName)
    {
      string settingValue = System.Configuration.ConfigurationManager.AppSettings[settingName];
      if (settingName == null)
      {
        return string.Empty;
      }
      return settingValue.Trim(); 
    }
  }
}

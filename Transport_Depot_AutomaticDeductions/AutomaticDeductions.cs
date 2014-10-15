using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transport_Depot_AutomaticDeductions
{
  public class AutomaticDeductions : IAutomaticDeductions
  {
    private DataSource _dataSource = new DataSource();
    public AutomaticDeductions()
    {

    }

    public ChangeCommissionLogicModel GetCommissionsToChange()
    {
      return GetDeductions();
    }

    public void SaveCommissionsToChange(IEnumerable<ChangeCommission> automaticDeductions)
    {
      this._dataSource.SaveCommissions(automaticDeductions);
    }


    public IEnumerable<string> GetLessorIDs()
    {
      return GetLessors();
    }

    public LessorDefaults GetLessorDefaults(string lessor_id)
    {
      return this._dataSource.GetLessorDefaults(lessor_id);
    }


    public void SaveLessorsDefaults(LessorDefaults lDefaults)
    {
      this._dataSource.Save(lDefaults);
    }

    public IEnumerable<DefaultTypeAndValue> GetDefaultTypes()
    {
      List<DefaultTypeAndValue> types = new List<DefaultTypeAndValue>();
      DefaultTypeAndValue t1 = new DefaultTypeAndValue();
      t1.DefaultType = "Commission";
      DefaultTypeAndValue t2 = new DefaultTypeAndValue();
      t2.DefaultType = "Liability";
      DefaultTypeAndValue t3 = new DefaultTypeAndValue();
      t3.DefaultType = "Cargo";
      types.Add(t1);
      types.Add(t2);
      types.Add(t3);

      return types;
    }

    public void AddDefaultValue(DefaultTypeAndValue aDefaults)
    {
      this._dataSource.Save(aDefaults);
    }


    private IEnumerable<string> GetLessors()
    {
      List<string> lessors = new List<string>();
      lessors.Add("Abel");
      lessors.Add("Tony");
      return lessors;
    }

    private ChangeCommissionLogicModel GetDeductions()
    {
      var model = this._dataSource.GetCommissionLogicModel();
      return model;
    }

    private ChangeCommissionLogicModel GetDeductionsTest()
    {
      ChangeCommissionLogicModel model = new ChangeCommissionLogicModel();
      List<decimal> rates = new List<decimal>();
      rates.Add(decimal.Parse("" + 0.05));
      rates.Add(decimal.Parse("" + 0.10));
      rates.Add(decimal.Parse("" + 0.12));
      model.CommissionRates = rates;
      List<ChangeCommission> deductions = new List<ChangeCommission>();
      for (int i = 1; i <= 4; i++)
      {
        ChangeCommission deduction = new ChangeCommission();
        deduction.TripNumber = "Trip_" + i;
        deduction.ChangeCommissionDefault = false;
        if (i <= 2)
        {
          deduction.LessorId = "Jorge";
          if (i == 2)
            deduction.Commission = decimal.Parse("" + 0.10);
          else deduction.Commission = decimal.Parse("" + 0.12);
        }
        else
        {
          deduction.LessorId = "Pepe";
          if (i == 4)
            deduction.Commission = decimal.Parse("" + 0.12);
          else deduction.Commission = decimal.Parse("" + 0.05);
        }
        deductions.Add(deduction);
      }
      model.Deductions = deductions;

      return model;
    }
  }
}

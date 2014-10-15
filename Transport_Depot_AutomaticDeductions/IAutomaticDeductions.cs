using System.Collections.Generic;
using System.ServiceModel;

namespace Transport_Depot_AutomaticDeductions
{
  [ServiceContract]
  interface IAutomaticDeductions
  {
    //[FaultContract(typeOf(...))]
    [OperationContract]
    ChangeCommissionLogicModel GetCommissionsToChange();

    [OperationContract]
    void SaveCommissionsToChange(IEnumerable<ChangeCommission> aDeductions);

    
    /*
     * Gets all the the values for every type of deduction
     */
    [OperationContract]
    LessorDefaults GetLessorDefaults(string lessor_id);

    /*
     * Save the defaults for a specific lessor
     */
    [OperationContract]
    void SaveLessorsDefaults(LessorDefaults lDefaults);

    /*
     * Gets all the availables deduction types
     */
    [OperationContract]
    IEnumerable<DefaultTypeAndValue> GetDefaultTypes();

    /*
     * Adds a default value to a type of deduction
     */
    [OperationContract]
    void AddDefaultValue(DefaultTypeAndValue aDefaults);
  }
}

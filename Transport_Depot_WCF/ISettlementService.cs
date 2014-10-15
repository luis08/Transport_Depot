using System;
using System.Collections.Generic;
using System.ServiceModel;
using Transport_Depot_WCF.Settlements;
using System.ServiceModel.Web;


namespace Transport_Depot_WCF
{
    [ServiceContract]
    public interface ISettlementService
    {
        [FaultContract(typeof(InvalidLessorId))]
        [OperationContract]
        Lessor GetLessor(string lessor_id);

        [OperationContract]
        IEnumerable<Lessor> GetLessors(string[] lessor_ids);

        [OperationContract]
        [WebInvoke(Method="POST", ResponseFormat = WebMessageFormat.Json)]
        IEnumerable<Lessor> GetAllLessors();

        [FaultContract(typeof(NoSettlementsAvailable))]
        [FaultContract(typeof(UnpostedSettlementsPending))]
        [FaultContract(typeof(InvalidLessorId))]
        [OperationContract]
        LessorSettlement GetLessorSettlement(string lessor_id);

        [FaultContract(typeof(InvalidLessorId))]
        [FaultContract(typeof(NoSettlementsAvailable))]
        [FaultContract(typeof(UnpostedSettlementsPending))]
        [OperationContract]
        PaidLessorSettlement GetPaidLessorSettlement( int scheduled_payment_id );

        [FaultContract(typeof(InvalidLessorId))]
        [FaultContract(typeof(NoSettlementsAvailable))]
        [FaultContract(typeof(UnpostedSettlementsPending))]
        [OperationContract]
        PaidLessorSettlement SchedulePayment(string lessor_id, string ip_address);

        [FaultContract(typeof(InvalidLessorId))]
        [FaultContract(typeof(NoSettlementsAvailable))]
        [OperationContract]
        IEnumerable<PaidLessorSettlementListItem> GetPaidLessorSettlements(string lessor_id);
    }
}

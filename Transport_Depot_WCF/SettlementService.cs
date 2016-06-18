using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Data.OleDb;
using System.Configuration;
using System.Data;
using Transport_Depot_WCF.Settlements;
using System.Collections.Specialized;
using TransportDepot.Data.DB;

namespace Transport_Depot_WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class SettlementService : ISettlementService
    {
      private DBDataSource _dataSource = new DBDataSource();
        /// <summary>
        /// Throws FaultException<InvalidLessorId>
        /// </summary>
        /// <param name="lessor_id"></param>
        /// <returns></returns>
        public Lessor GetLessor(string lessor_id)
        {
          return this.GetLessors(new string[] { lessor_id }).First();
        }


        /// <summary>
        /// Throws FaultException<InvalidLessorId>
        /// </summary>
        /// <param name="lessor_ids"></param>
        /// <returns></returns>
        public IEnumerable<Lessor> GetLessors(string [] lessor_ids)
        {
          var lessors = this._dataSource.GetLessors(lessor_ids);
          return this.Map(lessors);
        }

        /// <summary>
        /// Throws  FaultException<NoSettlementsAvailable>
        ///         FaultException<InvalidLessorId>
        /// </summary>
        /// <param name="lessor_id"></param>
        /// <returns></returns>
        public LessorSettlement GetLessorSettlement(string lessor_id)
        {
            LessorSettlement settlement = null;
            
            try
            {
              var dal = new SettlementDAL();
              settlement = dal.GetLessorSettlement(lessor_id);
            }
            catch (InvalidLessorIDException)
            {
              throw new FaultException<InvalidLessorId>(
                  new InvalidLessorId
                  {
                    Problem = string.Format("Lessor not found: '{0}'", lessor_id)
                  });
            }
            catch (NoSettlementsAvailableException)
            {
                return new LessorSettlement();
              //throw new FaultException<NoSettlementsAvailable>(
              //    new NoSettlementsAvailable());
            }

            catch (Exception)
            {
              throw new FaultException<UnpostedSettlementsPending>(new UnpostedSettlementsPending());
            }
            
            return settlement;
        }

        public IEnumerable<Lessor> GetAllLessors()
        {
          return this.GetLessors(null);
        }

        /// <summary>
        /// Throws  FaultException<NoSettlementsAvailable>
        /// </summary>
        /// <param name="scheduled_payment_id"></param>
        /// <returns></returns>
        public PaidLessorSettlement GetPaidLessorSettlement(int scheduled_payment_id)
        {
            PaidLessorSettlement settlement = null;
            try 
            {
                SettlementDAL dal = new SettlementDAL();
                settlement = dal.GetPaidLessorSettlement(scheduled_payment_id, true);
            }
            catch( NoSettlementsAvailableException )
            {
                throw new FaultException<NoSettlementsAvailable>(new NoSettlementsAvailable());
            }

            return settlement;
        }

        /// <summary>
        /// Throws  FaultException<NoSettlementsAvailable>
        /// Throws  FaultException<UnpostedSettlementsPending>
        /// Throws  FaultException<InvalidLessorId> 
        /// </summary>
        /// <param name="lessor_id"></param>
        /// <param name="ip_address"></param>
        /// <returns></returns>
        public PaidLessorSettlement SchedulePayment(string lessor_id, string ip_address)
        {
            PaidLessorSettlement settlement = new PaidLessorSettlement();
            
            try
            {
                var dal = new SettlementDAL();
                settlement = dal.SchedulePayment(lessor_id, ip_address);
            }
            catch (NoSettlementsAvailableException)
            {
                
                throw new FaultException<NoSettlementsAvailable>(new NoSettlementsAvailable());

            }
            catch (UnpostedSettlementsPendingException)
            {
                throw new FaultException<UnpostedSettlementsPending>(new UnpostedSettlementsPending());
            }
            catch (InvalidLessorIDException)
            {
                throw new FaultException<InvalidLessorId>(
                    new InvalidLessorId
                    {
                        Problem = string.Format("Lessor not found: '{0}'", lessor_id)
                    });
            }
            return settlement;
        }

        public IEnumerable<PaidLessorSettlementListItem> GetPaidLessorSettlements(string lessor_id)
        {
            IEnumerable<PaidLessorSettlementListItem> paid_settlements = null;
            try
            {
                SettlementDAL dal = new SettlementDAL();
                paid_settlements = dal.GetPaidLessorSettlements(lessor_id);
            }
            catch (InvalidLessorIDException)
            {
                var problem = string.Format("Lessor not found: '{0}'", lessor_id);
                throw new FaultException<InvalidLessorId>
                    (new InvalidLessorId { Problem = problem });
            }
            catch (NoSettlementsAvailableException)
            {
                var problem = string.Format("No Settlement Found");
                throw new FaultException<NoSettlementsAvailable>(new NoSettlementsAvailable());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return paid_settlements;
        }

        private IEnumerable<Lessor> Map(IEnumerable<TransportDepot.Models.Business.Lessor> lessors)
        {
          return lessors.Select(l => new Lessor
          {
            Name = l.Name,
            Address = new Address
            {
              StreetAddress = l.Address.StreetAddress,
              City = l.Address.City,
              State = l.Address.State,
              ZipCode = l.Address.ZipCode,
              Phone = l.Address.Phone
            },
            LessorId = l.LessorId
          });
        }

    }
}

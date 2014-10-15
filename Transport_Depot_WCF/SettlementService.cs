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

namespace Transport_Depot_WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class SettlementService : ISettlementService
    {
        /// <summary>
        /// Throws FaultException<InvalidLessorId>
        /// </summary>
        /// <param name="lessor_id"></param>
        /// <returns></returns>
        public Lessor GetLessor(string lessor_id)
        {
            Lessor lessor = null;
            try
            {
                SettlementDAL dal = new SettlementDAL();
                lessor = dal.GetLessor(lessor_id);
            }
            catch (InvalidLessorIDException)
            {
                throw new FaultException<InvalidLessorId>(
                    new InvalidLessorId
                    {
                        Problem = string.Format("Lessor not found: '{0}'", lessor_id)
                    });
            }
            return lessor;
        }

        /// <summary>
        /// Throws FaultException<InvalidLessorId>
        /// </summary>
        /// <param name="lessor_ids"></param>
        /// <returns></returns>
        public IEnumerable<Lessor> GetLessors(string [] lessor_ids)
        {
            IEnumerable<Lessor> lessors = null;
            try 
            {
                var dal = new SettlementDAL();
                lessors = dal.GetLessors(lessor_ids);
            }
            catch (InvalidLessorIDException)
            {
                throw new FaultException<InvalidLessorId>( new InvalidLessorId()); 
            }

            return lessors;
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

            catch (UnpostedSettlementsPendingException)
            {
              throw new FaultException<UnpostedSettlementsPending>(new UnpostedSettlementsPending());
            }
            catch (Exception ex)
            {
              var lessorMessage = string.Format(" Lessor '{0}'", lessor_id);
              this.Debug(lessorMessage + Environment.NewLine + "Error In Settlement Service:" + Environment.NewLine + ex.Message);
            }
            
            return settlement;
        }

        public IEnumerable<Lessor> GetAllLessors()
        {
            var dal = new SettlementDAL();
            return dal.GetAllLessors();
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


        private void Debug(string text)
        {
            //using (System.IO.StreamWriter w = new System.IO.StreamWriter(@"C:\Transport_Depot\output.txt", true))
            //{
            //    w.WriteLine(
            //        string.Format("{0}-{1}  {2}", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), debug_num++, text)
            //    );
            //}
        }
        private static int debug_num = 0;
    }
}

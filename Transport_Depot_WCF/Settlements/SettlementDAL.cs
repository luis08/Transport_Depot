using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.OleDb;
using System.Data;
using System.Collections.Specialized;
using System.IO;
namespace Transport_Depot_WCF.Settlements
{
    class SettlementDAL
    {

        public SettlementDAL()
        { 
        
        }

        public LessorSettlement GetLessorSettlement(string lessor_id)
        {
          
            var lessor_settlement = new LessorSettlement();
            var trips_stored_procedure = "sp_RS_Get_Lessor_Trips_With_Revenue";
            var adapter = new OleDbDataAdapter();
            var data_set = new DataSet();
            string tractor_id = string.Empty;
            using (var cn = this.Connection)
            {
                using (var cmd = cn.CreateCommand())
                {
                    cn.Open();
                    lessor_settlement.Lessor = this.GetLessor(cn, lessor_id);
                    var disablePaymentAcceptance = this.HasPendingPayments(cn, lessor_id);
              
                        
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = trips_stored_procedure;
                    cmd.Parameters.Add(new OleDbParameter("LessorID", lessor_id));
                    adapter = new OleDbDataAdapter(cmd);
                    data_set = new DataSet();
                    var trip_number = string.Empty;
                    adapter.Fill(data_set);

                    /* Each loop has a different trip number */
                    foreach (DataRow record in data_set.Tables[0].Rows)
                    {
                        trip_number = record["Trip_Number"].ToString();
                        if ((lessor_settlement.Tractor.TractorID == null) && (tractor_id == string.Empty) && (!string.IsNullOrEmpty(record["Tractor_ID"].ToString())))
                        {
                            tractor_id = record["Tractor_ID"].ToString();
                            lessor_settlement.Tractor = new SettlementTractor { TractorID = tractor_id };
                                
                        }

                        var settlement_trip = this.GetSettlementTrip(cn, trip_number);

                        settlement_trip.IsSelectedForPayment = record["IsSelectedForPayment"] == null ? false : (record["IsSelectedForPayment"].ToString() != "0");

                        settlement_trip.InvoiceDate = this.ParseDate(record["Invoice_Date"].ToString());
                        settlement_trip.ActivityDate = this.ParseDate(record["Activity_Date"].ToString());
                        lessor_settlement.Trips.Add(settlement_trip);
                    }
                    lessor_settlement.Advances = this.GetLessorAdvances(cn, lessor_id, false);
                    lessor_settlement.State = disablePaymentAcceptance ? SettlementState.PendingPost : SettlementState.NotAccepted;
                }
            }

            
            return lessor_settlement;    
        }

        public Lessor GetLessor(string lessor_id)
        { 
            using( var cn = this.Connection )
            { return this.GetLessor(cn, lessor_id); }
        }


        /// <summary>
        /// UnpostedSettlementsPendingException -- Can't schedule more until payment posts.
        /// InvalidLessorIDException
        /// </summary>
        /// <param name="lessor_id"></param>
        /// <param name="ip_address"></param>
        /// <returns></returns>
        public PaidLessorSettlement SchedulePayment(string lessor_id, string ip_address)
        {
            var settlement = new PaidLessorSettlement();
            
            using (var cn = this.Connection)
            {

                OleDbTransaction transaction = null;
                
                cn.Open();
                settlement.Lessor = this.GetLessor(cn, lessor_id);
                if (this.HasPendingPayments(cn, lessor_id))
                {
                    throw new UnpostedSettlementsPendingException();
                }

                transaction = cn.BeginTransaction();
                settlement.ScheduledPaymentID = AddPaymentScheduledAndGetId(lessor_id, ip_address, cn, transaction);
                settlement.ShceduledDate = DateTime.Now;
                if (settlement.ScheduledPaymentID == -1)
                {
                    throw new NoSettlementsAvailableException();
                }
                var trips_affected = AppendTripsForLessor(lessor_id, settlement.ScheduledPaymentID, cn, transaction);
                AppendAdvancesForLessor(lessor_id, settlement.ScheduledPaymentID, cn, transaction);
                if (trips_affected < 1)
                { 
                    transaction.Rollback();
                    throw new NoSettlementsAvailableException();
                }
                else
                { transaction.Commit(); }
                if (this.HasPendingPayments(cn, lessor_id))
                {
                    settlement.Message = string.Format("  Successful Payment Scheduled: {0}", settlement.ScheduledPaymentID);
                }
            }
            return this.GetPaidLessorSettlement(settlement.ScheduledPaymentID, true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lessor_ids"></param>
        /// <returns></returns>
        public IEnumerable<Lessor> GetLessors(string [] lessor_ids)
        {
            if( ( lessor_ids == null ) || (lessor_ids.Length == 0 ))
            {return  new List<Lessor>();   }
            var lessors = new List<Lessor>();
            var adapter = new OleDbDataAdapter();

            var lessors_query = string.Empty;
            var all_lessors = new StringBuilder();
            
            foreach( string this_lessor_id in lessor_ids )
            {
                
                all_lessors.Append("'");
                all_lessors.Append(this_lessor_id);
                all_lessors.Append("',");
            }

            var lessor_set = all_lessors.ToString();
            lessors_query = "SELECT L.cId AS Lessor_ID, L.cName AS Lessor_Name, L.cAddress AS Street_Address, L.cCity AS City, L.cState AS State, L.cZip AS Zip_Code, L.cPhone AS Phone FROM RsLessor AS L WHERE L.cId IN ( " + lessor_set.Substring(0, lessor_set.Length - 1) + ")  ORDER BY L.cId ";

            var data_set = new DataSet();

            using (var cn = this.Connection)
            {
                using (var cmd = cn.CreateCommand())
                {
                    cn.Open();

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = lessors_query;

                    adapter = new OleDbDataAdapter(cmd);
                    data_set = new DataSet();
                    adapter.Fill(data_set);

                }
            }

            int lessor_count_requested = lessor_ids.Count();

            foreach (DataRow lessor_record in data_set.Tables[0].Rows)
            {
                lessors.Add(
                    new Lessor
                    {
                        LessorId = lessor_record["Lessor_ID"].ToString(),
                        Name = lessor_record["Lessor_Name"].ToString(),
                        Address = new Address
                                    {
                                        StreetAddress = lessor_record["Street_Address"].ToString(),
                                        City = lessor_record["City"].ToString(),
                                        State = lessor_record["State"].ToString(),
                                        ZipCode = lessor_record["Zip_Code"].ToString(),
                                        Phone = lessor_record["Phone"].ToString()
                                    }
                    });
            }
            return lessors;
        }

        /// <summary>
        /// Only standard exceptions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Lessor> GetAllLessors()
        {
            var lessors = new List<Lessor>();
            var adapter = new OleDbDataAdapter();

            var lessors_query = "SELECT L.cId AS Lessor_ID, L.cName AS Lessor_Name, L.cAddress AS Street_Address, L.cCity AS City, L.cState AS State, L.cZip AS Zip_Code, L.cPhone AS Phone FROM RsLessor AS L  ORDER BY L.cId ";
            
            var data_set = new DataSet();

            using (var cn = this.Connection)
            {
                using (var cmd = cn.CreateCommand())
                {
                    cn.Open();

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = lessors_query;

                    adapter = new OleDbDataAdapter(cmd);
                    data_set = new DataSet();
                    adapter.Fill(data_set);

                    if ((data_set.Tables.Count == 0) || (data_set.Tables[0].Rows.Count == 0))
                    { return lessors; }
                }
            }

            foreach (DataRow lessor_record in data_set.Tables[0].Rows)
            {
                lessors.Add(
                    new Lessor
                    {
                        LessorId = lessor_record["Lessor_ID"].ToString(),
                        Name = lessor_record["Lessor_Name"].ToString(),
                        Address = new Address
                        {
                            StreetAddress = lessor_record["Street_Address"].ToString(),
                            City = lessor_record["City"].ToString(),
                            State = lessor_record["State"].ToString(),
                            ZipCode = lessor_record["Zip_Code"].ToString(),
                            Phone = lessor_record["Phone"].ToString()
                        }
                    });
            }
            return lessors;
        
        }

        /// <summary>
        /// UnpostedSettlementsPendingException
        /// NoSettlementsAvailableException
        /// Throws InvalidLessorIDException
        /// </summary>
        /// <param name="scheduled_payment_id"></param>
        /// <returns>null if no data found</returns>
        public PaidLessorSettlement GetPaidLessorSettlement(int scheduled_payment_id, bool include_scheduled_payments)
        {
            var lessor_settlement = new PaidLessorSettlement() { ScheduledPaymentID = scheduled_payment_id };
            var paid_trips_stored_procedure = "sp_RS_Get_Paid_Trips_For_Payment_ID";
            var is_payment_posted = false;
            include_scheduled_payments = true;
            var adapter = new OleDbDataAdapter();
            var data_set = new DataSet();
            string tractor_id = string.Empty;

            using (var cn = this.Connection)
            {
                cn.Open();
                is_payment_posted = this.IsPaymentPosted(cn, scheduled_payment_id);

                if (is_payment_posted && (!include_scheduled_payments))
                {
                    throw new UnpostedSettlementsPendingException();
                }
                lessor_settlement.State = is_payment_posted ? SettlementState.Paid : SettlementState.PendingPost;

                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = paid_trips_stored_procedure;
                    cmd.Parameters.AddWithValue("PaymentID", scheduled_payment_id);
                    adapter = new OleDbDataAdapter(cmd);
                    data_set = new DataSet();
                    var paid_trip_id = 0;
                    adapter.Fill(data_set);
                    
                    if (IsDataSetEmpty(data_set))
                    {
                        throw new NoSettlementsAvailableException();
                    }
                    
                    lessor_settlement.Lessor = this.GetLessor(cn, data_set.Tables[0].Rows[0]["Lessor_ID"].ToString());
                    lessor_settlement.ShceduledDate = this.ParseDate(data_set.Tables[0].Rows[0]["Accepted_Date"].ToString());
                    lessor_settlement.Tractor = new SettlementTractor { TractorID = data_set.Tables[0].Rows[0]["Tractor_ID"].ToString() };
                    
                    foreach (DataRow record in data_set.Tables[0].Rows)
                    {
                        paid_trip_id = int.Parse( record["Paid_Trip_ID"].ToString() );
                        var settlement_trip = new SettlementTrip 
                        { 
                            Number = record["Trip_Number"].ToString(),
                            ActivityDate = this.ParseDate(record["Invoice_Date"].ToString()),
                            RevenueItems = this.GetPaidTripRevenueDetail(cn, paid_trip_id),
                            InvoiceDate = this.ParseDate(record["Invoice_Date"].ToString()),
                            Deductions = this.GetPaidTripDeductions(cn, paid_trip_id),
                            IsSelectedForPayment = true
                        };

                        lessor_settlement.Trips.Add(settlement_trip);
                    }
                    lessor_settlement.Advances = this.GetPaidLessorAdvances(cn, scheduled_payment_id);
                }
            }
            
            return lessor_settlement;    
        }

        public IEnumerable<PaidLessorSettlementListItem> GetPaidLessorSettlements(string lessor_id)
        {
            var paid_settlements = new LinkedList<PaidLessorSettlementListItem>();
            var stored_procedure = "sp_Rs_Get_Paid_Lessor_Settlements";
            var adapter = new OleDbDataAdapter();
            var data_set = new DataSet();
            string tractor_id = string.Empty;

            using (var cn = this.Connection)
            {
                cn.Open();
                var lessor = this.GetLessor(cn, lessor_id);
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = stored_procedure;
                    cmd.Parameters.AddWithValue("LessorID", lessor_id);
                    adapter = new OleDbDataAdapter(cmd);
                    data_set = new DataSet();
                    adapter.Fill(data_set);                
                }
                if( IsDataSetEmpty( data_set ))
                {
                     throw new NoSettlementsAvailableException();
                }
                foreach (DataRow record in data_set.Tables[0].Rows)
                {
                    paid_settlements.AddLast(new PaidLessorSettlementListItem
                    {
                        ScheduledPaymentId = int.Parse( record["Payment_ID"].ToString() ),
                        AcceptedDate = this.ParseDate( record["Accepted_Date"].ToString() ),
                        InvoiceDate = this.ParseDate(record["Invoice_Date"].ToString()),
                        State = this.ParseState(record["IsPending"].ToString()),
                        Total = this.ParseDecimal(record["Total"].ToString())
                    });
                }
            }
            return paid_settlements;
        }

        #region Private Members

        private const string SETTLEMENT_CONNECTION_STRING_KEY = "SettlementsConnectionString";
        private const string DECIMAL_ZERO_STRING = "0.00";



        /// <summary>
        /// 
        /// </summary>
        /// <param name="cn"></param>
        /// <param name="trip_number"></param>
        /// <returns></returns>
        private SettlementTrip GetSettlementTrip(OleDbConnection cn, string trip_number)
        {

            var trip = new SettlementTrip
            {
                Number = trip_number,
                RevenueItems = this.GetTripRevenueItems(cn, trip_number),
                Deductions = this.GetTripDeductions(cn, trip_number)
            };

            return trip;
        }

        private List<Advance> GetLessorAdvances(OleDbConnection cn, string lessor_id, bool include_selected_for_payment)
        {
            var advances_stored_procedure = "sp_RS_Get_Lessor_Advances";
            var adapter = new OleDbDataAdapter();
            var data_set = new DataSet();
            var advance = new Advance();
            var advances = new List<Advance>();
            using (OleDbCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = advances_stored_procedure;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OleDbParameter("LessorID", lessor_id));

                if (cn.State != System.Data.ConnectionState.Open) { cn.Open(); }

                adapter = new OleDbDataAdapter(cmd);
                data_set = new DataSet();
                adapter.Fill(data_set);
            }
            string advance_date = string.Empty;
            DateTime activity_date = DateTime.MinValue;
            foreach (DataRow record in data_set.Tables[0].Rows)
            {
                advance_date = record["Advance_Date"].ToString().Trim();
                activity_date = DateTime.Parse(record["Activity_Date"].ToString());

                advance = new Advance
                {
                    TripNumber = record["Trip_Number"].ToString(),
                    ActivityDate = activity_date,
                    AdvanceDate = advance_date.Length == 0 ? activity_date.ToString("d") : this.ParseDate(advance_date).ToString("d"),
                    Reference = record["Reference"].ToString(),
                    Description = record["Description"].ToString(),
                    Amount = this.ParseDecimal(record["Advance_Amount"].ToString()),
                    IsSelectedForPayment = this.ParseMSAccessBolean( record["IsSelectedForPayment"].ToString() )
                };


                advances.Add(advance);
            }
            return advances;
        }

        private List<Advance> GetPaidLessorAdvances( OleDbConnection cn, int scheduled_payment_id)
        {
            var advances_stored_procedure = "sp_RS_Get_Paid_Lessor_Advances";
            var adapter = new OleDbDataAdapter();
            var data_set = new DataSet();
            var advance = new Advance();
            var advances = new List<Advance>();
            using (OleDbCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = advances_stored_procedure;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OleDbParameter("ForScheduledPaymentID", scheduled_payment_id));

                if (cn.State != System.Data.ConnectionState.Open) { cn.Open(); }

                adapter = new OleDbDataAdapter(cmd);
                data_set = new DataSet();
                adapter.Fill(data_set);
            }
            string advance_date = string.Empty;
            DateTime activity_date = DateTime.MinValue;
            foreach (DataRow record in data_set.Tables[0].Rows)
            {
                advance_date = record["Advance_Date"].ToString().Trim();
                activity_date = DateTime.Parse(record["Activity_Date"].ToString());

                advance = new Advance
                {
                    TripNumber = record["Trip_Number"].ToString(),
                    ActivityDate = activity_date,
                    AdvanceDate = advance_date.Length == 0 ? activity_date.ToString("d") : this.ParseDate(advance_date).ToString("d"),
                    Reference = record["Reference"].ToString(),
                    Description = record["Description"].ToString(),
                    Amount = this.ParseDecimal(record["Advance_Amount"].ToString())
                };
                
                advances.Add(advance);
            }
            return advances;
        }


        private List<TripRevenueItem> GetTripRevenueItems(OleDbConnection cn, string trip_number)
        {
            var stored_procedure = "sp_RS_Get_Trip_Revenue";
            var rev_items = new List<TripRevenueItem>();
            var adapter = new OleDbDataAdapter();
            var data_set = new DataSet();
            var rev_item = new TripRevenueItem();

            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = stored_procedure;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new OleDbParameter("TractorID", trip_number));

                if (cn.State != System.Data.ConnectionState.Open) { cn.Open(); }

                adapter = new OleDbDataAdapter(cmd);
                data_set = new DataSet();
                adapter.Fill(data_set);
            }

            if (data_set.Tables.Count == 0)
            { return rev_items; }

            foreach (DataRow record in data_set.Tables[0].Rows)
            {
                rev_items.Add(new TripRevenueItem
                {
                    ActivityDate = DateTime.Parse(record["Activity_Date"].ToString()),
                    Description = record["Trip_Description"].ToString(),
                    RevenueTotals = this.ParseDecimal(record["Revenue_Totals"].ToString())
                });
            }
            return rev_items;
        }

        private List<Deduction> GetTripDeductions(OleDbConnection cn, string trip_number)
        {

            var stored_procedure = "sp_RS_Get_Trip_Deductions";
            var deductions = new List<Deduction>();
            var adapter = new OleDbDataAdapter();
            var data_set = new DataSet();
            var deduction = new Deduction();

            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = stored_procedure;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new OleDbParameter("TripNumber", trip_number));

                if (cn.State != System.Data.ConnectionState.Open) { cn.Open(); }

                adapter = new OleDbDataAdapter(cmd);
                data_set = new DataSet();
                adapter.Fill(data_set);
            }

            if (data_set.Tables.Count == 0)
            { return deductions; }

            foreach (DataRow record in data_set.Tables[0].Rows)
            {

                deduction = new Deduction
                {
                    Description = record["Description"].ToString(),
                    Rate = this.ParseDecimal(record["Rate"].ToString()),
                    Amount = this.ParseDecimal(record["Amount"].ToString())
                };

                if (this.ParseDecimal(record["Total_Deduction"].ToString()) != deduction.TotalDeduction)
                {
                    string message = string.Format("INSERT INTO DEDUCTION_ERROR_LOG (Comments) VALUES ('There was a Total_Deduction Error adding deduction in Trip: {0}  Description: {1} Rate {2} Amount {3} Total Calculated {4} Total in Table: {5}' )",
                        trip_number, deduction.Description, deduction.Rate.ToString(), deduction.Amount.ToString(), deduction.TotalDeduction.ToString(), record["Total_Deduction"].ToString());
                    OleDbCommand cmd = cn.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = message;
                }
                deductions.Add(deduction);
            }

            return deductions;
        }

        /// <summary>
        /// Throws InvalidLessorIDException
        /// </summary>
        /// <param name="cn"></param>
        /// <param name="lessor_id"></param>
        /// <returns></returns>
        private Lessor GetLessor(OleDbConnection cn, string lessor_id)
        {
            var lessor_store_procedure = "sp_RS_Get_Lessor";
            var adapter = new OleDbDataAdapter();
            var data_set = new DataSet();

            using (OleDbCommand cmd = cn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = lessor_store_procedure;

                cmd.Parameters.Add(new OleDbParameter("LessorID", lessor_id));
                if (cn.State != ConnectionState.Open) { cn.Open(); }
                adapter = new OleDbDataAdapter(cmd);
                adapter.Fill(data_set);
            }
            if (IsDataSetEmpty( data_set ))
            {
                throw new InvalidLessorIDException();
            }
            DataRow record = data_set.Tables[0].Rows[0];
            return new Lessor()
            {
                LessorId = lessor_id,
                Name = record["Lessor_Name"].ToString(),
                Address = new Address
                {
                    StreetAddress = record["Street_Address"].ToString(),
                    City = record["City"].ToString(),
                    State = record["State"].ToString(),
                    ZipCode = record["Zip_Code"].ToString(),
                    Phone = record["Phone"].ToString()
                }
            };
        }

        private IEnumerable<Deduction> GetPaidTripDeductions(OleDbConnection cn, int paid_trip_id)
        {
            var stored_procedure = "sp_RS_Get_Paid_Trip_Deductions";
            var deductions = new List<Deduction>();
            var adapter = new OleDbDataAdapter();
            var data_set = new DataSet();
            var deduction = new Deduction();
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = stored_procedure;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("PaidTripID", paid_trip_id);

                if (cn.State != System.Data.ConnectionState.Open) { cn.Open(); }

                adapter = new OleDbDataAdapter(cmd);
                data_set = new DataSet();
                adapter.Fill(data_set);
            }
            
            if (data_set.Tables.Count == 0)
            { 
              return deductions;
            }

            foreach (DataRow record in data_set.Tables[0].Rows)
            {

                deduction = new Deduction
                {
                    Description = record["Description"].ToString(),
                    Rate = this.ParseDecimal(record["Rate"].ToString()),
                    Amount = this.ParseDecimal(record["Amount"].ToString())
                };

                deductions.Add(deduction);
            }

            return deductions;
        }

        // ready to test
        private IEnumerable<TripRevenueItem> GetPaidTripRevenueDetail(OleDbConnection cn, int paid_trip_id )
        {
            var stored_procedure = "sp_RS_Get_Paid_Trip_Revenue";
            var rev_items = new List<TripRevenueItem>();
            var adapter = new OleDbDataAdapter();
            var data_set = new DataSet();
            var rev_item = new TripRevenueItem();

            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = stored_procedure;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new OleDbParameter("PaidTripID", paid_trip_id));

                if (cn.State != System.Data.ConnectionState.Open) { cn.Open(); }

                adapter = new OleDbDataAdapter(cmd);
                data_set = new DataSet();
                adapter.Fill(data_set);
            }

            if (data_set.Tables.Count == 0)
            { return rev_items; }

            foreach (DataRow record in data_set.Tables[0].Rows)
            {
                rev_items.Add(new TripRevenueItem
                {
                    ActivityDate = DateTime.Parse(record["Activity_Date"].ToString()),
                    Description = record["Description"].ToString(),
                    RevenueTotals = this.ParseDecimal(record["Revenue_Total"].ToString())
                });
            }
            return rev_items;
        }

        private void AppendAdvancesForLessor(string lessor_id, int scheduled_payment_id, OleDbConnection cn, OleDbTransaction transaction)
        {
            var append_advances_to_payment_stored_procedure = "sp_Rs_Paid_Advance_Append";
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = append_advances_to_payment_stored_procedure;
                cmd.Parameters.AddWithValue("ForScheduledPaymentID", scheduled_payment_id);
                cmd.Parameters.AddWithValue("LessorID", lessor_id);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
            }
        }

        private int AppendTripsForLessor(string lessor_id, int scheduled_payment_id, OleDbConnection cn, OleDbTransaction transaction)
        {
            var append_trips_to_payment_stored_procedure = "sp_Rs_Paid_Trip_Append";
            var rows_affected = -1;
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = append_trips_to_payment_stored_procedure;
                cmd.Parameters.AddWithValue("LessorID", lessor_id);
                cmd.Parameters.AddWithValue("ForScheduledPaymentID", scheduled_payment_id);
                cmd.Transaction = transaction;
                rows_affected = cmd.ExecuteNonQuery();
            }
            AppendRevenueDetailsForTrips(scheduled_payment_id, cn, transaction);
            AppendDeductionsForTrips(scheduled_payment_id, cn, transaction);
            return rows_affected;
        }

        private void AppendDeductionsForTrips(int paid_trip_id, OleDbConnection cn, OleDbTransaction transaction)
        {
            var append_trips_deductions_to_payment_stored_procedure = "sp_Rs_Paid_Deduction_Append";
            var cmd = cn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = append_trips_deductions_to_payment_stored_procedure;
            cmd.Parameters.AddWithValue("ForScheduledPaymentID", paid_trip_id);
            cmd.Transaction = transaction;
            var rows_affected = cmd.ExecuteNonQuery();
        }

        private void AppendRevenueDetailsForTrips(int paid_trip_id, OleDbConnection cn, OleDbTransaction transaction)
        {
            var append_trip_details_to_payment_stored_procedure = "sp_Rs_Paid_Trip_Revenue_Detail_Append";
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = append_trip_details_to_payment_stored_procedure;
                cmd.Parameters.AddWithValue("ForScheduledPaymentID", paid_trip_id);
                cmd.Transaction = transaction;
                var rows_affected = cmd.ExecuteNonQuery();
            }
        }

        private int AddPaymentScheduledAndGetId(string lessor_id, string ip_address, OleDbConnection cn, OleDbTransaction transaction)
        {
            var append_payment_scheduled_stored_procedure = "sp_Rs_Payment_Scheduled_Append";

            using (var append_payment_scheduled_cmd = cn.CreateCommand())
            {
                append_payment_scheduled_cmd.Transaction = transaction;
                append_payment_scheduled_cmd.CommandType = CommandType.StoredProcedure;
                append_payment_scheduled_cmd.CommandText = append_payment_scheduled_stored_procedure;
                append_payment_scheduled_cmd.Parameters.AddWithValue("LessorID", lessor_id);
                append_payment_scheduled_cmd.Parameters.AddWithValue("IPAddress", ip_address);
                append_payment_scheduled_cmd.ExecuteNonQuery();
            }
            return GetLastPaymentIDForLessor(cn, lessor_id, transaction);
        }

        private int GetLastPaymentIDForLessor(OleDbConnection cn, string lessor_id, OleDbTransaction transaction)
        {
            var get_last_scheduled_payment_id_for_lessor_stored_procedure = "sp_Rs_Payment_Schedule_Get_Last_ID";
            int payment_id = -1;
            var adapter = new OleDbDataAdapter();
            var data_set = new DataSet();
            using (var last_scheduled_lessor_payment_id_cmd = cn.CreateCommand())
            {
                last_scheduled_lessor_payment_id_cmd.CommandType = CommandType.StoredProcedure;
                last_scheduled_lessor_payment_id_cmd.CommandText = get_last_scheduled_payment_id_for_lessor_stored_procedure;
                last_scheduled_lessor_payment_id_cmd.Parameters.AddWithValue("LessorID", lessor_id);
                last_scheduled_lessor_payment_id_cmd.Transaction = transaction;
                adapter.SelectCommand = last_scheduled_lessor_payment_id_cmd;
                adapter.Fill(data_set);
            }

            if ((data_set.Tables.Count == 0) || (data_set.Tables[0].Rows.Count == 0))
            { return -1; }

            if (!int.TryParse(data_set.Tables[0].Rows[0]["Last_Shcheduled_Payment_ID"].ToString(), out payment_id))
            { return -1; }
            return payment_id;
        }

        private bool HasPendingPayments( OleDbConnection cn, string lessor_id )
        {
            bool has_payments_pending = false;
            var has_payments_not_posted_stored_procedure = "sp_Rs_Paid_Trips_Not_Posted_Exist_For_Lessor";
            var adapter = new OleDbDataAdapter();
            var data_set = new DataSet();
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = has_payments_not_posted_stored_procedure;
                cmd.Parameters.AddWithValue("LessorID", lessor_id);
                adapter.SelectCommand = cmd;
                adapter.Fill(data_set);
            }
            
            if (IsDataSetEmpty(data_set))
            { 
                throw new IndexOutOfRangeException(
                    string.Format("No Result From sp_Rs_Paid_Trips_Not_Posted_Exist_For_Lessor for {0}", lessor_id ));
            }
            
            string ms_access_bool = data_set.Tables[0].Rows[0]["Trips_Not_Posted_Exist"].ToString().Trim();
            if (ms_access_bool == "0") { has_payments_pending = false; }
            else if (ms_access_bool == "-1") { has_payments_pending = true; }
            else
            { 
             throw new InvalidCastException(
                    string.Format("Unable to retrieve boolean from sp_Rs_Paid_Trips_Not_Posted_Exist_For_Lessor for {0}", lessor_id ));
            }
            return has_payments_pending;
        }

        private bool IsPaymentPosted(OleDbConnection cn, int scheduled_payment_id)
        {
            var is_posted = false;
            var stored_procedure = "sp_Rs_Payment_Scheduled_Is_Posted";
            var adapter = new OleDbDataAdapter();
            var data_set = new DataSet();
            
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = stored_procedure;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("ForScheduledPaymentID", scheduled_payment_id);

                if (cn.State != System.Data.ConnectionState.Open) { cn.Open(); }

                adapter = new OleDbDataAdapter(cmd);
                data_set = new DataSet();
                adapter.Fill(data_set);
            }
            
            var record = data_set.Tables[0].Rows[0];
            is_posted = this.ParseMSAccessBolean(record["Answer"].ToString());// (bool_in_int != 0);
            return is_posted;
        }

        private decimal ParseDecimal(string decimal_string)
        {
            decimal dec = 0.0m;

            if (!decimal.TryParse(decimal_string, out dec))
            { return 0.0m;  }

            return dec;
        }

        private bool ParseMSAccessBolean(string boolean_string)
        {
            var int_boolean = 0;
            boolean_string = boolean_string.Trim();

            if (!int.TryParse(boolean_string, out int_boolean))
            {
                throw new InvalidCastException(
                    string.Format("Cannot convert '{0}' to boolean", boolean_string ));

            }
            var result_boolean = (int_boolean == 0 ? false : true);
            return result_boolean;
        }

        private SettlementState ParseState(string is_pending_string)
        {
            var is_pending = this.ParseMSAccessBolean(is_pending_string);
            return is_pending ? SettlementState.PendingPost : SettlementState.Paid;
        }

        private DateTime ParseDate(string date_string)
        {
            var parsed_date = DateTime.MinValue;
            if( ! DateTime.TryParse( date_string, out parsed_date ) )
            {return DateTime.MinValue;}
            return parsed_date;
        }

        private string ConnectionString
        {
            get
            {
              var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[SETTLEMENT_CONNECTION_STRING_KEY].ConnectionString;
              return (connectionString??string.Empty).ToString() ;
            }
        }

        private bool IsDataSetEmpty(DataSet data_set)
        {
            if (data_set == null)
            { return true; }
            if (data_set.Tables == null)
            { return true; }
            if (data_set.Tables.Count == 0)
            { return true; }
            if (data_set.Tables[0].Rows == null)
            { return true; }
            if (data_set.Tables[0].Rows.Count == 0)
            { return true; }
            return false;
        }

       

        private OleDbConnection Connection
        {
            get { return new OleDbConnection(this.ConnectionString); }
        }

        #endregion Private Members
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Models
{
    public class Subscription
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        //master
        public int Id { get; set; }
        public string Name { get; set; }
        public int Days { get; set; }
        public decimal Amount { get; set; }

        //customer subscription
        public int CustomerId { get; set; }
        public int SubscriptionId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string PaymentStatus { get; set; }
        public string SubscriptionStatus { get; set; }

        //wallet
        public decimal CreditAmount { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal TotalBalance { get; set; }
        public int OrderId { get; set; }
        public string BillNo { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
        public int? TransactionType { get; set; }

        //Order
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int BuildingId { get; set; }
        public int CustSubscriptionId { get; set; }
        public string StateCode { get; set; }
        public string CityCode { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public decimal TotalGSTAmt { get; set; }

        public int ProductId { get; set; }
        public int AttributeId { get; set; }
        public int VendorId { get; set; }
        public int VendorCatId { get; set; }
        public int Qty { get; set; }
        public decimal SalePrice { get; set; }
        
        public decimal PurchasePrice { get; set; }
        public decimal Discount { get; set; }
        public long RewardPoint { get; set; }
        public decimal RewardPoint1 { get; set; }
        public decimal TotalFinalAmount { get; set; }
        public decimal CGSTAmount { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal IGSTAmount { get; set; }
        public DateTime OrderItemDate { get; set; }
        public decimal CGSTPerct { get; set; }
        public decimal SGSTPerct { get; set; }
        public decimal IGSTPerct { get; set; }
        public decimal Profit { get; set; }
        public string OrderFlag { get; set; }
        public string week { get; set; }
        public decimal MRPPrice { get; set; }
        public string DeliveryBoyId { get; set; }
        //Cancel Subscription

        public DateTime cancelFD { get; set; }
        public DateTime cancelTD { get; set; }
        public string cancelBy { get; set; }
        public DateTime cancelDate { get; set; }
        public decimal walletbalance { get; set; }
        public string remark { get; set; }

        public decimal Refund { get; set; }

        //Break Between Subscriptions
        public DateTime sFdate { get; set; }
        public DateTime sTdate { get; set; }

        public string Cashbacktype { get; set; }
        public string CashbackId { get; set; }

        public string CashbackStatus { get; set; }
        public string CashbackAmount { get; set; }
        public DateTime CashBackDate { get; set; }
        public string UtransactionId { get; set; }

        public string proqty { get; set; }

        public string previous1 { get; set; }
        public string next1 { get; set; }


        public string startpoint { get; set; }
        public string endpoint { get; set; }


        public string Previousdate { get; set; }
        public string nextdate { get; set; }

        public int CashCollectionId { get; set; }
        public int SectorId { get; set; }

        public decimal GeneralPayAmount { get; set; }
        public string GeneralPayTransactionId { get; set; }
        public DataTable getDuplicateSubscription(string name)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Select * from tbl_Subcription_Master where Name='" + name + "'", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getSubscriptionList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Subscription_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerSubscriptionList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Customer_Subscription_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerwiseSubscriptionList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Customer_Subscription_Sigle", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@cID", Id);
            else
                cmd.Parameters.AddWithValue("@cID", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public int InsertSubscriptionMst(Subscription obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Subscription_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Name", obj.Name);
                if (!string.IsNullOrEmpty(obj.Days.ToString()))
                    com.Parameters.AddWithValue("@Days", obj.Days);
                else
                    com.Parameters.AddWithValue("@Days", 0);
                if (!string.IsNullOrEmpty(obj.Amount.ToString()))
                    com.Parameters.AddWithValue("@Amount", obj.Amount);
                else
                    com.Parameters.AddWithValue("@Amount", 0);
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                com.ExecuteNonQuery();
                i = Convert.ToInt32(com.Parameters["@Id"].Value);
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }
        public int UpdateSubscriptionMst(Subscription obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Subscription_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.Id);
                com.Parameters.AddWithValue("@Name", obj.Name);
                if (!string.IsNullOrEmpty(obj.Days.ToString()))
                    com.Parameters.AddWithValue("@Days", obj.Days);
                else
                    com.Parameters.AddWithValue("@Days", 0);
                if (!string.IsNullOrEmpty(obj.Amount.ToString()))
                    com.Parameters.AddWithValue("@Amount", obj.Amount);
                else
                    com.Parameters.AddWithValue("@Amount", 0);
                com.ExecuteNonQuery();
                i = Convert.ToInt32(com.Parameters["@Id"].Value);
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        //delete
        public int DeleteSubscription(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Subcription_Master where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int DeleteSubscriptionByCustomer(int CustomerId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Customer_Subscription where CustomerId=" + CustomerId, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int DeleteDeliveryBoyAssignByCustomer(int CustomerId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_DeliveryBoy_Customer_Assign where CustomerId=" + CustomerId, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int DeletePaytmRequestByCustomer(int CustomerId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Paytm_Request where CustomerId=" + CustomerId, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int InsertSubscription(Subscription obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Customer_Subscription_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                if (!string.IsNullOrEmpty(obj.SubscriptionId.ToString()))
                    com.Parameters.AddWithValue("@SubscriptionId", obj.SubscriptionId);
                else
                    com.Parameters.AddWithValue("@SubscriptionId", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.FromDate.ToString()))
                    com.Parameters.AddWithValue("@FromDate", obj.FromDate);
                else
                    com.Parameters.AddWithValue("@FromDate", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.ToDate.ToString()))
                    com.Parameters.AddWithValue("@ToDate", obj.ToDate);
                else
                    com.Parameters.AddWithValue("@ToDate", DBNull.Value);
                com.Parameters.AddWithValue("@Amount", obj.Amount);
                com.Parameters.AddWithValue("@PaymentStatus", obj.PaymentStatus);
                com.Parameters.AddWithValue("@SubscriptionStatus", obj.SubscriptionStatus);
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                com.ExecuteNonQuery();
                i = Convert.ToInt32(com.Parameters["@Id"].Value);
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int UpdateSubscription(Subscription obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Customer_Subscription_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.Id);
                com.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                if (!string.IsNullOrEmpty(obj.SubscriptionId.ToString()))
                    com.Parameters.AddWithValue("@SubscriptionId", obj.SubscriptionId);
                else
                    com.Parameters.AddWithValue("@SubscriptionId", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.FromDate.ToString()))
                    com.Parameters.AddWithValue("@FromDate", obj.FromDate);
                else
                    com.Parameters.AddWithValue("@FromDate", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.ToDate.ToString()))
                    com.Parameters.AddWithValue("@ToDate", obj.ToDate);
                else
                    com.Parameters.AddWithValue("@ToDate", DBNull.Value);
                com.Parameters.AddWithValue("@Amount", obj.Amount);

                com.Parameters.AddWithValue("@PaymentStatus", obj.PaymentStatus);
                // com.Parameters.AddWithValue("@SubscriptionStatus", obj.SubscriptionStatus);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public DataTable CheckCustSubnExits(int? custid, int? subid, DateTime? fdate, DateTime? tdate)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Check_Customer_Subscription_Select", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(custid.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", custid);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(subid.ToString()))
                cmd.Parameters.AddWithValue("@SubscriptionId", subid);
            else
                cmd.Parameters.AddWithValue("@SubscriptionId", DBNull.Value);
            if (!string.IsNullOrEmpty(fdate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", fdate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(tdate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", tdate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        //delete
        public int DeleteCustSubscription(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Customer_Subscription where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int InsertWallet(Subscription obj)
        {
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Wallet_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                com.Parameters.AddWithValue("@TransactionDate", indianTime);
                //if (!string.IsNullOrEmpty(obj.CreditAmount.ToString()))
                //    com.Parameters.AddWithValue("@CreditAmount", obj.CreditAmount);
                //else
                //    com.Parameters.AddWithValue("@CreditAmount", DBNull.Value);
                //if (!string.IsNullOrEmpty(obj.DebitAmount.ToString()))
                //    com.Parameters.AddWithValue("@DebitAmount", obj.DebitAmount);
                //else
                //    com.Parameters.AddWithValue("@DebitAmount", DBNull.Value);
                com.Parameters.AddWithValue("@Amount", obj.Amount);
                if (!string.IsNullOrEmpty(obj.OrderId.ToString()))
                    com.Parameters.AddWithValue("@OrderId", obj.OrderId);
                else
                    com.Parameters.AddWithValue("@OrderId", 0);
                if (!string.IsNullOrEmpty(obj.BillNo))
                    com.Parameters.AddWithValue("@BillNo", obj.BillNo);
                else
                    com.Parameters.AddWithValue("@BillNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Description))
                    com.Parameters.AddWithValue("@Description", obj.Description);
                else
                    com.Parameters.AddWithValue("@Description", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Type))
                    com.Parameters.AddWithValue("@Type", obj.Type);
                else
                    com.Parameters.AddWithValue("@Type", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.CustSubscriptionId.ToString()))
                    com.Parameters.AddWithValue("@CustSubscriptionId", obj.CustSubscriptionId);
                else
                    com.Parameters.AddWithValue("@CustSubscriptionId", 0);
                if (!string.IsNullOrEmpty(obj.TransactionType.ToString()))
                    com.Parameters.AddWithValue("@TransactionType", obj.TransactionType);
                else
                    com.Parameters.AddWithValue("@TransactionType", 0);




                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public int InsertWalletScheduleOrder(Subscription obj)
        {
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Wallet_Insert_Schedule_Order", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                com.Parameters.AddWithValue("@TransactionDate", indianTime);
                
                com.Parameters.AddWithValue("@Amount", obj.Amount);
                if (!string.IsNullOrEmpty(obj.OrderId.ToString()))
                    com.Parameters.AddWithValue("@OrderId", obj.OrderId);
                else
                    com.Parameters.AddWithValue("@OrderId", 0);
                if (!string.IsNullOrEmpty(obj.BillNo))
                    com.Parameters.AddWithValue("@BillNo", obj.BillNo);
                else
                    com.Parameters.AddWithValue("@BillNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Description))
                    com.Parameters.AddWithValue("@Description", obj.Description);
                else
                    com.Parameters.AddWithValue("@Description", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Type))
                    com.Parameters.AddWithValue("@Type", obj.Type);
                else
                    com.Parameters.AddWithValue("@Type", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.CustSubscriptionId.ToString()))
                    com.Parameters.AddWithValue("@CustSubscriptionId", obj.CustSubscriptionId);
                else
                    com.Parameters.AddWithValue("@CustSubscriptionId", 0);
                if (!string.IsNullOrEmpty(obj.TransactionType.ToString()))
                    com.Parameters.AddWithValue("@TransactionType", obj.TransactionType);
                else
                    com.Parameters.AddWithValue("@TransactionType", 0);

                if (!string.IsNullOrEmpty(obj.Status.ToString()))
                    com.Parameters.AddWithValue("@status", obj.Status);
                else
                    com.Parameters.AddWithValue("@status", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.proqty.ToString()))
                    com.Parameters.AddWithValue("@proqty", obj.proqty);
                else
                    com.Parameters.AddWithValue("@proqty", DBNull.Value);

                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public int InsertWalletScheduleOrder1(Subscription obj)
        {
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Wallet_Insert_Schedule_Order", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                com.Parameters.AddWithValue("@TransactionDate", obj.TransactionDate);

                com.Parameters.AddWithValue("@Amount", obj.Amount);
                if (!string.IsNullOrEmpty(obj.OrderId.ToString()))
                    com.Parameters.AddWithValue("@OrderId", obj.OrderId);
                else
                    com.Parameters.AddWithValue("@OrderId", 0);
                if (!string.IsNullOrEmpty(obj.BillNo))
                    com.Parameters.AddWithValue("@BillNo", obj.BillNo);
                else
                    com.Parameters.AddWithValue("@BillNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Description))
                    com.Parameters.AddWithValue("@Description", obj.Description);
                else
                    com.Parameters.AddWithValue("@Description", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Type))
                    com.Parameters.AddWithValue("@Type", obj.Type);
                else
                    com.Parameters.AddWithValue("@Type", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.CustSubscriptionId.ToString()))
                    com.Parameters.AddWithValue("@CustSubscriptionId", obj.CustSubscriptionId);
                else
                    com.Parameters.AddWithValue("@CustSubscriptionId", 0);
                if (!string.IsNullOrEmpty(obj.TransactionType.ToString()))
                    com.Parameters.AddWithValue("@TransactionType", obj.TransactionType);
                else
                    com.Parameters.AddWithValue("@TransactionType", 0);

                if (!string.IsNullOrEmpty(obj.Status.ToString()))
                    com.Parameters.AddWithValue("@status", obj.Status);
                else
                    com.Parameters.AddWithValue("@status", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.proqty.ToString()))
                    com.Parameters.AddWithValue("@proqty", obj.proqty);
                else
                    com.Parameters.AddWithValue("@proqty", DBNull.Value);

                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int InsertWallet1(Subscription obj)
        {
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Wallet_Insert1", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                com.Parameters.AddWithValue("@TransactionDate", indianTime);
               
                com.Parameters.AddWithValue("@Amount", obj.Amount);
                if (!string.IsNullOrEmpty(obj.OrderId.ToString()))
                    com.Parameters.AddWithValue("@OrderId", obj.OrderId);
                else
                    com.Parameters.AddWithValue("@OrderId", 0);
                if (!string.IsNullOrEmpty(obj.BillNo))
                    com.Parameters.AddWithValue("@BillNo", obj.BillNo);
                else
                    com.Parameters.AddWithValue("@BillNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Description))
                    com.Parameters.AddWithValue("@Description", obj.Description);
                else
                    com.Parameters.AddWithValue("@Description", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Type))
                    com.Parameters.AddWithValue("@Type", obj.Type);
                else
                    com.Parameters.AddWithValue("@Type", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.CustSubscriptionId.ToString()))
                    com.Parameters.AddWithValue("@CustSubscriptionId", obj.CustSubscriptionId);
                else
                    com.Parameters.AddWithValue("@CustSubscriptionId", 0);
                if (!string.IsNullOrEmpty(obj.TransactionType.ToString()))
                    com.Parameters.AddWithValue("@TransactionType", obj.TransactionType);
                else
                    com.Parameters.AddWithValue("@TransactionType", 0);

                if (!string.IsNullOrEmpty(obj.Status))
                    com.Parameters.AddWithValue("@status", obj.Status);
                else
                    com.Parameters.AddWithValue("@status", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.Cashbacktype))
                    com.Parameters.AddWithValue("@Cashbacktype", obj.Cashbacktype);
                else
                    com.Parameters.AddWithValue("@Cashbacktype", DBNull.Value);



                if (!string.IsNullOrEmpty(obj.CashbackId))
                    com.Parameters.AddWithValue("@CashbackId", obj.CashbackId);
                else
                    com.Parameters.AddWithValue("@CashbackId", DBNull.Value);



                if (!string.IsNullOrEmpty(obj.UtransactionId))
                    com.Parameters.AddWithValue("@UtransactionId", obj.UtransactionId);
                else
                    com.Parameters.AddWithValue("@UtransactionId", DBNull.Value);

                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public int InsertWalletCashCollection(Subscription obj)
        {
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Wallet_InsertCashCollection", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                com.Parameters.AddWithValue("@TransactionDate", obj.TransactionDate);

                com.Parameters.AddWithValue("@Amount", obj.Amount);
                if (!string.IsNullOrEmpty(obj.OrderId.ToString()))
                    com.Parameters.AddWithValue("@OrderId", obj.OrderId);
                else
                    com.Parameters.AddWithValue("@OrderId", 0);
                if (!string.IsNullOrEmpty(obj.BillNo))
                    com.Parameters.AddWithValue("@BillNo", obj.BillNo);
                else
                    com.Parameters.AddWithValue("@BillNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Description))
                    com.Parameters.AddWithValue("@Description", obj.Description);
                else
                    com.Parameters.AddWithValue("@Description", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Type))
                    com.Parameters.AddWithValue("@Type", obj.Type);
                else
                    com.Parameters.AddWithValue("@Type", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.CustSubscriptionId.ToString()))
                    com.Parameters.AddWithValue("@CustSubscriptionId", obj.CustSubscriptionId);
                else
                    com.Parameters.AddWithValue("@CustSubscriptionId", 0);
                if (!string.IsNullOrEmpty(obj.TransactionType.ToString()))
                    com.Parameters.AddWithValue("@TransactionType", obj.TransactionType);
                else
                    com.Parameters.AddWithValue("@TransactionType", 0);

                if (!string.IsNullOrEmpty(obj.Status))
                    com.Parameters.AddWithValue("@status", obj.Status);
                else
                    com.Parameters.AddWithValue("@status", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.Cashbacktype))
                    com.Parameters.AddWithValue("@Cashbacktype", obj.Cashbacktype);
                else
                    com.Parameters.AddWithValue("@Cashbacktype", DBNull.Value);



                if (!string.IsNullOrEmpty(obj.CashbackId))
                    com.Parameters.AddWithValue("@CashbackId", obj.CashbackId);
                else
                    com.Parameters.AddWithValue("@CashbackId", DBNull.Value);



                if (!string.IsNullOrEmpty(obj.UtransactionId))
                    com.Parameters.AddWithValue("@UtransactionId", obj.UtransactionId);
                else
                    com.Parameters.AddWithValue("@UtransactionId", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.CashCollectionId.ToString()))
                    com.Parameters.AddWithValue("@CashCollectionID", obj.CashCollectionId);
                else
                    com.Parameters.AddWithValue("@CashCollectionID", 0);

                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public int UpdateCustomerWalletCashCollection(Subscription obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Customer_Wallet_Update_CashCollection", con);
                com.CommandType = CommandType.StoredProcedure;
                
                com.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                if (!string.IsNullOrEmpty(obj.Amount.ToString()))
                    com.Parameters.AddWithValue("@TotalFinalAmount", obj.Amount);
                else
                    com.Parameters.AddWithValue("@TotalFinalAmount", 0);
                if (!string.IsNullOrEmpty(obj.CashCollectionId.ToString()))
                    com.Parameters.AddWithValue("@CashCollectionID", obj.CashCollectionId);
                else
                    com.Parameters.AddWithValue("@CashCollectionID", 0);
                
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int InsertWalletMobile(Subscription obj)
        {
            // DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Wallet_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                com.Parameters.AddWithValue("@TransactionDate", indianTime);

                com.Parameters.AddWithValue("@Amount", obj.Amount);
                if (!string.IsNullOrEmpty(obj.OrderId.ToString()))
                    com.Parameters.AddWithValue("@OrderId", obj.OrderId);
                else
                    com.Parameters.AddWithValue("@OrderId", 0);
                if (!string.IsNullOrEmpty(obj.BillNo))
                    com.Parameters.AddWithValue("@BillNo", obj.BillNo);
                else
                    com.Parameters.AddWithValue("@BillNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Description))
                    com.Parameters.AddWithValue("@Description", obj.Description);
                else
                    com.Parameters.AddWithValue("@Description", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Type))
                    com.Parameters.AddWithValue("@Type", obj.Type);
                else
                    com.Parameters.AddWithValue("@Type", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.CustSubscriptionId.ToString()))
                    com.Parameters.AddWithValue("@CustSubscriptionId", obj.CustSubscriptionId);
                else
                    com.Parameters.AddWithValue("@CustSubscriptionId", 0);
                if (!string.IsNullOrEmpty(obj.TransactionDate.ToString()))
                    com.Parameters.AddWithValue("@TransactionType", obj.TransactionType);
                else
                    com.Parameters.AddWithValue("@TransactionType", 0);
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public DataTable getWalletSelect(int Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Wallet_Select", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerWallet(int Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Customer_Wallet_Select", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", Id);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public decimal GetCustomerBalace(int Id)
        {
            decimal credit = 0, debit = 0;
            var dt = getCustomerWallet(Id);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[i]["Type"].ToString()))
                    {
                        string Type = dt.Rows[i]["Type"].ToString();
                        if (Type == "Credit")
                            credit = Convert.ToDecimal(dt.Rows[i]["Amt"]);
                        else if (Type == "Debit")
                            debit = Convert.ToDecimal(dt.Rows[i]["Amt"]);
                    }
                }
            }
            return (credit - debit);
        }

        //delete
        public int DeleteCustomerWallet(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from [tbl_Customer_Wallet] where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int DeleteCustomerWalletByCustomer(int CustomerId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from [tbl_Customer_Wallet] where CustomerId=" + CustomerId, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int DeleteCustomerOTPByCustomer(string MobileNo)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from [tbl_Customer_OTP] where MobileNo='" + MobileNo + "'", con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public long InsertCustomerOrder(Subscription obj)
        {
            long i = 0;
            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();
                SqlCommand com = new SqlCommand("SP_Customer_Order_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                //com.Parameters.AddWithValue("@SoceityId", obj.BuildingId);                
                //com.Parameters.AddWithValue("@SoceityId", 0);
                if (!string.IsNullOrEmpty(obj.OrderNo.ToString()))
                    com.Parameters.AddWithValue("@OrderNo", obj.OrderNo);
                else
                    com.Parameters.AddWithValue("@OrderNo", 0);
                if (!string.IsNullOrEmpty(obj.OrderDate.ToString()))
                    com.Parameters.AddWithValue("@OrderDate", obj.OrderDate);
                else
                    com.Parameters.AddWithValue("@OrderDate", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.TotalAmount.ToString()))
                    com.Parameters.AddWithValue("@TotalAmount", obj.TotalAmount);
                else
                    com.Parameters.AddWithValue("@TotalAmount", 0);
                if (!string.IsNullOrEmpty(obj.Status))
                    com.Parameters.AddWithValue("@Status", obj.Status);
                else
                    com.Parameters.AddWithValue("@Status", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.CustSubscriptionId.ToString()))
                    com.Parameters.AddWithValue("@CustSubscriptionId", obj.CustSubscriptionId);
                else
                    com.Parameters.AddWithValue("@CustSubscriptionId", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.StateCode))
                    com.Parameters.AddWithValue("@StateCode", obj.StateCode);
                else
                    com.Parameters.AddWithValue("@StateCode", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.TotalGSTAmt.ToString()))
                    com.Parameters.AddWithValue("@TotalGSTAmt", obj.TotalGSTAmt);
                else
                    com.Parameters.AddWithValue("@TotalGSTAmt", DBNull.Value);
                com.Parameters.AddWithValue("@OrderFlag", obj.OrderFlag);
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                com.ExecuteNonQuery();
                i = Convert.ToInt64(com.Parameters["@Id"].Value);
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int DeleteTransactionOrder(long? id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from [tbl_Customer_Order_Transaction] where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int InsertCustomerOrderDetail(Subscription obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Customer_Order_Detail_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@OrderId", obj.OrderId);
                com.Parameters.AddWithValue("@ProductId", obj.ProductId);
                if (!string.IsNullOrEmpty(obj.Qty.ToString()))
                    com.Parameters.AddWithValue("@Qty", obj.Qty);
                else
                    com.Parameters.AddWithValue("@Qty", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.SalePrice.ToString()))
                    com.Parameters.AddWithValue("@SalePrice", obj.SalePrice);
                else
                    com.Parameters.AddWithValue("@SalePrice", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.Amount.ToString()))
                    com.Parameters.AddWithValue("@Amount", obj.Amount);
                else
                    com.Parameters.AddWithValue("@Amount", 0);

                if (!string.IsNullOrEmpty(obj.MRPPrice.ToString()))
                    com.Parameters.AddWithValue("@Mrp", obj.MRPPrice);
                else
                    com.Parameters.AddWithValue("@Mrp", 0);

                if (!string.IsNullOrEmpty(obj.PurchasePrice.ToString()))
                    com.Parameters.AddWithValue("@PurchasePrice", obj.PurchasePrice);
                else
                    com.Parameters.AddWithValue("@PurchasePrice", 0);


                if (!string.IsNullOrEmpty(obj.Discount.ToString()))
                    com.Parameters.AddWithValue("@Discount", obj.Discount);
                else
                    com.Parameters.AddWithValue("@Discount", 0);
                if (!string.IsNullOrEmpty(obj.RewardPoint.ToString()))
                    com.Parameters.AddWithValue("@RewardPoint", obj.RewardPoint);
                else
                    com.Parameters.AddWithValue("@RewardPoint", 0);
                if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                    com.Parameters.AddWithValue("@TotalFinalAmount", obj.TotalFinalAmount);
                else
                    com.Parameters.AddWithValue("@TotalFinalAmount", 0);
                if (!string.IsNullOrEmpty(obj.CGSTAmount.ToString()))
                    com.Parameters.AddWithValue("@CGSTAmount", obj.CGSTAmount);
                else
                    com.Parameters.AddWithValue("@CGSTAmount", 0);
                if (!string.IsNullOrEmpty(obj.SGSTAmount.ToString()))
                    com.Parameters.AddWithValue("@SGSTAmount", obj.SGSTAmount);
                else
                    com.Parameters.AddWithValue("@SGSTAmount", 0);
                if (!string.IsNullOrEmpty(obj.IGSTAmount.ToString()))
                    com.Parameters.AddWithValue("@IGSTAmount", obj.IGSTAmount);
                else
                    com.Parameters.AddWithValue("@IGSTAmount", 0);
                if (!string.IsNullOrEmpty(obj.OrderItemDate.ToString()))
                    com.Parameters.AddWithValue("@OrderItemdate", obj.OrderItemDate);
                else
                    com.Parameters.AddWithValue("@OrderItemdate", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Profit.ToString()))
                    com.Parameters.AddWithValue("@Profit", obj.Profit);
                else
                    com.Parameters.AddWithValue("@Profit", 0);


                if (!string.IsNullOrEmpty(obj.AttributeId.ToString()))
                    com.Parameters.AddWithValue("@AttributeId", obj.AttributeId);
                else
                    com.Parameters.AddWithValue("@AttributeId", 0);


                if (!string.IsNullOrEmpty(obj.VendorId.ToString()))
                    com.Parameters.AddWithValue("@VendorId", obj.VendorId);
                else
                    com.Parameters.AddWithValue("@VendorId", 0);

                if (!string.IsNullOrEmpty(obj.VendorCatId.ToString()))
                    com.Parameters.AddWithValue("@VendorCatId", obj.VendorCatId);
                else
                    com.Parameters.AddWithValue("@VendorCatId", 0);

                if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    com.Parameters.AddWithValue("@SectorId", obj.SectorId);
                else
                    com.Parameters.AddWithValue("@SectorId", 0);


                if (!string.IsNullOrEmpty(obj.DeliveryBoyId))
                    com.Parameters.AddWithValue("@DeliveryBoyId", obj.DeliveryBoyId);
                else
                    com.Parameters.AddWithValue("@DeliveryBoyId", 0);

                
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public DataTable getCustomerOrder(int custId, int SubscriptionId, DateTime odate)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Customer_Date_Order_Select", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(custId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", custId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(SubscriptionId.ToString()))
                cmd.Parameters.AddWithValue("@SubscriptionId", SubscriptionId);
            else
                cmd.Parameters.AddWithValue("@SubscriptionId", DBNull.Value);
            if (!string.IsNullOrEmpty(odate.ToString()))
                cmd.Parameters.AddWithValue("@EntryDate", odate);
            else
                cmd.Parameters.AddWithValue("@EntryDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public int UpdateCustomerOrder(Subscription obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Customer_Order_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.Id);
                com.Parameters.AddWithValue("@TotalAmount", obj.TotalAmount);
                if (!string.IsNullOrEmpty(obj.TotalGSTAmt.ToString()))
                    com.Parameters.AddWithValue("@TotalGSTAmt", obj.TotalGSTAmt);
                else
                    com.Parameters.AddWithValue("@TotalGSTAmt", 0);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public DataTable GetCustomerCredit(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SElect Credit From tbl_Customer_Master where Id=@Id", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
        public int UpdateCustomerOrderCancle(int? OId, int CustomerId, DateTime OrderDate, string Status)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Customer_Order_Status_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                if (!string.IsNullOrEmpty(OId.ToString()))
                    com.Parameters.AddWithValue("@OId", OId);
                else
                    com.Parameters.AddWithValue("@OId", DBNull.Value);
                com.Parameters.AddWithValue("@CustomerId", CustomerId);
                com.Parameters.AddWithValue("@Status", Status);
                if (!string.IsNullOrEmpty(OrderDate.ToString()))
                    com.Parameters.AddWithValue("@OrderDate", OrderDate);
                else
                    com.Parameters.AddWithValue("@OrderDate", 0);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public DataTable getCustomerWalletTrans(int CustomerId, DateTime? Fromdate, DateTime? Todate)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Wallet_Customer_transaction", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(Fromdate.ToString()))
                cmd.Parameters.AddWithValue("@Fromdate", Fromdate);
            else
                cmd.Parameters.AddWithValue("@Fromdate", DBNull.Value);
            if (!string.IsNullOrEmpty(Todate.ToString()))
                cmd.Parameters.AddWithValue("@Todate", Todate);
            else
                cmd.Parameters.AddWithValue("@Todate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getDateCustomerWalletTrans(int? CustomerId, DateTime? Transdate)
        {
            //con.Open();
            // SqlCommand cmd = new SqlCommand("Customer_Wallet_DateWise_Select", con);

            string query = "SELECT cw.Id,cw.CustomerId,cw.TransactionDate,Convert(varchar,TransactionDate,23) as Tdate,case when cw.[Type]='Credit' then cw.Amount else 0 end as CreditAmt,";
            query += " case when cw.[Type]='Debit' then cw.Amount else 0 end as DebitAmt,ot.OrderNo,Convert(varchar,ot.OrderDate,23) as OrderDate,cw.BillNo,cw.[Description],cw.[Type],cw.Amount,cw.CustSubscriptionId,cw.proqty AS Qty,";
            query += " cw.Cashbacktype AS Cashbacktype,cw.status AS cashbackstatus1,sm.Name as Subscription,cm.FirstName + ' '+ cm.LastName as Customer ";
            query += " from [dbo].[tbl_Customer_Wallet] cw left join [dbo].[tbl_Customer_Order_Transaction] ot on ot.Id = cw.OrderId ";
            query += " left join [dbo].[tbl_Customer_Subscription] cs on cs.Id = cw.CustSubscriptionId left join [dbo].[tbl_Subcription_Master] sm on sm.Id = cs.SubscriptionId";
            query += " left join tbl_Customer_Master cm on cm.Id = cw.CustomerId WHERE (@CustomerId IS NULL OR cw.[CustomerId] = @CustomerId) ";
            query += " and (@TransactionDate is null or Convert(varchar,TransactionDate,23)=@TransactionDate)";


            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(Transdate.ToString()))
                cmd.Parameters.AddWithValue("@TransactionDate", Transdate);
            else
                cmd.Parameters.AddWithValue("@TransactionDate", DBNull.Value);
            //if (!string.IsNullOrEmpty(Todate.ToString()))
            //    cmd.Parameters.AddWithValue("@Todate", Todate);
            //else
            //    cmd.Parameters.AddWithValue("@Todate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getDateCustomerWalletTransAll(int? CustomerId, DateTime? FromDate, DateTime? ToDate)
        {
            if (CustomerId == 0) CustomerId = null;
            con.Open();
            SqlCommand cmd = new SqlCommand("Customer_Wallet_DateWise_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(FromDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FromDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(ToDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", ToDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable getCustomerOrderList(string Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Customer_Order_List", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            //if (!string.IsNullOrEmpty(Fdate.ToString()))
            //    cmd.Parameters.AddWithValue("@FromDate", Fdate);
            //else
            //    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            //if (!string.IsNullOrEmpty(Tdate.ToString()))
            //    cmd.Parameters.AddWithValue("@ToDate", Tdate);
            //else
            //    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable getCustomerOrderSelect(string Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Customer_Order_Select", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            //if (!string.IsNullOrEmpty(Fdate.ToString()))
            //    cmd.Parameters.AddWithValue("@FromDate", Fdate);
            //else
            //    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            //if (!string.IsNullOrEmpty(Tdate.ToString()))
            //    cmd.Parameters.AddWithValue("@ToDate", Tdate);
            //else
            //    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public DataTable getCustomerOrderwalletSelect(string Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT convert(varchar,TransactionDate,23) as TransactionDate From tbl_Customer_Wallet where OrderId=@Id", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            //if (!string.IsNullOrEmpty(Fdate.ToString()))
            //    cmd.Parameters.AddWithValue("@FromDate", Fdate);
            //else
            //    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            //if (!string.IsNullOrEmpty(Tdate.ToString()))
            //    cmd.Parameters.AddWithValue("@ToDate", Tdate);
            //else
            //    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerOrderScheduleList(int? CustomerId, DateTime? orderDate)
        {
            //con.Open();
            DateTime OrderDate = orderDate.Value;
            // DateTime SubTo = Convert.ToDateTime("2020-12-27 00:00:00");
            //DateTime OrderDate = DateTime.Now.AddDays(1);
            var date = OrderDate.Date;
            //SqlCommand cmd = new SqlCommand("Customer_Order_Schedule_List", con);
            SqlCommand cmd = new SqlCommand("Customer_Order_Schedule_Listnew", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(date.ToString()))
                cmd.Parameters.AddWithValue("@OrderDate", date);
            else
                cmd.Parameters.AddWithValue("@OrderDate", DBNull.Value);
            //if (!string.IsNullOrEmpty(Fdate.ToString()))
            //    cmd.Parameters.AddWithValue("@FromDate", Fdate);
            //else
            //    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            //if (!string.IsNullOrEmpty(Tdate.ToString()))
            //    cmd.Parameters.AddWithValue("@ToDate", Tdate);
            //else
            //    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerOrderScheduleGroupList(DateTime? orderDate)
        {
            //con.Open();
            DateTime OrderDate = orderDate.Value;
            // DateTime SubTo = Convert.ToDateTime("2020-12-27 00:00:00");
            // DateTime OrderDate = DateTime.Now.AddDays(1);
            var date = OrderDate.Date;
            SqlCommand cmd = new SqlCommand("Customer_Order_Schedule_Group_List", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(date.ToString()))
                cmd.Parameters.AddWithValue("@OrderDate", date);
            else
                cmd.Parameters.AddWithValue("@OrderDate", DBNull.Value);
            //if (!string.IsNullOrEmpty(Fdate.ToString()))
            //    cmd.Parameters.AddWithValue("@FromDate", Fdate);
            //else
            //    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            //if (!string.IsNullOrEmpty(Tdate.ToString()))
            //    cmd.Parameters.AddWithValue("@ToDate", Tdate);
            //else
            //    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable TestgetCustomerOrderScheduleGroupList()
        {
            //con.Open();
            DateTime OrderDate = indianTime.AddDays(1);
            // DateTime SubTo = Convert.ToDateTime("2020-12-27 00:00:00");
            // DateTime OrderDate = DateTime.Now.AddDays(1);
            var date = OrderDate.Date;
            SqlCommand cmd = new SqlCommand("Customer_Order_Schedule_Group_List_test", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(date.ToString()))
                cmd.Parameters.AddWithValue("@OrderDate", date);
            else
                cmd.Parameters.AddWithValue("@OrderDate", DBNull.Value);
            //if (!string.IsNullOrEmpty(Fdate.ToString()))
            //    cmd.Parameters.AddWithValue("@FromDate", Fdate);
            //else
            //    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            //if (!string.IsNullOrEmpty(Tdate.ToString()))
            //    cmd.Parameters.AddWithValue("@ToDate", Tdate);
            //else
            //    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerOrderHis(int? CustomerId, DateTime? Fdate, DateTime? Tdate)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Customer_Order_History", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(Fdate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", Fdate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(Tdate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", Tdate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable getCustomerOrderHistoryAdmin(int? CustomerId, int? DeliveryBoyId, DateTime? Fdate, DateTime? Tdate)
        {
            //con.Open();
            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_Admin", con); change on 08-09-2021. for order history by delivery boy requirement
            SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(DeliveryBoyId.ToString()))
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DeliveryBoyId);
            else
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DBNull.Value);
            if (!string.IsNullOrEmpty(Fdate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", Fdate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(Tdate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", Tdate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerOrderHistoryAdminnew()
        {
            //con.Open();
            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_Admin", con); change on 08-09-2021. for order history by delivery boy requirement

            string query = "select  ot.Id,CONCAT(cm.FirstName ,' ', cm.LastName) as Customer,convert(varchar,ot.OrderDate,23) as OrderDate,ot.CustomerId,od.ProductId,ot.OrderNo	";
            query += " ,pm.ProductName,At.Name,od.Qty,";
            query += " od.Amount,od.RewardPOint,isnull(od.Profit,0) as Profit,ot.[Status]";
            query += " from  [dbo].[tbl_Customer_Order_Transaction] ot";
            query += " left join [dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id";
            query += " left join [dbo].[tbl_Product_Master] pm on pm.Id = od.ProductId";
            query += " left join [dbo].[tbl_Customer_Master] cm on cm.Id = ot.CustomerId";
            query += " left join [dbo].[tbl_Attributes] At on od.AttributeId = At.Id";
            query += " left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.CustomerId = cm.Id where (convert(varchar,ot.Orderdate,23) <= CONVERT(date, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'))) order by convert(varchar,ot.Orderdate,23) OFFSET 0 ROWS FETCH NEXT 50 ROWS ONLY";

            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_SelectAll", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public DataTable getCustomerOrderHistoryAdminnew1(string startpoint)
        {
            //con.Open();
            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_Admin", con); change on 08-09-2021. for order history by delivery boy requirement
            if(string.IsNullOrEmpty(startpoint) || startpoint=="")
            {
                startpoint = "0";
            }

            string query = "select  ot.Id,CONCAT(cm.FirstName ,' ', cm.LastName) as Customer,convert(varchar,ot.OrderDate,23) as OrderDate,ot.CustomerId,od.ProductId,ot.OrderNo	";
            query += " ,pm.ProductName,At.Name,od.Qty,";
            query += " od.Amount,od.RewardPOint,isnull(od.Profit,0) as Profit,ot.[Status]";
            query += " from  [dbo].[tbl_Customer_Order_Transaction] ot";
            query += " left join [dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id";
            query += " left join [dbo].[tbl_Product_Master] pm on pm.Id = od.ProductId";
            query += " left join [dbo].[tbl_Customer_Master] cm on cm.Id = ot.CustomerId";
            query += " left join [dbo].[tbl_Attributes] At on od.AttributeId = At.Id";
            query += " left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.CustomerId = cm.Id where (convert(varchar,ot.Orderdate,23) <= CONVERT(date, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')))  order by convert(varchar,ot.Orderdate,23) OFFSET "+startpoint+" ROWS FETCH NEXT 50 ROWS ONLY";

            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_SelectAll", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerOrderHistoryAdminnew11(DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_Admin", con); change on 08-09-2021. for order history by delivery boy requirement

            string query = "select  ot.Id,CONCAT(cm.FirstName ,' ', cm.LastName) as Customer,convert(varchar,ot.OrderDate,23) as OrderDate,ot.CustomerId,od.ProductId,ot.OrderNo	";
            query += " ,pm.ProductName,At.Name,od.Qty,";
            query += " od.Amount,od.RewardPOint,isnull(od.Profit,0) as Profit,ot.[Status]";
            query += " from  [dbo].[tbl_Customer_Order_Transaction] ot";
            query += " left join [dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id";
            query += " left join [dbo].[tbl_Product_Master] pm on pm.Id = od.ProductId";
            query += " left join [dbo].[tbl_Customer_Master] cm on cm.Id = ot.CustomerId";
            query += " left join [dbo].[tbl_Attributes] At on od.AttributeId = At.Id";
            query += " left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.CustomerId = cm.Id where (convert(varchar,ot.Orderdate,23) <= CONVERT(date, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')))";
            query += "  AND (@FromDate is null or @ToDate is null or convert(varchar,ot.Orderdate,23) between @FromDate and @Todate)  order by convert(varchar,ot.Orderdate,23)";
            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_SelectAll", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(FDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(TDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", TDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerOrderHistoryAdminnew1dup(string startid)
        {
            //con.Open();
            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_Admin", con); change on 08-09-2021. for order history by delivery boy requirement

            if (string.IsNullOrEmpty(startid) || startid == "")
            {
                startid = "0";
            }
            string query = "select  ot.Id,CONCAT(cm.FirstName ,' ', cm.LastName) as Customer,convert(varchar,ot.OrderDate,23) as OrderDate,ot.CustomerId,od.ProductId,ot.OrderNo	";
            query += " ,pm.ProductName,At.Name,od.Qty,";
            query += " od.Amount,od.RewardPOint,isnull(od.Profit,0) as Profit,ot.[Status]";
            query += " from  [dbo].[tbl_Customer_Order_Transaction] ot";
            query += " left join [dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id";
            query += " left join [dbo].[tbl_Product_Master] pm on pm.Id = od.ProductId";
            query += " left join [dbo].[tbl_Customer_Master] cm on cm.Id = ot.CustomerId";
            query += " left join [dbo].[tbl_Attributes] At on od.AttributeId = At.Id";
            query += " left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.CustomerId = cm.Id where (convert(varchar,ot.Orderdate,23) <= CONVERT(date, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'))) order by convert(varchar,ot.Orderdate,23) OFFSET "+startid+" ROWS FETCH NEXT 50 ROWS ONLY";

            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_SelectAll", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable getCustomerOrderHistoryAdminnew2(string startid)
        {
            //con.Open();
            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_Admin", con); change on 08-09-2021. for order history by delivery boy requirement
            if (string.IsNullOrEmpty(startid) || startid == "")
            {
                startid = "0";
            }
            string query = "select  ot.Id,CONCAT(cm.FirstName ,' ', cm.LastName) as Customer,convert(varchar,ot.OrderDate,23) as OrderDate,ot.CustomerId,od.ProductId,ot.OrderNo	";
            query += " ,pm.ProductName,At.Name,od.Qty,";
            query += " od.Amount,od.RewardPOint,isnull(od.Profit,0) as Profit,ot.[Status]";
            query += " from  [dbo].[tbl_Customer_Order_Transaction] ot";
            query += " left join [dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id";
            query += " left join [dbo].[tbl_Product_Master] pm on pm.Id = od.ProductId";
            query += " left join [dbo].[tbl_Customer_Master] cm on cm.Id = ot.CustomerId";
            query += " left join [dbo].[tbl_Attributes] At on od.AttributeId = At.Id";
            query += " left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.CustomerId = cm.Id where (convert(varchar,ot.Orderdate,23) <= CONVERT(date, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')))  order by convert(varchar,ot.Orderdate,23) OFFSET "+startid+" ROWS FETCH NEXT 50 ROWS ONLY";

            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_SelectAll", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable getCustomerOrderHistoryAdminnew2dup(string startid)
        {
            //con.Open();
            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_Admin", con); change on 08-09-2021. for order history by delivery boy requirement
            if (string.IsNullOrEmpty(startid) || startid == "")
            {
                startid = "0";
            }
            string query = "select  ot.Id,CONCAT(cm.FirstName ,' ', cm.LastName) as Customer,convert(varchar,ot.OrderDate,23) as OrderDate,ot.CustomerId,od.ProductId,ot.OrderNo	";
            query += " ,pm.ProductName,At.Name,od.Qty,";
            query += " od.Amount,od.RewardPOint,isnull(od.Profit,0) as Profit,ot.[Status]";
            query += " from  [dbo].[tbl_Customer_Order_Transaction] ot";
            query += " left join [dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id";
            query += " left join [dbo].[tbl_Product_Master] pm on pm.Id = od.ProductId";
            query += " left join [dbo].[tbl_Customer_Master] cm on cm.Id = ot.CustomerId";
            query += " left join [dbo].[tbl_Attributes] At on od.AttributeId = At.Id";
            query += " left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.CustomerId = cm.Id where (convert(varchar,ot.Orderdate,23) <= CONVERT(date, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'))) order by convert(varchar,ot.Orderdate,23) OFFSET "+startid+" ROWS FETCH NEXT 50 ROWS ONLY";

            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_SelectAll", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerOrderHistoryAdminnew3()
        {
            //con.Open();
            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_Admin", con); change on 08-09-2021. for order history by delivery boy requirement

            string query = "select Top 50  ot.Id,CONCAT(cm.FirstName ,' ', cm.LastName) as Customer,convert(varchar,ot.OrderDate,23) as OrderDate,ot.CustomerId,od.ProductId,ot.OrderNo	";
            query += " ,pm.ProductName,At.Name,od.Qty,";
            query += " od.Amount,od.RewardPOint,isnull(od.Profit,0) as Profit,ot.[Status]";
            query += " from  [dbo].[tbl_Customer_Order_Transaction] ot";
            query += " left join [dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id";
            query += " left join [dbo].[tbl_Product_Master] pm on pm.Id = od.ProductId";
            query += " left join [dbo].[tbl_Customer_Master] cm on cm.Id = ot.CustomerId";
            query += " left join [dbo].[tbl_Attributes] At on od.AttributeId = At.Id";
            query += " left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.CustomerId = cm.Id  where (convert(varchar,ot.Orderdate,23) <= CONVERT(date, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'))) order by convert(varchar,ot.Orderdate,23) DESC";

            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_SelectAll", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }



        public DataTable getCustomerOrderHistoryAdminntotal()
        {
            //con.Open();
            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_Admin", con); change on 08-09-2021. for order history by delivery boy requirement

            string query = "select Count(ot.Id) as tot ";
           
            query += " from  [dbo].[tbl_Customer_Order_Transaction] ot";
            query += " left join [dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id";
            query += " left join [dbo].[tbl_Product_Master] pm on pm.Id = od.ProductId";
            query += " left join [dbo].[tbl_Customer_Master] cm on cm.Id = ot.CustomerId";
            query += " left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.CustomerId = cm.Id where (convert(varchar,ot.Orderdate,23) <= CONVERT(date, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')))";

            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_SelectAll", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable getCustomerOrderFutureAdmintotal()
        {
            //con.Open();
            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_Admin", con); change on 08-09-2021. for order history by delivery boy requirement

            string query = "select Count(ot.Id) as tot ";

            query += " from  [dbo].[tbl_Customer_Order_Transaction] ot";
            query += " left join [dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id";
            query += " left join [dbo].[tbl_Product_Master] pm on pm.Id = od.ProductId";
            query += " left join [dbo].[tbl_Customer_Master] cm on cm.Id = ot.CustomerId";
            query += " where (convert(varchar,ot.Orderdate,23) > CONVERT(date,SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'))) ";
            

            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_SelectAll", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable getCustomerOrderFuture(int? CustomerId, DateTime? Fdate, DateTime? Tdate)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Customer_Order_Future", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(Fdate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", Fdate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(Tdate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", Tdate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable getCustomerOrderFutureAdmin(int? CustomerId, DateTime? Fdate, DateTime? Tdate)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Customer_Order_Future_Admin", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(Fdate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", Fdate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(Tdate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", Tdate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public DataTable getCustomerOrderFutureAdminnew()
        {
            //con.Open();
            string query = "select  ot.Id,CONCAT(cm.FirstName ,' ', cm.LastName) as Customer,convert(varchar,ot.OrderDate,23) as OrderDate,CustomerId,od.ProductId,ot.OrderNo	";
            query += " ,pm.ProductName,At.Name,od.Qty,";
            query += " od.Amount as OAmount,pm.SalePrice as Amount,od.RewardPOint,isnull(od.Profit,0) as Profit,ot.[Status]";
            query += " from  [dbo].[tbl_Customer_Order_Transaction] ot";
            query += " Inner join [dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id";
            query += " Inner join [dbo].[tbl_Product_Master] pm on pm.Id = od.ProductId";
            query += " Inner join [dbo].[tbl_Customer_Master] cm on cm.Id = ot.CustomerId";
            query += " Inner join [dbo].[tbl_Attributes] At on od.AttributeId = At.ID";

            query += " where (convert(varchar,ot.Orderdate,23) > CONVERT(date,SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'))) order by convert(varchar,ot.Orderdate,23) OFFSET 0 ROWS FETCH NEXT 50 ROWS ONLY";
            //SqlCommand cmd = new SqlCommand("Customer_Order_Future_Admin", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerOrderFutureAdminnew1(string startid)
        {
            //con.Open();
            if (string.IsNullOrEmpty(startid) || startid == "")
            {
                startid = "0";
            }
            string query = "select  ot.Id,CONCAT(cm.FirstName ,' ', cm.LastName) as Customer,convert(varchar,ot.OrderDate,23) as OrderDate,CustomerId,od.ProductId,ot.OrderNo	";
            query += " ,pm.ProductName,At.Name,od.Qty,";
            query += " od.Amount as OAmount,pm.SalePrice as Amount,od.RewardPOint,isnull(od.Profit,0) as Profit,ot.[Status]";
            query += " from  [dbo].[tbl_Customer_Order_Transaction] ot";
            query += " Inner join [dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id";
            query += " Inner join [dbo].[tbl_Product_Master] pm on pm.Id = od.ProductId";
            query += " Inner join [dbo].[tbl_Customer_Master] cm on cm.Id = ot.CustomerId";
            query += " Inner join [dbo].[tbl_Attributes] At on od.AttributeId = At.ID";
            query += " where (convert(varchar,ot.Orderdate,23) > CONVERT(date,SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'))) order by convert(varchar,ot.Orderdate,23) OFFSET " + startid + " ROWS FETCH NEXT 50 ROWS ONLY";
            //SqlCommand cmd = new SqlCommand("Customer_Order_Future_Admin", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable getCustomerOrderFutureAdminnew1dup(string startid)
        {
            //con.Open();
            if (string.IsNullOrEmpty(startid) || startid == "")
            {
                startid = "0";
            }
            string query = "select  ot.Id,CONCAT(cm.FirstName ,' ', cm.LastName) as Customer,convert(varchar,ot.OrderDate,23) as OrderDate,CustomerId,od.ProductId,ot.OrderNo	";
            query += " ,pm.ProductName,At.Name,od.Qty,";
            query += " od.Amount as OAmount,pm.SalePrice as Amount,od.RewardPOint,isnull(od.Profit,0) as Profit,ot.[Status]";
            query += " from  [dbo].[tbl_Customer_Order_Transaction] ot";
            query += " Inner join [dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id";
            query += " Inner join [dbo].[tbl_Product_Master] pm on pm.Id = od.ProductId";
            query += " Inner join [dbo].[tbl_Customer_Master] cm on cm.Id = ot.CustomerId";
            query += " Inner join [dbo].[tbl_Attributes] At on od.AttributeId = At.ID";
            query += " where (convert(varchar,ot.Orderdate,23) > CONVERT(date,SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'))) order by convert(varchar,ot.Orderdate,23) OFFSET " + startid + " ROWS FETCH NEXT 50 ROWS ONLY";
            //SqlCommand cmd = new SqlCommand("Customer_Order_Future_Admin", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable getCustomerOrderFutureAdminnew2( string startid)
        {
            //con.Open();
            if (string.IsNullOrEmpty(startid) || startid == "")
            {
                startid = "0";
            }
            string query = "select  ot.Id,CONCAT(cm.FirstName ,' ', cm.LastName) as Customer,convert(varchar,ot.OrderDate,23) as OrderDate,CustomerId,od.ProductId,ot.OrderNo	";
            query += " ,pm.ProductName,At.Name,od.Qty,";
            query += " od.Amount as OAmount,pm.SalePrice as Amount,od.RewardPOint,isnull(od.Profit,0) as Profit,ot.[Status]";
            query += " from  [dbo].[tbl_Customer_Order_Transaction] ot";
            query += " Inner join [dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id";
            query += " Inner join [dbo].[tbl_Product_Master] pm on pm.Id = od.ProductId";
            query += " Inner join [dbo].[tbl_Customer_Master] cm on cm.Id = ot.CustomerId";
            query += " Inner join [dbo].[tbl_Attributes] At on od.AttributeId = At.ID";
            query += " where (convert(varchar,ot.Orderdate,23) > CONVERT(date,SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'))) order by convert(varchar,ot.Orderdate,23) OFFSET " + startid + " ROWS FETCH NEXT 50 ROWS ONLY";
            //SqlCommand cmd = new SqlCommand("Customer_Order_Future_Admin", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable getCustomerOrderFutureAdminnew2dup(string startid)
        {
            //con.Open();
            if (string.IsNullOrEmpty(startid) || startid == "")
            {
                startid = "0";
            }
            string query = "select  ot.Id,CONCAT(cm.FirstName ,' ', cm.LastName) as Customer,convert(varchar,ot.OrderDate,23) as OrderDate,CustomerId,od.ProductId,ot.OrderNo	";
            query += " ,pm.ProductName,At.Name,od.Qty,";
            query += " od.Amount as OAmount,pm.SalePrice as Amount,od.RewardPOint,isnull(od.Profit,0) as Profit,ot.[Status]";
            query += " from  [dbo].[tbl_Customer_Order_Transaction] ot";
            query += " Inner join [dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id";
            query += " Inner join [dbo].[tbl_Product_Master] pm on pm.Id = od.ProductId";
            query += " Inner join [dbo].[tbl_Customer_Master] cm on cm.Id = ot.CustomerId";
            query += " Inner join [dbo].[tbl_Attributes] At on od.AttributeId = At.ID";
            query += " where (convert(varchar,ot.Orderdate,23) > CONVERT(date,SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'))) order by convert(varchar,ot.Orderdate,23) OFFSET " + startid + " ROWS FETCH NEXT 50 ROWS ONLY";
            //SqlCommand cmd = new SqlCommand("Customer_Order_Future_Admin", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable getCustomerOrderFutureAdminnew3()
        {
            //con.Open();
            string query = "select Top 50 ot.Id,CONCAT(cm.FirstName ,' ', cm.LastName) as Customer,convert(varchar,ot.OrderDate,23) as OrderDate,CustomerId,od.ProductId,ot.OrderNo	";
            query += " ,pm.ProductName,od.Qty,";
            query += " od.Amount as OAmount,pm.SalePrice as Amount,od.RewardPOint,isnull(od.Profit,0) as Profit,ot.[Status]";
            query += " from  [dbo].[tbl_Customer_Order_Transaction] ot";
            query += " left join [dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id";
            query += " left join [dbo].[tbl_Product_Master] pm on pm.Id = od.ProductId";
            query += " left join [dbo].[tbl_Customer_Master] cm on cm.Id = ot.CustomerId";
            query += " where (convert(varchar,ot.Orderdate,23) > CONVERT(date,SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'))) order by convert(varchar,ot.Orderdate,23) DESC";
            //SqlCommand cmd = new SqlCommand("Customer_Order_Future_Admin", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerOrderFutureAdminnew11(DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_Admin", con); change on 08-09-2021. for order history by delivery boy requirement

            string query = "select  ot.Id,CONCAT(cm.FirstName ,' ', cm.LastName) as Customer,convert(varchar,ot.OrderDate,23) as OrderDate,CustomerId,od.ProductId,ot.OrderNo	";
            query += " ,pm.ProductName,At.Name,od.Qty,";
            query += " od.Amount as OAmount,pm.SalePrice as Amount,od.RewardPOint,isnull(od.Profit,0) as Profit,ot.[Status]";
            query += " from  [dbo].[tbl_Customer_Order_Transaction] ot";
            query += " Inner join [dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id";
            query += " Inner join [dbo].[tbl_Product_Master] pm on pm.Id = od.ProductId";
            query += " Inner join [dbo].[tbl_Customer_Master] cm on cm.Id = ot.CustomerId";
            query += " Inner join [dbo].[tbl_Attributes] At on od.AttributeId = At.ID";
            query += " where (convert(varchar,ot.Orderdate,23) > CONVERT(date,SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30')))";
            query += "  AND (@FromDate is null or @ToDate is null or convert(varchar,ot.Orderdate,23) between @FromDate and @Todate)  order by convert(varchar,ot.Orderdate,23)";
            //SqlCommand cmd = new SqlCommand("Customer_Order_Histroy_SelectAll", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(FDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(TDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", TDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerOrderDetail(int CustomerId, DateTime OrderDate)
        {
            //con.Open();
            //SqlCommand cmd = new SqlCommand("Customer_Order_Detail", con);
            //cmd.CommandType = CommandType.StoredProcedure;


            SqlCommand cmd = new SqlCommand("select ot.Id,ot.OrderDate,max(CustomerId) AS CustomerId,od.ProductId,max(pm.ProductName) as ProductName,od.Qty,od.Amount as OAmount,(pm.SalePrice*od.Qty)  as Amount,ot.OrderFlag from  [dbo].[tbl_Customer_Order_Transaction] ot left join [dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id left join [dbo].[tbl_Product_Master] pm on pm.Id = od.ProductId where (@CustomerId is null or ot.CustomerId=@CustomerId) and od.Qty <> 0 and (@OrderDate is null or convert(varchar,ot.OrderDate,23) = @OrderDate) and ot.CustomerId = @CustomerId Group By ot.OrderDate,ot.Id,od.ProductId,od.Qty,od.Amount,pm.SalePrice,ot.OrderFlag", con);
            cmd.CommandType = CommandType.Text;

            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(OrderDate.ToString()))
                cmd.Parameters.AddWithValue("@OrderDate", OrderDate);
            else
                cmd.Parameters.AddWithValue("@OrderDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerHisOrderDetail(int CustomerId, DateTime OrderDate)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Customer_Order_History_Detail", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(OrderDate.ToString()))
                cmd.Parameters.AddWithValue("@OrderDate", OrderDate);
            else
                cmd.Parameters.AddWithValue("@OrderDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerOrderList(int? CustomerId, int? ProductId, DateTime Fdate, DateTime Tdate)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("select * from [tbl_Customer_Order_Transaction] ot left join[dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id where ot.CustomerId =@CustomerId and ot.OrderDate between @FromDate and @ToDate and od.ProductId=@ProductId and ot.Status='Pending'", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(ProductId.ToString()))
                cmd.Parameters.AddWithValue("@ProductId", ProductId);
            else
                cmd.Parameters.AddWithValue("@ProductId", DBNull.Value);
            if (!string.IsNullOrEmpty(Fdate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", Fdate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(Tdate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", Tdate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public DataTable getCustomerOrderListalternate(int? CustomerId, int? ProductId, DateTime Fdate, DateTime Tdate)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("select * from [tbl_Customer_Order_Transaction] ot left join[dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id where ot.CustomerId =@CustomerId and ot.OrderDate between @FromDate and @ToDate and od.ProductId=@ProductId and ot.Status='Pending' and ot.OrderFlag='Alternate'", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(ProductId.ToString()))
                cmd.Parameters.AddWithValue("@ProductId", ProductId);
            else
                cmd.Parameters.AddWithValue("@ProductId", DBNull.Value);
            if (!string.IsNullOrEmpty(Fdate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", Fdate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(Tdate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", Tdate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerOrderListMultidt(int? CustomerId, int? ProductId, DateTime Fdate, DateTime Tdate)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("select * from [tbl_Customer_Order_Transaction] ot left join[dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id where ot.CustomerId =@CustomerId and ot.OrderDate between @FromDate and @ToDate and od.ProductId=@ProductId and ot.Status='Pending' and ot.OrderFlag='MultipleDt'", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(ProductId.ToString()))
                cmd.Parameters.AddWithValue("@ProductId", ProductId);
            else
                cmd.Parameters.AddWithValue("@ProductId", DBNull.Value);
            if (!string.IsNullOrEmpty(Fdate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", Fdate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(Tdate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", Tdate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public DataTable getCustomerOrderListweeklyday(int? CustomerId, int? ProductId, DateTime Fdate, DateTime Tdate)
        {
            //con.Open();

          string wname=  Fdate.ToString("dddd");

            SqlCommand cmd = new SqlCommand("select * from [tbl_Customer_Order_Transaction] ot left join[dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id where ot.CustomerId =@CustomerId and ot.OrderDate between @FromDate and @ToDate and od.ProductId=@ProductId and ot.Status='Pending' and ot.OrderFlag='WeekDay' AND FORMAT(ot.OrderDate, 'dddd')=@wname", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(ProductId.ToString()))
                cmd.Parameters.AddWithValue("@ProductId", ProductId);
            else
                cmd.Parameters.AddWithValue("@ProductId", DBNull.Value);
            if (!string.IsNullOrEmpty(Fdate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", Fdate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(Tdate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", Tdate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);

            if (!string.IsNullOrEmpty(wname.ToString()))
                cmd.Parameters.AddWithValue("@wname", wname);
            else
                cmd.Parameters.AddWithValue("@wname", "");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable getCustomerOrderList(int? CustomerId, DateTime Fdate, DateTime Tdate)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("select * from [tbl_Customer_Order_Transaction] ot left join[dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id where ot.CustomerId =@CustomerId and ot.OrderDate between @FromDate and @ToDate and ot.Status='Pending'", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(Fdate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", Fdate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(Tdate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", Tdate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerSubscriptionOrderdate(Subscription obj)
        {
            SqlCommand cmd = new SqlCommand("select * from [dbo].[tbl_Customer_Subscription] where CustomerId=@CustomerId and FromDate < @FromDate and Todate > @FromDate and PaymentStatus='Yes' and SubscriptionStatus='Open'", con);
            //cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(obj.OrderDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", obj.OrderDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable GetAllOpenSubList(int Customerid)
        {
            SqlCommand cmd = new SqlCommand("select  [CustomerId], min([FromDate]) as [FromDate], max([ToDate]) as [ToDate] from [dbo].[tbl_Customer_Subscription] where CustomerId=@CustomerId and SubscriptionStatus='Open' Group by [CustomerId]", con);
            if (!string.IsNullOrEmpty(Customerid.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", Customerid);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public int UpdateStatusCustomerSubscription(Subscription obj)
        {
            int i = 0;
            con.Open();
            SqlCommand cmd = new SqlCommand("update [dbo].[tbl_Customer_Subscription] set  SubscriptionStatus='Close' where Id=@Id", con);
            //cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(obj.Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", obj.Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public DataTable getCustomerWeekOrderList(int CustomerId, DateTime Fdate, DateTime Tdate)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("select * from [tbl_Customer_Order_Transaction] where CustomerId=@CustomerId and OrderDate between @FromDate and @ToDate and OrderFlag='Week'", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(Fdate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", Fdate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(Tdate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", Tdate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerOnedateOrderList(int Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("select * from [tbl_Customer_Order_Transaction] ot left join [tbl_Customer_Order_Detail] od on od.OrderId = ot.Id where ot.Status='Pending' and ot.Id=@Id", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            //if (!string.IsNullOrEmpty(CustomerId.ToString()))
            //    cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            //else
            //    cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            //if (!string.IsNullOrEmpty(Odate.ToString()))
            //    cmd.Parameters.AddWithValue("@OrderDate", Odate);
            //else
            //    cmd.Parameters.AddWithValue("@OrderDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public int InsertDeleteOrder(int cid,int pid)
        {
            DateTime today = DateTime.Now;

            int i = 0;
            con.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO tbl_dailyDelete(Proid,CustomerId,UpdatedOn)VALUES(@Proid,@CustomerId,@UpdatedOn)", con);
            //cmd.CommandType = CommandType.Text;

            if (!string.IsNullOrEmpty(pid.ToString()))
                cmd.Parameters.AddWithValue("@Proid", pid);
            else
                cmd.Parameters.AddWithValue("@Proid", 0);
            if (!string.IsNullOrEmpty(cid.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", cid);
            else
                cmd.Parameters.AddWithValue("@CustomerId", 0);

            
                cmd.Parameters.AddWithValue("@UpdatedOn", today);
           
            i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int DeleteCustomerOrder(int id)
        {
            int i1 = 0;
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Customer_Order_Detail where OrderId=" + id, con);
            int i = cmd.ExecuteNonQuery();

            SqlCommand cmd1 = new SqlCommand("Delete from tbl_Customer_Order_Transaction where Id=" + id, con);
            i1 = cmd1.ExecuteNonQuery();
            con.Close();
            return i1;
        }


        public int DeleteCustomerOrderw(int id,DateTime Fromdate)
        {
            int i1 = 0;
            con.Open();



            string f = Fromdate.ToShortDateString();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Customer_Order_Detail where OrderId=" + id , con);



            int i = cmd.ExecuteNonQuery();

            SqlCommand cmd1 = new SqlCommand("Delete from tbl_Customer_Order_Transaction where  Id=" + id, con);
            i1 = cmd1.ExecuteNonQuery();
            con.Close();
            return i1;
        }
        public int DeleteCustomerOrderTrack(int CustomerId, int ProductId)
        {
            int i1 = 0;
            try
            {
                clsCommon _clsCommon = new clsCommon();
                _clsCommon.Get_Entity("Delete from tbl_Cusomter_Order_Track where CustomerID = " + CustomerId + " AND ProductID = " + ProductId);                
            }
            catch { }
            return i1;
        }

        public int DeleteCustomerOrderTrackw(int CustomerId, int ProductId,DateTime FromDate)
        {
            int i1 = 0;
            con.Open();
            string wname = FromDate.ToString("dddd");
            string f = FromDate.ToShortDateString();

            SqlCommand cmd1 = new SqlCommand("Delete from tbl_Cusomter_Order_Track where  FORMAT(NextOrderDate, 'dddd')='" + wname+ "' And CustomerID = " + CustomerId + " AND ProductID = " + ProductId, con);
            i1 = cmd1.ExecuteNonQuery();
            con.Close();
            return i1;

            //try
            //{
            //    clsCommon _clsCommon = new clsCommon();
            //    _clsCommon.Get_Entity(");
            //}
            //catch { }
            //return i1;
        }

        public int UpdateCustomerOrderMobile(Subscription obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Customer_Order_Mobile_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.Id);
                com.Parameters.AddWithValue("@TotalAmount", obj.TotalAmount);
                if (!string.IsNullOrEmpty(obj.Status))
                    com.Parameters.AddWithValue("@Status", obj.Status);
                else
                    com.Parameters.AddWithValue("@Status", DBNull.Value);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int UpdateCustomerOrderDetailMobile(Subscription obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Customer_OrderDetail_Mobile_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@OrderId", obj.OrderId);
                if (!string.IsNullOrEmpty(obj.Qty.ToString()))
                    com.Parameters.AddWithValue("@Qty", obj.Qty);
                else
                    com.Parameters.AddWithValue("@Qty", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Amount.ToString()))
                    com.Parameters.AddWithValue("@Amount", obj.Amount);
                else
                    com.Parameters.AddWithValue("@Amount", 0);
                //if (!string.IsNullOrEmpty(obj.MRPPrice.ToString()))
                //    com.Parameters.AddWithValue("@Mrp", obj.MRPPrice);
                //else
                //    com.Parameters.AddWithValue("@Mrp", 0);

                //if (!string.IsNullOrEmpty(obj.PurchasePrice.ToString()))
                //    com.Parameters.AddWithValue("@PurchasePrice", obj.PurchasePrice);
                //else
                //    com.Parameters.AddWithValue("@PurchasePrice", 0);

                if (!string.IsNullOrEmpty(obj.Discount.ToString()))
                    com.Parameters.AddWithValue("@Discount", obj.Discount);
                else
                    com.Parameters.AddWithValue("@Discount", 0);
                if (!string.IsNullOrEmpty(obj.RewardPoint.ToString()))
                    com.Parameters.AddWithValue("@RewardPoint", obj.RewardPoint);
                else
                    com.Parameters.AddWithValue("@RewardPoint", 0);
                if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                    com.Parameters.AddWithValue("@TotalFinalAmount", obj.TotalFinalAmount);
                else
                    com.Parameters.AddWithValue("@TotalFinalAmount", 0);
                if (!string.IsNullOrEmpty(obj.CGSTAmount.ToString()))
                    com.Parameters.AddWithValue("@CGSTAmount", obj.CGSTAmount);
                else
                    com.Parameters.AddWithValue("@CGSTAmount", 0);
                if (!string.IsNullOrEmpty(obj.SGSTAmount.ToString()))
                    com.Parameters.AddWithValue("@SGSTAmount", obj.SGSTAmount);
                else
                    com.Parameters.AddWithValue("@SGSTAmount", 0);
                if (!string.IsNullOrEmpty(obj.IGSTAmount.ToString()))
                    com.Parameters.AddWithValue("@IGSTAmount", obj.IGSTAmount);
                else
                    com.Parameters.AddWithValue("@IGSTAmount", 0);
                if (!string.IsNullOrEmpty(obj.Profit.ToString()))
                    com.Parameters.AddWithValue("@Profit", obj.Profit);
                else
                    com.Parameters.AddWithValue("@Profit", 0);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int UpdateCustomerWallet(Subscription obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Customer_Wallet_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@OrderId", obj.OrderId);
                com.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                if (!string.IsNullOrEmpty(obj.Amount.ToString()))
                    com.Parameters.AddWithValue("@TotalFinalAmount", obj.Amount);
                else
                    com.Parameters.AddWithValue("@TotalFinalAmount", 0);
                UpdateCustomerOrderTransaction(obj.OrderId, obj.Amount);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }
        public int UpdateCustomerWallet1(Subscription obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Customer_Wallet_Update1", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@OrderId", obj.OrderId);
                com.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                if (!string.IsNullOrEmpty(obj.Amount.ToString()))
                    com.Parameters.AddWithValue("@TotalFinalAmount", obj.Amount);
                else
                    com.Parameters.AddWithValue("@TotalFinalAmount", 0);
                if (!string.IsNullOrEmpty(obj.Qty.ToString()))
                    com.Parameters.AddWithValue("@proqty", obj.Qty);
                else
                    com.Parameters.AddWithValue("@proqty", 0);
                UpdateCustomerOrderTransaction(obj.OrderId, obj.Amount);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }
        public int UpdateCustomerOrderTransaction(int id, decimal? Amount)
        {
            int i1 = 0;
            try
            {
                SqlCommand cmd = new SqlCommand("Update tbl_Customer_Order_Transaction Set TotalAmount='" + Amount + "' where Id=" + id, con);
                int i = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return i1;
        }

        public int CheckCustomerWalletEntry(int Orderid, int customerId)
        {
            Models.clsCommon _clsCommon = new clsCommon();
            var dt = _clsCommon.selectwhere("*", "tbl_Customer_Wallet", "OrderId='" + OrderId + "' AND CustomerId='" + customerId + "'");
            if (dt.Rows.Count > 0)
                return Convert.ToInt32(dt.Rows[0]["Id"]);
            return 0;
        }

        public int UpdateCustomerOrderMain(Subscription obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Customer_Order_Main_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.Id);
                com.Parameters.AddWithValue("@TotalAmount", obj.Amount);
                if (!string.IsNullOrEmpty(obj.OrderDate.ToString()))
                    com.Parameters.AddWithValue("@OrderDate", obj.OrderDate);
                else
                    com.Parameters.AddWithValue("@OrderDate", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Status))
                    com.Parameters.AddWithValue("@Status", obj.Status);
                else
                    com.Parameters.AddWithValue("@Status", DBNull.Value);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public DataTable getLast2daytotal(string CustomerId)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("select isnull(sum(pm.SalePrice),0) as Total  from tbl_Customer_Order_transaction ot " +
" left join tbl_Customer_Order_Detail od on od.OrderId = ot.Id left join tbl_Product_master pm on pm.Id = od.ProductId " +
" where ot.CustomerId = '" + CustomerId + "' and ot.[Status] = 'Pending' and convert(varchar, ot.Orderdate, 23) between dateadd(day, 1, convert(varchar, getdate(), 23)) and dateadd(day, 2, Convert(varchar, getdate(), 23))", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public int getOrederCustomerId(string Id)
        {
            int result = 0;
            //con.Open();
            SqlCommand cmd = new SqlCommand("select CustomerId from tbl_Customer_Order_transaction ot " +
" where ot.Id = '" + Id + "'", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dt.Rows[0]["CustomerId"].ToString()))
                    result = Convert.ToInt32(dt.Rows[0]["CustomerId"].ToString());
            }
            return result;
        }

        public bool cancelSubscription(int id, int cid)
        {
            try
            {
                con.Open();
                SqlCommand cd = new SqlCommand("Customer_Subscription_Cancel", con);
                cd.CommandType = CommandType.StoredProcedure;
                cd.Parameters.AddWithValue("@id", id);
                cd.Parameters.AddWithValue("@cid", cid);
                int i = cd.ExecuteNonQuery();
                if (i > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }



        //Cancel Subscription Flow

        public DataTable getOrderDetails(Subscription s)
        {
            try
            {
                con.Open();
                SqlCommand cd = new SqlCommand("Cancelled_Order_Details_Select", con);
                cd.CommandType = CommandType.StoredProcedure;
                cd.Parameters.AddWithValue("@cid", s.CustomerId);
                cd.Parameters.AddWithValue("@fd", s.cancelFD);
                cd.Parameters.AddWithValue("@td", s.cancelTD);
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(cd);
                sda.Fill(dt);
                return dt;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public DataTable getOrderTransaction(Subscription s)
        {
            try
            {
                con.Open();
                SqlCommand cd = new SqlCommand("Cancelled_Order_Transection_Select", con);
                cd.CommandType = CommandType.StoredProcedure;
                cd.Parameters.AddWithValue("@cid", s.CustomerId);
                cd.Parameters.AddWithValue("@fd", s.cancelFD);
                cd.Parameters.AddWithValue("@td", s.cancelTD);
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(cd);
                sda.Fill(dt);
                return dt;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public bool deleteOrderDetails(Subscription s)
        {
            try
            {
                con.Open();
                SqlCommand cd = new SqlCommand("Cancelled_Order_Details_Delete", con);
                cd.CommandType = CommandType.StoredProcedure;
                cd.Parameters.AddWithValue("@cid", s.CustomerId);
                cd.Parameters.AddWithValue("@fd", s.cancelFD);
                cd.Parameters.AddWithValue("@td", s.cancelTD);
                int i = cd.ExecuteNonQuery();
                if (i > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public bool deleteOrderTransaction(Subscription s)
        {
            try
            {
                con.Open();
                SqlCommand cd = new SqlCommand("Cancelled_Order_Transection_Delete", con);
                cd.CommandType = CommandType.StoredProcedure;
                cd.Parameters.AddWithValue("@cid", s.CustomerId);
                cd.Parameters.AddWithValue("@fd", s.cancelFD);
                cd.Parameters.AddWithValue("@td", s.cancelTD);
                int i = cd.ExecuteNonQuery();
                if (i > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public bool AddCancelledsub(Subscription s)
        {
            try
            {
                con.Open();
                SqlCommand cd = new SqlCommand("cancelled_Subscription_Insert", con);
                cd.CommandType = CommandType.StoredProcedure;
                cd.Parameters.AddWithValue("@CustomerId", s.CustomerId);
                cd.Parameters.AddWithValue("@cancelFD", s.cancelFD);
                cd.Parameters.AddWithValue("@cancelTD", s.cancelTD);
                cd.Parameters.AddWithValue("@walletbalance", s.walletbalance);
                cd.Parameters.AddWithValue("@RewardPoint", s.RewardPoint);
                cd.Parameters.AddWithValue("@Refund", s.Refund);
                cd.Parameters.AddWithValue("@cancelBy", s.cancelBy);
                cd.Parameters.AddWithValue("@cancelDate", s.cancelDate);
                if (!string.IsNullOrEmpty(s.remark))
                    cd.Parameters.AddWithValue("@remark", s.remark);
                else
                    cd.Parameters.AddWithValue("@remark", DBNull.Value);
                int i = cd.ExecuteNonQuery();
                if (i > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public bool UpdateCustMasterAfterCancelSub(int cid, DateTime td)
        {
            try
            {
                con.Open();
                string q = "update tbl_Customer_Master set	SubnToDate= DATEADD(DAY,-1,'" + td + "'),RewardPoint=0 where Id=" + cid + "";
                SqlCommand cd = new SqlCommand(q, con);
                int i = cd.ExecuteNonQuery();
                if (i > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public DataTable CanceledSubscriptionList()
        {
            try
            {
                con.Open();
                SqlCommand cd = new SqlCommand("Cancelled_Subscription_List", con);
                cd.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(cd);
                sda.Fill(dt);
                return dt;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);

            }
            finally
            {
                con.Close();
            }
        }

        public bool DelCancelSub(int id)
        {
            try
            {
                con.Open();
                string s = "delete from tbl_Cancel_Subscription where Id=" + id + "";
                SqlCommand cd = new SqlCommand(s, con);
                int i = cd.ExecuteNonQuery();
                if (i == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public DataTable canSubListforRpt()
        {
            try
            {
                con.Open();
                SqlCommand cd = new SqlCommand("rpt_CanceledSubList", con);
                cd.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(cd);
                sda.Fill(dt);
                return dt;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public DataTable expiredSubList()
        {
            try
            {
                con.Open();
                SqlCommand cd = new SqlCommand("Sunscription_zero_days_SelectAll", con);
                cd.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(cd);
                sda.Fill(dt);
                return dt;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public DataTable GetCustomerSubscription(DateTime? fDate, DateTime? tDate)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SP_Customer_Subcription_Calculation", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(fDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", fDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(tDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", tDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public DataTable getCustomerOrderDetailnewversion(int? custId, int? productid, int? AttributeId,string NextOrderDate)
        {
            //con.Open();
            // string CurrentDate = Helper.indianTime.ToString("yyyy-MM-dd");
            string CurrentDate =Convert.ToDateTime(NextOrderDate).ToString("yyyy-MM-dd");
            //SqlCommand cmd = new SqlCommand("Customer_Date_Order_Select", con);
            string query = "Select Ot.*,Od.VendorId,Od.VendorCatId,Od.SectorId,Od.DeliveryBoyId from [tbl_Customer_Order_Transaction] Ot";
            query += " inner join tbl_Customer_Order_Detail Od On Ot.Id=Od.OrderId ";
            query += " where Od.ProductId=@ProductId And Od.AttributeId=@AttributeId And Ot.CustomerId=@CustomerId And Ot.Status='Complete' And convert(varchar, Ot.OrderDate, 23) = '" + CurrentDate + "'";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(custId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", custId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(productid.ToString()))
                cmd.Parameters.AddWithValue("@ProductId", productid);
            else
                cmd.Parameters.AddWithValue("@ProductId", DBNull.Value);
            if (!string.IsNullOrEmpty(AttributeId.ToString()))
                cmd.Parameters.AddWithValue("@AttributeId", AttributeId);
            else
                cmd.Parameters.AddWithValue("@AttributeId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public int InsertSectorSubscription(Subscription obj)
        {
            DateTime today = DateTime.Now;

            int i = 0;
            con.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO tbl_Sector_Subscription(StateId,CityId,SectorId,SubscriptionAmount,IsActive,UpdatedOn)VALUES(@StateId,@CityId,@SectorId,@SubscriptionAmount,@IsActive,@UpdatedOn)", con);
            //cmd.CommandType = CommandType.Text;

            if (!string.IsNullOrEmpty(obj.StateCode))
                cmd.Parameters.AddWithValue("@StateId", obj.StateCode);
            else
                cmd.Parameters.AddWithValue("@StateId", 0);
            if (!string.IsNullOrEmpty(obj.CityCode))
                cmd.Parameters.AddWithValue("@CityId", obj.CityCode);
            else
                cmd.Parameters.AddWithValue("@CityId", 0);

            if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", obj.SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", 0);

            if (!string.IsNullOrEmpty(obj.Amount.ToString()))
                cmd.Parameters.AddWithValue("@SubscriptionAmount", obj.Amount);
            else
                cmd.Parameters.AddWithValue("@SubscriptionAmount", 0);

            if (!string.IsNullOrEmpty(obj.IsActive.ToString()))
                cmd.Parameters.AddWithValue("@IsActive", obj.IsActive);
            else
                cmd.Parameters.AddWithValue("@IsActive", 0);


            cmd.Parameters.AddWithValue("@UpdatedOn", Helper.indianTime);

            i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }


        public DataTable getSectorSubscriptionList(int? Id,int? SectorId)
        {
            //con.Open();
            string query = "SELECT Se.Id,Se.SubscriptionAmount,Convert(varchar,Se.UpdatedOn,23) As Updated,Se.IsActive,Sm.statename,Cm.Cityname,Sem.SectorName";
            query += " FROM [dbo].[tbl_Sector_Subscription] Se";
            query += " Inner Join tblstatemaster Sm on Se.StateId=Sm.id";
            query += " Inner Join tblcitymaster Cm On Se.CityId=Cm.id";
            query += " Inner Join tbl_Sector_Master Sem On Se.SectorId=Sem.Id";
            query += " Where (@Id is null Or Se.Id=@Id) And (@SectorId Is Null Or Se.SectorId=@SectorId)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);

            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public int DeleteSectorSubscription(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Sector_Subscription where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }


        public int UpdateSectorSubscription(Subscription obj)
        {
            DateTime today = DateTime.Now;

            int i = 0;
            con.Open();
            SqlCommand cmd = new SqlCommand("Update tbl_Sector_Subscription set SubscriptionAmount=@SubscriptionAmount Where Id=@Id", con);
            //cmd.CommandType = CommandType.Text;

            
            if (!string.IsNullOrEmpty(obj.SubscriptionId.ToString()))
                cmd.Parameters.AddWithValue("@Id", obj.SubscriptionId);
            else
                cmd.Parameters.AddWithValue("@Id", 0);

            if (!string.IsNullOrEmpty(obj.Amount.ToString()))
                cmd.Parameters.AddWithValue("@SubscriptionAmount", obj.Amount);
            else
                cmd.Parameters.AddWithValue("@SubscriptionAmount", 0);


            i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }



        public DataTable getSectorSubscriptionByCustomer(int? Id)
        {
            //con.Open();
            string query = "SELECT Se.Id,Se.SubscriptionAmount,Convert(varchar,Se.UpdatedOn,23) As Updated,Se.IsActive";
            query += " FROM [dbo].[tbl_Sector_Subscription] Se";
            query += " Inner Join tbl_Customer_Master Cm on Cm.SectorId=Se.SectorId";
            
            query += " Where (@Id is null Or Cm.Id=@Id) And Se.IsActive='true'";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);

           
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public int InsertCustomerOrderCart(Subscription obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("InsertCart", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                if (!string.IsNullOrEmpty(obj.VendorId.ToString()))
                    com.Parameters.AddWithValue("@VendorId", obj.VendorId);
                else
                    com.Parameters.AddWithValue("@VendorId", 0);

                if (!string.IsNullOrEmpty(obj.VendorCatId.ToString()))
                    com.Parameters.AddWithValue("@VendorCatId", obj.VendorCatId);
                else
                    com.Parameters.AddWithValue("@VendorCatId", 0);

                if (!string.IsNullOrEmpty(obj.DeliveryBoyId))
                    com.Parameters.AddWithValue("@DmId", obj.DeliveryBoyId);
                else
                    com.Parameters.AddWithValue("@DmId", 0);

                if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    com.Parameters.AddWithValue("@SectorId", obj.SectorId);
                else
                    com.Parameters.AddWithValue("@SectorId", 0);

                com.Parameters.AddWithValue("@ProductId", obj.ProductId);

                if (!string.IsNullOrEmpty(obj.AttributeId.ToString()))
                    com.Parameters.AddWithValue("@AttributeId", obj.AttributeId);
                else
                    com.Parameters.AddWithValue("@AttributeId", 0);
                if (!string.IsNullOrEmpty(obj.Qty.ToString()))
                    com.Parameters.AddWithValue("@Qty", obj.Qty);
                else
                    com.Parameters.AddWithValue("@Qty", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.SalePrice.ToString()))
                    com.Parameters.AddWithValue("@SalePrice", obj.SalePrice);
                else
                    com.Parameters.AddWithValue("@SalePrice", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.Amount.ToString()))
                    com.Parameters.AddWithValue("@Amount", obj.Amount);
                else
                    com.Parameters.AddWithValue("@Amount", 0);


                if (!string.IsNullOrEmpty(obj.Discount.ToString()))
                    com.Parameters.AddWithValue("@Discount", obj.Discount);
                else
                    com.Parameters.AddWithValue("@Discount", 0);
                if (!string.IsNullOrEmpty(obj.RewardPoint.ToString()))
                    com.Parameters.AddWithValue("@RewardPoint", obj.RewardPoint);
                else
                    com.Parameters.AddWithValue("@RewardPoint", 0);
                if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                    com.Parameters.AddWithValue("@TotalFinalAmount", obj.TotalFinalAmount);
                else
                    com.Parameters.AddWithValue("@TotalFinalAmount", 0);
                if (!string.IsNullOrEmpty(obj.CGSTAmount.ToString()))
                    com.Parameters.AddWithValue("@CGSTAmount", obj.CGSTAmount);
                else
                    com.Parameters.AddWithValue("@CGSTAmount", 0);
                if (!string.IsNullOrEmpty(obj.SGSTAmount.ToString()))
                    com.Parameters.AddWithValue("@SGSTAmount", obj.SGSTAmount);
                else
                    com.Parameters.AddWithValue("@SGSTAmount", 0);
                if (!string.IsNullOrEmpty(obj.IGSTAmount.ToString()))
                    com.Parameters.AddWithValue("@IGSTAmount", obj.IGSTAmount);
                else
                    com.Parameters.AddWithValue("@IGSTAmount", 0);

                if (!string.IsNullOrEmpty(obj.OrderItemDate.ToString()))
                    com.Parameters.AddWithValue("@CartDate", obj.OrderItemDate);
                else
                    com.Parameters.AddWithValue("@CartDate", DBNull.Value);

           

                if (!string.IsNullOrEmpty(obj.Profit.ToString()))
                    com.Parameters.AddWithValue("@Profit", obj.Profit);
                else
                    com.Parameters.AddWithValue("@Profit", 0);
                if (!string.IsNullOrEmpty(obj.PurchasePrice.ToString()))
                    com.Parameters.AddWithValue("@PurchasePrice", obj.PurchasePrice);
                else
                    com.Parameters.AddWithValue("@PurchasePrice", 0);


                if (!string.IsNullOrEmpty(obj.MRPPrice.ToString()))
                    com.Parameters.AddWithValue("@Mrp", obj.MRPPrice);
                else
                    com.Parameters.AddWithValue("@Mrp", 0);


                com.Parameters.AddWithValue("@Status", obj.Status);


                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int UpdateCustomerOrderCart(Subscription obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("UpdateCart", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                

                com.Parameters.AddWithValue("@ProductId", obj.ProductId);

                if (!string.IsNullOrEmpty(obj.AttributeId.ToString()))
                    com.Parameters.AddWithValue("@AttributeId", obj.AttributeId);
                else
                    com.Parameters.AddWithValue("@AttributeId", 0);
                if (!string.IsNullOrEmpty(obj.Qty.ToString()))
                    com.Parameters.AddWithValue("@Qty", obj.Qty);
                else
                    com.Parameters.AddWithValue("@Qty", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.SalePrice.ToString()))
                    com.Parameters.AddWithValue("@SalePrice", obj.SalePrice);
                else
                    com.Parameters.AddWithValue("@SalePrice", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.Amount.ToString()))
                    com.Parameters.AddWithValue("@Amount", obj.Amount);
                else
                    com.Parameters.AddWithValue("@Amount", 0);


                if (!string.IsNullOrEmpty(obj.Discount.ToString()))
                    com.Parameters.AddWithValue("@Discount", obj.Discount);
                else
                    com.Parameters.AddWithValue("@Discount", 0);
                if (!string.IsNullOrEmpty(obj.RewardPoint.ToString()))
                    com.Parameters.AddWithValue("@RewardPoint", obj.RewardPoint);
                else
                    com.Parameters.AddWithValue("@RewardPoint", 0);
                if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                    com.Parameters.AddWithValue("@TotalFinalAmount", obj.TotalFinalAmount);
                else
                    com.Parameters.AddWithValue("@TotalFinalAmount", 0);
                if (!string.IsNullOrEmpty(obj.CGSTAmount.ToString()))
                    com.Parameters.AddWithValue("@CGSTAmount", obj.CGSTAmount);
                else
                    com.Parameters.AddWithValue("@CGSTAmount", 0);
                if (!string.IsNullOrEmpty(obj.SGSTAmount.ToString()))
                    com.Parameters.AddWithValue("@SGSTAmount", obj.SGSTAmount);
                else
                    com.Parameters.AddWithValue("@SGSTAmount", 0);
                if (!string.IsNullOrEmpty(obj.IGSTAmount.ToString()))
                    com.Parameters.AddWithValue("@IGSTAmount", obj.IGSTAmount);
                else
                    com.Parameters.AddWithValue("@IGSTAmount", 0);

               

                if (!string.IsNullOrEmpty(obj.Profit.ToString()))
                    com.Parameters.AddWithValue("@Profit", obj.Profit);
                else
                    com.Parameters.AddWithValue("@Profit", 0);
                if (!string.IsNullOrEmpty(obj.PurchasePrice.ToString()))
                    com.Parameters.AddWithValue("@PurchasePrice", obj.PurchasePrice);
                else
                    com.Parameters.AddWithValue("@PurchasePrice", 0);


                if (!string.IsNullOrEmpty(obj.MRPPrice.ToString()))
                    com.Parameters.AddWithValue("@Mrp", obj.MRPPrice);
                else
                    com.Parameters.AddWithValue("@Mrp", 0);


                com.Parameters.AddWithValue("@CartId", obj.Id);


                
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int InsertGeneralPayment(Subscription obj)
        {
            DateTime today = DateTime.Now;

            int i = 0;
            con.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO tbl_generalPayment(CustomerId,PayDate,PayAmount,TransactionId)VALUES(@CustomerId,@PayDate,@PayAmount,@TransactionId)", con);
            //cmd.CommandType = CommandType.Text;

            if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", 0);
            if (!string.IsNullOrEmpty(obj.OrderDate.ToString()))
                cmd.Parameters.AddWithValue("@PayDate", obj.OrderDate);
            else
                cmd.Parameters.AddWithValue("@PayDate",DBNull.Value);


            cmd.Parameters.AddWithValue("@PayAmount", obj.GeneralPayAmount);
            cmd.Parameters.AddWithValue("@TransactionId", obj.GeneralPayTransactionId);

            i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }


        public int UpdateCustomerOrderCartStatus(Subscription obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("UPDATE tbl_Cart Set Status='Ordered' Where Id=@Id", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", obj.Id);


                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public int DeleteCart(Subscription obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Delete From tbl_Cart Where Id=@Id", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", obj.Id);


                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }
    }
}
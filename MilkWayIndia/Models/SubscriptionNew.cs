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
    public class SubscriptionNew
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
        public string Status { get; set; }

        public decimal TotalGSTAmt { get; set; }

        public int ProductId { get; set; }
        public int Qty { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Discount { get; set; }
        public long RewardPoint { get; set; }
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


        //Testing Code Start
        public int getOrederCustomerIdnew(string Id)
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


        public DataTable getCustomerWalletNew(int Id)
        {
            //con.Open();



            SqlCommand cmd = new SqlCommand("SELECT cw.CustomerId,sum(cw.Amount) as Amt,cw.[Type] from [dbo].[tbl_Customer_Wallet] cw left join [dbo].[tbl_Customer_Master] cm on cm.Id = cw.CustomerId WHERE (@CustomerId IS NULL OR cw.[CustomerId] = @CustomerId) Group by cw.CustomerId,cw.Type", con);
           // cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", Id);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public decimal GetCustomerBalaceNew(int Id)
        {
            decimal credit = 0, debit = 0;
            var dt = getCustomerWalletNew(Id);
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



        public DataTable getCustomerOnedateOrderListNew(int Id)
        {
            //con.Open();
           // SqlCommand cmd = new SqlCommand("select * from [tbl_Customer_Order_Transaction] ot left join [tbl_Customer_Order_Detail] od on od.OrderId = ot.Id where ot.Status='Pending' and ot.Id=@Id", con);


            SqlCommand cmd = new SqlCommand("select * from [tbl_Customer_Order_Transaction] ot left join [tbl_Customer_Order_Detail] od on od.OrderId = ot.Id where  ot.Id=@Id", con);
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


        public DataTable BindProuct(int? Id)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT pm.*,pcm.CategoryName from [dbo].[tbl_Product_Master] pm left join [dbo].[tbl_Product_Category_Master] pcm on pcm.Id = pm.CategoryId WHERE (@Id IS NULL OR pm.[Id] = @Id) Order By OrderBy ASc", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id",Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public int UpdateCustomerOrderMobileNew(SubscriptionNew obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("UPDATE [dbo].[tbl_Customer_Order_Transaction] SET [TotalAmount] = @TotalAmount WHERE Id = @Id", con);
                com.CommandType = CommandType.Text;
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

        public int UpdateCustomerOrderDetailMobileNew(SubscriptionNew obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("UPDATE [dbo].[tbl_Customer_Order_Detail] SET [Qty] = @Qty,[Amount] = @Amount,[Discount] = @Discount,[RewardPoint] = @RewardPoint,[TotalFinalAmount] = @TotalFinalAmount,[CGSTAmount] = @CGSTAmount,[SGSTAmount] = @SGSTAmount,[IGSTAmount] = @IGSTAmount,[Profit] = @Profit WHERE OrderId = @OrderId", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@OrderId", obj.OrderId);
                if (!string.IsNullOrEmpty(obj.Qty.ToString()))
                    com.Parameters.AddWithValue("@Qty", obj.Qty);
                else
                    com.Parameters.AddWithValue("@Qty", DBNull.Value);
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
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }
        //Testing code end




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


        public int UpdateCustomerOrderMobile(SubscriptionNew obj)
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


        public int UpdateCustomerOrderDetailMobile(SubscriptionNew obj)
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
    }
}
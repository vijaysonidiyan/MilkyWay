using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Models
{
    public class CustomerOrderVendor
    {


        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public int SocietyId { get; set; }


        //order detail
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Qty { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public long RewardPoint { get; set; }

        public int StaffId { get; set; }
        public int SectorId { get; set; }
        public int BuildingId { get; set; }
        public int DaysId { get; set; }
        public int VendorId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public DataTable getDeliveryBoyCustomerOrder(int? DeliveryboyId, int? CustomerId, DateTime? FDate, DateTime? TDate, string status)
        {
            if (DeliveryboyId == 0) DeliveryboyId = null;
            if (CustomerId == 0) CustomerId = null;
            if (status == "0") status = null;
            //con.Open();

            FDate = DateTime.Today.AddDays(1);
            TDate = DateTime.Today.AddDays(1);

            SqlCommand cmd = new SqlCommand("Sector_Staff_Order_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DeliveryboyId);
            else
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DBNull.Value);
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(FDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(TDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", TDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            if (!string.IsNullOrEmpty(status))
                cmd.Parameters.AddWithValue("@OrderStatus", status);
            else
                cmd.Parameters.AddWithValue("@OrderStatus", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getDeliveryBoyWiseOrdervendorsector(int? DeliveryboyId, int? CustomerId, DateTime? FDate, DateTime? TDate, string status)
        {
            if (DeliveryboyId == 0) DeliveryboyId = null;
            if (CustomerId == 0) CustomerId = null;
            if (status == "0") status = null;
            //con.Open();


            // SqlCommand cmd = new SqlCommand("SP_Sector_Staff_Order_SelectAll", con);

            //SqlCommand cmd = new SqlCommand("SELECT DISTINCT(secm.Id) as sid from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (@CustomerId IS NULL OR otrans.CustomerId=@CustomerId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND odetail.Qty <> 0 AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus)", con);

            SqlCommand cmd = new SqlCommand("SELECT DISTINCT(secm.Id) as sid from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (@CustomerId IS NULL OR otrans.CustomerId=@CustomerId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND odetail.Qty <> 0 AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus)", con);
            //  cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DeliveryboyId);
            else
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DBNull.Value);
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(FDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(TDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", TDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            if (!string.IsNullOrEmpty(status))
                cmd.Parameters.AddWithValue("@OrderStatus", status);
            else
                cmd.Parameters.AddWithValue("@OrderStatus", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}
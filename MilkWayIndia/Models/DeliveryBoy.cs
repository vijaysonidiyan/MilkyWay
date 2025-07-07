using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Models
{
    public class DeliveryBoy
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);


        //master
        public int Id { get; set; }
        public string Name { get; set; }
        public int Days { get; set; }
        public decimal Amount { get; set; }


        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public int SectorId { get; set; }
        public int BuildingId { get; set; }
        public string Photo { get; set; }
        public string base64Image { get; set; }
        public int FlatId { get; set; }
        public DateTime SubnFromDate { get; set; }
        public DateTime SubnToDate { get; set; }
        public string fcm_token { get; set; }
        public long RewardPoint { get; set; }
        public string ReferralCode { get; set; }
        public int ReferralID { get; set; }
        public Decimal Credit { get; set; }
        public int OrderBy { get; set; }

        //assign delivery boy
        public int DeliveryBoyId { get; set; }
        public int CustomerId { get; set; }
        public int StaffId { get; set; }

        //otp 
        public string OTP { get; set; }
        public DateTime LastUpdateOtpDate { get; set; }
        public int Count { get; set; }



        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }


        //customer subscription

        public int SubscriptionId { get; set; }

        public string PaymentStatus { get; set; }
        public string SubscriptionStatus { get; set; }
        public string Description { get; set; }
        //Order
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public int CustSubscriptionId { get; set; }
        public string StateCode { get; set; }
        public string Status { get; set; }

        public decimal TotalGSTAmt { get; set; }

        public int ProductId { get; set; }
        public int Qty { get; set; }
        public int newqty { get; set; }
        public int OrderId { get; set; }
        public string productname { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Discount { get; set; }

        public decimal TotalFinalAmount { get; set; }
        public decimal CGSTAmount { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal IGSTAmount { get; set; }
        public DateTime OrderItemDate { get; set; }
        public DateTime Updatedon { get; set; }
        public decimal CGSTPerct { get; set; }
        public decimal SGSTPerct { get; set; }
        public decimal IGSTPerct { get; set; }
        public decimal Profit { get; set; }
        public string OrderFlag { get; set; }
        public string week { get; set; }
        public decimal MRPPrice { get; set; }






        public string Aadhar { get; set; }
        public string base64Aadhar { get; set; }
        public string Pan { get; set; }
        public string base64Pan { get; set; }
        public string License { get; set; }
        public string base64License { get; set; }

        public string bankaccount { get; set; }
        public string ifsc { get; set; }
        public string bankname { get; set; }
        public string Accholdername { get; set; }


        public string Aadharstatus { get; set; }
        public string Panstatus { get; set; }
        public string Licensestatus { get; set; }
        public string Bankstatus { get; set; }
        clsCommon _clsCommon = new clsCommon();
        Helper dHelper = new Helper();
        public DataTable CheckDeliveryBoyUserName(string UserName)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[tbl_Staff_Master] WHERE (@UserName IS NULL OR MobileNo = @UserName) AND Role='DeliveryBoy'", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(UserName))
                cmd.Parameters.AddWithValue("@UserName", UserName);
            else
                cmd.Parameters.AddWithValue("@UserName", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable DeliveryBoylogin(string UserName, string Password)
        {
            con.Open();
            SqlCommand cmdLoginUser = new SqlCommand("select * from tbl_Staff_Master WHERE MobileNo='" + UserName + "' and Password='" + Password + "'", con);
            SqlDataAdapter daLoginUser = new SqlDataAdapter(cmdLoginUser);
            DataTable dtLoginUser = new DataTable();
            daLoginUser.Fill(dtLoginUser);
            con.Close();
            return dtLoginUser;
        }

        public DataTable DeliveryBoylogin2(int CustomerId, string Password)
        {
            con.Open();
            SqlCommand cmdLoginUser = new SqlCommand("select * from tbl_Staff_Master WHERE Id=" + CustomerId + " and Password='" + Password + "'", con);
            SqlDataAdapter daLoginUser = new SqlDataAdapter(cmdLoginUser);
            DataTable dtLoginUser = new DataTable();
            daLoginUser.Fill(dtLoginUser);
            con.Close();
            return dtLoginUser;
        }
        public int UpdateDmPwd(int Id, string Password)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Update tbl_Staff_Master set Password=@Password where Id=@Id", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", Id);
                com.Parameters.AddWithValue("@Password", Password);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public DataTable BindDeliverboy(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Customer_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
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
        public DataTable getDeliveryBoyWiseCustomerList(int? DeliveryboyId)
        {

            if (DeliveryboyId == 0) DeliveryboyId = null;
            ToDate = DateTime.Today;
            FromDate = DateTime.Today;
            string query = "SELECT DISTINCT cm.Id AS CustomerId,sm.FirstName +' '+ sm.LastName as DeliveryBoy,secm.SectorName,";
            query += " cm.FirstName +' '+ cm.LastName as Customer,BM.orderBy, fm.orderBy from [dbo].[tbl_Staff_Master] sm";
            query += " left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id";
            query += " left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId";
            query += " left join [dbo].[tbl_Building_Master] bm on bm.Id = cm.BuildingId";
            query += " left join [dbo].[tbl_Building_Flat_Master] fm on fm.Id = cm.FlatId";
            query += " left join [dbo].[tbl_Sector_Master] secm on secm.Id = bm.SectorId";


            query += " left join[dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id";
            query += " left join[dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id";
            query += " left join[dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId";
            query += " WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId)";
            query += " and (@FromDate is null or @ToDate is null or convert(varchar,otrans.Orderdate,23) between @FromDate and @Todate) and odetail.Qty<>0";
            query += " Order by  BM.orderBy, fm.orderBy";



            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DeliveryboyId);
            else
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DBNull.Value);

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

            return dt;

        }

        public DataTable getDeliveryBoyWiseCustomerOrder(int? DeliveryBoyId, DateTime? Fromdate, DateTime? Todate)
        {

            if (DeliveryBoyId == 0) DeliveryBoyId = null;

            string status = "Pending";
            string status1 = "Complete";
            int NoDays = 0;
            ToDate = Todate;
            FromDate = Fromdate;
            //ToDate = DateTime.Today.AddDays(1);
            //FromDate = DateTime.Today.AddDays(1);
            //string query = "SELECT otrans.Id AS Id,cm.Id as CustomerId,secm.SectorName,cm.FirstName +' '+ cm.LastName as Customer,";
            //query += " cm.Address AS Address,cm.MobileNo As MobileNo,otrans.OrderNo,convert(varchar,otrans.OrderDate,23) as OrderDate,prodt.ProductName,prodt.Image as image,";
            //query += " odetail.Qty,odetail.Amount,odetail.Discount,odetail.RewardPoint,odetail.CGSTAmount,odetail.SGSTAmount,odetail.IGSTAmount,odetail.Profit,cm.Address";
            //query += " ,bm.BuildingName,bm.BlockNo,cast(fm.FlatNo as int) as FlatNo,otrans.[Status] As status,cm.orderby as orderby,secm.Id as sid,otrans.DeliveryStatus As DeliveryStatus";
            //query += " From [dbo].[tbl_Customer_Master] cm left join [dbo].[tbl_Building_Master] bm on bm.Id = cm.BuildingId";
            //query += " left join [dbo].[tbl_Building_Flat_Master] fm on fm.Id = cm.FlatId";
            //query += " left join [dbo].[tbl_Sector_Master] secm on secm.Id = bm.SectorId";
            //query += " left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id";
            //query += " left join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id";
            //query += " left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId";
            //query += " WHERE (@CustomerId is null or cm.Id=@CustomerId) AND (@CustomerId is null or otrans.CustomerId=@CustomerId)";
            //query += " AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate)";
            //query += " and odetail.Qty<>0 Order by  BM.orderBy, fm.orderBy";


            //ToDate = DateTime.Today.AddDays(-43);
            //FromDate = DateTime.Today.AddDays(-43);
            //SqlCommand cmd = new SqlCommand("SELECT otrans.Id AS Id,sm.FirstName +' '+ sm.LastName as DeliveryBoy,secm.SectorName,cm.FirstName +' '+ cm.LastName as Customer, otrans.OrderNo,convert(varchar,otrans.OrderDate,23) as OrderDate, prodt.ProductName, odetail.Qty,odetail.Amount,cm.Address, otrans.[Status],cm.orderby,secm.Id as sid from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (@CustomerId IS NULL OR otrans.CustomerId=@CustomerId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND odetail.Qty <> 0 AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus) 	order by cm.OrderBy,Customer", con);

           // SqlCommand cmd = new SqlCommand("SELECT otrans.Id AS Id,cm.Id as CustomerId,sm.FirstName +' '+ sm.LastName as DeliveryBoy,secm.SectorName,cm.FirstName +' '+ cm.LastName as Customer,cm.Address AS Address,cm.MobileNo As MobileNo, otrans.OrderNo,convert(varchar,otrans.OrderDate,23) as OrderDate,prodt.Id As Proid, prodt.ProductName,prodt.Image as image, odetail.Qty,odetail.Amount,cm.Address, otrans.[Status] As status,cm.OrderBy as orderby,secm.Id as sid,otrans.DeliveryStatus As DeliveryStatus,cm.lat,cm.lon,Dp.newqty from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId left join tbl_Dm_ProductUpdate Dp On Dp.OrderId=otrans.Id  WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate)  AND odetail.Qty <> 0 AND (otrans.[Status] = @OrderStatus Or otrans.[Status] = @OrderStatus1)	order by cm.OrderBy", con);

            // SqlCommand cmd = new SqlCommand(query, con);

            //NewVersion 22-12-2022

            string query = " SELECT otrans.Id AS Id,cm.Id as CustomerId,sm.FirstName +' '+ sm.LastName as DeliveryBoy,secm.SectorName,cm.FirstName +' '+ cm.LastName as Customer,";
            query += " cm.Address AS Address,cm.MobileNo As MobileNo, otrans.OrderNo,convert(varchar,otrans.OrderDate,23) as OrderDate,prodt.Id As Proid, ";
            query += " prodt.ProductName,prodt.Image as image, odetail.Qty,odetail.Amount,cm.Address, otrans.[Status] As status,cm.OrderBy as orderby,";
            query += " secm.Id as sid,otrans.DeliveryStatus As DeliveryStatus,cm.lat,cm.lon,Dp.newqty,At.Name ";
            query += " from [tbl_Customer_Order_Transaction] otrans ";
            query += " Inner join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id ";
            query += " Inner join [dbo].[tbl_Customer_Master] cm on cm.Id = otrans.CustomerId ";
            query += " Inner join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId ";
            query += " Inner join tbl_Product_Attributes Pat on Pat.ProductID = odetail.ProductId And Pat.AttributeID=odetail.AttributeId ";
            query += " Inner Join tbl_Attributes At On At.ID=odetail.AttributeId ";
            query += " Inner join tbl_Staff_Master Sm On Sm.Id=odetail.DeliveryBoyId ";
            query += " Inner join [tbl_Sector_Master] secm on secm.Id = odetail.SectorId";
            query += " Left join tbl_Dm_ProductUpdate Dp On Dp.OrderId=otrans.Id";
            query += " WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate)  AND odetail.Qty <> 0 AND (otrans.[Status] = @OrderStatus Or otrans.[Status] = @OrderStatus1) order by cm.OrderBy";


            SqlCommand cmd = new SqlCommand(query, con);

            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(DeliveryBoyId.ToString()))
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DeliveryBoyId);
            else
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DBNull.Value);

            if (!string.IsNullOrEmpty(FromDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FromDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(ToDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", ToDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            if (!string.IsNullOrEmpty(status))
                cmd.Parameters.AddWithValue("@OrderStatus", status);
            else
                cmd.Parameters.AddWithValue("@OrderStatus", DBNull.Value);

            if (!string.IsNullOrEmpty(status1))
                cmd.Parameters.AddWithValue("@OrderStatus1", status1);
            else
                cmd.Parameters.AddWithValue("@OrderStatus1", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;

        }


        //

        public DataTable getTest(int? CustomerId)
        {



            string status = "Pending";
            //string status = "Complete";
            int NoDays = 0;
            ToDate = DateTime.Today.AddDays(1);
            FromDate = DateTime.Today.AddDays(1);
            //ToDate = DateTime.Today.AddDays(-43);
            //FromDate = DateTime.Today.AddDays(-43);
            //SqlCommand cmd = new SqlCommand("SELECT otrans.Id AS Id,sm.FirstName +' '+ sm.LastName as DeliveryBoy,secm.SectorName,cm.FirstName +' '+ cm.LastName as Customer, otrans.OrderNo,convert(varchar,otrans.OrderDate,23) as OrderDate, prodt.ProductName, odetail.Qty,odetail.Amount,cm.Address, otrans.[Status],cm.orderby,secm.Id as sid from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (@CustomerId IS NULL OR otrans.CustomerId=@CustomerId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND odetail.Qty <> 0 AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus) 	order by cm.OrderBy,Customer", con);

            SqlCommand cmd = new SqlCommand("SELECT * from [tbl_Cusomter_Order_Track] where CustomerId=@CustomerId", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;

        }


        //


        public DataTable getDeliveryBoyWiseTomCustomerOrder(int? DeliveryboyId)
        {

            if (DeliveryboyId == 0) DeliveryboyId = null;

            string status = "Pending";
            //string status = "Complete";
            int NoDays = 0;
            ToDate = DateTime.Today.AddDays(1);
            FromDate = DateTime.Today.AddDays(1);
            //ToDate = DateTime.Today.AddDays(-43);
            //FromDate = DateTime.Today.AddDays(-43);
            //SqlCommand cmd = new SqlCommand("SELECT otrans.Id AS Id,sm.FirstName +' '+ sm.LastName as DeliveryBoy,secm.SectorName,cm.FirstName +' '+ cm.LastName as Customer, otrans.OrderNo,convert(varchar,otrans.OrderDate,23) as OrderDate, prodt.ProductName, odetail.Qty,odetail.Amount,cm.Address, otrans.[Status],cm.orderby,secm.Id as sid from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (@CustomerId IS NULL OR otrans.CustomerId=@CustomerId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND odetail.Qty <> 0 AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus) 	order by cm.OrderBy,Customer", con);
            //otrans.DeliveryStatus As DeliveryStatus
            SqlCommand cmd = new SqlCommand("SELECT otrans.Id AS Id,cm.Id as CustomerId,sm.FirstName +' '+ sm.LastName as DeliveryBoy,secm.SectorName,cm.FirstName +' '+ cm.LastName as Customer,cm.Address AS Address,cm.MobileNo As MobileNo, otrans.OrderNo,convert(varchar,otrans.OrderDate,23) as OrderDate,prodt.Id As Proid, prodt.ProductName,prodt.Image as image, odetail.Qty,odetail.Amount,cm.Address, otrans.[Status] As status,cm.OrderBy as orderby,secm.Id as sid,otrans.DeliveryStatus As DeliveryStatus,cm.lat,cm.lon from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate)  AND odetail.Qty <> 0 	order by cm.OrderBy", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DeliveryboyId);
            else
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DBNull.Value);

            if (!string.IsNullOrEmpty(FromDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FromDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(ToDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", ToDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            //if (!string.IsNullOrEmpty(status))
            //    cmd.Parameters.AddWithValue("@OrderStatus", status);
            //else
            //    cmd.Parameters.AddWithValue("@OrderStatus", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;

        }

        //Vendor Order

        public DataTable getDeliveryBoyWiseOrdervendorsector(int? DeliveryboyId, DateTime? FDate, DateTime? TDate)
        {
            if (DeliveryboyId == 0) DeliveryboyId = null;

            string status = "Pending";
            //string status = "Complete";
            //con.Open();


            // SqlCommand cmd = new SqlCommand("SP_Sector_Staff_Order_SelectAll", con);

            //SqlCommand cmd = new SqlCommand("SELECT DISTINCT(secm.Id) as sid from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (@CustomerId IS NULL OR otrans.CustomerId=@CustomerId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND odetail.Qty <> 0 AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus)", con);

            // SqlCommand cmd = new SqlCommand("SELECT DISTINCT(secm.Id) as sid from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (@CustomerId IS NULL OR otrans.CustomerId=@CustomerId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND odetail.Qty <> 0 AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus)", con);
            //  cmd.CommandType = CommandType.StoredProcedure;

            SqlCommand cmd = new SqlCommand("SELECT DISTINCT(secm.Id) as sid from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND odetail.Qty <> 0 AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate)", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DeliveryboyId);
            else
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DBNull.Value);

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


        public DataTable GetMultiSectorVendorOrder1(string SectorId, int? DeliveryboyId, DateTime? Fromdate, DateTime? Todate)
        {



            ToDate = Todate;
            FromDate = Fromdate;
            //ToDate = DateTime.Today.AddDays(-43);
            //FromDate = DateTime.Today.AddDays(-43);

            string status = "Pending";
            //string status = "Complete";
            //SqlCommand cmd = new SqlCommand("SELECT MAX( DISTINCT otrans.OrderNo) as OrderNo,MAX(CONVERT(VARCHAR,otrans.OrderDate,23)) AS OrderDate,SUM(odetail.Qty) AS Qty, MAX(vm.FirstName + ' '+ vm.LastName) AS Vendor,MAX(prodt.ProductName) AS ProductName,	MAX(SectorName) AS Sector,(MAX(prodt.PurchasePrice)*SUM(odetail.Qty)) AS PurchasePrice FROM [dbo].[tbl_Staff_Master] sm left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId left join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id AND vpa.SectorId=cm.SectorId left join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId WHERE (cm.SectorId IN (SELECT Value FROM fn_Split_Sector(@SectorId, ','))) AND  (CONVERT(VARCHAR,otrans.Orderdate,23) >= @FromDate) AND (CONVERT(VARCHAR,otrans.Orderdate,23) <=@ToDate) GROUP BY prodt.Id,SectorName ORDER BY Sector,Vendor", con);
            // cmd.CommandType = CommandType.StoredProcedure;

            //  SqlCommand cmd = new SqlCommand("SELECT MAX( DISTINCT otrans.OrderNo) as OrderNo,MAX(CONVERT(VARCHAR,otrans.OrderDate,23)) AS OrderDate,SUM(odetail.Qty) AS Qty, MAX(vm.FirstName + ' '+ vm.LastName) AS Vendor,MAX(prodt.ProductName) AS ProductName,	MAX(SectorName) AS Sector,(MAX(prodt.PurchasePrice)*SUM(odetail.Qty)) AS PurchasePrice FROM [dbo].[tbl_Staff_Master] sm left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId left join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id AND vpa.SectorId=cm.SectorId left join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId WHERE (cm.SectorId IN (SELECT Value FROM fn_Split_Sector(@SectorId, ','))) AND  (CONVERT(VARCHAR,otrans.Orderdate,23) >= @FromDate) AND (CONVERT(VARCHAR,otrans.Orderdate,23) <=@ToDate) GROUP BY prodt.Id,SectorName ORDER BY Sector,Vendor", con);

            //SqlCommand cmd = new SqlCommand("SELECT MAX( DISTINCT otrans.OrderNo) as OrderNo,MAX(CONVERT(VARCHAR,otrans.OrderDate,23)) AS OrderDate,SUM(odetail.Qty) AS Qty, MAX(vm.FirstName + ' '+ vm.LastName) AS Vendor,MAX(vm.MobileNo) AS MobileNo,MAX(prodt.ProductName) AS ProductName,	MAX(SectorName) AS Sector,(MAX(prodt.PurchasePrice)*SUM(odetail.Qty)) AS PurchasePrice FROM [dbo].[tbl_Staff_Master] sm left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId left join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id AND vpa.SectorId=cm.SectorId left join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) And (cm.SectorId IN (SELECT Value FROM fn_Split_Sector(@SectorId, ','))) AND  (CONVERT(VARCHAR,otrans.Orderdate,23) >= @FromDate) AND (CONVERT(VARCHAR,otrans.Orderdate,23) <=@ToDate) AND odetail.Qty <> 0  GROUP BY prodt.Id,SectorName ORDER BY Sector,Vendor", con);

            SqlCommand cmd = new SqlCommand("SELECT MAX( DISTINCT otrans.OrderNo) as OrderNo,MAX(CONVERT(VARCHAR,otrans.OrderDate,23)) AS OrderDate,SUM(odetail.Qty) AS Qty, MAX(vm.FirstName + ' '+ vm.LastName) AS Vendor,MAX(vm.MobileNo) AS MobileNo,MAX(prodt.ProductName) AS ProductName,	MAX(SectorName) AS Sector,(MAX(prodt.PurchasePrice)*SUM(odetail.Qty)) AS PurchasePrice FROM [dbo].[tbl_Staff_Master] sm left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id Inner join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id and (otrans.Status='Complete' Or otrans.Status='Pending') left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId left join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id AND vpa.SectorId=cm.SectorId left join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) And (cm.SectorId IN (SELECT Value FROM fn_Split_Sector(@SectorId, ','))) AND  (CONVERT(VARCHAR,otrans.Orderdate,23) >= @FromDate) AND (CONVERT(VARCHAR,otrans.Orderdate,23) <=@ToDate) AND odetail.Qty <> 0  GROUP BY prodt.Id,SectorName ORDER BY Sector,Vendor", con);

            if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DeliveryboyId);
            else
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DBNull.Value);
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(FromDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FromDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(ToDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", ToDate);
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




        public DataTable getDeliveryBoyWiseFilterCustomerOrder(int? DeliveryboyId, string status, DateTime? Fromdate, DateTime? Todate)
        {

            if (DeliveryboyId == 0) DeliveryboyId = null;
            if (status == "0") status = null;

            //string status = "Complete";
            int NoDays = 0;

            //ToDate = DateTime.Today.AddDays(-43);
            //FromDate = DateTime.Today.AddDays(-43);
            //SqlCommand cmd = new SqlCommand("SELECT otrans.Id AS Id,sm.FirstName +' '+ sm.LastName as DeliveryBoy,secm.SectorName,cm.FirstName +' '+ cm.LastName as Customer, otrans.OrderNo,convert(varchar,otrans.OrderDate,23) as OrderDate, prodt.ProductName, odetail.Qty,odetail.Amount,cm.Address, otrans.[Status],cm.orderby,secm.Id as sid from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (@CustomerId IS NULL OR otrans.CustomerId=@CustomerId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND odetail.Qty <> 0 AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus) 	order by cm.OrderBy,Customer", con);

            SqlCommand cmd = new SqlCommand("SELECT otrans.Id AS Id,sm.FirstName +' '+ sm.LastName as DeliveryBoy,secm.SectorName,cm.FirstName +' '+ cm.LastName as Customer, otrans.OrderNo,convert(varchar,otrans.OrderDate,23) as OrderDate, prodt.ProductName, odetail.Qty,odetail.Amount,cm.Address, otrans.[Status],cm.orderby,secm.Id as sid from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND odetail.Qty <> 0 AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus) 	order by cm.OrderBy,Customer", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DeliveryboyId);
            else
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DBNull.Value);

            if (!string.IsNullOrEmpty(Fromdate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", Fromdate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(Todate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", Todate);
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


        public int UpdateCustomerOrderMain(DeliveryBoy obj, string Ctime)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("UPDATE [dbo].[tbl_Customer_Order_Transaction] set DeliveryStatus = @Status,DeliveryTime=@DeliveryTime Where Id = @Id", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", obj.Id);

                if (!string.IsNullOrEmpty(obj.Status))
                    com.Parameters.AddWithValue("@Status", obj.Status);
                else
                    com.Parameters.AddWithValue("@Status", DBNull.Value);


                if (!string.IsNullOrEmpty(Ctime))
                    com.Parameters.AddWithValue("@DeliveryTime", Ctime);
                else
                    com.Parameters.AddWithValue("@DeliveryTime", DBNull.Value);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }



        public DataTable getDeliveryBoyWiseOrdervendorsector1(int? DeliveryboyId)
        {
            if (DeliveryboyId == 0) DeliveryboyId = null;
            SqlCommand cmd = new SqlCommand("SELECT DISTINCT(secm.Id) as sid,secm.SectorName as SectorName from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND odetail.Qty <> 0 ", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DeliveryboyId);
            else
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DBNull.Value);

            //if (!string.IsNullOrEmpty(status))
            //    cmd.Parameters.AddWithValue("@OrderStatus", status);
            //else
            //    cmd.Parameters.AddWithValue("@OrderStatus", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public DataTable GetCustomerSortOrder(int? DeliveryboyId, int? sid)
        {
            if (sid == 0) sid = null;
            if (DeliveryboyId == 0) DeliveryboyId = null;
            con.Open();
            SqlCommand cmd = new SqlCommand("SP_Customer_Sector_Staff", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(sid.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", sid);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                cmd.Parameters.AddWithValue("@StaffID", DeliveryboyId);
            else
                cmd.Parameters.AddWithValue("@StaffID", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public int UpdateCustomerSortOrder(DeliveryBoy obj, int? currentsortorder, int? nextsortorder, int? DeliveryboyId)
        {
            int so = 0, so1 = 0;
            con.Open();
            DataTable dt = new DataTable();




            if (currentsortorder < nextsortorder)
            {

                //OrderBy != currentsortorder;AND SectorId=@SectorID


                string j = currentsortorder.ToString();

                int k = Convert.ToInt32(j);

                if (currentsortorder == 0)
                {
                    SqlCommand cmd2 = new SqlCommand("UPDATE c SET OrderBy = OrderBy + 1 FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id = d.CustomerId WHERE OrderBy!=@OldSortOrder And OrderBy!=0 AND OrderBy>=@NewSortOrder AND c.Id != @CustomerId AND IsDeleted='False'  AND d.StaffId=@StaffId", con);


                    cmd2.CommandType = CommandType.Text;


                    if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                        cmd2.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                    else
                        cmd2.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                    if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                        cmd2.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                    else
                        cmd2.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                    if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                        cmd2.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                    else
                        cmd2.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                    //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    //    cmd2.Parameters.AddWithValue("@SectorID", obj.SectorId);
                    //else
                    //    cmd2.Parameters.AddWithValue("@SectorID", DBNull.Value);
                    if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                        cmd2.Parameters.AddWithValue("@StaffId", DeliveryboyId);
                    else
                        cmd2.Parameters.AddWithValue("@StaffId", DBNull.Value);

                    so = cmd2.ExecuteNonQuery();


                    SqlCommand cmd3 = new SqlCommand("UPDATE c SET OrderBy = @NewSortOrder FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id = d.CustomerId WHERE OrderBy = @OldSortOrder AND c.Id = @CustomerId AND IsDeleted='False' AND d.StaffId=@StaffId", con);


                    cmd3.CommandType = CommandType.Text;


                    if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                        cmd3.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                    else
                        cmd3.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                    if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                        cmd3.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                    else
                        cmd3.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                    if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                        cmd3.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                    else
                        cmd3.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                    //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    //    cmd3.Parameters.AddWithValue("@SectorID", obj.SectorId);
                    //else
                    //    cmd3.Parameters.AddWithValue("@SectorID", DBNull.Value);
                    if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                        cmd3.Parameters.AddWithValue("@StaffId", DeliveryboyId);
                    else
                        cmd3.Parameters.AddWithValue("@StaffId", DBNull.Value);
                    so1 = cmd3.ExecuteNonQuery();
                    con.Close();
                }
                else
                {


                    for (int i = k; i <= nextsortorder; i++)
                    {
                        SqlCommand cmd = new SqlCommand("UPDATE c SET OrderBy = OrderBy - 1 FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id = d.CustomerId WHERE OrderBy=@i1 And OrderBy!=0 AND c.Id != @CustomerId AND IsDeleted='False'  AND d.StaffId=@StaffId", con);


                        cmd.CommandType = CommandType.Text;

                        if (!string.IsNullOrEmpty(i.ToString()))
                            cmd.Parameters.AddWithValue("@i1", i);
                        else
                            cmd.Parameters.AddWithValue("@i1", DBNull.Value);
                        if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                            cmd.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                        else
                            cmd.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                        if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                            cmd.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                        else
                            cmd.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                        if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                            cmd.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                        else
                            cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                        //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                        //    cmd.Parameters.AddWithValue("@SectorID", obj.SectorId);
                        //else
                        //    cmd.Parameters.AddWithValue("@SectorID", DBNull.Value);
                        if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                            cmd.Parameters.AddWithValue("@StaffId", DeliveryboyId);
                        else
                            cmd.Parameters.AddWithValue("@StaffId", DBNull.Value);

                        so = cmd.ExecuteNonQuery();




                    }

                    SqlCommand cmd1 = new SqlCommand("UPDATE c SET OrderBy = @NewSortOrder FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id = d.CustomerId WHERE OrderBy = @OldSortOrder AND c.Id = @CustomerId AND IsDeleted='False'  AND d.StaffId=@StaffId", con);


                    cmd1.CommandType = CommandType.Text;


                    if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                        cmd1.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                    else
                        cmd1.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                    if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                        cmd1.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                    else
                        cmd1.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                    if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                        cmd1.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                    else
                        cmd1.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                    //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    //    cmd1.Parameters.AddWithValue("@SectorID", obj.SectorId);
                    //else
                    //    cmd1.Parameters.AddWithValue("@SectorID", DBNull.Value);
                    if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                        cmd1.Parameters.AddWithValue("@StaffId", DeliveryboyId);
                    else
                        cmd1.Parameters.AddWithValue("@StaffId", DBNull.Value);
                    so1 = cmd1.ExecuteNonQuery();
                    con.Close();
                }
            }


            if (currentsortorder > nextsortorder)
            {
                string j = currentsortorder.ToString();

                int k = Convert.ToInt32(j);
                for (int i = k; i >= nextsortorder; i--)
                {
                    SqlCommand cmd = new SqlCommand("UPDATE c SET OrderBy = OrderBy + 1 FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id = d.CustomerId WHERE OrderBy=@i1 And OrderBy!=0 AND c.Id != @CustomerId AND IsDeleted='False'  AND d.StaffId=@StaffId", con);


                    cmd.CommandType = CommandType.Text;

                    if (!string.IsNullOrEmpty(i.ToString()))
                        cmd.Parameters.AddWithValue("@i1", i);
                    else
                        cmd.Parameters.AddWithValue("@i1", DBNull.Value);
                    if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                        cmd.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                    else
                        cmd.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                    if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                        cmd.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                    else
                        cmd.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                    if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                        cmd.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                    else
                        cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                    //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    //    cmd.Parameters.AddWithValue("@SectorID", obj.SectorId);
                    //else
                    //    cmd.Parameters.AddWithValue("@SectorID", DBNull.Value);
                    if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                        cmd.Parameters.AddWithValue("@StaffId", DeliveryboyId);
                    else
                        cmd.Parameters.AddWithValue("@StaffId", DBNull.Value);

                    so = cmd.ExecuteNonQuery();




                }


                SqlCommand cmd1 = new SqlCommand("UPDATE c SET OrderBy = @NewSortOrder FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id = d.CustomerId WHERE OrderBy = @OldSortOrder AND c.Id = @CustomerId AND IsDeleted='False'  AND d.StaffId=@StaffId", con);


                cmd1.CommandType = CommandType.Text;


                if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                    cmd1.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                else
                    cmd1.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                    cmd1.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                else
                    cmd1.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                    cmd1.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                else
                    cmd1.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                //    cmd1.Parameters.AddWithValue("@SectorID", obj.SectorId);
                //else
                //    cmd1.Parameters.AddWithValue("@SectorID", DBNull.Value);
                if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                    cmd1.Parameters.AddWithValue("@StaffId", DeliveryboyId);
                else
                    cmd1.Parameters.AddWithValue("@StaffId", DBNull.Value);
                so1 = cmd1.ExecuteNonQuery();

                con.Close();
            }




            return so;



        }

        public int InsertProductPhoto(DeliveryBoy obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("INSERT INTO tbl_DM_Product_Photo(Photo,Description,Uploaddate,DmId)VALUES(@Photo,@Description,@Uploaddate,@DmId)", con);
                com.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(obj.Photo))
                    com.Parameters.AddWithValue("@Photo", obj.Photo);
                else
                    com.Parameters.AddWithValue("@Photo", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.Description))
                    com.Parameters.AddWithValue("@Description", obj.Description);
                else
                    com.Parameters.AddWithValue("@Description", DBNull.Value);

                com.Parameters.AddWithValue("@Uploaddate", DateTime.Now);

                if (!string.IsNullOrEmpty(obj.DeliveryBoyId.ToString()))
                    com.Parameters.AddWithValue("@DmId", obj.DeliveryBoyId);
                else
                    com.Parameters.AddWithValue("@DmId", DBNull.Value);
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }




        public int InsertDMStatus(int? DeliveryboyId, string status, string Ctime, DateTime? Fromdate)
        {
            int i = 0;
            try
            {
                FromDate = Fromdate;
                con.Open();
                SqlCommand com = new SqlCommand("INSERT INTO tbl_DM_CurrentStatus(DMid,startdate,CurrentStatus,CurrentTime)VALUES(@DMid,@startdate,@CurrentStatus,@CurrentTime)", con);
                com.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                    com.Parameters.AddWithValue("@DMid", DeliveryBoyId);
                else
                    com.Parameters.AddWithValue("@DMid", DBNull.Value);

                if (!string.IsNullOrEmpty(status))
                    com.Parameters.AddWithValue("@CurrentStatus", status);
                else
                    com.Parameters.AddWithValue("@CurrentStatus", DBNull.Value);
                if (!string.IsNullOrEmpty(Ctime))
                    com.Parameters.AddWithValue("@CurrentTime", Ctime);
                else
                    com.Parameters.AddWithValue("@CurrentTime", DBNull.Value);

                com.Parameters.AddWithValue("@startdate", Helper.indianTime);

                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }



        public int UpdateCash(int CustomerId, int id, int Dmid, DateTime? Fromdate, string CollectAmount, string Description)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("UPDATE [dbo].[tbl_Customer_Cash_Request] set DMId = @DMId,CollectDate=@CollectDate,CollectAmount=@CollectAmount,Description=@Description Where Id = @Id", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", id);
                com.Parameters.AddWithValue("@DMId", Dmid);
                com.Parameters.AddWithValue("@CollectDate", Fromdate);
                com.Parameters.AddWithValue("@CollectAmount", CollectAmount);
                com.Parameters.AddWithValue("@Description", Description);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;
        }


        public int UpdateCashsingle(int? id, string CollectAmount)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("UPDATE [dbo].[tbl_Customer_Cash_Request] set CollectAmount=@CollectAmount Where Id = @Id", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", id);
                com.Parameters.AddWithValue("@CollectAmount", CollectAmount);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;
        }

        public int InsertDoc(DeliveryBoy obj)
        {

            int i = 0;
            ToDate = DateTime.Today;
            string Status = "Pending";
            string Description = "";
            con.Open();
            SqlCommand com = new SqlCommand("Insert Into tbl_Dm_doc(DmId,Aadhar,Pan,License,BankAccount,Ifsc,Bankname,AccholderName,Aadharstatus,Panstatus,Licensestatus,Bankstatus,Status,Decsiption,Updatedon)Values(@DmId,@Aadhar,@Pan,@License,@BankAccount,@Ifsc,@Bankname,@AccholderName,@Aadharstatus,@Panstatus,@Licensestatus,@Bankstatus,@Status,@Description,@Updatedon)", con);
            com.CommandType = CommandType.Text;
            com.Parameters.AddWithValue("@DmId", obj.DeliveryBoyId);
            com.Parameters.AddWithValue("@Aadhar", obj.Aadhar);
            com.Parameters.AddWithValue("@Pan", obj.Pan);

            com.Parameters.AddWithValue("@License", obj.License);
            com.Parameters.AddWithValue("@BankAccount", obj.bankaccount);
            com.Parameters.AddWithValue("@Ifsc", obj.ifsc);
            com.Parameters.AddWithValue("@Bankname", obj.bankname);
            com.Parameters.AddWithValue("@AccholderName", obj.Accholdername);
            com.Parameters.AddWithValue("@Status", Status);
            com.Parameters.AddWithValue("@Description", Description);

            com.Parameters.AddWithValue("@Aadharstatus", obj.Aadharstatus);
            com.Parameters.AddWithValue("@Panstatus", obj.Panstatus);
            if (!string.IsNullOrEmpty(obj.Licensestatus.ToString()))
                com.Parameters.AddWithValue("@Licensestatus", obj.Licensestatus);
            else
                com.Parameters.AddWithValue("@Licensestatus", DBNull.Value);
            if (!string.IsNullOrEmpty(obj.Bankstatus))
                com.Parameters.AddWithValue("@Bankstatus", obj.Bankstatus);
            else
                com.Parameters.AddWithValue("@Bankstatus", DBNull.Value);

            com.Parameters.AddWithValue("@Updatedon", ToDate);
            i = com.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public DataTable getDeliveryBoyCurrentstatus(int? DeliveryboyId, DateTime? Fromdate, DateTime? Todate)
        {
            if (DeliveryboyId == 0) DeliveryboyId = null;

            string status = "Pending";
            ToDate = Todate;
            FromDate = Fromdate;

            SqlCommand cmd = new SqlCommand("SELECT Dc.DMid,Dc.CurrentStatus,convert(varchar,Dc.startdate,23) as StartDate,Concat(Sm.FirstName,' ',Sm.LastName) as Name from tbl_DM_CurrentStatus Dc inner join tbl_Staff_Master Sm On Dc.DMid=Sm.Id WHERE (@DeliveryBoyId IS NULL OR Dc.DMid=@DeliveryBoyId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,Dc.startdate,23) BETWEEN @FromDate AND @ToDate)", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DeliveryboyId);
            else
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DBNull.Value);

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
            return dt;
        }



        public DataTable getDeliveryBoySector(int? DeliveryboyId)
        {
            if (DeliveryboyId == 0) DeliveryboyId = null;

            string status = "Pending";
            ToDate = DateTime.Today;
            FromDate = DateTime.Today;
            SqlCommand cmd = new SqlCommand("SElect Distinct Cm.SectorId,Sem.SectorName From tbl_Customer_Master Cm Inner Join tbl_DeliveryBoy_Customer_Assign Dca on Dca.CustomerId=Cm.Id Inner join tbl_Staff_Master Sm on Sm.Id=Dca.StaffId Inner join tbl_Sector_Master Sem on Sem.Id=Cm.SectorId  WHERE (@DeliveryBoyId IS NULL OR Sm.Id=@DeliveryBoyId)", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DeliveryboyId);
            else
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DBNull.Value);



            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }




        public int UpdateContact(int Deliveryboyid, string ContactNo)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("UPDATE tbl_Staff_Master set MobileNo=@ContactNo Where Id = @DMId", con);
                com.CommandType = CommandType.Text;

                com.Parameters.AddWithValue("@DMId", Deliveryboyid);
                com.Parameters.AddWithValue("@ContactNo", ContactNo);


                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;
        }



        public DataTable getDeliveryBoyDocStaus(int? DeliveryboyId)
        {
            if (DeliveryboyId == 0) DeliveryboyId = null;

            string status = "Pending";
            ToDate = DateTime.Today;
            FromDate = DateTime.Today;
            SqlCommand cmd = new SqlCommand("SElect * From tbl_Dm_doc  WHERE (@DeliveryBoyId IS NULL OR DmId=@DeliveryBoyId)", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DeliveryboyId);
            else
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DBNull.Value);



            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public DataTable getDmstatusListReport(DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();

            cmd = new SqlCommand(" Select DC.*,Concat(Sm.FirstName,' ',Sm.LastName) As Name from tbl_DM_CurrentStatus DC inner join tbl_Staff_Master Sm on Dc.DMid=Sm.Id   where Sm.Role='DeliveryBoy'  and (@FromDate is null or @ToDate is null or convert(varchar,Dc.startdate,23) between @FromDate and @Todate)", con);
            cmd.CommandType = CommandType.Text;



            if (!string.IsNullOrEmpty(FDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(TDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", TDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;



        }

        public DataTable getDm(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Select Id,CONCAT(FirstName,' ',LastName) AS Name From tbl_Staff_Master where (@Id IS NULL OR [Id] = @Id) AND Role='DeliveryBoy'", con);
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


        public DataTable getDmstatusListReport1(int id, DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            if (id == 0)
            {
                cmd = new SqlCommand(" Select DC.*,Concat(Sm.FirstName,' ',Sm.LastName) As Name from tbl_DM_CurrentStatus DC inner join tbl_Staff_Master Sm on Dc.DMid=Sm.Id   where Sm.Role='DeliveryBoy'  and (@FromDate is null or @ToDate is null or convert(varchar,Dc.startdate,23) between @FromDate and @Todate)", con);
            }
            else
            {
                cmd = new SqlCommand(" Select DC.*,Concat(Sm.FirstName,' ',Sm.LastName) As Name from tbl_DM_CurrentStatus DC inner join tbl_Staff_Master Sm on Dc.DMid=Sm.Id   where Sm.Role='DeliveryBoy' and DC.DMid=@Id  and (@FromDate is null or @ToDate is null or convert(varchar,Dc.startdate,23) between @FromDate and @Todate)", con);
            }

            cmd.CommandType = CommandType.Text;


            if (!string.IsNullOrEmpty(id.ToString()))
                cmd.Parameters.AddWithValue("@Id", id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            if (!string.IsNullOrEmpty(FDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(TDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", TDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;



        }



        public DataTable getDmphotoListReport(DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();

            cmd = new SqlCommand(" Select DC.*,Concat(Sm.FirstName,' ',Sm.LastName) As Name from tbl_DM_Product_Photo DC inner join tbl_Staff_Master Sm on Dc.DMid=Sm.Id   where Sm.Role='DeliveryBoy'  and (@FromDate is null or @ToDate is null or convert(varchar,Dc.Uploaddate,23) between @FromDate and @Todate)", con);
            cmd.CommandType = CommandType.Text;



            if (!string.IsNullOrEmpty(FDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(TDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", TDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;



        }


        public DataTable getDmphotoListReport1(int id, DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            if (id == 0)
            {
                cmd = new SqlCommand(" Select DC.*,Concat(Sm.FirstName,' ',Sm.LastName) As Name from tbl_DM_Product_Photo DC inner join tbl_Staff_Master Sm on Dc.DMid=Sm.Id   where Sm.Role='DeliveryBoy'  and (@FromDate is null or @ToDate is null or convert(varchar,Dc.Uploaddate,23) between @FromDate and @Todate)", con);
            }
            else
            {
                cmd = new SqlCommand(" Select DC.*,Concat(Sm.FirstName,' ',Sm.LastName) As Name from tbl_DM_Product_Photo DC inner join tbl_Staff_Master Sm on Dc.DMid=Sm.Id   where Sm.Role='DeliveryBoy' and DC.DMid=@Id  and (@FromDate is null or @ToDate is null or convert(varchar,Dc.Uploaddate,23) between @FromDate and @Todate)", con);
            }

            cmd.CommandType = CommandType.Text;


            if (!string.IsNullOrEmpty(id.ToString()))
                cmd.Parameters.AddWithValue("@Id", id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            if (!string.IsNullOrEmpty(FDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(TDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", TDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;



        }




        public DataTable getDmCashListReport(DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();

            cmd = new SqlCommand("Select DC.*,Concat(Sm.FirstName,' ',Sm.LastName) As Name,CONCAT(CM.FirstName,' ',CM.LastName) as Cname,Cw.Amount,Cw.CashCollectionID  from tbl_Customer_Cash_Request DC inner join tbl_Staff_Master Sm on Dc.DMId=Sm.Id left join tbl_Customer_Wallet Cw On Dc.Id=Cw.CashCollectionID INNER Join tbl_Customer_Master CM on DC.CustomerId=CM.Id where Sm.Role='DeliveryBoy'  and (@FromDate is null or @ToDate is null or convert(varchar,Dc.CollectDate,23) between @FromDate and @Todate)", con);
            cmd.CommandType = CommandType.Text;



            if (!string.IsNullOrEmpty(FDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(TDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", TDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;



        }



        public DataTable getDmCashListReport1(int id, DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            if (id == 0)
            {
                cmd = new SqlCommand("Select DC.*,Concat(Sm.FirstName,' ',Sm.LastName) As Name,CONCAT(CM.FirstName,' ',CM.LastName) as Cname,Cw.Amount,Cw.CashCollectionID  from tbl_Customer_Cash_Request DC inner join tbl_Staff_Master Sm on Dc.DMId=Sm.Id left join tbl_Customer_Wallet Cw On Dc.Id=Cw.CashCollectionID INNER Join tbl_Customer_Master CM on DC.CustomerId=CM.Id   where Sm.Role='DeliveryBoy'  and (@FromDate is null or @ToDate is null or convert(varchar,Dc.CollectDate,23) between @FromDate and @Todate)", con);
            }
            else
            {
                cmd = new SqlCommand("Select DC.*,Concat(Sm.FirstName,' ',Sm.LastName) As Name,CONCAT(CM.FirstName,' ',CM.LastName) as Cname,Cw.Amount,Cw.CashCollectionID  from tbl_Customer_Cash_Request DC inner join tbl_Staff_Master Sm on Dc.DMId=Sm.Id left join tbl_Customer_Wallet Cw On Dc.Id=Cw.CashCollectionID INNER Join tbl_Customer_Master CM on DC.CustomerId=CM.Id   where Sm.Role='DeliveryBoy' and DC.DMid=@Id  and (@FromDate is null or @ToDate is null or convert(varchar,Dc.CollectDate,23) between @FromDate and @Todate)", con);
            }

            cmd.CommandType = CommandType.Text;


            if (!string.IsNullOrEmpty(id.ToString()))
                cmd.Parameters.AddWithValue("@Id", id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            if (!string.IsNullOrEmpty(FDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(TDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", TDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;



        }



        public DataTable getDmDocListReport(DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            //and(@FromDate is null or @ToDate is null or convert(varchar, Dc.Updatedon, 23) between @FromDate and @Todate)
            cmd = new SqlCommand("Select DC.*,Concat(Sm.FirstName,' ',Sm.LastName) As Name  from tbl_Dm_doc DC inner join tbl_Staff_Master Sm on Dc.DMId=Sm.Id  where Sm.Role='DeliveryBoy'  ", con);
            cmd.CommandType = CommandType.Text;



            //if (!string.IsNullOrEmpty(FDate.ToString()))
            //    cmd.Parameters.AddWithValue("@FromDate", FDate);
            //else
            //    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            //if (!string.IsNullOrEmpty(TDate.ToString()))
            //    cmd.Parameters.AddWithValue("@ToDate", TDate);
            //else
            //    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;



        }



        public DataTable getDmDocListReport1(int id, DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            if (id == 0)
            {
                cmd = new SqlCommand("Select DC.*,Concat(Sm.FirstName,' ',Sm.LastName) As Name from tbl_Dm_doc DC inner join tbl_Staff_Master Sm on Dc.DMId=Sm.Id    where Sm.Role='DeliveryBoy'  and (@FromDate is null or @ToDate is null or convert(varchar,Dc.Updatedon,23) between @FromDate and @Todate)", con);
            }
            else
            {
                cmd = new SqlCommand("Select DC.*,Concat(Sm.FirstName,' ',Sm.LastName) As Name from tbl_Dm_doc DC inner join tbl_Staff_Master Sm on Dc.DMId=Sm.Id    where Sm.Role='DeliveryBoy' and DC.DMid=@Id  and (@FromDate is null or @ToDate is null or convert(varchar,Dc.Updatedon,23) between @FromDate and @Todate)", con);
            }

            cmd.CommandType = CommandType.Text;


            if (!string.IsNullOrEmpty(id.ToString()))
                cmd.Parameters.AddWithValue("@Id", id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            if (!string.IsNullOrEmpty(FDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(TDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", TDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;



        }


        public DataTable getDmDocReport(int? Id)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();

            cmd = new SqlCommand("Select * From tbl_Dm_doc where (@Id IS NULL OR [Id] = @Id)", con);
            cmd.CommandType = CommandType.Text;



            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;



        }


        public int UpdateDmDocstatus(DeliveryBoy obj)
        {
            int i = 0;
            try
            {
                ToDate = DateTime.Today;
                con.Open();


                SqlCommand com = new SqlCommand("Update tbl_Dm_doc set Aadharstatus=@Aadharstatus,Panstatus=@Panstatus,Licensestatus=@Licensestatus,Bankstatus=@Bankstatus,Status=@Status,Decsiption=@Decsiption,Updatedon=@Updatedon WHERE (@Id IS NULL OR [Id] = @Id)", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", obj.Id);
                com.Parameters.AddWithValue("@Aadharstatus", obj.Aadharstatus);
                com.Parameters.AddWithValue("@Panstatus", obj.Panstatus);
                if (!string.IsNullOrEmpty(obj.Licensestatus.ToString()))
                    com.Parameters.AddWithValue("@Licensestatus", obj.Licensestatus);
                else
                    com.Parameters.AddWithValue("@Licensestatus", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Bankstatus))
                    com.Parameters.AddWithValue("@Bankstatus", obj.Bankstatus);
                else
                    com.Parameters.AddWithValue("@Bankstatus", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Status.ToString()))
                    com.Parameters.AddWithValue("@Status", obj.Status);
                else
                    com.Parameters.AddWithValue("@Status", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.Description.ToString()))
                    com.Parameters.AddWithValue("@Decsiption", obj.Description);
                else
                    com.Parameters.AddWithValue("@Decsiption", DBNull.Value);



                com.Parameters.AddWithValue("@Updatedon", ToDate);


                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public DataTable getDmwiseDoc(int? id)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            //and(@FromDate is null or @ToDate is null or convert(varchar, Dc.Updatedon, 23) between @FromDate and @Todate)
            cmd = new SqlCommand("SELECT * from tbl_Dm_doc  where DmId=" + id + "", con);
            cmd.CommandType = CommandType.Text;



            //if (!string.IsNullOrEmpty(FDate.ToString()))
            //    cmd.Parameters.AddWithValue("@FromDate", FDate);
            //else
            //    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            //if (!string.IsNullOrEmpty(TDate.ToString()))
            //    cmd.Parameters.AddWithValue("@ToDate", TDate);
            //else
            //    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;



        }


        public DataTable getCashcollectionwallet(int? id)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            //and(@FromDate is null or @ToDate is null or convert(varchar, Dc.Updatedon, 23) between @FromDate and @Todate)
            cmd = new SqlCommand("SELECT Cr.*,Cw.CashCollectionID from tbl_Customer_Cash_Request Cr left join tbl_Customer_Wallet Cw On Cr.Id=Cw.CashCollectionID  where @Id is Null Or Cr.Id=@Id", con);
            cmd.CommandType = CommandType.Text;



            if (!string.IsNullOrEmpty(id.ToString()))
                cmd.Parameters.AddWithValue("@Id", id);
            else
                cmd.Parameters.AddWithValue("@Id", 0);
            //if (!string.IsNullOrEmpty(TDate.ToString()))
            //    cmd.Parameters.AddWithValue("@ToDate", TDate);
            //else
            //    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;



        }

        public int UpdateDoc(DeliveryBoy obj)
        {
            int i = 0;
            try
            {
                ToDate = DateTime.Today;
                con.Open();
                if (!string.IsNullOrEmpty(obj.Aadhar.ToString()))
                {
                    SqlCommand com = new SqlCommand("Update tbl_Dm_doc set Aadhar=@Aadhar,BankAccount=@BankAccount,Ifsc=@Ifsc,Bankname=@Bankname,AccholderName=@AccholderName, Aadharstatus=@Aadharstatus,Bankstatus=@Bankstatus,Status=@Status,Updatedon=@Updatedon WHERE (@Id IS NULL OR [DmId] = @Id)", con);
                    com.CommandType = CommandType.Text;
                    com.Parameters.AddWithValue("@Id", obj.DeliveryBoyId);

                    com.Parameters.AddWithValue("@Aadhar", obj.Aadhar);
                    //com.Parameters.AddWithValue("@Pan", obj.Pan);

                    //com.Parameters.AddWithValue("@License", obj.License);

                    com.Parameters.AddWithValue("@BankAccount", obj.bankaccount);
                    com.Parameters.AddWithValue("@Ifsc", obj.ifsc);
                    com.Parameters.AddWithValue("@Bankname", obj.bankname);
                    com.Parameters.AddWithValue("@AccholderName", obj.Accholdername);
                    com.Parameters.AddWithValue("@Aadharstatus", obj.Aadharstatus);

                    if (!string.IsNullOrEmpty(obj.Bankstatus))
                        com.Parameters.AddWithValue("@Bankstatus", obj.Bankstatus);
                    else
                        com.Parameters.AddWithValue("@Bankstatus", DBNull.Value);
                    if (!string.IsNullOrEmpty(obj.Status.ToString()))
                        com.Parameters.AddWithValue("@Status", obj.Status);
                    else
                        com.Parameters.AddWithValue("@Status", DBNull.Value);






                    com.Parameters.AddWithValue("@Updatedon", ToDate);


                    i = com.ExecuteNonQuery();
                    //con.Close();
                }

                if (!string.IsNullOrEmpty(obj.Pan.ToString()))
                {
                    SqlCommand com = new SqlCommand("Update tbl_Dm_doc set Pan=@Pan,BankAccount=@BankAccount,Ifsc=@Ifsc,Bankname=@Bankname,AccholderName=@AccholderName, Panstatus=@Panstatus,Bankstatus=@Bankstatus,Status=@Status,Updatedon=@Updatedon WHERE (@Id IS NULL OR [DmId] = @Id)", con);
                    com.CommandType = CommandType.Text;
                    com.Parameters.AddWithValue("@Id", obj.DeliveryBoyId);

                    // com.Parameters.AddWithValue("@Aadhar", obj.Aadhar);
                    com.Parameters.AddWithValue("@Pan", obj.Pan);

                    //com.Parameters.AddWithValue("@License", obj.License);

                    com.Parameters.AddWithValue("@BankAccount", obj.bankaccount);
                    com.Parameters.AddWithValue("@Ifsc", obj.ifsc);
                    com.Parameters.AddWithValue("@Bankname", obj.bankname);
                    com.Parameters.AddWithValue("@AccholderName", obj.Accholdername);
                    com.Parameters.AddWithValue("@Panstatus", obj.Panstatus);

                    if (!string.IsNullOrEmpty(obj.Bankstatus))
                        com.Parameters.AddWithValue("@Bankstatus", obj.Bankstatus);
                    else
                        com.Parameters.AddWithValue("@Bankstatus", DBNull.Value);
                    if (!string.IsNullOrEmpty(obj.Status.ToString()))
                        com.Parameters.AddWithValue("@Status", obj.Status);
                    else
                        com.Parameters.AddWithValue("@Status", DBNull.Value);






                    com.Parameters.AddWithValue("@Updatedon", ToDate);


                    i = com.ExecuteNonQuery();
                }

                if (!string.IsNullOrEmpty(obj.License.ToString()))
                {
                    SqlCommand com = new SqlCommand("Update tbl_Dm_doc set License=@License,BankAccount=@BankAccount,Ifsc=@Ifsc,Bankname=@Bankname,AccholderName=@AccholderName, Licensestatus=@Licensestatus,Bankstatus=@Bankstatus,Status=@Status,Updatedon=@Updatedon WHERE (@Id IS NULL OR [DmId] = @Id)", con);
                    com.CommandType = CommandType.Text;
                    com.Parameters.AddWithValue("@Id", obj.DeliveryBoyId);

                    // com.Parameters.AddWithValue("@Aadhar", obj.Aadhar);
                    //com.Parameters.AddWithValue("@Pan", obj.Pan);

                    com.Parameters.AddWithValue("@License", obj.License);

                    com.Parameters.AddWithValue("@BankAccount", obj.bankaccount);
                    com.Parameters.AddWithValue("@Ifsc", obj.ifsc);
                    com.Parameters.AddWithValue("@Bankname", obj.bankname);
                    com.Parameters.AddWithValue("@AccholderName", obj.Accholdername);
                    com.Parameters.AddWithValue("@Licensestatus", obj.Licensestatus);

                    if (!string.IsNullOrEmpty(obj.Bankstatus))
                        com.Parameters.AddWithValue("@Bankstatus", obj.Bankstatus);
                    else
                        com.Parameters.AddWithValue("@Bankstatus", DBNull.Value);
                    if (!string.IsNullOrEmpty(obj.Status.ToString()))
                        com.Parameters.AddWithValue("@Status", obj.Status);
                    else
                        com.Parameters.AddWithValue("@Status", DBNull.Value);





                    com.Parameters.AddWithValue("@Updatedon", ToDate);


                    i = com.ExecuteNonQuery();
                }

                if (string.IsNullOrEmpty(obj.License.ToString()) && string.IsNullOrEmpty(obj.Aadhar.ToString()) && string.IsNullOrEmpty(obj.Pan.ToString()))
                {
                    SqlCommand com1 = new SqlCommand("Update tbl_Dm_doc set BankAccount=@BankAccount,Ifsc=@Ifsc,Bankname=@Bankname,AccholderName=@AccholderName, Bankstatus=@Bankstatus,Status=@Status,Updatedon=@Updatedon WHERE (@Id IS NULL OR [DmId] = @Id)", con);
                    com1.CommandType = CommandType.Text;
                    com1.Parameters.AddWithValue("@Id", obj.DeliveryBoyId);

                    // com.Parameters.AddWithValue("@Aadhar", obj.Aadhar);
                    //com.Parameters.AddWithValue("@Pan", obj.Pan);



                    com1.Parameters.AddWithValue("@BankAccount", obj.bankaccount);
                    com1.Parameters.AddWithValue("@Ifsc", obj.ifsc);
                    com1.Parameters.AddWithValue("@Bankname", obj.bankname);
                    com1.Parameters.AddWithValue("@AccholderName", obj.Accholdername);


                    if (!string.IsNullOrEmpty(obj.Bankstatus))
                        com1.Parameters.AddWithValue("@Bankstatus", obj.Bankstatus);
                    else
                        com1.Parameters.AddWithValue("@Bankstatus", DBNull.Value);
                    if (!string.IsNullOrEmpty(obj.Status.ToString()))
                        com1.Parameters.AddWithValue("@Status", obj.Status);
                    else
                        com1.Parameters.AddWithValue("@Status", DBNull.Value);






                    com1.Parameters.AddWithValue("@Updatedon", ToDate);


                    i = com1.ExecuteNonQuery();
                }


                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public DataTable getDmwisecustomer(int? id)
        {
            //con.Open();

            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            //and(@FromDate is null or @ToDate is null or convert(varchar, Dc.Updatedon, 23) between @FromDate and @Todate)
            string query = "  SELECT cm.Id As Id,Concat(sm.FirstName,' ',sm.LastName) as StaffName,Concat(cm.FirstName,' ',cm.LastName) as Customer";

            query += " from[dbo].[tbl_DeliveryBoy_Customer_Assign] scm";
            query += " left join[dbo].[tbl_Staff_Master] sm on sm.Id = scm.StaffId";

            query += " left join[dbo].[tbl_Customer_Master] cm on cm.Id = scm.CustomerId";

            query += " where(@Id IS NULL OR scm.[StaffId] = @Id)";



            //SqlCommand cmd = new SqlCommand("Staff_Customer_Assign_SelectAll", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(id.ToString()))
                cmd.Parameters.AddWithValue("@Id", id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            //if (!string.IsNullOrEmpty(TDate.ToString()))
            //    cmd.Parameters.AddWithValue("@ToDate", TDate);
            //else
            //    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;



        }


        public int UpdateDMCustomernorderby(string pid, string cid)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("update tbl_DeliveryBoy_Customer_Assign set StaffId='" + cid + "' where CustomerId='" + pid + "'  ", con);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }


        public int UpdateDMCustomerorderby(string pid, string cid)
        {
            con.Open();
            int o = 0;
            SqlCommand cmd = new SqlCommand("update tbl_DeliveryBoy_Customer_Assign set StaffId='" + cid + "' where CustomerId='" + pid + "'  ", con);
            int result = cmd.ExecuteNonQuery();

            SqlCommand cmd1 = new SqlCommand("update tbl_Customer_Master set OrderBy=" + o + " where Id='" + pid + "'  ", con);
            int result1 = cmd1.ExecuteNonQuery();
            con.Close();
            return result;
        }




        public int InsertQtyUpdate(DeliveryBoy obj)
        {
            con.Open();
            int o = 0;
            string status = "Pending";
            SqlCommand cmd = new SqlCommand("INSERT INTO tbl_Dm_ProductUpdate(DMId,OrderId,CustomerId,Qty,Proid,newqty,proname,Updatedon,Status)VALUES(@DMId,@OrderId,@CustomerId,@Qty,@Proid,@newqty,@proname,@Updatedon,@Status)", con);
            cmd.CommandType = CommandType.Text;


            if (!string.IsNullOrEmpty(obj.DeliveryBoyId.ToString()))
                cmd.Parameters.AddWithValue("@DMId", obj.DeliveryBoyId);
            else
                cmd.Parameters.AddWithValue("@DMId", 0);


            if (!string.IsNullOrEmpty(obj.OrderId.ToString()))
                cmd.Parameters.AddWithValue("@OrderId", obj.OrderId);
            else
                cmd.Parameters.AddWithValue("@OrderId", 0);

            if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", 0);

            if (!string.IsNullOrEmpty(obj.Qty.ToString()))
                cmd.Parameters.AddWithValue("@Qty", obj.Qty);
            else
                cmd.Parameters.AddWithValue("@Qty", 0);

            if (!string.IsNullOrEmpty(obj.ProductId.ToString()))
                cmd.Parameters.AddWithValue("@Proid", obj.ProductId);
            else
                cmd.Parameters.AddWithValue("@Proid", 0);

            if (!string.IsNullOrEmpty(obj.newqty.ToString()))
                cmd.Parameters.AddWithValue("@newqty", obj.newqty);
            else
                cmd.Parameters.AddWithValue("@newqty", 0);


            if (!string.IsNullOrEmpty(obj.productname.ToString()))
                cmd.Parameters.AddWithValue("@proname", obj.productname);
            else
                cmd.Parameters.AddWithValue("@proname", DBNull.Value);

            if (!string.IsNullOrEmpty(obj.Updatedon.ToString()))
                cmd.Parameters.AddWithValue("@Updatedon", obj.Updatedon);
            else
                cmd.Parameters.AddWithValue("@Updatedon", DBNull.Value);

            if (!string.IsNullOrEmpty(status.ToString()))
                cmd.Parameters.AddWithValue("@Status", status);
            else
                cmd.Parameters.AddWithValue("@Status", DBNull.Value);

            int result = cmd.ExecuteNonQuery();


            con.Close();
            return result;
        }




        public DataTable getOrderqtyupdatedetail(int? OrderId)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();

            cmd = new SqlCommand("Select * From tbl_Dm_ProductUpdate where (@Id IS NULL OR [OrderId] = @Id)", con);
            cmd.CommandType = CommandType.Text;



            if (!string.IsNullOrEmpty(OrderId.ToString()))
                cmd.Parameters.AddWithValue("@Id", OrderId);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;



        }



        public int UpdateQtyUpdate(DeliveryBoy obj)
        {
            con.Open();
            int o = 0;
            string Status = "Pending";
            SqlCommand cmd = new SqlCommand("Update tbl_Dm_ProductUpdate set DMId=@DMId,CustomerId=@CustomerId,Qty=@Qty,Proid=@Proid,newqty=@newqty,proname=@proname,Updatedon=@Updatedon,Status=@Status where OrderId=@OrderId", con);
            cmd.CommandType = CommandType.Text;


            if (!string.IsNullOrEmpty(obj.DeliveryBoyId.ToString()))
                cmd.Parameters.AddWithValue("@DMId", obj.DeliveryBoyId);
            else
                cmd.Parameters.AddWithValue("@DMId", 0);


            if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", 0);

            if (!string.IsNullOrEmpty(obj.Qty.ToString()))
                cmd.Parameters.AddWithValue("@Qty", obj.Qty);
            else
                cmd.Parameters.AddWithValue("@Qty", 0);

            if (!string.IsNullOrEmpty(obj.ProductId.ToString()))
                cmd.Parameters.AddWithValue("@Proid", obj.ProductId);
            else
                cmd.Parameters.AddWithValue("@Proid", 0);

            if (!string.IsNullOrEmpty(obj.newqty.ToString()))
                cmd.Parameters.AddWithValue("@newqty", obj.newqty);
            else
                cmd.Parameters.AddWithValue("@newqty", 0);


            if (!string.IsNullOrEmpty(obj.productname.ToString()))
                cmd.Parameters.AddWithValue("@proname", obj.productname);
            else
                cmd.Parameters.AddWithValue("@proname", DBNull.Value);

            if (!string.IsNullOrEmpty(obj.Updatedon.ToString()))
                cmd.Parameters.AddWithValue("@Updatedon", obj.Updatedon);
            else
                cmd.Parameters.AddWithValue("@Updatedon", DBNull.Value);


            if (!string.IsNullOrEmpty(obj.OrderId.ToString()))
                cmd.Parameters.AddWithValue("@OrderId", obj.OrderId);
            else
                cmd.Parameters.AddWithValue("@OrderId", 0);


            if (!string.IsNullOrEmpty(Status.ToString()))
                cmd.Parameters.AddWithValue("@Status", Status);
            else
                cmd.Parameters.AddWithValue("@Status", 0);

            int result = cmd.ExecuteNonQuery();


            con.Close();
            return result;
        }




        public DataTable getDmOrderqtyUpdateListReport(int? DeliveryBoyID, DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            string query = "Select Dp.Id,Dp.DMId,Dp.OrderId,Dp.Qty,Dp.Proid,Dp.newqty,convert(varchar,Dp.Updatedon,23) AS Updatedon,Concat(Sm.FirstName,' ',Sm.LastName) As DmName,CONCAT(CM.FirstName,' ',CM.LastName) as Cname,Pm.ProductName,convert(varchar,Ot.OrderDate,23) AS OrderDate,Dp.Status  ";
            query += " from tbl_Dm_ProductUpdate Dp inner join tbl_Staff_Master Sm on Dp.DMId=Sm.Id ";
            query += " inner join tbl_Product_Master Pm On Dp.Proid=Pm.Id ";
            query += " inner join tbl_Customer_Order_Transaction Ot On Ot.Id=Dp.OrderId ";
            query += " INNER Join tbl_Customer_Master CM on Dp.CustomerId=CM.Id where Sm.Role='DeliveryBoy' ";
            query += "  and (@FromDate is null or @ToDate is null or convert(varchar,Dp.Updatedon,23) between @FromDate and @ToDate) and (@DeliveryBoyid Is Null Or (Dp.DMId=@DeliveryBoyid))";


            cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;



            if (!string.IsNullOrEmpty(FDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(TDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", TDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);

            if (!string.IsNullOrEmpty(DeliveryBoyID.ToString()))
                cmd.Parameters.AddWithValue("@DeliveryBoyid", DeliveryBoyID);
            else
                cmd.Parameters.AddWithValue("@DeliveryBoyid", DBNull.Value);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;



        }


        public DataTable getDmOrderqtyUpdate(int? Id)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            string query = "Select Dp.Id,Dp.CustomerId,Dp.DMId,Dp.OrderId,Dp.Qty,Dp.Proid,Dp.newqty,convert(varchar,Dp.Updatedon,23) AS Updatedon,Concat(Sm.FirstName,' ',Sm.LastName) As DmName,CONCAT(CM.FirstName,' ',CM.LastName) as Cname,Pm.ProductName,convert(varchar,Ot.OrderDate,23) AS OrderDate,Ot.Status As Orderstatus,Dp.Status  ";
            query += " from tbl_Dm_ProductUpdate Dp inner join tbl_Staff_Master Sm on Dp.DMId=Sm.Id ";
            query += " inner join tbl_Product_Master Pm On Dp.Proid=Pm.Id ";
            query += " inner join tbl_Customer_Order_Transaction Ot On Ot.Id=Dp.OrderId ";
            query += " INNER Join tbl_Customer_Master CM on Dp.CustomerId=CM.Id where Sm.Role='DeliveryBoy' ";
            query += " and (@Id Is Null Or (Dp.Id=@Id))";


            cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;





            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;



        }



        public int UpdateQtyUpdatestatus(int? Id, int? newqty)
        {
            con.Open();
            String Status = "Approved";
            int o = 0;
            SqlCommand cmd = new SqlCommand("Update tbl_Dm_ProductUpdate set newqty=@newqty,Status=@Status where Id=@Id", con);
            cmd.CommandType = CommandType.Text;



            if (!string.IsNullOrEmpty(newqty.ToString()))
                cmd.Parameters.AddWithValue("@newqty", newqty);
            else
                cmd.Parameters.AddWithValue("@newqty", 0);

            if (!string.IsNullOrEmpty(Status.ToString()))
                cmd.Parameters.AddWithValue("@Status", Status);
            else
                cmd.Parameters.AddWithValue("@Status", 0);





            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", 0);

            int result = cmd.ExecuteNonQuery();


            con.Close();
            return result;
        }
    }
}
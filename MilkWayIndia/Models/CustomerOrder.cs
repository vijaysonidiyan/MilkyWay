using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Models
{
    public class CustomerOrder
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
            SqlCommand cmd = new SqlCommand("SP_Sector_Staff_Order_SelectAllNew", con);
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


        public DataTable getDeliveryBoyCustomerOrdervendor(int? DeliveryboyId, int? CustomerId, DateTime? FDate, DateTime? TDate, string status)
        {
            if (DeliveryboyId == 0) DeliveryboyId = null;
            if (CustomerId == 0) CustomerId = null;
            if (status == "0") status = null;
            //con.Open();
            //SqlCommand cmd = new SqlCommand("Sector_Staff_Order_SelectAll", con);
            SqlCommand cmd = new SqlCommand("SP_Sector_Staff_Order_SelectAllNew", con);
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

        public DataTable getDeliveryBoyWiseOrder(int? DeliveryboyId, int? CustomerId, DateTime? FDate, DateTime? TDate, string status)
        {
            if (DeliveryboyId == 0) DeliveryboyId = null;
            if (CustomerId == 0) CustomerId = null;
            if (status == "0") status = null;
            //con.Open();
            //SqlCommand cmd = new SqlCommand("SP_Sector_Staff_Order_SelectAll", con);
            SqlCommand cmd = new SqlCommand("SP_Sector_Staff_Order_SelectAllNew", con);
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


        public DataTable getDeliveryBoyWiseOrdervendor(int? DeliveryboyId, int? CustomerId, DateTime? FDate, DateTime? TDate, string status)
        {
            if (DeliveryboyId == 0) DeliveryboyId = null;
            if (CustomerId == 0) CustomerId = null;
            if (status == "0") status = null;
            //con.Open();


            // SqlCommand cmd = new SqlCommand("SP_Sector_Staff_Order_SelectAll", con);

            SqlCommand cmd = new SqlCommand("SELECT otrans.Id AS Id,Concat(sm.FirstName,' ',sm.LastName) as DeliveryBoy,secm.SectorName,Concat(cm.FirstName,' ',cm.LastName) as Customer, otrans.OrderNo,convert(varchar,otrans.OrderDate,23) as OrderDate, prodt.ProductName,At.Name As Attribute, odetail.Qty,odetail.Amount,cm.Address, otrans.[Status],cm.orderby,secm.Id as sid from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId Left join tbl_Attributes At on odetail.AttributeId=At.ID WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (@CustomerId IS NULL OR otrans.CustomerId=@CustomerId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND odetail.Qty <> 0 AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus) 	order by cm.OrderBy,Customer", con);
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
        public DataTable getSectorVendorOrder(int? SectorId, int? VendorId, DateTime? FDate, DateTime? TDate)
        {
            if (SectorId == 0) SectorId = null;
            if (VendorId == 0) VendorId = null;
           string VendorCatId = "";
            
            con.Open();
            //string query = "SELECT max(DISTINCT otrans.OrderNo) as OrderNo,max(convert(varchar,otrans.OrderDate,23)) as OrderDate,";
            //query += " sum(odetail.Qty) as Qty,MAX(Concat(vm.FirstName,' ', vm.LastName)) AS Vendor,max(prodt.ProductName) as ProductName, ";
            //query += " max(SectorName) as Sector,(max(prodt.PurchasePrice)*sum(odetail.Qty)) as PurchasePrice,max(SectorName) as Sector,(max(prodt.Price)*sum(odetail.Qty)) as MRP,max(prodt.SalePrice)*sum(odetail.Qty) as CustomerPrice,(max(prodt.SalePrice)*sum(odetail.Qty)-max(prodt.PurchasePrice)*sum(odetail.Qty)) AS Profit from [dbo].[tbl_Staff_Master] sm";
            //query += " left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id";
            //query += " left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId";
            //query += " left join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId";
            //query += " left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id";

            //query += " and (@FromDate is null or @ToDate is null or convert(varchar,otrans.Orderdate,23) between @FromDate and @Todate) ";

            //query += " left join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id";
            //query += " left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId";
            //query += " left join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id AND vpa.SectorId=cm.SectorId";
            //query += " left join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId where";
            //query += " (@SectorId is null or secm.Id=@SectorId) and (@VendorId is null or vm.Id=@VendorId)";

            //query += " and odetail.Qty <> 0 and  vpa.IsActive = 1 ";
            //query += " Group by prodt.Id Order by max(convert(varchar,otrans.OrderDate,23)) ASC";


            string query = "SELECT max(DISTINCT otrans.OrderNo) as OrderNo,max(convert(varchar,otrans.OrderDate,23)) as OrderDate,";
            query += " sum(odetail.Qty) as Qty,max(prodt.ProductName) as ProductName,max(At.Name) As Attribute, ";
            query += " sum(odetail.PurchasePrice) as PurchasePrice,sum(odetail.Mrp) as MRP,sum(odetail.Amount) as CustomerPrice,(sum(odetail.Amount)-sum(odetail.PurchasePrice)) AS Profit,Max(Secm.SectorName) as Sector,Max(Concat(Vm.FirstName,' ',Vm.LastName)) As Vendor ";
            query += "  from   [dbo].[tbl_Customer_Order_Transaction] otrans";
            query += " inner join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id And (@VendorId Is Null Or odetail.VendorId=@VendorId )";
            query += " And (@VendorCatId Is Null Or odetail.VendorCatId=@VendorCatId) And (@FromDate is null or @ToDate is null or convert(varchar,otrans.Orderdate,23) between @FromDate and @Todate)";
            query += " inner join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = odetail.ProductId and vpa.AttributeId=odetail.AttributeId";
            query += "  inner join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId";
            query += " inner join tbl_Attributes At On odetail.AttributeId=At.ID";
            query += " inner join tbl_Vendor_Master Vm On Vm.Id=odetail.VendorId";
            query += " inner join tbl_Sector_Master Secm On Secm.Id=odetail.SectorId";
            query += " where (@SectorId is null or secm.Id=@SectorId) And  odetail.Qty <> 0 Group by prodt.Id,At.ID Order by max(convert(varchar,otrans.OrderDate,23)) ASC";
            //and vm.FirstName is not null
            //SqlCommand cmd = new SqlCommand("Sector_Vendor_OrderTotal_SelectAll", con);[]
            //SqlCommand cmd = new SqlCommand("Sector_Vendor_Daily_OrderTotal_SelectAll", con);
            //cmd.CommandType = CommandType.StoredProcedure;

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 1200;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);

            if (!string.IsNullOrEmpty(VendorCatId.ToString()))
                cmd.Parameters.AddWithValue("@VendorCatId", VendorCatId);
            else
                cmd.Parameters.AddWithValue("@VendorCatId", DBNull.Value);
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
            con.Close();
            return dt;
        }

        public DataTable getSectorVendorOrderapi(int? VendorId, DateTime? FDate, DateTime? TDate)
        {
            //if (SectorId == 0) SectorId = null;
            if (VendorId == 0) VendorId = null;
            con.Open();
            //string query = "SELECT MAX(DISTINCT otrans.Id) AS Id,max(otrans.OrderNo) as OrderNo,max(convert(varchar,otrans.OrderDate,23)) as OrderDate,";
            //query += " sum(odetail.Qty) as Qty,MAX(Concat(vm.FirstName,' ', vm.LastName)) AS Vendor,max(prodt.ProductName) as ProductName,MAX(prodt.Image) as image,MAX(otrans.[Status]) As status, ";
            //query += " max(SectorName) as Sector,(max(prodt.PurchasePrice)*sum(odetail.Qty)) as PurchasePrice,max(SectorName) as Sector,(max(prodt.Price)*sum(odetail.Qty)) as MRP,max(prodt.SalePrice)*sum(odetail.Qty) as CustomerPrice,(max(prodt.SalePrice)*sum(odetail.Qty)-max(prodt.PurchasePrice)*sum(odetail.Qty)) AS Profit,MAX(otrans.DeliveryStatus) As DeliveryStatus,MAX(vm.MobileNo) AS MobileNo from [dbo].[tbl_Staff_Master] sm";
            //query += " Inner join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id";
            //query += " Inner join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId";
            //query += " Inner join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId";
            //query += " Inner join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id";

            //query += " and (@FromDate is null or @ToDate is null or convert(varchar,otrans.Orderdate,23) between @FromDate and @Todate) ";
            //query += " Inner join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id and (otrans.status='Complete' Or otrans.status='Pending')";
            //query += " Inner join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId";
            //query += " Inner join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id AND vpa.SectorId=cm.SectorId";
            //query += " Inner join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId where";
            //query += "  (@VendorId is null or vm.Id=@VendorId)";

            //query += " and odetail.Qty <> 0 and  vpa.IsActive = 1 ";
            //query += " Group by prodt.Id Order by max(convert(varchar,otrans.OrderDate,23)) ASC";

            //and vm.FirstName is not null
            //SqlCommand cmd = new SqlCommand("Sector_Vendor_OrderTotal_SelectAll", con);[]
            //SqlCommand cmd = new SqlCommand("Sector_Vendor_Daily_OrderTotal_SelectAll", con);
            //cmd.CommandType = CommandType.StoredProcedure;


            //new version 22-12-2022
            string query = "SELECT MAX(DISTINCT otrans.Id) AS Id,max(otrans.OrderNo) as OrderNo,max(convert(varchar,otrans.OrderDate,23)) as OrderDate,";
            query += " sum(odetail.Qty) as Qty,MAX(Concat(vm.FirstName,' ', vm.LastName)) AS Vendor,max(prodt.ProductName) as ProductName,MAX(prodt.Image) as image,MAX(otrans.[Status]) As status, ";
            query += " max(SectorName) as Sector,(max(Pat.PurchasePrice)*sum(odetail.Qty)) as PurchasePrice,max(SectorName) as Sector,(max(Pat.MRPPrice)*sum(odetail.Qty)) as MRP,max(Pat.SellPrice)*sum(odetail.Qty) as CustomerPrice,(max(Pat.SellPrice)*sum(odetail.Qty)-max(Pat.PurchasePrice)*sum(odetail.Qty)) AS Profit,MAX(otrans.DeliveryStatus) As DeliveryStatus,MAX(vm.MobileNo) AS MobileNo,MAX(At.Name) As Name ";
            query += " from tbl_Customer_Order_Transaction otrans ";
            query += " Inner join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id and (otrans.status='Complete' Or otrans.status='Pending')";
            query += " and (@FromDate is null or @ToDate is null or convert(varchar,otrans.Orderdate,23) between @FromDate and @Todate) ";
            query += " Inner join  [tbl_Vendor_Master] vm on vm.Id = odetail.VendorId ";
            query += " Inner join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId ";
            query += " Inner join [dbo].[tbl_Sector_Master] secm on secm.Id = odetail.SectorId ";
            query += " Inner join [dbo].tbl_Product_Attributes Pat on Pat.AttributeID = odetail.AttributeId And Pat.ProductID=odetail.ProductId ";

            query += " Inner Join tbl_Attributes At On At.ID=odetail.AttributeId ";
            query += " Where (@VendorId is null or vm.Id=@VendorId)";

            query += " and odetail.Qty <> 0 ";
            query += " Group by prodt.Id Order by max(convert(varchar,otrans.OrderDate,23)) ASC";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 1200;

            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);
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
            con.Close();
            return dt;
        }

        public DataTable GetMultiSectorVendorOrder(string SectorId, int? VendorId, DateTime? FDate, DateTime? TDate)
        {
            if (VendorId == 0) VendorId = null;
            SqlCommand cmd = new SqlCommand("SP_Sector_Vendor_Order", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);
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


        public DataTable GetMultiSectorVendorOrder1(string SectorId, DateTime? FDate, DateTime? TDate,int? DeliveryboyId)
        {
            //if (VendorId == 0) VendorId = null;
            //SqlCommand cmd = new SqlCommand("SP_Sector_Vendor_Order", con);
            //cmd.CommandType = CommandType.StoredProcedure;
            //if (!string.IsNullOrEmpty(SectorId.ToString()))
            //    cmd.Parameters.AddWithValue("@SectorId", SectorId);
            //else
            //    cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            //if (!string.IsNullOrEmpty(VendorId.ToString()))
            //    cmd.Parameters.AddWithValue("@VendorId", VendorId);
            //else
            //    cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);
            //if (!string.IsNullOrEmpty(FDate.ToString()))
            //    cmd.Parameters.AddWithValue("@FromDate", FDate);
            //else
            //    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            //if (!string.IsNullOrEmpty(TDate.ToString()))
            //    cmd.Parameters.AddWithValue("@ToDate", TDate);
            //else
            //    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataTable dt = new DataTable();
            //da.Fill(dt);
            //return dt;



            //SqlCommand cmd = new SqlCommand("SELECT MAX( DISTINCT otrans.OrderNo) as OrderNo,MAX(CONVERT(VARCHAR,otrans.OrderDate,23)) AS OrderDate,SUM(odetail.Qty) AS Qty, MAX(vm.FirstName + ' '+ vm.LastName) AS Vendor,MAX(prodt.ProductName) AS ProductName,	MAX(SectorName) AS Sector,(MAX(prodt.PurchasePrice)*SUM(odetail.Qty)) AS PurchasePrice FROM [dbo].[tbl_Staff_Master] sm left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId left join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id AND vpa.SectorId=cm.SectorId left join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId WHERE (cm.SectorId IN (SELECT Value FROM fn_Split_Sector(@SectorId, ','))) AND  (CONVERT(VARCHAR,otrans.Orderdate,23) >= @FromDate) AND (CONVERT(VARCHAR,otrans.Orderdate,23) <=@ToDate) GROUP BY prodt.Id,SectorName ORDER BY Sector,Vendor", con);
            // cmd.CommandType = CommandType.StoredProcedure;

            //  SqlCommand cmd = new SqlCommand("SELECT MAX( DISTINCT otrans.OrderNo) as OrderNo,MAX(CONVERT(VARCHAR,otrans.OrderDate,23)) AS OrderDate,SUM(odetail.Qty) AS Qty, MAX(vm.FirstName + ' '+ vm.LastName) AS Vendor,MAX(prodt.ProductName) AS ProductName,	MAX(SectorName) AS Sector,(MAX(prodt.PurchasePrice)*SUM(odetail.Qty)) AS PurchasePrice FROM [dbo].[tbl_Staff_Master] sm left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId left join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id AND vpa.SectorId=cm.SectorId left join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId WHERE (cm.SectorId IN (SELECT Value FROM fn_Split_Sector(@SectorId, ','))) AND  (CONVERT(VARCHAR,otrans.Orderdate,23) >= @FromDate) AND (CONVERT(VARCHAR,otrans.Orderdate,23) <=@ToDate) GROUP BY prodt.Id,SectorName ORDER BY Sector,Vendor", con);
            string query = "SELECT MAX( DISTINCT otrans.OrderNo) as OrderNo,MAX(CONVERT(VARCHAR,otrans.OrderDate,23)) AS OrderDate,SUM(odetail.Qty) AS Qty,";
            query += " MAX(vm.FirstName + ' '+ vm.LastName) AS Vendor,MAX(prodt.ProductName) AS ProductName,	MAX(SectorName) AS Sector,";
            query += " (MAX(prodt.PurchasePrice)*SUM(odetail.Qty)) AS PurchasePrice,Max(Pa.Name) As Attribute FROM [dbo].[tbl_Staff_Master] sm ";
            query += " left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId ";
            query += " left join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id ";
            query += " left join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id ";
            query += " left join tbl_Attributes Pa On odetail.AttributeId=Pa.ID";
            query += " left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId ";
            query += " left join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id AND vpa.SectorId=cm.SectorId ";
            query += " left join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId ";
            query += " WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (cm.SectorId IN (SELECT Value FROM fn_Split_Sector(@SectorId, ','))) AND vpa.IsActive=1 AND  (CONVERT(VARCHAR,otrans.Orderdate,23) >= @FromDate) AND (CONVERT(VARCHAR,otrans.Orderdate,23) <=@ToDate) AND odetail.Qty>0 GROUP BY prodt.Id,SectorName ORDER BY Sector,Vendor";
            SqlCommand cmd = new SqlCommand(query, con);
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(FDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(TDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", TDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);

            if (!string.IsNullOrEmpty(DeliveryboyId.ToString()))
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DeliveryboyId);
            else
                cmd.Parameters.AddWithValue("@DeliveryBoyId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable getSectorVendorOrderStatus(int? SectorId, int? VendorId, DateTime? FDate, DateTime? TDate, string status)
        {
            if (SectorId == 0) SectorId = null;
            if (VendorId == 0) VendorId = null;
            if (status == "0") status = null;
            //con.Open();
            //SqlCommand cmd = new SqlCommand("SP_Sector_Vendor_Daily_OrderTotal_SelectAll_Status", con);[]
            //SqlCommand cmd = new SqlCommand("Sector_Vendor_Daily_OrderTotal_SelectAll_Status", con);

            //SqlCommand cmd = new SqlCommand("SP_Sector_Vendor_Daily_OrderTotal_SelectAll_Status", con);

            SqlCommand cmd = new SqlCommand("SP_Sector_Vendor_Daily_OrderTotal_SelectAll_StatusNew", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);
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
        public DataTable getSectorVendorOrderStatus1(int? SectorId, int? VendorId, DateTime? FDate, DateTime? TDate, string status)
        {
            if (SectorId == 0) SectorId = null;
            if (VendorId == 0) VendorId = null;
            if (status == "0") status = null;
            //con.Open();
            //SqlCommand cmd = new SqlCommand("SP_Sector_Vendor_Daily_OrderTotal_SelectAll_Status", con);[]
            //SqlCommand cmd = new SqlCommand("Sector_Vendor_Daily_OrderTotal_SelectAll_Status", con);

            //SqlCommand cmd = new SqlCommand("SP_Sector_Vendor_Daily_OrderTotal_SelectAll_Status", con);

            SqlCommand cmd = new SqlCommand("SP_Sector_Vendor_Daily_OrderTotal_SelectAll_StatusNew1", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);
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

        public DataTable getSectorVendorOrderStatusnotification(int? SectorId, int? VendorId, DateTime? FDate, DateTime? TDate, string status)
        {
            if (SectorId == 0) SectorId = null;
            if (VendorId == 0) VendorId = null;
            if (status == "0") status = null;
            //con.Open();
            //SqlCommand cmd = new SqlCommand("SP_Sector_Vendor_Daily_OrderTotal_SelectAll_Status", con);[]
            //SqlCommand cmd = new SqlCommand("Sector_Vendor_Daily_OrderTotal_SelectAll_Status", con);

            string query = "Select MAX(CONVERT(VARCHAR,otrans.OrderDate,23)) as OrderDate,MAX(Pm.ParentCategory) As ParentCategory,MAX(Vc.VendorCatname) AS VendorCatname,MAX(prodt.ProductName) AS ProductName,SUM(odetail.Qty) AS Qty,MAX(Sm.SectorName) As SectorName,Max(Vm.MobileNo) As MobileNo from tbl_Customer_Order_Transaction otrans  ";
            query += " inner join tbl_Customer_Order_Detail odetail ON odetail.OrderId = otrans.Id";
            query += " inner join tbl_Product_Master prodt on prodt.Id=odetail.ProductId";
            query += " inner join tbl_VendorCat Vc On Vc.Id=odetail.VendorCatId";
            query += " inner join tbl_Parent_Category_Master Pm On Pm.Id=Vc.ParentCatId";
            query += " inner join tbl_Sector_Master Sm on Sm.Id=odetail.SectorId";
            query += " inner join tbl_Vendor_Master Vm on Vm.id=Vc.VendorId";
            query += " where  (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND (@VendorId is Null Or Vc.VendorId=@VendorId ) AND (@SectorId is Null Or odetail.SectorId=@SectorId ) And Vc.IsNotification='True' GROUP BY prodt.Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);
            if (!string.IsNullOrEmpty(FDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(TDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", TDate);
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

        public DataTable getSectorVendorWiseOrderStatus(int? SectorId, int? VendorId, DateTime? FDate, DateTime? TDate, string status)
        {
            if (SectorId == 0) SectorId = null;
            if (VendorId == 0) VendorId = null;
            if (status == "0") status = null;

            //string f = "2022-08-21";
            //string t = "2022-08-21";

            SqlCommand cmd = new SqlCommand("SP_Sector_Vendor_Daily_OrderTotal_SelectAll_Status", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);
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

        public DataTable getSectorSubscriptiondays(int? SectorId, int? BuildingId, int? Pendingday)
        {
            if (SectorId == 0) SectorId = null;
            if (BuildingId == 0) BuildingId = null;
            if (Pendingday == 0) Pendingday = null;
            //con.Open();
            SqlCommand cmd = new SqlCommand("Customer_Subscription_Days_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(BuildingId.ToString()))
                cmd.Parameters.AddWithValue("@BuildingId", BuildingId);
            else
                cmd.Parameters.AddWithValue("@BuildingId", DBNull.Value);
            if (!string.IsNullOrEmpty(Pendingday.ToString()))
                cmd.Parameters.AddWithValue("@Pendingday", Pendingday);
            else
                cmd.Parameters.AddWithValue("@Pendingday", DBNull.Value);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getSectorSubscriptiondaysDash(int? SectorId, int? BuildingId, int? Pendingday)
        {
            if (SectorId == 0) SectorId = null;
            if (BuildingId == 0) BuildingId = null;
            if (Pendingday == 0 || Pendingday == null)
                Pendingday = 15;
            //con.Open();
            SqlCommand cmd = new SqlCommand("Customer_Subscription_Days_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(BuildingId.ToString()))
                cmd.Parameters.AddWithValue("@BuildingId", BuildingId);
            else
                cmd.Parameters.AddWithValue("@BuildingId", DBNull.Value);
            if (!string.IsNullOrEmpty(Pendingday.ToString()))
                cmd.Parameters.AddWithValue("@Pendingday", Pendingday);
            else
                cmd.Parameters.AddWithValue("@Pendingday", DBNull.Value);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerWalletBalDash(int? SectorId, int? BuildingId)
        {
            if (SectorId == 0) SectorId = null;
            if (BuildingId == 0) BuildingId = null;
            //con.Open();
            SqlCommand cmd = new SqlCommand("Customer_Balance_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(BuildingId.ToString()))
                cmd.Parameters.AddWithValue("@BuildingId", BuildingId);
            else
                cmd.Parameters.AddWithValue("@BuildingId", DBNull.Value);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getCustomerWalletTotalDebit(int? SectorId, int? BuildingId)
        {
            if (SectorId == 0) SectorId = null;
            if (BuildingId == 0) BuildingId = null;
            //con.Open();
            SqlCommand cmd = new SqlCommand(@"select max([Type]) as [Type],max(cm.FirstName + ' ' + cm.LastName) as Customer, max(bm.BuildingName) as BuildingName, max(bm.BlockNo) as BlockNo, CustomerId, isnull(Sum(Amount),0) As Amt from tbl_Customer_Wallet cw " +
       " LEFT JOIN[dbo].[tbl_Customer_Master] cm on cm.Id = cw.CustomerId left join[dbo].[tbl_Building_Master] bm on bm.Id = cm.BuildingId " +
    " where Type = 'Debit' Group by CustomerId Order By CustomerId ASC, max([Type]) Asc ", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(BuildingId.ToString()))
                cmd.Parameters.AddWithValue("@BuildingId", BuildingId);
            else
                cmd.Parameters.AddWithValue("@BuildingId", DBNull.Value);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        #region Customer Favourite
        public int insertCustomerFavourite(int? CustomerId, int? ProductId)
        {
            int i = 0;
            try
            {
                if (CustomerId == 0) CustomerId = null;
                if (ProductId == 0) ProductId = null;

                con.Open();
                SqlCommand cmd = new SqlCommand("Customer_Favourite_Insert", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (!string.IsNullOrEmpty(CustomerId.ToString()))
                    cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
                else
                    cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                if (!string.IsNullOrEmpty(ProductId.ToString()))
                    cmd.Parameters.AddWithValue("@ProductId", ProductId);
                else
                    cmd.Parameters.AddWithValue("@ProductId", DBNull.Value);
                cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex) { string s = ex.Message; }
            return i;
        }

        public DataTable getCustomerFavourite(int? CustomerId, int psize, int pno)
        {
            DataTable dt = new DataTable();
            try
            {
                if (CustomerId == 0) CustomerId = null;

                con.Open();
                SqlCommand cmd = new SqlCommand("Customer_Favourite_Product_SelectAll", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (!string.IsNullOrEmpty(CustomerId.ToString()))
                    cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
                else
                    cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                cmd.Parameters.AddWithValue("@PageNum", pno);
                cmd.Parameters.AddWithValue("@PageSize", psize);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex) { string s = ex.Message; }
            return dt;
        }



        public DataTable getCustomerFavouriteshow(int? CustomerId, int psize, int pno)
        {
            DataTable dt = new DataTable();
            try
            {
                if (CustomerId == 0) CustomerId = null;


                string query = "DECLARE @stQuery NVARCHAR(MAX) SET @stQuery = 'WITH Paged AS(SELECT	DISTINCT DENSE_RANK() OVER (Order by pm.OrderBy ASC) AS Rownumber,";
                query += " cf.[ProductId] AS ProductId,pm.[CategoryId],pm.[ProductName],pm.[Code],pm.[Price],pm.[DiscountAmount],pm.[CGST],pm.[SGST],pm.[IGST],pm.[RewardPoint],pm.[Detail],pm.[Image],pm.[IsActive],pm.[PurchasePrice],pm.[SalePrice],pm.[Profit],pm.[IsDaily],pm.[OrderBy],pcm.CategoryName";
                query += " from tbl_Product_Master pm left join [dbo].[tbl_Product_Category_Master] pcm on pcm.Id = pm.CategoryId";
                query += " join [dbo].[tbl_Customer_Favourite] cf on pm.Id = cf.ProductId join [dbo].[tbl_Customer_Master] cm on cf.CustomerId=cm.Id";
                query += " join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId=pm.Id AND cm.SectorId=vpa.SectorId ' SET @stQuery = @stQuery + ' WHERE 1=1 AND pm.[IsActive]=1 '";
                query += " SET @stQuery =	@stQuery+')	SELECT	*,(SELECT COUNT(1) FROM Paged) TotalRecords FROM  Paged WHERE Paged.Rownumber ';";
                query += " SET @stQuery =@stQuery + ' BETWEEN (' + CONVERT(NVARCHAR(10), @PageNum) + ' - 1) * ' + CONVERT(NVARCHAR(10),@PageSize) + ' + 1 AND ' + CONVERT(NVARCHAR(10), @PageNum) + ' * ' + CONVERT(NVARCHAR(10), @PageSize)";
                query += " EXEC(@stQuery)";
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);





                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(CustomerId.ToString()))
                    cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
                else
                    cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                cmd.Parameters.AddWithValue("@PageNum", pno);
                cmd.Parameters.AddWithValue("@PageSize", psize);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex) { string s = ex.Message; }
            return dt;
        }
        public DataTable CheckCustomerOrderByDate(int? CustomerId, int? ProductId, string Date)
        {
            DataTable dt = new DataTable();
            try
            {
                if (CustomerId == 0) CustomerId = null;
                if (ProductId == 0) ProductId = null;
                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();
                SqlCommand cmd = new SqlCommand("SP_Check_Customer_Order", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (!string.IsNullOrEmpty(CustomerId.ToString()))
                    cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
                else
                    cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                if (!string.IsNullOrEmpty(ProductId.ToString()))
                    cmd.Parameters.AddWithValue("@ProductId", ProductId);
                else
                    cmd.Parameters.AddWithValue("@ProductId", DBNull.Value);
                if (!string.IsNullOrEmpty(Date.ToString()))
                    cmd.Parameters.AddWithValue("@OrderDate", Date);
                else
                    cmd.Parameters.AddWithValue("@OrderDate", DBNull.Value);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex) { string s = ex.Message; }
            return dt;
        }

        public DataTable CheckCustomerOrderByQtyDate(int? CustomerId, int? ProductId, int? Qty, string Date,int? AttributeId)
        {
            DataTable dt = new DataTable();
            try
            {
                if (CustomerId == 0) CustomerId = null;
                if (ProductId == 0) ProductId = null;
                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();
                //SqlCommand cmd = new SqlCommand("SP_Check_Customer_Order_Qty_Date", con);
                SqlCommand cmd = new SqlCommand("SP_Check_Customer_Order_Qty_Datenewversion", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (!string.IsNullOrEmpty(CustomerId.ToString()))
                    cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
                else
                    cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                if (!string.IsNullOrEmpty(ProductId.ToString()))
                    cmd.Parameters.AddWithValue("@ProductId", ProductId);
                else
                    cmd.Parameters.AddWithValue("@ProductId", DBNull.Value);
                if (!string.IsNullOrEmpty(Qty.ToString()))
                    cmd.Parameters.AddWithValue("@Qty", Qty);
                else
                    cmd.Parameters.AddWithValue("@Qty", DBNull.Value);
                if (!string.IsNullOrEmpty(Date.ToString()))
                    cmd.Parameters.AddWithValue("@OrderDate", Date);
                else
                    cmd.Parameters.AddWithValue("@OrderDate", DBNull.Value);


                if (!string.IsNullOrEmpty(AttributeId.ToString()))
                    cmd.Parameters.AddWithValue("@AttributeId", AttributeId);
                else
                    cmd.Parameters.AddWithValue("@AttributeId", DBNull.Value);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex) { string s = ex.Message; }
            return dt;
        }

        public int DeleteCustomerFavorite(int CustomerId, int ProductId)
        {
            try
            {
                Customer _customer = new Customer();
                var favourite = _customer.CheckCustomerFavourite(CustomerId, ProductId);
                if (favourite.Rows.Count > 0)
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("Delete from tbl_Customer_Favourite where CustomerId='" + CustomerId + "' and ProductId='" + ProductId + "'", con);
                    int i = cmd.ExecuteNonQuery();
                    con.Close();
                    return 1;
                }
            }
            catch { }
            return 0;
        }
        #endregion

        #region Customer Inquiry
        public int insertInquiry(string FirstName, string LastName, string Mobile, string Email, string Address1, string Address2, string Address3)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Inquiry_Insert", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FirstName", FirstName);
                cmd.Parameters.AddWithValue("@LastName", LastName);
                cmd.Parameters.AddWithValue("@MobileNo", Mobile);
                cmd.Parameters.AddWithValue("@Email", Email);
                cmd.Parameters.AddWithValue("@Address1", Address1);
                cmd.Parameters.AddWithValue("@Address2", Address2);
                cmd.Parameters.AddWithValue("@Address3", Address3);
                cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex) { string s = ex.Message; }
            return i;
        }

        public DataTable getCustomerInquiry()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand(@"select * from tbl_Inquiry Order By Id DESC ", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
        #endregion

        public DataTable GetCustomerReferralList(int? Id, DateTime? FDate, DateTime? TDate)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SP_Customer_Referral_List", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", Id);
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
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public int UpdatevendorOrder(int? OrderId,int? newvenId)
        {
            int i = 0;
            int vencatid = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Update tbl_Customer_Order_Detail set VendorId=@VendorId,VendorCatId=@VendorCatId Where OrderId=@OrderId", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@VendorId", newvenId);
                com.Parameters.AddWithValue("@VendorCatId", vencatid);
                com.Parameters.AddWithValue("@OrderId", OrderId);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }

            return i;
        }
    }
}
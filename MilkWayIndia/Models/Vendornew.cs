using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
namespace MilkWayIndia.Models
{
    public class Vendornew
    {

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);

        public int Id { get; set; }
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


        public long CompanyName { get; set; }
        public string PanCardNo { get; set; }
        public int GSTNo { get; set; }
        public Decimal Credit { get; set; }
        public int OrderBy { get; set; }

        public int VendorId { get; set; }
        public string Description { get; set; }
        //assign delivery boy
        public int CustomerId { get; set; }
        public int StaffId { get; set; }

        //otp 
        public string OTP { get; set; }
        public DateTime LastUpdateOtpDate { get; set; }
        public int Count { get; set; }



        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }


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
        public string Status { get; set; }

        public string GSTnewfield1 { get; set; }
        public string GSTnewfield2 { get; set; }
        public string newstatus { get; set; }

        public string lat { get; set; }
        public string lon { get; set; }
        clsCommon _clsCommon = new clsCommon();
        Helper dHelper = new Helper();


        public DataTable CheckVendorUserName(string UserName)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[tbl_Vendor_Master] WHERE (@UserName IS NULL OR MobileNo = @UserName)", con);
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

        public DataTable Vendorlogin(string UserName, string Password)
        {
            con.Open();
            SqlCommand cmdLoginUser = new SqlCommand("select * from tbl_Vendor_Master WHERE MobileNo='" + UserName + "' and CAST(Password AS VARBINARY(100)) =CAST(@Password AS VARBINARY(100))", con);
            cmdLoginUser.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(UserName))
                cmdLoginUser.Parameters.AddWithValue("@Password", Password);
            else
                cmdLoginUser.Parameters.AddWithValue("@Password", DBNull.Value);


            SqlDataAdapter daLoginUser = new SqlDataAdapter(cmdLoginUser);
            DataTable dtLoginUser = new DataTable();
            daLoginUser.Fill(dtLoginUser);
            con.Close();
            return dtLoginUser;
        }





        public DataTable getVendorCustomerOrder(int? VendorId)
        {

            if (VendorId == 0) VendorId = null;

            string status = "Complete";
            //string status = "Complete";
            int NoDays = 0;
            //ToDate = DateTime.Today.AddDays(1);
            //FromDate = DateTime.Today;
            //ToDate = DateTime.Today.AddDays(-43);AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus)
            //FromDate = DateTime.Today.AddDays(-43);
            ToDate = DateTime.Today;
            FromDate = DateTime.Today;
            //SqlCommand cmd = new SqlCommand("SELECT otrans.Id AS Id,sm.FirstName +' '+ sm.LastName as DeliveryBoy,secm.SectorName,cm.FirstName +' '+ cm.LastName as Customer, otrans.OrderNo,convert(varchar,otrans.OrderDate,23) as OrderDate, prodt.ProductName, odetail.Qty,odetail.Amount,cm.Address, otrans.[Status],cm.orderby,secm.Id as sid from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (@CustomerId IS NULL OR otrans.CustomerId=@CustomerId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND odetail.Qty <> 0 AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus) 	order by cm.OrderBy,Customer", con);

            SqlCommand cmd = new SqlCommand("SELECT MAX(DISTINCT otrans.Id) AS Id,MAX( DISTINCT otrans.OrderNo) as OrderNo,MAX(CONVERT(VARCHAR,otrans.OrderDate,23)) AS OrderDate,SUM(odetail.Qty) AS Qty,MAX(vm.FirstName + ' '+ vm.LastName) AS Vendor,MAX(prodt.ProductName) AS ProductName,MAX(prodt.Image) as image,	MAX(SectorName) AS Sector,(MAX(prodt.PurchasePrice)*SUM(odetail.Qty)) AS PurchasePrice,MAX(otrans.[Status]) As status,MAX(otrans.DeliveryStatus) As DeliveryStatus FROM [dbo].[tbl_Customer_Master] cm left join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId left join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id left join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId WHERE (@VendorId IS NULL OR vm.Id=@VendorId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND (odetail.Qty<>0)  GROUP BY prodt.Id", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);

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


        public DataTable getVendorTomCustomerOrder(int? VendorId)
        {

            if (VendorId == 0) VendorId = null;

            string status = "Complete";
            //string status = "Complete";
            int NoDays = 0;
            //ToDate = DateTime.Today.AddDays(1);
            //FromDate = DateTime.Today;
            //ToDate = DateTime.Today.AddDays(-43);AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus)
            //FromDate = DateTime.Today.AddDays(-43);
            ToDate = DateTime.Today.AddDays(1);
            FromDate = DateTime.Today.AddDays(1);
            //SqlCommand cmd = new SqlCommand("SELECT otrans.Id AS Id,sm.FirstName +' '+ sm.LastName as DeliveryBoy,secm.SectorName,cm.FirstName +' '+ cm.LastName as Customer, otrans.OrderNo,convert(varchar,otrans.OrderDate,23) as OrderDate, prodt.ProductName, odetail.Qty,odetail.Amount,cm.Address, otrans.[Status],cm.orderby,secm.Id as sid from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (@CustomerId IS NULL OR otrans.CustomerId=@CustomerId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND odetail.Qty <> 0 AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus) 	order by cm.OrderBy,Customer", con);

            SqlCommand cmd = new SqlCommand("SELECT MAX(DISTINCT otrans.Id) AS Id,MAX( DISTINCT otrans.OrderNo) as OrderNo,MAX(CONVERT(VARCHAR,otrans.OrderDate,23)) AS OrderDate,SUM(odetail.Qty) AS Qty,MAX(vm.FirstName + ' '+ vm.LastName) AS Vendor,MAX(prodt.ProductName) AS ProductName,MAX(prodt.Image) as image,	MAX(SectorName) AS Sector,(MAX(prodt.PurchasePrice)*SUM(odetail.Qty)) AS PurchasePrice,MAX(otrans.[Status]) As status,MAX(otrans.DeliveryStatus) As DeliveryStatus FROM [dbo].[tbl_Customer_Master] cm left join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId left join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id left join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId WHERE (@VendorId IS NULL OR vm.Id=@VendorId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND (odetail.Qty<>0)  GROUP BY prodt.Id", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);

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


        public DataTable getSectorProductList(int? VendorId)
        {
            //con.Open();
            string query = "SELECT vpa.*,sm.SectorName,vm.FirstName,vm.LastName,pcm.CategoryName,pm.ProductName,pm.Image ";
            query += " from[dbo].[tbl_Vendor_Product_Assign] vpa";
            query += " left join[dbo].[tbl_Sector_Master] sm on sm.Id = vpa.SectorId";

            query += "  left join[dbo].[tbl_Vendor_Master] vm on vm.Id = vpa.VendorId";
            query += " left join[dbo].[tbl_Product_Master] pm on pm.Id = vpa.ProductId";

            query += " left join[dbo].[tbl_Product_Category_Master] pcm on pcm.Id = pm.CategoryId";
            query += " WHERE (@VendorId IS NULL OR vpa.[VendorId] = @VendorId)";
            if (VendorId == 0) VendorId = null;

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public int InsertProductPhoto(Vendornew obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("INSERT INTO tbl_Vendor_ProductPhoto(Photo,Description,Uploaddate,VendorId)VALUES(@Photo,@Description,@Uploaddate,@VendorId)", con);
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


                if (!string.IsNullOrEmpty(obj.VendorId.ToString()))
                    com.Parameters.AddWithValue("@VendorId", obj.VendorId);
                else
                    com.Parameters.AddWithValue("@VendorId", DBNull.Value);
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public DataTable getVendorwiseDoc(int? id)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            //and(@FromDate is null or @ToDate is null or convert(varchar, Dc.Updatedon, 23) between @FromDate and @Todate)
            cmd = new SqlCommand("SELECT * from tbl_Vendor_doc  where VendorId=" + id + "", con);
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

        public int InsertDoc(Vendornew obj)
        {

            int i = 0;
            ToDate = DateTime.Today;
            string Status = "Pending";
            string Description = "";
            con.Open();
            SqlCommand com = new SqlCommand("Insert Into tbl_Vendor_doc(VendorId,Aadhar,Pan,License,BankAccount,Ifsc,Bankname,AccholderName,Aadharstatus,Panstatus,Licensestatus,Bankstatus,Status,Decsiption,Updatedon,Gstnewfield1,Gstnewfield2,newstatus)Values(@VendorId,@Aadhar,@Pan,@License,@BankAccount,@Ifsc,@Bankname,@AccholderName,@Aadharstatus,@Panstatus,@Licensestatus,@Bankstatus,@Status,@Description,@Updatedon,@Gstnewfield1,@Gstnewfield2,@newstatus)", con);
            com.CommandType = CommandType.Text;
            com.Parameters.AddWithValue("@VendorId", obj.VendorId);
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


            com.Parameters.AddWithValue("@Gstnewfield1", obj.GSTnewfield1);
            com.Parameters.AddWithValue("@Gstnewfield2", obj.GSTnewfield2);


            if (!string.IsNullOrEmpty(obj.newstatus.ToString()))
                com.Parameters.AddWithValue("@newstatus", obj.newstatus);
            else
                com.Parameters.AddWithValue("@newstatus", DBNull.Value);

            com.Parameters.AddWithValue("@Updatedon", ToDate);
            i = com.ExecuteNonQuery();
            con.Close();
            return i;
        }
        public int UpdateDoc(Vendornew obj)
        {
            int i = 0;
            obj.Status = "Pending";
            try
            {
                ToDate = DateTime.Today;
                con.Open();
                if (!string.IsNullOrEmpty(obj.Aadhar.ToString()))
                {
                    SqlCommand com = new SqlCommand("Update tbl_Vendor_doc set Aadhar=@Aadhar,BankAccount=@BankAccount,Ifsc=@Ifsc,Bankname=@Bankname,AccholderName=@AccholderName, Aadharstatus=@Aadharstatus,Bankstatus=@Bankstatus,Status=@Status,Updatedon=@Updatedon,Gstnewfield1=@Gstnewfield1,Gstnewfield2=@Gstnewfield2,newstatus=@newstatus WHERE (@Id IS NULL OR [VendorId] = @Id)", con);
                    com.CommandType = CommandType.Text;
                    com.Parameters.AddWithValue("@Id", obj.VendorId);

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

                    com.Parameters.AddWithValue("@Gstnewfield1", obj.GSTnewfield1);
                    com.Parameters.AddWithValue("@Gstnewfield2", obj.GSTnewfield2);


                    if (!string.IsNullOrEmpty(obj.newstatus.ToString()))
                        com.Parameters.AddWithValue("@newstatus", obj.newstatus);
                    else
                        com.Parameters.AddWithValue("@newstatus", DBNull.Value);

                    com.Parameters.AddWithValue("@Updatedon", ToDate);



                    i = com.ExecuteNonQuery();
                    //con.Close();
                }

                if (!string.IsNullOrEmpty(obj.Pan.ToString()))
                {
                    SqlCommand com = new SqlCommand("Update tbl_Vendor_doc set Pan=@Pan,BankAccount=@BankAccount,Ifsc=@Ifsc,Bankname=@Bankname,AccholderName=@AccholderName, Panstatus=@Panstatus,Bankstatus=@Bankstatus,Status=@Status,Updatedon=@Updatedon,Gstnewfield1=@Gstnewfield1,Gstnewfield2=@Gstnewfield2,newstatus=@newstatus WHERE (@Id IS NULL OR [VendorId] = @Id)", con);
                    com.CommandType = CommandType.Text;
                    com.Parameters.AddWithValue("@Id", obj.VendorId);

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


                    com.Parameters.AddWithValue("@Gstnewfield1", obj.GSTnewfield1);
                    com.Parameters.AddWithValue("@Gstnewfield2", obj.GSTnewfield2);


                    if (!string.IsNullOrEmpty(obj.newstatus.ToString()))
                        com.Parameters.AddWithValue("@newstatus", obj.newstatus);
                    else
                        com.Parameters.AddWithValue("@newstatus", DBNull.Value);



                    com.Parameters.AddWithValue("@Updatedon", ToDate);


                    i = com.ExecuteNonQuery();
                }

                if (!string.IsNullOrEmpty(obj.License.ToString()))
                {
                    SqlCommand com = new SqlCommand("Update tbl_Vendor_doc set License=@License,BankAccount=@BankAccount,Ifsc=@Ifsc,Bankname=@Bankname,AccholderName=@AccholderName, Licensestatus=@Licensestatus,Bankstatus=@Bankstatus,Status=@Status,Updatedon=@Updatedon,Gstnewfield1=@Gstnewfield1,Gstnewfield2=@Gstnewfield2,newstatus=@newstatus WHERE (@Id IS NULL OR [VendorId] = @Id)", con);
                    com.CommandType = CommandType.Text;
                    com.Parameters.AddWithValue("@Id", obj.VendorId);

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


                    com.Parameters.AddWithValue("@Gstnewfield1", obj.GSTnewfield1);
                    com.Parameters.AddWithValue("@Gstnewfield2", obj.GSTnewfield2);


                    if (!string.IsNullOrEmpty(obj.newstatus.ToString()))
                        com.Parameters.AddWithValue("@newstatus", obj.newstatus);
                    else
                        com.Parameters.AddWithValue("@newstatus", DBNull.Value);


                    com.Parameters.AddWithValue("@Updatedon", ToDate);


                    i = com.ExecuteNonQuery();
                }

                if (string.IsNullOrEmpty(obj.License.ToString()) && string.IsNullOrEmpty(obj.Aadhar.ToString()) && string.IsNullOrEmpty(obj.Pan.ToString()))
                {
                    SqlCommand com1 = new SqlCommand("Update tbl_Vendor_doc set BankAccount=@BankAccount,Ifsc=@Ifsc,Bankname=@Bankname,AccholderName=@AccholderName, Bankstatus=@Bankstatus,Status=@Status,Updatedon=@Updatedon,Gstnewfield1=@Gstnewfield1,Gstnewfield2=@Gstnewfield2,newstatus=@newstatus WHERE (@Id IS NULL OR [VendorId] = @Id)", con);
                    com1.CommandType = CommandType.Text;
                    com1.Parameters.AddWithValue("@Id", obj.VendorId);

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


                    com1.Parameters.AddWithValue("@Gstnewfield1", obj.GSTnewfield1);
                    com1.Parameters.AddWithValue("@Gstnewfield2", obj.GSTnewfield2);


                    if (!string.IsNullOrEmpty(obj.newstatus.ToString()))
                        com1.Parameters.AddWithValue("@newstatus", obj.newstatus);
                    else
                        com1.Parameters.AddWithValue("@newstatus", DBNull.Value);



                    com1.Parameters.AddWithValue("@Updatedon", ToDate);


                    i = com1.ExecuteNonQuery();
                }


                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }




        public DataTable getVendorCustomerOrderDatewise(int? VendorId, DateTime? Fromdate, DateTime? Todate)
        {

            if (VendorId == 0) VendorId = null;

            string status = "Complete";
            //string status = "Complete";
            int NoDays = 0;

            ToDate = Todate;
            FromDate = Fromdate;
            //SqlCommand cmd = new SqlCommand("SELECT otrans.Id AS Id,sm.FirstName +' '+ sm.LastName as DeliveryBoy,secm.SectorName,cm.FirstName +' '+ cm.LastName as Customer, otrans.OrderNo,convert(varchar,otrans.OrderDate,23) as OrderDate, prodt.ProductName, odetail.Qty,odetail.Amount,cm.Address, otrans.[Status],cm.orderby,secm.Id as sid from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (@CustomerId IS NULL OR otrans.CustomerId=@CustomerId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND odetail.Qty <> 0 AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus) 	order by cm.OrderBy,Customer", con);

            //SqlCommand cmd = new SqlCommand("SELECT MAX(DISTINCT otrans.Id) AS Id,MAX( DISTINCT otrans.OrderNo) as OrderNo,MAX(CONVERT(VARCHAR,otrans.OrderDate,23)) AS OrderDate,SUM(odetail.Qty) AS Qty,MAX(vm.FirstName + ' '+ vm.LastName) AS Vendor,MAX(prodt.ProductName) AS ProductName,MAX(prodt.Image) as image,	MAX(SectorName) AS Sector,(MAX(prodt.PurchasePrice)*SUM(odetail.Qty)) AS PurchasePrice,MAX(otrans.[Status]) As status,MAX(otrans.DeliveryStatus) As DeliveryStatus FROM [dbo].[tbl_Customer_Master] cm left join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId left join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id left join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId WHERE (@VendorId IS NULL OR vm.Id=@VendorId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND (odetail.Qty<>0)  GROUP BY prodt.Id", con);

            SqlCommand cmd = new SqlCommand("SELECT MAX(DISTINCT otrans.Id) AS Id,MAX( DISTINCT otrans.OrderNo) as OrderNo, (CONVERT(VARCHAR,otrans.OrderDate,23)) AS OrderDate,(SUM(odetail.Qty)) AS Qty,(vm.FirstName + ' '+ vm.LastName) AS Vendor,(prodt.ProductName) AS ProductName,(prodt.Image) as image,	(SectorName) AS Sector,((prodt.PurchasePrice)*SUM(odetail.Qty)) AS PurchasePrice,(otrans.[Status]) As status,(otrans.DeliveryStatus) As DeliveryStatus,Vm.MobileNo FROM [dbo].[tbl_Customer_Master] cm left join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId left join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id left join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId   WHERE (@VendorId IS NULL OR vm.Id=@VendorId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND (odetail.Qty<>0)  GROUP BY prodt.Id,(CONVERT(VARCHAR,otrans.OrderDate,23)), vm.FirstName,vm.LastName,prodt.ProductName,prodt.Image,SectorName,prodt.PurchasePrice,otrans.[Status],otrans.DeliveryStatus,Vm.MobileNo Order by (CONVERT(VARCHAR,otrans.OrderDate,23))", con);

            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);

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


        public DataTable getVendorPaymentList(int? VendorId)
        {
            //con.Open();
            string query = "SELECT vpa.* ";
            query += " from[dbo].[tbl_Vendor_Payment] vpa";

            query += " WHERE (@VendorId IS NULL OR vpa.[VendorId] = @VendorId) Order By CONVERT(varchar,vpa.UpdatedOn,23) Desc";
            if (VendorId == 0) VendorId = null;

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }



        public DataTable getVendorCustomerOrderSingleDate(int? VendorId, DateTime? Fromdate)
        {

            if (VendorId == 0) VendorId = null;

            string status = "Complete";
            //string status = "Complete";
            int NoDays = 0;
            //ToDate = DateTime.Today.AddDays(1);
            //FromDate = DateTime.Today;
            //ToDate = DateTime.Today.AddDays(-43);AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus)
            //FromDate = DateTime.Today.AddDays(-43);

            FromDate = Fromdate;
            //SqlCommand cmd = new SqlCommand("SELECT otrans.Id AS Id,sm.FirstName +' '+ sm.LastName as DeliveryBoy,secm.SectorName,cm.FirstName +' '+ cm.LastName as Customer, otrans.OrderNo,convert(varchar,otrans.OrderDate,23) as OrderDate, prodt.ProductName, odetail.Qty,odetail.Amount,cm.Address, otrans.[Status],cm.orderby,secm.Id as sid from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (@CustomerId IS NULL OR otrans.CustomerId=@CustomerId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND odetail.Qty <> 0 AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus) 	order by cm.OrderBy,Customer", con);

            //SqlCommand cmd = new SqlCommand("SELECT MAX(DISTINCT otrans.Id) AS Id,MAX( DISTINCT otrans.OrderNo) as OrderNo,MAX(CONVERT(VARCHAR,otrans.OrderDate,23)) AS OrderDate,SUM(odetail.Qty) AS Qty,MAX(vm.FirstName + ' '+ vm.LastName) AS Vendor,MAX(prodt.ProductName) AS ProductName,MAX(prodt.Image) as image,	MAX(SectorName) AS Sector,(MAX(prodt.PurchasePrice)*SUM(odetail.Qty)) AS PurchasePrice,MAX(otrans.[Status]) As status,MAX(otrans.DeliveryStatus) As DeliveryStatus FROM [dbo].[tbl_Customer_Master] cm left join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId left join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id left join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId WHERE (@VendorId IS NULL OR vm.Id=@VendorId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND (odetail.Qty<>0)  GROUP BY prodt.Id", con);

            string query = "SELECT MAX(DISTINCT otrans.Id) AS Id,max(otrans.OrderNo) as OrderNo,max(convert(varchar,otrans.OrderDate,23)) as OrderDate,";
            query += " sum(odetail.Qty) as Qty,MAX(Concat(vm.FirstName,' ', vm.LastName)) AS Vendor,max(prodt.ProductName) as ProductName,MAX(prodt.Image) as image,MAX(otrans.[Status]) As status, ";
            query += " max(SectorName) as Sector,(max(prodt.PurchasePrice)*sum(odetail.Qty)) as PurchasePrice,max(SectorName) as Sector,(max(prodt.Price)*sum(odetail.Qty)) as MRP,max(prodt.SalePrice)*sum(odetail.Qty) as CustomerPrice,(max(prodt.SalePrice)*sum(odetail.Qty)-max(prodt.PurchasePrice)*sum(odetail.Qty)) AS Profit,MAX(otrans.DeliveryStatus) As DeliveryStatus,MAX(vm.MobileNo) AS MobileNo from [dbo].[tbl_Staff_Master] sm";
            query += " left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id";
            query += " left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId";
            query += " left join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId";
            query += " left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id";

            query += " and (@FromDate is null or @ToDate is null or convert(varchar,otrans.Orderdate,23) between @FromDate and @ToDate) ";

            query += " left join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id";
            query += " left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId";
            query += " left join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id AND vpa.SectorId=cm.SectorId";
            query += " left join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId where";
            query += "  (@VendorId is null or vm.Id=@VendorId)";

            query += " and odetail.Qty <> 0 and  vpa.IsActive = 1 ";
            query += " Group by prodt.Id Order by max(convert(varchar,otrans.OrderDate,23)) ASC";
            SqlCommand cmd = new SqlCommand(query, con);
            // SqlCommand cmd = new SqlCommand("SELECT MAX(DISTINCT otrans.Id) AS Id,MAX( DISTINCT otrans.OrderNo) as OrderNo, (CONVERT(VARCHAR,otrans.OrderDate,23)) AS OrderDate,(SUM(odetail.Qty)) AS Qty,(vm.FirstName + ' '+ vm.LastName) AS Vendor,(prodt.ProductName) AS ProductName,(prodt.Image) as image,	(SectorName) AS Sector,((prodt.PurchasePrice)*SUM(odetail.Qty)) AS PurchasePrice,(otrans.[Status]) As status,(otrans.DeliveryStatus) As DeliveryStatus FROM [dbo].[tbl_Customer_Master] cm left join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId left join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id left join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId   WHERE (@VendorId IS NULL OR vm.Id=@VendorId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND (odetail.Qty<>0)   GROUP BY prodt.Id,(CONVERT(VARCHAR,otrans.OrderDate,23)), vm.FirstName,vm.LastName,prodt.ProductName,prodt.Image,SectorName,prodt.PurchasePrice,otrans.[Status],otrans.DeliveryStatus Order by (CONVERT(VARCHAR,otrans.OrderDate,23))", con);

            cmd.CommandType = CommandType.Text;



            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);

            if (!string.IsNullOrEmpty(FromDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FromDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(Fromdate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", Fromdate);
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



        public DataTable getVendorCustomerOrderDatewisenew(int? VendorId, DateTime? Fromdate, DateTime? Todate)
        {

            if (VendorId == 0) VendorId = null;

            string status = "Complete";
            //string status = "Complete";
            int NoDays = 0;
            //ToDate = DateTime.Today.AddDays(1);
            //FromDate = DateTime.Today;
            //ToDate = DateTime.Today.AddDays(-43);AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus)
            //FromDate = DateTime.Today.AddDays(-43);
            ToDate = Todate;
            FromDate = Fromdate;
            //SqlCommand cmd = new SqlCommand("SELECT otrans.Id AS Id,sm.FirstName +' '+ sm.LastName as DeliveryBoy,secm.SectorName,cm.FirstName +' '+ cm.LastName as Customer, otrans.OrderNo,convert(varchar,otrans.OrderDate,23) as OrderDate, prodt.ProductName, odetail.Qty,odetail.Amount,cm.Address, otrans.[Status],cm.orderby,secm.Id as sid from [tbl_Staff_Master] sm left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId left join [tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [tbl_Product_Master] prodt on prodt.Id = odetail.ProductId WHERE (@DeliveryBoyId IS NULL OR sm.Id=@DeliveryBoyId) AND (@CustomerId IS NULL OR otrans.CustomerId=@CustomerId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND odetail.Qty <> 0 AND (@OrderStatus IS NULL OR otrans.[Status] = @OrderStatus) 	order by cm.OrderBy,Customer", con);

            //SqlCommand cmd = new SqlCommand("SELECT MAX(DISTINCT otrans.Id) AS Id,MAX( DISTINCT otrans.OrderNo) as OrderNo,MAX(CONVERT(VARCHAR,otrans.OrderDate,23)) AS OrderDate,SUM(odetail.Qty) AS Qty,MAX(vm.FirstName + ' '+ vm.LastName) AS Vendor,MAX(prodt.ProductName) AS ProductName,MAX(prodt.Image) as image,	MAX(SectorName) AS Sector,(MAX(prodt.PurchasePrice)*SUM(odetail.Qty)) AS PurchasePrice,MAX(otrans.[Status]) As status,MAX(otrans.DeliveryStatus) As DeliveryStatus FROM [dbo].[tbl_Customer_Master] cm left join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId left join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id left join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId WHERE (@VendorId IS NULL OR vm.Id=@VendorId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND (odetail.Qty<>0)  GROUP BY prodt.Id", con);

            //string query = "SELECT  otrans.Id AS Id,(convert(varchar,otrans.OrderDate,23)) as OrderDate,";
            //query += " Concat(vm.FirstName,' ', vm.LastName) AS Vendor ";
            //query += " from [dbo].[tbl_Staff_Master] sm";
            //query += " left join [dbo].[tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id";
            //query += " left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId";
            //query += " left join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId";
            //query += " left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id";

            //query += " and (@FromDate is null or @ToDate is null or convert(varchar,otrans.Orderdate,23) between @FromDate and @Todate) ";

            //query += " left join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id";
            //query += " left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId";
            //query += " left join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id AND vpa.SectorId=cm.SectorId";
            //query += " left join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId where";
            //query += "  (@VendorId is null or vm.Id=@VendorId)";

            //query += " and odetail.Qty <> 0 and  vpa.IsActive = 1 ";
            //query += " Group by otrans.Id,otrans.OrderDate,vm.FirstName,vm.LastName";


            SqlCommand cmd = new SqlCommand("SELECT DISTINCT (CONVERT(VARCHAR,otrans.OrderDate,23)) AS OrderDate,(vm.FirstName + ' '+ vm.LastName) AS Vendor FROM [dbo].[tbl_Customer_Master] cm left join [dbo].[tbl_Sector_Master] secm on secm.Id = cm.SectorId left join [dbo].[tbl_Customer_Order_Transaction] otrans on otrans.CustomerId = cm.Id left join [dbo].[tbl_Customer_Order_Detail] odetail on odetail.OrderId = otrans.Id left join [dbo].[tbl_Product_Master] prodt on prodt.Id = odetail.ProductId left join [dbo].[tbl_Vendor_Product_Assign] vpa on vpa.ProductId = prodt.Id left join  [tbl_Vendor_Master] vm on vm.Id = vpa.VendorId      WHERE (@VendorId IS NULL OR vm.Id=@VendorId) AND (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,otrans.Orderdate,23) BETWEEN @FromDate AND @Todate) AND (odetail.Qty<>0)  GROUP BY prodt.Id,(CONVERT(VARCHAR,otrans.OrderDate,23)),vm.FirstName,vm.LastName", con);
            //SqlCommand cmd =new SqlCommand (query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);

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



        public int UpdateContact(int VendorId, string ContactNo)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("UPDATE tbl_Vendor_Master set MobileNo=@ContactNo Where Id = @VendorId", con);
                com.CommandType = CommandType.Text;

                com.Parameters.AddWithValue("@VendorId", VendorId);
                com.Parameters.AddWithValue("@ContactNo", ContactNo);


                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;
        }


        public DataTable getVendorDocStaus(int? VendorId)
        {
            if (VendorId == 0) VendorId = null;

            string status = "Pending";
            ToDate = DateTime.Today;
            FromDate = DateTime.Today;
            SqlCommand cmd = new SqlCommand("SElect * From tbl_Vendor_doc  WHERE (@VendorId IS NULL OR VendorId=@VendorId)", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);



            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }



        public DataTable getVendorphotoListReport(DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();

            cmd = new SqlCommand(" Select Vc.*,Concat(Vm.FirstName,' ',Vm.LastName) As Name from tbl_Vendor_ProductPhoto Vc inner join tbl_Vendor_Master Vm on Vc.VendorId=Vm.Id   where  (@FromDate is null or @ToDate is null or convert(varchar,Vc.Uploaddate,23) between @FromDate and @Todate)", con);
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


        public DataTable getVendorphotoListReport1(int id, DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            if (id == 0)
            {
                cmd = new SqlCommand(" Select Vp.*,Concat(Vm.FirstName,' ',Vm.LastName) As Name from tbl_Vendor_ProductPhoto Vp inner join tbl_Vendor_Master Vm on Vp.VendorId=Vm.Id   where  (@FromDate is null or @ToDate is null or convert(varchar,Vp.Uploaddate,23) between @FromDate and @Todate)", con);
            }
            else
            {
                cmd = new SqlCommand(" Select Vp.*,Concat(Vm.FirstName,' ',Vm.LastName) As Name from tbl_Vendor_ProductPhoto Vp inner join tbl_Vendor_Master Vm on Vp.VendorId=Vm.Id   where Vp.VendorId=@Id  and (@FromDate is null or @ToDate is null or convert(varchar,Vp.Uploaddate,23) between @FromDate and @Todate)", con);
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




        public DataTable getVendorDocListReport(DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            //and(@FromDate is null or @ToDate is null or convert(varchar, Dc.Updatedon, 23) between @FromDate and @Todate)
            cmd = new SqlCommand("Select VC.*,Concat(Vm.FirstName,' ',Vm.LastName) As Name  from tbl_Vendor_doc VC inner join tbl_Vendor_Master Vm on VC.VendorId=Vm.Id  ", con);
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


        public DataTable getVendorDocListReport1(int id, DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            if (id == 0)
            {
                cmd = new SqlCommand("Select VC.*,Concat(Vm.FirstName,' ',Vm.LastName) As Name from tbl_Vendor_doc VC inner join tbl_Vendor_Master Vm on VC.VendorId=Vm.Id    where  (@FromDate is null or @ToDate is null or convert(varchar,VC.Updatedon,23) between @FromDate and @Todate)", con);
            }
            else
            {
                cmd = new SqlCommand("Select VC.*,Concat(Vm.FirstName,' ',Vm.LastName) As Name from tbl_Vendor_doc VC inner join tbl_Vendor_Master Vm on VC.VendorId=Vm.Id    where  VC.VendorId=@Id  and (@FromDate is null or @ToDate is null or convert(varchar,VC.Updatedon,23) between @FromDate and @Todate)", con);
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



        public DataTable getVendorDocReport(int? Id)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();

            cmd = new SqlCommand("Select * From tbl_Vendor_doc where (@Id IS NULL OR [Id] = @Id)", con);
            cmd.CommandType = CommandType.Text;



            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;



        }


        public int UpdateVendorDocstatus(Vendornew obj)
        {
            int i = 0;
            try
            {
                ToDate = DateTime.Today;
                con.Open();


                SqlCommand com = new SqlCommand("Update tbl_Vendor_doc set Aadharstatus=@Aadharstatus,Panstatus=@Panstatus,Licensestatus=@Licensestatus,Bankstatus=@Bankstatus,Status=@Status,Decsiption=@Decsiption,Updatedon=@Updatedon WHERE (@Id IS NULL OR [Id] = @Id)", con);
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



        public DataTable Vendorlogin2(int VendorId, string Password)
        {
            con.Open();
            SqlCommand cmdLoginUser = new SqlCommand("select * from tbl_Vendor_Master WHERE Id=" + VendorId + " and Password='" + Password + "'", con);
            SqlDataAdapter daLoginUser = new SqlDataAdapter(cmdLoginUser);
            DataTable dtLoginUser = new DataTable();
            daLoginUser.Fill(dtLoginUser);
            con.Close();
            return dtLoginUser;
        }

        public int UpdateVendorPwd(int Id, string Password)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Update tbl_Vendor_Master set Password=@Password where Id=@Id", con);
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

        public int UpdateAddress(int VendorId, string Lat, string Lon)
        {
            int i = 0;
            string Status = "Pending";
            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();
                SqlCommand com = new SqlCommand("UPDATE tbl_Vendor_Master set Lat=@Lat,Lon=@Lon,LocStatus=@LocStatus where Id=@Id", con);
                com.CommandType = CommandType.Text;

                // @CustomerId,@FirstName,@LastName,@MobileNo,@SectorId,@SectorName,@OrderId,@OrderDate,@OrderFrom
                if (!string.IsNullOrEmpty(VendorId.ToString()))
                    com.Parameters.AddWithValue("@Id", VendorId);
                else
                    com.Parameters.AddWithValue("@Id", DBNull.Value);
                if (!string.IsNullOrEmpty(Lat.ToString()))
                    com.Parameters.AddWithValue("@Lat", Lat);
                else
                    com.Parameters.AddWithValue("@Lat", DBNull.Value);
                if (!string.IsNullOrEmpty(Lon.ToString()))
                    com.Parameters.AddWithValue("@Lon", Lon);
                else
                    com.Parameters.AddWithValue("@Lon", DBNull.Value);

                if (!string.IsNullOrEmpty(Status))
                    com.Parameters.AddWithValue("@LocStatus", Status);
                else
                    com.Parameters.AddWithValue("@LocStatus", DBNull.Value);


                //com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }
    }
}
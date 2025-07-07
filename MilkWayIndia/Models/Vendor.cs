using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Models
{
    public class Vendor
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
        public int? SectorId { get; set; }
        public string Photo { get; set; }
        public string CompanyName { get; set; }
        public string PanCardNo { get; set; }
        public string GSTNo { get; set; }
        public bool IsActive { get; set; }
        public bool IsNotification { get; set; }
        public int? CustomerId { get; set; }
        public int? DeliveryBoyId { get; set; }
        public int? VendorId { get; set; }
        public int? CategoryId { get; set; }
        public int ProductId { get; set; }
        public string VendorType { get; set; }

        public string VendorCatName { get; set; }



        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }

        public decimal VendorMinAmount { get; set; }
        public decimal VendorMaxAmount { get; set; }
        public string MinAmount1 { get; set; }
        public string MaxAmount1 { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public TimeSpan DeliveryFrom { get; set; }
        public TimeSpan DeliveryTo { get; set; }


        public int? AttributeId { get; set; }
        public int? AttributeId1 { get; set; }
        public int? VendorCatId { get; set; }


        //public int? ID { get; set; }
        //public int? ProductID { get; set; }
        public int? VendorProductAssignID { get; set; }
       // public int? AttributeID { get; set; }
       public string ProductName { get; set; }
        public string Detail { get; set; }
        public int? SortOrder { get; set; }
        public decimal? MRPPrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal? CGST { get; set; }
        public decimal? SGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? Profit { get; set; }
        public decimal? SellPrice { get; set; }
        
        public Boolean? IsDeleted { get; set; }
        public decimal? B2BSellPrice { get; set; }
       
        public decimal? B2BProfit { get; set; }

        public decimal? Payamount { get; set; }
        public decimal? Totalamount { get; set; }
        public decimal? Paidamount { get; set; }
        public decimal? Outstandingamount { get; set; }
        public string PayRefference { get; set; }
        public DateTime PayRefdate { get; set; }

        public int? Qty { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public DateTime? updatedon { get; set; }

        public string OrderId { get; set; }


        public decimal? Offerper { get; set; }
        public decimal? Offeramount { get; set; }
        public string MultipleDay { get; set; }
        public string Weekday { get; set; }
        public string Offerdaytype { get; set; }

        public string VendorMasterCat { get; set; }
        public string VendorCat { get; set; }
        public string StoreName { get; set; }
        public decimal? MilkywayPer { get; set; }
        public string VendorTerms { get; set; }
        public string Slider1 { get; set; }
        public string Slider2 { get; set; }
        public int? State { get; set; }
        public int? City { get; set; }
        public decimal? OverallDiscountPer { get; set; }
        public bool DiscountIsActive { get; set; }
        public string OfferType { get; set; }
        public decimal? OfferValue { get; set; }
        
        public string OfferDates { get; set; }
        public string Products { get; set; }
        public string OfferId { get; set; }
        //

        public int InsertVendor(Vendor obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Vendor_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
                if (!string.IsNullOrEmpty(obj.FirstName))
                    com.Parameters.AddWithValue("@FirstName", obj.FirstName);
                else
                    com.Parameters.AddWithValue("@FirstName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.LastName))
                    com.Parameters.AddWithValue("@LastName", obj.LastName);
                else
                    com.Parameters.AddWithValue("@LastName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.MobileNo))
                    com.Parameters.AddWithValue("@MobileNo", obj.MobileNo);
                else
                    com.Parameters.AddWithValue("@MobileNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Email))
                    com.Parameters.AddWithValue("@Email", obj.Email);
                else
                    com.Parameters.AddWithValue("@Email", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Address))
                    com.Parameters.AddWithValue("@Address", obj.Address);
                else
                    com.Parameters.AddWithValue("@Address", DBNull.Value);
                com.Parameters.AddWithValue("@UserName", obj.UserName);
                com.Parameters.AddWithValue("@Password", obj.Password);
                if (!string.IsNullOrEmpty(obj.Photo))
                    com.Parameters.AddWithValue("@Photo", obj.Photo);
                else
                    com.Parameters.AddWithValue("@Photo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    com.Parameters.AddWithValue("@SectorId", obj.SectorId);
                else
                    com.Parameters.AddWithValue("@SectorId", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.CompanyName))
                    com.Parameters.AddWithValue("@CompanyName", obj.CompanyName);
                else
                    com.Parameters.AddWithValue("@CompanyName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.PanCardNo))
                    com.Parameters.AddWithValue("@PanCardNo", obj.PanCardNo);
                else
                    com.Parameters.AddWithValue("@PanCardNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.GSTNo))
                    com.Parameters.AddWithValue("@GSTNo", obj.GSTNo);
                else
                    com.Parameters.AddWithValue("@GSTNo", DBNull.Value);
                com.Parameters.AddWithValue("@IsActive", obj.IsActive);


                if (!string.IsNullOrEmpty(obj.VendorType))
                    com.Parameters.AddWithValue("@VendorType", obj.VendorType);
                else
                    com.Parameters.AddWithValue("@VendorType", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.VendorMasterCat))
                    com.Parameters.AddWithValue("@MasterCat", obj.VendorMasterCat);
                else
                    com.Parameters.AddWithValue("@MasterCat", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.StoreName))
                    com.Parameters.AddWithValue("@Vendorstore", obj.StoreName);
                else
                    com.Parameters.AddWithValue("@Vendorstore", DBNull.Value);



                if (!string.IsNullOrEmpty(obj.VendorCat))
                    com.Parameters.AddWithValue("@VendorCat", obj.VendorCat);
                else
                    com.Parameters.AddWithValue("@VendorCat", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.Slider1))
                    com.Parameters.AddWithValue("@Slider1", obj.Slider1);
                else
                    com.Parameters.AddWithValue("@Slider1", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.Slider2))
                    com.Parameters.AddWithValue("@Slider2", obj.Slider2);
                else
                    com.Parameters.AddWithValue("@Slider2", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.State.ToString()))
                    com.Parameters.AddWithValue("@State", obj.State);
                else
                    com.Parameters.AddWithValue("@State", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.City.ToString()))
                    com.Parameters.AddWithValue("@City", obj.City);
                else
                    com.Parameters.AddWithValue("@City", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.MilkywayPer.ToString()))
                    com.Parameters.AddWithValue("@MilkywayPer", obj.MilkywayPer);
                else
                    com.Parameters.AddWithValue("@MilkywayPer", 0);

                if (!string.IsNullOrEmpty(obj.OverallDiscountPer.ToString()))
                    com.Parameters.AddWithValue("@DiscountPer", obj.OverallDiscountPer);
                else
                    com.Parameters.AddWithValue("@DiscountPer", 0);

                if (!string.IsNullOrEmpty(obj.DiscountIsActive.ToString()))
                    com.Parameters.AddWithValue("@DiscountActive", obj.DiscountIsActive);
                else
                    com.Parameters.AddWithValue("@DiscountActive", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.VendorTerms))
                    com.Parameters.AddWithValue("@Vendorterms", obj.VendorTerms);
                else
                    com.Parameters.AddWithValue("@Vendorterms", DBNull.Value);

                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                int id = Convert.ToInt32(com.Parameters["@Id"].Value);
               
                con.Close();
                return id;
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int UpdateVendor(Vendor obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Vendor_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.Id);
                if (!string.IsNullOrEmpty(obj.FirstName))
                    com.Parameters.AddWithValue("@FirstName", obj.FirstName);
                else
                    com.Parameters.AddWithValue("@FirstName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.LastName))
                    com.Parameters.AddWithValue("@LastName", obj.LastName);
                else
                    com.Parameters.AddWithValue("@LastName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.MobileNo))
                    com.Parameters.AddWithValue("@MobileNo", obj.MobileNo);
                else
                    com.Parameters.AddWithValue("@MobileNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Email))
                    com.Parameters.AddWithValue("@Email", obj.Email);
                else
                    com.Parameters.AddWithValue("@Email", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Address))
                    com.Parameters.AddWithValue("@Address", obj.Address);
                else
                    com.Parameters.AddWithValue("@Address", DBNull.Value);
                com.Parameters.AddWithValue("@UserName", obj.UserName);
                com.Parameters.AddWithValue("@Password", obj.Password);
                if (!string.IsNullOrEmpty(obj.Photo))
                    com.Parameters.AddWithValue("@Photo", obj.Photo);
                else
                    com.Parameters.AddWithValue("@Photo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    com.Parameters.AddWithValue("@SectorId", obj.SectorId);
                else
                    com.Parameters.AddWithValue("@SectorId", DBNull.Value);
                com.Parameters.AddWithValue("@IsActive", obj.IsActive);
                if (!string.IsNullOrEmpty(obj.CompanyName))
                    com.Parameters.AddWithValue("@CompanyName", obj.CompanyName);
                else
                    com.Parameters.AddWithValue("@CompanyName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.PanCardNo))
                    com.Parameters.AddWithValue("@PanCardNo", obj.PanCardNo);
                else
                    com.Parameters.AddWithValue("@PanCardNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.GSTNo))
                    com.Parameters.AddWithValue("@GSTNo", obj.GSTNo);
                else
                    com.Parameters.AddWithValue("@GSTNo", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.VendorType))
                    com.Parameters.AddWithValue("@VendorType", obj.VendorType);
                else
                    com.Parameters.AddWithValue("@VendorType", DBNull.Value);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public DataTable getVendorList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Vendor_SelectAll", con);
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
        public DataTable getSectorwiseVendorList(int? SectorId)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Sector_Vendor_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable GetSectorListByVendor(int? VendorId)
        {
            if (VendorId == 0) VendorId = null;            
            //con.Open();
            SqlCommand cmd = new SqlCommand("SP_VendorWiseSector", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable GetVendorListBySector(int? SectorId)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SP_SectorWiseVendor", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable GetVendorListByPraentCat(int? Parentcatid)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT vc.Id,vm.Id as Vid ,CONCAT(vm.FirstName,' ',vm.LastName) AS Vname,vc.VendorCatname FROM tbl_VendorCat vc inner join tbl_Vendor_Master vm on vc.VendorId=vm.Id where (@Parentcatid is Null Or (vc.ParentCatId=@Parentcatid))", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Parentcatid.ToString()))
                cmd.Parameters.AddWithValue("@Parentcatid", Parentcatid);
            else
                cmd.Parameters.AddWithValue("@Parentcatid", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
           // con.Close();
            return dt;
        }

        public DataTable GetVendorListByAttribute(string str)
        {
            //con.Open();

            int id = 0,count=0;
            int ProductID = 0;
            decimal Profit = 0;

            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            string a = "";
            foreach (string s in str.Split(delimiter))
            {
                count = count + 1;
                if (count == 1)
                    id = Convert.ToInt32(s);

                if (count == 2)
                    ProductID = Convert.ToInt32(s);

                if (count == 3)
                    Profit = Convert.ToDecimal(s);



            }

            SqlCommand cmd = new SqlCommand("Select Distinct Vc.Id ,Vc.VendorCatname,Concat(Vm.FirstName,' ',Vm.LastName) As Vendorname from tbl_Product_Attributes Pa Inner join tbl_Vendor_Product_Assign Vpa On Pa.VendorCatId=Vpa.VendorCatId and Pa.AttributeID=Vpa.AttributeId and Pa.VendorId=Vpa.VendorId Inner join tbl_VendorCat Vc On Vc.Id=Pa.VendorCatId Inner Join tbl_Vendor_Master Vm On Vm.Id=Vc.VendorId where (@Profit is Null Or (Pa.Profit=@Profit)) AND (@AttributeID is Null Or (Pa.AttributeID=@AttributeID)) AND (@ProductID is Null Or (Pa.ProductID=@ProductID)) AND Vpa.IsActive=1", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Profit.ToString()))
                cmd.Parameters.AddWithValue("@Profit", Profit);
            else
                cmd.Parameters.AddWithValue("@Profit", 0);

            if (!string.IsNullOrEmpty(id.ToString()))
                cmd.Parameters.AddWithValue("@AttributeID", id);
            else
                cmd.Parameters.AddWithValue("@AttributeID", 0);

            if (!string.IsNullOrEmpty(ProductID.ToString()))
                cmd.Parameters.AddWithValue("@ProductID", ProductID);
            else
                cmd.Parameters.AddWithValue("@ProductID", 0);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            // con.Close();
            return dt;
        }

        public DataTable GetSectorListByCity(int? CityId)
        {
            if (CityId == 0) CityId = null;
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT Id,SectorName FROM tbl_Sector_Master WHERE (@CityId IS NULL OR CityId = @CityId) GROUP BY Id,SectorName ORDER BY SectorName", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(CityId.ToString()))
                cmd.Parameters.AddWithValue("@CityId", CityId);
            else
                cmd.Parameters.AddWithValue("@CityId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        //delete
        public int DeleteVendor(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Vendor_Master where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public DataTable CheckVendorUserName(string username)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Vendor_Master where UserName='" + username + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable CheckDuplicateVendor(string fname, string lname, string mobile)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Vendor_Master where FirstName='" + fname + "' and LastName='" + lname + "' and MobileNo='" + mobile + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable CheckDuplicateSectProd(int? sectorid, int? vendorid, int productid)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Vendor_Product_Assign where SectorId='" + sectorid + "' and VendorId='" + vendorid + "' and ProductId='" + productid + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public int InsertSectorProduct(Vendor obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Vendor_Product_Assign_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
                if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    com.Parameters.AddWithValue("@SectorId", obj.SectorId);
                else
                    com.Parameters.AddWithValue("@SectorId", 0);
                if (!string.IsNullOrEmpty(obj.VendorId.ToString()))
                    com.Parameters.AddWithValue("@VendorId", obj.VendorId);
                else
                    com.Parameters.AddWithValue("@VendorId", 0);
                if (!string.IsNullOrEmpty(obj.CategoryId.ToString()))
                    com.Parameters.AddWithValue("@CategoryId", obj.CategoryId);
                else
                    com.Parameters.AddWithValue("@CategoryId", 0);
                if (!string.IsNullOrEmpty(obj.ProductId.ToString()))
                    com.Parameters.AddWithValue("@ProductId", obj.ProductId);
                else
                    com.Parameters.AddWithValue("@ProductId", 0);
                com.Parameters.AddWithValue("@IsActive", true);
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                try
                {
                    con.Close();
                    int id = Convert.ToInt32(com.Parameters["@Id"].Value);
                    return id;
                }
                catch { }       
            }
            catch (Exception ex)
            { }
            return i;
        }

        public int InsertSectorProduct1(Vendor obj)
        {
            int i = 0;
            try
            {
                con.Open();
                //SqlCommand com = new SqlCommand("Vendor_Product_Assign_Insert", con);
                SqlCommand com = new SqlCommand("Vendor_Product_Assign_Insert1", con);
                com.CommandType = CommandType.StoredProcedure;

                if (!string.IsNullOrEmpty(obj.VendorId.ToString()))
                    com.Parameters.AddWithValue("@VendorId", obj.VendorId);
                else
                    com.Parameters.AddWithValue("@VendorId", 0);
                if (!string.IsNullOrEmpty(obj.CategoryId.ToString()))
                    com.Parameters.AddWithValue("@CategoryId", obj.CategoryId);
                else
                    com.Parameters.AddWithValue("@CategoryId", 0);
                if (!string.IsNullOrEmpty(obj.ProductId.ToString()))
                    com.Parameters.AddWithValue("@ProductId", obj.ProductId);
                else
                    com.Parameters.AddWithValue("@ProductId", 0);
                com.Parameters.AddWithValue("@IsActive", true);

                if (!string.IsNullOrEmpty(obj.VendorCatId.ToString()))
                    com.Parameters.AddWithValue("@VendorCatId", obj.VendorCatId);
                else
                    com.Parameters.AddWithValue("@VendorCatId", 0);

                if (!string.IsNullOrEmpty(obj.AttributeId.ToString()))
                    com.Parameters.AddWithValue("@AttributeId", obj.AttributeId);
                else
                    com.Parameters.AddWithValue("@AttributeId", 0);

                if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    com.Parameters.AddWithValue("@SectorId", obj.SectorId);
                else
                    com.Parameters.AddWithValue("@SectorId", 0);

                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                try
                {
                    con.Close();
                    int id = Convert.ToInt32(com.Parameters["@Id"].Value);
                    return id;
                }
                catch { }
            }
            catch (Exception ex)
            { }
            return i;
        }
        public int DeleteSectorProductUnAssigned(int id)
        {
            try
            {
                con.Open();
                string q = "delete from tbl_Vendor_Product_Assign where Id=" + id + "";
                SqlCommand cd = new SqlCommand(q, con);
                int i = cd.ExecuteNonQuery();
                return i;
            }
            catch(Exception e)
            {
                throw;
            }
            finally
            {
                con.Close();
            }
            
        }

        public DataTable getSectorProductList(int? Id,int? SectorId,int? VendorId,int? CategoryId)
        {
            //con.Open();
            if (SectorId == 0) SectorId = null;
            if (VendorId == 0) VendorId = null;
            if (CategoryId == 0) CategoryId = null;
            SqlCommand cmd = new SqlCommand("Vendor_Product_Assign_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);
            if (!string.IsNullOrEmpty(CategoryId.ToString()))
                cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
            else
                cmd.Parameters.AddWithValue("@CategoryId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getSectorVendorProductList(int? Id, int? SectorId, int? VendorId, int? CategoryId)
        {
            //con.Open();
            if (SectorId == 0) SectorId = null;
            if (VendorId == 0) VendorId = null;
            if (CategoryId == 0) CategoryId = null;
            SqlCommand cmd = new SqlCommand("SP_Sector_Vendor_ProductList", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);
            if (!string.IsNullOrEmpty(CategoryId.ToString()))
                cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
            else
                cmd.Parameters.AddWithValue("@CategoryId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        //delete
        public int DeleteSectorProduct(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Vendor_Product_Assign where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int updateActiveProductStatus(string pid)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("update tbl_Vendor_Product_Assign set IsActive='1' where Id='" + pid + "'  ", con);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }

        public int updateInActiveProductStatus(string pid)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("update tbl_Vendor_Product_Assign set IsActive='0' where Id='" + pid + "'  ", con);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }


        public int InsertVendorSectorAssign(string state,string city,string vendor,string item,string Cat)
        {

            con.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO tbl_Vendor_Sector_Assign(StateId,CityId,VendorId,SectorId,IsActive,Category)VALUES(" + state+","+city+","+vendor+","+item+",'1','"+ Cat+"')", con);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;

            
        }

        public int InsertVendorcat(string vendorId, string VendorCatname, string Cat,string fname, string VendorMinAmount, string VendorMaxAmount, string FromTime, string ToTime, string DeliveryFrom, string Deliveryto,bool notstatus)
        {


            string active = "1";
            con.Open();
          //  SqlCommand cmd = new SqlCommand("INSERT INTO tbl_VendorCat(VendorId,VendorCatname,ParentCatId,IsActive)VALUES(@VendorId,@VendorCatName,@ParentCat,@IsActive)", con);

            SqlCommand cmd = new SqlCommand("InsertVendorCat", con);
            cmd.CommandType = CommandType.StoredProcedure;

            //  @MinAmount,@MaxAmount,@FromTime,@ToTime,@DeliveryFrom,@DeliveryTo
            //string VendorMinAmount,string VendorMaxAmount,string FromTime,string ToTime,string DeliveryFrom,string Deliveryto
            if (vendorId != "")
                cmd.Parameters.AddWithValue("@VendorId", vendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);
            if (VendorCatname != "")
                cmd.Parameters.AddWithValue("@VendorCatname", VendorCatname);
            else
                cmd.Parameters.AddWithValue("@VendorCatname", DBNull.Value);
            if (Cat != "")
                cmd.Parameters.AddWithValue("@ParentCatId", Cat);
            else
                cmd.Parameters.AddWithValue("@ParentCatId", DBNull.Value);
           

            if (active != "")
                cmd.Parameters.AddWithValue("@IsActive", active);
            else
                cmd.Parameters.AddWithValue("@IsActive", 0);

            if (fname != "")
                cmd.Parameters.AddWithValue("@Catimg", fname);
            else
                cmd.Parameters.AddWithValue("@Catimg", DBNull.Value);


             if (VendorMinAmount !="")
                cmd.Parameters.AddWithValue("@MinAmount", VendorMinAmount);
            else
                cmd.Parameters.AddWithValue("@MinAmount", 0);
            if (VendorMaxAmount!="")
                cmd.Parameters.AddWithValue("@MaxAmount", VendorMaxAmount);
            else
                cmd.Parameters.AddWithValue("@MaxAmount", 0);
            if (FromTime != null)
                cmd.Parameters.AddWithValue("@FromTime", FromTime);
            else
                cmd.Parameters.AddWithValue("@FromTime", 0);
            if (ToTime != null)
                cmd.Parameters.AddWithValue("@ToTime", ToTime);
            else
                cmd.Parameters.AddWithValue("@ToTime", 0);
            if (DeliveryFrom != null)
                cmd.Parameters.AddWithValue("@DeliveryFrom", DeliveryFrom);
            else
                cmd.Parameters.AddWithValue("@DeliveryFrom", 0);
            if (Deliveryto != null)
                cmd.Parameters.AddWithValue("@DeliveryTo", Deliveryto);
            else
                cmd.Parameters.AddWithValue("@DeliveryTo", 0);
            if(!string.IsNullOrEmpty(notstatus.ToString()))
            cmd.Parameters.AddWithValue("@IsNotification", notstatus);
            else
                cmd.Parameters.AddWithValue("@IsNotification", 0);

            cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
            int result = cmd.ExecuteNonQuery();
            try
            {
                con.Close();
                int id = Convert.ToInt32(cmd.Parameters["@Id"].Value);
                return id;
            }
            catch { }



            
            con.Close();
            return result;
        }

        public int InsertVendorSubcatAssign(string vendorId, string VendorCatname, string Cat, string item,string vendorcat)
        {
            string active = "1";
            con.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO tbl_Vendor_CatSubcat_Assign(VendorId,VendorCatName,ParentCat,Subcat,IsActive,VendorCatId)VALUES(@VendorId,@VendorCatName,@ParentCat,@Subcat,@IsActive,@VendorCatId)", con);

         
            if (vendorId != "")
                cmd.Parameters.AddWithValue("@VendorId", vendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);
            if (VendorCatname != "")
                cmd.Parameters.AddWithValue("@VendorCatname", VendorCatname);
            else
                cmd.Parameters.AddWithValue("@VendorCatname", DBNull.Value);
            if (Cat != "")
                cmd.Parameters.AddWithValue("@ParentCat", Cat);
            else
                cmd.Parameters.AddWithValue("@ParentCat", DBNull.Value);
            if (item != "")
                cmd.Parameters.AddWithValue("@Subcat", item);
            else
                cmd.Parameters.AddWithValue("@Subcat", DBNull.Value);

            if (active != "")
                cmd.Parameters.AddWithValue("@IsActive", active);
            else
                cmd.Parameters.AddWithValue("@IsActive", 0);

           
            if (vendorcat != null)
                cmd.Parameters.AddWithValue("@VendorCatId", vendorcat);
            else
                cmd.Parameters.AddWithValue("@VendorCatId", 0);




            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;


        }


        public int UpdateVendorSubcatAssign(string vendorId, string vendorcat, string item)
        {




            string active = "1";
            con.Open();

            


            SqlCommand cmd = new SqlCommand("UPDATE tbl_Vendor_CatSubcat_Assign set IsActive=@IsActive where VendorId=@VendorId and VendorCatId=@VendorCatId and Subcat=@Subcat", con);


            if (vendorId != "")
                cmd.Parameters.AddWithValue("@VendorId", vendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);


            if (vendorcat != null)
                cmd.Parameters.AddWithValue("@VendorCatId", vendorcat);
            else
                cmd.Parameters.AddWithValue("@VendorCatId", 0);

            if (item != "")
                cmd.Parameters.AddWithValue("@Subcat", item);
            else
                cmd.Parameters.AddWithValue("@Subcat", DBNull.Value);

            if (active != "")
                cmd.Parameters.AddWithValue("@IsActive", active);
            else
                cmd.Parameters.AddWithValue("@IsActive", 0);


            




            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;


        }


        public int Vendorcatupdate(string vendorId, string VendorCatname, string vendorcat, string fname, string VendorMinAmount, string VendorMaxAmount, string FromTime, string ToTime, string DeliveryFrom, string DeliveryTo)
        {




            string active = "1";
            con.Open();




            SqlCommand cmd = new SqlCommand("UPDATE tbl_VendorCat set VendorCatname=@VendorCatname,MinAmount=@MinAmount,MaxAmount=@MaxAmount,FromTime=@FromTime,ToTime=@ToTime,DeliveryFrom=@DeliveryFrom,DeliveryTo=@DeliveryTo,IsActive=@IsActive where VendorId=@VendorId and Id=@VendorCatId", con);


            if (vendorId != "")
                cmd.Parameters.AddWithValue("@VendorId", vendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);


            if (vendorcat != null)
                cmd.Parameters.AddWithValue("@VendorCatId", vendorcat);
            else
                cmd.Parameters.AddWithValue("@VendorCatId", 0);

           

            if (active != "")
                cmd.Parameters.AddWithValue("@IsActive", active);
            else
                cmd.Parameters.AddWithValue("@IsActive", 0);


            if (VendorCatname != "")
                cmd.Parameters.AddWithValue("@VendorCatname", VendorCatname);
            else
                cmd.Parameters.AddWithValue("@VendorCatname", DBNull.Value);

            if (fname != "")
                cmd.Parameters.AddWithValue("@Catimg", fname);
            else
                cmd.Parameters.AddWithValue("@Catimg", DBNull.Value);


            if (VendorMinAmount != "")
                cmd.Parameters.AddWithValue("@MinAmount", VendorMinAmount);
            else
                cmd.Parameters.AddWithValue("@MinAmount", 0);
            if (VendorMaxAmount != "")
                cmd.Parameters.AddWithValue("@MaxAmount", VendorMaxAmount);
            else
                cmd.Parameters.AddWithValue("@MaxAmount", 0);
            if (FromTime != null)
                cmd.Parameters.AddWithValue("@FromTime", FromTime);
            else
                cmd.Parameters.AddWithValue("@FromTime", 0);
            if (ToTime != null)
                cmd.Parameters.AddWithValue("@ToTime", ToTime);
            else
                cmd.Parameters.AddWithValue("@ToTime", 0);
            if (DeliveryFrom != null)
                cmd.Parameters.AddWithValue("@DeliveryFrom", DeliveryFrom);
            else
                cmd.Parameters.AddWithValue("@DeliveryFrom", 0);
            if (DeliveryTo != null)
                cmd.Parameters.AddWithValue("@DeliveryTo", DeliveryTo);
            else
                cmd.Parameters.AddWithValue("@DeliveryTo", 0);




            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;


        }

        public int UpdateVendorSubcatAssigncommon(string vendorId, string vendorcat)
        {




            string active = "0";
            con.Open();




            SqlCommand cmd = new SqlCommand("UPDATE tbl_Vendor_CatSubcat_Assign set IsActive=@IsActive where VendorId=@VendorId and VendorCatId=@VendorCatId", con);


            if (vendorId != "")
                cmd.Parameters.AddWithValue("@VendorId", vendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);


            if (vendorcat != null)
                cmd.Parameters.AddWithValue("@VendorCatId", vendorcat);
            else
                cmd.Parameters.AddWithValue("@VendorCatId", 0);

            

            if (active != "")
                cmd.Parameters.AddWithValue("@IsActive", active);
            else
                cmd.Parameters.AddWithValue("@IsActive", 0);







            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;


        }

        public DataTable ChkDuplSector(string vendor,string sector)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Vendor_Sector_Assign where VendorId=" + vendor + " and SectorId="+sector+"", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable ChkDuplSubcat(string vendor,string Cat,string subcat)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Vendor_CatSubcat_Assign where VendorId=" + vendor + " and ParentCat="+Cat+" and Subcat=" + subcat + "", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable getVendorAssignList(int? Id)
        {
            //con.Open();
            //string query = "Select Vm.FirstName,Vm.LastName,Sm.SectorName,Sm1.statename,Cm.Cityname,Va.VendorId,Va.SectorId,Va.Id  From tbl_Vendor_Master Vm";
            //query += " Inner Join tbl_Vendor_Sector_Assign Va on Va.VendorId=Vm.Id";
            //query += " Inner Join tbl_Sector_Master Sm On Sm.Id=Va.SectorId";
            //query += " Inner join tblstatemaster Sm1 on Va.StateId=Sm1.id";
            //query += " Inner Join tblcitymaster Cm On Va.CityId=Cm.id";

            string query = "Select Distinct Va.VendorId,Vm.FirstName,Vm.LastName  From tbl_Vendor_Master Vm ";
            query += " Inner Join tbl_Vendor_Sector_Assign Va on Va.VendorId=Vm.Id";
            query += " Inner Join tbl_Sector_Master Sm On Sm.Id=Va.SectorId";
            query += "  Inner join tblstatemaster Sm1 on Va.StateId=Sm1.id";
            query += " Inner Join tblcitymaster Cm On Va.CityId=Cm.id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(Id.ToString()))
            //    cmd.Parameters.AddWithValue("@Id", Id);
            //else
            //    cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getVendorAssignListnew(int? Id)
        {
            //con.Open();
            string query = "Select Vm.FirstName,Vm.LastName,Sm.SectorName,Sm1.statename,Cm.Cityname,Va.VendorId,Va.SectorId,Va.Id  From tbl_Vendor_Master Vm";
            query += " Inner Join tbl_Vendor_Sector_Assign Va on Va.VendorId=Vm.Id";
            query += " Inner Join tbl_Sector_Master Sm On Sm.Id=Va.SectorId";
            query += " Inner join tblstatemaster Sm1 on Va.StateId=Sm1.id";
            query += " Inner Join tblcitymaster Cm On Va.CityId=Cm.id";
            query += " where (@VendorId Is Null Or Va.VendorId=@VendorId)";


            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", Id);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable getVendorCatSubcatAssignList(int? Id)
        {
            //con.Open();
            string query = "Select DISTINCT Vm.FirstName,Vm.LastName,Va.VendorId,Vc.VendorCatname,Va.Id,Pm.ParentCategory,Ps.SubCatName,";
            query += " Sm1.statename,Cm.Cityname,Va.IsActive,Va.ParentCat,Va.VendorCatName,Va.MinAmount,Va.MaxAmount,Va.FromTime,Va.ToTime,Va.DeliveryFrom,";
            query += " Va.DeliveryTo,Va.Vendorcatimg,va.VendorCatId as editid,Vc.IsNotification  From tbl_Vendor_Master Vm";
            query += " inner join tbl_VendorCat Vc on Vc.VendorId=Vm.Id";
            query += " inner Join tbl_Parent_Category_Master Pm on Pm.Id=Vc.ParentCatId";
            query += " inner Join tbl_Vendor_CatSubcat_Assign Va on Va.ParentCat=Pm.Id";
            query += " inner join tbl_Product_Subcat_Master Ps on Ps.Id=Va.Subcat";
            query += " inner Join tbl_Vendor_Sector_Assign Va1 on Va1.VendorId=Vm.Id";
            query += " inner Join tbl_Sector_Master Sm On Sm.Id=Va1.SectorId";
            query += " inner join tblstatemaster Sm1 on Sm1.id=Va1.StateId";
            query += " inner Join tblcitymaster Cm On Va1.CityId=Cm.id";
            query += " where @Id is Null or Va.Id=@Id";

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


        public DataTable getVendorCatSubcatAssignListsingle(int? Id)
        {
            //con.Open();
            string query = "Select DISTINCT Vm.FirstName,Vm.LastName,Va.VendorId,Va.VendorCatName,Va.Id,Pm.ParentCategory,Sm1.statename,Cm.Cityname,Va.IsActive,Va.ParentCatId,Va.VendorCatname,Va.MinAmount,Va.MaxAmount,Va.FromTime,Va.ToTime,Va.DeliveryFrom,Va.DeliveryTo,Va.Catimg,Va.Id As editId,Va.IsNotification  From tbl_Vendor_Master Vm";
            query += " Inner Join tbl_VendorCat Va on Va.VendorId=Vm.Id";
            query += " Inner Join tbl_Parent_Category_Master Pm on Pm.Id=Va.ParentCatId";
           
            query += " Inner Join tbl_Vendor_Sector_Assign Va1 on Va1.VendorId=Vm.Id";
            query += " Inner Join tbl_Sector_Master Sm On Sm.Id=Va1.SectorId";
            query += " Inner join tblstatemaster Sm1 on Va1.StateId=Sm1.id";
            query += " Inner Join tblcitymaster Cm On Va1.CityId=Cm.id";
            query += " where @Id is Null or Va.Id=@Id";

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
        public DataTable getVendorAssignedSector(int? Id)
        {
            //con.Open();
            string query = "Select Sm.Id,Sm.SectorName,Vc.VendorId from tbl_VendorCat Vc Inner join tbl_Vendor_Sector_Assign Vs On Vc.VendorId=Vs.VendorId";
            query += " Inner join tbl_Sector_Master Sm On Vs.SectorId=Sm.Id where @Id IS NULL OR  Vc.Id=@Id";
            
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
        public int DeleteSectorVendorAssign(int id)
        {
            try
            {
                con.Open();
                string q = "delete from tbl_Vendor_Sector_Assign where Id=" + id + "";
                SqlCommand cd = new SqlCommand(q, con);
                int i = cd.ExecuteNonQuery();
                return i;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                con.Close();
            }

        }


        public int DeleteCatSubcatVendorAssign(int id)
        {
            try
            {
                con.Open();
                string q = "delete from tbl_Vendor_CatSubcat_Assign where Id=" + id + "";
                SqlCommand cd = new SqlCommand(q, con);
                int i = cd.ExecuteNonQuery();
                return i;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                con.Close();
            }

        }


        public DataTable ProductInTransaction(int pid,int sid)
        {
            DataTable dt = new DataTable();
            try
            {
                DateTime ToDate = Helper.indianTime;
                // ToDate = ToDate.AddDays(-50);
                con.Open();
                string q = "select o.ProductId,t.Id,convert(varchar,t.OrderDate,23) as OrderDate,t.CustomerId,cm.SectorId from tbl_Customer_Order_Transaction as t left join tbl_Customer_Order_Detail as o on o.OrderId=t.Id left join tbl_Customer_Master cm on t.CustomerId=cm.Id where o.ProductId=" + pid + " and t.Status='Pending' and CONVERT(VARCHAR,t.OrderDate,23)>=@ToDate and cm.SectorId="+sid+"";
                SqlCommand cd = new SqlCommand(q, con);
                cd.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(ToDate.ToString()))
                    cd.Parameters.AddWithValue("@ToDate", ToDate);
                else
                    cd.Parameters.AddWithValue("@ToDate", DBNull.Value);
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


        public DataTable getVendorCatid(int? VendorId,int? CategoryId)
        {
            //con.Open();
            string query = "Select Id from tbl_VendorCat where VendorId="+VendorId+ " AND ParentCatId="+CategoryId+"";
            
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(Id.ToString()))
            //    cmd.Parameters.AddWithValue("@Id", Id);
            //else
            //    cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public DataTable getVendorCat(int? VendorId)
        {
            //con.Open();
            string query = "Select Id,VendorCatname,VendorId from tbl_VendorCat where (@VendorId Is Null Or VendorId=@VendorId)";

            SqlCommand cmd = new SqlCommand(query, con);
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
        public DataTable getVendorid(int? VendorCatId)
        {
            //con.Open();
            string query = "Select VendorId,MinAmount,MaxAmount,FromTime,ToTime,DeliveryFrom,DeliveryTo from tbl_VendorCat where Id=" + VendorCatId + "";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(Id.ToString()))
            //    cmd.Parameters.AddWithValue("@Id", Id);
            //else
            //    cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getVendorcatsector(int? VendorCatId,int? SectorId)
        {
            //con.Open();
            string query = "Select VendorId from tbl_Vendorcat_SectorAssign where VendorCatId=" + VendorCatId + " AND SectorId="+SectorId+"";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(Id.ToString()))
            //    cmd.Parameters.AddWithValue("@Id", Id);
            //else
            //    cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable getVendorcatProduct(int? VendorCatId)
        {
            //con.Open();
            string query = "SELECT Distinct Pa.ProductID,Pm.ProductName,Pa.VendorCatId FROM [dbo].tbl_Product_Master Pm Inner Join tbl_Product_Attributes Pa On Pa.ProductID=Pm.Id where (@VendorCatId Is Null Or Pa.VendorCatId=@VendorCatId)";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(VendorCatId.ToString()))
                cmd.Parameters.AddWithValue("@VendorCatId", VendorCatId);
            else
                cmd.Parameters.AddWithValue("@VendorCatId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getVendorcatProductwiseAttribute(int? VendorCatId,int? prodid)
        {
            //con.Open();
            string query = "SELECT Distinct Pa.ProductID,At.ID,At.Name FROM tbl_Attributes At Inner Join tbl_Product_Attributes Pa On Pa.AttributeID=At.Id where Pa.VendorCatId=" + VendorCatId + " AND Pa.ProductID="+prodid+"";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(Id.ToString()))
            //    cmd.Parameters.AddWithValue("@Id", Id);
            //else
            //    cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable GetSectorAssignedMsg(string s)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select Concat(Vm.FirstName,' ',Vm.LastName) As Name,Vc.VendorCatname,Sm.SectorName,Sm.Id from tbl_VendorCat Vc Inner join tbl_Vendorcat_SectorAssign Vs on Vs.VendorCatId=Vc.Id  Inner join tbl_Sector_Master Sm On Vs.SectorId=Sm.Id Inner join tbl_Vendor_Master Vm On Vc.VendorId=Vm.Id  where  Vc.Id=@Id ", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(s.ToString()))
                cmd.Parameters.AddWithValue("@Id", s);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable GetSectorAssignedVendor(string s,int? atid,int? pid)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select Vc.VendorCatname,Sm.SectorName,Sm.Id,Sm1.statename,Cm.Cityname,Concat(Vm.FirstName,' ',Vm.LastName) As VendorName from tbl_VendorCat Vc inner join tbl_Vendor_Master Vm On Vm.Id=Vc.VendorId inner join tbl_Vendor_Product_Assign Vpa On vpa.VendorCatId=Vc.Id and Vpa.ProductId=@Pid and Vpa.AttributeId=@Aid and vpa.VendorCatId=@Id and vpa.IsActive='True' Inner join tbl_Sector_Master Sm On Vpa.SectorId=Sm.Id inner join tblcitymaster Cm on Cm.id=Sm.CityId inner join tblstatemaster Sm1 On Sm1.id=cm.StateName ", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(s.ToString()))
                cmd.Parameters.AddWithValue("@Id", s);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);


            if (!string.IsNullOrEmpty(pid.ToString()))
                cmd.Parameters.AddWithValue("@Pid", pid);
            else
                cmd.Parameters.AddWithValue("@Pid", 0);

            if (!string.IsNullOrEmpty(atid.ToString()))
                cmd.Parameters.AddWithValue("@Aid", atid);
            else
                cmd.Parameters.AddWithValue("@Aid", 0);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
        public int InsertVendorcatSector(int? vid,int? vendorcat,int? sid,string Cat)
        {
            string active = "1";
            con.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO tbl_Vendorcat_SectorAssign(VendorId,VendorCatId,SectorId,IsActive,Category)VALUES(@VendorId,@VendorCatId,@SectorId,@IsActive,@Category)", con);

            //  @MinAmount,@MaxAmount,@FromTime,@ToTime,@DeliveryFrom,@DeliveryTo
            //string VendorMinAmount,string VendorMaxAmount,string FromTime,string ToTime,string DeliveryFrom,string Deliveryto
            if (!string.IsNullOrEmpty(vid.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", vid);
            else
                cmd.Parameters.AddWithValue("@VendorId", 0);
            if (!string.IsNullOrEmpty(vendorcat.ToString()))
                cmd.Parameters.AddWithValue("@VendorCatId", vendorcat);
            else
                cmd.Parameters.AddWithValue("@VendorCatId", 0);
            if (!string.IsNullOrEmpty(sid.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", sid);
            else
                cmd.Parameters.AddWithValue("@SectorId", 0);


            if (active != "")
                cmd.Parameters.AddWithValue("@IsActive", active);
            else
                cmd.Parameters.AddWithValue("@IsActive", 0);


            if (!string.IsNullOrEmpty(Cat))
                cmd.Parameters.AddWithValue("@Category", Cat);
            else
                cmd.Parameters.AddWithValue("@Category", DBNull.Value);



            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;


        }


        public int updateVendorwiseProduct(Vendor objVendor)
        {
           
            con.Open();
            SqlCommand cmd = new SqlCommand("update tbl_Vendor_Product_Assign set AttributeId=@AttributeId,IsActive=@IsActive where ProductId=@ProductId and AttributeId=@AttributeId1 and VendorCatId=@VendorCatId and SectorId=@SectorId  ", con);

            cmd.Parameters.AddWithValue("@AttributeId", objVendor.AttributeId1);
            cmd.Parameters.AddWithValue("@AttributeId1", objVendor.AttributeId);
            cmd.Parameters.AddWithValue("@ProductId", objVendor.ProductId);
            cmd.Parameters.AddWithValue("@VendorCatId", objVendor.VendorCatId);

            cmd.Parameters.AddWithValue("@SectorId", objVendor.SectorId);
            cmd.Parameters.AddWithValue("@IsActive", objVendor.IsActive);
         
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }




        public int updateVendorwiseProductAttribute(Vendor objVendor)
        {

            con.Open();
            SqlCommand cmd = new SqlCommand("Product_Attribute_Update", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@AttributeId1", objVendor.AttributeId1);
            cmd.Parameters.AddWithValue("@AttributeId", objVendor.AttributeId);
            cmd.Parameters.AddWithValue("@ProductId", objVendor.ProductId);
            cmd.Parameters.AddWithValue("@VendorCatId", objVendor.VendorCatId);

            cmd.Parameters.AddWithValue("@MRPPrice", objVendor.MRPPrice);
            cmd.Parameters.AddWithValue("@PurchasePrice", objVendor.PurchasePrice);
            cmd.Parameters.AddWithValue("@DiscountPrice", objVendor.DiscountPrice);
            cmd.Parameters.AddWithValue("@CGST", objVendor.CGST);
            cmd.Parameters.AddWithValue("@SGST", objVendor.SGST);
            cmd.Parameters.AddWithValue("@IGST", objVendor.IGST);

            cmd.Parameters.AddWithValue("@Profit", objVendor.Profit);
            cmd.Parameters.AddWithValue("@SellPrice", objVendor.SellPrice);
            cmd.Parameters.AddWithValue("@IsActive", objVendor.IsActive);

            cmd.Parameters.AddWithValue("@B2BProfit", objVendor.B2BProfit);
            cmd.Parameters.AddWithValue("@B2BSellPrice", objVendor.B2BSellPrice);

            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }


        public int InsertVendorRefNo(Vendor obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Vendor_RefInsert", con);
                com.CommandType = CommandType.StoredProcedure;
                if (!string.IsNullOrEmpty(obj.VendorId.ToString()))
                    com.Parameters.AddWithValue("@VendorId", obj.VendorId);
                else
                    com.Parameters.AddWithValue("@VendorId", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.PayRefference))
                    com.Parameters.AddWithValue("@RefferenceNo", obj.PayRefference);
                else
                    com.Parameters.AddWithValue("@RefferenceNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Payamount.ToString()))
                    com.Parameters.AddWithValue("@Amount", obj.Payamount);
                else
                    com.Parameters.AddWithValue("@Amount", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.PayRefdate.ToString()))
                    com.Parameters.AddWithValue("@UpdatedOn", obj.PayRefdate);
                else
                    com.Parameters.AddWithValue("@UpdatedOn", DBNull.Value);
                

                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public DataTable getVendorPaymentList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Select Vp.Id,Vp.RefferenceNo,Vp.Amount,CONVERT(varchar,Vp.UpdatedOn,23) AS RefDate,Vp.VendorId,Concat(Vm.FirstName,' ',Vm.LastName) As Name from tbl_Vendor_Payment Vp Inner join tbl_Vendor_Master Vm On Vm.Id=Vp.VendorId where @Id is Null or Vp.Id=@Id", con);
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

        public DataTable getVendorPaymentListdatewise(int? Id, DateTime? Fromdate, DateTime? Todate)
        {
            //con.Open();
            ToDate = Todate;
            FromDate = Fromdate;
            SqlCommand cmd = new SqlCommand("Select Sum(Amount) As Amount from tbl_Vendor_Payment Vp where (@FromDate IS NULL OR @ToDate IS NULL OR CONVERT(VARCHAR,Vp.UpdatedOn,23) BETWEEN @FromDate AND @Todate) And( @Id is Null or Vp.VendorId=@Id)", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);

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

        public int UpdateVendorPayment(Vendor obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Vendor_Payment_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.Id);
                if (!string.IsNullOrEmpty(obj.VendorId.ToString()))
                    com.Parameters.AddWithValue("@VendorId", obj.VendorId);
                else
                    com.Parameters.AddWithValue("@VendorId", 0);
                if (!string.IsNullOrEmpty(obj.PayRefference))
                    com.Parameters.AddWithValue("@RefferenceNo", obj.PayRefference);
                else
                    com.Parameters.AddWithValue("@RefferenceNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Payamount.ToString()))
                    com.Parameters.AddWithValue("@Amount", obj.Payamount);
                else
                    com.Parameters.AddWithValue("@Amount", 0);
                if (!string.IsNullOrEmpty(obj.PayRefdate.ToString()))
                    com.Parameters.AddWithValue("@UpdatedOn", obj.PayRefdate);
                else
                    com.Parameters.AddWithValue("@UpdatedOn", DBNull.Value);
                
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public int DeleteVendorPayment(int id)
        {
            try
            {
                con.Open();
                string q = "delete from tbl_Vendor_Payment where Id=" + id + "";
                SqlCommand cd = new SqlCommand(q, con);
                int i = cd.ExecuteNonQuery();
                return i;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                con.Close();
            }

        }


        public DataTable getproductList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT Distinct Vpa.VendorId,Pm.ProductName,Pm.Id As Proid from tbl_Vendor_Product_Assign Vpa inner join tbl_Product_Master Pm On Vpa.ProductId=Pm.Id WHERE (@Id IS NULL OR Vpa.VendorId = @Id)", con);
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


        public DataTable getproductPrice(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT [Id],[Price],[PurchasePrice],[SalePrice],[B2BProfit],[B2BSalePrice] from tbl_Product_Master WHERE (@Id IS NULL OR Id = @Id)", con);
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


        public int InsertReplacement(Vendor obj)
        {
            int i = 0;
            try
            {
                con.Open();

                obj.MRPPrice = obj.MRPPrice * Convert.ToDecimal(obj.Qty);
                obj.PurchasePrice = obj.PurchasePrice * Convert.ToDecimal(obj.Qty);
                obj.SellPrice = obj.SellPrice * Convert.ToDecimal(obj.Qty);

                SqlCommand com = new SqlCommand("Vendor_ReplacementInsert", con);
                com.CommandType = CommandType.StoredProcedure;
                if (!string.IsNullOrEmpty(obj.VendorId.ToString()))
                    com.Parameters.AddWithValue("@VendorId", obj.VendorId);
                else
                    com.Parameters.AddWithValue("@VendorId", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.ProductId.ToString()))
                    com.Parameters.AddWithValue("@ProductId", obj.ProductId);
                else
                    com.Parameters.AddWithValue("@ProductId", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.MRPPrice.ToString()))
                    com.Parameters.AddWithValue("@Mrp", obj.MRPPrice);
                else
                    com.Parameters.AddWithValue("@Mrp", 0);

                if (!string.IsNullOrEmpty(obj.PurchasePrice.ToString()))
                    com.Parameters.AddWithValue("@PurchasePrice", obj.PurchasePrice);
                else
                    com.Parameters.AddWithValue("@PurchasePrice", 0);

                if (!string.IsNullOrEmpty(obj.SellPrice.ToString()))
                    com.Parameters.AddWithValue("@SalePrice", obj.SellPrice);
                else
                    com.Parameters.AddWithValue("@SalePrice", 0);


                if (!string.IsNullOrEmpty(obj.Qty.ToString()))
                    com.Parameters.AddWithValue("@Qty", obj.Qty);
                else
                    com.Parameters.AddWithValue("@Qty", 0);


                if (!string.IsNullOrEmpty(obj.PayRefdate.ToString()))
                    com.Parameters.AddWithValue("@UpdatedOn", obj.PayRefdate);
                else
                    com.Parameters.AddWithValue("@UpdatedOn", DBNull.Value);


                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public DataTable getVendorProductReplacementList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Select Vp.Id,Vp.Mrp,Vp.PurchasePrice,Vp.SalePrice,Vp.Qty,CONVERT(varchar,Vp.UpdatedOn,23) AS RefDate,Vp.VendorId,Concat(Vm.FirstName,' ',Vm.LastName) As Name,Pm.ProductName,Pm.Id AS Pid from tbl_VendorProductReplacement Vp Inner join tbl_Vendor_Master Vm On Vm.Id=Vp.VendorId inner join tbl_Product_Master Pm On Pm.Id=Vp.ProductId where @Id is Null or Vp.Id=@Id", con);
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


        public int DeleteVendorProductReplacement(int id)
        {
            try
            {
                con.Open();
                string q = "delete from tbl_VendorProductReplacement where Id=" + id + "";
                SqlCommand cd = new SqlCommand(q, con);
                int i = cd.ExecuteNonQuery();
                return i;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                con.Close();
            }

        }



        public int UpdateReplacement(Vendor obj)
        {
            int i = 0;
            try
            {
                con.Open();

                obj.MRPPrice = obj.MRPPrice * Convert.ToDecimal(obj.Qty);
                obj.PurchasePrice = obj.PurchasePrice * Convert.ToDecimal(obj.Qty);
                obj.SellPrice = obj.SellPrice * Convert.ToDecimal(obj.Qty);

                SqlCommand com = new SqlCommand("Vendor_Replacement_Update", con);
                com.CommandType = CommandType.StoredProcedure;

                if (!string.IsNullOrEmpty(obj.Id.ToString()))
                    com.Parameters.AddWithValue("@Id", obj.Id);
                else
                    com.Parameters.AddWithValue("@Id", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.VendorId.ToString()))
                    com.Parameters.AddWithValue("@VendorId", obj.VendorId);
                else
                    com.Parameters.AddWithValue("@VendorId", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.ProductId.ToString()))
                    com.Parameters.AddWithValue("@ProductId", obj.ProductId);
                else
                    com.Parameters.AddWithValue("@ProductId", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.MRPPrice.ToString()))
                    com.Parameters.AddWithValue("@Mrp", obj.MRPPrice);
                else
                    com.Parameters.AddWithValue("@Mrp", 0);

                if (!string.IsNullOrEmpty(obj.PurchasePrice.ToString()))
                    com.Parameters.AddWithValue("@PurchasePrice", obj.PurchasePrice);
                else
                    com.Parameters.AddWithValue("@PurchasePrice", 0);

                if (!string.IsNullOrEmpty(obj.SellPrice.ToString()))
                    com.Parameters.AddWithValue("@SalePrice", obj.SellPrice);
                else
                    com.Parameters.AddWithValue("@SalePrice", 0);


                if (!string.IsNullOrEmpty(obj.Qty.ToString()))
                    com.Parameters.AddWithValue("@Qty", obj.Qty);
                else
                    com.Parameters.AddWithValue("@Qty", 0);


                if (!string.IsNullOrEmpty(obj.PayRefdate.ToString()))
                    com.Parameters.AddWithValue("@UpdatedOn", obj.PayRefdate);
                else
                    com.Parameters.AddWithValue("@UpdatedOn", DBNull.Value);


                
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }



        public DataTable ProductInTransactiondatewise(Vendor obj,int pid)
        {
            DataTable dt = new DataTable();
            try
            {
                DateTime ToDate = Convert.ToDateTime(obj.updatedon);
                // ToDate = ToDate.AddDays(-80);
                con.Open();
                string q = "select o.ProductId,t.Id,convert(varchar,t.OrderDate,23) as OrderDate,o.Qty,t.Status,t.CustomerId from tbl_Customer_Order_Transaction as t left join tbl_Customer_Order_Detail as o on o.OrderId=t.Id where o.ProductId=" + pid + " and o.VendorCatId="+obj.VendorCatId+ " and o.AttributeId="+obj.AttributeId+ " and o.SectorId="+obj.SectorId+"  and CONVERT(VARCHAR,t.OrderDate,23)>=@ToDate";
                SqlCommand cd = new SqlCommand(q, con);
                cd.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(ToDate.ToString()))
                    cd.Parameters.AddWithValue("@ToDate", ToDate);
                else
                    cd.Parameters.AddWithValue("@ToDate", DBNull.Value);
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

        public int UpdatePriceFutureOrder(int orderid,double orderamount, double pamount, double mamount, double SalePrice, double profit, double cgst, double sgst, double igst, double disamount,int? Attributeid)
        {
            int i = 0;
            try
            {
                con.Open();
                //SqlCommand com = new SqlCommand("ProductOrder_Update1", con);
                SqlCommand com = new SqlCommand("ProductOrder_Update1new", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", orderid);

                if (!string.IsNullOrEmpty(orderamount.ToString()))
                    com.Parameters.AddWithValue("@TotalAmount", orderamount);
                else
                    com.Parameters.AddWithValue("@TotalAmount", 0);



                i = com.ExecuteNonQuery();
                //  int Id = Convert.ToInt32(com.Parameters["@Id"].Value);

                SqlCommand com1 = new SqlCommand("ProductOrder_Update2new", con);
                com1.CommandType = CommandType.StoredProcedure;
                com1.Parameters.AddWithValue("@Id", orderid);

                if (!string.IsNullOrEmpty(SalePrice.ToString()))
                    com1.Parameters.AddWithValue("@SalePrice", SalePrice);
                else
                    com1.Parameters.AddWithValue("@SalePrice", 0);

                if (!string.IsNullOrEmpty(orderamount.ToString()))
                    com1.Parameters.AddWithValue("@Amount", orderamount);
                else
                    com1.Parameters.AddWithValue("@Amount", 0);
                if (!string.IsNullOrEmpty(orderamount.ToString()))
                    com1.Parameters.AddWithValue("@TotalFinalAmount", orderamount);
                else
                    com1.Parameters.AddWithValue("@TotalFinalAmount", 0);

                if (!string.IsNullOrEmpty(pamount.ToString()))
                    com1.Parameters.AddWithValue("@PurchasePrice", pamount);
                else
                    com1.Parameters.AddWithValue("@PurchasePrice", 0);

                if (!string.IsNullOrEmpty(mamount.ToString()))
                    com1.Parameters.AddWithValue("@Mrp", mamount);
                else
                    com1.Parameters.AddWithValue("@Mrp", 0);

                if (!string.IsNullOrEmpty(disamount.ToString()))
                    com1.Parameters.AddWithValue("@Discount", disamount);
                else
                    com1.Parameters.AddWithValue("@Discount", 0);

                if (!string.IsNullOrEmpty(cgst.ToString()))
                    com1.Parameters.AddWithValue("@CGSTAmount", cgst);
                else
                    com1.Parameters.AddWithValue("@CGSTAmount", 0);

                if (!string.IsNullOrEmpty(sgst.ToString()))
                    com1.Parameters.AddWithValue("@SGSTAmount", sgst);
                else
                    com1.Parameters.AddWithValue("@SGSTAmount", 0);

                if (!string.IsNullOrEmpty(igst.ToString()))
                    com1.Parameters.AddWithValue("@IGSTAmount", igst);
                else
                    com1.Parameters.AddWithValue("@IGSTAmount", 0);

                if (!string.IsNullOrEmpty(profit.ToString()))
                    com1.Parameters.AddWithValue("@Profit", profit);
                else
                    com1.Parameters.AddWithValue("@Profit", 0);

                if (!string.IsNullOrEmpty(Attributeid.ToString()))
                    com1.Parameters.AddWithValue("@AttributeId", Attributeid);
                else
                    com1.Parameters.AddWithValue("@AttributeId", 0);


                i = com1.ExecuteNonQuery();

                con.Close();


            }
            catch (Exception ex)
            { }
            return i;

        }





        public DataTable GetAttributeList(string pid, string vid)
        {
            DataTable dt = new DataTable();
            try
            {
               
                con.Open();
                string q = "SELECT At.ID AS Atid,At.Name  FROM [dbo].[tbl_Attributes] At ";
                q += " inner join tbl_Vendor_Product_Assign Vpa On At.ID=Vpa.AttributeId ";
                q += " where (@Pid Is Null Or Vpa.ProductId=@Pid) and (@Vid is null Or Vpa.VendorCatId=@Vid) ";
                SqlCommand cd = new SqlCommand(q, con);
                cd.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(pid.ToString()))
                    cd.Parameters.AddWithValue("@Pid", pid);
                else
                    cd.Parameters.AddWithValue("@Pid", DBNull.Value);

                if (!string.IsNullOrEmpty(vid.ToString()))
                    cd.Parameters.AddWithValue("@Vid", vid);
                else
                    cd.Parameters.AddWithValue("@Vid", DBNull.Value);
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


        public int InsertVendorwiseProductAttribute(Vendor obj)
        {
            int i = 0;
            decimal rewardpoint = 0;
            //try
            //{
                con.Open();

               
                SqlCommand com = new SqlCommand("SaveProductAttributePriceList", con);
                com.CommandType = CommandType.StoredProcedure;

               

                if (!string.IsNullOrEmpty(obj.ProductId.ToString()))
                    com.Parameters.AddWithValue("@ProductId", obj.ProductId);
                else
                    com.Parameters.AddWithValue("@ProductId", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.AttributeId.ToString()))
                    com.Parameters.AddWithValue("@AttributeId", obj.AttributeId);
                else
                    com.Parameters.AddWithValue("@AttributeId", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.MRPPrice.ToString()))
                    com.Parameters.AddWithValue("@MRPPrice", obj.MRPPrice);
                else
                    com.Parameters.AddWithValue("@MRPPrice", 0);

                if (!string.IsNullOrEmpty(obj.PurchasePrice.ToString()))
                    com.Parameters.AddWithValue("@PurchasePrice", obj.PurchasePrice);
                else
                    com.Parameters.AddWithValue("@PurchasePrice", 0);

                if (!string.IsNullOrEmpty(obj.DiscountPrice.ToString()))
                    com.Parameters.AddWithValue("@DiscountPrice", obj.DiscountPrice);
                else
                    com.Parameters.AddWithValue("@DiscountPrice", 0);

                if (!string.IsNullOrEmpty(obj.CGST.ToString()))
                    com.Parameters.AddWithValue("@CGST", obj.CGST);
                else
                    com.Parameters.AddWithValue("@CGST", 0);
                if (!string.IsNullOrEmpty(obj.SGST.ToString()))
                    com.Parameters.AddWithValue("@SGST", obj.SGST);
                else
                    com.Parameters.AddWithValue("@SGST", 0);

                if (!string.IsNullOrEmpty(obj.IGST.ToString()))
                    com.Parameters.AddWithValue("@IGST", obj.IGST);
                else
                    com.Parameters.AddWithValue("@IGST", 0);

                if (!string.IsNullOrEmpty(obj.Profit.ToString()))
                    com.Parameters.AddWithValue("@Profit", obj.Profit);
                else
                    com.Parameters.AddWithValue("@Profit", 0);

               
                if (!string.IsNullOrEmpty(obj.SellPrice.ToString()))
                    com.Parameters.AddWithValue("@SellPrice", obj.SellPrice);
                else
                    com.Parameters.AddWithValue("@SellPrice", 0);


                if (!string.IsNullOrEmpty(obj.VendorId.ToString()))
                    com.Parameters.AddWithValue("@VendorId", obj.VendorId);
                else
                    com.Parameters.AddWithValue("@VendorId", 0);

                

                if (!string.IsNullOrEmpty(obj.VendorCatId.ToString()))
                    com.Parameters.AddWithValue("@VendorCatId", obj.VendorCatId);
                else
                    com.Parameters.AddWithValue("@VendorCatId", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.B2BProfit.ToString()))
                    com.Parameters.AddWithValue("@B2BProfit", obj.B2BProfit);
                else
                    com.Parameters.AddWithValue("@B2BProfit", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.B2BSellPrice.ToString()))
                    com.Parameters.AddWithValue("@B2BSellPrice", obj.B2BSellPrice);
                else
                    com.Parameters.AddWithValue("@B2BSellPrice", DBNull.Value);
                if (!string.IsNullOrEmpty(rewardpoint.ToString()))
                    com.Parameters.AddWithValue("@RewardPoint", rewardpoint);
                else
                    com.Parameters.AddWithValue("@RewardPoint", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.updatedon.ToString()))
                    com.Parameters.AddWithValue("@UpdatedOn", obj.updatedon);
                else
                    com.Parameters.AddWithValue("@UpdatedOn", DBNull.Value);
            com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
            i = com.ExecuteNonQuery();
                con.Close();
            //}
            //catch (Exception ex)
            //{ }
            return i;

        }

        public int UpdateproductReplacement(Vendor obj,int? newproid)
        {
            int i = 0;
            try
            {
                con.Open();

              

                SqlCommand com = new SqlCommand("Update tbl_Customer_Order_Detail Set AttributeId=@AttributeId,VendorId=@VendorId,VendorCatId=@VendorCatId,SectorId=@SectorId,DeliveryBoyId=@DeliveryBoyId,ProductId=@ProductId Where OrderId=@OrderId ", con);
                com.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(obj.AttributeId.ToString()))
                    com.Parameters.AddWithValue("@AttributeId", obj.AttributeId);
                else
                    com.Parameters.AddWithValue("@AttributeId", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.VendorId.ToString()))
                    com.Parameters.AddWithValue("@VendorId", obj.VendorId);
                else
                    com.Parameters.AddWithValue("@VendorId", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.VendorCatId.ToString()))
                    com.Parameters.AddWithValue("@VendorCatId", obj.VendorCatId);
                else
                    com.Parameters.AddWithValue("@VendorCatId", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    com.Parameters.AddWithValue("@SectorId", obj.SectorId);
                else
                    com.Parameters.AddWithValue("@SectorId", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.DeliveryBoyId.ToString()))
                    com.Parameters.AddWithValue("@DeliveryBoyId", obj.DeliveryBoyId);
                else
                    com.Parameters.AddWithValue("@DeliveryBoyId", 0);

                if (!string.IsNullOrEmpty(newproid.ToString()))
                    com.Parameters.AddWithValue("@ProductId", newproid);
                else
                    com.Parameters.AddWithValue("@ProductId", DBNull.Value);



                if (!string.IsNullOrEmpty(obj.OrderId))
                    com.Parameters.AddWithValue("@OrderId", obj.OrderId);
                else
                    com.Parameters.AddWithValue("@OrderId", 0);


               



                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public DataTable GetSectorListByCityVendor(int? CityId,int? VendorCatid)
        {
            if (CityId == 0) CityId = null;
            if (VendorCatid == 0) VendorCatid = null;
            string query = "SELECT Sm.Id,Sm.SectorName,Vsa.IsActive FROM tbl_Sector_Master Sm";
            query += " Left Join tbl_Vendorcat_SectorAssign Vsa On Sm.Id=Vsa.SectorId And (@VendorCatId Is Null Or Vsa.VendorCatId=@VendorCatId)";
            query += " WHERE (@CityId Is Null Or Sm.CityId =@CityId) ";
            //con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            if (!string.IsNullOrEmpty(VendorCatid.ToString()))
                cmd.Parameters.AddWithValue("@VendorCatId", VendorCatid);
            else
                cmd.Parameters.AddWithValue("@VendorCatId", DBNull.Value);

            if (!string.IsNullOrEmpty(CityId.ToString()))
                cmd.Parameters.AddWithValue("@CityId", CityId);
            else
                cmd.Parameters.AddWithValue("@CityId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable GetSectorListByCityVendorOffer(int? CityId, int? VendorCatid)
        {
            if (CityId == 0) CityId = null;
            if (VendorCatid == 0) VendorCatid = null;
            string query = "SELECT Sm.Id,Sm.SectorName,Vsa.IsActive FROM tbl_Sector_Master Sm";
            query += " Inner Join tbl_Vendorcat_SectorAssign Vsa On Sm.Id=Vsa.SectorId And (@VendorCatId Is Null Or Vsa.VendorCatId=@VendorCatId)";
            query += " WHERE (@CityId Is Null Or Sm.CityId =@CityId) ";
            //con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            if (!string.IsNullOrEmpty(VendorCatid.ToString()))
                cmd.Parameters.AddWithValue("@VendorCatId", VendorCatid);
            else
                cmd.Parameters.AddWithValue("@VendorCatId", DBNull.Value);

            if (!string.IsNullOrEmpty(CityId.ToString()))
                cmd.Parameters.AddWithValue("@CityId", CityId);
            else
                cmd.Parameters.AddWithValue("@CityId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable getVendorcatProductAttribute(int? VendorCatId,int? Proid)
        {
            //con.Open();
            string query = "SELECT Distinct Pa.ProductID,Pm.ProductName,Pa.VendorCatId,Pa.AttributeID,At.Id,At.Name FROM [dbo].tbl_Product_Master Pm Inner Join tbl_Product_Attributes Pa On Pa.ProductID=Pm.Id Inner Join tbl_Attributes At On At.Id=Pa.AttributeID where (@VendorCatId Is Null Or Pa.VendorCatId=@VendorCatId) And (@ProductID Is Null Or Pa.ProductID=@ProductID)";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(VendorCatId.ToString()))
                cmd.Parameters.AddWithValue("@VendorCatId", VendorCatId);
            else
                cmd.Parameters.AddWithValue("@VendorCatId", DBNull.Value);

            if (!string.IsNullOrEmpty(Proid.ToString()))
                cmd.Parameters.AddWithValue("@ProductID", Proid);
            else
                cmd.Parameters.AddWithValue("@ProductID", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getOfferVendorList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Select * from tbl_Vendor_Master Where VendorCat='Offer'", con);
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

        public DataTable CheckDuplicateProduct(string product,int VendorId)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Vendor_ProductDetail where VendorId='" + VendorId + "' and ProductName='" + product + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public int InsertProduct(Vendor obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Vendor_Product_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
                if (!string.IsNullOrEmpty(obj.VendorId.ToString()))
                    com.Parameters.AddWithValue("@VendorId", obj.VendorId);
                else
                    com.Parameters.AddWithValue("@VendorId", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.ProductName))
                    com.Parameters.AddWithValue("@ProductName", obj.ProductName);
                else
                    com.Parameters.AddWithValue("@ProductName", DBNull.Value);
                
                
                if (!string.IsNullOrEmpty(obj.CGST.ToString()))
                    com.Parameters.AddWithValue("@CGST", obj.CGST);
                else
                    com.Parameters.AddWithValue("@CGST", 0);
                if (!string.IsNullOrEmpty(obj.SGST.ToString()))
                    com.Parameters.AddWithValue("@SGST", obj.SGST);
                else
                    com.Parameters.AddWithValue("@SGST", 0);
                if (!string.IsNullOrEmpty(obj.IGST.ToString()))
                    com.Parameters.AddWithValue("@IGST", obj.IGST);
                else
                    com.Parameters.AddWithValue("@IGST", 0);
               
                if (!string.IsNullOrEmpty(obj.Detail))
                    com.Parameters.AddWithValue("@Detail", obj.Detail);
                else
                    com.Parameters.AddWithValue("@Detail", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Photo))
                    com.Parameters.AddWithValue("@Image", obj.Photo);
                else
                    com.Parameters.AddWithValue("@Image", DBNull.Value);
               
                if (!string.IsNullOrEmpty(obj.SellPrice.ToString()))
                    com.Parameters.AddWithValue("@SalePrice", obj.SellPrice);
                else
                    com.Parameters.AddWithValue("@SalePrice", 0);
               
               
                if (!string.IsNullOrEmpty(obj.SortOrder.ToString()))
                    com.Parameters.AddWithValue("@OrderBy", obj.SortOrder);
                else
                    com.Parameters.AddWithValue("@OrderBy", 0);

               
                com.Parameters.AddWithValue("@IsActive", obj.IsActive);
              
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;




                i = com.ExecuteNonQuery();
                i = Convert.ToInt32(com.Parameters["@Id"].Value);

              
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public int UpdateOfferVendor(Vendor obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Offer_Vendor_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.VendorId);
                if (!string.IsNullOrEmpty(obj.FirstName))
                    com.Parameters.AddWithValue("@FirstName", obj.FirstName);
                else
                    com.Parameters.AddWithValue("@FirstName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.LastName))
                    com.Parameters.AddWithValue("@LastName", obj.LastName);
                else
                    com.Parameters.AddWithValue("@LastName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.MobileNo))
                    com.Parameters.AddWithValue("@MobileNo", obj.MobileNo);
                else
                    com.Parameters.AddWithValue("@MobileNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Email))
                    com.Parameters.AddWithValue("@Email", obj.Email);
                else
                    com.Parameters.AddWithValue("@Email", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Address))
                    com.Parameters.AddWithValue("@Address", obj.Address);
                else
                    com.Parameters.AddWithValue("@Address", DBNull.Value);
                com.Parameters.AddWithValue("@UserName", obj.UserName);
                com.Parameters.AddWithValue("@Password", obj.Password);
                if (!string.IsNullOrEmpty(obj.Photo))
                    com.Parameters.AddWithValue("@Photo", obj.Photo);
                else
                    com.Parameters.AddWithValue("@Photo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    com.Parameters.AddWithValue("@SectorId", obj.SectorId);
                else
                    com.Parameters.AddWithValue("@SectorId", DBNull.Value);
                com.Parameters.AddWithValue("@IsActive", obj.IsActive);
                if (!string.IsNullOrEmpty(obj.CompanyName))
                    com.Parameters.AddWithValue("@CompanyName", obj.CompanyName);
                else
                    com.Parameters.AddWithValue("@CompanyName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.PanCardNo))
                    com.Parameters.AddWithValue("@PanCardNo", obj.PanCardNo);
                else
                    com.Parameters.AddWithValue("@PanCardNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.GSTNo))
                    com.Parameters.AddWithValue("@GSTNo", obj.GSTNo);
                else
                    com.Parameters.AddWithValue("@GSTNo", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.VendorType))
                    com.Parameters.AddWithValue("@VendorType", obj.VendorType);
                else
                    com.Parameters.AddWithValue("@VendorType", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.VendorMasterCat))
                    com.Parameters.AddWithValue("@MasterCat", obj.VendorMasterCat);
                else
                    com.Parameters.AddWithValue("@MasterCat", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.StoreName))
                    com.Parameters.AddWithValue("@Vendorstore", obj.StoreName);
                else
                    com.Parameters.AddWithValue("@Vendorstore", DBNull.Value);



                if (!string.IsNullOrEmpty(obj.VendorCat))
                    com.Parameters.AddWithValue("@VendorCat", obj.VendorCat);
                else
                    com.Parameters.AddWithValue("@VendorCat", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.Slider1))
                    com.Parameters.AddWithValue("@Slider1", obj.Slider1);
                else
                    com.Parameters.AddWithValue("@Slider1", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.Slider2))
                    com.Parameters.AddWithValue("@Slider2", obj.Slider2);
                else
                    com.Parameters.AddWithValue("@Slider2", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.State.ToString()))
                    com.Parameters.AddWithValue("@State", obj.State);
                else
                    com.Parameters.AddWithValue("@State", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.City.ToString()))
                    com.Parameters.AddWithValue("@City", obj.City);
                else
                    com.Parameters.AddWithValue("@City", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.MilkywayPer.ToString()))
                    com.Parameters.AddWithValue("@MilkywayPer", obj.MilkywayPer);
                else
                    com.Parameters.AddWithValue("@MilkywayPer", 0);

                if (!string.IsNullOrEmpty(obj.OverallDiscountPer.ToString()))
                    com.Parameters.AddWithValue("@DiscountPer", obj.OverallDiscountPer);
                else
                    com.Parameters.AddWithValue("@DiscountPer", 0);

                if (!string.IsNullOrEmpty(obj.DiscountIsActive.ToString()))
                    com.Parameters.AddWithValue("@DiscountActive", obj.DiscountIsActive);
                else
                    com.Parameters.AddWithValue("@DiscountActive", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.VendorTerms))
                    com.Parameters.AddWithValue("@Vendorterms", obj.VendorTerms);
                else
                    com.Parameters.AddWithValue("@Vendorterms", DBNull.Value);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public int UpdateVendorSectorAssigncommon(string vendorId)
        {




            string active = "0";
            con.Open();
            SqlCommand cmd = new SqlCommand("UPDATE tbl_Vendorcat_SectorAssign set IsActive=@IsActive where VendorId=@VendorId", con);


            if (vendorId != "")
                cmd.Parameters.AddWithValue("@VendorId", vendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);

            if (active != "")
                cmd.Parameters.AddWithValue("@IsActive", active);
            else
                cmd.Parameters.AddWithValue("@IsActive", 0);

            int result = cmd.ExecuteNonQuery();


            SqlCommand cmd1 = new SqlCommand("UPDATE tbl_Vendor_Sector_Assign set IsActive=@IsActive where VendorId=@VendorId", con);


            if (vendorId != "")
                cmd1.Parameters.AddWithValue("@VendorId", vendorId);
            else
                cmd1.Parameters.AddWithValue("@VendorId", DBNull.Value);

            if (active != "")
                cmd1.Parameters.AddWithValue("@IsActive", active);
            else
                cmd1.Parameters.AddWithValue("@IsActive", 0);

            int result1 = cmd1.ExecuteNonQuery();

            con.Close();
            return result;


        }
        public int UpdateVendorSectorAssign(string vendorId,string item)
        {

           string active = "1";
            con.Open();


            SqlCommand cmd = new SqlCommand("UPDATE tbl_Vendor_Sector_Assign set IsActive=@IsActive where VendorId=@VendorId and SectorId=@SectorId", con);


            if (vendorId != "")
                cmd.Parameters.AddWithValue("@VendorId", vendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);
            
            if (item != "")
                cmd.Parameters.AddWithValue("@SectorId", item);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);

            if (active != "")
                cmd.Parameters.AddWithValue("@IsActive", active);
            else
                cmd.Parameters.AddWithValue("@IsActive", 0);


            int result = cmd.ExecuteNonQuery();




            SqlCommand cmd1 = new SqlCommand("UPDATE tbl_Vendorcat_SectorAssign set IsActive=@IsActive where VendorId=@VendorId and SectorId=@SectorId", con);


            if (vendorId != "")
                cmd1.Parameters.AddWithValue("@VendorId", vendorId);
            else
                cmd1.Parameters.AddWithValue("@VendorId", DBNull.Value);

            if (item != "")
                cmd1.Parameters.AddWithValue("@SectorId", item);
            else
                cmd1.Parameters.AddWithValue("@SectorId", DBNull.Value);

            if (active != "")
                cmd1.Parameters.AddWithValue("@IsActive", active);
            else
                cmd1.Parameters.AddWithValue("@IsActive", 0);


            int result1 = cmd1.ExecuteNonQuery();

            con.Close();
            return result;


        }

        public DataTable getOfferVendorProductList(int? VendorId)
        {
            
            if (VendorId == 0) VendorId = null;
            string query = "SELECT * FROM tbl_Vendor_ProductDetail ";
            
            query += " WHERE (@VendorId Is Null Or VendorId =@VendorId) ";
            //con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
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

        public int UpdateOfferVendorProduct(Vendor obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Vendor_Product_Update_self", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.Id);
                if (!string.IsNullOrEmpty(obj.ProductName))
                    com.Parameters.AddWithValue("@ProductName", obj.ProductName);
                else
                    com.Parameters.AddWithValue("@ProductName", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.CGST.ToString()))
                    com.Parameters.AddWithValue("@CGST", obj.CGST);
                else
                    com.Parameters.AddWithValue("@CGST", 0);
                if (!string.IsNullOrEmpty(obj.SGST.ToString()))
                    com.Parameters.AddWithValue("@SGST", obj.SGST);
                else
                    com.Parameters.AddWithValue("@SGST", 0);
                if (!string.IsNullOrEmpty(obj.IGST.ToString()))
                    com.Parameters.AddWithValue("@IGST", obj.IGST);
                else
                    com.Parameters.AddWithValue("@IGST", 0);

                if (!string.IsNullOrEmpty(obj.Detail))
                    com.Parameters.AddWithValue("@Detail", obj.Detail);
                else
                    com.Parameters.AddWithValue("@Detail", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Photo))
                    com.Parameters.AddWithValue("@Image", obj.Photo);
                else
                    com.Parameters.AddWithValue("@Image", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.SellPrice.ToString()))
                    com.Parameters.AddWithValue("@SalePrice", obj.SellPrice);
                else
                    com.Parameters.AddWithValue("@SalePrice", 0);


                if (!string.IsNullOrEmpty(obj.SortOrder.ToString()))
                    com.Parameters.AddWithValue("@OrderBy", obj.SortOrder);
                else
                    com.Parameters.AddWithValue("@OrderBy", 0);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int OfferDetailInsert(Vendor obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Vendor_Offer_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
                if (!string.IsNullOrEmpty(obj.VendorId.ToString()))
                    com.Parameters.AddWithValue("@VendorId", obj.VendorId);
                else
                    com.Parameters.AddWithValue("@VendorId", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.FromDate.ToString()))
                    com.Parameters.AddWithValue("@Validfrom", obj.FromDate);
                else
                    com.Parameters.AddWithValue("@Validfrom", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.ToDate.ToString()))
                    com.Parameters.AddWithValue("@ValidTo", obj.ToDate);
                else
                    com.Parameters.AddWithValue("@ValidTo", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.OfferType))
                    com.Parameters.AddWithValue("@OfferType", obj.OfferType);
                else
                    com.Parameters.AddWithValue("@OfferType", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Offeramount.ToString()))
                    com.Parameters.AddWithValue("@OfferValue", obj.Offeramount);
                else
                    com.Parameters.AddWithValue("@OfferValue", 0);

                if (!string.IsNullOrEmpty(obj.Offerdaytype))
                    com.Parameters.AddWithValue("@OfferCat", obj.Offerdaytype);
                else
                    com.Parameters.AddWithValue("@OfferCat", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.OfferDates))
                    com.Parameters.AddWithValue("@offerdates", obj.OfferDates);
                else
                    com.Parameters.AddWithValue("@offerdates", DBNull.Value);

            

               


                com.Parameters.AddWithValue("@IsActive", obj.IsActive);
                if (!string.IsNullOrEmpty(obj.updatedon.ToString()))
                    com.Parameters.AddWithValue("@UpdatedOn", obj.updatedon);
                else
                    com.Parameters.AddWithValue("@UpdatedOn", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.Products))
                    com.Parameters.AddWithValue("@ProductId", obj.Products);
                else
                    com.Parameters.AddWithValue("@ProductId", DBNull.Value);

                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;




                i = com.ExecuteNonQuery();
                i = Convert.ToInt32(com.Parameters["@Id"].Value);


                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public int OfferSectorInsert(Vendor obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Insert Into tbl_OfferVoucherSector(SectorId,IsActive,OfferId,VendorId) Values(@SectorId,@IsActive,@OfferId,@VendorId)", con);
                com.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(obj.VendorId.ToString()))
                    com.Parameters.AddWithValue("@VendorId", obj.VendorId);
                else
                    com.Parameters.AddWithValue("@VendorId", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    com.Parameters.AddWithValue("@SectorId", obj.SectorId);
                else
                    com.Parameters.AddWithValue("@SectorId", DBNull.Value);


                com.Parameters.AddWithValue("@IsActive", obj.IsActive);
              

                if (!string.IsNullOrEmpty(obj.OfferId))
                    com.Parameters.AddWithValue("@OfferId", obj.OfferId);
                else
                    com.Parameters.AddWithValue("@OfferId", DBNull.Value);

                i = com.ExecuteNonQuery();
               // i = Convert.ToInt32(com.Parameters["@Id"].Value);


                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }
    }
}
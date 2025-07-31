using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Models
{
    public class Product
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
        //category
        public int? ParentCategoryId { get; set; }
        public int Id { get; set; }
        public string Category { get; set; }
        public int OrderBy { get; set; }
        public string SubCategory { get; set; }
        //product
        public int CategoryId { get; set; }
        public string ProductName { get; set; }
        public string Code { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
        public long RewardPoint { get; set; }
        public string Image { get; set; }
        public string Detail { get; set; }
        public decimal Subscription { get; set; }
        public bool IsActive { get; set; }
        public bool IsDaily { get; set; }
        public bool IsAlternate { get; set; }
        public bool IsMultipleDay { get; set; }
        public bool IsWeeklyDay { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal SaleAmount { get; set; }
        public decimal Profit { get; set; }
        public string YoutubeTitle { get; set; }
        public string YoutubeURL { get; set; }

        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public TimeSpan DeliveryFrom { get; set; }
        public TimeSpan DeliveryTo { get; set; }
        public string MainImage { get; set; }

        public string previous1 { get; set; }
        public string next1 { get; set; }


        public string startpoint { get; set; }
        public string endpoint { get; set; }

        public DateTime updatedon { get; set; }
        clsCommon _clsCommon = new clsCommon();



        public DataTable BindProuctAttributewise(int? Id,int? Atid,int? Vid)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            string query = "SELECT Pa.*,Pm.ProductName From tbl_Product_Attributes Pa inner join tbl_Product_Master Pm ";
            query += " On Pa.ProductID=Pm.Id ";
            query += " where (@ProductId is null or pa.ProductID=@ProductId) and ( @AttributeID is null or Pa.AttributeID=@AttributeID) and (@VendorCatId Is null or Pa.VendorCatId=@VendorCatId) ";

            //SqlCommand cmd = new SqlCommand("Product_SelectAll", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@ProductId", Id);
            else
                cmd.Parameters.AddWithValue("@ProductId", DBNull.Value);

            if (!string.IsNullOrEmpty(Atid.ToString()))
                cmd.Parameters.AddWithValue("@AttributeID", Atid);
            else
                cmd.Parameters.AddWithValue("@AttributeID", DBNull.Value);

            if (!string.IsNullOrEmpty(Vid.ToString()))
                cmd.Parameters.AddWithValue("@VendorCatId", Vid);
            else
                cmd.Parameters.AddWithValue("@VendorCatId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public DataTable BindProuctAttribute(int? Id)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();


            SqlCommand cmd = new SqlCommand("Select Pa.*,At.Name,Pm.ProductName from tbl_Product_Attributes Pa Inner Join tbl_Attributes At On Pa.AttributeID=At.ID Inner Join tbl_Product_Master Pm On pm.Id=Pa.ProductID Where @Id is Null Or Pa.ID=@Id", con);

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


        public DataTable BindOfferProuctOrder(int? Id, int? VendorId, int? OfferId)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();


            SqlCommand cmd = new SqlCommand("Select * from tbl_Vendor_ProductDetail where Id=" + Id + " and VendorId=" + VendorId + "", con);

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

        public DataTable BindProuctOrder(int? Id,int AttributeId,int VendorId,int VendorcatId)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();


            SqlCommand cmd = new SqlCommand("Select * from tbl_Product_Attributes where ProductID="+Id+ " and AttributeID="+AttributeId+ " and VendorId="+VendorId+ " and VendorCatId="+VendorcatId+"", con);

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
        public DataTable BindProuct(int? Id)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();

          
            SqlCommand cmd = new SqlCommand("Product_SelectAll", con);
           
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
        public DataTable BindProuct1(int? Id)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();

            string query = "SELECT pm.*,pcm.id,pcm.CategoryName AS Subcat,pcm.ParentCategoryId,pcm1.CategoryName AS Maincat,pcm1.Id ";
            query += "FROM [dbo].[tbl_Product_Category_Master] pcm   left Join ";
            query += "[tbl_Product_Category_Master] pcm1 On pcm1.Id=pcm.ParentCategoryId ";
            query += "inner Join tbl_Product_Master pm On pcm.Id=pm.CategoryId WHERE (@Id IS NULL OR pm.CategoryId = @Id) order by pm.OrderBy ASC";
            // SqlCommand cmd = new SqlCommand("Product_SelectAll", con);
            SqlCommand cmd = new SqlCommand(query, con);
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

		public DataTable BindProuctnew()
		{
			if (con.State == ConnectionState.Open)
			{
				con.Close();
			}

			string query = @"
        SELECT 
            Pm.*, 
            Pcm.Id, 
            Pcm.CategoryName AS Maincat
        FROM 
            [dbo].[tbl_Product_Master] Pm
        INNER JOIN 
            [dbo].[tbl_Product_Category_Master] Pcm ON Pm.CategoryId = Pcm.Id
        ORDER BY 
            Pm.OrderBy ASC
        OFFSET 0 ROWS FETCH NEXT 50 ROWS ONLY";

			con.Open();

			SqlCommand cmd = new SqlCommand(query, con);
			cmd.CommandType = CommandType.Text;

			SqlDataAdapter da = new SqlDataAdapter(cmd);
			DataTable dt = new DataTable();
			da.Fill(dt);
			con.Close();
			return dt;
		}

		public DataTable BindProductVendorList()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }

            string query = @"
        SELECT PVM.*, 
               PM.ProductName, 
               VM.Firstname
        FROM tbl_ProductVendor_Master PVM
        LEFT JOIN tbl_Product_Master PM ON PVM.ProductId = PM.Id
        LEFT JOIN tbl_Vendor_Master VM ON PVM.VendorId = VM.Id
        ORDER BY PVM.OrderBy ASC
        OFFSET 0 ROWS FETCH NEXT 50 ROWS ONLY";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            con.Open();
            da.Fill(dt);
            con.Close();

            return dt;
        }

        public DataTable BindProucttotal()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }

            string query = "SELECT COUNT(*) as totalproduct ";
            query += "FROM [dbo].[tbl_Product_Category_Master] pcm   left Join ";
            query += "[tbl_Product_Category_Master] pcm1 On pcm1.Id=pcm.ParentCategoryId ";
            query += "inner Join tbl_Product_Master pm On pcm.Id=pm.CategoryId";
            con.Open();


            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(Id.ToString()))
            //    cmd.Parameters.AddWithValue("@Id", Id);
            //else
            //    cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
        public DataTable BindProuctcategorywise(string catid)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }

            string query = "SELECT pm.*,pcm.id,pcm.CategoryName AS Subcat,pcm.ParentCategoryId,pcm1.CategoryName AS Maincat,pcm1.Id ";
            query += "FROM [dbo].[tbl_Product_Category_Master] pcm   left Join ";
            query += "[tbl_Product_Category_Master] pcm1 On pcm1.Id=pcm.ParentCategoryId ";
            query += "inner Join tbl_Product_Master pm On pcm.Id=pm.CategoryId where (pm.CategoryId="+catid+ " or pcm.ParentCategoryId="+catid+") order by pm.OrderBy ASC";
            con.Open();


            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(Id.ToString()))
            //    cmd.Parameters.AddWithValue("@Id", Id);
            //else
            //    cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable BindProuctnewnext(string startid)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }

            string query = "SELECT pm.*,pcm.id,pcm.CategoryName AS Subcat,pcm.ParentCategoryId,pcm1.CategoryName AS Maincat,pcm1.Id ";
            query += "FROM [dbo].[tbl_Product_Category_Master] pcm   left Join ";
            query += "[tbl_Product_Category_Master] pcm1 On pcm1.Id=pcm.ParentCategoryId ";
            query += "inner Join tbl_Product_Master pm On pcm.Id=pm.CategoryId order by pm.OrderBy ASC OFFSET "+startid+" ROWS FETCH NEXT 50 ROWS ONLY";
            con.Open();


            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(Id.ToString()))
            //    cmd.Parameters.AddWithValue("@Id", Id);
            //else
            //    cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public DataTable BindProuctnewLast()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }

            string query = "SELECT Top 50 pm.*,pcm.id,pcm.CategoryName AS Subcat,pcm.ParentCategoryId,pcm1.CategoryName AS Maincat,pcm1.Id ";
            query += "FROM [dbo].[tbl_Product_Category_Master] pcm   left Join ";
            query += "[tbl_Product_Category_Master] pcm1 On pcm1.Id=pcm.ParentCategoryId ";
            query += "inner Join tbl_Product_Master pm On pcm.Id=pm.CategoryId order by pm.OrderBy DESC";
            con.Open();


            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(Id.ToString()))
            //    cmd.Parameters.AddWithValue("@Id", Id);
            //else
            //    cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
        public DataTable BindProuctPhotos(int? Id)
        {
            var lstPhoto = _clsCommon.selectwhere("*", "tbl_Product_Images", " ProductId='" + Id + "'");
            return lstPhoto;
        }

        public DataTable BindSectorProuct(int? SectorId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Sector_Product_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable BindCategProuct(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Category_Product_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@CategoryId", Id);
            else
                cmd.Parameters.AddWithValue("@CategoryId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable BindCategActiveProuct(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Category_Product_Active_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@CategoryId", Id);
            else
                cmd.Parameters.AddWithValue("@CategoryId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable BindCategsubActiveProuct(int? Id,int? pcatid)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("CategorySubcat_Product_Active_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(pcatid.ToString()))
                cmd.Parameters.AddWithValue("@CategoryId", pcatid);
            else
                cmd.Parameters.AddWithValue("@CategoryId", DBNull.Value);
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@SubCategoryId", Id);
            else
                cmd.Parameters.AddWithValue("@SubCategoryId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable BindSectAssignedProduct(int? cId, int? sId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Sectorwise_Category_Product_Assigned_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@CategoryId", cId);
            else
                cmd.Parameters.AddWithValue("@CategoryId", DBNull.Value);
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@SecId", sId);
            else
                cmd.Parameters.AddWithValue("@SecId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable GetVendorAssignedProduct(int? cId, int? sId,int vendorId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SP_Vendor_Product_Assigned_List", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@CategoryId", cId);
            else
                cmd.Parameters.AddWithValue("@CategoryId", DBNull.Value);
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@SecId", sId);
            else
                cmd.Parameters.AddWithValue("@SecId", DBNull.Value);
            if (!string.IsNullOrEmpty(vendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", vendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
       
        public DataTable BindSectorCategProuct(int? SectorId, int? CategoryId)
        {
            con.Open();


            string query = " SELECT pm.*,pcm.CategoryName  from tbl_Vendor_Product_Assign vpa left join[dbo].[tbl_Product_Category_Master] pcm on pcm.Id = vpa.CategoryId";
            query += "  left join[dbo].[tbl_Product_Master] pm on pm.Id = vpa.ProductId  WHERE (@SectorId IS NULL OR vpa.[SectorId] = @SectorId) and pm.IsActive=1 and vpa.IsActive= 1";

            query += " and (@CategoryId IS NULL OR vpa.[CategoryId] = @CategoryId or pcm.ParentCategoryId=@CategoryId OR pm.CategoryId=@CategoryId ) Order by pm.OrderBy ASC";




            // SqlCommand cmd = new SqlCommand("Sector_Category_Product_SelectAll", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
               
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(CategoryId.ToString()))
                cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
            else
                cmd.Parameters.AddWithValue("@CategoryId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable BindSectorCategProuctNew(string SectorId, string CategoryId, int psize, int pno)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Sector_Category_Product_SelectAll_New", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(SectorId))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(CategoryId))
                cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
            else
                cmd.Parameters.AddWithValue("@CategoryId", DBNull.Value);
            cmd.Parameters.AddWithValue("@PageNum", pno);
            cmd.Parameters.AddWithValue("@PageSize", psize);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public DataTable BindSectorVendorCategProuctNew(string SectorId, string ParentCat, string Subcat, string VendorId, string VendorCatId, int psize, int pno)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Sector_VendorCategory_Product_SelectAll_New", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(SectorId))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(ParentCat))
                cmd.Parameters.AddWithValue("@ParentCatId", ParentCat);
            else
                cmd.Parameters.AddWithValue("@ParentCatId", DBNull.Value);

            if (!string.IsNullOrEmpty(Subcat))
                cmd.Parameters.AddWithValue("@ParentSubCatId", Subcat);
            else
                cmd.Parameters.AddWithValue("@ParentSubCatId", DBNull.Value);


            if (!string.IsNullOrEmpty(VendorId))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);

            if (!string.IsNullOrEmpty(VendorCatId))
                cmd.Parameters.AddWithValue("@VendorCatId", VendorCatId);
            else
                cmd.Parameters.AddWithValue("@VendorCatId", DBNull.Value);

            cmd.Parameters.AddWithValue("@PageNum", pno);
            cmd.Parameters.AddWithValue("@PageSize", psize);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
        public int InsertProdtcategory(Product obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("SP_Product_Category_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
                if (obj.ParentCategoryId != null)
                    com.Parameters.AddWithValue("@ParentCategoryId", obj.ParentCategoryId);
                else
                    com.Parameters.AddWithValue("@ParentCategoryId", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Category))
                    com.Parameters.AddWithValue("@CategoryName", obj.Category);
                else
                    com.Parameters.AddWithValue("@CategoryName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.IsActive.ToString()))
                    com.Parameters.AddWithValue("@IsActive", obj.IsActive);
                else
                    com.Parameters.AddWithValue("@IsActive", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Image))
                    com.Parameters.AddWithValue("@Image", obj.Image);
                else
                    com.Parameters.AddWithValue("@Image", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.OrderBy.ToString()))
                    com.Parameters.AddWithValue("@OrderBy", obj.OrderBy);
                else
                    com.Parameters.AddWithValue("@OrderBy", 0);

                if (obj.MinAmount > 0)
                    com.Parameters.AddWithValue("@MinAmount", obj.MinAmount);
                else
                    com.Parameters.AddWithValue("@MinAmount", 0);
                if (obj.MinAmount > 0)
                    com.Parameters.AddWithValue("@MaxAmount", obj.MaxAmount);
                else
                    com.Parameters.AddWithValue("@MaxAmount", 0);
                if (obj.FromTime != null)
                    com.Parameters.AddWithValue("@FromTime", obj.FromTime);
                else
                    com.Parameters.AddWithValue("@FromTime", 0);
                if (obj.ToTime != null)
                    com.Parameters.AddWithValue("@ToTime", obj.ToTime);
                else
                    com.Parameters.AddWithValue("@ToTime", 0);
                if (obj.DeliveryFrom != null)
                    com.Parameters.AddWithValue("@DeliveryFrom", obj.DeliveryFrom);
                else
                    com.Parameters.AddWithValue("@DeliveryFrom", 0);
                if (obj.DeliveryTo != null)
                    com.Parameters.AddWithValue("@DeliveryTo", obj.DeliveryTo);
                else
                    com.Parameters.AddWithValue("@DeliveryTo", 0);

                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public int InsertProdtParentcategory(Product obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("INSERT INTO tbl_Parent_Category_Master(ParentCategory,IsActive,Image,OrderBy)VALUES(@CategoryName,@IsActive,@Image,@OrderBy)", con);
                com.CommandType = CommandType.Text;
                
                if (!string.IsNullOrEmpty(obj.Category))
                    com.Parameters.AddWithValue("@CategoryName", obj.Category);
                else
                    com.Parameters.AddWithValue("@CategoryName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.IsActive.ToString()))
                    com.Parameters.AddWithValue("@IsActive", obj.IsActive);
                else
                    com.Parameters.AddWithValue("@IsActive", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Image))
                    com.Parameters.AddWithValue("@Image", obj.Image);
                else
                    com.Parameters.AddWithValue("@Image", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.OrderBy.ToString()))
                    com.Parameters.AddWithValue("@OrderBy", obj.OrderBy);
                else
                    com.Parameters.AddWithValue("@OrderBy", 0);

                

                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public int InsertProdtSubcategory(Product obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("INSERT INTO tbl_Product_Subcat_Master(ParentCategoryId,SubCatName,IsActive,Image,OrderBy)VALUES(@ParentCategoryId,@SubCatName,@IsActive,@Image,@OrderBy)", con);
                com.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(obj.CategoryId.ToString()))
                    com.Parameters.AddWithValue("@ParentCategoryId", obj.CategoryId);
                else
                    com.Parameters.AddWithValue("@ParentCategoryId", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.SubCategory.ToString()))
                    com.Parameters.AddWithValue("@SubCatName", obj.SubCategory);
                else
                    com.Parameters.AddWithValue("@SubCatName", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.IsActive.ToString()))
                    com.Parameters.AddWithValue("@IsActive", obj.IsActive);
                else
                    com.Parameters.AddWithValue("@IsActive", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Image))
                    com.Parameters.AddWithValue("@Image", obj.Image);
                else
                    com.Parameters.AddWithValue("@Image", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.OrderBy.ToString()))
                    com.Parameters.AddWithValue("@OrderBy", obj.OrderBy);
                else
                    com.Parameters.AddWithValue("@OrderBy", 0);



                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }
        public int UpdateProdtcategory(Product obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("SP_Product_Category_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.CategoryId);
                if (obj.ParentCategoryId != null)
                    com.Parameters.AddWithValue("@ParentCategoryId", obj.ParentCategoryId);
                else
                    com.Parameters.AddWithValue("@ParentCategoryId", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Category))
                    com.Parameters.AddWithValue("@CategoryName", obj.Category);
                else
                    com.Parameters.AddWithValue("@CategoryName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.IsActive.ToString()))
                    com.Parameters.AddWithValue("@IsActive", obj.IsActive);
                else
                    com.Parameters.AddWithValue("@IsActive", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Image))
                    com.Parameters.AddWithValue("@Image", obj.Image);
                else
                    com.Parameters.AddWithValue("@Image", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.OrderBy.ToString()))
                    com.Parameters.AddWithValue("@OrderBy", obj.OrderBy);
                else
                    com.Parameters.AddWithValue("@OrderBy", 0);

                if (obj.MinAmount > 0)
                    com.Parameters.AddWithValue("@MinAmount", obj.MinAmount);
                else
                    com.Parameters.AddWithValue("@MinAmount", 0);
                if (obj.MaxAmount > 0)
                    com.Parameters.AddWithValue("@MaxAmount", obj.MaxAmount);
                else
                    com.Parameters.AddWithValue("@MaxAmount", 0);
                if (obj.FromTime != null)
                    com.Parameters.AddWithValue("@FromTime", obj.FromTime);
                else
                    com.Parameters.AddWithValue("@FromTime", 0);
                if (obj.ToTime != null)
                    com.Parameters.AddWithValue("@ToTime", obj.ToTime);
                else
                    com.Parameters.AddWithValue("@ToTime", 0);

                if (obj.DeliveryFrom != null)
                    com.Parameters.AddWithValue("@DeliveryFrom", obj.DeliveryFrom);
                else
                    com.Parameters.AddWithValue("@DeliveryFrom", 0);
                if (obj.DeliveryTo != null)
                    com.Parameters.AddWithValue("@DeliveryTo", obj.DeliveryTo);
                else
                    com.Parameters.AddWithValue("@DeliveryTo", 0);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }
        public DataTable GetAllProductNames()
        {
            DataTable dt = new DataTable();
            dt = _clsCommon.selectwhere("Id, ProductName", "tbl_Product_Master", "1=1");
            return dt;
        }

        public DataTable GetAllMaincategory()
        {
            DataTable dt = new DataTable();
            dt = _clsCommon.selectwhere("*", "tbl_Product_Category_Master", " ParentCategoryId Is NULL");
            return dt;
        }
        public DataTable GetAllProduct()
        {
            DataTable dt = new DataTable();
            dt = _clsCommon.selectwhere("*", "tbl_Product_Category_Master", " ParentCategoryId Is NULL");
            return dt;
        }
        public DataTable GetAllProducts()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT Id, ProductName FROM tbl_Product_Master ORDER BY ProductName";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                   
                }
           
            }
            return dt;
        }

        public DataTable GetAllMaincategorynew()
        {


            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Parent_Category_Master", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
            //DataTable dt = new DataTable();
            //dt = _clsCommon.selectwhere("*", "tbl_Product_Category_Master", " ParentCategoryId Is NULL");
            //return dt;
        }

        public DataTable GetSubMaincategory(int? CategoryId)
        {
            DataTable dt = new DataTable();
            dt = _clsCommon.selectwhere("*", "tbl_Product_Category_Master", " IsActive='True' AND ParentCategoryId ='" + CategoryId + "' Order by OrderBy ASC");
            return dt;
        }

		public DataTable GetProductFromCategory(int? CategoryId)
		{
			DataTable dt = new DataTable();
			dt = _clsCommon.selectwhere("*", "tbl_Product_Master", " IsActive='True' AND CategoryId ='" + CategoryId + "' Order by OrderBy ASC");
			return dt;
		}

		public DataTable GetSubMaincategorynew(int? CategoryId)
        {

            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Product_Subcat_Master where ParentCategoryId ='" + CategoryId + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
            
        }

        public DataTable BindCategory(int? Id)
        {
            SqlCommand cmd = new SqlCommand("Product_Category_SelectAll", con);
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

        public DataTable BindVendorCategory(int? SectorId,int? ParentCatId)
        {

            string query = "SELECT Vs.VendorId,Vs.VendorCatId,Vs.SectorId,Sm.SectorName,Vc.VendorCatname,Concat(Vm.FirstName,' ',Vm.LastName) As Name,Vc.Catimg,Vc.IsActive,Pcm.ParentCategory,Pcm.Id";
           


            query += " from tbl_Vendorcat_SectorAssign Vs ";
            query += " inner join tbl_VendorCat Vc On Vc.Id=Vs.VendorCatId";
            query += " inner join tbl_Vendor_Master Vm On Vm.Id=Vc.VendorId";
            query += " Inner join tbl_Parent_Category_Master Pcm On Pcm.Id=Vc.ParentCatId";
            query += " inner join tbl_Sector_Master Sm on Sm.Id=Vs.SectorId";
            query += " where (@ParentCatId Is Null Or Pcm.Id=@ParentCatId) and (@SectorId Is Null Or Vs.SectorId=@SectorId) and Vc.IsActive='True'";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", 0);

            if (!string.IsNullOrEmpty(ParentCatId.ToString()))
                cmd.Parameters.AddWithValue("@ParentCatId", ParentCatId);
            else
                cmd.Parameters.AddWithValue("@ParentCatId", 0);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public DataTable BindVendorSubCategory(int? SectorId, int? ParentCatId,int? VendorId,int? VendorCatId)
        {

            string query = "SELECT Vs.VendorId,Vs.VendorCatId,Vs.SectorId,Sm.SectorName,Vc.VendorCatname,Concat(Vm.FirstName,' ',Vm.LastName) As Name,Vc.Catimg,Vc.IsActive,Pcm.ParentCategory,Pcm.Id,";
            query += " Vca.Subcat,Psm.SubCatName,Psm.Id As subcatId,Psm.Image As SubcatImage";
            query += " ,Vc.MinAmount,Vc.MaxAmount,Vc.FromTime,Vc.ToTime,Vc.DeliveryFrom,Vc.DeliveryTo";
            query += " from tbl_Vendorcat_SectorAssign Vs ";
            query += " inner join tbl_VendorCat Vc On Vc.Id=Vs.VendorCatId";
            query += " inner join tbl_Vendor_Master Vm On Vm.Id=Vc.VendorId";
            query += " Inner join tbl_Parent_Category_Master Pcm On Pcm.Id=Vc.ParentCatId";
            query += " inner join tbl_Sector_Master Sm on Sm.Id=Vs.SectorId";
            query += " inner join tbl_Vendor_CatSubcat_Assign Vca on Vca.ParentCat=Vc.ParentCatId";
            query += " inner join tbl_Product_Subcat_Master Psm on Psm.Id=Vca.Subcat";
            query += " where @ParentCatId Is Null Or Pcm.Id=@ParentCatId and @SectorId Is Null Or Vs.SectorId=@SectorId and Vc.IsActive='True'";
            query += " and Vca.IsActive='True'  and (@VendorId Is Null Or  Vca.VendorId=@VendorId) and (@VendorCatId Is Null Or Vca.VendorCatId=@VendorCatId)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", 0);

            if (!string.IsNullOrEmpty(ParentCatId.ToString()))
                cmd.Parameters.AddWithValue("@ParentCatId", ParentCatId);
            else
                cmd.Parameters.AddWithValue("@ParentCatId", 0);


            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", 0);

            if (!string.IsNullOrEmpty(VendorCatId.ToString()))
                cmd.Parameters.AddWithValue("@VendorCatId", VendorCatId);
            else
                cmd.Parameters.AddWithValue("@VendorCatId", 0);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable BindParentCategory(int? Id)
        {
            SqlCommand cmd = new SqlCommand("Product_Parent_Category_SelectAll", con);
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

        public DataTable BindCategoryList(int? Id)
        {
            SqlCommand cmd = new SqlCommand("SP_Product_Category_List", con);
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

        public DataTable BindActiveCategory(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Product_Active_Category_SelectAll", con);
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

        public DataTable BindSectorCategory(int? SectorId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Sector_Product_Category_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable CheckDuplicate(string category)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Product_Category_Master where CategoryName='" + category + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable CheckDuplicateParentCat(string category)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Parent_Category_Master where ParentCategory='" + category + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable CheckDuplicateSubCat(string pid,string subcategory)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Product_Subcat_Master where ParentCategoryId=" + pid + " and SubCatName='"+subcategory+"'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable CheckMainCategorySector(int categoryId, int? SectorId)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_product_category_master pc join tbl_Vendor_Product_Assign vp on pc.id = vp.CategoryId where pc.ParentCategoryId = '" + categoryId + "' AND vp.SectorID='" + SectorId + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable CheckSubCategorySector(int categoryId, int? SectorId)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_product_category_master pc join tbl_Vendor_Product_Assign vp on pc.id = vp.CategoryId where pc.Id = '" + categoryId + "' AND vp.SectorID='" + SectorId + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public int GeCategoryIdByName(string category)
        {
            try
            {
                var sCategory = category.Split('|');
                if (!string.IsNullOrEmpty(sCategory[0]))
                {
                    SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Product_Category_Master where CategoryName='" + sCategory[0] + "' AND ParentCategoryId Is Null", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(sCategory[1]))
                        {
                            SqlDataAdapter da1 = new SqlDataAdapter("select * from tbl_Product_Category_Master where CategoryName='" + sCategory[1] + "' AND ParentCategoryId='" + dt.Rows[0]["Id"] + "'", con);
                            DataTable dt1 = new DataTable();
                            da1.Fill(dt1);
                            if (dt1.Rows.Count > 0)
                            {
                                return Convert.ToInt32(dt1.Rows[0]["Id"].ToString());
                            }
                        }
                        else
                            return Convert.ToInt32(dt.Rows[0]["Id"].ToString());
                    }
                }
            }
            catch { }
            return 0;
        }

        //delete
        public int DeleteCategory(int id)
        {
            DataTable dt = new DataTable();
            dt = _clsCommon.selectwhere("*", "tbl_Product_Category_Master", " ParentCategoryId ='" + id + "'");
            if (dt.Rows.Count > 0) { }
            else
            {
                dt = _clsCommon.selectwhere("*", "tbl_Product_Master", " CategoryId ='" + id + "'");
                if (dt.Rows.Count > 0) { }
                else
                {
                    _clsCommon.deletedata("tbl_Product_Category_Master", "Id='" + id + "'");
                    return 1;
                }
            }
            return 0;
            //con.Open();
            //SqlCommand cmd = new SqlCommand("Delete from tbl_Product_Category_Master where Id=" + id, con);
            //int i = cmd.ExecuteNonQuery();
            //con.Close();
            //return i;
        }

        //delete
        public int DeleteProduct(int id,int OrderBy)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Product_Master where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();


            SqlCommand cmd1 = new SqlCommand("UPDATE tbl_Product_Master set OrderBy=OrderBy -1 where Id<>" + id + " AND OrderBy>=" + OrderBy + "", con);


            cmd1.CommandType = CommandType.Text;
            int so1 = cmd1.ExecuteNonQuery();
            con.Close();
            return i;
        }
        public int DeleteProductVendor(int id, int orderBy)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM tbl_Vendor_Product_Assign WHERE Id = " + id, con);
            int result = cmd.ExecuteNonQuery();

            SqlCommand cmd1 = new SqlCommand("UPDATE tbl_Vendor_Product_Assign SET OrderBy = OrderBy - 1 WHERE Id <> " + id + " AND OrderBy >= " + orderBy, con);
            cmd1.CommandType = CommandType.Text;
            int affected = cmd1.ExecuteNonQuery();

            con.Close();

            return result;
        }

        public int DeleteProductFavourite(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Customer_Favourite where ProductId=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int DeleteProductOrderDetail(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Customer_Order_Detail where ProductId=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int DeleteFutureOrder(int orderid)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Customer_Order_Transaction where Id=" + orderid, con);
            int i = cmd.ExecuteNonQuery();


            SqlCommand cmd1 = new SqlCommand("Delete from tbl_Customer_Order_Detail where OrderId=" + orderid, con); 


          //  cmd1.CommandType = CommandType.Text;
            int so1 = cmd1.ExecuteNonQuery();
            con.Close();
            return i;
        }

            public int InsertProduct(Product obj)
            {
                int i = 0;
                try
                {
                    con.Open();
                    SqlCommand com = new SqlCommand("Product_Insert", con);
                    com.CommandType = CommandType.StoredProcedure;
                    if (!string.IsNullOrEmpty(obj.CategoryId.ToString()))
                        com.Parameters.AddWithValue("@CategoryId", obj.CategoryId);
                    else
                        com.Parameters.AddWithValue("@CategoryId", DBNull.Value);
                    if (!string.IsNullOrEmpty(obj.ProductName))
                        com.Parameters.AddWithValue("@ProductName", obj.ProductName);
                    else
                        com.Parameters.AddWithValue("@ProductName", DBNull.Value);
                    if (!string.IsNullOrEmpty(obj.Code))
                        com.Parameters.AddWithValue("@Code", obj.Code);
                    else
                        com.Parameters.AddWithValue("@Code", DBNull.Value);
                    if (!string.IsNullOrEmpty(obj.Price.ToString()))
                        com.Parameters.AddWithValue("@Price", obj.Price);
                    else
                        com.Parameters.AddWithValue("@Price", 0);
                    if (!string.IsNullOrEmpty(obj.DiscountAmount.ToString()))
                        com.Parameters.AddWithValue("@DiscountAmount", obj.DiscountAmount);
                    else
                        com.Parameters.AddWithValue("@DiscountAmount", 0);
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
                    if (!string.IsNullOrEmpty(obj.RewardPoint.ToString()))
                        com.Parameters.AddWithValue("@RewardPoint", obj.RewardPoint);
                    else
                        com.Parameters.AddWithValue("@RewardPoint", 0);
                    if (!string.IsNullOrEmpty(obj.Detail))
                        com.Parameters.AddWithValue("@Detail", obj.Detail);
                    else
                        com.Parameters.AddWithValue("@Detail", DBNull.Value);
                    if (!string.IsNullOrEmpty(obj.Image))
                        com.Parameters.AddWithValue("@Image", obj.Image);
                    else
                        com.Parameters.AddWithValue("@Image", DBNull.Value);

                    if (!string.IsNullOrEmpty(obj.PurchaseAmount.ToString()))
                        com.Parameters.AddWithValue("@PurchasePrice", obj.PurchaseAmount);
                    else
                        com.Parameters.AddWithValue("@PurchasePrice", 0);
                    if (!string.IsNullOrEmpty(obj.SaleAmount.ToString()))
                        com.Parameters.AddWithValue("@SalePrice", obj.SaleAmount);
                    else
                        com.Parameters.AddWithValue("@SalePrice", 0);
                    if (!string.IsNullOrEmpty(obj.Profit.ToString()))
                        com.Parameters.AddWithValue("@Profit", obj.Profit);
                    else
                        com.Parameters.AddWithValue("@Profit", 0);
                    if (!string.IsNullOrEmpty(obj.Subscription.ToString()))
                        com.Parameters.AddWithValue("@Subscription", obj.Subscription);
                    else
                        com.Parameters.AddWithValue("@Subscription", 0);
                    if (!string.IsNullOrEmpty(obj.OrderBy.ToString()))
                        com.Parameters.AddWithValue("@OrderBy", obj.OrderBy);
                    else
                        com.Parameters.AddWithValue("@OrderBy", 0);

                    if (!string.IsNullOrEmpty(obj.YoutubeTitle))
                        com.Parameters.AddWithValue("@YoutubeTitle", obj.YoutubeTitle);
                    else
                        com.Parameters.AddWithValue("@YoutubeTitle", DBNull.Value);
                    if (!string.IsNullOrEmpty(obj.YoutubeURL))
                        com.Parameters.AddWithValue("@YoutubeURL", obj.YoutubeURL);
                    else
                        com.Parameters.AddWithValue("@YoutubeURL", DBNull.Value);
                    com.Parameters.AddWithValue("@IsActive", obj.IsActive);
                    com.Parameters.AddWithValue("@IsDaily", obj.IsDaily);

                    com.Parameters.AddWithValue("@IsAlternate", obj.IsAlternate);
                    com.Parameters.AddWithValue("@IsMultiple", obj.IsMultipleDay);
                    com.Parameters.AddWithValue("@IsWeeklyDay", obj.IsWeeklyDay);

                    com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;

                
               

                    i = com.ExecuteNonQuery();
                    i = Convert.ToInt32(com.Parameters["@Id"].Value);

                    SqlCommand cmd1 = new SqlCommand("UPDATE tbl_Product_Master set OrderBy=OrderBy +1 where Id<>" + i + " AND OrderBy>="+ obj.OrderBy+"", con);


                    cmd1.CommandType = CommandType.Text;
                   int so1 = cmd1.ExecuteNonQuery();



                    con.Close();
                }
                catch (Exception ex)
                { }
                return i;

            }

        public DataTable ProductInTransaction(int pid)
        {
            DataTable dt = new DataTable();
            try
            {
                DateTime ToDate =Helper.indianTime;
               // ToDate = ToDate.AddDays(-50);
                con.Open();
                string q = "select o.ProductId,t.Id,convert(varchar,t.OrderDate,23) as OrderDate from tbl_Customer_Order_Transaction as t left join tbl_Customer_Order_Detail as o on o.OrderId=t.Id where o.ProductId=" + pid + " and t.Status='Pending' and CONVERT(VARCHAR,t.OrderDate,23)>=@ToDate";
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


        public DataTable ProductInTransaction1(int pid)
        {
            DataTable dt = new DataTable();
            try
            {
                DateTime ToDate = Helper.indianTime;
              // ToDate = ToDate.AddDays(-80);
                con.Open();
                string q = "select o.ProductId,t.Id,convert(varchar,t.OrderDate,23) as OrderDate,o.Qty from tbl_Customer_Order_Transaction as t left join tbl_Customer_Order_Detail as o on o.OrderId=t.Id where o.ProductId=" + pid + " and t.Status='Pending' and CONVERT(VARCHAR,t.OrderDate,23)>=@ToDate";
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


        public DataTable ProductInTransactiondatewise(int pid, DateTime? FDate)
        {
            DataTable dt = new DataTable();
            try
            {
                DateTime ToDate = Convert.ToDateTime(FDate);
                // ToDate = ToDate.AddDays(-80);
                con.Open();
                string q = "select o.ProductId,t.Id,convert(varchar,t.OrderDate,23) as OrderDate,o.Qty,t.Status,t.CustomerId from tbl_Customer_Order_Transaction as t left join tbl_Customer_Order_Detail as o on o.OrderId=t.Id where o.ProductId=" + pid + "  and CONVERT(VARCHAR,t.OrderDate,23)>=@ToDate";
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

        public int UpdateProduct(Product obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Product_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.Id);
                if (!string.IsNullOrEmpty(obj.CategoryId.ToString()))
                    com.Parameters.AddWithValue("@CategoryId", obj.CategoryId);
                else
                    com.Parameters.AddWithValue("@CategoryId", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.ProductName))
                    com.Parameters.AddWithValue("@ProductName", obj.ProductName);
                else
                    com.Parameters.AddWithValue("@ProductName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Code))
                    com.Parameters.AddWithValue("@Code", obj.Code);
                else
                    com.Parameters.AddWithValue("@Code", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Price.ToString()))
                    com.Parameters.AddWithValue("@Price", obj.Price);
                else
                    com.Parameters.AddWithValue("@Price", 0);
                if (!string.IsNullOrEmpty(obj.DiscountAmount.ToString()))
                    com.Parameters.AddWithValue("@DiscountAmount", obj.DiscountAmount);
                else
                    com.Parameters.AddWithValue("@DiscountAmount", 0);
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
                if (!string.IsNullOrEmpty(obj.RewardPoint.ToString()))
                    com.Parameters.AddWithValue("@RewardPoint", obj.RewardPoint);
                else
                    com.Parameters.AddWithValue("@RewardPoint", 0);
                if (!string.IsNullOrEmpty(obj.Detail))
                    com.Parameters.AddWithValue("@Detail", obj.Detail);
                else
                    com.Parameters.AddWithValue("@Detail", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.MainImage))
                    com.Parameters.AddWithValue("@Image", obj.MainImage);
                else
                    com.Parameters.AddWithValue("@Image", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.PurchaseAmount.ToString()))
                    com.Parameters.AddWithValue("@PurchasePrice", obj.PurchaseAmount);
                else
                    com.Parameters.AddWithValue("@PurchasePrice", 0);
                if (!string.IsNullOrEmpty(obj.SaleAmount.ToString()))
                    com.Parameters.AddWithValue("@SalePrice", obj.SaleAmount);
                else
                    com.Parameters.AddWithValue("@SalePrice", 0);
                if (!string.IsNullOrEmpty(obj.Profit.ToString()))
                    com.Parameters.AddWithValue("@Profit", obj.Profit);
                else
                    com.Parameters.AddWithValue("@Profit", 0);
                if (!string.IsNullOrEmpty(obj.Subscription.ToString()))
                    com.Parameters.AddWithValue("@Subscription", obj.Subscription);
                else
                    com.Parameters.AddWithValue("@Subscription", 0);
                if (!string.IsNullOrEmpty(obj.OrderBy.ToString()))
                    com.Parameters.AddWithValue("@OrderBy", obj.OrderBy);
                else
                    com.Parameters.AddWithValue("@OrderBy", 0);

                if (!string.IsNullOrEmpty(obj.YoutubeTitle))
                    com.Parameters.AddWithValue("@YoutubeTitle", obj.YoutubeTitle);
                else
                    com.Parameters.AddWithValue("@YoutubeTitle", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.YoutubeURL))
                    com.Parameters.AddWithValue("@YoutubeURL", obj.YoutubeURL);
                else
                    com.Parameters.AddWithValue("@YoutubeURL", DBNull.Value);

                com.Parameters.AddWithValue("@IsActive", obj.IsActive);
                com.Parameters.AddWithValue("@IsDaily", obj.IsDaily);

                com.Parameters.AddWithValue("@IsAlternate", obj.IsAlternate);
                com.Parameters.AddWithValue("@IsMultiple", obj.IsMultipleDay);
                com.Parameters.AddWithValue("@IsWeeklyDay", obj.IsWeeklyDay);
                i = com.ExecuteNonQuery();
                //  int Id = Convert.ToInt32(com.Parameters["@Id"].Value);
                con.Close();

                //SqlCommand cmd1 = new SqlCommand("SELECT od.OrderId as OrderId,od.ProductId as ProductId,od.Qty As Qty from tbl_Customer_Order_Detail od left join tbl_Customer_Order_Transaction ot ON  od.OrderId=ot.Id where (@Productid IS NULL OR od.ProductId=@Productid) and ot.Status='Pending' ", con);
                //cmd1.CommandType = CommandType.Text;

                //int pid = obj.Id;

                //if (!string.IsNullOrEmpty(pid.ToString()))
                //    cmd1.Parameters.AddWithValue("@Productid", pid);
                //else
                //    cmd1.Parameters.AddWithValue("@Productid", DBNull.Value);


                //SqlDataAdapter da = new SqlDataAdapter(cmd1);
                //DataTable dt = new DataTable();
                //da.Fill(dt);

                //if(dt.Rows.Count>0)
                //{
                //    int oid = 0, pid1 = 0, newprice =Convert.ToInt16(obj.SaleAmount.ToString()), newamount = 0,qty=0;
                //    for (int j=0;j<dt.Rows.Count-1;j++)
                //    {

                //        oid =Convert.ToInt16(dt.Rows[j].ItemArray[0].ToString());
                //        qty= Convert.ToInt16(dt.Rows[j].ItemArray[2].ToString());
                //        newamount = qty * newprice;




                //    }
                //}
            }
            catch (Exception ex)
            { }
            return i;

        }


        public int UpdatePriceFutureOrder(int orderid,double orderamount,double pamount,double mamount,double SalePrice)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("ProductOrder_Update1", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", orderid);
               
                if (!string.IsNullOrEmpty(orderamount.ToString()))
                    com.Parameters.AddWithValue("@TotalAmount", orderamount);
                else
                    com.Parameters.AddWithValue("@TotalAmount", 0);
                             

                
                i = com.ExecuteNonQuery();
                //  int Id = Convert.ToInt32(com.Parameters["@Id"].Value);

                SqlCommand com1 = new SqlCommand("ProductOrder_Update2", con);
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


                //if (!string.IsNullOrEmpty(pamount.ToString()))
                //    com1.Parameters.AddWithValue("@PurchasePrice", pamount);
                //else
                //    com1.Parameters.AddWithValue("@PurchasePrice", 0);


                //if (!string.IsNullOrEmpty(mamount.ToString()))
                //    com1.Parameters.AddWithValue("@Mrp", mamount);
                //else
                //    com1.Parameters.AddWithValue("@Mrp", 0);

                i = com1.ExecuteNonQuery();

                con.Close();

                
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int UpdateProductSelf(Product obj,int? OrderBy)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Product_Update_self", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.Id);
               
                if (!string.IsNullOrEmpty(obj.ProductName))
                    com.Parameters.AddWithValue("@ProductName", obj.ProductName);
                else
                    com.Parameters.AddWithValue("@ProductName", DBNull.Value);
                
                if (!string.IsNullOrEmpty(obj.Price.ToString()))
                    com.Parameters.AddWithValue("@Price", obj.Price);
                else
                    com.Parameters.AddWithValue("@Price", 0);
                if (!string.IsNullOrEmpty(obj.DiscountAmount.ToString()))
                    com.Parameters.AddWithValue("@DiscountAmount", obj.DiscountAmount);
                else
                    com.Parameters.AddWithValue("@DiscountAmount", 0);
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
               
              
                if (!string.IsNullOrEmpty(obj.Image))
                    com.Parameters.AddWithValue("@Image", obj.Image);
                else
                    com.Parameters.AddWithValue("@Image", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.PurchaseAmount.ToString()))
                    com.Parameters.AddWithValue("@PurchasePrice", obj.PurchaseAmount);
                else
                    com.Parameters.AddWithValue("@PurchasePrice", 0);
                if (!string.IsNullOrEmpty(obj.SaleAmount.ToString()))
                    com.Parameters.AddWithValue("@SalePrice", obj.SaleAmount);
                else
                    com.Parameters.AddWithValue("@SalePrice", 0);
                if (!string.IsNullOrEmpty(obj.Profit.ToString()))
                    com.Parameters.AddWithValue("@Profit", obj.Profit);
                else
                    com.Parameters.AddWithValue("@Profit", 0);
               
                if (!string.IsNullOrEmpty(obj.OrderBy.ToString()))
                    com.Parameters.AddWithValue("@OrderBy", obj.OrderBy);
                else
                    com.Parameters.AddWithValue("@OrderBy", 0);

             
               

                //com.Parameters.AddWithValue("@IsActive", obj.IsActive);
              
                i = com.ExecuteNonQuery();
                //  int Id = Convert.ToInt32(com.Parameters["@Id"].Value);

                //Initial Sort order case
                if(OrderBy==0)
                {
                    SqlCommand cmdnew = new SqlCommand("SELECT * From tbl_Product_Master p Order By OrderBy Asc", con);
                    cmdnew.CommandType = CommandType.Text;

                    SqlDataAdapter da = new SqlDataAdapter(cmdnew);
                    DataTable dt1 = new DataTable();
                    da.Fill(dt1);
                    int so1 = 0;
                    if(dt1.Rows.Count>0)
                    {
                        so1 = 0;
                        for (int j=0;j<dt1.Rows.Count;j++)
                        {
                            int k = j + 1;
                            SqlCommand cmd1 = new SqlCommand("UPDATE tbl_Product_Master set OrderBy="+k+" where Id="+dt1.Rows[j].ItemArray[0] +"", con);


                            cmd1.CommandType = CommandType.Text;
                            so1 = cmd1.ExecuteNonQuery();
                        }
                      
                    }
                }

                else
                {
                    if (OrderBy < obj.OrderBy)
                    {
                        //OrderBy=Old,obj.OrderBy=New

                        string j = OrderBy.ToString();

                        int k = Convert.ToInt32(j);
                        int so1 = 0;int so2=0;


                        for (int l = k; l <= obj.OrderBy; l++)
                        {

                            SqlCommand cmd1 = new SqlCommand("UPDATE tbl_Product_Master set OrderBy=OrderBy -1 where Id<>" + obj.Id + " AND OrderBy=" + l+ "", con);


                            cmd1.CommandType = CommandType.Text;
                            so1 = cmd1.ExecuteNonQuery();
                        }
                            


                        //SqlCommand cmd2 = new SqlCommand("UPDATE tbl_Product_Master set OrderBy="+OrderBy+" where Id<>" + obj.Id + " AND OrderBy=" + obj.OrderBy + "", con);


                        //cmd2.CommandType = CommandType.Text;
                        //so2 = cmd2.ExecuteNonQuery();
                    }
                    if (OrderBy>obj.OrderBy)
                    {
                        string j = OrderBy.ToString();

                        int k = Convert.ToInt32(j);
                        int so1 = 0; int so2 = 0;


                        for (int l = k; l >= obj.OrderBy; l--)
                        {

                            SqlCommand cmd1 = new SqlCommand("UPDATE tbl_Product_Master set OrderBy=OrderBy +1 where Id<>" + obj.Id + " AND OrderBy=" + l + "", con);


                            cmd1.CommandType = CommandType.Text;
                            so1 = cmd1.ExecuteNonQuery();
                        }
                    }

                   
                }

                con.Close();

                
            }
            catch (Exception ex)
            { }
            return i;

        }

        public DataTable CheckDuplicateProduct(int cid, string product)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Product_Master where CategoryId='" + cid + "' and ProductName='" + product + "' and IsActive='true'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable SearchCategProuct(string search)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Search_Category_Product_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(search.ToString()))
                cmd.Parameters.AddWithValue("@search", search);
            else
                cmd.Parameters.AddWithValue("@search", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable SearchSectorCategProuct(string sectorid, string search)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Sector_Search_Category_Product_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(sectorid))
                cmd.Parameters.AddWithValue("@SectorId", sectorid);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(search))
                cmd.Parameters.AddWithValue("@search", search);
            else
                cmd.Parameters.AddWithValue("@search", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable ProductIdByName(string product)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Product_Master where REPLACE(ProductName, ' ', '') = REPLACE('" + product + "', ' ', '') and IsActive=1", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public int updateActiveProductCatgStatus(string pid)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("update tbl_Product_Category_Master set IsActive='1' where Id='" + pid + "'  ", con);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }

        public int updateInActiveProductCatgStatus(string pid)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("update tbl_Product_Category_Master set IsActive='0' where Id='" + pid + "'  ", con);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }

        public int updateActiveProductStatus(string pid)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("update tbl_Product_Master set IsActive='1' where Id='" + pid + "'  ", con);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }

        public int updateInActiveProductStatus(string pid)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("update tbl_Product_Master set IsActive='0' where Id='" + pid + "'  ", con);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }

        public int UpdateCategoryInProduct(string pid, string cid,string pcat,string pccat)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("update tbl_Product_Master set CategoryId='" + pcat + "',SubcatId='"+pccat+"' where Id='" + pid + "'  ", con);
            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;
        }

        //get total Pending order from Product Id
        public DataTable GetProductWiseOrder(int ProductId)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from [dbo].[tbl_Customer_Order_Transaction] ot left join[dbo].[tbl_Customer_Order_Detail] od on od.[OrderId] = ot.Id " +
            " where ot.[Status] = 'pending'  and od.[ProductId] = '" + ProductId + "' Order by ot.Orderdate ASC", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable GetCategoryById(int? CategoryId)
        {
            DataTable dt = new DataTable();
            dt = _clsCommon.selectwhere("*", "tbl_Product_Category_Master", " Id ='" + CategoryId + "'");
            return dt;
        }

        public DataTable GetCategoryByIdnew(int? ProductId,int? VendorcatId)
        {
           


            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_VendorCat where Id="+VendorcatId+"", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public class TimmingModel
        {
            public Boolean IsTime { get; set; }
            public string message { get; set; }
        }

        public string GetCategoryTiming(int CategoryId)
        {
            string str = "";
            var category = GetCategoryById(CategoryId);
            if (category.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(category.Rows[0]["ParentCategoryId"].ToString()))
                {
                    var mainCategory = GetCategoryById(Convert.ToInt32(category.Rows[0]["ParentCategoryId"]));
                    if (mainCategory.Rows.Count > 0)
                    {
                        str += "<b>Min. Amount</b> :- " + mainCategory.Rows[0]["MinAmount"];
                        str += "<br /><b>Max. Amount</b> :- " + mainCategory.Rows[0]["MaxAmount"];
                        str += "<br /><b>Order Time</b> :- " + mainCategory.Rows[0]["FromTime"] + "-" + mainCategory.Rows[0]["ToTime"];
                        str += "<br /><b>Delivery Time</b> :- " + mainCategory.Rows[0]["DeliveryFrom"] + "-" + mainCategory.Rows[0]["DeliveryTo"];
                    }
                }
                else
                {
                    str += "<b>Min. Amount</b> :- " + category.Rows[0]["MinAmount"];
                    str += "<br /><b>Max. Amount</b> :- " + category.Rows[0]["MaxAmount"];
                    str += "<br /><b>Order Time</b> :- " + category.Rows[0]["FromTime"] + "-" + category.Rows[0]["ToTime"];
                    str += "<br /><b>Delivery Time</b> :- " + category.Rows[0]["DeliveryFrom"] + "-" + category.Rows[0]["DeliveryTo"];
                }
            }
            return str;
        }

        public TimmingModel CheckProductOrderTimimg(int ProductId)
        {
            TimmingModel timming = new TimmingModel();
            timming.IsTime = false;
            try
            {
                DateTime centuryBegin = new DateTime(2001, 1, 1);
                var currentDate = Helper.indianTime;
                long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
                TimeSpan elapsedSpan = TimeSpan.Parse(currentDate.ToString("HH:mm"));

                var product = BindProuct(ProductId);
                if (product.Rows.Count > 0)
                {
                    var category = GetCategoryById(Convert.ToInt32(product.Rows[0]["CategoryId"]));
                    if (category.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(category.Rows[0]["ParentCategoryId"].ToString()))
                        {
                            var mainCategory = GetCategoryById(Convert.ToInt32(category.Rows[0]["ParentCategoryId"]));
                            if (mainCategory.Rows.Count > 0)
                            {
                                TimeSpan fromTime = TimeSpan.Parse(mainCategory.Rows[0]["FromTime"].ToString());
                                TimeSpan toTime = TimeSpan.Parse(mainCategory.Rows[0]["ToTime"].ToString());
                                if (fromTime < elapsedSpan && toTime > elapsedSpan)
                                    timming.IsTime = true;
                                else
                                    timming.message = "You can place order between " + fromTime + " to " + toTime;
                            }
                        }
                        else
                        {
                            TimeSpan fromTime = TimeSpan.Parse(category.Rows[0]["FromTime"].ToString());
                            TimeSpan toTime = TimeSpan.Parse(category.Rows[0]["ToTime"].ToString());
                            if (fromTime < elapsedSpan && toTime > elapsedSpan)
                                timming.IsTime = true;
                            else
                                timming.message = "You can place order between " + fromTime + " to " + toTime;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return timming;
        }



        public TimmingModel CheckProductOrderTimimgNew(int ProductId,int VendorcatId)
        {
            TimmingModel timming = new TimmingModel();
            timming.IsTime = false;
            try
            {
                DateTime centuryBegin = new DateTime(2001, 1, 1);
                var currentDate = Helper.indianTime;
                long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
                TimeSpan elapsedSpan = TimeSpan.Parse(currentDate.ToString("HH:mm"));

               
               
                    
                        
                            var mainCategory = GetCategoryByIdnew(ProductId,VendorcatId);
                            if (mainCategory.Rows.Count > 0)
                            {
                                TimeSpan fromTime = TimeSpan.Parse(mainCategory.Rows[0]["FromTime"].ToString());
                                TimeSpan toTime = TimeSpan.Parse(mainCategory.Rows[0]["ToTime"].ToString());
                                if (fromTime < elapsedSpan && toTime > elapsedSpan)
                                    timming.IsTime = true;
                                else
                                    timming.message = "You can place order between " + fromTime + " to " + toTime;
                            }
                        
                     
                  
              
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return timming;
        }

        public class OrderAmountModel
        {
            public Boolean IsOrderAmount { get; set; }
            public string message { get; set; }
        }

        public decimal GetProductOrderAmount(int ProductId)
        {
            try
            {
                var product = BindProuct(ProductId);
                if (product.Rows.Count > 0)
                {
                    var category = GetCategoryById(Convert.ToInt32(product.Rows[0]["CategoryId"]));
                    if (category.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(category.Rows[0]["ParentCategoryId"].ToString()))
                        {
                            var mainCategory = GetCategoryById(Convert.ToInt32(category.Rows[0]["ParentCategoryId"]));
                            if (mainCategory.Rows.Count > 0)
                            {
                                Decimal minAmount = Convert.ToDecimal(mainCategory.Rows[0]["MinAmount"].ToString());
                                Decimal maxAmount = Convert.ToDecimal(mainCategory.Rows[0]["MaxAmount"].ToString());
                                return minAmount;
                            }
                        }
                        else
                        {
                            Decimal minAmount = Convert.ToDecimal(category.Rows[0]["MinAmount"].ToString());
                            Decimal maxAmount = Convert.ToDecimal(category.Rows[0]["MaxAmount"].ToString());
                            return minAmount;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return 0;
        }

        public OrderAmountModel CheckProductOrderAmount(int ProductId, decimal Amount)
        {
            OrderAmountModel model = new OrderAmountModel();
            model.IsOrderAmount = false;

            try
            {
                var product = BindProuct(ProductId);
                if (product.Rows.Count > 0)
                {
                    var category = GetCategoryById(Convert.ToInt32(product.Rows[0]["CategoryId"]));
                    if (category.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(category.Rows[0]["ParentCategoryId"].ToString()))
                        {
                            var mainCategory = GetCategoryById(Convert.ToInt32(category.Rows[0]["ParentCategoryId"]));
                            if (mainCategory.Rows.Count > 0)
                            {
                                Decimal minAmount = Convert.ToDecimal(mainCategory.Rows[0]["MinAmount"].ToString());
                                Decimal maxAmount = Convert.ToDecimal(mainCategory.Rows[0]["MaxAmount"].ToString());
                                if (minAmount < Amount && maxAmount > Amount)
                                    model.IsOrderAmount = true;
                                else
                                    model.message = "Order place between Min. Amount " + minAmount + " Max. Amount " + maxAmount;
                            }
                        }
                        else
                        {
                            Decimal minAmount = Convert.ToDecimal(category.Rows[0]["MinAmount"].ToString());
                            Decimal maxAmount = Convert.ToDecimal(category.Rows[0]["MaxAmount"].ToString());
                            if (minAmount < Amount && maxAmount > Amount)
                                model.IsOrderAmount = true;
                            else
                                model.message = "Order place between Min. Amount " + minAmount + " Max. Amount " + maxAmount;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return model;
        }

        public DataTable GetProductAssignAttribute(int? ProductID)
        {
            con.Open();
            //SqlCommand cmd = new SqlCommand("SP_GetProduct_Assign_Attribute", con);
            SqlCommand cmd = new SqlCommand("SP_GetProduct_Assign_Attributenew", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(ProductID.ToString()))
                cmd.Parameters.AddWithValue("@ProductID", ProductID);
            else
                cmd.Parameters.AddWithValue("@ProductID", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
        public DataTable GetProductAssignAttributeVendorsector(int? VendorId,int? ProductId,int? AttributeId,string Profit)
        {
            con.Open();
            string query = "SELECT vpa.Id,vpa.ProductId,sm.SectorName,vpa.SectorId ";
            query += " FROM tbl_product_attributes pa";
            query += " JOIN tbl_Vendor_Product_Assign vpa ON pa.VendorId=vpa.VendorId and pa.VendorCatId=vpa.VendorCatId and pa.AttributeID=vpa.AttributeId and pa.ProductID=vpa.ProductId";
            query += " JOIN tbl_Product_Master pm ON vpa.ProductId =pm.id ";
            query += " join tbl_Vendor_Master Vm On Vm.Id=vpa.VendorId";
            query += " JOIN tbl_Attributes am ON pa.AttributeID=am.ID JOIN tbl_Sector_Master sm ON vpa.SectorId=sm.id";
            query += " WHERE pa.IsDeleted='false' AND (@ProductID IS NULL OR vpa.ProductId = @ProductID)";
            query += " AND vpa.AttributeId=@AttributeId And vpa.VendorId=@VendorId ";

            //SqlCommand cmd = new SqlCommand("SP_GetProduct_Assign_Attribute", con);AND pa.Profit=@Profit
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(ProductId.ToString()))
                cmd.Parameters.AddWithValue("@ProductID", ProductId);
            else
                cmd.Parameters.AddWithValue("@ProductID", DBNull.Value);


            //

            if (!string.IsNullOrEmpty(AttributeId.ToString()))
                cmd.Parameters.AddWithValue("@AttributeId", AttributeId);
            else
                cmd.Parameters.AddWithValue("@AttributeId", DBNull.Value);

            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);

            //if (!string.IsNullOrEmpty(Profit.ToString()))
            //    cmd.Parameters.AddWithValue("@Profit", Profit);
            //else
            //    cmd.Parameters.AddWithValue("@Profit", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        //public DataTable BindParentCategory(int? Id)
        //{
        //    SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_Parent_Category_Master WHERE (@Id IS NULL OR [Id] = @Id) Order By OrderBy ASc", con);
        //    cmd.CommandType = CommandType.Text;
        //    if (!string.IsNullOrEmpty(Id.ToString()))
        //        cmd.Parameters.AddWithValue("@Id", Id);
        //    else
        //        cmd.Parameters.AddWithValue("@Id", DBNull.Value);
        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    DataTable dt = new DataTable();
        //    da.Fill(dt);
        //    con.Close();
        //    return dt;
        //}

        public DataTable BindSubCategory(int? Id)
        {
            SqlCommand cmd = new SqlCommand("Select Ps.*,Pc.ParentCategory from tbl_Product_Subcat_Master Ps Inner Join tbl_Parent_Category_Master Pc On Ps.ParentCategoryId=Pc.Id WHERE (@Id IS NULL OR Ps.Id = @Id) Order By Ps.OrderBy ASc", con);
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


        public DataTable BindSubCategory1(int? Id)
        {
            SqlCommand cmd = new SqlCommand("Select * from tbl_Product_Subcat_Master WHERE (@Id IS NULL OR ParentCategoryId = @Id) Order By OrderBy ASc", con);
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


        public DataTable BindSubCategoryVendorWise(int? Id,int? VendorId)
        {
            SqlCommand cmd = new SqlCommand("Select Pm.*,Vca.VendorCatName,Vca.IsActive as IsActive1 from tbl_Product_Subcat_Master Pm left join tbl_Vendor_CatSubcat_Assign Vca On Pm.Id=Vca.Subcat and (@Vendor Is Null Or Vca.VendorId=@Vendor) WHERE (@Id IS NULL OR ParentCategoryId = @Id) Order By OrderBy ASc", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);


            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@Vendor", VendorId);
            else
                cmd.Parameters.AddWithValue("@Vendor", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
        public DataTable BindAttributes()
        {
            SqlCommand cmd = new SqlCommand("Select * from tbl_Attributes", con);
            cmd.CommandType = CommandType.Text;
           
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable GetAllParentcategory()
        {
            DataTable dt = new DataTable();
            dt = _clsCommon.select("*", "tbl_Parent_Category_Master");
            return dt;
        }


        public int UpdateProdtParentcategory(Product obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Update tbl_Parent_Category_Master SET ParentCategory=@CategoryName,Image=@Image,OrderBy=@OrderBy,IsActive=@IsActive WHERE Id = @Id", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", obj.Id);
              
                if (!string.IsNullOrEmpty(obj.Category))
                    com.Parameters.AddWithValue("@CategoryName", obj.Category);
                else
                    com.Parameters.AddWithValue("@CategoryName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.IsActive.ToString()))
                    com.Parameters.AddWithValue("@IsActive", obj.IsActive);
                else
                    com.Parameters.AddWithValue("@IsActive", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Image))
                    com.Parameters.AddWithValue("@Image", obj.Image);
                else
                    com.Parameters.AddWithValue("@Image", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.OrderBy.ToString()))
                    com.Parameters.AddWithValue("@OrderBy", obj.OrderBy);
                else
                    com.Parameters.AddWithValue("@OrderBy", 0);

               
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int UpdateProdtSubcategory(Product obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Update tbl_Product_Subcat_Master SET ParentCategoryId=@ParentCategoryId,SubCatName=@SubCatName,Image=@Image,OrderBy=@OrderBy,IsActive=@IsActive WHERE Id = @Id", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", obj.Id);
                if (!string.IsNullOrEmpty(obj.CategoryId.ToString()))
                    com.Parameters.AddWithValue("@ParentCategoryId", obj.CategoryId);
                else
                    com.Parameters.AddWithValue("@ParentCategoryId", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.SubCategory))
                    com.Parameters.AddWithValue("@SubCatName", obj.SubCategory);
                else
                    com.Parameters.AddWithValue("@SubCatName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.IsActive.ToString()))
                    com.Parameters.AddWithValue("@IsActive", obj.IsActive);
                else
                    com.Parameters.AddWithValue("@IsActive", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Image))
                    com.Parameters.AddWithValue("@Image", obj.Image);
                else
                    com.Parameters.AddWithValue("@Image", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.OrderBy.ToString()))
                    com.Parameters.AddWithValue("@OrderBy", obj.OrderBy);
                else
                    com.Parameters.AddWithValue("@OrderBy", 0);


                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int DeleteProductParentcat(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Parent_Category_Master where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }


        public int DeleteProductSubcat(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Product_Subcat_Master where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }


        public DataTable BindProuctPriceChangeList(int? Id)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            string query = "SELECT Pr.*,Pm.ProductName,At.Name,convert(varchar,Pr.UpdatedOn,23) As Updated  ";
            query += " FROM [dbo].[tbl_product_PriceList] Pr inner Join tbl_Product_Master Pm On Pr.ProductId=Pm.Id inner join tbl_Attributes At On Pr.AttributeId=At.Id ";
            query += " where (@ProductId is null or Pr.ProductId=@ProductId) ";

            //SqlCommand cmd = new SqlCommand("Product_SelectAll", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@ProductId", Id);
            else
                cmd.Parameters.AddWithValue("@ProductId", DBNull.Value);

           
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public DataTable BindProuctForwardnewversion(int? Id,int? AttributeId)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();


            SqlCommand cmd = new SqlCommand("Product_SelectAll_Forwardnewversion", con);

            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);

            if (!string.IsNullOrEmpty(AttributeId.ToString()))
                cmd.Parameters.AddWithValue("@AttributeId", AttributeId);
            else
                cmd.Parameters.AddWithValue("@AttributeId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public DataTable getItemwiseCartdetail(int? CustomerId, int? VendorId, int? VendorCatId, int? productID, int? AttributeId)
        {
            //con.Open();
            string query = "SELECT Id,Qty,TotalFinalAmount,Convert(varchar,CartDate,23) As CartDate";
            query += " FROM [dbo].[tbl_Cart] ";
            

            query += " Where (@CustomerId is null Or CustomerId=@CustomerId) And Status='Cart' And";
            query += " (@VendorId is null Or VendorId=@VendorId) And (@VendorCatId is null Or VendorCatId=@VendorCatId)";
            query += " And (@ProductId is null Or ProductId=@ProductId) And (@AttributeId is null Or AttributeId=@AttributeId)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);


            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);

            if (!string.IsNullOrEmpty(VendorCatId.ToString()))
                cmd.Parameters.AddWithValue("@VendorCatId", VendorCatId);
            else
                cmd.Parameters.AddWithValue("@VendorCatId", DBNull.Value);

            if (!string.IsNullOrEmpty(productID.ToString()))
                cmd.Parameters.AddWithValue("@ProductId", productID);
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

        public DataTable BindAttributewiseProuctNew(string SectorId, string ProductId, string AttributeId, string VendorId, string VendorCatId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Sector_Attributewise_Product_SelectAll_New", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(SectorId))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(ProductId))
                cmd.Parameters.AddWithValue("@ProductId", ProductId);
            else
                cmd.Parameters.AddWithValue("@ProductId", DBNull.Value);

            if (!string.IsNullOrEmpty(AttributeId))
                cmd.Parameters.AddWithValue("@AttributeId", AttributeId);
            else
                cmd.Parameters.AddWithValue("@AttributeId", DBNull.Value);


            if (!string.IsNullOrEmpty(VendorId))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@VendorId", DBNull.Value);

            if (!string.IsNullOrEmpty(VendorCatId))
                cmd.Parameters.AddWithValue("@VendorCatId", VendorCatId);
            else
                cmd.Parameters.AddWithValue("@VendorCatId", DBNull.Value);

            //cmd.Parameters.AddWithValue("@PageNum", pno);
            //cmd.Parameters.AddWithValue("@PageSize", psize);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public DataTable getCartdetail(int? CartId)
        {
            //con.Open();
            string query = "SELECT *";
            query += " FROM [dbo].[tbl_Cart] ";


            query += " Where (@CartId is null Or Id=@CartId) And Status='Cart'";
           
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(CartId.ToString()))
                cmd.Parameters.AddWithValue("@CartId", CartId);
            else
                cmd.Parameters.AddWithValue("@CartId", DBNull.Value);


           
            

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }



        public DataTable getCustomerOrderIdwiseList(int? CustomerId, DateTime? orderDate,int? OrderId)
        {
            //con.Open();
            DateTime OrderDate = orderDate.Value;
            
            var date = OrderDate.Date;
            string query = "select ot.Id AS OId,cm.FirstName +' '+ cm.LastName as Customer,";
            query += "convert(varchar,ot.OrderDate,23) as OrderDate,CustomerId,";
            query += "ot.OrderNo,od.Amount as OAmt,od.RewardPOint as OPoint,isnull(od.Profit,0) as OProfit,ot.[Status],";
            query += "(od.SalePrice*od.Qty)  as Amount,(od.RewardPoint*od.Qty) as RewardPoint,isnull((od.Profit*od.Qty),0) as Profit,";
            query += "od.Qty,pm.ProductName,At.Name As AttributeName";
            query += " from  [dbo].[tbl_Customer_Order_Transaction] ot";
            query += " left join [dbo].[tbl_Customer_Order_Detail] od on od.OrderId = ot.Id";
            query += " left join [dbo].[tbl_Product_Master] pm on pm.Id = od.ProductId";
            query += " left join tbl_Product_Attributes Pat On od.AttributeId=pat.ID";
            query += " inner join tbl_Attributes At On od.AttributeId=At.ID";
            query += " left join [dbo].[tbl_Customer_Master] cm on cm.Id = ot.CustomerId";
            query += " where (@OrderDate is null or convert(varchar,ot.OrderDate,23) = @OrderDate)";
            query += "  and (ot.[Status] ='Complete') and (@CustomerId is null or ot.CustomerId=@CustomerId)";
            query += " AND od.Amount Is Not NULL And (@OrderId Is Null Or ot.Id=@OrderId)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(date.ToString()))
                cmd.Parameters.AddWithValue("@OrderDate", date);
            else
                cmd.Parameters.AddWithValue("@OrderDate", DBNull.Value);
            if (!string.IsNullOrEmpty(OrderId.ToString()))
                cmd.Parameters.AddWithValue("@OrderId", OrderId);
            else
                cmd.Parameters.AddWithValue("@OrderId", DBNull.Value);
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
    }
}
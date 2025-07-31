using MilkWayIndia.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MilkWayIndia.Controllers
{
    public class ProductController : Controller
    {
        Product objProdt = new Product();
        clsCommon _clsCommon = new clsCommon();
        Helper dHelper = new Helper();
        public string msg = "";
        // GET: Product
        [HttpGet]
        public ActionResult AddProduct()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            //if (HttpContext.Session["UserId"] == null)
            //    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            DataTable dtcategory = new DataTable();
            dtcategory = objProdt.GetAllMaincategory();
            ViewBag.Category = dtcategory;

            return View();
        }

        // GET: Product
        [HttpGet]
        public ActionResult AddProductVendor()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            //if (control.IsView == false)
            //    return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            // Fetch product names and set to ViewBag.ProductList
            DataTable dtProductNames = objProdt.GetAllProductNames();
            ViewBag.ProductList = dtProductNames;

			DataTable dtcategory = new DataTable();
			dtcategory = objProdt.GetAllMaincategory();


			ViewBag.Category = dtcategory;

			return View();
        }


        [HttpGet]
        public ActionResult AddProductNew()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            //if (HttpContext.Session["UserId"] == null)
            //    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            DataTable dtcategory = new DataTable();
            dtcategory = objProdt.GetAllMaincategorynew();
            ViewBag.Category = dtcategory;

            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(Product objProdt, FormCollection form, HttpPostedFileBase Document1, HttpPostedFileBase[] photos)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            //if (HttpContext.Session["UserId"] == null)
            //    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            string category = Request["ddlSubCategory"];
            if (category != null)
                objProdt.CategoryId = Convert.ToInt32(category);
            else
            {
                category = Request["ddlCategory"];
                objProdt.CategoryId = Convert.ToInt32(category);
            }

            string active = Request["IsActive"].Split(',')[0];
            if (!string.IsNullOrEmpty(active))
            {
                objProdt.IsActive = Convert.ToBoolean(active);
            }


            string alternate = Request["IsAlternate"].Split(',')[0];
            if (!string.IsNullOrEmpty(alternate))
            {
                objProdt.IsAlternate = Convert.ToBoolean(alternate);
            }


            string multipleday = Request["IsMultiple"].Split(',')[0];
            if (!string.IsNullOrEmpty(multipleday))
            {
                objProdt.IsMultipleDay = Convert.ToBoolean(multipleday);
            }

            string WeekDay = Request["IsWeekDay"].Split(',')[0];
            if (!string.IsNullOrEmpty(WeekDay))
            {
                objProdt.IsWeeklyDay = Convert.ToBoolean(WeekDay);
            }
            //check data duplicate
            DataTable dtDupliProduct = new DataTable();
            dtDupliProduct = objProdt.CheckDuplicateProduct(objProdt.CategoryId, objProdt.ProductName);
            if (dtDupliProduct.Rows.Count > 100)
            {
                ViewBag.SuccessMsg = "Product Already Exits!!!";
            }
            else
            {
                string fname = null, path = null;
                HttpPostedFileBase document1 = Request.Files["Document1"];
                string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png", ".PNG" };
                if (document1 != null)
                {
                    if (document1.ContentLength > 0)
                    {
                        try
                        {
                            HttpFileCollectionBase files = Request.Files;
                            HttpPostedFileBase file = Document1;
                            //Resize image 500*300 coding
                            WebImage img = new WebImage(file.InputStream);
                            img.Resize(300, 300, false, false);
                            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            string extension = Path.GetExtension(file.FileName);
                            if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                            {
                                string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                fname = testfiles[testfiles.Length - 1];
                            }
                            else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.ToLower().LastIndexOf('.'))))
                            {
                                ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
                            }
                            else
                            {
                                fileName = dHelper.RemoveIllegalCharacters(fileName);
                                fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                            }
                            path = Path.Combine(Server.MapPath("~/image/product/"), fname);
                            img.Save(path);
                            //file.SaveAs(path);
                            objProdt.Image = fname;
                        }

                        catch (Exception ex)
                        {
                            ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                        }
                    }
                }
				

				//int addresult = 0;
				int addresult = objProdt.InsertProduct(objProdt);

                foreach (HttpPostedFileBase file in photos)
                {
                    if (file != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        fileName = dHelper.RemoveIllegalCharacters(fileName);
                        string _ext = Path.GetExtension(file.FileName);
                        fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + _ext;

                        var ServerSavePath = Path.Combine(Server.MapPath("~/image/product/") + fname);
                        file.SaveAs(ServerSavePath);
						_clsCommon.updatedata("tbl_Product_Master", "MainImage='" + fname + "'", "Id=" + addresult);
					}
                }

                if (addresult > 0)
                { ViewBag.SuccessMsg = "Product Inserted Successfully!!!"; }
                else
                { ViewBag.SuccessMsg = "Product Not Inserted!!!"; }
            }
            ModelState.Clear();
            DataTable dtcategory = new DataTable();
            dtcategory = objProdt.GetAllMaincategory();
            ViewBag.Category = dtcategory;
            return View();
        }



        [HttpPost]
        public ActionResult AddProductNew(Product objProdt, FormCollection form, HttpPostedFileBase Document1, HttpPostedFileBase[] photos, string[] chkSubcat)
        {
            if (chkSubcat != null)
            {
                foreach (var item in chkSubcat)
                {

                    if (!string.IsNullOrEmpty(item))
                    {

                        string item1 = item.ToString();
                    }
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddProductVendorNew(Product objProdt, FormCollection form)
        {
            int i = 0;
            try
            {
                if (Request.Cookies["gstusr"] == null)
                    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
                string cookieValue = Request.Cookies["gstusr"].Value;
                string numberOnly = new string(cookieValue.Where(char.IsDigit).ToArray());
                int vendorId = 0;
                int categoryId = 0;

                if (!int.TryParse(numberOnly, out vendorId))
                    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString))
                {
                    con.Open();
                    SqlCommand checkCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM tbl_Vendor_Product_Assign 
                    WHERE VendorId = @VendorId AND ProductId = @ProductId", con);
                    checkCmd.Parameters.AddWithValue("@VendorId", vendorId);
                    checkCmd.Parameters.AddWithValue("@ProductId", objProdt.Id);

                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (exists > 0)
                    {
                        ViewBag.ErrorMsg = "This product is already added for the current vendor.";
                        DataTable dtProductNames = objProdt.GetAllProductNames();
                        ViewBag.ProductList = dtProductNames;
                        return View("AddProductVendor"); 
                    }

                    SqlCommand com = new SqlCommand(@"
                INSERT INTO tbl_Vendor_Product_Assign
                (SectorId,VendorId,CategoryId,ProductId,IsActive,MRPPrice,DiscountPrice,CGST,SGST,IGST,Profit,SellPrice,RewardPoint,IsDaily, IsAlternate, IsMultiple, IsWeekDay,OrderBy,PurchasePrice,Subscription,SubCategoryId)
                VALUES
                (@SectorId,@VendorId,@CategoryId,@ProductId,@IsActive,@MRPPrice,@DiscountPrice,@CGST,@SGST,@IGST,@Profit,@SellPrice,@RewardPoint,@IsDaily, @IsAlternate, @IsMultiple, @IsWeekDay,@OrderBy,@PurchasePrice,@Subscription,@SubCategoryId)", con);
                //(ProductId, VendorId, Price, DiscountAmount, CGST, SGST, IGST, RewardPoint,
                // IsActive, IsDaily, IsAlternate, IsMultiple, IsWeekDay,
                // PurchasePrice, SalePrice, Profit, OrderBy)
                //VALUES
                //(@ProductId, @VendorId, @Price, @DiscountAmount, @CGST, @SGST, @IGST, @RewardPoint,
                // @IsActive, @IsDaily, @IsAlternate, @IsMultiple, @IsWeekDay,
                // @PurchasePrice, @SalePrice, @Profit, @OrderBy, @Subscription)", con);

                    com.Parameters.AddWithValue("@SectorId", Convert.ToInt32(Session["VendorSectorId"]));
					com.Parameters.AddWithValue("@VendorId", vendorId);
					com.Parameters.AddWithValue("@CategoryId", (object)objProdt.ParentCategoryId ?? DBNull.Value);
					com.Parameters.AddWithValue("@SubCategoryId", (object)objProdt.CategoryId ?? DBNull.Value);
					com.Parameters.AddWithValue("@ProductId", (object)objProdt.Id ?? DBNull.Value);
					com.Parameters.AddWithValue("@IsActive", form["IsActive"]?.Contains("true") == true);					
                    com.Parameters.AddWithValue("@MRPPrice", (object)objProdt.Price ?? DBNull.Value);
                    com.Parameters.AddWithValue("@DiscountPrice", (object)objProdt.DiscountAmount ?? DBNull.Value);
                    com.Parameters.AddWithValue("@CGST", (object)objProdt.CGST ?? DBNull.Value);
					com.Parameters.AddWithValue("@Subscription", (object)objProdt.SGST ?? DBNull.Value);
					com.Parameters.AddWithValue("@SGST", (object)objProdt.SGST ?? DBNull.Value);
                    com.Parameters.AddWithValue("@IGST", (object)objProdt.IGST ?? DBNull.Value);
					com.Parameters.AddWithValue("@Profit", (object)objProdt.Profit ?? DBNull.Value);
					com.Parameters.AddWithValue("@SellPrice", (object)objProdt.SaleAmount ?? DBNull.Value);
					com.Parameters.AddWithValue("@RewardPoint", (object)objProdt.RewardPoint ?? DBNull.Value);
                    com.Parameters.AddWithValue("@IsDaily", form["IsDaily"]?.Contains("true") == true);
                    com.Parameters.AddWithValue("@IsAlternate", form["IsAlternate"]?.Contains("true") == true);
                    com.Parameters.AddWithValue("@IsMultiple", form["IsMultiple"]?.Contains("true") == true);
                    com.Parameters.AddWithValue("@IsWeekDay", form["IsWeekDay"]?.Contains("true") == true);
                    com.Parameters.AddWithValue("@PurchasePrice", (object)objProdt.PurchaseAmount ?? DBNull.Value);
                    com.Parameters.AddWithValue("@OrderBy", (object)objProdt.OrderBy ?? DBNull.Value);

                    i = com.ExecuteNonQuery();
                    con.Close();
                }

                ViewBag.SuccessMsg = "Product vendor saved successfully.";
                return RedirectToAction("ProductVendorList");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Error occurred: " + ex.Message;
                DataTable dtProductNames = objProdt.GetAllProductNames();
                ViewBag.ProductList = dtProductNames;

                return View("AddProductVendor");
            }
        }

        [HttpGet]
        public ActionResult ProductList()
        {
            //if (Request.Cookies["gstusr"] == null)
            //    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            if (HttpContext.Session["Msg"].ToString() != "" && HttpContext.Session["Msg"].ToString() != null)
            {
                ViewBag.SuccessMsg = HttpContext.Session["Msg"].ToString();
                Session["Msg"] = "";
            }
            else
            {
                ViewBag.SuccessMsg = "";
            }
            DataTable dt = new DataTable();
            dt = objProdt.BindProuctnew();
            int userRecords = 0;
            userRecords = dt.Rows.Count;
            ViewBag.previousid = "0";

            ViewBag.nextid = userRecords.ToString();

            //ViewBag.nextid = dtprodRecord.Rows[dtprodRecord.Rows.Count - 1].ItemArray[0].ToString();

           
           
            ViewBag.startpoint = "1";
            ViewBag.endpoint = userRecords.ToString();
            ViewBag.ProductList = dt;

            DataTable dtcategory = new DataTable();
            dtcategory = objProdt.GetAllMaincategory();
            ViewBag.Category = dtcategory;
            ViewBag.RarURL = Request.Url.ToString();
            if (Request.Url.ToString().Contains("portal") || Request.Url.ToString().Contains("localhost:4937"))
                ViewBag.IsAttribute = true;

            return View();
        }
        [HttpGet]
        public ActionResult ProductVendorList()
        {
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var control = Helper.CheckPermission(Request.RawUrl.ToString());

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            if (HttpContext.Session["Msg"] != null && !string.IsNullOrEmpty(HttpContext.Session["Msg"].ToString()))
            {
                ViewBag.SuccessMsg = HttpContext.Session["Msg"].ToString();
                HttpContext.Session["Msg"] = "";
            }
            else
            {
                ViewBag.SuccessMsg = "";
            }

            int currentVendorId = Convert.ToInt32(HttpContext.Session["UserId"]); 

            DataTable dtProductVendor = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
            SELECT 
                 pvm.Id, pvm.OrderBy, pvm.ProductId, pm.ProductName,
                 pvm.VendorId, vm.UserName,
                 pvm.RewardPoint, pvm.MRPPrice, pvm.PurchasePrice, 
                 pvm.DiscountPrice as DiscountAmount, pvm.Profit, pvm.SellPrice, 
                 pvm.CGST, pvm.SGST, pvm.IGST, pvm.IsActive
            FROM 
                tbl_Vendor_Product_Assign pvm
            LEFT JOIN 
                tbl_Product_Master pm ON pm.Id = pvm.ProductId
            LEFT JOIN 
                tbl_Vendor_Master vm ON vm.Id = pvm.VendorId
            WHERE 
                pvm.VendorId = @VendorId
            ORDER BY 
                pvm.OrderBy ASC", con);

                cmd.Parameters.AddWithValue("@VendorId", currentVendorId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtProductVendor);
            }

            int totalRecords = dtProductVendor.Rows.Count;

            ViewBag.previousid = "0";
            ViewBag.nextid = totalRecords.ToString();
            ViewBag.startpoint = "1";
            ViewBag.endpoint = totalRecords.ToString();
            ViewBag.ProductList = dtProductVendor;

            // Load category list
            DataTable dtCategory = objProdt.GetAllMaincategory();
            ViewBag.Category = dtCategory;

            // Load product master list
            DataTable dtProductMaster = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id, ProductName FROM tbl_Product_Master ORDER BY ProductName ASC", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtProductMaster);
            }
            ViewBag.ProductListMaster = dtProductMaster;

            ViewBag.RarURL = Request.Url.ToString();
            ViewBag.IsAttribute = Request.Url.ToString().Contains("portal") || Request.Url.ToString().Contains("localhost:4937");

            return View();
        }


        [HttpGet]
        public ActionResult Product_List_New()
        {
            //if (Request.Cookies["gstusr"] == null)
            //    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            if (HttpContext.Session["Msg"].ToString() != "" && HttpContext.Session["Msg"].ToString() != null)
            {
                ViewBag.SuccessMsg = HttpContext.Session["Msg"].ToString();
                Session["Msg"] = "";
            }
            else
            {
                ViewBag.SuccessMsg = "";
            }
            DataTable dt = new DataTable();
            dt = objProdt.BindProuctnew();
            int userRecords = 0;
            userRecords = dt.Rows.Count;
            ViewBag.previousid = "0";

            ViewBag.nextid = userRecords.ToString();

            //ViewBag.nextid = dtprodRecord.Rows[dtprodRecord.Rows.Count - 1].ItemArray[0].ToString();



            ViewBag.startpoint = "1";
            ViewBag.endpoint = userRecords.ToString();
            ViewBag.ProductList = dt;

            DataTable dtcategory = new DataTable();
            dtcategory = objProdt.GetAllMaincategory();
            ViewBag.Category = dtcategory;
            ViewBag.RarURL = Request.Url.ToString();
            if (Request.Url.ToString().Contains("portal") || Request.Url.ToString().Contains("localhost:4937"))
                ViewBag.IsAttribute = true;

            return View();
        }

        [HttpGet]
        public ActionResult EditProduct(int id = 0)
        {
            //if (Request.Cookies["gstusr"] == null)
            //    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);


          //  DateTime dateFromString = DateTime.Parse(Helper.indianTime.ToString(), System.Globalization.CultureInfo.InvariantCulture);


            ViewBag.EffectiveDate = Helper.indianTime.ToString("dd/MM/yyyy");


            DataTable dtcategory = new DataTable();
            dtcategory = objProdt.GetAllMaincategory();
            ViewBag.Category = dtcategory;

            DataTable dt = objProdt.BindProuct(id);
            if (dt.Rows.Count > 0)
            {
                // ViewBag.ProductId = dt.Rows[0]["Id"].ToString();
                int catID = 0;
                if (!string.IsNullOrEmpty(dt.Rows[0]["CategoryId"].ToString()))
                    catID = Convert.ToInt32(dt.Rows[0]["CategoryId"].ToString());

                ViewBag.CategoryId = catID;
                if (!string.IsNullOrEmpty(dt.Rows[0]["ProductName"].ToString()))
                    ViewBag.ProductName = dt.Rows[0]["ProductName"].ToString();
                else
                    ViewBag.ProductName = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["Code"].ToString()))
                    ViewBag.Code = dt.Rows[0]["Code"].ToString();
                else
                    ViewBag.Code = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["Price"].ToString()))
                    ViewBag.Price = dt.Rows[0]["Price"].ToString();
                else
                    ViewBag.Price = "0";
                if (!string.IsNullOrEmpty(dt.Rows[0]["DiscountAmount"].ToString()))
                    ViewBag.DiscountAmount = dt.Rows[0]["DiscountAmount"].ToString();
                else
                    ViewBag.DiscountAmount = "0";
                if (!string.IsNullOrEmpty(dt.Rows[0]["CGST"].ToString()))
                    ViewBag.CGST = dt.Rows[0]["CGST"].ToString();
                else
                    ViewBag.CGST = "0";
                if (!string.IsNullOrEmpty(dt.Rows[0]["IGST"].ToString()))
                    ViewBag.IGST = dt.Rows[0]["IGST"].ToString();
                else
                    ViewBag.IGST = "0";
                if (!string.IsNullOrEmpty(dt.Rows[0]["SGST"].ToString()))
                    ViewBag.SGST = dt.Rows[0]["SGST"].ToString();
                else
                    ViewBag.SGST = "0";
                if (!string.IsNullOrEmpty(dt.Rows[0]["RewardPoint"].ToString()))
                    ViewBag.RewardPoint = dt.Rows[0]["RewardPoint"].ToString();
                else
                    ViewBag.RewardPoint = "0";
                if (!string.IsNullOrEmpty(dt.Rows[0]["Detail"].ToString()))
                    ViewBag.Detail = dt.Rows[0]["Detail"].ToString();
                else
                    ViewBag.Detail = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["OrderBy"].ToString()))
                    ViewBag.OrderBy = dt.Rows[0]["OrderBy"].ToString();
                else
                    ViewBag.OrderBy = "0";
                if (!string.IsNullOrEmpty(dt.Rows[0]["SalePrice"].ToString()))
                    ViewBag.SaleAmount = dt.Rows[0]["SalePrice"].ToString();
                else
                    ViewBag.SaleAmount = "0";
                if (!string.IsNullOrEmpty(dt.Rows[0]["PurchasePrice"].ToString()))
                    ViewBag.PurchaseAmount = dt.Rows[0]["PurchasePrice"].ToString();
                else
                    ViewBag.PurchaseAmount = "0";
                if (!string.IsNullOrEmpty(dt.Rows[0]["Profit"].ToString()))
                    ViewBag.Profit = dt.Rows[0]["Profit"].ToString();
                else
                    ViewBag.Profit = "0";
                if (!string.IsNullOrEmpty(dt.Rows[0]["Subscription"].ToString()))
                    ViewBag.Subscription = dt.Rows[0]["Subscription"].ToString();
                else
                    ViewBag.Subscription = "0";
                if (!string.IsNullOrEmpty(dt.Rows[0]["Image"].ToString()))
                    ViewBag.Image = dt.Rows[0]["Image"].ToString();
                else
                    ViewBag.Image = "";
				if (!string.IsNullOrEmpty(dt.Rows[0]["MainImage"].ToString()))
					ViewBag.MainImage = dt.Rows[0]["MainImage"].ToString();
				else
					ViewBag.MainImage = "";
				if (!string.IsNullOrEmpty(dt.Rows[0]["YoutubeTitle"].ToString()))
                    ViewBag.YoutubeTitle = dt.Rows[0]["YoutubeTitle"].ToString();
                else
                    ViewBag.YoutubeTitle = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["YoutubeURL"].ToString()))
                    ViewBag.YoutubeURL = dt.Rows[0]["YoutubeURL"].ToString();
                else
                    ViewBag.YoutubeURL = "";

                ViewBag.Active = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                ViewBag.Daily = Convert.ToBoolean(dt.Rows[0]["IsDaily"]);

                string a = dt.Rows[0]["IsAlternate"].ToString();
                if (dt.Rows[0]["IsAlternate"].ToString()=="True")
                {
                    ViewBag.Alternate = Convert.ToBoolean(dt.Rows[0]["IsAlternate"]);
                }
                if (dt.Rows[0]["IsAlternate"].ToString() != "True")
                {
                    int b = 0;
                    ViewBag.Alternate = Convert.ToBoolean(b);
                }

                if (dt.Rows[0]["IsMultiple"].ToString() == "True")
                {
                    ViewBag.Multiple = Convert.ToBoolean(dt.Rows[0]["IsMultiple"]);
                }
                if (dt.Rows[0]["IsMultiple"].ToString() != "True")
                {
                    int b = 0;
                    ViewBag.Multiple = Convert.ToBoolean(b);
                }

                if (dt.Rows[0]["IsWeeklyday"].ToString() == "True")
                {
                    ViewBag.WeeklyDay = Convert.ToBoolean(dt.Rows[0]["IsWeeklyday"]);
                }
                if (dt.Rows[0]["IsWeeklyday"].ToString() != "True")
                {
                    int b = 0;
                    ViewBag.WeeklyDay = Convert.ToBoolean(b);
                }



                //ViewBag.WeeklyDay = Convert.ToBoolean(dt.Rows[0]["IsWeeklyday"]);

                var mainCat = objProdt.BindCategory(catID);
                if (mainCat.Rows.Count > 0)
                    if (!string.IsNullOrEmpty(mainCat.Rows[0]["ParentCategoryId"].ToString()))
                        ViewBag.MainCategoryId = mainCat.Rows[0]["ParentCategoryId"];
                    else
                        ViewBag.MainCategoryId = catID;

                //Get Photo list
                var lstPhoto = _clsCommon.selectwhere("*", "tbl_Product_Images", " ProductId='" + id + "'");
                ViewBag.lstPhoto = lstPhoto;
            }
            return View();
        }

		[HttpGet]
		public ActionResult EditProductVendor(int id = 0)
		{
			if (HttpContext.Session["UserId"] == null)
				return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

			
			DataTable categoryData = _clsCommon.selectwhere("*", "tbl_Product_Category_Master", "");
			ViewBag.Category = categoryData;

			
			DataTable productData = objProdt.GetAllProducts();
			ViewBag.Products = productData;
			ViewBag.SelectedProductId = null;
			ViewBag.CategorId = null;
			ViewBag.SubCategoryId = null;

			
			if (id > 0)
			{
				DataTable dt = _clsCommon.selectwhere("*", "tbl_Vendor_Product_Assign", $"Id='{id}'");
				if (dt.Rows.Count > 0)
				{
					DataRow dr = dt.Rows[0];

					ViewBag.SelectedProductId = Convert.ToInt32(dr["ProductId"]);
					ViewBag.CategorId = Convert.ToInt32(dr["CategoryId"]);
					ViewBag.SubCategoryId = Convert.ToInt32(dr["SubCategoryId"]);

					// Set other data for text fields/checkboxes
					ViewBag.Price = dr["MRPPrice"].ToString();
					ViewBag.DiscountAmount = dr["DiscountPrice"].ToString();
					ViewBag.CGST = dr["CGST"].ToString();
					ViewBag.SGST = dr["SGST"].ToString();
					ViewBag.IGST = dr["IGST"].ToString();
					ViewBag.RewardPoint = dr["RewardPoint"].ToString();
					ViewBag.IsActive = Convert.ToBoolean(dr["IsActive"]);
					ViewBag.IsDaily = Convert.ToBoolean(dr["IsDaily"]);
					ViewBag.IsAlternate = Convert.ToBoolean(dr["IsAlternate"]);
					ViewBag.IsMultiple = Convert.ToBoolean(dr["IsMultiple"]);
					ViewBag.IsWeekDay = Convert.ToBoolean(dr["IsWeekDay"]);
					ViewBag.PurchaseAmount = dr["PurchasePrice"].ToString();
					ViewBag.SaleAmount = dr["SellPrice"].ToString();
					ViewBag.Subscription = dr["Subscription"].ToString();
					ViewBag.Profit = dr["Profit"].ToString();
					ViewBag.OrderBy = dr["OrderBy"].ToString();
				}
			}
			if (ViewBag.CategorId != null)
			{
				DataTable subcategoryData = _clsCommon.selectwhere("*", "tbl_Product_Category_Master", $"Id = '{ViewBag.CategorId}'");
				ViewBag.SubCategory = subcategoryData;
			}
			if (ViewBag.SubCategoryId != null)
			{
				DataTable subcategoryData = _clsCommon.selectwhere("*", "tbl_Product_Category_Master", $"Id = '{ViewBag.SubCategoryId}'");
				ViewBag.SubCategory = subcategoryData;
			}
			else
			{
				ViewBag.SubCategory = new DataTable();
			}

			return View();
		}




		[HttpPost]
        public ActionResult EditProduct(Product objProdt, FormCollection form, HttpPostedFileBase Document1, HttpPostedFileBase[] photos)
        {
            //if (Request.Cookies["gstusr"] == null)
            //    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            Subscription objsub = new Subscription();
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);


            var fdate = Request["datepicker"];
            if (!string.IsNullOrEmpty(fdate.ToString()))
            {
                objProdt.updatedon = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
            }
           // var _fdate = objProdt.updatedon.Value.ToString("dd-MM-yyyy");

            string category = Request["ddlSubCategory"];
            if (category != null)
                objProdt.CategoryId = Convert.ToInt32(category);
            else
            {
                category = Request["ddlCategory"];
                objProdt.CategoryId = Convert.ToInt32(category);
            }
            string active = Request["IsActive"];
            if (active == "on")
            {
                bool add = Convert.ToBoolean(true);
                objProdt.IsActive = Convert.ToBoolean(add);
            }
            else
            {
                if (!string.IsNullOrEmpty(active))
                {
                    bool add = Convert.ToBoolean(Request["IsActive"].Split(',')[0]);
                    objProdt.IsActive = Convert.ToBoolean(add);
                }
            }
            string isDaily = Request["IsDaily"];
            if (isDaily == "on")
            {
                bool add = Convert.ToBoolean(true);
                objProdt.IsDaily = Convert.ToBoolean(add);
            }
            else
            {
                if (!string.IsNullOrEmpty(isDaily))
                {
                    bool add = Convert.ToBoolean(Request["IsDaily"].Split(',')[0]);
                    objProdt.IsDaily = Convert.ToBoolean(add);
                }
            }


            string isAlternate = Request["IsAlternate"];
            if (isAlternate == "on")
            {
                bool add = Convert.ToBoolean(true);
                objProdt.IsAlternate = Convert.ToBoolean(add);
            }
            else
            {
                if (!string.IsNullOrEmpty(isAlternate))
                {
                    bool add = Convert.ToBoolean(Request["IsAlternate"].Split(',')[0]);
                    objProdt.IsAlternate = Convert.ToBoolean(add);
                }
            }


            string isMultiple = Request["IsMultiple"];
            if (isMultiple == "on")
            {
                bool add = Convert.ToBoolean(true);
                objProdt.IsMultipleDay = Convert.ToBoolean(add);
            }
            else
            {
                if (!string.IsNullOrEmpty(isMultiple))
                {
                    bool add = Convert.ToBoolean(Request["IsMultiple"].Split(',')[0]);
                    objProdt.IsMultipleDay = Convert.ToBoolean(add);
                }
            }



            string isWeekDay = Request["IsWeekDay"];
            if (isWeekDay == "on")
            {
                bool add = Convert.ToBoolean(true);
                objProdt.IsWeeklyDay = Convert.ToBoolean(add);
            }
            else
            {
                if (!string.IsNullOrEmpty(isWeekDay))
                {
                    bool add = Convert.ToBoolean(Request["IsWeekDay"].Split(',')[0]);
                    objProdt.IsWeeklyDay = Convert.ToBoolean(add);
                }
            }
            //Check weather there is entry of product in Orders or Transaction
            if (objProdt.IsActive == false)
            {
                DataTable ProductInTransaction = objProdt.ProductInTransaction(objProdt.Id);
                if (ProductInTransaction.Rows.Count > 0)
                {

                    for(int i=0;i< ProductInTransaction.Rows.Count;i++)
                    {
                        int orderid = Convert.ToInt32(ProductInTransaction.Rows[i].ItemArray[1].ToString());

                        int od = objProdt.DeleteFutureOrder(orderid);

                    }
                    

                    //ViewBag.SuccessMsg = "You cannot InActive Product !!";
                    //return View();

                    // int od = objProdt.DeleteFutureOrder();

                }
            }

           
            if (!string.IsNullOrEmpty(Request.Form["Subscription"].ToString()))
                objProdt.Subscription = Convert.ToDecimal(Request.Form["Subscription"].ToString());
            //check data duplicate
            DataTable dtDupliProduct = new DataTable();
            dtDupliProduct = objProdt.CheckDuplicateProduct(objProdt.CategoryId, objProdt.ProductName);
            if (dtDupliProduct.Rows.Count > 0)
            {
                int SId = Convert.ToInt32(dtDupliProduct.Rows[0]["Id"]);
                //if (SId == objProdt.Id)
                if (SId > 0)
                {
                    string fname = null, path = null;
                    HttpPostedFileBase document1 = Request.Files["Document1"];
                    string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png", ".PNG" };
                    if (document1 != null)
                    {
                        if (document1.ContentLength > 0)
                        {
                            try
                            {
                                HttpFileCollectionBase files = Request.Files;
                                HttpPostedFileBase file = Document1;
                                //Resize image 500*300 coding
                                WebImage img = new WebImage(file.InputStream);
                                img.Resize(300, 300, false, false);
                                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                                string extension = Path.GetExtension(file.FileName);
                                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                                {
                                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                    fname = testfiles[testfiles.Length - 1];
                                }
                                else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.ToLower().LastIndexOf('.'))))
                                {
                                    ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
                                }
                                else
                                {
                                    fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                                    fname = fname.Replace(" ", "");
                                }
                                path = Path.Combine(Server.MapPath("~/image/product/"), fname);
                                img.Save(path);
                                // file.SaveAs(path);
                                objProdt.MainImage = fname;
                            }
                            catch (Exception ex)
                            {
                                ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                            }
                        }
                        else
                        {
                            ViewBag.SuccessMsg = "File Not Get Updated!!!";
                            var existfile = "";
                            DataTable dt1 = objProdt.BindProuct(objProdt.Id);
                            ViewBag.Image = dt1.Rows[0]["MainImage"].ToString();
                            existfile = ViewBag.Image;
                            objProdt.Image = existfile;
                        }
                    }
                    else
                    {
                        ViewBag.SuccessMsg = "File Not Get Updated!!!";
                        var existfile = "";
                        DataTable dt1 = objProdt.BindProuct(objProdt.Id);
                        ViewBag.Image = dt1.Rows[0]["Image"].ToString();
                        existfile = ViewBag.Image;
                        objProdt.Image = existfile;


                    }

                    //check MRP and SalePrice Changed or Not
                    decimal MRPPrice = 0, SalePrice = 0,PurchasePrice=0;
                    decimal NewMRPPrice = objProdt.Price, NewSalePrice = objProdt.SaleAmount , NewPurchasePrice=objProdt.PurchaseAmount;
                    DataTable dtCheckPrice = objProdt.BindProuct(objProdt.Id);
                    if (dtCheckPrice.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dtCheckPrice.Rows[0]["Price"].ToString()))
                            MRPPrice = Convert.ToDecimal(dtCheckPrice.Rows[0]["Price"]);
                        if (!string.IsNullOrEmpty(dtCheckPrice.Rows[0]["SalePrice"].ToString()))
                            SalePrice = Convert.ToDecimal(dtCheckPrice.Rows[0]["SalePrice"]);

                        if (!string.IsNullOrEmpty(dtCheckPrice.Rows[0]["PurchasePrice"].ToString()))
                            PurchasePrice = Convert.ToDecimal(dtCheckPrice.Rows[0]["PurchasePrice"]);
                    }

                    int addresult = 0;
                    if (MRPPrice == NewMRPPrice && SalePrice == NewSalePrice && PurchasePrice==NewPurchasePrice)
                    {
                        addresult = objProdt.UpdateProduct(objProdt);
                    }
                    else
                    {
                        addresult = objProdt.UpdateProduct(objProdt);
                        //update Pending order Prices

                    if(Convert.ToDecimal(MRPPrice) != Convert.ToDecimal(NewMRPPrice) || Convert.ToDecimal(SalePrice) != Convert.ToDecimal(NewSalePrice) || Convert.ToDecimal(PurchasePrice)!= Convert.ToDecimal(NewPurchasePrice))
                        {
                            DataTable ProductInTransaction = objProdt.ProductInTransactiondatewise(objProdt.Id, objProdt.updatedon);
                            if (ProductInTransaction.Rows.Count > 0)
                            {

                                for (int i = 0; i < ProductInTransaction.Rows.Count; i++)
                                {
                                    int orderid = Convert.ToInt32(ProductInTransaction.Rows[i].ItemArray[1].ToString());
                                    int qty= Convert.ToInt32(ProductInTransaction.Rows[i].ItemArray[3].ToString());

                                    double samount = Convert.ToDouble(qty) * Convert.ToDouble(NewSalePrice);

                                    double pamount = Convert.ToDouble(qty) * Convert.ToDouble(NewPurchasePrice);
                                    double mamount = Convert.ToDouble(qty) * Convert.ToDouble(NewMRPPrice);

                                    int od = objProdt.UpdatePriceFutureOrder(orderid,samount,pamount,mamount, Convert.ToDouble(NewSalePrice));

                                    //if(ProductInTransaction.Rows[i].ItemArray[4].ToString()== "Complete")
                                    //{
                                    //    objsub.OrderId = orderid;
                                    //    objsub.Qty = qty;
                                    //    objsub.Amount =Convert.ToDecimal(samount);
                                    //    objsub.CustomerId=Convert.ToInt32(ProductInTransaction.Rows[i].ItemArray[5].ToString());
                                    //   int j = objsub.UpdateCustomerWallet1(objsub);
                                    //}

                                }


                            }
                        }
                    }

					foreach (HttpPostedFileBase file in photos)
					{
						if (file != null)
						{
							string fileName = Path.GetFileNameWithoutExtension(file.FileName);
							fileName = dHelper.RemoveIllegalCharacters(fileName);
							string _ext = Path.GetExtension(file.FileName);
							fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + _ext;

							var ServerSavePath = Path.Combine(Server.MapPath("~/image/product/") + fname);
							file.SaveAs(ServerSavePath);
							_clsCommon.updatedata("tbl_Product_Master", "Image='" + fname + "'", "Id=" + objProdt.Id);
						}
					}
					if (addresult > 0)
                    {
                        ViewBag.SuccessMsg = "Product Updated Successfully!!!";
                    }
                    else
                    { ViewBag.SuccessMsg = "Product Not Updated!!!"; }
                }
                else
                { ViewBag.SuccessMsg = "Product Already Exits!!!"; }
            }
            else
            {
                string fname = null, path = null;
                HttpPostedFileBase document1 = Request.Files["Document1"];
                string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png" };
                if (document1 != null)
                {
                    if (document1.ContentLength > 0)
                    {
                        try
                        {
                            HttpFileCollectionBase files = Request.Files;
                            HttpPostedFileBase file = Document1;
                            //Resize image 500*300 coding
                            WebImage img = new WebImage(file.InputStream);
                            img.Resize(300, 300, false, false);
                            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            string extension = Path.GetExtension(file.FileName);
                            if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                            {
                                string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                fname = testfiles[testfiles.Length - 1];
                            }
                            else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                            {
                                ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
                            }
                            else
                            {
                                fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                            }
                            path = Path.Combine(Server.MapPath("~/image/product/"), fname);
                            img.Save(path);
                            // file.SaveAs(path);
                            objProdt.Image = fname;
                        }
                        catch (Exception ex)
                        {
                            ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                        }
                    }
                    else
                    {
                        ViewBag.SuccessMsg = "File Not Get Updated!!!";
                        var existfile = "";
                        DataTable dt1 = objProdt.BindProuct(objProdt.Id);
                        ViewBag.Image = dt1.Rows[0]["Image"].ToString();
                        existfile = ViewBag.Image;
                        objProdt.Image = existfile;
                    }
                }
                else
                {
                    ViewBag.SuccessMsg = "File Not Get Updated!!!";
                    var existfile = "";
                    DataTable dt1 = objProdt.BindProuct(objProdt.Id);
                    ViewBag.Image = dt1.Rows[0]["Image"].ToString();
                    existfile = ViewBag.Image;
                    objProdt.Image = existfile;
                }
                //check MRP and SalePrice Changed or Not
                decimal MRPPrice = 0, SalePrice = 0, PurchasePrice = 0;
                decimal NewMRPPrice = objProdt.Price, NewSalePrice = objProdt.SaleAmount, NewPurchasePrice = objProdt.PurchaseAmount;
                DataTable dtCheckPrice = objProdt.BindProuct(objProdt.Id);
                if (dtCheckPrice.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtCheckPrice.Rows[0]["Price"].ToString()))
                        MRPPrice = Convert.ToDecimal(dtCheckPrice.Rows[0]["Price"]);
                    if (!string.IsNullOrEmpty(dtCheckPrice.Rows[0]["SalePrice"].ToString()))
                        SalePrice = Convert.ToDecimal(dtCheckPrice.Rows[0]["SalePrice"]);

                    if (!string.IsNullOrEmpty(dtCheckPrice.Rows[0]["PurchasePrice"].ToString()))
                        PurchasePrice = Convert.ToDecimal(dtCheckPrice.Rows[0]["PurchasePrice"]);
                }


                int addresult = 0;
                if (MRPPrice == NewMRPPrice && SalePrice == NewSalePrice && PurchasePrice == NewPurchasePrice)
                {
                    addresult = objProdt.UpdateProduct(objProdt);
                }
                else
                {
                    addresult = objProdt.UpdateProduct(objProdt);
                    //update Pending order Prices
                    //DataTable dtGetProductOrder = objProdt.GetProductWiseOrder(objProdt.Id);
                    //if (dtGetProductOrder.Rows.Count > 0)
                    //{

                    //}


                    if (Convert.ToDecimal(MRPPrice) != Convert.ToDecimal(NewMRPPrice) || Convert.ToDecimal(SalePrice) != Convert.ToDecimal(NewSalePrice) || Convert.ToDecimal(PurchasePrice) != Convert.ToDecimal(NewPurchasePrice))
                    {
                        DataTable ProductInTransaction = objProdt.ProductInTransactiondatewise(objProdt.Id, objProdt.updatedon);
                        if (ProductInTransaction.Rows.Count > 0)
                        {

                            for (int i = 0; i < ProductInTransaction.Rows.Count; i++)
                            {
                                int orderid = Convert.ToInt32(ProductInTransaction.Rows[i].ItemArray[1].ToString());
                                int qty = Convert.ToInt32(ProductInTransaction.Rows[i].ItemArray[3].ToString());

                                double samount = Convert.ToDouble(qty) * Convert.ToDouble(NewSalePrice);

                                double pamount = Convert.ToDouble(qty) * Convert.ToDouble(NewPurchasePrice);
                                double mamount = Convert.ToDouble(qty) * Convert.ToDouble(NewMRPPrice);

                                int od = objProdt.UpdatePriceFutureOrder(orderid, samount, pamount, mamount, Convert.ToDouble(NewSalePrice));



                            }


                        }
                    }

                }
                if (addresult > 0)
                {
                    ViewBag.SuccessMsg = "Product Updated Successfully!!!";
                }
                else
                { ViewBag.SuccessMsg = "Product Not Updated!!!"; }
            }

            DataTable dtcategory = new DataTable();
            dtcategory = objProdt.GetAllMaincategory();
            ViewBag.Category = dtcategory;

            DataTable dt = objProdt.BindProuct(objProdt.Id);
            int catID = 0;
            if (!string.IsNullOrEmpty(dt.Rows[0]["CategoryId"].ToString()))
                catID = Convert.ToInt32(dt.Rows[0]["CategoryId"].ToString());

            ViewBag.CategoryId = catID;

            if (!string.IsNullOrEmpty(dt.Rows[0]["ProductName"].ToString()))
                ViewBag.ProductName = dt.Rows[0]["ProductName"].ToString();
            else
                ViewBag.ProductName = "";
            if (!string.IsNullOrEmpty(dt.Rows[0]["Code"].ToString()))
                ViewBag.Code = dt.Rows[0]["Code"].ToString();
            else
                ViewBag.Code = "";
            if (!string.IsNullOrEmpty(dt.Rows[0]["Price"].ToString()))
                ViewBag.Price = dt.Rows[0]["Price"].ToString();
            else
                ViewBag.Price = "";
            if (!string.IsNullOrEmpty(dt.Rows[0]["DiscountAmount"].ToString()))
                ViewBag.DiscountAmount = dt.Rows[0]["DiscountAmount"].ToString();
            else
                ViewBag.DiscountAmount = "";
            if (!string.IsNullOrEmpty(dt.Rows[0]["CGST"].ToString()))
                ViewBag.CGST = dt.Rows[0]["CGST"].ToString();
            else
                ViewBag.CGST = "";
            if (!string.IsNullOrEmpty(dt.Rows[0]["IGST"].ToString()))
                ViewBag.IGST = dt.Rows[0]["IGST"].ToString();
            else
                ViewBag.IGST = "";
            if (!string.IsNullOrEmpty(dt.Rows[0]["SGST"].ToString()))
                ViewBag.SGST = dt.Rows[0]["SGST"].ToString();
            else
                ViewBag.SGST = "";
            if (!string.IsNullOrEmpty(dt.Rows[0]["RewardPoint"].ToString()))
                ViewBag.RewardPoint = dt.Rows[0]["RewardPoint"].ToString();
            else
                ViewBag.RewardPoint = "";
            if (!string.IsNullOrEmpty(dt.Rows[0]["Detail"].ToString()))
                ViewBag.Detail = dt.Rows[0]["Detail"].ToString();
            else
                ViewBag.Detail = "";
            if (!string.IsNullOrEmpty(dt.Rows[0]["Image"].ToString()))
                ViewBag.Image = dt.Rows[0]["Image"].ToString();
            else
                ViewBag.Image = "";
            if (!string.IsNullOrEmpty(dt.Rows[0]["OrderBy"].ToString()))
                ViewBag.OrderBy = dt.Rows[0]["OrderBy"].ToString();
            else
                ViewBag.OrderBy = "0";
            if (!string.IsNullOrEmpty(dt.Rows[0]["SalePrice"].ToString()))
                ViewBag.SaleAmount = dt.Rows[0]["SalePrice"].ToString();
            else
                ViewBag.SaleAmount = "0";
            if (!string.IsNullOrEmpty(dt.Rows[0]["PurchasePrice"].ToString()))
                ViewBag.PurchaseAmount = dt.Rows[0]["PurchasePrice"].ToString();
            else
                ViewBag.PurchaseAmount = "0";
            if (!string.IsNullOrEmpty(dt.Rows[0]["Profit"].ToString()))
                ViewBag.Profit = dt.Rows[0]["Profit"].ToString();
            else
                ViewBag.Profit = "0";
            if (!string.IsNullOrEmpty(dt.Rows[0]["Subscription"].ToString()))
                ViewBag.Subscription = dt.Rows[0]["Subscription"].ToString();
            else
                ViewBag.Subscription = "0";
            if (!string.IsNullOrEmpty(dt.Rows[0]["YoutubeTitle"].ToString()))
                ViewBag.YoutubeTitle = dt.Rows[0]["YoutubeTitle"].ToString();
            else
                ViewBag.YoutubeTitle = "";
            if (!string.IsNullOrEmpty(dt.Rows[0]["YoutubeURL"].ToString()))
                ViewBag.YoutubeURL = dt.Rows[0]["YoutubeURL"].ToString();
            else
                ViewBag.YoutubeURL = "";
            ViewBag.Active = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
            ViewBag.Daily = Convert.ToBoolean(dt.Rows[0]["IsDaily"]);

            if (dt.Rows[0]["IsAlternate"].ToString() == "True")
            {
                ViewBag.Alternate = Convert.ToBoolean(dt.Rows[0]["IsAlternate"]);
            }
            if (dt.Rows[0]["IsAlternate"].ToString() != "True")
            {
                int b = 0;
                ViewBag.Alternate = Convert.ToBoolean(b);
            }

            if (dt.Rows[0]["IsMultiple"].ToString() == "True")
            {
                ViewBag.Multiple = Convert.ToBoolean(dt.Rows[0]["IsMultiple"]);
            }
            if (dt.Rows[0]["IsMultiple"].ToString() != "True")
            {
                int b = 0;
                ViewBag.Multiple = Convert.ToBoolean(b);
            }

            if (dt.Rows[0]["IsWeeklyday"].ToString() == "True")
            {
                ViewBag.WeeklyDay = Convert.ToBoolean(dt.Rows[0]["IsWeeklyday"]);
            }
            if (dt.Rows[0]["IsWeeklyday"].ToString() != "True")
            {
                int b = 0;
                ViewBag.WeeklyDay = Convert.ToBoolean(b);
            }


            var mainCat = objProdt.BindCategory(catID);
            if (mainCat.Rows.Count > 0)
                if (!string.IsNullOrEmpty(mainCat.Rows[0]["ParentCategoryId"].ToString()))
                    ViewBag.MainCategoryId = mainCat.Rows[0]["ParentCategoryId"];
                else
                    ViewBag.MainCategoryId = catID;

            ViewBag.EffectiveDate = Helper.indianTime.ToString("dd/MM/yyyy");
            return View();
        }
        [HttpPost]
        public ActionResult EditProductVendor(Product objProdt, FormCollection form)
        {
            int i = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand(@"
                UPDATE tbl_Vendor_Product_Assign SET
                    CategoryId = @ParentCategoryId,
                    SubCategoryId = @CategoryId,
                    ProductId = @ProductId,
                    MRPPrice = @Price,
                    DiscountPrice = @DiscountAmount,
                    CGST = @CGST,
                    SGST = @SGST,
                    IGST = @IGST,
                    RewardPoint = @RewardPoint,
                    IsActive = @IsActive,
                    IsDaily = @IsDaily,
                    IsAlternate = @IsAlternate,
                    IsMultiple = @IsMultiple,
                    IsWeekDay = @IsWeekDay,
                    PurchasePrice = @PurchasePrice,
                    SellPrice = @SalePrice,
                    Subscription = @Subscription,
                    Profit = @Profit,
                    OrderBy = @OrderBy
                WHERE Id = @Id", con);

                    // Bind parameters
                    com.Parameters.AddWithValue("@Id", objProdt.Id); // Assuming objProdt.Id is the record ID in tbl_ProductVendor_Master

                    com.Parameters.AddWithValue("@ProductId", (object)objProdt.Id ?? DBNull.Value);
					com.Parameters.AddWithValue("@ParentCategoryId", (object)objProdt.Id ?? DBNull.Value);
					com.Parameters.AddWithValue("@CategoryId", (object)objProdt.Id ?? DBNull.Value);
					com.Parameters.AddWithValue("@Price", (object)objProdt.Price ?? DBNull.Value);
                    com.Parameters.AddWithValue("@DiscountAmount", (object)objProdt.DiscountAmount ?? DBNull.Value);
                    com.Parameters.AddWithValue("@CGST", (object)objProdt.CGST ?? DBNull.Value);
                    com.Parameters.AddWithValue("@SGST", (object)objProdt.SGST ?? DBNull.Value);
                    com.Parameters.AddWithValue("@IGST", (object)objProdt.IGST ?? DBNull.Value);
                    com.Parameters.AddWithValue("@Subscription", (object)objProdt.Subscription ?? DBNull.Value);
                    com.Parameters.AddWithValue("@RewardPoint", (object)objProdt.RewardPoint ?? DBNull.Value);

                    com.Parameters.AddWithValue("@IsActive", form["IsActive"]?.Contains("true") == true);
                    com.Parameters.AddWithValue("@IsDaily", form["IsDaily"]?.Contains("true") == true);
                    com.Parameters.AddWithValue("@IsAlternate", form["IsAlternate"]?.Contains("true") == true);
                    com.Parameters.AddWithValue("@IsMultiple", form["IsMultiple"]?.Contains("true") == true);
                    com.Parameters.AddWithValue("@IsWeekDay", form["IsWeekDay"]?.Contains("true") == true);

                    com.Parameters.AddWithValue("@PurchasePrice", (object)objProdt.PurchaseAmount ?? DBNull.Value);
                    com.Parameters.AddWithValue("@SalePrice", (object)objProdt.SaleAmount ?? DBNull.Value);
                    com.Parameters.AddWithValue("@Profit", (object)objProdt.Profit ?? DBNull.Value);
                    com.Parameters.AddWithValue("@OrderBy", (object)objProdt.OrderBy ?? DBNull.Value);

                    i = com.ExecuteNonQuery();
                    con.Close();
                }

                if (i > 0)
                {
                    TempData["SuccessMsg"] = "Product vendor updated successfully.";
                    return RedirectToAction("ProductVendorList");
                }
                else
                {
                    ViewBag.ErrorMsg = "Update failed. Record not found.";
                    return View(objProdt);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Error occurred: " + ex.Message;
                return View(objProdt);
            }
        }


        [HttpGet]
        public ActionResult DeleteProduct(int id)
        {
            try
            {

                int OrderBy = 0;
                
                objProdt.Id = id;
                DataTable dtCheckPrice = objProdt.BindProuct(objProdt.Id);
                if (dtCheckPrice.Rows.Count > 0)
                {
                    OrderBy = Convert.ToInt32(dtCheckPrice.Rows[0]["OrderBy"]);
                }

                objProdt.DeleteProductFavourite(id);
                objProdt.DeleteProductOrderDetail(id);
                int delresult = objProdt.DeleteProduct(id,OrderBy);                
                return RedirectToAction("ProductList");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_orderdetail_product"))
                {
                    TempData["error"] = String.Format("You can not deleted. Child record found.");
                }
                else
                    throw ex;
            }
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("ProductList");
        }
        [HttpGet]
        public ActionResult DeleteProductVendor(int id)
        {
            try
            {
                int OrderBy = 0;
                objProdt.Id = id;
                DataTable dtCheckPrice = objProdt.BindProuct(objProdt.Id);
                if (dtCheckPrice.Rows.Count > 0)
                {
                    OrderBy = Convert.ToInt32(dtCheckPrice.Rows[0]["OrderBy"]);
                }
                int delresult = objProdt.DeleteProductVendor(id, OrderBy);
                return RedirectToAction("ProductVendorList");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_orderdetail_product"))
                {
                    TempData["error"] = String.Format("You can not deleted. Child record found.");
                }
                else
                    throw ex;
            }
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("ProductList");
        }


        [HttpPost]
        public ActionResult ProductList(FormCollection form, HttpPostedFileBase Document1,Product objproduct1)
        {
            int userRecords = 0;
            DataTable dtprodRecord = new DataTable();
            DataTable dtprodRecordtotal = new DataTable();
            Product obj = new Product();
            string submit = Request["submit"];
           
            Document1 = Request.Files[submit + "Document1"];

            


            if (submit == "Delete" || submit == "Next" || submit == "Previous" || submit == "First" || submit == "Last" || submit== "Search")
            {

                if(submit== "Search")
                {
                    ViewBag.ProductCatselect = Request["ddlCategory"];

                    string categoryid = ViewBag.ProductCatselect;
                    dtprodRecord = obj.BindProuctcategorywise(categoryid);
                    userRecords = dtprodRecord.Rows.Count;
                    ViewBag.startpoint = "1";
                    ViewBag.endpoint = userRecords.ToString();
                    ViewBag.ProductList = dtprodRecord;
                }


                if (submit == "Next")
                {
                    userRecords = 0;
                    string previous = objproduct1.previous1;


                    string next = objproduct1.next1;

                    if (string.IsNullOrEmpty(previous) || previous == "")
                    {
                        previous = "0";
                    }

                    if (string.IsNullOrEmpty(next) || next == "" || next == "0")
                    {
                        next = "0";
                        return RedirectToAction("ProductList", "Product");
                    }
                    string startid = next;
                  
                    string startpointn = objproduct1.startpoint;
                    string endpointn = objproduct1.endpoint;
                    int spoint = 0, epoint = 0;


                    dtprodRecord = obj.BindProuctnewnext(next);
                    userRecords = dtprodRecord.Rows.Count;

                    if (userRecords > 0)
                    {


                        spoint = Convert.ToInt32(startpointn) + 50;
                        epoint = Convert.ToInt32(endpointn) + userRecords;


                        ViewBag.startpoint = spoint.ToString();
                        ViewBag.endpoint = epoint.ToString();
                        //ViewBag.previousid = dtprodRecord.Rows[0].ItemArray[0].ToString();
                        //ViewBag.nextid = dtprodRecord.Rows[dtprodRecord.Rows.Count - 1].ItemArray[0].ToString();

                        ViewBag.previousid = next.ToString();
                        ViewBag.nextid = Convert.ToInt32(next) + userRecords;

                       


                    }

                    else
                    {

                        dtprodRecord = obj.BindProuctnewnext(previous);
                        ViewBag.startpoint = startpointn.ToString();
                        ViewBag.endpoint = endpointn.ToString();
                        ViewBag.previousid = previous.ToString();
                        ViewBag.nextid = next.ToString();
                    }

                    ViewBag.ProductList = dtprodRecord;
                }


                if (submit == "Previous")
                {
                    userRecords = 0;
                    string previous = objproduct1.previous1;
                    string next = objproduct1.next1;
                    int previous1 = Convert.ToInt32(previous) - 50;
                    if (string.IsNullOrEmpty(previous) || previous == "" || previous == "0")
                    {
                        return RedirectToAction("ProductList", "Product");
                        previous = "0";
                        previous1 = 0;
                    }



                    if (string.IsNullOrEmpty(next) || next == "")
                    {
                        next = "0";
                    }
                    string startid = next;
                    string startpointn = objproduct1.startpoint;
                    string endpointn = objproduct1.endpoint;
                    int spoint = 0, epoint = 0;


                    string previous2 = previous1.ToString();
                    dtprodRecord = obj.BindProuctnewnext(previous2);
                    userRecords = dtprodRecord.Rows.Count;

                    if (userRecords > 0)
                    {
                        if (Convert.ToInt32(startpointn) > 50)
                        {
                            spoint = Convert.ToInt32(startpointn) - 50;
                            epoint = Convert.ToInt32(endpointn) - 50;
                        }
                        ViewBag.startpoint = spoint.ToString();
                        ViewBag.endpoint = epoint.ToString();
                        ViewBag.previousid = previous2.ToString();
                        ViewBag.nextid = Convert.ToInt32(previous2) + userRecords; ;
                    }

                    else
                    {
                        dtprodRecord = obj.BindProuctnew();
                        userRecords = dtprodRecord.Rows.Count;
                        ViewBag.previousid = "0";
                        ViewBag.nextid = userRecords.ToString();
                        userRecords = dtprodRecord.Rows.Count;
                        ViewBag.startpoint = "1";
                        ViewBag.endpoint = userRecords.ToString();
                    }


                    ViewBag.ProductList = dtprodRecord;

                }


                if (submit == "First")
                {
                    dtprodRecord = obj.BindProuctnew();
                    userRecords = dtprodRecord.Rows.Count;
                    ViewBag.previousid = "0";
                    ViewBag.nextid = userRecords.ToString();
                    userRecords = dtprodRecord.Rows.Count;
                    ViewBag.startpoint = "1";
                    ViewBag.endpoint = userRecords.ToString();
                    ViewBag.ProductList = dtprodRecord;
                }


                if (submit == "Last")
                {
                    string previous = objproduct1.previous1;
                    string next = objproduct1.next1;
                    userRecords = 0;
                    string startid = objproduct1.next1;
                    string startpointn = objproduct1.startpoint;
                    string endpointn = objproduct1.endpoint;
                    int spoint = 0, epoint = 0;
                    dtprodRecordtotal = obj.BindProucttotal();

                    string totrecord1 = dtprodRecordtotal.Rows[0].ItemArray[0].ToString();

                    dtprodRecord = obj.BindProuctnewLast();
                    userRecords = dtprodRecord.Rows.Count;

                    if (userRecords > 0)
                    {
                        if (userRecords >= 50)
                        {
                            spoint = Convert.ToInt32(totrecord1) - 49;
                        }
                        else
                        {
                            spoint = Convert.ToInt32(startpointn);
                        }
                        epoint = Convert.ToInt32(totrecord1);
                        ViewBag.startpoint = spoint.ToString();
                        ViewBag.endpoint = epoint.ToString();

                        int previous1 = Convert.ToInt32(totrecord1) - 50;

                        ViewBag.previousid = previous1.ToString();
                        ViewBag.nextid = totrecord1;
                    }

                    else
                    {
                        ViewBag.startpoint = startpointn.ToString();
                        ViewBag.endpoint = endpointn.ToString();


                        ViewBag.previousid = previous.ToString();
                        ViewBag.nextid = next.ToString();
                    }
                    ViewBag.ProductList = dtprodRecord;

                }
                if (submit == "Delete")
                {

                    string proid = Request["txtproid"];

                    string delimStr = ",";
                    char[] delimiter = delimStr.ToCharArray();
                    int id = 0;
                    try
                    {
                        foreach (string s in proid.Split(delimiter))
                        {
                            int OrderBy = 0;
                            id = Convert.ToInt16(s);
                            objProdt.Id = id;
                            DataTable dtCheckPrice = objProdt.BindProuct(objProdt.Id);
                            if (dtCheckPrice.Rows.Count > 0)
                            {
                                OrderBy = Convert.ToInt32(dtCheckPrice.Rows[0]["OrderBy"]);
                            }


                            objProdt.DeleteProductFavourite(id);
                            objProdt.DeleteProductOrderDetail(id);
                            int delresult = objProdt.DeleteProduct(id, OrderBy);
                            if (delresult > 0)
                            {
                                msg = "Product Deleted Successfully";
                                Session["Msg"] = msg;
                            }
                        }

                        return RedirectToAction("ProductList");

                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        if (ex.Message.ToLower().Contains("fk_orderdetail_product"))
                        {
                            TempData["error"] = String.Format("You can not deleted. Child record found.");
                        }
                        else
                            throw ex;
                    }
                    catch (Exception ex)
                    {
                        TempData["error"] = String.Format("First Select Product to delete");
                    }
                }
                
            }


            else
            {
                objProdt.Id =Convert.ToInt32(submit);
               string fname = null, path = null;
                HttpPostedFileBase document1 = Request.Files[submit + "Document1"];
                string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png" };
                if (document1 != null)
                {
                    if (document1.ContentLength > 0)
                    {
                        try
                        {
                            HttpFileCollectionBase files = Request.Files;
                            HttpPostedFileBase file = Document1;
                            //Resize image 500*300 coding
                            WebImage img = new WebImage(file.InputStream);
                            img.Resize(300, 300, false, false);
                            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            string extension = Path.GetExtension(file.FileName);
                            if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                            {
                                string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                fname = testfiles[testfiles.Length - 1];
                            }
                            else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                            {
                                msg = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
                            }
                            else
                            {
                                fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                            }
                            path = Path.Combine(Server.MapPath("~/image/product/"), fname);
                            img.Save(path);
                            // file.SaveAs(path);
                            objProdt.Image = fname;
                        }
                        catch (Exception ex)
                        {
                            msg = "Error occurred. Error details: " + ex.Message;
                        }
                    }
                    else
                    {
                        msg = "File Not Get Updated!!!";
                        var existfile = "";
                        DataTable dt1 = objProdt.BindProuct(objProdt.Id);
                        ViewBag.Image = dt1.Rows[0]["Image"].ToString();
                        existfile = ViewBag.Image;
                        objProdt.Image = existfile;
                    }
                }
                else
                {
                    msg = "File Not Get Updated!!!";
                    var existfile = "";
                    DataTable dt1 = objProdt.BindProuct(objProdt.Id);
                    ViewBag.Image = dt1.Rows[0]["Image"].ToString();
                    existfile = ViewBag.Image;
                    objProdt.Image = existfile;
                }
                //check MRP and SalePrice Changed or Not
                decimal MRPPrice = 0, SalePrice = 0, PurchasePrice=0;
                int OrderBy = 0;
                objProdt.Price =Convert.ToDecimal(Request[submit + "mrp"]);
                objProdt.SaleAmount= Convert.ToDecimal(Request[submit + "saleprice"]);
                objProdt.PurchaseAmount= Convert.ToDecimal(Request[submit + "purchase"]);
                objProdt.DiscountAmount= Convert.ToDecimal(Request[submit + "discount"]);
                objProdt.Profit= Convert.ToDecimal(Request[submit + "profitmargin"]);

                objProdt.CGST= Convert.ToDecimal(Request[submit + "cgst"]);
                objProdt.SGST= Convert.ToDecimal(Request[submit + "sgst"]);
                objProdt.IGST= Convert.ToDecimal(Request[submit + "igst"]);

                objProdt.ProductName = Request[submit + "proname"];

                decimal NewMRPPrice = objProdt.Price;
                decimal NewSalePrice = objProdt.SaleAmount;
                decimal NewPurchasePrice = objProdt.PurchaseAmount;
                DataTable dtCheckPrice = objProdt.BindProuct(objProdt.Id);
                if (dtCheckPrice.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtCheckPrice.Rows[0]["Price"].ToString()))
                        MRPPrice = Convert.ToDecimal(dtCheckPrice.Rows[0]["Price"]);
                    if (!string.IsNullOrEmpty(dtCheckPrice.Rows[0]["SalePrice"].ToString()))
                        SalePrice = Convert.ToDecimal(dtCheckPrice.Rows[0]["SalePrice"]);

                    if (!string.IsNullOrEmpty(dtCheckPrice.Rows[0]["PurchasePrice"].ToString()))
                        PurchasePrice = Convert.ToDecimal(dtCheckPrice.Rows[0]["PurchasePrice"]);
                }
                OrderBy =Convert.ToInt32(dtCheckPrice.Rows[0]["OrderBy"]);

                objProdt.OrderBy= Convert.ToInt32(Request[submit + "sortorder"]);

                int addresult = 0;
                if (MRPPrice == NewMRPPrice && SalePrice == NewSalePrice && PurchasePrice == NewPurchasePrice)
                {
                    addresult = objProdt.UpdateProductSelf(objProdt,OrderBy);
                }
                else
                {
                    addresult = objProdt.UpdateProductSelf(objProdt,OrderBy);
                    //update Pending order Prices
                    DataTable dtGetProductOrder = objProdt.GetProductWiseOrder(objProdt.Id);
                    if (dtGetProductOrder.Rows.Count > 0)
                    {

                    }

                    if (Convert.ToDecimal(MRPPrice) != Convert.ToDecimal(NewMRPPrice) || Convert.ToDecimal(SalePrice) != Convert.ToDecimal(NewSalePrice) || Convert.ToDecimal(PurchasePrice) != Convert.ToDecimal(NewPurchasePrice))
                    {
                        DataTable ProductInTransaction = objProdt.ProductInTransaction1(objProdt.Id);
                        if (ProductInTransaction.Rows.Count > 0)
                        {

                            for (int i = 0; i < ProductInTransaction.Rows.Count; i++)
                            {
                                int orderid = Convert.ToInt32(ProductInTransaction.Rows[i].ItemArray[1].ToString());
                                int qty = Convert.ToInt32(ProductInTransaction.Rows[i].ItemArray[3].ToString());

                                double samount = Convert.ToDouble(qty) * Convert.ToDouble(NewSalePrice);

                                double pamount = Convert.ToDouble(qty) * Convert.ToDouble(NewPurchasePrice);
                                double mamount = Convert.ToDouble(qty) * Convert.ToDouble(NewMRPPrice);

                                int od = objProdt.UpdatePriceFutureOrder(orderid, samount, pamount, mamount, Convert.ToDouble(NewSalePrice));
                                


                            }


                        }
                    }
                }
                if (addresult > 0)
                {
                    msg = "Product Updated Successfully!!!";
                    
                    
                }
                else
                { msg = "Product Not Updated!!!";
                    
                }

                Session["Msg"] = msg;
            }

            DataTable dtcategory = new DataTable();
            dtcategory = objProdt.GetAllMaincategory();
            ViewBag.Category = dtcategory;

            return View();
        }

        public ActionResult ActiveProduct(string pid)
        {
            int updproductstatus = objProdt.updateActiveProductStatus(pid);
            return RedirectToAction("ProductList", "Product");
        }
        public ActionResult InActiveProduct(string pid)
        {
            int updproductstatus = objProdt.updateInActiveProductStatus(pid);
            return RedirectToAction("ProductList", "Product");
        }

        //category
        [HttpGet]
        public ActionResult AddProductCategory()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                var category = objProdt.GetAllMaincategory();
                ViewBag.lstCategory = category;

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddProductCategory(Product objProdt, FormCollection form, HttpPostedFileBase Document1)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                string active = Request["IsActive"].Split(',')[0];
                if (!string.IsNullOrEmpty(active))
                {
                    objProdt.IsActive = Convert.ToBoolean(active);
                }
                //check data duplicate
                DataTable dtDuplicateg = new DataTable();
                dtDuplicateg = objProdt.CheckDuplicate(objProdt.Category);
                if (dtDuplicateg.Rows.Count > 0)
                {
                    ViewBag.SuccessMsg = "Category Already Exits!!!";
                }
                else
                {
                    string fname = null, path = null;
                    HttpPostedFileBase document1 = Request.Files["Document1"];
                    string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png" };
                    if (document1 != null)
                    {
                        if (document1.ContentLength > 0)
                        {
                            try
                            {
                                HttpFileCollectionBase files = Request.Files;
                                HttpPostedFileBase file = Document1;
                                //Resize image 200*200 coding
                                WebImage img = new WebImage(file.InputStream);
                                img.Resize(200, 200, false, false);
                                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                                string extension = Path.GetExtension(file.FileName);
                                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                                {
                                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                    fname = testfiles[testfiles.Length - 1];
                                }
                                else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                                {
                                    ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
                                }
                                else
                                {
                                    fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                                }
                                fname = dHelper.RemoveIllegalCharacters(fname);
                                path = Path.Combine(Server.MapPath("~/image/productcategory/"), fname);
                                img.Save(path);
                                objProdt.Image = fname;
                            }

                            catch (Exception ex)
                            {
                                ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                            }
                        }
                    }
                    // int addresult = 0;
                    int addresult = objProdt.InsertProdtcategory(objProdt);
                    if (addresult > 0)
                    { ViewBag.SuccessMsg = "Category Inserted Successfully!!!"; }
                    else
                    { ViewBag.SuccessMsg = "Category Not Inserted!!!"; }
                }
                ModelState.Clear();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult EditProductCategory(int? id)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var category = objProdt.GetAllMaincategory();
                ViewBag.lstCategory = category;

                DataTable dt = objProdt.BindCategory(id);
                if (dt.Rows.Count > 0)
                {
                    // ViewBag.CategoryId = dt.Rows[0]["Id"].ToString();
                    if (!string.IsNullOrEmpty(dt.Rows[0]["ParentCategoryId"].ToString()))
                        ViewBag.ParentCaregoryId = dt.Rows[0]["ParentCategoryId"].ToString();
                    else
                        ViewBag.ParentCaregoryId = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["CategoryName"].ToString()))
                        ViewBag.CategoryName = dt.Rows[0]["CategoryName"].ToString();
                    else
                        ViewBag.CategoryName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["OrderBy"].ToString()))
                        ViewBag.OrderBy = dt.Rows[0]["OrderBy"].ToString();
                    else
                        ViewBag.OrderBy = "0";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Image"].ToString()))
                        ViewBag.Image = dt.Rows[0]["Image"].ToString();
                    else
                        ViewBag.Image = "";
                    ViewBag.Active = Convert.ToBoolean(dt.Rows[0]["IsActive"]);

                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditProductCategory(Product objProdt, FormCollection form, HttpPostedFileBase Document1)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                string active = Request["IsActive"];
                if (active == "on")
                {
                    bool add = Convert.ToBoolean(true);
                    objProdt.IsActive = Convert.ToBoolean(add);
                }
                else
                {
                    if (active != null)
                    {
                        bool add = Convert.ToBoolean(Request["IsActive"].Split(',')[0]);
                        objProdt.IsActive = Convert.ToBoolean(add);
                    }
                }
                //check data duplicate
                DataTable dtDuplicateg = new DataTable();
                dtDuplicateg = objProdt.CheckDuplicate(objProdt.Category);
                if (dtDuplicateg.Rows.Count > 0)
                {
                    int SId = Convert.ToInt32(dtDuplicateg.Rows[0]["Id"]);
                    if (SId == objProdt.Id)
                    {
                        string fname = null, path = null;
                        HttpPostedFileBase document1 = Request.Files["Document1"];
                        string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png" };
                        if (document1 != null)
                        {
                            if (document1.ContentLength > 0)
                            {
                                try
                                {
                                    HttpFileCollectionBase files = Request.Files;
                                    HttpPostedFileBase file = Document1;
                                    //Resize image 200*200 coding
                                    WebImage img = new WebImage(file.InputStream);
                                    img.Resize(200, 200, false, false);
                                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                                    string extension = Path.GetExtension(file.FileName);
                                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                                    {
                                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                        fname = testfiles[testfiles.Length - 1];
                                    }
                                    else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                                    {
                                        ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
                                    }
                                    else
                                    {
                                        fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                                    }
                                    path = Path.Combine(Server.MapPath("~/image/productcategory/"), fname);
                                    img.Save(path);
                                    objProdt.Image = fname;
                                }

                                catch (Exception ex)
                                {
                                    ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                                }
                            }
                            else
                            {
                                var existfile = "";
                                DataTable dt1 = objProdt.BindCategory(objProdt.Id);
                                ViewBag.Image = dt1.Rows[0]["Image"].ToString();
                                existfile = ViewBag.Image;
                                objProdt.Image = existfile;
                            }
                        }
                        else
                        {
                            var existfile = "";
                            DataTable dt1 = objProdt.BindCategory(objProdt.Id);
                            ViewBag.Image = dt1.Rows[0]["Image"].ToString();
                            existfile = ViewBag.Image;
                            objProdt.Image = existfile;
                        }

                        int addresult = objProdt.UpdateProdtcategory(objProdt);
                        if (addresult > 0)
                        {
                            ViewBag.SuccessMsg = "Category Updated Successfully!!!";
                        }
                        else
                        { ViewBag.SuccessMsg = "Category Not Updated!!!"; }
                    }
                    else
                    { ViewBag.SuccessMsg = "Category Already Exits!!!"; }

                }


                DataTable dt = objProdt.BindCategory(objProdt.Id);
                if (!string.IsNullOrEmpty(dt.Rows[0]["CategoryName"].ToString()))
                    ViewBag.CategoryName = dt.Rows[0]["CategoryName"].ToString();
                else
                    ViewBag.CategoryName = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["Image"].ToString()))
                    ViewBag.Image = dt.Rows[0]["Image"].ToString();
                else
                    ViewBag.Image = "";
                ViewBag.Active = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult ProductCategoryList()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                DataTable dt = new DataTable();
                dt = objProdt.BindCategory(null);
                ViewBag.CategoryList = dt;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult DeleteProductCategory(int id)
        {
            try
            {
                int delresult = objProdt.DeleteCategory(id);
                if (delresult == 1)
                    return Redirect("/Category/Index");
                else
                    TempData["error"] = String.Format("You can not deleted. Child record found.");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_product_category"))
                {
                    TempData["error"] = String.Format("You can not deleted. Child record found.");
                }
                else
                    throw ex;
            }
            catch (Exception ex)
            {
                return View();
            }
            return Redirect("/Category/Index");            
        }

        public ActionResult ActiveProductCateg(string pid)
        {
            if (Session["UserName"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                int updproductstatus = objProdt.updateActiveProductCatgStatus(pid);

                return RedirectToAction("ProductCategoryList", "Product");
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }
        public ActionResult InActiveProductCateg(string pid)
        {
            if (Session["UserName"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                int updproductstatus = objProdt.updateInActiveProductCatgStatus(pid);

                return RedirectToAction("ProductCategoryList", "Product");
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        public JsonResult GetSubCategory(int CategoryId)
        {
            string code = string.Empty;
            StringBuilder sb = new StringBuilder();
            var dtcategory = objProdt.GetSubMaincategory(CategoryId);
            if (dtcategory.Rows.Count > 0)
            {
                sb.Append("<option value='0' >---Select SubCategory---</option>");
                for (int i = 0; i < dtcategory.Rows.Count; i++)
                {
                    sb.Append("<option value='" + dtcategory.Rows[i]["Id"] + "' >" + dtcategory.Rows[i]["CategoryName"] + " </option> ");
                }
            }

            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }


		public JsonResult setPrroductCategory(int CategoryId)
		{
			string code = string.Empty;
			StringBuilder sb = new StringBuilder();
			var dtcategory = objProdt.GetProductFromCategory(CategoryId);
			if (dtcategory.Rows.Count > 0)
			{
				sb.Append("<option value='0' >---Select Product---</option>");
				for (int i = 0; i < dtcategory.Rows.Count; i++)
				{
					sb.Append("<option value='" + dtcategory.Rows[i]["Id"] + "' >" + dtcategory.Rows[i]["ProductName"] + " </option> ");
				}
			}

			return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetSubCategorynew(int CategoryId)
        {
            string code = string.Empty;
            StringBuilder sb = new StringBuilder();
            var dtcategory = objProdt.GetSubMaincategorynew(CategoryId);
            if (dtcategory.Rows.Count > 0)
            {
                sb.Append("<option value='0' >---Select SubCategory---</option>");

                for (int i = 0; i < dtcategory.Rows.Count; i++)
                {
                    sb.Append("<option value='" + dtcategory.Rows[i]["Id"] + "' >" + dtcategory.Rows[i]["SubCatName"] + " </option> ");
                }
            }

            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }



        public PartialViewResult FetchAttributeList()
        {
            Vendor objvendor = new Vendor();
            Product objProdt = new Product();
            List<SubcatViewModel> list = new List<SubcatViewModel>();
            var Subcat = objProdt.BindAttributes();
            if (Subcat.Rows.Count > 0)
            {
                for (int i = 0; i < Subcat.Rows.Count; i++)
                {
                    list.Add(new SubcatViewModel { ID = Subcat.Rows[i]["ID"].ToString(), Name = Subcat.Rows[i]["Name"].ToString() });
                }
            }
            return PartialView("_ChkSubcatList", list);
        }

        [HttpGet]
        public ActionResult ProductParentCtaegoryList()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            
            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;


            DataTable dt = new DataTable();
            dt = objProdt.BindParentCategory(null);
            ViewBag.ProductParentCatList = dt;
            return View();


        }
        [HttpGet]
        public ActionResult ProductSubCategoryList()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");


            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;


            DataTable dt = new DataTable();
            dt = objProdt.BindSubCategory(null);
            ViewBag.ProductParentCatList = dt;
            return View();


        }

        [HttpGet]
        public ActionResult AddProductParentCtaegory()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                //var category = objProdt.GetAllMaincategory();
                //ViewBag.lstCategory = category;

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;
                return View();
            
            
        }

        [HttpGet]
        public ActionResult AddProductSubCategory()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            //var category = objProdt.GetAllMaincategory();
            //ViewBag.lstCategory = category;

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            DataTable dtcategory = new DataTable();
            dtcategory = objProdt.BindParentCategory(null);
            ViewBag.ParentCategory = dtcategory;
            return View();


        }

        [HttpPost]
        public ActionResult AddProductParentCtaegory(Product objProdt, FormCollection form, HttpPostedFileBase Document1)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

           

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;

            string active = Request["IsActive"].Split(',')[0];
                if (!string.IsNullOrEmpty(active))
                {
                    objProdt.IsActive = Convert.ToBoolean(active);
                }
                //check data duplicate
                DataTable dtDuplicateg = new DataTable();
                dtDuplicateg = objProdt.CheckDuplicateParentCat(objProdt.Category);
                if (dtDuplicateg.Rows.Count > 0)
                {
                    ViewBag.SuccessMsg = "Parent Category Already Exits!!!";
                }
                else
                {
                    string fname = null, path = null;
                    HttpPostedFileBase document1 = Request.Files["Document1"];
                    string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png" };
                    if (document1 != null)
                    {
                        if (document1.ContentLength > 0)
                        {
                            try
                            {
                                HttpFileCollectionBase files = Request.Files;
                                HttpPostedFileBase file = Document1;
                                //Resize image 200*200 coding
                                WebImage img = new WebImage(file.InputStream);
                                img.Resize(200, 200, false, false);
                                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                                string extension = Path.GetExtension(file.FileName);
                                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                                {
                                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                    fname = testfiles[testfiles.Length - 1];
                                }
                                else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                                {
                                    ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
                                }
                                else
                                {
                                    fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                                }
                               // fname = dHelper.RemoveIllegalCharacters(fname);
                                path = Path.Combine(Server.MapPath("~/image/productcategory/"), fname);
                                img.Save(path);
                                objProdt.Image = fname;
                            }

                            catch (Exception ex)
                            {
                                ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                            }
                        }
                    }
                    // int addresult = 0;
                    int addresult = objProdt.InsertProdtParentcategory(objProdt);
                    if (addresult > 0)
                    { ViewBag.SuccessMsg = "Parent Category Inserted Successfully!!!"; }
                    else
                    { ViewBag.SuccessMsg = "Parent Category Not Inserted!!!"; }
                }
                ModelState.Clear();
                return View();
           
        }



        [HttpPost]
        public ActionResult AddProductSubCategory(Product objProdt, FormCollection form, HttpPostedFileBase Document1)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");



            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;

            string active = Request["IsActive"].Split(',')[0];
            if (!string.IsNullOrEmpty(active))
            {
                objProdt.IsActive = Convert.ToBoolean(active);
            }

            string pid = Request["ddlCategory"];
            ViewBag.ParentCategoryName = pid;
            objProdt.CategoryId = Convert.ToInt32(pid);
            //check data duplicate
            DataTable dtDuplicateg = new DataTable();
            dtDuplicateg = objProdt.CheckDuplicateSubCat(pid,objProdt.SubCategory);
            if (dtDuplicateg.Rows.Count > 0)
            {
                ViewBag.SuccessMsg = "Sub Category Already Exits!!!";
            }
            else
            {
                string fname = null, path = null;
                HttpPostedFileBase document1 = Request.Files["Document1"];
                string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png" };
                if (document1 != null)
                {
                    if (document1.ContentLength > 0)
                    {
                        try
                        {
                            HttpFileCollectionBase files = Request.Files;
                            HttpPostedFileBase file = Document1;
                            //Resize image 200*200 coding
                            WebImage img = new WebImage(file.InputStream);
                            img.Resize(200, 200, false, false);
                            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            string extension = Path.GetExtension(file.FileName);
                            if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                            {
                                string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                fname = testfiles[testfiles.Length - 1];
                            }
                            else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                            {
                                ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
                            }
                            else
                            {
                                fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                            }
                            // fname = dHelper.RemoveIllegalCharacters(fname);
                            path = Path.Combine(Server.MapPath("~/image/subcategory/"), fname);
                            img.Save(path);
                            objProdt.Image = fname;
                        }

                        catch (Exception ex)
                        {
                            ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                        }
                    }
                }
                // int addresult = 0;
                int addresult = objProdt.InsertProdtSubcategory(objProdt);
                if (addresult > 0)
                { ViewBag.SuccessMsg = "Sub Category Inserted Successfully!!!"; }
                else
                { ViewBag.SuccessMsg = "Sub Category Not Inserted!!!"; }
            }
            ModelState.Clear();
            return View();

        }

        [HttpGet]
        public ActionResult EditParentCategory(int? id)
        {

            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            //var category = objProdt.GetAllMaincategory();
            //ViewBag.lstCategory = category;

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            var category = objProdt.GetAllParentcategory();
                ViewBag.lstCategory = category;


            DataTable dt = objProdt.BindParentCategory(id);


            if (dt.Rows.Count > 0)
                {
                // ViewBag.CategoryId = dt.Rows[0]["Id"].ToString();

                if (!string.IsNullOrEmpty(dt.Rows[0]["Id"].ToString()))
                    ViewBag.ParentCaregoryId = dt.Rows[0]["Id"].ToString();
                else
                    ViewBag.ParentCaregoryId = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["ParentCategory"].ToString()))
                    ViewBag.CategoryName = dt.Rows[0]["ParentCategory"].ToString();
                else
                    ViewBag.CategoryName = "";
               
                    if (!string.IsNullOrEmpty(dt.Rows[0]["OrderBy"].ToString()))
                        ViewBag.OrderBy = dt.Rows[0]["OrderBy"].ToString();
                    else
                        ViewBag.OrderBy = "0";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Image"].ToString()))
                        ViewBag.Image = dt.Rows[0]["Image"].ToString();
                    else
                        ViewBag.Image = "";
                    ViewBag.Active = Convert.ToBoolean(dt.Rows[0]["IsActive"]);

                }
                return View();
           
        }


        [HttpGet]
        public ActionResult EditSubCategory(int? id)
        {

            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            //var category = objProdt.GetAllMaincategory();
            //ViewBag.lstCategory = category;

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            var category = objProdt.GetAllParentcategory();
            ViewBag.lstCategory = category;

          
            DataTable dt = new DataTable();
            dt = objProdt.BindSubCategory(id);
            if (dt.Rows.Count > 0)
            {
                // ViewBag.CategoryId = dt.Rows[0]["Id"].ToString();
                if (!string.IsNullOrEmpty(dt.Rows[0]["ParentCategoryId"].ToString()))
                    ViewBag.ParentCategoryName = dt.Rows[0]["ParentCategoryId"].ToString();
                else
                    ViewBag.ParentCategoryName = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["SubCatName"].ToString()))
                    ViewBag.SubCatName = dt.Rows[0]["SubCatName"].ToString();
                else
                    ViewBag.SubCatName = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["OrderBy"].ToString()))
                    ViewBag.OrderBy = dt.Rows[0]["OrderBy"].ToString();
                else
                    ViewBag.OrderBy = "0";
                if (!string.IsNullOrEmpty(dt.Rows[0]["Image"].ToString()))
                    ViewBag.Image = dt.Rows[0]["Image"].ToString();
                else
                    ViewBag.Image = "";
                ViewBag.Active = Convert.ToBoolean(dt.Rows[0]["IsActive"]);

            }
            return View();

        }


        [HttpPost]
        public ActionResult EditParentCategory(Product objProdt, FormCollection form, HttpPostedFileBase Document1)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            string active = Request["IsActive"];
                if (active == "on")
                {
                    bool add = Convert.ToBoolean(true);
                    objProdt.IsActive = Convert.ToBoolean(add);
                }
                else
                {
                    if (active != null)
                    {
                        bool add = Convert.ToBoolean(Request["IsActive"].Split(',')[0]);
                        objProdt.IsActive = Convert.ToBoolean(add);
                    }
                }
                //check data duplicate
                DataTable dtDuplicateg = new DataTable();
                dtDuplicateg = objProdt.CheckDuplicateParentCat(objProdt.Category);
                if (dtDuplicateg.Rows.Count > 0)
                {
                    int SId = Convert.ToInt32(dtDuplicateg.Rows[0]["Id"]);
                    if (SId == objProdt.Id)
                    {
                        string fname = null, path = null;
                        HttpPostedFileBase document1 = Request.Files["Document1"];
                        string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png" };
                        if (document1 != null)
                        {
						if (document1 != null)
						{
							if (document1.ContentLength > 0)
							{
								try
								{
									string extension = Path.GetExtension(document1.FileName).ToLower();
									if (!sAllowedExt.Contains(extension))
									{
										ViewBag.Message = "Please upload a file of type: " + string.Join(", ", sAllowedExt);
									}
									else
									{
										
										WebImage img = new WebImage(document1.InputStream);
										img.Resize(300, 300, false, false);

										string fileName = Path.GetFileNameWithoutExtension(document1.FileName);
										fname = fileName + "_" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + extension;
										fname = fname.Replace(" ", "");

										path = Path.Combine(Server.MapPath("~/image/product/"), fname);
										img.Save(path);

										objProdt.MainImage = fname;
									}
								}
								catch (Exception ex)
								{
									ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
								}
							}
							else
							{
								ViewBag.SuccessMsg = "File not updated!";
								DataTable dt1 = objProdt.BindProuct(objProdt.Id);
								if (dt1.Rows.Count > 0)
								{
									var existingFile = dt1.Rows[0]["MainImage"].ToString();
									objProdt.MainImage = existingFile;  
								}
							}
						}

						else
						{
                            var existfile = "";
                            DataTable dt1 = objProdt.BindParentCategory(objProdt.Id);
                            ViewBag.Image = dt1.Rows[0]["Image"].ToString();
                            existfile = ViewBag.Image;
                            objProdt.Image = existfile;
                        }
                        }
                        else
                        {
                        var existfile = "";
                        DataTable dt1 = objProdt.BindParentCategory(objProdt.Id);
                        ViewBag.Image = dt1.Rows[0]["Image"].ToString();
                        existfile = ViewBag.Image;
                        objProdt.Image = existfile;
                    }

                    int addresult = objProdt.UpdateProdtParentcategory(objProdt);
                    if (addresult > 0)
                    {
                        ViewBag.SuccessMsg = "Category Updated Successfully!!!";
                    }
                    else
                    { ViewBag.SuccessMsg = "Category Not Updated!!!"; }
                }
                    else
                    { ViewBag.SuccessMsg = "Category Already Exits!!!"; }

                }


            DataTable dt = objProdt.BindParentCategory(objProdt.Id);
            if (!string.IsNullOrEmpty(dt.Rows[0]["ParentCategory"].ToString()))
                ViewBag.CategoryName = dt.Rows[0]["ParentCategory"].ToString();
            else
                ViewBag.CategoryName = "";
            if (!string.IsNullOrEmpty(dt.Rows[0]["Image"].ToString()))
                ViewBag.Image = dt.Rows[0]["Image"].ToString();
            else
                ViewBag.Image = "";
            ViewBag.Active = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
            return View();
           
        }


        [HttpPost]
        public ActionResult EditSubCategory(Product objProdt, FormCollection form, HttpPostedFileBase Document1)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            string active = Request["IsActive"];
            if (active == "on")
            {
                bool add = Convert.ToBoolean(true);
                objProdt.IsActive = Convert.ToBoolean(add);
            }
            else
            {
                if (active != null)
                {
                    bool add = Convert.ToBoolean(Request["IsActive"].Split(',')[0]);
                    objProdt.IsActive = Convert.ToBoolean(add);
                }
            }
            //check data duplicate
            string pid = Request["ddlCategory"];
            ViewBag.ParentCategoryName = pid;
            objProdt.CategoryId = Convert.ToInt32(pid);
            //check data duplicate
            DataTable dtDuplicateg = new DataTable();
            dtDuplicateg = objProdt.CheckDuplicateSubCat(pid, objProdt.SubCategory);
            //if (dtDuplicateg.Rows.Count > 0)
            //{
            //    ViewBag.SuccessMsg = "Sub Category Already Exits!!!";
            //}
            if (dtDuplicateg.Rows.Count > 0)
            {
                int SId = Convert.ToInt32(dtDuplicateg.Rows[0]["Id"]);
                if (SId == objProdt.Id)
                {
                    string fname = null, path = null;
                    HttpPostedFileBase document1 = Request.Files["Document1"];
                    string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png" };
                    if (document1 != null)
                    {
                        if (document1.ContentLength > 0)
                        {
                            try
                            {
                                HttpFileCollectionBase files = Request.Files;
                                HttpPostedFileBase file = Document1;
                                //Resize image 200*200 coding
                                WebImage img = new WebImage(file.InputStream);
                                img.Resize(200, 200, false, false);
                                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                                string extension = Path.GetExtension(file.FileName);
                                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                                {
                                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                    fname = testfiles[testfiles.Length - 1];
                                }
                                else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                                {
                                    ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
                                }
                                else
                                {
                                    fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                                }
                                path = Path.Combine(Server.MapPath("~/image/subcategory/"), fname);
                                img.Save(path);
                                objProdt.Image = fname;
                            }

                            catch (Exception ex)
                            {
                                ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                            }
                        }
                        else
                        {
                            var existfile = "";
                            DataTable dt1 = objProdt.BindSubCategory(objProdt.Id);
                            ViewBag.Image = dt1.Rows[0]["Image"].ToString();
                            existfile = ViewBag.Image;
                            objProdt.Image = existfile;
                        }
                    }
                    else
                    {
                        var existfile = "";
                        DataTable dt1 = objProdt.BindSubCategory(objProdt.Id);
                        ViewBag.Image = dt1.Rows[0]["Image"].ToString();
                        existfile = ViewBag.Image;
                        objProdt.Image = existfile;
                    }

                    int addresult = objProdt.UpdateProdtSubcategory(objProdt);
                    if (addresult > 0)
                    {
                        ViewBag.SuccessMsg = "SubCategory Updated Successfully!!!";
                    }
                    else
                    { ViewBag.SuccessMsg = "SubCategory Not Updated!!!"; }
                }
                else
                { ViewBag.SuccessMsg = "SubCategory Already Exits!!!"; }

            }




            else
            {

                string fname = null, path = null;
                HttpPostedFileBase document1 = Request.Files["Document1"];
                string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png" };
                if (document1 != null)
                {
                    if (document1.ContentLength > 0)
                    {
                        try
                        {
                            HttpFileCollectionBase files = Request.Files;
                            HttpPostedFileBase file = Document1;
                            //Resize image 200*200 coding
                            WebImage img = new WebImage(file.InputStream);
                            img.Resize(200, 200, false, false);
                            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            string extension = Path.GetExtension(file.FileName);
                            if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                            {
                                string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                fname = testfiles[testfiles.Length - 1];
                            }
                            else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                            {
                                ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
                            }
                            else
                            {
                                fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                            }
                            path = Path.Combine(Server.MapPath("~/image/subcategory/"), fname);
                            img.Save(path);
                            objProdt.Image = fname;
                        }

                        catch (Exception ex)
                        {
                            ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                        }
                    }
                    else
                    {
                        var existfile = "";
                        DataTable dt1 = objProdt.BindSubCategory(objProdt.Id);
                        ViewBag.Image = dt1.Rows[0]["Image"].ToString();
                        existfile = ViewBag.Image;
                        objProdt.Image = existfile;
                    }
                }
                else
                {
                    var existfile = "";
                    DataTable dt1 = objProdt.BindSubCategory(objProdt.Id);
                    ViewBag.Image = dt1.Rows[0]["Image"].ToString();
                    existfile = ViewBag.Image;
                    objProdt.Image = existfile;
                }

                int addresult = objProdt.UpdateProdtSubcategory(objProdt);
                if (addresult > 0)
                {
                    ViewBag.SuccessMsg = "SubCategory Updated Successfully!!!";
                }
                else
                { ViewBag.SuccessMsg = "SubCategory Not Updated!!!"; }
            }
            var category = objProdt.GetAllParentcategory();
            ViewBag.lstCategory = category;


            DataTable dt = new DataTable();

              dt = objProdt.BindSubCategory(objProdt.Id);
            if (dt.Rows.Count > 0)
            {
                // ViewBag.CategoryId = dt.Rows[0]["Id"].ToString();
                if (!string.IsNullOrEmpty(dt.Rows[0]["ParentCategoryId"].ToString()))
                    ViewBag.ParentCategoryName = dt.Rows[0]["ParentCategoryId"].ToString();
                else
                    ViewBag.ParentCategoryName = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["SubCatName"].ToString()))
                    ViewBag.SubCatName = dt.Rows[0]["SubCatName"].ToString();
                else
                    ViewBag.SubCatName = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["OrderBy"].ToString()))
                    ViewBag.OrderBy = dt.Rows[0]["OrderBy"].ToString();
                else
                    ViewBag.OrderBy = "0";
                if (!string.IsNullOrEmpty(dt.Rows[0]["Image"].ToString()))
                    ViewBag.Image = dt.Rows[0]["Image"].ToString();
                else
                    ViewBag.Image = "";
                ViewBag.Active = Convert.ToBoolean(dt.Rows[0]["IsActive"]);

            }
            return View();

        }

        [HttpGet]
        public ActionResult DeleteParentCategory(int id)
        {
            try
            {
                // int delresult = 0;
                Product  objproduct = new Product();
                int delresult = objproduct.DeleteProductParentcat(id);
                return RedirectToAction("ProductParentCtaegoryList");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_staff_staffcustassign"))
                {
                    TempData["error"] = String.Format("You can not deleted. Child record found.");
                }
                else
                    throw ex;
            }
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("ProductParentCtaegoryList");
        }


        [HttpGet]
        public ActionResult DeleteSubCategory(int id)
        {
            try
            {
                // int delresult = 0;
                Product objproduct = new Product();
                int delresult = objproduct.DeleteProductSubcat(id);
                return RedirectToAction("ProductSubCategoryList");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_staff_staffcustassign"))
                {
                    TempData["error"] = String.Format("You can not deleted. Child record found.");
                }
                else
                    throw ex;
            }
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("ProductSubCategoryList");
        }
    }
}
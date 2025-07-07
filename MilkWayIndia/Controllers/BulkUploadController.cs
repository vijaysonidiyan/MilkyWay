using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MilkWayIndia.Models;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Data.SqlClient;
using MilkWayIndia.Concrete;
using MilkWayIndia.Entity;
using MilkWayIndia.Abstract;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace MilkWayIndia.Controllers
{
    public class BulkUploadController : Controller
    {
        string excel03 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
        string excel07 = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
        Helper dHelper = new Helper();
        Product objProduct = new Product();
        clsCommon _clsCommon = new clsCommon();

        private IBulkUpload _BulkRepo;
        private IProduct _ProductRepo;
        public BulkUploadController()
        {
            this._BulkRepo = new BulkUploadRepository();
            this._ProductRepo = new ProductRepository();
        }

        public ActionResult Index()
        {
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            PopulateGrid();
            return View();
        }

        public void PopulateGrid()
        {
            var bulkUpload = _BulkRepo.PendingBulkUpload();
            ViewBag.lstBulkUpload = bulkUpload;
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            int Count = 0;
            string filePath = string.Empty;
            if (postedFile != null)
            {
                try
                {
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    string extension = Path.GetExtension(postedFile.FileName);
                    string filename = dHelper.RemoveIllegalCharacters(Path.GetFileNameWithoutExtension(postedFile.FileName) + " " + DateTime.Now) + extension;
                    filePath = path + filename;
                    postedFile.SaveAs(filePath);

                    tbl_BulkUpload upload = new tbl_BulkUpload();
                    upload.FileName = postedFile.FileName;
                    upload.FilePath = "/Uploads/" + filename;
                    upload.UploadItem = 0;
                    upload.IsUpload = false;
                    upload.CreatedOn = Helper.indianTime;

                    List<tbl_Product_Temp> list = new List<tbl_Product_Temp>();
                    using (var sreader = new StreamReader(postedFile.InputStream))
                    {
                        string[] headers = sreader.ReadLine().Split(',');
                        while (!sreader.EndOfStream)
                        {
                            int col = 1;
                            string[] rows = sreader.ReadLine().Split(',');
                            tbl_Product_Temp product = new tbl_Product_Temp();
                            if (!string.IsNullOrEmpty(rows[0].ToString()))
                                product.SortOrder = Convert.ToInt32(rows[0].ToString());
                            else product.SortOrder = 0;
                            string _category = "";

                            if (!string.IsNullOrEmpty(rows[1].ToString()))
                                _category = rows[1].ToString() + "|";
                            if (!string.IsNullOrEmpty(rows[2].ToString()))
                                _category = _category + rows[2].ToString();
                            product.CategoryName = _category;
                            product.ProductName = rows[3].ToString(); col = 3;
                            product.ProductImage = rows[4].ToString(); col = 4;

                            if (!string.IsNullOrEmpty(rows[5].ToString()))
                                product.MRP = Convert.ToDecimal(rows[5].ToString());
                            else product.MRP = 0;

                            if (!string.IsNullOrEmpty(rows[6].ToString()))
                                product.SalePrice = Convert.ToDecimal(rows[6].ToString());
                            else product.SalePrice = 0;

                            if (!string.IsNullOrEmpty(rows[7].ToString()))
                                product.PurchasePrice = Convert.ToDecimal(rows[7].ToString());
                            else product.PurchasePrice = 0;

                            if (!string.IsNullOrEmpty(rows[8].ToString()))
                                product.CGST = Convert.ToDecimal(rows[8].ToString());
                            else product.CGST = 0;

                            if (!string.IsNullOrEmpty(rows[9].ToString()))
                                product.SGST = Convert.ToDecimal(rows[9].ToString());
                            else product.SGST = 0;

                            if (!string.IsNullOrEmpty(rows[10].ToString()))
                                product.IGST = Convert.ToDecimal(rows[10].ToString());
                            else product.IGST = 0;

                            if (!string.IsNullOrEmpty(rows[11].ToString()))
                                product.ProfitL = Convert.ToDecimal(rows[11].ToString());
                            else product.ProfitL = 0;

                            if (!string.IsNullOrEmpty(rows[12].ToString()))
                                product.ProfitP = Convert.ToDecimal(rows[12].ToString());
                            else product.ProfitP = 0;

                            if (!string.IsNullOrEmpty(rows[13].ToString()))
                                product.Details = rows[13].ToString();
                            else product.Details = "";

                            if (!string.IsNullOrEmpty(rows[14].ToString()))
                                product.Subcription = Convert.ToDecimal(rows[14].ToString());
                            else product.Subcription = 0;

                            if (!string.IsNullOrEmpty(rows[15].ToString()))
                                product.YoutubeTitle = rows[15].ToString();
                            else product.YoutubeTitle = "";

                            if (!string.IsNullOrEmpty(rows[16].ToString()))
                                product.YoutubeURL = rows[16].ToString();
                            else product.YoutubeURL = "";

                            if (!string.IsNullOrEmpty(rows[17].ToString()))
                                product.Status = Convert.ToBoolean(rows[17].ToString());
                            else product.Status = false;

                            if (!string.IsNullOrEmpty(rows[18].ToString()))
                                product.IsDaily = Convert.ToBoolean(rows[18].ToString());
                            else product.IsDaily = false;

                            decimal? _gst = (product.CGST + product.SGST + product.IGST);
                            decimal? _gstAmount = (product.MRP * _gst) / 100;
                            if (product.ProfitL == 0 && product.ProfitP == 0)
                            {
                                decimal? _profitL = 0;
                                _profitL = product.MRP - product.PurchasePrice;
                                if (product.SalePrice == 0)
                                    product.SalePrice = product.PurchasePrice + _profitL;
                                else
                                    _profitL = product.SalePrice - product.PurchasePrice;
                                product.ProfitL = _profitL;
                                product.Profit = _profitL;
                            }
                            if (product.ProfitP > 0)
                            {
                                decimal? _profit = (product.SalePrice * product.ProfitP) / 100;
                                if (product.SalePrice == 0)
                                {
                                    product.PurchasePrice = product.MRP - _profit;
                                    product.SalePrice = product.PurchasePrice + _profit;
                                }
                                else
                                    product.PurchasePrice = product.SalePrice - _profit;
                                product.Profit = _profit;
                            }
                            if (product.ProfitL > 0)
                            {
                                decimal? _profit = product.ProfitL;
                                if (product.SalePrice == 0)
                                {
                                    product.PurchasePrice = product.MRP - _profit;
                                    product.SalePrice = product.PurchasePrice + _profit;
                                }
                                else
                                    product.PurchasePrice = product.SalePrice - _profit;                                
                                product.Profit = product.ProfitL;
                            }
                            product.CreatedOn = Helper.indianTime;
                            product.IsUpload = false;
                            product.ErrorMessage = "";
                            list.Add(product);
                            Count = Count + 1;
                        }
                    }
                    if (Count > 150)
                        ViewBag.SuccessMsg = "Only upload 150 products...";
                    else
                    {
                        upload.TotalItem = Count;
                        upload.list = list;
                        var result = _BulkRepo.SaveBulkUpload(upload);
                        ViewBag.SuccessMsg = "File upload successfully...";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.SuccessMsg = "Error in line. " + (Count + 1) + "   " + ex.Message;
                }
            }
            PopulateGrid();
            return View();
        }

        #region Excell Upload not Working on Share Hosting
        [HttpPost]
        public ActionResult Index_Old(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            if (postedFile != null)
            {
                try
                {
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    string extension = Path.GetExtension(postedFile.FileName);
                    string filename = dHelper.RemoveIllegalCharacters(Path.GetFileNameWithoutExtension(postedFile.FileName) + "_" + DateTime.Now) + extension;
                    filePath = path + filename;
                    postedFile.SaveAs(filePath);

                    string conString = string.Empty;
                    switch (extension)
                    {
                        case ".xls": //Excel 97-03.
                            conString = excel03;
                            break;
                        case ".xlsx": //Excel 07 and above.
                            conString = excel07;
                            break;
                    }

                    DataTable dt = new DataTable();
                    conString = string.Format(conString, filePath);
                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {
                                cmdExcel.Connection = connExcel;
                                //Get the name of First Sheet.
                                connExcel.Open();
                                DataTable dtExcelSchema;
                                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                connExcel.Close();

                                //Read Data from First Sheet.
                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(dt);
                                connExcel.Close();
                            }
                        }
                    }
                    if (dt.Rows.Count > 0)
                    {
                        tbl_BulkUpload upload = new tbl_BulkUpload();
                        upload.FileName = postedFile.FileName;
                        upload.FilePath = "/Uploads/" + filename;
                        upload.TotalItem = dt.Rows.Count;
                        upload.UploadItem = 0;
                        upload.IsUpload = false;
                        upload.CreatedOn = Helper.indianTime;

                        List<tbl_Product_Temp> list = new List<tbl_Product_Temp>();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            tbl_Product_Temp product = new tbl_Product_Temp();
                            if (!string.IsNullOrEmpty(dt.Rows[i]["Sort Order"].ToString()))
                                product.SortOrder = Convert.ToInt32(dt.Rows[i]["Sort Order"].ToString());
                            else product.SortOrder = 0;

                            product.CategoryName = dt.Rows[i]["Category"].ToString();
                            product.ProductName = dt.Rows[i]["Product Name"].ToString();
                            product.ProductImage = dt.Rows[i]["Product Image"].ToString();
                            if (!string.IsNullOrEmpty(dt.Rows[i]["MRP"].ToString()))
                                product.MRP = Convert.ToDecimal(dt.Rows[i]["MRP"].ToString());
                            else product.MRP = 0;

                            if (!string.IsNullOrEmpty(dt.Rows[i]["Sale Price"].ToString()))
                                product.SalePrice = Convert.ToDecimal(dt.Rows[i]["Sale Price"].ToString());
                            else product.SalePrice = 0;

                            if (!string.IsNullOrEmpty(dt.Rows[i]["Purchase Price"].ToString()))
                                product.PurchasePrice = Convert.ToDecimal(dt.Rows[i]["Purchase Price"].ToString());
                            else product.PurchasePrice = 0;

                            if (!string.IsNullOrEmpty(dt.Rows[i]["CGST"].ToString()))
                                product.CGST = Convert.ToDecimal(dt.Rows[i]["CGST"].ToString());
                            else product.CGST = 0;

                            if (!string.IsNullOrEmpty(dt.Rows[i]["SGST"].ToString()))
                                product.SGST = Convert.ToDecimal(dt.Rows[i]["SGST"].ToString());
                            else product.SGST = 0;

                            if (!string.IsNullOrEmpty(dt.Rows[i]["IGST"].ToString()))
                                product.IGST = Convert.ToDecimal(dt.Rows[i]["IGST"].ToString());
                            else product.IGST = 0;

                            if (!string.IsNullOrEmpty(dt.Rows[i]["Profit Lumpsump"].ToString()))
                                product.ProfitL = Convert.ToDecimal(dt.Rows[i]["Profit Lumpsump"].ToString());
                            else product.ProfitL = 0;

                            if (!string.IsNullOrEmpty(dt.Rows[i]["Profit"].ToString()))
                                product.ProfitP = Convert.ToDecimal(dt.Rows[i]["Profit"].ToString());
                            else product.ProfitP = 0;

                            if (!string.IsNullOrEmpty(dt.Rows[i]["Details"].ToString()))
                                product.Details = dt.Rows[i]["Details"].ToString();
                            else product.Details = "";

                            if (!string.IsNullOrEmpty(dt.Rows[i]["Subcription"].ToString()))
                                product.Subcription = Convert.ToInt32(dt.Rows[i]["Subcription"].ToString());
                            else product.Subcription = 0;

                            if (!string.IsNullOrEmpty(dt.Rows[i]["Youtube Title"].ToString()))
                                product.YoutubeTitle = dt.Rows[i]["Youtube Title"].ToString();
                            else product.YoutubeTitle = "";

                            if (!string.IsNullOrEmpty(dt.Rows[i]["Youtube URL"].ToString()))
                                product.YoutubeURL = dt.Rows[i]["Youtube URL"].ToString();
                            else product.YoutubeURL = "";

                            if (!string.IsNullOrEmpty(dt.Rows[i]["Status"].ToString()))
                                product.Status = Convert.ToBoolean(dt.Rows[i]["Status"].ToString());
                            else product.Status = false;

                            if (!string.IsNullOrEmpty(dt.Rows[i]["IsDaily"].ToString()))
                                product.IsDaily = Convert.ToBoolean(dt.Rows[i]["IsDaily"].ToString());
                            else product.IsDaily = false;

                            decimal? _gst = (product.CGST + product.SGST + product.IGST);
                            decimal? _gstAmount = (product.MRP * _gst) / 100;
                            if (product.ProfitL == 0 && product.ProfitP == 0)
                            {
                                decimal? _profitL = 0;
                                _profitL = product.MRP - product.PurchasePrice;
                                if (product.SalePrice == 0)
                                    product.SalePrice = product.PurchasePrice + _profitL;
                                else
                                    _profitL = product.SalePrice - product.PurchasePrice;
                                product.ProfitL = _profitL;
                                product.Profit = _profitL;
                            }
                            if (product.ProfitP > 0)
                            {
                                decimal? _profit = (product.MRP * product.ProfitP) / 100;
                                if (product.SalePrice == 0)
                                {
                                    product.PurchasePrice = product.MRP - _profit;
                                    product.SalePrice = product.PurchasePrice + _profit;
                                }
                                else
                                    product.PurchasePrice = product.SalePrice - _profit;
                                product.Profit = _profit;
                            }
                            product.CreatedOn = Helper.indianTime;
                            product.IsUpload = false;
                            product.ErrorMessage = "";
                            list.Add(product);
                        }
                        upload.list = list;
                        var result = _BulkRepo.SaveBulkUpload(upload);
                        ViewBag.SuccessMsg = "File upload successfully...";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.SuccessMsg = ex.Message;
                }
            }
            PopulateGrid();
            return View();
        }
        #endregion

        public ActionResult Detail(int? ID)
        {
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            ViewBag.UploadID = ID;
            var product = _BulkRepo.GetAllProduct(ID);
            ViewBag.lstProduct = product.Where(s => s.IsUpload == false);
            return View();            
        }

        public ActionResult Edit(int? ID)
        {
            var product = _BulkRepo.GetProductByID(ID);
            if (product != null)
            {
                ViewBag.UploadID = product.UploadID;
                ViewBag.IsActive = product.Status == null ? "" : "checked";
                ViewBag.IsDaily = product.IsDaily == null ? "" : "checked";
            }
            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(tbl_Product_Temp model)
        {
            model.IsDaily = Request.Form["IsDaily"] == "on" ? true : false;
            model.Status = Request.Form["IsActive"] == "on" ? true : false;
            _BulkRepo.UpdateProduct(model);
            ViewBag.SuccessMsg = "Successfully update product...";
            return View();
        }

        public ActionResult Delete(int? ID)
        {
            _BulkRepo.DeleteBulkUpload(ID);
            return Redirect("/BulkUpload/Index");
        }

        public ActionResult UploadProduct()
        {
            try
            {
                int UploadID = Convert.ToInt32(Request.Form["UploadID"]);
                var temp = _BulkRepo.GetAllProduct(UploadID).ToList();
                foreach (var item in temp)
                {
                    try
                    {
                        if (item.IsUpload == false)
                        {
                            tbl_Product_Master product = new tbl_Product_Master();
                            var cid = objProduct.GeCategoryIdByName(item.CategoryName);
                            //var category = objProduct.CheckDuplicate(item.CategoryName);
                            if (cid > 0)
                            {
                                product.CategoryId = cid;
                                product.ProductName = item.ProductName;
                                product.Code = "";
                                product.Price = item.MRP;
                                product.DiscountAmount = 0;
                                product.CGST = item.CGST;
                                product.IGST = item.IGST;
                                product.SGST = item.SGST;
                                product.RewardPoint = 0;
                                product.Subscription = item.Subcription;
                                product.Detail = item.Details;
                                product.YoutubeTitle = item.YoutubeTitle;
                                product.YoutubeURL = item.YoutubeURL;
                                product.IsActive = true;
                                product.IsDaily = item.IsDaily;
                                product.PurchasePrice = item.PurchasePrice;
                                product.SalePrice = item.SalePrice;
                                product.Profit = item.Profit;
                                product.OrderBy = item.SortOrder;

                                var response = _ProductRepo.SaveProduct(product);
                                if (response > 0)
                                    _BulkRepo.UpdateProductStatus(item.ID, true, "");

                                if (item.ProductImage != null)
                                {
                                    int j = 1;
                                    string[] img = item.ProductImage.Split('|');
                                    foreach (var m in img)
                                    {
                                        if (j == 1)
                                            product.Image = Helper.DownloadImageFromUrl(m);
                                        else
                                        {
                                            string fname = Helper.DownloadImageFromUrl(m);
                                            _clsCommon.insertdata("tbl_Product_Images", "(ProductId,Image,IsDefault)", "'" + response + "','" + fname + "','false'");
                                        }
                                        j++;
                                    }
                                }
                            }
                            else
                                _BulkRepo.UpdateProductStatus(item.ID, false, "Invalid CategoryName...");
                        }
                    }
                    catch (Exception ex)
                    {
                        _BulkRepo.UpdateProductStatus(item.ID, false, ex.Message);
                    }
                }
                _BulkRepo.UpdateBulkStatus(UploadID);
                return Redirect("/BulkUpload/Detail/" + UploadID);
            }
            catch(Exception ex)
            {

            }
            return Redirect("/BulkUpload/Index");
        }

        public ActionResult DownloadProduct()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                DownloadDropdown();
                return View();
            }
            else
            {
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            }
        }

        public void DownloadDropdown()
        {
            Sector _sector = new Sector();
            var lstSector = _sector.getSectorList(null);
            ViewBag.lstSector = lstSector;

            Vendor objvendor = new Vendor();
            DataTable dt1 = new DataTable();
            dt1 = objvendor.getVendorList(null);
            ViewBag.lstVendor = dt1;

            var lstCategory = dHelper.GetCategoryList();
            ViewBag.lstCategory = new SelectList(lstCategory, "Value", "Text");
        }

        [HttpPost]
        public ActionResult DownloadProduct(FormCollection frm)
        {
            DownloadDropdown();
            Vendor _vendor = new Vendor();
            string VendorId = Request["ddlVendor"];
            if (!string.IsNullOrEmpty(VendorId) && Convert.ToInt32(VendorId) != 0)
                _vendor.VendorId = Convert.ToInt32(VendorId);
            string SectorId = Request["ddlSector"];
            if (!string.IsNullOrEmpty(SectorId) && Convert.ToInt32(SectorId) != 0)
                _vendor.SectorId = Convert.ToInt32(SectorId);
            string categoryID = Request["ddlCategory"];
            if (!string.IsNullOrEmpty(categoryID) && Convert.ToInt32(categoryID) != 0)
                _vendor.CategoryId = Convert.ToInt32(categoryID);
            else
                _vendor.CategoryId = null;

            ViewBag.VendorId = _vendor.VendorId;
            ViewBag.SectorId = _vendor.SectorId;
            ViewBag.CategoryId = _vendor.CategoryId;

            var product = _vendor.getSectorVendorProductList(null, _vendor.SectorId, _vendor.VendorId, _vendor.CategoryId);
            if (product.Rows.Count > 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ProductId", typeof(Int64));
                dt.Columns.Add("CategoryName", typeof(string));
                dt.Columns.Add("ProductName", typeof(string));
                dt.Columns.Add("YoutubeTitle", typeof(string));
                dt.Columns.Add("YoutubeURL", typeof(string));
                dt.Columns.Add("Subscription", typeof(string));
                dt.Columns.Add("Price", typeof(string));
                dt.Columns.Add("CGST", typeof(string));
                dt.Columns.Add("SGST", typeof(string));
                dt.Columns.Add("IGST", typeof(string));
                dt.Columns.Add("PurchasePrice", typeof(string));
                dt.Columns.Add("SalePrice", typeof(string));
                dt.Columns.Add("Profit", typeof(string));
                dt.Columns.Add("IsDaily", typeof(string));
                for (int i = 0; i < product.Rows.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["ProductId"] = product.Rows[i]["ProductId"].ToString();
                    dr["CategoryName"] = product.Rows[i]["CategoryName"].ToString();
                    dr["ProductName"] = product.Rows[i]["ProductName"].ToString();
                    dr["CategoryName"] = product.Rows[i]["CategoryName"].ToString();
                    dr["YoutubeTitle"] = product.Rows[i]["YoutubeTitle"].ToString();
                    dr["YoutubeURL"] = product.Rows[i]["YoutubeURL"].ToString();
                    dr["Subscription"] = product.Rows[i]["Subscription"] == null ? "0" : product.Rows[i]["Subscription"].ToString();
                    dr["Price"] = product.Rows[i]["Price"].ToString();
                    dr["CGST"] = product.Rows[i]["CGST"].ToString();
                    dr["SGST"] = product.Rows[i]["CGST"].ToString();
                    dr["IGST"] = product.Rows[i]["IGST"].ToString();
                    dr["PurchasePrice"] = product.Rows[i]["PurchasePrice"].ToString();
                    dr["SalePrice"] = product.Rows[i]["SalePrice"].ToString();
                    dr["Profit"] = product.Rows[i]["Profit"].ToString();
                    dr["IsDaily"] = product.Rows[i]["IsDaily"].ToString();
                    dt.Rows.Add(dr);
                }
                var gv = new GridView();
                gv.DataSource = dt;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=DownloadProduct_" + DateTime.Now + ".xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
            }
            return View("DownloadProduct");
        }

        [HttpPost]
        public ActionResult updateproduct(HttpPostedFileBase postedFile)
        {
            int Count = 0;
            string filePath = string.Empty, Error = "";
            if (postedFile != null)
            {
                try
                {
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    string extension = Path.GetExtension(postedFile.FileName);
                    string filename = dHelper.RemoveIllegalCharacters(Path.GetFileNameWithoutExtension(postedFile.FileName) + " " + DateTime.Now) + extension;
                    filePath = path + filename;
                    postedFile.SaveAs(filePath);

                    using (var sreader = new StreamReader(postedFile.InputStream))
                    {
                        string[] headers = sreader.ReadLine().Split(',');
                        while (!sreader.EndOfStream)
                        {
                            try
                            {
                                string[] rows = sreader.ReadLine().Split(',');
                                tbl_Product_Master product = new tbl_Product_Master();
                                if (!string.IsNullOrEmpty(rows[0].ToString()))
                                    product.Id = Convert.ToInt32(rows[0].ToString());

                                var category = objProduct.CheckDuplicate(rows[1].ToString());
                                if (category.Rows.Count > 0)
                                    product.CategoryId = Convert.ToInt32(category.Rows[0]["Id"]);

                                product.ProductName = rows[2] == "" ? "" : rows[2].ToString();
                                product.YoutubeTitle = rows[3] == "" ? "" : rows[3].ToString();
                                product.YoutubeURL = rows[4] == "" ? "" : rows[4].ToString();
                                product.Subscription = rows[5] == "" ? 0 : Convert.ToDecimal(rows[5].ToString());
                                product.Price = rows[6] == "" ? 0 : Convert.ToDecimal(rows[6].ToString());
                                product.CGST = rows[7] == "" ? 0 : Convert.ToDecimal(rows[7].ToString());
                                product.SGST = rows[8] == "" ? 0 : Convert.ToDecimal(rows[8].ToString());
                                product.IGST = rows[9] == "" ? 0 : Convert.ToDecimal(rows[9].ToString());
                                product.PurchasePrice = rows[10] == "" ? 0 : Convert.ToDecimal(rows[10].ToString());
                                product.SalePrice = rows[11] == "" ? 0 : Convert.ToDecimal(rows[11].ToString());
                                product.Profit = rows[12] == "" ? 0 : Convert.ToDecimal(rows[12].ToString());
                                product.IsDaily = rows[13] == "" ? false : Convert.ToBoolean(rows[13].ToString());
                                _ProductRepo.BulkUpdateProduct(product);

                            }
                            catch
                            {
                                Error = (Count + 1) + ",";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            if (Error != "")
                ViewBag.SuccessMsg = "Error in line. " + Error;
            else
                ViewBag.SuccessMsg = "Successfully update all products...";
            PopulateGrid();
            return View("DownloadProduct");
        }
    }
}
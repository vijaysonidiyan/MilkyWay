using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using MilkWayIndia.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rotativa;
using System.Web.Helpers;
using MilkWayIndia.Entity;
using MilkWayIndia.Abstract;
using MilkWayIndia.Concrete;
using System.Text;

namespace MilkWayIndia.Controllers
{
    public class VendorController : Controller
    {

        public class ProMas
        {
            public int Procode { get; set; }
            public string ProName { get; set; }
            public int Vendorcode { get; set; }
        }

        public class ProductMas
        {
            public int Procode { get; set; }
            public string MRP { get; set; }
            public string Pprice { get; set; }
            public string Sprice { get; set; }
            
        }
        clsCommon _clsCommon = new clsCommon();
        Helper dHelper = new Helper();
        Dictionary<string, object> res = new Dictionary<string, object>();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
        Vendor objVendor = new Vendor();
        private IVendorCatSubcat _VendorRepo;

        public VendorController()
        {
            this._VendorRepo = new VendorRepository();
        }
        // GET: Vendor

        [HttpGet]
        public ActionResult AddVendor()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                Sector objsector = new Sector();
                DataTable dt = new DataTable();
                dt = objsector.getSectorList(null);
                ViewBag.Sector = dt;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddVendor(Vendor obj, FormCollection form, HttpPostedFileBase Photo)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                string sectorid = Request["ddlSector"];
                string vendortype = Request["ddlVendorType"];
                if (!string.IsNullOrEmpty(vendortype))
                {
                    obj.VendorType = vendortype;
                }
                if (!string.IsNullOrEmpty(sectorid))
                {
                    obj.SectorId = Convert.ToInt32(sectorid);
                }
                //check username
                DataTable dtuserRecord1 = new DataTable();
                dtuserRecord1 = obj.CheckVendorUserName(obj.UserName);
                int userRecords1 = dtuserRecord1.Rows.Count;
                if (userRecords1 > 0)
                {
                    ViewBag.SuccessMsg = "UserName Already Exits!!!";
                    return View();
                }

                //check data duplicate
                DataTable dtDupliStaff = new DataTable();
                dtDupliStaff = obj.CheckDuplicateVendor(obj.FirstName, obj.LastName, obj.MobileNo);
                if (dtDupliStaff.Rows.Count > 0)
                {
                    ViewBag.SuccessMsg = "Data Already Exits!!!";
                }
                else
                {
                    string fname = null, path = null;
                    HttpPostedFileBase document1 = Request.Files["Photo"];
                    string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png" };
                    if (document1 != null)
                    {
                        if (document1.ContentLength > 0)
                        {
                            try
                            {
                                HttpFileCollectionBase files = Request.Files;
                                HttpPostedFileBase file = Photo;
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
                                path = Path.Combine(Server.MapPath("~/image/vendorphoto/"), fname);
                                file.SaveAs(path);
                                obj.Photo = fname;
                            }

                            catch (Exception ex)
                            {
                                ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                            }
                        }
                    }
                    int addresult = obj.InsertVendor(obj);
                    if (addresult > 0)
                    { ViewBag.SuccessMsg = "Vendor Inserted Successfully!!!"; }
                    else
                    { ViewBag.SuccessMsg = "Vendor Not Inserted!!!"; }
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
        public ActionResult EditVendor(int id = 0)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                Sector objsector = new Sector();
                DataTable dtsec = new DataTable();
                dtsec = objsector.getSectorList(null);
                ViewBag.Sector = dtsec;

                DataTable dt = new DataTable();
                dt = objVendor.getVendorList(id);
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["SectorId"].ToString()))
                        ViewBag.SectorId = dt.Rows[0]["SectorId"].ToString();
                    else
                        ViewBag.SectorId = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["FirstName"].ToString()))
                        ViewBag.FirstName = dt.Rows[0]["FirstName"].ToString();
                    else
                        ViewBag.FirstName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["LastName"].ToString()))
                        ViewBag.LastName = dt.Rows[0]["LastName"].ToString();
                    else
                        ViewBag.LastName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["MobileNo"].ToString()))
                        ViewBag.MobileNo = dt.Rows[0]["MobileNo"].ToString();
                    else
                        ViewBag.MobileNo = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Email"].ToString()))
                        ViewBag.Email = dt.Rows[0]["Email"].ToString();
                    else
                        ViewBag.Email = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Address"].ToString()))
                        ViewBag.Address = dt.Rows[0]["Address"].ToString();
                    else
                        ViewBag.Address = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Photo"].ToString()))
                        ViewBag.Photo = dt.Rows[0]["Photo"].ToString();
                    else
                        ViewBag.Photo = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["UserName"].ToString()))
                        ViewBag.UserName = dt.Rows[0]["UserName"].ToString();
                    else
                        ViewBag.UserName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Password"].ToString()))
                        ViewBag.Password = dt.Rows[0]["Password"].ToString();
                    else
                        ViewBag.Password = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["CompanyName"].ToString()))
                        ViewBag.CompanyName = dt.Rows[0]["CompanyName"].ToString();
                    else
                        ViewBag.CompanyName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["PanCardNo"].ToString()))
                        ViewBag.PanCardNo = dt.Rows[0]["PanCardNo"].ToString();
                    else
                        ViewBag.PanCardNo = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["GSTNo"].ToString()))
                        ViewBag.GSTNo = dt.Rows[0]["GSTNo"].ToString();
                    else
                        ViewBag.GSTNo = "";


                    if (!string.IsNullOrEmpty(dt.Rows[0]["VendorType"].ToString()))
                        ViewBag.VendorType = dt.Rows[0]["VendorType"].ToString();
                    else
                        ViewBag.VendorType = "";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditVendor(Vendor obj, FormCollection form, HttpPostedFileBase Photo)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                Sector objsector = new Sector();
                DataTable dtsec = new DataTable();


                DataTable dt = new DataTable();
                string sectorid = Request["ddlSector"];
                string VendorType = Request["ddlVendorType"];
                if (!string.IsNullOrEmpty(sectorid))
                {
                    obj.SectorId = Convert.ToInt32(sectorid);
                }

                if (!string.IsNullOrEmpty(VendorType))
                {
                    obj.VendorType = VendorType;
                }
                //check data duplicate
                //DataTable dtDupliStaff = new DataTable();
                //dtDupliStaff = obj.CheckDuplicateVendor(obj.FirstName, obj.LastName, obj.MobileNo);
                //if (dtDupliStaff.Rows.Count > 0)
                //{
                //    int SId = Convert.ToInt32(dtDupliStaff.Rows[0]["Id"]);
                //    if (SId == obj.Id)
                //    {
                //check username
                DataTable dtuserRecord1 = new DataTable();
                dtuserRecord1 = obj.CheckVendorUserName(obj.UserName);
                int userRecords1 = dtuserRecord1.Rows.Count;
                if (userRecords1 > 0)
                {
                    int SId = Convert.ToInt32(dtuserRecord1.Rows[0]["Id"]);
                    if (SId == obj.Id)
                    {
                    }
                    else
                    {
                        ViewBag.SuccessMsg = "UserName Already Exits!!!";
                        dtsec = objsector.getSectorList(null);
                        ViewBag.Sector = dtsec;
                        dt = obj.getVendorList(obj.Id);
                        if (dt.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dt.Rows[0]["SectorId"].ToString()))
                                ViewBag.SectorId = dt.Rows[0]["SectorId"].ToString();
                            else
                                ViewBag.SectorId = "";
                            if (!string.IsNullOrEmpty(dt.Rows[0]["FirstName"].ToString()))
                                ViewBag.FirstName = dt.Rows[0]["FirstName"].ToString();
                            else
                                ViewBag.FirstName = "";
                            if (!string.IsNullOrEmpty(dt.Rows[0]["LastName"].ToString()))
                                ViewBag.LastName = dt.Rows[0]["LastName"].ToString();
                            else
                                ViewBag.LastName = "";
                            if (!string.IsNullOrEmpty(dt.Rows[0]["MobileNo"].ToString()))
                                ViewBag.MobileNo = dt.Rows[0]["MobileNo"].ToString();
                            else
                                ViewBag.MobileNo = "";
                            if (!string.IsNullOrEmpty(dt.Rows[0]["Email"].ToString()))
                                ViewBag.Email = dt.Rows[0]["Email"].ToString();
                            else
                                ViewBag.Email = "";
                            if (!string.IsNullOrEmpty(dt.Rows[0]["Address"].ToString()))
                                ViewBag.Address = dt.Rows[0]["Address"].ToString();
                            else
                                ViewBag.Address = "";
                            if (!string.IsNullOrEmpty(dt.Rows[0]["Photo"].ToString()))
                                ViewBag.Photo = dt.Rows[0]["Photo"].ToString();
                            else
                                ViewBag.Photo = "";
                            if (!string.IsNullOrEmpty(dt.Rows[0]["UserName"].ToString()))
                                ViewBag.UserName = dt.Rows[0]["UserName"].ToString();
                            else
                                ViewBag.UserName = "";
                            if (!string.IsNullOrEmpty(dt.Rows[0]["Password"].ToString()))
                                ViewBag.Password = dt.Rows[0]["Password"].ToString();
                            else
                                ViewBag.Password = "";
                            if (!string.IsNullOrEmpty(dt.Rows[0]["CompanyName"].ToString()))
                                ViewBag.CompanyName = dt.Rows[0]["CompanyName"].ToString();
                            else
                                ViewBag.CompanyName = "";
                            if (!string.IsNullOrEmpty(dt.Rows[0]["PanCardNo"].ToString()))
                                ViewBag.PanCardNo = dt.Rows[0]["PanCardNo"].ToString();
                            else
                                ViewBag.PanCardNo = "";
                            if (!string.IsNullOrEmpty(dt.Rows[0]["GSTNo"].ToString()))
                                ViewBag.GSTNo = dt.Rows[0]["GSTNo"].ToString();
                            else
                                ViewBag.GSTNo = "";
                        }
                        return View();
                    }
                }

                string fname = null, path = null;
                HttpPostedFileBase document1 = Request.Files["Photo"];
                string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png" };
                if (document1 != null)
                {
                    if (document1.ContentLength > 0)
                    {
                        try
                        {
                            HttpFileCollectionBase files = Request.Files;
                            HttpPostedFileBase file = Photo;
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
                            path = Path.Combine(Server.MapPath("~/image/vendorphoto/"), fname);
                            file.SaveAs(path);
                            obj.Photo = fname;
                        }
                        catch (Exception ex)
                        {
                            ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                        }
                    }
                    else
                    {
                        var existfile = "";
                        DataTable dt1 = obj.getVendorList(obj.Id);
                        ViewBag.Photo = dt1.Rows[0]["Photo"].ToString();
                        existfile = ViewBag.Photo;
                        obj.Photo = existfile;
                    }
                }
                else
                {
                    var existfile = "";
                    DataTable dt1 = obj.getVendorList(obj.Id);
                    ViewBag.Photo = dt1.Rows[0]["Photo"].ToString();
                    existfile = ViewBag.Photo;
                    obj.Photo = existfile;
                }
                int result = obj.UpdateVendor(obj);
                if (result > 0)
                {
                    ViewBag.SuccessMsg = "Vendor Updated Successfully!!!";
                }
                else
                { ViewBag.SuccessMsg = "Vendor Not Updated!!!"; }
                //}
                //else
                //{
                //    ViewBag.SuccessMsg = "Data Already Exits!!!";
                //}
                //}

                //  DataTable dt = new DataTable();
                dtsec = objsector.getSectorList(null);
                ViewBag.Sector = dtsec;

                dt = obj.getVendorList(obj.Id);
                if (dt.Rows.Count > 0)
                {

                    if (!string.IsNullOrEmpty(dt.Rows[0]["SectorId"].ToString()))
                        ViewBag.SectorId = dt.Rows[0]["SectorId"].ToString();
                    else
                        ViewBag.SectorId = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["FirstName"].ToString()))
                        ViewBag.FirstName = dt.Rows[0]["FirstName"].ToString();
                    else
                        ViewBag.FirstName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["LastName"].ToString()))
                        ViewBag.LastName = dt.Rows[0]["LastName"].ToString();
                    else
                        ViewBag.LastName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["MobileNo"].ToString()))
                        ViewBag.MobileNo = dt.Rows[0]["MobileNo"].ToString();
                    else
                        ViewBag.MobileNo = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Email"].ToString()))
                        ViewBag.Email = dt.Rows[0]["Email"].ToString();
                    else
                        ViewBag.Email = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Address"].ToString()))
                        ViewBag.Address = dt.Rows[0]["Address"].ToString();
                    else
                        ViewBag.Address = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Photo"].ToString()))
                        ViewBag.Photo = dt.Rows[0]["Photo"].ToString();
                    else
                        ViewBag.Photo = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["UserName"].ToString()))
                        ViewBag.UserName = dt.Rows[0]["UserName"].ToString();
                    else
                        ViewBag.UserName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Password"].ToString()))
                        ViewBag.Password = dt.Rows[0]["Password"].ToString();
                    else
                        ViewBag.Password = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["CompanyName"].ToString()))
                        ViewBag.CompanyName = dt.Rows[0]["CompanyName"].ToString();
                    else
                        ViewBag.CompanyName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["PanCardNo"].ToString()))
                        ViewBag.PanCardNo = dt.Rows[0]["PanCardNo"].ToString();
                    else
                        ViewBag.PanCardNo = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["GSTNo"].ToString()))
                        ViewBag.GSTNo = dt.Rows[0]["GSTNo"].ToString();
                    else
                        ViewBag.GSTNo = "";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult VendorList()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                DataTable dtList = new DataTable();
                dtList = objVendor.getVendorList(null);
                ViewBag.StaffList = dtList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult DeleteVendor(int id)
        {
            try
            {
                int delresult = objVendor.DeleteVendor(id);
                return RedirectToAction("VendorList");
            }
            //catch (System.Data.SqlClient.SqlException ex)
            //{
            //    if (ex.Message.ToLower().Contains("fk_staff_staffcustassign"))
            //    {
            //        TempData["error"] = String.Format("You can not deleted. Child record found.");
            //    }
            //    else
            //        throw ex;
            //}
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("VendorList");
        }

        //edit get existing table records
        public JsonResult GetExistProduct(int? id)
        {
            Product objprod = new Product();
            ArrayList list = new ArrayList();
            DataTable dtnew = objprod.BindCategActiveProuct(id);
            int rc = dtnew.Rows.Count;//for test 
            ////string[][] str;
            ////for (int i = 0; i < dtnew.Rows.Count; i++)
            ////{
            ////    str = new[] { dtnew.Rows[i].ItemArray.Select(x => x.ToString()).ToArray() };
            ////    list.AddRange(str);
            ////}

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dtnew.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dtnew.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            // return serializer.Serialize(rows);


            return Json(serializer.Serialize(rows), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddSectorProductAssign()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            Sector objsector = new Sector();
            DataTable dt = new DataTable();
            dt = objsector.getSectorList(null);
            ViewBag.Sector = dt;

            Product objProdt = new Product();
            DataTable dtcategory = new DataTable();
            dtcategory = objProdt.BindActiveCategory(null);
            ViewBag.Category = dtcategory;

            Vendor objvendor = new Vendor();
            DataTable dt1 = new DataTable();
            dt1 = objvendor.getVendorList(null);
            ViewBag.Vendor = dt1;

            return View();
        }

        public ActionResult AddSectorProductAssign(string SectorId, string VendorId, string CategoryId, string htmltbl, string json)//List<string[]> handsontbl
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            int tablecount = 0;
            int notaddcount = 0;
            Product objprodt = new Product();
            if (!string.IsNullOrEmpty(SectorId))
            {
                objVendor.SectorId = Convert.ToInt32(SectorId);
            }
            else
                objVendor.SectorId = 0;
            if (!string.IsNullOrEmpty(VendorId))
            {
                objVendor.VendorId = Convert.ToInt32(VendorId);
            }
            else
                objVendor.VendorId = 0;
            if (!string.IsNullOrEmpty(CategoryId))
            {
                objVendor.CategoryId = Convert.ToInt32(CategoryId);
            }
            else
                objVendor.CategoryId = 0;
            int AddSectorProduct = 0;
            Boolean IsSuccess = false;
            if (CategoryId != null)
            {
                DataTable dtNew = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
                // dtNew.Rows.Remove();

                dtNew.Rows.Remove(dtNew.Rows[0]);
                JArray jsonResponse = JArray.FromObject(dtNew.AsEnumerable().Select(r => r.ItemArray.ToList()).ToList());
                tablecount = dtNew.Rows.Count;

                int ic = 0;
                for (int i = 0; i < dtNew.Rows.Count; i++)
                {
                    if (i == 0 || (i % 2) == 0)
                    {
                        objVendor.IsActive = true;
                        if (objVendor.IsActive == true)
                        {
                            objVendor.ProductId = Convert.ToInt32(dtNew.Rows[i]["Product Name"].ToString());
                            //add  data 
                            if (objVendor.ProductId > 0)
                            {
                                //check Duplicate data
                                DataTable dtSectorProd = objVendor.CheckDuplicateSectProd(objVendor.SectorId, objVendor.VendorId, objVendor.ProductId);
                                if (dtSectorProd.Rows.Count > 0)
                                { notaddcount++; }
                                else
                                {
                                    AddSectorProduct = objVendor.InsertSectorProduct(objVendor);
                                    IsSuccess = true;
                                    ic++;
                                }
                            }
                        }
                    }
                }
                ViewBag.ic = ic;
            }
            string jsonString1 = string.Empty;
            if (tablecount == notaddcount)
            {
                jsonString1 = JsonConvert.SerializeObject(2);
            }
            else
            {
                if (IsSuccess == true)
                    jsonString1 = JsonConvert.SerializeObject(1);
                else
                    jsonString1 = JsonConvert.SerializeObject(3);
            }
            return Json(jsonString1, JsonRequestBehavior.AllowGet);
        }

        #region Product Assign Hitesh
        [HttpGet]
        public ActionResult ProductAssign()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            Sector objsector = new Sector();
            DataTable dt = new DataTable();
            dt = objsector.getSectorList(null);
            ViewBag.Sector = dt;

            Product objProdt = new Product();
            DataTable dtcategory = new DataTable();
            dt = objProdt.BindCategoryList(null);
            ViewBag.Category = dt;

            Vendor objvendor = new Vendor();
            DataTable dt1 = new DataTable();
            dt1 = objvendor.getVendorList(null);
            ViewBag.Vendor = dt1;

            return View();
        }

        public PartialViewResult GetProductByCategory(int id)
        {
            Product objprod = new Product();
            DataTable dtProduct = objprod.BindCategActiveProuct(id);
            ViewBag.lstProduct = dtProduct;
            return PartialView("_ProductList");
        }

        [HttpPost]
        public JsonResult ProductAssign(FormCollection frm, int[] chk)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            res["success"] = "0";
            int AddSectorProduct = 0, AddProduct = 0;
            objVendor.VendorId = Convert.ToInt32(frm["ddlVendor"]);
            objVendor.SectorId = Convert.ToInt32(frm["ddlSector"]);
            objVendor.CategoryId = Convert.ToInt32(frm["ddlCategory"]);
            objVendor.IsActive = true;
            if (chk != null)
            {
                foreach (var item in chk)
                {
                    objVendor.ProductId = item;
                    DataTable dtSectorProd = objVendor.CheckDuplicateSectProd(objVendor.SectorId, objVendor.VendorId, objVendor.ProductId);
                    if (dtSectorProd.Rows.Count == 0)
                    {
                        AddSectorProduct = objVendor.InsertSectorProduct(objVendor);
                        AddProduct++;
                    }
                }
            }
            if (AddProduct > 0)
            {
                res["success"] = "1";
                res["message"] = "Success!! Inserted Successfully.";
            }
            else
                res["success"] = "0";
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Product UnAssign Hitesh
        [HttpGet]
        public ActionResult ProductUnAssign()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            Sector objsector = new Sector();
            DataTable dt = new DataTable();
            dt = objsector.getSectorList(null);
            ViewBag.Sector = dt;

            Product objProdt = new Product();
            DataTable dtcategory = new DataTable();
            dt = objProdt.BindCategoryList(null);
            ViewBag.Category = dt;

            Vendor objvendor = new Vendor();
            DataTable dt1 = new DataTable();
            dt1 = objvendor.getVendorList(null);
            ViewBag.Vendor = dt1;

            return View();
        }

        public PartialViewResult GetCategoryProductAssigned(int Id, int sId, int vendorid)
        {
            Product objprod = new Product();
            DataTable dtProduct = objprod.GetVendorAssignedProduct(Id, sId, vendorid);
            List<ProductVM> product = new List<ProductVM>();
            if (dtProduct.Rows.Count > 0)
            {
                for (int i = 0; i < dtProduct.Rows.Count; i++)
                {
                    ProductVM p = new ProductVM();
                    if (!string.IsNullOrEmpty(dtProduct.Rows[i]["vID"].ToString()))
                        p.Value = Convert.ToInt32(dtProduct.Rows[i]["vID"].ToString());
                    if (!string.IsNullOrEmpty(dtProduct.Rows[i]["ProductName"].ToString()))
                        p.Text = dtProduct.Rows[i]["ProductName"].ToString();
                    p.IsChecked = true;
                    product.Add(p);
                }
            }
            ProductListVM _list = new ProductListVM();
            _list.ProductList = product;
            //ViewBag.lstProduct = dtProduct;
            return PartialView("_AssignProductList", _list);
        }

        [HttpPost]
        public JsonResult ProductUnAssign(FormCollection frm, ProductListVM model)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            res["success"] = "0";
            int AddSectorProduct = 0, AddProduct = 0;
            objVendor.VendorId = Convert.ToInt32(frm["ddlVendor"]);
            objVendor.SectorId = Convert.ToInt32(frm["ddlSector"]);
            objVendor.CategoryId = Convert.ToInt32(frm["ddlCategory"]);
            objVendor.IsActive = true;
            var chk = frm["chk"];

            foreach (var item in model.ProductList)
            {
                if (!item.IsChecked)
                {
                    AddSectorProduct = objVendor.DeleteSectorProductUnAssigned(item.Value);
                    AddProduct++;
                }
            }

            if (AddProduct > 0)
            {
                res["success"] = "1";
                res["message"] = "Success!! Update Successfully.";
            }
            else
                res["success"] = "0";
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpGet]
        public ActionResult SectorProductUnAssign()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                Sector objsector = new Sector();
                DataTable dt = new DataTable();
                dt = objsector.getSectorList(null);
                ViewBag.Sector = dt;

                Product objProdt = new Product();
                DataTable dtcategory = new DataTable();
                dtcategory = objProdt.BindActiveCategory(null);
                ViewBag.Category = dtcategory;

                Vendor objvendor = new Vendor();
                DataTable dt1 = new DataTable();
                dt1 = objvendor.getVendorList(null);
                ViewBag.Vendor = dt1;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult SectorProductUnAssign(string SectorId, string VendorId, string CategoryId, string htmltbl, string json)//List<string[]> handsontbl
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                int tablecount = 0;

                Product objprodt = new Product();
                if (!string.IsNullOrEmpty(SectorId))
                {
                    objVendor.SectorId = Convert.ToInt32(SectorId);
                }
                else
                    objVendor.SectorId = 0;
                if (!string.IsNullOrEmpty(VendorId))
                {
                    objVendor.VendorId = Convert.ToInt32(VendorId);
                }
                else
                    objVendor.VendorId = 0;
                if (!string.IsNullOrEmpty(CategoryId))
                {
                    objVendor.CategoryId = Convert.ToInt32(CategoryId);
                }
                else
                    objVendor.CategoryId = 0;
                int AddSectorProduct = 0;
                if (CategoryId != null)
                {
                    DataTable dtNew = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
                    // dtNew.Rows.Remove();

                    dtNew.Rows.Remove(dtNew.Rows[0]);
                    JArray jsonResponse = JArray.FromObject(dtNew.AsEnumerable().Select(r => r.ItemArray.ToList()).ToList());
                    tablecount = dtNew.Rows.Count;

                    int ic = 0;
                    for (int i = 0; i < dtNew.Rows.Count; i++)
                    {
                        if (i == 0 || (i % 2) == 0)
                        {
                            objVendor.IsActive = true;
                            if (objVendor.IsActive == true)
                            {
                                objVendor.ProductId = Convert.ToInt32(dtNew.Rows[i]["Product Name"].ToString());
                                //add  data 
                                if (objVendor.ProductId > 0)
                                {


                                    AddSectorProduct = objVendor.DeleteSectorProductUnAssigned(objVendor.ProductId);

                                }
                            }
                        }
                    }


                }
                string jsonString1 = string.Empty;

                if (AddSectorProduct > 0)
                    jsonString1 = JsonConvert.SerializeObject(1);
                else
                    jsonString1 = JsonConvert.SerializeObject(0);

                return Json(jsonString1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public JsonResult GetAssignedProduct(int? id, int? sId)
        {
            Product objprod = new Product();
            ArrayList list = new ArrayList();
            DataTable dtnew = objprod.BindSectAssignedProduct(id, sId);
            int rc = dtnew.Rows.Count;//for test 
            ////string[][] str;
            ////for (int i = 0; i < dtnew.Rows.Count; i++)
            ////{
            ////    str = new[] { dtnew.Rows[i].ItemArray.Select(x => x.ToString()).ToArray() };
            ////    list.AddRange(str);
            ////}

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dtnew.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dtnew.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            // return serializer.Serialize(rows);


            return Json(serializer.Serialize(rows), JsonRequestBehavior.AllowGet);
        }




        [HttpGet]
        public ActionResult SectorProductAssignList()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                Sector objsector = new Sector();
                DataTable dt = new DataTable();
                dt = objsector.getSectorList(null);
                ViewBag.Sector = dt;

                Product objProdt = new Product();
                DataTable dtcategory = new DataTable();
                dtcategory = objProdt.BindCategory(null);
                ViewBag.Category = dtcategory;

                Vendor objvendor = new Vendor();
                DataTable dt1 = new DataTable();
                dt1 = objvendor.getVendorList(null);
                ViewBag.Vendor = dt1;

                DataTable dtList = new DataTable();
                dtList = objVendor.getSectorProductList(null, null, null, null);
                ViewBag.ProductAssignList = dtList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult SectorProductAssignList(Vendor obj, FormCollection form)
        {
            if (Session["UserName"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                string SectorId = Request["ddlSector"];
                if (!string.IsNullOrEmpty(SectorId) && Convert.ToInt32(SectorId) == 0)
                {
                    objVendor.SectorId = Convert.ToInt32(SectorId);
                }
                else
                    objVendor.SectorId = null;

                string VendorId = Request["ddlVendor"];
                if (!string.IsNullOrEmpty(VendorId) && Convert.ToInt32(VendorId) == 0)
                {
                    objVendor.VendorId = Convert.ToInt32(VendorId);
                }
                else
                    objVendor.VendorId = null;

                string CategoryId = Request["ddlCategory"];
                if (!string.IsNullOrEmpty(CategoryId) && Convert.ToInt32(CategoryId) == 0)
                {
                    objVendor.CategoryId = Convert.ToInt32(CategoryId);
                }
                else
                    objVendor.CategoryId = null;

                DataTable dtList = new DataTable();
                dtList = objVendor.getSectorProductList(null, objVendor.SectorId, objVendor.VendorId, objVendor.CategoryId);
                ViewBag.ProductAssignList = dtList;
                return PartialView("SectorProductAssignPartial");
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }

        }

        [HttpPost]
        public ActionResult GetVendorList(int id)
        {
            DataTable dt = new DataTable();
            var vendorList = objVendor.GetVendorListBySector(id);
            List<SectorViewModel> list = new List<SectorViewModel>();
            if (vendorList.Rows.Count > 0)
            {
                for (int i = 0; i < vendorList.Rows.Count; i++)
                {
                    list.Add(new SectorViewModel { ID = vendorList.Rows[i]["Id"].ToString(), Name = vendorList.Rows[i]["VendorName"].ToString() });
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVendorList1(int id)
        {
            DataTable dt = new DataTable();
            var vendorList = objVendor.GetVendorListBySector(id);
            List<SectorViewModel> list = new List<SectorViewModel>();
            if (vendorList.Rows.Count > 0)
            {
                for (int i = 0; i < vendorList.Rows.Count; i++)
                {
                    list.Add(new SectorViewModel { ID = vendorList.Rows[i]["Id"].ToString(), Name = vendorList.Rows[i]["VendorName"].ToString() });
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DeleteSectorProductAssign(int id)
        {
            try
            {
                int delresult = objVendor.DeleteSectorProduct(id);
                return RedirectToAction("SectorProductAssignList");
            }
            //catch (System.Data.SqlClient.SqlException ex)
            //{
            //    if (ex.Message.ToLower().Contains("fk_staff_staffcustassign"))
            //    {
            //        TempData["error"] = String.Format("You can not deleted. Child record found.");
            //    }
            //    else
            //        throw ex;
            //}
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("SectorProductAssignList");
        }

        public ActionResult ActiveProductAssign(string pid)
        {
            if (Session["UserName"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                int updproductstatus = objVendor.updateActiveProductStatus(pid);

                return RedirectToAction("SectorProductAssignList", "Vendor");
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }
        public ActionResult InActiveProductAssign(string pid, string sid, string proid)
        {
            if (Session["UserName"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {

                Product objProdt = new Product();
                DataTable ProductInTransaction = objVendor.ProductInTransaction(Convert.ToInt32(proid), Convert.ToInt32(sid));
                if (ProductInTransaction.Rows.Count > 0)
                {

                    for (int i = 0; i < ProductInTransaction.Rows.Count; i++)
                    {
                        int orderid = Convert.ToInt32(ProductInTransaction.Rows[i].ItemArray[1].ToString());

                        int od = objProdt.DeleteFutureOrder(orderid);

                    }
                }



                int updproductstatus = objVendor.updateInActiveProductStatus(pid);



                return RedirectToAction("SectorProductAssignList", "Vendor");
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpGet]
        public ActionResult VendorProductOrder()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            Customer objcust = new Customer();
            CustomerOrder objorder = new CustomerOrder();
            Sector objsector = new Sector();

            DataTable dt = new DataTable();
            dt = objsector.getSectorList(null);
            ViewBag.Sector = dt;

            Product objProdt = new Product();
            DataTable dtcategory = new DataTable();
            dtcategory = objProdt.BindActiveCategory(null);
            ViewBag.Category = dtcategory;

            Vendor objvendor = new Vendor();
            DataTable dt1 = new DataTable();
            dt1 = objvendor.getVendorList(null);
            ViewBag.Vendor = dt1;

            DataTable dtList = new DataTable();
            dtList = objorder.getSectorVendorOrder(null, null, null, null);
            ViewBag.ProductorderList = dtList;

            return View();
        }


        [HttpGet]
        public ActionResult VendorOrderNotificationList()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            Customer objcust = new Customer();
            CustomerOrder objorder = new CustomerOrder();
            Sector objsector = new Sector();

            DataTable dt = new DataTable();
            dt = objsector.getSectorList(null);
            ViewBag.Sector = dt;

            Product objProdt = new Product();
            DataTable dtcategory = new DataTable();
            dtcategory = objProdt.BindActiveCategory(null);
            ViewBag.Category = dtcategory;

            Vendor objvendor = new Vendor();
            DataTable dt1 = new DataTable();
            dt1 = objvendor.getVendorList(null);
            ViewBag.Vendor = dt1;

            //DataTable dtList = new DataTable();
            //dtList = objorder.getSectorVendorOrder(null, null, null, null);
            //ViewBag.ProductorderList = dtList;
            CustomerOrder objorder1 = new CustomerOrder();
            DataTable dtList11 = new DataTable();
            dtList11 = objorder1.getSectorVendorOrderStatusnotification(null, null, System.DateTime.Now.Date.AddDays(1), System.DateTime.Now.Date.AddDays(2), null);
            ViewBag.ProductorderListnot = dtList11;
            ViewBag.ProductorderListcount = dtList11.Rows.Count;
            return View();
        }


        [HttpPost]
        public ActionResult VendorOrderNotificationList(FormCollection form, CustomerOrder objorder)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var submit = Request["submit"];
            Staff objstaff = new Staff();
            Sector objsector = new Sector();
            Subscription objsub = new Subscription();


            //

            CustomerOrder objorder1 = new CustomerOrder();
            DataTable dtList11 = new DataTable();
            dtList11 = objorder1.getSectorVendorOrderStatusnotification(null, null, System.DateTime.Now.Date.AddDays(1), System.DateTime.Now.Date.AddDays(2), null);
            ViewBag.ProductorderListnot = dtList11;
            ViewBag.ProductorderListcount = dtList11.Rows.Count;

            //


            string SectorId = Request["ddlSector"];
            if (!string.IsNullOrEmpty(SectorId) && Convert.ToInt32(SectorId) != 0)
            {
                objorder.SectorId = Convert.ToInt32(SectorId);
            }
            string VendorId = Request["ddlVendor"];
            if (!string.IsNullOrEmpty(VendorId) && Convert.ToInt32(VendorId) != 0)
            {
                objorder.VendorId = Convert.ToInt32(VendorId);
            }
            var fdate = Request["datepicker"];
            if (!string.IsNullOrEmpty(fdate.ToString()))
            {
                objorder.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
            }
            var tdate = Request["datepicker1"];
            if (!string.IsNullOrEmpty(tdate.ToString()))
            {
                objorder.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
            }
            var _fdate = objorder.FromDate.Value.ToString("dd-MM-yyyy");
            var _tdate = objorder.ToDate.Value.ToString("dd-MM-yyyy");
            if (submit == "submit")
            {
               



                DataTable dtList = new DataTable();
                dtList = objorder.getSectorVendorOrderStatusnotification(objorder.SectorId, objorder.VendorId, objorder.FromDate, objorder.ToDate,null);
                //dtList = objorder.getSectorVendorOrder(objorder.SectorId, objorder.VendorId, objorder.FromDate, objorder.ToDate);
                ViewBag.ProductorderList = dtList;

                DataTable dt = new DataTable();
                dt = objsector.getSectorList(null);
                ViewBag.Sector = dt;

                Product objProdt = new Product();
                DataTable dtcategory = new DataTable();
                dtcategory = objProdt.BindActiveCategory(null);
                ViewBag.Category = dtcategory;

                Vendor objvendor = new Vendor();
                DataTable dt1 = new DataTable();
                dt1 = objvendor.getVendorList(null);
                ViewBag.Vendor = dt1;

                ViewBag.SectorId = objorder.SectorId;
                ViewBag.VendorId = objorder.VendorId;
                if (!string.IsNullOrEmpty(fdate.ToString()))
                {
                    ViewBag.FromDate = fdate;
                }
                if (!string.IsNullOrEmpty(tdate.ToString()))
                {
                    ViewBag.ToDate = tdate;
                }
                ViewBag.btnpr = "true";
                return View();
            }
            else if (submit == "print")
            {
                string query = string.Format("SectorId={0}&VendorId={1}&FDate={2}&TDate={3}",
                    objorder.SectorId, objorder.VendorId, _fdate, _tdate);
                return Redirect("/Vendor/VendorProductOrderReport?" + query);
            }
            else if (submit == "export")
            {
                string query = string.Format("SectorId={0}&VendorId={1}&FDate={2}&TDate={3}",
                   objorder.SectorId, objorder.VendorId, _fdate, _tdate);

                CustomerOrder order = new CustomerOrder();
                ViewBag.FromDate = _fdate;
                var dtList = objorder.getSectorVendorOrder(objorder.SectorId, objorder.VendorId, objorder.FromDate, objorder.ToDate);
                ViewBag.ProductorderList = dtList;
                //return new UrlAsPdf("/Vendor/VendorProductOrderReport?" + query);
                var r = new PartialViewAsPdf("VendorProductOrderReport", new
                {
                    SectorId = objorder.SectorId,
                    VendorId = objorder.VendorId,
                    FDate = _fdate,
                    TDate = _tdate,
                })
                { FileName = "VendorProductOrderReport.pdf" };
                return r;
            }
            return View();
        }


        [HttpPost]
        public ActionResult VendorProductOrder(FormCollection form, CustomerOrder objorder)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var submit = Request["submit"];
            Staff objstaff = new Staff();
            Sector objsector = new Sector();
            Subscription objsub = new Subscription();
            string SectorId = Request["ddlSector"];
            if (!string.IsNullOrEmpty(SectorId) && Convert.ToInt32(SectorId) != 0)
            {
                objorder.SectorId = Convert.ToInt32(SectorId);
            }
            string VendorId = Request["ddlVendor"];
            if (!string.IsNullOrEmpty(VendorId) && Convert.ToInt32(VendorId) != 0)
            {
                objorder.VendorId = Convert.ToInt32(VendorId);
            }
            var fdate = Request["datepicker"];
            if (!string.IsNullOrEmpty(fdate.ToString()))
            {
                objorder.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
            }
            var tdate = Request["datepicker1"];
            if (!string.IsNullOrEmpty(tdate.ToString()))
            {
                objorder.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
            }
            var _fdate = objorder.FromDate.Value.ToString("dd-MM-yyyy");
            var _tdate = objorder.ToDate.Value.ToString("dd-MM-yyyy");
            if (submit == "submit")
            {
                DataTable dtList = new DataTable();
                dtList = objorder.getSectorVendorOrder(objorder.SectorId, objorder.VendorId, objorder.FromDate, objorder.ToDate);
                ViewBag.ProductorderList = dtList;

                DataTable dt = new DataTable();
                dt = objsector.getSectorList(null);
                ViewBag.Sector = dt;

                Product objProdt = new Product();
                DataTable dtcategory = new DataTable();
                dtcategory = objProdt.BindActiveCategory(null);
                ViewBag.Category = dtcategory;

                Vendor objvendor = new Vendor();
                DataTable dt1 = new DataTable();
                dt1 = objvendor.getVendorList(null);
                ViewBag.Vendor = dt1;

                ViewBag.SectorId = objorder.SectorId;
                ViewBag.VendorId = objorder.VendorId;
                if (!string.IsNullOrEmpty(fdate.ToString()))
                {
                    ViewBag.FromDate = fdate;
                }
                if (!string.IsNullOrEmpty(tdate.ToString()))
                {
                    ViewBag.ToDate = tdate;
                }
                ViewBag.btnpr = "true";
                return View();
            }
            else if (submit == "print")
            {
                string query = string.Format("SectorId={0}&VendorId={1}&FDate={2}&TDate={3}",
                    objorder.SectorId, objorder.VendorId, _fdate, _tdate);
                return Redirect("/Vendor/VendorProductOrderReport?" + query);
            }
            else if (submit == "export")
            {
                string query = string.Format("SectorId={0}&VendorId={1}&FDate={2}&TDate={3}",
                   objorder.SectorId, objorder.VendorId, _fdate, _tdate);

                CustomerOrder order = new CustomerOrder();
                ViewBag.FromDate = _fdate;
                var dtList = objorder.getSectorVendorOrder(objorder.SectorId, objorder.VendorId, objorder.FromDate, objorder.ToDate);
                ViewBag.ProductorderList = dtList;
                //return new UrlAsPdf("/Vendor/VendorProductOrderReport?" + query);
                var r = new PartialViewAsPdf("VendorProductOrderReport", new
                {
                    SectorId = objorder.SectorId,
                    VendorId = objorder.VendorId,
                    FDate = _fdate,
                    TDate = _tdate,
                })
                { FileName = "VendorProductOrderReport.pdf" };
                return r;
            }
            return View();
        }

        public ActionResult VendorProductOrderReport(int? SectorId, int? VendorId, string FDate, string TDate)
        {
            DateTime _fdate = DateTime.Now, _tdate = DateTime.Now;
            if (!string.IsNullOrEmpty(FDate.ToString()))
                _fdate = Convert.ToDateTime(DateTime.ParseExact(FDate, @"dd-MM-yyyy", null));
            if (!string.IsNullOrEmpty(TDate.ToString()))
                _tdate = Convert.ToDateTime(DateTime.ParseExact(TDate, @"dd-MM-yyyy", null));

            CustomerOrder order = new CustomerOrder();
            ViewBag.FromDate = _fdate.ToString("dd-MMM-yyyy");
            var dtList = order.getSectorVendorOrder(SectorId, VendorId, _fdate, _tdate);
            ViewBag.ProductorderList = dtList;
            return View();
        }

        public ActionResult VendorProductOrderReportStatus(int? SectorId, int? VendorId, DateTime? FDate, DateTime? TDate, string Status)
        {
            string date = "";
            if (!string.IsNullOrEmpty(FDate.ToString()))
                date = FDate.Value.ToString("dd-MMM-yyyy");
            else
                date = DateTime.Now.ToString("dd-MMM-yyyy");
            CustomerOrder order = new CustomerOrder();
            ViewBag.FromDate = date;
            var dtList = order.getSectorVendorOrderStatus(SectorId, VendorId, FDate, TDate, Status);
            ViewBag.ProductorderList = dtList;
            return View();
        }

        public ActionResult VendorProductOrderPrint(string SectorId, string VendorId, string FromDate, string ToDate)
        {
            CustomerOrder objorder = new CustomerOrder();
            if (SectorId == "0") SectorId = null;
            if (VendorId == "0") VendorId = null;
            string Filter = null;
            var fdate = FromDate;
            if (!string.IsNullOrEmpty(fdate.ToString()))
            {
                objorder.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
            }
            var tdate = ToDate;
            if (!string.IsNullOrEmpty(tdate.ToString()))
            {
                objorder.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
            }
            Filter = objorder.FromDate.Value.ToShortDateString() + " To " + objorder.ToDate.Value.ToShortDateString();
            DataTable dt = new DataTable();
            ReportDocument rd = new ReportDocument();
            rd.Load(Server.MapPath("~/Report/SectorWiseProductSummary.rpt"));
            // rd.SetDatabaseLogon("courtDB", "CC#1055in", @"154.16.126.38,2525\SQLEXPRESS", "courtDB");
            //SqlDataAdapter da = new SqlDataAdapter("Sector_Vendor_OrderTotal_SelectAll", con);
            SqlDataAdapter da = new SqlDataAdapter("Sector_Vendor_Daily_OrderTotal_SelectAll", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(SectorId))
                da.SelectCommand.Parameters.AddWithValue("@SectorId", SectorId);
            else
                da.SelectCommand.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(VendorId))
                da.SelectCommand.Parameters.AddWithValue("@VendorId", VendorId);
            else
                da.SelectCommand.Parameters.AddWithValue("@VendorId", DBNull.Value);
            if (!string.IsNullOrEmpty(FromDate.ToString()))
                da.SelectCommand.Parameters.AddWithValue("@FromDate", objorder.FromDate);
            else
                da.SelectCommand.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(ToDate.ToString()))
                da.SelectCommand.Parameters.AddWithValue("@ToDate", objorder.ToDate);
            else
                da.SelectCommand.Parameters.AddWithValue("@ToDate", DBNull.Value);
            da.Fill(dt);

            rd.Database.Tables[0].SetDataSource(dt);
            rd.SetParameterValue("@SectorId", SectorId);
            rd.SetParameterValue("@VendorId", VendorId);
            rd.SetParameterValue("@FromDate", FromDate);
            rd.SetParameterValue("@ToDate", ToDate);
            rd.SetParameterValue("@Filter", Filter);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //CrystalReportViewer1.RefreshReport();
            try
            {
                Stream str = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                str.Seek(0, SeekOrigin.Begin);
                return File(str, "application/pdf", "VendorDailyOrder.pdf");
            }
            catch (Exception e)
            {
                throw;
            }

        }

        [HttpGet]
        public ActionResult VendorProductOrderAmount()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            Customer objcust = new Customer();
            CustomerOrder objorder = new CustomerOrder();
            Sector objsector = new Sector();

            DataTable dt = new DataTable();
            dt = objsector.getSectorList(null);
            ViewBag.Sector = dt;

            Product objProdt = new Product();
            DataTable dtcategory = new DataTable();
            dtcategory = objProdt.BindActiveCategory(null);
            ViewBag.Category = dtcategory;

            Vendor objvendor = new Vendor();
            DataTable dt1 = new DataTable();
            dt1 = objvendor.getVendorList(null);
            ViewBag.Vendor = dt1;

            DataTable dtList = new DataTable();
            dtList = objorder.getSectorVendorOrder(null, null, null, null);
            ViewBag.ProductorderList = dtList;

            if (dtList.Rows.Count > 0)
            {
                Decimal TotalQty = Convert.ToDecimal(dtList.Compute("SUM(Qty)", string.Empty));
                Decimal TotalAmt = Convert.ToDecimal(dtList.Compute("SUM(PurchasePrice)", string.Empty));


                Decimal MRP = Convert.ToDecimal(dtList.Compute("SUM(MRP)", string.Empty));
                Decimal CustomerPrice = Convert.ToDecimal(dtList.Compute("SUM(CustomerPrice)", string.Empty));
                Decimal Profit = Convert.ToDecimal(dtList.Compute("SUM(Profit)", string.Empty));

                ViewBag.TotalQty = TotalQty;
                ViewBag.TotalAmt = TotalAmt;

                ViewBag.MRP = MRP;
                ViewBag.CustomerPrice = CustomerPrice;
                ViewBag.Profit = Profit;
            }
            return View();
        }

        [HttpPost]
        public ActionResult VendorProductOrderAmount(FormCollection form, CustomerOrder objorder)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            Staff objstaff = new Staff();
            Sector objsector = new Sector();
            Subscription objsub = new Subscription();
            string SectorId = Request["ddlSector"];
            if (!string.IsNullOrEmpty(SectorId) && Convert.ToInt32(SectorId) != 0)
            {
                objorder.SectorId = Convert.ToInt32(SectorId);
            }
            string VendorId = Request["ddlVendor"];
            if (!string.IsNullOrEmpty(VendorId) && Convert.ToInt32(VendorId) != 0)
            {
                objorder.VendorId = Convert.ToInt32(VendorId);
            }
            var fdate = Request["datepicker"];
            if (!string.IsNullOrEmpty(fdate.ToString()))
            {
                objorder.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
            }
            var tdate = Request["datepicker1"];
            if (!string.IsNullOrEmpty(tdate.ToString()))
            {
                objorder.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
            }
            DataTable dtList = new DataTable();
            dtList = objorder.getSectorVendorOrder(objorder.SectorId, objorder.VendorId, objorder.FromDate, objorder.ToDate);
            ViewBag.ProductorderList = dtList;

            if (dtList.Rows.Count > 0)
            {
                Decimal TotalQty = Convert.ToDecimal(dtList.Compute("SUM(Qty)", string.Empty));
                Decimal TotalAmt = Convert.ToDecimal(dtList.Compute("SUM(PurchasePrice)", string.Empty));


                Decimal MRP = Convert.ToDecimal(dtList.Compute("SUM(MRP)", string.Empty));
                Decimal CustomerPrice = Convert.ToDecimal(dtList.Compute("SUM(CustomerPrice)", string.Empty));
                Decimal Profit = Convert.ToDecimal(dtList.Compute("SUM(Profit)", string.Empty));

                ViewBag.TotalQty = TotalQty;
                ViewBag.TotalAmt = TotalAmt;

                ViewBag.MRP = MRP;
                ViewBag.CustomerPrice = CustomerPrice;
                ViewBag.Profit = Profit;


            }

            DataTable dt = new DataTable();
            dt = objsector.getSectorList(null);
            ViewBag.Sector = dt;

            Product objProdt = new Product();
            DataTable dtcategory = new DataTable();
            dtcategory = objProdt.BindActiveCategory(null);
            ViewBag.Category = dtcategory;

            Vendor objvendor = new Vendor();
            DataTable dt1 = new DataTable();
            dt1 = objvendor.getVendorList(null);
            ViewBag.Vendor = dt1;

            ViewBag.SectorId = objorder.SectorId;
            ViewBag.VendorId = objorder.VendorId;
            if (!string.IsNullOrEmpty(fdate.ToString()))
            {
                ViewBag.FromDate = fdate;
            }
            if (!string.IsNullOrEmpty(tdate.ToString()))
            {
                ViewBag.ToDate = tdate;
            }
            return View();
        }

        public ActionResult VendorProductOrderAmountPrint(string SectorId, string VendorId, string FromDate, string ToDate)
        {
            CustomerOrder objorder = new CustomerOrder();
            if (SectorId == "0") SectorId = null;
            if (VendorId == "0") VendorId = null;
            string Filter = null;
            var fdate = FromDate;
            if (!string.IsNullOrEmpty(fdate.ToString()))
            {
                objorder.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
            }
            var tdate = ToDate;
            if (!string.IsNullOrEmpty(tdate.ToString()))
            {
                objorder.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
            }
            Filter = objorder.FromDate.Value.ToShortDateString() + " To " + objorder.ToDate.Value.ToShortDateString();
            DataTable dt = new DataTable();
            ReportDocument rd = new ReportDocument();
            rd.Load(Server.MapPath("~/Report/VendorWisePurchaseAmt.rpt"));
            // rd.SetDatabaseLogon("courtDB", "CC#1055in", @"154.16.126.38,2525\SQLEXPRESS", "courtDB");
            //SqlDataAdapter da = new SqlDataAdapter("Sector_Vendor_OrderTotal_SelectAll", con);
            SqlDataAdapter da = new SqlDataAdapter("Sector_Vendor_Daily_OrderTotal_SelectAll", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(SectorId))
                da.SelectCommand.Parameters.AddWithValue("@SectorId", SectorId);
            else
                da.SelectCommand.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(VendorId))
                da.SelectCommand.Parameters.AddWithValue("@VendorId", VendorId);
            else
                da.SelectCommand.Parameters.AddWithValue("@VendorId", DBNull.Value);
            if (!string.IsNullOrEmpty(FromDate.ToString()))
                da.SelectCommand.Parameters.AddWithValue("@FromDate", objorder.FromDate);
            else
                da.SelectCommand.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(ToDate.ToString()))
                da.SelectCommand.Parameters.AddWithValue("@ToDate", objorder.ToDate);
            else
                da.SelectCommand.Parameters.AddWithValue("@ToDate", DBNull.Value);
            da.Fill(dt);

            rd.Database.Tables[0].SetDataSource(dt);
            rd.SetParameterValue("@SectorId", SectorId);
            rd.SetParameterValue("@VendorId", VendorId);
            rd.SetParameterValue("@FromDate", FromDate);
            rd.SetParameterValue("@ToDate", ToDate);
            rd.SetParameterValue("@Filter", Filter);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //CrystalReportViewer1.RefreshReport();
            try
            {
                Stream str = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                str.Seek(0, SeekOrigin.Begin);
                return File(str, "application/pdf", "VendorDailyOrderAmount.pdf");
            }
            catch (Exception e)
            {
                throw;
            }

        }



        [HttpGet]
        public ActionResult AssignVendorSector()
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

            //Sector objsector = new Sector();
            //DataTable dt = new DataTable();
            //dt = objsector.getSectorList(null);
            //ViewBag.Sector = dt;
            Sector objsector = new Sector();
            DataTable dtList = new DataTable();
            dtList = objsector.getStateList(null);
            ViewBag.StateList = dtList;



            dtList = objVendor.getVendorList(null);
            ViewBag.VendorList = dtList;
            return View();

        }


        public PartialViewResult FetchSectorList(int id)
        {
            Vendor objvendor = new Vendor();
            List<SectorViewModel> list = new List<SectorViewModel>();
            var sector = objvendor.GetSectorListByCity(id);
            if (sector.Rows.Count > 0)
            {
                for (int i = 0; i < sector.Rows.Count; i++)
                {
                    list.Add(new SectorViewModel { ID = sector.Rows[i]["Id"].ToString(), Name = sector.Rows[i]["SectorName"].ToString() });
                }
            }
            return PartialView("_ChkSectorList", list);
        }



        [HttpPost]
        public ActionResult AssignVendorSector(ProductAssignVM model, FormCollection frm, string[] chkSector)
        {

            string vendorId = frm["ddlVendor"];
            string state = frm["ddlState"];
            string City = frm["ddlCity"];
            int count = 0, dupcount = 0;
            if (chkSector != null)
            {
                objVendor.VendorCat = "";
                foreach (var item in chkSector)
                {

                    if (!string.IsNullOrEmpty(item))
                    {

                        string item1 = item.ToString();

                        DataTable dtList = new DataTable();
                        dtList = objVendor.ChkDuplSector(vendorId, item1);
                        if (dtList.Rows.Count > 0)
                        {
                            dupcount = dtList.Rows.Count;
                        }
                        else
                        {
                            int Vendorassign = objVendor.InsertVendorSectorAssign(state, City, vendorId, item1, objVendor.VendorCat);
                            if (Vendorassign > 0)
                                count++;
                        }

                    }
                }

                if (count > 0 || dupcount > 0)
                {
                    ViewBag.SuccessMsg = "Vendor Assigned to State,City and " + count + " Sector Successfully.  " + dupcount + " Duplicate Sector Ignored";
                }
                else
                    ViewBag.SuccessMsg = "Error Occured";
            }

            return View();

        }



        [HttpGet]
        public ActionResult AssignVendorSectorList()
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

            
            Vendor objvendor = new Vendor();
            DataTable dtList = new DataTable();
            dtList = objvendor.getVendorAssignList(null);
            if(dtList.Rows.Count>0)
            {
                string htmlstring = "";
                for (int i=0;i<dtList.Rows.Count;i++)
                {
                    int j = i + 1;
                    htmlstring += "<tr>";
                    htmlstring += "<td>"+j+"</td>";
                    htmlstring += "<td>" + dtList.Rows[i]["FirstName"].ToString() + " "+ dtList.Rows[i]["LastName"].ToString() + "</td>";

                    DataTable dtlist1= objvendor.getVendorAssignListnew(Convert.ToInt32(dtList.Rows[i]["VendorId"].ToString()));
                    if(dtlist1.Rows.Count>0)
                    {
                        string htmlnew = "";
                        for(int k=0;k<dtlist1.Rows.Count;k++)
                        {
                            htmlnew +="  ["+ dtlist1.Rows[k]["statename"].ToString()+"/" + dtlist1.Rows[k]["Cityname"].ToString()+"/" + dtlist1.Rows[k]["SectorName"].ToString()+"]";
                           
                            //htmlstring += " <td>,</td>";
                            //htmlstring += " <td>,</td>";
                        }
                        htmlstring += "<td>"+htmlnew+"</td>";
                    }

                    htmlstring += "<td><a href=\"/Vendor/DeleteSectorVendorAssign/" + dtList.Rows[i]["VendorId"].ToString() + ",onclick=\"return confirm('Are you sure you want to delete this Data?'); \"><i class=\"fa fa-trash-o\"></i></a></td>";
                }

                ViewBag.VendorSectorList = htmlstring;
            }

            ViewBag.VendorAssignList = dtList;
            return View();
        }




        [HttpGet]
        public ActionResult DeleteSectorVendorAssign(int id)
        {
            try
            {
                int delresult = objVendor.DeleteSectorVendorAssign(id);
                return RedirectToAction("AssignVendorSectorList");
            }

            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("AssignVendorSectorList");
        }

        [HttpGet]
        public ActionResult AssignVendorCatSubcat()
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

           
            DataTable dtList = new DataTable();
           
            Product objProdt = new Product();
            var category = objProdt.GetAllParentcategory();
            ViewBag.lstCategory = category;


            dtList = objVendor.getVendorList(null);
            ViewBag.VendorList = dtList;
            return View();

        }



        [HttpGet]
        public ActionResult EditVendorCatSubcat(int id = 0)
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


            DataTable dtList = new DataTable();

            Product objProdt = new Product();
            var category = objProdt.GetAllParentcategory();
            ViewBag.lstCategory = category;


            dtList = objVendor.getVendorList(null);
            ViewBag.VendorList = dtList;


            dtList = objVendor.getVendorCatSubcatAssignListsingle(id);


            if(dtList.Rows.Count>0)
            {

                if (!string.IsNullOrEmpty(dtList.Rows[0].ItemArray[18].ToString()))
                {
                    ViewBag.VendorCatId = dtList.Rows[0].ItemArray[18].ToString();
                }

                if (!string.IsNullOrEmpty(dtList.Rows[0].ItemArray[2].ToString()))
                {
                    ViewBag.VendorId = dtList.Rows[0].ItemArray[2].ToString();
                }

                if (!string.IsNullOrEmpty(dtList.Rows[0].ItemArray[9].ToString()))
                {
                    ViewBag.Parentcat = dtList.Rows[0].ItemArray[9].ToString();
                }

                if (!string.IsNullOrEmpty(dtList.Rows[0].ItemArray[10].ToString()))
                {
                    ViewBag.VendorCatName = dtList.Rows[0].ItemArray[10].ToString();
                }
                if (!string.IsNullOrEmpty(dtList.Rows[0].ItemArray[11].ToString()))
                {
                    ViewBag.MinAmount = dtList.Rows[0].ItemArray[11].ToString();
                }

                if (!string.IsNullOrEmpty(dtList.Rows[0].ItemArray[12].ToString()))
                {
                    ViewBag.MaxAmount = dtList.Rows[0].ItemArray[12].ToString();
                }

                if (!string.IsNullOrEmpty(dtList.Rows[0].ItemArray[13].ToString()))
                {
                    ViewBag.FromTime = dtList.Rows[0].ItemArray[13].ToString();
                }

                if (!string.IsNullOrEmpty(dtList.Rows[0].ItemArray[14].ToString()))
                {
                    ViewBag.ToTime = dtList.Rows[0].ItemArray[14].ToString();
                }

                if (!string.IsNullOrEmpty(dtList.Rows[0].ItemArray[15].ToString()))
                {
                    ViewBag.DeliveryFrom = dtList.Rows[0].ItemArray[15].ToString();
                }
                if (!string.IsNullOrEmpty(dtList.Rows[0].ItemArray[16].ToString()))
                {
                    ViewBag.DeliveryTo = dtList.Rows[0].ItemArray[16].ToString();
                }
                if (!string.IsNullOrEmpty(dtList.Rows[0]["Catimg"].ToString()))
                    ViewBag.Image = dtList.Rows[0]["Catimg"].ToString();
                else
                    ViewBag.Image = "";

                ViewBag.Notification = Convert.ToBoolean(dtList.Rows[0]["IsNotification"]);
            }
            
            return View();

        }


        [HttpPost]
        public ActionResult EditVendorCatSubcat(ProductAssignVM model, FormCollection frm, string[] chkSubcat, HttpPostedFileBase Document1)
        {
            string vendorcat = frm["VendorCatId"];

            string vendorId = frm["ddlVendor"];
            string Cat = frm["ddlCategory"];
            string VendorCatname = frm["VendorCatName"];

            string VendorMinAmount = frm["VendorMinAmount"];
            string VendorMaxAmount = frm["VendorMaxAmount"];
            string FromTime = frm["FromTime"];
            string ToTime = frm["ToTime"];
            string DeliveryFrom = frm["DeliveryFrom"];
            string DeliveryTo = frm["DeliveryTo"];
            int count = 0, dupcount = 0;

            //string notification = Request["IsNotification"];

            //if (notification == "on")
            //{
            //    bool add = Convert.ToBoolean(true);
            //    model.IsNotification = Convert.ToBoolean(add);
            //}
            //else
            //{
            //    if (!string.IsNullOrEmpty(notification))
            //    {
            //        bool add = Convert.ToBoolean(Request["IsNotification"].Split(',')[0]);
            //        model.IsNotification = Convert.ToBoolean(add);
            //    }
            //}

            //

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
                        //objVendor.Photo = fname;
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
                    DataTable dt1 = objVendor.getVendorCatSubcatAssignListsingle(Convert.ToInt32(vendorcat));
                    ViewBag.Image = dt1.Rows[0]["Catimg"].ToString();
                    existfile = ViewBag.Image;

                    fname = existfile.ToString();
                }
            }
            else
            {
                ViewBag.SuccessMsg = "File Not Get Updated!!!";
                var existfile = "";
                DataTable dt1 = objVendor.getVendorCatSubcatAssignListsingle(Convert.ToInt32(vendorcat));
                ViewBag.Image = dt1.Rows[0]["Catimg"].ToString();
                existfile = ViewBag.Image;
                fname = existfile.ToString();
            }
            //

            int vendorcatupdate = objVendor.Vendorcatupdate(vendorId, VendorCatname, vendorcat, fname, VendorMinAmount, VendorMaxAmount, FromTime, ToTime, DeliveryFrom, DeliveryTo);


            int vendorassignupdate = objVendor.UpdateVendorSubcatAssigncommon(vendorId, vendorcat);



            if (chkSubcat != null)
            {


                foreach (var item in chkSubcat)
                {

                    if (!string.IsNullOrEmpty(item))
                    {

                        string item1 = item.ToString();

                       DataTable dtList = new DataTable();
                        dtList = objVendor.ChkDuplSubcat(vendorId, Cat, item1);
                        if (dtList.Rows.Count > 0)
                        {

                             vendorassignupdate = objVendor.UpdateVendorSubcatAssign(vendorId, vendorcat,item1);

                            dupcount = dtList.Rows.Count;
                        }
                        else
                        {



                            int Vendorassign = objVendor.InsertVendorSubcatAssign(vendorId, VendorCatname, Cat, item1, vendorcat.ToString());
                            if (Vendorassign > 0)
                                count++;
                        }

                    }
                }



                if (count > 0 || dupcount > 0)
                {
                    ViewBag.SuccessMsg = "Edit Successfull";
                }
                else
                    ViewBag.SuccessMsg = "Error Occured";
            }




                return View();
        }

         public PartialViewResult FetchSubcatListVendorWise(int id,int vendorid)
        {
            Vendor objvendor = new Vendor();
            Product objProdt = new Product();
            List<SubcatVendorwiseViewModel> list = new List<SubcatVendorwiseViewModel>();
           var Subcat = objProdt.BindSubCategoryVendorWise(id, vendorid);
           // var Subcat = objProdt.BindSubCategory1(id);
            if (Subcat.Rows.Count > 0)
            {
                for (int i = 0; i < Subcat.Rows.Count; i++)
                {
                    list.Add(new SubcatVendorwiseViewModel { ID = Subcat.Rows[i]["Id"].ToString(), Name = Subcat.Rows[i]["SubCatName"].ToString(),VendorName= Subcat.Rows[i]["IsActive1"].ToString() });
                }
            }
            return PartialView("_ChkSubcatList1", list);
        }

        public PartialViewResult FetchSubcatList(int id)
        {
            Vendor objvendor = new Vendor();
            Product objProdt = new Product();
            List<SubcatViewModel> list = new List<SubcatViewModel>();
            var Subcat = objProdt.BindSubCategory1(id);
            if (Subcat.Rows.Count > 0)
            {
                for (int i = 0; i < Subcat.Rows.Count; i++)
                {
                    list.Add(new SubcatViewModel { ID = Subcat.Rows[i]["Id"].ToString(), Name = Subcat.Rows[i]["SubCatName"].ToString() });
                }
            }
            return PartialView("_ChkSubcatList", list);
        }



        [HttpPost]
        public ActionResult AssignVendorCatSubcat(ProductAssignVM model, FormCollection frm, string[] chkSubcat, HttpPostedFileBase Document1)
        {

            string vendorId = frm["ddlVendor"];
            string Cat = frm["ddlCategory"];
            string VendorCatname = frm["VendorCatName"];

            string VendorMinAmount = frm["VendorMinAmount"];
            string VendorMaxAmount = frm["VendorMaxAmount"];
            string FromTime = frm["FromTime"];
            string ToTime = frm["ToTime"];
            string DeliveryFrom = frm["DeliveryFrom"];
            string DeliveryTo = frm["DeliveryTo"];
            int count = 0, dupcount = 0;
            bool notstatus=true;
            string notification = Request["IsNotification"];
          
            if (notification == "on")
            {
                bool add = Convert.ToBoolean(true);
                notstatus = Convert.ToBoolean(add);
            }
            else
            {
                if (!string.IsNullOrEmpty(notification))
                {
                    bool add = Convert.ToBoolean(Request["IsNotification"].Split(',')[0]);
                    notstatus = Convert.ToBoolean(add);
                }
            }


            if (chkSubcat != null)
            {

                //
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
                            path = Path.Combine(Server.MapPath("~/image/VendorCatImage/"), fname);
                            img.Save(path);
                            // objProdt.Image = fname;
                        }

                        catch (Exception ex)
                        {
                            ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                        }
                    }
                }

                //


                int vendorcat = objVendor.InsertVendorcat(vendorId, VendorCatname, Cat, fname, VendorMinAmount, VendorMaxAmount, FromTime, ToTime, DeliveryFrom, DeliveryTo, notstatus);
                DataTable dtList = new DataTable();
                dtList = objVendor.getVendorAssignedSector(vendorcat);
                if (dtList.Rows.Count > 0)
                {
                    objVendor.VendorCat = "";
                    for (int i = 0; i < dtList.Rows.Count; i++)
                    {
                        int sid = Convert.ToInt32(dtList.Rows[i].ItemArray[0].ToString());
                        int vid = Convert.ToInt32(dtList.Rows[i].ItemArray[2].ToString());

                        int Vendorcatsectorassign = objVendor.InsertVendorcatSector(vid, vendorcat, sid, objVendor.VendorCat);
                    }

                }

                foreach (var item in chkSubcat)
                {

                    if (!string.IsNullOrEmpty(item))
                    {

                        string item1 = item.ToString();

                        dtList = new DataTable();
                        dtList = objVendor.ChkDuplSubcat(vendorId, Cat, item1);
                        if (dtList.Rows.Count > 0)
                        {
                            dupcount = dtList.Rows.Count;
                        }
                        else
                        {



                            int Vendorassign = objVendor.InsertVendorSubcatAssign(vendorId, VendorCatname, Cat, item1,vendorcat.ToString());
                            if (Vendorassign > 0)
                                count++;
                        }

                    }
                }

                if (count > 0 || dupcount > 0)
                {
                    ViewBag.SuccessMsg = "Vendor Assigned to Cat,Subcat - and " + count + " Subcat Successfully.  " + dupcount + " Duplicate Subcat Ignored";
                }
                else
                    ViewBag.SuccessMsg = "Error Occured";
            }

            return View();

        }



        [HttpGet]
        public ActionResult AssignVendorCatSubcatList()
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

            Vendor objvendor = new Vendor();
            DataTable dtList = new DataTable();
            dtList = objvendor.getVendorCatSubcatAssignList(null);
            ViewBag.VendorAssignList = dtList;
            return View();
        }

        public JsonResult Status(int ID)
        {
            // _VendorRepo.UpdateVendorCatSubcatStatus
            var userInfo = _VendorRepo.UpdateVendorCatSubcatStatus(ID);
            if (userInfo != null)
            {
                if (userInfo.IsActive == false)
                {
                    userInfo.IsActive = false;
                    res["class"] = "label-danger";
                    res["name"] = "InActive";
                }
                else
                {
                    userInfo.IsActive = true;
                    res["class"] = "label-success";
                    res["name"] = "Active";
                }
                res["id"] = ID.ToString();
                res["status"] = "1";
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DeleteCatSubcatVendorAssign(int id)
        {
            try
            {
                int delresult = objVendor.DeleteCatSubcatVendorAssign(id);
                return RedirectToAction("AssignVendorCatSubcatList");
            }

            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("AssignVendorCatSubcatList");
        }



        [HttpGet]
        public ActionResult AddVendorPayment()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            //
            CustomerOrder objorder1 = new CustomerOrder();
            DataTable dtList11 = new DataTable();
            dtList11 = objorder1.getSectorVendorOrderStatusnotification(null, null, System.DateTime.Now.Date.AddDays(1), System.DateTime.Now.Date.AddDays(2), null);
            ViewBag.ProductorderListnot = dtList11;
            ViewBag.ProductorderListcount = dtList11.Rows.Count;

            //
            


                Sector objsector = new Sector();
            DataTable dt = new DataTable();
            dt = objsector.getSectorList(null);
            ViewBag.Sector = dt;

            Vendor objvendor = new Vendor();
            DataTable dt1 = new DataTable();
            dt1 = objvendor.getVendorList(null);
            ViewBag.Vendor = dt1;
            return View();

        }


        [HttpPost]
        public ActionResult AddVendorPayment(Vendor obj, FormCollection form)
        {

            Sector objsector = new Sector();
            DataTable dt = new DataTable();
            dt = objsector.getSectorList(null);
            ViewBag.Sector = dt;

            Vendor objvendor = new Vendor();
            DataTable dt1 = new DataTable();
            dt1 = objvendor.getVendorList(null);
            ViewBag.Vendor = dt1;


            CustomerOrder objorder = new CustomerOrder();

            string sectorid = Request["ddlSector"];
            ViewBag.SectorId = sectorid;
            string vendor = Request["ddlVendor"];
            ViewBag.VendorId = vendor;

            var submit = Request["submit"];
            if (!string.IsNullOrEmpty(vendor))
            {
                obj.VendorId = Convert.ToInt32(vendor);
                objorder.VendorId= Convert.ToInt32(vendor);
            }
            if (!string.IsNullOrEmpty(sectorid))
            {
                obj.SectorId = Convert.ToInt32(sectorid);
                objorder.SectorId= Convert.ToInt32(sectorid);
            }

            //var fdate = Request["datepicker"];
            //if (!string.IsNullOrEmpty(fdate.ToString()))
            //{
            //    obj.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
            //}
            //var tdate = Request["datepicker1"];
            //if (!string.IsNullOrEmpty(tdate.ToString()))
            //{
            //    obj.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
            //}

            if (submit=="search")
            {
                // Vendor objvendor = new Vendor();
                // DataTable dt1 = new DataTable();
                // dt1 = objvendor.getVendorList(obj.VendorId,obj.FromDate,obj.ToDate);
               

                var fdate = Request["datepicker"];
                if (!string.IsNullOrEmpty(fdate.ToString()))
                {
                    objorder.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
                }
                ViewBag.FromDate = fdate;
                var tdate = Request["datepicker1"];
                if (!string.IsNullOrEmpty(tdate.ToString()))
                {
                    objorder.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
                }
                ViewBag.ToDate = tdate;
                DataTable dtList = new DataTable();
                dtList = objorder.getSectorVendorOrder(objorder.SectorId, objorder.VendorId, objorder.FromDate, objorder.ToDate);
                ViewBag.ProductorderList = dtList;
                DataTable  dtList1 = objVendor.getVendorPaymentListdatewise(Convert.ToInt32(vendor), objorder.FromDate, objorder.ToDate);
                ViewBag.StaffList = dtList1;
                decimal oamount = 0;
                Decimal paidAmount = 0;
                Decimal TotalAmt = 0;
                if (dtList.Rows.Count > 0)
                {
                    
                    Decimal TotalQty = Convert.ToDecimal(dtList.Compute("SUM(Qty)", string.Empty));
                     TotalAmt = Convert.ToDecimal(dtList.Compute("SUM(PurchasePrice)", string.Empty));
                    ViewBag.Totalamount = TotalAmt;
                    
                    
                    if(dtList1.Rows.Count>0)
                    {
                         paidAmount = Convert.ToDecimal(dtList1.Compute("SUM(Amount)", string.Empty));
                        ViewBag.Paidamount = paidAmount;
                    }

                     oamount = TotalAmt - paidAmount;

                    ViewBag.Outstandingamount = oamount;
                }

            }
            else
            {

                int addresult = obj.InsertVendorRefNo(obj);
                if (addresult > 0)
                { ViewBag.SuccessMsg = "Refference  Inserted Successfully!!!"; }
                else
                { ViewBag.ErrorMsg = "Refference  Not Inserted!!!"; }
            }

            

            return View();
        }




        [HttpGet]
        public ActionResult VendorPaymentList()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                DataTable dtList = new DataTable();
                dtList = objVendor.getVendorPaymentList(null);
                ViewBag.StaffList = dtList;
                return View();
            
        }



        [HttpGet]
        public ActionResult EditVendorPayment(int id = 0)
        {
            Vendor objvendor = new Vendor();
            DataTable dt1 = new DataTable();
            dt1 = objvendor.getVendorList(null);
            ViewBag.Vendor = dt1;


            DataTable dt = new DataTable();
            dt = objVendor.getVendorPaymentList(id);
            if (dt.Rows.Count > 0)
            {
               
                if (!string.IsNullOrEmpty(dt.Rows[0]["VendorId"].ToString()))
                    ViewBag.VendorId = dt.Rows[0]["VendorId"].ToString();
                else
                    ViewBag.VendorId = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["Amount"].ToString()))
                    ViewBag.Payamount = dt.Rows[0]["Amount"].ToString();
                else
                    ViewBag.Payamount = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["RefferenceNo"].ToString()))
                    ViewBag.PayRefference = dt.Rows[0]["RefferenceNo"].ToString();
                else
                    ViewBag.PayRefference = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["RefDate"].ToString()))
                {

                    ViewBag.PayRefdate = dt.Rows[0]["RefDate"].ToString();
                    DateTime dateFromString =DateTime.Parse(ViewBag.PayRefdate, System.Globalization.CultureInfo.InvariantCulture);


                    ViewBag.PayRefdate = dateFromString.ToString("dd/MM/yyyy");
                }
                   
                else
                    ViewBag.PayRefdate =null;
               
            }

            return View();
        }


        [HttpPost]
        public ActionResult EditVendorPayment(Vendor objvendor, FormCollection form)
        {
            Sector objsector = new Sector();
            DataTable dtsec = new DataTable();
            int id = objvendor.Id;

            DataTable dt = new DataTable();
            string sectorid = Request["ddlSector"];
            string vendor = Request["ddlVendor"];
            objvendor.VendorId = Convert.ToInt32(vendor);
            int result = objvendor.UpdateVendorPayment(objvendor);
            if (result > 0)
            {
                ViewBag.SuccessMsg = "Vendor Payment Updated Successfully!!!";
            }
            else
            { ViewBag.ErrorMsg = "Vendor Payment Not Updated!!!"; }

            DataTable dt1 = new DataTable();
            dt1 = objvendor.getVendorList(null);
            ViewBag.Vendor = dt1;

            dt = objVendor.getVendorPaymentList(id);
            if (dt.Rows.Count > 0)
            {

                if (!string.IsNullOrEmpty(dt.Rows[0]["VendorId"].ToString()))
                    ViewBag.VendorId = dt.Rows[0]["VendorId"].ToString();
                else
                    ViewBag.VendorId = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["Amount"].ToString()))
                    ViewBag.Payamount = dt.Rows[0]["Amount"].ToString();
                else
                    ViewBag.Payamount = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["RefferenceNo"].ToString()))
                    ViewBag.PayRefference = dt.Rows[0]["RefferenceNo"].ToString();
                else
                    ViewBag.PayRefference = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["RefDate"].ToString()))
                {

                    ViewBag.PayRefdate = dt.Rows[0]["RefDate"].ToString();
                    DateTime dateFromString = DateTime.Parse(ViewBag.PayRefdate, System.Globalization.CultureInfo.InvariantCulture);


                    ViewBag.PayRefdate = dateFromString.ToString("dd/MM/yyyy");
                }

                else
                    ViewBag.PayRefdate = null;
            }

                return View();

        }



        [HttpGet]
        public ActionResult DeleteVendorPayment(int id)
        {
            try
            {
                int delresult = objVendor.DeleteVendorPayment(id);
                return RedirectToAction("VendorPaymentList");
            }

            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("VendorPaymentList");
        }



        [HttpGet]
        public ActionResult AddVendorProductReplacement()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            Sector objsector = new Sector();
            DataTable dt = new DataTable();
            dt = objsector.getSectorList(null);
            ViewBag.Sector = dt;

            Vendor objvendor = new Vendor();
            DataTable dt1 = new DataTable();
            dt1 = objvendor.getVendorList(null);
            ViewBag.Vendor = dt1;
            return View();

        }
        public ActionResult AddVendorProductReplacement(Vendor obj, FormCollection form)
        {

           
            string vendor = Request["ddlVendor"];

            string product = Request["ddlProduct"];
            if (!string.IsNullOrEmpty(vendor))
            {
                obj.VendorId = Convert.ToInt32(vendor);
            }
            if (!string.IsNullOrEmpty(product))
            {
                obj.ProductId = Convert.ToInt32(product);
            }

            int addresult = obj.InsertReplacement(obj);
            if (addresult > 0)
            { ViewBag.SuccessMsg = "Replacement  Inserted Successfully!!!"; }
            else
            { ViewBag.ErrorMsg = "Replacement  Not Inserted!!!"; }

            return View();


        }
        public ActionResult GetProduct(int STATECODE)
        {
            List<ProMas> lstDesig = PopulateDesignation().Where(s => s.Vendorcode.Equals(STATECODE)).ToList();
            return Json(lstDesig, JsonRequestBehavior.AllowGet);
        }


        public List<ProMas> PopulateDesignation()
        {
            Vendor objvendor = new Vendor();
            DataTable dtList = new DataTable();
            dtList = objvendor.getproductList(null);

            List<ProMas> lstCity = new List<ProMas>();
            ProMas objDesig;


            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    objDesig = new ProMas();
                    objDesig.Vendorcode = Convert.ToInt32(dtList.Rows[i].ItemArray[0]);
                    objDesig.ProName = dtList.Rows[i].ItemArray[1].ToString();
                    objDesig.Procode = Convert.ToInt32(dtList.Rows[i].ItemArray[2].ToString());
                    lstCity.Add(objDesig);
                }
            }

            else
            {
                objDesig = new ProMas();
                objDesig.Vendorcode = 0;
                objDesig.ProName = "No data Found";
                objDesig.Procode = 0;
                lstCity.Add(objDesig);
            }





            return lstCity;
        }



        public ActionResult GetProductprice(int STATECODE)
        {
            List<ProductMas> lstDesig = PopulatePrice().Where(s => s.Procode.Equals(STATECODE)).ToList();
            return Json(lstDesig, JsonRequestBehavior.AllowGet);
        }


        public List<ProductMas> PopulatePrice()
        {
            Vendor objvendor = new Vendor();
            DataTable dtList = new DataTable();
            dtList = objvendor.getproductPrice(null);

            List<ProductMas> lstCity1 = new List<ProductMas>();
            ProductMas objDesig;


            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    objDesig= new ProductMas();
                    objDesig.Procode = Convert.ToInt32(dtList.Rows[i].ItemArray[0].ToString());
                    objDesig.MRP =(dtList.Rows[i].ItemArray[1].ToString());
                    objDesig.Pprice = (dtList.Rows[i].ItemArray[2].ToString());
                    objDesig.Sprice = (dtList.Rows[i].ItemArray[3].ToString());
                    lstCity1.Add(objDesig);
                }
            }

            else
            {
                objDesig = new ProductMas();
                objDesig.Procode = 0;
                objDesig.MRP = "0";
                objDesig.Pprice = "0";
                objDesig.Sprice = "0";
                lstCity1.Add(objDesig);
            }





            return lstCity1;
        }


        [HttpGet]
        public ActionResult VendorReplacementList()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            DataTable dtList = new DataTable();
            dtList = objVendor.getVendorProductReplacementList(null);
            ViewBag.StaffList = dtList;
            return View();

        }

        [HttpGet]
        public ActionResult DeleteVendorReplacement(int id)
        {
            try
            {
                int delresult = objVendor.DeleteVendorProductReplacement(id);
                return RedirectToAction("VendorReplacementList");
            }

            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("VendorReplacementList");
        }


        [HttpGet]
        public ActionResult EditVendorReplacement(int id = 0)
        {
            Vendor objvendor = new Vendor();
            DataTable dt1 = new DataTable();
            dt1 = objvendor.getVendorList(null);
            ViewBag.Vendor = dt1;

            dt1 = objvendor.getproductList(null);
            ViewBag.Product = dt1;

            DataTable dt = new DataTable();
            dt = objVendor.getVendorProductReplacementList(id);
            if (dt.Rows.Count > 0)
            {

                if (!string.IsNullOrEmpty(dt.Rows[0]["VendorId"].ToString()))
                    ViewBag.VendorId = dt.Rows[0]["VendorId"].ToString();
                else
                    ViewBag.VendorId = "";



                if (!string.IsNullOrEmpty(dt.Rows[0]["Pid"].ToString()))
                    ViewBag.ProductName = dt.Rows[0]["Pid"].ToString();
                else
                    ViewBag.ProductName = "";


                if (!string.IsNullOrEmpty(dt.Rows[0]["Mrp"].ToString()))

                {

                    ViewBag.Mrp = dt.Rows[0]["Mrp"].ToString();
                    decimal mprice = Convert.ToDecimal(dt.Rows[0]["Mrp"].ToString()) / Convert.ToDecimal(dt.Rows[0]["Qty"].ToString());
                    ViewBag.Mrp = mprice.ToString();
                }
                    
                else
                    ViewBag.Mrp = "0";

                if (!string.IsNullOrEmpty(dt.Rows[0]["PurchasePrice"].ToString()))

                {
                    ViewBag.PurchasePrice = dt.Rows[0]["PurchasePrice"].ToString();

                    decimal pprice = Convert.ToDecimal(dt.Rows[0]["PurchasePrice"].ToString()) / Convert.ToDecimal(dt.Rows[0]["Qty"].ToString());
                    ViewBag.PurchasePrice = pprice.ToString();
                }
                    
                else
                    ViewBag.PurchasePrice = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["SalePrice"].ToString()))
                {


                    ViewBag.SalePrice = dt.Rows[0]["SalePrice"].ToString();

                    decimal sprice = Convert.ToDecimal(dt.Rows[0]["SalePrice"].ToString()) / Convert.ToDecimal(dt.Rows[0]["Qty"].ToString());
                    ViewBag.SalePrice = sprice.ToString();
                }
                    
                else
                    ViewBag.SalePrice = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["Qty"].ToString()))
                    ViewBag.Qty = dt.Rows[0]["Qty"].ToString();
                else
                    ViewBag.Qty = "0";

                if (!string.IsNullOrEmpty(dt.Rows[0]["RefDate"].ToString()))
                {

                    ViewBag.PayRefdate = dt.Rows[0]["RefDate"].ToString();
                    DateTime dateFromString = DateTime.Parse(ViewBag.PayRefdate, System.Globalization.CultureInfo.InvariantCulture);


                    ViewBag.PayRefdate = dateFromString.ToString("dd/MM/yyyy");
                }

                else
                    ViewBag.PayRefdate = null;

            }

            return View();
        }


        [HttpPost]
        public ActionResult EditVendorReplacement(Vendor obj, FormCollection form)
        {


            int id = obj.Id;

            string vendor = Request["ddlVendor"];

            string product = Request["ddlProduct"];
            if (!string.IsNullOrEmpty(vendor))
            {
                obj.VendorId = Convert.ToInt32(vendor);
            }
            if (!string.IsNullOrEmpty(product))
            {
                obj.ProductId = Convert.ToInt32(product);
            }

            int addresult = obj.UpdateReplacement(obj);
            if (addresult > 0)
            { ViewBag.SuccessMsg = "Replacement  Updated Successfully!!!"; }
            else
            { ViewBag.ErrorMsg = "Replacement  Not Updated!!!"; }


            return View();
        }


        [HttpGet]
        public ActionResult AddVendorSwap()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            Customer objcust = new Customer();
            CustomerOrder objorder = new CustomerOrder();
            Sector objsector = new Sector();

            Vendor objvendor = new Vendor();
            DataTable dt1 = new DataTable();
            dt1 = objvendor.getVendorList(null);
            ViewBag.Vendor = dt1;


            
            DataTable dtvp = new DataTable();
            dtvp = objorder.getSectorVendorOrderStatus1(null, null, System.DateTime.Now.Date.AddDays(1), System.DateTime.Now.Date.AddDays(1), null);
            ViewBag.vendorpList = dtvp;
            return View();
        }
        [HttpPost]
        public ActionResult AddVendorSwap(FormCollection form,Vendor obj, string secId, string venId, string Status)
        {
            string proid = Request["txtproid"];
            string submit = Request["submit"];
            DateTime FDate; DateTime TDate;
            FDate = DateTime.Today;
            TDate = DateTime.Today;
            var fdate = Request["datepicker"];
            if (!string.IsNullOrEmpty(fdate.ToString()))
            {
                FDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
            }

            ViewBag.FromDate = fdate;
            secId = "0";
            venId = Request["ddlVendor"];
            ViewBag.VendorId = venId;
            Status = "Pending";
            CustomerOrder objorder = new CustomerOrder();
            //try
            //{
                if (!string.IsNullOrEmpty(secId) && Convert.ToInt32(secId) != 0)
                {
                    objorder.SectorId = Convert.ToInt32(secId);
                }
                if (!string.IsNullOrEmpty(venId) && Convert.ToInt32(venId) != 0)
                {
                    objorder.VendorId = Convert.ToInt32(venId);
                }
                if (!string.IsNullOrEmpty(Status))
                {
                    objorder.Status = Status;
                }

                if(submit== "Swap")
                {

                int uswap = 0;
                  int  newvenId =Convert.ToInt32(Request["ddlVendornew"]);

                ViewBag.VendorIdnew = newvenId;
                    string delimStr = ",";
                    char[] delimiter = delimStr.ToCharArray();
                    int id = 0;
                    foreach (string s in proid.Split(delimiter))
                    {
                        
                        id = Convert.ToInt32(s);

                         uswap = objorder.UpdatevendorOrder(id,newvenId);
                    }

                    if(uswap>0)
                {
                    objorder.VendorId = Convert.ToInt32(newvenId);
                    ViewBag.SuccessMsg = "Vendor Change Successfull";
                }
                    else
                {
                    ViewBag.ErrorMsg = "Vendor Change Not Successfull";
                }
                        
                }

                DataTable vp = objorder.getSectorVendorOrderStatus1(objorder.SectorId, objorder.VendorId, FDate, FDate, objorder.Status);
                ViewBag.vendorpList = vp;

                Vendor objvendor = new Vendor();
                DataTable dt1 = new DataTable();
                dt1 = objvendor.getVendorList(null);
                ViewBag.Vendor = dt1;

            //}
            //catch (Exception e)
            //{
            //    Console.Write(e);
            //}
            return View();
        }


        [HttpPost]
        public ActionResult VendorProduct(string secId, string venId, string Status)
        {
            CustomerOrder objorder = new CustomerOrder();
            try
            {
                if (!string.IsNullOrEmpty(secId) && Convert.ToInt32(secId) != 0)
                {
                    objorder.SectorId = Convert.ToInt32(secId);
                }
                if (!string.IsNullOrEmpty(venId) && Convert.ToInt32(venId) != 0)
                {
                    objorder.VendorId = Convert.ToInt32(venId);
                }
                if (!string.IsNullOrEmpty(Status))
                {
                    objorder.Status = Status;
                }




                DataTable vp = objorder.getSectorVendorOrderStatus(objorder.SectorId, objorder.VendorId, System.DateTime.Now.Date.AddDays(1), System.DateTime.Now.Date.AddDays(1), objorder.Status);
                string data = string.Empty;
                data = JsonConvert.SerializeObject(vp);
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            return View();
        }


        [HttpGet]
        public ActionResult VendorOfferSettings(int Id=0)
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


            DataTable dtList = new DataTable();
            Sector objsector = new Sector();
            Product objProdt = new Product();
            //var category = objProdt.GetAllParentcategory();
            //ViewBag.lstCategory = category;
            ViewBag.VendorId = Id;

            dtList = objVendor.getVendorList(Id);
            if(dtList.Rows.Count>0)
            {
                ViewBag.VendorName = dtList.Rows[0]["FirstName"].ToString() + " " + dtList.Rows[0]["LastName"].ToString();
                ViewBag.VendorStore = dtList.Rows[0]["Vendorstore"].ToString();
                ViewBag.StateId = dtList.Rows[0]["State"].ToString();
                ViewBag.CityId = dtList.Rows[0]["City"].ToString();
            }
           DataTable dt = objVendor.getOfferVendorProductList(Id);
            if (dt.Rows.Count > 0)
            {
                ViewBag.ProductList = dt;
            }

            dtList = objsector.getStateList(null);
            ViewBag.StateList = dtList;
            return View();

        }


        public ActionResult GetVendorcat(int STATECODE)
        {
            List<ProMas> lstDesig = PopulateVendorcat().Where(s => s.Vendorcode.Equals(STATECODE)).ToList();
            return Json(lstDesig, JsonRequestBehavior.AllowGet);
        }


        public List<ProMas> PopulateVendorcat()
        {
            Vendor objvendor = new Vendor();
            DataTable dtList = new DataTable();
            dtList = objvendor.getVendorCat(null);

            List<ProMas> lstCity = new List<ProMas>();
            ProMas objDesig;


            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    objDesig = new ProMas();
                    objDesig.Vendorcode = Convert.ToInt32(dtList.Rows[i].ItemArray[2]);
                    objDesig.ProName = dtList.Rows[i].ItemArray[1].ToString();
                    objDesig.Procode = Convert.ToInt32(dtList.Rows[i].ItemArray[0].ToString());
                    lstCity.Add(objDesig);
                }
            }

            else
            {
                objDesig = new ProMas();
                objDesig.Vendorcode = 0;
                objDesig.ProName = "No data Found";
                objDesig.Procode = 0;
                lstCity.Add(objDesig);
            }





            return lstCity;
        }



        public ActionResult GetVendorcatProduct(int STATECODE)
        {
            List<ProMas> lstDesig = PopulateVendorcatProduct().Where(s => s.Vendorcode.Equals(STATECODE)).ToList();
            return Json(lstDesig, JsonRequestBehavior.AllowGet);
        }


        public List<ProMas> PopulateVendorcatProduct()
        {
            Vendor objvendor = new Vendor();
            DataTable dtList = new DataTable();
            dtList = objvendor.getVendorcatProduct(null);

            List<ProMas> lstCity = new List<ProMas>();
            ProMas objDesig;


            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    objDesig = new ProMas();
                    objDesig.Vendorcode = Convert.ToInt32(dtList.Rows[i].ItemArray[2]);
                    objDesig.ProName = dtList.Rows[i].ItemArray[1].ToString();
                    objDesig.Procode = Convert.ToInt32(dtList.Rows[i].ItemArray[0].ToString());
                    lstCity.Add(objDesig);
                }
            }

            else
            {
                objDesig = new ProMas();
                objDesig.Vendorcode = 0;
                objDesig.ProName = "No data Found";
                objDesig.Procode = 0;
                lstCity.Add(objDesig);
            }





            return lstCity;
        }

        public PartialViewResult FetchSectorListByVendocat(int id,int VendorCatid)
        {
            Vendor objvendor = new Vendor();
            // List<SectorViewModel> list = new List<SectorViewModel>();
            List<SubcatVendorwiseViewModel> list = new List<SubcatVendorwiseViewModel>();
            var sector = objvendor.GetSectorListByCityVendor(id, VendorCatid);
            if (sector.Rows.Count > 0)
            {
                for (int i = 0; i < sector.Rows.Count; i++)
                {
                    //list.Add(new SectorViewModel { ID = sector.Rows[i]["Id"].ToString(), Name = sector.Rows[i]["SectorName"].ToString() });
                    list.Add(new SubcatVendorwiseViewModel { ID = sector.Rows[i]["Id"].ToString(), Name = sector.Rows[i]["SectorName"].ToString(), VendorName = sector.Rows[i]["IsActive"].ToString() });
                }
            }
            return PartialView("_ChkSubcatList1", list);



            //

           
           
        }


        public PartialViewResult FetchSectorListByOfferVendorcat(int id, int VendorCatid)
        {
            Vendor objvendor = new Vendor();
            // List<SectorViewModel> list = new List<SectorViewModel>();
            List<SubcatVendorwiseViewModel> list = new List<SubcatVendorwiseViewModel>();
            var sector = objvendor.GetSectorListByCityVendorOffer(id, VendorCatid);
            if (sector.Rows.Count > 0)
            {
                for (int i = 0; i < sector.Rows.Count; i++)
                {
                    //list.Add(new SectorViewModel { ID = sector.Rows[i]["Id"].ToString(), Name = sector.Rows[i]["SectorName"].ToString() });
                    list.Add(new SubcatVendorwiseViewModel { ID = sector.Rows[i]["Id"].ToString(), Name = sector.Rows[i]["SectorName"].ToString(), VendorName = sector.Rows[i]["IsActive"].ToString() });
                }
            }
            return PartialView("_ChkSubcatList1", list);



            //



        }

        public JsonResult GetVendorcatProductAttributenew(int? Vendorcat, int? ProId)
        {
            Vendor objvendor = new Vendor();
            string code = string.Empty;
            StringBuilder sb = new StringBuilder();

            if (Vendorcat.ToString() == "" || Vendorcat == 0)
            {
                Vendorcat = null;
            }

            var dtcategory = objvendor.getVendorcatProductAttribute(Vendorcat, ProId);
            if (dtcategory.Rows.Count > 0)
            {
                sb.Append("<option value='0' >--Select Attribute--</option>");
                for (int i = 0; i < dtcategory.Rows.Count; i++)
                {

                    sb.Append("<option value=" + dtcategory.Rows[i]["Id"] + " >" + dtcategory.Rows[i]["Name"] + "</option> ");


                }
            }

            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult VendorOfferSettings(Vendor obj,FormCollection frm,string[] chkWeekday, string[] chkSubcat,string[] chkpro)
        {
            int addresult = 0;
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

            string product = "";
            DataTable dtList = new DataTable();
            Sector objsector = new Sector();
            Product objProdt = new Product();
            DateTime FromDate =Convert.ToDateTime(obj.FromDate);
            DateTime ToDate = Convert.ToDateTime(obj.ToDate);

            if (chkpro != null)
            {
                foreach (var item in chkpro)
                {
                    string item1 = item.ToString();
                    if (product != "")
                    {
                        product += product + "," + item1.ToString();
                    }
                    else
                        product = item1.ToString();
                }
            }
            obj.Products = product;

                    obj.updatedon = Helper.indianTime;
            obj.IsActive = true;

            obj.Offerdaytype = Request["ddlofferday"];
            obj.OfferType = Request["ddlOffercat"];
            if(obj.Offerdaytype== "From-To Date")
            {
                //for (int idate = 0; FromDate <= ToDate; idate++)
                //{


                //    FromDate = FromDate.AddDays(1);


                //}

                addresult = obj.OfferDetailInsert(obj);
            }

            if (obj.Offerdaytype == "Week Day")
            {
                string delimStr = ",";
                char[] delimiter = delimStr.ToCharArray();
                string choosenday = "";
                int currentdayno = 0, daydiff = 0;
                DayOfWeek currentday = Helper.indianTime.DayOfWeek;
                if (currentday.ToString() == "Monday") currentdayno = 1;
                if (currentday.ToString() == "Tuesday") currentdayno = 2;
                if (currentday.ToString() == "Wednesday") currentdayno = 3;
                if (currentday.ToString() == "Thursday") currentdayno = 4;
                if (currentday.ToString() == "Friday") currentdayno = 5;
                if (currentday.ToString() == "Saturday") currentdayno = 6;
                if (currentday.ToString() == "Sunday") currentdayno = 7;

                //obj.Weekday = Request["ddlweekday"];
                //string selecteddays = obj.Weekday.ToString();
                if (chkWeekday != null)
                {
                    foreach (var item in chkWeekday)
                    {
                        string item1 = item.ToString();

                        daydiff = 0;
                        if(choosenday!="")
                        {
                            choosenday += choosenday + "," + item1.ToString();
                        }
                        else
                        choosenday = item1.ToString();
                        //if (Convert.ToInt32(choosenday) <= currentdayno)
                        //{
                        //    FromDate = Convert.ToDateTime(obj.FromDate);
                        //    int dayadd = 0;
                        //    daydiff = currentdayno - Convert.ToInt32(choosenday);
                        //    dayadd = 7 - daydiff;
                        //    FromDate = FromDate.AddDays(dayadd);

                        //}

                        //if (Convert.ToInt32(choosenday) > currentdayno)
                        //{
                        //    FromDate = Convert.ToDateTime(obj.FromDate);
                            
                        //    daydiff = Convert.ToInt32(choosenday) - currentdayno;
                        //    FromDate = FromDate.AddDays(daydiff);

                        //}
                    }
                    obj.OfferDates = choosenday;
                    addresult = obj.OfferDetailInsert(obj);
                }
               
           
                


            }

            if (obj.Offerdaytype == "Multiple Day")
            {
                string date = Request["MultipleDay"];
                string delimStr = ",";
                char[] delimiter = delimStr.ToCharArray();
                string a = "";
                //foreach (string s in date.Split(delimiter))
                //{
                //    FromDate = Convert.ToDateTime(s);
                //}

                obj.OfferDates = date;
                addresult = obj.OfferDetailInsert(obj);

            }




            int addsector = 0;
            if(addresult>0)
            {
                obj.OfferId = addresult.ToString();

                if (chkSubcat != null)
                {
                    foreach (var item in chkSubcat)
                    {
                        string item1 = item.ToString();
                        obj.SectorId =Convert.ToInt32(item1);
                        addsector = obj.OfferSectorInsert(obj);
                    }
                }
            }



            if(addsector>0)
            { ViewBag.SuccessMsg = "Updation Successful"; }
            else
            { ViewBag.SuccessMsg = "Updation Unsuccessful"; }
            //var category = objProdt.GetAllParentcategory();
            //ViewBag.lstCategory = category;


            //dtList = objVendor.getVendorList(null);
            //ViewBag.VendorList = dtList;

            //dtList = objsector.getStateList(null);
            //ViewBag.StateList = dtList;



            ViewBag.VendorId = obj.VendorId;

            dtList = objVendor.getVendorList(obj.VendorId);
            if (dtList.Rows.Count > 0)
            {
                ViewBag.VendorName = dtList.Rows[0]["FirstName"].ToString() + " " + dtList.Rows[0]["LastName"].ToString();
                ViewBag.VendorStore = dtList.Rows[0]["Vendorstore"].ToString();
                ViewBag.StateId = dtList.Rows[0]["State"].ToString();
                ViewBag.CityId = dtList.Rows[0]["City"].ToString();
            }
            DataTable dt = objVendor.getOfferVendorProductList(obj.VendorId);
            if (dt.Rows.Count > 0)
            {
                ViewBag.ProductList = dt;
            }

            dtList = objsector.getStateList(null);
            ViewBag.StateList = dtList;
            return View();

        }


        [HttpGet]
        public ActionResult AddOfferVendor()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                Sector objsector = new Sector();
                DataTable dt = new DataTable();
                //dt = objsector.getSectorList(null);
                //ViewBag.Sector = dt;
               DataTable dtList = objsector.getStateList(null);
                ViewBag.StateList = dtList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpPost]
        public ActionResult AddOfferVendor(Vendor obj, FormCollection form, HttpPostedFileBase Photo, HttpPostedFileBase Slider1, HttpPostedFileBase Slider2, string[] chkSector)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                //string sectorid = Request["ddlSector"];
                string vendortype = Request["ddlVendorType"];
                if (!string.IsNullOrEmpty(vendortype))
                {
                    obj.VendorType = vendortype;
                }
                //if (!string.IsNullOrEmpty(sectorid))
                //{
                //    obj.SectorId = Convert.ToInt32(sectorid);
                //}
                //check username
                DataTable dtuserRecord1 = new DataTable();
                dtuserRecord1 = obj.CheckVendorUserName(obj.UserName);
                int userRecords1 = dtuserRecord1.Rows.Count;
                if (userRecords1 > 0)
                {
                    ViewBag.SuccessMsg = "UserName Already Exits!!!";
                    return View();
                }

                //check data duplicate
                DataTable dtDupliStaff = new DataTable();
                dtDupliStaff = obj.CheckDuplicateVendor(obj.FirstName, obj.LastName, obj.MobileNo);
                if (dtDupliStaff.Rows.Count > 0)
                {
                    ViewBag.SuccessMsg = "Data Already Exits!!!";
                }
                else
                {
                    string fname = null, path = null;
                    HttpPostedFileBase document1 = Request.Files["Photo"];
                    string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png" };
                    if (document1 != null)
                    {
                        if (document1.ContentLength > 0)
                        {
                            try
                            {
                                HttpFileCollectionBase files = Request.Files;
                                HttpPostedFileBase file = Photo;
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
                                path = Path.Combine(Server.MapPath("~/image/vendorphoto/"), fname);
                                file.SaveAs(path);
                                obj.Photo = fname;
                            }

                            catch (Exception ex)
                            {
                                ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                            }
                        }
                    }

                    document1 = Request.Files["Slider1"];

                    if (document1 != null)
                    {
                        if (document1.ContentLength > 0)
                        {
                            try
                            {
                                HttpFileCollectionBase files = Request.Files;
                                HttpPostedFileBase file = Photo;
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
                                path = Path.Combine(Server.MapPath("~/image/vendorphoto/"), fname);
                                file.SaveAs(path);
                                obj.Slider1 = fname;
                            }

                            catch (Exception ex)
                            {
                                ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                            }
                        }
                    }

                    document1 = Request.Files["Slider2"];

                    if (document1 != null)
                    {
                        if (document1.ContentLength > 0)
                        {
                            try
                            {
                                HttpFileCollectionBase files = Request.Files;
                                HttpPostedFileBase file = Photo;
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
                                path = Path.Combine(Server.MapPath("~/image/vendorphoto/"), fname);
                                file.SaveAs(path);
                                obj.Slider2 = fname;
                            }

                            catch (Exception ex)
                            {
                                ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                            }
                        }
                    }

                    obj.VendorCat = "Offer";
                    obj.State =Convert.ToInt32(Request["ddlState"]);
                    obj.City = Convert.ToInt32(Request["ddlCity"]);
                    obj.VendorMasterCat = Request["ddlmastercat"];
                    string active = Request["IsDiscountActive"].Split(',')[0];
                    if (!string.IsNullOrEmpty(active))
                    {
                        obj.DiscountIsActive = Convert.ToBoolean(active);
                    }
                    int addresult = obj.InsertVendor(obj);
                    if (addresult > 0)
                    {

                        int count = 0, dupcount = 0;
                        if (chkSector != null)
                        {
                            foreach (var item in chkSector)
                            {

                                if (!string.IsNullOrEmpty(item))
                                {

                                    string item1 = item.ToString();

                                    DataTable dtList = new DataTable();
                                    dtList = objVendor.ChkDuplSector(addresult.ToString(), item1);
                                    if (dtList.Rows.Count > 0)
                                    {
                                        dupcount = dtList.Rows.Count;
                                    }
                                    else
                                    {
                                        int Vendorassign = objVendor.InsertVendorSectorAssign(obj.State.ToString(), obj.City.ToString(), addresult.ToString(), item1,obj.VendorCat);
                                        int VendorcatsectorAssign = objVendor.InsertVendorcatSector(addresult,addresult,Convert.ToInt32(item1), obj.VendorCat);
                                        if (Vendorassign > 0)
                                        { }
                                    }

                                }
                            }

                            //if (count > 0 || dupcount > 0)
                            //{
                            //    ViewBag.SuccessMsg = "Vendor Assigned to State,City and " + count + " Sector Successfully.  " + dupcount + " Duplicate Sector Ignored";
                            //}
                            //else
                            //    ViewBag.SuccessMsg = "Error Occured";
                        }

                        ViewBag.SuccessMsg = "Vendor Inserted Successfully!!!";

                    }
                    else
                    { ViewBag.SuccessMsg = "Vendor Not Inserted!!!"; }
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
        public ActionResult OfferVendorList()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                DataTable dtList = new DataTable();
                dtList = objVendor.getOfferVendorList(null);
                ViewBag.StaffList = dtList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult AddVendorProduct(int Id=0)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;
                ViewBag.VendorId = Id.ToString();
                DataTable dtList = new DataTable();
                //dtList = objVendor.getOfferVendorList(null);
                //ViewBag.StaffList = dtList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddVendorProduct(Vendor obj, FormCollection form, HttpPostedFileBase Document1)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            //ViewBag.IsAdmin = control.IsAdmin;
            //ViewBag.IsView = control.IsView;
            //ViewBag.IsAdd = control.IsAdd;
            //ViewBag.VendorId = Id.ToString();
            //Vendor objvendor = new Vendor();
            obj.IsActive = true;
            DataTable dtDupliProduct = new DataTable();
            dtDupliProduct = objVendor.CheckDuplicateProduct(obj.ProductName,Convert.ToInt32(obj.VendorId));
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
                            obj.Photo = fname;
                        }

                        catch (Exception ex)
                        {
                            ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                        }
                    }
                }
                //int addresult = 0;
                int addresult = objVendor.InsertProduct(obj);

               

                if (addresult > 0)
                { ViewBag.SuccessMsg = "Product Inserted Successfully!!!"; }
                else
                { ViewBag.SuccessMsg = "Product Not Inserted!!!"; }
            }
            ModelState.Clear();

            return View();
        }


        [HttpGet]
        public ActionResult EditVendorOffer(int id = 0)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                Sector objsector = new Sector();
                DataTable dtsec = new DataTable();
                Vendor obj = new Vendor();
                //dtsec = objsector.getSectorList(null);
                //ViewBag.Sector = dtsec;
                DataTable dtList = objsector.getStateList(null);
                ViewBag.StateList = dtList;
                DataTable dt = new DataTable();
                dt = objVendor.getOfferVendorList(id);
                ViewBag.VendorId = id;
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["State"].ToString()))
                        ViewBag.State = dt.Rows[0]["State"].ToString();
                    else
                        ViewBag.State = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["City"].ToString()))
                        ViewBag.City = dt.Rows[0]["City"].ToString();
                    else
                        ViewBag.City = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["FirstName"].ToString()))
                        ViewBag.FirstName = dt.Rows[0]["FirstName"].ToString();
                    else
                        ViewBag.FirstName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["LastName"].ToString()))
                        ViewBag.LastName = dt.Rows[0]["LastName"].ToString();
                    else
                        ViewBag.LastName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["MobileNo"].ToString()))
                        ViewBag.MobileNo = dt.Rows[0]["MobileNo"].ToString();
                    else
                        ViewBag.MobileNo = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Email"].ToString()))
                        ViewBag.Email = dt.Rows[0]["Email"].ToString();
                    else
                        ViewBag.Email = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Address"].ToString()))
                    {
                        ViewBag.Address = dt.Rows[0]["Address"].ToString();
                        obj.Address= dt.Rows[0]["Address"].ToString();
                    }
                       
                    else
                    {
                        ViewBag.Address = "";
                        obj.Address = "";
                    }
                        
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Photo"].ToString()))
                        ViewBag.Photo = dt.Rows[0]["Photo"].ToString();
                    else
                        ViewBag.Photo = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["UserName"].ToString()))
                        ViewBag.UserName = dt.Rows[0]["UserName"].ToString();
                    else
                        ViewBag.UserName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Password"].ToString()))
                        ViewBag.Password = dt.Rows[0]["Password"].ToString();
                    else
                        ViewBag.Password = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["CompanyName"].ToString()))
                        ViewBag.CompanyName = dt.Rows[0]["CompanyName"].ToString();
                    else
                        ViewBag.CompanyName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["PanCardNo"].ToString()))
                        ViewBag.PanCardNo = dt.Rows[0]["PanCardNo"].ToString();
                    else
                        ViewBag.PanCardNo = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["GSTNo"].ToString()))
                        ViewBag.GSTNo = dt.Rows[0]["GSTNo"].ToString();
                    else
                        ViewBag.GSTNo = "";


                    if (!string.IsNullOrEmpty(dt.Rows[0]["VendorType"].ToString()))
                        ViewBag.VendorType = dt.Rows[0]["VendorType"].ToString();
                    else
                        ViewBag.VendorType = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["MasterCat"].ToString()))
                        ViewBag.MasterCat = dt.Rows[0]["MasterCat"].ToString();
                    else
                        ViewBag.MasterCat = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["Vendorstore"].ToString()))
                        ViewBag.Vendorstore = dt.Rows[0]["Vendorstore"].ToString();
                    else
                        ViewBag.Vendorstore = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["VendorCat"].ToString()))
                        ViewBag.VendorCat = dt.Rows[0]["VendorCat"].ToString();
                    else
                        ViewBag.VendorCat = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["Slider1"].ToString()))
                        ViewBag.Slider1 = dt.Rows[0]["Slider1"].ToString();
                    else
                        ViewBag.Slider1 = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["Slider2"].ToString()))
                        ViewBag.Slider2 = dt.Rows[0]["Slider2"].ToString();
                    else
                        ViewBag.Slider2 = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["MilkywayPer"].ToString()))
                        ViewBag.MilkywayPer = dt.Rows[0]["MilkywayPer"].ToString();
                    else
                        ViewBag.MilkywayPer = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["DiscountPer"].ToString()))
                        ViewBag.DiscountPer = dt.Rows[0]["DiscountPer"].ToString();
                    else
                        ViewBag.DiscountPer = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["DiscountActive"].ToString()))
                        ViewBag.DiscountActive = dt.Rows[0]["DiscountActive"].ToString();
                    else
                        ViewBag.DiscountActive = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["Vendorterms"].ToString()))
                    {
                        ViewBag.Vendorterms = dt.Rows[0]["Vendorterms"].ToString();
                        obj.VendorTerms= dt.Rows[0]["Vendorterms"].ToString();
                    }
                        
                    else
                    {
                        ViewBag.Vendorterms = "";
                        obj.VendorTerms = "";
                    }
                        
                }



                dt = objVendor.getOfferVendorProductList(id);
                if(dt.Rows.Count>0)
                {
                    ViewBag.ProductList = dt;
                }
                return View(obj);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpPost]
        public ActionResult EditVendorOffer(Vendor obj, FormCollection form, HttpPostedFileBase Photo, HttpPostedFileBase Slider1, HttpPostedFileBase Slider2, string[] chkSubcat)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                //string sectorid = Request["ddlSector"];
                DataTable dtList = new DataTable();
                Sector objsector = new Sector();
                string vendortype = Request["ddlVendorType"];
                string submit = Request["submit"];

                

                if (submit == "Update")
                {
                    if (!string.IsNullOrEmpty(vendortype))
                    {
                        obj.VendorType = vendortype;
                    }

                    //check username
                    
                    string fname = null, path = null;
                    HttpPostedFileBase document1 = Request.Files["Photo"];
                    string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png" };
                    if (document1 != null)
                    {
                        if (document1.ContentLength > 0)
                        {
                            try
                            {
                                HttpFileCollectionBase files = Request.Files;
                                HttpPostedFileBase file = Photo;
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
                                path = Path.Combine(Server.MapPath("~/image/vendorphoto/"), fname);
                                file.SaveAs(path);
                                obj.Photo = fname;
                            }

                            catch (Exception ex)
                            {
                                ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                            }
                        }
                    }

                    document1 = Request.Files["Slider1"];

                    if (document1 != null)
                    {
                        if (document1.ContentLength > 0)
                        {
                            try
                            {
                                HttpFileCollectionBase files = Request.Files;
                                HttpPostedFileBase file = Photo;
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
                                path = Path.Combine(Server.MapPath("~/image/vendorphoto/"), fname);
                                file.SaveAs(path);
                                obj.Slider1 = fname;
                            }

                            catch (Exception ex)
                            {
                                ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                            }
                        }
                    }

                    document1 = Request.Files["Slider2"];

                    if (document1 != null)
                    {
                        if (document1.ContentLength > 0)
                        {
                            try
                            {
                                HttpFileCollectionBase files = Request.Files;
                                HttpPostedFileBase file = Photo;
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
                                path = Path.Combine(Server.MapPath("~/image/vendorphoto/"), fname);
                                file.SaveAs(path);
                                obj.Slider2 = fname;
                            }

                            catch (Exception ex)
                            {
                                ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                            }
                        }
                    }

                    obj.VendorCat = "Offer";
                    obj.State = Convert.ToInt32(Request["ddlState"]);
                    obj.City = Convert.ToInt32(Request["ddlCity"]);
                    obj.VendorMasterCat = Request["ddlmastercat"];
                    string active = Request["IsDiscountActive"].Split(',')[0];
                    if (!string.IsNullOrEmpty(active))
                    {
                        obj.DiscountIsActive = Convert.ToBoolean(active);
                    }
                    int addresult = obj.UpdateOfferVendor(obj);
                    int vendorassignupdate = obj.UpdateVendorSectorAssigncommon(obj.VendorId.ToString());
                    if (addresult > 0)
                    {

                        int count = 0, dupcount = 0;
                        if (chkSubcat != null)
                        {
                            foreach (var item in chkSubcat)
                            {

                                if (!string.IsNullOrEmpty(item))
                                {

                                    string item1 = item.ToString();


                                    dtList = objVendor.ChkDuplSector(obj.VendorId.ToString(), item1);
                                    if (dtList.Rows.Count > 0)
                                    {
                                        vendorassignupdate = obj.UpdateVendorSectorAssign(obj.VendorId.ToString(), item1);
                                        dupcount = dtList.Rows.Count;
                                    }
                                    else
                                    {
                                        int Vendorassign = obj.InsertVendorSectorAssign(obj.State.ToString(), obj.City.ToString(), obj.VendorId.ToString(), item1, obj.VendorCat);
                                        int VendorcatsectorAssign = obj.InsertVendorcatSector(obj.VendorId, obj.VendorId, Convert.ToInt32(item1), obj.VendorCat);
                                        if (Vendorassign > 0)
                                        { }
                                    }

                                }
                            }


                        }

                        ViewBag.SuccessMsg = "Vendor Updated Successfully!!!";

                    }
                    else
                    { ViewBag.SuccessMsg = "Vendor Not Updated!!!"; }
                }
                else
                {
                    
                    obj.Id = Convert.ToInt32(submit);
                    obj.Photo = Request[submit + "photo"];
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
                                HttpPostedFileBase file = document1;
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
                                file.SaveAs(path);
                                obj.Photo = fname;
                            }

                            catch (Exception ex)
                            {
                                ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                            }
                        }
                    }


                    //
                    decimal MRPPrice = 0, SalePrice = 0, PurchasePrice = 0;
                    int OrderBy = 0;
                   
                    obj.SellPrice = Convert.ToDecimal(Request[submit + "saleprice"]);
                    obj.Detail = Request[submit + "des"];
                    obj.SortOrder =Convert.ToInt32(Request[submit + "sortorder"]);
                   

                    obj.CGST = Convert.ToDecimal(Request[submit + "cgst"]);
                    obj.SGST = Convert.ToDecimal(Request[submit + "sgst"]);
                    obj.IGST = Convert.ToDecimal(Request[submit + "igst"]);
                    
                    obj.ProductName = Request[submit + "proname"];
                    int addresult = obj.UpdateOfferVendorProduct(obj);
                    //
                }
            ModelState.Clear();
            //check data duplicate
            DataTable dtDupliStaff = new DataTable();
                dtList = objsector.getStateList(null);
                ViewBag.StateList = dtList;
                DataTable dt = new DataTable();
                dt = objVendor.getOfferVendorList(obj.VendorId);
                ViewBag.VendorId = obj.VendorId;
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["State"].ToString()))
                        ViewBag.State = dt.Rows[0]["State"].ToString();
                    else
                        ViewBag.State = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["City"].ToString()))
                        ViewBag.City = dt.Rows[0]["City"].ToString();
                    else
                        ViewBag.City = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["FirstName"].ToString()))
                        ViewBag.FirstName = dt.Rows[0]["FirstName"].ToString();
                    else
                        ViewBag.FirstName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["LastName"].ToString()))
                        ViewBag.LastName = dt.Rows[0]["LastName"].ToString();
                    else
                        ViewBag.LastName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["MobileNo"].ToString()))
                        ViewBag.MobileNo = dt.Rows[0]["MobileNo"].ToString();
                    else
                        ViewBag.MobileNo = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Email"].ToString()))
                        ViewBag.Email = dt.Rows[0]["Email"].ToString();
                    else
                        ViewBag.Email = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Address"].ToString()))
                    {
                        ViewBag.Address = dt.Rows[0]["Address"].ToString();
                        obj.Address = dt.Rows[0]["Address"].ToString();
                    }

                    else
                    {
                        ViewBag.Address = "";
                        obj.Address = "";
                    }

                    if (!string.IsNullOrEmpty(dt.Rows[0]["Photo"].ToString()))
                        ViewBag.Photo = dt.Rows[0]["Photo"].ToString();
                    else
                        ViewBag.Photo = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["UserName"].ToString()))
                        ViewBag.UserName = dt.Rows[0]["UserName"].ToString();
                    else
                        ViewBag.UserName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Password"].ToString()))
                        ViewBag.Password = dt.Rows[0]["Password"].ToString();
                    else
                        ViewBag.Password = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["CompanyName"].ToString()))
                        ViewBag.CompanyName = dt.Rows[0]["CompanyName"].ToString();
                    else
                        ViewBag.CompanyName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["PanCardNo"].ToString()))
                        ViewBag.PanCardNo = dt.Rows[0]["PanCardNo"].ToString();
                    else
                        ViewBag.PanCardNo = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["GSTNo"].ToString()))
                        ViewBag.GSTNo = dt.Rows[0]["GSTNo"].ToString();
                    else
                        ViewBag.GSTNo = "";


                    if (!string.IsNullOrEmpty(dt.Rows[0]["VendorType"].ToString()))
                        ViewBag.VendorType = dt.Rows[0]["VendorType"].ToString();
                    else
                        ViewBag.VendorType = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["MasterCat"].ToString()))
                        ViewBag.MasterCat = dt.Rows[0]["MasterCat"].ToString();
                    else
                        ViewBag.MasterCat = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["Vendorstore"].ToString()))
                        ViewBag.Vendorstore = dt.Rows[0]["Vendorstore"].ToString();
                    else
                        ViewBag.Vendorstore = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["VendorCat"].ToString()))
                        ViewBag.VendorCat = dt.Rows[0]["VendorCat"].ToString();
                    else
                        ViewBag.VendorCat = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["Slider1"].ToString()))
                        ViewBag.Slider1 = dt.Rows[0]["Slider1"].ToString();
                    else
                        ViewBag.Slider1 = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["Slider2"].ToString()))
                        ViewBag.Slider2 = dt.Rows[0]["Slider2"].ToString();
                    else
                        ViewBag.Slider2 = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["MilkywayPer"].ToString()))
                        ViewBag.MilkywayPer = dt.Rows[0]["MilkywayPer"].ToString();
                    else
                        ViewBag.MilkywayPer = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["DiscountPer"].ToString()))
                        ViewBag.DiscountPer = dt.Rows[0]["DiscountPer"].ToString();
                    else
                        ViewBag.DiscountPer = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["DiscountActive"].ToString()))
                        ViewBag.DiscountActive = dt.Rows[0]["DiscountActive"].ToString();
                    else
                        ViewBag.DiscountActive = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["Vendorterms"].ToString()))
                    {
                        ViewBag.Vendorterms = dt.Rows[0]["Vendorterms"].ToString();
                        obj.VendorTerms = dt.Rows[0]["Vendorterms"].ToString();
                    }

                    else
                    {
                        ViewBag.Vendorterms = "";
                        obj.VendorTerms = "";
                    }

                }

                dt = objVendor.getOfferVendorProductList(obj.VendorId);
                if (dt.Rows.Count > 0)
                {
                    ViewBag.ProductList = dt;
                }
            }
            return View(obj);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MilkWayIndia.Models;
using System.Data;
using System.Web.Helpers;
using System.IO;
using System.Collections;

namespace MilkWayIndia.Controllers
{
    public class CategoryController : Controller
    {
        Product objProdt = new Product();
        Helper dHelper = new Helper();
        Dictionary<string, object> res = new Dictionary<string, object>();
        // GET: Category
        public ActionResult Index()
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
            dt = objProdt.BindCategoryList(null);
            ViewBag.CategoryList = dt;

            if (Session["AlertMessage"] != null)
            {
                ViewBag.Message = "<div class=\"alert alert-success alert-dismissible\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-hidden=\"true\">×</button>" + Session["AlertMessage"] + "</div>";
                Session.Remove("AlertMessage");
            }
            return View();
        }

        public void PopulateDrp()
        {
            var category = objProdt.GetAllMaincategory();
            var lstCategory = (from c in category.AsEnumerable()
                               select new Product
                               {
                                   CategoryId = c.Field<int>("ID"),
                                   Category = c.Field<string>("CategoryName").ToString()
                               }).ToList();
            ViewBag.lstCategory = new SelectList(lstCategory, "CategoryId", "Category");
        }

        public ActionResult Create()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            PopulateDrp();
            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            return View();
        }

        public ActionResult Edit(int? ID)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            Product model = new Product();
            PopulateDrp();

            DataTable dt = objProdt.BindCategory(ID);
            if (dt.Rows.Count > 0)
            {
                model.CategoryId = Convert.ToInt32(dt.Rows[0]["Id"].ToString());
                if (!string.IsNullOrEmpty(dt.Rows[0]["ParentCategoryId"].ToString()))
                    model.ParentCategoryId = Convert.ToInt32(dt.Rows[0]["ParentCategoryId"].ToString());

                if (!string.IsNullOrEmpty(dt.Rows[0]["CategoryName"].ToString()))
                    model.Category = dt.Rows[0]["CategoryName"].ToString();

                if (!string.IsNullOrEmpty(dt.Rows[0]["OrderBy"].ToString()))
                    model.OrderBy = Convert.ToInt32(dt.Rows[0]["OrderBy"].ToString());
                else
                    model.OrderBy = 0;
                if (!string.IsNullOrEmpty(dt.Rows[0]["Image"].ToString()))
                {
                    ViewBag.Image = dt.Rows[0]["Image"].ToString();
                    model.Image = dt.Rows[0]["Image"] == null ? "" : dt.Rows[0]["Image"].ToString();
                }
                else
                    ViewBag.Image = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["MinAmount"].ToString()))
                    model.MinAmount = Convert.ToDecimal(dt.Rows[0]["MinAmount"]);
                if (!string.IsNullOrEmpty(dt.Rows[0]["MaxAmount"].ToString()))
                    model.MaxAmount = Convert.ToDecimal(dt.Rows[0]["MaxAmount"]);
                if (!string.IsNullOrEmpty(dt.Rows[0]["FromTime"].ToString()))
                    model.FromTime = TimeSpan.Parse(dt.Rows[0]["FromTime"].ToString());
                if (!string.IsNullOrEmpty(dt.Rows[0]["ToTime"].ToString()))
                    model.ToTime = TimeSpan.Parse(dt.Rows[0]["ToTime"].ToString());

                if (!string.IsNullOrEmpty(dt.Rows[0]["DeliveryFrom"].ToString()))
                    model.DeliveryFrom = TimeSpan.Parse(dt.Rows[0]["DeliveryFrom"].ToString());
                if (!string.IsNullOrEmpty(dt.Rows[0]["DeliveryTo"].ToString()))
                    model.DeliveryTo = TimeSpan.Parse(dt.Rows[0]["DeliveryTo"].ToString());
                ViewBag.Active = Convert.ToBoolean(dt.Rows[0]["IsActive"]);

                return View(model);
            }
            return View();
        }

        public int InsertCategory(Product model, HttpPostedFileBase Document1)
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

                        fileName = dHelper.RemoveIllegalCharacters(fileName);
                        if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                            ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
                        else
                            fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                        
                        path = Path.Combine(Server.MapPath("~/image/productcategory/"), fname);
                        img.Save(path);
                        model.Image = fname;
                    }
                    catch (Exception ex)
                    {
                        ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                    }
                }
            }
            if (model.CategoryId > 0)
            {
                int addresult = objProdt.UpdateProdtcategory(model);
                return addresult;
            }
            else
            {
                int addresult = objProdt.InsertProdtcategory(model);
                return addresult;
            }
        }

        [HttpPost]
        public ActionResult Create(Product model, HttpPostedFileBase Document1)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            PopulateDrp();
            string active = Request["IsActive"].Split(',')[0];
            if (!string.IsNullOrEmpty(active))
                model.IsActive = Convert.ToBoolean(active);
            //DataTable dtDuplicateg = new DataTable();//Dupliate category remove on 08/01/2022
            //dtDuplicateg = objProdt.CheckDuplicate(model.Category);
            //if (dtDuplicateg.Rows.Count > 0)
            //{
            //    int SId = Convert.ToInt32(dtDuplicateg.Rows[0]["Id"]);
            //    if (SId != model.CategoryId)
            //    {
            //        ViewBag.SuccessMsg = "Category Already Exits!!!";
            //        return View(model);
            //    }
            //}
            var response = InsertCategory(model, Document1);
            if (response > 0)
                ViewBag.SuccessMsg = "Category Inserted Successfully!!!";
            else
                ViewBag.SuccessMsg = "Category Not Inserted!!!";

            return View();
        }

        [HttpPost]
        public ActionResult Edit(Product model, HttpPostedFileBase Document1)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            PopulateDrp();
            string active = Request["IsActive"].Split(',')[0];
            if (!string.IsNullOrEmpty(active))
                model.IsActive = Convert.ToBoolean(active);
            //DataTable dtDuplicateg = new DataTable();
            //dtDuplicateg = objProdt.CheckDuplicate(model.Category);
            //if (dtDuplicateg.Rows.Count > 0)
            //{
            //    int SId = Convert.ToInt32(dtDuplicateg.Rows[0]["Id"]);
            //    if (SId != model.CategoryId)
            //    {
            //        ViewBag.SuccessMsg = "Category Already Exits!!!";
            //        return View(model);
            //    }
            //}
            var response = InsertCategory(model, Document1);
            if (response > 0)
                ViewBag.SuccessMsg = "Category Update Successfully!!!";
            else
                ViewBag.SuccessMsg = "Category Not Inserted!!!";

            return View();
        }

        public ActionResult ActiveProductCateg(string pid)
        {
            int updproductstatus = objProdt.updateActiveProductCatgStatus(pid);
            return Redirect("/Category/Index");
        }

        public ActionResult InActiveProductCateg(string pid)
        {
            int updproductstatus = objProdt.updateInActiveProductCatgStatus(pid);
            return Redirect("/Category/Index");
        }

        public ActionResult AssignSubCategory()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var category = objProdt.GetAllMaincategory();
            ViewBag.lstCategory = category;


            DataTable dtcategory = new DataTable();
            dtcategory = objProdt.GetAllMaincategorynew();
            ViewBag.Category = dtcategory;
            return View();
        }

        public JsonResult GetProductByCategory(int? id)
        {
            Product objprod = new Product();
            ArrayList list = new ArrayList();
            DataTable dtnew = objprod.BindCategActiveProuct(id);
            int rc = dtnew.Rows.Count;//for test            

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

        public JsonResult GetProductBySubCategory(int? id,int? pcat)
        {
            Product objprod = new Product();
            ArrayList list = new ArrayList();
            DataTable dtnew = objprod.BindCategsubActiveProuct(id, pcat);
            int rc = dtnew.Rows.Count;//for test            

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

        public JsonResult UpdateCategory(FormCollection frm,string[] chk)
        {
            try
            {
                foreach (var item in chk)
                {
                    objProdt.UpdateCategoryInProduct(item, frm["ddlSubCategory"], frm["ddlpCategory"], frm["ddlSubpCategory"]);
                }
                res["success"] = "1";
            }
            catch
            {

            }
            
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}
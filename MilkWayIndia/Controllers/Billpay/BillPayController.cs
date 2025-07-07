using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MilkWayIndia.Abstract;
using MilkWayIndia.Entity;
using MilkWayIndia.Concrete;
using MilkWayIndia.Models;
using System.IO;
using System.Web.Hosting;

namespace MilkWayIndia.Controllers.Billpay
{
    public class BillPayController : Controller
    {
        Dictionary<string, object> res = new Dictionary<string, object>();
        private IBillPay _BillpayRepo;
        public BillPayController()
        {
            this._BillpayRepo = new BillPayRepository();
        }
        // GET: BillPay
        public ActionResult Index()
        {
            return View();
        }

        #region Billpay Circle
        public ActionResult CircleList()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            //if (HttpContext.Session["UserId"] == null)
            //    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var circle = _BillpayRepo.GetAllBillPayCircle();
            return View(circle);
        }

        public PartialViewResult AddCircle()
        {
            ViewBag.ModelTitle = "Add Circle";
            return PartialView("_CreateOrUpdateCircle");
        }

        [HttpPost]
        public JsonResult AddCirlce(tblBillPayCircle model)
        {
            try
            {
                res["status"] = "0";
                var circle = _BillpayRepo.ValidateCircleName(model.Name);
                var circle1 = model.CircleCode;
                //if (circle == true)
                //{
                //    res["message"] = "Error : Duplicate Circle Name Found.";
                //}
                //else
                //{
                    model.IsActive = true;
                    model.IsDeleted = false;
                    _BillpayRepo.SaveBillPayCircle(model);
                    res["status"] = "1";
                //}
            }
            catch (Exception ex) { res["message"] = "Error : " + ex.Message; }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult EditCircle(int ID)
        {
            ViewBag.ModelTitle = "Edit Circle";
            var circle = _BillpayRepo.GetBillPayCircleByID(ID);
            if (circle != null)
            {
                BillPayCircleVM model = new BillPayCircleVM();
                model.ID = circle.ID;
                model.Name = circle.Name;
                model.CircleCode = circle.CircleCode;
                model.SortOrder = circle.SortOrder;
                return PartialView("_CreateOrUpdateCircle", model);
            }
            return PartialView("_CreateOrUpdateCircle");
        }

        public ActionResult UpdateCircle(int ID)
        {
            _BillpayRepo.UpdateBillPayCircleStatus(ID);
            return Redirect("/billPay/circlelist");
        }

        public ActionResult DeleteCircle(int ID)
        {
            _BillpayRepo.DeleteBillPayCircle(ID);
            return Redirect("/billPay/circlelist");
        }

        public JsonResult ValidateCircleName(string Name)
        {
            var circle = _BillpayRepo.ValidateCircleName(Name);
            if (circle == true)
            {
                res["status"] = "0";
                res["message"] = "Error : Duplicate Circle Name Found.";
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Billpay Services
        public ActionResult ServiceList()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            //if (HttpContext.Session["UserId"] == null)
            //    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var circle = _BillpayRepo.GetAllBillPayService();
            return View(circle);
        }

        public PartialViewResult AddService()
        {
            ViewBag.ModelTitle = "Add Service";
            return PartialView("_CreateOrUpdateService");
        }

        [HttpPost]
        public JsonResult AddService(tblBillPayService model, string photostr, string fileName)
        {
            try
            {
                res["status"] = "0";
                model.IsActive = true;
                model.IsDeleted = false;
                if (photostr != "")
                {
                    var imageParts = photostr.Split(',').ToList<string>();
                    string _FileName = Guid.NewGuid() + Path.GetExtension(fileName);
                    string filePath = HostingEnvironment.MapPath("~/image/" + _FileName);
                    var bytes = Convert.FromBase64String(imageParts[1]);
                    using (var imageFile = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
                    }
                    string filePath1 = "/image/" + _FileName;
                    model.PhotoPath = filePath1;
                }

                _BillpayRepo.SaveBillPayService(model);
                res["status"] = "1";
            }
            catch (Exception ex) { res["message"] = "Error : " + ex.Message; }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult EditService(int ID)
        {
            ViewBag.ModelTitle = "Edit Service";
            var service = _BillpayRepo.GetBillPayServiceByID(ID);
            if (service != null)
            {
                BillPaySerciceVM model = new BillPaySerciceVM();
                model.ID = service.ID;
                model.Name = service.Name;
                ViewBag.PhotoPath = Helper.PhotoFolderPath + "/" + service.PhotoPath;
                model.SortOrder = service.SortOrder == null ? 0 : service.SortOrder;
                return PartialView("_CreateOrUpdateService", model);
            }
            return PartialView("_CreateOrUpdateService");
        }

        public ActionResult UpdateService(int ID)
        {
            _BillpayRepo.UpdateBillPayServiceStatus(ID);
            return Redirect("/billPay/servicelist");
        }

        public ActionResult DeleteService(int ID)
        {
            _BillpayRepo.DeleteBillPayService(ID);
            return Redirect("/billPay/servicelist");
        }
        #endregion

        #region Billpay Cities
        public ActionResult CityList()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            //if (HttpContext.Session["UserId"] == null)
            //    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var city = _BillpayRepo.GetAllBillPayCity();
            return View(city);
        }

        public PartialViewResult AddCity()
        {
            ViewBag.ModelTitle = "Add City";
            return PartialView("_CreateOrUpdateCity");
        }

        [HttpPost]
        public JsonResult AddCity(tblBillPayCity model)
        {
            try
            {
                res["status"] = "0";
                if (model.ID == null)
                {
                    var city = _BillpayRepo.ValidateCityName(model.Name);
                    if (city == true)
                    {
                        res["message"] = "Error : Duplicate City Name Found.";
                        return Json(res, JsonRequestBehavior.AllowGet);
                    }
                }
                model.IsActive = true;
                model.IsDeleted = false;
                _BillpayRepo.SaveBillPayCity(model);
                res["status"] = "1";

            }
            catch (Exception ex) { res["message"] = "Error : " + ex.Message; }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult EditCity(int ID)
        {
            ViewBag.ModelTitle = "Edit City";
            var circle = _BillpayRepo.GetBillPayCityByID(ID);
            if (circle != null)
            {
                BillPayCircleVM model = new BillPayCircleVM();
                model.ID = circle.ID;
                model.Name = circle.Name;
                return PartialView("_CreateOrUpdateCity", model);
            }
            return PartialView("_CreateOrUpdateCity");
        }

        public ActionResult UpdateCity(int ID)
        {
            _BillpayRepo.UpdateBillPayCityStatus(ID);
            return Redirect("/billPay/citylist");
        }

        public ActionResult DeleteCity(int ID)
        {
            _BillpayRepo.DeleteBillPayCity(ID);
            return Redirect("/billPay/circlelist");
        }

        #endregion

        #region Billpay Operator
        public ActionResult OperatorList()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            //if (HttpContext.Session["UserId"] == null)
            //    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
          var service=  _BillpayRepo.GetAllBillPayService();
            var op = _BillpayRepo.GetAllBillPayOperator();
            return View(op);
        }      

        public ActionResult AddOperator()
        {
            ViewBag.ModelTitle = "Add Operator";
            CashBack objservicelist = new CashBack();

            ViewBag.ServiceList = objservicelist.getServiceList(null);
            return PartialView("_CreateOrUpdateOperator");
        }

        [HttpPost]
        public JsonResult AddOperator(tblBillPayOperator model)
        {
            try
            {
                res["status"] = "0";
                if (model.ID == null)
                {
                    var city = _BillpayRepo.ValidateOperatorName(model.Name);
                    if (city == true)
                    {
                        res["message"] = "Error : Duplicate Operator Name Found.";
                        return Json(res, JsonRequestBehavior.AllowGet);
                    }
                }
                model.IsActive = true;
                model.IsDeleted = false;
                string a = Request["ddloperator"];
                model.Type = Request["ddloperator"];
                _BillpayRepo.SaveBillPayOperator(model);
                res["status"] = "1";

            }
            catch (Exception ex) { res["message"] = "Error : " + ex.Message; }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult EditOperator(int ID)
        {
            ViewBag.ModelTitle = "Edit Operator";
            var circle = _BillpayRepo.GetBillPayOperatorByID(ID);
            if (circle != null)
            {
                CashBack objservicelist = new CashBack();

                ViewBag.ServiceList = objservicelist.getServiceList(null);
                BillPayOperatorVM model = new BillPayOperatorVM();
                model.ID = circle.ID;
                model.Name = circle.Name;
                model.OperatorCode = circle.OperatorCode;
                model.Type = circle.Type;

                ViewBag.Servicename = model.Type.ToString();


                return PartialView("_CreateOrUpdateOperator", model);
            }
            return PartialView("_CreateOrUpdateCity");
        }

        public ActionResult UpdateOperator(int ID)
        {

            string type = Request["ddloperator"];
            _BillpayRepo.UpdateBillPayOperatorStatus(ID,type);
            return Redirect("/billPay/Operatorlist");
        }

        public ActionResult DeleteOperator(int ID)
        {
            _BillpayRepo.DeleteBillPayOperator(ID);
            return Redirect("/billPay/Operatorlist");
        }
        #endregion

        #region Billpay Provider
        public ActionResult ProviderList()
        {
            //if (Request.Cookies["gstusr"] == null)
            //    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var op = _BillpayRepo.GetAllBillPayProvider();
            return View(op);
        }

        public void PopulateDropdown()
        {
            var lstService = _BillpayRepo.GetAllBillPayService().Where(s => s.IsActive == true);
            ViewBag.lstService = new SelectList(lstService, "ID", "Name");

            var lstCircle = _BillpayRepo.GetAllBillPayCircle().Where(s => s.IsActive == true);
            ViewBag.lstCircle = new SelectList(lstCircle, "ID", "Name");

            var lstCity = _BillpayRepo.GetAllBillPayCity().Where(s => s.IsActive == true);
            ViewBag.lstCity = new SelectList(lstCity, "ID", "Name");

            var lstOperator = _BillpayRepo.GetAllBillPayOperator().Where(s => s.IsActive == true);
            ViewBag.lstOperator = new SelectList(lstOperator, "ID", "Name");
        }

        public ActionResult AddProvider()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            PopulateDropdown();          
            return View();
        }

        [HttpPost]
        public ActionResult AddProvider(tblBillPayProvider model)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            PopulateDropdown();
            var IsPartial = Request.Form["IsPartial"];
            if (IsPartial == "on")
                model.IsPartial = true;
            model.IsActive = true;
            model.IsDeleted = false;
            var op = _BillpayRepo.SaveBillPayProvider(model);
            if (op > 0)
            {
                ViewBag.SuccessMsg = "Operator Inserted Successfully!!!";
                ModelState.Clear();
                return View();
            }            
            return View(model);
        }

        public ActionResult EditProvider(int ID)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            PopulateDropdown();
            var provider = _BillpayRepo.GetBillPayProviderByID(ID);
            if (provider != null)
            {
                return View(provider);
            }
            return View();
        }

        [HttpPost]
        public ActionResult EditProvider(tblBillPayProvider model)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            PopulateDropdown();
            
            var IsPartial = Request.Form["IsPartial"];
            if (IsPartial == "on")
                model.IsPartial = true;
            model.IsActive = true;
            model.IsDeleted = false;
            var op = _BillpayRepo.UpdateBillPayProvider(model);
            if (op > 0)
                ViewBag.SuccessMsg = "Operator Updated Successfully!!!";
            ModelState.Clear();
            return View();
        }
        #endregion
    }
}
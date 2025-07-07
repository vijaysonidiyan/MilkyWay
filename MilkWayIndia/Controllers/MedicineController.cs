using MilkWayIndia.Abstract;
using MilkWayIndia.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MilkWayIndia.Models;

namespace MilkWayIndia.Controllers
{
    public class MedicineController : Controller
    {
        private IMedicine _MedicineRepo;
        public MedicineController()
        {
            this._MedicineRepo = new MedicineRepository();
        }
        // GET: Medicine
        public ActionResult Index()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;               
            }
            else
            {
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            }
            return View();
        }
        
        public JsonResult GetAllMedicine()
        {
            var medicine = _MedicineRepo.GetAllMedicine();                        
            return Json(new { data = medicine },JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int ID)
        {
            _MedicineRepo.DeleteMedicine(ID);
            return Redirect("/medicine/index");
        }
    }
}
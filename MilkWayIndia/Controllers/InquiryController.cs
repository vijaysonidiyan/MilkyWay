using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MilkWayIndia.Models;

namespace MilkWayIndia.Controllers
{
    public class InquiryController : Controller
    {
        clsCommon _clsCommon = new clsCommon();
        // GET: Inquiry
        public ActionResult Index()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                CustomerOrder obj = new CustomerOrder();
                DataTable dtinquiry = new DataTable();
                dtinquiry = obj.getCustomerInquiry();                
                ViewBag.InquiryList = dtinquiry;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }            
        }

        public ActionResult Delete(int? ID)
        {
            var str = _clsCommon.deletedata("tbl_Inquiry", "Id='" + ID + "'");
            return Redirect("/inquiry/index");
        }
    }
}
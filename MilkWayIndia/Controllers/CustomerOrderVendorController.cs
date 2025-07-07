using CrystalDecisions.CrystalReports.Engine;
using HtmlAgilityPack;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using MilkWayIndia.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Rotativa;
using Rotativa.Options;
namespace MilkWayIndia.Controllers
{
    public class CustomerOrderVendorController : Controller
    {
        // GET: CustomerOrderVendor
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        Customer objcust = new Customer();
        Subscription objsub = new Subscription();

        [HttpGet]
        public ActionResult DeliveryBoyDailyOrdervendor()
        {
            //if (Request.Cookies["gstusr"] == null)
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);


            CustomerOrder objorder = new CustomerOrder();
            DataTable dt = new DataTable();
            dt = objcust.GetAllCustomer(null);
            ViewBag.Customer = dt;

            Staff objStaff = new Staff();
            DataTable dtStaff = new DataTable();
            dtStaff = objStaff.getDeliveryBoyList(null);
            ViewBag.Staff = dtStaff;
            ViewBag.Testing = dtStaff.Rows.Count.ToString();
            DataTable dtList = new DataTable();
            CustomerOrderVendor objorder1 = new CustomerOrderVendor();
            //dtList = objorder1.getDeliveryBoyCustomerOrder(null, null, null, null, null);
            dtList = objorder.getDeliveryBoyCustomerOrder(null, null, System.DateTime.Now.Date.AddDays(1), System.DateTime.Now.Date.AddDays(1), null);
            ViewBag.ProductorderList = dtList;

            return View();
        }


        [HttpPost]
        public ActionResult DeliveryBoyDailyOrdervendor(FormCollection form, CustomerOrder objorder)
        {
            //if (Request.Cookies["gstusr"] == null)
            //    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var submit = Request["submit"];
            Staff objstaff = new Staff();
            Subscription objsub = new Subscription();
            string StaffId = Request["ddlStaff"];
            if (!string.IsNullOrEmpty(StaffId) && Convert.ToInt32(StaffId) != 0)
            {
                objorder.StaffId = Convert.ToInt32(StaffId);
            }
            string CustomerId = Request["ddlCustomer"];
            if (!string.IsNullOrEmpty(CustomerId) && Convert.ToInt32(CustomerId) != 0)
            {
                objorder.CustomerId = Convert.ToInt32(CustomerId);
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
            string StatusId = Request["ddlStatus"];
            if (!string.IsNullOrEmpty(StatusId))
            {
                objorder.Status = StatusId;
            }
            var _fdate = objorder.FromDate.Value.ToString("dd-MM-yyyy");
            var _tdate = objorder.ToDate.Value.ToString("dd-MM-yyyy");

            DataTable dtList = new DataTable();
            dtList = objorder.getDeliveryBoyCustomerOrdervendor(objorder.StaffId, objorder.CustomerId, objorder.FromDate, objorder.ToDate, objorder.Status);
            ViewBag.ProductorderList = dtList;
            if (submit == "submit")
            {
                DataTable dt = new DataTable();
                dt = objcust.GetAllCustomer(null);
                ViewBag.Customer = dt;

                Staff objStaff = new Staff();
                DataTable dtStaff = new DataTable();
                dtStaff = objStaff.getDeliveryBoyList(null);
                ViewBag.Staff = dtStaff;

                ViewBag.DeliveryBoyId = objorder.StaffId;
                ViewBag.CustomerId = objorder.CustomerId;
                if (!string.IsNullOrEmpty(fdate.ToString()))
                {
                    ViewBag.FromDate = fdate;
                }
                if (!string.IsNullOrEmpty(tdate.ToString()))
                {
                    ViewBag.ToDate = tdate;
                }
                ViewBag.StatusId = objorder.Status;
                return View();
            }
            else if (submit == "print")
            {
                string query = string.Format("DeliveryboyId={0}&CustomerId={1}&FDate={2}&TDate={3}&status={4}",
                    objorder.StaffId, objorder.CustomerId, _fdate, _tdate, objorder.Status);
                return Redirect("/Reportnew/DeliveryBoyDailyReportvendor1?" + query);
            }
            else if (submit == "export")
            {
                string query = string.Format("DeliveryboyId={0}&CustomerId={1}&FDate={2}&TDate={3}&status={4}",
                    objorder.StaffId, objorder.CustomerId, _fdate, _tdate, objorder.Status);
                //return new UrlAsPdf("/customerorder/DeliveryBoyDailyReport?" + query);
                var r = new PartialViewAsPdf("DeliveryBoyDailyReportvendor", new
                {
                    DeliveryboyId = objorder.StaffId,
                    CustomerId = objorder.CustomerId,
                    FDate = _fdate,
                    TDate = _tdate,
                    status = objorder.Status
                })
                { FileName = "DeliveryBoyDailyReport.pdf" };
                return r;
            }
            return View();
        }
    }
}
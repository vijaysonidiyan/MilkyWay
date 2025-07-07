using MilkWayIndia.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.IO;
using MilkWayIndia.Abstract;
using MilkWayIndia.Entity;

using MilkWayIndia.Concrete;
namespace MilkWayIndia.Controllers
{
    public class SubscriptionController : Controller
    {
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        Dictionary<string, object> res = new Dictionary<string, object>();
        private ISubscription _subRepo;

        public SubscriptionController()
        {
            this._subRepo = new SubscriptionRepository();
        }
        // private ISubscription _CustomerRepo;
        Subscription objsub = new Subscription();
        // GET: Subscription
        [HttpGet]
        public ActionResult AddSubscription()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

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
        public ActionResult AddSubscription(Subscription obj,FormCollection form)
        {
            Customer objcust = new Customer();
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                int addresult = 0;
                //check Duplicate
                DataTable dtdupli = obj.getDuplicateSubscription(obj.Name);
                if (dtdupli.Rows.Count > 0)
                { ViewBag.SuccessMsg = "Subscription Name Already Exits!!!"; }
                else
                {
                    //check alredy exits

                    addresult = obj.InsertSubscriptionMst(obj);
                    if (addresult > 0)
                    { ViewBag.SuccessMsg = "Subscription Inserted Successfully!!!"; }
                    else
                    { ViewBag.SuccessMsg = "Subscription Not Inserted!!!"; }
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
        public ActionResult EditSubscription(int id =0)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                DataTable dt = objsub.getSubscriptionList(id);
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Name"].ToString()))
                        ViewBag.Name = dt.Rows[0]["Name"].ToString();
                    else
                        ViewBag.Name = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Days"].ToString()))
                        ViewBag.Days = dt.Rows[0]["Days"].ToString();
                    else
                        ViewBag.Days = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Amount"].ToString()))
                        ViewBag.Amount = dt.Rows[0]["Amount"].ToString();
                    else
                        ViewBag.Amount = "";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditSubscription(Subscription obj, FormCollection form)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                int addresult = 0;
                //check Duplicate
                DataTable dtdupli = obj.getDuplicateSubscription(obj.Name);
                if (dtdupli.Rows.Count > 0)
                {
                    int SId = Convert.ToInt32(dtdupli.Rows[0]["Id"]);
                    if (SId == obj.Id)
                    {
                        addresult = obj.UpdateSubscriptionMst(obj);
                        if (addresult > 0)
                        { ViewBag.SuccessMsg = "Subscription Updated Successfully!!!"; }
                        else
                        { ViewBag.SuccessMsg = "Subscription Not Updated!!!"; }
                    }
                    else
                    { ViewBag.SuccessMsg = "Subscription Name Already Exits!!!"; }
                }
                else
                {
                    addresult = obj.UpdateSubscriptionMst(obj);
                    if (addresult > 0)
                    { ViewBag.SuccessMsg = "Subscription Updated Successfully!!!"; }
                    else
                    { ViewBag.SuccessMsg = "Subscription Not Updated!!!"; }

                }
                DataTable dt = objsub.getSubscriptionList(obj.Id);
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Name"].ToString()))
                        ViewBag.Name = dt.Rows[0]["Name"].ToString();
                    else
                        ViewBag.Name = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Days"].ToString()))
                        ViewBag.Days = dt.Rows[0]["Days"].ToString();
                    else
                        ViewBag.Days = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Amount"].ToString()))
                        ViewBag.Amount = dt.Rows[0]["Amount"].ToString();
                    else
                        ViewBag.Amount = "";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpGet]
        public ActionResult SubscriptionList()
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
                dtList = objsub.getSubscriptionList(null);
                ViewBag.SubscriptionList = dtList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult DeleteSubscription(int id)
        {
            try
            {
                int delresult = objsub.DeleteSubscription(id);
                return RedirectToAction("SubscriptionList");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_custsubscription_subscription"))
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
            return RedirectToAction("SubscriptionList");
        }


        //customer subscription
        [HttpGet]
        public ActionResult AddCustomerSubscription()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                Customer objcust = new Customer();
                DataTable dt = new DataTable();
                dt = objcust.BindCustomer(null);
                ViewBag.Customer = dt;

                DataTable dtsubscri = new DataTable();
                dtsubscri = objsub.getSubscriptionList(null);
                ViewBag.Subscription = dtsubscri;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddCustomerSubscription(Subscription obj,FormCollection form)
        {
            Customer objcust = new Customer();
            string SubscriptionBreak="";

           
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                int addresult = 0;
                string customer = Request["ddlCustomer"];
                if (!string.IsNullOrEmpty(customer))
                {
                    obj.CustomerId = Convert.ToInt32(customer);
                }

                //get subto date from customerMAster
                DataTable cusdt = objcust.BindCustomer(obj.CustomerId);
                string td = cusdt.Rows[0]["SubnToDate"].ToString();
                if(!string.IsNullOrEmpty(td))
                {
                    objcust.SubnToDate = DateTime.Parse(td, System.Globalization.CultureInfo.InvariantCulture);
                }
               

                string subscri = Request["ddlSubscription"];
                if (!string.IsNullOrEmpty(subscri))
                {
                    obj.SubscriptionId = Convert.ToInt32(subscri);
                }
                var fdate = Request["FromDate"];
                if (!string.IsNullOrEmpty(fdate.ToString()))
                {
                    obj.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
                    obj.sFdate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
                }
                var tdate = Request["ToDate"];
                if (!string.IsNullOrEmpty(tdate.ToString()))
                {
                    obj.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
                    obj.sTdate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
                }
                ////if (DateTime.Now.Hour < 20)
                ////{
                ////}
                ////else{
                ////    obj.FromDate = obj.FromDate.AddDays(1);
                ////obj.ToDate = obj.ToDate.AddDays(1);}

                //chcek wallet amount
                decimal Walletbal = 0, TotalCredit = 0, TotalDebit = 0;
                DataTable dtprodRecord = new DataTable();
                dtprodRecord = obj.getCustomerWallet(obj.CustomerId);
                int userRecords = dtprodRecord.Rows.Count;
                if (userRecords > 0)
                {
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["Amt"].ToString()))
                        TotalCredit = Convert.ToDecimal(dtprodRecord.Rows[0]["Amt"]);
                    if (userRecords > 1)
                    {
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[1]["Amt"].ToString()))
                            TotalDebit = Convert.ToDecimal(dtprodRecord.Rows[1]["Amt"]);
                    }
                    Walletbal = TotalCredit - TotalDebit;
                }

                
                obj.PaymentStatus = "Yes";
                obj.SubscriptionStatus = "Open";
                int NoDays = 0;
                //get No of days from Subscription master
                DataTable dtSubscribe = new DataTable();
                dtSubscribe = obj.getSubscriptionList(obj.SubscriptionId);
                if (dtSubscribe.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtSubscribe.Rows[0]["Days"].ToString()))
                    {
                        NoDays = Convert.ToInt32(dtSubscribe.Rows[0]["Days"]);
                        //Amount = Convert.ToDecimal(dtSubscribe.Rows[0]["Amount"]);
                        obj.Days = NoDays - 1;
                    }
                }
                if (Walletbal > obj.Amount)
                {
                    DataTable Checkuserexits = obj.CheckCustSubnExits(obj.CustomerId, null, null, null);
                    if (Checkuserexits.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(Checkuserexits.Rows[0]["FromDate"].ToString()))
                            obj.FromDate = Convert.ToDateTime(Checkuserexits.Rows[0]["FromDate"]);
                        if (!string.IsNullOrEmpty(Checkuserexits.Rows[0]["ToDate"].ToString()))
                            obj.ToDate = Convert.ToDateTime(Checkuserexits.Rows[0]["ToDate"]);
                        //if()

                        //Check Wether subdate is less than objcust.SubnToDate

                        if (obj.sFdate <= objcust.SubnToDate)
                        {
                            obj.FromDate = obj.ToDate.AddDays(1);
                            obj.ToDate = obj.FromDate.AddDays(obj.Days);
                            //insert
                            addresult = obj.InsertSubscription(obj);
                        }
                        else
                        {
                            obj.FromDate = obj.sFdate;
                            obj.ToDate = obj.sFdate.AddDays(obj.Days);
                            //insert
                            addresult = obj.InsertSubscription(obj);
                            // result = 0;
                            SubscriptionBreak = "true";
                        }


                    }
                    else
                    {
                        obj.FromDate = DateTime.Now;
                        obj.ToDate = DateTime.Today.AddDays(obj.Days);
                        //insert
                        addresult = obj.InsertSubscription(obj);
                        // result = 0;
                    }
                    if (addresult > 0)
                    {
                        //add wallet
                        obj.CustomerId = obj.CustomerId;
                        obj.TotalBalance = obj.Amount;
                        obj.OrderId = 0;
                        obj.BillNo = null;
                        obj.Description = "Purchase Subscription";
                        obj.Type = "Debit";
                        obj.CustSubscriptionId = addresult;
                        int walletresult = obj.InsertWallet(obj);

                        if (Checkuserexits.Rows.Count > 0)
                        {
                            if(SubscriptionBreak != "true")
                            {
                                objcust.Id = obj.CustomerId;
                                objcust.SubnToDate = obj.ToDate;
                                int update = objcust.UpdateCustomerToDate(objcust);
                            }
                            else
                            {
                                objcust.Id = obj.CustomerId;
                                objcust.SubnFromDate = obj.sFdate;
                                objcust.SubnToDate = obj.sFdate.AddDays(obj.Days);
                                int update = objcust.UpdateCustomerFromToDate(objcust);
                            }
                            
                        }
                        else
                        {
                            objcust.Id = obj.CustomerId;
                            objcust.SubnFromDate = obj.FromDate;
                            objcust.SubnToDate = obj.ToDate;
                            int update = objcust.UpdateCustomerFromToDate(objcust);
                        }
                        ViewBag.SuccessMsg = "Customer Subscription Inserted Successfully!!!";
                    }
                    else
                    { ViewBag.SuccessMsg = "Customer Subscription Not Inserted!!!"; }
                }
                else
                { ViewBag.SuccessMsg = "Customer Wallet Balance is Low !!!"; }
                ModelState.Clear();
               // Customer objcust = new Customer();
                DataTable dt = new DataTable();
                dt = objcust.BindCustomer(null);
                ViewBag.Customer = dt;

                DataTable dtsubscri = new DataTable();
                dtsubscri = objsub.getSubscriptionList(null);
                ViewBag.Subscription = dtsubscri;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult EditCustomerSubscription(int id = 0)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                Customer objcust = new Customer();
                DataTable dtcust = new DataTable();
                dtcust = objcust.BindCustomer(null);
                ViewBag.Customer = dtcust;

                DataTable dtsubscri = new DataTable();
                dtsubscri = objsub.getSubscriptionList(null);
                ViewBag.Subscription = dtsubscri;

                DataTable dt = objsub.getCustomerSubscriptionList(id);
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["CustomerId"].ToString()))
                        ViewBag.CustomerId = dt.Rows[0]["CustomerId"].ToString();
                    else
                        ViewBag.CustomerId = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["SubscriptionId"].ToString()))
                        ViewBag.SubscriptionId = dt.Rows[0]["SubscriptionId"].ToString();
                    else
                        ViewBag.SubscriptionId = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Amount"].ToString()))
                        ViewBag.Amount = dt.Rows[0]["Amount"].ToString();
                    else
                        ViewBag.Amount = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["FromDate"].ToString()))
                    {
                        var fdate = dt.Rows[0]["FromDate"].ToString();
                        DateTime dateFromString =
                              DateTime.Parse(fdate.ToString(), null);
                        ViewBag.FromDate = dateFromString.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        ViewBag.FromDate = null;
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[0]["ToDate"].ToString()))
                    {
                        var TDate = dt.Rows[0]["ToDate"].ToString();
                        DateTime dateFromString =
                              DateTime.Parse(TDate.ToString(), null);
                        ViewBag.ToDate = dateFromString.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        ViewBag.ToDate = null;
                    }

                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditCustomerSubscription(Subscription obj, FormCollection form)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                Customer objcust = new Customer();
                DataTable dtcust = new DataTable();
                DataTable dtsubscri = new DataTable();

                string customer = Request["ddlCustomer"];
                if (!string.IsNullOrEmpty(customer))
                {
                    obj.CustomerId = Convert.ToInt32(customer);
                }
                string subscri = Request["ddlSubscription"];
                if (!string.IsNullOrEmpty(subscri))
                {
                    obj.SubscriptionId = Convert.ToInt32(subscri);
                }
                var fdate = Request["FromDate"];
                if (!string.IsNullOrEmpty(fdate.ToString()))
                {
                    obj.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
                }
                var tdate = Request["ToDate"];
                if (!string.IsNullOrEmpty(tdate.ToString()))
                {
                    obj.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
                }
                obj.PaymentStatus = "Yes";
                //check duplicate
                DataTable dtDuplicateSub = obj.CheckCustSubnExits(obj.CustomerId, obj.SubscriptionId, obj.FromDate, obj.ToDate);
                if (dtDuplicateSub.Rows.Count > 0)
                {
                    int CSId = Convert.ToInt32(dtDuplicateSub.Rows[0]["Id"]);
                    if (CSId == objsub.Id)
                    {
                        int addresult = obj.UpdateSubscription(obj);
                        if (addresult > 0)
                        { ViewBag.SuccessMsg = "Customer Subscription Updated Successfully!!!"; }
                        else
                        { ViewBag.SuccessMsg = "Customer Subscription Not Updated!!!"; }
                    }
                    else
                        ViewBag.SuccessMsg = "Customer Subscription Already Exits!!!";
                }
                else
                {
                    int addresult = obj.UpdateSubscription(obj);
                    if (addresult > 0)
                    { ViewBag.SuccessMsg = "Customer Subscription Updated Successfully!!!"; }
                    else
                    { ViewBag.SuccessMsg = "Customer Subscription Not Updated!!!"; }
                }
                dtcust = objcust.BindCustomer(null);
                ViewBag.Customer = dtcust;

                dtsubscri = objsub.getSubscriptionList(null);
                ViewBag.Subscription = dtsubscri;

                DataTable dt = objsub.getCustomerSubscriptionList(obj.Id);
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["CustomerId"].ToString()))
                        ViewBag.CustomerId = dt.Rows[0]["CustomerId"].ToString();
                    else
                        ViewBag.CustomerId = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["SubscriptionId"].ToString()))
                        ViewBag.SubscriptionId = dt.Rows[0]["SubscriptionId"].ToString();
                    else
                        ViewBag.SubscriptionId = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Amount"].ToString()))
                        ViewBag.Amount = dt.Rows[0]["Amount"].ToString();
                    else
                        ViewBag.Amount = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["FromDate"].ToString()))
                    {
                        var Fdate = dt.Rows[0]["FromDate"].ToString();
                        DateTime dateFromString =
                              DateTime.Parse(Fdate.ToString(), null);
                        ViewBag.FromDate = dateFromString.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        ViewBag.FromDate = null;
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[0]["ToDate"].ToString()))
                    {
                        var TDate = dt.Rows[0]["ToDate"].ToString();
                        DateTime dateFromString =
                              DateTime.Parse(TDate.ToString(), null);
                        ViewBag.ToDate = dateFromString.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        ViewBag.ToDate = null;
                    }

                }

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult CustomerSubscriptionList()
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
                dtList = objsub.getCustomerSubscriptionList(null);
                ViewBag.CustomerSubscriptionList = dtList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult DeleteCustomerSubscription(int id)
        {
            try
            {
                //find order found or not
                DateTime FromDate = DateTime.Now;
                DateTime ToDate = DateTime.Now; objsub.CustomerId = 0;
                 DataTable dtdate = new DataTable();
                dtdate = objsub.getCustomerSubscriptionList(id);
                if (dtdate.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtdate.Rows[0]["FDate"].ToString()))
                        FromDate = Convert.ToDateTime(dtdate.Rows[0]["FDate"]);
                    if (!string.IsNullOrEmpty(dtdate.Rows[0]["TDate"].ToString()))
                        ToDate = Convert.ToDateTime(dtdate.Rows[0]["TDate"]);
                    if(!string.IsNullOrEmpty(dtdate.Rows[0]["CustomerId"].ToString()))
                        objsub.CustomerId = Convert.ToInt32(dtdate.Rows[0]["CustomerId"]);

                    //delete order data 
                    //get OrderId from parent to remove child records
                    DataTable dtprodRecord = new DataTable();
                    dtprodRecord = objsub.getCustomerOrderList(objsub.CustomerId, FromDate, ToDate);
                    int userRecords = dtprodRecord.Rows.Count;
                    objsub.OrderId = 0;
                    int DelProductOrder = 0;
                    if (userRecords > 0)
                    {
                        for (int i = 0; i < userRecords; i++)
                        {
                            if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Id"].ToString()))
                            {
                                objsub.OrderId = Convert.ToInt32(dtprodRecord.Rows[i]["Id"]);
                                objsub.OrderDate = FromDate;

                                DelProductOrder = objsub.DeleteCustomerOrder(objsub.OrderId);

                                FromDate = FromDate.AddDays(1);
                            }
                            else
                            {
                            }
                        }
                    }
                }


                //delete subscription
                //int delresult = 0;
                int delresult = objsub.DeleteCustSubscription(id);

                if (delresult > 0)
                {
                    //update in customer subscription date
                    Customer objcust = new Customer();
                    objcust.Id = objsub.CustomerId;
                    //check min and max subscription
                    DateTime SubFromDate = DateTime.Now, SubToDate = DateTime.Now;
                    DataTable dtCustSub = objcust.BindsubDateAllSubsnCustomer(objcust.Id);
                    if (dtCustSub.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dtCustSub.Rows[0]["FDate"].ToString()))
                            SubFromDate = Convert.ToDateTime(dtCustSub.Rows[0]["FDate"]);
                        if (!string.IsNullOrEmpty(dtCustSub.Rows[0]["TDate"].ToString()))
                            SubToDate = Convert.ToDateTime(dtCustSub.Rows[0]["TDate"]);
                    }

                    objcust.SubnFromDate = SubFromDate;
                    objcust.SubnToDate = SubToDate;
                    int update = objcust.UpdateCustomerFromToDate(objcust);
                }

                return RedirectToAction("CustomerSubscriptionList");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_wallet_custsubscription"))
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
            return RedirectToAction("CustomerSubscriptionList");
        }

        [HttpPost]
        public ActionResult GetSubscriptionData(int subscriptionid,string fdate)
        {
            int day = 0;
            DataTable dt = new DataTable();
            string format = "dd/MM/yyyy";
            DateTime dateTime = DateTime.ParseExact(fdate, format, CultureInfo.InvariantCulture);
            string strNewDate = dateTime.ToString("yyyy/MM/dd");

            //DateTime dateFromString = DateTime.Parse(fdate, null);
            //fdate = dateFromString.ToString("yyyy/MM/dd");

            DateTime fmdate = Convert.ToDateTime(strNewDate);
            DateTime tdate = DateTime.Now;
            dt = objsub.getSubscriptionList(subscriptionid);
            if (dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dt.Rows[0]["Days"].ToString()))
                    day = Convert.ToInt32(dt.Rows[0]["Days"]);
            }
            if (day > 0)
            {
                if (!string.IsNullOrEmpty(fdate.ToString()))
                    tdate = fmdate.AddDays(day);
            }
            dt.Columns.Add("Todate",typeof(string));
           // dt = new DataRow();
            dt.Rows[0]["Todate"] = tdate.ToString("yyyy-MM-dd");

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(rows);
            return Json(jsonString, JsonRequestBehavior.AllowGet);
            //return Json(rows);
        }

        [HttpGet]
        public ActionResult CancelCustomerSubscription()
        {
            try
            {
                var fdate = Request["datepicker"];
                if (!string.IsNullOrEmpty(fdate.ToString()))
                {
                    objsub.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
                }
                var tdate = Request["datepicker1"];
                if (!string.IsNullOrEmpty(tdate.ToString()))
                {
                    objsub.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
                }
                var customerId = Request["hdfCustomerId"];
                DataTable dtFindOrder = new DataTable();

                //delete order data
                // int delresult = objsub.DeleteCustSubscriptionOrder(id);
                //delete subscription
               // int delresult1 = objsub.DeleteCustSubscription(id);
                //credit wallet amount return

                return RedirectToAction("CustomerSubscriptionList");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_wallet_custsubscription"))
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
            return RedirectToAction("CustomerSubscriptionList");
        }

        [HttpPost]
        public ActionResult CancelCustomerSubscription(string Customerid, string Fromdate, string Todate,string SubscriptionId)
        {


            try
            {
                Customer objcust = new Customer();
                int DelProductOrder = 0;
                var fdate = Fromdate;
                if (!string.IsNullOrEmpty(fdate.ToString()))
                {
                    objsub.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
                }
                var tdate = Todate;
                if (!string.IsNullOrEmpty(tdate.ToString()))
                {
                    objsub.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
                }
                var customerId = Customerid;
                DataTable dtFindOrder = new DataTable();
                dtFindOrder = objsub.getCustomerOrderFutureAdmin(Convert.ToInt32(Customerid), objsub.FromDate, objsub.ToDate);
                if (dtFindOrder.Rows.Count > 0)
                {
                    for (int i = 0; i < dtFindOrder.Rows.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(dtFindOrder.Rows[i]["Id"].ToString()))
                        {
                            objsub.OrderId = Convert.ToInt32(dtFindOrder.Rows[i]["Id"]);
                            objsub.OrderDate = objsub.FromDate;

                           /// DelProductOrder = objsub.DeleteCustomerOrder(objsub.OrderId);

                            objsub.FromDate = objsub.FromDate.AddDays(1);
                        }
                        else
                        {
                        }
                    }
                    if (DelProductOrder > 0)
                    {
                        //delete subscription

                        //update subscription
                        int updateresult = 1;
                       //// int updateresult = objsub.UpdateStatusCustomerSubscription(objsub);

                        if (updateresult > 0)
                        {
                            DateTime? FromDate = null;
                            DateTime? ToDate = null;
                            //update date on customer table subscription
                            DataTable dtopenSublist = objsub.GetAllOpenSubList(Convert.ToInt32(Customerid));
                            if (dtopenSublist.Rows.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(dtopenSublist.Rows[0]["FromDate"].ToString()))
                                    objcust.SubnFromDate = Convert.ToDateTime(dtopenSublist.Rows[0]["FromDate"]);
                                if (!string.IsNullOrEmpty(dtopenSublist.Rows[0]["ToDate"].ToString()))
                                    objcust.SubnToDate = Convert.ToDateTime(dtopenSublist.Rows[0]["ToDate"]);
                               //// int updateCustSubscrip = objcust.UpdateCustomerFromToDate(objcust);
                            }
                         
                        }
                        string msg = "Your Subscription Cancled Successfully...";
                        // TempData["error"] = String.Format("You can not deleted. No record found.");
                        string jsonString = string.Empty;
                        jsonString = JsonConvert.SerializeObject(msg);
                        return Json(jsonString, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    string msg = "You can not Cancle Subscription. No record found.it's already Expired..";
                   // TempData["error"] = String.Format("You can not deleted. No record found.");
                    string jsonString = string.Empty;
                    jsonString = JsonConvert.SerializeObject(msg);
                    return Json(jsonString, JsonRequestBehavior.AllowGet);
                   
                }
                return RedirectToAction("CustomerSubscriptionList");
            }
            //catch (System.Data.SqlClient.SqlException ex)
            //{
            //    if (ex.Message.ToLower().Contains("fk_wallet_custsubscription"))
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
        }

        

        [HttpPost]
        public ActionResult GetCustomerSubscriptionDetail(string customerid,string subscriptionid)
        {
           // Subscription objsub = new Subscription();
            Customer objcust = new Customer();

            DataTable dt = new DataTable();
            //dt = objcust.BindsubDateCustomer(Convert.ToInt32(customerid));
            dt = objsub.getCustomerSubscriptionList(Convert.ToInt32(subscriptionid));

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(rows);

            //chcek wallet amount
            decimal Walletbal = 0, TotalCredit = 0, TotalDebit = 0;
            DataTable dtprodRecord = new DataTable();
            dtprodRecord = objsub.getCustomerWallet(Convert.ToInt32(customerid));
            int userRecords = dtprodRecord.Rows.Count;
            if (userRecords > 0)
            {
                if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["Amt"].ToString()))
                    TotalCredit = Convert.ToDecimal(dtprodRecord.Rows[0]["Amt"]);
                if (userRecords > 1)
                {
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[1]["Amt"].ToString()))
                        TotalDebit = Convert.ToDecimal(dtprodRecord.Rows[1]["Amt"]);
                }
                Walletbal = TotalCredit - TotalDebit;
            }

            string jsonString1 = string.Empty;
            jsonString1 = JsonConvert.SerializeObject(Walletbal);

            int RewardPoint = 0;
            DataTable dtreward = new DataTable();
            dtreward = objcust.BindCustomer(Convert.ToInt32(customerid));
            int userRecords1 = dtreward.Rows.Count;

            if (userRecords1 > 0)
            {
                if (!string.IsNullOrEmpty(dtreward.Rows[0]["RewardPoint"].ToString()))
                    RewardPoint = Convert.ToInt32(dtreward.Rows[0]["RewardPoint"]);
            }
            string jsonString2 = string.Empty;
            jsonString2 = JsonConvert.SerializeObject(RewardPoint);


            return Json(new { jsonString, jsonString1, jsonString2 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CanceledSubscriptionList()
        {
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            Subscription sc = new Subscription();
            DataTable cancList = sc.CanceledSubscriptionList();
            ViewBag.cansub = cancList;
            return View();
        }

        [HttpGet]
        public ActionResult CancelSubscription()
         {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                Customer objcust = new Customer();
                DataTable dt = new DataTable();
                dt = objcust.BindCustomer(null);
                ViewBag.Customer = dt;
                
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
                return View();
        }

        [HttpPost]
        public ActionResult CancelSubscription(Subscription smodel,FormCollection form)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                try
                {
                    Customer objcust = new Customer();

                    var cust = form["ddlCustomer"].ToString();
                    if (!string.IsNullOrEmpty(cust))
                    {
                        smodel.CustomerId = Convert.ToInt32(cust);
                    }
                    var fdate = form["cancelFD"].ToString();
                    if (!string.IsNullOrEmpty(fdate))
                    {
                        smodel.cancelFD = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
                    }

                    DataTable cdt = objcust.BindCustomer(smodel.CustomerId);
                    var td = cdt.Rows[0]["SubnToDate"].ToString();
                    DateTime tdt = Convert.ToDateTime(DateTime.Parse(td, CultureInfo.InvariantCulture));
                    var tddd = tdt.ToString("dd/MM/yyyy");
                    smodel.cancelTD = Convert.ToDateTime(DateTime.ParseExact(tddd, @"dd/MM/yyyy", null));

                    //get order details

                    DataTable odetails = smodel.getOrderDetails(smodel);
                    if (odetails.Rows.Count > 0)
                    {
                        //Delete Order Details
                        bool r = smodel.deleteOrderDetails(smodel);

                    }
                    //get Order Transaction
                    DataTable otransac = smodel.getOrderTransaction(smodel);
                    if (otransac.Rows.Count > 0)
                    {
                        //delete order transec
                        bool r = smodel.deleteOrderTransaction(smodel);
                    }
                    //if there is reward amount add to wallet
                    if (smodel.RewardPoint > 0)
                    {
                        smodel.Amount = Convert.ToInt32(smodel.RewardPoint);
                        smodel.Description = "Total Reward Points Credited to Wallet";
                        smodel.Type = "Credit";
                        int resrp = objsub.InsertWallet(smodel);
                       

                    }


                    //get wallet balance
                    decimal Walletbal = 0, TotalCredit = 0, TotalDebit = 0;
                    DataTable dtprodRecord = new DataTable();
                    dtprodRecord = objsub.getCustomerWallet(Convert.ToInt32(smodel.CustomerId));
                    int userRecords = dtprodRecord.Rows.Count;
                    if (userRecords > 0)
                    {
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["Amt"].ToString()))
                            TotalCredit = Convert.ToDecimal(dtprodRecord.Rows[0]["Amt"]);
                        if (userRecords > 1)
                        {
                            if (!string.IsNullOrEmpty(dtprodRecord.Rows[1]["Amt"].ToString()))
                                TotalDebit = Convert.ToDecimal(dtprodRecord.Rows[1]["Amt"]);
                        }
                        Walletbal = TotalCredit - TotalDebit;
                    }

                    smodel.Amount = Walletbal-smodel.RewardPoint;
                    smodel.Description = "Cancel Subscription";
                    smodel.Type = "Debit";
                    //Debit entry in Wallet
                    int res = objsub.InsertWallet(smodel);

                    //debit entry of reward point
                    if(smodel.RewardPoint>0)
                    {
                      smodel.Amount = Convert.ToInt32(smodel.RewardPoint);
                        smodel.Description = "Total Reward Points Debited From Wallet";
                        smodel.Type = "Debit";
                        int resrp = objsub.InsertWallet(smodel);

                    }

                    //get Reward Points
                    
                    //DataTable dtGetCustomerPoint = objcust.BindCustomer(smodel.CustomerId);
                    //smodel.RewardPoint = Convert.ToInt32(dtGetCustomerPoint.Rows[0]["RewardPoint"].ToString());
                    //if (smodel.RewardPoint >= 10)
                    //{
                    //    smodel.Refund = smodel.Amount + Convert.ToDecimal((smodel.RewardPoint / 10));
                    //}
                    //else
                    //{
                    //    smodel.Refund = 0;
                    //}

                    //var ctd = dtGetCustomerPoint.Rows[0]["SubnToDate"].ToString();
                    //DateTime fdt = Convert.ToDateTime(DateTime.Parse(ctd, CultureInfo.InvariantCulture));
                    //var fdtt = fdt.ToString("dd/MM/yyyy");
                    //smodel.cancelTD = Convert.ToDateTime(DateTime.ParseExact(fdtt, @"dd/MM/yyyy", null));

                    
                    smodel.cancelBy = Session["Username"].ToString();
                    smodel.cancelDate = indianTime;

                    //Do Entry in Cancel Subscription Table
                    smodel.walletbalance = Walletbal-smodel.RewardPoint;
                    bool b = smodel.AddCancelledsub(smodel);
                    if (b == true)
                    {
                        //update cust master affter cancelling subscription
                        
                        bool r = smodel.UpdateCustMasterAfterCancelSub(smodel.CustomerId, smodel.cancelFD);
                        if (r == true)
                            ViewBag.SuccessMsg = "Subscription Cancelled Successfully !!!";

                    }
                    ModelState.Clear();

                    DataTable dt = new DataTable();
                    dt = objcust.BindCustomer(null);
                    ViewBag.Customer = dt;

                }
                catch (Exception e)
                {
                    throw;
                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

            
            return View();
        }

        [HttpPost]
        public ActionResult getSingleCustomerSubscriptionList(string cId)
        {
            Customer objcust = new Customer();
            string sublist,rp,td,wb= string.Empty;
            try
            {
                DataTable dtList = new DataTable();
                dtList = objsub.getCustomerwiseSubscriptionList(Convert.ToInt32(cId));
                sublist = JsonConvert.SerializeObject(dtList);

                DataTable dtGetCustomerPoint = objcust.BindCustomer(Convert.ToInt32(cId));
                 var rewaedp= dtGetCustomerPoint.Rows[0]["RewardPoint"].ToString();
                if(Convert.ToInt32(rewaedp)>=10)
                {
                    int rpo = Convert.ToInt32(rewaedp) / 10;
                    rp = JsonConvert.SerializeObject(Convert.ToString(rpo));
                }
                else
                {
                    rp = JsonConvert.SerializeObject("0");
                }
                

                var ctd = dtGetCustomerPoint.Rows[0]["SubnToDate"].ToString();
                DateTime fdt = Convert.ToDateTime(DateTime.Parse(ctd, CultureInfo.InvariantCulture));
                string enddate= fdt.ToString("dd/MM/yyyy");
                td = JsonConvert.SerializeObject(enddate);

                decimal Walletbal = 0, TotalCredit = 0, TotalDebit = 0;
                DataTable dtprodRecord = new DataTable();
                dtprodRecord = objsub.getCustomerWallet(Convert.ToInt32(Convert.ToInt32(cId)));
                int userRecords = dtprodRecord.Rows.Count;
                if (userRecords > 0)
                {
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["Amt"].ToString()))
                        TotalCredit = Convert.ToDecimal(dtprodRecord.Rows[0]["Amt"]);
                    if (userRecords > 1)
                    {
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[1]["Amt"].ToString()))
                            TotalDebit = Convert.ToDecimal(dtprodRecord.Rows[1]["Amt"]);
                    }
                    Walletbal = TotalCredit - TotalDebit;
                }
                wb = JsonConvert.SerializeObject(Walletbal);



            }
            catch(Exception e)
            {
                throw;
            }
            return Json(new { sublist ,rp,td,wb}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteCanceledSubscription(int id)
        {
            try
            {
                bool b = objsub.DelCancelSub(id);
                if(b==true)
                {
                    TempData["error"] = "Data Deleted Successfully !!";
                }
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
                TempData["error"] = e.Message;
            }
            return RedirectToAction("CanceledSubscriptionList");
        }


        public ActionResult CanceledSubPrint()
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Server.MapPath("~/Report/CanceledSubscription.rpt"));
            DataTable dt = objsub.canSubListforRpt();
            rd.Database.Tables[0].SetDataSource(dt);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            try
            {
                Stream str = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                str.Seek(0, SeekOrigin.Begin);
                return File(str, "application/pdf", "CanceledSubscriptions.pdf");

            }
            catch(Exception e)
            {
                throw;
            }

            
        }

        [HttpGet]
        public ActionResult ExpiredSubscriptionList()
        {
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            DataTable dtexplist = objsub.expiredSubList();
            ViewBag.expiredList = dtexplist;
            return View();
        }


        [HttpGet]
        public ActionResult AddSectorSubscription()
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


            Sector objsector = new Sector();
            DataTable dtList = new DataTable();
            dtList = objsector.getStateList(null);
            ViewBag.StateList = dtList;
            Customer objcust = new Customer();


            return View();
        }

        [HttpPost]
        public ActionResult AddSectorSubscription(Subscription obj,FormCollection frm, string[] chkSector)
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
            Sector objsector = new Sector();
            DataTable dtList = new DataTable();
            dtList = objsector.getStateList(null);
            ViewBag.StateList = dtList;
            string active = "true";
            obj.StateCode = Request["ddlState"];
            obj.CityCode = Request["ddlCity"];
            obj.IsActive = Convert.ToBoolean(active);
            int count = 0;
            int Vendorassign = 0;
            if (chkSector != null)
            {
                foreach (var item in chkSector)
                {

                    if (!string.IsNullOrEmpty(item))
                    {

                        string item1 = item.ToString();
                        obj.SectorId =Convert.ToInt32(item1);
                        dtList = objsub.getSectorSubscriptionList(null, obj.SectorId);
                        if (dtList.Rows.Count > 0)
                        {
                            int sid =Convert.ToInt32(dtList.Rows[0]["Id"]);
                            obj.SubscriptionId = sid;
                            //dupcount = dtList.Rows.Count;
                            Vendorassign=obj.UpdateSectorSubscription(obj);

                            if (Vendorassign > 0)
                                count++;
                        }

                        else
                        {
                             Vendorassign = obj.InsertSectorSubscription(obj);
                            if (Vendorassign > 0)
                                count++;
                        }
                        
                        

                    }
                }

                if (count > 0)
                {
                    ViewBag.SuccessMsg = "Susbscription Assigned successfully  " ;
                }
                else
                    ViewBag.SuccessMsg = "Error Occured";
            }

            return View();
        }

        [HttpGet]
        public ActionResult SectorSubscriptionList()
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
                dtList = objsub.getSectorSubscriptionList(null,null);
                ViewBag.SubscriptionList = dtList;
                return View();
           
        }


        [HttpGet]
        public ActionResult DeleteSectorSubscription(int id)
        {
            try
            {
                int delresult = objsub.DeleteSectorSubscription(id);
                return RedirectToAction("SectorSubscriptionList");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_custsubscription_subscription"))
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
            return RedirectToAction("SubscriptionList");
        }


        [HttpGet]
        public ActionResult EditSectorSubscription(int id = 0)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                Customer objcust = new Customer();
                DataTable dtcust = new DataTable();




               DataTable dt = objsub.getSectorSubscriptionList(id,null);
                ViewBag.SubscriptionId = id;
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["statename"].ToString()))
                        ViewBag.statename = dt.Rows[0]["statename"].ToString();
                    else
                        ViewBag.statename = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Cityname"].ToString()))
                        ViewBag.Cityname = dt.Rows[0]["Cityname"].ToString();
                    else
                        ViewBag.Cityname = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["SectorName"].ToString()))
                        ViewBag.SectorName = dt.Rows[0]["SectorName"].ToString();
                    else
                        ViewBag.SectorName = "";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["SubscriptionAmount"].ToString()))
                        ViewBag.SubscriptionAmount = dt.Rows[0]["SubscriptionAmount"].ToString();
                    else
                        ViewBag.SubscriptionAmount = "";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditSectorSubscription(Subscription obj, FormCollection frm)
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
            Sector objsector = new Sector();
            DataTable dtList = new DataTable();
            dtList = objsector.getStateList(null);
            ViewBag.StateList = dtList;
            string active = "true";
            obj.StateCode = Request["ddlState"];
            obj.CityCode = Request["ddlCity"];
            obj.IsActive = Convert.ToBoolean(active);
            obj.SubscriptionId =Convert.ToInt32(Request["txtid"]);
            int count = 0;

            int Vendorassign = obj.UpdateSectorSubscription(obj);

            if (Vendorassign > 0)
                {
                    ViewBag.SuccessMsg = "Susbscription Updated successfully  ";
                }
                else
                    ViewBag.SuccessMsg = "Error Occured";

            DataTable dt = objsub.getSectorSubscriptionList(null,obj.SubscriptionId);
            ViewBag.SubscriptionId = obj.SubscriptionId;
            if (dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dt.Rows[0]["statename"].ToString()))
                    ViewBag.statename = dt.Rows[0]["statename"].ToString();
                else
                    ViewBag.statename = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["Cityname"].ToString()))
                    ViewBag.Cityname = dt.Rows[0]["Cityname"].ToString();
                else
                    ViewBag.Cityname = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["SectorName"].ToString()))
                    ViewBag.SectorName = dt.Rows[0]["SectorName"].ToString();
                else
                    ViewBag.SectorName = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["SubscriptionAmount"].ToString()))
                    ViewBag.SubscriptionAmount = dt.Rows[0]["SubscriptionAmount"].ToString();
                else
                    ViewBag.SubscriptionAmount = "";
            }
            return View();
           
        }


        public JsonResult Status(int ID)
        {
            var userInfo = _subRepo.UpdateSubscriptionStatus(ID);
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
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MilkWayIndia.Models;

namespace MilkWayIndia.Controllers
{
    public class OrderController : Controller
    {
        Helper dHelper = new Helper();
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Schedule()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");
                if (control.IsAdmin == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;
                ViewBag.IsAdmin = true;
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Schedule(FormCollection frm)
        {
            var response = dHelper.ScheduleOrder(Convert.ToDateTime(frm["orderdate"]));
            ViewBag.SuccessMsg = response;
            return View();
        }

        public ActionResult SubscribSchedule()
        {
            DateTime CurrentDate = Helper.indianTime;
            Subscription _subscription = new Subscription();
            Subscription objsub = new Subscription();
            DateTime lastDate = Helper.GetMonthLastDate(CurrentDate);
            if (CurrentDate.Month == lastDate.Month && CurrentDate.Day == lastDate.Day && CurrentDate.Year == lastDate.Year)
            {
                DateTime FromDate = Helper.GetMonthFirstDate(CurrentDate);
                DateTime ToDate = lastDate;
                var customer = _subscription.GetCustomerSubscription(FromDate, ToDate);
                if (customer.Rows.Count > 0)
                {
                    for (int i = 0; i < customer.Rows.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(customer.Rows[i]["CustomerId"].ToString()))
                            _subscription.CustomerId = Convert.ToInt32(customer.Rows[i]["CustomerId"]);
                        decimal Amount = 0,chkAmount=0;
                        if (!string.IsNullOrEmpty(customer.Rows[i]["TotalBag"].ToString()))
                            Amount = Convert.ToDecimal(customer.Rows[i]["TotalBag"].ToString());
                        DataTable dt = objsub.getSectorSubscriptionByCustomer(_subscription.CustomerId);
                        if (dt.Rows.Count > 0)
                            chkAmount = Convert.ToDecimal(dt.Rows[0]["SubscriptionAmount"]);
                        else
                            chkAmount = 0;
                        if (Amount > 0)
                        {
                            //if (Amount >= 100)
                            //    Amount = 100;
                            if (Amount >= chkAmount)
                                Amount = chkAmount;
                            _subscription.TotalBalance = Amount;
                            _subscription.Amount = Amount;
                            _subscription.OrderId = 0;
                            _subscription.BillNo = null;
                            _subscription.Description = string.Format("Subscription Charges From {0} To {1}", FromDate.ToShortDateString(), ToDate.ToShortDateString());
                            _subscription.Type = "Debit";
                            _subscription.CustSubscriptionId = 0;
                            _subscription.TransactionType = Convert.ToInt32(Helper.TransactionType.Subscription);
                            int walletresult = _subscription.InsertWallet(_subscription);
                        }
                    }
                }
            }
            return View();
        }
    }
}
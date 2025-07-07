using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Paytm;
using MilkWayIndia.Models;
using MilkWayIndia.Entity;
using MilkWayIndia.Abstract;
using MilkWayIndia.Concrete;
using Newtonsoft.Json.Linq;

namespace MilkWayIndia.Controllers.Security
{
    public class SecurityController : Controller
    {
        public static string PaytmMerchantID = "NXEpnY32055934299372";
        public static string PaytmMerchantKey = "j_Vnb_oN6XqIPVI5";
        private readonly Random _random = new Random();
        Models.Helper dHelper = new Models.Helper();
        private ISecPaytm _SecPaytmRepo;
        EFDbContext db = new EFDbContext();
        Subscription _subscription = new Subscription();

        public SecurityController()
        {
            this._SecPaytmRepo = new SecPaytmRepository();
        }
        // GET: Security
        public ActionResult Index(string CustomerId, string PlanId)
        {
            try
            {
                var response = dHelper.InitiateSubscription(CustomerId, Convert.ToInt32(PlanId));
                if (response.status == "200")
                    return Redirect("/security/authenticate?token=" + response.token);
                else
                    return Redirect("/security/error");
            }
            catch
            {
                return Redirect("/security/error");
            }
        }

        public ActionResult Error()
        {
            return View();
        }

        // Generates a random number within a range.      
        public int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        public ActionResult Authenticate(string token)
        {
            try
            {
                var detail = _SecPaytmRepo.GetDetailByPaytmToken(token);
                ViewBag.JSScript = string.Format("<script type=\"text/javascript\" crossorigin=\"anonymous\" src=\"{0}/merchantpgpui/checkoutjs/merchants/{1}.js\"></script>", Helper.PaytmAPI, Helper.PaytmMerchantID);
                ViewBag.Token = token;
                ViewBag.PaytmMerchentID = Helper.PaytmMerchantID;
                ViewBag.PaytmAPI = Helper.PaytmAPI;
                ViewBag.OrderNo = detail.OrderNo;

                //ViewBag.Token = token;
                //ViewBag.PaytmMerchentID = Helper.PaytmMerchantID;
                //ViewBag.PaytmAPI = Helper.PaytmAPI;
                //ViewBag.OrderNo = orderid;
            }
            catch (Exception ex)
            {
                return Redirect("/security/error");
            }
            return View();
        }

        public ActionResult PaytmPreNotify()
        {
            try
            {
                var paytmPreNotify = db.tbl_Paytm_Request.Where(s => s.Authenticated == true && s.PreNofifyCall == false).ToList();
                foreach (var item in paytmPreNotify)
                {
                    try
                    {
                        decimal? Amount = 1;
                        var fillAmount = dHelper.GetAutoPayPlan().FirstOrDefault(s => s.id == item.PlanID);
                        if (fillAmount != null)
                        {
                            Amount = fillAmount.deposit;
                            var balance = _subscription.GetCustomerBalace(item.CustomerID.Value);
                            var a = fillAmount.balance.Value;
                            if (balance <= a)
                                dHelper.PaytmPreNotify(item.CustomerID, Amount);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return Redirect("/security/error");
            }
            return View();
        }

        public ActionResult PaytmRequestPayment()
        {
            try
            {
                var currentDate = Helper.indianTime;
                var renewal = db.tbl_Paytm_Request_Details.Where(s => s.RenewalDate.Value.Day == currentDate.Day && s.RenewalDate.Value.Month == currentDate.Month && s.RenewalDate.Value.Year == currentDate.Year && s.IsConfirm == false).ToList();
                foreach (var item in renewal)
                {
                    dHelper.PaytmRequestPayment(item.ID, item.CustomerID, item.Amount);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return Redirect("/security/error");
            }
            return View();
        }

        public void TransactionStatus()
        {
            try
            {
                var currentDate = Helper.indianTime;
                var renewal = db.tbl_Paytm_Request_Details.Where(s => s.RenewalDate.Value.Day == currentDate.Day && s.RenewalDate.Value.Month == currentDate.Month && s.RenewalDate.Value.Year == currentDate.Year && s.IsConfirm == false).ToList();
                foreach (var item in renewal)
                {
                    dHelper.PaytmTransactoinStatus(item.ID, item.RenewalOrderID);
                }
            }
            catch { }
        }

        public void InitiateTransaction(string CustomerId, decimal Amount)
        {
            dHelper.InitiateTransaction(CustomerId, Amount);
        }


    }
}
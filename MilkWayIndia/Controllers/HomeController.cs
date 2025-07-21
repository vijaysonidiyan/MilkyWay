using MilkWayIndia.Abstract;
using MilkWayIndia.Concrete;
using MilkWayIndia.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MilkWayIndia.Entity;
using Newtonsoft.Json;

namespace MilkWayIndia.Controllers
{
    public class HomeController : Controller
    {
        Customer objCustomer = new Customer();
        private ISecPaytm _SecPaytmRepo;
        Helper dHelper = new Helper();

        public HomeController()
        {
            this._SecPaytmRepo = new SecPaytmRepository();
        }      
        // GET: Home
        [HttpGet]
        public ActionResult Login(string ReturnURL)
        {
            if (!string.IsNullOrEmpty(ReturnURL))
                ViewBag.ReturnURL = ReturnURL;
            ClearAllCookies();

            //string responseData = "{\"head\":{\"responseTimestamp\":\"1645963575072\",\"version\":\"v1\",\"signature\":\"QJpUBC3K3nR5oJES2hNpfm/EgpEf9VJcwnRAHJ3Qvf+zp/qQYW6RFN/CuAgSvcCXfEKSgRdqFYG0eNQqKk+FiTnJKXMGfcoV0EPfHFSM5vU=\"},\"body\":{\"resultInfo\":{\"resultStatus\":\"TXN_SUCCESS\",\"resultCode\":\"01\",\"resultMsg\":\"Txn Success\"},\"txnId\":\"20220227111212800110168197911900127\",\"bankTxnId\":\"205891952385\",\"orderId\":\"737631314\",\"txnAmount\":\"1.00\",\"txnType\":\"SALE\",\"gatewayName\":\"PPBS\",\"mid\":\"RhFYmH27735725379698\",\"paymentMode\":\"UPI\",\"refundAmt\":\"0.00\",\"txnDate\":\"2022-02-27 17:32:43.0\",\"subsId\":\"100426534244\"}}";
            //dynamic response = JsonConvert.DeserializeObject(responseData);
            //var status = response.body.resultInfo.resultCode;
            //var txnId= response.body.txnId;
            //var bankTxnId = response.body.bankTxnId;
            //var txnDate = response.body.txnDate;

            //Subscription _subscription = new Subscription();
            //_subscription.OrderId = 0;
            //_subscription.BillNo = "2022022816071902";
            //_subscription.CustomerId = 200;
            //_subscription.TransactionDate = Helper.indianTime;
            //_subscription.Type = "Credit";
            //_subscription.TransactionType = Convert.ToInt32(Helper.TransactionType.Deposit);
            //_subscription.CustSubscriptionId = 0;
            //_subscription.Description = "Add To Wallet TxnID - " + txnId;
            //var r = _subscription.InsertWallet(_subscription);


            return View();
        }
        [HttpPost]
        public ActionResult Login(Staff staff)
        {
            //int result = 0;
            var ReturnURL = Request.Form["ReturnURL"];
            DataTable dtUser = new DataTable();
            if (staff.UserName != null && staff.Password != null)
            {
                DataTable dt = staff.Adminlogin(staff.UserName, staff.Password);
                if (dt.Rows.Count > 0)
                {
                    string a = dt.Rows[0]["Id"].ToString();
                    
                    HttpCookie cookie = new HttpCookie("gstusr");
                    cookie.Values.Add("key", dt.Rows[0]["Id"].ToString());
                    cookie.Expires = DateTime.Now.AddHours(3);
                    Response.Cookies.Add(cookie);

                    // string b = Request.Cookies["gstusr"].Value;
                    Session["Msg"] = "";
                    Session["UserId"] = dt.Rows[0]["Id"].ToString();
                    Session["Username"] = staff.UserName;
                    Session["RoleName"] = dt.Rows[0]["Role"].ToString();
					Session["isVendorLogin"] = false;
					Session["VendorSectorId"] = "0";
					if (!string.IsNullOrEmpty(dt.Rows[0]["Photo"].ToString()))
                        Session["ProfilePic"] = dt.Rows[0]["Photo"].ToString();
                    else
                        Session["ProfilePic"] = "";
                    if (ReturnURL != "")
                        return Redirect(ReturnURL);
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
					DataTable dtVendor = staff.VendorLogin(staff.UserName, staff.Password);
					if (dtVendor.Rows.Count > 0)
					{
						string a = dtVendor.Rows[0]["Id"].ToString();

						HttpCookie cookie = new HttpCookie("gstusr");
						cookie.Values.Add("key", dtVendor.Rows[0]["Id"].ToString());
						cookie.Expires = DateTime.Now.AddHours(3);
						Response.Cookies.Add(cookie);
                        
                        // string b = Request.Cookies["gstusr"].Value;
						Session["Msg"] = "";
						Session["UserId"] = dtVendor.Rows[0]["Id"].ToString();
						Session["Username"] = dtVendor.Rows[0]["FirstName"].ToString() + " " + dtVendor.Rows[0]["LastName"].ToString();
						Session["RoleName"] = "Vendor";
						Session["isVendorLogin"] = true;
						Session["VendorSectorId"] = dtVendor.Rows[0]["SectorId"].ToString();
						if (!string.IsNullOrEmpty(dtVendor.Rows[0]["Photo"].ToString()))
							Session["ProfilePic"] = dtVendor.Rows[0]["Photo"].ToString();
						else
							Session["ProfilePic"] = "";
						if (ReturnURL != "")
							return Redirect(ReturnURL);
						return RedirectToAction("Index", "Admin");
					}
					else
					{
						ViewBag.SuccessMsg = "Incorrect Username or Password.";
						ModelState.Clear();
					}
				}
            }
            else
            {
                ViewBag.SuccessMsg = "Enter Username or Password.";
            }
            return View();
        }

        public void ClearAllCookies()
        {
            string[] myCookies = Request.Cookies.AllKeys;
            foreach (string cookie in myCookies)
            {
                Response.Cookies.Remove(cookie);
                Response.Cookies[cookie].Expires = DateTime.MinValue;
                Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            }
        }

        public ActionResult LogOut()
        {
            ClearAllCookies();
            Session["Username"] = "";
            Session["UserId"] = "";
            Session["ProfilePic"] = "";
            Session["RoleName"] = "";
            Session.Clear();
            Session.Abandon();
            Response.Cookies.Clear();
            Response.Cookies.Remove("gstusr");
            Response.Cookies["gstusr"].Expires = DateTime.MinValue;
            Response.Cookies["gstusr"].Expires = DateTime.Now.AddDays(-30);
            return RedirectToAction("Login", "Home");
        }

        public PartialViewResult GetLeftMenu()
        {
            var control = Helper.CheckPermission("");
            ViewBag.IsAdmin = control.IsAdmin;
            return PartialView("_LeftMenu");
        }

        public ActionResult Callback()
        {
            try
            {
                var status = Request.Form["STATUS"];
                var responseCode = Request.Form["RESPCODE"];
                var orderID = Request.Form["ORDERID"];
                tbl_Paytm_Request model = new tbl_Paytm_Request();
                model.OrderNo = orderID == null ? "0" : orderID;
                if (responseCode == "01")
                {
                    model.Authenticated = true;
                    model.TransactionID = Request.Form["TXNID"];
                    ViewBag.Status = "Confirm";
                }
                model.UpdatedDate = Helper.indianTime;
                var paytm = _SecPaytmRepo.UpdatePaytmResponseByOrderID(model);
                if (paytm != null)
                {
                    var customer = objCustomer.BindCustomer(paytm.CustomerID);
                    if (customer.Rows.Count > 0)
                    {
                        string cName = "";
                        if (!string.IsNullOrEmpty(customer.Rows[0]["FirstName"].ToString()))
                            cName = customer.Rows[0]["FirstName"].ToString();
                        if (!string.IsNullOrEmpty(customer.Rows[0]["LastName"].ToString()))
                            cName = cName + " " + customer.Rows[0]["LastName"].ToString();

                        ViewBag.CustomerName = cName;
                        if (!string.IsNullOrEmpty(customer.Rows[0]["MobileNo"].ToString()))
                            ViewBag.MobileNo = customer.Rows[0]["MobileNo"].ToString();
                    }
                }
                ViewBag.OrderNo = orderID;
                if (responseCode == "01")
                    return Redirect("/home/success");
            }
            catch { }
            return Redirect("/home/failed");
        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult Failed()
        {
            return View();
        }
    }
}
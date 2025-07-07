using MilkWayIndia.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MilkWayIndia.Controllers
{
    public class CashBackController : Controller
    {

        CashBack objcashback = new CashBack();
        Subscription objsub = new Subscription();
        // DataRow dr = dtNew.NewRow();
        // GET: CashBack
        public ActionResult Index()
        {
            return View();
        }



        [HttpGet]
        public ActionResult CashBackSettings()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                DataTable dtList = new DataTable();
                dtList = objcashback.getCashbackList(null);
                ViewBag.CashBackList = dtList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpGet]
        public ActionResult AddCashBack()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                DataTable dt = new DataTable();
                dt = objcashback.getServiceList(null);
                ViewBag.Service = dt;
                string service = "Mobile";
                dt = objcashback.getproviderList(service);
                ViewBag.Provider = dt;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddCashBack(CashBack objcashback, FormCollection form)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                int addresult = 0;
                var submit = Request["submit"];
                if (submit == "Save")
                {
                    string service = Request["ddlservice"];

                    ViewBag.Servicename = service;
                    if (!string.IsNullOrEmpty(service))
                    {
                        objcashback.Service = service.ToString();
                    }

                    objcashback.OperatorCode = Convert.ToInt32(Request["ddlprovider"]);
                    
                    objcashback.Type = Request["ddltype"];

                    //  DataTable dtDuplicateSec = objsector.getCheckDuplBuilding(objsector.SectorId, objsector.BuildingName, objsector.BlockNo);
                    // if (dtDuplicateSec.Rows.Count > 0)
                    // {
                    // ViewBag.SuccessMsg = "Building Name Already Exits!!!";
                    // }
                    // else
                    //{
                    addresult = objcashback.InsertCashbackSetting(objcashback);
                    if (addresult > 0)
                    {
                        ViewBag.SuccessMsg = "CashBack Settings Inserted Successfully!!!";
                    }
                    else
                    { ViewBag.SuccessMsg = "CashBack Settings Not Inserted!!!"; }
                    // }
                    ModelState.Clear();
                    DataTable dt = new DataTable();
                    dt = objcashback.getServiceList(null);
                    ViewBag.Service = dt;
                    string service1 = "Mobile";
                    dt = objcashback.getproviderList(service1);
                    ViewBag.Provider = dt;

                    ViewBag.Providername = Request["ddlprovider"];
                    return View();
                }
                else
                {
                    string service = Request["ddlservice"];
                    ViewBag.Servicename = service;
                    DataTable dt = new DataTable();
                    dt = objcashback.getproviderList(service);
                    ViewBag.Provider = dt;
                    dt = objcashback.getServiceList(null);
                    ViewBag.Service = dt;
                    return View();

                }
            }

            else
            {
                return RedirectToAction("Login", "Home");
            }

            
        }


        [HttpGet]
        public ActionResult EditCashBackSet(int id=0)
        {
            //if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            //{
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;


            DataTable dt = new DataTable();
            dt = objcashback.getCashbackList(id);
            if (dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dt.Rows[0]["Amount1"].ToString()))
                    ViewBag.Amount = dt.Rows[0]["Amount1"].ToString();
                else
                    ViewBag.Amount = "0";

                if (!string.IsNullOrEmpty(dt.Rows[0]["Service"].ToString()))
                    ViewBag.Servicename = dt.Rows[0]["Service"].ToString();
                else
                    ViewBag.Servicename = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["Type"].ToString()))
                    ViewBag.Type = dt.Rows[0]["Type"].ToString();
                else
                    ViewBag.Type = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["OperatorId"].ToString()))
                    ViewBag.Providername = dt.Rows[0]["OperatorId"].ToString();
                else
                    ViewBag.Providername = "";


                if (!string.IsNullOrEmpty(dt.Rows[0]["ProviderName"].ToString()))
                    ViewBag.ProviderName1 = dt.Rows[0]["ProviderName"].ToString();
                else
                    ViewBag.ProviderName1 = "";
            }

                dt = new DataTable();
                dt = objcashback.getServiceList(null);
                ViewBag.Service = dt;
                string service = ViewBag.Servicename;
                dt = objcashback.getproviderList(service);
                ViewBag.Provider = dt;

                return View();
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Home");
            //}
        }


        [HttpPost]
        public ActionResult EditCashBackSet(CashBack objcashback, FormCollection form)
        {
            //if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            //{
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            int addresult = 0;
                var submit = Request["submit"];
                if (submit == "Save")
                {
                    string service = Request["ddlservice"];

                    ViewBag.Servicename = service;
                    if (!string.IsNullOrEmpty(service))
                    {
                        objcashback.Service = service.ToString();
                    }

                    objcashback.OperatorCode = Convert.ToInt32(Request["ddlprovider"]);

                    objcashback.Type = Request["ddltype"];

                   
                    addresult = objcashback.UpdateCashbackSetting(objcashback);
                    if (addresult > 0)
                    {
                        ViewBag.SuccessMsg = "CashBack Settings Updated Successfully!!!";
                    }
                    else
                    { ViewBag.SuccessMsg = "CashBack Settings Not Updated!!!"; }
                    // }
                    ModelState.Clear();
                    DataTable dt = new DataTable();
                    dt = objcashback.getServiceList(null);
                    ViewBag.Service = dt;
                string service1 = ViewBag.Servicename;
                dt = objcashback.getproviderList(service1);
                    ViewBag.Provider = dt;

                    ViewBag.Providername = Request["ddlprovider"];
                    return View();
                }
                else
                {
                    string service = Request["ddlservice"];
                    ViewBag.Servicename = service;
                    DataTable dt = new DataTable();
                    dt = objcashback.getproviderList(service);
                    ViewBag.Provider = dt;
                    dt = objcashback.getServiceList(null);
                    ViewBag.Service = dt;
                    return View();

                }
            //}

            //else
            //{
            //    return RedirectToAction("Login", "Home");
            //}


        }
        [HttpGet]
        public ActionResult DeleteCashBackSet(int id)
        {
            try
            {
                // int delresult = 0;
                int delresult = objcashback.DeleteCashbackSetting(id);
                return RedirectToAction("CashBackSettings");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_staff_staffcustassign"))
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
            return RedirectToAction("CashBackSettings");
        }


        [HttpGet]
        public ActionResult CustomerCashBackListflipamz()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                DataTable dtList = new DataTable();
                dtList = objcashback.getCashbackflipamzList(null);
                ViewBag.CashBackList = dtList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }



        [HttpPost]
        public ActionResult CustomerCashBackListflipamz(CashBack objcashback, FormCollection form)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                int addresult = 0;
                // string b = id.ToString();

                //  int id=
               var submit = Request["submit"];
                var customerid = Request["CustomerId"];
                objsub.CustomerId = Convert.ToInt32(customerid);
                
                //  int id=
                var orderfrom = Request[submit + "OrderFrom"];
                var orderid1= Request[submit + "OrderId"];
                var abc = Request["Amount"];
                int id = Convert.ToInt32(submit);

                DateTime cdate = DateTime.Now;
                // objcashback.Amount = Convert.ToDecimal(Request[b].ToString());

                string a = objcashback.Amount.ToString();

                objcashback.Amount = Convert.ToDecimal(abc);
                addresult = objcashback.AddCashBackAmount(objcashback, id, cdate);
                if (addresult > 0)
                {

                    if (!string.IsNullOrEmpty(cdate.ToString()))
                    {
                        objsub.TransactionDate = cdate;
                    }

                    objsub.BillNo = null;
                    objsub.Type = "Credit";
                    objsub.CustSubscriptionId = 0;
                    objsub.TransactionType = Convert.ToInt32(Helper.TransactionType.Cashback);
                    objsub.Description = "Cashback For " +orderfrom +"Order Id:"+orderid1;
                    string orderid = "0";
                    if (!string.IsNullOrEmpty(orderid))
                    {
                        objsub.OrderId = Convert.ToInt32(orderid);
                    }
                    objsub.Amount = objcashback.Amount;
                    objsub.Status = "Cashback";

                    objsub.Cashbacktype = orderfrom.ToString();
                    objsub.CashbackId = orderid1.ToString();
                    int addwallet = objsub.InsertWallet1(objsub);

                    if (addwallet > 0)
                    {
                        ViewBag.SuccessMsgcashback = "CashBack Added Successfully!!!";
                        return RedirectToAction("CustomerCashBackListflipamz");
                    }


                }
                else
                {
                    ViewBag.SuccessMsgcashback = "CashBack Not Added!!!";
                }


            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

            return RedirectToAction("CustomerCashBackListflipamz");
        }

        [HttpGet]
        public ActionResult BillPayCashback()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;


                CashBack objService = new CashBack();
                DataTable dtService = new DataTable();
                dtService = objService.getService(null);
                ViewBag.Service = dtService;


                //string Service = Request["ddlservice"];
                //if (!string.IsNullOrEmpty(Service) && Convert.ToInt32(Service) != 0)
                //{
                //    objcashback.Service = Service;
                //}

                DataTable dtCashbackbillList = new DataTable();
                dtCashbackbillList = objcashback.getCashbackBillList(null);
                ViewBag.CashbackBillList = dtCashbackbillList;
                //DataTable dtList = new DataTable();
                //dtList = objcashback.getCashbackflipamzList(null);
                //ViewBag.CashBackList = dtList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpPost]
        public ActionResult BillPayCashback(FormCollection form, CashBack objcashback)
        {
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var submit = Request["submit"];

            if (submit == "search")
            {
                string Service = Request["ddlservice"];
                if (!string.IsNullOrEmpty(Service) && Service != "---Select Service---")
                {
                    objcashback.Service = Service;
                }
                ViewBag.Servicename = Service;

                DataTable dtCashbackbillList = new DataTable();
                dtCashbackbillList = objcashback.getCashbackBillList(objcashback.Service);
                ViewBag.CashbackBillList = dtCashbackbillList;


                CashBack objService = new CashBack();
                DataTable dtService = new DataTable();
                dtService = objService.getService(null);
                ViewBag.Service = dtService;
            }
            else
            {

                int addresult = 0;
                // string b = id.ToString();
                submit = Request["submit"];
                //  int id=
                var customerid = Request[submit+"CustomerId"];
                objsub.CustomerId = Convert.ToInt32(customerid);

                 var rtype= Request[submit + "RechargeType"];
                var abc = Request[submit + "Amount"];

                var tid= Request[submit + "transactionid"];
                var rno= Request[submit + "rno"];

                int id = Convert.ToInt32(submit);
                objcashback.Service = rtype.ToString();
                DateTime cdate = DateTime.Now;
                // objcashback.Amount = Convert.ToDecimal(Request[b].ToString());

                string a = objcashback.Amount.ToString();

                objcashback.Amount = Convert.ToDecimal(abc);
                addresult = objcashback.AddCashBackAmount1(objcashback, id, cdate);



                if (addresult > 0)
                {

                    if (!string.IsNullOrEmpty(cdate.ToString()))
                    {
                        objsub.TransactionDate = cdate;
                    }

                    objsub.BillNo = null;
                    objsub.Type = "Credit";
                    objsub.CustSubscriptionId = 0;
                    objsub.TransactionType = Convert.ToInt32(Helper.TransactionType.Deposit);
                    objsub.Description = "Cashback BillPay- "+rtype +",Transaction Id:"+tid +",Rechargeno:"+rno;
                    string orderid = "0";
                    if (!string.IsNullOrEmpty(orderid))
                    {
                        objsub.OrderId = Convert.ToInt32(orderid);
                    }
                    objsub.Amount = objcashback.Amount;

                    objsub.Status = "Cashback";

                    objsub.Cashbacktype = rtype.ToString();
                    objsub.CashbackId = tid.ToString();

                    int addwallet = objsub.InsertWallet1(objsub);

                    if (addwallet > 0)
                    {
                        ViewBag.SuccessMsgcashback = "CashBack Added Successfully!!!";
                       // return RedirectToAction("CustomerCashBackListflipamz");
                    }


                }
                else
                {
                    ViewBag.SuccessMsgcashback = "CashBack Not Added!!!";
                }




                string Service = rtype.ToString();
                if (!string.IsNullOrEmpty(Service) && Service != "0")
                {
                    objcashback.Service = Service;
                }

                if(Service== "0")
                {
                    Service = "Mobile";
                    objcashback.Service = Service;
                }

                ViewBag.Servicename = Service;

                DataTable dtCashbackbillList = new DataTable();
                dtCashbackbillList = objcashback.getCashbackBillList(objcashback.Service);
                ViewBag.CashbackBillList = dtCashbackbillList;


                CashBack objService = new CashBack();
                DataTable dtService = new DataTable();
                dtService = objService.getService(null);
                ViewBag.Service = dtService;
            }



            return View();
        }




        [HttpGet]
        public ActionResult CashBackreport()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;


                CashBack objService = new CashBack();
                DataTable dtService = new DataTable();
                dtService = objService.getService(null);
                ViewBag.Service = dtService;


                //string Service = Request["ddlservice"];
                //if (!string.IsNullOrEmpty(Service) && Convert.ToInt32(Service) != 0)
                //{
                //    objcashback.Service = Service;
                //}

                DataTable dtCashbackbillList = new DataTable();
                dtCashbackbillList = objcashback.getCashbackListReport(null,null,null);
                ViewBag.CashbackBillList = dtCashbackbillList;




                //DataTable dtList = new DataTable();
                //dtList = objcashback.getCashbackflipamzList(null);
                //ViewBag.CashBackList = dtList;




                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }




        [HttpPost]
        public ActionResult CashBackreport(FormCollection form, CashBack objcashback)
        {


            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;


                DateTime FDate; DateTime TDate;
                string Service = Request["ddlservice"];
                if (!string.IsNullOrEmpty(Service) && Service != "---Select Service---")
                {
                    objcashback.Service = Service;
                }
                ViewBag.Servicename = Service;



                FDate = DateTime.Today;
                TDate = DateTime.Today;
                var fdate = Request["datepicker"];
                if (!string.IsNullOrEmpty(fdate.ToString()))
                {
                    FDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
                }
                var tdate = Request["datepicker1"];
                if (!string.IsNullOrEmpty(tdate.ToString()))
                {
                    TDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
                }


                ViewBag.FromDate = fdate;
                ViewBag.ToDate = tdate;


                DataTable dtCashbackbillList = new DataTable();
                dtCashbackbillList = objcashback.getCashbackListReport(objcashback.Service, FDate, TDate);
                ViewBag.CashbackBillList = dtCashbackbillList;


                CashBack objService = new CashBack();
                DataTable dtService = new DataTable();
                dtService = objService.getService(null);
                ViewBag.Service = dtService;
                return View();
            }



             else
            {
                return RedirectToAction("Login", "Home");
            }



            }







        }
}
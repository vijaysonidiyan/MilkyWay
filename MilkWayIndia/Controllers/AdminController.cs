using MilkWayIndia.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Helpers;

namespace MilkWayIndia.Controllers
{
    public class AdminController : Controller
    {
        Helper dHelper = new Helper();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        Customer objcust = new Customer();
        Subscription objsub = new Subscription();
        
        [HttpGet]
        public ActionResult Index()
        {
            Staff objstaff = new Staff();
            Subscription objsub = new Subscription();
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                try
                {
                    //string role = Session["RoleName"].ToString();
                    //string a = Session["Username"].ToString();
                    var control = Helper.CheckPermission(Request.RawUrl.ToString());
                    ViewBag.IsAdmin = control.IsAdmin;

                    Customer objcust = new Customer();

                    int TotalCustomer = 0, ActiveCustomer = 0;decimal TodayOrder = 0, YesterdayOrder = 0, RecentRecharge = 0, RecentBillPay = 0;
                    int RecentRecharges = 0, RecentRechargep = 0, RecentRechargef = 0, RecentBillPays = 0, RecentBillPayp = 0, RecentBillPayf = 0, TodayPendingOrder = 0;
                    var dashboard = objcust.getDashboard();
                    TotalCustomer =Convert.ToInt32(dashboard.TotalCustomer);
                    ActiveCustomer = Convert.ToInt32(dashboard.ActiveCustomer);
                    TodayOrder = Convert.ToDecimal(dashboard.TodayOrder);
                    YesterdayOrder = Convert.ToDecimal(dashboard.YesterdayOrder);
                    TodayPendingOrder = Convert.ToInt32(dashboard.TodayPendingOrder);

                    RecentRecharge = Convert.ToInt32(dashboard.Recentrecharge);
                    RecentRecharges = Convert.ToInt32(dashboard.Recentrecharges);
                    RecentRechargep = Convert.ToInt32(dashboard.Recentrechargep);
                    RecentRechargef = Convert.ToInt32(dashboard.Recentrechargef);



                    RecentBillPay = Convert.ToInt32(dashboard.RecentBillPay);
                    RecentBillPays = Convert.ToInt32(dashboard.RecentBillPays);
                    RecentBillPayp = Convert.ToInt32(dashboard.RecentBillPayp);
                    RecentBillPayf = Convert.ToInt32(dashboard.RecentBillPayf);

                    ViewBag.TotalCustomer = TotalCustomer;
                    ViewBag.ActiveCustomer = ActiveCustomer;
                    ViewBag.TodayOrder = TodayOrder;
                    ViewBag.YesterdayOrder = YesterdayOrder;
                    ViewBag.TodayPendingOrder = TodayPendingOrder;

                    ViewBag.RecentRecharge = RecentRecharge;
                    ViewBag.RecentRecharges = RecentRecharges;
                    ViewBag.RecentRechargep = RecentRechargep;
                    ViewBag.RecentRechargef = RecentRechargef;


                    ViewBag.RecentBillPay = RecentBillPay;
                    ViewBag.RecentBillPays = RecentBillPays;
                    ViewBag.RecentBillPayp = RecentBillPayp;
                    ViewBag.RecentBillPayf = RecentBillPayf;

                    CustomerOrder objorder = new CustomerOrder();
                    Sector objsector = new Sector();

                    // Notification
                    CustomerOrder objorder1 = new CustomerOrder();
                    DataTable dtList11 = new DataTable();
                    dtList11 = objorder1.getSectorVendorOrderStatusnotification(null, null, System.DateTime.Now.Date.AddDays(1), System.DateTime.Now.Date.AddDays(10), null);
                    ViewBag.ProductorderListnot = dtList11;
                    ViewBag.ProductorderListcount = dtList11.Rows.Count;

                    //
                    DataTable dt = new DataTable();
                    dt = objsector.getSectorList(null);
                    ViewBag.Sector = dt;

                    Sector objsec = new Sector();
                    DataTable dt1 = new DataTable();
                    dt1 = objsec.getBuildingList(null);
                    ViewBag.Building = dt1;

                    DataTable dtList = new DataTable();
                    dtList = objorder.getSectorSubscriptiondaysDash(null, null, null);
                    ViewBag.ProductorderList = dtList;

                    DataTable dtList1 = new DataTable();
                    dtList1 = objorder.getCustomerWalletBalDash(null, null);
                    ViewBag.WalletList = dtList1;

                    DataTable dtList2 = new DataTable();
                    dtList2 = objorder.getCustomerWalletTotalDebit(null, null);
                    ViewBag.WalletList1 = dtList2;

                    //DataTable dttemp = new DataTable();
                    //dttemp.Columns.Add("Type", typeof(string));
                    //dttemp.Columns.Add("Customer", typeof(string));
                    //dttemp.Columns.Add("BuildingName", typeof(string));
                    //dttemp.Columns.Add("BlockNo", typeof(string));
                    //dttemp.Columns.Add("CustomerId", typeof(int));
                    //dttemp.Columns.Add("Amt", typeof(decimal));
                    //dttemp.Columns.Add("DrAmt", typeof(decimal));
                    //dttemp.PrimaryKey = new DataColumn[] { dttemp.Columns["CustomerId"] };

                    //DataTable dttemp1 = new DataTable();
                    //dttemp1.Columns.Add("Type", typeof(string));
                    //dttemp1.Columns.Add("Customer", typeof(string));
                    //dttemp1.Columns.Add("BuildingName", typeof(string));
                    //dttemp1.Columns.Add("BlockNo", typeof(string));
                    //dttemp1.Columns.Add("CustomerId", typeof(int));
                    //dttemp1.Columns.Add("Amt", typeof(decimal));
                    //dttemp1.Columns.Add("DrAmt", typeof(decimal));
                    //dttemp1.Columns.Add("FinalAmt", typeof(decimal));
                    //dttemp1.PrimaryKey = new DataColumn[] { dttemp1.Columns["CustomerId"] };

                    //string searchExpression = "CustomerId > 0";
                    //DataRow[] foundRows1 = dtList1.Select(searchExpression);
                    //foreach (DataRow dr1 in foundRows1)
                    //{
                    //    //check customer
                    //    string customer = "Type='Credit' and CustomerId = " + dr1["CustomerId"].ToString();

                    //    string x = dr1["CustomerId"].ToString().Trim().ToUpper();
                    //    string y = dr1["CustomerId"].ToString();
                    //    if (dr1["CustomerId"].ToString().Trim().ToUpper().Contains(dr1["CustomerId"].ToString()))
                    //    {
                    //        if (dttemp.Rows.Count > 0)
                    //        {
                    //            if (dttemp.Rows.Contains(Convert.ToInt32(dr1["CustomerId"])))
                    //            {
                    //            }
                    //            else
                    //            {
                    //                dttemp.ImportRow(dr1);
                    //            }
                    //        }
                    //        else
                    //            dttemp.ImportRow(dr1);
                    //    }
                    //}
                    //string searchExpression1 = "CustomerId > 0 and Type='Debit'";
                    //DataRow[] foundRows2 = dtList2.Select(searchExpression1);
                    //dtList2.Columns.Remove("Type");
                    ////dtList2.Columns.Remove("CustomerId");
                    //dtList2.Columns.Remove("BuildingName");
                    //dtList2.Columns.Remove("BlockNo");
                    //dtList2.Columns.Remove("Customer");

                    //foreach (DataRow dr1 in foundRows2)
                    //{
                    //    //check customer
                    //    for (int i = 0; i < dtList2.Rows.Count; i++)
                    //    {
                    //        if (Convert.ToInt32(dttemp.Rows[i]["CustomerId"]) == Convert.ToInt32(dtList2.Rows[i]["CustomerId"]))
                    //            dttemp.Rows[i]["DrAmt"] = dtList2.Rows[i]["Amt"].ToString();
                    //        else
                    //            dttemp.Rows[i]["DrAmt"] = 0;
                    //    }
                    //}
                    //dttemp.Columns.Add("FinalAmt");
                    //for (int i = 0; i < dttemp.Rows.Count; i++)
                    //{
                    //    if (!string.IsNullOrEmpty(dttemp.Rows[i]["DrAmt"].ToString()))
                    //        dttemp.Rows[i]["FinalAmt"] = Convert.ToDecimal(dttemp.Rows[i]["Amt"]) - Convert.ToDecimal(dttemp.Rows[i]["DrAmt"]);
                    //    else
                    //        dttemp.Rows[i]["FinalAmt"] = Convert.ToDecimal(dttemp.Rows[i]["Amt"]) - 0;
                    //}
                    //string searchExpression3 = "CustomerId > 0";
                    //DataRow[] foundRows3 = dttemp.Select(searchExpression3);
                    //foreach (DataRow dr1 in foundRows3)
                    //{
                    //    if (Convert.ToDecimal(dr1["FinalAmt"]) < 200)
                    //        dttemp1.ImportRow(dr1);
                    //}

                    //ViewBag.WalletList = dttemp1;
                    //TempData["WalletList"] = ViewBag.WalletList;

                    //Low balance v
                    //chcek wallet amount
                    decimal TotalCredit = 0, TotalDebit = 0;
                    DataTable dttemp = new DataTable();
                    dttemp.Columns.Add("Type", typeof(string));
                    dttemp.Columns.Add("Customer", typeof(string));
                    dttemp.Columns.Add("Sector", typeof(string));
                    dttemp.Columns.Add("BuildingName", typeof(string));
                    dttemp.Columns.Add("BlockNo", typeof(string));
                    dttemp.Columns.Add("flatno", typeof(string));
                    dttemp.Columns.Add("CustomerId", typeof(int));
                    dttemp.Columns.Add("Amt", typeof(decimal));
                    dttemp.Columns.Add("DrAmt", typeof(decimal));
                    dttemp.Columns.Add("FinalAmt", typeof(decimal));
                    dttemp.Columns.Add("Mobile", typeof(string));
                    dttemp.PrimaryKey = new DataColumn[] { dttemp.Columns["CustomerId"] };

                    DataTable dttemp1 = new DataTable();
                    dttemp1.Columns.Add("Type", typeof(string));
                    dttemp1.Columns.Add("Customer", typeof(string));
                    dttemp1.Columns.Add("Sector", typeof(string));
                    dttemp1.Columns.Add("BuildingName", typeof(string));
                    dttemp1.Columns.Add("BlockNo", typeof(string));
                    dttemp1.Columns.Add("flatno", typeof(string));
                    dttemp1.Columns.Add("CustomerId", typeof(int));
                    dttemp1.Columns.Add("Amt", typeof(decimal));
                    dttemp1.Columns.Add("DrAmt", typeof(decimal));
                    dttemp1.Columns.Add("FinalAmt", typeof(decimal));
                    dttemp1.Columns.Add("Mobile", typeof(string));
                    dttemp1.PrimaryKey = new DataColumn[] { dttemp1.Columns["CustomerId"] };

                    string searchExpression = "CustomerId > 0";
                    DataRow[] foundRows1 = dtList1.Select(searchExpression);
                    foreach (DataRow dr1 in foundRows1)
                    {
                        //check customer
                        string customer = "Type='Credit' and CustomerId = " + dr1["CustomerId"].ToString();

                        string x = dr1["CustomerId"].ToString().Trim().ToUpper();
                        string y = dr1["CustomerId"].ToString();
                        if (dr1["CustomerId"].ToString().Trim().ToUpper().Contains(dr1["CustomerId"].ToString()))
                        {
                            if (dttemp.Rows.Count > 0)
                            {
                                if (dttemp.Rows.Contains(Convert.ToInt32(dr1["CustomerId"])))
                                {
                                }
                                else
                                {
                                    dttemp.ImportRow(dr1);
                                }
                            }
                            else
                                dttemp.ImportRow(dr1);
                        }
                    }
                    DataTable dtprodRecord = new DataTable();
                    DataRow[] dtwall = dttemp.Select();
                    //foreach(DataRow fr in dtwall)
                    //{
                    for (int i = 0; i < dttemp.Rows.Count; i++)
                    {
                        var balance = objsub.GetCustomerBalace(Convert.ToInt32(dttemp.Rows[i]["CustomerId"]));
                        dttemp.Rows[i]["FinalAmt"] = balance;
                        //dtprodRecord = objsub.getCustomerWallet(Convert.ToInt32(dttemp.Rows[i]["CustomerId"]));
                        //int userRecords = dtprodRecord.Rows.Count;
                        //if (userRecords > 0)
                        //{
                        //    if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["Amt"].ToString()))
                        //        TotalCredit = Convert.ToDecimal(dtprodRecord.Rows[0]["Amt"]);
                        //    if (userRecords > 1)
                        //    {
                        //        if (!string.IsNullOrEmpty(dtprodRecord.Rows[1]["Amt"].ToString()))
                        //            TotalDebit = Convert.ToDecimal(dtprodRecord.Rows[1]["Amt"]);
                        //        else
                        //            TotalDebit = 0;
                        //    }
                        //    else
                        //    {
                        //        TotalDebit = 0;
                        //    }

                        //    dttemp.Rows[i]["FinalAmt"] = TotalCredit - TotalDebit;
                        //}
                    }
                    //}

                    string searchExpression3 = "CustomerId > 0";
                    DataRow[] foundRows3 = dttemp.Select(searchExpression3);
                    foreach (DataRow dr1 in foundRows3)
                    {
                        if (Convert.ToDecimal(dr1["FinalAmt"]) < 200)
                            dttemp1.ImportRow(dr1);
                    }

                    ViewBag.WalletList = dttemp1;
                    TempData["WalletList"] = ViewBag.WalletList;


                    //DeliveryBoy Status v
                    var userID = Helper.CurrentLoginUser();
                    Staff objStaff = new Staff();
                    DataTable dtStaff = new DataTable();

                    DataTable del = new DataTable();
                    if (control.IsAdmin == true)
                    {
                        dtStaff = objStaff.getDeliveryBoyList(null);
                        del = objorder.getDeliveryBoyWiseOrder(null, null, System.DateTime.Now.Date.AddDays(1), System.DateTime.Now.Date.AddDays(1), null);
                        ViewBag.Staff = dtStaff;
                    }
                    else
                    {
                        dtStaff = objStaff.getDeliveryBoyList(Convert.ToInt32(userID));
                        del = objorder.getDeliveryBoyWiseOrder(Convert.ToInt32(userID), null, System.DateTime.Now.Date.AddDays(1), System.DateTime.Now.Date.AddDays(1), null);
                        ViewBag.Staff = dtStaff;
                    }
                    
                    ViewBag.PorderList = del;
                    TempData["PorderList"] = ViewBag.PorderList;

                    //Vendor v
                    DataTable dtsec = new DataTable();
                    dtsec = objsector.getSectorList(null);
                    ViewBag.Sector = dtsec;

                    Vendor objvendor = new Vendor();
                    DataTable dtven = new DataTable();
                    dtven = objvendor.getVendorList(null);
                    ViewBag.Vendor = dtven;

                    DataTable dtvp = new DataTable();
                    dtvp = objorder.getSectorVendorOrderStatus(null, null, System.DateTime.Now.Date.AddDays(1), System.DateTime.Now.Date.AddDays(1), null);
                    ViewBag.vendorpList = dtvp;
                    TempData["vendorpList"] = ViewBag.vendorpList;



                    return View();
                }
                catch(Exception e)
                {
                    throw;
                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            Staff objstaff = new Staff();
            Subscription objsub = new Subscription();
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                Customer objcust = new Customer();
                CustomerOrder objorder = new CustomerOrder();
                Sector objsector = new Sector();
                string SectorId = Request["ddlSector"];
                if (!string.IsNullOrEmpty(SectorId) && Convert.ToInt32(SectorId) != 0)
                {
                    objorder.SectorId = Convert.ToInt32(SectorId);
                }
                string BuildingId = Request["ddlBuilding"];
                if (!string.IsNullOrEmpty(BuildingId) && Convert.ToInt32(BuildingId) != 0)
                {
                    objorder.BuildingId = Convert.ToInt32(BuildingId);
                }

                string DaysId = Request["ddlDays"];
                if (!string.IsNullOrEmpty(DaysId) && Convert.ToInt32(DaysId) != 0)
                {
                    objorder.DaysId = Convert.ToInt32(DaysId);
                }

                DataTable dtList = new DataTable();
                dtList = objorder.getSectorSubscriptiondaysDash(objorder.SectorId, objorder.BuildingId, objorder.DaysId);
                ViewBag.ProductorderList = dtList;

                //Fill All Grids v


                Sector objsec = new Sector();
                DataTable dt = new DataTable();
                dt = objsec.getSectorList(null);
                ViewBag.Sector = dt;


                DataTable dt1 = new DataTable();
                dt1 = objsec.getBuildingList(null);
                ViewBag.Building = dt1;


                //Wallet

                DataTable dtList1 = new DataTable();
                dtList1 = objorder.getCustomerWalletBalDash(null, null);
                ViewBag.WalletList = dtList1;

                DataTable dtList2 = new DataTable();
                dtList2 = objorder.getCustomerWalletTotalDebit(null, null);
                ViewBag.WalletList1 = dtList2;

                //DataTable dttemp = new DataTable();
                //dttemp.Columns.Add("Type", typeof(string));
                //dttemp.Columns.Add("Customer", typeof(string));
                //dttemp.Columns.Add("BuildingName", typeof(string));
                //dttemp.Columns.Add("BlockNo", typeof(string));
                //dttemp.Columns.Add("CustomerId", typeof(int));
                //dttemp.Columns.Add("Amt", typeof(decimal));
                //dttemp.Columns.Add("DrAmt", typeof(decimal));
                //dttemp.PrimaryKey = new DataColumn[] { dttemp.Columns["CustomerId"] };

                //DataTable dttemp1 = new DataTable();
                //dttemp1.Columns.Add("Type", typeof(string));
                //dttemp1.Columns.Add("Customer", typeof(string));
                //dttemp1.Columns.Add("BuildingName", typeof(string));
                //dttemp1.Columns.Add("BlockNo", typeof(string));
                //dttemp1.Columns.Add("CustomerId", typeof(int));
                //dttemp1.Columns.Add("Amt", typeof(decimal));
                //dttemp1.Columns.Add("DrAmt", typeof(decimal));
                //dttemp1.Columns.Add("FinalAmt", typeof(decimal));
                //dttemp1.PrimaryKey = new DataColumn[] { dttemp1.Columns["CustomerId"] };

                //string searchExpression = "CustomerId > 0";
                //DataRow[] foundRows1 = dtList1.Select(searchExpression);
                //foreach (DataRow dr1 in foundRows1)
                //{
                //    //check customer
                //    string customer = "Type='Credit' and CustomerId = " + dr1["CustomerId"].ToString();

                //    string x = dr1["CustomerId"].ToString().Trim().ToUpper();
                //    string y = dr1["CustomerId"].ToString();
                //    if (dr1["CustomerId"].ToString().Trim().ToUpper().Contains(dr1["CustomerId"].ToString()))
                //    {
                //        if (dttemp.Rows.Count > 0)
                //        {
                //            if (dttemp.Rows.Contains(Convert.ToInt32(dr1["CustomerId"])))
                //            {
                //            }
                //            else
                //            {
                //                dttemp.ImportRow(dr1);
                //            }
                //        }
                //        else
                //            dttemp.ImportRow(dr1);
                //    }
                //}
                //string searchExpression1 = "CustomerId > 0 and Type='Debit'";
                //DataRow[] foundRows2 = dtList2.Select(searchExpression1);
                //dtList2.Columns.Remove("Type");
                ////dtList2.Columns.Remove("CustomerId");
                //dtList2.Columns.Remove("BuildingName");
                //dtList2.Columns.Remove("BlockNo");
                //dtList2.Columns.Remove("Customer");

                //foreach (DataRow dr1 in foundRows2)
                //{
                //    //check customer
                //    for (int i = 0; i < dtList2.Rows.Count; i++)
                //    {
                //        if (Convert.ToInt32(dttemp.Rows[i]["CustomerId"]) == Convert.ToInt32(dtList2.Rows[i]["CustomerId"]))
                //            dttemp.Rows[i]["DrAmt"] = dtList2.Rows[i]["Amt"].ToString();
                //        else
                //            dttemp.Rows[i]["DrAmt"] = 0;
                //    }
                //}
                //dttemp.Columns.Add("FinalAmt");
                //for (int i = 0; i < dttemp.Rows.Count; i++)
                //{
                //    if (!string.IsNullOrEmpty(dttemp.Rows[i]["DrAmt"].ToString()))
                //        dttemp.Rows[i]["FinalAmt"] = Convert.ToDecimal(dttemp.Rows[i]["Amt"]) - Convert.ToDecimal(dttemp.Rows[i]["DrAmt"]);
                //    else
                //        dttemp.Rows[i]["FinalAmt"] = Convert.ToDecimal(dttemp.Rows[i]["Amt"]) - 0;
                //}
                //string searchExpression3 = "CustomerId > 0";
                //DataRow[] foundRows3 = dttemp.Select(searchExpression3);
                //foreach (DataRow dr1 in foundRows3)
                //{
                //    if (Convert.ToDecimal(dr1["FinalAmt"]) < 200)
                //        dttemp1.ImportRow(dr1);
                //}

                //ViewBag.WalletList = dttemp1;
                //TempData["WalletList"] = ViewBag.WalletList;

                //Low balance v
                //chcek wallet amount
                decimal TotalCredit = 0, TotalDebit = 0;
                DataTable dttemp = new DataTable();
                dttemp.Columns.Add("Type", typeof(string));
                dttemp.Columns.Add("Customer", typeof(string));
                dttemp.Columns.Add("Sector", typeof(string));
                dttemp.Columns.Add("BuildingName", typeof(string));
                dttemp.Columns.Add("BlockNo", typeof(string));
                dttemp.Columns.Add("flatno", typeof(string));
                dttemp.Columns.Add("CustomerId", typeof(int));
                dttemp.Columns.Add("Amt", typeof(decimal));
                dttemp.Columns.Add("DrAmt", typeof(decimal));
                dttemp.Columns.Add("FinalAmt", typeof(decimal));
                dttemp.PrimaryKey = new DataColumn[] { dttemp.Columns["CustomerId"] };

                DataTable dttemp1 = new DataTable();
                dttemp1.Columns.Add("Type", typeof(string));
                dttemp1.Columns.Add("Customer", typeof(string));
                dttemp1.Columns.Add("Sector", typeof(string));
                dttemp1.Columns.Add("BuildingName", typeof(string));
                dttemp1.Columns.Add("BlockNo", typeof(string));
                dttemp1.Columns.Add("flatno", typeof(string));
                dttemp1.Columns.Add("CustomerId", typeof(int));
                dttemp1.Columns.Add("Amt", typeof(decimal));
                dttemp1.Columns.Add("DrAmt", typeof(decimal));
                dttemp1.Columns.Add("FinalAmt", typeof(decimal));
                dttemp1.PrimaryKey = new DataColumn[] { dttemp1.Columns["CustomerId"] };

                string searchExpression = "CustomerId > 0";
                DataRow[] foundRows1 = dtList1.Select(searchExpression);
                foreach (DataRow dr1 in foundRows1)
                {
                    //check customer
                    string customer = "Type='Credit' and CustomerId = " + dr1["CustomerId"].ToString();

                    string x = dr1["CustomerId"].ToString().Trim().ToUpper();
                    string y = dr1["CustomerId"].ToString();
                    if (dr1["CustomerId"].ToString().Trim().ToUpper().Contains(dr1["CustomerId"].ToString()))
                    {
                        if (dttemp.Rows.Count > 0)
                        {
                            if (dttemp.Rows.Contains(Convert.ToInt32(dr1["CustomerId"])))
                            {
                            }
                            else
                            {
                                dttemp.ImportRow(dr1);
                            }
                        }
                        else
                            dttemp.ImportRow(dr1);
                    }
                }
                DataTable dtprodRecord = new DataTable();
                DataRow[] dtwall = dttemp.Select();
                //foreach(DataRow fr in dtwall)
                //{
                for (int i = 0; i < dttemp.Rows.Count; i++)
                {
                    var balance = objsub.GetCustomerBalace(Convert.ToInt32(dttemp.Rows[i]["CustomerId"]));
                    dttemp.Rows[i]["FinalAmt"] = balance;

                    //dtprodRecord = objsub.getCustomerWallet(Convert.ToInt32(dttemp.Rows[i]["CustomerId"]));
                    //int userRecords = dtprodRecord.Rows.Count;
                    //if (userRecords > 0)
                    //{
                    //    if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["Amt"].ToString()))
                    //        TotalCredit = Convert.ToDecimal(dtprodRecord.Rows[0]["Amt"]);
                    //    if (userRecords > 1)
                    //    {
                    //        if (!string.IsNullOrEmpty(dtprodRecord.Rows[1]["Amt"].ToString()))
                    //            TotalDebit = Convert.ToDecimal(dtprodRecord.Rows[1]["Amt"]);
                    //        else
                    //            TotalDebit = 0;
                    //    }
                    //    else
                    //    {
                    //        TotalDebit = 0;
                    //    }

                    //    dttemp.Rows[i]["FinalAmt"] = TotalCredit - TotalDebit;
                    //}
                }
                //}

                string searchExpression3 = "CustomerId > 0";
                DataRow[] foundRows3 = dttemp.Select(searchExpression3);
                foreach (DataRow dr1 in foundRows3)
                {
                    if (Convert.ToDecimal(dr1["FinalAmt"]) < 200)
                        dttemp1.ImportRow(dr1);
                }

                ViewBag.WalletList = dttemp1;
                TempData["WalletList"] = ViewBag.WalletList;

                //DeliveryBoy Status v
                Staff objStaff = new Staff();
                DataTable dtStaff = new DataTable();
                dtStaff = objStaff.getDeliveryBoyList(null);
                ViewBag.Staff = dtStaff;

                DataTable del = new DataTable();
                del = objorder.getDeliveryBoyCustomerOrder(null, null, System.DateTime.Now.Date.AddDays(1), System.DateTime.Now.Date.AddDays(1), null);
                ViewBag.PorderList = del;
                TempData["PorderList"] = ViewBag.PorderList;

                //Vendor v
                DataTable dtsec = new DataTable();
                dtsec = objsector.getSectorList(null);
                ViewBag.Sector = dtsec;

                Vendor objvendor = new Vendor();
                DataTable dtven = new DataTable();
                dtven = objvendor.getVendorList(null);
                ViewBag.Vendor = dtven;

                DataTable dtvp = new DataTable();
                dtvp = objorder.getSectorVendorOrderStatus(null, null, System.DateTime.Now.Date.AddDays(1), System.DateTime.Now.Date.AddDays(1), null);
                ViewBag.vendorpList = dtvp;
                TempData["vendorpList"] = ViewBag.vendorpList;



                ViewBag.SectorId = objorder.SectorId;
                ViewBag.BuildingId = objorder.BuildingId;
                ViewBag.DaysId = objorder.DaysId;


                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

      
        
        [HttpPost]
        public JsonResult DeliveryStatus(string StaffId, string Status)
        {
            CustomerOrder objorder = new CustomerOrder();
            string data = "";
            try
            {
                if (!string.IsNullOrEmpty(StaffId) && Convert.ToInt32(StaffId) != 0)
                {
                    objorder.StaffId = Convert.ToInt32(StaffId);
                }
                if (!string.IsNullOrEmpty(Status))
                {
                    objorder.Status = Status;
                }
                DataTable delstatusList = objorder.getDeliveryBoyCustomerOrder(objorder.StaffId, null, System.DateTime.Now.Date.AddDays(1), System.DateTime.Now.Date.AddDays(1), objorder.Status);
                data = string.Empty;
                data = JsonConvert.SerializeObject(delstatusList);
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult VendorProduct(string secId, string venId, string Status)
        {
            CustomerOrder objorder = new CustomerOrder();
            try
            {
                if (!string.IsNullOrEmpty(secId) && Convert.ToInt32(secId) != 0)
                {
                    objorder.SectorId = Convert.ToInt32(secId);
                }
                if (!string.IsNullOrEmpty(venId) && Convert.ToInt32(venId) != 0)
                {
                    objorder.VendorId = Convert.ToInt32(venId);
                }
                if (!string.IsNullOrEmpty(Status))
                {
                    objorder.Status = Status;
                }
                DataTable vp = objorder.getSectorVendorOrderStatus(objorder.SectorId, objorder.VendorId, System.DateTime.Now.Date.AddDays(1), System.DateTime.Now.Date.AddDays(1), objorder.Status);
                string data = string.Empty;
                data = JsonConvert.SerializeObject(vp);
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            return View();
        }

        [HttpPost]
        public ActionResult LowBalanceList(string secId,string BuildingId)
        {
            Customer objcust = new Customer();
            CustomerOrder objorder = new CustomerOrder();
            Sector objsector = new Sector();
            string data="";
            try
            {
                DataTable dtList1 = new DataTable();
                dtList1 = objorder.getCustomerWalletBalDash(Convert.ToInt32(secId),Convert.ToInt32(BuildingId));
                ViewBag.WalletList = dtList1;

                decimal TotalCredit = 0, TotalDebit = 0;
                DataTable dttemp = new DataTable();
                dttemp.Columns.Add("Type", typeof(string));
                dttemp.Columns.Add("Customer", typeof(string));
                dttemp.Columns.Add("Sector", typeof(string));
                dttemp.Columns.Add("BuildingName", typeof(string));
                dttemp.Columns.Add("BlockNo", typeof(string));
                dttemp.Columns.Add("flatno", typeof(string));
                dttemp.Columns.Add("CustomerId", typeof(int));
                dttemp.Columns.Add("Amt", typeof(decimal));
                dttemp.Columns.Add("DrAmt", typeof(decimal));
                dttemp.Columns.Add("FinalAmt", typeof(decimal));
                dttemp.PrimaryKey = new DataColumn[] { dttemp.Columns["CustomerId"] };

                DataTable dttemp1 = new DataTable();
                dttemp1.Columns.Add("Type", typeof(string));
                dttemp1.Columns.Add("Customer", typeof(string));
                dttemp1.Columns.Add("Sector", typeof(string));
                dttemp1.Columns.Add("BuildingName", typeof(string));
                dttemp1.Columns.Add("BlockNo", typeof(string));
                dttemp1.Columns.Add("flatno", typeof(string));
                dttemp1.Columns.Add("CustomerId", typeof(int));
                dttemp1.Columns.Add("Amt", typeof(decimal));
                dttemp1.Columns.Add("DrAmt", typeof(decimal));
                dttemp1.Columns.Add("FinalAmt", typeof(decimal));
                dttemp1.PrimaryKey = new DataColumn[] { dttemp1.Columns["CustomerId"] };

                string searchExpression = "CustomerId > 0";
                DataRow[] foundRows1 = dtList1.Select(searchExpression);
                foreach (DataRow dr1 in foundRows1)
                {
                    //check customer
                    string customer = "Type='Credit' and CustomerId = " + dr1["CustomerId"].ToString();

                    string x = dr1["CustomerId"].ToString().Trim().ToUpper();
                    string y = dr1["CustomerId"].ToString();
                    if (dr1["CustomerId"].ToString().Trim().ToUpper().Contains(dr1["CustomerId"].ToString()))
                    {
                        if (dttemp.Rows.Count > 0)
                        {
                            if (dttemp.Rows.Contains(Convert.ToInt32(dr1["CustomerId"])))
                            {
                            }
                            else
                            {
                                dttemp.ImportRow(dr1);
                            }
                        }
                        else
                            dttemp.ImportRow(dr1);
                    }
                }
                DataTable dtprodRecord = new DataTable();
                DataRow[] dtwall = dttemp.Select();
                //foreach(DataRow fr in dtwall)
                //{
                for (int i = 0; i < dttemp.Rows.Count; i++)
                {
                    dtprodRecord = objsub.getCustomerWallet(Convert.ToInt32(dttemp.Rows[i]["CustomerId"]));
                    int userRecords = dtprodRecord.Rows.Count;
                    if (userRecords > 0)
                    {
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["Amt"].ToString()))
                            TotalCredit = Convert.ToDecimal(dtprodRecord.Rows[0]["Amt"]);
                        if (userRecords > 1)
                        {
                            if (!string.IsNullOrEmpty(dtprodRecord.Rows[1]["Amt"].ToString()))
                                TotalDebit = Convert.ToDecimal(dtprodRecord.Rows[1]["Amt"]);
                            else
                                TotalDebit = 0;
                        }
                        else
                        {
                            TotalDebit = 0;
                        }

                        dttemp.Rows[i]["FinalAmt"] = TotalCredit - TotalDebit;
                    }
                }
                //}

                string searchExpression3 = "CustomerId > 0";
                DataRow[] foundRows3 = dttemp.Select(searchExpression3);
                foreach (DataRow dr1 in foundRows3)
                {
                    if (Convert.ToDecimal(dr1["FinalAmt"]) < 200)
                        dttemp1.ImportRow(dr1);
                }

                data = string.Empty;
                data = JsonConvert.SerializeObject(dttemp1);
            }
            catch(Exception e)
            {
                TempData["ErrorMsg"] = e.Message;
            }
            return Json(data,JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeliveryBoyDailyOrderPrint(string StaffId, string Status)
        {
            try
            {
                CustomerOrder objorder = new CustomerOrder();
                if (StaffId == "0") StaffId = null;
                if (Status == "0") Status = null;

                DataTable dt = new DataTable();
                Report.StaffOrder obj = new Report.StaffOrder();
                ReportDocument rd = new ReportDocument();
                rd.Load(Server.MapPath("~/Report/StaffOrderFlatWise.rpt"));
                SqlDataAdapter da = new SqlDataAdapter("Sector_Staff_Order_SelectAll", con);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                if (!string.IsNullOrEmpty(StaffId))
                    da.SelectCommand.Parameters.AddWithValue("@DeliveryBoyId", StaffId);
                else
                    da.SelectCommand.Parameters.AddWithValue("@DeliveryBoyId", DBNull.Value);
                
                    da.SelectCommand.Parameters.AddWithValue("@CustomerId", DBNull.Value);
               
                    da.SelectCommand.Parameters.AddWithValue("@FromDate", System.DateTime.Now.Date.AddDays(1));
                
                    da.SelectCommand.Parameters.AddWithValue("@ToDate", System.DateTime.Now.Date.AddDays(1));
                if (!string.IsNullOrEmpty(Status))
                    da.SelectCommand.Parameters.AddWithValue("@OrderStatus", Status);
                else
                    da.SelectCommand.Parameters.AddWithValue("@OrderStatus", DBNull.Value);
                da.Fill(dt);

                rd.Database.Tables[0].SetDataSource(dt);
                rd.SetParameterValue("@DeliveryBoyId", StaffId);
                rd.SetParameterValue("@CustomerId", null);
                rd.SetParameterValue("@FromDate", System.DateTime.Now.Date.AddDays(1));
                rd.SetParameterValue("@ToDate", System.DateTime.Now.Date.AddDays(1));
                rd.SetParameterValue("@OrderStatus", Status);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                //CrystalReportViewer1.RefreshReport();
                try
                {
                    Stream str = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    str.Seek(0, SeekOrigin.Begin);
                    return File(str, "application/pdf", "DeliveryBoyDailyOrder.pdf");
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public ActionResult DeliveryBoyDailyOrderReport(string StaffId, string Status)
        {
            try
            {
                CustomerOrder objorder = new CustomerOrder();
                if (StaffId == "0") StaffId = null;
                if (Status == "0") Status = null;
                string Fdate = System.DateTime.Now.Date.AddDays(1).ToString("dd-MM-yyyy");
                string Tdate = System.DateTime.Now.Date.AddDays(1).ToString("dd-MM-yyyy");
                string query = string.Format("DeliveryboyId={0}&CustomerId={1}&FDate={2}&TDate={3}&status={4}",
                       StaffId, null, Fdate, Tdate, Status);
                //return Redirect("/customerorder/DeliveryBoyDailyReport?" + query);
                //return Redirect("/Report/DeliveryBoyDailyReport?" + query);
                return Redirect("/Reportnew/DeliveryBoyDailyReportvendor1?" + query);
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public ActionResult VendorProductOrderPrint(string SectorId, string VendorId,string status)
        {
            CustomerOrder objorder = new CustomerOrder();
          
            string Filter = null;
            

            DateTime fd = DateTime.Now.Date.AddDays(1);
            DateTime td = DateTime.Now.Date.AddDays(1);
       
            Filter =fd.ToString("dd/MM/yyyy");
            DataTable dt = new DataTable();
            ReportDocument rd = new ReportDocument();
            rd.Load(Server.MapPath("~/Report/SectorWiseProductSummaryStatus.rpt"));
          

            dt = objorder.getSectorVendorOrderStatus(Convert.ToInt32(SectorId), Convert.ToInt32(VendorId), System.DateTime.Now.Date.AddDays(1), System.DateTime.Now.Date.AddDays(1), status);

            rd.Database.Tables[0].SetDataSource(dt);
            rd.SetParameterValue("@SectorId", SectorId);
            rd.SetParameterValue("@VendorId", VendorId);
            rd.SetParameterValue("@FromDate", System.DateTime.Now.AddDays(1));
            rd.SetParameterValue("@ToDate", System.DateTime.Now.AddDays(1));
            rd.SetParameterValue("@Filter", Filter);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //CrystalReportViewer1.RefreshReport();
            try
            {
                Stream str = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                str.Seek(0, SeekOrigin.Begin);
                return File(str, "application/pdf", "VendorDailyOrderwithStatus.pdf");
            }
            catch
            {
                throw;
            }

        }

        public ActionResult VendorProductOrderReport(string SectorId, string VendorId, string status)
        {
            try
            {
                CustomerOrder objorder = new CustomerOrder();
                if (SectorId == "0") SectorId = null;
                if (VendorId == "0") VendorId = null;
                if (status == "0") status = null;                
                string Fdate = System.DateTime.Now.Date.AddDays(1).ToString("dd-MM-yyyy");
                string Tdate = System.DateTime.Now.Date.AddDays(1).ToString("dd-MM-yyyy");

                string query = string.Format("SectorId={0}&VendorId={1}&FDate={2}&TDate={3}&Status={4}",
                       SectorId, VendorId, Fdate, Tdate, status);
                //return Redirect("/Vendor/VendorProductOrderReportStatus?" + query);                
                return Redirect("/Report/VendorProductOrderReportStatus?" + query);
            }
            catch
            {
                throw;
            }

        }

        public ActionResult CustomerLowBalanceListPrint(string secId, string BuildingId)
        {
            Customer objcust = new Customer();
            CustomerOrder objorder = new CustomerOrder();
            Sector objsector = new Sector();

            string Tdate = null;
            DateTime fd = DateTime.Now.Date;
            Tdate = fd.ToString("dd/MM/yyyy");
           
            ReportDocument rd = new ReportDocument();
            rd.Load(Server.MapPath("~/Report/CustomerLowBalance.rpt"));

            DataTable dtList1 = new DataTable();
            dtList1 = objorder.getCustomerWalletBalDash(Convert.ToInt32(secId), Convert.ToInt32(BuildingId));
            ViewBag.WalletList = dtList1;

            decimal TotalCredit = 0, TotalDebit = 0;
            DataTable dttemp = new DataTable();
            dttemp.Columns.Add("Type", typeof(string));
            dttemp.Columns.Add("Customer", typeof(string));
            dttemp.Columns.Add("Sector", typeof(string));
            dttemp.Columns.Add("BuildingName", typeof(string));
            dttemp.Columns.Add("BlockNo", typeof(string));
            dttemp.Columns.Add("flatno", typeof(string));
            dttemp.Columns.Add("CustomerId", typeof(int));
            dttemp.Columns.Add("Amt", typeof(decimal));
            dttemp.Columns.Add("DrAmt", typeof(decimal));
            dttemp.Columns.Add("FinalAmt", typeof(decimal));
            dttemp.PrimaryKey = new DataColumn[] { dttemp.Columns["CustomerId"] };

            DataTable dttemp1 = new DataTable();
            dttemp1.Columns.Add("Type", typeof(string));
            dttemp1.Columns.Add("Customer", typeof(string));
            dttemp1.Columns.Add("Sector", typeof(string));
            dttemp1.Columns.Add("BuildingName", typeof(string));
            dttemp1.Columns.Add("BlockNo", typeof(string));
            dttemp1.Columns.Add("flatno", typeof(string));
            dttemp1.Columns.Add("CustomerId", typeof(int));
            dttemp1.Columns.Add("Amt", typeof(decimal));
            dttemp1.Columns.Add("DrAmt", typeof(decimal));
            dttemp1.Columns.Add("FinalAmt", typeof(decimal));
            dttemp1.PrimaryKey = new DataColumn[] { dttemp1.Columns["CustomerId"] };

            string searchExpression = "CustomerId > 0";
            DataRow[] foundRows1 = dtList1.Select(searchExpression);
            foreach (DataRow dr1 in foundRows1)
            {
                //check customer
                string customer = "Type='Credit' and CustomerId = " + dr1["CustomerId"].ToString();

                string x = dr1["CustomerId"].ToString().Trim().ToUpper();
                string y = dr1["CustomerId"].ToString();
                if (dr1["CustomerId"].ToString().Trim().ToUpper().Contains(dr1["CustomerId"].ToString()))
                {
                    if (dttemp.Rows.Count > 0)
                    {
                        if (dttemp.Rows.Contains(Convert.ToInt32(dr1["CustomerId"])))
                        {
                        }
                        else
                        {
                            dttemp.ImportRow(dr1);
                        }
                    }
                    else
                        dttemp.ImportRow(dr1);
                }
            }
            DataTable dtprodRecord = new DataTable();
            DataRow[] dtwall = dttemp.Select();
            //foreach(DataRow fr in dtwall)
            //{
            for (int i = 0; i < dttemp.Rows.Count; i++)
            {
                dtprodRecord = objsub.getCustomerWallet(Convert.ToInt32(dttemp.Rows[i]["CustomerId"]));
                int userRecords = dtprodRecord.Rows.Count;
                if (userRecords > 0)
                {
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["Amt"].ToString()))
                        TotalCredit = Convert.ToDecimal(dtprodRecord.Rows[0]["Amt"]);
                    if (userRecords > 1)
                    {
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[1]["Amt"].ToString()))
                            TotalDebit = Convert.ToDecimal(dtprodRecord.Rows[1]["Amt"]);
                        else
                            TotalDebit = 0;
                    }
                    else
                    {
                        TotalDebit = 0;
                    }

                    dttemp.Rows[i]["FinalAmt"] = TotalCredit - TotalDebit;
                }
            }
            //}

            string searchExpression3 = "CustomerId > 0";
            DataRow[] foundRows3 = dttemp.Select(searchExpression3);
            foreach (DataRow dr1 in foundRows3)
            {
                if (Convert.ToDecimal(dr1["FinalAmt"]) < 200)
                    dttemp1.ImportRow(dr1);
            }

            rd.Database.Tables[0].SetDataSource(dttemp1);
            //rd.SetParameterValue("@SectorId", secId);
            //rd.SetParameterValue("@VendorId", BuildingId);
           

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            //CrystalReportViewer1.RefreshReport();
            try
            {
                Stream str = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                str.Seek(0, SeekOrigin.Begin);
                return File(str, "application/pdf", "CustomerWithLowBalance.pdf");
            }
            catch (Exception e)
            {
                throw;
            }

        }


        [HttpGet]
        public ActionResult BackTutorial()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            Chatbot objchatbot = new Chatbot();
            DataTable dtList = new DataTable();
            dtList = objchatbot.getBackOfficeSteps(null);
            if(dtList.Rows.Count>0)
            {
                string htmlstring = "";
                for(int i=0;i<dtList.Rows.Count;i++)
                {
                    htmlstring += " <div class=\"row\"><div class=\"col-md-12\">";
                    htmlstring += " <a href=\"#\" data-key='{\"Param\":"+ dtList.Rows[i]["Id"]+"}' style=\"padding:5px;text-decoration:none;border-radius:5px;background-color:#fff;\" class=\"myLink\">"+dtList.Rows[i]["Stepname"].ToString()+"</a>";
                    htmlstring += " <div id=" + dtList.Rows[i]["Id"] + " style=\"display: none; \">";
                    htmlstring += " <div class=\"box box-info\"><div class=\"box\"> <div class=\"box-header with-border\"> " + dtList.Rows[i]["stepfollow"].ToString() + " ";
                    if(!string.IsNullOrEmpty(dtList.Rows[i]["image"].ToString()))
                    {
                        htmlstring += "<br/><img src=\"../image/Backoffice/" + dtList.Rows[i]["image"].ToString() + "\" style=\"width:200px;height:200px;\"></img>";
                    }
                    if (!string.IsNullOrEmpty(dtList.Rows[i]["VideoLink"].ToString()))
                    {
                        //htmlstring += "<br/><img src=\"../image/Backoffice/" + dtList.Rows[i]["VideoLink"].ToString() + "\"></img>";
                        htmlstring += "<br/> <video id=\"VideoPlayer\" src=\"../Video/" + dtList.Rows[i]["VideoLink"].ToString() + "\" controls=\"true\" width = \"300\" height = \"300\" loop = \"true\" /> ";
                    }
                    htmlstring += "</div>";
                    htmlstring += " </div></div></div> </div> </div> <hr />";
                }

                ViewBag.htmlList = htmlstring;
            }

            return View();
        }

        [HttpGet]
        public ActionResult AddBackOfficeSetting()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;




            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AddBackOfficeSetting(Chatbot obj, FormCollection form, HttpPostedFileBase Document1, HttpPostedFileBase Document2)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            string fname = null, path = null;
            HttpPostedFileBase document1 = Request.Files["Document1"];
            string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png", ".PNG" };
            if (document1 != null)
            {
                if (document1.ContentLength > 0)
                {
                    try
                    {
                        HttpFileCollectionBase files = Request.Files;
                        HttpPostedFileBase file = Document1;
                        //Resize image 500*300 coding
                        WebImage img = new WebImage(file.InputStream);
                        img.Resize(300, 300, false, false);
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string extension = Path.GetExtension(file.FileName);
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.ToLower().LastIndexOf('.'))))
                        {
                            ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
                        }
                        else
                        {
                            fileName = dHelper.RemoveIllegalCharacters(fileName);
                            fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                        }
                        path = Path.Combine(Server.MapPath("~/image/Backoffice/"), fname);
                        img.Save(path);
                        //file.SaveAs(path);
                        obj.Image = fname;
                    }

                    catch (Exception ex)
                    {
                        ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                    }
                }
            }
            //
            
             document1 = Request.Files["Document2"];
             sAllowedExt = new string[] { ".mp4", ".jpeg", ".png", ".PNG" };
            if (document1 != null)
            {
                if (document1.ContentLength > 0)
                {
                    try
                    {
                        HttpFileCollectionBase files = Request.Files;
                        HttpPostedFileBase file = Document2;
                        //Resize image 500*300 coding
                        //WebImage img = new WebImage(file.InputStream);
                        //img.Resize(300, 300, false, false);
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string extension = Path.GetExtension(file.FileName);
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.ToLower().LastIndexOf('.'))))
                        {
                            ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
                        }
                        else
                        {
                            fileName = dHelper.RemoveIllegalCharacters(fileName);
                            fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                        }
                        path = Path.Combine(Server.MapPath("~/Video/"), fname);
                        file.SaveAs(path);
                        //file.SaveAs(path);
                        obj.videolink = fname;
                    }

                    catch (Exception ex)
                    {
                        ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                    }
                }
            }

            //
            int addresult = obj.insertBackofficeSteps(obj);
            if (addresult > 0)
            { ViewBag.SuccessMsg = "Backoffice Steps Inserted Successfully!!!"; }
            else
            { ViewBag.SuccessMsg = "Backoffice Steps Not Inserted!!!"; }

            ModelState.Clear();

            return View();
        }


        [HttpGet]
        public ActionResult BackTutorialList()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            Chatbot objchatbot = new Chatbot();
            DataTable dtList = new DataTable();
            dtList = objchatbot.getBackOfficeSteps(null);
            ViewBag.BackOfficestepList = dtList;


            return View();
        }

        [HttpGet]
        public ActionResult EditBackOffice(int id = 0)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");
            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            Chatbot objchatbot = new Chatbot();
            var model = new Chatbot();
            DataTable dt = new DataTable();
            dt = objchatbot.getBackOfficeSteps(id);
            if (dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dt.Rows[0]["Stepname"].ToString()))
                {
                    model.Query = dt.Rows[0]["Stepname"].ToString();
                    
                }

                else
                    ViewBag.Query = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["youtubelink"].ToString()))
                {
                    model.Youtubelink = dt.Rows[0]["youtubelink"].ToString();
                   
                }

                if (!string.IsNullOrEmpty(dt.Rows[0]["stepfollow"].ToString()))
                {
                    model.steps = dt.Rows[0]["stepfollow"].ToString();
                    
                }

                else
                    ViewBag.QueryYes = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["image"].ToString()))
                {
                    model.Image = dt.Rows[0]["image"].ToString();

                }

                if (!string.IsNullOrEmpty(dt.Rows[0]["VideoLink"].ToString()))
                {
                    model.videolink = dt.Rows[0]["VideoLink"].ToString();

                }
                model.BackOfficeId = id.ToString();

            }


            return View(model);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult EditBackOffice(Chatbot obj, FormCollection form, HttpPostedFileBase Document1, HttpPostedFileBase Document2)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            string fname = null, path = null;
            HttpPostedFileBase document1 = Request.Files["Document1"];
            string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png", ".PNG" };
            if (document1 != null)
            {
                if (document1.ContentLength > 0)
                {
                    try
                    {
                        HttpFileCollectionBase files = Request.Files;
                        HttpPostedFileBase file = Document1;
                        //Resize image 500*300 coding
                        WebImage img = new WebImage(file.InputStream);
                        img.Resize(300, 300, false, false);
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string extension = Path.GetExtension(file.FileName);
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.ToLower().LastIndexOf('.'))))
                        {
                            ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
                        }
                        else
                        {
                            fileName = dHelper.RemoveIllegalCharacters(fileName);
                            fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                        }
                        path = Path.Combine(Server.MapPath("~/image/Backoffice/"), fname);
                        img.Save(path);
                        //file.SaveAs(path);
                        obj.Image = fname;
                    }

                    catch (Exception ex)
                    {
                        ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                    }
                }
            }
            //
            document1 = Request.Files["Document2"];
            sAllowedExt = new string[] { ".mp4", ".jpeg", ".png", ".PNG" };
            if (document1 != null)
            {
                if (document1.ContentLength > 0)
                {
                    try
                    {
                        HttpFileCollectionBase files = Request.Files;
                        HttpPostedFileBase file = Document2;
                        //Resize image 500*300 coding
                        //WebImage img = new WebImage(file.InputStream);
                        //img.Resize(300, 300, false, false);
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string extension = Path.GetExtension(file.FileName);
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.ToLower().LastIndexOf('.'))))
                        {
                            ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
                        }
                        else
                        {
                            fileName = dHelper.RemoveIllegalCharacters(fileName);
                            fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                        }
                        path = Path.Combine(Server.MapPath("~/Video/"), fname);
                        file.SaveAs(path);
                        //file.SaveAs(path);
                        obj.videolink = fname;
                    }

                    catch (Exception ex)
                    {
                        ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                    }
                }
            }
            //
            int addresult = obj.UpdateBackofficeSteps(obj);
            if (addresult > 0)
            { ViewBag.SuccessMsg = "Backoffice Steps Updated Successfully!!!"; }
            else
            { ViewBag.SuccessMsg = "Backoffice Steps Not Updated!!!"; }

            ModelState.Clear();

            return View();
        }

        public ActionResult DeleteBackoffice(int id)
        {
            try
            {
                Chatbot obj = new Chatbot();
                // int delresult = 0;
                int delresult = obj.DeleteBackOfficeSet(id);
                return RedirectToAction("BackTutorialList");
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
            return RedirectToAction("BackTutorialList");
        }
    }
}
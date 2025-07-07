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
    public class CustomerOrderController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        Customer objcust = new Customer();
        Subscription objsub = new Subscription();
        // GET: CustomerOrder
        [HttpGet]
        public ActionResult AddCustomerOrder()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            Customer objorder = new Customer();
            DataTable dt = new DataTable();
            dt = objorder.BindCustomer(null);
            ViewBag.Customer = dt;

            Product objProduct = new Product();
            DataTable dt1 = new DataTable();
            dt1 = objProduct.BindProuct1(null);
            ViewBag.Product = dt1;

            DataTable dtcategory = new DataTable();
            dtcategory = objProduct.BindCategory(null);
            ViewBag.Category = dtcategory;

            return View();
        }

        [HttpPost]
        public ActionResult AddCustomerOrder(string CustomerId, string Type, string fromDate, string todate, string ProductId, string Qty, string json)
        {
            Subscription objsub = new Subscription();
            Customer objorder = new Customer();
            DataTable dt = new DataTable();
            Product objProduct = new Product();
            DataTable dt1 = new DataTable();

            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            else
            {
                DataTable dtcuttime = objcust.GetSchedularTime(null);
                int dbcutOfftime = Convert.ToInt32(dtcuttime.Rows[0]["CutOffTime"]);

                long AddProductOrder = 1; int AddProductDetail = 0;
                //string customer = Request["ddlCustomer"];
                //if (!string.IsNullOrEmpty(customer))
                //{
                //    //objsub.CustomerId = Convert.ToInt32(customer);
                //    objsub.CustomerId = Convert.ToInt32(customer);
                //}
                objsub.CustomerId = Convert.ToInt32(CustomerId);
                //string type = Request["ddlType"];
                //if (!string.IsNullOrEmpty(type))
                //{
                //    objsub.OrderFlag = type;
                //}
                if (Type == "1")
                    objsub.OrderFlag = "Daily";
                else if (Type == "2")
                    objsub.OrderFlag = "Week";
                else if (Type == "3")
                    objsub.OrderFlag = "Add";
                else
                    objsub.OrderFlag = null;

                //string product = Request["ddlProduct"];
                //if (!string.IsNullOrEmpty(product))
                //{
                //    objsub.ProductId = Convert.ToInt32(product);
                //}
                objsub.ProductId = Convert.ToInt32(ProductId);
                objsub.Qty = Convert.ToInt32(Qty);
                //get BuildingId
                int societyid = 0;
                DataTable dtBuildingId = objcust.BindCustomer(objsub.CustomerId);
                if (dtBuildingId.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["BuildingId"].ToString()))
                        societyid = Convert.ToInt32(dtBuildingId.Rows[0]["BuildingId"]);
                }
                if (!string.IsNullOrEmpty(societyid.ToString()))
                { objsub.BuildingId = Convert.ToInt32(societyid); }
                else { objsub.BuildingId = 0; }

                objsub.Status = "Pending";
                objsub.StateCode = null;

                DateTime FromDate = DateTime.Now;
                if (DateTime.Now.Hour < dbcutOfftime)
                {
                    FromDate = FromDate.AddDays(1);
                }
                else
                    FromDate = FromDate.AddDays(2);
                ////  DateTime FromDate = DateTime.Now.AddDays(1);
                DateTime ToDate = DateTime.Now;

                //var fdate = form["datepicker"];
                var fdate = fromDate;
                if (!string.IsNullOrEmpty(fdate.ToString()))
                {
                    objsub.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
                    FromDate = objsub.FromDate;
                }
                // var tdate = form["datepicker1"];
                var tdate = todate;
                if (!string.IsNullOrEmpty(tdate.ToString()))
                {
                    if (Type != "1")
                    {
                        objsub.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
                        ToDate = objsub.ToDate;
                    }
                }
                //get Product detail
                DataTable dtProduct = objProduct.BindProuct(objsub.ProductId);
                if (dtProduct.Rows.Count > 0)
                {
                    //if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Id"].ToString()))
                    //    objsub.productid = dtProduct.Rows[0]["Id"].ToString();
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SalePrice"].ToString()))
                        objsub.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
                    else
                        objsub.SalePrice = 0;
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SalePrice"].ToString()))
                        objsub.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]) * objsub.Qty;
                    else
                        objsub.Amount = 0;
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountAmount"].ToString()))
                        objsub.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountAmount"]) * objsub.Qty;
                    else
                        objsub.Discount = 0;
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                        objsub.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * objsub.Qty;
                    else
                        objsub.CGSTPerct = 0;
                    //count cgst Amount
                    if (objsub.CGSTPerct > 0)
                        objsub.CGSTAmount = (objsub.Amount * objsub.CGSTPerct) / 100;
                    else
                        objsub.CGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                        objsub.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * objsub.Qty;
                    else
                        objsub.IGSTPerct = 0;
                    //count igst Amount
                    if (objsub.IGSTPerct > 0)
                        objsub.IGSTAmount = (objsub.Amount * objsub.IGSTPerct) / 100;
                    else
                        objsub.IGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                        objsub.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * objsub.Qty;
                    else
                        objsub.SGSTPerct = 0;
                    //count sgst Amount
                    if (objsub.SGSTPerct > 0)
                        objsub.SGSTAmount = (objsub.Amount * objsub.SGSTPerct) / 100;
                    else
                        objsub.SGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                        objsub.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * objsub.Qty;
                    else
                        objsub.RewardPoint = 0;
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                        objsub.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * objsub.Qty;
                    else
                        objsub.Profit = 0;

                    //Final Amount
                    objsub.TotalFinalAmount = objsub.Amount;
                }
                objsub.TotalGSTAmt = objsub.CGSTAmount + objsub.SGSTAmount;
                objsub.TotalAmount = objsub.TotalFinalAmount;

                //chcek wallet amount
                decimal Walletbal = 0;
                var balance = objsub.GetCustomerBalace(objsub.CustomerId);
                Walletbal = balance;

                if (Type == "1")
                {
                    ToDate = Helper.indianTime.AddMonths(2);
                }
                // if(Walletbal > objsub)
                DateTime _fromDate = FromDate;
                if (objsub.OrderFlag == "Daily")
                {
                    for (int idate = 0; FromDate <= ToDate; idate++)
                    {
                        //Generate OrderNo
                        con.Open();
                        SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                        com1.CommandType = CommandType.StoredProcedure;
                        var Formno = com1.ExecuteScalar();
                        con.Close();

                        objsub.OrderNo = Convert.ToInt32(Formno);
                        objsub.OrderDate = FromDate;

                        //get Subscription Id
                        DataTable dtCustSubscription = objsub.getCustomerSubscriptionOrderdate(objsub);
                        if (dtCustSubscription.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                                objsub.CustSubscriptionId = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                            else
                                objsub.CustSubscriptionId = 0;
                        }
                        else
                            objsub.CustSubscriptionId = 0;

                        AddProductOrder = objsub.InsertCustomerOrder(objsub);

                        if (AddProductOrder > 0)
                        {
                            objsub.OrderId = Convert.ToInt32(AddProductOrder);
                            objsub.OrderItemDate = FromDate;
                            AddProductDetail = objsub.InsertCustomerOrderDetail(objsub);
                        }
                        FromDate = FromDate.AddDays(1);
                    }
                }
                else if (objsub.OrderFlag == "Week")
                {
                    DataTable dtweekdetail = new DataTable();
                    if (json != null)
                    {
                        json = json.ToString().Replace("undefined", "week");
                        json = json.Trim();
                        dtweekdetail = (DataTable)JsonConvert.DeserializeObject(json.ToString(), (typeof(DataTable)));
                    }

                    for (int idate = 0; FromDate <= ToDate; idate++)
                    {
                        //Generate OrderNo
                        con.Open();
                        SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                        com1.CommandType = CommandType.StoredProcedure;
                        var Formno = com1.ExecuteScalar();
                        con.Close();

                        objsub.OrderNo = Convert.ToInt32(Formno);
                        objsub.OrderDate = FromDate;
                        string orderday = objsub.OrderDate.ToString("dddd");
                        //check week day selected
                        DataColumn[] columns = dtweekdetail.Columns.Cast<DataColumn>().ToArray();
                        bool anyFieldContains = dtweekdetail.AsEnumerable()
                            .Any(row => columns.Any(col => row[col].ToString().TrimStart().ToLower() == orderday.ToLower()));
                        ///bool anyFieldContains = false;
                        if (anyFieldContains == true)
                        {
                            //get Subscription Id
                            DataTable dtCustSubscription = objsub.getCustomerSubscriptionOrderdate(objsub);
                            if (dtCustSubscription.Rows.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                                    objsub.CustSubscriptionId = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                                else
                                    objsub.CustSubscriptionId = 0;
                            }
                            else
                                objsub.CustSubscriptionId = 0;

                            AddProductOrder = objsub.InsertCustomerOrder(objsub);

                            if (AddProductOrder > 0)
                            {
                                objsub.OrderId = Convert.ToInt32(AddProductOrder);
                                objsub.OrderItemDate = FromDate;
                                AddProductDetail = objsub.InsertCustomerOrderDetail(objsub);
                            }
                        }
                        FromDate = FromDate.AddDays(1);
                    }
                }
                if (objsub.OrderFlag == "Add")
                {
                    for (int idate = 0; FromDate <= ToDate; idate++)
                    {
                        //Generate OrderNo
                        con.Open();
                        SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                        com1.CommandType = CommandType.StoredProcedure;
                        var Formno = com1.ExecuteScalar();
                        con.Close();

                        objsub.OrderNo = Convert.ToInt32(Formno);
                        objsub.OrderDate = FromDate;
                        objsub.CustSubscriptionId = 0;
                        AddProductOrder = objsub.InsertCustomerOrder(objsub);

                        if (AddProductOrder > 0)
                        {
                            objsub.OrderId = Convert.ToInt32(AddProductOrder);
                            objsub.OrderItemDate = FromDate;

                            AddProductDetail = objsub.InsertCustomerOrderDetail(objsub);
                        }
                        FromDate = FromDate.AddDays(1);
                    }
                }

                if (AddProductDetail > 0)
                {
                    if (Type == "1")
                    {
                        Helper dHelper = new Helper();
                        dHelper.InsertCustomerOrderTrack(objsub.CustomerId, objsub.ProductId, objsub.Qty, _fromDate, ToDate);
                    }
                    //ViewBag.SuccessMsg = "Order Inserted Successfully!!!";
                    string jsonString1 = string.Empty;
                    jsonString1 = JsonConvert.SerializeObject(1);
                    return Json(jsonString1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string jsonString1 = string.Empty;
                    jsonString1 = JsonConvert.SerializeObject(0);
                    return Json(jsonString1, JsonRequestBehavior.AllowGet);
                }

                dt = objorder.BindCustomer(null);
                ViewBag.Customer = dt;

                dt1 = objProduct.BindProuct(null);
                ViewBag.Product = dt1;
                //return View();
            }
        }

        [HttpGet]
        public ActionResult CustomerOrderList()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            //if (control.IsView == false)
            //    return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            Customer objcust = new Customer();
            DataTable dt = new DataTable();
            dt = objcust.BindCustomer(null);
            ViewBag.Customer = dt;

            Sector objsec = new Sector();
            DataTable dt1 = new DataTable();
            dt1 = objsec.getBuildingList(null);
            ViewBag.Building = dt1;

            DataTable dtsec = new DataTable();
            dtsec = objsec.getSectorList(null);
            ViewBag.Sector = dtsec;

            Subscription obj = new Subscription();
            DataTable dtprodRecord = new DataTable();
            // dtprodRecord = obj.getCustomerOrderList(null);
            //dtprodRecord = obj.getCustomerOrderFutureAdmin(null, null, null);
            dtprodRecord = obj.getCustomerOrderFutureAdminnew();
            int userRecords = 0;
            if (dtprodRecord.Rows.Count>0)
            {
                //ViewBag.previousid = dtprodRecord.Rows[0].ItemArray[0].ToString();
                //ViewBag.nextid = dtprodRecord.Rows[dtprodRecord.Rows.Count - 1].ItemArray[0].ToString();
                userRecords = dtprodRecord.Rows.Count;

                ViewBag.previousid = "0";

                ViewBag.nextid = userRecords.ToString();
                ViewBag.startpoint = "1";
                ViewBag.endpoint = userRecords.ToString();
            }
             userRecords = dtprodRecord.Rows.Count;
            ViewBag.Orderdata = dtprodRecord;

            return View();
        }

        [HttpPost]
        public ActionResult CustomerOrderList(FormCollection form, Subscription objsub)
        {
            int userRecords = 0;
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            //if (control.IsView == false)
            //    return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            Customer objcust = new Customer();
            DataTable dt = new DataTable();
            dt = objcust.BindCustomer(null);
            ViewBag.Customer = dt;

            Sector objsec = new Sector();
            DataTable dt1 = new DataTable();
            dt1 = objsec.getBuildingList(null);
            ViewBag.Building = dt1;

            DataTable dtsec = new DataTable();
            dtsec = objsec.getSectorList(null);
            ViewBag.Sector = dtsec;

            Subscription obj = new Subscription();
            DataTable dtprodRecord = new DataTable();
            DataTable dtprodRecordtotal = new DataTable();
            // dtprodRecord = obj.getCustomerOrderList(null);
            if (control.IsAdmin == true)
            {
                //dtprodRecord = obj.getCustomerOrderHistoryAdmin(null, null, null, null);
                string submit = Request["submit"];

                if (submit == "Search")
                {

                    userRecords = 0;


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

                    dtprodRecord = obj.getCustomerOrderFutureAdminnew11(objsub.FromDate, objsub.ToDate);
                    userRecords = dtprodRecord.Rows.Count;
                    ViewBag.startpoint = "1";
                    ViewBag.endpoint = userRecords.ToString();

                }

                if (submit == "Next")
                {
                    userRecords = 0;
                    string previous = objsub.previous1;
                    string next = objsub.next1;
                   // string startid = objsub.next1;
                    if (string.IsNullOrEmpty(previous) || previous == "")
                    {
                        previous = "0";
                    }

                    if (string.IsNullOrEmpty(next) || next == "" || next=="0")
                    {
                        return RedirectToAction("CustomerOrderList", "CustomerOrder");
                        next = "0";
                    }
                    string startid = next;
                    //string previousdate = objsub.Previousdate;
                    //string startdate = objsub.nextdate;
                    string startpointn = objsub.startpoint;
                    string endpointn = objsub.endpoint;
                    int spoint = 0, epoint = 0;


                    dtprodRecord = obj.getCustomerOrderFutureAdminnew1(startid);
                    userRecords = dtprodRecord.Rows.Count;

                    if (userRecords > 0)
                    {


                        spoint = Convert.ToInt32(startpointn) + 50;
                        epoint = Convert.ToInt32(endpointn) + userRecords;


                        ViewBag.startpoint = spoint.ToString();
                        ViewBag.endpoint = epoint.ToString();
                        //ViewBag.previousid = dtprodRecord.Rows[0].ItemArray[0].ToString();
                        //ViewBag.nextid = dtprodRecord.Rows[dtprodRecord.Rows.Count - 1].ItemArray[0].ToString();

                        ViewBag.previousid = next.ToString();
                        ViewBag.nextid = Convert.ToInt32(next) + userRecords;

                        //ViewBag.Previousdate = dtprodRecord.Rows[0].ItemArray[2].ToString();
                        //ViewBag.nextdate = dtprodRecord.Rows[dtprodRecord.Rows.Count - 1].ItemArray[2].ToString();


                    }

                    else
                    {

                        dtprodRecord = obj.getCustomerOrderFutureAdminnew1dup(previous);
                        ViewBag.startpoint = startpointn.ToString();
                        ViewBag.endpoint = endpointn.ToString();
                        ViewBag.previousid = previous.ToString();
                        ViewBag.nextid = next.ToString();
                    }

                }

                if (submit == "Previous")
                {
                    userRecords = 0;
                    string previous = objsub.previous1;
                    string next = objsub.next1;

                    string startpointn = objsub.startpoint;
                    string endpointn = objsub.endpoint;
                    int spoint = 0, epoint = 0;
                    int previous1 = Convert.ToInt32(previous) - 50;
                    if (string.IsNullOrEmpty(previous) || previous == "" || previous=="0")
                    {

                        previous = "0";
                        previous1 = 0;
                        return RedirectToAction("CustomerOrderList", "CustomerOrder");
                    }

                    if (string.IsNullOrEmpty(next) || next == "")
                    {
                        next = "0";
                    }
                   
                    string previous2 = previous1.ToString();

                    dtprodRecord = obj.getCustomerOrderFutureAdminnew2(previous2);
                    userRecords = dtprodRecord.Rows.Count;

                    if (userRecords > 0)
                    {
                        if (Convert.ToInt32(startpointn) > 50)
                        {
                            spoint = Convert.ToInt32(startpointn) - 50;
                            epoint = Convert.ToInt32(endpointn) - 50;
                        }
                        ViewBag.startpoint = spoint.ToString();
                        ViewBag.endpoint = epoint.ToString();
                        ViewBag.previousid = previous2.ToString();
                        ViewBag.nextid = Convert.ToInt32(previous2) + userRecords; ;
                    }

                    else
                    {
                        dtprodRecord = obj.getCustomerOrderFutureAdminnew();
                        userRecords = dtprodRecord.Rows.Count;
                        ViewBag.previousid = "0";
                        ViewBag.nextid = userRecords.ToString();
                        userRecords = dtprodRecord.Rows.Count;
                        ViewBag.startpoint = "1";
                        ViewBag.endpoint = userRecords.ToString();
                    }

                }
                //dtprodRecord = obj.getCustomerOrderHistoryAdminnew();
                if (submit == "First")
                {
                    dtprodRecord = obj.getCustomerOrderFutureAdminnew();
                    userRecords = dtprodRecord.Rows.Count;
                    ViewBag.previousid = "0";
                    ViewBag.nextid = userRecords.ToString();
                    userRecords = dtprodRecord.Rows.Count;
                    ViewBag.startpoint = "1";
                    ViewBag.endpoint = userRecords.ToString();
                }

                if (submit == "Last")
                {
                    string previous = objsub.previous1;
                    string next = objsub.next1;
                    userRecords = 0;
                    string startid = objsub.next1;
                    string startpointn = objsub.startpoint;
                    string endpointn = objsub.endpoint;
                    int spoint = 0, epoint = 0;
                    dtprodRecordtotal = obj.getCustomerOrderFutureAdmintotal();

                    string totrecord1 = dtprodRecordtotal.Rows[0].ItemArray[0].ToString();

                    dtprodRecord = obj.getCustomerOrderFutureAdminnew3();
                    userRecords = dtprodRecord.Rows.Count;

                    if (userRecords > 0)
                    {
                        if (userRecords >= 50)
                        {
                            spoint = Convert.ToInt32(totrecord1) - 49;
                        }
                        else
                        {
                            spoint = Convert.ToInt32(startpointn);
                        }
                        epoint = Convert.ToInt32(totrecord1);
                        ViewBag.startpoint = spoint.ToString();
                        ViewBag.endpoint = epoint.ToString();

                        int previous1 = Convert.ToInt32(totrecord1) - 50;

                        ViewBag.previousid = previous1.ToString();
                        ViewBag.nextid = totrecord1;
                    }

                    else
                    {
                        ViewBag.startpoint = startpointn.ToString();
                        ViewBag.endpoint = endpointn.ToString();


                        ViewBag.previousid = previous.ToString();
                        ViewBag.nextid = next.ToString();
                    }


                }
            }
            else
            {
                var userID = Helper.CurrentLoginUser();
                dtprodRecord = obj.getCustomerOrderHistoryAdmin(null, Convert.ToInt32(userID), null, null);
            }
            userRecords = dtprodRecord.Rows.Count;
            ViewBag.Orderdata = dtprodRecord;


            return View();
        }
        [HttpGet]
        public ActionResult CustomerHistoryOrderList()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                //if (control.IsView == false)
                //    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                Customer objcust = new Customer();
                DataTable dt = new DataTable();
                dt = objcust.BindCustomer(null);
                ViewBag.Customer = dt;

                Sector objsec = new Sector();
                DataTable dt1 = new DataTable();
                dt1 = objsec.getBuildingList(null);
                ViewBag.Building = dt1;

                DataTable dtsec = new DataTable();
                dtsec = objsec.getSectorList(null);
                ViewBag.Sector = dtsec;

                Subscription obj = new Subscription();
                DataTable dtprodRecord = new DataTable();
                // dtprodRecord = obj.getCustomerOrderList(null);
                int userRecords = 0;
                if (control.IsAdmin == true)
                {
                    //dtprodRecord = obj.getCustomerOrderHistoryAdmin(null, null, null, null);
                    dtprodRecord = obj.getCustomerOrderHistoryAdminnew();

                    userRecords = dtprodRecord.Rows.Count;
                    ViewBag.previousid = "0";
                   
                   ViewBag.nextid = userRecords.ToString();
                    
                    //ViewBag.nextid = dtprodRecord.Rows[dtprodRecord.Rows.Count - 1].ItemArray[0].ToString();

                    ViewBag.Previousdate = dtprodRecord.Rows[0].ItemArray[2].ToString();
                    ViewBag.nextdate = dtprodRecord.Rows[dtprodRecord.Rows.Count - 1].ItemArray[2].ToString();
                    userRecords = dtprodRecord.Rows.Count;
                    ViewBag.startpoint = "1";
                    ViewBag.endpoint = userRecords.ToString();
                }
                else
                {
                    var userID = Helper.CurrentLoginUser();
                    dtprodRecord = obj.getCustomerOrderHistoryAdmin(null, Convert.ToInt32(userID), null, null);
                }
                 
                ViewBag.Orderdata = dtprodRecord;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult CustomerHistoryOrderList(FormCollection form, Subscription objsub)
        {
            int userRecords = 0,userRecordsnew=0;
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            //if (control.IsView == false)
            //    return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            Customer objcust = new Customer();
            DataTable dt = new DataTable();
            dt = objcust.BindCustomer(null);
            ViewBag.Customer = dt;

            Sector objsec = new Sector();
            DataTable dt1 = new DataTable();
            dt1 = objsec.getBuildingList(null);
            ViewBag.Building = dt1;

            DataTable dtsec = new DataTable();
            dtsec = objsec.getSectorList(null);
            ViewBag.Sector = dtsec;

            Subscription obj = new Subscription();
            DataTable dtprodRecord = new DataTable();
            DataTable dtprodRecordtotal = new DataTable();
            DataTable dtprodRecordnew = new DataTable();
            // dtprodRecord = obj.getCustomerOrderList(null);
            if (control.IsAdmin == true)
            {
                //dtprodRecord = obj.getCustomerOrderHistoryAdmin(null, null, null, null);
                string submit = Request["submit"];

                if(submit== "Search")
                {

                    userRecords = 0;


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

                    dtprodRecord = obj.getCustomerOrderHistoryAdminnew11(objsub.FromDate, objsub.ToDate);
                    userRecords = dtprodRecord.Rows.Count;
                    ViewBag.startpoint = "1";
                    ViewBag.endpoint = userRecords.ToString();

                }

                if(submit=="Next")
                {
                    userRecords = 0;
                    string previous = objsub.previous1;


                    string next = objsub.next1;

                    if(string.IsNullOrEmpty(previous) || previous=="")
                    {
                        previous = "0";
                    }

                    if (string.IsNullOrEmpty(next) || next == "" || next=="0")
                    {
                        next = "0";
                        return RedirectToAction("CustomerHistoryOrderList", "CustomerOrder");
                    }
                    string startid = next;
                    string previousdate = objsub.Previousdate;
                    string startdate = objsub.nextdate;
                    string startpointn = objsub.startpoint;
                    string endpointn = objsub.endpoint;
                    int spoint = 0, epoint = 0;
                   
                    
                    dtprodRecord = obj.getCustomerOrderHistoryAdminnew1(next);
                    userRecords = dtprodRecord.Rows.Count;

                    if(userRecords>0)
                    {
                       

                        spoint = Convert.ToInt32(startpointn) + 50;
                        epoint = Convert.ToInt32(endpointn) + userRecords;


                        ViewBag.startpoint = spoint.ToString();
                        ViewBag.endpoint = epoint.ToString();
                        //ViewBag.previousid = dtprodRecord.Rows[0].ItemArray[0].ToString();
                        //ViewBag.nextid = dtprodRecord.Rows[dtprodRecord.Rows.Count - 1].ItemArray[0].ToString();

                        ViewBag.previousid = next.ToString();
                        ViewBag.nextid = Convert.ToInt32(next)+userRecords;

                        ViewBag.Previousdate = dtprodRecord.Rows[0].ItemArray[2].ToString();
                        ViewBag.nextdate = dtprodRecord.Rows[dtprodRecord.Rows.Count - 1].ItemArray[2].ToString();


                    }
                   
                    else
                    {

                        dtprodRecord = obj.getCustomerOrderHistoryAdminnew1dup(previous);
                        ViewBag.startpoint = startpointn.ToString();
                        ViewBag.endpoint = endpointn.ToString();
                        ViewBag.previousid = previous.ToString();
                        ViewBag.nextid = next.ToString();
                    }
                   
                }

                if (submit == "Previous")
                {
                    userRecords = 0;
                    string previous = objsub.previous1;
                    string next = objsub.next1;
                    int previous1 = Convert.ToInt32(previous) - 50;
                    if (string.IsNullOrEmpty(previous) || previous == "" || previous=="0")
                    {
                        return RedirectToAction("CustomerHistoryOrderList", "CustomerOrder");
                        previous = "0";
                        previous1 = 0;
                    }

                    

                    if (string.IsNullOrEmpty(next) || next == "")
                    {
                        next = "0";
                    }
                    string startid = next;
                    string startpointn = objsub.startpoint;
                    string endpointn = objsub.endpoint;
                    int spoint = 0, epoint = 0;

                    
                    string previous2 = previous1.ToString();
                    dtprodRecord = obj.getCustomerOrderHistoryAdminnew2(previous2);
                    userRecords = dtprodRecord.Rows.Count;

                    if (userRecords > 0)
                    {
                        if (Convert.ToInt32(startpointn) > 50)
                        {
                            spoint = Convert.ToInt32(startpointn) - 50;
                            epoint = Convert.ToInt32(endpointn) - 50;
                        }
                        ViewBag.startpoint = spoint.ToString();
                        ViewBag.endpoint = epoint.ToString();
                        ViewBag.previousid = previous2.ToString();
                        ViewBag.nextid = Convert.ToInt32(previous2) + userRecords; ;
                    }

                    else
                    {
                        dtprodRecord = obj.getCustomerOrderHistoryAdminnew();
                        userRecords = dtprodRecord.Rows.Count;
                        ViewBag.previousid = "0";
                        ViewBag.nextid = userRecords.ToString();
                        userRecords = dtprodRecord.Rows.Count;
                        ViewBag.startpoint = "1";
                        ViewBag.endpoint = userRecords.ToString();
                    }

                }
                //dtprodRecord = obj.getCustomerOrderHistoryAdminnew();
                if (submit == "First")
                {
                    dtprodRecord = obj.getCustomerOrderHistoryAdminnew();
                    userRecords = dtprodRecord.Rows.Count;
                    ViewBag.previousid = "0";
                    ViewBag.nextid = userRecords.ToString();
                    userRecords = dtprodRecord.Rows.Count;
                    ViewBag.startpoint = "1";
                    ViewBag.endpoint = userRecords.ToString();
                }

                if (submit == "Last")
                {
                    string previous = objsub.previous1;
                    string next = objsub.next1;
                    userRecords = 0;
                    string startid = objsub.next1;
                    string startpointn = objsub.startpoint;
                    string endpointn = objsub.endpoint;
                    int spoint = 0, epoint = 0;
                    dtprodRecordtotal = obj.getCustomerOrderHistoryAdminntotal();

                    string totrecord1 = dtprodRecordtotal.Rows[0].ItemArray[0].ToString();

                    dtprodRecord = obj.getCustomerOrderHistoryAdminnew3();
                    userRecords = dtprodRecord.Rows.Count;

                    if (userRecords > 0)
                    {
                        if (userRecords >= 50)
                        {
                            spoint = Convert.ToInt32(totrecord1) - 49;
                        }
                        else
                        {
                            spoint = Convert.ToInt32(startpointn);
                        }
                        epoint = Convert.ToInt32(totrecord1) ;
                        ViewBag.startpoint = spoint.ToString();
                        ViewBag.endpoint = epoint.ToString();

                        int previous1 = Convert.ToInt32(totrecord1) - 50;

                        ViewBag.previousid = previous1.ToString();
                        ViewBag.nextid = totrecord1;
                    }

                    else
                    {
                        ViewBag.startpoint = startpointn.ToString();
                        ViewBag.endpoint = endpointn.ToString();


                        ViewBag.previousid = previous.ToString();
                        ViewBag.nextid = next.ToString();
                    }
                   

                }
            }
            else
            {
                var userID = Helper.CurrentLoginUser();
                dtprodRecord = obj.getCustomerOrderHistoryAdmin(null, Convert.ToInt32(userID), null, null);
            }
             userRecords = dtprodRecord.Rows.Count;
            ViewBag.Orderdata = dtprodRecord;


            return View();
        }
        //delete
        [HttpGet]
        public ActionResult DeleteCustomerOrder(int id)
        {
            try
            {
                //check order pending or not
                DataTable dt = objsub.getCustomerOrderList(id.ToString());
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Status"].ToString()))
                    {
                        if (dt.Rows[0]["Status"].ToString() == "Pending")
                        {
                            int delresult = objsub.DeleteCustomerOrder(id);
                        }
                        else
                        { }
                    }
                }
                else
                { }
                if (Request.RawUrl.Contains("CustomerHistoryOrderList"))
                    return RedirectToAction("CustomerHistoryOrderList");
                else
                    return RedirectToAction("CustomerOrderList");
            }
            //catch (System.Data.SqlClient.SqlException ex)
            //{
            //    if (ex.Message.ToLower().Contains("fk_staff_staffcustassign"))
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
            return RedirectToAction("CustomerOrderList");
        }

        //order edit
        [HttpGet]
        public ActionResult EditCustomerOrder(int id = 0)
        {
           // if (Request.Cookies["gstusr"] == null)
               // return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            Customer objorder = new Customer();
            DataTable dt = new DataTable();
            dt = objorder.GetAllCustomer(null);
            ViewBag.Customer = dt;

            Product objProduct = new Product();
            DataTable dt1 = new DataTable();
            dt1 = objProduct.BindProuct(null);
            ViewBag.Product = dt1;

            DataTable dtedit = new DataTable();
            dtedit = objsub.getCustomerOrderSelect(id.ToString());
            if (dtedit.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["VendorCatId"].ToString()))
                    ViewBag.VendorCatId = dtedit.Rows[0]["VendorCatId"].ToString();
                else
                    ViewBag.VendorCatId = "0";


                if (!string.IsNullOrEmpty(dtedit.Rows[0]["CustomerId"].ToString()))
                    ViewBag.CustomerId = dtedit.Rows[0]["CustomerId"].ToString();
                else
                    ViewBag.CustomerId = "0";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["OrderNo"].ToString()))
                    ViewBag.OrderNo = dtedit.Rows[0]["OrderNo"].ToString();
                else
                    ViewBag.OrderNo = "";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["OrderDate"].ToString()))
                {
                    ViewBag.OrderDate = dtedit.Rows[0]["OrderDate"].ToString();
                    DateTime dateFromString =
                          DateTime.Parse(ViewBag.OrderDate, System.Globalization.CultureInfo.InvariantCulture);
                    ViewBag.OrderDate = dateFromString.ToString("dd/MM/yyyy");
                }
                else
                {
                    ViewBag.OrderDate = null;
                }
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["Status"].ToString()))
                    ViewBag.Status = dtedit.Rows[0]["Status"].ToString();
                else
                    ViewBag.Status = "";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["ProductId"].ToString()))
                    ViewBag.ProductId = dtedit.Rows[0]["ProductId"].ToString();
                else
                    ViewBag.ProductId = "0";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["Qty"].ToString()))
                    ViewBag.Qty = dtedit.Rows[0]["Qty"].ToString();
                else
                    ViewBag.Qty = "0";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["Amount"].ToString()))
                    ViewBag.SalePrice = dtedit.Rows[0]["Amount"].ToString();
                else
                    ViewBag.SalePrice = "0";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["Discount"].ToString()))
                    ViewBag.Discount = dtedit.Rows[0]["Discount"].ToString();
                else
                    ViewBag.Discount = "0";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["RewardPoint"].ToString()))
                    ViewBag.RewardPoint = dtedit.Rows[0]["RewardPoint"].ToString();
                else
                    ViewBag.RewardPoint = "0";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["CGSTAmount"].ToString()))
                    ViewBag.CGSTAmount = dtedit.Rows[0]["CGSTAmount"].ToString();
                else
                    ViewBag.CGSTAmount = "0";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["SGSTAmount"].ToString()))
                    ViewBag.SGSTAmount = dtedit.Rows[0]["SGSTAmount"].ToString();
                else
                    ViewBag.SGSTAmount = "0";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["IGSTAmount"].ToString()))
                    ViewBag.IGSTAmount = dtedit.Rows[0]["IGSTAmount"].ToString();
                else
                    ViewBag.IGSTAmount = "0";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["Profit"].ToString()))
                    ViewBag.Profit = dtedit.Rows[0]["Profit"].ToString();
                else
                    ViewBag.Profit = "0";



                if (!string.IsNullOrEmpty(dtedit.Rows[0]["AttributeId"].ToString()))
                    ViewBag.AttributeId = dtedit.Rows[0]["AttributeId"].ToString();
                else
                    ViewBag.AttributeId = "0";
                //get MRP Price
                DataTable dtProduct = objProduct.BindProuct(Convert.ToInt32(ViewBag.ProductId));
                if (dtProduct.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Price"].ToString()))
                        ViewBag.MRPPrice = Convert.ToDecimal(dtProduct.Rows[0]["Price"]);
                    else
                        ViewBag.MRPPrice = 0;
                }

                dtedit = objsub.getCustomerOrderwalletSelect(id.ToString());
                if(dtedit.Rows.Count>0)
                {
                    ViewBag.TransactionDate = dtedit.Rows[0]["TransactionDate"].ToString();
                    DateTime dateFromString =
                          DateTime.Parse(ViewBag.TransactionDate, System.Globalization.CultureInfo.InvariantCulture);
                    ViewBag.TransactionDate = dateFromString.ToString("dd/MM/yyyy");
                }
                else
                {
                    ViewBag.TransactionDate = "";
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult EditCustomerOrder(FormCollection form, Subscription objsub)
        {
            //if (Request.Cookies["gstusr"] == null)
            //  return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            string Proname = "";
            Product objProduct = new Product();
            string customer = Request["ddlCustomer"];
            if (!string.IsNullOrEmpty(customer))
                objsub.CustomerId = Convert.ToInt32(customer);

            string product = Request["ddlProduct"];
            if (!string.IsNullOrEmpty(product))
                objsub.ProductId = Convert.ToInt32(product);

            string status = Request["ddlStatus"];
            if (!string.IsNullOrEmpty(status))
                objsub.Status = status.ToString();

            var fdate = Request["OrderDate"];
            if (!string.IsNullOrEmpty(fdate.ToString()))
            {
                objsub.OrderDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
            }

            var TransactionDate = Request["TransactionDate"];
            if (!string.IsNullOrEmpty(TransactionDate.ToString()))
            {
                objsub.TransactionDate = Convert.ToDateTime(DateTime.ParseExact(TransactionDate, @"dd/MM/yyyy", null));
            }
            //if (objsub.Qty == 0)
            //{ }
            //else
            //{ objsub.Status = "Pending"; }
            //int UpdateOrder = 0;

            DataTable dtProduct = objProduct.BindProuct(Convert.ToInt32(objsub.ProductId));
            if (dtProduct.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Price"].ToString()))
                    Proname = dtProduct.Rows[0]["ProductName"].ToString();
                else
                    Proname = "";
            }

            int UpdateOrder = objsub.UpdateCustomerOrderMain(objsub);
            //update item order
            objsub.OrderId = objsub.Id;
            int UpdateAddProductDetail = objsub.UpdateCustomerOrderDetailMobile(objsub);
            int j = 0;
            if (status == "Complete")
            {
                var i = objsub.CheckCustomerWalletEntry(objsub.OrderId, objsub.CustomerId);
                if (i == 0)
                {
                    //objsub.Description = "Place Order";
                    //objsub.Type = "Debit";

                   
                    objsub.Type = "Debit";
                   
                    objsub.Status = "Purchase";
                    objsub.CustSubscriptionId = 0;
                    objsub.TransactionType = Convert.ToInt32(Helper.TransactionType.Purchase);
                    objsub.Description = Proname;
                    objsub.proqty = objsub.Qty.ToString();
                    //objsub.InsertWallet(objsub);
                    j = objsub.InsertWalletScheduleOrder1(objsub);
                }
                else
                {
                    j=objsub.UpdateCustomerWallet1(objsub);
                }
            }
            else if (status != "Complete")
            {
                var i = objsub.CheckCustomerWalletEntry(objsub.OrderId, objsub.CustomerId);
                objsub.DeleteCustomerWallet(i);
            }

            if (UpdateAddProductDetail > 0 || UpdateOrder > 0)
            {
                ViewBag.SuccessMsg = "Order Updated Successfully!!!";
            }
            else
            { ViewBag.SuccessMsg = "Order not Updated !!!"; }
            Customer objorder = new Customer();
            DataTable dt = new DataTable();
            dt = objorder.GetAllCustomer(null);
            ViewBag.Customer = dt;

           
            DataTable dt1 = new DataTable();
            dt1 = objProduct.BindProuct(null);
            ViewBag.Product = dt1;

            DataTable dtedit = new DataTable();
            dtedit = objsub.getCustomerOrderSelect(objsub.Id.ToString());
            if (dtedit.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["CustomerId"].ToString()))
                    ViewBag.CustomerId = dtedit.Rows[0]["CustomerId"].ToString();
                else
                    ViewBag.CustomerId = "0";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["OrderNo"].ToString()))
                    ViewBag.OrderNo = dtedit.Rows[0]["OrderNo"].ToString();
                else
                    ViewBag.OrderNo = "";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["OrderDate"].ToString()))
                {
                    ViewBag.OrderDate = dtedit.Rows[0]["OrderDate"].ToString();
                    DateTime dateFromString =
                          DateTime.Parse(ViewBag.OrderDate, System.Globalization.CultureInfo.InvariantCulture);
                    ViewBag.OrderDate = dateFromString.ToString("dd/MM/yyyy");
                }
                else
                {
                    ViewBag.OrderDate = null;
                }
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["Status"].ToString()))
                    ViewBag.Status = dtedit.Rows[0]["Status"].ToString();
                else
                    ViewBag.Status = "";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["ProductId"].ToString()))
                    ViewBag.ProductId = dtedit.Rows[0]["ProductId"].ToString();
                else
                    ViewBag.ProductId = "0";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["Qty"].ToString()))
                    ViewBag.Qty = dtedit.Rows[0]["Qty"].ToString();
                else
                    ViewBag.Qty = "0";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["Amount"].ToString()))
                    ViewBag.SalePrice = dtedit.Rows[0]["Amount"].ToString();
                else
                    ViewBag.SalePrice = "0";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["Discount"].ToString()))
                    ViewBag.Discount = dtedit.Rows[0]["Discount"].ToString();
                else
                    ViewBag.Discount = "0";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["RewardPoint"].ToString()))
                    ViewBag.RewardPoint = dtedit.Rows[0]["RewardPoint"].ToString();
                else
                    ViewBag.RewardPoint = "0";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["CGSTAmount"].ToString()))
                    ViewBag.CGSTAmount = dtedit.Rows[0]["CGSTAmount"].ToString();
                else
                    ViewBag.CGSTAmount = "0";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["SGSTAmount"].ToString()))
                    ViewBag.SGSTAmount = dtedit.Rows[0]["SGSTAmount"].ToString();
                else
                    ViewBag.SGSTAmount = "0";
                if (!string.IsNullOrEmpty(dtedit.Rows[0]["Profit"].ToString()))
                    ViewBag.Profit = dtedit.Rows[0]["Profit"].ToString();
                else
                    ViewBag.Profit = "0";
            }

            dtedit = objsub.getCustomerOrderwalletSelect(objsub.Id.ToString());
            if (dtedit.Rows.Count > 0)
            {
                ViewBag.TransactionDate = dtedit.Rows[0]["TransactionDate"].ToString();
                DateTime dateFromString =
                      DateTime.Parse(ViewBag.TransactionDate, System.Globalization.CultureInfo.InvariantCulture);
                ViewBag.TransactionDate = dateFromString.ToString("dd/MM/yyyy");
            }
            else
            {
                ViewBag.TransactionDate = "";
            }

            return View();
        }

        //multiple delete
        [HttpGet]
        public ActionResult OrderMultipleDelete()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                Customer objcust = new Customer();
                DataTable dt = new DataTable();
                dt = objcust.BindCustomer(null);
                ViewBag.Customer = dt;

                Sector objsec = new Sector();
                DataTable dt1 = new DataTable();
                dt1 = objsec.getBuildingList(null);
                ViewBag.Building = dt1;

                DataTable dtsec = new DataTable();
                dtsec = objsec.getSectorList(null);
                ViewBag.Sector = dtsec;

                Subscription obj = new Subscription();
                DataTable dtprodRecord = new DataTable();
                dtprodRecord = obj.getCustomerOrderFutureAdmin(null, null, null);
                int userRecords = dtprodRecord.Rows.Count;
                ViewBag.Orderdata = dtprodRecord;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }



        [HttpGet]
        public ActionResult CustomerWiseOrder()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                Customer objcust = new Customer();
                DataTable dt = new DataTable();
                dt = objcust.BindCustomer(null);
                ViewBag.Customer = dt;

                Sector objsec = new Sector();
                DataTable dt1 = new DataTable();
                dt1 = objsec.getBuildingList(null);
                ViewBag.Building = dt1;

                DataTable dtsec = new DataTable();
                dtsec = objsec.getSectorList(null);
                ViewBag.Sector = dtsec;

                Subscription obj = new Subscription();
                DataTable dtprodRecord = new DataTable();
                dtprodRecord = obj.getCustomerOrderHis(obj.CustomerId, null, null);
                int userRecords = dtprodRecord.Rows.Count;
                ViewBag.Orderdata = dtprodRecord;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }



        [HttpPost]
        public ActionResult CustomerWiseOrder(FormCollection form, Subscription obj)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                //string sector = Request["ddlSector"];
                //if (!string.IsNullOrEmpty(sector))
                //{
                //    obj.SectorId = Convert.ToInt32(sector);
                //}
                string building = Request["ddlBuilding"];
                if (!string.IsNullOrEmpty(building))
                {
                    obj.BuildingId = Convert.ToInt32(building);
                }
                string customer = Request["ddlCustomer"];
                if (!string.IsNullOrEmpty(customer))
                {
                    obj.CustomerId = Convert.ToInt32(customer);
                }
                DateTime? FromDate = null;
                DateTime? ToDate = null;

                DataTable dtprodRecord = new DataTable();
                dtprodRecord = obj.getCustomerOrderHis(obj.CustomerId, FromDate, ToDate);
                int userRecords = dtprodRecord.Rows.Count;
                ViewBag.Orderdata = dtprodRecord;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult GetProductData(string id, int qty,int Atid,int Vid)
        {
            Subscription obj = new Subscription();
            Product objproduct = new Product();

            DataTable dt = new DataTable();
            dt.Columns.Add("MRPPrice", typeof(decimal));
            dt.Columns.Add("SalePrice", typeof(decimal));
            dt.Columns.Add("DiscountPrice", typeof(decimal));
            dt.Columns.Add("CGSTPrice", typeof(decimal));
            dt.Columns.Add("SGSTPrice", typeof(decimal));
            dt.Columns.Add("IGSTPrice", typeof(decimal));
            dt.Columns.Add("Reward", typeof(decimal));
            dt.Columns.Add("Profit", typeof(decimal));
            dt.Columns.Add("PurchasePrice", typeof(decimal));
            obj.Qty = Convert.ToInt32(qty);

            obj.ProductId = Convert.ToInt32(id);
            //get Product detail
            //DataTable dtProduct = objproduct.BindProuct(obj.ProductId);

            DataTable dtProduct = objproduct.BindProuctAttributewise(obj.ProductId,Atid,Vid);
            if (dtProduct.Rows.Count > 0)
            {
                string searchExpression = "ProductName <> ''";
                DataRow[] foundRows = dtProduct.Select(searchExpression);
                if (foundRows.Length > 0)
                {
                    foreach (DataRow dr in foundRows)
                    {
                        DataRow dtr = dt.NewRow();
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Id"].ToString()))
                            id = dtProduct.Rows[0]["Id"].ToString();
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["MRPPrice"].ToString()))
                            dtr["MRPPrice"] = Convert.ToDecimal(dtProduct.Rows[0]["MRPPrice"]) * obj.Qty;
                        else
                            dtr["MRPPrice"] = 0;
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SellPrice"].ToString()))
                            dtr["SalePrice"] = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]) * obj.Qty;
                        else
                            dtr["SalePrice"] = 0;
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountPrice"].ToString()))
                            dtr["DiscountPrice"] = Convert.ToDecimal(dtProduct.Rows[0]["DiscountPrice"]) * obj.Qty;
                        else
                            dtr["DiscountPrice"] = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                            obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                        else
                            obj.CGSTPerct = 0;
                        //count cgst Amount
                        if (obj.CGSTPerct > 0)
                            dtr["CGSTPrice"] = (Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]) * obj.CGSTPerct) / 100;
                        else
                            dtr["CGSTPrice"] = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                            obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                        else
                            obj.IGSTPerct = 0;
                        //count igst Amount
                        if (obj.IGSTPerct > 0)
                            dtr["IGSTPrice"] = (Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]) * obj.IGSTPerct) / 100;
                        else
                            dtr["IGSTPrice"] = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                            obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                        else
                            obj.SGSTPerct = 0;
                        //count sgst Amount
                        if (obj.SGSTPerct > 0)
                            dtr["SGSTPrice"] = (Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]) * obj.SGSTPerct) / 100;//DiscountPrice
                        else
                            dtr["SGSTPrice"] = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                            dtr["Reward"] = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                        else
                            dtr["Reward"] = 0;
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                            dtr["Profit"] = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                        else
                            dtr["Profit"] = 0;
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["PurchasePrice"].ToString()))
                            dtr["PurchasePrice"] = Convert.ToDecimal(dtProduct.Rows[0]["PurchasePrice"]) * obj.Qty;
                        else
                            dtr["PurchasePrice"] = 0;
                        //Final Amount
                        //  dr["DiscountPrice"] = obj.Amount;
                        dt.Rows.Add(dtr);
                    }
                }
            }

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
        }

        //Order Schedule
        [HttpGet]
        public ActionResult OrderSchedule1()
        {
            //if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            //{
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            int AddWallet = 0;
            DataTable dtDateOrderSchel = new DataTable();
            DataTable dtGetCustomerPoint = new DataTable();
            dtDateOrderSchel = obj.getCustomerOrderScheduleList(null, DateTime.Now.AddDays(1));
            if (dtDateOrderSchel.Rows.Count > 0)
            {
                int TotalRewardPoint = 0;
                //find order amount deduct from wallet
                for (int i = 0; i < dtDateOrderSchel.Rows.Count; i++)
                {
                    string pname = "", proqty = "0";
                    obj.CustomerId = Convert.ToInt32(dtDateOrderSchel.Rows[i]["CustomerId"]);
                    obj.TransactionDate = DateTime.Now.AddDays(1);
                    obj.Amount = Convert.ToDecimal(dtDateOrderSchel.Rows[i]["Amount"]);
                    obj.OrderId = Convert.ToInt32(dtDateOrderSchel.Rows[i]["OId"]);
                    //obj.BillNo = DBNull.Value;
                    pname = dtDateOrderSchel.Rows[i]["ProductName"].ToString();
                    obj.proqty = dtDateOrderSchel.Rows[i]["Qty"].ToString();
                    obj.Description = pname;
                    obj.Status = "Purchase";
                    obj.Type = "Debit";
                    obj.CustSubscriptionId = 0;
                    obj.RewardPoint = Convert.ToInt64(dtDateOrderSchel.Rows[i]["RewardPoint"]);

                    //check subscription expired
                    DateTime SubTo = DateTime.Now;
                    DataTable Checkuserexits = obj.CheckCustSubnExits(obj.CustomerId, null, null, null);
                    if (Checkuserexits.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(Checkuserexits.Rows[0]["ToDate"].ToString()))
                            SubTo = Convert.ToDateTime(Checkuserexits.Rows[0]["ToDate"]);
                    }
                    if (!string.IsNullOrEmpty(SubTo.ToString()))
                    {
                        TimeSpan DiffDays = SubTo - obj.TransactionDate;
                        if (DiffDays.Days <= 5)
                        {
                            //notification for subscription expired
                            //string title = "Subscription Expired";
                            //string content = "Dear Milkyway Family Member, Your subscription get expired very soon so renew your subscription for seamless delivery service...";
                            //string type = "Notification";//PRODUCT   NEWS_INFO  ORDER
                            //string obj_id = "1";
                            //string image = "";
                            //int appnotification = AppNotification(obj.CustomerId, title, content, type, obj_id, image);
                        }
                    }

                    //chcek wallet balance
                    decimal Walletbal = 0, TotalCredit = 0, TotalDebit = 0, Last2days = 0;
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
                    Last2days = obj.Amount + obj.Amount;
                    if (Walletbal < Last2days)
                    {
                        //notification for low balance
                        string title = "Low balance";
                        string content = "Dear Milkyway Family member, your wallet balance is low kindly upload balance for seamless delivery service- Milkyway accounts dept.";
                        string type = "Notification";//PRODUCT   NEWS_INFO  ORDER
                        string obj_id = "1";
                        string image = "";
                        int appnotification = AppNotification(obj.CustomerId, title, content, type, obj_id, image);
                        //// SendSMS(obj.CustomerId.ToString());
                    }
                    if (Walletbal < obj.Amount)
                    {
                        //notification for low balance
                        string title = "Low balance";
                        string content = "Dear Milkyway Family member, your wallet balance is low kindly upload balance for seamless delivery service- Milkyway accounts dept.";
                        string type = "Notification";//PRODUCT   NEWS_INFO  ORDER
                        string obj_id = "1";
                        string image = "";
                        int appnotification = AppNotification(obj.CustomerId, title, content, type, obj_id, image);

                        var date = obj.TransactionDate.Date;
                        //order status 
                        int UpdateOrderStatus = obj.UpdateCustomerOrderCancle(obj.OrderId, obj.CustomerId, Convert.ToDateTime(date), "InComplete");
                    }
                    else
                    {
                        //check dupliacte records
                        //debit from wallet
                        AddWallet = obj.InsertWallet(obj);
                        //    AddWallet = 1;
                        if (AddWallet > 0)
                        {
                            var date = obj.TransactionDate.Date;
                            //order status 
                            int UpdateOrderStatus = obj.UpdateCustomerOrderCancle(obj.OrderId, obj.CustomerId, Convert.ToDateTime(date), "Complete");
                            // int UpdateOrderStatus = 0;
                            //add Rewards Point to Customer table
                            dtGetCustomerPoint = objcust.BindCustomer(obj.CustomerId);
                            if (dtGetCustomerPoint.Rows.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(dtGetCustomerPoint.Rows[0]["RewardPoint"].ToString()))
                                    TotalRewardPoint = Convert.ToInt32(dtGetCustomerPoint.Rows[0]["RewardPoint"]);
                                obj.RewardPoint = obj.RewardPoint + TotalRewardPoint;
                                int UpdateCustomer = objcust.UpdateCustomerPoint(obj.CustomerId, Convert.ToInt64(obj.RewardPoint));
                            }
                            if (UpdateOrderStatus > 0)
                            {
                                //notification
                                //string title = "Order Confirmed";
                                //string content = "Dear Customer, Your Order is confirmed.Thank you for Purchase Order with MilkyWay India! ";
                                //string type = "Notification";//PRODUCT   NEWS_INFO  ORDER
                                //string obj_id = "1";
                                //string image = "";
                                //int appnotification = AppNotification(obj.CustomerId, title, content, type, obj_id, image);
                            }
                        }
                    }
                }
            }

            if (AddWallet > 0)
            {
                ViewBag.SuccessMsg = "Schedular Inserted Successfully!!!";
            }
            else
            { ViewBag.SuccessMsg = "Schedular Not Inserted !!!"; }

            return View();
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Home");
            //}
        }



        [HttpGet]
        public ActionResult OrderSchedule()
        {
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            int AddWallet = 0;
            DataTable dtCustomerOrderGroup = new DataTable();
            DataTable dtDateOrderSchel = new DataTable();
            DataTable dtGetCustomerPoint = new DataTable();
            DateTime orderDate = indianTime.AddDays(0);
            Helper dHelper = new Helper();
            dHelper.ScheduleOrder(orderDate);
            //dtCustomerOrderGroup = obj.getCustomerOrderScheduleGroupList(orderDate);
            //if (dtCustomerOrderGroup.Rows.Count > 0)
            //{
            //    for (int i = 0; i < dtCustomerOrderGroup.Rows.Count; i++)
            //    {
            //        obj.CustomerId = Convert.ToInt32(dtCustomerOrderGroup.Rows[i]["CustomerId"]);
            //        obj.Amount = Convert.ToDecimal(dtCustomerOrderGroup.Rows[i]["Amount"]);
            //        obj.TransactionDate = orderDate;

            //        dtDateOrderSchel = obj.getCustomerOrderScheduleList(obj.CustomerId, orderDate);
            //        if (dtDateOrderSchel.Rows.Count > 0)
            //        {
            //            //check subscription expired
            //            DateTime SubTo = indianTime;
            //            // DateTime SubTo = DateTime.Now;
            //            DataTable Checkuserexits = obj.CheckCustSubnExits(obj.CustomerId, null, null, null);
            //            if (Checkuserexits.Rows.Count > 0)
            //            {
            //                if (!string.IsNullOrEmpty(Checkuserexits.Rows[0]["ToDate"].ToString()))
            //                    SubTo = Convert.ToDateTime(Checkuserexits.Rows[0]["ToDate"]);
            //            }
            //            if (!string.IsNullOrEmpty(SubTo.ToString()))
            //            {
            //                TimeSpan DiffDays = SubTo - obj.TransactionDate;
            //                if (DiffDays.Days <= 5)
            //                {
            //                    //notification for subscription expired
            //                    //string title = "Subscription Expired";
            //                    //string content = "Dear Milkyway Family Member, Your subscription get expired very soon so renew your subscription for seamless delivery service...";
            //                    //string type = "Notification";//PRODUCT   NEWS_INFO  ORDER
            //                    //string obj_id = "1";
            //                    //string image = "";
            //                    //int appnotification = AppNotification(obj.CustomerId, title, content, type, obj_id, image);
            //                }
            //            }
            //            //chcek wallet balance
            //            decimal Walletbal = 0, TotalCredit = 0, TotalDebit = 0, Last2days = 0; bool AllowOrderWallet = false;
            //            DataTable dtprodRecord = new DataTable();
            //            dtprodRecord = obj.getCustomerWallet(obj.CustomerId);
            //            int userRecords = dtprodRecord.Rows.Count;
            //            if (userRecords > 0)
            //            {
            //                if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["Amt"].ToString()))
            //                    TotalCredit = Convert.ToDecimal(dtprodRecord.Rows[0]["Amt"]);
            //                if (userRecords > 1)
            //                {
            //                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[1]["Amt"].ToString()))
            //                        TotalDebit = Convert.ToDecimal(dtprodRecord.Rows[1]["Amt"]);
            //                }
            //                Walletbal = TotalCredit - TotalDebit;
            //            }
            //            Last2days = obj.Amount + obj.Amount;
            //            if (Walletbal < Last2days)
            //            {
            //                //notification for low balance
            //                //string title = "Low balance";
            //                //string content = "Dear Milkyway Family member, your wallet balance is low kindly upload balance for seamless delivery service- Milkyway accounts dept.";
            //                //string type = "Notification";//PRODUCT   NEWS_INFO  ORDER
            //                //string obj_id = "1";
            //                //string image = "";
            //                //int appnotification = AppNotification(obj.CustomerId, title, content, type, obj_id, image);
            //            }
            //            if (Walletbal < obj.Amount)
            //            {
            //                AllowOrderWallet = true;
            //            }
            //            else { AllowOrderWallet = true; }
            //            int TotalRewardPoint = 0;
            //            int UpdateOrderStatus = 0, UpdateOrderStatusCancle = 0;
            //            //find order amount deduct from wallet
            //            for (int j = 0; j < dtDateOrderSchel.Rows.Count; j++)
            //            {
            //                obj.CustomerId = Convert.ToInt32(dtDateOrderSchel.Rows[j]["CustomerId"]);
            //                //  obj.TransactionDate = DateTime.Now.AddDays(1);
            //                obj.TransactionDate = indianTime.AddDays(1);
            //                obj.Amount = Convert.ToDecimal(dtDateOrderSchel.Rows[j]["Amount"]);
            //                obj.OrderId = Convert.ToInt32(dtDateOrderSchel.Rows[j]["OId"]);
            //                obj.Description = "Place Order";
            //                obj.Type = "Debit";
            //                obj.CustSubscriptionId = 0;
            //                obj.RewardPoint = Convert.ToInt64(dtDateOrderSchel.Rows[j]["RewardPoint"]);

            //                if (AllowOrderWallet == false)
            //                {
            //                    var date = obj.TransactionDate.Date;
            //                    //order status 
            //                    UpdateOrderStatusCancle = obj.UpdateCustomerOrderCancle(obj.OrderId, obj.CustomerId, Convert.ToDateTime(date), "InComplete");
            //                }
            //                else
            //                {
            //                    //check dupliacte records
            //                    //debit from wallet
            //                    AddWallet = obj.InsertWallet(obj);
            //                    // AddWallet = 1;
            //                    if (AddWallet > 0)
            //                    {
            //                        var date = obj.TransactionDate.Date;
            //                        //order status 
            //                        UpdateOrderStatus = obj.UpdateCustomerOrderCancle(obj.OrderId, obj.CustomerId, Convert.ToDateTime(date), "Complete");
            //                        // UpdateOrderStatus = 1;
            //                        //add Rewards Point to Customer table
            //                        dtGetCustomerPoint = objcust.BindCustomer(obj.CustomerId);
            //                        if (dtGetCustomerPoint.Rows.Count > 0)
            //                        {
            //                            if (!string.IsNullOrEmpty(dtGetCustomerPoint.Rows[0]["RewardPoint"].ToString()))
            //                                TotalRewardPoint = Convert.ToInt32(dtGetCustomerPoint.Rows[0]["RewardPoint"]);
            //                            obj.RewardPoint = obj.RewardPoint + TotalRewardPoint;
            //                            int UpdateCustomer = objcust.UpdateCustomerPoint(obj.CustomerId, Convert.ToInt64(obj.RewardPoint));
            //                        }

            //                    }
            //                }
            //            }
            //            if (UpdateOrderStatus > 0)
            //            {
            //                //notification
            //                //string title = "Order Confirmed";
            //                //string content = "Dear Customer, Your Order is confirmed.Thank you for Purchase Order with MilkyWay India! ";
            //                //string type = "Notification";//PRODUCT   NEWS_INFO  ORDER
            //                //string obj_id = "1";
            //                //string image = "";
            //                //int appnotification = AppNotification(obj.CustomerId, title, content, type, obj_id, image);
            //            }
            //        }
            //    }
            //}
            if (AddWallet > 0)
            {
                ViewBag.SuccessMsg = "Schedular Inserted Successfully!!!";
            }
            else
            { ViewBag.SuccessMsg = "Schedular Not Inserted !!!"; }
            //Sector objsector = new Sector();
            //int addresult = objsector.InsertSector1(objsector, "test", "394550");
            //if (addresult > 0)
            //{
            //    ViewBag.SuccessMsg = "Schedular Inserted Successfully!!!";
            //}
            //else
            //{ ViewBag.SuccessMsg = "Schedular Not Inserted !!!"; }

            return View();
        }

        public ActionResult ForwardDailyOrder()
        {
            Helper dHelper = new Helper();
            CustomerOrder _order = new CustomerOrder();
            string CurrentDate = Helper.indianTime.ToString("yyyy-MM-dd");
            string NextDate = Helper.indianTime.AddDays(30).ToString("yyyy-MM-dd");
            clsCommon _clsCommon = new clsCommon();
            var dt = _clsCommon.showdata(@"SELECT * from tbl_Cusomter_Order_Track Where convert(varchar, NextOrderDate, 23) > '" + CurrentDate + "' AND convert(varchar, NextOrderDate, 23) < '" + NextDate + "' And OrderFlag='Daily'");

            //var dt = _clsCommon.showdata(@"SELECT * from tbl_Cusomter_Order_Track Where CustomerID=391 And ProductID=11 And OrderFlag='Alternate'");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    Boolean CheckOrder = false;
                    int Id = Convert.ToInt32(dt.Rows[i]["Id"]);
                    int CustomerId = Convert.ToInt32(dt.Rows[i]["CustomerId"]);
                    int ProductId = Convert.ToInt32(dt.Rows[i]["ProductId"]);
                    int AttributeId = Convert.ToInt32(dt.Rows[i]["AttributeId"]);
                    decimal Qty = Convert.ToInt32(dt.Rows[i]["Qty"]);
                    DateTime NextOrderDate = Convert.ToDateTime(dt.Rows[i]["NextOrderDate"]);
                    DateTime StartDate = NextOrderDate.AddDays(1);
                    string ToDate = StartDate.AddMonths(1).ToString("yyyy-MM-dd");
                    
                    string OrderDate2 = Helper.indianTime.AddDays(-2).ToString("yyyy-MM-dd");
                    string OrderDate1 = Helper.indianTime.AddDays(-1).ToString("yyyy-MM-dd");
                    var dtOrder1 = _order.CheckCustomerOrderByDate(CustomerId, ProductId, OrderDate1);
                    if (dtOrder1.Rows.Count > 0)
                        CheckOrder = true;
                    var dtOrder2 = _order.CheckCustomerOrderByDate(CustomerId, ProductId, OrderDate2);
                    if (dtOrder2.Rows.Count > 0)
                        CheckOrder = true;
                    
                    if (CheckOrder == true)
                    {
                        var order = dHelper.AddCustomerDailyOrder(CustomerId.ToString(), ProductId.ToString(), Qty.ToString(), StartDate, AttributeId.ToString(), NextOrderDate.ToString());
                        if (order > 0)
                        {
                            string s = "update tbl_Cusomter_Order_Track set NextOrderDate='" + ToDate + "' where id=" + Id + "";
                            _clsCommon.update(s);
                        }
                    }
            }
                catch { }
        }
            return View();
        }


        public ActionResult ForwardAlternateOrder()
        {
            Helper dHelper = new Helper();
            CustomerOrder _order = new CustomerOrder();
            string CurrentDate = Helper.indianTime.ToString("yyyy-MM-dd");
            string NextDate = Helper.indianTime.AddDays(30).ToString("yyyy-MM-dd");
            clsCommon _clsCommon = new clsCommon();
            var dt = _clsCommon.showdata(@"SELECT * from tbl_Cusomter_Order_Track Where convert(varchar, NextOrderDate, 23) > '" + CurrentDate + "' AND convert(varchar, NextOrderDate, 23) < '" + NextDate + "' And OrderFlag='Alternate'");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    Boolean CheckOrder = false;
                    int Id = Convert.ToInt32(dt.Rows[i]["Id"]);
                    int CustomerId = Convert.ToInt32(dt.Rows[i]["CustomerId"]);
                    int ProductId = Convert.ToInt32(dt.Rows[i]["ProductId"]);
                    int AttributeId = Convert.ToInt32(dt.Rows[i]["AttributeId"]);
                    decimal Qty = Convert.ToInt32(dt.Rows[i]["Qty"]);
                    DateTime NextOrderDate = Convert.ToDateTime(dt.Rows[i]["NextOrderDate"]);
                    DateTime StartDate = NextOrderDate.AddDays(2);
                    DateTime ToDate1 = StartDate.AddMonths(1);
                    string ToDate = StartDate.AddMonths(1).ToString("yyyy-MM-dd");

                    //string OrderDate2 = NextOrderDate.AddDays(-2).ToString("yyyy-MM-dd");
                    //string OrderDate1 = NextOrderDate.ToString("yyyy-MM-dd");

                    string OrderDate2 = Helper.indianTime.AddDays(-2).ToString("yyyy-MM-dd");
                    string OrderDate1 = Helper.indianTime.AddDays(-1).ToString("yyyy-MM-dd");
                    var dtOrder1 = _order.CheckCustomerOrderByDate(CustomerId, ProductId, OrderDate1);
                    if (dtOrder1.Rows.Count > 0)
                        CheckOrder = true;
                    var dtOrder2 = _order.CheckCustomerOrderByDate(CustomerId, ProductId, OrderDate2);
                    if (dtOrder2.Rows.Count > 0)
                        CheckOrder = true;




                    if (CheckOrder == true)
                    {
                        var order = dHelper.AddCustomerAlternateOrder(CustomerId.ToString(), ProductId.ToString(), Qty.ToString(), StartDate, AttributeId.ToString(), NextOrderDate.ToString());
                        if (order > 0)
                        {

                            for (int idate = 0; StartDate <= ToDate1; idate++)
                            {
                              StartDate= StartDate.AddDays(2);
                            }
                            string ToDate2 = StartDate.AddDays(-2).ToString("yyyy-MM-dd");
                            string s = "update tbl_Cusomter_Order_Track set NextOrderDate='" + ToDate2 + "' where id=" + Id + "";
                            _clsCommon.update(s);
                        }
                    }
                }
                catch { }
            }
            return View();
        }



        public ActionResult ForwardWeeklyOrder()
        {
            Helper dHelper = new Helper();
            CustomerOrder _order = new CustomerOrder();
            string CurrentDate = Helper.indianTime.ToString("yyyy-MM-dd");
            string NextDate = Helper.indianTime.AddDays(30).ToString("yyyy-MM-dd");
            clsCommon _clsCommon = new clsCommon();
            var dt = _clsCommon.showdata(@"SELECT * from tbl_Cusomter_Order_Track Where convert(varchar, NextOrderDate, 23) > '" + CurrentDate + "' AND convert(varchar, NextOrderDate, 23) < '" + NextDate + "' And OrderFlag='WeekDay'");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    Boolean CheckOrder = false;
                    int Id = Convert.ToInt32(dt.Rows[i]["Id"]);
                    int CustomerId = Convert.ToInt32(dt.Rows[i]["CustomerId"]);
                    int ProductId = Convert.ToInt32(dt.Rows[i]["ProductId"]);
                    int AttributeId = Convert.ToInt32(dt.Rows[i]["AttributeId"]);
                    decimal Qty = Convert.ToInt32(dt.Rows[i]["Qty"]);
                    DateTime NextOrderDate = Convert.ToDateTime(dt.Rows[i]["NextOrderDate"]);
                    DateTime StartDate = NextOrderDate.AddDays(7);
                    DateTime ToDate1 = StartDate.AddMonths(1);
                    string ToDate = StartDate.AddMonths(1).ToString("yyyy-MM-dd");

                    //string OrderDate2 = NextOrderDate.AddDays(-7).ToString("yyyy-MM-dd");
                    //string OrderDate1 = NextOrderDate.ToString("yyyy-MM-dd");

                    string OrderDate2 = Helper.indianTime.AddDays(-2).ToString("yyyy-MM-dd");
                    string OrderDate1 = Helper.indianTime.AddDays(-1).ToString("yyyy-MM-dd");
                    var dtOrder1 = _order.CheckCustomerOrderByDate(CustomerId, ProductId, OrderDate1);
                    if (dtOrder1.Rows.Count > 0)
                        CheckOrder = true;
                    var dtOrder2 = _order.CheckCustomerOrderByDate(CustomerId, ProductId, OrderDate2);
                    if (dtOrder2.Rows.Count > 0)
                        CheckOrder = true;




                    if (CheckOrder == true)
                    {
                        var order = dHelper.AddCustomerWeekOrder(CustomerId.ToString(), ProductId.ToString(), Qty.ToString(), StartDate, AttributeId.ToString(), NextOrderDate.ToString());
                        if (order > 0)
                        {

                            for (int idate = 0; StartDate <= ToDate1; idate++)
                            {
                                StartDate = StartDate.AddDays(7);
                            }
                            string ToDate2 = StartDate.AddDays(-7).ToString("yyyy-MM-dd");
                            string s = "update tbl_Cusomter_Order_Track set NextOrderDate='" + ToDate2 + "' where id=" + Id + "";
                            _clsCommon.update(s);
                        }
                    }
                }
                catch { }
            }
            return View();
        }

        //public ActionResult ForwardDailyOrder_old()
        //{
        //    Helper dHelper = new Helper();
        //    CustomerOrder _order = new CustomerOrder();
        //    string CurrentDate = Helper.indianTime.ToString("yyyy-MM-dd");
        //    clsCommon _clsCommon = new clsCommon();
        //    var dt = _clsCommon.showdata(@"SELECT * from tbl_Cusomter_Order_Track Where convert(varchar, NextOrderDate, 23) = '" + CurrentDate + "'");
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        try
        //        {
        //            Boolean CheckOrder = false;
        //            int Id = Convert.ToInt32(dt.Rows[i]["Id"]);
        //            int CustomerId = Convert.ToInt32(dt.Rows[i]["CustomerId"]);
        //            int ProductId = Convert.ToInt32(dt.Rows[i]["ProductId"]);
        //            decimal Qty = Convert.ToInt32(dt.Rows[i]["Qty"]);
        //            string OrderDate2 = Helper.indianTime.AddDays(-2).ToString("yyyy-MM-dd");
        //            string OrderDate1 = Helper.indianTime.AddDays(-1).ToString("yyyy-MM-dd");
        //            string ToDate = Helper.indianTime.AddMonths(1).ToString("yyyy-MM-dd");

        //            var dtOrder1 = _order.CheckCustomerOrderByDate(CustomerId, ProductId, OrderDate1);
        //            if (dtOrder1.Rows.Count > 0)
        //                CheckOrder = true;
        //            var dtOrder2 = _order.CheckCustomerOrderByDate(CustomerId, ProductId, OrderDate2);
        //            if (dtOrder2.Rows.Count > 0)
        //                CheckOrder = true;
        //            if (CheckOrder == true)
        //            {
        //                var order = dHelper.AddCustomerDailyOrder(CustomerId.ToString(), ProductId.ToString(), Qty.ToString(), Helper.indianTime.AddDays(1));
        //                if (order > 0)
        //                {
        //                    string s = "update tbl_Cusomter_Order_Track set NextOrderDate='" + ToDate + "' where id=" + Id + "";
        //                    _clsCommon.update(s);
        //                }
        //            }
        //        }
        //        catch { }
        //    }
        //    return View();
        //}

        [HttpGet]
        public ActionResult Test_OrderSchedule()
        {
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            int AddWallet = 0;
            DataTable dtCustomerOrderGroup = new DataTable();
            DataTable dtDateOrderSchel = new DataTable();
            DataTable dtGetCustomerPoint = new DataTable();
            DateTime orderDate = indianTime.AddDays(1);
            dtCustomerOrderGroup = obj.TestgetCustomerOrderScheduleGroupList();
            if (dtCustomerOrderGroup.Rows.Count > 0)
            {
                for (int i = 0; i < dtCustomerOrderGroup.Rows.Count; i++)
                {
                    obj.CustomerId = Convert.ToInt32(dtCustomerOrderGroup.Rows[i]["CustomerId"]);
                    obj.Amount = Convert.ToDecimal(dtCustomerOrderGroup.Rows[i]["Amount"]);
                    obj.TransactionDate = indianTime.AddDays(1);

                    dtDateOrderSchel = obj.getCustomerOrderScheduleList(obj.CustomerId, orderDate);
                    if (dtDateOrderSchel.Rows.Count > 0)
                    {
                        //check subscription expired
                        DateTime SubTo = indianTime;
                        // DateTime SubTo = DateTime.Now;
                        DataTable Checkuserexits = obj.CheckCustSubnExits(obj.CustomerId, null, null, null);
                        if (Checkuserexits.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(Checkuserexits.Rows[0]["ToDate"].ToString()))
                                SubTo = Convert.ToDateTime(Checkuserexits.Rows[0]["ToDate"]);
                        }
                        if (!string.IsNullOrEmpty(SubTo.ToString()))
                        {
                            TimeSpan DiffDays = SubTo - obj.TransactionDate;
                            if (DiffDays.Days <= 5)
                            {
                                //notification for subscription expired
                                //string title = "Subscription Expired";
                                //string content = "Dear Milkyway Family Member, Your subscription get expired very soon so renew your subscription for seamless delivery service...";
                                //string type = "Notification";//PRODUCT   NEWS_INFO  ORDER
                                //string obj_id = "1";
                                //string image = "";
                                //int appnotification = AppNotification(obj.CustomerId, title, content, type, obj_id, image);
                            }
                        }
                        //chcek wallet balance
                        decimal Walletbal = 0, TotalCredit = 0, TotalDebit = 0, Last2days = 0; bool AllowOrderWallet = false;
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
                        Last2days = obj.Amount + obj.Amount;
                        if (Walletbal < Last2days)
                        {
                            //notification for low balance
                            string title = "Low balance";
                            string content = "Dear Milkyway Family member, your wallet balance is low kindly upload balance for seamless delivery service- Milkyway accounts dept.";
                            string type = "Notification";//PRODUCT   NEWS_INFO  ORDER
                            string obj_id = "1";
                            string image = "";
                            int appnotification = AppNotification(obj.CustomerId, title, content, type, obj_id, image);
                        }
                        if (Walletbal < obj.Amount)
                        {
                            AllowOrderWallet = true;
                        }
                        else { AllowOrderWallet = true; }
                        int TotalRewardPoint = 0;
                        int UpdateOrderStatus = 0, UpdateOrderStatusCancle = 0;
                        //find order amount deduct from wallet
                        for (int j = 0; j < dtDateOrderSchel.Rows.Count; j++)
                        {
                            obj.CustomerId = Convert.ToInt32(dtDateOrderSchel.Rows[j]["CustomerId"]);
                            //  obj.TransactionDate = DateTime.Now.AddDays(1);
                            obj.TransactionDate = indianTime.AddDays(1);
                            obj.Amount = Convert.ToDecimal(dtDateOrderSchel.Rows[j]["Amount"]);
                            obj.OrderId = Convert.ToInt32(dtDateOrderSchel.Rows[j]["OId"]);
                            obj.Description = "Place Order";
                            obj.Type = "Debit";
                            obj.CustSubscriptionId = 0;
                            obj.RewardPoint = Convert.ToInt64(dtDateOrderSchel.Rows[j]["RewardPoint"]);

                            if (AllowOrderWallet == false)
                            {
                                var date = obj.TransactionDate.Date;
                                //order status 
                                UpdateOrderStatusCancle = obj.UpdateCustomerOrderCancle(obj.OrderId, obj.CustomerId, Convert.ToDateTime(date), "InComplete");
                            }
                            else
                            {
                                //check dupliacte records
                                //debit from wallet
                                AddWallet = obj.InsertWallet(obj);
                                // AddWallet = 1;
                                if (AddWallet > 0)
                                {
                                    var date = obj.TransactionDate.Date;
                                    //order status 
                                    UpdateOrderStatus = obj.UpdateCustomerOrderCancle(obj.OrderId, obj.CustomerId, Convert.ToDateTime(date), "Complete");
                                    // UpdateOrderStatus = 1;
                                    //add Rewards Point to Customer table
                                    dtGetCustomerPoint = objcust.BindCustomer(obj.CustomerId);
                                    if (dtGetCustomerPoint.Rows.Count > 0)
                                    {
                                        if (!string.IsNullOrEmpty(dtGetCustomerPoint.Rows[0]["RewardPoint"].ToString()))
                                            TotalRewardPoint = Convert.ToInt32(dtGetCustomerPoint.Rows[0]["RewardPoint"]);
                                        obj.RewardPoint = obj.RewardPoint + TotalRewardPoint;
                                        int UpdateCustomer = objcust.UpdateCustomerPoint(obj.CustomerId, Convert.ToInt64(obj.RewardPoint));
                                    }

                                }
                            }
                        }
                        if (UpdateOrderStatus > 0)
                        {
                            //notification
                            //string title = "Order Confirmed";
                            //string content = "Dear Customer, Your Order is confirmed.Thank you for Purchase Order with MilkyWay India! ";
                            //string type = "Notification";//PRODUCT   NEWS_INFO  ORDER
                            //string obj_id = "1";
                            //string image = "";
                            //int appnotification = AppNotification(obj.CustomerId, title, content, type, obj_id, image);
                        }
                    }
                }
            }
            if (AddWallet > 0)
            {
                ViewBag.SuccessMsg = "Schedular Inserted Successfully!!!";
            }
            else
            { ViewBag.SuccessMsg = "Schedular Not Inserted !!!"; }
            //Sector objsector = new Sector();
            //int addresult = objsector.InsertSector1(objsector, "test", "394550");
            //if (addresult > 0)
            //{
            //    ViewBag.SuccessMsg = "Schedular Inserted Successfully!!!";
            //}
            //else
            //{ ViewBag.SuccessMsg = "Schedular Not Inserted !!!"; }

            return View();
        }

        private void SendSMS(string id)
        {
            string mobileNo = "";
            Customer objcust = new Customer();
            DataTable dtmobileno = new DataTable();
            dtmobileno = objcust.BindCustomer(Convert.ToInt32(id));
            if (dtmobileno.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dtmobileno.Rows[0]["MobileNo"].ToString()))
                    mobileNo = dtmobileno.Rows[0]["MobileNo"].ToString();
            }

            if ((mobileNo != "" && mobileNo != null))
            {
                //string Msg = "Hello, " + Name + ". Your OTP is:" + otp + " For Registartion.";
                string Msg = "Dear Milkyway Family member, your wallet balance is low kindly upload balance for seamless delivery service- Milkyway accounts dept.";

                string strUrl = "";
                //india sms
                ////strUrl = "https://apps.vibgyortel.in/client/api/sendmessage?apikey=dca6c57e6c6f4638&mobiles=" + mobileNo + "&sms=" + Msg + "&senderid=Aruhat";
                strUrl = "http://trans.magicsms.co.in/api/v4/?api_key=Ac02aa3f338e616196e3fbacad1f76e06&method=sms&message=" + Msg + "&to=" + mobileNo + "&sender=MLKYwy";
                // Create a request object  
                WebRequest request = HttpWebRequest.Create(strUrl);
                // Get the response back  
                try
                {
                    HttpWebResponse responsesms = (HttpWebResponse)request.GetResponse();
                    Stream s = (Stream)responsesms.GetResponseStream();
                    StreamReader readStream = new StreamReader(s);
                    string dataString = readStream.ReadToEnd();
                    responsesms.Close();
                    s.Close();
                    readStream.Close();
                }
                catch (Exception ex)
                {
                }
            }
        }

        //customer subscription date
        [HttpPost]
        public ActionResult GetSubscriptionDate(string id)
        {
            Customer obj = new Customer();
            Product objproduct = new Product();
            Subscription objsub = new Subscription();
            int SectorId = 0;
            DataTable dt = new DataTable();
            dt = obj.BindCustomer(Convert.ToInt32(id));
            if (dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dt.Rows[0]["SectorId"].ToString()))
                    SectorId = Convert.ToInt32(dt.Rows[0]["SectorId"]);
            }
            else
            {

            }

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
            //get customer sectorId
            //chcek wallet amount
            decimal Walletbal = 0;
            var balance = objsub.GetCustomerBalace(Convert.ToInt32(id));
            Walletbal = balance;

            //customer sector wise product and category
            DataTable dtcateg = new DataTable();
            dtcateg = objproduct.BindSectorCategory(SectorId);

            DataTable dtproduct = new DataTable();
            dtproduct = objproduct.BindSectorProuct(SectorId);

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(rows);

            string jsonString1 = string.Empty;
            jsonString1 = JsonConvert.SerializeObject(dtcateg);

            string jsonString2 = string.Empty;
            jsonString2 = JsonConvert.SerializeObject(dtproduct);

            string jsonString3 = string.Empty;
            jsonString3 = JsonConvert.SerializeObject(Walletbal);

            return Json(new { jsonString, jsonString1, jsonString2, jsonString3 }, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult CustomerOrderHistory(int Id)
        {
            Subscription obj = new Subscription();
            DateTime toDate = DateTime.Now.AddMonths(3);
            DataTable dtprodRecord = obj.getCustomerOrderHistoryAdmin(Id, null, Helper.indianTime.AddDays(-1), toDate);
            ViewBag.Orderdata = dtprodRecord;
            return PartialView("_CustomerOrderHistory");
        }

        public int AppNotification(Int64 UserId, string notificationtitle, string notificationcontent, string notificationtype, string notificationobj_id, string notificationimage)
        {
            Customer objLogin = new Customer();
            DataTable dtToken = new DataTable();
            if (UserId == 0)
            {

            }
            else
            {
                dtToken = objLogin.getDeviceInstanceId(UserId);
            }
            if (dtToken.Rows.Count > 0)
            {

                string str = "";

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                //serverKey - Key from Firebase cloud messaging server  
                tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAP1kVwxg:APA91bEGnOhfhaWJqwwBtW0uzn3nF-mSbJxDtjggcMRts5mith8ArpqpnW57HEhO3yKpohZZZs7PPF1LCsYMlioCFXzyFt5nRxeTCgPk-zlrX-YfQps6yCn1Z9bdVAFK7HnCja_S3Nsp"));
                //Sender Id - From firebase project setting  
                tRequest.Headers.Add(string.Format("Sender: id={0}", "272077538072"));
                tRequest.ContentType = "application/json";
                var payload = new
                {
                    to = dtToken.Rows[0]["fcm_token"].ToString(),
                    ////to = "caMqaVHNrDQ:APA91bGCO5RKdaJYZNyCqSlcgRaQr4_K5PZ2UlRWTDsHV25u2y_Wwvq6J11IaVUgrVVcoFOSsuEfiHic5mpa9W2hcbN7NeinWmD5IbZY5qKG6stQN6Y1QSC2eMCZySokEln-s-iXy50D",
                    //// to = "cM2O8S-69e0:APA91bEpBJZhPu9amYDGY2ZBqVA0ubB9D-TYVmsSHkxiJetthLzHzvfToVbDz53aGS_w5qiXsd-g6C3wCFSQefxhISf-DX3HhL4XyIrMrG7lfCT1uQdxhTSOEG5DSSgeOKQP0bwhnJJS",
                    priority = "high",
                    content_available = true,
                    notification = new
                    {
                        body = notificationcontent,
                        title = notificationtitle,
                        badge = 1
                    },
                    data = new
                    {
                        click_action = "FLUTTER_NOTIFICATION_CLICK",
                        body = notificationcontent,
                        title = notificationtitle
                    }
                };

                string postbody = JsonConvert.SerializeObject(payload).ToString();
                Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
                                    //result.Response = sResponseFromServer;
                                }
                        }
                    }
                }
            }
            return 1;
        }

        [HttpGet]
        public ActionResult DeliveryBoyDailyOrder()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            CustomerOrder objorder = new CustomerOrder();
            DataTable dt = new DataTable();
            dt = objcust.GetAllCustomer(null);
            ViewBag.Customer = dt;

            Staff objStaff = new Staff();
            DataTable dtStaff = new DataTable();
            dtStaff = objStaff.getDeliveryBoyList(null);
            ViewBag.Staff = dtStaff;

            DataTable dtList = new DataTable();
            dtList = objorder.getDeliveryBoyCustomerOrder(null, null, null, null, null);
            ViewBag.ProductorderList = dtList;

            return View();
        }

        public ActionResult TestingReport()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DeliveryBoyDailyOrdervendor()
        {
            if (Request.Cookies["gstusr"] == null)
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
            dtList = objorder.getDeliveryBoyCustomerOrder(null, null, null, null, null);
            ViewBag.ProductorderList = dtList;

            return View();
        }
        [HttpPost]
        public ActionResult DeliveryBoyDailyOrder(FormCollection form, CustomerOrder objorder)
        {
            if (Request.Cookies["gstusr"] == null)
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
            dtList = objorder.getDeliveryBoyCustomerOrder(objorder.StaffId, objorder.CustomerId, objorder.FromDate, objorder.ToDate, objorder.Status);
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
                return Redirect("/Report/DeliveryBoyDailyReport?" + query);
            }
            else if (submit == "export")
            {
                string query = string.Format("DeliveryboyId={0}&CustomerId={1}&FDate={2}&TDate={3}&status={4}",
                    objorder.StaffId, objorder.CustomerId, _fdate, _tdate, objorder.Status);
                //return new UrlAsPdf("/customerorder/DeliveryBoyDailyReport?" + query);
                var r = new PartialViewAsPdf("DeliveryBoyDailyReport", new
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



        [HttpPost]
        public ActionResult DeliveryBoyDailyOrdervendor(FormCollection form, CustomerOrder objorder)
        {
            if (Request.Cookies["gstusr"] == null)
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
                return Redirect("/Report/DeliveryBoyDailyReportvendor1?" + query);
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

        public ActionResult DeliveryBoyDailyReport(int? DeliveryboyId, int? CustomerId, string FDate, string TDate, string status)
        {
            DateTime _fdate = DateTime.Now, _tdate = DateTime.Now;
            if (!string.IsNullOrEmpty(FDate.ToString()))
                _fdate = Convert.ToDateTime(DateTime.ParseExact(FDate, @"dd-MM-yyyy", null));
            if (!string.IsNullOrEmpty(TDate.ToString()))
                _tdate = Convert.ToDateTime(DateTime.ParseExact(TDate, @"dd-MM-yyyy", null));

            CustomerOrder order = new CustomerOrder();
            ViewBag.FromDate = _fdate.ToString("dd-MMM-yyyy");
            var dtList = order.getDeliveryBoyCustomerOrder(DeliveryboyId, CustomerId, _fdate, _tdate, status);
            ViewBag.ProductorderList = dtList;
            return View();
        }
        //public ActionResult DeliveryBoyDailyReportvendor1(int? DeliveryboyId, int? CustomerId, string FDate, string TDate, string status)
        //{
        //    DateTime _fdate = DateTime.Now, _tdate = DateTime.Now;
        //    if (!string.IsNullOrEmpty(FDate.ToString()))
        //        _fdate = Convert.ToDateTime(DateTime.ParseExact(FDate, @"dd-MM-yyyy", null));
        //    if (!string.IsNullOrEmpty(TDate.ToString()))
        //        _tdate = Convert.ToDateTime(DateTime.ParseExact(TDate, @"dd-MM-yyyy", null));

        //    CustomerOrder order = new CustomerOrder();
        //    ViewBag.FromDate = _fdate.ToString("dd-MMM-yyyy");
        //    var dtList = order.getDeliveryBoyCustomerOrder(DeliveryboyId, CustomerId, _fdate, _tdate, status);
        //    ViewBag.ProductorderList = dtList;
        //    return View();
        //}
        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export(string GridHtml)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                HtmlNode.ElementsFlags["img"] = HtmlElementFlag.Closed;
                HtmlNode.ElementsFlags["input"] = HtmlElementFlag.Closed;
                HtmlDocument doc = new HtmlDocument();
                doc.OptionFixNestedTags = true;
                doc.LoadHtml(GridHtml);
                GridHtml = doc.DocumentNode.OuterHtml;

                Encoding unicode = Encoding.UTF8;
                StringReader sr = new StringReader(GridHtml);
                Document pdfDoc = new Document(PageSize.A4, 5, 5, 5, 5);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();


                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "DeliveryBoyReport.pdf");

                //StringReader sr = new StringReader(GridHtml);
                //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                //PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                //pdfDoc.Open();
                //XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                //pdfDoc.Close();
                //return File(stream.ToArray(), "application/pdf", "DeliveryBoyReport.pdf");
            }
        }

        private void writeText(PdfContentByte cb, string Text, int X, int Y, BaseFont font, int Size)
        {
            cb.SetFontAndSize(font, Size);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, Text, X, Y, 0);
        }

        [HttpPost]
        public ActionResult GetSectorCategWiseProduct(int SectorId, int CategoryId)
        {
            Product obj = new Product();
            DataTable dt = new DataTable();
            dt = obj.BindSectorCategProuct(SectorId, CategoryId);

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

        [HttpPost]
        public ActionResult GetCategWiseProduct(int? CategoryId)
        {
            Product obj = new Product();
            DataTable dt = new DataTable();
            dt = obj.BindCategProuct(CategoryId);

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

        public ActionResult DeliveryBoyDailyOrderPrint(string DeliveryboyId, string CustomerId, string FromDate, string ToDate, string status)
        {
            CustomerOrder objorder = new CustomerOrder();
            if (DeliveryboyId == "0") DeliveryboyId = null;
            if (CustomerId == "0") CustomerId = null;
            if (status == "0") status = null;
            var fdate = FromDate;
            if (!string.IsNullOrEmpty(fdate.ToString()))
            {
                objorder.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
            }
            var tdate = ToDate;
            if (!string.IsNullOrEmpty(tdate.ToString()))
            {
                objorder.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
            }
            DataTable dt = new DataTable();
            Report.StaffOrder obj = new Report.StaffOrder();
            ReportDocument rd = new ReportDocument();
            rd.Load(Server.MapPath("~/Report/StaffOrderFlatWise.rpt"));
            SqlDataAdapter da = new SqlDataAdapter("Sector_Staff_Order_SelectAll", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(DeliveryboyId))
                da.SelectCommand.Parameters.AddWithValue("@DeliveryBoyId", DeliveryboyId);
            else
                da.SelectCommand.Parameters.AddWithValue("@DeliveryBoyId", DBNull.Value);
            if (!string.IsNullOrEmpty(CustomerId))
                da.SelectCommand.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                da.SelectCommand.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(FromDate.ToString()))
                da.SelectCommand.Parameters.AddWithValue("@FromDate", objorder.FromDate);
            else
                da.SelectCommand.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(ToDate.ToString()))
                da.SelectCommand.Parameters.AddWithValue("@ToDate", objorder.ToDate);
            else
                da.SelectCommand.Parameters.AddWithValue("@ToDate", DBNull.Value);
            if (!string.IsNullOrEmpty(status))
                da.SelectCommand.Parameters.AddWithValue("@OrderStatus", status);
            else
                da.SelectCommand.Parameters.AddWithValue("@OrderStatus", DBNull.Value);
            da.Fill(dt);

            rd.Database.Tables[0].SetDataSource(dt);
            rd.SetParameterValue("@DeliveryBoyId", DeliveryboyId);
            rd.SetParameterValue("@CustomerId", CustomerId);
            rd.SetParameterValue("@FromDate", FromDate);
            rd.SetParameterValue("@ToDate", ToDate);
            rd.SetParameterValue("@OrderStatus", status);

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

        [HttpPost]
        public JsonResult DeliveryBoyDailyOrderData(int? DeliveryboyId, int? CustomerId, string FromDate, string ToDate, FormCollection form)
        {
            CustomerOrder objorder = new CustomerOrder();
            if (DeliveryboyId == 0) DeliveryboyId = null;
            if (CustomerId == 0) CustomerId = null;
            var fdate = FromDate;
            if (!string.IsNullOrEmpty(fdate.ToString()))
            {
                objorder.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
            }
            var tdate = ToDate;
            if (!string.IsNullOrEmpty(tdate.ToString()))
            {
                objorder.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
            }
            DataTable dtList = new DataTable();
            dtList = objorder.getDeliveryBoyCustomerOrder(objorder.StaffId, objorder.CustomerId, objorder.FromDate, objorder.ToDate, null);
            ViewBag.ProductorderList = dtList;


            // var rows2= dataSet1.Tables[0];
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dtList.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dtList.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return Json(rows);
        }


        [HttpGet]
        public ActionResult ReplaceOrder()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            Customer objorder = new Customer();
            DataTable dt = new DataTable();
            Vendor objVendor = new Vendor();

            Product objProduct = new Product();
            DataTable dt1 = new DataTable();
            dt1 = objProduct.BindProuct1(null);
            ViewBag.Product = dt1;

            DataTable dtcategory = new DataTable();
            dtcategory = objProduct.BindCategory(null);
            ViewBag.Category = dtcategory;
            dt = objVendor.getVendorList(null);
            ViewBag.VendorList = dt;

            return View();
        }


        [HttpPost]
        public ActionResult ReplaceOrder(Subscription obj,FormCollection frm)
        {
            string msg = "";
            DataTable dt = new DataTable();
            Vendor objVendor = new Vendor();
            Customer objsec = new Customer();
            string submit = Request["submit"];

            obj.ProductId =Convert.ToInt32(Request["ddlProduct"]);
            if(submit== "Search")
            {
               
               
                dt = objsec.GetCustomerPoductWise(obj.ProductId);
                ViewBag.CustomerList = dt;
                ViewBag.ProductId = obj.ProductId;
            }
            if(submit=="Update")
            {
                string proid = Request["txtproid"];
                int updateresult = 0;
                string delimStr = ",";
                char[] delimiter = delimStr.ToCharArray();
                int id = 0;
                objVendor.VendorId = Convert.ToInt32(Request["ddlvendor"]);
                objVendor.VendorCatId = Convert.ToInt32(Request["ddlvendorcat"]);
                objVendor.AttributeId= Convert.ToInt32(Request["ddlattribute"]);
                int newproid =Convert.ToInt32(Request["ddlproduct1"]);
                foreach (string s in proid.Split(delimiter))
                {
                    int OrderBy = 0;
                    id = Convert.ToInt16(s);
                    objVendor.CustomerId = id;
                    objVendor.SectorId =Convert.ToInt32(Request[id + "Sector"]);
                    objVendor.DeliveryBoyId=Convert.ToInt32(Request[id + "Dm"]);
                    objVendor.ProductId=Convert.ToInt32(Request[id + "Pro"]);

                    dt = objsec.GetCustomerOrderPoductWise(id,obj.ProductId);
                    if(dt.Rows.Count>0)
                    {
                        for(int i=0;i<dt.Rows.Count;i++)
                        {
                            objVendor.OrderId = dt.Rows[i]["OrderId"].ToString();
                            updateresult = objVendor.UpdateproductReplacement(objVendor, newproid);
                        }

                    }
                   
                    if (updateresult > 0)
                    {
                        ViewBag.SuccessMsg = "Replacement Successfull";
                        msg = "Product Deleted Successfully";
                        Session["Msg"] = msg;
                    }
                    else
                    {
                        ViewBag.SuccessMsg = "Error Occur";
                    }
                }
            }

            Product objProduct = new Product();
            DataTable dt1 = new DataTable();
            dt1 = objProduct.BindProuct1(null);
            ViewBag.Product = dt1;

            DataTable dtcategory = new DataTable();
            dtcategory = objProduct.BindCategory(null);
            ViewBag.Category = dtcategory;

           dt = objVendor.getVendorList(null);
            ViewBag.VendorList = dt;

            return View();
        }


        [HttpPost]
        public ActionResult GetvendorWisecat(int? VendorId)
        {
            Product obj = new Product();
            Vendor objvendor = new Vendor();
            DataTable dt = new DataTable();
            dt = objvendor.getVendorCat(VendorId);

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


        [HttpPost]
        public ActionResult GetvendorcatWiseProduct(int? VendorCatId)
        {
            Product obj = new Product();
            Vendor objvendor = new Vendor();
            DataTable dt = new DataTable();
            dt = objvendor.getVendorcatProduct(VendorCatId);

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


        [HttpPost]
        public ActionResult GetvendorcatProdWiseAttribute(int? VendorCatId,int? ProdId)
        {
            Product obj = new Product();
            Vendor objvendor = new Vendor();
            DataTable dt = new DataTable();
            dt = objvendor.getVendorcatProductwiseAttribute(VendorCatId,ProdId);

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
        //[HttpGet]





        /* public ActionResult SendNotification()
         {
            // string serverKey = "AAAAHhkm8Z8:APA91bHnaCxvovFdO-ML4LLKxNc_qhdK0FjQtgUhSbW3L9awmVcHyQbrFUMtAAl5LkRlhNRmZ9svlzoTo24BbFQNnH1klFcnioEkTxKU_Nu34tt2ypP3kKXr04g2KLELKilrLzGZDJsE"; // server key
             //notification
             string title = "Order Confirmed";
             string content = "Dear Customer, Your Order is confirmed.Thank you for Purchase Order with MilkyWay India! ";
             string type = "Notification";//PRODUCT   NEWS_INFO  ORDER
             string obj_id = "1";
             string image = "";
            //// int appnotification = AppNotification(52, title, content, type, obj_id, image);
             Customer objLogin = new Customer();
             string arrray = "eRhRbil6EQo:APA91bG1PNi_lScKJ7g-j0ToZk7xsH7U8x02lmB973xcOcyt3jny5dhLtWaTm5TjarIIiTx7u9KSM24qlSJuCU8GqyV8oyVn7mC0XfyBh7DDwa5ZA-QDgOT3SKAUMj753P1czERCQbpg";
             //if (dtToken.Rows.Count > 0)
             //{
             //    string[] arrray = dtToken.Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToArray();                // array of fcmid's
                 try
                 {
                     string applicationID = "AAAAHhkm8Z8:APA91bHnaCxvovFdO-ML4LLKxNc_qhdK0FjQtgUhSbW3L9awmVcHyQbrFUMtAAl5LkRlhNRmZ9svlzoTo24BbFQNnH1klFcnioEkTxKU_Nu34tt2ypP3kKXr04g2KLELKilrLzGZDJsE"; // server key
                     string senderId = "129271001503"; // senderid
                     WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                     tRequest.Method = "post";
                     tRequest.ContentType = "application/json";
                     var data = new
                     {
                         // collapse_key = "demo",
                         // delay_while_idle = true,
                         // content_available = true,
                         priority = "high",
                         click_action = "FLUTTER_NOTIFICATION_CLICK",
                         registration_ids = arrray,   //  arrray               //for send multiple user fcm_id
                                                      // to="eAzOLUH6_Vg:APA91bFeGPrVJqpY71tz7e66iSSAQn2SRHuxJH10QBVE4IGEBk7xXl1_YujhmLLBPI0MOeWdf8qR4oZJT7afzEBxP5mjXYGfhvLqaDxAkbi3D-Z9Y5VLdPSTtYjC2FPmntVcJvsSYb8h";  
                                                      // priority = "high",
                         notification = new
                         {
                             body = content,
                             title = title,
                         }
                         //data = new
                         //{
                         //    title = notificationtitle,
                         //    content = notificationcontent,//"Dear Customer, Your Order is confirmed.Thnks for Shopping with PLP Mega Mall!",
                         //    type = notificationtype, // "ORDER",//PRODUCT   NEWS_INFO
                         //    obj_id = notificationobj_id,// '1',
                         //    image = notificationimage, //"banner2.jpg",
                         //}
                     };
                     var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                     var json = serializer.Serialize(data);
                     Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                     tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                     tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                     tRequest.ContentLength = byteArray.Length;
                     using (Stream dataStream = tRequest.GetRequestStream())
                     {
                         dataStream.Write(byteArray, 0, byteArray.Length);
                         using (WebResponse tResponse = tRequest.GetResponse())
                         {
                             using (Stream dataStreamResponse = tResponse.GetResponseStream())
                             {
                                 using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                 {
                                     String sResponseFromServer = tReader.ReadToEnd();
                                     string str = sResponseFromServer;
                                 }
                             }
                         }
                     }
                 }
                 catch (Exception ex)
                 {
                     string str = ex.Message;
                 }
          //   }
             return View();
             //}
             //else
             //{
             //    return RedirectToAction("Login", "Home");
             //}
         }*/
    }
}
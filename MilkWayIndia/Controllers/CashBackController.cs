using MilkWayIndia.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MilkWayIndia.Controllers
{
    public class CashBackController : Controller
    {

        CashBack objcashback = new CashBack();
        clsCommon _clsCommon = new clsCommon();
        Subscription objsub = new Subscription();
        // DataRow dr = dtNew.NewRow();


        //Payment Source
        [HttpGet]
        public ActionResult AddPaymentSource()
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

            DataTable dtcategory = new DataTable();

            ViewBag.Category = dtcategory;

            return View();
        }
        [HttpPost]
        public ActionResult AddPaymentSourceNew(FormCollection form)
        {
            try
            {
                if (Request.Cookies["gstusr"] == null)
                    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

                string paymentSource = form["PaymentSource"];
                bool isActive = true;
                if (form.AllKeys.Contains("IsActive"))
                    isActive = form["IsActive"]?.Contains("true") == true;

                if (string.IsNullOrWhiteSpace(paymentSource))
                {
                    ViewBag.ErrorMsg = "Payment Source name is required.";
                    return View("AddPaymentSource");
                }

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand(@"
                INSERT INTO tbl_PaymentSourceMaster (PaymentSource, IsActive)
                VALUES (@PaymentSource, @IsActive)", con);

                    com.Parameters.AddWithValue("@PaymentSource", paymentSource.Trim());
                    com.Parameters.AddWithValue("@IsActive", isActive);

                    int i = com.ExecuteNonQuery();
                    con.Close();

                    if (i > 0)
                        HttpContext.Session["Msg"] = "Payment source added successfully.";
                    else
                        ViewBag.ErrorMsg = "Something went wrong while saving.";
                }

                return RedirectToAction("PaymentSourceList");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Error occurred: " + ex.Message;
                return View("AddPaymentSource");
            }
        }
       [HttpGet]
        public ActionResult PaymentSourceList()
        {
            // Check user session
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (!control.IsView)
                return Redirect("/notaccess/index");
            // Set permission flags for view
            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            // Display success message from session (if any)
            ViewBag.SuccessMsg = HttpContext.Session["Msg"]?.ToString() ?? "";
            HttpContext.Session["Msg"] = "";

            // Load payment source list
            DataTable dtPaymentSource = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id, PaymentSource, IsActive FROM tbl_PaymentSourceMaster ORDER BY PaymentSource ASC", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtPaymentSource);
            }

            // Pass data to view
            ViewBag.PaymentSourceList = dtPaymentSource;
            ViewBag.ReturnUrl = Request.Url.ToString();
            ViewBag.IsAttribute = Request.Url.ToString().Contains("portal") || Request.Url.ToString().Contains("localhost");

            return View();
        }
        [HttpGet]
        public ActionResult DeletePaymentSource(int id)
        {
            try
            {
                objcashback.Id = id;
                int delresult = objcashback.DeletePaymentSource(id);

                if (delresult > 0)
                {
                    TempData["SuccessMsg"] = "Payment source deleted successfully.";
                }
                else
                {
                    TempData["ErrorMsg"] = "Payment source not found or already deleted.";
                }

                return RedirectToAction("PaymentSourceList");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_orderdetail_product"))
                {
                    TempData["ErrorMsg"] = "You cannot delete this record because it is referenced by other records.";
                }
                else
                {
                    TempData["ErrorMsg"] = "SQL error: " + ex.Message;
                }
                return RedirectToAction("PaymentSourceList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("PaymentSourceList");
            }
        }

        [HttpGet]
        public ActionResult EditPaymentSource(int id = 0)
        {
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            ViewBag.IsEdit = false;

            if (id > 0)
            {
                DataTable dt = _clsCommon.selectwhere("*", "tbl_PaymentSourceMaster", $"Id = '{id}'");
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];

                    ViewBag.IsEdit = true;
                    ViewBag.Id = Convert.ToInt32(dr["Id"]);
                    ViewBag.PaymentSource = dr["PaymentSource"].ToString();
                    ViewBag.IsActive = true;
                }
                else
                {
                    TempData["ErrorMsg"] = "Payment source not found.";
                    return RedirectToAction("PaymentSourceList");
                }
            }

            return View();
        }
        [HttpPost]
        public ActionResult UpdatePaymentSourceList(CashBack objCash, FormCollection form)
        {
            int i = 0;
            try
            {
                string paymentSource = form["PaymentSource"];
                bool isActive = form["IsActive"]?.Contains("true") == true;
                int id = Convert.ToInt32(form["Id"]);

                if (string.IsNullOrWhiteSpace(paymentSource))
                {
                    ViewBag.ErrorMsg = "Payment Source name is required.";
                    return View("EditPaymentSource");
                }

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString))
                {
                    con.Open();

                    SqlCommand com = new SqlCommand(@"
                UPDATE tbl_PaymentSourceMaster
                SET PaymentSource = @PaymentSource,
                    IsActive = @IsActive
                WHERE Id = @Id", con);

                    com.Parameters.AddWithValue("@PaymentSource", paymentSource.Trim());
                    com.Parameters.AddWithValue("@IsActive", isActive);
                    com.Parameters.AddWithValue("@Id", id);

                    i = com.ExecuteNonQuery();

                    con.Close();
                }

                if (i > 0)
                {
                    TempData["SuccessMsg"] = "Payment source updated successfully.";
                }
                else
                {
                    TempData["ErrorMsg"] = "Update failed. Record not found.";
                }

                return RedirectToAction("PaymentSourceList");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Error occurred: " + ex.Message;
                return View("EditPaymentSource");
            }
        }
        //PaymentSourcewise PlateFormFees
        [HttpGet]
        public ActionResult AddPaymentPlateFormFees()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            DataTable dtPaymentSource = new DataTable();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id, PaymentSource FROM tbl_PaymentSourceMaster WHERE IsActive = 1", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtPaymentSource);
            }

            ViewBag.PaymentSourceList = dtPaymentSource;

            return View();
        }
        [HttpPost]
        public ActionResult AddPaymentPlateFormFeesNew(FormCollection form)
        {
            try
            {
                if (Request.Cookies["gstusr"] == null)
                    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

                int paymentSourceId = Convert.ToInt32(form["PaymentSourceId"]);
                bool isPriceRangeApplicable = form["IsPriceRangeApplicable"]?.Contains("true") == true;

                decimal? fromPrice = string.IsNullOrWhiteSpace(form["FromPrice"]) ? (decimal?)null : Convert.ToDecimal(form["FromPrice"]);
                decimal? toPrice = string.IsNullOrWhiteSpace(form["ToPrice"]) ? (decimal?)null : Convert.ToDecimal(form["ToPrice"]);
                decimal? percentage = string.IsNullOrWhiteSpace(form["Percentage"]) ? (decimal?)null : Convert.ToDecimal(form["Percentage"]);
                decimal? lumsumAmount = string.IsNullOrWhiteSpace(form["LumsumAmount"]) ? (decimal?)null : Convert.ToDecimal(form["LumsumAmount"]);
                decimal? platformChargesPercentage = string.IsNullOrWhiteSpace(form["PlatformChargesPercentage"]) ? (decimal?)null : Convert.ToDecimal(form["PlatformChargesPercentage"]);
                decimal? platformChargesLumsumAmount = string.IsNullOrWhiteSpace(form["PlatformChargesLumsumAmount"]) ? (decimal?)null : Convert.ToDecimal(form["PlatformChargesLumsumAmount"]);
                bool isActive = form.AllKeys.Contains("IsActive") && form["IsActive"]?.Contains("true") == true;

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand(@"
                INSERT INTO tbl_PaymentSourceWisePlatformFees
                (
                    PaymentSourceId,
                    FromPrice,
                    ToPrice,
                    IsPriceRangeApplicable,
                    Percentage,
                    LumsumAmount,
                    PlatformChargesPercentage,
                    PlatformChargesLumsumAmount,
                    IsActive
                )
                VALUES
                (
                    @PaymentSourceId,
                    @FromPrice,
                    @ToPrice,
                    @IsPriceRangeApplicable,
                    @Percentage,
                    @LumsumAmount,
                    @PlatformChargesPercentage,
                    @PlatformChargesLumsumAmount,
                    @IsActive
                )", con);

                    com.Parameters.AddWithValue("@PaymentSourceId", paymentSourceId);
                    com.Parameters.AddWithValue("@FromPrice", (object)fromPrice ?? DBNull.Value);
                    com.Parameters.AddWithValue("@ToPrice", (object)toPrice ?? DBNull.Value);
                    com.Parameters.AddWithValue("@IsPriceRangeApplicable", isPriceRangeApplicable);
                    com.Parameters.AddWithValue("@Percentage", (object)percentage ?? DBNull.Value);
                    com.Parameters.AddWithValue("@LumsumAmount", (object)lumsumAmount ?? DBNull.Value);
                    com.Parameters.AddWithValue("@PlatformChargesPercentage", (object)platformChargesPercentage ?? DBNull.Value);
                    com.Parameters.AddWithValue("@PlatformChargesLumsumAmount", (object)platformChargesLumsumAmount ?? DBNull.Value);
                    com.Parameters.AddWithValue("@IsActive", isActive);

                    int i = com.ExecuteNonQuery();
                    con.Close();

                    if (i > 0)
                        HttpContext.Session["Msg"] = "Platform fee details added successfully.";
                    else
                        ViewBag.ErrorMsg = "Something went wrong while saving.";
                }

                return RedirectToAction("PaymentPlateFormFeesList");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Error occurred: " + ex.Message;
                return View("AddPaymentPlateFormFees");
            }
        }
        [HttpGet]
        public ActionResult PaymentPlateFormFeesList()
        {
            // Check user session
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (!control.IsView)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            // Show success message if available
            ViewBag.SuccessMsg = HttpContext.Session["Msg"]?.ToString() ?? "";
            HttpContext.Session["Msg"] = "";

            // Load combined list with JOIN
            DataTable dtPaymentFees = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
            SELECT 
                PF.Id,
                PSM.PaymentSource,
                PF.FromPrice,
                PF.ToPrice,
                PF.IsPriceRangeApplicable,
                PF.Percentage,
                PF.LumsumAmount,
                PF.PlatformChargesPercentage,
                PF.PlatformChargesLumsumAmount,
                PF.IsActive
            FROM tbl_PaymentSourceWisePlatformFees PF
            INNER JOIN tbl_PaymentSourceMaster PSM ON PF.PaymentSourceId = PSM.Id
            ORDER BY PSM.PaymentSource ASC", con);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtPaymentFees);
            }

            ViewBag.PaymentFeesList = dtPaymentFees;
            ViewBag.ReturnUrl = Request.Url.ToString();
            ViewBag.IsAttribute = Request.Url.ToString().Contains("portal") || Request.Url.ToString().Contains("localhost");

            return View();
        }
        [HttpGet]
        public ActionResult DeletePaymentPlateFormFees(int id)
        {
            try
            {
                objcashback.Id = id;
                int delresult = objcashback.DeletePaymentPlateFormFees(id);

                if (delresult > 0)
                {
                    TempData["SuccessMsg"] = "Payment source deleted successfully.";
                }
                else
                {
                    TempData["ErrorMsg"] = "Payment source not found or already deleted.";
                }

                return RedirectToAction("PaymentSourceList");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_orderdetail_product"))
                {
                    TempData["ErrorMsg"] = "You cannot delete this record because it is referenced by other records.";
                }
                else
                {
                    TempData["ErrorMsg"] = "SQL error: " + ex.Message;
                }
                return RedirectToAction("PaymentPlateFormFeesList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("PaymentPlateFormFeesList");
            }
        }
        [HttpGet]
        public ActionResult EditPaymentPlateFormFees(int id = 0)
        {
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            ViewBag.IsEdit = false;
            DataTable dtPaymentSource = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id, PaymentSource FROM tbl_PaymentSourceMaster WHERE IsActive = 1", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtPaymentSource);
            }
            ViewBag.PaymentSourceList = dtPaymentSource;

            if (id > 0)
            {
                // Fetch platform fee record with Payment Source info
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(@"
                SELECT PF.*, PSM.PaymentSource 
                FROM tbl_PaymentSourceWisePlatformFees PF
                INNER JOIN tbl_PaymentSourceMaster PSM ON PF.PaymentSourceId = PSM.Id
                WHERE PF.Id = @Id", con);
                    cmd.Parameters.AddWithValue("@Id", id);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    ViewBag.IsEdit = true;

                    ViewBag.Id = Convert.ToInt32(dr["Id"]);
                    ViewBag.PaymentSourceId = Convert.ToInt32(dr["PaymentSourceId"]);
                    ViewBag.FromPrice = dr["FromPrice"] != DBNull.Value ? Convert.ToDecimal(dr["FromPrice"]) : (decimal?)null;
                    ViewBag.ToPrice = dr["ToPrice"] != DBNull.Value ? Convert.ToDecimal(dr["ToPrice"]) : (decimal?)null;
                    ViewBag.IsPriceRangeApplicable = Convert.ToBoolean(dr["IsPriceRangeApplicable"]);
                    ViewBag.Percentage = dr["Percentage"] != DBNull.Value ? Convert.ToDecimal(dr["Percentage"]) : (decimal?)null;
                    ViewBag.LumsumAmount = dr["LumsumAmount"] != DBNull.Value ? Convert.ToDecimal(dr["LumsumAmount"]) : (decimal?)null;
                    ViewBag.PlatformChargesPercentage = dr["PlatformChargesPercentage"] != DBNull.Value ? Convert.ToDecimal(dr["PlatformChargesPercentage"]) : (decimal?)null;
                    ViewBag.PlatformChargesLumsumAmount = dr["PlatformChargesLumsumAmount"] != DBNull.Value ? Convert.ToDecimal(dr["PlatformChargesLumsumAmount"]) : (decimal?)null;
                    ViewBag.IsActive = dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false;
                }
                else
                {
                    TempData["ErrorMsg"] = "Payment platform fee record not found.";
                    return RedirectToAction("PaymentPlateFormFeesList");
                }
            }

            return View();
        }
        [HttpPost]
        public ActionResult UpdatePaymentPlateFormFees(FormCollection form)
        {
            int i = 0;
            try
            {
                if (Request.Cookies["gstusr"] == null)
                    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

                int id = Convert.ToInt32(form["Id"]);
                int paymentSourceId = Convert.ToInt32(form["PaymentSourceId"]);
                bool isPriceRangeApplicable = form["IsPriceRangeApplicable"]?.Contains("true") == true;
                bool isActive = form.AllKeys.Contains("IsActive") && form["IsActive"]?.Contains("true") == true;

                decimal? fromPrice = string.IsNullOrWhiteSpace(form["FromPrice"]) ? (decimal?)null : Convert.ToDecimal(form["FromPrice"]);
                decimal? toPrice = string.IsNullOrWhiteSpace(form["ToPrice"]) ? (decimal?)null : Convert.ToDecimal(form["ToPrice"]);
                decimal? percentage = string.IsNullOrWhiteSpace(form["Percentage"]) ? (decimal?)null : Convert.ToDecimal(form["Percentage"]);
                decimal? lumsumAmount = string.IsNullOrWhiteSpace(form["LumsumAmount"]) ? (decimal?)null : Convert.ToDecimal(form["LumsumAmount"]);
                decimal? platformChargesPercentage = string.IsNullOrWhiteSpace(form["PlatformChargesPercentage"]) ? (decimal?)null : Convert.ToDecimal(form["PlatformChargesPercentage"]);
                decimal? platformChargesLumsumAmount = string.IsNullOrWhiteSpace(form["PlatformChargesLumsumAmount"]) ? (decimal?)null : Convert.ToDecimal(form["PlatformChargesLumsumAmount"]);

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString))
                {
                    con.Open();

                    SqlCommand com = new SqlCommand(@"
                UPDATE tbl_PaymentSourceWisePlatformFees
                SET 
                    PaymentSourceId = @PaymentSourceId,
                    FromPrice = @FromPrice,
                    ToPrice = @ToPrice,
                    IsPriceRangeApplicable = @IsPriceRangeApplicable,
                    Percentage = @Percentage,
                    LumsumAmount = @LumsumAmount,
                    PlatformChargesPercentage = @PlatformChargesPercentage,
                    PlatformChargesLumsumAmount = @PlatformChargesLumsumAmount,
                    IsActive = @IsActive
                WHERE Id = @Id", con);

                    com.Parameters.AddWithValue("@Id", id);
                    com.Parameters.AddWithValue("@PaymentSourceId", paymentSourceId);
                    com.Parameters.AddWithValue("@FromPrice", (object)fromPrice ?? DBNull.Value);
                    com.Parameters.AddWithValue("@ToPrice", (object)toPrice ?? DBNull.Value);
                    com.Parameters.AddWithValue("@IsPriceRangeApplicable", isPriceRangeApplicable);
                    com.Parameters.AddWithValue("@Percentage", (object)percentage ?? DBNull.Value);
                    com.Parameters.AddWithValue("@LumsumAmount", (object)lumsumAmount ?? DBNull.Value);
                    com.Parameters.AddWithValue("@PlatformChargesPercentage", (object)platformChargesPercentage ?? DBNull.Value);
                    com.Parameters.AddWithValue("@PlatformChargesLumsumAmount", (object)platformChargesLumsumAmount ?? DBNull.Value);
                    com.Parameters.AddWithValue("@IsActive", isActive);

                    i = com.ExecuteNonQuery();
                    con.Close();
                }

                if (i > 0)
                {
                    TempData["SuccessMsg"] = "Platform fee record updated successfully.";
                }
                else
                {
                    TempData["ErrorMsg"] = "Update failed. Record not found.";
                }

                return RedirectToAction("PaymentPlateFormFeesList");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Error occurred: " + ex.Message;
                return View("EditPaymentPlateFormFees");
            }
        }

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
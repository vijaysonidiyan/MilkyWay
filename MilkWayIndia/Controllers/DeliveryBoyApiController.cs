using MilkWayIndia.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace MilkWayIndia.Controllers
{
    public class DeliveryBoyApiController : ApiController
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        [Route("api/DeliveryBoyApi/Login/{username?}/{password?}")]
        [HttpGet]

        public HttpResponseMessage Login(string username, string password, string fcm_token = "")
        {



            DeliveryBoy objlogin = new DeliveryBoy();

            DataTable dtUser = new DataTable();
            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("msg", typeof(string));
            //   dtNew.Columns.Add("error_msg", typeof(string));
            dtNew.Columns.Add("userid", typeof(Int64));
            dtNew.Columns.Add("firstname", typeof(string));
            dtNew.Columns.Add("lastname", typeof(string));
            dtNew.Columns.Add("mobileno", typeof(string));
            dtNew.Columns.Add("email", typeof(string));
            dtNew.Columns.Add("address", typeof(string));
            dtNew.Columns.Add("photo", typeof(string));


            dtNew.Columns.Add("SectorId", typeof(int));

            DataRow dr = dtNew.NewRow();
            if (username != null)
            {
                DataTable dtUsername = new DataTable();
                dtUsername = objlogin.CheckDeliveryBoyUserName(username);

                DataTable dtpassword = new DataTable();
                dtpassword = objlogin.DeliveryBoylogin(username, password);

                if (dtUsername.Rows.Count == 0)
                {
                    dr["status"] = "Failed";
                    dr["msg"] = "Username Not Found";
                }
                else if (dtpassword.Rows.Count == 0)
                {
                    dr["status"] = "Failed";
                    dr["msg"] = "Wrong Password";
                }

                if (dtpassword.Rows.Count > 0)
                {
                    dr["status"] = "Success";
                    dr["msg"] = "Find User";
                    int deliverboyId = Convert.ToInt32(dtpassword.Rows[0]["Id"]);

                    // var _deliverboy = objlogin.BindDeliverboy(deliverboyId);

                    dr["userid"] = Convert.ToInt32(dtpassword.Rows[0]["Id"]);
                    dr["firstname"] = dtpassword.Rows[0]["FirstName"].ToString().Trim();
                    dr["lastname"] = dtpassword.Rows[0]["LastName"].ToString().Trim();
                    dr["mobileno"] = dtpassword.Rows[0]["MobileNo"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtpassword.Rows[0]["Email"].ToString()))
                        dr["email"] = dtpassword.Rows[0]["Email"].ToString().Trim();
                    else
                        dr["email"] = "";
                    if (!string.IsNullOrEmpty(dtpassword.Rows[0]["Photo"].ToString()))
                    {
                        var encoded = Uri.EscapeUriString(dtpassword.Rows[0]["Photo"].ToString().Trim());
                        dr["photo"] = Helper.PhotoFolderPath + "/image/staffphoto/" + encoded;
                    }
                    else
                        dr["photo"] = "";
                    if (!string.IsNullOrEmpty(dtpassword.Rows[0]["SectorId"].ToString()))
                        dr["SectorId"] = Convert.ToInt32(dtpassword.Rows[0]["SectorId"]);
                    else
                        dr["SectorId"] = 0;

                    if (!string.IsNullOrEmpty(dtpassword.Rows[0]["Address"].ToString()))
                        dr["address"] = dtpassword.Rows[0]["Address"];
                    else
                        dr["address"] = 0;

                }

            }

            dtNew.Rows.Add(dr);
            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dtNew);
            //jsonString = jsonString.Replace(@"[", "");
            // jsonString = jsonString.Replace(@"]", "");

            // ViewBag.Status = jsonString;
            // return jsonString;
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return response;
        }


        [Route("api/DeliveryBoyApi/DailyOrderCustomer1/{Deliveryboyid?}/{FDate?}/{TDate?}")]
        [HttpGet]

        public HttpResponseMessage DailyOrderCustomer1(String Deliveryboyid, DateTime? FDate, DateTime? TDate)
        {

            DateTime Fromdate = DateTime.Now, Todate = DateTime.Now;

            Fromdate = Convert.ToDateTime(FDate);
            Todate = Convert.ToDateTime(TDate);
            DeliveryBoy obj = new DeliveryBoy();

            string DeliveryBoyname = "";
            // obj.StaffId = Convert.ToInt32(Deliveryboyid);

            obj.StaffId = Convert.ToInt32(Deliveryboyid);

            DeliveryBoy order = new DeliveryBoy();


            DataTable dtNew1 = new DataTable();
            DataTable dtList = new DataTable();
            DataTable dtList1 = new DataTable();

            dtList1 = order.getDeliveryBoyWiseCustomerList(obj.StaffId);
            if (dtList1.Rows.Count > 0)
            {
                //dtNew1.Columns.Add("DeliveryBoy", typeof(string));
                var str = "";
                string two = "";
                string jsonString = string.Empty;
                //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();



                DeliveryBoyname = dtList1.Rows[0][1].ToString();
                for (int j = 0; j < dtList1.Rows.Count; j++)
                {
                    obj.CustomerId = Convert.ToInt32(dtList1.Rows[j].ItemArray[0].ToString());
                    dtList = order.getDeliveryBoyWiseCustomerOrder(obj.CustomerId, Fromdate, Todate);
                    int userRecords = dtList.Rows.Count;

                    if (userRecords > 0)
                    {
                        DataTable dtNew = new DataTable();
                        dtNew.Columns.Add("id", typeof(Int64));
                        dtNew.Columns.Add("SectorName", typeof(string));
                        dtNew.Columns.Add("CustomerId", typeof(string));
                        dtNew.Columns.Add("Customer", typeof(string));
                        dtNew.Columns.Add("Contact", typeof(string));
                        dtNew.Columns.Add("Address", typeof(string));
                        dtNew.Columns.Add("OrderNo", typeof(Int64));
                        dtNew.Columns.Add("OrderDate", typeof(string));
                        dtNew.Columns.Add("ProductName", typeof(string));
                        dtNew.Columns.Add("image", typeof(string));
                        dtNew.Columns.Add("Qty", typeof(Int64));
                        dtNew.Columns.Add("Amount", typeof(decimal));
                        dtNew.Columns.Add("status", typeof(string));
                        dtNew.Columns.Add("orderby", typeof(Int64));
                        dtNew.Columns.Add("sid", typeof(Int64));
                        dtNew.Columns.Add("DeliveryStatus", typeof(string));

                        for (int i = 0; i < dtList.Rows.Count; i++)
                        {
                            try
                            {
                                //Status = "0";
                                DataRow dr = dtNew.NewRow();
                                // dr["status"] = "Success";
                                dr["id"] = dtList.Rows[i]["Id"].ToString().Trim();

                                //int productID = Convert.ToInt32(dtList.Rows[i]["Id"].ToString().Trim());
                                dr["SectorName"] = dtList.Rows[i]["SectorName"].ToString().Trim();
                                dr["CustomerId"] = dtList.Rows[i]["CustomerId"].ToString().Trim();
                                dr["Customer"] = dtList.Rows[i]["Customer"].ToString();
                                dr["Contact"] = dtList.Rows[i]["MobileNo"].ToString();
                                dr["Address"] = dtList.Rows[i]["Address"].ToString();

                                dr["OrderNo"] = dtList.Rows[i]["OrderNo"].ToString().Trim();

                                dr["OrderDate"] = dtList.Rows[i]["OrderDate"].ToString().Trim();


                                dr["ProductName"] = dtList.Rows[i]["ProductName"].ToString();


                                if (!string.IsNullOrEmpty(dtList.Rows[i]["image"].ToString()))
                                {
                                    var encoded = Uri.EscapeUriString(dtList.Rows[i]["image"].ToString().Trim());
                                    dr["image"] = Helper.PhotoFolderPath + "/image/product/" + encoded;
                                }
                                else
                                    dr["image"] = "";

                                if (!string.IsNullOrEmpty(dtList.Rows[i]["Qty"].ToString()))
                                    dr["Qty"] = Convert.ToInt32(dtList.Rows[i]["Qty"]);
                                else
                                    dr["Qty"] = "0";
                                if (!string.IsNullOrEmpty(dtList.Rows[i]["Amount"].ToString()))
                                    dr["Amount"] = Convert.ToDecimal(dtList.Rows[i]["Amount"]);
                                else
                                    dr["Amount"] = "0";

                                dr["status"] = dtList.Rows[i]["status"].ToString();

                                if (!string.IsNullOrEmpty(dtList.Rows[i]["orderby"].ToString()))
                                    dr["orderby"] = Convert.ToInt32(dtList.Rows[i]["orderby"]);
                                else
                                    dr["orderby"] = "0";
                                if (!string.IsNullOrEmpty(dtList.Rows[i]["sid"].ToString()))
                                    dr["sid"] = Convert.ToInt64(dtList.Rows[i]["sid"]);
                                else
                                    dr["sid"] = "0";


                                dr["DeliveryStatus"] = dtList.Rows[i]["DeliveryStatus"].ToString();




                                dtNew.Rows.Add(dr);
                            }
                            catch { }
                        }

                        jsonString = JsonConvert.SerializeObject(dtNew);

                        dict["status"] = "success";

                        dict["DeliveryBoy"] = DeliveryBoyname;
                        dict["OrderDetail"] = jsonString;

                        two = two + @",""OrderDetail"":" + dict["OrderDetail"];


                    }



                    //  string jsonString = string.Empty;
                    //new Newtonsoft.Json.Formatting()

                    // var dict = new Dictionary<string, string>();


                }
                string one = @"{""status"":""success""";
                string twodup = @",""DeliveryBoy"":" + dict["DeliveryBoy"];

                string three = one + twodup + two + "}";

                str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;



            }



            else
            {
                DataTable dtNew = new DataTable();
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["OrderDetail"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""OrderDetail"":" + dict["OrderDetail"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }

            var str1 = "a";
            var response1 = Request.CreateResponse(HttpStatusCode.OK);
            response1.Content = new StringContent(str1, Encoding.UTF8, "application/json");
            return response1;


        }

        [Route("api/DeliveryBoyApi/DailyOrderCustomer/{Deliveryboyid?}/{FDate?}/{TDate?}")]
        [HttpGet]

        public HttpResponseMessage DailyOrderCustomer(String Deliveryboyid, DateTime? FDate, DateTime? TDate)
        {

            DateTime Fromdate = DateTime.Now, Todate = DateTime.Now;

            Fromdate = Convert.ToDateTime(FDate);
            Todate = Convert.ToDateTime(TDate);
            DeliveryBoy obj = new DeliveryBoy();

            Customer objcust = new Customer();
            string jsonString1 = string.Empty;


            obj.StaffId = Convert.ToInt32(Deliveryboyid);

            DeliveryBoy order = new DeliveryBoy();

            DataTable dtNew = new DataTable();
            DataTable dtNew1 = new DataTable();
            DataTable dtList = new DataTable();
            DataTable dtList1 = new DataTable();

            dtList1 = objcust.BindCustomerCashOrder(obj.StaffId, Fromdate, Todate);
            int userRecords1 = dtList1.Rows.Count;
            if (dtList1.Rows.Count > 0)
            {
                dtNew1.Columns.Add("CustomerId", typeof(string));
                dtNew1.Columns.Add("Customer", typeof(string));
                dtNew1.Columns.Add("Contact", typeof(string));
                dtNew1.Columns.Add("Address", typeof(string));
                dtNew1.Columns.Add("CashCollectionId", typeof(string));
                dtNew1.Columns.Add("CashCollectionAmount", typeof(string));
                dtNew1.Columns.Add("orderby", typeof(Int64));
                dtNew1.Columns.Add("Lat", typeof(string));
                dtNew1.Columns.Add("Lon", typeof(string));

                dtNew1.Columns.Add("DMID", typeof(string));
                dtNew1.Columns.Add("Collectdate", typeof(string));
                for (int i = 0; i < dtList1.Rows.Count; i++)
                {
                    try
                    {
                        DataRow dr1 = dtNew1.NewRow();
                        dr1["CustomerId"] = dtList1.Rows[i]["CustomerId"].ToString().Trim();
                        dr1["Customer"] = dtList1.Rows[i]["Customer"].ToString();
                        dr1["Contact"] = dtList1.Rows[i]["MobileNo"].ToString();
                        dr1["Address"] = dtList1.Rows[i]["Address"].ToString();
                        dr1["CashCollectionId"] = dtList1.Rows[i]["cashId"].ToString();
                        dr1["CashCollectionAmount"] = dtList1.Rows[i]["CashAmount"].ToString();
                        if (!string.IsNullOrEmpty(dtList1.Rows[i]["OrderBy"].ToString()))
                            dr1["orderby"] = Convert.ToInt32(dtList1.Rows[i]["OrderBy"]);
                        else
                            dr1["orderby"] = "0";
                        dr1["Lat"] = dtList1.Rows[i]["lat"].ToString();
                        dr1["Lon"] = dtList1.Rows[i]["lon"].ToString();

                        dr1["DMID"] = dtList1.Rows[i]["DMId"].ToString();
                        dr1["Collectdate"] = dtList1.Rows[i]["CollectDate"].ToString();
                        dtNew1.Rows.Add(dr1);
                    }

                    catch { }


                }



                jsonString1 = JsonConvert.SerializeObject(dtNew1);
            }

            dtList = order.getDeliveryBoyWiseCustomerOrder(obj.StaffId, Fromdate, Todate);






            int userRecords = dtList.Rows.Count;
            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("DeliveryBoy", typeof(string));

                dtNew.Columns.Add("SectorName", typeof(string));
                dtNew.Columns.Add("CustomerId", typeof(string));
                dtNew.Columns.Add("Customer", typeof(string));
                dtNew.Columns.Add("Contact", typeof(string));
                dtNew.Columns.Add("Address", typeof(string));

                dtNew.Columns.Add("OrderNo", typeof(Int64));
                dtNew.Columns.Add("OrderDate", typeof(string));
                dtNew.Columns.Add("ProductId", typeof(string));
                dtNew.Columns.Add("ProductName", typeof(string));
                dtNew.Columns.Add("image", typeof(string));
                dtNew.Columns.Add("Qty", typeof(Int64));
                dtNew.Columns.Add("Amount", typeof(decimal));
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("orderby", typeof(Int64));
                dtNew.Columns.Add("sid", typeof(Int64));
                dtNew.Columns.Add("DeliveryStatus", typeof(string));
                dtNew.Columns.Add("Lat", typeof(string));
                dtNew.Columns.Add("Lon", typeof(string));

                dtNew.Columns.Add("NewQty", typeof(string));

                dtNew.Columns.Add("AttributeName", typeof(string));
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    try
                    {
                        //Status = "0";
                        DataRow dr = dtNew.NewRow();
                        // dr["status"] = "Success";
                        dr["id"] = dtList.Rows[i]["Id"].ToString().Trim();
                        dr["DeliveryBoy"] = dtList.Rows[i]["DeliveryBoy"].ToString();

                        //int productID = Convert.ToInt32(dtList.Rows[i]["Id"].ToString().Trim());
                        dr["SectorName"] = dtList.Rows[i]["SectorName"].ToString().Trim();
                        dr["CustomerId"] = dtList.Rows[i]["CustomerId"].ToString().Trim();
                        dr["Customer"] = dtList.Rows[i]["Customer"].ToString();
                        dr["Contact"] = dtList.Rows[i]["MobileNo"].ToString();
                        dr["Address"] = dtList.Rows[i]["Address"].ToString();
                        //dr["CashCollectionId"] = dtList.Rows[i]["cashId"].ToString();
                        //dr["CashCollectionAmount"] = dtList.Rows[i]["CashAmount"].ToString();

                        dr["OrderNo"] = dtList.Rows[i]["OrderNo"].ToString().Trim();

                        dr["OrderDate"] = dtList.Rows[i]["OrderDate"].ToString().Trim();

                        dr["ProductId"] = dtList.Rows[i]["Proid"].ToString();
                        dr["ProductName"] = dtList.Rows[i]["ProductName"].ToString();


                        if (!string.IsNullOrEmpty(dtList.Rows[i]["image"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtList.Rows[i]["image"].ToString().Trim());
                            dr["image"] = Helper.PhotoFolderPath + "/image/product/" + encoded;
                        }
                        else
                            dr["image"] = "";

                        if (!string.IsNullOrEmpty(dtList.Rows[i]["Qty"].ToString()))
                            dr["Qty"] = Convert.ToInt32(dtList.Rows[i]["Qty"]);
                        else
                            dr["Qty"] = "0";
                        if (!string.IsNullOrEmpty(dtList.Rows[i]["Amount"].ToString()))
                            dr["Amount"] = Convert.ToDecimal(dtList.Rows[i]["Amount"]);
                        else
                            dr["Amount"] = "0";

                        dr["status"] = dtList.Rows[i]["status"].ToString();

                        if (!string.IsNullOrEmpty(dtList.Rows[i]["orderby"].ToString()))
                            dr["orderby"] = Convert.ToInt32(dtList.Rows[i]["orderby"]);
                        else
                            dr["orderby"] = "0";
                        if (!string.IsNullOrEmpty(dtList.Rows[i]["sid"].ToString()))
                            dr["sid"] = Convert.ToInt64(dtList.Rows[i]["sid"]);
                        else
                            dr["sid"] = "0";


                        dr["DeliveryStatus"] = dtList.Rows[i]["DeliveryStatus"].ToString();

                        dr["Lat"] = dtList.Rows[i]["lat"].ToString();
                        dr["Lon"] = dtList.Rows[i]["lon"].ToString();


                        dr["NewQty"] = dtList.Rows[i]["newqty"].ToString();
                        dr["AttributeName"] = dtList.Rows[i]["Name"].ToString();

                        dtNew.Rows.Add(dr);
                    }
                    catch { }
                }


            }

            if (userRecords > 0 || userRecords1 > 0)
            {

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();

                if (userRecords == 0)
                {
                    dtNew.Columns.Add("status", typeof(string));
                    dtNew.Columns.Add("msg", typeof(string));
                    DataRow dr = dtNew.NewRow();
                    dr["status"] = "Fail";
                    dr["msg"] = "No Record Found";
                    dtNew.Rows.Add(dr);
                    jsonString = string.Empty;
                    jsonString = JsonConvert.SerializeObject(dtNew);
                }
                if (userRecords1 == 0)
                {
                    dtNew1.Columns.Add("status", typeof(string));
                    dtNew1.Columns.Add("msg", typeof(string));
                    DataRow dr1 = dtNew1.NewRow();
                    dr1["status"] = "Fail";
                    dr1["msg"] = "No Record Found";
                    dtNew1.Rows.Add(dr1);
                    jsonString1 = string.Empty;
                    jsonString1 = JsonConvert.SerializeObject(dtNew1);

                }


                dict["status"] = "success";
                dict["OrderDetail"] = jsonString;
                dict["CashDetail"] = jsonString1;

                string one = @"{""status"":""success""";
                string two = @",""OrderDetail"":" + dict["OrderDetail"];
                string three = @",""CashDetail"":" + dict["CashDetail"];
                string four = one + two + three + "}";

                var str = four.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {

                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                dtNew1.Columns.Add("status", typeof(string));
                dtNew1.Columns.Add("msg", typeof(string));
                DataRow dr1 = dtNew1.NewRow();
                dr1["status"] = "Fail";
                dr1["msg"] = "No Record Found";
                dtNew1.Rows.Add(dr1);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()
                jsonString1 = string.Empty;
                jsonString1 = JsonConvert.SerializeObject(dtNew1);
                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["OrderDetail"] = jsonString;
                dict["CashDetail"] = jsonString1;

                string one = @"{""status"":""Fail""";
                string two = @",""OrderDetail"":" + dict["OrderDetail"];
                string three = @",""CashDetail"":" + dict["CashDetail"];
                string four = one + two + three + "}";

                var str = four.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }

        }


        [Route("api/DeliveryBoyApi/Dailytest/{CustomerId?}")]
        [HttpGet]

        public IHttpActionResult Dailytest(String CustomerId)
        {
            DeliveryBoy obj = new DeliveryBoy();


            obj.CustomerId = Convert.ToInt32(CustomerId);

            DeliveryBoy order = new DeliveryBoy();

            var dtList = order.getTest(obj.CustomerId);


            return Ok(dtList);
        }


        [Route("api/DeliveryBoyApi/DailyTomOrderCustomer/{Deliveryboyid?}")]

        [HttpGet]

        public HttpResponseMessage DailyTomOrderCustomer(String Deliveryboyid)
        {

            DeliveryBoy obj = new DeliveryBoy();

            Customer objcust = new Customer();
            string jsonString1 = string.Empty;


            obj.StaffId = Convert.ToInt32(Deliveryboyid);

            DeliveryBoy order = new DeliveryBoy();

            DataTable dtNew = new DataTable();
            DataTable dtNew1 = new DataTable();
            DataTable dtList = new DataTable();
            DataTable dtList1 = new DataTable();

            dtList1 = objcust.BindCustomerCashOrderTom(obj.StaffId);
            int userRecords1 = dtList1.Rows.Count;
            if (dtList1.Rows.Count > 0)
            {
                dtNew1.Columns.Add("CustomerId", typeof(string));
                dtNew1.Columns.Add("Customer", typeof(string));
                dtNew1.Columns.Add("Contact", typeof(string));
                dtNew1.Columns.Add("Address", typeof(string));
                dtNew1.Columns.Add("CashCollectionId", typeof(string));
                dtNew1.Columns.Add("CashCollectionAmount", typeof(string));
                dtNew1.Columns.Add("orderby", typeof(Int64));
                dtNew1.Columns.Add("Lat", typeof(string));
                dtNew1.Columns.Add("Lon", typeof(string));
                for (int i = 0; i < dtList1.Rows.Count; i++)
                {
                    try
                    {
                        DataRow dr1 = dtNew1.NewRow();
                        dr1["CustomerId"] = dtList1.Rows[i]["CustomerId"].ToString().Trim();
                        dr1["Customer"] = dtList1.Rows[i]["Customer"].ToString();
                        dr1["Contact"] = dtList1.Rows[i]["MobileNo"].ToString();
                        dr1["Address"] = dtList1.Rows[i]["Address"].ToString();
                        dr1["CashCollectionId"] = dtList1.Rows[i]["cashId"].ToString();
                        dr1["CashCollectionAmount"] = dtList1.Rows[i]["CashAmount"].ToString();
                        if (!string.IsNullOrEmpty(dtList1.Rows[i]["OrderBy"].ToString()))
                            dr1["orderby"] = Convert.ToInt32(dtList1.Rows[i]["OrderBy"]);
                        else
                            dr1["orderby"] = "0";
                        dr1["Lat"] = dtList1.Rows[i]["lat"].ToString();
                        dr1["Lon"] = dtList1.Rows[i]["lon"].ToString();
                        dtNew1.Rows.Add(dr1);
                    }

                    catch { }


                }



                jsonString1 = JsonConvert.SerializeObject(dtNew1);
            }

            dtList = order.getDeliveryBoyWiseTomCustomerOrder(obj.StaffId);






            int userRecords = dtList.Rows.Count;
            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("DeliveryBoy", typeof(string));
                dtNew.Columns.Add("SectorName", typeof(string));
                dtNew.Columns.Add("CustomerId", typeof(string));
                dtNew.Columns.Add("Customer", typeof(string));
                dtNew.Columns.Add("Contact", typeof(string));
                dtNew.Columns.Add("Address", typeof(string));

                dtNew.Columns.Add("OrderNo", typeof(Int64));
                dtNew.Columns.Add("OrderDate", typeof(string));
                dtNew.Columns.Add("ProductId", typeof(string));
                dtNew.Columns.Add("ProductName", typeof(string));
                dtNew.Columns.Add("image", typeof(string));
                dtNew.Columns.Add("Qty", typeof(Int64));
                dtNew.Columns.Add("Amount", typeof(decimal));
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("orderby", typeof(Int64));
                dtNew.Columns.Add("sid", typeof(Int64));
                dtNew.Columns.Add("DeliveryStatus", typeof(string));
                dtNew.Columns.Add("Lat", typeof(string));
                dtNew.Columns.Add("Lon", typeof(string));


                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    try
                    {
                        //Status = "0";
                        DataRow dr = dtNew.NewRow();
                        // dr["status"] = "Success";
                        dr["id"] = dtList.Rows[i]["Id"].ToString().Trim();
                        dr["DeliveryBoy"] = dtList.Rows[i]["DeliveryBoy"].ToString();
                        //int productID = Convert.ToInt32(dtList.Rows[i]["Id"].ToString().Trim());
                        dr["SectorName"] = dtList.Rows[i]["SectorName"].ToString().Trim();
                        dr["CustomerId"] = dtList.Rows[i]["CustomerId"].ToString().Trim();
                        dr["Customer"] = dtList.Rows[i]["Customer"].ToString();
                        dr["Contact"] = dtList.Rows[i]["MobileNo"].ToString();
                        dr["Address"] = dtList.Rows[i]["Address"].ToString();
                        //dr["CashCollectionId"] = dtList.Rows[i]["cashId"].ToString();
                        //dr["CashCollectionAmount"] = dtList.Rows[i]["CashAmount"].ToString();

                        dr["OrderNo"] = dtList.Rows[i]["OrderNo"].ToString().Trim();

                        dr["OrderDate"] = dtList.Rows[i]["OrderDate"].ToString().Trim();

                        dr["ProductId"] = dtList.Rows[i]["Proid"].ToString();
                        dr["ProductName"] = dtList.Rows[i]["ProductName"].ToString();


                        if (!string.IsNullOrEmpty(dtList.Rows[i]["image"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtList.Rows[i]["image"].ToString().Trim());
                            dr["image"] = Helper.PhotoFolderPath + "/image/product/" + encoded;
                        }
                        else
                            dr["image"] = "";

                        if (!string.IsNullOrEmpty(dtList.Rows[i]["Qty"].ToString()))
                            dr["Qty"] = Convert.ToInt32(dtList.Rows[i]["Qty"]);
                        else
                            dr["Qty"] = "0";
                        if (!string.IsNullOrEmpty(dtList.Rows[i]["Amount"].ToString()))
                            dr["Amount"] = Convert.ToDecimal(dtList.Rows[i]["Amount"]);
                        else
                            dr["Amount"] = "0";

                        dr["status"] = dtList.Rows[i]["status"].ToString();

                        if (!string.IsNullOrEmpty(dtList.Rows[i]["orderby"].ToString()))
                            dr["orderby"] = Convert.ToInt32(dtList.Rows[i]["orderby"]);
                        else
                            dr["orderby"] = "0";
                        if (!string.IsNullOrEmpty(dtList.Rows[i]["sid"].ToString()))
                            dr["sid"] = Convert.ToInt64(dtList.Rows[i]["sid"]);
                        else
                            dr["sid"] = "0";


                        dr["DeliveryStatus"] = dtList.Rows[i]["DeliveryStatus"].ToString();

                        dr["Lat"] = dtList.Rows[i]["lat"].ToString();
                        dr["Lon"] = dtList.Rows[i]["lon"].ToString();


                        dtNew.Rows.Add(dr);
                    }
                    catch { }
                }


            }

            if (userRecords > 0 || userRecords1 > 0)
            {

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();

                if (userRecords == 0)
                {
                    dtNew.Columns.Add("status", typeof(string));
                    dtNew.Columns.Add("msg", typeof(string));
                    DataRow dr = dtNew.NewRow();
                    dr["status"] = "Fail";
                    dr["msg"] = "No Record Found";
                    dtNew.Rows.Add(dr);
                    jsonString = string.Empty;
                    jsonString = JsonConvert.SerializeObject(dtNew);
                }
                if (userRecords1 == 0)
                {
                    dtNew1.Columns.Add("status", typeof(string));
                    dtNew1.Columns.Add("msg", typeof(string));
                    DataRow dr1 = dtNew1.NewRow();
                    dr1["status"] = "Fail";
                    dr1["msg"] = "No Record Found";
                    dtNew1.Rows.Add(dr1);
                    jsonString1 = string.Empty;
                    jsonString1 = JsonConvert.SerializeObject(dtNew1);

                }


                dict["status"] = "success";
                dict["OrderDetail"] = jsonString;
                dict["CashDetail"] = jsonString1;

                string one = @"{""status"":""success""";
                string two = @",""OrderDetail"":" + dict["OrderDetail"];
                string three = @",""CashDetail"":" + dict["CashDetail"];
                string four = one + two + three + "}";

                var str = four.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {

                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                dtNew1.Columns.Add("status", typeof(string));
                dtNew1.Columns.Add("msg", typeof(string));
                DataRow dr1 = dtNew.NewRow();
                dr1["status"] = "Fail";
                dr1["msg"] = "No Record Found";
                dtNew1.Rows.Add(dr1);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()
                jsonString1 = string.Empty;
                jsonString1 = JsonConvert.SerializeObject(dtNew1);
                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["OrderDetail"] = jsonString;
                dict["CashDetail"] = jsonString1;

                string one = @"{""status"":""Fail""";
                string two = @",""OrderDetail"":" + dict["OrderDetail"];
                string three = @",""CashDetail"":" + dict["CashDetail"];
                string four = one + two + three + "}";

                var str = four.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }

            //return Ok(dtList);
        }


        [Route("api/DeliveryBoyApi/DailyOrderVendor/{Deliveryboyid?}/{FDate?}/{TDate?}")]
        [HttpGet]

        public IHttpActionResult DailyOrderVendor(String Deliveryboyid, DateTime? FDate, DateTime? TDate)
        {
            DeliveryBoy obj = new DeliveryBoy();

            DateTime Fromdate = DateTime.Now, Todate = DateTime.Now;

            Fromdate = Convert.ToDateTime(FDate);
            Todate = Convert.ToDateTime(TDate);

            obj.StaffId = Convert.ToInt32(Deliveryboyid);

            DeliveryBoy order = new DeliveryBoy();
            //ViewBag.FromDate = _fdate.ToString("dd-MMM-yyyy");
            //ViewBag.FromDate = FDate;
            //ViewBag.ToDate = TDate;

            string sid = "";
            DeliveryBoy order1 = new DeliveryBoy();
            var dtListsector = order1.getDeliveryBoyWiseOrdervendorsector(obj.StaffId, Fromdate, Todate);
            DataTable dt1 = new DataTable();
            if (dtListsector.Rows.Count > 0)
            {


                DataRow dr = null;
                DataTable dt = new DataTable();
                //List<SectorViewModel> list = new List<SectorViewModel>();
                for (int i = 0; i < dtListsector.Rows.Count; i++)
                {


                    sid = dtListsector.Rows[i].ItemArray[0].ToString();
                    order = new DeliveryBoy();
                    dt = order.GetMultiSectorVendorOrder1(sid, obj.StaffId, Fromdate, Todate);
                    dt1.Merge(dt);
                    dt.Clear();

                }


            }

            return Ok(dt1);
        }



        [Route("api/DeliveryBoyApi/DailyOrderCustomerFilter/{Deliveryboyid?}/{orderstatus?}/{FDate?}/{TDate?}")]
        [HttpGet]
        public IHttpActionResult DailyOrderCustomerFilter(string Deliveryboyid, string orderstatus, string FDate, string TDate) //JsonResult
        {

            SubscriptionNew obj = new SubscriptionNew();

            DeliveryBoy objd = new DeliveryBoy();
            DataTable dtNew = new DataTable();
            DataTable dtNewItem = new DataTable();
            DataTable dtprodRecord = new DataTable();


            objd.StaffId = Convert.ToInt32(Deliveryboyid);
            objd.Status = orderstatus;
            DateTime Fromdate = DateTime.Now, Todate = DateTime.Now;
            if (!string.IsNullOrEmpty(FDate.ToString()))
                Fromdate = Convert.ToDateTime(DateTime.ParseExact(FDate, @"dd-MM-yyyy", null));
            if (!string.IsNullOrEmpty(TDate.ToString()))
                Todate = Convert.ToDateTime(DateTime.ParseExact(TDate, @"dd-MM-yyyy", null));

            DeliveryBoy order = new DeliveryBoy();
            var dtList = order.getDeliveryBoyWiseFilterCustomerOrder(objd.StaffId, objd.Status, Fromdate, Todate);


            return Ok(dtList);


        }


        [Route("api/DeliveryBoyApi/DailyOrderAlerts/{orderstatus?}/{OrderId?}/{CustomerId?}/{Ctime?}")]
        [HttpGet]
        public IHttpActionResult DailyOrderAlerts(string orderstatus, string OrderId, string CustomerId, string Ctime)//JsonResult
        {
            DeliveryBoy obj = new DeliveryBoy();
            Subscription objsub = new Subscription();
            Ctime = Ctime.Replace("-", ":");
            obj.Status = orderstatus;
            int UpdateOrder = 0;

            int c = OrderId.Length;

            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            string a = "";
            foreach (string s in OrderId.Split(delimiter))
            {

                obj.Id = Convert.ToInt32(s);

                if (!string.IsNullOrEmpty(obj.Id.ToString()) && !string.IsNullOrEmpty(obj.Status))
                {
                    UpdateOrder = obj.UpdateCustomerOrderMain(obj, Ctime);
                }

            }




            if (UpdateOrder > 0)
            {

                if (orderstatus == "Complete" || orderstatus == "Delivered" || orderstatus == "Completed")
                {
                    objsub.CustomerId = Convert.ToInt32(CustomerId);
                    objsub.OrderId = obj.Id;
                    //var i = objsub.CheckCustomerWalletEntry(objsub.OrderId, objsub.CustomerId);
                    //if (i == 0)
                    //{
                    //    objsub.Description = "Place Order";
                    //    objsub.Type = "Debit";
                    //    objsub.InsertWallet(objsub);
                    //}
                    //else
                    //{
                    //    objsub.UpdateCustomerWallet(objsub);
                    //}
                }
                return Ok("Status Updated By DM Successfully");
            }
            else
            {
                return Ok("Status Not Updated");
            }
        }



        [Route("api/DeliveryBoyApi/DMwiseSector/{DeliveryboyId?}")]
        [HttpGet]
        public IHttpActionResult DMwiseSector(int DeliveryboyId)//JsonResult
        {

            DeliveryBoy order1 = new DeliveryBoy();
            var dtListsector = order1.getDeliveryBoyWiseOrdervendorsector1(DeliveryboyId);

            return Ok(dtListsector);

        }


        [Route("api/DeliveryBoyApi/DMwiseCustomerOrderSortList/{DeliveryboyId?}/{sid?}")]
        [HttpGet]

        public IHttpActionResult DMwiseCustomerOrderSortList(int DeliveryboyId, int sid)//JsonResult
        {

            DeliveryBoy order1 = new DeliveryBoy();

            var dtCustomerSort = order1.GetCustomerSortOrder(DeliveryboyId, sid);

            return Ok(dtCustomerSort);

        }


        [Route("api/DeliveryBoyApi/DMwiseCustomerSortUpdate/{CustomerId?}/{currentsortorder?}/{nextsortorder?}/{DeliveryboyId?}")]
        [HttpGet]

        public IHttpActionResult DMwiseCustomerSortUpdate(int CustomerId, int currentsortorder, int nextsortorder, int DeliveryboyId)//JsonResult
        {
            DeliveryBoy obj = new DeliveryBoy();
            Subscription objsub = new Subscription();
            obj.CustomerId = CustomerId;
            //obj.SectorId = sid;

            int UpdateOrder = 0;
            if (!string.IsNullOrEmpty(obj.CustomerId.ToString()) && !string.IsNullOrEmpty(obj.SectorId.ToString()))
            {
                UpdateOrder = obj.UpdateCustomerSortOrder(obj, currentsortorder, nextsortorder, DeliveryboyId);
            }

            return Ok("Status Updated");
        }



        public IHttpActionResult UpdateProductPhoto(DeliveryBoy item)
        {
            DeliveryBoy obj = new DeliveryBoy();
            string jsonstring = null;

            DataTable dt = new DataTable();
            DataTable dtNew = new DataTable();

            int result = 0, updatepoint = 0;
            dtNew.Columns.Add("imagepath", typeof(string));
            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("message", typeof(string));
            DataRow dr = dtNew.NewRow();
            dr["status"] = "failed";
            dr["message"] = "";

            if ((!string.IsNullOrEmpty(item.DeliveryBoyId.ToString()) || item.DeliveryBoyId != 0) && (!string.IsNullOrEmpty(item.Description.ToString())))
            {


                string filepath = "~/image/DmImage/";
                //fname = item.filename.ToString(); && (!string.IsNullOrEmpty(item.base64Image.ToString()))
                //byte[] bytes = Convert.FromBase64String(item.);

                //string strm = "R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7";
                //Generate unique filename
                //string filepath = "D:/vijyeta/MilkWayIndia/MilkWayIndia/image/customer/" + item.Photo;
                // string filepath = "https://portal.milkywayindia.com/image/DmImage/" + item.Photo;
                // filepath = Path.Combine(Server.MapPath("~/image/customer/"), item.Photo);

                if (!string.IsNullOrEmpty(item.base64Image.ToString()))
                {
                    filepath = AppDomain.CurrentDomain.BaseDirectory + ("image\\DmImage\\" + item.Photo);//active
                    string filename = Path.GetFileName(filepath);

                    var bytess = Convert.FromBase64String(item.base64Image);
                    using (var imageFile = new FileStream(filepath, FileMode.Create))
                    {
                        imageFile.Write(bytess, 0, bytess.Length);
                        imageFile.Flush();
                    }
                }
                obj.Photo = item.Photo;
                obj.DeliveryBoyId = item.DeliveryBoyId;
                obj.Description = item.Description;
                result = obj.InsertProductPhoto(obj);

                if (result > 0)
                {

                    dr["imagepath"] = Helper.PhotoFolderPath + "/image/DmImage/" + obj.Photo;
                    dr["status"] = "success";
                    dr["message"] = "Photo Updated!!!";
                }
                else
                {
                    dr["imagepath"] = "";
                    dr["status"] = "failed";
                    dr["message"] = "Photo not Updated!!!";
                }

            }
            else
            {
                dr["status"] = "failed";
                dr["message"] = "Please Enter Valid Data";
            }


            dtNew.Rows.Add(dr);
            jsonstring = string.Empty;
            jsonstring = JsonConvert.SerializeObject(dtNew);
            jsonstring = jsonstring.Remove(jsonstring.Length - 1);
            jsonstring = jsonstring.Remove(0, 1);
            return Ok(dtNew);
        }

        [Route("api/DeliveryBoyApi/DMOrderEdit/{OrderId?}/{CustomerId?}/{Qty?}/{ProId?}/{newQty?}/{Proname}/{DeliveryboyId?}/{dates}")]
        [HttpGet]
        public HttpResponseMessage DMOrderEdit(int? OrderId, int? CustomerId, int? Qty, int? ProId, int? newQty, string Proname, int? DeliveryboyId, string dates)//JsonResult
        {
            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();
            Product objProduct = new Product();
            Subscription objsub = new Subscription();
            DeliveryBoy objdeliveryboy = new DeliveryBoy();
            objdeliveryboy.OrderId = Convert.ToInt32(OrderId);
            objdeliveryboy.CustomerId = Convert.ToInt32(CustomerId);
            objdeliveryboy.Qty = Convert.ToInt32(Qty);
            objdeliveryboy.ProductId = Convert.ToInt32(ProId);
            objdeliveryboy.newqty = Convert.ToInt32(newQty);
            objdeliveryboy.productname = Proname.ToString();
            objdeliveryboy.DeliveryBoyId = Convert.ToInt32(DeliveryboyId);
            objdeliveryboy.Updatedon = Convert.ToDateTime(dates);

            int addresult = 0;
            DataTable dt = new DataTable();
            dt = objdeliveryboy.getOrderqtyupdatedetail(objdeliveryboy.OrderId);

            if (dt.Rows.Count > 0)
            {

                addresult = objdeliveryboy.UpdateQtyUpdate(objdeliveryboy);
            }
            else
            {
                addresult = objdeliveryboy.InsertQtyUpdate(objdeliveryboy);
            }

            if (addresult > 0)
            {
                dr["status"] = "Success";
                dr["error_msg"] = "Updation Complete";
                dtNew.Rows.Add(dr);
            }
            else
            {
                dr["status"] = "Error";
                dr["error_msg"] = "Not Updated";
                dtNew.Rows.Add(dr);
            }


            //Calculaion part Start
            //DataTable dtedit = new DataTable();
            //dtedit = objsub.getCustomerOrderSelect(OrderId.ToString());
            //objsub.OrderId = Convert.ToInt32(OrderId);
            //objsub.CustomerId = Convert.ToInt32(CustomerId);
            //objsub.Qty = Convert.ToInt32(newQty);






            //objsub.Discount = Convert.ToDecimal(dtedit.Rows[0]["Discount"].ToString());
            //objsub.RewardPoint = Convert.ToInt64(dtedit.Rows[0]["RewardPoint"]);
            //objsub.CGSTAmount = Convert.ToDecimal(dtedit.Rows[0]["CGSTAmount"].ToString());
            //objsub.SGSTAmount = Convert.ToDecimal(dtedit.Rows[0]["SGSTAmount"].ToString());
            //objsub.IGSTAmount = Convert.ToDecimal(dtedit.Rows[0]["IGSTAmount"].ToString());
            //objsub.Profit = Convert.ToDecimal(dtedit.Rows[0]["Profit"].ToString());
            //objsub.OrderDate= Convert.ToDateTime(dtedit.Rows[0]["OrderDate"].ToString());


            //DataTable dtProduct = objProduct.BindProuct(ProId);
            //if (dtProduct.Rows.Count > 0)
            //{
            //    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SalePrice"].ToString()))
            //        objsub.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
            //    else
            //        objsub.Amount = 0;
            //}
            //objsub.Amount = ((objsub.Amount)) * Convert.ToDecimal(newQty);
            //objsub.Discount = ((objsub.Discount) / Convert.ToDecimal(Qty)) * Convert.ToDecimal(newQty);
            //objsub.RewardPoint = (Convert.ToInt64(objsub.RewardPoint) / Convert.ToInt64(Qty)) * Convert.ToInt64(newQty);
            //objsub.CGSTAmount = ((objsub.CGSTAmount) / Convert.ToDecimal(Qty)) * Convert.ToDecimal(newQty);
            //objsub.SGSTAmount = ((objsub.SGSTAmount) / Convert.ToDecimal(Qty)) * Convert.ToDecimal(newQty);
            //objsub.IGSTAmount = ((objsub.IGSTAmount) / Convert.ToDecimal(Qty)) * Convert.ToDecimal(newQty);
            //objsub.Profit = ((objsub.Profit) / Convert.ToDecimal(Qty)) * Convert.ToDecimal(newQty);

            //objsub.TotalFinalAmount = objsub.Amount - objsub.Discount;
            ////Calculation End





            //string jsonString = null;

            //objsub.Id =Convert.ToInt32(OrderId);
            //objsub.Status = "Complete";
            //int UpdateOrder = objsub.UpdateCustomerOrderMain(objsub);
            ////update item order

            //int UpdateAddProductDetail = objsub.UpdateCustomerOrderDetailMobile(objsub);
            //int j = 0;
            //if (Qty>0)
            //{
            //    j = 0;
            //    string Status = "Complete";
            //    if (Status == "Complete")
            //    {
            //        var i = objsub.CheckCustomerWalletEntry(objsub.OrderId, objsub.CustomerId);
            //        if (i == 0)
            //        {
            //            objsub.Description = "Place Order";
            //            objsub.Type = "Debit";
            //            objsub.proqty =newQty.ToString();
            //            objsub.Status = "Purchase";
            //            objsub.CustSubscriptionId = 0;
            //            objsub.TransactionType = Convert.ToInt32(Helper.TransactionType.Purchase);
            //            objsub.Description = Proname;
            //      j= objsub.InsertWalletScheduleOrder(objsub);



            //        }
            //        else
            //        {
            //           j=  objsub.UpdateCustomerWallet1(objsub);
            //        }
            //        if(j>0)
            //        {
            //            dr["status"] = "Success";
            //            dr["error_msg"] = "Updation Complete";
            //            dtNew.Rows.Add(dr);
            //        }
            //        else
            //        {

            //                dr["status"] = "Error";
            //                dr["error_msg"] = "Not Updated";
            //            dtNew.Rows.Add(dr);
            //        }

            //    }
            //}
            //else
            //{
            //    int i = objsub.CheckCustomerWalletEntry(objsub.OrderId, objsub.CustomerId);
            //    j= objsub.DeleteCustomerWallet(i);
            //    if (j > 0)
            //    {
            //        dr["status"] = "Success";
            //        dr["error_msg"] = "Updation Complete";
            //        dtNew.Rows.Add(dr);
            //    }
            //    else
            //    {

            //        dr["status"] = "Error";
            //        dr["error_msg"] = "Not Updated";
            //        dtNew.Rows.Add(dr);

            //    }
            //}
            string jsonString = null;
            jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dtNew);

            //  return jsonString;
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return response;

        }



        [Route("api/DeliveryBoyApi/DMCurrentStatusUpdate/{DeliveryboyId?}/{Status}/{Ctime}/{startdate?}")]
        [HttpGet]
        public HttpResponseMessage DMCurrentStatusUpdate(int? DeliveryboyId, string Status, string Ctime, DateTime? startdate)//JsonResult
        {
            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();
            Subscription objsub = new Subscription();

            DeliveryBoy obj = new DeliveryBoy();
            Ctime = Ctime.Replace("-", ":");
            DateTime FromDate = Convert.ToDateTime(startdate);
            string jsonString = null;

            obj.DeliveryBoyId = Convert.ToInt32(DeliveryboyId);
            obj.Status = Status.ToString();

            int j = obj.InsertDMStatus(obj.DeliveryBoyId, obj.Status, Ctime, FromDate);
            if (j > 0)
            {
                dr["status"] = "Success";
                dr["error_msg"] = "Status Updated";
                dtNew.Rows.Add(dr);
            }
            else
            {

                dr["status"] = "Error";
                dr["error_msg"] = "Status Not Updated";
                dtNew.Rows.Add(dr);

            }


            jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dtNew);

            //  return jsonString;
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return response;
        }

        [Route("api/DeliveryBoyApi/DMCashUpdate/{CustomerId?}/{id?}/{Dmid?}/{startdate?}/{CollectAmount}/{Description}")]
        [HttpGet]

        public IHttpActionResult DMCashUpdate(int CustomerId, int id, int Dmid, DateTime? startdate, string CollectAmount, string Description)//JsonResult
        {
            DeliveryBoy obj = new DeliveryBoy();
            Subscription objsub = new Subscription();
            obj.CustomerId = CustomerId;
            DateTime FromDate = Convert.ToDateTime(startdate);

            int UpdateOrder = 0;
            objsub.CustomerId = CustomerId;
            objsub.TransactionDate = Convert.ToDateTime(startdate);
            objsub.BillNo = null;
            objsub.Type = "Credit";
            objsub.CustSubscriptionId = 0;
            objsub.TransactionType = Convert.ToInt32(Helper.TransactionType.Deposit);
            objsub.Description = "Cash Given-";

            string orderid1 = "0";
            if (!string.IsNullOrEmpty(orderid1))
            {
                objsub.OrderId = Convert.ToInt32(orderid1);
            }
            objsub.Amount = Convert.ToDecimal(CollectAmount);

            objsub.Status = "";

            objsub.Cashbacktype = "";
            objsub.CashbackId = "";
            objsub.UtransactionId = "";

            //Wallet Update
            //int  addwallet = objsub.InsertWallet1(objsub);

            if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
            {
                UpdateOrder = obj.UpdateCash(CustomerId, id, Dmid, FromDate, CollectAmount, Description);
            }

            return Ok("Status Updated");
        }


        //[HttpPost]
        //[Route("api/DeliveryBoyApi/SaveFile")]
        //public HttpResponseMessage SaveFile()
        //{
        //fname = item.filename.ToString();
        //byte[] bytes = Convert.FromBase64String(item.);

        //string strm = "R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7";
        //Generate unique filename
        //string filepath = "D:/vijyeta/MilkWayIndia/MilkWayIndia/image/customer/" + item.Photo;
        // string filepath = "https://portal.milkywayindia.com/image/DmImage/" + item.Photo;
        // filepath = Path.Combine(Server.MapPath("~/image/customer/"), item.Photo);
        //    DeliveryBoy obj = new DeliveryBoy();
        //    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
        //    string path = HttpContext.Current.Server.MapPath("~/image/");
        //    //Check if Request contains File.
        //    if (HttpContext.Current.Request.Files.Count == 0)
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }

        //    //Read the File data from Request.Form collection.
        //    string dmId = HttpContext.Current.Request.Form["fileName"].ToString();
        //    string fileName="", panFile = "";
        //    string l = HttpContext.Current.Request.Files[0].ContentLength.ToString();
        //    string p = HttpContext.Current.Request.Files[1].ContentLength.ToString();
        //    if (Convert.ToInt32(l)>0)
        //    {
        //        HttpPostedFile postedFile = HttpContext.Current.Request.Files[0];
        //        fileName = "Aadhar" + postedFile.FileName;
        //        postedFile.SaveAs(path + fileName);
        //        obj.Aadhar = fileName.ToString();
        //    }
        //    if (HttpContext.Current.Request.Files[1].ContentLength >0)
        //    {
        //        HttpPostedFile pan = HttpContext.Current.Request.Files[1];
        //        panFile = "Pan" + pan.FileName;
        //        pan.SaveAs(path + panFile);
        //        obj.Pan = panFile.ToString();
        //    }
        //    else
        //    {
        //        obj.Pan = "";
        //    }





        //    return Request.CreateResponse(HttpStatusCode.OK, fileName);


        //}

        public IHttpActionResult DMdocUpload(DeliveryBoy item)
        {

            string jsonstring = null;

            DataTable dt = new DataTable();
            DataTable dtNew = new DataTable();

            int result = 0, updatepoint = 0;
            dtNew.Columns.Add("imagepath", typeof(string));
            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("message", typeof(string));
            DataRow dr = dtNew.NewRow();
            dr["status"] = "failed";
            dr["message"] = "";

            if ((!string.IsNullOrEmpty(item.DeliveryBoyId.ToString()) || item.DeliveryBoyId != 0))
            {

                DeliveryBoy obj = new DeliveryBoy();
                string filepath = "~/image/DmDoc/";
                string filename = "";


                if (!string.IsNullOrEmpty(item.base64Aadhar.ToString()))
                {
                    filepath = AppDomain.CurrentDomain.BaseDirectory + ("image\\DmDoc\\" + item.Aadhar);//active
                    filename = Path.GetFileName(filepath);

                    var bytess = Convert.FromBase64String(item.base64Aadhar);
                    using (var imageFileAadhar = new FileStream(filepath, FileMode.Create))
                    {
                        imageFileAadhar.Write(bytess, 0, bytess.Length);
                        imageFileAadhar.Flush();
                    }

                    item.Aadharstatus = "Pending";
                }

                if (!string.IsNullOrEmpty(item.base64Pan.ToString()))
                {
                    filepath = AppDomain.CurrentDomain.BaseDirectory + ("image\\DmDoc\\" + item.Pan);//active
                    filename = Path.GetFileName(filepath);

                    var bytess = Convert.FromBase64String(item.base64Pan);
                    using (var imageFilePan = new FileStream(filepath, FileMode.Create))
                    {
                        imageFilePan.Write(bytess, 0, bytess.Length);
                        imageFilePan.Flush();
                    }
                    item.Panstatus = "Pending";
                }

                if (!string.IsNullOrEmpty(item.base64License.ToString()))
                {
                    filepath = AppDomain.CurrentDomain.BaseDirectory + ("image\\DmDoc\\" + item.License);//active
                    filename = Path.GetFileName(filepath);

                    var bytess = Convert.FromBase64String(item.base64License);
                    using (var imageFileLicense = new FileStream(filepath, FileMode.Create))
                    {
                        imageFileLicense.Write(bytess, 0, bytess.Length);
                        imageFileLicense.Flush();
                    }
                    item.Licensestatus = "Pending";
                }


                obj.Aadhar = item.Aadhar;
                obj.Pan = item.Pan;
                obj.License = item.License;
                obj.DeliveryBoyId = item.DeliveryBoyId;
                obj.bankaccount = item.bankaccount;
                obj.ifsc = item.ifsc;
                obj.bankname = item.bankname;
                obj.Accholdername = item.Accholdername;

                DataTable dtDMStatusList = new DataTable();
                dtDMStatusList = obj.getDmwiseDoc(item.DeliveryBoyId);

                if (dtDMStatusList.Rows.Count > 0)
                {
                    if (dtDMStatusList.Rows[0].ItemArray[5].ToString() != obj.bankaccount)
                    {
                        obj.Bankstatus = "Pending";
                    }
                    if (dtDMStatusList.Rows[0].ItemArray[6].ToString() != obj.ifsc)
                    {
                        obj.Bankstatus = "Pending";
                    }

                    if (dtDMStatusList.Rows[0].ItemArray[7].ToString() != obj.bankname)
                    {
                        obj.Bankstatus = "Pending";
                    }

                    if (dtDMStatusList.Rows[0].ItemArray[8].ToString() != obj.Accholdername)
                    {
                        obj.Bankstatus = "Pending";
                    }

                    result = obj.UpdateDoc(obj);
                }
                else
                {
                    obj.Aadharstatus = "Pending";
                    obj.Panstatus = "Pending";
                    obj.Licensestatus = "Pending";

                    obj.Bankstatus = "Pending";
                    result = obj.InsertDoc(obj);
                }


                if (result > 0)
                {

                    dr["imagepath"] = Helper.PhotoFolderPath + "/image/DmDoc/";
                    dr["status"] = "success";
                    dr["message"] = "Photo Updated!!!";
                }
                else
                {
                    dr["imagepath"] = "";
                    dr["status"] = "failed";
                    dr["message"] = "Photo not Updated!!!";
                }

            }
            else
            {
                dr["status"] = "failed";
                dr["message"] = "Please Enter Valid Data";
            }


            dtNew.Rows.Add(dr);
            jsonstring = string.Empty;
            jsonstring = JsonConvert.SerializeObject(dtNew);
            jsonstring = jsonstring.Remove(jsonstring.Length - 1);
            jsonstring = jsonstring.Remove(0, 1);
            return Ok(dtNew);
        }


        [Route("api/DeliveryBoyApi/getDMCurrentStatus/{Deliveryboyid?}/{FDate?}/{TDate?}")]

        [HttpGet]

        public HttpResponseMessage getDMCurrentStatus(String Deliveryboyid, DateTime? FDate, DateTime? TDate)
        {
            DateTime Fromdate = DateTime.Now, Todate = DateTime.Now;

            Fromdate = Convert.ToDateTime(FDate);
            Todate = Convert.ToDateTime(TDate);
            DeliveryBoy obj = new DeliveryBoy();

            Customer objcust = new Customer();
            string jsonString1 = string.Empty;


            obj.StaffId = Convert.ToInt32(Deliveryboyid);

            DeliveryBoy order = new DeliveryBoy();

            DataTable dtNew = new DataTable();
            DataTable dtNew1 = new DataTable();
            DataTable dtList = new DataTable();
            DataTable dtList1 = new DataTable();

            dtList1 = obj.getDeliveryBoyCurrentstatus(obj.StaffId, Fromdate, Todate);
            int userRecords1 = dtList1.Rows.Count;
            if (dtList1.Rows.Count > 0)
            {
                dtNew1.Columns.Add("Deliveryboyid", typeof(string));
                dtNew1.Columns.Add("Name", typeof(string));
                dtNew1.Columns.Add("Date", typeof(string));
                dtNew1.Columns.Add("Status", typeof(string));

                for (int i = 0; i < dtList1.Rows.Count; i++)
                {
                    try
                    {
                        DataRow dr1 = dtNew1.NewRow();
                        dr1["Deliveryboyid"] = dtList1.Rows[i]["DMid"].ToString().Trim();
                        dr1["Name"] = dtList1.Rows[i]["Name"].ToString();
                        dr1["Date"] = dtList1.Rows[i]["StartDate"].ToString();
                        dr1["Status"] = dtList1.Rows[i]["CurrentStatus"].ToString();

                        dtNew1.Rows.Add(dr1);
                    }

                    catch { }


                }



                jsonString1 = JsonConvert.SerializeObject(dtNew1);
            }


            if (userRecords1 > 0)
            {

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();



                dict["status"] = "success";

                dict["DmStatus"] = jsonString1;

                string one = @"{""status"":""success""";

                string three = @",""DmStatus"":" + dict["DmStatus"];
                string four = one + three + "}";

                var str = four.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {



                dtNew1.Columns.Add("status", typeof(string));
                dtNew1.Columns.Add("msg", typeof(string));
                DataRow dr1 = dtNew1.NewRow();
                dr1["status"] = "Fail";
                dr1["msg"] = "No Record Found";
                dtNew1.Rows.Add(dr1);

                //new Newtonsoft.Json.Formatting()
                jsonString1 = string.Empty;
                jsonString1 = JsonConvert.SerializeObject(dtNew1);
                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";

                dict["DmStatus"] = jsonString1;

                string one = @"{""status"":""Fail""";

                string three = @",""DmStatus"":" + dict["DmStatus"];
                string four = one + three + "}";

                var str = four.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }

            //return Ok(dtList);
        }


        [Route("api/DeliveryBoyApi/DMGetSector/{Deliveryboyid?}")]

        [HttpGet]

        public HttpResponseMessage DMGetSector(String Deliveryboyid)
        {

            DeliveryBoy obj = new DeliveryBoy();

            Customer objcust = new Customer();
            string jsonString1 = string.Empty;


            obj.StaffId = Convert.ToInt32(Deliveryboyid);

            DeliveryBoy order = new DeliveryBoy();

            DataTable dtNew = new DataTable();
            DataTable dtNew1 = new DataTable();
            DataTable dtList = new DataTable();
            DataTable dtList1 = new DataTable();

            dtList1 = obj.getDeliveryBoySector(obj.StaffId);
            int userRecords1 = dtList1.Rows.Count;
            if (dtList1.Rows.Count > 0)
            {

                dtNew1.Columns.Add("SectorId", typeof(string));
                dtNew1.Columns.Add("SectorName", typeof(string));


                for (int i = 0; i < dtList1.Rows.Count; i++)
                {
                    try
                    {
                        DataRow dr1 = dtNew1.NewRow();
                        dr1["SectorId"] = dtList1.Rows[i]["SectorId"].ToString().Trim();
                        dr1["SectorName"] = dtList1.Rows[i]["SectorName"].ToString();


                        dtNew1.Rows.Add(dr1);
                    }

                    catch { }


                }



                jsonString1 = JsonConvert.SerializeObject(dtNew1);
            }


            if (userRecords1 > 0)
            {

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();



                dict["status"] = "success";

                dict["DmStatus"] = jsonString1;

                string one = @"{""status"":""success""";

                string three = @",""DmStatus"":" + dict["DmStatus"];
                string four = one + three + "}";

                var str = four.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {



                dtNew1.Columns.Add("status", typeof(string));
                dtNew1.Columns.Add("msg", typeof(string));
                DataRow dr1 = dtNew1.NewRow();
                dr1["status"] = "Fail";
                dr1["msg"] = "No Record Found";
                dtNew1.Rows.Add(dr1);

                //new Newtonsoft.Json.Formatting()
                jsonString1 = string.Empty;
                jsonString1 = JsonConvert.SerializeObject(dtNew1);
                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";

                dict["DmStatus"] = jsonString1;

                string one = @"{""status"":""Fail""";

                string three = @",""DmStatus"":" + dict["DmStatus"];
                string four = one + three + "}";

                var str = four.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }

            //return Ok(dtList);
        }


        [Route("api/DeliveryBoyApi/DMContactUpdate/{Deliveryboyid?}/{ContactNo?}")]
        [HttpGet]

        public IHttpActionResult DMContactUpdate(int Deliveryboyid, string ContactNo)//JsonResult
        {
            DeliveryBoy obj = new DeliveryBoy();

            obj.DeliveryBoyId = Deliveryboyid;
            obj.MobileNo = ContactNo.ToString();
            int updatecontact = 0;

            if (!string.IsNullOrEmpty(obj.DeliveryBoyId.ToString()) && !string.IsNullOrEmpty(obj.MobileNo.ToString()))
            {
                updatecontact = obj.UpdateContact(Deliveryboyid, ContactNo);
            }

            if (updatecontact > 0)
            {
                return Ok("Contact Updated");
            }
            else
            {
                return Ok("Contact Not Updated");
            }
        }



        [Route("api/DeliveryBoyApi/DMGetDocStatus/{Deliveryboyid?}")]

        [HttpGet]

        public HttpResponseMessage DMGetDocStatus(String Deliveryboyid)
        {

            DeliveryBoy obj = new DeliveryBoy();

            Customer objcust = new Customer();
            string jsonString1 = string.Empty;


            obj.StaffId = Convert.ToInt32(Deliveryboyid);

            DeliveryBoy order = new DeliveryBoy();

            DataTable dtNew = new DataTable();
            DataTable dtNew1 = new DataTable();
            DataTable dtList = new DataTable();
            DataTable dtList1 = new DataTable();

            dtList1 = obj.getDeliveryBoyDocStaus(obj.StaffId);
            int userRecords1 = dtList1.Rows.Count;
            if (dtList1.Rows.Count > 0)
            {

                dtNew1.Columns.Add("Aadhar", typeof(string));
                dtNew1.Columns.Add("Pan", typeof(string));
                dtNew1.Columns.Add("License", typeof(string));
                dtNew1.Columns.Add("BankAccount", typeof(string));
                dtNew1.Columns.Add("Ifsc", typeof(string));
                dtNew1.Columns.Add("Bankname", typeof(string));
                dtNew1.Columns.Add("AccholderName", typeof(string));
                dtNew1.Columns.Add("Status", typeof(string));
                dtNew1.Columns.Add("Decsiption", typeof(string));
                for (int i = 0; i < dtList1.Rows.Count; i++)
                {
                    try
                    {
                        DataRow dr1 = dtNew1.NewRow();

                        if (!string.IsNullOrEmpty(dtList1.Rows[0]["Aadhar"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtList1.Rows[0]["Aadhar"].ToString().Trim());
                            dr1["Aadhar"] = Helper.PhotoFolderPathtest + "/image/DmDoc/" + encoded;
                        }
                        else
                            dr1["Aadhar"] = "";

                        if (!string.IsNullOrEmpty(dtList1.Rows[0]["Pan"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtList1.Rows[0]["Pan"].ToString().Trim());
                            dr1["Pan"] = Helper.PhotoFolderPathtest + "/image/DmDoc/" + encoded;
                        }
                        else
                            dr1["Pan"] = "";

                        if (!string.IsNullOrEmpty(dtList1.Rows[0]["License"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtList1.Rows[0]["License"].ToString().Trim());
                            dr1["License"] = Helper.PhotoFolderPathtest + "/image/DmDoc/" + encoded;
                        }
                        else
                            dr1["License"] = "";


                        dr1["BankAccount"] = dtList1.Rows[i]["BankAccount"].ToString();
                        dr1["Ifsc"] = dtList1.Rows[i]["Ifsc"].ToString();
                        dr1["Bankname"] = dtList1.Rows[i]["Bankname"].ToString();
                        dr1["AccholderName"] = dtList1.Rows[i]["AccholderName"].ToString();
                        dr1["Status"] = dtList1.Rows[i]["Status"].ToString();
                        dr1["Decsiption"] = dtList1.Rows[i]["Decsiption"].ToString();


                        dtNew1.Rows.Add(dr1);
                    }

                    catch { }


                }



                jsonString1 = JsonConvert.SerializeObject(dtNew1);
            }


            if (userRecords1 > 0)
            {

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();



                dict["status"] = "success";

                dict["DmStatus"] = jsonString1;

                string one = @"{""status"":""success""";

                string three = @",""DmStatus"":" + dict["DmStatus"];
                string four = one + three + "}";

                var str = four.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {



                dtNew1.Columns.Add("status", typeof(string));
                dtNew1.Columns.Add("msg", typeof(string));
                DataRow dr1 = dtNew1.NewRow();
                dr1["status"] = "Fail";
                dr1["msg"] = "No Record Found";
                dtNew1.Rows.Add(dr1);

                //new Newtonsoft.Json.Formatting()
                jsonString1 = string.Empty;
                jsonString1 = JsonConvert.SerializeObject(dtNew1);
                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";

                dict["DmStatus"] = jsonString1;

                string one = @"{""status"":""Fail""";

                string three = @",""DmStatus"":" + dict["DmStatus"];
                string four = one + three + "}";

                var str = four.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }

            //return Ok(dtList);
        }


        [Route("api/DeliveryBoyApi/UpdatePassword/{Deliveryboyid}/{Oldpass}/{NewPass}")]
        [HttpGet]
        public HttpResponseMessage UpdatePassword(int Deliveryboyid, string Oldpass, string NewPass)
        {
            string Status = ""; string jsonString = null; string str1 = null; string str2 = null;

            DeliveryBoy obj = new DeliveryBoy();

            DataTable dtNew = new DataTable();

            DeliveryBoy objlogin = new DeliveryBoy();

            DataTable dtUser = new DataTable();



            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("msg", typeof(string));

            DataRow dr = dtNew.NewRow();
            dr["status"] = "failed";
            dr["msg"] = "";


            if (Deliveryboyid.ToString() != null)
            {


                DataTable dtpassword = new DataTable();
                dtpassword = objlogin.DeliveryBoylogin2(Deliveryboyid, Oldpass);


                if (dtpassword.Rows.Count > 0)
                {
                    //dr["status"] = "Success";
                    //dr["msg"] = "Find User";
                    //int customerId = Convert.ToInt32(dtpassword.Rows[0]["Id"]);
                    //var _customer = objlogin.BindCustomer(customerId);

                    int updPwd = obj.UpdateDmPwd(Deliveryboyid, NewPass);

                    if (updPwd > 0)
                    {
                        dr["status"] = "success";
                        dr["msg"] = "Password Updated Successfully";
                    }

                }

                else
                {
                    dr["status"] = "Failed";
                    dr["msg"] = "Old Password Mismatched";
                }
            }







            dtNew.Rows.Add(dr);
            jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dtNew);
            str1 = jsonString.ToString().Replace(@"[", "");
            str2 = str1.ToString().Replace(@"]", "");
            jsonString = str2;
            //  return jsonString;
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return response;
        }
    }
}

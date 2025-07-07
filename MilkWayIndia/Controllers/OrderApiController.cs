using MilkWayIndia.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace MilkWayIndia.Controllers
{
    public class OrderApiController : ApiController
    {
        Subscription obj = new Subscription();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        [Route("api/OrderApi/SubscriptionList")]
        [HttpGet]
        public HttpResponseMessage SubscriptionList() //JsonResult
        {
            Subscription obj = new Subscription();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.getSubscriptionList(null);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("name", typeof(string));
                dtNew.Columns.Add("days", typeof(int));
                dtNew.Columns.Add("amount", typeof(decimal));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["id"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    dr["name"] = dtprodRecord.Rows[i]["Name"].ToString().Trim();
                    dr["days"] = dtprodRecord.Rows[i]["Days"].ToString().Trim();
                    dr["amount"] = dtprodRecord.Rows[i]["Amount"].ToString().Trim();
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["subscription"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""subscription"":" + dict["subscription"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
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

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["subscription"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""subscription"":" + dict["subscription"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }

        public IHttpActionResult AddCustomerSubscription(Subscription item)
        {   //
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            dtNew.Columns.Add("msg", typeof(string));

            DataRow dr = dtNew.NewRow();
            if ((item.CustomerId != 0 && item.CustomerId.ToString() != null) && (item.SubscriptionId != 0 && item.SubscriptionId.ToString() != null))
            {
                decimal Amount = 0;
                int result = 0;
                int NoDays = 0;
                string SubscriptionBreak = "";
                obj.CustomerId = item.CustomerId;

                //get subto date from customerMAster v
                DataTable cusdt = objcust.BindCustomer(obj.CustomerId);
                string td = cusdt.Rows[0]["SubnToDate"].ToString();
                if (!string.IsNullOrEmpty(td))
                {
                    objcust.SubnToDate = DateTime.Parse(td, System.Globalization.CultureInfo.InvariantCulture);
                }


                //get No of days from Subscription master
                DataTable dtSubscribe = new DataTable();
                dtSubscribe = obj.getSubscriptionList(item.SubscriptionId);
                if (dtSubscribe.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtSubscribe.Rows[0]["Days"].ToString()))
                    {
                        NoDays = Convert.ToInt32(dtSubscribe.Rows[0]["Days"]);
                        Amount = Convert.ToDecimal(dtSubscribe.Rows[0]["Amount"]);
                        NoDays = NoDays - 1;
                    }
                }

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


                obj.SubscriptionId = item.SubscriptionId;
                obj.Amount = Amount;
                obj.PaymentStatus = "Yes";
                obj.SubscriptionStatus = "Open";

                //check alredy exits
                DataTable Checkuserexits = obj.CheckCustSubnExits(item.CustomerId, null, null, null);
                if (Walletbal > obj.Amount)
                {
                    if (Checkuserexits.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(Checkuserexits.Rows[0]["FromDate"].ToString()))
                            obj.FromDate = Convert.ToDateTime(Checkuserexits.Rows[0]["FromDate"]);
                        if (!string.IsNullOrEmpty(Checkuserexits.Rows[0]["ToDate"].ToString()))
                            obj.ToDate = Convert.ToDateTime(Checkuserexits.Rows[0]["ToDate"]);

                        //check cot off time
                        //if (DateTime.Now.Hour < 18)
                        //{
                        //    obj.FromDate = obj.FromDate.AddDays(1);
                        //}
                        //else
                        //    obj.FromDate = obj.FromDate.AddDays(2);

                        //Check Wether subFromdate is less than objcust.SubnToDate
                        if (DateTime.Now <= objcust.SubnToDate)
                        {
                            NoDays = NoDays + 1;
                            obj.ToDate = obj.ToDate.AddDays(NoDays);
                            //insert
                            result = obj.InsertSubscription(obj);
                        }
                        else
                        {
                            obj.FromDate = DateTime.Now;
                            obj.ToDate = obj.FromDate.AddDays(NoDays);
                            result = obj.InsertSubscription(obj);
                            SubscriptionBreak = "true";
                        }

                    }
                    else
                    {
                        obj.FromDate = DateTime.Now;
                        ////if (DateTime.Now.Hour < 20)
                        ////{
                        ////}
                        ////else{
                        ////    obj.FromDate = obj.FromDate.AddDays(1);
                        ////DateTime ToDate = obj.FromDate.AddDays(NoDays);}
                        obj.ToDate = obj.FromDate.AddDays(NoDays);
                        //insert
                        result = obj.InsertSubscription(obj);
                        // result = 0;
                    }
                }

                if (result > 0)
                {
                    //add wallet
                    obj.CustomerId = item.CustomerId;
                    obj.TotalBalance = Amount;
                    obj.OrderId = 0;
                    obj.BillNo = null;
                    obj.Description = "Purchase Subscription";
                    obj.Type = "Debit";
                    obj.CustSubscriptionId = result;
                    int walletresult = obj.InsertWalletMobile(obj);

                    if (Checkuserexits.Rows.Count > 0)
                    {
                        // check wether SubscriptionBreak is true
                        if (SubscriptionBreak != "true")
                        {
                            objcust.Id = item.CustomerId;
                            objcust.SubnToDate = obj.ToDate;
                            int update = objcust.UpdateCustomerToDate(objcust);
                        }
                        else
                        {
                            objcust.Id = obj.CustomerId;
                            objcust.SubnFromDate = DateTime.Now;
                            objcust.SubnToDate = DateTime.Now.AddDays(NoDays);
                            int update = objcust.UpdateCustomerFromToDate(objcust);
                        }

                    }
                    else
                    {
                        objcust.Id = item.CustomerId;
                        objcust.SubnFromDate = obj.FromDate;
                        objcust.SubnToDate = obj.ToDate;
                        int update = objcust.UpdateCustomerFromToDate(objcust);
                    }
                    dr["status"] = "Success";
                    dr["error_msg"] = "Subscription Successfully";
                }
                else
                {
                    if (Walletbal > obj.Amount)
                    {
                        dr["status"] = "Failed";
                        dr["error_msg"] = "Subscription Not Successfully";
                    }
                    else
                    {
                        dr["status"] = "Failed";
                        dr["error_msg"] = "Subscription Not Successfully ...Wallet balance is Low!";
                    }
                }

            }
            else
            {
                dr["status"] = "Failed";
                dr["error_msg"] = "Please Fill Correct Details";
            }
            dtNew.Rows.Add(dr);
            return Ok(dtNew);
        }

        [Route("api/OrderApi/CustomerWalletBalance/{CustomerId?}")]
        [HttpGet]
        public HttpResponseMessage CustomerWalletBalance(string CustomerId) //JsonResult
        {
            Subscription obj = new Subscription();
            DataTable dtNew = new DataTable();
            decimal TotalCredit = 0, TotalDebit = 0;
            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.getCustomerWallet(Convert.ToInt32(CustomerId));
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                //  dict["status"] = "success";
                if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["Amt"].ToString()))
                    TotalCredit = Convert.ToDecimal(dtprodRecord.Rows[0]["Amt"]);
                if (userRecords > 1)
                {
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[1]["Amt"].ToString()))
                        TotalDebit = Convert.ToDecimal(dtprodRecord.Rows[1]["Amt"]);
                }
                var balance = obj.GetCustomerBalace(Convert.ToInt32(CustomerId));
                dict["walletbalance"] = balance.ToString();
                //dict["walletbalance"] = (TotalCredit - TotalDebit).ToString();

                string one = @"{""status"":""success""";
                string two = @",""walletbalance"":" + dict["walletbalance"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
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

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["walletbalance"] = "0";


                string one = @"{""status"":""Fail""";
                string two = @",""walletbalance"":" + dict["walletbalance"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }

        //Customer order
        public IHttpActionResult AddCustomerOrderOld(string strjson)
        {   //
            Subscription obj = new Subscription();
            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();
            decimal TotalOrderAmt = 0, TotalGSTAmt = 0;
            long AddProductOrder = 0; int AddProductDetail = 0;
            if (!string.IsNullOrEmpty(strjson))
            {
                JObject jObject = JObject.Parse(strjson);
                string customerid = (string)jObject.SelectToken("custometid");
                string societyid = (string)jObject.SelectToken("societyid");
                decimal totalamt = (decimal)jObject.SelectToken("totalamount");
                string status = (string)jObject.SelectToken("status");
                string custsubscriptionid = (string)jObject.SelectToken("custsubscriptionid");
                string statecode = (string)jObject.SelectToken("statecode");
                decimal totalgstamt = (decimal)jObject.SelectToken("totalGSTamount");


                if (!string.IsNullOrEmpty(customerid))
                { obj.CustomerId = Convert.ToInt32(customerid); }
                else { obj.CustomerId = 0; }

                if (!string.IsNullOrEmpty(societyid))
                { obj.BuildingId = Convert.ToInt32(societyid); }
                else { obj.BuildingId = 0; }

                if (!string.IsNullOrEmpty(totalamt.ToString()))
                { obj.TotalAmount = Convert.ToDecimal(totalamt); }
                else { obj.TotalAmount = 0; }

                if (!string.IsNullOrEmpty(status))
                { obj.Status = status; }
                else { obj.Status = null; }

                if (!string.IsNullOrEmpty(custsubscriptionid))
                { obj.CustSubscriptionId = Convert.ToInt32(custsubscriptionid); }
                else { obj.CustSubscriptionId = 0; }

                if (!string.IsNullOrEmpty(statecode))
                { obj.StateCode = statecode; }
                else { obj.StateCode = null; }

                //Generate OrderNo
                //con.Open();
                //SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                //com1.CommandType = CommandType.StoredProcedure;
                //var Formno = com1.ExecuteScalar();
                //con.Close();

                //obj.OrderNo = Formno.ToString();
                //obj.OrderDate = DateTime.Now;

                DateTime FromDate = DateTime.Now;
                DateTime ToDate = DateTime.Now;
                //add order table
                //  AddProductOrder = obj.InsertCustomerOrder(obj);

                var container2 = (JContainer)JsonConvert.DeserializeObject(strjson.ToString());
                var item_details = container2["Items"];

                if (item_details != null)
                {

                    DataTable dtProductdetail = (DataTable)JsonConvert.DeserializeObject(item_details.ToString(), (typeof(DataTable)));
                    for (int i = 0; i < dtProductdetail.Rows.Count; i++)
                    {
                        int productid = Convert.ToInt32(dtProductdetail.Rows[i]["productid"]);
                        int qty = Convert.ToInt32(dtProductdetail.Rows[i]["qty"]);
                        decimal amt = Convert.ToDecimal(dtProductdetail.Rows[i]["MRPamount"]);
                        decimal discount = Convert.ToDecimal(dtProductdetail.Rows[i]["discount"]);
                        long rewardpoint = Convert.ToInt64(dtProductdetail.Rows[i]["rewardpoint"]);
                        decimal finalamt = Convert.ToDecimal(dtProductdetail.Rows[i]["finalamt"]);
                        decimal cgstamt = Convert.ToDecimal(dtProductdetail.Rows[i]["CGSTAmt"]);
                        decimal sgstamt = Convert.ToDecimal(dtProductdetail.Rows[i]["SGSTAmt"]);
                        decimal igstamt = Convert.ToDecimal(dtProductdetail.Rows[i]["IGSTAmt"]);
                        FromDate = Convert.ToDateTime(dtProductdetail.Rows[i]["Fromdate"]);
                        ToDate = Convert.ToDateTime(dtProductdetail.Rows[i]["Todate"]);

                        obj.ProductId = Convert.ToInt32(productid);
                        obj.Qty = Convert.ToInt32(qty);
                        obj.Amount = amt;
                        obj.Discount = discount;
                        obj.RewardPoint = rewardpoint;

                        for (int idate = 0; FromDate <= ToDate; idate++)
                        {
                            //int idate = 0;
                            //Generate OrderNo
                            con.Open();
                            SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                            com1.CommandType = CommandType.StoredProcedure;
                            var Formno = com1.ExecuteScalar();
                            con.Close();

                            obj.OrderNo = Convert.ToInt32(Formno);
                            obj.OrderDate = FromDate;

                            //check order by datewise inserted
                            //DataTable dtorderexits = obj.getCustomerOrder(obj.CustomerId, obj.CustSubscriptionId, obj.OrderDate);
                            //if (dtorderexits.Rows.Count > 0)
                            //{
                            //    TotalOrderAmt = Convert.ToDecimal(dtorderexits.Rows[0]["TotalAmount"]);
                            //    TotalGSTAmt = Convert.ToDecimal(dtorderexits.Rows[0]["TotalGSTAmt"]);
                            //    obj.Id = Convert.ToInt32(dtorderexits.Rows[0]["Id"]);
                            //    obj.TotalAmount = TotalOrderAmt + amt;
                            //    obj.TotalGSTAmt = TotalGSTAmt + sgstamt + cgstamt;
                            //    AddProductOrder = obj.UpdateCustomerOrder(obj);
                            //}
                            //else
                            //{
                            AddProductOrder = obj.InsertCustomerOrder(obj);
                            //}
                            //AddProductOrder = 1;
                            if (AddProductOrder > 0)
                            {
                                //if (dtorderexits.Rows.Count > 0)
                                //{
                                //    obj.OrderId = obj.Id;
                                //}
                                //else {
                                obj.OrderId = Convert.ToInt32(AddProductOrder); //}
                                obj.ProductId = Convert.ToInt32(productid);
                                obj.Qty = Convert.ToInt32(qty);

                                obj.Amount = amt;
                                obj.Discount = discount;
                                obj.RewardPoint = rewardpoint;
                                obj.TotalAmount = finalamt;
                                obj.CGSTAmount = cgstamt;
                                obj.SGSTAmount = sgstamt;
                                obj.IGSTAmount = igstamt;
                                obj.OrderItemDate = FromDate;

                                Product objproduct = new Product();
                                DataTable dtProduct = objproduct.BindProuct(obj.ProductId);
                                if (dtProduct.Rows.Count > 0)
                                {
                                    obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
                                }
                                AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                            }
                            //if (idate == 0) 
                            FromDate = FromDate.AddDays(1);
                            //else
                            //  FromDate = FromDate.AddDays(idate);
                        }
                    }
                    dr["status"] = "Success";
                    dr["error_msg"] = "Order Placed Successfully";
                }
                else
                {
                    dr["status"] = "Failed";
                    dr["error_msg"] = "Please Fill Correct Details";
                }
            }
            dtNew.Rows.Add(dr);
            return Ok(dtNew);
        }

        public IHttpActionResult AddCustomerWeekOrderOld(string strjson)
        {   //
            Subscription obj = new Subscription();
            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();
            //   decimal TotalOrderAmt = 0, TotalGSTAmt = 0;
            long AddProductOrder = 0; int AddProductDetail = 0;
            if (!string.IsNullOrEmpty(strjson))
            {
                JObject jObject = JObject.Parse(strjson);
                string customerid = (string)jObject.SelectToken("custometid");
                string societyid = (string)jObject.SelectToken("societyid");
                decimal totalamt = (decimal)jObject.SelectToken("totalamount");
                string status = (string)jObject.SelectToken("status");
                string custsubscriptionid = (string)jObject.SelectToken("custsubscriptionid");
                string statecode = (string)jObject.SelectToken("statecode");
                decimal totalgstamt = (decimal)jObject.SelectToken("totalGSTamount");

                var container3 = (JContainer)JsonConvert.DeserializeObject(strjson.ToString());
                var week = container3["week"];
                ///string week = (string)jObject.SelectToken("week");

                if (!string.IsNullOrEmpty(customerid))
                { obj.CustomerId = Convert.ToInt32(customerid); }
                else { obj.CustomerId = 0; }

                if (!string.IsNullOrEmpty(societyid))
                { obj.BuildingId = Convert.ToInt32(societyid); }
                else { obj.BuildingId = 0; }

                if (!string.IsNullOrEmpty(totalamt.ToString()))
                { obj.TotalAmount = Convert.ToDecimal(totalamt); }
                else { obj.TotalAmount = 0; }

                if (!string.IsNullOrEmpty(status))
                { obj.Status = status; }
                else { obj.Status = null; }

                if (!string.IsNullOrEmpty(custsubscriptionid))
                { obj.CustSubscriptionId = Convert.ToInt32(custsubscriptionid); }
                else { obj.CustSubscriptionId = 0; }

                if (!string.IsNullOrEmpty(statecode))
                { obj.StateCode = statecode; }
                else { obj.StateCode = null; }

                DataTable dtweekdetail = new DataTable();
                if (week != null)
                {
                    dtweekdetail = (DataTable)JsonConvert.DeserializeObject(week.ToString(), (typeof(DataTable)));
                }

                DateTime FromDate = DateTime.Now;
                DateTime ToDate = DateTime.Now;

                var container2 = (JContainer)JsonConvert.DeserializeObject(strjson.ToString());
                var item_details = container2["Items"];

                if (item_details != null)
                {
                    DataTable dtProductdetail = (DataTable)JsonConvert.DeserializeObject(item_details.ToString(), (typeof(DataTable)));
                    for (int i = 0; i < dtProductdetail.Rows.Count; i++)
                    {
                        int productid = Convert.ToInt32(dtProductdetail.Rows[i]["productid"]);
                        int qty = Convert.ToInt32(dtProductdetail.Rows[i]["qty"]);
                        decimal amt = Convert.ToDecimal(dtProductdetail.Rows[i]["MRPamount"]);
                        decimal discount = Convert.ToDecimal(dtProductdetail.Rows[i]["discount"]);
                        long rewardpoint = Convert.ToInt64(dtProductdetail.Rows[i]["rewardpoint"]);
                        decimal finalamt = Convert.ToDecimal(dtProductdetail.Rows[i]["finalamt"]);
                        decimal cgstamt = Convert.ToDecimal(dtProductdetail.Rows[i]["CGSTAmt"]);
                        decimal sgstamt = Convert.ToDecimal(dtProductdetail.Rows[i]["SGSTAmt"]);
                        decimal igstamt = Convert.ToDecimal(dtProductdetail.Rows[i]["IGSTAmt"]);
                        FromDate = Convert.ToDateTime(dtProductdetail.Rows[i]["Fromdate"]);
                        ToDate = Convert.ToDateTime(dtProductdetail.Rows[i]["Todate"]);

                        obj.ProductId = Convert.ToInt32(productid);
                        obj.Qty = Convert.ToInt32(qty);
                        obj.Amount = amt;
                        obj.Discount = discount;
                        obj.RewardPoint = rewardpoint;

                        for (int idate = 0; FromDate <= ToDate; idate++)
                        {
                            //int idate = 0;

                            obj.OrderDate = FromDate;
                            string orderday = obj.OrderDate.ToString("ddd");
                            //check week day selected
                            DataColumn[] columns = dtweekdetail.Columns.Cast<DataColumn>().ToArray();
                            bool anyFieldContains = dtweekdetail.AsEnumerable()
                                .Any(row => columns.Any(col => row[col].ToString() == orderday.ToLower()));

                            if (anyFieldContains == true)
                            {
                                //check order by datewise inserted
                                ////DataTable dtorderexits = obj.getCustomerOrder(obj.CustomerId, obj.CustSubscriptionId, obj.OrderDate);
                                ////if (dtorderexits.Rows.Count > 0)
                                ////{
                                ////    TotalOrderAmt = Convert.ToDecimal(dtorderexits.Rows[0]["TotalAmount"]);
                                ////    TotalGSTAmt = Convert.ToDecimal(dtorderexits.Rows[0]["TotalGSTAmt"]);
                                ////    obj.Id = Convert.ToInt32(dtorderexits.Rows[0]["Id"]);
                                ////    obj.TotalAmount = TotalOrderAmt + amt;
                                ////    obj.TotalGSTAmt = TotalGSTAmt + sgstamt + cgstamt;
                                ////    AddProductOrder = obj.UpdateCustomerOrder(obj);
                                ////}
                                ////else
                                ////{
                                //Generate OrderNo
                                con.Open();
                                SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                                com1.CommandType = CommandType.StoredProcedure;
                                var Formno = com1.ExecuteScalar();
                                con.Close();
                                obj.OrderNo = Convert.ToInt32(Formno);

                                AddProductOrder = obj.InsertCustomerOrder(obj);
                                ////}
                                if (AddProductOrder > 0)
                                {
                                    ////if (dtorderexits.Rows.Count > 0)
                                    ////{
                                    ////    obj.OrderId = obj.Id;
                                    ////}
                                    ////else {
                                    obj.OrderId = Convert.ToInt32(AddProductOrder); ////}
                                    obj.ProductId = Convert.ToInt32(productid);
                                    obj.Qty = Convert.ToInt32(qty);

                                    obj.Amount = amt;
                                    obj.Discount = discount;
                                    obj.RewardPoint = rewardpoint;
                                    obj.TotalAmount = finalamt;
                                    obj.CGSTAmount = cgstamt;
                                    obj.SGSTAmount = sgstamt;
                                    obj.IGSTAmount = igstamt;
                                    obj.OrderItemDate = FromDate;

                                    Product objproduct = new Product();
                                    DataTable dtProduct = objproduct.BindProuct(obj.ProductId);
                                    if (dtProduct.Rows.Count > 0)
                                    {
                                        obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
                                    }
                                    AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                                }
                            }
                            FromDate = FromDate.AddDays(1);
                        }
                    }
                    dr["status"] = "Success";
                    dr["error_msg"] = "Order Placed Successfully";
                }
                else
                {
                    dr["status"] = "Failed";
                    dr["error_msg"] = "Please Fill Correct Details";
                }
            }
            dtNew.Rows.Add(dr);
            return Ok(dtNew);
        }

        [Route("api/OrderApi/CustomerSubscriptionDateOld/{CustomerId?}")]
        [HttpGet]
        public HttpResponseMessage CustomerSubscriptionDateOld(string CustomerId) //JsonResult
        {
            Subscription obj = new Subscription();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.CheckCustSubnExits(Convert.ToInt32(CustomerId), null, null, null);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("CustomerId", typeof(int));
                dtNew.Columns.Add("SubscriptionId", typeof(int));
                dtNew.Columns.Add("Amount", typeof(decimal));
                dtNew.Columns.Add("PaymentStatus", typeof(string));
                dtNew.Columns.Add("SubscriptionStatus", typeof(string));
                dtNew.Columns.Add("Fromdate", typeof(DateTime));
                dtNew.Columns.Add("Todate", typeof(DateTime));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["CustomerId"] = Convert.ToInt32(dtprodRecord.Rows[i]["CustomerId"]);
                    dr["SubscriptionId"] = Convert.ToInt32(dtprodRecord.Rows[i]["SubscriptionId"]);
                    dr["Amount"] = Convert.ToDecimal(dtprodRecord.Rows[i]["Amount"]);
                    dr["PaymentStatus"] = dtprodRecord.Rows[i]["PaymentStatus"].ToString();
                    dr["SubscriptionStatus"] = dtprodRecord.Rows[i]["SubscriptionStatus"].ToString();
                    dr["Fromdate"] = Convert.ToDateTime(dtprodRecord.Rows[i]["SDate"]);
                    dr["Todate"] = Convert.ToDateTime(dtprodRecord.Rows[i]["EDate"]);
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                //  dict["status"] = "success";
                dict["SubscriptionDate"] = jsonString;

                string one = @"{""status"":""success""";
                string two = @",""SubscriptionDate"":" + dict["SubscriptionDate"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
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

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["SubscriptionDate"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""SubscriptionDate"":" + dict["SubscriptionDate"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }

        [Route("api/OrderApi/CustomerSubscriptionDate/{CustomerId?}")]
        [HttpGet]
        public HttpResponseMessage CustomerSubscriptionDate(string CustomerId) //JsonResult
        {
            Customer obj = new Customer();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.BindsubDateCustomer(Convert.ToInt32(CustomerId));
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("CustomerId", typeof(int));
                //dtNew.Columns.Add("SubscriptionId", typeof(int));
                //dtNew.Columns.Add("Amount", typeof(decimal));
                //dtNew.Columns.Add("PaymentStatus", typeof(string));
                //dtNew.Columns.Add("SubscriptionStatus", typeof(string));
                dtNew.Columns.Add("Fromdate", typeof(DateTime));
                dtNew.Columns.Add("Todate", typeof(DateTime));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["CustomerId"] = Convert.ToInt32(dtprodRecord.Rows[i]["Id"]);
                    //dr["SubscriptionId"] = Convert.ToInt32(dtprodRecord.Rows[i]["SubscriptionId"]);
                    //dr["Amount"] = Convert.ToDecimal(dtprodRecord.Rows[i]["Amount"]);
                    //dr["PaymentStatus"] = dtprodRecord.Rows[i]["PaymentStatus"].ToString();
                    //dr["SubscriptionStatus"] = dtprodRecord.Rows[i]["SubscriptionStatus"].ToString();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["SubnFromDate"].ToString()))
                        dr["Fromdate"] = Convert.ToDateTime(dtprodRecord.Rows[i]["SubnFromDate"]);
                    else
                        dr["Fromdate"] = "";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["SubnToDate"].ToString()))
                        dr["Todate"] = Convert.ToDateTime(dtprodRecord.Rows[i]["SubnToDate"]);
                    else
                        dr["Todate"] = "";
                    dtNew.Rows.Add(dr);
                }

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                //  dict["status"] = "success";
                dict["SubscriptionDate"] = jsonString;

                string one = @"{""status"":""success""";
                string two = @",""SubscriptionDate"":" + dict["SubscriptionDate"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
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
                //  dr["Fromdate"] = "";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["SubscriptionDate"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""SubscriptionDate"":" + dict["SubscriptionDate"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }


        #region wallet add
        public IHttpActionResult AddCustomerWallet(Subscription item)
        {   //
            Subscription obj = new Subscription();

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            dtNew.Columns.Add("msg", typeof(string));

            DataRow dr = dtNew.NewRow();
            if ((item.CustomerId != 0 && item.CustomerId.ToString() != null) && (item.Amount != 0 && item.Amount.ToString() != null))
            {
                int result = 0;
                //insert
                obj.Id = 0;
                obj.CustomerId = item.CustomerId;
                obj.TransactionDate = DateTime.Now;
                obj.Amount = item.Amount;
                obj.OrderId = 0;
                //generate bill no
                con.Open();
                SqlCommand com1 = new SqlCommand("Wallet_GetBillNo", con);
                com1.CommandType = CommandType.StoredProcedure;
                var Formno = com1.ExecuteScalar();
                con.Close();

                obj.BillNo = "MWI" + Formno.ToString();
                obj.Description = "Add To Wallet";
                obj.Type = "Credit";
                obj.CustSubscriptionId = 0;
                obj.TransactionType = Convert.ToInt32(Helper.TransactionType.Deposit);
                result = obj.InsertWalletMobile(obj);
                // result = 0;
                if (result > 0)
                {
                    dr["status"] = "Success";
                    dr["error_msg"] = "Amount Added into Wallet Successfully";
                }
                else
                {
                    dr["status"] = "Failed";
                    dr["error_msg"] = "Not Successfully";
                }
            }
            else
            {
                dr["status"] = "Failed";
                dr["error_msg"] = "Please Fill Correct Details";
            }

            dtNew.Rows.Add(dr);

            return Ok(dtNew);
        }
        #endregion

        #region Wallet Transaction list
        [Route("api/OrderApi/CustomerWalletTransaction/{CustomerId?}")]
        [HttpGet]
        public HttpResponseMessage CustomerWalletTransaction(string CustomerId) //JsonResult
        {
            Subscription obj = new Subscription();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.getCustomerWalletTrans(Convert.ToInt32(CustomerId), null, null);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("CustomerId", typeof(int));
                dtNew.Columns.Add("TransactionDate", typeof(DateTime));
                dtNew.Columns.Add("CreditAmt", typeof(decimal));
                dtNew.Columns.Add("DebitAmt", typeof(decimal));
                dtNew.Columns.Add("OrderNo", typeof(string));
                dtNew.Columns.Add("OrderDate", typeof(string));
                dtNew.Columns.Add("BillNo", typeof(string));
                dtNew.Columns.Add("Description", typeof(string));
                dtNew.Columns.Add("Type", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["CustomerId"] = Convert.ToInt32(dtprodRecord.Rows[i]["CustomerId"]);
                    dr["TransactionDate"] = Convert.ToDateTime(dtprodRecord.Rows[i]["TransactionDate"]);
                    dr["CreditAmt"] = Convert.ToDecimal(dtprodRecord.Rows[i]["CreditAmt"]);
                    dr["DebitAmt"] = Convert.ToDecimal(dtprodRecord.Rows[i]["DebitAmt"]);
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["OrderNo"].ToString()))
                        dr["OrderNo"] = dtprodRecord.Rows[i]["OrderNo"].ToString().Trim();
                    else
                        dr["OrderNo"] = "";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["OrderDate"].ToString()))
                        dr["OrderDate"] = Convert.ToDateTime(dtprodRecord.Rows[i]["OrderDate"]);
                    else
                        dr["OrderDate"] = "";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["BillNo"].ToString()))
                        dr["BillNo"] = dtprodRecord.Rows[i]["BillNo"].ToString().Trim();
                    else
                        dr["BillNo"] = "";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Description"].ToString()))
                        dr["Description"] = dtprodRecord.Rows[i]["Description"].ToString().Trim();
                    else
                        dr["Description"] = "";
                    dr["Type"] = dtprodRecord.Rows[i]["Type"].ToString().Trim();
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                //  dict["status"] = "success";
                dict["Transaction"] = jsonString;

                string one = @"{""status"":""success""";
                string two = @",""Transaction"":" + dict["Transaction"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
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

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["Transaction"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""Transaction"":" + dict["Transaction"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }
        #endregion

        [Route("api/OrderApi/CustomerWalletTransactionDatewise/{CustomerId?}/{Fromdate?}/{ToDate?}")]
        [HttpGet]
        public HttpResponseMessage CustomerWalletTransactionDatewise(string CustomerId, DateTime? Fromdate, DateTime? ToDate) //JsonResult
        {
            Subscription obj = new Subscription();
            DataTable dtNew = new DataTable();
            DataTable dtNewItem = new DataTable();
            DataTable dtprodRecord = new DataTable();

            DateTime FDate = DateTime.Now; DateTime TDate = DateTime.Now;
            //var date = Fromdate.Date;
            //ToDate = date.Date.AddHours(00).AddMinutes(00).AddSeconds(00);
            //var date1 = ToDate.Date;
            //ToDate = date1.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            //TimeSpan time = DateTime.Now.TimeOfDay;
            if (!string.IsNullOrEmpty(Fromdate.ToString()))
            {
                FDate = Convert.ToDateTime(Fromdate);
                var date = FDate.Date;
                Fromdate = date.Date.AddHours(00).AddMinutes(00).AddSeconds(00);
            }
            if (!string.IsNullOrEmpty(ToDate.ToString()))
            {
                TDate = Convert.ToDateTime(ToDate);
                var date1 = TDate.Date;
                ToDate = date1.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            dtprodRecord = obj.getCustomerWalletTrans(Convert.ToInt32(CustomerId), Fromdate, ToDate);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("CustomerId", typeof(int));
                dtNew.Columns.Add("TransactionDate", typeof(DateTime));
                dtNew.Columns.Add("itemdetail", typeof(string));

                // dtNewItem.Columns.Add("CreditAmt", typeof(decimal));
                dtNewItem.Columns.Add("Amount", typeof(decimal));
                dtNewItem.Columns.Add("OrderNo", typeof(string));
                //dtNewItem.Columns.Add("OrderDate", typeof(string));
                dtNewItem.Columns.Add("BillNo", typeof(string));
                dtNewItem.Columns.Add("Description", typeof(string));
                dtNewItem.Columns.Add("CustSubscriptionId", typeof(string));
                dtNewItem.Columns.Add("Subscription", typeof(string));
                dtNewItem.Columns.Add("Type", typeof(string));

                dtNewItem.Columns.Add("Qty", typeof(string));
                dtNewItem.Columns.Add("Cashbacktype", typeof(string));
                dtNewItem.Columns.Add("PayFor", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["CustomerId"] = Convert.ToInt32(dtprodRecord.Rows[i]["CustomerId"]);
                    dr["TransactionDate"] = Convert.ToDateTime(dtprodRecord.Rows[i]["TransactionDate"]);

                    DataTable dtitemRecord = new DataTable();
                    dtitemRecord = obj.getDateCustomerWalletTrans(Convert.ToInt32(CustomerId), Convert.ToDateTime(dr["TransactionDate"]));
                    int orderRecords = dtitemRecord.Rows.Count;

                    if (orderRecords > 0)
                    {
                        for (int j = 0; j < dtitemRecord.Rows.Count; j++)
                        {
                            DataRow dritem = dtNewItem.NewRow();
                            //  dritem["CreditAmt"] = Convert.ToDecimal(dtitemRecord.Rows[j]["CreditAmt"]);
                            dritem["Amount"] = Convert.ToDecimal(dtitemRecord.Rows[j]["Amount"]);
                            if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["OrderNo"].ToString()))
                                dritem["OrderNo"] = dtitemRecord.Rows[j]["OrderNo"].ToString().Trim();
                            else
                                dritem["OrderNo"] = "";
                            //if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["OrderDate"].ToString()))
                            //    dritem["OrderDate"] = Convert.ToDateTime(dtprodRecord.Rows[i]["OrderDate"]);
                            //else
                            //    dritem["OrderDate"] = "";
                            if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["BillNo"].ToString()))
                                dritem["BillNo"] = dtitemRecord.Rows[j]["BillNo"].ToString().Trim();
                            else
                                dritem["BillNo"] = "";
                            if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["Description"].ToString()))
                                dritem["Description"] = dtitemRecord.Rows[j]["Description"].ToString().Trim();
                            else
                                dritem["Description"] = "";
                            if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["CustSubscriptionId"].ToString()))
                                dritem["CustSubscriptionId"] = dtitemRecord.Rows[j]["CustSubscriptionId"].ToString().Trim();
                            else
                                dritem["CustSubscriptionId"] = "";
                            if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["Subscription"].ToString()))
                                dritem["Subscription"] = dtitemRecord.Rows[j]["Subscription"].ToString().Trim();
                            else
                                dritem["Subscription"] = "";
                            dritem["Type"] = dtitemRecord.Rows[j]["Type"].ToString().Trim();



                            dritem["Qty"] = dtitemRecord.Rows[j]["Qty"].ToString().Trim();

                            dritem["Cashbacktype"] = dtitemRecord.Rows[j]["Cashbacktype"].ToString().Trim();

                            dritem["PayFor"] = dtitemRecord.Rows[j]["cashbackstatus1"].ToString().Trim();
                            dtNewItem.Rows.Add(dritem);
                        }
                        string jsonString1 = string.Empty;
                        jsonString1 = JsonConvert.SerializeObject(dtNewItem); //new Newtonsoft.Json.Formatting()

                        var dict1 = new Dictionary<string, string>();
                        dict1["walletitem"] = jsonString1;

                        dr["itemdetail"] = dict1["walletitem"];
                    }
                    dtNewItem.Clear();
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                //  dict["status"] = "success";
                dict["Transaction"] = jsonString;

                string one = @"{""status"":""success""";
                string two = @",""Transaction"":" + dict["Transaction"];
                string three = one + two + "}";
                three = three.Replace("\"[", "[");
                three = three.Replace("]\"", "]");
                var str = three.ToString().Replace(@"\", "");
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

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["Transaction"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""Transaction"":" + dict["Transaction"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }

        //order history
        [Route("api/OrderApi/CustomerHistoryOrder/{CustomerId?}")]
        [HttpGet]
        public HttpResponseMessage CustomerHistoryOrder(string CustomerId) //JsonResult
        {
            Subscription obj = new Subscription();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.getCustomerOrderHis(Convert.ToInt32(CustomerId), null, null);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("OrderId", typeof(int));
                dtNew.Columns.Add("CustomerId", typeof(int));
                dtNew.Columns.Add("TotalAmount", typeof(decimal));
                // dtNew.Columns.Add("OrderNo", typeof(string));
                dtNew.Columns.Add("OrderDate", typeof(string));
                dtNew.Columns.Add("Status", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["OrderId"] = Convert.ToInt32(dtprodRecord.Rows[i]["Id"]);
                    dr["CustomerId"] = Convert.ToInt32(dtprodRecord.Rows[i]["CustomerId"]);
                    dr["TotalAmount"] = Convert.ToDecimal(dtprodRecord.Rows[i]["TotalAmount"]);
                    //if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["OrderNo"].ToString()))
                    //    dr["OrderNo"] = dtprodRecord.Rows[i]["OrderNo"].ToString().Trim();
                    //else
                    //    dr["OrderNo"] = "";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["OrderDate"].ToString()))
                        dr["OrderDate"] = Convert.ToDateTime(dtprodRecord.Rows[i]["OrderDate"]);
                    else
                        dr["OrderDate"] = "";
                    dr["Status"] = dtprodRecord.Rows[i]["Status"].ToString().Trim();
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                //  dict["status"] = "success";
                dict["HistoryOrder"] = jsonString;

                string one = @"{""status"":""success""";
                string two = @",""HistoryOrder"":" + dict["HistoryOrder"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
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

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["HistoryOrder"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""HistoryOrder"":" + dict["HistoryOrder"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }

        [Route("api/OrderApi/CustomerHistoryOrderDateWise/{CustomerId?}/{FromDate?}/{ToDate?}")]
        [HttpGet]
        public HttpResponseMessage CustomerHistoryOrderDateWise(string CustomerId, DateTime? FromDate, DateTime? ToDate) //JsonResult
        {
            Subscription obj = new Subscription();
            DataTable dtNew = new DataTable();
            DataTable dtNewItem = new DataTable();

            DataTable dtNewItem1 = new DataTable();
            // var attstr1 = ""; string attstr = "";
            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.getCustomerOrderHis(Convert.ToInt32(CustomerId), FromDate, ToDate);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                // dtNew.Columns.Add("OrderId", typeof(int));
                dtNew.Columns.Add("CustomerId", typeof(int));
                // dtNew.Columns.Add("TotalAmount", typeof(decimal));
                // dtNew.Columns.Add("OrderNo", typeof(string));
                dtNew.Columns.Add("OrderDate", typeof(DateTime));
                dtNew.Columns.Add("Status", typeof(string));
                dtNew.Columns.Add("itemdetail", typeof(string));

                dtNewItem.Columns.Add("Id", typeof(int));
                dtNewItem.Columns.Add("OrderDate", typeof(DateTime));
                //dtNewItem.Columns.Add("CustomerId", typeof(int));
                dtNewItem.Columns.Add("ProductId", typeof(int));
                dtNewItem.Columns.Add("ProductName", typeof(string));
                dtNewItem.Columns.Add("Qty", typeof(int));
                dtNewItem.Columns.Add("Amount", typeof(decimal));
                //dtNewItem.Columns.Add("records", typeof(string));

                dtNewItem1.Columns.Add("records", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();

                    // dr["status"] = "Success";
                    //dr["OrderId"] = Convert.ToInt32(dtprodRecord.Rows[i]["Id"]);
                    dr["CustomerId"] = Convert.ToInt32(dtprodRecord.Rows[i]["CustomerId"]);
                    // dr["TotalAmount"] = Convert.ToDecimal(dtprodRecord.Rows[i]["TotalAmount"]);
                    //if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["OrderNo"].ToString()))
                    //    dr["OrderNo"] = dtprodRecord.Rows[i]["OrderNo"].ToString().Trim();
                    //else
                    //    dr["OrderNo"] = "";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["OrderDate"].ToString()))
                        dr["OrderDate"] = Convert.ToDateTime(dtprodRecord.Rows[i]["OrderDate"]);
                    else
                        dr["OrderDate"] = "";
                    dr["Status"] = dtprodRecord.Rows[i]["Status"].ToString().Trim();

                    DataTable dtitemRecord = new DataTable();
                    dtitemRecord = obj.getCustomerHisOrderDetail(Convert.ToInt32(CustomerId), Convert.ToDateTime(dr["OrderDate"]));
                    int orderRecords = dtitemRecord.Rows.Count;

                    if (orderRecords > 0)
                    {
                        for (int j = 0; j < dtitemRecord.Rows.Count; j++)
                        {

                            DataRow dritem = dtNewItem.NewRow();
                            if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["Id"].ToString()))
                                dritem["Id"] = Convert.ToInt32(dtitemRecord.Rows[j]["Id"]);
                            else
                                dritem["Id"] = "0";
                            if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["OrderDate"].ToString()))
                                dritem["OrderDate"] = Convert.ToDateTime(dtitemRecord.Rows[j]["OrderDate"]);
                            else
                                dritem["OrderDate"] = "0";
                            //if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["CustomerId"].ToString()))
                            //    dritem["CustomerId"] = Convert.ToInt32(dtitemRecord.Rows[j]["CustomerId"]);
                            //else
                            //    dritem["CustomerId"] = "0";
                            if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["ProductId"].ToString()))
                                dritem["ProductId"] = Convert.ToInt32(dtitemRecord.Rows[j]["ProductId"]);
                            else
                                dritem["ProductId"] = "0";
                            if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["ProductName"].ToString()))
                                dritem["ProductName"] = dtitemRecord.Rows[j]["ProductName"].ToString();
                            else
                                dritem["ProductName"] = "";
                            if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["Qty"].ToString()))
                                dritem["Qty"] = Convert.ToInt32(dtitemRecord.Rows[j]["Qty"]);
                            else
                                dritem["Qty"] = "0";
                            if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["Amount"].ToString()))
                                dritem["Amount"] = Convert.ToDecimal(dtitemRecord.Rows[j]["Amount"]);
                            else
                                dritem["Amount"] = "0";
                            //   string ordervalue = string.Empty;
                            //   ordervalue = JsonConvert.SerializeObject(dtitemRecord);
                            //  //dritem["records"] = ordervalue;

                            //   DataRow dritem1 = dtNewItem1.NewRow();
                            ////   dritem1["records"] = ordervalue;
                            //   dtNewItem1.Rows.Add(ordervalue);

                            //if (dtNewItem.Rows.Count > 0)
                            //{
                            //    string str1 = @"""" + "{";
                            //    for (int k = 0; k < dtNewItem.Rows.Count; k++)
                            //    {
                            //        //string attrnamestr = dtNewItem.Rows[k]["attributeName"].ToString().Trim();
                            //        string attrrecordstr = dtNewItem.Rows[k]["records"].ToString().Trim();

                            //        //str1 += @"""" + attrnamestr + @"""" + ":" + @"""" + attrrecordstr + @"""";
                            //        str1 += @"""" + attrrecordstr + @"""";
                            //        if (k != (dtNewItem.Rows.Count - 1))
                            //        {
                            //            str1 += ",";
                            //        }
                            //    }

                            //    str1 += "}" + @"""";
                            //    attstr = str1;
                            //    attstr1 = attstr.Replace("\"{", "a{");
                            //    attstr1 = attstr1.Replace("}\"", "}a");
                            //}
                            dtNewItem.Rows.Add(dritem);
                        }
                        string jsonString1 = string.Empty;
                        jsonString1 = JsonConvert.SerializeObject(dtNewItem); //new Newtonsoft.Json.Formatting()

                        var dict1 = new Dictionary<string, string>();
                        //  dict["status"] = "success";
                        dict1["Historyitem"] = jsonString1;

                        //string jsonStringitem = string.Empty;
                        //jsonStringitem = JsonConvert.SerializeObject(dtNewItem1);
                        //var dict1 = new Dictionary<string, string>();
                        //dict1["itemdetail"] = jsonStringitem;
                        dr["itemdetail"] = dict1["Historyitem"];
                    }
                    dtNewItem.Clear();
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                //  dict["status"] = "success";
                dict["HistoryOrder"] = jsonString;

                string one = @"{""status"":""success""";
                string two = @",""HistoryOrder"":" + dict["HistoryOrder"];
                // string four = @",""Historyitem"":" + dict["Historyitem"];
                string three = one + two + "}";
                three = three.Replace("\"[", "[");
                three = three.Replace("]\"", "]");
                var str = three.ToString().Replace(@"\", "");

                //   str = str.ToString().Replace("]\\"\", ']');

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

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["HistoryOrder"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""HistoryOrder"":" + dict["HistoryOrder"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }

        //Future order
        [Route("api/OrderApi/CustomerFutureOrder/{CustomerId?}")]
        [HttpGet]
        public HttpResponseMessage CustomerFutureOrder(string CustomerId) //JsonResult
        {
            Subscription obj = new Subscription();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.getCustomerOrderFuture(Convert.ToInt32(CustomerId), null, null);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("OrderId", typeof(int));
                dtNew.Columns.Add("CustomerId", typeof(int));
                dtNew.Columns.Add("TotalAmount", typeof(decimal));
                //dtNew.Columns.Add("OrderNo", typeof(string));
                dtNew.Columns.Add("OrderDate", typeof(string));
                dtNew.Columns.Add("Status", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["OrderId"] = Convert.ToInt32(dtprodRecord.Rows[i]["Id"]);
                    dr["CustomerId"] = Convert.ToInt32(dtprodRecord.Rows[i]["CustomerId"]);
                    dr["TotalAmount"] = Convert.ToDecimal(dtprodRecord.Rows[i]["TotalAmount"]);
                    //if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["OrderNo"].ToString()))
                    //    dr["OrderNo"] = dtprodRecord.Rows[i]["OrderNo"].ToString().Trim();
                    //else
                    //    dr["OrderNo"] = "";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["OrderDate"].ToString()))
                        dr["OrderDate"] = Convert.ToDateTime(dtprodRecord.Rows[i]["OrderDate"]);
                    else
                        dr["OrderDate"] = "";
                    dr["Status"] = dtprodRecord.Rows[i]["Status"].ToString().Trim();
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                //  dict["status"] = "success";
                dict["FutureOrder"] = jsonString;

                string one = @"{""status"":""success""";
                string two = @",""FutureOrder"":" + dict["FutureOrder"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
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

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["FutureOrder"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""FutureOrder"":" + dict["FutureOrder"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }

        [Route("api/OrderApi/CustomerFutureOrderDateWise/{CustomerId?}/{FromDate?}/{ToDate?}")]
        [HttpGet]
        public HttpResponseMessage CustomerFutureOrderDateWise(string CustomerId, DateTime? FromDate, DateTime? ToDate) //JsonResult
        {
            Subscription obj = new Subscription();
            DataTable dtNew = new DataTable();
            DataTable dtNewItem = new DataTable();

            DataTable dtNewItem1 = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.getCustomerOrderFuture(Convert.ToInt32(CustomerId), FromDate, ToDate);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                // dtNew.Columns.Add("OrderId", typeof(int));
                dtNew.Columns.Add("CustomerId", typeof(int));
                // dtNew.Columns.Add("TotalAmount", typeof(decimal));
                // dtNew.Columns.Add("OrderNo", typeof(string));
                dtNew.Columns.Add("OrderDate", typeof(DateTime));
                dtNew.Columns.Add("Status", typeof(string));
                dtNew.Columns.Add("itemdetail", typeof(string));

                dtNewItem.Columns.Add("Id", typeof(int));
                dtNewItem.Columns.Add("OrderDate", typeof(DateTime));
                //dtNewItem.Columns.Add("CustomerId", typeof(int));
                dtNewItem.Columns.Add("ProductId", typeof(int));
                dtNewItem.Columns.Add("ProductName", typeof(string));
                dtNewItem.Columns.Add("Qty", typeof(int));
                dtNewItem.Columns.Add("Amount", typeof(decimal));
                //dtNewItem.Columns.Add("records", typeof(string));
                dtNewItem.Columns.Add("OrderFlag", typeof(string));
                dtNewItem1.Columns.Add("records", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    //dr["OrderId"] = Convert.ToInt32(dtprodRecord.Rows[i]["Id"]);
                    dr["CustomerId"] = Convert.ToInt32(dtprodRecord.Rows[i]["CustomerId"]);
                    //dr["TotalAmount"] = Convert.ToDecimal(dtprodRecord.Rows[i]["TotalAmount"]);
                    //if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["OrderNo"].ToString()))
                    //    dr["OrderNo"] = dtprodRecord.Rows[i]["OrderNo"].ToString().Trim();
                    //else
                    //    dr["OrderNo"] = "";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["OrderDate"].ToString()))
                        dr["OrderDate"] = Convert.ToDateTime(dtprodRecord.Rows[i]["OrderDate"]);
                    else
                        dr["OrderDate"] = "";
                    dr["Status"] = dtprodRecord.Rows[i]["Status"].ToString().Trim();
                    DataTable dtitemRecord = new DataTable();
                    dtitemRecord = obj.getCustomerOrderDetail(Convert.ToInt32(CustomerId), Convert.ToDateTime(dr["OrderDate"]));
                    int orderRecords = dtitemRecord.Rows.Count;

                    if (orderRecords > 0)
                    {
                        for (int j = 0; j < dtitemRecord.Rows.Count; j++)
                        {

                            DataRow dritem = dtNewItem.NewRow();
                            if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["Id"].ToString()))
                                dritem["Id"] = Convert.ToInt32(dtitemRecord.Rows[j]["Id"]);
                            else
                                dritem["Id"] = "0";
                            if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["OrderDate"].ToString()))
                                dritem["OrderDate"] = Convert.ToDateTime(dtitemRecord.Rows[j]["OrderDate"]);
                            else
                                dritem["OrderDate"] = "0";
                            //if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["CustomerId"].ToString()))
                            //    dritem["CustomerId"] = Convert.ToInt32(dtitemRecord.Rows[j]["CustomerId"]);
                            //else
                            //    dritem["CustomerId"] = "0";
                            if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["ProductId"].ToString()))
                                dritem["ProductId"] = Convert.ToInt32(dtitemRecord.Rows[j]["ProductId"]);
                            else
                                dritem["ProductId"] = "0";
                            if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["ProductName"].ToString()))
                                dritem["ProductName"] = dtitemRecord.Rows[j]["ProductName"].ToString();
                            else
                                dritem["ProductName"] = "";
                            if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["Qty"].ToString()))
                                dritem["Qty"] = Convert.ToInt32(dtitemRecord.Rows[j]["Qty"]);
                            else
                                dritem["Qty"] = "0";
                            if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["Amount"].ToString()))
                                dritem["Amount"] = Convert.ToDecimal(dtitemRecord.Rows[j]["Amount"]);
                            else
                                dritem["Amount"] = "0";

                            if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["OrderFlag"].ToString()))
                                dritem["OrderFlag"] = dtitemRecord.Rows[j]["OrderFlag"].ToString();
                            else
                                dritem["OrderFlag"] = "NoFlag";

                            dtNewItem.Rows.Add(dritem);
                        }
                        string jsonString1 = string.Empty;
                        jsonString1 = JsonConvert.SerializeObject(dtNewItem); //new Newtonsoft.Json.Formatting()

                        var dict1 = new Dictionary<string, string>();
                        dict1["Historyitem"] = jsonString1;
                        dr["itemdetail"] = dict1["Historyitem"];
                    }
                    dtNewItem.Clear();
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                //  dict["status"] = "success";
                dict["FutureOrder"] = jsonString;

                string one = @"{""status"":""success""";
                string two = @",""FutureOrder"":" + dict["FutureOrder"];
                string three = one + two + "}";
                three = three.Replace("\"[", "[");
                three = three.Replace("]\"", "]");
                var str = three.ToString().Replace(@"\", "");

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

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["FutureOrder"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""FutureOrder"":" + dict["FutureOrder"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }

        //single order detail
        [Route("api/OrderApi/OrderDetail/{CustomerId?}/{OrderDate?}")]
        [HttpGet]
        public HttpResponseMessage OrderDetail(string CustomerId, DateTime OrderDate) //JsonResult
        {
            Subscription obj = new Subscription();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.getCustomerOrderDetail(Convert.ToInt32(CustomerId), OrderDate);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                //dtNew.Columns.Add("OrderId", typeof(int));
                //dtNew.Columns.Add("CustomerId", typeof(int));
                //dtNew.Columns.Add("TotalAmount", typeof(decimal));
                //dtNew.Columns.Add("OrderNo", typeof(string));
                //dtNew.Columns.Add("OrderDate", typeof(string));
                //dtNew.Columns.Add("Status", typeof(string));

                dtNew.Columns.Add("Id", typeof(int));
                dtNew.Columns.Add("OrderDate", typeof(DateTime));
                dtNew.Columns.Add("CustomerId", typeof(int));
                dtNew.Columns.Add("ProductId", typeof(int));
                dtNew.Columns.Add("ProductName", typeof(string));
                dtNew.Columns.Add("Qty", typeof(int));
                dtNew.Columns.Add("Amount", typeof(decimal));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    DataRow dr = dtNew.NewRow();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Id"].ToString()))
                        dr["Id"] = Convert.ToInt32(dtprodRecord.Rows[i]["Id"]);
                    else
                        dr["Id"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["OrderDate"].ToString()))
                        dr["OrderDate"] = Convert.ToDateTime(dtprodRecord.Rows[i]["OrderDate"]);
                    else
                        dr["OrderDate"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["CustomerId"].ToString()))
                        dr["CustomerId"] = Convert.ToInt32(dtprodRecord.Rows[i]["CustomerId"]);
                    else
                        dr["CustomerId"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["ProductId"].ToString()))
                        dr["ProductId"] = Convert.ToInt32(dtprodRecord.Rows[i]["ProductId"]);
                    else
                        dr["ProductId"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["ProductName"].ToString()))
                        dr["ProductName"] = dtprodRecord.Rows[i]["ProductName"].ToString();
                    else
                        dr["ProductName"] = "";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Qty"].ToString()))
                        dr["Qty"] = Convert.ToInt32(dtprodRecord.Rows[i]["Qty"]);
                    else
                        dr["Qty"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Amount"].ToString()))
                        dr["Amount"] = Convert.ToDecimal(dtprodRecord.Rows[i]["Amount"]);
                    else
                        dr["Amount"] = "0";
                    dtNew.Rows.Add(dr);
                }

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                //  dict["status"] = "success";
                dict["OrderDetail"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""OrderDetail"":" + dict["OrderDetail"];
                ///   string three = @",""OrderId: " + OId + ",\"CustomerId:\"" + ":" + CustomerId + ",\"TotalAmount\"" + ":" + TotalAmount + ",\"OrderNo\"" + ":" + OrderNo + ",\"OrderDate\"" + ":" + Odate + "," + "\"Status\"" + ":" + status;

                string four = one + two + "}";
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

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["OrderDetail"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""OrderDetail"":" + dict["OrderDetail"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }

        //cancle order
        public IHttpActionResult CustomerOrderCancle(Subscription item)
        {   //
            Subscription obj = new Subscription();
            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();
            if (!string.IsNullOrEmpty(item.CustomerId.ToString()) && !string.IsNullOrEmpty(item.OrderDate.ToString()))
            {
                //update order status as Canclelled
                int result = item.UpdateCustomerOrderCancle(null, item.CustomerId, item.OrderDate, "Cancle");
            }
            else
            {
                dr["status"] = "failed";
                dr["error_msg"] = "Please Enter Valid Data";
            }
            dtNew.Rows.Add(dr);
            return Ok(dtNew);
        }

        //working Customer order Daily
        public IHttpActionResult AddCustomerOrder(string customerid, string productid, string qty, string AttributeId, string VendorId, string VendorcatId, string SectorId,string CusType)//string strjson
        {
            //Get CurrentTime

            DateTime centuryBegin = new DateTime(2001, 1, 1);
            var currentDate = Helper.indianTime;
            long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
            TimeSpan elapsedSpan = TimeSpan.Parse(currentDate.ToString("HH:mm"));
            string curhour = elapsedSpan.ToString();
            curhour = curhour.Substring(0, 2);
            //
            decimal minorderamount = 0, dailyorderamount = 0;
            decimal credit = 0;
            decimal Walletbal = 0, TotalCredit = 0, TotalDebit = 0;
            //
            Vendor objvendor = new Vendor();
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();
            DataTable dtNew = new DataTable();
            DataTable dtVendor = new DataTable();

            dtVendor = objvendor.getVendorid(Convert.ToInt32(VendorcatId));
            if (dtVendor.Rows.Count > 0)
            {
                minorderamount = Convert.ToDecimal(dtVendor.Rows[0]["MinAmount"]);
            }
            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            long AddProductOrder = 0; int AddProductDetail = 0;
            if (!string.IsNullOrEmpty(customerid) && Convert.ToInt32(customerid) != 0 && !string.IsNullOrEmpty(productid) && Convert.ToInt32(productid) != 0 && !string.IsNullOrEmpty(qty) && Convert.ToInt32(qty) != 0)
            {
                //check subscription cutt off time
                int r = 0;
                string msg = "Order Placed Successfully";
                string msg1 = "";

                DataTable dtcuttime = objcust.GetSchedularTime(null);
                int dbcutOfftime = Convert.ToInt32(dtcuttime.Rows[0]["CutOffTime"]);
                DateTime FromDate = Helper.indianTime;
                DateTime FromDate1 = Helper.indianTime;
                //if (Helper.indianTime.Hour < dbcutOfftime)
                if (Convert.ToInt32(curhour) < dbcutOfftime)
                {
                    FromDate = FromDate.AddDays(1);
                    //New
                    //var timming = objproduct.CheckProductOrderTimimg(Convert.ToInt32(productid));
                    var timming = objproduct.CheckProductOrderTimimgNew(Convert.ToInt32(productid), Convert.ToInt32(VendorcatId));
                    if (timming.IsTime == false)
                    {
                        FromDate = FromDate.AddDays(1);
                        msg = timming.message + "So your order successfully placed on " + FromDate.Date;
                       
                    }
                    //New End
                }
                else
                    FromDate = FromDate.AddDays(2);
                //// DateTime FromDate = DateTime.Now.AddDays(1);
                DateTime ToDate = Helper.indianTime.AddDays(1);

                if (!string.IsNullOrEmpty(customerid))
                { obj.CustomerId = Convert.ToInt32(customerid); }
                else { obj.CustomerId = 0; }

                //r = FromDate.Date.CompareTo(FromDate1.Date);
                //if (r==0)
                //{
                //    var timming = objproduct.CheckProductOrderTimimg(Convert.ToInt32(productid));
                //    if (timming.IsTime == false)
                //    {
                //        dr["status"] = "Fail";
                //        dr["error_msg"] = timming.message;
                //        dtNew.Rows.Add(dr);
                //        return Ok(dtNew);
                //    }
                //}
                
                DataTable dtDupliAssign = objcust.DuplicateStaffCustomer(null, obj.CustomerId);
                if (dtDupliAssign.Rows.Count == 0)
                {
                    dr["status"] = "Fail";
                    dr["error_msg"] = "Deliveryboy not assign...";
                    dtNew.Rows.Add(dr);
                    return Ok(dtNew);
                }
                else
                { obj.DeliveryBoyId = dtDupliAssign.Rows[0]["StaffId"].ToString(); }
                //get subscription Id
                // int custsubscriptionid = 0;
                int societyid = 0;
                //DataTable dtCustSubscription = obj.CheckCustSubnExits(obj.CustomerId, null, null, null);
                //if (dtCustSubscription.Rows.Count > 0)
                //{
                //    if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                //        custsubscriptionid = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                //    if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["ToDate"].ToString()))
                //        ToDate = Convert.ToDateTime(dtCustSubscription.Rows[0]["ToDate"].ToString());
                //    ToDate = ToDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                //}
                //get todate from customer table 
                //DataTable dtCustSubscriptiondate = objcust.BindsubDateCustomer(obj.CustomerId);
                //if (dtCustSubscriptiondate.Rows.Count > 0)
                //{
                //    if (!string.IsNullOrEmpty(dtCustSubscriptiondate.Rows[0]["SubnToDate"].ToString()))
                //        ToDate = Convert.ToDateTime(dtCustSubscriptiondate.Rows[0]["SubnToDate"].ToString());
                //    var date = ToDate.Date;
                //    ToDate = date.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                //}
                ToDate = Helper.indianTime.AddMonths(2);
                //if (!string.IsNullOrEmpty(custsubscriptionid.ToString()))
                //{ obj.CustSubscriptionId = Convert.ToInt32(custsubscriptionid); }
                //else { obj.CustSubscriptionId = 0; }

                //get BuildingId
                DataTable dtBuildingId = objcust.BindCustomer(obj.CustomerId);
                if (dtBuildingId.Rows.Count > 0)
                {
                    //if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["BuildingId"].ToString()))
                    //    societyid = Convert.ToInt32(dtBuildingId.Rows[0]["BuildingId"]);

                    societyid = 0;
                    if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["IsActive"].ToString()))
                    {
                        if (dtBuildingId.Rows[0]["IsActive"].ToString() == "False")
                        {
                            dr["status"] = "Fail";
                            dr["error_msg"] = "Account Not Active...";
                            dtNew.Rows.Add(dr);
                            return Ok(dtNew);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(societyid.ToString()))
                { obj.BuildingId = Convert.ToInt32(societyid); }
                else { obj.BuildingId = 0; }

                obj.Qty = Convert.ToInt32(qty);

                obj.ProductId = Convert.ToInt32(productid);
                //get Product detail
                DataTable dtProduct = objproduct.BindProuctOrder(obj.ProductId, Convert.ToInt32(AttributeId), Convert.ToInt32(VendorId), Convert.ToInt32(VendorcatId));
                if (dtProduct.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["ProductID"].ToString()))
                        productid = dtProduct.Rows[0]["ProductID"].ToString();
                    //if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SellPrice"].ToString()))
                    //{
                    //    obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]) * obj.Qty;
                    //    obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]);
                    //}
                    //else
                    //{
                    //    obj.Amount = 0;
                    //    obj.SalePrice = 0;
                    //}

                    if (string.IsNullOrEmpty(CusType) || CusType == "General")
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SellPrice"].ToString()))
                        {
                            obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]) * obj.Qty;
                            obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]);
                        }
                        else
                        {
                            obj.Amount = 0;
                            obj.SalePrice = 0;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["B2BSellPrice"].ToString()))
                        {
                            obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["B2BSellPrice"]) * obj.Qty;
                            obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["B2BSellPrice"]);
                        }
                        else
                        {
                            obj.Amount = 0;
                            obj.SalePrice = 0;
                        }
                    }

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["PurchasePrice"].ToString()))
                    {
                        obj.PurchasePrice = Convert.ToDecimal(dtProduct.Rows[0]["PurchasePrice"]) * obj.Qty;

                    }
                    else
                    {
                        obj.PurchasePrice = 0;

                    }

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["MRPPrice"].ToString()))
                    {
                        obj.MRPPrice = Convert.ToDecimal(dtProduct.Rows[0]["MRPPrice"]) * obj.Qty;

                    }
                    else
                    {
                        obj.MRPPrice = 0;

                    }
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountPrice"].ToString()))
                        obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountPrice"]) * obj.Qty;
                    else
                        obj.Discount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                        obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                    else
                        obj.CGSTPerct = 0;
                    //count cgst Amount
                    if (obj.CGSTPerct > 0)
                        obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                    else
                        obj.CGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                        obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                    else
                        obj.IGSTPerct = 0;
                    //count igst Amount
                    if (obj.IGSTPerct > 0)
                        obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                    else
                        obj.IGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                        obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                    else
                        obj.SGSTPerct = 0;
                    //count sgst Amount
                    if (obj.SGSTPerct > 0)
                        obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                    else
                        obj.SGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                        obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                    else
                        obj.RewardPoint = 0;
                    if (string.IsNullOrEmpty(CusType) || CusType == "General")
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                            obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                        else
                            obj.Profit = 0;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["B2BProfit"].ToString()))
                            obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["B2BProfit"]) * obj.Qty;
                        else
                            obj.Profit = 0;
                    }

                    //Final Amount
                    obj.TotalFinalAmount = obj.Amount;
                }

                if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                { obj.TotalAmount = obj.TotalFinalAmount; }
                else { obj.TotalAmount = 0; }

                obj.TotalGSTAmt = obj.CGSTAmount + obj.SGSTAmount;

                obj.Status = "Pending";
                obj.StateCode = null;
                obj.ProductId = Convert.ToInt32(productid);

                obj.OrderFlag = "Daily";
                //var checkAmount = objproduct.CheckProductOrderAmount(Convert.ToInt32(productid), obj.TotalAmount);
                //if (checkAmount.IsOrderAmount == false)
                //{
                //    dr["status"] = "Fail";
                //    dr["error_msg"] = checkAmount.message;
                //    dtNew.Rows.Add(dr);
                //    return Ok(dtNew);
                //}


                //Calculate wallet with Order Amount 02-01-2022
                DataTable dt = new DataTable();
                dt = obj.GetCustomerCredit(obj.CustomerId);

                if (dt.Rows.Count > 0)
                {
                    credit = Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString());
                }
                DataTable dtprodRecord1 = obj.getCustomerWallet(obj.CustomerId);
                int userRecords = dtprodRecord1.Rows.Count;
                if (userRecords > 0)
                {
                    if (!string.IsNullOrEmpty(dtprodRecord1.Rows[0]["Amt"].ToString()))
                        TotalCredit = Convert.ToDecimal(dtprodRecord1.Rows[0]["Amt"]);
                    if (userRecords > 1)
                    {
                        if (!string.IsNullOrEmpty(dtprodRecord1.Rows[1]["Amt"].ToString()))
                            TotalDebit = Convert.ToDecimal(dtprodRecord1.Rows[1]["Amt"]);
                    }
                    Walletbal = TotalCredit - TotalDebit;
                }
                if ((dailyorderamount + obj.TotalAmount) < minorderamount)
                {
                    if ((dailyorderamount + obj.TotalAmount) < (Walletbal + credit))
                    {
                        msg1 = "Minimum Order Value for this Category is " + minorderamount;
                    }

                    if ((dailyorderamount + obj.TotalAmount) > (Walletbal + credit))
                    {
                        msg1 += ".wallet balance is lower than order value please upload balance before 10:00 PM to process this order ";
                    }
                }

                if ((dailyorderamount + obj.TotalAmount) > minorderamount)
                {
                    if ((dailyorderamount + obj.TotalAmount) > (Walletbal + credit))
                    {
                        msg1 = "wallet balance is lower than order value please upload balance before 10:00 PM to process this order ";
                    }
                }
                //send notification
                if (msg1 != "")
                {
                    Customer objcustomer = new Customer();
                    Customer objLogin = new Customer();
                    DataTable dtToken = new DataTable();
                    string customerId = obj.CustomerId.ToString();
                    dtToken = objLogin.getCustomerSector(Convert.ToInt32(customerId));
                    if (dtToken.Rows.Count > 0)
                    {
                        objcustomer.UserName = dtToken.Rows[0].ItemArray[1].ToString();
                        objcustomer.SectorId = Convert.ToInt32(dtToken.Rows[0].ItemArray[0]);
                    }

                    string type1 = "Customer";

                    objcustomer.ntitle = "Related Order";
                    objcustomer.ntext = msg1;
                    objcustomer.Photo = "";
                    objcustomer.nlink = "";
                    var addresult = objcustomer.InsertNotification(objcustomer, type1);


                    //DataTable dtToken = new DataTable();

                    int cusnot = 0;

                    objcustomer.nid = addresult.ToString();

                    cusnot = objcustomer.InsertCustomerNotification(objcustomer);

                }
                //

                DateTime _fromDate = FromDate;
                for (int idate = 0; FromDate <= ToDate; idate++)
                {
                    //Generate OrderNo
                    con.Open();
                    SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                    com1.CommandType = CommandType.StoredProcedure;
                    var Formno = com1.ExecuteScalar();
                    con.Close();

                    obj.OrderNo = Convert.ToInt32(Formno);
                    obj.OrderDate = FromDate;

                    //get Subscription Id
                    DataTable dtCustSubscription = obj.getCustomerSubscriptionOrderdate(obj);
                    if (dtCustSubscription.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                            obj.CustSubscriptionId = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                        else
                            obj.CustSubscriptionId = 0;
                    }
                    else
                        obj.CustSubscriptionId = 0;
                    obj.AttributeId = Convert.ToInt32(AttributeId);
                    obj.VendorId = Convert.ToInt32(VendorId);
                    obj.VendorCatId = Convert.ToInt32(VendorcatId);
                    obj.SectorId = Convert.ToInt32(SectorId);
                    AddProductOrder = obj.InsertCustomerOrder(obj);

                    if (AddProductOrder > 0)
                    {
                        obj.OrderId = Convert.ToInt32(AddProductOrder);
                        obj.ProductId = Convert.ToInt32(productid);
                        obj.Qty = Convert.ToInt32(qty);
                        obj.OrderItemDate = FromDate;

                        AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                    }
                    FromDate = FromDate.AddDays(1);
                }
                if (AddProductDetail > 0)
                {
                    dr["status"] = "Success";
                    dr["error_msg"] = msg.ToString();
                    string OrderFlag = "Daily";
                    Helper dHelper = new Helper();
                    // dHelper.InsertCustomerOrderTrackNew(obj.CustomerId, obj.ProductId, obj.Qty, _fromDate, ToDate,OrderFlag);
                    dHelper.InsertCustomerOrderTrackNeworder(obj.CustomerId, obj.ProductId, obj.Qty, _fromDate, ToDate, OrderFlag, Convert.ToInt32(AttributeId));
                }
                else
                {
                    dr["status"] = "Fail";
                    dr["error_msg"] = "Order Not Inserted.";
                }
            }
            else
            {
                dr["status"] = "Failed";
                dr["error_msg"] = "Please Fill Correct Details";
            }
            dtNew.Rows.Add(dr);
            return Ok(dtNew);
        }

        //Customer order weekly
        public IHttpActionResult AddCustomerWeekOrderNew(string customerid, string productid, string qty, DateTime fromDate, DateTime todate, string week, string AttributeId, string VendorId, string VendorcatId, string SectorId,string CusType)//string strjson
        {
            //Get CurrentTime

            DateTime centuryBegin = new DateTime(2001, 1, 1);
            var currentDate = Helper.indianTime;
            long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
            TimeSpan elapsedSpan = TimeSpan.Parse(currentDate.ToString("HH:mm"));
            string curhour = elapsedSpan.ToString();
            curhour = curhour.Substring(0, 2);
            //

            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            long AddProductOrder = 0; int AddProductDetail = 0;
            if (!string.IsNullOrEmpty(customerid) && Convert.ToInt32(customerid) != 0 && !string.IsNullOrEmpty(productid) && Convert.ToInt32(productid) != 0 && !string.IsNullOrEmpty(qty) && Convert.ToInt32(qty) != 0 && !string.IsNullOrEmpty(fromDate.ToString()) && !string.IsNullOrEmpty(todate.ToString()))
            {
                //JObject jObject = JObject.Parse(strjson);
                //string customerid = (string)jObject.SelectToken("custometid");
                //string productid = (string)jObject.SelectToken("productid");
                //decimal qty = (decimal)jObject.SelectToken("qty");
                //DateTime FromDate = (DateTime)jObject.SelectToken("fromdate");
                //DateTime ToDate = (DateTime)jObject.SelectToken("todate");
                DateTime FromDate = fromDate;
                DateTime ToDate = todate;

                //var container3 = (JContainer)JsonConvert.DeserializeObject(week.ToString());
                //var week = container3["week"];

                DataTable dtweekdetail = new DataTable();
                if (week != null)
                {
                    dtweekdetail = (DataTable)JsonConvert.DeserializeObject(week.ToString(), (typeof(DataTable)));
                }

                if (!string.IsNullOrEmpty(customerid))
                { obj.CustomerId = Convert.ToInt32(customerid); }
                else { obj.CustomerId = 0; }

                //get subscription Id
                int custsubscriptionid = 0; int societyid = 0;
                

                DataTable dtDupliAssign = objcust.DuplicateStaffCustomer(null, obj.CustomerId);
                if (dtDupliAssign.Rows.Count == 0)
                {
                    dr["status"] = "Fail";
                    dr["error_msg"] = "Deliveryboy not assign...";
                    dtNew.Rows.Add(dr);
                    return Ok(dtNew);
                }
                else
                { obj.DeliveryBoyId = dtDupliAssign.Rows[0]["StaffId"].ToString(); }
                //get BuildingId
                DataTable dtBuildingId = objcust.BindCustomer(obj.CustomerId);
                if (dtBuildingId.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["BuildingId"].ToString()))
                        societyid = Convert.ToInt32(dtBuildingId.Rows[0]["BuildingId"]);
                }
                if (!string.IsNullOrEmpty(societyid.ToString()))
                { obj.BuildingId = Convert.ToInt32(societyid); }
                else { obj.BuildingId = 0; }

                obj.Qty = Convert.ToInt32(qty);
                obj.ProductId = Convert.ToInt32(productid);
                //get Product detail
                DataTable dtProduct = objproduct.BindProuctOrder(obj.ProductId, Convert.ToInt32(AttributeId), Convert.ToInt32(VendorId), Convert.ToInt32(VendorcatId));
                if (dtProduct.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["ProductID"].ToString()))
                        productid = dtProduct.Rows[0]["ProductID"].ToString();
                    if (string.IsNullOrEmpty(CusType) || CusType == "General")
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SellPrice"].ToString()))
                        {
                            obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]) * obj.Qty;
                            obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]);
                        }
                        else
                        {
                            obj.Amount = 0;
                            obj.SalePrice = 0;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["B2BSellPrice"].ToString()))
                        {
                            obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["B2BSellPrice"]) * obj.Qty;
                            obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["B2BSellPrice"]);
                        }
                        else
                        {
                            obj.Amount = 0;
                            obj.SalePrice = 0;
                        }
                    }

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["PurchasePrice"].ToString()))
                    {
                        obj.PurchasePrice = Convert.ToDecimal(dtProduct.Rows[0]["PurchasePrice"]) * obj.Qty;

                    }
                    else
                    {
                        obj.PurchasePrice = 0;

                    }

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["MRPPrice"].ToString()))
                    {
                        obj.MRPPrice = Convert.ToDecimal(dtProduct.Rows[0]["MRPPrice"]) * obj.Qty;

                    }
                    else
                    {
                        obj.MRPPrice = 0;

                    }
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountPrice"].ToString()))
                        obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountPrice"]) * obj.Qty;
                    else
                        obj.Discount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                        obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                    else
                        obj.CGSTPerct = 0;
                    //count cgst Amount
                    if (obj.CGSTPerct > 0)
                        obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                    else
                        obj.CGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                        obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                    else
                        obj.IGSTPerct = 0;
                    //count igst Amount
                    if (obj.IGSTPerct > 0)
                        obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                    else
                        obj.IGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                        obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                    else
                        obj.SGSTPerct = 0;
                    //count sgst Amount
                    if (obj.SGSTPerct > 0)
                        obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                    else
                        obj.SGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                        obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                    else
                        obj.RewardPoint = 0;
                    if (string.IsNullOrEmpty(CusType) || CusType == "General")
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                            obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                        else
                            obj.Profit = 0;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["B2BProfit"].ToString()))
                            obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["B2BProfit"]) * obj.Qty;
                        else
                            obj.Profit = 0;
                    }

                    //Final Amount
                    obj.TotalFinalAmount = obj.Amount;
                }


                if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                { obj.TotalAmount = obj.TotalFinalAmount; }
                else { obj.TotalAmount = 0; }

                obj.TotalGSTAmt = obj.CGSTAmount + obj.SGSTAmount;

                obj.Status = "Pending";
                obj.StateCode = null;
                obj.ProductId = Convert.ToInt32(productid);

                obj.OrderFlag = "Week";

                for (int idate = 0; FromDate <= ToDate; idate++)
                {
                    //Generate OrderNo
                    con.Open();
                    SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                    com1.CommandType = CommandType.StoredProcedure;
                    var Formno = com1.ExecuteScalar();
                    con.Close();

                    obj.OrderNo = Convert.ToInt32(Formno);
                    obj.OrderDate = FromDate;
                    string orderday = obj.OrderDate.ToString("ddd");
                    //check week day selected
                    DataColumn[] columns = dtweekdetail.Columns.Cast<DataColumn>().ToArray();
                    bool anyFieldContains = dtweekdetail.AsEnumerable()
                        .Any(row => columns.Any(col => row[col].ToString() == orderday.ToLower()));

                    if (anyFieldContains == true)
                    {
                        //get Subscription Id
                        DataTable dtCustSubscription = obj.getCustomerSubscriptionOrderdate(obj);
                        if (dtCustSubscription.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                                obj.CustSubscriptionId = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                            else
                                obj.CustSubscriptionId = 0;
                        }
                        else
                            obj.CustSubscriptionId = 0;

                        obj.AttributeId = Convert.ToInt32(AttributeId);
                        obj.VendorId = Convert.ToInt32(VendorId);
                        obj.VendorCatId = Convert.ToInt32(VendorcatId);
                        obj.SectorId = Convert.ToInt32(SectorId);
                        AddProductOrder = obj.InsertCustomerOrder(obj);

                        if (AddProductOrder > 0)
                        {
                            obj.OrderId = Convert.ToInt32(AddProductOrder);
                            obj.ProductId = Convert.ToInt32(productid);
                            obj.Qty = Convert.ToInt32(qty);
                            obj.OrderItemDate = FromDate;

                            AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                        }
                    }
                    FromDate = FromDate.AddDays(1);
                }
                dr["status"] = "Success";
                dr["error_msg"] = "Order Placed Successfully";
            }
            else
            {
                dr["status"] = "Failed";
                dr["error_msg"] = "Please Fill Correct Details";
            }
            dtNew.Rows.Add(dr);
            return Ok(dtNew);
        }

        #region Working Create order Api
        public IHttpActionResult AddCustomerWeekOrder(Subscription item)//string strjson
        {   //

            int countday = 0;
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            long AddProductOrder = 0; int AddProductDetail = 0;
            if (!string.IsNullOrEmpty(item.CustomerId.ToString()) && Convert.ToInt32(item.CustomerId) != 0 && !string.IsNullOrEmpty(item.ProductId.ToString()) && Convert.ToInt32(item.ProductId.ToString()) != 0 && !string.IsNullOrEmpty(item.Qty.ToString()) && Convert.ToInt32(item.Qty) != 0 && !string.IsNullOrEmpty(item.FromDate.ToString()) && !string.IsNullOrEmpty(item.ToDate.ToString()))
            {
                int _productID = item.ProductId;
                int r = 0;
                //JObject jObject = JObject.Parse(strjson);
                //string customerid = (string)jObject.SelectToken("custometid");
                //string productid = (string)jObject.SelectToken("productid");
                //decimal qty = (decimal)jObject.SelectToken("qty");
                //DateTime FromDate = (DateTime)jObject.SelectToken("fromdate");
                //DateTime ToDate = (DateTime)jObject.SelectToken("todate");

                //Dibakar
                string msg = "Order Placed Successfully";
                DataTable dtcuttime = objcust.GetSchedularTime(null);
                int dbcutOfftime = Convert.ToInt32(dtcuttime.Rows[0]["CutOffTime"]);
                DateTime FromDate = item.FromDate;
                DateTime FromDate1 = DateTime.Now;
                FromDate1= FromDate1.AddDays(1);
               r = FromDate.Date.CompareTo(FromDate1.Date);
                if (r==0)
                {
                    if (DateTime.Now.Hour < dbcutOfftime)
                    {
                        //FromDate = FromDate.AddDays(1);
                        //New
                        FromDate = item.FromDate;
                        var timming = objproduct.CheckProductOrderTimimg(Convert.ToInt32(_productID));
                        if (timming.IsTime == false)
                        {
                            FromDate = FromDate.AddDays(1);
                            msg = timming.message + "So your order successfully placed on " + FromDate.Date;

                        }
                        //New End
                    }
                    else
                        FromDate = FromDate.AddDays(1);

                }
              
                     else
                {
                    FromDate = item.FromDate;
                }
                    


                //End

                //var timming = objproduct.CheckProductOrderTimimg(_productID);
                //if (timming.IsTime == false)
                //{
                //    dr["status"] = "Fail";
                //    dr["error_msg"] = timming.message;
                //    dtNew.Rows.Add(dr);
                //    return Ok(dtNew);
                //}
                DataTable dtDupliAssign = objcust.DuplicateStaffCustomer(null, item.CustomerId);
                if (dtDupliAssign.Rows.Count == 0)
                {
                    dr["status"] = "Fail";
                    dr["error_msg"] = "Deliveryboy not assign...";
                    dtNew.Rows.Add(dr);
                    return Ok(dtNew);
                }

                //DataTable dtcuttime = objcust.GetSchedularTime(null); erase
                //int dbcutOfftime = Convert.ToInt32(dtcuttime.Rows[0]["CutOffTime"]); erase
                DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                //DateTime FromDate = item.FromDate;erase
                if (indianTime.Hour < dbcutOfftime)
                {
                    //DateTime.Now.Hour
                    ////FromDate = FromDate.AddDays(1);
                }
                //else
                //    FromDate = FromDate.AddDays(1);erase
                //// DateTime FromDate = item.FromDate;
                DateTime ToDate = item.ToDate;

                //var container3 = (JContainer)JsonConvert.DeserializeObject(week.ToString());
                //var week = container3["week"];

                DataTable dtweekdetail = new DataTable();
                if (item.week != null)
                {
                    dtweekdetail = (DataTable)JsonConvert.DeserializeObject(item.week.ToString(), (typeof(DataTable)));
                }

                if (!string.IsNullOrEmpty(item.CustomerId.ToString()))
                { obj.CustomerId = Convert.ToInt32(item.CustomerId); }
                else { obj.CustomerId = 0; }

                //get subscription Id
                int custsubscriptionid = 0; int societyid = 0;
                //DataTable dtCustSubscription = obj.CheckCustSubnExits(obj.CustomerId, null, null, null);
                //if (dtCustSubscription.Rows.Count > 0)
                //{
                //    if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                //        custsubscriptionid = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                //    if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["ToDate"].ToString()))
                //        ToDate = Convert.ToDateTime(dtCustSubscription.Rows[0]["ToDate"].ToString());
                //}
                //if (!string.IsNullOrEmpty(custsubscriptionid.ToString()))
                //{ obj.CustSubscriptionId = Convert.ToInt32(custsubscriptionid); }
                //else { obj.CustSubscriptionId = 0; }

                ////DataTable dtCustSubscriptiondate = objcust.BindsubDateCustomer(obj.CustomerId);
                ////if (dtCustSubscriptiondate.Rows.Count > 0)
                ////{
                ////    if (!string.IsNullOrEmpty(dtCustSubscriptiondate.Rows[0]["SubnToDate"].ToString()))
                ////        ToDate = Convert.ToDateTime(dtCustSubscriptiondate.Rows[0]["SubnToDate"].ToString());
                ////    var date = ToDate.Date;
                ////    ToDate = date.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                ////}


                //get BuildingId
                DataTable dtBuildingId = objcust.GetAllCustomer(obj.CustomerId);
                if (dtBuildingId.Rows.Count > 0)
                {
                    //if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["BuildingId"].ToString()))
                    //    societyid = Convert.ToInt32(dtBuildingId.Rows[0]["BuildingId"]);
                    societyid = 0;
                    if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["IsActive"].ToString()))
                    {
                        if (dtBuildingId.Rows[0]["IsActive"].ToString() == "False")
                        {
                            dr["status"] = "Fail";
                            dr["error_msg"] = "Account Not Active...";
                            dtNew.Rows.Add(dr);
                            return Ok(dtNew);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(societyid.ToString()))
                { obj.BuildingId = Convert.ToInt32(societyid); }
                else { obj.BuildingId = 0; }

                obj.Qty = Convert.ToInt32(item.Qty);
                obj.ProductId = Convert.ToInt32(item.ProductId);
                //get Product detail
                DataTable dtProduct = objproduct.BindProuct(obj.ProductId);
                if (dtProduct.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Id"].ToString()))
                        item.ProductId = Convert.ToInt32(dtProduct.Rows[0]["Id"]);
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SalePrice"].ToString()))
                    {
                        obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]) * obj.Qty;
                        obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
                    }
                    else
                    {
                        obj.Amount = 0;
                        obj.SalePrice = 0;
                    }
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountAmount"].ToString()))
                        obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountAmount"]) * obj.Qty;
                    else
                        obj.Discount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                        obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                    else
                        obj.CGSTPerct = 0;
                    //count cgst Amount
                    if (obj.CGSTPerct > 0)
                        obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                    else
                        obj.CGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                        obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                    else
                        obj.IGSTPerct = 0;
                    //count igst Amount
                    if (obj.IGSTPerct > 0)
                        obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                    else
                        obj.IGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                        obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                    else
                        obj.SGSTPerct = 0;
                    //count sgst Amount
                    if (obj.SGSTPerct > 0)
                        obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                    else
                        obj.SGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                        obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                    else
                        obj.RewardPoint = 0;
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                        obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                    else
                        obj.Profit = 0;

                    //Final Amount
                    obj.TotalFinalAmount = obj.Amount;
                }


                if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                { obj.TotalAmount = obj.TotalFinalAmount; }
                else { obj.TotalAmount = 0; }

                obj.TotalGSTAmt = obj.CGSTAmount + obj.SGSTAmount;

                obj.Status = "Pending";
                obj.StateCode = null;
                obj.ProductId = Convert.ToInt32(item.ProductId);

                obj.OrderFlag = "Week";

                var checkAmount = objproduct.CheckProductOrderAmount(_productID, obj.TotalAmount);
                if (checkAmount.IsOrderAmount == false)
                {
                    dr["status"] = "Fail";
                    dr["error_msg"] = checkAmount.message;
                    dtNew.Rows.Add(dr);
                    return Ok(dtNew);
                }

                for (int idate = 0; FromDate <= ToDate; idate++)
                {
                    //Generate OrderNo
                    con.Open();
                    SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                    com1.CommandType = CommandType.StoredProcedure;
                    var Formno = com1.ExecuteScalar();
                    con.Close();

                    obj.OrderNo = Convert.ToInt32(Formno);
                    obj.OrderDate = FromDate;
                    string orderday = obj.OrderDate.ToString("ddd");

                    //check week day selected
                    DataColumn[] columns = dtweekdetail.Columns.Cast<DataColumn>().ToArray();
                    bool anyFieldContains = dtweekdetail.AsEnumerable()
                        .Any(row => columns.Any(col => row[col].ToString().ToLower() == orderday.ToLower()));

                    if (anyFieldContains == true)
                    {
                        countday++;
                        //get Subscription Id
                        DataTable dtCustSubscription = obj.getCustomerSubscriptionOrderdate(obj);
                        if (dtCustSubscription.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                                obj.CustSubscriptionId = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                            else
                                obj.CustSubscriptionId = 0;
                        }
                        else
                            obj.CustSubscriptionId = 0;

                        AddProductOrder = obj.InsertCustomerOrder(obj);

                        if (AddProductOrder > 0)
                        {
                            obj.OrderId = Convert.ToInt32(AddProductOrder);
                            obj.ProductId = Convert.ToInt32(item.ProductId);
                            obj.Qty = Convert.ToInt32(item.Qty);
                            obj.OrderItemDate = FromDate;

                            AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                            if (AddProductDetail == 0)
                                obj.DeleteTransactionOrder(AddProductOrder);
                        }
                    }
                    else if (FromDate == ToDate)
                    {
                        countday++;
                        //get Subscription Id
                        DataTable dtCustSubscription = obj.getCustomerSubscriptionOrderdate(obj);
                        if (dtCustSubscription.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                                obj.CustSubscriptionId = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                            else
                                obj.CustSubscriptionId = 0;
                        }
                        else
                            obj.CustSubscriptionId = 0;

                        AddProductOrder = obj.InsertCustomerOrder(obj);

                        if (AddProductOrder > 0)
                        {
                            obj.OrderId = Convert.ToInt32(AddProductOrder);
                            obj.ProductId = Convert.ToInt32(item.ProductId);
                            obj.Qty = Convert.ToInt32(item.Qty);
                            obj.OrderItemDate = FromDate;

                            AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                            if (AddProductDetail == 0)
                                obj.DeleteTransactionOrder(AddProductOrder);
                        }
                    }
                    FromDate = FromDate.AddDays(1);
                }
                if (AddProductDetail > 0)
                {
                    dr["status"] = "Success";
                    dr["error_msg"] = "Order Placed Successfully";
                }
                else
                {
                    if (countday >= dtweekdetail.Rows.Count)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Order Not Inserted.";
                    }
                    else
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Order Not Inserted.Please Select Valid Days.";
                    }

                }
            }
            else
            {
                dr["status"] = "Failed";
                dr["error_msg"] = "Please Fill Correct Details";
            }
            dtNew.Rows.Add(dr);
            return Ok(dtNew);
        }
        #endregion

        #region Update Customer Order Working
        //edit order
        [Route("api/OrderApi/UpdateCustomerOrder")]
        public IHttpActionResult UpdateCustomerOrder(Subscription item)//string strjson
        {   //
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();
            int UpdateProductOrder = 0; int UpdateAddProductDetail = 0;

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            if (!string.IsNullOrEmpty(item.Id.ToString()) && item.Id != 0 && !string.IsNullOrEmpty(item.Qty.ToString()) && item.Qty != 0)
            {

                obj.Id = item.Id;
                obj.Qty = item.Qty;

                //get customer Id From order
                int CustomerId = 0;
                CustomerId = obj.getOrederCustomerId(obj.Id.ToString());
                obj.CustomerId = CustomerId;

                //check wallet balance
                decimal Walletbal = 0, TotalCredit = 0, TotalDebit = 0, Total2daybal = 0;
                DataTable dtprodRecord1 = new DataTable();
                dtprodRecord1 = obj.getCustomerWallet(obj.CustomerId);
                int userRecords1 = dtprodRecord1.Rows.Count;
                if (userRecords1 > 0)
                {
                    var balance = obj.GetCustomerBalace(obj.CustomerId);
                    Walletbal = balance;

                    DataTable dtcheckbal = new DataTable();
                    if (CustomerId > 0)
                    {
                        // check balance future 2 days
                        dtcheckbal = obj.getLast2daytotal(CustomerId.ToString());
                        if (dtcheckbal.Rows.Count > 0)
                        {
                            Total2daybal = Convert.ToDecimal(dtcheckbal.Rows[0]["Total"].ToString());
                        }
                    }
                }
                //if (Walletbal < (Total2daybal))
                //{
                //    dr["status"] = "Failed";
                //    dr["error_msg"] = "Wallet balance is Low!!Can't Update Qty";
                //}
                //else
                //{
                    //if (Walletbal > obj.Amount)
                    //{
                        //get OrderId from parent to remove child records
                        DataTable dtprodRecord = new DataTable();
                        dtprodRecord = obj.getCustomerOnedateOrderList(obj.Id);
                        int userRecords = dtprodRecord.Rows.Count;
                        obj.OrderId = 0;

                        if (userRecords > 0)
                        {
                            if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["OrderId"].ToString()))
                            {
                                obj.OrderId = Convert.ToInt32(dtprodRecord.Rows[0]["OrderId"]);
                                //get Item 
                                obj.ProductId = Convert.ToInt32(dtprodRecord.Rows[0]["ProductId"]);
                                DataTable dtProduct = objproduct.BindProuct(obj.ProductId);

                                if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["OrderDate"].ToString()))
                                {
                                    DateTime CurrentDate = Helper.indianTime.AddDays(1);
                                    DateTime OrderDate = Convert.ToDateTime(dtprodRecord.Rows[0]["OrderDate"]);
                                    if (CurrentDate.Day == OrderDate.Day && CurrentDate.Month == OrderDate.Month && CurrentDate.Year == OrderDate.Year)
                                    {
                                        var timming = objproduct.CheckProductOrderTimimg(obj.ProductId);
                                        if (timming.IsTime == false)
                                        {
                                            dr["status"] = "Fail";
                                            dr["error_msg"] = "Unable to update... - " + timming.message;
                                            dtNew.Rows.Add(dr);
                                            return Ok(dtNew);
                                        }
                                    }
                                }

                                if (dtProduct.Rows.Count > 0)
                                {
                                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SalePrice"].ToString()))
                                        obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]) * obj.Qty;
                                    else
                                        obj.Amount = 0;
                                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountAmount"].ToString()))
                                        obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountAmount"]) * obj.Qty;
                                    else
                                        obj.Discount = 0;


                                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                                        obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                                    else
                                        obj.CGSTPerct = 0;
                                    //count cgst Amount
                                    if (obj.CGSTPerct > 0)
                                        obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                                    else
                                        obj.CGSTAmount = 0;

                                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                                        obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                                    else
                                        obj.IGSTPerct = 0;
                                    //count igst Amount
                                    if (obj.IGSTPerct > 0)
                                        obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                                    else
                                        obj.IGSTAmount = 0;

                                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                                        obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                                    else
                                        obj.SGSTPerct = 0;
                                    //count sgst Amount
                                    if (obj.SGSTPerct > 0)
                                        obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                                    else
                                        obj.SGSTAmount = 0;

                                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                                        obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                                    else
                                        obj.RewardPoint = 0;
                                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                                        obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                                    else
                                        obj.Profit = 0;

                                    //Final Amount
                                    obj.TotalFinalAmount = obj.Amount - obj.Discount;
                                }
                                //update order main
                                obj.Status = "Pending";

                                obj.TotalAmount = obj.TotalFinalAmount;
                                UpdateProductOrder = obj.UpdateCustomerOrderMobile(obj);

                                //update item order
                                obj.OrderId = item.Id;
                                UpdateAddProductDetail = obj.UpdateCustomerOrderDetailMobile(obj);
                            }
                            else
                            {
                                dr["status"] = "Success";
                                dr["error_msg"] = "Order Not Found";
                            }
                            if (UpdateAddProductDetail > 0)
                            {
                                dr["status"] = "Success";
                                dr["error_msg"] = "Order Updated Successfully";
                            }
                            else
                            {
                                dr["status"] = "Success";
                                dr["error_msg"] = "Order Not Updated";
                            }
                        }
                        else
                        {
                            dr["status"] = "Success";
                            dr["error_msg"] = "Order Not Found";
                        }
                    //}
                    //else
                    //{
                    //    dr["status"] = "Failed";
                    //    dr["error_msg"] = "Wallet balance is Low!!Can't Update Qty";
                    //}
                //}
            }
            else if (!string.IsNullOrEmpty(item.Id.ToString()) && item.Id != 0 && !string.IsNullOrEmpty(item.Qty.ToString()) && item.Qty == 0)
            {
                obj.Id = item.Id;
                obj.Qty = item.Qty;
                //get OrderId from parent to remove child records
                DataTable dtprodRecord = new DataTable();
                dtprodRecord = obj.getCustomerOnedateOrderList(obj.Id);
                int userRecords = dtprodRecord.Rows.Count;
                obj.OrderId = 0;

                if (userRecords > 0)
                {
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["OrderId"].ToString()))
                    {
                        obj.OrderId = Convert.ToInt32(dtprodRecord.Rows[0]["OrderId"]);

                        //get Item 
                        obj.ProductId = Convert.ToInt32(dtprodRecord.Rows[0]["ProductId"]);
                        DataTable dtProduct = objproduct.BindProuct(obj.ProductId);
                        if (dtProduct.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["OrderDate"].ToString()))
                            {
                                DateTime CurrentDate = Helper.indianTime.AddDays(1);
                                DateTime OrderDate = Convert.ToDateTime(dtprodRecord.Rows[0]["OrderDate"]);
                                if (CurrentDate.Day == OrderDate.Day && CurrentDate.Month == OrderDate.Month && CurrentDate.Year == OrderDate.Year)
                                {
                                    var timming = objproduct.CheckProductOrderTimimg(obj.ProductId);
                                    if (timming.IsTime == false)
                                    {
                                        dr["status"] = "Fail";
                                        dr["error_msg"] = "Unable to update... - " + timming.message;
                                        dtNew.Rows.Add(dr);
                                        return Ok(dtNew);
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SalePrice"].ToString()))
                                obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
                            else
                                obj.Amount = 0;
                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountAmount"].ToString()))
                                obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountAmount"]);
                            else
                                obj.Discount = 0;


                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                                obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]);
                            else
                                obj.CGSTPerct = 0;
                            //count cgst Amount
                            if (obj.CGSTPerct > 0)
                                obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                            else
                                obj.CGSTAmount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                                obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]);
                            else
                                obj.IGSTPerct = 0;
                            //count igst Amount
                            if (obj.IGSTPerct > 0)
                                obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                            else
                                obj.IGSTAmount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                                obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]);
                            else
                                obj.SGSTPerct = 0;
                            //count sgst Amount
                            if (obj.SGSTPerct > 0)
                                obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                            else
                                obj.SGSTAmount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                                obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]);
                            else
                                obj.RewardPoint = 0;
                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                                obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]);
                            else
                                obj.Profit = 0;

                            //Final Amount
                            obj.TotalFinalAmount = obj.Amount - obj.Discount;
                        }
                        //update order main
                        obj.Status = "Cancel";

                        obj.TotalAmount = obj.TotalFinalAmount;
                        UpdateProductOrder = obj.UpdateCustomerOrderMobile(obj);

                        //update item order
                        obj.OrderId = item.Id;
                        UpdateAddProductDetail = obj.UpdateCustomerOrderDetailMobile(obj);                        
                    }
                    else
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Not Found";
                    }
                    if (UpdateAddProductDetail > 0)
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Cancelled Successfully";
                    }
                    else
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Not Cancelled";
                    }
                }
                else
                {
                    dr["status"] = "Success";
                    dr["error_msg"] = "Order Not Found";
                }
            }
            else
            {
                dr["status"] = "Failed";
                dr["error_msg"] = "Please Fill Correct Details";
            }
            dtNew.Rows.Add(dr);
            return Ok(dtNew);
        }
        #endregion

        //delete order
        [Route("api/OrderApi/CustomerDailyOrderDelete")]
        public IHttpActionResult CustomerDailyOrderDelete(Subscription item)//string strjson
        {   //
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            if (!string.IsNullOrEmpty(item.CustomerId.ToString()) && item.CustomerId != 0 && !string.IsNullOrEmpty(item.ProductId.ToString()) && item.ProductId != 0)
            {
                DateTime Currentdate = Helper.indianTime;
                DateTime FromDate = Currentdate;
                DateTime ToDate = DateTime.Now;
                ToDate = Helper.indianTime.AddMonths(3);

                if (!string.IsNullOrEmpty(item.CustomerId.ToString()))
                { obj.CustomerId = Convert.ToInt32(item.CustomerId); }
                else { obj.CustomerId = 0; }

                if (!string.IsNullOrEmpty(item.ProductId.ToString()))
                { obj.ProductId = Convert.ToInt32(item.ProductId); }
                else { obj.ProductId = 0; }
              
                //get OrderId from parent to remove child records
                DataTable dtprodRecord = new DataTable();
                dtprodRecord = obj.getCustomerOrderList(obj.CustomerId, obj.ProductId, FromDate, ToDate);
                int userRecords = dtprodRecord.Rows.Count;
                obj.OrderId = 0;
                int DelProductOrder = 0; int DelAddProductDetail = 0;
                if (userRecords > 0)
                {
                   // int CustomerId = obj.CustomerId;
                    int idao = obj.InsertDeleteOrder(obj.CustomerId, obj.ProductId);
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["OrderDate"].ToString()))
                    {
                        DateTime CurrentDate = Helper.indianTime.AddDays(1);
                        DateTime OrderDate = Convert.ToDateTime(dtprodRecord.Rows[0]["OrderDate"]);
                        if (CurrentDate.Day == OrderDate.Day && CurrentDate.Month == OrderDate.Month && CurrentDate.Year == OrderDate.Year)
                        {
                            var timming = objproduct.CheckProductOrderTimimg(obj.ProductId);
                            if (timming.IsTime == false)
                            {
                                dr["status"] = "Fail";
                                dr["error_msg"] = "Unable to delete... - " + timming.message;
                                dtNew.Rows.Add(dr);
                                return Ok(dtNew);
                            }
                        }
                    }
                   
                    for (int i = 0; i < userRecords; i++)
                    {
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Id"].ToString()))
                        {
                            obj.OrderId = Convert.ToInt32(dtprodRecord.Rows[i]["Id"]);
                            obj.OrderDate = FromDate;
                            DelProductOrder = obj.DeleteCustomerOrder(obj.OrderId);
                            FromDate = FromDate.AddDays(1);
                        }
                        else
                        {
                            dr["status"] = "Success";
                            dr["error_msg"] = "Order Not Found";
                        }
                    }
                    if (DelProductOrder > 0)
                    {
                        obj.DeleteCustomerOrderTrack(obj.CustomerId, obj.ProductId);
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Deleted Successfully";
                    }
                    else
                    {
                        dr["status"] = "Failed";
                        dr["error_msg"] = "Order Not Deleted";
                    }
                }
                else
                {
                    dr["status"] = "Success";
                    dr["error_msg"] = "Order Not Found";
                }
            }
            else
            {
                dr["status"] = "Failed";
                dr["error_msg"] = "Please Fill Correct Details";
            }
            dtNew.Rows.Add(dr);
            return Ok(dtNew);
        }

        [Route("api/OrderApi/CustomerWeekOrderDelete")]
        public IHttpActionResult CustomerWeekOrderDelete(Subscription item)//string strjson
        {   //
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            if (!string.IsNullOrEmpty(item.CustomerId.ToString()) && item.CustomerId != 0 && !string.IsNullOrEmpty(item.FromDate.ToString()) && !string.IsNullOrEmpty(item.ToDate.ToString()))
            {
                DateTime Currentdate = DateTime.Now;
                DateTime FromDate = item.FromDate;
                DateTime ToDate = item.ToDate;

                if (!string.IsNullOrEmpty(item.CustomerId.ToString()))
                { obj.CustomerId = Convert.ToInt32(item.CustomerId); }
                else { obj.CustomerId = 0; }

                var fdate = FromDate.Date;
                FromDate = fdate.AddHours(00).AddMinutes(00).AddSeconds(00);

                var tdate = ToDate.Date;
                ToDate = tdate.AddHours(23).AddMinutes(59).AddSeconds(59);

                //get OrderId from parent to remove child records
                DataTable dtprodRecord = new DataTable();
                dtprodRecord = obj.getCustomerWeekOrderList(obj.CustomerId, FromDate, ToDate);
                int userRecords = dtprodRecord.Rows.Count;
                obj.OrderId = 0;
                int DelProductOrder = 0; int DelAddProductDetail = 0;

                if (userRecords > 0)
                {
                    for (int i = 0; i < userRecords; i++)
                    {
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Id"].ToString()))
                        {
                            obj.OrderId = Convert.ToInt32(dtprodRecord.Rows[i]["Id"]);
                            var dtOrder = obj.getCustomerOnedateOrderList(obj.OrderId);
                            if (dtOrder.Rows.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(dtOrder.Rows[i]["ProductId"].ToString()))
                                {
                                    var productID = Convert.ToInt32(dtOrder.Rows[i]["ProductId"]);
                                    var timming = objproduct.CheckProductOrderTimimg(productID);
                                    if (timming.IsTime == false)
                                    {
                                        dr["status"] = "Fail";
                                        dr["error_msg"] = "Unable to delete... - " + timming.message;
                                        dtNew.Rows.Add(dr);
                                        return Ok(dtNew);
                                    }
                                }
                            }

                            obj.OrderDate = FromDate;
                            DelProductOrder = obj.DeleteCustomerOrder(obj.OrderId);

                            //if (DelAddProductOrder > 0)
                            //{
                            //    obj.OrderId = Convert.ToInt32(AddProductOrder);
                            //    obj.ProductId = Convert.ToInt32(productid);
                            //    obj.Qty = Convert.ToInt32(qty);
                            //    obj.OrderItemDate = FromDate;

                            //    DelAddProductDetail = obj.DeleteCustomerOrderDetail(obj);
                            //}
                            FromDate = FromDate.AddDays(1);
                        }
                        else
                        {
                            dr["status"] = "Success";
                            dr["error_msg"] = "Order Not Found";
                        }
                    }
                    if (DelProductOrder > 0)
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Deleted Successfully";
                    }
                    else
                    {
                        dr["status"] = "Failed";
                        dr["error_msg"] = "Order Not Deleted";
                    }
                }
                else
                {
                    dr["status"] = "Success";
                    dr["error_msg"] = "Order Not Found";
                }
            }
            else
            {
                dr["status"] = "Failed";
                dr["error_msg"] = "Please Fill Correct Details";
            }
            dtNew.Rows.Add(dr);
            return Ok(dtNew);
        }

        //Application Version 
        [Route("api/OrderApi/AppVersion")]
        [HttpGet]
        public HttpResponseMessage AppVersion() //JsonResult
        {
            Customer obj = new Customer();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.GetAppVersion(null);
            int userRecords = dtprodRecord.Rows.Count;


            //dtNew.Columns.Add("Id", typeof(int));
            //dtNew.Columns.Add("AndroidVersion", typeof(string));
            //dtNew.Columns.Add("IOSVersion", typeof(string));

            //for (int i = 0; i < dtprodRecord.Rows.Count; i++)
            //{
            //    //Status = "0";
            //    DataRow dr = dtNew.NewRow();
            //    // dr["status"] = "Success";
            //    dr["Id"] = Convert.ToInt32(dtprodRecord.Rows[i]["Id"]);
            //    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["AndroidVersion"].ToString()))
            //        dr["AndroidVersion"] = Convert.ToString(dtprodRecord.Rows[i]["AndroidVersion"]);
            //    else
            //        dr["AndroidVersion"] = "";
            //    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["IOSVersion"].ToString()))
            //        dr["IOSVersion"] = Convert.ToString(dtprodRecord.Rows[i]["IOSVersion"]);
            //    else
            //        dr["IOSVersion"] = "";
            //    dtNew.Rows.Add(dr);
            //}

            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dtprodRecord); //new Newtonsoft.Json.Formatting()

            var dict = new Dictionary<string, string>();
            //  dict["status"] = "success";
            dict["AppVersion"] = jsonString;

            string one = @"{""status"":""success""";
            string two = @",""AndroidVersion"":" + dtprodRecord.Rows[0]["AndroidVersion"];
            string three = @",""IOSVersion"":" + dtprodRecord.Rows[0]["IOSVersion"];
            //string two = @",""AppVersion"":" + dict["AppVersion"];
            string four = one + two + three + "}";

            var str = four.ToString().Replace(@"\", "");
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(str, Encoding.UTF8, "application/json");
            return response;
        }

        //Schedular cuttoff time
        [Route("api/OrderApi/SchedularCutOffTime")]
        [HttpGet]
        public HttpResponseMessage SchedularCutOffTime() //JsonResult
        {
            Customer obj = new Customer();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.GetSchedularTime(null);
            int userRecords = dtprodRecord.Rows.Count;


            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dtprodRecord); //new Newtonsoft.Json.Formatting()

            var dict = new Dictionary<string, string>();
            //  dict["status"] = "success";
            dict["SchedularCutOffTime"] = jsonString;
            string msg = "" + dtprodRecord.Rows[0]["Msg"].ToString() + "";
            string one = @"{""status"":""success""";
            string two = @",""CutOffTime"":" + dtprodRecord.Rows[0]["CutOffTime"];
            string three = @",""Msgtext"":""\" + Convert.ToString(msg) + "\"";
            //string two = @",""AppVersion"":" + dict["AppVersion"];
            string four = one + two + three + "}";

            var str = four.ToString().Replace(@"\", "");
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(str, Encoding.UTF8, "application/json");
            return response;
        }
    }
}

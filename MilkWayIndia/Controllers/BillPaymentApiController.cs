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
    public class BillPaymentApiController : ApiController
    {

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        [Route("api/BillPaymentApi/FetchBill/{CustomerId}/{CustomerNo}/{OperatorCode}/{Field1}/{Field2}/{Field3}/{Type}")]
        [HttpGet]

        public HttpResponseMessage FetchBill(int CustomerId, string CustomerNo, int OperatorCode, string Field1,string Field2,string Field3, string Type)
        {

            SubscriptionNew obj = new SubscriptionNew();
            Subscription objsub = new Subscription();
            MobileRecharge objrecharge = new MobileRecharge();
            string transactionid, resmessage = "";
            decimal Walletbal = 0, marginper = 0, marginamnt = 0, Total2daybal = 0;
            int UtransactionId = 0, status = 0, addrecharge = 0, addrecharge1 = 0;
            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();
            string Status = ""; string jsonString = null; string str1 = null; string str2 = null;

            string strUrl = "";

          //  strUrl = "http://env.specificstep.com/neo/bbps/bill?username=GJ1436&password=gj1436&operatorcode=" + OperatorCode + "&utransactionid=123213ABC&number=" + CustomerNo + "&field1=" + Field1 + "&field2=" + Field2 + "&field3=" + Field3;
            //strUrl = "http://env.specificstep.com/neo/bbps/bill?username=GJ1436&password=gj1436&operatorcode=" + OperatorCode + "&utransactionid=123213ABC&number="+ CustomerNo + "&field1="+ConsumerNo+ "&field2=yyy&field3=zzz";
           strUrl = "https://portal.specificstep.com/neo/bbps/bill?username=GJ1460&password=7778869169&operatorcode=" + OperatorCode + "&utransactionid=123213ABC&number=" + CustomerNo + "&field1=" + Field1 + "&field2="+Field2+"&field3="+Field3;
            // Create a request object  
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
            // Get the response back  


            HttpWebResponse httpres = (HttpWebResponse)request.GetResponse();
            Stream s = (Stream)httpres.GetResponseStream();
            StreamReader readStream = new StreamReader(s);
            string dataString = readStream.ReadToEnd();
            httpres.Close();
            s.Close();
            readStream.Close();

            str1 = dataString.Replace(@"\", "");





            jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dataString);
            //str1 = jsonString.ToString().Replace(@"{", "");
            //str2 = str1.ToString().Replace(@"}", "");
            //str2 = str2.ToString().Replace(@"\", "");
            //str2 = str2.ToString().Replace(@"""", "");

            jsonString = str1;
            //  return jsonString;
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            return response;
        }

        [Route("api/BillPaymentApi/GetProvider/{Type}/{CircleCode}")]
        [HttpGet]
        public IHttpActionResult GetProvider(string Type,int? CircleCode) //JsonResult
        {
            BillPayment order = new BillPayment();
            var dtList = order.GetProvider(Type,CircleCode);


            return Ok(dtList);
        }


        [Route("api/BillPaymentApi/BillPay/{CustomerId}/{OperatorCode}/{NumberTag}/{Fieldtag1}/{Type}/{Amount}/{Lat}/{Long}/{AgentId}/{PaymentMode}/{CustomerMobile}/{Fieldtag2}/{FieldTag3}")]
        [HttpGet]

        public HttpResponseMessage BillPay(int CustomerId, int OperatorCode, string NumberTag, string Fieldtag1, string Type, int Amount, string Lat, string Long, string AgentId, string PaymentMode, string CustomerMobile, string Fieldtag2, string FieldTag3)
        {

            //balance,billno,billdate,duedate,billamnt,
            SubscriptionNew obj = new SubscriptionNew();
            Subscription objsub = new Subscription();
            BillPayment objrecharge = new BillPayment();
            string transactionid, resmessage = "",msg="",msg1="",msgres = "", msgcashamount = "0";
            decimal Walletbal = 0, marginper = 0, marginamnt = 0, Total2daybal = 0;
            int UtransactionId = 0, status = 0, addrecharge = 0, addrecharge1 = 0;
            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            dtNew.Columns.Add("Response", typeof(string));
            dtNew.Columns.Add("Cashback_Amount", typeof(string));
            DataRow dr = dtNew.NewRow();
            string Status = ""; string jsonString = null; string str1 = null; string str2 = null;

            string strUrl = "";


            //strUrl = "http://env.specificstep.com/neo/api/balance?username=GJ1436&password=gj1436";
            strUrl = "https://portal.specificstep.com/neo/api/balance?username=GJ1460&password=7778869169";
            // Create a request object  
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
            // Get the response back  


            HttpWebResponse httpres = (HttpWebResponse)request.GetResponse();
            Stream s = (Stream)httpres.GetResponseStream();
            StreamReader readStream = new StreamReader(s);
            string dataString = readStream.ReadToEnd();
            httpres.Close();
            s.Close();
            readStream.Close();

            str1 = dataString.Replace(@"\", "");





            jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dataString);
            str1 = jsonString.ToString().Replace(@"{", "");
            str2 = str1.ToString().Replace(@"}", "");
            str2 = str2.ToString().Replace(@"\", "");
            str2 = str2.ToString().Replace(@"""", "");
            str2 = str2.ToString().Replace(@"tBalance:", "");
            str2 = str2.ToString().Replace(@"DateTime:", "");
            str2 = str2.ToString().Replace(@"Status:", "");
            str2 = str2.ToString().Replace(@"ResposneMessage:", "");
            jsonString = str2;
            //  return jsonString;
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            double balance = 0;
            string date, responsemsg = "";

            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            string a = "";
            int c = 0;
            foreach (string s1 in str2.Split(delimiter))
            {
                c = c + 1;
                if (c == 1) balance = Convert.ToDouble(s1);
                if (c == 2) date = s1.ToString();
                if (c == 4) responsemsg = s1.ToString();


            }



            DataTable dtprodRecord1 = new DataTable();
            int userRecords1 = 0;
            if (balance < Amount)
            {


                dtprodRecord1 = obj.getCustomerWalletNew(CustomerId);
                userRecords1 = dtprodRecord1.Rows.Count;

                if (userRecords1 > 0)
                {
                    var balance1 = obj.GetCustomerBalaceNew(CustomerId);
                    Walletbal = balance1;
                }


                if (Walletbal >= Amount)
                {
                    userRecords1 = 0;
                    UtransactionId = 0;

                    DataTable dtutid = new DataTable();

                    dtutid = objrecharge.getUtransactionId();
                    userRecords1 = dtutid.Rows.Count;

                    if (dtutid.Rows.Count > 0)
                    {
                        UtransactionId = Convert.ToInt32(dtutid.Rows[dtutid.Rows.Count - 1].ItemArray[1].ToString()) + 1;

                    }

                    else
                    {
                        UtransactionId = 1000;
                    }


                    String UtransactionIdnew = "M-" + UtransactionId.ToString();


                    DateTime rdate = DateTime.Now;


                    string billstatus = "";
                    objrecharge.CustomerId = CustomerId;
                    ////  objrecharge.Rechargeno = RechargeNo;
                    objrecharge.UtransactionId = UtransactionIdnew;
                    objrecharge.NumberCustomer = NumberTag.ToString();
                    objrecharge.RechargeAmount = Convert.ToDecimal(Amount);
                    objrecharge.OperatorId = OperatorCode.ToString();
                    objrecharge.Lat = Lat.ToString();
                    objrecharge.Lon = Long.ToString();
                    objrecharge.PaymentMode = PaymentMode.ToString();
                    objrecharge.CustomerMobile = CustomerMobile.ToString();
                    objrecharge.FieldTag1 = Fieldtag1;
                    objrecharge.FieldTag2 = Fieldtag2;
                    objrecharge.FieldTag3 = FieldTag3;
                    objrecharge.TransactionId = "";
                    objrecharge.Status = 5;
                    objrecharge.Responsemsg = "Insufficient Balance With Provider.";
                    objrecharge.RechargeType = Type.ToString();
                    objrecharge.errorcode = "0";
                    objrecharge.Agentid = AgentId.ToString();


                    objrecharge.FromDate = rdate;


                    objrecharge.Billstatus = billstatus.ToString();
                    //  objrecharge.RechargeType = Type.ToString();

                    //  if (status == 1)

                    //  {
                    objrecharge.UtransactionId1 = UtransactionId.ToString();
                    addrecharge = objrecharge.insertrecharge(objrecharge);


                    if (addrecharge > 0)
                    {
                        if (!string.IsNullOrEmpty(rdate.ToString()))
                        {
                            objsub.TransactionDate = rdate;
                        }
                        objsub.CustomerId = CustomerId;

                        objsub.BillNo = null;
                        objsub.Type = "Debit";
                        objsub.CustSubscriptionId = 0;
                        objsub.TransactionType = Convert.ToInt32(Helper.TransactionType.Purchase);
                        objsub.Description = "Recahrge - " + Type + ",Rechargeno: " + NumberTag;
                        string orderid = "0";
                        if (!string.IsNullOrEmpty(orderid))
                        {
                            objsub.OrderId = Convert.ToInt32(orderid);
                        }
                        objsub.Amount = objrecharge.RechargeAmount;
                        objsub.Cashbacktype = "";
                        objsub.CashbackId = "";
                        objsub.CashbackStatus = "";
                        objsub.UtransactionId = UtransactionIdnew.ToString();
                        objsub.Status = "BillPay-" + Type;
                        int addwallet = objsub.InsertWallet1(objsub);
                        if (addwallet > 0)
                        {


                            DataTable dtcashback = new DataTable();

                            dtcashback = objrecharge.getCashbackper(Type, OperatorCode.ToString());
                            userRecords1 = dtcashback.Rows.Count;
                            decimal amount = Convert.ToDecimal(Amount);
                            decimal cashbackamnt = 0;
                            if (userRecords1 > 0)
                            {
                                string per = dtcashback.Rows[0].ItemArray[0].ToString();
                                cashbackamnt = (amount * Convert.ToDecimal(per)) / 100;

                                objsub.BillNo = null;
                                objsub.Type = "Credit";
                                objsub.CustSubscriptionId = 0;
                                objsub.TransactionType = Convert.ToInt32(Helper.TransactionType.Deposit);
                                objsub.Description = "Cashback Billpay- " + Type + ",Transaction Id:" + ",Rechargeno:" + NumberTag;
                                string orderid1 = "0";
                                if (!string.IsNullOrEmpty(orderid1))
                                {
                                    objsub.OrderId = Convert.ToInt32(orderid1);
                                }
                                objsub.Amount = cashbackamnt;

                                objsub.Status = "Cashback";

                                objsub.Cashbacktype = Type.ToString();
                                objsub.CashbackId = "";
                                objsub.UtransactionId = UtransactionIdnew.ToString();
                                addwallet = objsub.InsertWallet1(objsub);

                                objrecharge.CashbackStatus = "Complete";
                                objrecharge.CashbackAmount = cashbackamnt.ToString();
                                objrecharge.CashbackDate = rdate;
                                objrecharge.Billstatus = "Insufficient Balance With Provider.";
                                int updatebillpayrecharge = objrecharge.Updaterecharge(objrecharge);
                            }
                            msgcashamount = cashbackamnt.ToString();
                            msgres = "Success";
                            dr["status"] = "Recharge Success";
                            dr["error_msg"] = "Your " + Type + " Recharge Done";
                            dr["Response"] = msgres;
                            dr["Cashback_Amount"] = msgcashamount;
                            dtNew.Rows.Add(dr);

                            jsonString = string.Empty;
                            jsonString = JsonConvert.SerializeObject(dtNew);


                            response = Request.CreateResponse(HttpStatusCode.OK);
                            response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                            return response;
                        }





                    }

                }


                else
                {
                    dr["status"] = "Recharge Fail";
                    dr["error_msg"] = "Recharge Amount is High.Add Wallet Balance";
                    dtNew.Rows.Add(dr);
                }

            }






            if (balance >= Amount)
            {


                               
                dtprodRecord1 = obj.getCustomerWalletNew(CustomerId);
                userRecords1 = dtprodRecord1.Rows.Count;

                if (userRecords1 > 0)
                {
                    var balance1 = obj.GetCustomerBalaceNew(CustomerId);
                    Walletbal = balance1;
                }


                if (Walletbal >= Amount)
                {
                    userRecords1 = 0;
                    UtransactionId = 0;

                    DataTable dtutid = new DataTable();

                    dtutid = objrecharge.getUtransactionId();
                    userRecords1 = dtutid.Rows.Count;

                    if (dtutid.Rows.Count > 0)
                    {
                        UtransactionId = Convert.ToInt32(dtutid.Rows[dtutid.Rows.Count - 1].ItemArray[1].ToString()) + 1;

                    }

                    else
                    {
                        UtransactionId = 1000;
                    }


                    String UtransactionIdnew = "M-" + UtransactionId.ToString();


                    DateTime rdate = DateTime.Now;

                    //

                    string billstatus = "Pending";
                    objrecharge.CustomerId = CustomerId;
                    ////  objrecharge.Rechargeno = RechargeNo;
                    objrecharge.UtransactionId = UtransactionIdnew;
                    objrecharge.NumberCustomer = NumberTag.ToString();
                    objrecharge.RechargeAmount = Convert.ToDecimal(Amount);
                    objrecharge.OperatorId = OperatorCode.ToString();
                    objrecharge.Lat = Lat.ToString();
                    objrecharge.Lon = Long.ToString();
                    objrecharge.PaymentMode = PaymentMode.ToString();
                    objrecharge.CustomerMobile = CustomerMobile.ToString();
                    objrecharge.FieldTag1 = Fieldtag1;
                    objrecharge.FieldTag2 = Fieldtag2;
                    objrecharge.FieldTag3 = FieldTag3;
                    objrecharge.TransactionId = "";
                    objrecharge.Status = 0;
                    objrecharge.Responsemsg = "Pending";
                    objrecharge.RechargeType = Type.ToString();
                    objrecharge.errorcode = "0";
                    objrecharge.Agentid = AgentId.ToString();


                    objrecharge.FromDate = rdate;


                    objrecharge.Billstatus = billstatus.ToString();
                    //  objrecharge.RechargeType = Type.ToString();

                    //  if (status == 1)

                    //  {
                    objrecharge.UtransactionId1 = UtransactionId.ToString();
                    addrecharge = objrecharge.insertrecharge(objrecharge);


                    if (addrecharge > 0)
                    {
                        if (!string.IsNullOrEmpty(rdate.ToString()))
                        {
                            objsub.TransactionDate = rdate;
                        }
                        objsub.CustomerId = CustomerId;

                        objsub.BillNo = null;
                        objsub.Type = "Debit";
                        objsub.CustSubscriptionId = 0;
                        objsub.TransactionType = Convert.ToInt32(Helper.TransactionType.Purchase);
                        objsub.Description = "Recahrge - " + Type + ",Rechargeno: " + NumberTag;
                        string orderid = "0";
                        if (!string.IsNullOrEmpty(orderid))
                        {
                            objsub.OrderId = Convert.ToInt32(orderid);
                        }
                        objsub.Amount = objrecharge.RechargeAmount;
                        objsub.Cashbacktype = "";
                        objsub.CashbackId = "";
                        objsub.CashbackStatus = "";
                        objsub.UtransactionId = UtransactionIdnew.ToString();
                        objsub.Status = "BillPay-" + Type;
                        int addwallet = objsub.InsertWallet1(objsub);
                        if (addwallet > 0)
                        {


                            DataTable dtcashback = new DataTable();

                            dtcashback = objrecharge.getCashbackper(Type, OperatorCode.ToString());
                            userRecords1 = dtcashback.Rows.Count;
                            decimal amount = Convert.ToDecimal(Amount);
                            decimal cashbackamnt = 0;
                            if (userRecords1 > 0)
                            {
                                string per = dtcashback.Rows[0].ItemArray[0].ToString();
                                cashbackamnt = (amount * Convert.ToDecimal(per)) / 100;

                                objsub.BillNo = null;
                                objsub.Type = "Credit";
                                objsub.CustSubscriptionId = 0;
                                objsub.TransactionType = Convert.ToInt32(Helper.TransactionType.Deposit);
                                objsub.Description = "Cashback Billpay- " + Type + ",Rechargeno:" + NumberTag;
                                string orderid1 = "0";
                                if (!string.IsNullOrEmpty(orderid1))
                                {
                                    objsub.OrderId = Convert.ToInt32(orderid1);
                                }
                                objsub.Amount = cashbackamnt;

                                objsub.Status = "Cashback";

                                objsub.Cashbacktype = Type.ToString();
                                objsub.CashbackId = "";
                                objsub.UtransactionId = UtransactionIdnew.ToString();
                                addwallet = objsub.InsertWallet1(objsub);

                                objrecharge.CashbackStatus = "Pending";
                                objrecharge.CashbackAmount = cashbackamnt.ToString();
                                objrecharge.CashbackDate = rdate;




                                //

                                //  strUrl = "http://env.specificstep.com/neo/bbps?username=GJ1436&password=gj1436&operatorcode=" + OperatorCode + "&utransactionid=" + UtransactionIdnew + "&number=" + NumberTag + "&amount=" + Amount + "&lat=" + Lat + "&long=" + Long + "&agentid=" + AgentId + "&paymentmode=" + PaymentMode + "&customermobile=" + CustomerMobile + "&field1=" + Fieldtag1 + "&field2=" + Fieldtag2 + "&field3=" + FieldTag3 + "";
                                strUrl = "https://portal.specificstep.com/neo/bbps?username=GJ1460&password=7778869169&operatorcode=" + OperatorCode + "&utransactionid=" + UtransactionIdnew + "&number=" + NumberTag + "&amount=" + Amount + "&lat=" + Lat + "&long=" + Long + "&agentid=" + AgentId + "&paymentmode=" + PaymentMode + "&customermobile=" + CustomerMobile + "&field1=" + Fieldtag1 + "&field2=" + Fieldtag2 + "&field3=" + FieldTag3 + "";

                                request = (HttpWebRequest)WebRequest.Create(strUrl);
                                // Get the response back  


                                httpres = (HttpWebResponse)request.GetResponse();
                                s = (Stream)httpres.GetResponseStream();
                                readStream = new StreamReader(s);
                                dataString = readStream.ReadToEnd();
                                httpres.Close();
                                s.Close();
                                readStream.Close();

                                jsonString = string.Empty;
                                jsonString = JsonConvert.SerializeObject(dataString);
                                str1 = jsonString.ToString().Replace(@"{", "");
                                str2 = str1.ToString().Replace(@"}", "");
                                str2 = str2.ToString().Replace(@"\", "");
                                str2 = str2.ToString().Replace(@"""", "");
                                str2 = str2.ToString().Replace(@"'", "");
                                str2 = str2.ToString().Replace(@"TransactionID:", "");
                                str2 = str2.ToString().Replace(@"UtransactionID:", "");
                                str2 = str2.ToString().Replace(@"OperatorID:", "");
                                str2 = str2.ToString().Replace(@"number:", "");
                                str2 = str2.ToString().Replace(@"amount:", "");
                                str2 = str2.ToString().Replace(@"Status:", "");
                                str2 = str2.ToString().Replace(@"ResposneMessage:", "");
                                //str2 = str2.ToString().Replace(@"MarginPercentage:", "");
                                //str2 = str2.ToString().Replace(@"MarginAmount:", "");
                                //str2 = str2.ToString().Replace(@"Margin", "");

                                jsonString = str2;
                                //  return jsonString;
                                response = Request.CreateResponse(HttpStatusCode.OK);
                                response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                                delimStr = ",";
                                char[] delimiter1 = delimStr.ToCharArray();
                                a = "";
                                c = 0;
                                resmessage = ""; transactionid = "";
                                foreach (string s1 in str2.Split(delimiter1))
                                {
                                    if (s1 == "error:Please leave an interval of five minutes before recharging the same number.")
                                    {
                                        resmessage = s1.ToString();
                                        status = 2;
                                    }
                                    else
                                    {
                                        c = c + 1;
                                        if (c == 1) transactionid = s1.ToString();
                                        if (c == 6)
                                        {
                                            if (s1.ToString() != "")
                                            {
                                                status = Convert.ToInt32(s1);
                                            }

                                        }

                                        if (c == 7) resmessage = s1.ToString();

                                    }

                                }
                                rdate = DateTime.Now;

                                billstatus = "";
                                objrecharge.CustomerId = CustomerId;
                                ////  objrecharge.Rechargeno = RechargeNo;
                                objrecharge.UtransactionId = UtransactionIdnew;
                                objrecharge.NumberCustomer = NumberTag.ToString();
                                objrecharge.RechargeAmount = Convert.ToDecimal(Amount);
                                objrecharge.OperatorId = OperatorCode.ToString();
                                objrecharge.Lat = Lat.ToString();
                                objrecharge.Lon = Long.ToString();
                                objrecharge.PaymentMode = PaymentMode.ToString();
                                objrecharge.CustomerMobile = CustomerMobile.ToString();
                                objrecharge.FieldTag1 = Fieldtag1;
                                objrecharge.FieldTag2 = Fieldtag2;
                                objrecharge.FieldTag3 = FieldTag3;
                                objrecharge.TransactionId = transactionid.ToString();
                                objrecharge.Status = status;
                                objrecharge.Responsemsg = resmessage;
                                objrecharge.RechargeType = Type.ToString();
                                objrecharge.errorcode = "0";
                                objrecharge.Agentid = AgentId.ToString();
                                //  objrecharge.Marginper = marginper.ToString();
                                //  objrecharge.Marginamnt = marginamnt;
                                //  objrecharge.TransactionId = transactionid.ToString();


                                objrecharge.FromDate = rdate;



                                string cstatus = "";
                                if (status == 0)
                                {
                                    billstatus = "Pending";
                                    objrecharge.CashbackStatus = "Complete";
                                     cstatus = "Cr";



                                    //strUrl = "http://env.specificstep.com/neo/bbps/status?username=GJ1436&password=gj1436&transactionid=&utransactionid=" + UtransactionIdnew;


                                    strUrl = "https://portal.specificstep.com/neo/bbps/status?username=GJ1460&password=7778869169&transactionid=&utransactionid=" + UtransactionIdnew;


                                    request = (HttpWebRequest)WebRequest.Create(strUrl);


                                    httpres = (HttpWebResponse)request.GetResponse();
                                    s = (Stream)httpres.GetResponseStream();
                                    readStream = new StreamReader(s);
                                    dataString = readStream.ReadToEnd();
                                    httpres.Close();
                                    s.Close();
                                    readStream.Close();

                                    jsonString = string.Empty;
                                    jsonString = JsonConvert.SerializeObject(dataString);
                                    str1 = jsonString.ToString().Replace(@"{", "");
                                    str2 = str1.ToString().Replace(@"}", "");
                                    str2 = str2.ToString().Replace(@"\", "");
                                    str2 = str2.ToString().Replace(@"""", "");
                                    str2 = str2.ToString().Replace(@"'", "");
                                    str2 = str2.ToString().Replace(@"TransactionID:", "");
                                    str2 = str2.ToString().Replace(@"OperatorID:", "");
                                    str2 = str2.ToString().Replace(@"UtransactionID:", "");


                                    str2 = str2.ToString().Replace(@"Status:", "");
                                    str2 = str2.ToString().Replace(@"ResposneMessage:", "");
                                    jsonString = str2;
                                    //  return jsonString;
                                    response = Request.CreateResponse(HttpStatusCode.OK);
                                    response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                                    delimStr = ",";
                                    char[] delimiter2 = delimStr.ToCharArray();
                                    a = "";
                                    c = 0;


                                    resmessage = ""; transactionid = "";
                                    foreach (string s1 in str2.Split(delimiter2))
                                    {
                                        if (s1 == "error:Transaction ID Not Found")
                                        {
                                            resmessage = s1.ToString();
                                            status = 2;
                                        }
                                        else
                                        {
                                            c = c + 1;
                                            if (c == 1) transactionid = s1.ToString();
                                            if (c == 4)
                                            {
                                                if (s1.ToString() != "")
                                                {
                                                    status = Convert.ToInt32(s1);
                                                }

                                            }

                                            if (c == 5) resmessage = s1.ToString();

                                        }
                                    }

                                    if (status == 1)
                                    {
                                        billstatus = "Success";
                                        objrecharge.CashbackStatus = "Complete";
                                        cstatus = "Credit";
                                    }
                                    if (status == 2 || status == 3 || status == 4)
                                    {
                                        billstatus = "Failure";
                                        objrecharge.CashbackStatus = "";
                                        cstatus = "Fail";
                                    }
                                    objrecharge.Status = status;
                                }
                                if (status == 1)
                                {
                                    objrecharge.Status = status;
                                    billstatus = "Success";
                                    objrecharge.CashbackStatus = "Complete";
                                    cstatus = "Credit";
                                }
                                if (status == 2 || status == 3 || status == 4)
                                {
                                    objrecharge.Status = status;
                                    billstatus = "Failure";
                                    objrecharge.CashbackStatus = "";
                                    cstatus = "Fail";
                                }

                                objrecharge.Billstatus = billstatus.ToString();
                                //

                                if (status == 0 || status == 1)
                                {
                                    int updatebillpayrecharge = objrecharge.Updaterecharge1(objrecharge);
                                    msg = "Bill Pay Success";
                                    msg1 = "Bill Pay Done";
                                    msgres = billstatus;
                                    msgcashamount = cashbackamnt.ToString();

                                }
                                else
                                {
                                    int updatemobilerecharge = objrecharge.Updaterecharge2(objrecharge);
                                    msg = resmessage;
                                    msg1 = "Bill Pay Fail";
                                    msgres = billstatus;
                                    msgcashamount = "0";
                                }


                            }

                            dr["status"] = msg;
                            dr["error_msg"] = "Your " + Type + " "+msg1;
                            dr["Response"] = msgres;
                            dr["Cashback_Amount"] = msgcashamount;
                            dtNew.Rows.Add(dr);

                            jsonString = string.Empty;
                            jsonString = JsonConvert.SerializeObject(dtNew);


                            response = Request.CreateResponse(HttpStatusCode.OK);
                            response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                            return response;
                        }





                    }


                     
                }



                else
                {
                    dr["status"] = "Recharge Fail";
                    dr["error_msg"] = "Recharge Amount is High.Add Wallet Balance";
                    dtNew.Rows.Add(dr);
                }

            }
        

              
 

    jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dtNew);
          
            //  return jsonString;
             response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return response;

            
        }




        [Route("api/BillPaymentApi/GetCircleProvider/{Operatorcode}")]
        [HttpGet]
        public IHttpActionResult GetCircleProvider(string Operatorcode) //JsonResult
        {
            BillPayment order = new BillPayment();
            var dtList = order.GetCircleProvider(Operatorcode);


            return Ok(dtList);
        }

    }
}

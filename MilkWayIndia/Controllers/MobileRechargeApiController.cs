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
    public class MobileRechargeApiController : ApiController
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        [Route("api/MobileRechargeApi/GetCircle/")]
        [HttpGet]
        public IHttpActionResult GetCircle() //JsonResult
        {
            MobileRecharge order = new MobileRecharge();
            var dtList = order.getCircles();


            return Ok(dtList);
        }

        [Route("api/MobileRechargeApi/GetOperator/{Type}")]
        [HttpGet]
        public IHttpActionResult GetOperator(string Type) //JsonResult
        {
            MobileRecharge order = new MobileRecharge();
            var dtList = order.getOperator(Type);


            return Ok(dtList);
        }


        [Route("api/MobileRechargeApi/MobileRecharge/{CustomerId}/{RechargeNo}/{CircleCode}/{OperatorCode}/{RechargeAmount}/{Type}")]
        [HttpGet]
        public HttpResponseMessage MobileRecharge(int CustomerId, string RechargeNo, int CircleCode, int OperatorCode, int RechargeAmount, string Type)
        {
            SubscriptionNew obj = new SubscriptionNew();
            Subscription objsub = new Subscription();
            MobileRecharge objrecharge = new MobileRecharge();
            string transactionid, resmessage = "",msg="",msg1="",msgres="",msgcashamount="0";
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
            //return response;

            if (balance < RechargeAmount)
            {


                DataTable dtprodRecord1 = new DataTable();
                dtprodRecord1 = obj.getCustomerWalletNew(CustomerId);
                int userRecords1 = dtprodRecord1.Rows.Count;

                if (userRecords1 > 0)
                {
                    var balance1 = obj.GetCustomerBalaceNew(CustomerId);
                    Walletbal = balance1;
                }

                // dtNew.Rows.Add(dr);


                if (Walletbal >= RechargeAmount)
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

                    objrecharge.UtransactionId1 = UtransactionId.ToString();
                    objrecharge.CustomerId = CustomerId;
                    objrecharge.Rechargeno = RechargeNo;
                    objrecharge.UtransactionId = UtransactionIdnew;
                    objrecharge.CircleCode = CircleCode.ToString();
                    objrecharge.OperatorId = OperatorCode.ToString();
                    objrecharge.Status = 4;
                    objrecharge.Responsemsg = "Insufficient Balance With Provider.";
                    objrecharge.Marginper = marginper.ToString();
                    objrecharge.Marginamnt = marginamnt;
                    objrecharge.TransactionId = "";
                    
                    objrecharge.RechargeAmount = Convert.ToDecimal(RechargeAmount);
                    objrecharge.FromDate = rdate;
                    objrecharge.RechargeType = Type.ToString();
                    
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
                        objsub.Description = "Recahrge - " + Type + ",Rechargeno: " + RechargeNo;
                        
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
                        objsub.Status = "Recharge-" +Type;
                        int addwallet = objsub.InsertWallet1(objsub);
                        if (addwallet > 0)
                        {


                            DataTable dtcashback = new DataTable();

                            dtcashback = objrecharge.getCashbackper(Type, OperatorCode.ToString());
                            userRecords1 = dtcashback.Rows.Count;
                            decimal amount = Convert.ToDecimal(RechargeAmount);
                            decimal cashbackamnt = 0;
                            if (userRecords1 > 0)
                            {
                                string per = dtcashback.Rows[0].ItemArray[0].ToString();
                                cashbackamnt = (amount * Convert.ToDecimal(per)) / 100;

                                objsub.BillNo = null;
                                objsub.Type = "Credit";
                                objsub.CustSubscriptionId = 0;
                                objsub.TransactionType = Convert.ToInt32(Helper.TransactionType.Deposit);
                                objsub.Description = "Cashback Recharge- " + Type + ",Rechargeno:" + RechargeNo;
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
                                objrecharge.RechargeStatus = "Insufficient Balance With Provider.";

                                int updatemobilerecharge = objrecharge.Updaterecharge(objrecharge);
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
            //insufficient balance end
            if (balance >= RechargeAmount)
            {

                DataTable dtprodRecord1 = new DataTable();
                dtprodRecord1 = obj.getCustomerWalletNew(CustomerId);
                int userRecords1 = dtprodRecord1.Rows.Count;

                if (userRecords1 > 0)
                {
                    var balance1 = obj.GetCustomerBalaceNew(CustomerId);
                    Walletbal = balance1;
                }

                if (Walletbal >= RechargeAmount)
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

                    /////
                    ///

                    DateTime rdate = DateTime.Now;

                    objrecharge.UtransactionId1 = UtransactionId.ToString();
                    objrecharge.CustomerId = CustomerId;
                    objrecharge.Rechargeno = RechargeNo;
                    objrecharge.UtransactionId = UtransactionIdnew;
                    objrecharge.CircleCode = CircleCode.ToString();
                    objrecharge.OperatorId = OperatorCode.ToString();
                    objrecharge.Status = 0;
                    objrecharge.Responsemsg = "Pending";
                    objrecharge.Marginper = marginper.ToString();
                    objrecharge.Marginamnt = marginamnt;
                    objrecharge.TransactionId = "";

                    objrecharge.RechargeAmount = Convert.ToDecimal(RechargeAmount);
                    objrecharge.FromDate = rdate;
                    objrecharge.RechargeType = Type.ToString();
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
                        objsub.Description = "Recharge - " + Type + ",Rechargeno: " + RechargeNo;
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
                        objsub.Status = "Recharge-" + Type;
                        int addwallet = objsub.InsertWallet1(objsub);
                        if (addwallet > 0)
                        {


                            DataTable dtcashback = new DataTable();

                            dtcashback = objrecharge.getCashbackper(Type, OperatorCode.ToString());
                            userRecords1 = dtcashback.Rows.Count;
                            decimal amount = Convert.ToDecimal(RechargeAmount);
                            decimal cashbackamnt = 0;
                            if (userRecords1 > 0)
                            {
                                string per = dtcashback.Rows[0].ItemArray[0].ToString();
                                cashbackamnt = (amount * Convert.ToDecimal(per)) / 100;

                                objsub.BillNo = null;
                                objsub.Type = "Credit";
                                objsub.CustSubscriptionId = 0;
                                objsub.TransactionType = Convert.ToInt32(Helper.TransactionType.Deposit);
                                objsub.Description = "Cashback Recharge- " + Type + "" + ",Rechargeno:" + RechargeNo;
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


                                //

                                //strUrl = "http://env.specificstep.com/neo/api?username=GJ1436&password=gj1436&utransactionid=" + UtransactionIdnew + "&circlecode=" + CircleCode + "&operatorcode=" + OperatorCode + "&number=" + RechargeNo + "&amount=" + RechargeAmount;
                                strUrl = "https://portal.specificstep.com/neo/api?username=GJ1460&password=7778869169&utransactionid=" + UtransactionIdnew + "&circlecode=" + CircleCode + "&operatorcode=" + OperatorCode + "&number=" + RechargeNo + "&amount=" + RechargeAmount;
                                // Create a request object  
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
                                str2 = str2.ToString().Replace(@"tTransactionID:", "");
                                str2 = str2.ToString().Replace(@"UtransactionID:", "");
                                str2 = str2.ToString().Replace(@"OperatorID:", "");
                                str2 = str2.ToString().Replace(@"Number:", "");
                                str2 = str2.ToString().Replace(@"Amount:", "");
                                str2 = str2.ToString().Replace(@"Status:", "");
                                str2 = str2.ToString().Replace(@"ResposneMessage:", "");
                                str2 = str2.ToString().Replace(@"MarginPercentage:", "");
                                str2 = str2.ToString().Replace(@"MarginAmount:", "");
                                str2 = str2.ToString().Replace(@"Margin", "");

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
                                    c = c + 1;
                                    if (c == 1) transactionid = s1.ToString();
                                    if (c == 6) status = Convert.ToInt32(s1);
                                    if (c == 7) resmessage = s1.ToString();
                                    if (c == 8) marginper = Convert.ToDecimal(s1.ToString());
                                    if (c == 9) marginamnt = Convert.ToDecimal(s1.ToString());


                                }

                                rdate = DateTime.Now;


                                objrecharge.CustomerId = CustomerId;
                                objrecharge.Rechargeno = RechargeNo;
                                objrecharge.UtransactionId = UtransactionIdnew;
                                objrecharge.CircleCode = CircleCode.ToString();
                                objrecharge.OperatorId = OperatorCode.ToString();
                                objrecharge.Status = status;
                                objrecharge.Responsemsg = resmessage;
                                objrecharge.Marginper = marginper.ToString();
                                objrecharge.Marginamnt = marginamnt;
                                objrecharge.TransactionId = transactionid.ToString();

                                objrecharge.RechargeAmount = Convert.ToDecimal(RechargeAmount);
                                objrecharge.FromDate = rdate;
                                objrecharge.RechargeType = Type.ToString();

                                string rechargestatus = "";
                                if (status == 0) rechargestatus = "Pending";
                                if (status == 1) rechargestatus = "Success";
                                if (status == 2 || status == 3 || status == 4) rechargestatus = "Failure";

                                objrecharge.RechargeStatus = rechargestatus.ToString();
                                //

                                //objsub.Description = "Cashback Recharge- " + Type + ",Transaction Id:" + ",Rechargeno:" + RechargeNo;

                                if(status==0 || status==1)
                                { int updatemobilerecharge = objrecharge.Updaterecharge1(objrecharge);
                                    msg = "Recharge Success";
                                    msg1 = "Recharge Done";
                                    msgres = rechargestatus;
                                    msgcashamount = cashbackamnt.ToString();
                                }

                                else
                                {
                                    int updatemobilerecharge = objrecharge.Updaterecharge2(objrecharge);
                                    msg = resmessage;
                                    msg1 = "Recharge Fail";
                                    msgres = rechargestatus;
                                    msgcashamount = "0";
                                }
                                
                            }

                            ///// dtNew.Columns.Add("Response", typeof(string));dtNew.Columns.Add("Cashback_Amount", typeof(string));
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
                jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew);

                //  return jsonString;
                response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                return response;
                //return dtNew;
            }

            jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dtNew);

            //  return jsonString;
            response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return response;
        }


        [Route("api/MobileRechargeApi/FetchPlan/{CustomerId}/{CircleId}/{OperatorId}/{Type}")]
        [HttpGet]

        public HttpResponseMessage FetchPlan(int CustomerId,string CircleId, string OperatorId, string Type)
        {
            string Status = ""; string jsonString = null; string str1 = null; string str2 = null;
            int cid = CustomerId;
           
            string strUrl = "";
            //strUrl = "http://env.specificstep.com/neo/plan?username=GJ1436&password=gj1436&circle=" + CircleId + "&operator="+ OperatorId + "&type="+Type;
          strUrl = "https://portal.specificstep.com/neo/plan?username=GJ1460&password=7778869169&circle=" + CircleId + "&operator=" + OperatorId + "&type="+Type;
            // Create a request object  
            WebRequest request = HttpWebRequest.Create(strUrl);
            // Get the response back  

            
                HttpWebResponse responsesms = (HttpWebResponse)request.GetResponse();
                Stream s = (Stream)responsesms.GetResponseStream();
                StreamReader readStream = new StreamReader(s);
                string dataString = readStream.ReadToEnd();
                responsesms.Close();
                s.Close();
                readStream.Close();

            str1 = dataString;
            str1 = str1.Replace(@"\", "");

           

            jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(str1);
            //str1 = jsonString.ToString().Replace(@"{", "");
            //str2 = str1.ToString().Replace(@"}", "");
            //str2 = str2.ToString().Replace(@"\", "");
            //str2 = str2.ToString().Replace(@"""", "");

            jsonString = str1;
            //  return jsonString;
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            return response;



            //return Ok("");
        }



        [Route("api/MobileRechargeApi/FetchBalance/")]
        [HttpGet]

        public HttpResponseMessage FetchBalance()
        {

            string Status = ""; string jsonString = null; string str1 = null; string str2 = null;

            string strUrl = "";
            //strUrl = "http://env.specificstep.com/neo/api/balance?username=GJ1436&password=gj1436";
            strUrl = "https://portal.specificstep.com/neo/api/balance?username=GJ1460&password=7778869169";
            // Create a request object  
            HttpWebRequest  request = (HttpWebRequest)WebRequest.Create(strUrl);
            // Get the response back  

          
                HttpWebResponse httpres = (HttpWebResponse)request.GetResponse();
                Stream s = (Stream)httpres.GetResponseStream();
                StreamReader readStream = new StreamReader(s);
                string dataString = readStream.ReadToEnd();
                httpres.Close();
                s.Close();
                readStream.Close();
               
            str1=dataString.Replace(@"\","");





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

            int balance = 0;
            string date, responsemsg;

            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            string a = "";
            int c = 0;
            foreach (string s1 in str2.Split(delimiter))
            {
                c = c + 1;
                if(c==1) balance = Convert.ToInt32(s1);
                if(c==2) date = s1.ToString();
                if(c==3) responsemsg = s1.ToString();


            }


           

            return response;
            // return Ok("");
        }





    }
}

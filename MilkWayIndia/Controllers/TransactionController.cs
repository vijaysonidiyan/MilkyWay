using MilkWayIndia.Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
namespace MilkWayIndia.Controllers
{
    public class TransactionController : Controller
    {
        // GET: Transaction
        Transaction objtransaction = new Transaction();
        public ActionResult Index()
        {
            return View();
        }

        public class ngodarpan
        {
            public string TransactionID { get; set; }
            public string UtransactionID { get; set; }
            public string OperatorID { get; set; }
            public string Number { get; set; }
            public string Amount { get; set; }

            public string Status { get; set; }
            public string ResposneMessage { get; set; }
            public string MarginPercentage { get; set; }
            public string MarginAmount { get; set; }


            public string CustomerId { get; set; }
            public string Name { get; set; }
            public string RechargeNo { get; set; }
            public string CStatus { get; set; }
            public string CResponsemsg { get; set; }
            public string RechargeType { get; set; }
            public string RechargeDate { get; set; }


            public string Rechargestatus { get; set; }
            public string Type { get; set; }
        }


        public class ElectricityGas1
        {
            public string error { get; set; }
            public string TransactionID { get; set; }
            public string UtransactionID { get; set; }
            public string OperatorID { get; set; }
            public string Number { get; set; }
            public string Amount { get; set; }

            public string Status { get; set; }
            public string ResposneMessage { get; set; }



            public string CustomerId { get; set; }
            public string Name { get; set; }
            public string RechargeNo { get; set; }
            public string CStatus { get; set; }
            public string CResponsemsg { get; set; }
            public string RechargeType { get; set; }
            public string RechargeDate { get; set; }
            public string Billstatus { get; set; }
            public string Type { get; set; }

        }
        List<ElectricityGas1> ngo = new List<ElectricityGas1>();
        [HttpGet]
        public ActionResult MobileDthTransaction()
        {

            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");
            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            string Status = ""; string jsonString = null; string str1 = null; string str2 = null; string ac = null;


            DataTable dtList = new DataTable();
            dtList = objtransaction.getTransactionMobileDTHList(null);
            List<ngodarpan> ngo = new List<ngodarpan>();
            if (dtList.Rows.Count > 0)
            {
                string UtransactionId = "";
                ac = "";


                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    UtransactionId = dtList.Rows[i].ItemArray[0].ToString();




                    string strUrl = "";
                     //strUrl = "http://env.specificstep.com/neo/api/status?username=GJ1436&password=gj1436&utransactionid=" + UtransactionId + "&gettransid=true";

                    strUrl = "https://portal.specificstep.com/neo/api/status?username=GJ1460&password=7778869169&utransactionid=" + UtransactionId + "&gettransid=true";



                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);


                    HttpWebResponse httpres = (HttpWebResponse)request.GetResponse();
                    Stream s = (Stream)httpres.GetResponseStream();
                    StreamReader readStream = new StreamReader(s);
                    string dataString = readStream.ReadToEnd();
                    httpres.Close();
                    s.Close();
                    readStream.Close();







                    jsonString = string.Empty;
                    jsonString = JsonConvert.SerializeObject(dataString);
                    str1 = jsonString;
                    str1 = str1.Replace(@"\t", "");
                    object obj = JsonConvert.DeserializeObject(str1);
                    string a = obj.ToString();
                    //DataSet dsDetails = JsonConvert.DeserializeObject<DataSet>(((JObject)obj));

                    //DataSet empObj = JsonConvert.DeserializeObject<DataSet>(a);

                    ngodarpan empObj = JsonConvert.DeserializeObject<ngodarpan>(a);


                    ngo.Add(new ngodarpan
                    {



                        TransactionID = empObj.TransactionID,
                        UtransactionID = empObj.UtransactionID,
                        OperatorID = empObj.OperatorID,
                        Number = empObj.Number,
                        Amount = dtList.Rows[i].ItemArray[7].ToString(),

                        Status = empObj.Status,
                        ResposneMessage = empObj.ResposneMessage,
                        MarginPercentage = empObj.MarginPercentage,
                        MarginAmount = empObj.MarginAmount,
                        CustomerId = dtList.Rows[i].ItemArray[1].ToString(),
                        Name = dtList.Rows[i].ItemArray[2].ToString() + " " + dtList.Rows[i].ItemArray[3].ToString(),

                        RechargeNo = dtList.Rows[i].ItemArray[4].ToString(),
                        CStatus = dtList.Rows[i].ItemArray[5].ToString(),
                        CResponsemsg = dtList.Rows[i].ItemArray[6].ToString(),
                        RechargeType = dtList.Rows[i].ItemArray[9].ToString(),
                        RechargeDate = dtList.Rows[i].ItemArray[8].ToString(),
                        Rechargestatus = dtList.Rows[i].ItemArray[10].ToString()




                    });


                }

                //   ViewBag.TransactionList = ngo;

                // return Json(ngo, JsonRequestBehavior.AllowGet);
                return View(ngo);
            }



            else
            {
                ngo.Clear();
                ngo.Add(new ngodarpan
                {



                    TransactionID = "",
                    UtransactionID = "",
                    OperatorID = "",
                    Number = "",
                    Amount = "",

                    Status = "",
                    ResposneMessage = "",
                    MarginPercentage = "",
                    MarginAmount = "",
                    CustomerId = "",
                    Name = "",

                    RechargeNo = "",
                    CStatus = "",
                    CResponsemsg = "",
                    RechargeType = "",
                    RechargeDate = "",

                    Rechargestatus = ""


                });

                return View(ngo);
            }



            //else
            //{
            //    return RedirectToAction("Login", "Home");
            //}

            // return View();
        }






        [HttpGet]
        public ActionResult ElectricityGasTransaction()
        {

            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            string Status = ""; string jsonString = null; string str1 = null; string str2 = null; string ac = null;


            DataTable dtList = new DataTable();
            dtList = objtransaction.getTransactionElectricityGasList(null);
            List<ElectricityGas1> ngo = new List<ElectricityGas1>();
            if (dtList.Rows.Count > 0)
            {
                string UtransactionId = "";
                ac = "";


                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    UtransactionId = dtList.Rows[i].ItemArray[0].ToString();




                    string strUrl = "";
                    // strUrl = "http://env.specificstep.com/neo/bbps/status?username=GJ1436&password=gj1436&transactionid=&utransactionid=" + UtransactionId;

                    strUrl = "https://portal.specificstep.com/neo/bbps/status?username=GJ1460&password=7778869169&transactionid=&utransactionid=" + UtransactionId;



                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);


                    HttpWebResponse httpres = (HttpWebResponse)request.GetResponse();
                    Stream s = (Stream)httpres.GetResponseStream();
                    StreamReader readStream = new StreamReader(s);
                    string dataString = readStream.ReadToEnd();
                    httpres.Close();
                    s.Close();
                    readStream.Close();







                    jsonString = string.Empty;
                    jsonString = JsonConvert.SerializeObject(dataString);
                    str1 = jsonString;
                    str1 = str1.Replace(@"\t", "");
                    object obj = JsonConvert.DeserializeObject(str1);
                    string a = obj.ToString();
                    //DataSet dsDetails = JsonConvert.DeserializeObject<DataSet>(((JObject)obj));

                    //DataSet empObj = JsonConvert.DeserializeObject<DataSet>(a);

                    ElectricityGas1 empObj = JsonConvert.DeserializeObject<ElectricityGas1>(a);

                    if (empObj.error == "Transaction ID Not Found")
                    {
                        ngo.Add(new ElectricityGas1
                        {

                            TransactionID = "",
                            UtransactionID = dtList.Rows[i].ItemArray[0].ToString(),
                            OperatorID = dtList.Rows[i].ItemArray[1].ToString(),
                            Number = dtList.Rows[i].ItemArray[5].ToString(),
                            Amount = dtList.Rows[i].ItemArray[8].ToString(),

                            Status = "2",
                            ResposneMessage = empObj.error,

                            CustomerId = dtList.Rows[i].ItemArray[2].ToString(),
                            Name = dtList.Rows[i].ItemArray[3].ToString() + " " + dtList.Rows[i].ItemArray[4].ToString(),

                            RechargeNo = dtList.Rows[i].ItemArray[5].ToString(),
                            CStatus = dtList.Rows[i].ItemArray[6].ToString(),
                            CResponsemsg = dtList.Rows[i].ItemArray[7].ToString(),
                            RechargeType = dtList.Rows[i].ItemArray[10].ToString(),
                            RechargeDate = dtList.Rows[i].ItemArray[9].ToString(),

                            Type = dtList.Rows[i].ItemArray[10].ToString(),
                            Billstatus = dtList.Rows[i].ItemArray[11].ToString()




                        });
                    }

                    else
                    {
                        ngo.Add(new ElectricityGas1
                        {


                            TransactionID = empObj.TransactionID,
                            UtransactionID = empObj.UtransactionID,
                            OperatorID = dtList.Rows[i].ItemArray[1].ToString(),
                            Number = dtList.Rows[i].ItemArray[5].ToString(),
                            Amount = dtList.Rows[i].ItemArray[8].ToString(),

                            Status = empObj.Status,
                            ResposneMessage = empObj.ResposneMessage,

                            CustomerId = dtList.Rows[i].ItemArray[2].ToString(),
                            Name = dtList.Rows[i].ItemArray[3].ToString() + " " + dtList.Rows[i].ItemArray[4].ToString(),

                            RechargeNo = dtList.Rows[i].ItemArray[5].ToString(),
                            CStatus = dtList.Rows[i].ItemArray[6].ToString(),
                            CResponsemsg = dtList.Rows[i].ItemArray[7].ToString(),
                            RechargeType = dtList.Rows[i].ItemArray[10].ToString(),
                            RechargeDate = dtList.Rows[i].ItemArray[9].ToString(),


                            Type = dtList.Rows[i].ItemArray[10].ToString(),
                            Billstatus = dtList.Rows[i].ItemArray[11].ToString()




                            //TransactionID = "",
                            //UtransactionID = UtransactionId,
                            //OperatorID = "",
                            //Number = "",
                            //Amount = "1271",

                            //Status = "",
                            //ResposneMessage = "",

                            //CustomerId = "",
                            //Name = "",

                            //RechargeNo = "",
                            //CStatus = "",
                            //CResponsemsg = "",
                            //RechargeType = "",
                            //RechargeDate = "",


                            //Type = "",
                            //Billstatus = ""

                        });
            
                    }

                }

                //   ViewBag.TransactionList = ngo;

                // return Json(ngo, JsonRequestBehavior.AllowGet);
                return View(ngo);
            }
            //else
            //{
            //    return RedirectToAction("Login", "Home");
            //}
            //}


            else
            {


                ngo.Clear();
                ngo.Add(new ElectricityGas1
                {

                    TransactionID = "",
                    UtransactionID = "",
                    OperatorID = "",
                    Number = "",
                    Amount = "",

                    Status = "2",
                    ResposneMessage = "",

                    CustomerId = "",
                    Name = "",

                    RechargeNo = "",
                    CStatus = "",
                    CResponsemsg = "",
                    RechargeType = "",
                    RechargeDate = "",


                    Type = "",
                    Billstatus = ""

                });
            }
            return View(ngo);
            //return View();
        }






        [HttpPost]
        public ActionResult MobileDthTransaction(FormCollection form, Transaction objtransaction)
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
            DateTime FDate; DateTime TDate;
            string Status = ""; string jsonString = null; string str1 = null; string str2 = null; string ac = null;

            var submit = Request["submit"];

            if (submit == "search")
            {
                string Service = Request["ddlservice"];
                if (!string.IsNullOrEmpty(Service) && Service != "---Select Service---")
                {
                    objtransaction.Service = Service;
                }
                int statuscode = Convert.ToInt32(Request["ddlstatus"]);

                if (!string.IsNullOrEmpty(statuscode.ToString()) && statuscode != 5)
                {
                    objtransaction.statuscode = statuscode;
                }


                ViewBag.Servicename = Service;

                ViewBag.statuscode = statuscode;

                string status = Request["ddlstatus"];
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
                DataTable dtList = new DataTable();
                dtList = objtransaction.getTransactionMobileDTHListFilter(objtransaction.Service, objtransaction.statuscode, FDate, TDate);
                List<ngodarpan> ngo = new List<ngodarpan>();
                if (dtList.Rows.Count > 0)
                {
                    ngo.Clear();
                    string UtransactionId = "";
                    ac = "";


                    for (int i = 0; i < dtList.Rows.Count; i++)
                    {
                        UtransactionId = dtList.Rows[i].ItemArray[0].ToString();




                        string strUrl = "";
                       //  strUrl = "http://env.specificstep.com/neo/api/status?username=GJ1436&password=gj1436&utransactionid=" + UtransactionId + "&gettransid=true";
                        strUrl = "https://portal.specificstep.com/neo/api/status?username=GJ1460&password=7778869169&utransactionid=" + UtransactionId + "&gettransid=true";




                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);


                        HttpWebResponse httpres = (HttpWebResponse)request.GetResponse();
                        Stream s = (Stream)httpres.GetResponseStream();
                        StreamReader readStream = new StreamReader(s);
                        string dataString = readStream.ReadToEnd();
                        httpres.Close();
                        s.Close();
                        readStream.Close();







                        jsonString = string.Empty;
                        jsonString = JsonConvert.SerializeObject(dataString);
                        str1 = jsonString;
                        str1 = str1.Replace(@"\t", "");
                        object obj = JsonConvert.DeserializeObject(str1);
                        string a = obj.ToString();
                        //DataSet dsDetails = JsonConvert.DeserializeObject<DataSet>(((JObject)obj));

                        //DataSet empObj = JsonConvert.DeserializeObject<DataSet>(a);

                        ngodarpan empObj = JsonConvert.DeserializeObject<ngodarpan>(a);


                        ngo.Add(new ngodarpan
                        {



                            TransactionID = empObj.TransactionID,
                            UtransactionID = empObj.UtransactionID,
                            OperatorID = empObj.OperatorID,
                            Number = empObj.Number,
                            Amount = dtList.Rows[i].ItemArray[7].ToString(),

                            Status = empObj.Status,
                            ResposneMessage = empObj.ResposneMessage,
                            MarginPercentage = empObj.MarginPercentage,
                            MarginAmount = empObj.MarginAmount,
                            CustomerId = dtList.Rows[i].ItemArray[1].ToString(),
                            Name = dtList.Rows[i].ItemArray[2].ToString() + " " + dtList.Rows[i].ItemArray[3].ToString(),

                            RechargeNo = dtList.Rows[i].ItemArray[4].ToString(),
                            CStatus = dtList.Rows[i].ItemArray[5].ToString(),
                            CResponsemsg = dtList.Rows[i].ItemArray[6].ToString(),
                            RechargeType = dtList.Rows[i].ItemArray[9].ToString(),
                            RechargeDate = dtList.Rows[i].ItemArray[8].ToString(),

                            Rechargestatus = dtList.Rows[i].ItemArray[10].ToString()


                        });


                    }

                    //   ViewBag.TransactionList = ngo;

                    // return Json(ngo, JsonRequestBehavior.AllowGet);
                    return View(ngo);
                }
                else
                {
                    ngo.Clear();
                    ngo.Add(new ngodarpan
                    {



                        TransactionID = "",
                        UtransactionID = "",
                        OperatorID = "",
                        Number = "",
                        Amount = "",

                        Status = "",
                        ResposneMessage = "",
                        MarginPercentage = "",
                        MarginAmount = "",
                        CustomerId = "",
                        Name = "",

                        RechargeNo = "",
                        CStatus = "",
                        CResponsemsg = "",
                        RechargeType = "",
                        RechargeDate = "",

                        Rechargestatus = ""


                    });

                    return View(ngo);
                }

            }

            
            else
            {
                int updateresult = 0;
                // string b = id.ToString();

                submit = Request["submit"];

                if(submit == "Insufficientbalance")
                {



                    string strUrl = "";

                   // strUrl = "https://portal.specificstep.com/neo/api?username=GJ1460&password=7778869169&utransactionid=" + UtransactionId + "&circlecode=" + CircleCode + "&operatorcode=" + OperatorCode + "&number=" + RechargeNo + "&amount=" + RechargeAmount;
                    // Create a request object  
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
                    // Get the response back  


                    HttpWebResponse httpres = (HttpWebResponse)request.GetResponse();
                    Stream s = (Stream)httpres.GetResponseStream();
                    StreamReader readStream = new StreamReader(s);
                  string  dataString = readStream.ReadToEnd();
                    httpres.Close();
                    s.Close();
                    readStream.Close();


                    updateresult = 1;
                }

                //  int id=
                else
                {
                    var itemstatus = Request[submit + "itemstatus"];
                    var itemmsg = Request[submit + "itemmsg"];
                    string rechargestatus = "";
                    if (itemstatus == "1") rechargestatus = "Success";
                    if (itemstatus == "0") rechargestatus = "Pending";
                    if (itemstatus == "2" || itemstatus == "3") rechargestatus = "Fail";
                    if (itemstatus == "1")
                    {
                        updateresult = objtransaction.Updaterecharge(submit, itemstatus, itemmsg, rechargestatus);


                    }
                }
            

                if (updateresult > 0)

                {


                    string Service = Request["ddlservice"];
                    if (!string.IsNullOrEmpty(Service) && Service != "---Select Service---")
                    {
                        objtransaction.Service = Service;
                    }
                    int statuscode = Convert.ToInt32(Request["ddlstatus"]);

                    if (!string.IsNullOrEmpty(statuscode.ToString()) && statuscode != 5)
                    {
                        objtransaction.statuscode = statuscode;
                    }


                    ViewBag.Servicename = Service;

                    ViewBag.statuscode = statuscode;

                    string status = Request["ddlstatus"];
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

                    if (Service != "" && statuscode != 5)
                    {

                        DataTable dtList = new DataTable();
                        dtList = objtransaction.getTransactionMobileDTHListFilter(objtransaction.Service, objtransaction.statuscode, FDate, TDate);
                        List<ngodarpan> ngo = new List<ngodarpan>();
                        if (dtList.Rows.Count > 0)
                        {
                            ngo.Clear();
                            string UtransactionId = "";
                            ac = "";


                            for (int i = 0; i < dtList.Rows.Count; i++)
                            {
                                UtransactionId = dtList.Rows[i].ItemArray[0].ToString();




                                string strUrl = "";
                               // strUrl = "http://env.specificstep.com/neo/api/status?username=GJ1436&password=gj1436&utransactionid=" + UtransactionId + "&gettransid=true";

                                strUrl = "https://portal.specificstep.com/neo/api/status?username=GJ1460&password=7778869169&utransactionid=" + UtransactionId + "&gettransid=true";



                                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);


                                HttpWebResponse httpres = (HttpWebResponse)request.GetResponse();
                                Stream s = (Stream)httpres.GetResponseStream();
                                StreamReader readStream = new StreamReader(s);
                                string dataString = readStream.ReadToEnd();
                                httpres.Close();
                                s.Close();
                                readStream.Close();







                                jsonString = string.Empty;
                                jsonString = JsonConvert.SerializeObject(dataString);
                                str1 = jsonString;
                                str1 = str1.Replace(@"\t", "");
                                object obj = JsonConvert.DeserializeObject(str1);
                                string a = obj.ToString();
                                //DataSet dsDetails = JsonConvert.DeserializeObject<DataSet>(((JObject)obj));

                                //DataSet empObj = JsonConvert.DeserializeObject<DataSet>(a);

                                ngodarpan empObj = JsonConvert.DeserializeObject<ngodarpan>(a);


                                ngo.Add(new ngodarpan
                                {



                                    TransactionID = empObj.TransactionID,
                                    UtransactionID = empObj.UtransactionID,
                                    OperatorID = empObj.OperatorID,
                                    Number = empObj.Number,
                                    Amount = dtList.Rows[i].ItemArray[7].ToString(),

                                    Status = empObj.Status,
                                    ResposneMessage = empObj.ResposneMessage,
                                    MarginPercentage = empObj.MarginPercentage,
                                    MarginAmount = empObj.MarginAmount,
                                    CustomerId = dtList.Rows[i].ItemArray[1].ToString(),
                                    Name = dtList.Rows[i].ItemArray[2].ToString() + " " + dtList.Rows[i].ItemArray[3].ToString(),

                                    RechargeNo = dtList.Rows[i].ItemArray[4].ToString(),
                                    CStatus = dtList.Rows[i].ItemArray[5].ToString(),
                                    CResponsemsg = dtList.Rows[i].ItemArray[6].ToString(),
                                    RechargeType = dtList.Rows[i].ItemArray[9].ToString(),
                                    RechargeDate = dtList.Rows[i].ItemArray[8].ToString(),

                                    Rechargestatus = dtList.Rows[i].ItemArray[10].ToString()


                                });


                            }

                            //   ViewBag.TransactionList = ngo;

                            // return Json(ngo, JsonRequestBehavior.AllowGet);
                            return View(ngo);
                        }
                        else
                        {
                            ngo.Clear();
                            ngo.Add(new ngodarpan
                            {



                                TransactionID = "",
                                UtransactionID = "",
                                OperatorID = "",
                                Number = "",
                                Amount = "",

                                Status = "",
                                ResposneMessage = "",
                                MarginPercentage = "",
                                MarginAmount = "",
                                CustomerId = "",
                                Name = "",

                                RechargeNo = "",
                                CStatus = "",
                                CResponsemsg = "",
                                RechargeType = "",
                                RechargeDate = "",

                                Rechargestatus = ""


                            });

                            return View(ngo);
                        }
                    }
                    else
                    {


                        DataTable dtList = new DataTable();
                        dtList = objtransaction.getTransactionMobileDTHList(null);
                        List<ngodarpan> ngo = new List<ngodarpan>();
                        if (dtList.Rows.Count > 0)
                        {
                            ngo.Clear();
                            string UtransactionId = "";
                            ac = "";


                            for (int i = 0; i < dtList.Rows.Count; i++)
                            {
                                UtransactionId = dtList.Rows[i].ItemArray[0].ToString();




                                string strUrl = "";
                               // strUrl = "http://env.specificstep.com/neo/api/status?username=GJ1436&password=gj1436&utransactionid=" + UtransactionId + "&gettransid=true";

                                strUrl = "https://portal.specificstep.com/neo/api/status?username=GJ1460&password=7778869169&utransactionid=" + UtransactionId + "&gettransid=true";



                                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);


                                HttpWebResponse httpres = (HttpWebResponse)request.GetResponse();
                                Stream s = (Stream)httpres.GetResponseStream();
                                StreamReader readStream = new StreamReader(s);
                                string dataString = readStream.ReadToEnd();
                                httpres.Close();
                                s.Close();
                                readStream.Close();







                                jsonString = string.Empty;
                                jsonString = JsonConvert.SerializeObject(dataString);
                                str1 = jsonString;
                                str1 = str1.Replace(@"\t", "");
                                object obj = JsonConvert.DeserializeObject(str1);
                                string a = obj.ToString();
                                //DataSet dsDetails = JsonConvert.DeserializeObject<DataSet>(((JObject)obj));

                                //DataSet empObj = JsonConvert.DeserializeObject<DataSet>(a);

                                ngodarpan empObj = JsonConvert.DeserializeObject<ngodarpan>(a);


                                ngo.Add(new ngodarpan
                                {



                                    TransactionID = empObj.TransactionID,
                                    UtransactionID = empObj.UtransactionID,
                                    OperatorID = empObj.OperatorID,
                                    Number = empObj.Number,
                                    Amount = dtList.Rows[i].ItemArray[7].ToString(),

                                    Status = empObj.Status,
                                    ResposneMessage = empObj.ResposneMessage,
                                    MarginPercentage = empObj.MarginPercentage,
                                    MarginAmount = empObj.MarginAmount,
                                    CustomerId = dtList.Rows[i].ItemArray[1].ToString(),
                                    Name = dtList.Rows[i].ItemArray[2].ToString() + " " + dtList.Rows[i].ItemArray[3].ToString(),

                                    RechargeNo = dtList.Rows[i].ItemArray[4].ToString(),
                                    CStatus = dtList.Rows[i].ItemArray[5].ToString(),
                                    CResponsemsg = dtList.Rows[i].ItemArray[6].ToString(),
                                    RechargeType = dtList.Rows[i].ItemArray[9].ToString(),
                                    RechargeDate = dtList.Rows[i].ItemArray[8].ToString(),

                                    Rechargestatus = dtList.Rows[i].ItemArray[10].ToString()


                                });


                            }

                            //   ViewBag.TransactionList = ngo;

                            // return Json(ngo, JsonRequestBehavior.AllowGet);
                            return View(ngo);
                        }
                        else
                        {
                            ngo.Clear();
                            ngo.Add(new ngodarpan
                            {



                                TransactionID = "",
                                UtransactionID = "",
                                OperatorID = "",
                                Number = "",
                                Amount = "",

                                Status = "",
                                ResposneMessage = "",
                                MarginPercentage = "",
                                MarginAmount = "",
                                CustomerId = "",
                                Name = "",

                                RechargeNo = "",
                                CStatus = "",
                                CResponsemsg = "",
                                RechargeType = "",
                                RechargeDate = "",

                                Rechargestatus = ""


                            });

                            return View(ngo);
                        }

                    }

                }
            }

            // }
            return View();
        }





        [HttpGet]
        public ActionResult Rerecharge(string id)
        {
            try
            {
                // int delresult = 0;
                string Status = ""; string jsonString = null; string str1 = null; string str2 = null;
                int status = 0;
                string circlecode, operatorcode, rechargeno, rechargeamount,cashbackamount;
                string Utransactionid = id.ToString();
                DateTime cashbackdate;
                decimal Walletbal = 0, marginper = 0, marginamnt = 0, Total2daybal = 0;
                DataTable dt = new DataTable();
                dt = objtransaction.getparticularMobileUtransaction(id.ToString());
                if (dt.Rows.Count > 0)
                {
                    circlecode = dt.Rows[0].ItemArray[0].ToString();
                    operatorcode = dt.Rows[0].ItemArray[1].ToString();
                    rechargeno = dt.Rows[0].ItemArray[2].ToString();
                    rechargeamount = dt.Rows[0].ItemArray[3].ToString();
                    cashbackamount= dt.Rows[0].ItemArray[5].ToString();
                    cashbackdate=Convert.ToDateTime(dt.Rows[0].ItemArray[6]);
                  //  string strUrl = "http://env.specificstep.com/neo/api?username=GJ1436&password=gj1436&utransactionid=" + Utransactionid + "&circlecode=" + circlecode + "&operatorcode=" + operatorcode + "&number=" + rechargeno + "&amount=" + rechargeamount;
                    string strUrl = "https://portal.specificstep.com/neo/api?username=GJ1460&password=7778869169&utransactionid=" + Utransactionid + "&circlecode=" + circlecode + "&operatorcode=" + operatorcode + "&number=" + rechargeno + "&amount=" + rechargeamount;
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
                   
                    string delimStr = ",";
                    char[] delimiter1 = delimStr.ToCharArray();
                    string a = "";
                    int c = 0;
                   string resmessage = "", transactionid = "";
                    foreach (string s1 in str2.Split(delimiter1))
                    {
                        c = c + 1;
                        if (c == 1) transactionid = s1.ToString();
                        if (c == 6) status = Convert.ToInt32(s1);
                        if (c == 7) resmessage = s1.ToString();
                        if (c == 8) marginper = Convert.ToDecimal(s1.ToString());
                        if (c == 9) marginamnt = Convert.ToDecimal(s1.ToString());


                    }

                    MobileRecharge objrecharge = new MobileRecharge();
                    DateTime rdate = Helper.indianTime;


                   // objrecharge.CustomerId = CustomerId;
                   // objrecharge.Rechargeno = RechargeNo;
                    objrecharge.UtransactionId = Utransactionid;
                   // objrecharge.CircleCode = CircleCode.ToString();
                   // objrecharge.OperatorId = OperatorCode.ToString();
                    objrecharge.Status = status;
                    objrecharge.Responsemsg = resmessage;
                    objrecharge.Marginper = marginper.ToString();
                    objrecharge.Marginamnt = marginamnt;
                    objrecharge.TransactionId = transactionid.ToString();


                    objrecharge.CashbackStatus = "Complete";
                    objrecharge.CashbackAmount = cashbackamount.ToString();
                    objrecharge.CashbackDate = cashbackdate;

                    //objrecharge.RechargeAmount = Convert.ToDecimal(RechargeAmount);
                    objrecharge.FromDate = rdate;
                  //  objrecharge.RechargeType = Type.ToString();

                    string rechargestatus = "";
                    if (status == 0) rechargestatus = "Pending";
                    if (status == 1) rechargestatus = "Success";
                    if (status == 2 || status == 3 || status == 4) rechargestatus = "Failure";

                    objrecharge.RechargeStatus = rechargestatus.ToString();
                    //

                    //objsub.Description = "Cashback Recharge- " + Type + ",Transaction Id:" + ",Rechargeno:" + RechargeNo;

                    if (status == 0 || status == 1)
                    {
                        int updatemobilerecharge = objrecharge.Updaterecharge1(objrecharge);
                        //msg = "Recharge Success";
                        //msg1 = "Recharge Done";
                        //msgres = rechargestatus;
                        //msgcashamount = cashbackamnt.ToString();
                        if (updatemobilerecharge > 0)
                        { ViewBag.SuccessMsg = "Recharge Successfull!!!"; }
                        else
                        { ViewBag.SuccessMsg = "Some Internal Error Occur!!!"; }
                    }

                    else
                    {
                        int updatemobilerecharge = objrecharge.Updaterecharge2(objrecharge);
                        //msg = resmessage;
                        //msg1 = "Recharge Fail";
                        //msgres = rechargestatus;
                        //msgcashamount = "0";
                        if (updatemobilerecharge > 0)
                        { ViewBag.SuccessMsg = "Recharge Fail!!!"; }
                        else
                        { ViewBag.SuccessMsg = "Some Internal Error Occur!!!"; }
                    }

                   
                }

              //  int Rerecharge = objtransaction.UpdateRerecharge(id);



                return RedirectToAction("MobileDthTransaction");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_staff_staffcustassign"))
                {
                    TempData["error"] = String.Format("You can not Update. Child record found.");
                }
                else
                    throw ex;
            }
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("MobileDthTransaction");
        }
        [HttpGet]
        public ActionResult Refund(int id)
        {
            try
            {
                // int delresult = 0;
               int delresult = objtransaction.Refund(id);

               
                    return RedirectToAction("MobileDthTransaction");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_staff_staffcustassign"))
                {
                    TempData["error"] = String.Format("You can not Refund. Child record found.");
                }
                else
                    throw ex;
            }
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("MobileDthTransaction");
        }

        [HttpGet]
        public ActionResult Rebillpay(string id)
        {
            try
            {

                string Status = ""; string jsonString = null; string str1 = null; string str2 = null;
                int status = 0;
                string OperatorCode, NumberTag, Amount, Lat, Long, AgentId, PaymentMode, CustomerMobile, Fieldtag1, Fieldtag2, FieldTag3, cashbackamount;
                string Utransactionid = id.ToString();
                DateTime cashbackdate;
                decimal Walletbal = 0, marginper = 0, marginamnt = 0, Total2daybal = 0;
                DataTable dt = new DataTable();
                dt = objtransaction.getparticularBillUtransaction(id.ToString());

                if (dt.Rows.Count > 0)
                {

                    OperatorCode = dt.Rows[0].ItemArray[0].ToString();
                    NumberTag = dt.Rows[0].ItemArray[1].ToString();
                    Amount = dt.Rows[0].ItemArray[2].ToString();
                    Lat = dt.Rows[0].ItemArray[3].ToString();
                    Long = dt.Rows[0].ItemArray[4].ToString();
                    AgentId = dt.Rows[0].ItemArray[5].ToString();
                    PaymentMode = dt.Rows[0].ItemArray[6].ToString();
                    CustomerMobile = dt.Rows[0].ItemArray[7].ToString();
                    Fieldtag1 = dt.Rows[0].ItemArray[8].ToString();
                    Fieldtag2 = dt.Rows[0].ItemArray[9].ToString();
                    FieldTag3 = dt.Rows[0].ItemArray[10].ToString();
                    cashbackamount = dt.Rows[0].ItemArray[11].ToString();
                    cashbackdate = Convert.ToDateTime(dt.Rows[0].ItemArray[13]);
                    //string strUrl = "http://env.specificstep.com/neo/bbps?username=GJ1436&password=gj1436&operatorcode=" + OperatorCode + "&utransactionid=" + Utransactionid + "&number=" + NumberTag + "&amount=" + Amount + "&lat=" + Lat + "&long=" + Long + "&agentid=" + AgentId + "&paymentmode=" + PaymentMode + "&customermobile=" + CustomerMobile + "&field1=" + Fieldtag1 + "&field2=" + Fieldtag2 + "&field3=" + FieldTag3 + "";

                    string strUrl = "https://portal.specificstep.com/neo/bbps?username=GJ1460&password=7778869169&operatorcode=" + OperatorCode + "&utransactionid=" + Utransactionid + "&number=" + NumberTag + "&amount=" + Amount + "&lat=" + Lat + "&long=" + Long + "&agentid=" + AgentId + "&paymentmode=" + PaymentMode + "&customermobile=" + CustomerMobile + "&field1=" + Fieldtag1 + "&field2=" + Fieldtag2 + "&field3=" + FieldTag3 + "";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
                    // Get the response back  


                    HttpWebResponse httpres = (HttpWebResponse)request.GetResponse();
                    Stream s = (Stream)httpres.GetResponseStream();
                    StreamReader readStream = new StreamReader(s);
                    string dataString = readStream.ReadToEnd();
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
                    //response = Request.CreateResponse(HttpStatusCode.OK);
                    //response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    string delimStr = ",";
                    char[] delimiter1 = delimStr.ToCharArray();
                    string a = "";
                    int c = 0;
                    string resmessage = "", transactionid = "";
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
                    DateTime rdate = Helper.indianTime;
                    BillPayment objrecharge = new BillPayment();
                    string billstatus = "";
                    // objrecharge.CustomerId = CustomerId;
                    ////  objrecharge.Rechargeno = RechargeNo;
                    objrecharge.UtransactionId = Utransactionid;
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
                    //objrecharge.RechargeType = Type.ToString();
                    objrecharge.errorcode = "0";
                    objrecharge.Agentid = AgentId.ToString();
                    //  objrecharge.Marginper = marginper.ToString();
                    //  objrecharge.Marginamnt = marginamnt;
                    //  objrecharge.TransactionId = transactionid.ToString();
                    objrecharge.CashbackAmount = cashbackamount.ToString();
                    objrecharge.CashbackDate = cashbackdate;

                    objrecharge.FromDate = rdate;



                    string cstatus = "";
                    if (status == 0)
                    {
                        billstatus = "Pending";
                        objrecharge.CashbackStatus = "Complete";
                        cstatus = "Cr";



                      //  strUrl = "http://env.specificstep.com/neo/bbps/status?username=GJ1436&password=gj1436&transactionid=&utransactionid=" + Utransactionid;


                        strUrl = "https://portal.specificstep.com/neo/bbps/status?username=GJ1460&password=7778869169&transactionid=&utransactionid=" + Utransactionid;


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
                        //response = Request.CreateResponse(HttpStatusCode.OK);
                        //response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

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
                        //msg = "Bill Pay Success";
                        //msg1 = "Bill Pay Done";
                        //msgres = billstatus;
                        //msgcashamount = cashbackamnt.ToString();
                        if (updatebillpayrecharge > 0)
                        { ViewBag.SuccessMsg = "Bill Pay Successfull!!!"; }
                        else
                        { ViewBag.SuccessMsg = "Some Internal Error Occur!!!"; }
                    }
                    else
                    {
                        int updatemobilerecharge = objrecharge.Updaterecharge2(objrecharge);
                        //msg = resmessage;
                        //msg1 = "Bill Pay Fail";
                        //msgres = billstatus;
                        //msgcashamount = "0";
                        if (updatemobilerecharge > 0)
                        { ViewBag.SuccessMsg = "Bill Pay Fail!!!"; }
                        else
                        { ViewBag.SuccessMsg = "Some Internal Error Occur!!!"; }
                    }


                }
            
                //dr["status"] = msg;
                //dr["error_msg"] = "Your " + Type + " " + msg1;
                //dr["Response"] = msgres;
                //dr["Cashback_Amount"] = msgcashamount;
                //dtNew.Rows.Add(dr);

                //jsonString = string.Empty;
                //jsonString = JsonConvert.SerializeObject(dtNew);


                //response = Request.CreateResponse(HttpStatusCode.OK);
                //response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                //return response;
            



                
                return RedirectToAction("ElectricityGasTransaction");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_staff_staffcustassign"))
                {
                    TempData["error"] = String.Format("You can not Refund. Child record found.");
                }
                else
                    throw ex;
            }
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("ElectricityGasTransaction");
        }




        [HttpGet]
        public ActionResult Refund1(int id)
        {
            try
            {
                // int delresult = 0;
                int delresult = objtransaction.Refund1(id);


                return RedirectToAction("ElectricityGasTransaction");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_staff_staffcustassign"))
                {
                    TempData["error"] = String.Format("You can not Refund. Child record found.");
                }
                else
                    throw ex;
            }
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("ElectricityGasTransaction");
        }
        [HttpPost]
        public ActionResult ElectricityGasTransaction(FormCollection form, Transaction objtransaction)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            DateTime FDate; DateTime TDate;
            string Status = ""; string jsonString = null; string str1 = null; string str2 = null; string ac = null;
            string Service = Request["ddlservice"];

            var submit = Request["submit"];

            if (submit == "search")
            {
                if (!string.IsNullOrEmpty(Service) && Service != "---Select Service---")
                {
                    objtransaction.Service = Service;
                }
                int statuscode = Convert.ToInt32(Request["ddlstatus"]);

                if (!string.IsNullOrEmpty(statuscode.ToString()) && statuscode != 6)
                {
                    objtransaction.statuscode = statuscode;
                }


                ViewBag.Servicename = Service;

                ViewBag.statuscode = statuscode;

                string status = Request["ddlstatus"];
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
                DataTable dtList = new DataTable();
                dtList = objtransaction.getTransactionElectricityGasListFilter(objtransaction.Service, objtransaction.statuscode, FDate, TDate);
                List<ElectricityGas1> ngo = new List<ElectricityGas1>();
                if (dtList.Rows.Count > 0)
                {
                    string UtransactionId = "";
                    ac = "";
                    ngo.Clear();

                    for (int i = 0; i < dtList.Rows.Count; i++)
                    {
                        UtransactionId = dtList.Rows[i].ItemArray[0].ToString();




                        string strUrl = "";
                       // strUrl = "http://env.specificstep.com/neo/bbps/status?username=GJ1436&password=gj1436&transactionid=&utransactionid=" + UtransactionId;
                       strUrl = "https://portal.specificstep.com/neo/bbps/status?username=GJ1460&password=7778869169&transactionid=&utransactionid=" + UtransactionId;




                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);


                        HttpWebResponse httpres = (HttpWebResponse)request.GetResponse();
                        Stream s = (Stream)httpres.GetResponseStream();
                        StreamReader readStream = new StreamReader(s);
                        string dataString = readStream.ReadToEnd();
                        httpres.Close();
                        s.Close();
                        readStream.Close();







                        jsonString = string.Empty;
                        jsonString = JsonConvert.SerializeObject(dataString);
                        str1 = jsonString;
                        str1 = str1.Replace(@"\t", "");
                        object obj = JsonConvert.DeserializeObject(str1);
                        string a = obj.ToString();
                        //DataSet dsDetails = JsonConvert.DeserializeObject<DataSet>(((JObject)obj));

                        //DataSet empObj = JsonConvert.DeserializeObject<DataSet>(a);

                        ElectricityGas1 empObj = JsonConvert.DeserializeObject<ElectricityGas1>(a);

                        if (empObj.error == "Transaction ID Not Found")
                        {
                            ngo.Add(new ElectricityGas1
                            {

                                TransactionID = "",
                                UtransactionID = dtList.Rows[i].ItemArray[0].ToString(),
                                OperatorID = dtList.Rows[i].ItemArray[1].ToString(),
                                Number = dtList.Rows[i].ItemArray[5].ToString(),
                                Amount = dtList.Rows[i].ItemArray[8].ToString(),

                                Status = "2",
                                ResposneMessage = empObj.error,

                                CustomerId = dtList.Rows[i].ItemArray[2].ToString(),
                                Name = dtList.Rows[i].ItemArray[3].ToString() + " " + dtList.Rows[i].ItemArray[4].ToString(),

                                RechargeNo = dtList.Rows[i].ItemArray[5].ToString(),
                                CStatus = dtList.Rows[i].ItemArray[6].ToString(),
                                CResponsemsg = dtList.Rows[i].ItemArray[7].ToString(),
                                RechargeType = dtList.Rows[i].ItemArray[10].ToString(),
                                RechargeDate = dtList.Rows[i].ItemArray[9].ToString(),


                                Type = dtList.Rows[i].ItemArray[10].ToString(),
                                Billstatus = dtList.Rows[i].ItemArray[11].ToString()

                            });
                        }

                        else
                        {
                            ngo.Add(new ElectricityGas1
                            {

                                TransactionID = empObj.TransactionID,
                                UtransactionID = empObj.UtransactionID,
                                OperatorID = dtList.Rows[i].ItemArray[1].ToString(),
                                Number = dtList.Rows[i].ItemArray[5].ToString(),
                                Amount = dtList.Rows[i].ItemArray[8].ToString(),

                                Status = empObj.Status,
                                ResposneMessage = empObj.ResposneMessage,

                                CustomerId = dtList.Rows[i].ItemArray[2].ToString(),
                                Name = dtList.Rows[i].ItemArray[3].ToString() + " " + dtList.Rows[i].ItemArray[4].ToString(),

                                RechargeNo = dtList.Rows[i].ItemArray[5].ToString(),
                                CStatus = dtList.Rows[i].ItemArray[6].ToString(),
                                CResponsemsg = dtList.Rows[i].ItemArray[7].ToString(),
                                RechargeType = dtList.Rows[i].ItemArray[10].ToString(),
                                RechargeDate = dtList.Rows[i].ItemArray[9].ToString(),



                                Type = dtList.Rows[i].ItemArray[10].ToString(),
                                Billstatus = dtList.Rows[i].ItemArray[11].ToString()
                            });
                        }

                    }


                    //   ViewBag.TransactionList = ngo;

                    // return Json(ngo, JsonRequestBehavior.AllowGet);
                    return View(ngo);
                }
                else
                {


                    ngo.Clear();
                    ngo.Add(new ElectricityGas1
                    {

                        TransactionID = "",
                        UtransactionID = "",
                        OperatorID = "",
                        Number = "",
                        Amount = "",

                        Status = "2",
                        ResposneMessage = "",

                        CustomerId = "",
                        Name = "",

                        RechargeNo = "",
                        CStatus = "",
                        CResponsemsg = "",
                        RechargeType = "",
                        RechargeDate = "",


                        Type = "",
                        Billstatus = ""

                    });
                }
                return View(ngo);
            }


            else if(submit=="")
            {

            }


            else
            {

                int updateresult = 0;
                int updatebillpayrecharge = 0;
                int updatemobilerecharge = 0;
                // string b = id.ToString();
                submit = Request["submit"];
                //  int id=
                var itemstatus = Request[submit + "itemstatus"];
                var itemmsg = Request[submit + "itemmsg"];
                var prevmsg= Request[submit + "Previousmsg"];
                string rechargestatus = "";
                if (itemstatus == "1") rechargestatus = "Success";
                if (itemstatus == "0") rechargestatus = "Pending";
                if (itemstatus == "2" || itemstatus == "3" || itemstatus == "4") rechargestatus = "Fail";
                if (itemstatus == "1")
                {
                    updateresult = objtransaction.Updaterechargeelectricity(submit, itemstatus, itemmsg, rechargestatus);


                }
                if((prevmsg== "Insufficient Balance With Provider." || prevmsg== "Insufficient Balance.") && itemstatus != "1")
                {
                    SubscriptionNew obj = new SubscriptionNew();
                    Subscription objsub = new Subscription();
                    BillPayment objrecharge = new BillPayment();

                    string strUrl = "";


                   // strUrl = "http://env.specificstep.com/neo/api/balance?username=GJ1436&password=gj1436";
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
                  //  var response = Request.CreateResponse(HttpStatusCode.OK);
                   // response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

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

                    //Insufficient Balance Code
                    int CustomerId; int OperatorCode; string NumberTag; string Fieldtag1; string Type; int Amount; string Lat; string Long; string AgentId; string PaymentMode;
                    string CustomerMobile; string Fieldtag2; string FieldTag3;string UtransactionIdnew = "";

                    UtransactionIdnew = Request[submit + "UtransactionID"];
                    CustomerId = Convert.ToInt32(Request[submit + "CustomerId"]);
                    DataTable dtList = new DataTable();
                    dtList = objtransaction.getparticularUtransaction(UtransactionIdnew,CustomerId);
                    if(dtList.Rows.Count>0)
                    {
                       
                        NumberTag = dtList.Rows[0].ItemArray[3].ToString();
                        Amount = Convert.ToInt32(dtList.Rows[0].ItemArray[4].ToString());
                        Lat = dtList.Rows[0].ItemArray[5].ToString();
                        Long = dtList.Rows[0].ItemArray[6].ToString();
                        OperatorCode = Convert.ToInt32(dtList.Rows[0].ItemArray[7].ToString());

                        PaymentMode = dtList.Rows[0].ItemArray[8].ToString();
                        CustomerMobile = dtList.Rows[0].ItemArray[9].ToString();
                        Fieldtag1= dtList.Rows[0].ItemArray[10].ToString();
                       Fieldtag2=dtList.Rows[0].ItemArray[11].ToString();
                        FieldTag3= dtList.Rows[0].ItemArray[12].ToString();

                        AgentId = dtList.Rows[0].ItemArray[17].ToString();
                        Type = dtList.Rows[0].ItemArray[18].ToString();

                       










                        if (balance >= Amount)
                        {
                           // strUrl = "http://env.specificstep.com/neo/bbps?username=GJ1436&password=gj1436&operatorcode=" + OperatorCode + "&utransactionid=" + UtransactionIdnew + "&number=" + NumberTag + "&amount=" + Amount + "&lat=" + Lat + "&long=" + Long + "&agentid=" + AgentId + "&paymentmode=" + PaymentMode + "&customermobile=" + CustomerMobile + "&field1=" + Fieldtag1 + "&field2=" + Fieldtag2 + "&field3=" + FieldTag3 + "";
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
                            //  response = Request.CreateResponse(HttpStatusCode.OK);
                            // response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                            delimStr = ",";
                            char[] delimiter1 = delimStr.ToCharArray();
                            a = "";
                            c = 0;
                            int status = 0;
                            string resmessage = ""; string transactionid = "";
                            foreach (string s1 in str2.Split(delimiter1))
                            {
                                if (s1 == "error:Please leave an interval of five minutes before recharging the same number.")
                                {
                                    resmessage = s1.ToString();
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
                                objrecharge.CashbackStatus = "";
                                cstatus = "Cr";
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

                            objrecharge.Billstatus = billstatus.ToString();
                            //


                            if (status == 0 || status == 1)
                            {
                                 updatebillpayrecharge = objrecharge.Updaterecharge1(objrecharge);

                                if (updatebillpayrecharge > 0)
                                {
                                    ViewBag.msg = "Your Transaction With UtransactionId:" +UtransactionIdnew +" Updated";
                                }
                            }
                            else
                            {
                                 updatemobilerecharge = objrecharge.Updaterecharge2(objrecharge);
                                if (updatemobilerecharge > 0)
                                {
                                    ViewBag.msg = "Your Transaction With UtransactionId:" + UtransactionIdnew + " Updated";
                                }
                            }
                          //  int updatebillpayrecharge = objrecharge.Updaterecharge1(objrecharge);

                            //
                        }

                        else
                        {
                            ViewBag.msg = "Your Balance WIth Provider Still Low";
                        }
                    }

                   
                     
                   
                  
                   
                   
                    //AgentId.ToString();
           


                }

                

                    ////


                     GetList();

               

               

            }


            //}
            return View(ngo);
        }



     public   List<ElectricityGas1> GetList()
        {
            DateTime FDate; DateTime TDate;
            string Status = ""; string jsonString = null; string str1 = null; string str2 = null; string ac = null;
            string Service = Request["ddlservice"];

            if (!string.IsNullOrEmpty(Service) && Service != "---Select Service---")
            {
                objtransaction.Service = Service;
            }
            int statuscode = Convert.ToInt32(Request["ddlstatus"]);

            if (!string.IsNullOrEmpty(statuscode.ToString()) && statuscode != 6)
            {
                objtransaction.statuscode = statuscode;
            }


            ViewBag.Servicename = Service;

            ViewBag.statuscode = statuscode;

            string status = Request["ddlstatus"];
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

            if (Service != "" && statuscode != 6)
            {

                DataTable dtList = new DataTable();
                dtList = objtransaction.getTransactionElectricityGasListFilter(objtransaction.Service, objtransaction.statuscode, FDate, TDate);
                
                if (dtList.Rows.Count > 0)
                {
                    string UtransactionId = "";
               
                    ngo.Clear();

                    for (int i = 0; i < dtList.Rows.Count; i++)
                    {
                        UtransactionId = dtList.Rows[i].ItemArray[0].ToString();




                        string strUrl = "";
                        //strUrl = "http://env.specificstep.com/neo/bbps/status?username=GJ1436&password=gj1436&transactionid=&utransactionid=" + UtransactionId;
                        strUrl = "https://portal.specificstep.com/neo/bbps/status?username=GJ1460&password=7778869169&transactionid=&utransactionid=" + UtransactionId;




                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);


                        HttpWebResponse httpres = (HttpWebResponse)request.GetResponse();
                        Stream s = (Stream)httpres.GetResponseStream();
                        StreamReader readStream = new StreamReader(s);
                        string dataString = readStream.ReadToEnd();
                        httpres.Close();
                        s.Close();
                        readStream.Close();







                        jsonString = string.Empty;
                        jsonString = JsonConvert.SerializeObject(dataString);
                        str1 = jsonString;
                        str1 = str1.Replace(@"\t", "");
                        object obj = JsonConvert.DeserializeObject(str1);
                        string a = obj.ToString();
                        //DataSet dsDetails = JsonConvert.DeserializeObject<DataSet>(((JObject)obj));

                        //DataSet empObj = JsonConvert.DeserializeObject<DataSet>(a);

                        ElectricityGas1 empObj = JsonConvert.DeserializeObject<ElectricityGas1>(a);

                        if (empObj.error == "Transaction ID Not Found")
                        {
                            ngo.Add(new ElectricityGas1
                            {

                                TransactionID = "",
                                UtransactionID = dtList.Rows[i].ItemArray[0].ToString(),
                                OperatorID = dtList.Rows[i].ItemArray[1].ToString(),
                                Number = dtList.Rows[i].ItemArray[5].ToString(),
                                Amount = dtList.Rows[i].ItemArray[8].ToString(),

                                Status = "2",
                                ResposneMessage = empObj.error,

                                CustomerId = dtList.Rows[i].ItemArray[2].ToString(),
                                Name = dtList.Rows[i].ItemArray[3].ToString() + " " + dtList.Rows[i].ItemArray[4].ToString(),

                                RechargeNo = dtList.Rows[i].ItemArray[5].ToString(),
                                CStatus = dtList.Rows[i].ItemArray[6].ToString(),
                                CResponsemsg = dtList.Rows[i].ItemArray[7].ToString(),
                                RechargeType = dtList.Rows[i].ItemArray[10].ToString(),
                                RechargeDate = dtList.Rows[i].ItemArray[9].ToString(),


                                Type = dtList.Rows[i].ItemArray[10].ToString(),
                                Billstatus = dtList.Rows[i].ItemArray[11].ToString()

                            });
                        }

                        else
                        {
                            ngo.Add(new ElectricityGas1
                            {

                                TransactionID = empObj.TransactionID,
                                UtransactionID = empObj.UtransactionID,
                                OperatorID = dtList.Rows[i].ItemArray[1].ToString(),
                                Number = dtList.Rows[i].ItemArray[5].ToString(),
                                Amount = dtList.Rows[i].ItemArray[8].ToString(),

                                Status = empObj.Status,
                                ResposneMessage = empObj.ResposneMessage,

                                CustomerId = dtList.Rows[i].ItemArray[2].ToString(),
                                Name = dtList.Rows[i].ItemArray[3].ToString() + " " + dtList.Rows[i].ItemArray[4].ToString(),

                                RechargeNo = dtList.Rows[i].ItemArray[5].ToString(),
                                CStatus = dtList.Rows[i].ItemArray[6].ToString(),
                                CResponsemsg = dtList.Rows[i].ItemArray[7].ToString(),
                                RechargeType = dtList.Rows[i].ItemArray[10].ToString(),
                                RechargeDate = dtList.Rows[i].ItemArray[9].ToString(),



                                Type = dtList.Rows[i].ItemArray[10].ToString(),
                                Billstatus = dtList.Rows[i].ItemArray[11].ToString()
                            });
                        }

                    }


                    //   ViewBag.TransactionList = ngo;

                    // return Json(ngo, JsonRequestBehavior.AllowGet);
                   // return ngo;
                }
                else
                {


                    ngo.Clear();
                    ngo.Add(new ElectricityGas1
                    {

                        TransactionID = "",
                        UtransactionID = "",
                        OperatorID = "",
                        Number = "",
                        Amount = "",

                        Status = "2",
                        ResposneMessage = "",

                        CustomerId = "",
                        Name = "",

                        RechargeNo = "",
                        CStatus = "",
                        CResponsemsg = "",
                        RechargeType = "",
                        RechargeDate = "",


                        Type = "",
                        Billstatus = ""

                    });
                }
                
            }




            else
            {

                DataTable dtList = new DataTable();
                dtList = objtransaction.getTransactionElectricityGasList(null);
               // List<ElectricityGas1> ngo = new List<ElectricityGas1>();
                if (dtList.Rows.Count > 0)
                {
                    string UtransactionId = "";
                    ac = "";
                    ngo.Clear();

                    for (int i = 0; i < dtList.Rows.Count; i++)
                    {
                        UtransactionId = dtList.Rows[i].ItemArray[0].ToString();




                        string strUrl = "";
                        //strUrl = "http://env.specificstep.com/neo/bbps/status?username=GJ1436&password=gj1436&transactionid=&utransactionid=" + UtransactionId;
                        strUrl = "https://portal.specificstep.com/neo/bbps/status?username=GJ1460&password=7778869169&transactionid=&utransactionid=" + UtransactionId;




                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);


                        HttpWebResponse httpres = (HttpWebResponse)request.GetResponse();
                        Stream s = (Stream)httpres.GetResponseStream();
                        StreamReader readStream = new StreamReader(s);
                        string dataString = readStream.ReadToEnd();
                        httpres.Close();
                        s.Close();
                        readStream.Close();







                        jsonString = string.Empty;
                        jsonString = JsonConvert.SerializeObject(dataString);
                        str1 = jsonString;
                        str1 = str1.Replace(@"\t", "");
                        object obj = JsonConvert.DeserializeObject(str1);
                        string a = obj.ToString();
                        //DataSet dsDetails = JsonConvert.DeserializeObject<DataSet>(((JObject)obj));

                        //DataSet empObj = JsonConvert.DeserializeObject<DataSet>(a);

                        ElectricityGas1 empObj = JsonConvert.DeserializeObject<ElectricityGas1>(a);

                        if (empObj.error == "Transaction ID Not Found")
                        {
                            ngo.Add(new ElectricityGas1
                            {

                                TransactionID = "",
                                UtransactionID = dtList.Rows[i].ItemArray[0].ToString(),
                                OperatorID = dtList.Rows[i].ItemArray[1].ToString(),
                                Number = dtList.Rows[i].ItemArray[5].ToString(),
                                Amount = dtList.Rows[i].ItemArray[8].ToString(),

                                Status = "2",
                                ResposneMessage = empObj.error,

                                CustomerId = dtList.Rows[i].ItemArray[2].ToString(),
                                Name = dtList.Rows[i].ItemArray[3].ToString() + " " + dtList.Rows[i].ItemArray[4].ToString(),

                                RechargeNo = dtList.Rows[i].ItemArray[5].ToString(),
                                CStatus = dtList.Rows[i].ItemArray[6].ToString(),
                                CResponsemsg = dtList.Rows[i].ItemArray[7].ToString(),
                                RechargeType = dtList.Rows[i].ItemArray[10].ToString(),
                                RechargeDate = dtList.Rows[i].ItemArray[9].ToString(),


                                Type = dtList.Rows[i].ItemArray[10].ToString(),
                                Billstatus = dtList.Rows[i].ItemArray[11].ToString()

                            });
                        }

                        else
                        {
                            ngo.Add(new ElectricityGas1
                            {

                                TransactionID = empObj.TransactionID,
                                UtransactionID = empObj.UtransactionID,
                                OperatorID = dtList.Rows[i].ItemArray[1].ToString(),
                                Number = dtList.Rows[i].ItemArray[5].ToString(),
                                Amount = dtList.Rows[i].ItemArray[8].ToString(),

                                Status = empObj.Status,
                                ResposneMessage = empObj.ResposneMessage,

                                CustomerId = dtList.Rows[i].ItemArray[2].ToString(),
                                Name = dtList.Rows[i].ItemArray[3].ToString() + " " + dtList.Rows[i].ItemArray[4].ToString(),

                                RechargeNo = dtList.Rows[i].ItemArray[5].ToString(),
                                CStatus = dtList.Rows[i].ItemArray[6].ToString(),
                                CResponsemsg = dtList.Rows[i].ItemArray[7].ToString(),
                                RechargeType = dtList.Rows[i].ItemArray[10].ToString(),
                                RechargeDate = dtList.Rows[i].ItemArray[9].ToString(),



                                Type = dtList.Rows[i].ItemArray[10].ToString(),
                                Billstatus = dtList.Rows[i].ItemArray[11].ToString()
                            });
                        }

                    }


                    //   ViewBag.TransactionList = ngo;

                    // return Json(ngo, JsonRequestBehavior.AllowGet);
                   // return ngo;
                }
                else
                {


                    ngo.Clear();
                    ngo.Add(new ElectricityGas1
                    {

                        TransactionID = "",
                        UtransactionID = "",
                        OperatorID = "",
                        Number = "",
                        Amount = "",

                        Status = "2",
                        ResposneMessage = "",

                        CustomerId = "",
                        Name = "",

                        RechargeNo = "",
                        CStatus = "",
                        CResponsemsg = "",
                        RechargeType = "",
                        RechargeDate = "",


                        Type = "",
                        Billstatus = ""

                    });
                }
              //  return ngo;
            }
            return ngo;
        }
    }
}
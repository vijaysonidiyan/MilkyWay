using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MilkWayIndia.Models;
using System.Data;
using MilkWayIndia.Concrete;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace MilkWayIndia.Controllers.Reportsnew
{
    public class ReportnewController : Controller
    {
        // GET: Reportnew
        Customer objcust = new Customer();
        Vendor objvendor = new Vendor();
        EFDbContext db = new EFDbContext();
        Dictionary<string, object> res = new Dictionary<string, object>();
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult DeliveryBoyDailyReportvendor1(int? DeliveryboyId, int? CustomerId, string FDate, string TDate, string status)
        {
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            DateTime _fdate = DateTime.Now, _tdate = DateTime.Now;
            if (!string.IsNullOrEmpty(FDate.ToString()))
                _fdate = Convert.ToDateTime(DateTime.ParseExact(FDate, @"dd-MM-yyyy", null));
            if (!string.IsNullOrEmpty(TDate.ToString()))
                _tdate = Convert.ToDateTime(DateTime.ParseExact(TDate, @"dd-MM-yyyy", null));

            CustomerOrder order = new CustomerOrder();
            ViewBag.FromDate = _fdate.ToString("dd-MMM-yyyy");
            ViewBag.FromDate = FDate;
            ViewBag.ToDate = TDate;
            var dtList = order.getDeliveryBoyWiseOrdervendor(DeliveryboyId, CustomerId, _fdate, _tdate, status);
            ViewBag.ProductorderList = dtList;

           
            string a = "";
            string sid = "";
            CustomerOrderVendor order1 = new CustomerOrderVendor();
            var dtListsector= order1.getDeliveryBoyWiseOrdervendorsector(DeliveryboyId, CustomerId, _fdate, _tdate, status);

            if (dtListsector.Rows.Count > 0)
            {

                DataTable dt1 = new DataTable();
                DataRow dr = null;
                DataTable dt = new DataTable();
                //List<SectorViewModel> list = new List<SectorViewModel>();
                for (int i = 0; i < dtListsector.Rows.Count; i++)
                {


                    sid = dtListsector.Rows[i].ItemArray[0].ToString();
                    order = new CustomerOrder();
                    dt = order.GetMultiSectorVendorOrder1(sid, _fdate, _tdate, DeliveryboyId);
                    dt1.Merge(dt);
                    dt.Clear();

                }
                

                //sid = dtList.Rows[0].ItemArray[12].ToString();

               
                ViewBag.ProductorderList1 = dt1;
            }

            //DateTime _fdate = DateTime.Now, _tdate = DateTime.Now;
            //if (!string.IsNullOrEmpty(FDate.ToString()))
            //    _fdate = Convert.ToDateTime(DateTime.ParseExact(FDate, @"dd-MM-yyyy", null));
            //if (!string.IsNullOrEmpty(TDate.ToString()))
            //    _tdate = Convert.ToDateTime(DateTime.ParseExact(TDate, @"dd-MM-yyyy", null));

            //order = new CustomerOrder();
            //ViewBag.FromDate = _fdate.ToString("dd-MMM-yyyy");
            //ViewBag.ToDate = _tdate.ToString("dd-MMM-yyyy");
            // var dtList1 = order.GetMultiSectorVendorOrder(SectorId, _fdate, _tdate);
            //ViewBag.ProductorderList = dtList1;
            return View();
        }




        [HttpGet]
        public ActionResult DailyBillPayReport()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                string requirebalance = "0";
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                //DateTime FromDate = DateTime.Now;
                //DateTime ToDate = DateTime.Now;

                CashBack objService = new CashBack();
                DataTable dtService = new DataTable();
                dtService = objService.getService(null);
                ViewBag.Service = dtService;
                //DataTable dtList = new DataTable();
                //dtList = objcashback.getCashbackflipamzList(null);
                //ViewBag.CashBackList = dtList;

              DateTime  FDate = DateTime.Today;
               DateTime TDate = DateTime.Today;
                CashBack objcashback = new CashBack();
                DataTable dtCashbackbillList = new DataTable();
                dtCashbackbillList = objcashback.getCashbackBillListReport(null,FDate,TDate);
                ViewBag.CashbackBillList = dtCashbackbillList;

                string Status = ""; string jsonString = null; string str1 = null; string str2 = null; string ac = null;
                //

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


                ViewBag.Balance = balance.ToString();
                //
                double requirebalancenew = 0;

                requirebalance =  getrequirebalance();
                if(balance<=Convert.ToDouble(requirebalance))
                {
                    requirebalancenew = Convert.ToDouble(requirebalance) - balance;
                }
                if (balance > Convert.ToDouble(requirebalance))
                {
                    requirebalancenew = 0;
                }

                if (Convert.ToDouble(requirebalancenew) >0)
                {
                    ViewBag.requirebalance = requirebalancenew.ToString();
                }
                else
                    ViewBag.requirebalance = "0";

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpGet]
        public ActionResult DeliveryBoyCurrentStatus()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            DeliveryBoy objdeliveryboy = new DeliveryBoy();
            DateTime FDate; DateTime TDate;

            FDate = DateTime.Today;
            TDate = DateTime.Today;

            DataTable dtDMStatusList = new DataTable();
            dtDMStatusList = objdeliveryboy.getDmstatusListReport(FDate, TDate);
            ViewBag.DMStatusList = dtDMStatusList;

            DataTable dtDm = new DataTable();
            dtDm = objdeliveryboy.getDm(null);
            ViewBag.Dm = dtDm;

            return View();
        }

        [HttpGet]
        public ActionResult DeliveryBoyProductPhoto()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            DeliveryBoy objdeliveryboy = new DeliveryBoy();
            DateTime FDate; DateTime TDate;

            FDate = DateTime.Today;
            TDate = DateTime.Today;

            DataTable dtDMStatusList = new DataTable();
            dtDMStatusList = objdeliveryboy.getDmphotoListReport(FDate, TDate);
            ViewBag.DMStatusList = dtDMStatusList;

            DataTable dtDm = new DataTable();
            dtDm = objdeliveryboy.getDm(null);
            ViewBag.Dm = dtDm;

            return View();
        }


        [HttpGet]
        public ActionResult DeliveryBoyCashReport()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            DeliveryBoy objdeliveryboy = new DeliveryBoy();
            DateTime FDate; DateTime TDate;

            FDate = DateTime.Today;
            TDate = DateTime.Today;

            DataTable dtDMStatusList = new DataTable();
            dtDMStatusList = objdeliveryboy.getDmCashListReport(FDate, TDate);
            ViewBag.DMStatusList = dtDMStatusList;

            DataTable dtDm = new DataTable();
            dtDm = objdeliveryboy.getDm(null);
            ViewBag.Dm = dtDm;

            return View();
        }

        [HttpGet]
        public ActionResult DeliveryBoyDoc()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            DeliveryBoy objdeliveryboy = new DeliveryBoy();
            DateTime FDate; DateTime TDate;

            FDate = DateTime.Today;
            TDate = DateTime.Today;

            DataTable dtDMStatusList = new DataTable();
            dtDMStatusList = objdeliveryboy.getDmDocListReport(FDate, TDate);
            ViewBag.DMStatusList = dtDMStatusList;

            DataTable dtDm = new DataTable();
            dtDm = objdeliveryboy.getDm(null);
            ViewBag.Dm = dtDm;

            return View();
        }



        [HttpGet]
        public ActionResult DmDocView(int id = 0)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            DeliveryBoy objdeliveryboy = new DeliveryBoy();
           
            DataTable dtDMStatusList = new DataTable();
            dtDMStatusList = objdeliveryboy.getDmDocReport(id);
            //ViewBag.DMStatusList = dtDMStatusList;

            //DataTable dtDm = new DataTable();
            //dtDm = objdeliveryboy.getDm(null);
            //ViewBag.Dm = dtDm;
            var model = new DeliveryBoy();
            if (dtDMStatusList.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["BankAccount"].ToString()))
                    ViewBag.BankAccount = dtDMStatusList.Rows[0]["BankAccount"].ToString();
                else
                    ViewBag.BankAccount = "";

                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Ifsc"].ToString()))
                    ViewBag.Ifsc = dtDMStatusList.Rows[0]["Ifsc"].ToString();
                else
                    ViewBag.Ifsc = "";

                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Bankname"].ToString()))
                    ViewBag.Bankname = dtDMStatusList.Rows[0]["Bankname"].ToString();
                else
                    ViewBag.Bankname = "";

                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["AccholderName"].ToString()))
                    ViewBag.AccholderName = dtDMStatusList.Rows[0]["AccholderName"].ToString();
                else
                    ViewBag.AccholderName = "";


                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Status"].ToString()))
                    ViewBag.Status = dtDMStatusList.Rows[0]["Status"].ToString();
                else
                    ViewBag.Status = "";


                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Aadharstatus"].ToString()))
                    ViewBag.Aadharstatus = dtDMStatusList.Rows[0]["Aadharstatus"].ToString();
                else
                    ViewBag.Aadharstatus = "";

                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Panstatus"].ToString()))
                    ViewBag.Panstatus = dtDMStatusList.Rows[0]["Panstatus"].ToString();
                else
                    ViewBag.Panstatus = "";

                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Licensestatus"].ToString()))
                    ViewBag.Licensestatus = dtDMStatusList.Rows[0]["Licensestatus"].ToString();
                else
                    ViewBag.Licensestatus = "";

                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Bankstatus"].ToString()))
                    ViewBag.Bankstatus = dtDMStatusList.Rows[0]["Bankstatus"].ToString();
                else
                    ViewBag.Bankstatus = "";



                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Aadhar"].ToString()))
                    ViewBag.Aadhar = dtDMStatusList.Rows[0]["Aadhar"].ToString();
                else
                    ViewBag.Aadhar = "N/A";

                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Pan"].ToString()))
                    ViewBag.Pan = dtDMStatusList.Rows[0]["Pan"].ToString();
                else
                    ViewBag.Pan = "N/A";

                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["License"].ToString()))
                    ViewBag.License = dtDMStatusList.Rows[0]["License"].ToString();
                else
                    ViewBag.License = "N/A";
                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Decsiption"].ToString()))
                    ViewBag.Decsiption = dtDMStatusList.Rows[0]["Decsiption"].ToString();
                else
                    ViewBag.Decsiption = "N/A";

                model.Description = ViewBag.Decsiption;
            }



            return View(model);
        }
        [HttpPost]
        public ActionResult DeliveryBoyCurrentStatus(FormCollection form, DeliveryBoy objdeliveryboy)
        {
            string requirebalance = "0";
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            DateTime FDate; DateTime TDate;


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

            string Dmid = Request["ddlDm"];
            if (!string.IsNullOrEmpty(Dmid) && Dmid != "0")
            {
                objdeliveryboy.Id = Convert.ToInt32(Dmid);
            }
            else
                objdeliveryboy.Id = 0;
            ViewBag.Dm1 = Dmid;
            DataTable dtDMStatusList = new DataTable();
            dtDMStatusList = objdeliveryboy.getDmstatusListReport1(objdeliveryboy.Id,FDate, TDate);
            ViewBag.DMStatusList = dtDMStatusList;

            DataTable dtDm = new DataTable();
            dtDm = objdeliveryboy.getDm(null);
            ViewBag.Dm = dtDm;

            return View();
        }



        [HttpPost]
        public ActionResult DeliveryBoyProductPhoto(FormCollection form, DeliveryBoy objdeliveryboy)
        {
            string requirebalance = "0";
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            DateTime FDate; DateTime TDate;


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

            string Dmid = Request["ddlDm"];
            if (!string.IsNullOrEmpty(Dmid) && Dmid != "0")
            {
                objdeliveryboy.Id = Convert.ToInt32(Dmid);
            }
            else
                objdeliveryboy.Id = 0;
            ViewBag.Dm1 = Dmid;
            DataTable dtDMStatusList = new DataTable();
            dtDMStatusList = objdeliveryboy.getDmphotoListReport1(objdeliveryboy.Id, FDate, TDate);
            ViewBag.DMStatusList = dtDMStatusList;

            DataTable dtDm = new DataTable();
            dtDm = objdeliveryboy.getDm(null);
            ViewBag.Dm = dtDm;

            return View();
        }

        [HttpPost]
        public ActionResult DeliveryBoyCashReport(FormCollection form, DeliveryBoy objdeliveryboy)
        {
            string requirebalance = "0";
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            DateTime FDate; DateTime TDate;
            Subscription objsub = new Subscription();
            int UpdateOrder = 0;
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

            string Dmid = Request["ddlDm"];
            if (!string.IsNullOrEmpty(Dmid) && Dmid != "0")
            {
                objdeliveryboy.DeliveryBoyId = Convert.ToInt32(Dmid);
            }
            else
                objdeliveryboy.DeliveryBoyId = 0;
            ViewBag.Dm1 = Dmid;


            string submit = Request["submit"];
            if (submit == "search" || submit == "UpdateAll")
            {

                if (submit == "search")
                {
                    DataTable dtDMStatusList = new DataTable();
                    dtDMStatusList = objdeliveryboy.getDmCashListReport1(objdeliveryboy.DeliveryBoyId, FDate, TDate);
                    ViewBag.DMStatusList = dtDMStatusList;

                    DataTable dtDm = new DataTable();
                    dtDm = objdeliveryboy.getDm(null);
                    ViewBag.Dm = dtDm;
                }
                if (submit == "UpdateAll")
                {
                    string proid = Request["txtproid"];

                    string delimStr = ",";
                    char[] delimiter = delimStr.ToCharArray();

                    foreach (string s in proid.Split(delimiter))
                    {



                        objdeliveryboy.Id = Convert.ToInt32(s);

                        objdeliveryboy.Amount = Convert.ToDecimal(Request[Convert.ToInt32(s) + "CollectAmount"]);
                        string CollectAmount = objdeliveryboy.Amount.ToString();
                         UpdateOrder = objdeliveryboy.UpdateCashsingle(objdeliveryboy.Id, CollectAmount);

                        DataTable dtDm = new DataTable();
                        dtDm = objdeliveryboy.getCashcollectionwallet(objdeliveryboy.Id);
                        if (dtDm.Rows.Count > 0)
                        {
                            objsub.CustomerId = Convert.ToInt32(dtDm.Rows[0].ItemArray[1].ToString());
                            objsub.TransactionDate = Convert.ToDateTime(dtDm.Rows[0].ItemArray[5]);
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
                            objsub.CashCollectionId = objdeliveryboy.Id;

                            if (dtDm.Rows[0].ItemArray[8].ToString() == objdeliveryboy.Id.ToString())
                            {
                                //Wallet Update
                                int addwallet = objsub.UpdateCustomerWalletCashCollection(objsub);
                            }

                            else
                            {
                                //Wallet Update
                                int addwallet = objsub.InsertWalletCashCollection(objsub);
                            }
                        }

                    }

                    if (UpdateOrder > 0)
                    {
                        ViewBag.SuccessMsg = "Cash Collection Updated";
                    }
                    else
                        ViewBag.ErrorMsg = "Cash Collection Not Updated";
                }

            }
            else
            {
                objdeliveryboy.Id = Convert.ToInt32(submit);

                objdeliveryboy.Amount = Convert.ToDecimal(Request[submit + "CollectAmount"]);
                string CollectAmount = objdeliveryboy.Amount.ToString();
                 UpdateOrder = objdeliveryboy.UpdateCashsingle(objdeliveryboy.Id, CollectAmount);

                DataTable dtDm = new DataTable();
                dtDm = objdeliveryboy.getCashcollectionwallet(objdeliveryboy.Id);
                if (dtDm.Rows.Count > 0)
                {
                    objsub.CustomerId = Convert.ToInt32(dtDm.Rows[0].ItemArray[1].ToString());
                    objsub.TransactionDate = Convert.ToDateTime(dtDm.Rows[0].ItemArray[5]);
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
                    objsub.CashCollectionId = objdeliveryboy.Id;

                    if (dtDm.Rows[0].ItemArray[8].ToString() == objdeliveryboy.Id.ToString())
                    {
                        //Wallet Update
                        int addwallet = objsub.UpdateCustomerWalletCashCollection(objsub);
                    }

                    else
                    {
                        //Wallet Update
                        int addwallet = objsub.InsertWalletCashCollection(objsub);
                    }
                }

                if (UpdateOrder > 0)
                {
                    ViewBag.SuccessMsg = "Cash Collection Updated";
                }
                else
                    ViewBag.ErrorMsg = "Cash Collection Not Updated";

            }


            

            DataTable dtDMStatusList1 = new DataTable();
            dtDMStatusList1 = objdeliveryboy.getDmCashListReport1(objdeliveryboy.DeliveryBoyId, FDate, TDate);
            ViewBag.DMStatusList = dtDMStatusList1;

            DataTable dtDm1 = new DataTable();
            dtDm1 = objdeliveryboy.getDm(null);
            ViewBag.Dm = dtDm1;
            return View();
        }



        [HttpPost]
        public ActionResult DeliveryBoyDoc(FormCollection form, DeliveryBoy objdeliveryboy)
        {
            string requirebalance = "0";
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            DateTime FDate; DateTime TDate;


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

            string Dmid = Request["ddlDm"];
            if (!string.IsNullOrEmpty(Dmid) && Dmid != "0")
            {
                objdeliveryboy.Id = Convert.ToInt32(Dmid);
            }
            else
                objdeliveryboy.Id = 0;
            ViewBag.Dm1 = Dmid;
            DataTable dtDMStatusList = new DataTable();
            dtDMStatusList = objdeliveryboy.getDmDocListReport1(objdeliveryboy.Id, FDate, TDate);
            ViewBag.DMStatusList = dtDMStatusList;

            DataTable dtDm = new DataTable();
            dtDm = objdeliveryboy.getDm(null);
            ViewBag.Dm = dtDm;

            return View();
        }


        [HttpPost]
        public ActionResult DmDocView(FormCollection form, DeliveryBoy objdeliveryboy)
        {
            string requirebalance = "0";
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);



            int id = objdeliveryboy.Id;


            string Aadharstatus = Request["ddlaadhar"];

            string Panstatus = Request["ddlpan"];

            string Licensestatus = Request["ddllicense"];

            string Bankstatus = Request["ddlbank"];

            objdeliveryboy.Aadharstatus = Aadharstatus.ToString();
            objdeliveryboy.Panstatus = Panstatus.ToString();
            objdeliveryboy.Licensestatus = Licensestatus.ToString();
            objdeliveryboy.Bankstatus = Bankstatus.ToString();

            if(Aadharstatus== "Approved" && Panstatus== "Approved" && Licensestatus== "Approved" && Bankstatus== "Approved")
            {
                objdeliveryboy.Status = "Approved";
            }
            else
            {
                objdeliveryboy.Status = "Pending";
            }

            string description = objdeliveryboy.Description.ToString();

            //string Dmid = Request["ddlDm"];
            //if (!string.IsNullOrEmpty(Dmid) && Dmid != "0")
            //{
            //    objdeliveryboy.Id = Convert.ToInt32(Dmid);
            //}
            //else
            //    objdeliveryboy.Id = 0;
            //ViewBag.Dm1 = Dmid;
            //DataTable dtDMStatusList = new DataTable();
            //dtDMStatusList = objdeliveryboy.getDmDocListReport1(objdeliveryboy.Id, FDate, TDate);
            //ViewBag.DMStatusList = dtDMStatusList;

            //DataTable dtDm = new DataTable();
            //dtDm = objdeliveryboy.getDm(null);
            //ViewBag.Dm = dtDm;

          int  addresult = objdeliveryboy.UpdateDmDocstatus(objdeliveryboy);
            if (addresult > 0)
            {
                ViewBag.SuccessMsg = "Document Status Updated Successfully!!!";
            }
            else
            { ViewBag.SuccessMsg = "Document Status Not Updated!!!"; }

            return View();
        }

        [HttpPost]
        public ActionResult DailyBillPayReport(FormCollection form, CashBack objcashback)
        {
            string requirebalance = "0";
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var submit = Request["submit"];

            DateTime FDate; DateTime TDate;
            

            if (submit == "WeekBillPayment")
            {
                string Service = Request["ddlservice"];
                if (!string.IsNullOrEmpty(Service) && Service != "---Select Service---")
                {
                    objcashback.Service = Service;
                }

                ViewBag.Servicename = Service;
                FDate = DateTime.Today.AddDays(-7);
                 TDate = DateTime.Today;
                
                DataTable dtCashbackbillList = new DataTable();
                dtCashbackbillList = objcashback.getCashbackBillListReport(objcashback.Service, FDate, TDate);
                ViewBag.CashbackBillList = dtCashbackbillList;



                CashBack objService = new CashBack();
                DataTable dtService = new DataTable();
                dtService = objService.getService(null);
                ViewBag.Service = dtService;

            }




            if (submit == "TodaysBillPayment")
            {
                string Service = Request["ddlservice"];
                if (!string.IsNullOrEmpty(Service) && Service != "---Select Service---")
                {
                    objcashback.Service = Service;
                }

                ViewBag.Servicename = Service;
                FDate = DateTime.Today;
                TDate = DateTime.Today;

                DataTable dtCashbackbillList = new DataTable();
                dtCashbackbillList = objcashback.getCashbackBillListReport(objcashback.Service, FDate, TDate);
                ViewBag.CashbackBillList = dtCashbackbillList;



                CashBack objService = new CashBack();
                DataTable dtService = new DataTable();
                dtService = objService.getService(null);
                ViewBag.Service = dtService;

            }



            if (submit == "YesterdayBillPayment")
            {
                string Service = Request["ddlservice"];
                if (!string.IsNullOrEmpty(Service) && Service != "---Select Service---")
                {
                    objcashback.Service = Service;
                }

                ViewBag.Servicename = Service;
                FDate = DateTime.Today.AddDays(-1);
                TDate = DateTime.Today.AddDays(-1);

                DataTable dtCashbackbillList = new DataTable();
                dtCashbackbillList = objcashback.getCashbackBillListReport(objcashback.Service, FDate, TDate);
                ViewBag.CashbackBillList = dtCashbackbillList;



                CashBack objService = new CashBack();
                DataTable dtService = new DataTable();
                dtService = objService.getService(null);
                ViewBag.Service = dtService;

            }


            if (submit == "submit")
            {
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
                dtCashbackbillList = objcashback.getCashbackBillListReport(objcashback.Service, FDate, TDate);
                ViewBag.CashbackBillList = dtCashbackbillList;



                CashBack objService = new CashBack();
                DataTable dtService = new DataTable();
                dtService = objService.getService(null);
                ViewBag.Service = dtService;

            }


            string Status = ""; string jsonString = null; string str1 = null; string str2 = null; string ac = null;
            //

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


            ViewBag.Balance = balance.ToString();
            //
            double requirebalancenew = 0;

            requirebalance = getrequirebalance();
            if (balance <= Convert.ToDouble(requirebalance))
            {
                requirebalancenew = Convert.ToDouble(requirebalance) - balance;
            }
            if (balance > Convert.ToDouble(requirebalance))
            {
                requirebalancenew = 0;
            }

            if (Convert.ToDouble(requirebalancenew) > 0)
            {
                ViewBag.requirebalance = requirebalancenew.ToString();
            }
            else
                ViewBag.requirebalance = "0";
            return View();
        }






        public string getrequirebalance()
        {
            string requirebalance = "0";
            string rmobilebalance = "0";
            string rgasbalance = "0";
            double totrequirebalance = 0;
            MobileRecharge objService = new MobileRecharge();

            DataTable dtbalance1 = new DataTable();
            dtbalance1 = objService.getmobilebalance();
            if(dtbalance1.Rows.Count>0)
            {
                rmobilebalance = dtbalance1.Rows[0].ItemArray[0].ToString();
            }

            dtbalance1 = objService.getgasbalance();
            if (dtbalance1.Rows.Count > 0)
            {
                rgasbalance = dtbalance1.Rows[0].ItemArray[0].ToString();
            }
            if(rmobilebalance.ToString()=="" || rmobilebalance==null)
            {
                rmobilebalance = "0";
            }

            if (rgasbalance.ToString() == "" || rgasbalance == null)
            {
                rgasbalance = "0";
            }
            totrequirebalance = Convert.ToDouble(rmobilebalance) + Convert.ToDouble(rgasbalance);
            requirebalance = totrequirebalance.ToString();
            return requirebalance;

        }



        [HttpGet]
        public ActionResult DeleteOrderReport()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            Customer objcus = new Customer();
            DateTime FDate; DateTime TDate;

            FDate = DateTime.Today;
            TDate = DateTime.Today;

            DataTable dtDMStatusList = new DataTable();
            dtDMStatusList = objcus.DailyDeleteOrder(null,null);
            ViewBag.DMStatusList = dtDMStatusList;

            //DataTable dtDm = new DataTable();
            //dtDm = objdeliveryboy.getDm(null);
            //ViewBag.Dm = dtDm;

            return View();
        }


        [HttpPost]
        public ActionResult DeleteOrderReport(FormCollection form, Customer objcus)
        {

            DateTime FDate; DateTime TDate;
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
            DataTable dtDMStatusList = new DataTable();
            dtDMStatusList = objcus.DailyDeleteOrder(FDate, TDate);
            ViewBag.DMStatusList = dtDMStatusList;
           
            return View();
        }


        //Vendor

        [HttpGet]
        public ActionResult VendorProductPhoto()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            Vendornew objvendor = new Vendornew();
            DateTime FDate; DateTime TDate;

            FDate = DateTime.Today;
            TDate = DateTime.Today;

            DataTable dtDMStatusList = new DataTable();
            dtDMStatusList = objvendor.getVendorphotoListReport(FDate, TDate);
            ViewBag.VendorStatusList = dtDMStatusList;
            Vendor obj = new Vendor();
            DataTable dtDm = new DataTable();
            dtDm = obj.getVendorList(null);
            ViewBag.Vendor = dtDm;

            return View();
        }


        [HttpPost]
        public ActionResult VendorProductPhoto(FormCollection form, Vendornew objvendor)
        {
            string requirebalance = "0";
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            DateTime FDate; DateTime TDate;


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

            string VendorId = Request["ddlVendor"];
            if (!string.IsNullOrEmpty(VendorId) && VendorId != "0")
            {
                objvendor.VendorId = Convert.ToInt32(VendorId);
            }
            else
                objvendor.VendorId = 0;
            ViewBag.VendorName = VendorId;
            DataTable dtDMStatusList = new DataTable();
            dtDMStatusList = objvendor.getVendorphotoListReport1(objvendor.VendorId, FDate, TDate);
            ViewBag.VendorStatusList = dtDMStatusList;

            Vendor obj = new Vendor();
            DataTable dtDm = new DataTable();
            dtDm = obj.getVendorList(null);
            ViewBag.Vendor = dtDm;

            return View();
        }
        [HttpGet]
        public ActionResult VendorDoc()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            Vendornew objvendor = new Vendornew();
            DateTime FDate; DateTime TDate;

            FDate = DateTime.Today;
            TDate = DateTime.Today;

            DataTable dtDMStatusList = new DataTable();
            dtDMStatusList = objvendor.getVendorDocListReport(FDate, TDate);
            ViewBag.VendorStatusList = dtDMStatusList;

            Vendor obj = new Vendor();
            DataTable dtDm = new DataTable();
            dtDm = obj.getVendorList(null);
            ViewBag.Vendor = dtDm;

            return View();
        }


        [HttpPost]
        public ActionResult VendorDoc(FormCollection form, Vendornew objvendor)
        {
            string requirebalance = "0";
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            DateTime FDate; DateTime TDate;


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

            string vendorid = Request["ddlVendor"];
            if (!string.IsNullOrEmpty(vendorid) && vendorid != "0")
            {
                objvendor.VendorId = Convert.ToInt32(vendorid);
            }
            else
                objvendor.VendorId = 0;
            ViewBag.Vendorname = vendorid;
            DataTable dtDMStatusList = new DataTable();
            dtDMStatusList = objvendor.getVendorDocListReport1(objvendor.VendorId, FDate, TDate);
            ViewBag.VendorStatusList = dtDMStatusList;

            Vendor obj = new Vendor();
            DataTable dtDm = new DataTable();
            dtDm = obj.getVendorList(null);
            ViewBag.Vendor = dtDm;

            return View();
        }


        [HttpGet]
        public ActionResult VendorDocView(int id = 0)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            Vendornew objvendor = new Vendornew();

            DataTable dtDMStatusList = new DataTable();
            dtDMStatusList = objvendor.getVendorDocReport(id);
            //ViewBag.DMStatusList = dtDMStatusList;

            //DataTable dtDm = new DataTable();
            //dtDm = objdeliveryboy.getDm(null);
            //ViewBag.Dm = dtDm;
            var model = new Vendornew();
            if (dtDMStatusList.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["BankAccount"].ToString()))
                    ViewBag.BankAccount = dtDMStatusList.Rows[0]["BankAccount"].ToString();
                else
                    ViewBag.BankAccount = "";

                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Ifsc"].ToString()))
                    ViewBag.Ifsc = dtDMStatusList.Rows[0]["Ifsc"].ToString();
                else
                    ViewBag.Ifsc = "";

                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Bankname"].ToString()))
                    ViewBag.Bankname = dtDMStatusList.Rows[0]["Bankname"].ToString();
                else
                    ViewBag.Bankname = "";

                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["AccholderName"].ToString()))
                    ViewBag.AccholderName = dtDMStatusList.Rows[0]["AccholderName"].ToString();
                else
                    ViewBag.AccholderName = "";


                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Status"].ToString()))
                    ViewBag.Status = dtDMStatusList.Rows[0]["Status"].ToString();
                else
                    ViewBag.Status = "";


                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Aadharstatus"].ToString()))
                    ViewBag.Aadharstatus = dtDMStatusList.Rows[0]["Aadharstatus"].ToString();
                else
                    ViewBag.Aadharstatus = "";

                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Panstatus"].ToString()))
                    ViewBag.Panstatus = dtDMStatusList.Rows[0]["Panstatus"].ToString();
                else
                    ViewBag.Panstatus = "";

                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Licensestatus"].ToString()))
                    ViewBag.Licensestatus = dtDMStatusList.Rows[0]["Licensestatus"].ToString();
                else
                    ViewBag.Licensestatus = "";

                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Bankstatus"].ToString()))
                    ViewBag.Bankstatus = dtDMStatusList.Rows[0]["Bankstatus"].ToString();
                else
                    ViewBag.Bankstatus = "";



                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Aadhar"].ToString()))
                    ViewBag.Aadhar = dtDMStatusList.Rows[0]["Aadhar"].ToString();
                else
                    ViewBag.Aadhar = "N/A";

                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Pan"].ToString()))
                    ViewBag.Pan = dtDMStatusList.Rows[0]["Pan"].ToString();
                else
                    ViewBag.Pan = "N/A";

                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["License"].ToString()))
                    ViewBag.License = dtDMStatusList.Rows[0]["License"].ToString();
                else
                    ViewBag.License = "N/A";
                if (!string.IsNullOrEmpty(dtDMStatusList.Rows[0]["Decsiption"].ToString()))
                    ViewBag.Decsiption = dtDMStatusList.Rows[0]["Decsiption"].ToString();
                else
                    ViewBag.Decsiption = "N/A";

                model.Description = ViewBag.Decsiption;
            }



            return View(model);
        }



        [HttpPost]
        public ActionResult VendorDocView(FormCollection form, Vendornew objvendor)
        {
            string requirebalance = "0";
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);



            int id = objvendor.Id;


            string Aadharstatus = Request["ddlaadhar"];

            string Panstatus = Request["ddlpan"];

            string Licensestatus = Request["ddllicense"];

            string Bankstatus = Request["ddlbank"];

            objvendor.Aadharstatus = Aadharstatus.ToString();
            objvendor.Panstatus = Panstatus.ToString();
            objvendor.Licensestatus = Licensestatus.ToString();
            objvendor.Bankstatus = Bankstatus.ToString();

            if (Aadharstatus == "Approved" && Panstatus == "Approved" && Licensestatus == "Approved" && Bankstatus == "Approved")
            {
                objvendor.Status = "Approved";
            }
            else
            {
                objvendor.Status = "Pending";
            }

            string description = objvendor.Description.ToString();



            int addresult = objvendor.UpdateVendorDocstatus(objvendor);
            if (addresult > 0)
            {
                ViewBag.SuccessMsg = "Document Status Updated Successfully!!!";
            }
            else
            { ViewBag.SuccessMsg = "Document Status Not Updated!!!"; }

            return View();
        }


        [HttpGet]
        public ActionResult DeliveryBoyReplacement()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            DeliveryBoy objdeliveryboy = new DeliveryBoy();
            DateTime FDate; DateTime TDate;

            FDate = DateTime.Today;
            TDate = DateTime.Today;

            DataTable dtDMStatusList = new DataTable();
            dtDMStatusList = objdeliveryboy.getDmphotoListReport(FDate, TDate);
            ViewBag.DMStatusList = dtDMStatusList;

            DataTable dtDm = new DataTable();
            dtDm = objdeliveryboy.getDm(null);
            ViewBag.Dm = dtDm;

            return View();
        }



        public JsonResult GetCustomerByDm(int? id)
        {
            DeliveryBoy objdeliveryboy = new DeliveryBoy();
            //ArrayList list = new ArrayList();

            DataTable dtnew = objdeliveryboy.getDmwisecustomer(id);
            int rc = dtnew.Rows.Count;//for test            

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dtnew.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dtnew.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            // return serializer.Serialize(rows);
            return Json(serializer.Serialize(rows), JsonRequestBehavior.AllowGet);
        }


        public JsonResult UpdateCategory(FormCollection frm, string[] chk)
        {

            string status = frm["txtstatus"];
            DeliveryBoy objdeliveryboy = new DeliveryBoy();
            try
            {
                if (status == "1")
                {
                    foreach (var item in chk)
                    {
                        objdeliveryboy.UpdateDMCustomernorderby(item, frm["ddlnewDm"]);
                    }
                }
                if (status == "0")
                {
                    foreach (var item in chk)
                    {
                        objdeliveryboy.UpdateDMCustomerorderby(item, frm["ddlnewDm"]);
                    }
                }
                res["success"] = "1";
            }
            catch
            {

            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }




        [HttpGet]
        public ActionResult DeliveryBoyOrderQtyUpdateReport()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            DeliveryBoy objdeliveryboy = new DeliveryBoy();
            DateTime FDate; DateTime TDate;

            FDate = DateTime.Today;
            TDate = DateTime.Today;

            DataTable dtDMStatusList = new DataTable();
            dtDMStatusList = objdeliveryboy.getDmOrderqtyUpdateListReport(null, FDate, TDate);
            ViewBag.DMStatusList = dtDMStatusList;

            DataTable dtDm = new DataTable();
            dtDm = objdeliveryboy.getDm(null);
            ViewBag.Dm = dtDm;

            return View();
        }



        [HttpPost]
        public ActionResult DeliveryBoyOrderQtyUpdateReport(FormCollection form, DeliveryBoy objdeliveryboy)
        {
            string requirebalance = "0";
            if (HttpContext.Session["UserId"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            DateTime FDate;
            DateTime TDate;
            Subscription objsub = new Subscription();
            int UpdateOrder = 0;
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

            string Dmid = Request["ddlDm"];
            if (!string.IsNullOrEmpty(Dmid) && Dmid != "0")
            {
                objdeliveryboy.DeliveryBoyId = Convert.ToInt32(Dmid);
            }
            else
                objdeliveryboy.DeliveryBoyId = 0;
            ViewBag.Dm1 = Dmid;


            string submit = Request["submit"];
            if (submit == "search" || submit == "UpdateAll")
            {

                if (submit == "search")
                {

                    DataTable dtDMStatusList = new DataTable();
                    dtDMStatusList = objdeliveryboy.getDmOrderqtyUpdateListReport(objdeliveryboy.DeliveryBoyId, FDate, TDate);
                    ViewBag.DMStatusList = dtDMStatusList;

                }


                if (submit == "UpdateAll")
                {
                    string proid = Request["txtproid"];

                    string delimStr = ",";
                    char[] delimiter = delimStr.ToCharArray();

                    foreach (string s in proid.Split(delimiter))
                    {

                        Product objProduct = new Product();
                        objdeliveryboy.Id = Convert.ToInt32(s);
                        int newQty = Convert.ToInt32(Request[objdeliveryboy.Id + "newqty"]);
                        //get data from tbl_Dm_ProductUpdate
                        DataTable dt = new DataTable();
                        dt = objdeliveryboy.getDmOrderqtyUpdate(objdeliveryboy.Id);

                        int OrderId = Convert.ToInt32(dt.Rows[0]["OrderId"]);
                        int Qty = Convert.ToInt32(dt.Rows[0]["Qty"]);
                        string Proname = dt.Rows[0]["ProductName"].ToString();
                        string OrderStatus = dt.Rows[0]["Orderstatus"].ToString();
                        // Calculaion part Start
                        DataTable dtedit = new DataTable();
                        dtedit = objsub.getCustomerOrderSelect(OrderId.ToString());
                        objsub.OrderId = Convert.ToInt32(OrderId);
                        objsub.CustomerId = Convert.ToInt32(dt.Rows[0]["CustomerId"]);
                        objsub.Qty = Convert.ToInt32(newQty);






                        objsub.Discount = Convert.ToDecimal(dtedit.Rows[0]["Discount"].ToString());
                        objsub.RewardPoint = Convert.ToInt64(dtedit.Rows[0]["RewardPoint"]);
                        objsub.CGSTAmount = Convert.ToDecimal(dtedit.Rows[0]["CGSTAmount"].ToString());
                        objsub.SGSTAmount = Convert.ToDecimal(dtedit.Rows[0]["SGSTAmount"].ToString());
                        objsub.IGSTAmount = Convert.ToDecimal(dtedit.Rows[0]["IGSTAmount"].ToString());
                        objsub.Profit = Convert.ToDecimal(dtedit.Rows[0]["Profit"].ToString());
                        objsub.OrderDate = Convert.ToDateTime(dtedit.Rows[0]["OrderDate"].ToString());


                        DataTable dtProduct = objProduct.BindProuct(Convert.ToInt32(dt.Rows[0]["Proid"]));
                        if (dtProduct.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SalePrice"].ToString()))
                                objsub.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
                            else
                                objsub.Amount = 0;


                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Price"].ToString()))
                            {
                                objsub.MRPPrice = Convert.ToDecimal(dtProduct.Rows[0]["Price"]) * Convert.ToDecimal(newQty);

                            }
                            else
                            {
                                objsub.MRPPrice = 0;

                            }

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["PurchasePrice"].ToString()))
                            {
                                objsub.PurchasePrice = Convert.ToDecimal(dtProduct.Rows[0]["PurchasePrice"]) * Convert.ToDecimal(newQty);

                            }
                            else
                            {
                                objsub.PurchasePrice = 0;

                            }
                        }
                        objsub.Amount = ((objsub.Amount)) * Convert.ToDecimal(newQty);
                        objsub.Discount = ((objsub.Discount) / Convert.ToDecimal(Qty)) * Convert.ToDecimal(newQty);
                        objsub.RewardPoint = (Convert.ToInt64(objsub.RewardPoint) / Convert.ToInt64(Qty)) * Convert.ToInt64(newQty);
                        objsub.CGSTAmount = ((objsub.CGSTAmount) / Convert.ToDecimal(Qty)) * Convert.ToDecimal(newQty);
                        objsub.SGSTAmount = ((objsub.SGSTAmount) / Convert.ToDecimal(Qty)) * Convert.ToDecimal(newQty);
                        objsub.IGSTAmount = ((objsub.IGSTAmount) / Convert.ToDecimal(Qty)) * Convert.ToDecimal(newQty);
                        objsub.Profit = ((objsub.Profit) / Convert.ToDecimal(Qty)) * Convert.ToDecimal(newQty);

                        objsub.TotalFinalAmount = objsub.Amount - objsub.Discount;
                        //Calculation End





                        string jsonString = null;

                        objsub.Id = Convert.ToInt32(OrderId);
                        objsub.Status = OrderStatus;
                        UpdateOrder = objsub.UpdateCustomerOrderMain(objsub);
                        //update item order

                        int UpdateAddProductDetail = objsub.UpdateCustomerOrderDetailMobile(objsub);
                        int j = 0;
                        if (Qty > 0)
                        {
                            j = 0;

                            if (OrderStatus == "Complete")
                            {
                                var i = objsub.CheckCustomerWalletEntry(objsub.OrderId, objsub.CustomerId);
                                if (i == 0)
                                {
                                    objsub.Description = "Place Order";
                                    objsub.Type = "Debit";
                                    objsub.proqty = newQty.ToString();
                                    objsub.Status = "Purchase";
                                    objsub.CustSubscriptionId = 0;
                                    objsub.TransactionType = Convert.ToInt32(Helper.TransactionType.Purchase);
                                    objsub.Description = Proname;
                                    j = objsub.InsertWalletScheduleOrder(objsub);



                                }
                                else
                                {
                                    j = objsub.UpdateCustomerWallet1(objsub);
                                }
                                if (j > 0)
                                {
                                    ViewBag.SuccessMsg = "Record Approved Successfully";
                                }
                                else
                                {
                                    ViewBag.ErrorMsg = "Error Occured ! Record Not Approved";

                                }

                            }
                        }
                        else
                        {
                            int i = objsub.CheckCustomerWalletEntry(objsub.OrderId, objsub.CustomerId);
                            j = objsub.DeleteCustomerWallet(i);

                            if (i > 0)
                            {
                                if (j > 0)
                                {
                                    ViewBag.SuccessMsg = "Record Approved Successfully";
                                }
                                else
                                {
                                    ViewBag.ErrorMsg = "Error Occured ! Record Not Approved";


                                }
                            }


                            else
                            {
                                ViewBag.SuccessMsg = "Record Approved Successfully";
                            }

                        }


                        int addresult = objdeliveryboy.UpdateQtyUpdatestatus(objdeliveryboy.Id, newQty);
                    }

                }

            }
            else
            {
                Product objProduct = new Product();
                objdeliveryboy.Id = Convert.ToInt32(submit);
                int newQty = Convert.ToInt32(Request[submit + "newqty"]);
                //get data from tbl_Dm_ProductUpdate
                DataTable dt = new DataTable();
                dt = objdeliveryboy.getDmOrderqtyUpdate(objdeliveryboy.Id);

                int OrderId = Convert.ToInt32(dt.Rows[0]["OrderId"]);
                int Qty = Convert.ToInt32(dt.Rows[0]["Qty"]);
                string Proname = dt.Rows[0]["ProductName"].ToString();
                string OrderStatus = dt.Rows[0]["Orderstatus"].ToString();
                // Calculaion part Start
                DataTable dtedit = new DataTable();
                dtedit = objsub.getCustomerOrderSelect(OrderId.ToString());
                objsub.OrderId = Convert.ToInt32(OrderId);
                objsub.CustomerId = Convert.ToInt32(dt.Rows[0]["CustomerId"]);
                objsub.Qty = Convert.ToInt32(newQty);






                objsub.Discount = Convert.ToDecimal(dtedit.Rows[0]["Discount"].ToString());
                objsub.RewardPoint = Convert.ToInt64(dtedit.Rows[0]["RewardPoint"]);
                objsub.CGSTAmount = Convert.ToDecimal(dtedit.Rows[0]["CGSTAmount"].ToString());
                objsub.SGSTAmount = Convert.ToDecimal(dtedit.Rows[0]["SGSTAmount"].ToString());
                objsub.IGSTAmount = Convert.ToDecimal(dtedit.Rows[0]["IGSTAmount"].ToString());
                objsub.Profit = Convert.ToDecimal(dtedit.Rows[0]["Profit"].ToString());
                objsub.OrderDate = Convert.ToDateTime(dtedit.Rows[0]["OrderDate"].ToString());


                DataTable dtProduct = objProduct.BindProuct(Convert.ToInt32(dt.Rows[0]["Proid"]));
                if (dtProduct.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SalePrice"].ToString()))
                        objsub.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
                    else
                        objsub.Amount = 0;


                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Price"].ToString()))
                    {
                        objsub.MRPPrice = Convert.ToDecimal(dtProduct.Rows[0]["Price"]) * Convert.ToDecimal(newQty);

                    }
                    else
                    {
                        objsub.MRPPrice = 0;

                    }

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["PurchasePrice"].ToString()))
                    {
                        objsub.PurchasePrice = Convert.ToDecimal(dtProduct.Rows[0]["PurchasePrice"]) * Convert.ToDecimal(newQty);

                    }
                    else
                    {
                        objsub.PurchasePrice = 0;

                    }
                }
                objsub.Amount = ((objsub.Amount)) * Convert.ToDecimal(newQty);
                objsub.Discount = ((objsub.Discount) / Convert.ToDecimal(Qty)) * Convert.ToDecimal(newQty);
                objsub.RewardPoint = (Convert.ToInt64(objsub.RewardPoint) / Convert.ToInt64(Qty)) * Convert.ToInt64(newQty);
                objsub.CGSTAmount = ((objsub.CGSTAmount) / Convert.ToDecimal(Qty)) * Convert.ToDecimal(newQty);
                objsub.SGSTAmount = ((objsub.SGSTAmount) / Convert.ToDecimal(Qty)) * Convert.ToDecimal(newQty);
                objsub.IGSTAmount = ((objsub.IGSTAmount) / Convert.ToDecimal(Qty)) * Convert.ToDecimal(newQty);
                objsub.Profit = ((objsub.Profit) / Convert.ToDecimal(Qty)) * Convert.ToDecimal(newQty);

                objsub.TotalFinalAmount = objsub.Amount - objsub.Discount;
                //Calculation End





                string jsonString = null;

                objsub.Id = Convert.ToInt32(OrderId);
                objsub.Status = OrderStatus;
                UpdateOrder = objsub.UpdateCustomerOrderMain(objsub);
                //update item order

                int UpdateAddProductDetail = objsub.UpdateCustomerOrderDetailMobile(objsub);
                int j = 0;
                if (Qty > 0)
                {
                    j = 0;

                    if (OrderStatus == "Complete")
                    {
                        var i = objsub.CheckCustomerWalletEntry(objsub.OrderId, objsub.CustomerId);
                        if (i == 0)
                        {
                            objsub.Description = "Place Order";
                            objsub.Type = "Debit";
                            objsub.proqty = newQty.ToString();
                            objsub.Status = "Purchase";
                            objsub.CustSubscriptionId = 0;
                            objsub.TransactionType = Convert.ToInt32(Helper.TransactionType.Purchase);
                            objsub.Description = Proname;
                            j = objsub.InsertWalletScheduleOrder(objsub);



                        }
                        else
                        {
                            j = objsub.UpdateCustomerWallet1(objsub);
                        }
                        if (j > 0)
                        {
                            ViewBag.SuccessMsg = "Record Approved Successfully";
                        }
                        else
                        {
                            ViewBag.ErrorMsg = "Error Occured ! Record Not Approved";

                        }

                    }
                }
                else
                {
                    int i = objsub.CheckCustomerWalletEntry(objsub.OrderId, objsub.CustomerId);
                    j = objsub.DeleteCustomerWallet(i);

                    if (i > 0)
                    {
                        if (j > 0)
                        {
                            ViewBag.SuccessMsg = "Record Approved Successfully";
                        }
                        else
                        {
                            ViewBag.ErrorMsg = "Error Occured ! Record Not Approved";


                        }
                    }


                    else
                    {
                        ViewBag.SuccessMsg = "Record Approved Successfully";
                    }

                }


                int addresult = objdeliveryboy.UpdateQtyUpdatestatus(objdeliveryboy.Id, newQty);

            }




            DataTable dtDm = new DataTable();
            dtDm = objdeliveryboy.getDm(null);
            ViewBag.Dm = dtDm;

            if (!string.IsNullOrEmpty(fdate.ToString()) && !string.IsNullOrEmpty(tdate.ToString()) && !string.IsNullOrEmpty(Dmid) && Dmid != "0")
            {
                DataTable dtDMStatusList = new DataTable();
                dtDMStatusList = objdeliveryboy.getDmOrderqtyUpdateListReport(objdeliveryboy.DeliveryBoyId, FDate, TDate);
                ViewBag.DMStatusList = dtDMStatusList;
            }

            else
            {


                DataTable dtDMStatusList = new DataTable();
                dtDMStatusList = objdeliveryboy.getDmOrderqtyUpdateListReport(null, FDate, TDate);
                ViewBag.DMStatusList = dtDMStatusList;
            }




            return View();
        }

    }
}
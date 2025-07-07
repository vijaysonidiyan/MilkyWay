using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MilkWayIndia.Models;
using System.Data;
using MilkWayIndia.Concrete;

namespace MilkWayIndia.Controllers.Reports
{
    public class ReportController : Controller
    {
        Customer objcust = new Customer();
        Vendor objvendor = new Vendor();
        EFDbContext db = new EFDbContext();
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DeliveryBoyDailyReport(int? DeliveryboyId, int? CustomerId, string FDate, string TDate, string status)
        {
            if (Request.Cookies["gstusr"] == null)
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
            var dtList = order.getDeliveryBoyWiseOrder(DeliveryboyId, CustomerId, _fdate, _tdate, status);
            ViewBag.ProductorderList = dtList;
            return View();
        }
        public ActionResult DeliveryBoyDailyReportvendor1(int? DeliveryboyId, int? CustomerId, string FDate, string TDate, string status)
        {
            if (Request.Cookies["gstusr"] == null)
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

            ViewBag.sectorid = dtList.Rows[0].ItemArray[12];
            string a = "";
            string sid = "";
            if (dtList.Rows.Count>0)
            {

                DataTable dt1 = new DataTable();
                DataRow dr = null;
                DataTable dt = new DataTable();
                //List<SectorViewModel> list = new List<SectorViewModel>();
                for (int i=0;i<dtList.Rows.Count;i++)
                {
                   
                    sid = dtList.Rows[i].ItemArray[12].ToString();
                    
                    order = new CustomerOrder();
                    dt = order.GetMultiSectorVendorOrder1(sid,_fdate,_tdate, DeliveryboyId);
                   // dt1.Merge(dt);
                   // dt1.AcceptChanges();
                   // dt.Clear();
                    
                }

                ViewBag.ProductorderList1 = dt;
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
        public ActionResult VendorProductOrderReportStatus(int? SectorId, int? VendorId, string FDate, string TDate, string Status)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            DateTime _fdate = DateTime.Now, _tdate = DateTime.Now;
            if (!string.IsNullOrEmpty(FDate.ToString()))
                _fdate = Convert.ToDateTime(DateTime.ParseExact(FDate, @"dd-MM-yyyy", null));
            if (!string.IsNullOrEmpty(TDate.ToString()))
                _tdate = Convert.ToDateTime(DateTime.ParseExact(TDate, @"dd-MM-yyyy", null));

            CustomerOrder order = new CustomerOrder();
            ViewBag.FromDate = FDate;
            ViewBag.ToDate = TDate;
            var dtList = order.getSectorVendorWiseOrderStatus(SectorId, VendorId, _fdate, _tdate, Status);
            
          
            ViewBag.ProductorderList = dtList;
            return View();
        }

        public ActionResult ReferralReport()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            PopulateDropdown();
            return View();
        }

        public void PopulateDropdown()
        {
            var customer = objcust.BindCustomer(null);
            ViewBag.lstCustomer = customer;

            ViewBag.Vendor = objvendor.getVendorList(null);
        }

        [HttpPost]
        public ActionResult ReferralReport(FormCollection form)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            PopulateDropdown();
            CustomerOrder _customer = new CustomerOrder();
            string customerId = Request["ddlCustomer"];
            if (!string.IsNullOrEmpty(customerId) && Convert.ToInt32(customerId) != 0)
                _customer.CustomerId = Convert.ToInt32(customerId);

            var fdate = Request["datepicker"];
            if (!string.IsNullOrEmpty(fdate.ToString()))
                _customer.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd-MM-yyyy", null));

            var tdate = Request["datepicker1"];
            if (!string.IsNullOrEmpty(tdate.ToString()))
                _customer.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd-MM-yyyy", null));

            var _fdate = _customer.FromDate.Value.ToString("dd-MM-yyyy");
            var _tdate = _customer.ToDate.Value.ToString("dd-MM-yyyy");
            ViewBag.CustomerId = _customer.CustomerId;
            ViewBag.FromDate = _fdate;
            ViewBag.ToDate = _tdate;
            DataTable dtList = new DataTable();
            dtList = _customer.GetCustomerReferralList(_customer.CustomerId, _customer.FromDate, _customer.ToDate);
            ViewBag.lstCustomerReferral = dtList;
            return View();
        }

        public ActionResult MultiVendorProductOrder()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            return View();
        }

        public PartialViewResult FetchSectorList(int id)
        {
            List<SectorViewModel> list = new List<SectorViewModel>();
            var sector = objvendor.GetSectorListByVendor(id);
            if (sector.Rows.Count > 0)
            {
                for (int i = 0; i < sector.Rows.Count; i++)
                {
                    list.Add(new SectorViewModel { ID = sector.Rows[i]["Id"].ToString(), Name = sector.Rows[i]["SectorName"].ToString() });
                }
            }
            return PartialView("_ChkSectorList", list);
        }

        public PartialViewResult FetchVendorList(int id)
        {
            List<SectorViewModel> list = new List<SectorViewModel>();
            var sector = objvendor.GetVendorListByPraentCat(id);
            if (sector.Rows.Count > 0)
            {
                for (int i = 0; i < sector.Rows.Count; i++)
                {
                    list.Add(new SectorViewModel { ID = sector.Rows[i]["Id"].ToString(), Name = sector.Rows[i]["Vname"].ToString()+"/"+ sector.Rows[i]["VendorCatname"].ToString() });
                }
            }
            return PartialView("_ChkVendorList", list);
        }

        public PartialViewResult FetchVendorListAttribute(string id)
        {
            List<SectorViewModel> list = new List<SectorViewModel>();


            var sector = objvendor.GetVendorListByAttribute(id);
            if (sector.Rows.Count > 0)
            {
                for (int i = 0; i < sector.Rows.Count; i++)
                {
                    list.Add(new SectorViewModel { ID = sector.Rows[i]["Id"].ToString(), Name = sector.Rows[i]["Vendorname"].ToString() + "/" + sector.Rows[i]["VendorCatname"].ToString() });
                }
            }
            return PartialView("_ChkVendorList1", list);
        }

        public ActionResult MultiSectorProductOrder()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            PopulateDropdown();
            return View();
        }

        [HttpPost]
        public ActionResult MultiSectorProductOrder(FormCollection frm, string[] chkSector)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            PopulateDropdown();

            var vendorId = frm["ddlVendor"];
            CustomerOrder _order = new CustomerOrder();
            var fdate = frm["datepicker"];
            if (!string.IsNullOrEmpty(fdate.ToString()))
                _order.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));

            var tdate = frm["datepicker1"];
            if (!string.IsNullOrEmpty(tdate.ToString()))
                _order.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));

            var sectorId = "";
            foreach (var item in chkSector)
            {
                sectorId += item + ",";
            }
            if (sectorId.Length > 0)
                sectorId.Substring(0, sectorId.Length - 1);

            var _fdate = _order.FromDate.Value.ToString("dd-MM-yyyy");
            var _tdate = _order.ToDate.Value.ToString("dd-MM-yyyy");
            string query = string.Format("SectorId={0}&VendorId={1}&FDate={2}&TDate={3}",
                 sectorId, vendorId, _fdate, _tdate);
            return Redirect("/Report/VendorProductOrderReport?" + query);
        }

        public ActionResult VendorProductOrderReport(string SectorId, int? VendorId, string FDate, string TDate)
        {
            DateTime _fdate = DateTime.Now, _tdate = DateTime.Now;
            if (!string.IsNullOrEmpty(FDate.ToString()))
                _fdate = Convert.ToDateTime(DateTime.ParseExact(FDate, @"dd-MM-yyyy", null));
            if (!string.IsNullOrEmpty(TDate.ToString()))
                _tdate = Convert.ToDateTime(DateTime.ParseExact(TDate, @"dd-MM-yyyy", null));

            CustomerOrder order = new CustomerOrder();
            ViewBag.FromDate = _fdate.ToString("dd-MMM-yyyy");
            ViewBag.ToDate = _tdate.ToString("dd-MMM-yyyy");
            var dtList = order.GetMultiSectorVendorOrder(SectorId, VendorId, _fdate, _tdate);
            ViewBag.ProductorderList = dtList;
            //https://stackoverflow.com/questions/16872056/how-to-pass-string-parameter-with-in-operator-in-stored-procedure-sql-server-2/16872537
            return View();
        }

        public ActionResult PaytmAuthorized()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var report = (from p in db.tbl_Paytm_Request
                          join c in db.tbl_Customer_Master on p.CustomerID equals c.ID
                          select new PaytmAuthroziedVM
                          {
                              ID = p.ID,
                              FirstName = c.FirstName,
                              LastName = c.LastName,
                              MobileNo = c.MobileNo,
                              Authenticated = p.Authenticated,
                              CreateDate = p.CreatedDate,
                              ErrorMessage = p.ErrorMessage
                          }).ToList();
            ViewBag.lstCustomer = report;
            return View();
        }

        public ActionResult DeleteAuthorized(int? ID)
        {
            var paytm = db.tbl_Paytm_Request.FirstOrDefault(s => s.ID == ID);
            if (paytm != null)
            {
                var detail = db.tbl_Paytm_Request_Details.Where(s => s.PaytmRequestID == paytm.ID);
                db.tbl_Paytm_Request_Details.RemoveRange(detail);
                db.tbl_Paytm_Request.Remove(paytm);
                db.SaveChanges();
            }
            return Redirect("/Report/PaytmAuthorized");
        }



        public ActionResult GetAttribute(string ProductId,string Vid)
        {
            List<SectorViewModel> list = new List<SectorViewModel>();


            var sector = objvendor.GetAttributeList(ProductId,Vid);
            if (sector.Rows.Count > 0)
            {
                for (int i = 0; i < sector.Rows.Count; i++)
                {
                    list.Add(new SectorViewModel { ID = sector.Rows[i]["Atid"].ToString(), Name = sector.Rows[i]["Name"].ToString()});
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
using MilkWayIndia.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MilkWayIndia.Abstract;
using MilkWayIndia.Concrete;
namespace MilkWayIndia.Controllers
{
    public class OfferController : Controller
    {

        Dictionary<string, object> res = new Dictionary<string, object>();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
        private IOffer _OfferRepo;

        public OfferController()
        {
            this._OfferRepo = new OfferRepository();
        }
        // GET: Offer
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddNewCustomerMsg()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            Sector objsector = new Sector();

            DataTable dt = new DataTable();
            dt = objsector.getStateList(null);
            ViewBag.State = dt;
            return View();
        }



        [HttpPost]

        public ActionResult AddNewCustomerMsg(Offer objoffer, FormCollection form)
        {
            string state = Request["ddlState"];
            string city = Request["ddlCity"];
            string Type = Request["ddlStatus"];
            if (!string.IsNullOrEmpty(state))
            {
                objoffer.StateId = Convert.ToInt32(state);
            }

            if (!string.IsNullOrEmpty(city))
            {
                objoffer.CityId = Convert.ToInt32(city);
            }

            if (!string.IsNullOrEmpty(Type))
            {
                objoffer.Type = Type.ToString();
            }
            // ViewBag.StateName = objsector.Statename;

            int addresult = objoffer.InsertNewCustomerMsg(objoffer);
            if (addresult > 0)
            { ViewBag.SuccessMsg = "Message Inserted Successfully!!!"; }
            else
            { ViewBag.SuccessMsg = "Message Not Inserted!!!"; }

            ModelState.Clear();

            Sector objsector = new Sector();

            DataTable dt = new DataTable();
            dt = objsector.getStateList(null);
            ViewBag.State = dt;

            return View();
        }


        [HttpGet]
        public ActionResult CustomerNewMsgList()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            //Sector objsector = new Sector();
            Offer objoffer = new Offer();
            DataTable dt = new DataTable();
            dt = objoffer.getMessageList(null);
            ViewBag.MessageList = dt;
            return View();
        }



        public JsonResult Status(int ID)
        {
            // _VendorRepo.UpdateVendorCatSubcatStatus
            var userInfo = _OfferRepo.UpdateVendorCatSubcatStatus(ID);
            if (userInfo != null)
            {
                if (userInfo.IsActive == false)
                {
                    userInfo.IsActive = false;
                    res["class"] = "label-danger";
                    res["name"] = "InActive";
                }
                else
                {
                    userInfo.IsActive = true;
                    res["class"] = "label-success";
                    res["name"] = "Active";
                }
                res["id"] = ID.ToString();
                res["status"] = "1";
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Status1(int ID)
        {
            // _VendorRepo.UpdateVendorCatSubcatStatus
            var userInfo = _OfferRepo.UpdateSectorMsg(ID);
            if (userInfo != null)
            {
                if (userInfo.IsActive == false)
                {
                    userInfo.IsActive = false;
                    res["class"] = "label-danger";
                    res["name"] = "InActive";
                }
                else
                {
                    userInfo.IsActive = true;
                    res["class"] = "label-success";
                    res["name"] = "Active";
                }
                res["id"] = ID.ToString();
                res["status"] = "1";
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AddOfferSectorWise()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            Sector objsector = new Sector();

            DataTable dt = new DataTable();
            dt = objsector.getStateList(null);
            ViewBag.State = dt;
            return View();
        }

        [HttpPost]
        public ActionResult AddOfferSectorWise(Offer objoffer, FormCollection frm, string[] chkSector)
        {

            //string vendorId = frm["ddlVendor"];
            //string Cat = frm["ddlCategory"];
            //string VendorCatname = frm["VendorCatName"];

            //string VendorMinAmount = frm["VendorMinAmount"];
            //string VendorMaxAmount = frm["VendorMaxAmount"];
            //string FromTime = frm["FromTime"];
            //string ToTime = frm["ToTime"];
            //string DeliveryFrom = frm["DeliveryFrom"];
            //string DeliveryTo = frm["DeliveryTo"];

           // Offer objoffer = new Offer();
            string StateId = frm["ddlState"];
            string CiytId = frm["ddlCity"];
            //string message = frm["Detail"];
            //if(message=="")
            //{
            //    objoffer.NewCustomerMsg = "";
            //}
            //else
            //objoffer.NewCustomerMsg = message.ToString();
            objoffer.StateId =Convert.ToInt32(StateId);
            objoffer.CityId = Convert.ToInt32(CiytId);

            DateTime Updatedon = Helper.indianTime;
            objoffer.Updatedon = Updatedon;
            
            int count = 0, dupcount = 0;

            int SectorOffer = 0;

            if (chkSector != null)
            {
                //int vendorcat = objVendor.InsertVendorcat(vendorId, VendorCatname, Cat);
                
                foreach (var item in chkSector)
                {

                    if (!string.IsNullOrEmpty(item))
                    {

                        string item1 = item.ToString();
                        objoffer.SectorId = Convert.ToInt32(item1);

                         SectorOffer = objoffer.InsertSectorOffer(objoffer);

                    }
                }
            }

            if (SectorOffer > 0 )
            {
                ViewBag.SuccessMsg = "Offer Inserted";
            }
            else
                ViewBag.SuccessMsg = "Error Occured";

            Sector objsector = new Sector();

            DataTable dt = new DataTable();
            dt = objsector.getStateList(null);
            ViewBag.State = dt;

            return View();
        }


        [HttpGet]
        public ActionResult SectorWiseMsgList()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            //Sector objsector = new Sector();
            Offer objoffer = new Offer();
            DataTable dt = new DataTable();
            dt = objoffer.getSectorMessageList(null);
            ViewBag.MessageList = dt;
            return View();
        }


        [HttpGet]
        public ActionResult EditSectorMsg(int id = 0)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;


            Offer objoffer = new Offer();

            DataTable dt = new DataTable();
            dt = objoffer.getSectorMessageListById(id);
            var model = new Offer();
           
            if (dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dt.Rows[0]["Message"].ToString()))
                    ViewBag.Message = dt.Rows[0]["Message"].ToString();
                else
                    ViewBag.Message = "";
                model.NewCustomerMsg = ViewBag.Message;
                model.NewCustomerMsg1 = ViewBag.Message;
                if (!string.IsNullOrEmpty(dt.Rows[0]["statename"].ToString()))
                    model.StateName = dt.Rows[0]["statename"].ToString();
                else
                    model.StateName = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["Cityname"].ToString()))
                    model.CityName = dt.Rows[0]["Cityname"].ToString();
                else
                    model.CityName = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["StateId"].ToString()))
                    model.StateId =Convert.ToInt32(dt.Rows[0]["StateId"].ToString());
                else
                    model.StateId = 0;
                if (!string.IsNullOrEmpty(dt.Rows[0]["CityId"].ToString()))
                    model.CityId =Convert.ToInt32(dt.Rows[0]["CityId"].ToString());
                else
                    model.CityId = 0;

            }
                //Sector objsector = new Sector();

                //DataTable dt = new DataTable();
                //dt = objsector.getStateList(null);
                //ViewBag.State = dt;
                return View(model);
        }

        public PartialViewResult GetSectorListMsgWise(int cId, int sId, string Msg)
        {
            Offer objprod = new Offer();
            DataTable dtProduct = objprod.GetSectorAssignedMsg(cId, sId, Msg);
            List<MsgSectorVM> product = new List<MsgSectorVM>();
            if (dtProduct.Rows.Count > 0)
            {
                for (int i = 0; i < dtProduct.Rows.Count; i++)
                {
                    MsgSectorVM p = new MsgSectorVM();
                    if (!string.IsNullOrEmpty(dtProduct.Rows[i]["SectorId"].ToString()))
                        p.Value = Convert.ToInt32(dtProduct.Rows[i]["SectorId"].ToString());
                    if (!string.IsNullOrEmpty(dtProduct.Rows[i]["SectorName"].ToString()))
                        p.Text = dtProduct.Rows[i]["SectorName"].ToString();
                    p.IsChecked = true;
                    product.Add(p);
                }
            }
            MsgSectorListVM _list = new MsgSectorListVM();
            _list.MsgSector = product;
            //ViewBag.lstProduct = dtProduct;
            return PartialView("_AssignSectorList", _list);
        }


        [HttpPost]
        public JsonResult EditSectorMsg(Offer objoffer, FormCollection frm, MsgSectorListVM model)
        {
            //Offer objoffer = new Offer();
            Dictionary<string, object> res = new Dictionary<string, object>();
            res["success"] = "0";
            int AddSectorProduct = 0, AddProduct = 0;
            //objVendor.VendorId = Convert.ToInt32(frm["ddlVendor"]);
            //objVendor.SectorId = Convert.ToInt32(frm["ddlSector"]);
            //objVendor.CategoryId = Convert.ToInt32(frm["ddlCategory"]);
            objoffer.IsActive = true;
            string a = objoffer.NewCustomerMsg;
           // objoffer.NewCustomerMsg = frm["Detail"];
            var chk = frm["chk"];
            var submit = frm["submit"];

            if(submit== "UnAssign Sector")
            {
                foreach (var item in model.MsgSector)
                {

                    string item1 = item.Value.ToString();


                    if (item.IsChecked == false)
                    {
                        AddSectorProduct = objoffer.DeleteSectorMessageUnAssigned(item.Value, objoffer.NewCustomerMsg);
                        AddProduct++;
                    }
                }
            }
            if(submit== "Update Message")
            {
                string oldmsg = frm["NewCustomerMsg1"];
                string newmsg = frm["NewCustomerMsg"];
                foreach (var item in model.MsgSector)
                {

                    string item1 = item.Value.ToString();


                    if (item.IsChecked == true)
                    {
                        AddSectorProduct = objoffer.UpdateSectorMessage(item.Value, oldmsg,newmsg);
                        AddProduct++;
                    }
                }
            }
        

            if (AddProduct > 0)
            {
                res["success"] = "1";
                res["message"] = "Success!! Update Successfully.";
            }
            else
                res["success"] = "0";
            return Json(res, JsonRequestBehavior.AllowGet);
        }




        [HttpGet]
        public ActionResult EditNewCustomerMsg(int id = 0)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;


            Offer objoffer = new Offer();

            DataTable dt = new DataTable();
            dt = objoffer.getNewMessageListById(id);
            var model = new Offer();

            if (dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dt.Rows[0]["Message"].ToString()))
                    ViewBag.Message = dt.Rows[0]["Message"].ToString();
                else
                    ViewBag.Message = "";
                model.NewCustomerMsg = ViewBag.Message;
                model.NewCustomerMsg1 = ViewBag.Message;
                //if (!string.IsNullOrEmpty(dt.Rows[0]["statename"].ToString()))
                //    model.StateName = dt.Rows[0]["statename"].ToString();
                //else
                //    model.StateName = "";

                //if (!string.IsNullOrEmpty(dt.Rows[0]["Cityname"].ToString()))
                //    model.CityName = dt.Rows[0]["Cityname"].ToString();
                //else
                //    model.CityName = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["StateId"].ToString()))
                    ViewBag.StateId = Convert.ToInt32(dt.Rows[0]["StateId"].ToString());
                else
                    ViewBag.StateId = 0;
                if (!string.IsNullOrEmpty(dt.Rows[0]["CityId"].ToString()))
                    ViewBag.CityName = Convert.ToInt32(dt.Rows[0]["CityId"].ToString());
                else
                    ViewBag.CityName = 0;


                if (!string.IsNullOrEmpty(dt.Rows[0]["MobileNo"].ToString()))
                    model.MobileNo =dt.Rows[0]["MobileNo"].ToString();
                else
                    model.MobileNo = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["Whatsappno"].ToString()))
                    model.Whatsappno = dt.Rows[0]["Whatsappno"].ToString();
                else
                    model.Whatsappno = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["Type"].ToString()))
                    ViewBag.Type = dt.Rows[0]["Type"].ToString();
                else
                    ViewBag.Type = "";

            }
            //Sector objsector = new Sector();

            //DataTable dt = new DataTable();
            //dt = objsector.getStateList(null);
            //ViewBag.State = dt;

            Sector objsector = new Sector();

             dt = new DataTable();
            dt = objsector.getStateList(null);
            ViewBag.State = dt;


            dt = new DataTable();
            dt = objsector.getCityList(null);
            ViewBag.City = dt;

            return View(model);
        }

        [HttpPost]
        public ActionResult EditNewCustomerMsg(Offer objoffer, FormCollection form)
        {
            //DataTable dtDuplicateSec = objsector.getCheckDuplSector(objsector.SectorName);
            //if (dtDuplicateSec.Rows.Count > 0)
            //{
            //    int SId = Convert.ToInt32(dtDuplicateSec.Rows[0]["Id"]);
            //    if (SId == objsector.Id)
            //    {
            //        int result = objsector.UpdateSector(objsector);
            //        if (result > 0)
            //        {
            //            ViewBag.SuccessMsg = "Sector Updated Successfully!!!";
            //        }
            //        else
            //        {
            //            ViewBag.SuccessMsg = "Sector Not Updated!!!";
            //        }
            //    }
            //    else
            //    {
            //        ViewBag.SuccessMsg = "Sector Name Already Exits!!!";
            //    }
            //}
            //else
            //{
            //    int result = objsector.UpdateSector(objsector);
            //    if (result > 0)
            //    {
            //        ViewBag.SuccessMsg = "Sector Updated Successfully!!!";
            //    }
            //    else
            //    {
            //        ViewBag.SuccessMsg = "Sector Not Updated!!!";
            //    }
            //}
            string id = objoffer.Id.ToString();
            string state = Request["ddlState"];
            string city = Request["ddlCity"];
            string Type = Request["ddlStatus"];
            if (!string.IsNullOrEmpty(state))
            {
                objoffer.StateId = Convert.ToInt32(state);
            }

            if (!string.IsNullOrEmpty(city))
            {
                objoffer.CityId = Convert.ToInt32(city);
            }

            if (!string.IsNullOrEmpty(Type))
            {
                objoffer.Type = Type.ToString();
            }
            // ViewBag.StateName = objsector.Statename;

            int addresult = objoffer.UpdateNewCustomerMsg(objoffer);

          if(addresult>0)
            {

                ViewBag.SuccessMsg = "Message Updated Successfully!!!";
            }
            else
            {
                ViewBag.SuccessMsg = "Message Not Updated!!!";
            }


            DataTable dt = new DataTable();
            dt = objoffer.getNewMessageListById(objoffer.Id);
            var model = new Offer();

            if (dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dt.Rows[0]["Message"].ToString()))
                    ViewBag.Message = dt.Rows[0]["Message"].ToString();
                else
                    ViewBag.Message = "";
                model.NewCustomerMsg = ViewBag.Message;
                model.NewCustomerMsg1 = ViewBag.Message;
                //if (!string.IsNullOrEmpty(dt.Rows[0]["statename"].ToString()))
                //    model.StateName = dt.Rows[0]["statename"].ToString();
                //else
                //    model.StateName = "";

                //if (!string.IsNullOrEmpty(dt.Rows[0]["Cityname"].ToString()))
                //    model.CityName = dt.Rows[0]["Cityname"].ToString();
                //else
                //    model.CityName = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["StateId"].ToString()))
                    ViewBag.StateId = Convert.ToInt32(dt.Rows[0]["StateId"].ToString());
                else
                    ViewBag.StateId = 0;
                if (!string.IsNullOrEmpty(dt.Rows[0]["CityId"].ToString()))
                    ViewBag.CityName = Convert.ToInt32(dt.Rows[0]["CityId"].ToString());
                else
                    ViewBag.CityName = 0;


                if (!string.IsNullOrEmpty(dt.Rows[0]["MobileNo"].ToString()))
                    model.MobileNo = dt.Rows[0]["MobileNo"].ToString();
                else
                    model.MobileNo = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["Whatsappno"].ToString()))
                    model.Whatsappno = dt.Rows[0]["Whatsappno"].ToString();
                else
                    model.Whatsappno = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["Type"].ToString()))
                    ViewBag.Type = dt.Rows[0]["Type"].ToString();
                else
                    ViewBag.Type = "";

            }
            //Sector objsector = new Sector();

            //DataTable dt = new DataTable();
            //dt = objsector.getStateList(null);
            //ViewBag.State = dt;

            Sector objsector = new Sector();

            dt = new DataTable();
            dt = objsector.getStateList(null);
            ViewBag.State = dt;


            dt = new DataTable();
            dt = objsector.getCityList(null);
            ViewBag.City = dt;

            return View(model);
        }

    }
}

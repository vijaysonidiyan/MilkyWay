using MilkWayIndia.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MilkWayIndia.Controllers
{
    public class ChatbotController : Controller
    {
        // GET: Chatbot
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult ChatbotList()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;
            Chatbot objchatbot = new Chatbot();
                DataTable dtList = new DataTable();
                dtList = objchatbot.getChatbotquery(null);
                ViewBag.ChatbotList = dtList;
                return View();
            
        }


        [HttpGet]
        public ActionResult AddChatbot()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");
            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            return View();
        }

        [HttpPost]
        public ActionResult AddChatbot(Chatbot objchatbot, FormCollection form)
        {

            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            string category = Request["ddlcat"];
            string yesno = Request["ddlyesno"];
            objchatbot.ChatbotCategory = category;
            objchatbot.Displayyesno = yesno;


            string category2 = Request["ddlcat2"];
            string yesno2 = Request["ddlyesno2"];
            objchatbot.Chatbot2Category = category2;
            objchatbot.Chatbot2DisplayYesNo = yesno2;


            string category3 = Request["ddlcat3"];
            string yesno3 = Request["ddlyesno3"];
            objchatbot.Chatbot3Category = category3;
            objchatbot.Chatbot3DisplayYesNo = yesno3;

            int addresult = objchatbot.Insertchatbot(objchatbot);
            if (addresult > 0)
            { ViewBag.SuccessMsg = "Chatbot Query Inserted Successfully!!!"; }
            else
            { ViewBag.SuccessMsg = "Chatbot Query Not Inserted!!!"; }

            ModelState.Clear();
            return View();
           
           
        }

        [HttpGet]
        public ActionResult EditChatbot(int id=0)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");
            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            Chatbot objchatbot = new Chatbot();
            var model = new Chatbot();
            DataTable dt = new DataTable();
            dt = objchatbot.getChatbotquery(id);
            if (dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dt.Rows[0]["ChatbotQue"].ToString()))
                {
                    model.Chatbotquery = dt.Rows[0]["ChatbotQue"].ToString();
                    ViewBag.Query = dt.Rows[0]["ChatbotQue"].ToString();
                }
                   
                else
                    ViewBag.Query = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["generalreply"].ToString()))
                {
                    model.ChatbotGeneralReply = dt.Rows[0]["generalreply"].ToString();
                    ViewBag.QueryYes = dt.Rows[0]["generalreply"].ToString();
                }

                if (!string.IsNullOrEmpty(dt.Rows[0]["Chatbotyesreply"].ToString()))
                {
                    model.ChatbotYesReply= dt.Rows[0]["Chatbotyesreply"].ToString();
                    ViewBag.QueryYes = dt.Rows[0]["Chatbotyesreply"].ToString();
                }
                   
                else
                    ViewBag.QueryYes = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["Chatbotnoreply"].ToString()))

                {
                    model.ChatbotNoReply = dt.Rows[0]["Chatbotnoreply"].ToString();
                    ViewBag.QueryNo = dt.Rows[0]["Chatbotnoreply"].ToString();
                }
                    
                else
                    ViewBag.QueryNo = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["SortOrder"].ToString()))
                    model.SortOrder =Convert.ToInt32(dt.Rows[0]["SortOrder"].ToString());
                else
                    ViewBag.SortOrder = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["Category"].ToString()))
                    ViewBag.CategoryId =dt.Rows[0]["Category"].ToString();
                else
                    ViewBag.CategoryId = "0";
                if (!string.IsNullOrEmpty(dt.Rows[0]["displayyesno"].ToString()))
                    ViewBag.DisplayId = dt.Rows[0]["displayyesno"].ToString();
                else
                    ViewBag.DisplayId = "0";



                if (!string.IsNullOrEmpty(dt.Rows[0]["ChatbotQue2Yes"].ToString()))

                {
                    model.ChatbotQue2Yes = dt.Rows[0]["ChatbotQue2Yes"].ToString();
                   
                }

                if (!string.IsNullOrEmpty(dt.Rows[0]["Chatbot2YesReply"].ToString()))

                {
                    model.Chatbot2YesReply = dt.Rows[0]["Chatbot2YesReply"].ToString();

                }
                if (!string.IsNullOrEmpty(dt.Rows[0]["Chatbot2NoReply"].ToString()))

                {
                    model.Chatbot2NoReply = dt.Rows[0]["Chatbot2NoReply"].ToString();

                }

                if (!string.IsNullOrEmpty(dt.Rows[0]["Chatbot2GeneralReply"].ToString()))

                {
                    model.Chatbot2GeneralReply = dt.Rows[0]["Chatbot2GeneralReply"].ToString();

                }


                if (!string.IsNullOrEmpty(dt.Rows[0]["ChatbotCategory"].ToString()))
                    ViewBag.CategoryId2 = dt.Rows[0]["ChatbotCategory"].ToString();
                else
                    ViewBag.CategoryId2 = "0";
                if (!string.IsNullOrEmpty(dt.Rows[0]["Chatbot2DisplayYesNo"].ToString()))
                    ViewBag.DisplayId2 = dt.Rows[0]["Chatbot2DisplayYesNo"].ToString();
                else
                    ViewBag.DisplayId2 = "0";

                if (!string.IsNullOrEmpty(dt.Rows[0]["ChatbotQue3No"].ToString()))

                {
                    model.ChatbotQue3No = dt.Rows[0]["ChatbotQue3No"].ToString();

                }
                if (!string.IsNullOrEmpty(dt.Rows[0]["Chatbot3YesReply"].ToString()))

                {
                    model.Chatbot3YesReply = dt.Rows[0]["Chatbot3YesReply"].ToString();

                }

                if (!string.IsNullOrEmpty(dt.Rows[0]["Chatbot3NoReply"].ToString()))

                {
                    model.Chatbot3NoReply = dt.Rows[0]["Chatbot3NoReply"].ToString();

                }


                if (!string.IsNullOrEmpty(dt.Rows[0]["Chatbot3GeneralReply"].ToString()))

                {
                    model.Chatbot3GeneralReply = dt.Rows[0]["Chatbot3GeneralReply"].ToString();

                }

                if (!string.IsNullOrEmpty(dt.Rows[0]["Chatbot3Category"].ToString()))
                    ViewBag.CategoryId3 = dt.Rows[0]["Chatbot3Category"].ToString();
                else
                    ViewBag.CategoryId3 = "0";
                if (!string.IsNullOrEmpty(dt.Rows[0]["Chatbot3DisplayYesNo"].ToString()))
                    ViewBag.DisplayId3 = dt.Rows[0]["Chatbot3DisplayYesNo"].ToString();
                else
                    ViewBag.DisplayId3 = "0";

            }


                return View(model);
        }



        [HttpPost]
        public ActionResult EditChatbot(Chatbot objchatbot, FormCollection form)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            string category = Request["ddlcat"];
            string yesno = Request["ddlyesno"];
            objchatbot.ChatbotCategory = category;
            objchatbot.Displayyesno = yesno;


            string category2 = Request["ddlcat2"];
            string yesno2 = Request["ddlyesno2"];
            objchatbot.Chatbot2Category = category2;
            objchatbot.Chatbot2DisplayYesNo = yesno2;


            string category3 = Request["ddlcat3"];
            string yesno3 = Request["ddlyesno3"];
            objchatbot.Chatbot3Category = category3;
            objchatbot.Chatbot3DisplayYesNo = yesno3;


            DataTable dt = new DataTable();
                
                //check data duplicate

                int result = objchatbot.UpdateChatbot(objchatbot);
                if (result > 0)
                {
                    ViewBag.SuccessMsg = "Chatbot Updated Successfully!!!";
                }
                else
                { ViewBag.SuccessMsg = "Chatbot Not Updated!!!"; }





           // DataTable dt = new DataTable();
            dt = objchatbot.getChatbotquery(objchatbot.Id);
            if (dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dt.Rows[0]["ChatbotQue"].ToString()))
                    ViewBag.Query = dt.Rows[0]["ChatbotQue"].ToString();
                else
                    ViewBag.Query = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["Chatbotyesreply"].ToString()))
                    ViewBag.QueryYes = dt.Rows[0]["Chatbotyesreply"].ToString();
                else
                    ViewBag.QueryYes = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["Chatbotnoreply"].ToString()))
                    ViewBag.QueryNo = dt.Rows[0]["Chatbotnoreply"].ToString();
                else
                    ViewBag.QueryNo = "";

                if (!string.IsNullOrEmpty(dt.Rows[0]["SortOrder"].ToString()))
                    ViewBag.SortOrder = dt.Rows[0]["SortOrder"].ToString();
                else
                    ViewBag.SortOrder = "";
            }
            return View();
           
        }


        [HttpGet]
        public ActionResult DeleteChatbot(int id)
        {
            try
            {
                // int delresult = 0;
                Chatbot objchatbot = new Chatbot();
                int delresult = objchatbot.DeleteChatbot(id);
                return RedirectToAction("ChatbotList");
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
            return RedirectToAction("ChatbotList");
        }
    }
}
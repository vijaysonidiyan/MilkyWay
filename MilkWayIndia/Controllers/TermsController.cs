using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MilkWayIndia.Models;
namespace MilkWayIndia.Controllers
{
    public class TermsController : Controller
    {
        // GET: Terms


        [HttpGet]
        public ActionResult TermsList(int? Id)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            Terms objterms = new Terms();
            DataTable dtList = new DataTable();
            dtList = objterms.getTermsList(null);
            ViewBag.TermsList = dtList;
            return View();

        }
        [HttpGet]
        public ActionResult AddTerms()
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


        [HttpPost, ValidateInput(false)]
        public ActionResult AddTerms(Terms model, HttpPostedFileBase photo)
        {

            string terms = model.terms;
           

            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            int addresult = model.Insertterms(model);
            if (addresult > 0)
            { ViewBag.SuccessMsg = "Terms & Condition Inserted Successfully!!!"; }
            else
            { ViewBag.SuccessMsg = "Terms & Condition Not Inserted!!!"; }

            ModelState.Clear();

            return View();
        }



        [HttpGet]
        public ActionResult EditTerms(int? ID)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");
            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;


            Terms model = new Terms();
            var advertisement = model.getTermsList(ID);

            DataTable dtList = new DataTable();
            dtList = model.getTermsList(ID);


            if(dtList.Rows.Count>0)
            {
                model.Id =Convert.ToInt32(dtList.Rows[0].ItemArray[0]);
                model.Pos = Convert.ToInt32(dtList.Rows[0].ItemArray[1]);
                model.terms = dtList.Rows[0].ItemArray[2].ToString();

            }
            
            return View(model);

            
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult EditTerms(Terms model, HttpPostedFileBase photo)
        {

            string terms = model.terms;


            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            int addresult = model.Updateterms(model);
            if (addresult > 0)
            { ViewBag.SuccessMsg = "Terms & Condition Updated Successfully!!!"; }
            else
            { ViewBag.SuccessMsg = "Terms & Condition Not Updated!!!"; }

            ModelState.Clear();

            return View();
        }


        [HttpGet]
        public ActionResult DeleteTerms(int id)
        {
            try
            {
                // int delresult = 0;
                Terms objterms = new Terms();
                int delresult = objterms.Deleteterms(id);
                return RedirectToAction("TermsList");
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
            return RedirectToAction("TermsList");
        }


        [HttpGet]
        public ActionResult Invoice()
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
    }
}
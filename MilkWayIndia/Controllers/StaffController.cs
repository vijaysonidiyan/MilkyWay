using MilkWayIndia.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MilkWayIndia.Controllers
{
    public class StaffController : Controller
    {
        Staff objStaff = new Staff();
        // GET: Staff
        [HttpGet]
        public ActionResult AddStaff()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddStaff(Staff objStaff, FormCollection form, HttpPostedFileBase Photo)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                string role = Request["ddlRole"];
                if (!string.IsNullOrEmpty(role))
                {
                    objStaff.Role = role;
                }
                //check username
                DataTable dtuserRecord1 = new DataTable();
                dtuserRecord1 = objStaff.CheckStaffUserName(objStaff.UserName);
                int userRecords1 = dtuserRecord1.Rows.Count;
                if (userRecords1 > 0)
                {
                    ViewBag.SuccessMsg = "UserName Already Exits!!!";
                    return View();
                }

                var dob = Request["DOB"];
                if (!string.IsNullOrEmpty(dob.ToString()))
                {
                    objStaff.DOB = Convert.ToDateTime(DateTime.ParseExact(dob, @"dd/MM/yyyy", null));
                    // FromDate = objsub.FromDate;
                }
                else
                { objStaff.DOB = null; }

                //check data duplicate
                DataTable dtDupliStaff = new DataTable();
                dtDupliStaff = objStaff.CheckDuplicateStaff(objStaff.Role, objStaff.FirstName, objStaff.LastName, objStaff.MobileNo);
                if (dtDupliStaff.Rows.Count > 0)
                {
                    ViewBag.SuccessMsg = "Data Already Exits!!!";
                }
                else
                {
                    string fname = null, path = null;
                    HttpPostedFileBase document1 = Request.Files["Photo"];
                    string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png" };
                    if (document1 != null)
                    {
                        if (document1.ContentLength > 0)
                        {
                            try
                            {
                                HttpFileCollectionBase files = Request.Files;
                                HttpPostedFileBase file = Photo;
                                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                                string extension = Path.GetExtension(file.FileName);
                                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                                {
                                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                    fname = testfiles[testfiles.Length - 1];
                                }
                                else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                                {
                                    ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
                                }
                                else
                                {
                                    fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                                }
                                path = Path.Combine(Server.MapPath("~/image/staffphoto/"), fname);
                                file.SaveAs(path);
                                objStaff.Photo = fname;
                            }

                            catch (Exception ex)
                            {
                                ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                            }
                        }
                    }
                    int addresult = objStaff.InsertStaff(objStaff);
                    if (addresult > 0)
                    { ViewBag.SuccessMsg = "Staff Inserted Successfully!!!"; }
                    else
                    { ViewBag.SuccessMsg = "Staff Not Inserted!!!"; }
                }
                ModelState.Clear();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        [HttpGet]
        public ActionResult EditStaff(int id = 0)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                DataTable dt = new DataTable();
                dt = objStaff.getStaffList(id);
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Role"].ToString()))
                        ViewBag.Role = dt.Rows[0]["Role"].ToString();
                    else
                        ViewBag.Role = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["FirstName"].ToString()))
                        ViewBag.FirstName = dt.Rows[0]["FirstName"].ToString();
                    else
                        ViewBag.FirstName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["LastName"].ToString()))
                        ViewBag.LastName = dt.Rows[0]["LastName"].ToString();
                    else
                        ViewBag.LastName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["MobileNo"].ToString()))
                        ViewBag.MobileNo = dt.Rows[0]["MobileNo"].ToString();
                    else
                        ViewBag.MobileNo = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Email"].ToString()))
                        ViewBag.Email = dt.Rows[0]["Email"].ToString();
                    else
                        ViewBag.Email = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Address"].ToString()))
                        ViewBag.Address = dt.Rows[0]["Address"].ToString();
                    else
                        ViewBag.Address = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["DOB"].ToString()))
                    {
                        ViewBag.DOB = dt.Rows[0]["DOB"].ToString();
                        DateTime dateFromString =
                              DateTime.Parse(ViewBag.DOB, System.Globalization.CultureInfo.InvariantCulture);
                        ViewBag.DOB = dateFromString.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        ViewBag.DOB = null;
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Photo"].ToString()))
                        ViewBag.Photo = dt.Rows[0]["Photo"].ToString();
                    else
                        ViewBag.Photo = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["UserName"].ToString()))
                        ViewBag.UserName = dt.Rows[0]["UserName"].ToString();
                    else
                        ViewBag.UserName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Password"].ToString()))
                        ViewBag.Password = dt.Rows[0]["Password"].ToString();
                    else
                        ViewBag.Password = "";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditStaff(Staff objStaff, FormCollection form, HttpPostedFileBase Photo)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                DataTable dt = new DataTable();
                string role = Request["ddlRole"];
                if (!string.IsNullOrEmpty(role))
                {
                    objStaff.Role = role;
                }
                //check data duplicate
                DataTable dtDupliStaff = new DataTable();
                dtDupliStaff = objStaff.CheckDuplicateStaff(objStaff.Role, objStaff.FirstName, objStaff.LastName, objStaff.MobileNo);
                if (dtDupliStaff.Rows.Count > 0)
                {
                    int SId = Convert.ToInt32(dtDupliStaff.Rows[0]["Id"]);
                    if (SId == objStaff.Id)
                    {
                        //check username
                        DataTable dtuserRecord1 = new DataTable();
                        dtuserRecord1 = objStaff.CheckStaffUserName(objStaff.UserName);
                        int userRecords1 = dtuserRecord1.Rows.Count;
                        if (userRecords1 > 0)
                        {
                            int sId = Convert.ToInt32(dtuserRecord1.Rows[0]["Id"]);
                            if (sId == objStaff.Id)
                            {
                            }
                            else
                            {
                                ViewBag.SuccessMsg = "UserName Already Exits!!!";
                                dt = objStaff.getStaffList(objStaff.Id);
                                if (dt.Rows.Count > 0)
                                {
                                    if (!string.IsNullOrEmpty(dt.Rows[0]["Role"].ToString()))
                                        ViewBag.Role = dt.Rows[0]["Role"].ToString();
                                    else
                                        ViewBag.Role = "";
                                    if (!string.IsNullOrEmpty(dt.Rows[0]["FirstName"].ToString()))
                                        ViewBag.FirstName = dt.Rows[0]["FirstName"].ToString();
                                    else
                                        ViewBag.FirstName = "";
                                    if (!string.IsNullOrEmpty(dt.Rows[0]["LastName"].ToString()))
                                        ViewBag.LastName = dt.Rows[0]["LastName"].ToString();
                                    else
                                        ViewBag.LastName = "";
                                    if (!string.IsNullOrEmpty(dt.Rows[0]["MobileNo"].ToString()))
                                        ViewBag.MobileNo = dt.Rows[0]["MobileNo"].ToString();
                                    else
                                        ViewBag.MobileNo = "";
                                    if (!string.IsNullOrEmpty(dt.Rows[0]["Email"].ToString()))
                                        ViewBag.Email = dt.Rows[0]["Email"].ToString();
                                    else
                                        ViewBag.Email = "";
                                    if (!string.IsNullOrEmpty(dt.Rows[0]["Address"].ToString()))
                                        ViewBag.Address = dt.Rows[0]["Address"].ToString();
                                    else
                                        ViewBag.Address = "";
                                    if (!string.IsNullOrEmpty(dt.Rows[0]["DOB"].ToString()))
                                    {
                                        ViewBag.DOB = dt.Rows[0]["DOB"].ToString();
                                        DateTime dateFromString =
                                              DateTime.Parse(ViewBag.DOB, System.Globalization.CultureInfo.InvariantCulture);
                                        ViewBag.DOB = dateFromString.ToString("dd/MM/yyyy");
                                    }
                                    else
                                    {
                                        ViewBag.DOB = null;
                                    }
                                    if (!string.IsNullOrEmpty(dt.Rows[0]["Photo"].ToString()))
                                        ViewBag.Photo = dt.Rows[0]["Photo"].ToString();
                                    else
                                        ViewBag.Photo = "";
                                    if (!string.IsNullOrEmpty(dt.Rows[0]["UserName"].ToString()))
                                        ViewBag.UserName = dt.Rows[0]["UserName"].ToString();
                                    else
                                        ViewBag.UserName = "";
                                    if (!string.IsNullOrEmpty(dt.Rows[0]["Password"].ToString()))
                                        ViewBag.Password = dt.Rows[0]["Password"].ToString();
                                    else
                                        ViewBag.Password = "";
                                }
                                return View();
                            }

                        }
                        var dob = Request["DOB"];
                        if (!string.IsNullOrEmpty(dob.ToString()))
                        {
                            objStaff.DOB = Convert.ToDateTime(DateTime.ParseExact(dob, @"dd/MM/yyyy", null));
                            // FromDate = objsub.FromDate;
                        }
                        else
                        { objStaff.DOB = null; }

                        string fname = null, path = null;
                        HttpPostedFileBase document1 = Request.Files["Photo"];
                        string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png" };
                        if (document1 != null)
                        {
                            if (document1.ContentLength > 0)
                            {
                                try
                                {
                                    HttpFileCollectionBase files = Request.Files;
                                    HttpPostedFileBase file = Photo;
                                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                                    string extension = Path.GetExtension(file.FileName);
                                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                                    {
                                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                        fname = testfiles[testfiles.Length - 1];
                                    }
                                    else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                                    {
                                        ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
                                    }
                                    else
                                    {
                                        fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                                    }
                                    path = Path.Combine(Server.MapPath("~/image/staffphoto/"), fname);
                                    file.SaveAs(path);
                                    objStaff.Photo = fname;
                                }
                                catch (Exception ex)
                                {
                                    ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                                }
                            }
                            else
                            {
                                var existfile = "";
                                DataTable dt1 = objStaff.getStaffList(objStaff.Id);
                                ViewBag.Photo = dt1.Rows[0]["Photo"].ToString();
                                existfile = ViewBag.Photo;
                                objStaff.Photo = existfile;
                            }
                        }
                        else
                        {
                            var existfile = "";
                            DataTable dt1 = objStaff.getStaffList(objStaff.Id);
                            ViewBag.Photo = dt1.Rows[0]["Photo"].ToString();
                            existfile = ViewBag.Photo;
                            objStaff.Photo = existfile;
                        }
                        int result = objStaff.UpdateStaff(objStaff);
                        if (result > 0)
                        {
                            ViewBag.SuccessMsg = "Staff Updated Successfully!!!";
                        }
                        else
                        { ViewBag.SuccessMsg = "Staff Not Updated!!!"; }
                    }
                    else
                    {
                        ViewBag.SuccessMsg = "Data Already Exits!!!";
                    }
                }
                else
                {
                    //check username
                    DataTable dtuserRecord1 = new DataTable();
                    dtuserRecord1 = objStaff.CheckStaffUserName(objStaff.UserName);
                    int userRecords1 = dtuserRecord1.Rows.Count;
                    if (userRecords1 > 0)
                    {
                        int SId = Convert.ToInt32(dtuserRecord1.Rows[0]["Id"]);
                        if (SId == objStaff.Id)
                        {
                        }
                        else
                        {
                            ViewBag.SuccessMsg = "UserName Already Exits!!!";
                            dt = objStaff.getStaffList(objStaff.Id);
                            if (dt.Rows.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(dt.Rows[0]["Role"].ToString()))
                                    ViewBag.Role = dt.Rows[0]["Role"].ToString();
                                else
                                    ViewBag.Role = "";
                                if (!string.IsNullOrEmpty(dt.Rows[0]["FirstName"].ToString()))
                                    ViewBag.FirstName = dt.Rows[0]["FirstName"].ToString();
                                else
                                    ViewBag.FirstName = "";
                                if (!string.IsNullOrEmpty(dt.Rows[0]["LastName"].ToString()))
                                    ViewBag.LastName = dt.Rows[0]["LastName"].ToString();
                                else
                                    ViewBag.LastName = "";
                                if (!string.IsNullOrEmpty(dt.Rows[0]["MobileNo"].ToString()))
                                    ViewBag.MobileNo = dt.Rows[0]["MobileNo"].ToString();
                                else
                                    ViewBag.MobileNo = "";
                                if (!string.IsNullOrEmpty(dt.Rows[0]["Email"].ToString()))
                                    ViewBag.Email = dt.Rows[0]["Email"].ToString();
                                else
                                    ViewBag.Email = "";
                                if (!string.IsNullOrEmpty(dt.Rows[0]["Address"].ToString()))
                                    ViewBag.Address = dt.Rows[0]["Address"].ToString();
                                else
                                    ViewBag.Address = "";
                                if (!string.IsNullOrEmpty(dt.Rows[0]["DOB"].ToString()))
                                {
                                    ViewBag.DOB = dt.Rows[0]["DOB"].ToString();
                                    DateTime dateFromString =
                                          DateTime.Parse(ViewBag.DOB, System.Globalization.CultureInfo.InvariantCulture);
                                    ViewBag.DOB = dateFromString.ToString("dd/MM/yyyy");
                                }
                                else
                                {
                                    ViewBag.DOB = null;
                                }
                                if (!string.IsNullOrEmpty(dt.Rows[0]["Photo"].ToString()))
                                    ViewBag.Photo = dt.Rows[0]["Photo"].ToString();
                                else
                                    ViewBag.Photo = "";
                                if (!string.IsNullOrEmpty(dt.Rows[0]["UserName"].ToString()))
                                    ViewBag.UserName = dt.Rows[0]["UserName"].ToString();
                                else
                                    ViewBag.UserName = "";
                                if (!string.IsNullOrEmpty(dt.Rows[0]["Password"].ToString()))
                                    ViewBag.Password = dt.Rows[0]["Password"].ToString();
                                else
                                    ViewBag.Password = "";
                            }
                            return View();
                        }
                    }
                    var dob = Request["DOB"];
                    if (!string.IsNullOrEmpty(dob.ToString()))
                    {
                        objStaff.DOB = Convert.ToDateTime(DateTime.ParseExact(dob, @"dd/MM/yyyy", null));
                        // FromDate = objsub.FromDate;
                    }
                    else
                    { objStaff.DOB = null; }

                    string fname = null, path = null;
                    HttpPostedFileBase document1 = Request.Files["Photo"];
                    string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png" };
                    if (document1 != null)
                    {
                        if (document1.ContentLength > 0)
                        {
                            try
                            {
                                HttpFileCollectionBase files = Request.Files;
                                HttpPostedFileBase file = Photo;
                                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                                string extension = Path.GetExtension(file.FileName);
                                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                                {
                                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                    fname = testfiles[testfiles.Length - 1];
                                }
                                else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                                {
                                    ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
                                }
                                else
                                {
                                    fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                                }
                                path = Path.Combine(Server.MapPath("~/image/staffphoto/"), fname);
                                file.SaveAs(path);
                                objStaff.Photo = fname;
                            }
                            catch (Exception ex)
                            {
                                ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
                            }
                        }
                        else
                        {
                            var existfile = "";
                            DataTable dt1 = objStaff.getStaffList(objStaff.Id);
                            ViewBag.Photo = dt1.Rows[0]["Photo"].ToString();
                            existfile = ViewBag.Photo;
                            objStaff.Photo = existfile;
                        }
                    }
                    else
                    {
                        var existfile = "";
                        DataTable dt1 = objStaff.getStaffList(objStaff.Id);
                        ViewBag.Photo = dt1.Rows[0]["Photo"].ToString();
                        existfile = ViewBag.Photo;
                        objStaff.Photo = existfile;
                    }
                    int result = objStaff.UpdateStaff(objStaff);
                    if (result > 0)
                    {
                        ViewBag.SuccessMsg = "Staff Updated Successfully!!!";
                    }
                    else
                    { ViewBag.SuccessMsg = "Staff Not Updated!!!"; }
                }

                dt = objStaff.getStaffList(objStaff.Id);
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Role"].ToString()))
                        ViewBag.Role = dt.Rows[0]["Role"].ToString();
                    else
                        ViewBag.Role = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["FirstName"].ToString()))
                        ViewBag.FirstName = dt.Rows[0]["FirstName"].ToString();
                    else
                        ViewBag.FirstName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["LastName"].ToString()))
                        ViewBag.LastName = dt.Rows[0]["LastName"].ToString();
                    else
                        ViewBag.LastName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["MobileNo"].ToString()))
                        ViewBag.MobileNo = dt.Rows[0]["MobileNo"].ToString();
                    else
                        ViewBag.MobileNo = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Email"].ToString()))
                        ViewBag.Email = dt.Rows[0]["Email"].ToString();
                    else
                        ViewBag.Email = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Address"].ToString()))
                        ViewBag.Address = dt.Rows[0]["Address"].ToString();
                    else
                        ViewBag.Address = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["DOB"].ToString()))
                    {
                        ViewBag.DOB = dt.Rows[0]["DOB"].ToString();
                        DateTime dateFromString =
                              DateTime.Parse(ViewBag.DOB, System.Globalization.CultureInfo.InvariantCulture);
                        ViewBag.DOB = dateFromString.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        ViewBag.DOB = null;
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Photo"].ToString()))
                        ViewBag.Photo = dt.Rows[0]["Photo"].ToString();
                    else
                        ViewBag.Photo = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["UserName"].ToString()))
                        ViewBag.UserName = dt.Rows[0]["UserName"].ToString();
                    else
                        ViewBag.UserName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Password"].ToString()))
                        ViewBag.Password = dt.Rows[0]["Password"].ToString();
                    else
                        ViewBag.Password = "";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult StaffList()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                DataTable dtList = new DataTable();
                dtList = objStaff.getStaffList(null);
                ViewBag.StaffList = dtList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult DeleteStaff(int id)
        {
            try
            {
                // int delresult = 0;
                int delresult = objStaff.DeleteStaff(id);
                return RedirectToAction("StaffList");
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
            return RedirectToAction("StaffList");
        }




        [HttpGet]
        public ActionResult PlatformList()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                DataTable dtList = new DataTable();
                dtList = objStaff.getPlatformList(null);
                ViewBag.PlatformList = dtList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }



        [HttpGet]
        public ActionResult AddPlatform()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpPost]
        public ActionResult AddPlatform(Staff objStaff, FormCollection form, string[] chkSector)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                int dupcount = 0, count = 0;
                DataTable dtList = new DataTable();
                dtList = objStaff.ChkDuplRole(objStaff.PlatformName);
                if (dtList.Rows.Count > 0)
                {
                    ViewBag.SuccessMsg = "Role Already Exist!!!";
                }
                else
                {
                    int addresult = objStaff.InsertPlatform(objStaff);
                    if (addresult > 0)
                    {

                        if (chkSector != null)
                        {
                            foreach (var item in chkSector)
                            {
                                if (!string.IsNullOrEmpty(item))
                                {
                                    string item1 = item.ToString();


                                    int Vendorassign = objStaff.InsertRoleModule(item1, addresult.ToString());
                                    if (Vendorassign > 0)
                                        count++;
                                    //}
                                }

                            }

                        }
                        ViewBag.SuccessMsg = "Role Inserted with  " + count + " Module Successfully.  " + dupcount + " Duplicate Module Ignored";




                    }

                    else
                    { ViewBag.SuccessMsg = "Role Not Inserted!!!"; }
                }



                ModelState.Clear();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpGet]
        public ActionResult DeleteRole(int id)
        {
            try
            {
                // int delresult = 0;
                int delresult = objStaff.DeleteRole(id);
                return RedirectToAction("PlatformList");
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
            return RedirectToAction("PlatformList");
        }


        [HttpGet]
        public ActionResult ModuleList()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                DataTable dtList = new DataTable();
                dtList = objStaff.getModuleList(null);
                ViewBag.ModuleList = dtList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult AddModule()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddModule(Staff objStaff, FormCollection form)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {

                objStaff.PlatformName = Request["ddlplatform"];
                int addresult = objStaff.InsertModule(objStaff);
                if (addresult > 0)
                { ViewBag.SuccessMsg = "Module Inserted Successfully!!!"; }
                else
                { ViewBag.SuccessMsg = "Module Not Inserted!!!"; }

                ModelState.Clear();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpGet]
        public ActionResult EditModule(int id = 0)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                DataTable dt = new DataTable();
                dt = objStaff.getModuleList(id);
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["PlatformName"].ToString()))
                        ViewBag.PlatformName = dt.Rows[0]["PlatformName"].ToString();
                    else
                        ViewBag.PlatformName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Modulename"].ToString()))
                        ViewBag.Modulename = dt.Rows[0]["Modulename"].ToString();
                    else
                        ViewBag.Modulename = "";


                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditModule(Staff objStaff, FormCollection form)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                DataTable dt = new DataTable();
                string Platform = Request["ddlplatform"];
                if (!string.IsNullOrEmpty(Platform))
                {
                    objStaff.PlatformName = Platform;
                }
                int id = objStaff.Id;
                //check data duplicate

                int result = objStaff.UpdateModule(objStaff);
                if (result > 0)
                {
                    ViewBag.SuccessMsg = "Module Updated Successfully!!!";
                }
                else
                { ViewBag.SuccessMsg = "Module Not Updated!!!"; }





                dt = objStaff.getModuleList(objStaff.Id);
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["PlatformName"].ToString()))
                        ViewBag.PlatformName = dt.Rows[0]["PlatformName"].ToString();
                    else
                        ViewBag.PlatformName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Modulename"].ToString()))
                        ViewBag.Modulename = dt.Rows[0]["Modulename"].ToString();
                    else
                        ViewBag.Modulename = "";

                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpGet]
        public ActionResult DeleteModule(int id)
        {
            try
            {
                // int delresult = 0;
                int delresult = objStaff.DeleteModule(id);
                return RedirectToAction("ModuleList");
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
            return RedirectToAction("ModuleList");
        }
        [HttpGet]
        public ActionResult UserValidation()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                DataTable dt = new DataTable();



                Staff objStaff = new Staff();
                DataTable dtStaff = new DataTable();
                dtStaff = objStaff.getStaffList();
                ViewBag.Staff = dtStaff;


                DataTable dtList = new DataTable();

                Staff objValidation = new Staff();
                dtList = objValidation.getStaffUserRole(null);
                ViewBag.ValidationList = dtList;

                dtList = objStaff.getRoleList();
                ViewBag.RoleList = dtList;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        [HttpPost]
        public ActionResult UserValidation(Staff objstaff, FormCollection form)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            string staff = Request["ddlstaff"];
            if (staff != null)
            {
                objstaff.Id = Convert.ToInt32(staff);
                ViewBag.staffname = objstaff.Id;
            }


            //string Platform = Request["ddlplatform"];
            //if (Platform != null)
            //{
            //    objstaff.PlatformName = Platform;
            //    ViewBag.Platformname = objstaff.PlatformName;
            //}

            //string Module = Request["ddlmodule"];
            //if (Module != null)
            //{
            //    objstaff.ModuleId = Convert.ToInt32(Module);
            //    ViewBag.ModuleName = objstaff.ModuleId;
            //}
            objstaff.RoleId =Convert.ToInt32(Request["ddlrole"]);

            string view1 = Request["IsView"].Split(',')[0];
            if (!string.IsNullOrEmpty(view1))
            {
                objstaff.IsView = Convert.ToBoolean(view1);
            }

            string Add1 = Request["IsAdd"].Split(',')[0];
            if (!string.IsNullOrEmpty(Add1))
            {
                objstaff.IsAdd = Convert.ToBoolean(Add1);
            }

            string Edit1 = Request["IsEdit"].Split(',')[0];
            if (!string.IsNullOrEmpty(Edit1))
            {
                objstaff.IsEdit = Convert.ToBoolean(Edit1);
            }

            string Delete1 = Request["IsDelete"].Split(',')[0];
            if (!string.IsNullOrEmpty(Delete1))
            {
                objstaff.IsDelete = Convert.ToBoolean(Delete1);
            }

            string mfund = Request["IsmanageFund"].Split(',')[0];
            if (!string.IsNullOrEmpty(mfund))
            {
                objstaff.IsmanageFund = Convert.ToBoolean(mfund);
            }

            int addresult = objstaff.InsertUserValidation(objstaff);

            if (addresult > 0)
            { ViewBag.SuccessMsg = "User Validation Inserted Successfully!!!"; }
            else
            { ViewBag.SuccessMsg = "User Validation Not Inserted!!!"; }

            ModelState.Clear();
            //    DataTable dtcategory = new DataTable();
            //dtcategory = objProdt.GetAllMaincategory();
            //    ViewBag.Category = dtcategory;

            Staff objStaff = new Staff();
            DataTable dtStaff = new DataTable();
            dtStaff = objStaff.getStaffList();
            ViewBag.Staff = dtStaff;
           

            DataTable dtList = new DataTable();

            Staff objValidation = new Staff();
            dtList = objValidation.getStaffUserRole(null);
            ViewBag.ValidationList = dtList;

            dtList = objStaff.getRoleList();
            ViewBag.RoleList = dtList;

            return View();
        }

        [HttpPost]
        public ActionResult GetPlatformWiseModule(string Platform)
        {
            Staff obj = new Staff();
            DataTable dt = new DataTable();
            dt = obj.BindPlatformWiseModule(Platform);

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(rows);
            return Json(jsonString, JsonRequestBehavior.AllowGet);
            //return Json(rows);
        }


        [HttpGet]
        public ActionResult DeleteUserValidation(int id)
        {
            try
            {
                // int delresult = 0;
                int delresult = objStaff.DeleteModule(id);
                return RedirectToAction("ModuleList");
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
            return RedirectToAction("ModuleList");
        }

        [HttpGet]
        public ActionResult DmDocUpload()
        {
            return View();
        }


        public PartialViewResult FetchModuleList(int id)
        {
            Staff objstaff = new Staff();
            List<SectorViewModel> list = new List<SectorViewModel>();
            var staffmodule = objstaff.GetModuleList();
            if (staffmodule.Rows.Count > 0)
            {
                for (int i = 0; i < staffmodule.Rows.Count; i++)
                {
                    list.Add(new SectorViewModel { ID = staffmodule.Rows[i]["Id"].ToString(), Name = staffmodule.Rows[i]["PlatformName"].ToString() + "/" + staffmodule.Rows[i]["Modulename"].ToString() });
                }
            }
            return PartialView("_ChkSectorList", list);
        }
        public PartialViewResult FetchModuleListByRole(int id)
        {
            Staff objstaff = new Staff();
            List<SectorViewModel> list = new List<SectorViewModel>();
            var staffmodule = objstaff.getModuleListByrole(id);
            if (staffmodule.Rows.Count > 0)
            {
                for (int i = 0; i < staffmodule.Rows.Count; i++)
                {

                    string a = staffmodule.Rows[i]["Id"].ToString();
                    string b = staffmodule.Rows[i]["RoleId"].ToString();
                    list.Add(new SectorViewModel { ID = staffmodule.Rows[i]["Id1"].ToString(), Name = staffmodule.Rows[i]["PlatformName"].ToString() + "/" + staffmodule.Rows[i]["Modulename"].ToString(),RoleId= staffmodule.Rows[i]["RoleId"].ToString() });
                }
            }
            return PartialView("_ChkSectorList1", list);
        }

        [HttpGet]
        public ActionResult EditPlatform(int id = 0)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                DataTable dtList = new DataTable();
                dtList = objStaff.getPlatformList(id);
                ViewBag.PlatformList = dtList;
                if (dtList.Rows.Count > 0)
                {
                    ViewBag.DesignationName = dtList.Rows[0]["DesignationName"].ToString();
                    ViewBag.PlatformId = dtList.Rows[0]["Id"].ToString();
                }

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpPost]
        public ActionResult EditPlatform(Staff objStaff, FormCollection form, string[] chkSector)
        {
            DataTable dtList = new DataTable();
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                DataTable dt = new DataTable();
                // string role = Request["ddlRole"];
                int count = 0, dupcount = 0; ;

                int vendorassignupdate = objStaff.UpdateRoleModulecommon(objStaff.PlatformId);

               // int result = objStaff.UpdateStaff(objStaff);
                if (vendorassignupdate > 0)
                {

                    if (chkSector != null)
                    {
                        foreach (var item in chkSector)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                string item1 = item.ToString();


                                //

                                
                                dtList = objStaff.ChkDuplRoleModule(objStaff.PlatformId, item1);
                                if (dtList.Rows.Count > 0)
                                {

                                    vendorassignupdate = objStaff.UpdateRoleModule(objStaff.PlatformId, item1);

                                    dupcount = dtList.Rows.Count;
                                }
                                else
                                {



                                    int Vendorassign = objStaff.InsertRoleModule(item1, objStaff.PlatformId.ToString());
                                    if (Vendorassign > 0)
                                        count++;
                                }

                                                           }

                        }

                    }

                    ViewBag.SuccessMsg = "Role Module Updated Successfully!!!";
                }
                else
                { ViewBag.SuccessMsg = "Role Module Not Updated!!!"; }


                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                
                dtList = objStaff.getPlatformList(Convert.ToInt32(objStaff.PlatformId));
                ViewBag.PlatformList = dtList;
                if (dtList.Rows.Count > 0)
                {
                    ViewBag.DesignationName = dtList.Rows[0]["DesignationName"].ToString();
                    ViewBag.PlatformId = dtList.Rows[0]["Id"].ToString();
                }

                return View();
            }


           
      
    
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        public ActionResult EditUserValidation(int id = 0)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                Staff objStaff = new Staff();
                DataTable dtList = new DataTable();

                Staff objValidation = new Staff();
                DataTable dtStaff = new DataTable();
                dtStaff = objStaff.getStaffList();
                ViewBag.Staff = dtStaff;


                dtList = objStaff.getRoleList();
                ViewBag.RoleList = dtList;

                dtStaff = objStaff.getStaffUserRole(id);
                if(dtStaff.Rows.Count>0)
                {
                    ViewBag.UservalidationId = dtStaff.Rows[0]["Id"].ToString();
                    ViewBag.StaffId = dtStaff.Rows[0]["StaffId"].ToString();
                    ViewBag.RoleId = dtStaff.Rows[0]["RoleId"].ToString();

                    ViewBag.IsView1 = Convert.ToBoolean(dtStaff.Rows[0]["IsView"]);
                    ViewBag.IsAdd1 = Convert.ToBoolean(dtStaff.Rows[0]["IsAdd"]);
                    ViewBag.IsEdit1 = Convert.ToBoolean(dtStaff.Rows[0]["IsEdit"]);
                    ViewBag.IsDelete1 = Convert.ToBoolean(dtStaff.Rows[0]["IsDelete"]);
                    ViewBag.IsmanageFund = Convert.ToBoolean(dtStaff.Rows[0]["IsmanageFund"]);
                }

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpPost]
        public ActionResult EditUserValidation(Staff objstaff, FormCollection form)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            string staff = Request["ddlstaff"];
            if (staff != null)
            {
                objstaff.Id = Convert.ToInt32(staff);
                ViewBag.staffname = objstaff.Id;
            }


          
            objstaff.RoleId = Convert.ToInt32(Request["ddlrole"]);

            string view1 = Request["IsView1"].Split(',')[0];
            if (!string.IsNullOrEmpty(view1))
            {
                objstaff.IsView = Convert.ToBoolean(view1);
            }

            string Add1 = Request["IsAdd1"].Split(',')[0];
            if (!string.IsNullOrEmpty(Add1))
            {
                objstaff.IsAdd = Convert.ToBoolean(Add1);
            }

            string Edit1 = Request["IsEdit1"].Split(',')[0];
            if (!string.IsNullOrEmpty(Edit1))
            {
                objstaff.IsEdit = Convert.ToBoolean(Edit1);
            }

            string Delete1 = Request["IsDelete1"].Split(',')[0];
            if (!string.IsNullOrEmpty(Delete1))
            {
                objstaff.IsDelete = Convert.ToBoolean(Delete1);
            }

            string mfund = Request["IsmanageFund"].Split(',')[0];
            if (!string.IsNullOrEmpty(mfund))
            {
                objstaff.IsmanageFund = Convert.ToBoolean(mfund);
            }

            int addresult = objstaff.UpdateUserValidation(objstaff);

            if (addresult > 0)
            { ViewBag.SuccessMsg = "User Validation Updation Successfull!!!"; }
            else
            { ViewBag.SuccessMsg = "User Validation Updation Not Successfull!!!"; }

            ModelState.Clear();




            //

            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;

            Staff objStaff = new Staff();
            DataTable dtList = new DataTable();

            Staff objValidation = new Staff();
            DataTable dtStaff = new DataTable();
            dtStaff = objStaff.getStaffList();
            ViewBag.Staff = dtStaff;


            dtList = objStaff.getRoleList();
            ViewBag.RoleList = dtList;

            dtStaff = objStaff.getStaffUserRole(objstaff.UservalidationId);
            if (dtStaff.Rows.Count > 0)
            {
                ViewBag.StaffId = dtStaff.Rows[0]["StaffId"].ToString();
                ViewBag.RoleId = dtStaff.Rows[0]["RoleId"].ToString();

                ViewBag.IsView1 = Convert.ToBoolean(dtStaff.Rows[0]["IsView"]);
                ViewBag.IsAdd1 = Convert.ToBoolean(dtStaff.Rows[0]["IsAdd"]);
                ViewBag.IsEdit1 = Convert.ToBoolean(dtStaff.Rows[0]["IsEdit"]);
                ViewBag.IsDelete1 = Convert.ToBoolean(dtStaff.Rows[0]["IsDelete"]);
                ViewBag.IsmanageFund = Convert.ToBoolean(dtStaff.Rows[0]["IsmanageFund"]);
            }

            return View();
        }

    }
}
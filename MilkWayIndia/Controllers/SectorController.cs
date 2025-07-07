using MilkWayIndia.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MilkWayIndia.Controllers
{
    public class SectorController : Controller
    {
        Sector objsector = new Sector();
        CustomerApiController capi = new CustomerApiController();

        public class StateMas
        {
            public int Statecode { get; set; }
            public string StateName { get; set; }
        }
        public class CityMas
        {
            public int Citycode { get; set; }
            public string CityName { get; set; }
            public int Statecode { get; set; }
        }
        // GET: Sector
        [HttpGet]
        public ActionResult AddSector()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                //int appnotification = capi.AppNotification1();
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;


                DataTable dtList = new DataTable();
                dtList = objsector.getStateList(null);
                ViewBag.StateList = dtList;

                 
                //dtList = objsector.getCityList(null);
                //ViewBag.CityList = dtList;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult GetCity(int STATECODE)
        {
            List<CityMas> lstDesig = PopulateDesignation().Where(s => s.Statecode.Equals(STATECODE)).ToList();
            return Json(lstDesig, JsonRequestBehavior.AllowGet);
        }
        public List<CityMas> PopulateDesignation()
        {

            DataTable dtList = new DataTable();
            dtList = objsector.getCityList(null);

            List<CityMas> lstCity = new List<CityMas>();
            CityMas objDesig;
            

            if (dtList.Rows.Count > 0)
            {
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    objDesig = new CityMas();
                    objDesig.Statecode =Convert.ToInt32(dtList.Rows[i].ItemArray[2]);
                    objDesig.CityName = dtList.Rows[i].ItemArray[1].ToString();
                    objDesig.Citycode = Convert.ToInt32(dtList.Rows[i].ItemArray[0].ToString());
                    lstCity.Add(objDesig);
                }
            }

            else
            {
                objDesig = new CityMas();
                objDesig.Statecode = 0;
                objDesig.CityName = "No data Found";
                objDesig.Citycode = 0;
                lstCity.Add(objDesig);
            }
            
            

           

            return lstCity;
        }

        public JsonResult GetSector(int? Stateid, int? CityId)
        {
            Vendor objvendor = new Vendor();
            string code = string.Empty;
            StringBuilder sb = new StringBuilder();
            var dtsector = objvendor.GetSectorListByCity(CityId);
            if (dtsector.Rows.Count > 0)
            {
                sb.Append("<option value='0' >---Select Sector---</option>");
                for (int i = 0; i < dtsector.Rows.Count; i++)
                {
                    sb.Append("<option value='" + dtsector.Rows[i]["Id"] + "' >" + dtsector.Rows[i]["SectorName"] + " </option> ");
                }
            }

            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddSector(Sector objsector,FormCollection form)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                int addresult = 0;
                string state = Request["ddlState"];
                string City = Request["ddlCity"];
                objsector.StateId =Convert.ToInt32(state);
                objsector.CityId =Convert.ToInt32(City);
                //chcek duplicate value
                DataTable dtDuplicateSec = objsector.getCheckDuplSector(objsector.SectorName);
                if (dtDuplicateSec.Rows.Count > 0)
                {
                    ViewBag.SuccessMsg = "Sector Name Already Exits!!!";
                }
                else
                {
                    addresult = objsector.InsertSector(objsector);
                    if (addresult > 0)
                    { ViewBag.SuccessMsg = "Sector Inserted Successfully!!!"; }
                    else
                    { ViewBag.SuccessMsg = "Sector Not Inserted!!!"; }
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
        public ActionResult EditSector(int Id = 0)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                DataTable dt = new DataTable();
                dt = objsector.getSectorList(Id);
                if (dt.Rows.Count > 0)
                {
                   // ViewBag.SectorId = dt.Rows[0]["Id"].ToString();
                    if (!string.IsNullOrEmpty(dt.Rows[0]["SectorName"].ToString()))
                        ViewBag.SectorName = dt.Rows[0]["SectorName"].ToString();
                    else
                        ViewBag.SectorName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["LandMark"].ToString()))
                        ViewBag.LandMark = dt.Rows[0]["LandMark"].ToString();
                    else
                        ViewBag.LandMark = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["PinCode"].ToString()))
                        ViewBag.PinCode = dt.Rows[0]["PinCode"].ToString();
                    else
                        ViewBag.PinCode = "";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditSector(Sector objsector, FormCollection form)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                DataTable dtDuplicateSec = objsector.getCheckDuplSector(objsector.SectorName);
                if (dtDuplicateSec.Rows.Count > 0)
                {
                    int SId = Convert.ToInt32(dtDuplicateSec.Rows[0]["Id"]);
                    if (SId == objsector.Id)
                    {
                        int result = objsector.UpdateSector(objsector);
                        if (result > 0)
                        {
                            ViewBag.SuccessMsg = "Sector Updated Successfully!!!";
                        }
                        else
                        {
                            ViewBag.SuccessMsg = "Sector Not Updated!!!";
                        }
                    }
                    else
                    {
                        ViewBag.SuccessMsg = "Sector Name Already Exits!!!";
                    }
                }
                else
                {
                    int result = objsector.UpdateSector(objsector);
                    if (result > 0)
                    {
                        ViewBag.SuccessMsg = "Sector Updated Successfully!!!";
                    }
                    else
                    {
                        ViewBag.SuccessMsg = "Sector Not Updated!!!";
                    }
                }
                DataTable dt = objsector.getSectorList(objsector.Id);
                if (dt.Rows.Count > 0)
                {
                    //ViewBag.SectorId = dt.Rows[0]["Id"].ToString();
                    if (!string.IsNullOrEmpty(dt.Rows[0]["SectorName"].ToString()))
                        ViewBag.SectorName = dt.Rows[0]["SectorName"].ToString();
                    else
                        ViewBag.SectorName = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["LandMark"].ToString()))
                        ViewBag.LandMark = dt.Rows[0]["LandMark"].ToString();
                    else
                        ViewBag.LandMark = "";
                    if (!string.IsNullOrEmpty(dt.Rows[0]["PinCode"].ToString()))
                        ViewBag.PinCode = dt.Rows[0]["PinCode"].ToString();
                    else
                        ViewBag.PinCode = "";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult SectorList()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;                

                DataTable dtList = new DataTable();
                dtList = objsector.getSectorList(null);
                ViewBag.SectorList = dtList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult DeleteSector(int id)
        {
            try
            {
                int delresult = objsector.DeleteSector(id);
                return RedirectToAction("SectorList");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_tbl_building_master_tbl_sector_master"))
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
            return RedirectToAction("SectorList");
        }
        //Building
        [HttpGet]
        public ActionResult AddBuilding()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                DataTable dt = new DataTable();
                dt = objsector.getSectorList(null);
                ViewBag.Sector = dt;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddBuilding(Sector objsector, FormCollection form)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                int addresult = 0;
                string sector = Request["ddlSector"];
                if (!string.IsNullOrEmpty(sector))
                {
                    objsector.SectorId = Convert.ToInt32(sector);
                }
                DataTable dtDuplicateSec = objsector.getCheckDuplBuilding(objsector.SectorId,objsector.BuildingName,objsector.BlockNo);
                if (dtDuplicateSec.Rows.Count > 0)
                {
                    ViewBag.SuccessMsg = "Building Name Already Exits!!!";
                }
                else
                {
                    addresult = objsector.InsertBuilding(objsector);
                    if (addresult > 0)
                    {
                        ViewBag.SuccessMsg = "Building Inserted Successfully!!!";
                    }
                    else
                    { ViewBag.SuccessMsg = "Building Not Inserted!!!"; }
                }
                ModelState.Clear();
                DataTable dt = new DataTable();
                dt = objsector.getSectorList(null);
                ViewBag.Sector = dt;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult EditBuilding(int id = 0)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                DataTable dt = new DataTable();
                dt = objsector.getSectorList(null);
                ViewBag.Sector = dt;

                DataTable dtgetdata = new DataTable();
                dtgetdata = objsector.getBuildingList(id);
                if (dtgetdata.Rows.Count > 0)
                {
                    //ViewBag.BuildingId = dtgetdata.Rows[0]["Id"].ToString();
                    if (!string.IsNullOrEmpty(dtgetdata.Rows[0]["SectorId"].ToString()))
                        ViewBag.SectorId = dtgetdata.Rows[0]["SectorId"].ToString();
                    else
                        ViewBag.SectorId = "";
                    if (!string.IsNullOrEmpty(dtgetdata.Rows[0]["BuildingName"].ToString()))
                        ViewBag.BuildingName = dtgetdata.Rows[0]["BuildingName"].ToString();
                    else
                        ViewBag.BuildingName = "";
                    if (!string.IsNullOrEmpty(dtgetdata.Rows[0]["BlockNo"].ToString()))
                        ViewBag.BlockNo = dtgetdata.Rows[0]["BlockNo"].ToString();
                    else
                        ViewBag.BlockNo = "";
                    if (!string.IsNullOrEmpty(dtgetdata.Rows[0]["FlatNo"].ToString()))
                        ViewBag.FlatNo = dtgetdata.Rows[0]["FlatNo"].ToString();
                    else
                        ViewBag.FlatNo = "";
                    if (!string.IsNullOrEmpty(dtgetdata.Rows[0]["orderBy"].ToString()))
                        ViewBag.orderBy = dtgetdata.Rows[0]["orderBy"].ToString();
                    else
                        ViewBag.orderBy = "";

                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditBuilding(Sector objsector, FormCollection form)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                string sector = Request["ddlSector"];
                if (!string.IsNullOrEmpty(sector))
                {
                    objsector.SectorId = Convert.ToInt32(sector);
                }
                DataTable dtDuplicateSec = objsector.getCheckDuplBuilding(objsector.SectorId, objsector.BuildingName, objsector.BlockNo);
                if (dtDuplicateSec.Rows.Count > 0)
                {
                    int BId = Convert.ToInt32(dtDuplicateSec.Rows[0]["Id"]);
                    if (BId == objsector.Id)
                    {
                        int result = objsector.UpdateBuilding(objsector);
                        if (result > 0)
                        { ViewBag.SuccessMsg = "Building Updated Successfully!!!"; }
                        else
                        { ViewBag.SuccessMsg = "Building Not Updated!!!"; }
                    }
                    else
                    {
                        ViewBag.SuccessMsg = "Building Name Already Exits!!!";
                    }
                }
                else
                {
                    int result = objsector.UpdateBuilding(objsector);
                    if (result > 0)
                    { ViewBag.SuccessMsg = "Building Updated Successfully!!!"; }
                    else
                    { ViewBag.SuccessMsg = "Building Not Updated!!!"; }
                }
                DataTable dt = new DataTable();
                dt = objsector.getSectorList(null);
                ViewBag.Sector = dt;

                DataTable dtgetdata = new DataTable();
                dtgetdata = objsector.getBuildingList(objsector.Id);
                if (dtgetdata.Rows.Count > 0)
                {
                    //ViewBag.BuildingId = dtgetdata.Rows[0]["Id"].ToString();
                    if (!string.IsNullOrEmpty(dtgetdata.Rows[0]["SectorId"].ToString()))
                        ViewBag.SectorId = dtgetdata.Rows[0]["SectorId"].ToString();
                    else
                        ViewBag.SectorId = "";
                    if (!string.IsNullOrEmpty(dtgetdata.Rows[0]["BuildingName"].ToString()))
                        ViewBag.BuildingName = dtgetdata.Rows[0]["BuildingName"].ToString();
                    else
                        ViewBag.BuildingName = "";
                    if (!string.IsNullOrEmpty(dtgetdata.Rows[0]["BlockNo"].ToString()))
                        ViewBag.BlockNo = dtgetdata.Rows[0]["BlockNo"].ToString();
                    else
                        ViewBag.BlockNo = "";
                    if (!string.IsNullOrEmpty(dtgetdata.Rows[0]["orderBy"].ToString()))
                        ViewBag.orderBy = dtgetdata.Rows[0]["orderBy"].ToString();
                    else
                        ViewBag.orderBy = "";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult BuildingList()
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
                dtList = objsector.getBuildingList(null);
                ViewBag.BuildingList = dtList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult GetBuildingList(int id)
        {
            DataTable dt = new DataTable();
            dt = objsector.geSectorwisetBuildingList(id);

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
        public ActionResult DeleteBuilding(int id)
        {
            try
            {
                int delresult = objsector.DeleteBuilding(id);
                return RedirectToAction("BuildingList");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_buildingmaster_flatmaster")|| ex.Message.ToLower().Contains("fk_customer_buildingmaster") || ex.Message.ToLower().Contains("fk_order_buildingmaster"))
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
            return RedirectToAction("BuildingList");
        }

        //Flat No
        [HttpGet]
        public ActionResult AddFlatNo()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                Sector objsec = new Sector();
                DataTable dt = new DataTable();
                dt = objsec.getBuildingList(null);
                ViewBag.Building = dt;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddFlatNo(Sector objsector, FormCollection form)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                string building = Request["ddlBuilding"];
                if (!string.IsNullOrEmpty(building))
                {
                    objsector.BuildingId = Convert.ToInt32(building);
                }
                //chcek duplicate no
                DataTable dtDupliFlat = objsector.getCheckDupliFlatNo(objsector.BuildingId,objsector.FlatNo);
                if (dtDupliFlat.Rows.Count > 0)
                {
                    ViewBag.SuccessMsg = "FlatNO Already Exits!!!";
                }
                else
                {
                   // int addresult = 0;
                    int addresult = objsector.InsertFlatNo(objsector);
                    if (addresult > 0)
                    { ViewBag.SuccessMsg = "FlatNO Inserted Successfully!!!"; }
                    else
                    { ViewBag.SuccessMsg = "FlatNO Not Inserted!!!"; }
                }
                ModelState.Clear();
                Sector objsec = new Sector();
                DataTable dt = new DataTable();
                dt = objsec.getBuildingList(null);
                ViewBag.Building = dt;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        [HttpGet]
        public ActionResult EditFlatNo(int id = 0)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                Sector objsec = new Sector();
                DataTable dt = new DataTable();
                dt = objsec.getBuildingList(null);
                ViewBag.Building = dt;

                DataTable dtedit = objsec.getFlatNoList(id);
                if (dtedit.Rows.Count > 0)
                {
                   // ViewBag.FlatNoId = dtedit.Rows[0]["Id"].ToString();
                    if (!string.IsNullOrEmpty(dtedit.Rows[0]["BuildingId"].ToString()))
                        ViewBag.BuildingId = dtedit.Rows[0]["BuildingId"].ToString();
                    else
                        ViewBag.BuildingId = "";
                    if (!string.IsNullOrEmpty(dtedit.Rows[0]["FlatNo"].ToString()))
                        ViewBag.FlatNo = dtedit.Rows[0]["FlatNo"].ToString();
                    else
                        ViewBag.FlatNo = "";
                    if (!string.IsNullOrEmpty(dtedit.Rows[0]["orderBy"].ToString()))
                        ViewBag.orderBy = dtedit.Rows[0]["orderBy"].ToString();
                    else
                        ViewBag.orderBy = "";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditFlatNo(Sector objsector, FormCollection form)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                string building = Request["ddlBuilding"];
                if (!string.IsNullOrEmpty(building))
                {
                    objsector.BuildingId = Convert.ToInt32(building);
                }
                //chcek duplicate no
                DataTable dtDupliFlat = objsector.getCheckDupliFlatNo(objsector.BuildingId, objsector.FlatNo);
                if (dtDupliFlat.Rows.Count > 0)
                {
                    int FId = Convert.ToInt32(dtDupliFlat.Rows[0]["Id"]);
                    if (FId == objsector.Id)
                    {
                        int addresult = objsector.UpdateFlatNo(objsector);
                        if (addresult > 0)
                        {
                            ViewBag.SuccessMsg = "FlatNO Updated Successfully!!!";
                        }
                        else
                        { ViewBag.SuccessMsg = "FlatNO Not Updated!!!"; }
                    }
                    else
                        ViewBag.SuccessMsg = "FlatNO Already Exits!!!";
                }
                else
                {
                    int addresult = objsector.UpdateFlatNo(objsector);
                    if (addresult > 0)
                    {
                        ViewBag.SuccessMsg = "FlatNO Updated Successfully!!!";
                    }
                    else
                    { ViewBag.SuccessMsg = "FlatNO Not Updated!!!"; }
                }
                DataTable dtbuild = new DataTable();
                dtbuild = objsector.getBuildingList(null);
                ViewBag.Building = dtbuild;

                DataTable dt = objsector.getFlatNoList(objsector.Id);
                if (!string.IsNullOrEmpty(dt.Rows[0]["BuildingId"].ToString()))
                    ViewBag.BuildingId = dt.Rows[0]["BuildingId"].ToString();
                else
                    ViewBag.BuildingId = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["FlatNo"].ToString()))
                    ViewBag.FlatNo = dt.Rows[0]["FlatNo"].ToString();
                else
                    ViewBag.FlatNo = "";
                if (!string.IsNullOrEmpty(dt.Rows[0]["orderBy"].ToString()))
                    ViewBag.orderBy = dt.Rows[0]["orderBy"].ToString();
                else
                    ViewBag.orderBy = "";
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        [HttpGet]
        public ActionResult FlatNoList()
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
                dtList = objsector.getFlatNoList(null);
                ViewBag.FlatNoList = dtList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult GetFlatNoList(int id)
        {
            DataTable dt = new DataTable();
            dt = objsector.getBuildingwiseFlatNoList(id);

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
        public ActionResult DeleteFlatNo(int id)
        {
            try
            {
                int delresult = objsector.DeleteFlatNo(id);
                return RedirectToAction("FlatNoList");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.ToLower().Contains("fk_customer_flatmaster"))
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
            return RedirectToAction("FlatNoList");
        }

        [HttpPost]
        public ActionResult GetBuildingCustomerList(int id)
        {
            DataTable dt = new DataTable();
            dt = objsector.getBuildingWiseCustomerList(id);

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
        public ActionResult StateList()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                DataTable dtList = new DataTable();
                dtList = objsector.getStateList(null);
                ViewBag.SectorList = dtList;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpGet]
        public ActionResult AddState()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                //int appnotification = capi.AppNotification1();
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
        public ActionResult AddState(Sector objsector, FormCollection form)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                int addresult = 0;
                //chcek duplicate value
                DataTable dtDuplicateState = objsector.getCheckDuplState(objsector.Statename);
                if (dtDuplicateState.Rows.Count > 0)
                {
                    ViewBag.SuccessMsg = "State Name Already Exits!!!";
                }
                else
                {
                    addresult = objsector.InsertState(objsector);
                    if (addresult > 0)
                    { ViewBag.SuccessMsg = "State Inserted Successfully!!!"; }
                    else
                    { ViewBag.SuccessMsg = "State Not Inserted!!!"; }
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
        public ActionResult EditState(int Id = 0)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                DataTable dt = new DataTable();
                dt = objsector.getStateList(Id);
                if (dt.Rows.Count > 0)
                {
                    // ViewBag.SectorId = dt.Rows[0]["Id"].ToString();
                    if (!string.IsNullOrEmpty(dt.Rows[0]["statename"].ToString()))
                        ViewBag.StateName = dt.Rows[0]["statename"].ToString();
                    else
                        ViewBag.StateName = "";
                   
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditState(Sector objsector, FormCollection form)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                DataTable dtDuplicateSec = objsector.getCheckDuplState(objsector.Statename);
                if (dtDuplicateSec.Rows.Count > 0)
                {
                    int SId = Convert.ToInt32(dtDuplicateSec.Rows[0]["Id"]);
                    if (SId == objsector.Id)
                    {
                        int result = objsector.UpdateState(objsector);
                        if (result > 0)
                        {
                            ViewBag.SuccessMsg = "State Updated Successfully!!!";
                        }
                        else
                        {
                            ViewBag.SuccessMsg = "State Not Updated!!!";
                        }
                    }
                    else
                    {
                        ViewBag.SuccessMsg = "State Name Already Exits!!!";
                    }
                }
                else
                {
                    int result = objsector.UpdateState(objsector);
                    if (result > 0)
                    {
                        ViewBag.SuccessMsg = "State Updated Successfully!!!";
                    }
                    else
                    {
                        ViewBag.SuccessMsg = "State Not Updated!!!";
                    }
                }
                DataTable dt = objsector.getStateList(objsector.Id);
                if (dt.Rows.Count > 0)
                {
                    //ViewBag.SectorId = dt.Rows[0]["Id"].ToString();
                    if (!string.IsNullOrEmpty(dt.Rows[0]["statename"].ToString()))
                        ViewBag.StateName = dt.Rows[0]["statename"].ToString();
                    else
                        ViewBag.StateName = "";
                    
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }



        [HttpGet]
        public ActionResult DeleteState(int id)
        {
            try
            {
                int delresult = objsector.DeleteState(id);
                return RedirectToAction("StateList");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
               
            }
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("StateList");
        }




        [HttpGet]
        public ActionResult CityList()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                DataTable dtList = new DataTable();
                dtList = objsector.getCityList(null);
                ViewBag.SectorList = dtList;

                DataTable dt = new DataTable();
                dt = objsector.getStateList(null);
                ViewBag.State = dt;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }


        [HttpGet]
        public ActionResult AddCity()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                

                DataTable dt = new DataTable();
                dt = objsector.getStateList(null);
                ViewBag.State = dt;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }



        [HttpPost]
        public ActionResult AddCity(Sector objsector, FormCollection form)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                int addresult = 0;
                //chcek duplicate value
                string state = Request["ddlState"];

                if (!string.IsNullOrEmpty(state))
                {
                    objsector.Statename = state;
                }
                ViewBag.StateName = objsector.Statename;

                DataTable dtDuplicateState = objsector.getCheckDuplCity(objsector.Statename,objsector.Cityname);
                if (dtDuplicateState.Rows.Count > 0)
                {
                    ViewBag.SuccessMsg = "City Name Already Exits!!!";
                    DataTable dt = new DataTable();
                    dt = objsector.getStateList(null);
                    ViewBag.State = dt;
                }
                else
                {
                    addresult = objsector.InsertCity(objsector);
                    if (addresult > 0)
                    { ViewBag.SuccessMsg = "City Inserted Successfully!!!"; }
                    else
                    { ViewBag.SuccessMsg = "City Not Inserted!!!"; }

                    DataTable dt = new DataTable();
                    dt = objsector.getStateList(null);
                    ViewBag.State = dt;
                    
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
        public ActionResult EditCity(int Id = 0)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                DataTable dt = new DataTable();
                dt = objsector.getCityList(Id);
                if (dt.Rows.Count > 0)
                {
                    // ViewBag.SectorId = dt.Rows[0]["Id"].ToString();
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Cityname"].ToString()))
                        ViewBag.CityName = dt.Rows[0]["Cityname"].ToString();
                    else
                        ViewBag.CityName = "";


                    if (!string.IsNullOrEmpty(dt.Rows[0]["state"].ToString()))
                        ViewBag.StateName = dt.Rows[0]["state"].ToString();
                    else
                        ViewBag.StateName = "";

                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditCity(Sector objsector, FormCollection form)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {



                DataTable dtDuplicateSec = objsector.getCheckDuplCity(objsector.Statename,objsector.Cityname);
                if (dtDuplicateSec.Rows.Count > 0)
                {
                    int SId = Convert.ToInt32(dtDuplicateSec.Rows[0]["Id"]);
                    if (SId == objsector.Id)
                    {
                        int result = objsector.UpdateCity(objsector);
                        if (result > 0)
                        {
                            ViewBag.SuccessMsg = "City Updated Successfully!!!";
                        }
                        else
                        {
                            ViewBag.SuccessMsg = "City Not Updated!!!";
                        }
                    }
                    else
                    {
                        ViewBag.SuccessMsg = "City Name Already Exits!!!";
                    }
                }
                else
                {
                    int result = objsector.UpdateCity(objsector);
                    if (result > 0)
                    {
                        ViewBag.SuccessMsg = "City Updated Successfully!!!";
                    }
                    else
                    {
                        ViewBag.SuccessMsg = "City Not Updated!!!";
                    }
                }
                DataTable dt = objsector.getCityList(objsector.Id);
                if (dt.Rows.Count > 0)
                {
                    // ViewBag.SectorId = dt.Rows[0]["Id"].ToString();
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Cityname"].ToString()))
                        ViewBag.CityName = dt.Rows[0]["Cityname"].ToString();
                    else
                        ViewBag.CityName = "";


                    if (!string.IsNullOrEmpty(dt.Rows[0]["state"].ToString()))
                        ViewBag.StateName = dt.Rows[0]["state"].ToString();
                    else
                        ViewBag.StateName = "";

                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }



        [HttpGet]
        public ActionResult DeleteCity(int id)
        {
            try
            {
                int delresult = objsector.DeleteCity(id);
                return RedirectToAction("CityList");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {

            }
            catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("CityList");
        }
    }
}
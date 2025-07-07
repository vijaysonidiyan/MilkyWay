using MilkWayIndia.Abstract;
using MilkWayIndia.Concrete;
using MilkWayIndia.Entity;
using MilkWayIndia.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MilkWayIndia.Controllers
{
    public class SliderController : Controller
    {
        Product objProdt = new Product();
        Sector _sector = new Sector();
        Vendor objvendor = new Vendor();
        Helper dHelper = new Helper();
        private ISlider _SliderRepo;
        public SliderController()
        {
            this._SliderRepo = new SliderRepository();
        }
        // GET: Slider
        public ActionResult Index()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");
            PopulateGrid();
            return View();
        }

        public void PopulateGrid()
        {
            var slider = _SliderRepo.GetAllSlider();
            ViewBag.lstSlider = slider;
        }

        public void PopulateDrp()
        {
            var lstCategory = dHelper.GetCategoryList();
            ViewBag.lstCategory = new SelectList(lstCategory, "Value", "Text");

            var sectorList = _sector.getSectorList(null);
            List<SelectListItem> lstSector = new List<SelectListItem>();
            if (sectorList.Rows.Count > 0)
            {
                for (int i = 0; i < sectorList.Rows.Count; i++)
                {
                    lstSector.Add(new SelectListItem { Text = sectorList.Rows[i]["Sectorname"].ToString(), Value = sectorList.Rows[i]["Id"].ToString() });
                }
            }
            ViewBag.lstSector = new SelectList(lstSector, "Value", "Text");
        }

        public ActionResult Create()
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            PopulateDrp();
            ViewBag.IsAdmin = control.IsAdmin;
            ViewBag.IsView = control.IsView;
            ViewBag.IsAdd = control.IsAdd;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(tblSliders model, HttpPostedFileBase Document1, string[] chkSector)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            if (chkSector == null)
            {
                ViewBag.SuccessMsg = "Please select sector...";
                return View(model);
            }
            PopulateDrp();
            model.CreatedOn = Helper.indianTime;
            var response = InsertSlider(model, Document1, chkSector);
            if (response.ID > 0)
                ViewBag.SuccessMsg = "Banner Inserted Successfully!!!";
            else
                ViewBag.SuccessMsg = "Banner Not Inserted!!!";
            return View();
        }

        public ActionResult Edit(int? ID)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            var control = Helper.CheckPermission(Request.RawUrl.ToString());
            if (control.IsView == false)
                return Redirect("/notaccess/index");

            PopulateDrp();
            var slider = _SliderRepo.GetSliderByID(ID);
            return View(slider);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(tblSliders model, HttpPostedFileBase Document1, string[] chkSector)
        {
            if (Request.Cookies["gstusr"] == null)
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

            PopulateDrp();
            var response = InsertSlider(model, Document1, chkSector);
            if (response.ID > 0)
                ViewBag.SuccessMsg = "Banner Updated Successfully!!!";
            else
                ViewBag.SuccessMsg = "Banner Not Inserted!!!";
            return View();
        }

        [ValidateInput(false)]
        public tblSliders InsertSlider(tblSliders model, HttpPostedFileBase Document1, string[] chkSector)
        {
            if (Document1 != null)
            {
                string path = Server.MapPath("~/Image/");
                string extension = Path.GetExtension(Document1.FileName);
                string filename = Guid.NewGuid() + extension;
                var filePath = path + filename;
                Document1.SaveAs(filePath);
                model.PhotoPath = filename;
                model.FileName = Document1.FileName;
            }
            var response = _SliderRepo.SaveSlider(model, chkSector);
            return response;
        }

        public ActionResult Delete(int ID)
        {
            _SliderRepo.DeleteSlider(ID);
            return Redirect("/slider/index");
        }

        public PartialViewResult FetchSectorList(string id)
        {
            List<SectorViewModel> list = new List<SectorViewModel>();
            var sector = objvendor.GetSectorListByVendor(0);
            if (sector.Rows.Count > 0)
            {
                for (int i = 0; i < sector.Rows.Count; i++)
                {
                    int SliderID = 0;
                    if (!string.IsNullOrEmpty(id))                    
                        SliderID = Convert.ToInt32(id);
                    
                    var checkSector = _SliderRepo.CheckSliderSector(SliderID, Convert.ToInt32(sector.Rows[i]["Id"]));
                    if (checkSector == true)
                        list.Add(new SectorViewModel { ID = sector.Rows[i]["Id"].ToString(), Name = sector.Rows[i]["SectorName"].ToString(), IsSelected = "checked" });
                    else
                        list.Add(new SectorViewModel { ID = sector.Rows[i]["Id"].ToString(), Name = sector.Rows[i]["SectorName"].ToString() });
                }
            }
            return PartialView("_ChkSectorList", list);
        }
    }
}
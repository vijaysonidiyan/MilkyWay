using MilkWayIndia.Abstract;
using MilkWayIndia.Concrete;
using MilkWayIndia.Entity;
using MilkWayIndia.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MilkWayIndia.Controllers
{
    public class AdvertisementController : Controller
    {
        Helper dHelper = new Helper();
        Dictionary<string, object> res = new Dictionary<string, object>();
        private IAdvertisement _AdvertisementRepo;

        public AdvertisementController()
        {
            this._AdvertisementRepo = new AdvertisementRepository();
        }
        // GET: Advertisement
        public ActionResult Index()
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                ViewBag.IsAdmin = control.IsAdmin;
                ViewBag.IsView = control.IsView;
                ViewBag.IsAdd = control.IsAdd;

                var advertisement = PopulateGrid();
                return View(advertisement);
            }
            else
            {
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            }
        }

        public List<tbl_Advertisement> PopulateGrid()
        {
            var advertisement = _AdvertisementRepo.GetAllAdvertisement();
            return advertisement;
        }

        public ActionResult Create()
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
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
        }

        public ActionResult Edit(int? ID)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var control = Helper.CheckPermission(Request.RawUrl.ToString());
                if (control.IsView == false)
                    return Redirect("/notaccess/index");

                AdvertisementModel model = new AdvertisementModel();
                var advertisement = _AdvertisementRepo.GetAdvertisementID(ID);
                if (advertisement != null)
                {                    
                    model.ID = advertisement.ID;
                    model.AdsType = advertisement.AdsType;
                    model.Title = advertisement.Title;
                    model.Mobile = advertisement.Mobile;
                    model.StartDate= advertisement.StartDate == null ? "" : advertisement.StartDate.Value.ToString("yyyy-MM-dd");
                    model.ExpiredDate= advertisement.ExpiredDate == null ? "" : advertisement.ExpiredDate.Value.ToString("yyyy-MM-dd");
                    model.Description = advertisement.Description;
                    model.WebsiteLink = advertisement.WebsiteLink;
                    model.AppLink = advertisement.AppLink;
                    model.PhotoPath = advertisement.PhotoPath;               
                    ViewBag.PhotoPath = advertisement.PhotoPath;                    
                }
                return View(model);
            }
            else
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(AdvertisementModel model, HttpPostedFileBase photo)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {                
                var response = InsertUpdate(model, photo);
                if (response.ID > 0)
                    ViewBag.SuccessMsg = "Advertisement Inserted Successfully!!!";
                else
                    ViewBag.SuccessMsg = "Advertisement Not Inserted!!!";
            }
            else
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(AdvertisementModel model, HttpPostedFileBase photo)
        {
            if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
            {
                var response = InsertUpdate(model, photo);
                if (response.ID > 0)
                    ViewBag.SuccessMsg = "Advertisement Updated Successfully!!!";
                else
                    ViewBag.SuccessMsg = "Advertisement Not Inserted!!!";
            }
            else
                return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
            return View();
        }

        [ValidateInput(false)]
        public tbl_Advertisement InsertUpdate(AdvertisementModel model, HttpPostedFileBase photo)
        {
            tbl_Advertisement _ads = new tbl_Advertisement();
            if (photo != null)
            {
                string path = Server.MapPath("~/Image/");
                string extension = Path.GetExtension(photo.FileName);
                string filename = Guid.NewGuid() + extension;
                var filePath = path + filename;
                photo.SaveAs(filePath);
                model.PhotoPath = "/image/" + filename;
                _ads.PhotoPath = model.PhotoPath;
            }
            else
                _ads.PhotoPath = model.PhotoPath;
            _ads.ID = model.ID;
            _ads.AdsType = model.AdsType;
            _ads.Title = model.Title;
            _ads.Mobile = model.Mobile;
            _ads.Description = model.Description;
            _ads.WebsiteLink = model.WebsiteLink;
            _ads.AppLink = model.AppLink;
            _ads.IsDeleted = false;
            if (!string.IsNullOrEmpty(model.StartDate))
                _ads.StartDate = DateTime.ParseExact(model.StartDate, "yyyy-MM-dd", null);
            if (!string.IsNullOrEmpty(model.ExpiredDate))
                _ads.ExpiredDate = DateTime.ParseExact(model.ExpiredDate, "yyyy-MM-dd", null);
            ViewBag.PhotoPath = model.PhotoPath;
            var response = _AdvertisementRepo.SaveAdvertisement(_ads);
            return response;
        }

        public ActionResult Delete(int ID)
        {
            _AdvertisementRepo.DeleteAdvertisement(ID);
            return Redirect("/advertisement/index");
        }
    }
}
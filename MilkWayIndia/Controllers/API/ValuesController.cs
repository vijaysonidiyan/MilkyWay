using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MilkWayIndia.Models;
using MilkWayIndia.Concrete;
using MilkWayIndia.Abstract;
using System.Web;
using System.IO;
using MilkWayIndia.Entity;

namespace MilkWayIndia.Controllers.API
{
    public class ValuesController : ApiController
    {
        private ISlider _BannerRepo;
        private IMedicine _MedicineRepo;
        private IAdvertisement _AdvertisementRepo;
        public ValuesController()
        {
            this._BannerRepo = new SliderRepository();
            this._MedicineRepo = new MedicineRepository();
            this._AdvertisementRepo = new AdvertisementRepository();
        }

        #region Get All Banners
        [Route("api/gethomebanner"), HttpGet]
        public HttpResponseMessage GetHomeBanner()
        {
            BannerResponse response = new BannerResponse();
            response.status = "400";
            try
            {
                var banner = _BannerRepo.GetAllSlider();
                response.banners = banner;
                response.status = "200";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                response.msg = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }
        #endregion

        #region Get Sectorwise Banner
        [Route("api/getbannerbysector/{sectorId?}"), HttpGet]
        public HttpResponseMessage GetBannerSector(int sectorId = 0)
        {
            BannerResponse response = new BannerResponse();
            response.status = "400";
            try
            {
                if (sectorId > 0)
                {
                    var banner = _BannerRepo.GetBannerBySector(sectorId);
                    response.banners = banner;
                }
                else
                {
                    var banner = _BannerRepo.GetAllSlider();
                    response.banners = banner;
                }

                response.status = "200";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                response.msg = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }
        #endregion

        #region Get Banner Detail By ID
        [Route("api/getbannerbyid/{bannerid?}"), HttpGet]
        public HttpResponseMessage GetBannerById(int bannerId)
        {
            BannerResponse response = new BannerResponse();
            response.status = "400";
            try
            {
                var banner = _BannerRepo.GetAllSlider();
                if (bannerId > 0)
                    banner = banner.Where(s => s.id == bannerId).OrderBy(s => s.sortorder).ToList();
                response.banners = banner;
                response.status = "200";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                response.msg = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }
        #endregion

        #region Get Advertisement By Type
        [Route("api/getadvertisement/{adstype?}"), HttpGet]
        public HttpResponseMessage GetAdvertisement(int adstype = 0)
        {
            AdsResponse response = new AdsResponse();
            response.status = "400";
            try
            {
                var ads = _AdvertisementRepo.GetAllAdvertisement().Select(s => new AdsViewModel
                {
                    id = s.ID,
                    adstype = s.AdsType,
                    title = s.Title,
                    mobile = s.Mobile,
                    websitelink = s.WebsiteLink,
                    applink = s.AppLink,
                    description = s.Description,
                    startdate = s.StartDate,
                    expireddate = s.ExpiredDate,
                    image = Helper.PhotoFolderPath + "/Image/" + s.PhotoPath
                }).ToList();
                if (adstype > 0)
                    ads = ads.Where(s => s.adstype == adstype).ToList();
                response.list = ads;
                response.status = "200";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                response.msg = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }
        #endregion

        #region Insert Customer Click
        [Route("api/insercustomerads/{CustomerId?}/{AdvertisementId?}"), HttpPost]
        public HttpResponseMessage InsertCustomerAds(int CustomerId, int AdvertisementId)
        {
            AdsResponse response = new AdsResponse();
            response.status = "400";
            try
            {
                _AdvertisementRepo.InsertCustomerAds(CustomerId, AdvertisementId);
                response.status = "200";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                response.msg = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }
        #endregion

        #region Get Advertisement Detail
        [Route("api/getadvertisementbyid/{AdvertisementId?}"), HttpGet]
        public HttpResponseMessage GetAdvertisementbyID(int AdvertisementId)
        {
            AdsResponse response = new AdsResponse();
            response.status = "400";
            try
            {
                var ads = _AdvertisementRepo.GetAllAdvertisement().Select(s => new AdsViewModel
                {
                    id = s.ID,
                    title = s.Title,
                    mobile = s.Mobile,
                    websitelink = s.WebsiteLink,
                    applink = s.AppLink,
                    description = s.Description,
                    startdate = s.StartDate,
                    expireddate = s.ExpiredDate,
                    image = Helper.PhotoFolderPath + "/Image/" + s.PhotoPath
                }).Where(s => s.id == AdvertisementId).ToList();
                response.list = ads;
                response.status = "200";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                response.msg = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }
        #endregion

        #region Insert Medicine
        [Route("api/InsertMedicine/{CustomerId?}"), HttpPost]
        public HttpResponseMessage InsertMedicine(int CustomerId)
        {
            MedicineResponse response = new MedicineResponse();
            response.status = "400";
            try
            {
                tblMedicines model = new tblMedicines();
                model.CustomerId = CustomerId;
                var httpRequest = HttpContext.Current.Request;
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  
                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            var message = string.Format("Please Upload image of type .jpg,.gif,.png.");
                            response.msg = message;
                            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {
                            var message = string.Format("Please Upload a file upto 1 mb.");
                            response.msg = message;
                            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                        }
                        else
                        {
                            string path = HttpContext.Current.Server.MapPath("~/Uploads/");
                            var filename = path.FetchUniquePath(postedFile.FileName);
                            postedFile.SaveAs(path + filename);

                            model.PhotoPath = "/Uploads/" + filename;
                            model.Status = "Pending";
                            _MedicineRepo.SaveMedicine(model);
                        }
                    }
                    var message1 = string.Format("Image Updated Successfully.");
                    return Request.CreateErrorResponse(HttpStatusCode.Created, message1); ;
                }
                var res = string.Format("Please Upload a image.");
                response.msg = res;
                return Request.CreateResponse(HttpStatusCode.NotFound, response);

            }
            catch (Exception ex)
            {
                response.msg = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }
        #endregion

        #region Upload Medicine Image
        [Route("api/PostUserImage"), HttpPost]
        public HttpResponseMessage PostUserImage()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  
                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            var message = string.Format("Please Upload image of type .jpg,.gif,.png.");
                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {
                            var message = string.Format("Please Upload a file upto 1 mb.");
                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {
                            string path = HttpContext.Current.Server.MapPath("~/Uploads/");
                            var filename = path.FetchUniquePath(postedFile.FileName); ;
                            //var filePath = HttpContext.Current.Server.MapPath("~/Uploads/" + postedFile.FileName + extension);
                            postedFile.SaveAs(path + filename);
                        }
                    }
                    var message1 = string.Format("Image Updated Successfully.");
                    return Request.CreateErrorResponse(HttpStatusCode.Created, message1); ;
                }
                var res = string.Format("Please Upload a image.");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            catch (Exception ex)
            {
                var res = ex.Message;
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
        }
        #endregion
    }
}

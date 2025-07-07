using MilkWayIndia.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MilkWayIndia.Entity;

namespace MilkWayIndia.Concrete
{
    public class AdvertisementRepository : IAdvertisement
    {
        private EFDbContext db = new EFDbContext();
        public List<tbl_Advertisement> GetAllAdvertisement()
        {
            var advertisement = db.tblAdvertisement.Where(s => s.IsDeleted == false).ToList();
            return advertisement;
        }

        public tbl_Advertisement SaveAdvertisement(tbl_Advertisement model)
        {
            var ads = db.tblAdvertisement.FirstOrDefault(s => s.ID == model.ID);
            if (ads != null)
            {
                ads.AdsType = model.AdsType;
                ads.Title = model.Title;
                ads.Mobile = model.Mobile;
                ads.Description = model.Description;
                ads.WebsiteLink = model.WebsiteLink;
                ads.AppLink = model.AppLink;
                ads.PhotoPath = model.PhotoPath;
                ads.StartDate = model.StartDate;
                ads.ExpiredDate = model.ExpiredDate;
                ads.UpdatedDate = Models.Helper.indianTime;
            }
            else
            {
                model.ClickCount = 0;
                model.CreatedDate = Models.Helper.indianTime;
                db.tblAdvertisement.Add(model);
            }
            db.SaveChanges();
            return model;
        }
        

        public void DeleteAdvertisement(int? ID)
        {
            var ads = db.tblAdvertisement.FirstOrDefault(s => s.ID == ID);
            if (ads != null)
            {
                ads.IsDeleted = true;
                db.SaveChanges();
            }
        }

        public tbl_Advertisement GetAdvertisementID(int? ID)
        {
            var ads = db.tblAdvertisement.FirstOrDefault(s => s.ID == ID);
            return ads;
        }

        public void InsertCustomerAds(int? CustomerID, int? AdvertisementID)
        {
            tbl_Cust_Advertisement customer = new tbl_Cust_Advertisement();
            customer.CustomerID = CustomerID;
            customer.AdvertisementID = AdvertisementID;
            customer.CreatedDate = Models.Helper.indianTime;
            db.tbl_Cust_Advertisement.Add(customer);
            var ads = db.tblAdvertisement.FirstOrDefault(s => s.ID == AdvertisementID);
            if (ads != null)
            {
                ads.ClickCount = ads.ClickCount + 1;
            }
            db.SaveChanges();
        }
    }
}
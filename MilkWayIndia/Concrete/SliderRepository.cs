using MilkWayIndia.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MilkWayIndia.Entity;
using MilkWayIndia.Models;

namespace MilkWayIndia.Concrete
{
    public class SliderRepository : ISlider
    {
        private EFDbContext db = new EFDbContext();

        public List<BannerViewModel> GetAllSlider()
        {
            var banner = (from s in db.tblSliders
                          join s1 in db.tblSliderSectors on s.ID equals s1.SliderID
                          group s by s.ID into g
                          select new BannerViewModel
                          {
                              id = g.FirstOrDefault().ID,
                              sortorder = g.FirstOrDefault().SortOrder,
                              sectorid = g.FirstOrDefault().SectorID,
                              categoryid = g.FirstOrDefault().CategoryID,
                              title = g.FirstOrDefault().Title,
                              webistelink = g.FirstOrDefault().WebsiteLink,
                              applink = g.FirstOrDefault().AppLink,
                              mobile = g.FirstOrDefault().Mobile,
                              description = g.FirstOrDefault().Description,
                              image = Helper.PhotoFolderPath + "/Image/" + g.FirstOrDefault().PhotoPath
                          }).OrderBy(s => s.sortorder).ToList();
            return banner.ToList();
        }

        public List<BannerViewModel> GetBannerBySector(int? SectorID)
        {
            var banner = (from g in db.tblSliders
                          join s1 in db.tblSliderSectors on g.ID equals s1.SliderID
                          where s1.SectorID == SectorID
                          select new BannerViewModel
                          {
                              id = g.ID,
                              sortorder = g.SortOrder,
                              sectorid = s1.SectorID,
                              categoryid = g.CategoryID,
                              title = g.Title,
                              webistelink = g.WebsiteLink,
                              applink = g.AppLink,
                              mobile = g.Mobile,
                              description = g.Description,
                              image = Helper.PhotoFolderPath + "/Image/" + g.PhotoPath
                          }).OrderBy(s => s.sortorder).ToList();
            return banner.ToList();
        }

        public tblSliders SaveSlider(tblSliders model, string[] chkSector)
        {
            var slider = db.tblSliders.FirstOrDefault(s => s.ID == model.ID);
            if (slider != null)
            {
                slider.Title = model.Title;
                slider.PhotoPath = model.PhotoPath;
                slider.SortOrder = model.SortOrder;
                slider.UpdatedOn = Models.Helper.indianTime;
                slider.FileName = model.FileName;
                slider.CategoryID = model.CategoryID;
                slider.SectorID = model.SectorID;
                slider.WebsiteLink = model.WebsiteLink;
                slider.AppLink = model.AppLink;
                slider.Mobile = model.Mobile;
                slider.Description = model.Description;
            }
            else
            {
                db.tblSliders.Add(model);
            }
            db.SaveChanges();
            var delete = db.tblSliderSectors.Where(s => s.SliderID == model.ID);
            if (delete.Count() > 0)
                db.tblSliderSectors.RemoveRange(delete);
            if (chkSector != null)
            {
                foreach (var item in chkSector)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        tblSliderSectors sector = new tblSliderSectors();
                        sector.SectorID = Convert.ToInt32(item);
                        sector.SliderID = model.ID;
                        db.tblSliderSectors.Add(sector);
                    }
                }
            }
            db.SaveChanges();
            return model;
        }

        public tblSliders GetSliderByID(int? ID)
        {
            var product = db.tblSliders.FirstOrDefault(s => s.ID == ID);
            return product;
        }

        public void DeleteSlider(int? ID)
        {
            var slider = db.tblSliders.FirstOrDefault(s => s.ID == ID);
            if (slider != null)
            {
                db.tblSliders.Remove(slider);
                db.SaveChanges();
            }
        }

        public Boolean CheckSliderSector(int? SliderID, int? SectorID)
        {
            var slider = db.tblSliderSectors.FirstOrDefault(s => s.SliderID == SliderID && s.SectorID == SectorID);
            if (slider != null)
                return true;
            return false;
        }
    }
}
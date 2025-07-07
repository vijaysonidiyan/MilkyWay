using MilkWayIndia.Entity;
using MilkWayIndia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkWayIndia.Abstract
{
    public interface ISlider
    {
        tblSliders SaveSlider(tblSliders model, string[] chkSector);
        List<BannerViewModel> GetAllSlider();
        List<BannerViewModel> GetBannerBySector(int? SectorID);
        tblSliders GetSliderByID(int? ID);
        void DeleteSlider(int? ID);
        Boolean CheckSliderSector(int? SliderID, int? SectorID);
    }
}

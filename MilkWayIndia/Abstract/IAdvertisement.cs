using MilkWayIndia.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkWayIndia.Abstract
{
    public interface IAdvertisement
    {
        tbl_Advertisement SaveAdvertisement(tbl_Advertisement model);
        List<tbl_Advertisement> GetAllAdvertisement();
        tbl_Advertisement GetAdvertisementID(int? ID);
        void DeleteAdvertisement(int? ID);

        void InsertCustomerAds(int? CustomerID, int? AdvertisementID);
    }
}

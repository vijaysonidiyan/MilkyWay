using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Models
{
    public class FavouriteResponse
    {
        public string status { get; set; }
        public string error_msg { get; set; }
        public string msg { get; set; }
    }


    public class CashbackSeenResponse
    {
        public string status { get; set; }
        public string error_msg { get; set; }
        public string msg { get; set; }
    }

    public class ProductResponse
    {
        public string status { get; set; }
        public string error_msg { get; set; }
        public string msg { get; set; }
        public ProductViewModel products { get; set; }
    }

    public class FavouriteProductResponse
    {
        public string status { get; set; }
        public string error_msg { get; set; }
        public string msg { get; set; }
        public List<ProductViewModel> products { get; set; }
    }

    public class ProductViewModel
    {
        public int? id { get; set; }
        public int? productid { get; set; }
        public int? producttotal { get; set; }
        public int? categoryid { get; set; }
        public string product { get; set; }
        public string code { get; set; }
        public decimal? price { get; set; }
        public decimal? mrp { get; set; }
        public decimal? discountamt { get; set; }
        public decimal? cgst { get; set; }
        public decimal? sgst { get; set; }
        public decimal? igst { get; set; }
        public long? rewardpoint { get; set; }
        public string detail { get; set; }
        public string image { get; set; }
        public Boolean? isdaily { get; set; }
        public Boolean? IsAlternate { get; set; }
        public Boolean? IsMultipleDay { get; set; }
        public Boolean? IsWeeklyDay { get; set; }
        public string categoryname { get; set; }
        public string youtubetitle { get; set; }
        public string youtubeurl { get; set; }
        public string OrderTime { get; set; }
        public string DeliveryTime { get; set; }
        public decimal? minAmount { get; set; }
        public List<ProductPhotoModel> photos { get; set; }
    }

    public class ProductPhotoModel
    {
        public string photopath { get; set; }
    }

    public class InquiryResponse
    {
        public string status { get; set; }
        public string error_msg { get; set; }
        public string msg { get; set; }
    }

    public class DashboadModel
    {
        public string TotalCustomer { get; set; }
        public string ActiveCustomer { get; set; }
        public string TodayOrder { get; set; }
        public string YesterdayOrder { get; set; }
        public string TodayPendingOrder { get; set; }

        public string Recentrecharge { get; set; }
        public string Recentrechargep { get; set; }
        public string Recentrecharges { get; set; }
        public string Recentrechargef { get; set; }
        public string RecentBillPay { get; set; }
        public string RecentBillPayp { get; set; }
        public string RecentBillPays { get; set; }
        public string RecentBillPayf { get; set; }
    }

    public class CategoryResponse
    {
        public string status { get; set; }
        public string error_msg { get; set; }
        public string msg { get; set; }
        public List<CategoryModel> categories { get; set; }
    }

    public class CategoryModel
    {
        public int? id { get; set; }
        public string categoryname { get; set; }
        public string image { get; set; }
    }

    public class BannerResponse
    {
        public string status { get; set; }
        public string error_msg { get; set; }
        public string msg { get; set; }
        public List<BannerViewModel> banners { get; set; }
    }

    public class InitiateSubscriptionResponse
    {
        public string status { get; set; }
        public string error_msg { get; set; }
        public string msg { get; set; }
        public string token { get; set; }
    }

    public class BannerViewModel
    {
        public int? id { get; set; }
        public int? sortorder { get; set; }
        public int? categoryid { get; set; }
        public int? sectorid { get; set; }
        public string title { get; set; }
        public string image { get; set; }
        public string webistelink { get; set; }
        public string applink { get; set; }
        public string mobile { get; set; }
        public string description { get; set; }
    }

    public class AutoPayResponse
    {
        public string status { get; set; }
        public string error_msg { get; set; }
        public string msg { get; set; }
        public List<AutoPayiewModel> autopay { get; set; }
    }

    public class AutoPayiewModel
    {
        public int? id { get; set; }
        public decimal? balance { get; set; }
        public decimal? deposit { get; set; }
    }

    public class MedicineResponse
    {
        public string status { get; set; }
        public string error_msg { get; set; }
        public string msg { get; set; }
    }

    public class PaytmInitiateTransaction
    {
        public string token { get; set; }
        public string orderid { get; set; }
    }

    public class PaytmResponse
    {
        public head head { get; set; }
        public body body { get; set; }
    }

    public class head
    {
        public string responseTimestamp { get; set; }
        public string version { get; set; }
        public string signature { get; set; }
    }

    public class body
    {
        public resultInfo resultInfo { get; set; }
        public string txnToken { get; set; }
        public string subscriptionId { get; set; }
        public string isPromoCodeValid { get; set; }
        public string authenticated { get; set; }
    }

    public class resultInfo
    {
        public string resultStatus { get; set; }
        public string resultCode { get; set; }
        public string resultMsg { get; set; }
    }

    public class AdsResponse
    {
        public string status { get; set; }
        public string error_msg { get; set; }
        public string msg { get; set; }
        public List<AdsViewModel> list { get; set; }
    }

    public class AdsViewModel
    {
        public int? id { get; set; }
        public int? adstype { get; set; }
        public string title { get; set; }
        public string websitelink { get; set; }
        public string applink { get; set; }
        public string mobile { get; set; }
        public string description { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? expireddate { get; set; }
        public string image { get; set; }
    }

    public class CustomerWalletModel
    {
        public string Id { get; set; }
        public string Tdate { get; set; }
        public string Customer { get; set; }
        public string BillNo { get; set; }
        public string OrderNo { get; set; }
        public string OrderDate { get; set; }
        public string Subscription { get; set; }
        public string Amount { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }

    public class SectorViewModel
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public string IsSelected { get; set; }
        public string RoleId { get; set; }
    }

    public class VendorViewModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
       // public string CatName { get; set; }
        public string IsSelected { get; set; }
    }
    public class SubcatViewModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string IsSelected { get; set; }
    }

    public class SubcatVendorwiseViewModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string VendorName { get; set; }
        public string IsSelected { get; set; }
       
    }
    public class BillPayCircleVM
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public int? CircleCode { get; set; }
        public int? SortOrder { get; set; }
    }

    public class BillPaySerciceVM
    {
        public int? ID { get; set; }
        [Required]
        public int? SortOrder { get; set; }
        [Required]
        public string Name { get; set; }
        public string PhotoPath { get; set; }
    }

    public class BillPayOperatorVM
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public int? OperatorCode { get; set; }
        public string Type { get; set; }
    }

    public class BillPayProviderVM
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public int? OperatorID { get; set; }
        public string OperatorName { get; set; }
        public int? CircleID { get; set; }
        public string CircleName { get; set; }
        public int? CityID { get; set; }
        public string CityName { get; set; }
        public Boolean? IsPartial { get; set; }
        public string OperatorCode { get; set; }
        public int? ServiceID { get; set; }
        public string ServiceName { get; set; }
        public string NumberTag { get; set; }
        public string FieldTag1 { get; set; }
        public string FieldTag2 { get; set; }
        public string FieldTag3 { get; set; }
        public Boolean? IsDeleted { get; set; }
        public Boolean? IsActive { get; set; }
        public int? SortOrder { get; set; }
    }

    public class ProductAssignVM
    {
        public int? ID { get; set; }
        public int ProductID { get; set; }
        public int? VendorID { get; set; }
        public int? SectorID { get; set; }
        public int? AttributeID { get; set; }

        public int? AttributeID1 { get; set; }
        public decimal? MRPPrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal? SellPrice { get; set; }
        public decimal? B2BSellPrice { get; set; }
        public decimal? Profit { get; set; }
        public decimal? B2BProfit { get; set; }
        public decimal? CGST { get; set; }
        public decimal? SGST { get; set; }
        public decimal? IGST { get; set; }

        public string VendorName { get; set; }
        public string SectorName { get; set; }
        public string AttributeName { get; set; }

        public int? ParentcatId { get; set; }
        public int? SubcatId { get; set; }
        public int? VendorCatId { get; set; }
    }

    public class PaytmAuthroziedVM
    {
        public int? ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNo { get; set; }
        public Boolean? Authenticated { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? CreateDate { get; set; }
    }

    public class ProductVM
    {
        public int Value { get; set; }        
        public string Text { get; set; }        
        public bool IsChecked { get; set; }
    }

    public class MsgSectorVM
    {
        public int Value { get; set; }
        public string Text { get; set; }
        public bool IsChecked { get; set; }
    }

    public class ProductListVM
    {        
        public List<ProductVM> ProductList { get; set; }
    }


    public class MsgSectorListVM
    {
        public List<MsgSectorVM> MsgSector { get; set; }
    }

    public class VendorSectorVM
    {
       
        public string Text { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public string Text4 { get; set; }
        public string Text5 { get; set; }
    }

    public class VendorSectorListVM
    {
        public List<VendorSectorVM> VendorSector { get; set; }
    }

}
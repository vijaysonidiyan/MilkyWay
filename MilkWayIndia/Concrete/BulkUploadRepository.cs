using MilkWayIndia.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MilkWayIndia.Entity;

namespace MilkWayIndia.Concrete
{
    public class BulkUploadRepository : IBulkUpload
    {
        private EFDbContext db = new EFDbContext();
        //public BulkUploadRepository()
        //{
        //    this.db = efDbContext;
        //}

        public tbl_BulkUpload SaveBulkUpload(tbl_BulkUpload objentity)
        {
            db.tbl_BulkUpload.Add(objentity);
            db.SaveChanges();
            return objentity;
            //            return new Tuple<BulkUpload, string>(objentity, "");
        }

        public List<tbl_BulkUpload> GetAllBulkUpload()
        {
            var bulkUpload = (from b in db.tbl_BulkUpload.AsEnumerable()
                              select new tbl_BulkUpload
                              {
                                  ID = b.ID,
                                  FileName = b.FileName,
                                  TotalItem = b.TotalItem,
                                  UploadItem = b.UploadItem,
                                  IsUpload = b.IsUpload == null ? false : true,
                              }).ToList();
            return bulkUpload;
        }

        public List<tbl_BulkUpload> PendingBulkUpload()
        {
            var bulkUpload = db.tbl_BulkUpload.OrderByDescending(s => s.CreatedOn).ToList();
            //var bulkUpload = db.tbl_BulkUpload.Where(s => s.TotalItem > s.UploadItem).OrderByDescending(s => s.CreatedOn).ToList();
            return bulkUpload;
        }

        public List<tbl_Product_Temp> GetAllProduct(int? ID)
        {
            var product = db.tbl_Product_Temp.Where(s => s.UploadID == ID).ToList();
            return product;
        }

        public tbl_Product_Temp GetProductByID(int? ID)
        {
            var product = db.tbl_Product_Temp.FirstOrDefault(s => s.ID == ID);
            return product;
        }

        public void DeleteBulkUpload(int? ID)
        {
            var bulkUpload = db.tbl_BulkUpload.FirstOrDefault(s => s.ID == ID);
            if (bulkUpload != null)
            {
                var detail = db.tbl_Product_Temp.Where(s => s.UploadID == bulkUpload.ID);
                db.tbl_Product_Temp.RemoveRange(detail);
                db.tbl_BulkUpload.Remove(bulkUpload);
                db.SaveChanges();
            }
        }

        public void UpdateProduct(tbl_Product_Temp model)
        {
            var product = db.tbl_Product_Temp.FirstOrDefault(s => s.ID == model.ID);
            if (product != null)
            {
                product.CategoryName = model.CategoryName;
                product.ProductName = model.ProductName;
                product.MRP = model.MRP;
                product.SalePrice = model.SalePrice;
                product.PurchasePrice = model.PurchasePrice;
                product.CGST = model.CGST;
                product.SGST = model.SGST;
                product.IGST = model.IGST;
                product.Profit = model.Profit;
                product.Details = model.Details;
                product.Subcription = model.Subcription;
                product.YoutubeTitle = model.YoutubeTitle;
                product.YoutubeURL = model.YoutubeURL;
                product.IsDaily = model.IsDaily;
                product.Status = model.Status;
                db.SaveChanges();
            }
        }

        public void UpdateProductStatus(int? ID, Boolean IsUpload,string Message)
        {
            var product = db.tbl_Product_Temp.FirstOrDefault(s => s.ID == ID);
            if (product != null)
            {
                product.ErrorMessage = Message;
                product.IsUpload = IsUpload;
                db.SaveChanges();
            }
        }

        public void UpdateBulkStatus(int? ID)
        {
            var product = db.tbl_BulkUpload.FirstOrDefault(s => s.ID == ID);
            if (product != null)
            {
                var pro = db.tbl_Product_Temp.Where(s => s.UploadID == product.ID && s.IsUpload == true).Count();
                if (product.TotalItem == pro)
                {
                    product.IsUpload = true;
                    product.UploadItem = product.TotalItem;
                }
                db.SaveChanges();
            }
        }
    }
}
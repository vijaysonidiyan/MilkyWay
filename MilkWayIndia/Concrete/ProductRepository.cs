using MilkWayIndia.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MilkWayIndia.Entity;

namespace MilkWayIndia.Concrete
{
    public class ProductRepository : IProduct
    {
        private EFDbContext db = new EFDbContext();
        public int SaveProduct(tbl_Product_Master model)
        {
            try
            {
                var product = db.tbl_Product_Master.Add(model);
                db.SaveChanges();
                return product.Id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public void BulkUpdateProduct(tbl_Product_Master model)
        {
            var product = db.tbl_Product_Master.FirstOrDefault(s => s.Id == model.Id);
            if (product != null)
            {
                product.ProductName = model.ProductName;
                product.CategoryId = model.CategoryId;
                product.YoutubeTitle = model.YoutubeTitle;
                product.YoutubeURL = model.YoutubeURL;
                product.Subscription = model.Subscription;
                product.Price = model.Price;
                product.CGST = model.CGST;
                product.SGST = model.SGST;
                product.IGST = model.IGST;
                product.PurchasePrice = model.PurchasePrice;
                product.SalePrice = model.SalePrice;
                product.Profit = model.Profit;
                product.IsDaily = model.IsDaily;
                db.SaveChanges();
            }
        }
    }
}
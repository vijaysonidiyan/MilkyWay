using MilkWayIndia.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MilkWayIndia.Entity;
using MilkWayIndia.Models;

namespace MilkWayIndia.Concrete
{
    public class AttributeRepository : IAttribute
    {
        private EFDbContext db = new EFDbContext();
        public List<tbl_Attributes> GetAllAttribute()
        {
            var attribute = db.tbl_Attributes.Where(s => s.IsDeleted == false).OrderBy(s => s.Name).ToList();
            return attribute;
        }

        public int SaveAttribute(tbl_Attributes model)
        {
            try
            {
                if (model.ID == null)
                {
                    model.IsDeleted = false;
                    model.CreatedOn = Helper.indianTime;
                    db.tbl_Attributes.Add(model);
                }
                else
                {
                    var attribute = db.tbl_Attributes.FirstOrDefault(s => s.ID == model.ID);
                    if (attribute != null)
                    {
                        attribute.Name = model.Name;
                        attribute.UpdatedOn = Helper.indianTime;
                    }
                }
                db.SaveChanges();
                return 1;
            }
            catch { }
            return 0;
        }

        public tbl_Attributes GetAttributeByID(int? ID)
        {
            var attribute = db.tbl_Attributes.FirstOrDefault(s => s.ID == ID);
            if (attribute != null)
                return attribute;
            return null;
        }

        public int SaveProductAttribute(tbl_Product_Attributes model)
        {
            try
            {
                db.tbl_Product_Attributes.Add(model);
                db.SaveChanges();
                return 1;
            }
            catch { }
            return 0;
        }    
        
        public int DeleteProduct(int? ID)
        {
            var product = db.tbl_Product_Attributes.FirstOrDefault(s => s.ID == ID);
            if (product != null)
            {
                product.IsActive = false;
                product.IsDeleted = true;
                db.SaveChanges();
                return product.ProductID.Value;
            }
            return 0;
        }


        public tbl_Vendor_Product_Assign UpdateProductAttributeStatus(int Id)
        {
            var customer = db.tbl_Vendor_Product_Assign.FirstOrDefault(s => s.Id == Id);
            if (customer != null)
            {
                if (customer.IsActive == true)
                    customer.IsActive = false;
                else
                    customer.IsActive = true;

                db.SaveChanges();
                return customer;
            }
            return null;
        }

    }
}
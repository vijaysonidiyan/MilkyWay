using MilkWayIndia.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MilkWayIndia.Entity;
using System.Data.SqlClient;
using System.Data.Common;
using System.Text;

namespace MilkWayIndia.Concrete
{
    public class CustomerRepository : ICustomer
    {
        private EFDbContext db = new EFDbContext();        
        public tbl_Customer_Master SaveCustomer(tbl_Customer_Master model)
        {            
            var customer = db.tbl_Customer_Master.FirstOrDefault(s => s.ID == model.ID);
            if (customer != null)
            {
                
                customer.FirstName = model.FirstName;
                customer.LastName = model.LastName;
                customer.MobileNo = model.MobileNo;
                customer.UserName = model.MobileNo;
                customer.Email = model.Email;
                customer.Address = model.Address;
                customer.Password = model.Password;
                customer.SectorId = model.SectorId;
                customer.Photo = model.Photo;
                customer.Credit = model.Credit;
                customer.OrderBy = model.OrderBy;
                customer.UpdatedOn = Models.Helper.indianTime;
                customer.lat = model.lat;
                customer.lon = model.lon;

                customer.CustomerType = model.CustomerType;
            }
            else
            {
                model.CreatedOn = Models.Helper.indianTime;
                db.tbl_Customer_Master.Add(model);
            }
            db.SaveChanges();
            return model;
        }

        public tbl_Customer_Master GetCustomerByID(int ID)
        {
            var customer = db.tbl_Customer_Master.FirstOrDefault(s => s.ID == ID);
            if (customer != null)
                return customer;
            return null;
        }

        public tbl_Cusomter_Order_Track InsertCustomerOrderTrack(tbl_Cusomter_Order_Track model)
        {
            try
            {
               
                var r = db.tbl_Cusomter_Order_Track.Add(model);
                db.SaveChanges();
                return r;
            }
            catch
            {

            }
            return null;
        }

        public tbl_Cusomter_Order_Track InsertCustomerOrderTracknew(tbl_Cusomter_Order_Track model)
        {
            try
            {

                var r = db.tbl_Cusomter_Order_Track.Add(model);
                db.SaveChanges();
                return r;
            }
            catch
            {

            }
            return null;
        }

        public tbl_Customer_Master UpdateCustomerStatus(int ID)
        {
            var customer = db.tbl_Customer_Master.FirstOrDefault(s => s.ID == ID);
            if (customer != null)
            {
                if (customer.IsActive == true)
                    customer.IsActive = false;
                else
                    customer.IsActive = true;
                customer.UpdatedOn = Models.Helper.indianTime;
                db.SaveChanges();
                return customer;
            }
            return null;
        }

        public tbl_Customer_Master DeleteCustomer(int ID)
        {
            var customer = db.tbl_Customer_Master.FirstOrDefault(s => s.ID == ID);
            if (customer != null)
            {
                customer.IsDeleted = true;
                customer.UpdatedOn = Models.Helper.indianTime;
                db.SaveChanges();
                return customer;
            }
            return null;
        }
        public void UpdateCustomerSortOrder(int ID, int OrderBy)
        {
            try
            {
                var customer = db.tbl_Customer_Master.FirstOrDefault(s => s.ID == ID);
                if (customer != null)
                {
                    customer.OrderBy = OrderBy;
                    customer.UpdatedOn = Models.Helper.indianTime;
                    db.SaveChanges();
                }
            }catch (Exception ex){ }
        }

        public List<tbl_Cusomter_Order_Track> GetCustomerOrderByDate(DateTime? Date)
        {
            var order = db.tbl_Cusomter_Order_Track.Where(s => s.NextOrderDate.Value.Year == Date.Value.Year && s.NextOrderDate.Value.Day == Date.Value.Day && s.NextOrderDate.Value.Month == Date.Value.Month);
            return order.ToList();
        }
    }
}

using MilkWayIndia.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MilkWayIndia.Entity;
using MilkWayIndia.Models;

namespace MilkWayIndia.Concrete
{
    public class BillPayRepository : IBillPay
    {
        private EFDbContext db = new EFDbContext();
        public List<tblBillPayCircle> GetAllBillPayCircle()
        {


            var circle = db.tblBillPayCircle.Where(s => s.IsDeleted == false).OrderBy(s => s.SortOrder).ToList();
            
            return circle;
        }

        public int SaveBillPayCircle(tblBillPayCircle model)
        {
            if (model.ID == null)
            {
                model.CreatedOn = Models.Helper.indianTime;
                db.tblBillPayCircle.Add(model);
            }
            else
            {
                var circle = db.tblBillPayCircle.FirstOrDefault(s => s.ID == model.ID);
                if (circle != null)
                {
                    circle.Name = model.Name;
                    circle.CircleCode = model.CircleCode;
                    circle.SortOrder = model.SortOrder;
                    circle.UpdatedOn = Models.Helper.indianTime;
                }
            }
            db.SaveChanges();
            return model.ID.Value;
        }

        public tblBillPayCircle GetBillPayCircleByID(int ID)
        {
            var circle = db.tblBillPayCircle.FirstOrDefault(s => s.ID == ID);
            if (circle != null)
            {
                return circle;
            }
            return null;
        }

        public void UpdateBillPayCircleStatus(int ID)
        {
            var circle = db.tblBillPayCircle.FirstOrDefault(s => s.ID == ID);
            if (circle != null)
            {
                circle.UpdatedOn = Models.Helper.indianTime;
                if (circle.IsActive == true)
                    circle.IsActive = false;
                else
                    circle.IsActive = true;
                db.SaveChanges();
            }
        }

        public void DeleteBillPayCircle(int ID)
        {
            var circle = db.tblBillPayCircle.FirstOrDefault(s => s.ID == ID);
            if (circle != null)
            {
                circle.UpdatedOn = Models.Helper.indianTime;
                circle.IsActive = false;
                circle.IsDeleted = true;
                db.SaveChanges();
            }
        }

        public Boolean ValidateCircleName(string Name)
        {
            var circle = db.tblBillPayCircle.FirstOrDefault(s => s.Name.ToLower() == Name.ToLower());
            if (circle != null)
                return true;
            else
                return false;
        }

        #region BillPay Services
        public List<tblBillPayService> GetAllBillPayService()
        {
            var service = db.tblBillPayService.Where(s => s.IsDeleted == false).OrderBy(s => s.SortOrder).ToList();
            return service;
        }

        public int SaveBillPayService(tblBillPayService model)
        {
            if (model.ID == null)
            {
                model.CreatedOn = Models.Helper.indianTime;
                db.tblBillPayService.Add(model);
            }
            else
            {
                var service = db.tblBillPayService.FirstOrDefault(s => s.ID == model.ID);
                if (service != null)
                {
                    service.Name = model.Name;
                    service.SortOrder = model.SortOrder;
                    if (!string.IsNullOrEmpty(model.PhotoPath))
                        service.PhotoPath = model.PhotoPath;
                    service.UpdatedOn = Models.Helper.indianTime;
                }
            }
            db.SaveChanges();
            return model.ID.Value;
        }

        public tblBillPayService GetBillPayServiceByID(int ID)
        {
            var service = db.tblBillPayService.FirstOrDefault(s => s.ID == ID);
            if (service != null)
            {
                return service;
            }
            return null;
        }

        public void UpdateBillPayServiceStatus(int ID)
        {
            var service = db.tblBillPayService.FirstOrDefault(s => s.ID == ID);
            if (service != null)
            {
                service.UpdatedOn = Models.Helper.indianTime;
                if (service.IsActive == true)
                    service.IsActive = false;
                else
                    service.IsActive = true;
                db.SaveChanges();
            }
        }

        public void DeleteBillPayService(int ID)
        {
            var service = db.tblBillPayService.FirstOrDefault(s => s.ID == ID);
            if (service != null)
            {
                service.UpdatedOn = Models.Helper.indianTime;
                service.IsActive = false;
                service.IsDeleted = true;
                db.SaveChanges();
            }
        }
        #endregion

        #region Billpay City
        public List<tblBillPayCity> GetAllBillPayCity()
        {
            var city = db.tblBillPayCity.Where(s => s.IsDeleted == false).ToList();
            return city;
        }

        public int SaveBillPayCity(tblBillPayCity model)
        {
            if (model.ID == null)
            {
                model.CreatedOn = Models.Helper.indianTime;
                db.tblBillPayCity.Add(model);
            }
            else
            {
                var circle = db.tblBillPayCity.FirstOrDefault(s => s.ID == model.ID);
                if (circle != null)
                {
                    circle.Name = model.Name;
                    circle.UpdatedOn = Models.Helper.indianTime;
                }
            }
            db.SaveChanges();
            return model.ID.Value;
        }

        public tblBillPayCity GetBillPayCityByID(int ID)
        {
            var circle = db.tblBillPayCity.FirstOrDefault(s => s.ID == ID);
            if (circle != null)
            {
                return circle;
            }
            return null;
        }

        public void UpdateBillPayCityStatus(int ID)
        {
            var city = db.tblBillPayCity.FirstOrDefault(s => s.ID == ID);
            if (city != null)
            {
                city.UpdatedOn = Models.Helper.indianTime;
                if (city.IsActive == true)
                    city.IsActive = false;
                else
                    city.IsActive = true;
                db.SaveChanges();
            }
        }

        public void DeleteBillPayCity(int ID)
        {
            var circle = db.tblBillPayCity.FirstOrDefault(s => s.ID == ID);
            if (circle != null)
            {
                circle.UpdatedOn = Models.Helper.indianTime;
                circle.IsActive = false;
                circle.IsDeleted = true;
                db.SaveChanges();
            }
        }

        public Boolean ValidateCityName(string Name)
        {
            var circle = db.tblBillPayCity.FirstOrDefault(s => s.Name.ToLower() == Name.ToLower());
            if (circle != null)
                return true;
            else
                return false;
        }
        #endregion

        #region Billpay Operator
        public List<tblBillPayOperator> GetAllBillPayOperator()
        {
            var op = db.tblBillPayOperator.Where(s => s.IsDeleted == false).ToList();
            return op;
        }

        public int SaveBillPayOperator(tblBillPayOperator model)
        {
            if (model.ID == null)
            {
                model.CreatedOn = Models.Helper.indianTime;
                db.tblBillPayOperator.Add(model);
            }
            else
            {
                var op = db.tblBillPayOperator.FirstOrDefault(s => s.ID == model.ID);
                if (op != null)
                {
                    op.Type = model.Type;
                    op.Name = model.Name;
                    op.OperatorCode = model.OperatorCode;
                    op.UpdatedOn = Models.Helper.indianTime;
                }
            }
            db.SaveChanges();
            return model.ID.Value;
        }

        public tblBillPayOperator GetBillPayOperatorByID(int ID)
        {
            var op = db.tblBillPayOperator.FirstOrDefault(s => s.ID == ID);
            if (op != null)
            {
                return op;
            }
            return null;
        }

        public void UpdateBillPayOperatorStatus(int ID,string type)
        {
            var op = db.tblBillPayOperator.FirstOrDefault(s => s.ID == ID);
            
            if (op != null)
            {
                op.Type = type;
                op.UpdatedOn = Models.Helper.indianTime;
                if (op.IsActive == true)
                    op.IsActive = false;
                else
                    op.IsActive = true;
                db.SaveChanges();
            }
        }

        public void DeleteBillPayOperator(int ID)
        {
            var circle = db.tblBillPayOperator.FirstOrDefault(s => s.ID == ID);
            if (circle != null)
            {
                circle.UpdatedOn = Models.Helper.indianTime;
                circle.IsActive = false;
                circle.IsDeleted = true;
                db.SaveChanges();
            }
        }

        public Boolean ValidateOperatorName(string Name)
        {
            var op = db.tblBillPayOperator.FirstOrDefault(s => s.Name.ToLower() == Name.ToLower());
            if (op != null)
                return true;
            else
                return false;
        }
        #endregion

        #region Billpay Provider
        public List<BillPayProviderVM> GetAllBillPayProvider()
        {
            var op = (from o in db.tblBillPayProvider
                      join c in db.tblBillPayCircle on o.CircleID equals c.ID
                      join c1 in db.tblBillPayCity on o.CityID equals c1.ID
                      join s in db.tblBillPayService on o.ServiceID equals s.ID
                      join o1 in db.tblBillPayOperator on o.OperatorID equals o1.ID
                      where o.IsDeleted == false
                      select new BillPayProviderVM
                      {
                          ID = o.ID,
                          OperatorID = o.OperatorID,
                          OperatorName = o1.Name,
                          CircleID = o.CircleID,
                          CircleName = c.Name,
                          CityID = o.CityID,
                          CityName = c1.Name,
                          ServiceID = o.ServiceID,
                          ServiceName = s.Name,
                          Name = o.Name,
                          NumberTag = o.NumberTag,
                          FieldTag1 = o.FieldTag1,
                          FieldTag2 = o.FieldTag2,
                          FieldTag3 = o.FieldTag3,
                          IsActive = o.IsActive,
                          IsDeleted = o.IsDeleted,
                          IsPartial = o.IsPartial,
                          OperatorCode = o.OperatorCode
                      }).ToList();
            return op;
        }

        public int SaveBillPayProvider(tblBillPayProvider model)
        {
            try
            {
                db.tblBillPayProvider.Add(model);
                db.SaveChanges();
                return model.ID.Value;
            }
            catch { }
            return 0;
        }

        public int UpdateBillPayProvider(tblBillPayProvider model)
        {
            try
            {
                var provider = db.tblBillPayProvider.FirstOrDefault(s => s.ID == model.ID);
                if (provider != null)
                {
                    var userID = Helper.CurrentLoginUser();
                    provider.OperatorID = model.OperatorID;
                    provider.ServiceID = model.ServiceID;
                    provider.CircleID = model.CircleID;
                    provider.CityID = model.CityID;
                    provider.IsPartial = model.IsPartial;
                    provider.OperatorCode = model.OperatorCode;
                    provider.NumberTag = model.NumberTag;
                    provider.FieldTag1 = model.FieldTag1;
                    provider.FieldTag2 = model.FieldTag2;
                    provider.FieldTag3 = model.FieldTag3;
                    provider.UpdatedOn = Models.Helper.indianTime;
                    provider.UpdatedBy =Convert.ToInt32(userID);
                    db.SaveChanges();
                    return 1;
                }
            }
            catch { }
            return 0;
        }

        public tblBillPayProvider GetBillPayProviderByID(int ID)
        {
            var provider = db.tblBillPayProvider.FirstOrDefault(s => s.ID == ID);
            if (provider != null)
            {
                return provider;
            }
            return null;
        }

        public void UpdateBillPayProviderStatus(int ID)
        {
            var provider = db.tblBillPayProvider.FirstOrDefault(s => s.ID == ID);
            if (provider != null)
            {
                provider.UpdatedOn = Models.Helper.indianTime;
                if (provider.IsActive == true)
                    provider.IsActive = false;
                else
                    provider.IsActive = true;
                db.SaveChanges();
            }
        }

        public void DeleteBillPayProvider(int ID)
        {
            var provider = db.tblBillPayProvider.FirstOrDefault(s => s.ID == ID);
            if (provider != null)
            {
                provider.UpdatedOn = Models.Helper.indianTime;
                provider.IsActive = false;
                provider.IsDeleted = true;
                db.SaveChanges();
            }
        }
        #endregion
    }
}
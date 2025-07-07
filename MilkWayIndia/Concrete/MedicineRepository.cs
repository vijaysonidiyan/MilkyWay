using MilkWayIndia.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MilkWayIndia.Entity;

namespace MilkWayIndia.Concrete
{
    public class MedicineRepository : IMedicine
    {
        private EFDbContext db = new EFDbContext();

        public List<tblMedicines> GetAllMedicine()
        {
            var medicine = (from m in db.tblMedicine.AsEnumerable()
                            join c in db.tbl_Customer_Master on m.CustomerId equals c.ID
                            select new tblMedicines
                            {
                                ID = m.ID,
                                CustomerId = m.CustomerId,
                                CreateDate = m.CreateDate,
                                CreateDateStr = m.CreateDate == null ? "" : m.CreateDate.Value.ToString("dd-MMM-yyyy"),
                                CustomerName = c.FirstName + " " + c.LastName,
                                PhotoPath = m.PhotoPath,
                                Status = m.Status
                            }).ToList();
            return medicine.OrderByDescending(s => s.CreateDate).ToList();
        }

        public int SaveMedicine(tblMedicines model)
        {
            if (model.ID == null)
            {
                model.CreateDate = Models.Helper.indianTime;
                db.tblMedicine.Add(model);
            }
            else
            {
                var medicine = db.tblMedicine.FirstOrDefault(s => s.ID == model.ID);
                if (medicine != null)
                {
                    medicine.CustomerId = model.CustomerId;
                    medicine.PhotoPath = model.PhotoPath;
                    medicine.UpdateDate = Models.Helper.indianTime;
                }
            }
            db.SaveChanges();
            return model.ID.Value;
        }

        public int DeleteMedicine(int ID)
        {
            var medicine = db.tblMedicine.FirstOrDefault(s => s.ID == ID);
            if (medicine != null)
            {
                db.tblMedicine.Remove(medicine);
                db.SaveChanges();
                return 1;
            }
            return 0;
        }
    }
}
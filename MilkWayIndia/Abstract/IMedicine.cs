using MilkWayIndia.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkWayIndia.Abstract
{
    public interface IMedicine
    {
        int SaveMedicine(tblMedicines model);
        List<tblMedicines> GetAllMedicine();
        int DeleteMedicine(int ID);
    }
}

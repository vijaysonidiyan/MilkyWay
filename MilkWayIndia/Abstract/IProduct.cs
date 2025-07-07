using MilkWayIndia.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkWayIndia.Abstract
{
    public interface IProduct
    {
        int SaveProduct(tbl_Product_Master model);
        void BulkUpdateProduct(tbl_Product_Master model);
    }
}

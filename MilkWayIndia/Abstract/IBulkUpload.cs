using MilkWayIndia.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkWayIndia.Abstract
{
    public interface IBulkUpload
    {
        tbl_BulkUpload SaveBulkUpload(tbl_BulkUpload objentity);
        List<tbl_BulkUpload> GetAllBulkUpload();
        List<tbl_BulkUpload> PendingBulkUpload();
        void DeleteBulkUpload(int? ID);

        List<tbl_Product_Temp> GetAllProduct(int? ID);
        tbl_Product_Temp GetProductByID(int? ID);
        void UpdateProduct(tbl_Product_Temp model);
        void UpdateProductStatus(int? ID, Boolean IsUpload, string Message);
        void UpdateBulkStatus(int? ID);
    }
}

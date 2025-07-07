using MilkWayIndia.Entity;
using MilkWayIndia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkWayIndia.Abstract
{
    public interface IAttribute
    {
        List<tbl_Attributes> GetAllAttribute();
        int SaveAttribute(tbl_Attributes model);
        tbl_Attributes GetAttributeByID(int? ID);

        int SaveProductAttribute(tbl_Product_Attributes model);
        int DeleteProduct(int? ID);


        tbl_Vendor_Product_Assign UpdateProductAttributeStatus(int Id);
    }
}

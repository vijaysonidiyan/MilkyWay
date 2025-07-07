using MilkWayIndia.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkWayIndia.Abstract
{
    public interface ICustomer
    {
        tbl_Customer_Master SaveCustomer(tbl_Customer_Master model);
        tbl_Customer_Master GetCustomerByID(int ID);
        tbl_Cusomter_Order_Track InsertCustomerOrderTrack(tbl_Cusomter_Order_Track model);
        tbl_Cusomter_Order_Track InsertCustomerOrderTracknew(tbl_Cusomter_Order_Track model);
        tbl_Customer_Master UpdateCustomerStatus(int ID);
        tbl_Customer_Master DeleteCustomer(int ID);
        void UpdateCustomerSortOrder(int Id, int OrderBy);
        List<tbl_Cusomter_Order_Track> GetCustomerOrderByDate(DateTime? Date);
    }
}

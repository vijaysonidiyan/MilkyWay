using MilkWayIndia.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkWayIndia.Abstract
{
    public interface ISecPaytm
    {
        int SavePaytmRequest(tbl_Paytm_Request model);
        string GenerateOrderNo();
        tbl_Paytm_Request GetDetailByPaytmToken(string token);
        tbl_Paytm_Request UpdatePaytmResponseByOrderID(tbl_Paytm_Request model);
        tbl_Paytm_Request GetDetailByCustomerID(int? CustomerID);
        tbl_Paytm_Request UpdatePaytmPrenotify(tbl_Paytm_Request model);
        int InsertPaytmPreNotify(tbl_Paytm_Request_Details model);
    }
}

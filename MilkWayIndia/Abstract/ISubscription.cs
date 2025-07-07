using MilkWayIndia.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkWayIndia.Abstract
{
    public interface ISubscription
    {
        tbl_Sector_Subscription UpdateSubscriptionStatus(int ID);
    }
}
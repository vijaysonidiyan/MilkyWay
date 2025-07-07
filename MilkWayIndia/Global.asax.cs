using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using MilkWayIndia.Models;

namespace MilkWayIndia
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        //Added code
        protected void Application_BeginRequest(object sender, EventArgs e)
        {            
            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.Flush();
            }
        }

        //My Session Code
        void Session_Start(object sender, EventArgs e)
        {
            if (Session.IsNewSession)
            {
                //do things that need to happen
                //when a new session starts.
                Session.Timeout = 1200;
            }
        }
    }
}

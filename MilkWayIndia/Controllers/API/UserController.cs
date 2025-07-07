using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MilkWayIndia.Entity;
using MilkWayIndia.Models;
using MilkWayIndia.Abstract;
using MilkWayIndia.Concrete;
using Newtonsoft.Json;
using Paytm;
using System.IO;

namespace MilkWayIndia.Controllers.API
{
    public class UserController : ApiController
    {
        Helper dHelper = new Helper();
        private ISecPaytm _SecPaymentRepo;
        public UserController()
        {
            this._SecPaymentRepo = new SecPaytmRepository();
        }

        [Route("api/InitiateSubscription/{CustomerId?}"), HttpPost]
        public HttpResponseMessage InitiateSubscription(string CustomerId, string PlanId)
        {
            if (!string.IsNullOrEmpty(CustomerId))
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            if (!string.IsNullOrEmpty(PlanId))
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var response = dHelper.InitiateSubscription(CustomerId, Convert.ToInt32(PlanId));
            if (response.status == "200")
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }

        [Route("api/GetAutoPayPlan"), HttpGet]
        public HttpResponseMessage GetAutoPayPlan()
        {
            AutoPayResponse response = new AutoPayResponse();
            response.status = "400";
            try
            {
                response.autopay = dHelper.GetAutoPayPlan();
                response.status = "200";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                response.msg = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }

        [Route("api/InitiateTransaction/{CustomerId?}/{Amount?}"), HttpGet]
        public HttpResponseMessage InitiateTransaction(string CustomerId, decimal Amount)
        {
            try
            {
                //var s = dHelper.InitiateTransaction(CustomerId, Amount);
                var s = dHelper.InitiateTransactionnew(CustomerId, Amount);
                return Request.CreateResponse(HttpStatusCode.OK, s);
            }
            catch (Exception ex)
            {

            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}

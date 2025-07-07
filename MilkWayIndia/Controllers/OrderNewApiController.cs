using MilkWayIndia.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace MilkWayIndia.Controllers
{
    public class OrderNewApiController : ApiController
    {

        SubscriptionNew obj = new SubscriptionNew();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");




        [Route("api/OrderNewApi/UpdateOrder/{id?}/{newqty?}/{CusType?}")]
        [HttpGet]
        public IHttpActionResult UpdateOrder(int id,int newqty,string CusType)
        {
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();
            int UpdateProductOrder = 0; int UpdateAddProductDetail = 0;

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            if (!string.IsNullOrEmpty(id.ToString()) && id != 0 && !string.IsNullOrEmpty(newqty.ToString()) && newqty != 0)
            {

                obj.Id = id;
                obj.Qty = newqty;

                //get customer Id From order
                int CustomerId = 0;
                CustomerId = obj.getOrederCustomerId(obj.Id.ToString());
                obj.CustomerId = CustomerId;

                //check wallet balance
                decimal Walletbal = 0, TotalCredit = 0, TotalDebit = 0, Total2daybal = 0;
                DataTable dtprodRecord1 = new DataTable();
                dtprodRecord1 = obj.getCustomerWallet(obj.CustomerId);
                int userRecords1 = dtprodRecord1.Rows.Count;
                if (userRecords1 > 0)
                {
                    var balance = obj.GetCustomerBalace(obj.CustomerId);
                    Walletbal = balance;

                    DataTable dtcheckbal = new DataTable();
                    if (CustomerId > 0)
                    {
                        // check balance future 2 days
                        dtcheckbal = obj.getLast2daytotal(CustomerId.ToString());
                        if (dtcheckbal.Rows.Count > 0)
                        {
                            Total2daybal = Convert.ToDecimal(dtcheckbal.Rows[0]["Total"].ToString());
                        }
                    }
                }
                //if (Walletbal < (Total2daybal))
                //{
                //    dr["status"] = "Failed";
                //    dr["error_msg"] = "Wallet balance is Low!!Can't Update Qty";
                //}
                //else
                //{
                //if (Walletbal > obj.Amount)
                //{
                //get OrderId from parent to remove child records
                DataTable dtprodRecord = new DataTable();
                dtprodRecord = obj.getCustomerOnedateOrderList(obj.Id);
                int userRecords = dtprodRecord.Rows.Count;
                obj.OrderId = 0;

                if (userRecords > 0)
                {
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["OrderId"].ToString()))
                    {
                        obj.OrderId = Convert.ToInt32(dtprodRecord.Rows[0]["OrderId"]);
                        //get Item 
                        obj.ProductId = Convert.ToInt32(dtprodRecord.Rows[0]["ProductId"]);
                        DataTable dtProduct = objproduct.BindProuct(obj.ProductId);

                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["OrderDate"].ToString()))
                        {
                            DateTime CurrentDate = Helper.indianTime.AddDays(1);
                            DateTime OrderDate = Convert.ToDateTime(dtprodRecord.Rows[0]["OrderDate"]);
                            if (CurrentDate.Day == OrderDate.Day && CurrentDate.Month == OrderDate.Month && CurrentDate.Year == OrderDate.Year)
                            {
                                var timming = objproduct.CheckProductOrderTimimg(obj.ProductId);
                                if (timming.IsTime == false)
                                {
                                    dr["status"] = "Fail";
                                    dr["error_msg"] = "Unable to update... - " + timming.message;
                                    dtNew.Rows.Add(dr);
                                    return Ok(dtNew);
                                }
                            }
                        }

                        if (dtProduct.Rows.Count > 0)
                        {
                            if (string.IsNullOrEmpty(CusType) || CusType == "General")
                            {
                                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SellPrice"].ToString()))
                                {
                                    obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]) * obj.Qty;
                                    obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]);
                                }
                                else
                                {
                                    obj.Amount = 0;
                                    obj.SalePrice = 0;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["B2BSellPrice"].ToString()))
                                {
                                    obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["B2BSellPrice"]) * obj.Qty;
                                    obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["B2BSellPrice"]);
                                }
                                else
                                {
                                    obj.Amount = 0;
                                    obj.SalePrice = 0;
                                }
                            }


                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["PurchasePrice"].ToString()))
                            {
                                obj.PurchasePrice = Convert.ToDecimal(dtProduct.Rows[0]["PurchasePrice"]) * obj.Qty;

                            }
                            else
                            {
                                obj.PurchasePrice = 0;

                            }

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Price"].ToString()))
                            {
                                obj.MRPPrice = Convert.ToDecimal(dtProduct.Rows[0]["Price"]) * obj.Qty;

                            }
                            else
                            {
                                obj.MRPPrice = 0;

                            }

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountAmount"].ToString()))
                                obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountAmount"]) * obj.Qty;
                            else
                                obj.Discount = 0;


                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                                obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                            else
                                obj.CGSTPerct = 0;
                            //count cgst Amount
                            if (obj.CGSTPerct > 0)
                                obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                            else
                                obj.CGSTAmount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                                obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                            else
                                obj.IGSTPerct = 0;
                            //count igst Amount
                            if (obj.IGSTPerct > 0)
                                obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                            else
                                obj.IGSTAmount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                                obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                            else
                                obj.SGSTPerct = 0;
                            //count sgst Amount
                            if (obj.SGSTPerct > 0)
                                obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                            else
                                obj.SGSTAmount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                                obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                            else
                                obj.RewardPoint = 0;
                            if (string.IsNullOrEmpty(CusType) || CusType == "General")
                            {
                                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                                    obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                                else
                                    obj.Profit = 0;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["B2BProfit"].ToString()))
                                    obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["B2BProfit"]) * obj.Qty;
                                else
                                    obj.Profit = 0;
                            }

                            //Final Amount
                            obj.TotalFinalAmount = obj.Amount - obj.Discount;
                        }
                        //update order main
                        obj.Status = "Pending";

                        obj.TotalAmount = obj.TotalFinalAmount;
                        UpdateProductOrder = obj.UpdateCustomerOrderMobile(obj);

                        //update item order
                        obj.OrderId = id;
                        UpdateAddProductDetail = obj.UpdateCustomerOrderDetailMobile(obj);
                    }
                    else
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Not Found";
                    }
                    if (UpdateAddProductDetail > 0)
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Updated Successfully";
                    }
                    else
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Not Updated";
                    }
                }
                else
                {
                    dr["status"] = "Success";
                    dr["error_msg"] = "Order Not Found";
                }
                //}
                //else
                //{
                //    dr["status"] = "Failed";
                //    dr["error_msg"] = "Wallet balance is Low!!Can't Update Qty";
                //}
                //}
            }
            else if (!string.IsNullOrEmpty(id.ToString()) && id != 0 && !string.IsNullOrEmpty(newqty.ToString()) && newqty == 0)
            {
                obj.Id = id;
                obj.Qty = newqty;
                //get OrderId from parent to remove child records
                DataTable dtprodRecord = new DataTable();
                dtprodRecord = obj.getCustomerOnedateOrderList(obj.Id);
                int userRecords = dtprodRecord.Rows.Count;
                obj.OrderId = 0;

                if (userRecords > 0)
                {
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["OrderId"].ToString()))
                    {
                        obj.OrderId = Convert.ToInt32(dtprodRecord.Rows[0]["OrderId"]);

                        //get Item 
                        obj.ProductId = Convert.ToInt32(dtprodRecord.Rows[0]["ProductId"]);
                        DataTable dtProduct = objproduct.BindProuct(obj.ProductId);
                        if (dtProduct.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["OrderDate"].ToString()))
                            {
                                DateTime CurrentDate = Helper.indianTime.AddDays(1);
                                DateTime OrderDate = Convert.ToDateTime(dtprodRecord.Rows[0]["OrderDate"]);
                                if (CurrentDate.Day == OrderDate.Day && CurrentDate.Month == OrderDate.Month && CurrentDate.Year == OrderDate.Year)
                                {
                                    var timming = objproduct.CheckProductOrderTimimg(obj.ProductId);
                                    if (timming.IsTime == false)
                                    {
                                        dr["status"] = "Fail";
                                        dr["error_msg"] = "Unable to update... - " + timming.message;
                                        dtNew.Rows.Add(dr);
                                        return Ok(dtNew);
                                    }
                                }
                            }
                            if (string.IsNullOrEmpty(CusType) || CusType == "General")
                            {
                                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SellPrice"].ToString()))
                                {
                                    obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]) * obj.Qty;
                                    obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]);
                                }
                                else
                                {
                                    obj.Amount = 0;
                                    obj.SalePrice = 0;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["B2BSellPrice"].ToString()))
                                {
                                    obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["B2BSellPrice"]) * obj.Qty;
                                    obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["B2BSellPrice"]);
                                }
                                else
                                {
                                    obj.Amount = 0;
                                    obj.SalePrice = 0;
                                }
                            }

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountAmount"].ToString()))
                                obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountAmount"]);
                            else
                                obj.Discount = 0;


                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                                obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]);
                            else
                                obj.CGSTPerct = 0;
                            //count cgst Amount
                            if (obj.CGSTPerct > 0)
                                obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                            else
                                obj.CGSTAmount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                                obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]);
                            else
                                obj.IGSTPerct = 0;
                            //count igst Amount
                            if (obj.IGSTPerct > 0)
                                obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                            else
                                obj.IGSTAmount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                                obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]);
                            else
                                obj.SGSTPerct = 0;
                            //count sgst Amount
                            if (obj.SGSTPerct > 0)
                                obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                            else
                                obj.SGSTAmount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                                obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]);
                            else
                                obj.RewardPoint = 0;
                            if (string.IsNullOrEmpty(CusType) || CusType == "General")
                            {
                                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                                    obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                                else
                                    obj.Profit = 0;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["B2BProfit"].ToString()))
                                    obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["B2BProfit"]) * obj.Qty;
                                else
                                    obj.Profit = 0;
                            }

                            //Final Amount
                            obj.TotalFinalAmount = obj.Amount - obj.Discount;
                        }
                        //update order main
                        obj.Status = "Cancel";

                        obj.TotalAmount = obj.TotalFinalAmount;
                        UpdateProductOrder = obj.UpdateCustomerOrderMobile(obj);

                        //update item order
                        obj.OrderId = id;
                        UpdateAddProductDetail = obj.UpdateCustomerOrderDetailMobile(obj);
                    }
                    else
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Not Found";
                    }
                    if (UpdateAddProductDetail > 0)
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Cancelled Successfully";
                    }
                    else
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Not Cancelled";
                    }
                }
                else
                {
                    dr["status"] = "Success";
                    dr["error_msg"] = "Order Not Found";
                }
            }
            else
            {
                dr["status"] = "Failed";
                dr["error_msg"] = "Please Fill Correct Details";
            }
            dtNew.Rows.Add(dr);
            return Ok(dtNew);

        }



        #region Update Customer Order Working
        //edit order
        [Route("api/OrderNewApi/UpdateCustomerOrder")]
        public IHttpActionResult UpdateCustomerOrder(SubscriptionNew item)//string strjson
        {   //
            SubscriptionNew obj = new SubscriptionNew();
            Customer objcust = new Customer();
            Product objproduct = new Product();
            int UpdateProductOrder = 0; int UpdateAddProductDetail = 0;

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            if (!string.IsNullOrEmpty(item.Id.ToString()) && item.Id != 0 && !string.IsNullOrEmpty(item.Qty.ToString()) && item.Qty != 0)
            {

                obj.Id = item.Id;
                obj.Qty = item.Qty;

                //get customer Id From order
                int CustomerId = 0;
                CustomerId = obj.getOrederCustomerId(obj.Id.ToString());
                obj.CustomerId = CustomerId;

                //check wallet balance
                decimal Walletbal = 0, TotalCredit = 0, TotalDebit = 0, Total2daybal = 0;
                DataTable dtprodRecord1 = new DataTable();
                dtprodRecord1 = obj.getCustomerWallet(obj.CustomerId);
                int userRecords1 = dtprodRecord1.Rows.Count;
                if (userRecords1 > 0)
                {
                    var balance = obj.GetCustomerBalace(obj.CustomerId);
                    Walletbal = balance;

                    DataTable dtcheckbal = new DataTable();
                    if (CustomerId > 0)
                    {
                        // check balance future 2 days
                        dtcheckbal = obj.getLast2daytotal(CustomerId.ToString());
                        if (dtcheckbal.Rows.Count > 0)
                        {
                            Total2daybal = Convert.ToDecimal(dtcheckbal.Rows[0]["Total"].ToString());
                        }
                    }
                }
                //if (Walletbal < (Total2daybal))
                //{
                //    dr["status"] = "Failed";
                //    dr["error_msg"] = "Wallet balance is Low!!Can't Update Qty";
                //}
                //else
                //{
                    //if (Walletbal > obj.Amount)
                    //{
                        //get OrderId from parent to remove child records
                        DataTable dtprodRecord = new DataTable();
                        dtprodRecord = obj.getCustomerOnedateOrderList(obj.Id);
                        int userRecords = dtprodRecord.Rows.Count;
                        obj.OrderId = 0;

                        if (userRecords > 0)
                        {
                            if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["OrderId"].ToString()))
                            {
                                obj.OrderId = Convert.ToInt32(dtprodRecord.Rows[0]["OrderId"]);
                                //get Item 
                                obj.ProductId = Convert.ToInt32(dtprodRecord.Rows[0]["ProductId"]);
                                DataTable dtProduct = objproduct.BindProuct(obj.ProductId);

                                if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["OrderDate"].ToString()))
                                {
                                    DateTime CurrentDate = Helper.indianTime.AddDays(1);
                                    DateTime OrderDate = Convert.ToDateTime(dtprodRecord.Rows[0]["OrderDate"]);
                                    if (CurrentDate.Day == OrderDate.Day && CurrentDate.Month == OrderDate.Month && CurrentDate.Year == OrderDate.Year)
                                    {
                                        var timming = objproduct.CheckProductOrderTimimg(obj.ProductId);
                                        if (timming.IsTime == false)
                                        {
                                            dr["status"] = "Fail";
                                            dr["error_msg"] = "Unable to update... - " + timming.message;
                                            dtNew.Rows.Add(dr);
                                            return Ok(dtNew);
                                        }
                                    }
                                }

                                if (dtProduct.Rows.Count > 0)
                                {
                                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SalePrice"].ToString()))
                                        obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]) * obj.Qty;
                                    else
                                        obj.Amount = 0;
                                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountAmount"].ToString()))
                                        obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountAmount"]) * obj.Qty;
                                    else
                                        obj.Discount = 0;


                                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                                        obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                                    else
                                        obj.CGSTPerct = 0;
                                    //count cgst Amount
                                    if (obj.CGSTPerct > 0)
                                        obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                                    else
                                        obj.CGSTAmount = 0;

                                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                                        obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                                    else
                                        obj.IGSTPerct = 0;
                                    //count igst Amount
                                    if (obj.IGSTPerct > 0)
                                        obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                                    else
                                        obj.IGSTAmount = 0;

                                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                                        obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                                    else
                                        obj.SGSTPerct = 0;
                                    //count sgst Amount
                                    if (obj.SGSTPerct > 0)
                                        obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                                    else
                                        obj.SGSTAmount = 0;

                                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                                        obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                                    else
                                        obj.RewardPoint = 0;
                                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                                        obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                                    else
                                        obj.Profit = 0;

                                    //Final Amount
                                    obj.TotalFinalAmount = obj.Amount - obj.Discount;
                                }
                                //update order main
                                obj.Status = "Pending";

                                obj.TotalAmount = obj.TotalFinalAmount;
                                UpdateProductOrder = obj.UpdateCustomerOrderMobile(obj);

                                //update item order
                                obj.OrderId = item.Id;
                                UpdateAddProductDetail = obj.UpdateCustomerOrderDetailMobile(obj);
                            }
                            else
                            {
                                dr["status"] = "Success";
                                dr["error_msg"] = "Order Not Found";
                            }
                            if (UpdateAddProductDetail > 0)
                            {
                                dr["status"] = "Success";
                                dr["error_msg"] = "Order Updated Successfully";
                            }
                            else
                            {
                                dr["status"] = "Success";
                                dr["error_msg"] = "Order Not Updated";
                            }
                        }
                        else
                        {
                            dr["status"] = "Success";
                            dr["error_msg"] = "Order Not Found";
                        }
                    //}
                    //else
                    //{
                    //    dr["status"] = "Failed";
                    //    dr["error_msg"] = "Wallet balance is Low!!Can't Update Qty";
                    //}
                //}
            }
            else if (!string.IsNullOrEmpty(item.Id.ToString()) && item.Id != 0 && !string.IsNullOrEmpty(item.Qty.ToString()) && item.Qty == 0)
            {
                obj.Id = item.Id;
                obj.Qty = item.Qty;
                //get OrderId from parent to remove child records
                DataTable dtprodRecord = new DataTable();
                dtprodRecord = obj.getCustomerOnedateOrderList(obj.Id);
                int userRecords = dtprodRecord.Rows.Count;
                obj.OrderId = 0;

                if (userRecords > 0)
                {
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["OrderId"].ToString()))
                    {
                        obj.OrderId = Convert.ToInt32(dtprodRecord.Rows[0]["OrderId"]);

                        //get Item 
                        obj.ProductId = Convert.ToInt32(dtprodRecord.Rows[0]["ProductId"]);
                        DataTable dtProduct = objproduct.BindProuct(obj.ProductId);
                        if (dtProduct.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["OrderDate"].ToString()))
                            {
                                DateTime CurrentDate = Helper.indianTime.AddDays(1);
                                DateTime OrderDate = Convert.ToDateTime(dtprodRecord.Rows[0]["OrderDate"]);
                                if (CurrentDate.Day == OrderDate.Day && CurrentDate.Month == OrderDate.Month && CurrentDate.Year == OrderDate.Year)
                                {
                                    var timming = objproduct.CheckProductOrderTimimg(obj.ProductId);
                                    if (timming.IsTime == false)
                                    {
                                        dr["status"] = "Fail";
                                        dr["error_msg"] = "Unable to update... - " + timming.message;
                                        dtNew.Rows.Add(dr);
                                        return Ok(dtNew);
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SalePrice"].ToString()))
                                obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
                            else
                                obj.Amount = 0;
                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountAmount"].ToString()))
                                obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountAmount"]);
                            else
                                obj.Discount = 0;


                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                                obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]);
                            else
                                obj.CGSTPerct = 0;
                            //count cgst Amount
                            if (obj.CGSTPerct > 0)
                                obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                            else
                                obj.CGSTAmount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                                obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]);
                            else
                                obj.IGSTPerct = 0;
                            //count igst Amount
                            if (obj.IGSTPerct > 0)
                                obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                            else
                                obj.IGSTAmount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                                obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]);
                            else
                                obj.SGSTPerct = 0;
                            //count sgst Amount
                            if (obj.SGSTPerct > 0)
                                obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                            else
                                obj.SGSTAmount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                                obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]);
                            else
                                obj.RewardPoint = 0;
                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                                obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]);
                            else
                                obj.Profit = 0;

                            //Final Amount
                            obj.TotalFinalAmount = obj.Amount - obj.Discount;
                        }
                        //update order main
                        obj.Status = "Cancel";

                        obj.TotalAmount = obj.TotalFinalAmount;
                        UpdateProductOrder = obj.UpdateCustomerOrderMobile(obj);

                        //update item order
                        obj.OrderId = item.Id;
                        UpdateAddProductDetail = obj.UpdateCustomerOrderDetailMobile(obj);
                    }
                    else
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Not Found";
                    }
                    if (UpdateAddProductDetail > 0)
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Cancelled Successfully";
                    }
                    else
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Not Cancelled";
                    }
                }
                else
                {
                    dr["status"] = "Success";
                    dr["error_msg"] = "Order Not Found";
                }
            }
            else
            {
                dr["status"] = "Failed";
                dr["error_msg"] = "Please Fill Correct Details";
            }
            dtNew.Rows.Add(dr);
            return Ok(dtNew);
        }
        #endregion

        [Route("api/OrderNewApi/AddCustomerOrderalternate/{customerid?}/{productid?}/{qty?}/{startdate?}/{AttributeId?}/{VendorId?}/{VendorcatId?}/{SectorId?}/{DmId?}/{CusType?}")]
        [HttpGet]
        public IHttpActionResult AddCustomerOrderalternate(string customerid, string productid, string qty, DateTime? startdate, string AttributeId, string VendorId, string VendorcatId,string SectorId,string DmId,string CusType)//string strjson
        {
            //Get CurrentTime

            DateTime centuryBegin = new DateTime(2001, 1, 1);
            var currentDate = Helper.indianTime;
            long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
            TimeSpan elapsedSpan = TimeSpan.Parse(currentDate.ToString("HH:mm"));
            string curhour = elapsedSpan.ToString();
            curhour = curhour.Substring(0, 2);

            //
            decimal minorderamount = 0,dailyorderamount=0;
            decimal credit = 0;
            decimal Walletbal = 0, TotalCredit = 0, TotalDebit=0;
            Vendor objvendor = new Vendor();
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();


            DataTable dtNew = new DataTable();

            DataTable dtVendor = new DataTable();

            dtVendor = objvendor.getVendorid(Convert.ToInt32(VendorcatId));

            if(dtVendor.Rows.Count>0)
            {
                minorderamount = Convert.ToDecimal(dtVendor.Rows[0]["MinAmount"]);
            }
           

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            long AddProductOrder = 0; int AddProductDetail = 0;
            if (!string.IsNullOrEmpty(customerid) && Convert.ToInt32(customerid) != 0 && !string.IsNullOrEmpty(productid) && Convert.ToInt32(productid) != 0 && !string.IsNullOrEmpty(qty) && Convert.ToInt32(qty) != 0  && !string.IsNullOrEmpty(startdate.ToString()))
            {
                int r = 0;
                string msg = "Order Placed Successfully";
                string msg1 = "";
                DataTable dtcuttime = objcust.GetSchedularTime(null);
                int dbcutOfftime = Convert.ToInt32(dtcuttime.Rows[0]["CutOffTime"]);
                DateTime FromDate =Convert.ToDateTime(startdate);
                DateTime FromDate1 = Helper.indianTime;
                FromDate1 = FromDate1.AddDays(1);
                //if (DateTime.Now.Hour < dbcutOfftime)
                if (Convert.ToInt32(curhour) < dbcutOfftime)
                {
                    FromDate = Convert.ToDateTime(startdate);

                    //Find daily order total
                    DataTable dtprodRecord = new DataTable();
                    dtprodRecord = obj.getCustomerOrderFuture(Convert.ToInt32(customerid), FromDate, FromDate);
                    if(dtprodRecord.Rows.Count>0)
                    {
                        for(int i=0;i<dtprodRecord.Rows.Count;i++)
                        {
                            dailyorderamount = dailyorderamount + Convert.ToDecimal(dtprodRecord.Rows[i]["Amount"]);
                        }
                    }
                    
                    //
                    r = FromDate.Date.CompareTo(FromDate1.Date);
                    if (r == 0)
                    {
                        //var timming = objproduct.CheckProductOrderTimimg(Convert.ToInt32(productid));

                        var timming = objproduct.CheckProductOrderTimimgNew(Convert.ToInt32(productid), Convert.ToInt32(VendorcatId));
                        if (timming.IsTime == false)
                        {
                            //dr["status"] = "Fail";
                            //dr["error_msg"] = timming.message;
                            //dtNew.Rows.Add(dr);
                            //return Ok(dtNew);
                            FromDate = FromDate.AddDays(2);
                            msg = timming.message + "So your order successfully placed on " + FromDate.Date;
                        }
                    }
                }
                else
                    FromDate = FromDate.AddDays(2);

                DateTime ToDate = FromDate;


                if (!string.IsNullOrEmpty(customerid))
                { obj.CustomerId = Convert.ToInt32(customerid); }
                else { obj.CustomerId = 0; }

         
                    
                DataTable dtDupliAssign = objcust.DuplicateStaffCustomer(null, obj.CustomerId);
                if (dtDupliAssign.Rows.Count == 0)
                {
                    dr["status"] = "Fail";
                    dr["error_msg"] = "Deliveryboy not assign...";
                    dtNew.Rows.Add(dr);
                    return Ok(dtNew);
                }
                else
                { obj.DeliveryBoyId = dtDupliAssign.Rows[0]["StaffId"].ToString(); }


                int societyid = 0;

                ToDate = Helper.indianTime.AddMonths(2);

                DataTable dtBuildingId = objcust.BindCustomer(obj.CustomerId);

                if (dtBuildingId.Rows.Count > 0)
                {
                    //if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["BuildingId"].ToString()))
                    //    societyid = Convert.ToInt32(dtBuildingId.Rows[0]["BuildingId"]);

                    societyid = 0;
                    if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["IsActive"].ToString()))
                    {
                        if (dtBuildingId.Rows[0]["IsActive"].ToString() == "False")
                        {
                            dr["status"] = "Fail";
                            dr["error_msg"] = "Account Not Active...";
                            dtNew.Rows.Add(dr);
                            return Ok(dtNew);
                        }
                    }
                }



                if (!string.IsNullOrEmpty(societyid.ToString()))
                { obj.BuildingId = Convert.ToInt32(societyid); }
                else { obj.BuildingId = 0; }

                obj.Qty = Convert.ToInt32(qty);


                obj.ProductId = Convert.ToInt32(productid);
                
                //get Product detail
                DataTable dtProduct = objproduct.BindProuctOrder(obj.ProductId,Convert.ToInt32(AttributeId),Convert.ToInt32(VendorId), Convert.ToInt32(VendorcatId));
                if (dtProduct.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["ProductID"].ToString()))
                        productid = dtProduct.Rows[0]["ProductID"].ToString();

                    if(string.IsNullOrEmpty(CusType) || CusType== "General")
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SellPrice"].ToString()))
                        {
                            obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]) * obj.Qty;
                            obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]);
                        }
                        else
                        {
                            obj.Amount = 0;
                            obj.SalePrice = 0;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["B2BSellPrice"].ToString()))
                        {
                            obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["B2BSellPrice"]) * obj.Qty;
                            obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["B2BSellPrice"]);
                        }
                        else
                        {
                            obj.Amount = 0;
                            obj.SalePrice = 0;
                        }
                    }
                    

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["PurchasePrice"].ToString()))
                    {
                        obj.PurchasePrice = Convert.ToDecimal(dtProduct.Rows[0]["PurchasePrice"]) * obj.Qty;

                    }
                    else
                    {
                        obj.PurchasePrice = 0;

                    }

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["MRPPrice"].ToString()))
                    {
                        obj.MRPPrice = Convert.ToDecimal(dtProduct.Rows[0]["MRPPrice"]) * obj.Qty;

                    }
                    else
                    {
                        obj.MRPPrice = 0;

                    }
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountPrice"].ToString()))
                        obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountPrice"]) * obj.Qty;
                    else
                        obj.Discount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                        obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                    else
                        obj.CGSTPerct = 0;
                    //count cgst Amount
                    if (obj.CGSTPerct > 0)
                        obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                    else
                        obj.CGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                        obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                    else
                        obj.IGSTPerct = 0;
                    //count igst Amount
                    if (obj.IGSTPerct > 0)
                        obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                    else
                        obj.IGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                        obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                    else
                        obj.SGSTPerct = 0;
                    //count sgst Amount
                    if (obj.SGSTPerct > 0)
                        obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                    else
                        obj.SGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                        obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                    else
                        obj.RewardPoint = 0;
                    if (string.IsNullOrEmpty(CusType) || CusType == "General")
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                            obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                        else
                            obj.Profit = 0;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["B2BProfit"].ToString()))
                            obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["B2BProfit"]) * obj.Qty;
                        else
                            obj.Profit = 0;
                    }
                       

                    //Final Amount
                    obj.TotalFinalAmount = obj.Amount;
                }

                if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                { obj.TotalAmount = obj.TotalFinalAmount; }
                else { obj.TotalAmount = 0; }

                obj.TotalGSTAmt = obj.CGSTAmount + obj.SGSTAmount;

                obj.Status = "Pending";
                obj.StateCode = null;
                obj.ProductId = Convert.ToInt32(productid);

                obj.OrderFlag = "Alternate";
                //var checkAmount = objproduct.CheckProductOrderAmountNew(Convert.ToInt32(productid), obj.TotalAmount);
                //if (checkAmount.IsOrderAmount == false)
                //{
                //    dr["status"] = "Fail";
                //    dr["error_msg"] = checkAmount.message;
                //    dtNew.Rows.Add(dr);
                //    return Ok(dtNew);
                //}


                //Calculate wallet with Order Amount 02-01-2022
                DataTable dt = new DataTable();
                dt = obj.GetCustomerCredit(obj.CustomerId);

                if (dt.Rows.Count > 0)
                {
                    credit = Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString());
                }
                DataTable dtprodRecord1 = obj.getCustomerWallet(obj.CustomerId);
                int userRecords = dtprodRecord1.Rows.Count;
                if (userRecords > 0)
                {
                    if (!string.IsNullOrEmpty(dtprodRecord1.Rows[0]["Amt"].ToString()))
                        TotalCredit = Convert.ToDecimal(dtprodRecord1.Rows[0]["Amt"]);
                    if (userRecords > 1)
                    {
                        if (!string.IsNullOrEmpty(dtprodRecord1.Rows[1]["Amt"].ToString()))
                            TotalDebit = Convert.ToDecimal(dtprodRecord1.Rows[1]["Amt"]);
                    }
                    Walletbal = TotalCredit - TotalDebit;
                }

                decimal sumamount = dailyorderamount + obj.TotalAmount;
                if ((dailyorderamount+ obj.TotalAmount)<minorderamount)
                {
                    if((dailyorderamount + obj.TotalAmount)<(Walletbal+credit))
                    {
                        msg1 = "Minimum Order Value for this Category is "+minorderamount;
                    }

                    if ((dailyorderamount + obj.TotalAmount) > (Walletbal + credit))
                    {
                        msg1 += ".wallet balance is lower than order value please upload balance before 10:00 PM to process this order ";
                    }
                }

                if ((dailyorderamount + obj.TotalAmount) > minorderamount)
                {
                    if ((dailyorderamount + obj.TotalAmount) > (Walletbal + credit))
                    {
                        msg1 = "wallet balance is lower than order value please upload balance before 10:00 PM to process this order ";
                    }
                }

                //send notification
                if (msg1 != "")
                {
                    Customer objcustomer = new Customer();
                    Customer objLogin = new Customer();
                    DataTable dtToken = new DataTable();
                    string customerId = obj.CustomerId.ToString();
                    dtToken = objLogin.getCustomerSector(Convert.ToInt32(customerId));
                    if (dtToken.Rows.Count > 0)
                    {
                        objcustomer.UserName = dtToken.Rows[0].ItemArray[1].ToString();
                        objcustomer.SectorId = Convert.ToInt32(dtToken.Rows[0].ItemArray[0]);
                    }

                    string type1 = "Customer";

                    objcustomer.ntitle = "Related Order";
                    objcustomer.ntext = msg1;
                    objcustomer.Photo = "";
                    objcustomer.nlink = "";
                    var addresult = objcustomer.InsertNotification(objcustomer, type1);


                    //DataTable dtToken = new DataTable();

                    int cusnot = 0;

                    objcustomer.nid = addresult.ToString();

                    cusnot = objcustomer.InsertCustomerNotification(objcustomer);

                }
                //

                DateTime _fromDate = FromDate;
                for (int idate = 0; FromDate <= ToDate; idate++)
                {
                    //Generate OrderNo
                    con.Open();
                    SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                    com1.CommandType = CommandType.StoredProcedure;
                    var Formno = com1.ExecuteScalar();
                    con.Close();

                    obj.OrderNo = Convert.ToInt32(Formno);
                    obj.OrderDate = FromDate;

                    //get Subscription Id
                    DataTable dtCustSubscription = obj.getCustomerSubscriptionOrderdate(obj);
                    if (dtCustSubscription.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                            obj.CustSubscriptionId = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                        else
                            obj.CustSubscriptionId = 0;
                    }
                    else
                        obj.CustSubscriptionId = 0;


                    obj.AttributeId = Convert.ToInt32(AttributeId);
                    obj.VendorId = Convert.ToInt32(VendorId);
                    obj.VendorCatId = Convert.ToInt32(VendorcatId);
                    obj.SectorId = Convert.ToInt32(SectorId);
                    AddProductOrder = obj.InsertCustomerOrder(obj);

                    if (AddProductOrder > 0)
                    {
                        obj.OrderId = Convert.ToInt32(AddProductOrder);
                        obj.ProductId = Convert.ToInt32(productid);
                        obj.Qty = Convert.ToInt32(qty);
                        obj.OrderItemDate = FromDate;

                        AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                    }
                    FromDate = FromDate.AddDays(2);

                    
                }
                ToDate = FromDate.AddDays(-2);

                if (AddProductDetail > 0)
                {
                    dr["status"] = "Success";
                    dr["error_msg"] = msg.ToString()+msg1.ToString();
                    string OrderFlag = "Alternate";
                    Helper dHelper = new Helper();
                    // dHelper.InsertCustomerOrderTrackNew(obj.CustomerId, obj.ProductId, obj.Qty, _fromDate, ToDate,OrderFlag);

                    dHelper.InsertCustomerOrderTrackNeworder(obj.CustomerId, obj.ProductId, obj.Qty, _fromDate, ToDate, OrderFlag,Convert.ToInt32(AttributeId));
                    dtNew.Rows.Add(dr);
                    return Ok(dtNew);
                }
                else
                {
                    dr["status"] = "Fail";
                    dr["error_msg"] = "Order Not Inserted.";
                }
            }
            else
            {
                dr["status"] = "Failed";
                dr["error_msg"] = "Please Fill Correct Details";
            }


            return Ok(dtNew);
        }


        //Work Week Day Order Start old
        [Route("api/OrderNewApi/AddCustomerOrderweeklyday/{customerid?}/{productid?}/{qty?}/{firstdate?}/{seconddate?}/{thirddate?}/{fourthdate?}/{fifthdate?}/{sixthdate?}/{seventhdate?}")]
        [HttpGet]
        public IHttpActionResult AddCustomerOrderweeklyday(string customerid, string productid, string qty, string firstdate, string seconddate, string thirddate, string fourthdate, string fifthdate,string sixthdate, string seventhdate)//string strjson
        {
            

            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();


            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            long AddProductOrder = 0; int AddProductDetail = 0;

            if (!string.IsNullOrEmpty(customerid) && Convert.ToInt32(customerid) != 0 && !string.IsNullOrEmpty(productid) && Convert.ToInt32(productid) != 0 && !string.IsNullOrEmpty(qty) && Convert.ToInt32(qty) != 0)
            {

                if(!string.IsNullOrEmpty(firstdate.ToString()) && firstdate.ToString()!="0")
                {
                    DataTable dtcuttime = objcust.GetSchedularTime(null);
                    int dbcutOfftime = Convert.ToInt32(dtcuttime.Rows[0]["CutOffTime"]);
                    DateTime FromDate = Convert.ToDateTime(firstdate);

                    if (DateTime.Now.Hour < dbcutOfftime)
                    {
                        FromDate = Convert.ToDateTime(firstdate);
                    }
                    else
                        FromDate = FromDate.AddDays(1);

                    DateTime ToDate = FromDate;

                    if (!string.IsNullOrEmpty(customerid))
                    { obj.CustomerId = Convert.ToInt32(customerid); }
                    else { obj.CustomerId = 0; }

                    var timming = objproduct.CheckProductOrderTimimg(Convert.ToInt32(productid));
                    if (timming.IsTime == false)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = timming.message;
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }
                    DataTable dtDupliAssign = objcust.DuplicateStaffCustomer(null, obj.CustomerId);
                    if (dtDupliAssign.Rows.Count == 0)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Deliveryboy not assign...";
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }
                    int societyid = 0;

                    ToDate = FromDate.AddMonths(2);

                    DataTable dtBuildingId = objcust.BindCustomer(obj.CustomerId);

                    if (dtBuildingId.Rows.Count > 0)
                    {
                        //if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["BuildingId"].ToString()))
                        //    societyid = Convert.ToInt32(dtBuildingId.Rows[0]["BuildingId"]);

                        societyid = 0;
                        if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["IsActive"].ToString()))
                        {
                            if (dtBuildingId.Rows[0]["IsActive"].ToString() == "False")
                            {
                                dr["status"] = "Fail";
                                dr["error_msg"] = "Account Not Active...";
                                dtNew.Rows.Add(dr);
                                return Ok(dtNew);
                            }
                        }
                    }



                    if (!string.IsNullOrEmpty(societyid.ToString()))
                    { obj.BuildingId = Convert.ToInt32(societyid); }
                    else { obj.BuildingId = 0; }

                    obj.Qty = Convert.ToInt32(qty);


                    obj.ProductId = Convert.ToInt32(productid);
                    //get Product detail
                    DataTable dtProduct = objproduct.BindProuct(obj.ProductId);

                    if (dtProduct.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Id"].ToString()))
                            productid = dtProduct.Rows[0]["Id"].ToString();
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SalePrice"].ToString()))
                        {
                            obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]) * obj.Qty;
                            obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
                        }
                        else
                        {
                            obj.Amount = 0;
                            obj.SalePrice = 0;
                        }
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountAmount"].ToString()))
                            obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountAmount"]) * obj.Qty;
                        else
                            obj.Discount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                            obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                        else
                            obj.CGSTPerct = 0;
                        //count cgst Amount
                        if (obj.CGSTPerct > 0)
                            obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                        else
                            obj.CGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                            obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                        else
                            obj.IGSTPerct = 0;
                        //count igst Amount
                        if (obj.IGSTPerct > 0)
                            obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                        else
                            obj.IGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                            obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                        else
                            obj.SGSTPerct = 0;
                        //count sgst Amount
                        if (obj.SGSTPerct > 0)
                            obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                        else
                            obj.SGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                            obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                        else
                            obj.RewardPoint = 0;
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                            obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                        else
                            obj.Profit = 0;

                        //Final Amount
                        obj.TotalFinalAmount = obj.Amount;
                    }

                    if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                    { obj.TotalAmount = obj.TotalFinalAmount; }
                    else { obj.TotalAmount = 0; }

                    obj.TotalGSTAmt = obj.CGSTAmount + obj.SGSTAmount;

                    obj.Status = "Pending";
                    obj.StateCode = null;
                    obj.ProductId = Convert.ToInt32(productid);

                    obj.OrderFlag = "WeekDay";
                    var checkAmount = objproduct.CheckProductOrderAmount(Convert.ToInt32(productid), obj.TotalAmount);
                    if (checkAmount.IsOrderAmount == false)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = checkAmount.message;
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }

                    DateTime _fromDate = FromDate;
                    for (int idate = 0; FromDate <= ToDate; idate++)
                    {
                        //Generate OrderNo
                        con.Open();
                        SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                        com1.CommandType = CommandType.StoredProcedure;
                        var Formno = com1.ExecuteScalar();
                        con.Close();

                        obj.OrderNo = Convert.ToInt32(Formno);
                        obj.OrderDate = FromDate;

                        //get Subscription Id
                        DataTable dtCustSubscription = obj.getCustomerSubscriptionOrderdate(obj);
                        if (dtCustSubscription.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                                obj.CustSubscriptionId = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                            else
                                obj.CustSubscriptionId = 0;
                        }
                        else
                            obj.CustSubscriptionId = 0;

                        AddProductOrder = obj.InsertCustomerOrder(obj);

                        if (AddProductOrder > 0)
                        {
                            obj.OrderId = Convert.ToInt32(AddProductOrder);
                            obj.ProductId = Convert.ToInt32(productid);
                            obj.Qty = Convert.ToInt32(qty);
                            obj.OrderItemDate = FromDate;

                            AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                        }
                        FromDate = FromDate.AddDays(7);


                    }
                    ToDate = FromDate.AddDays(-7);

                    if (AddProductDetail > 0)
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Placed Successfully";
                        string OrderFlag = "WeekDay";
                        Helper dHelper = new Helper();
                        dHelper.InsertCustomerOrderTrackNew(obj.CustomerId, obj.ProductId, obj.Qty, _fromDate, ToDate,OrderFlag);

                        
                    }
                    else
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Order Not Inserted.";
                    }
                
                //else
                //{
                //    dr["status"] = "Failed";
                //    dr["error_msg"] = "Please Fill Correct Details";
                //}

            }

                if (!string.IsNullOrEmpty(seconddate.ToString()) && seconddate.ToString() != "0")
                {
                    DataTable dtcuttime = objcust.GetSchedularTime(null);
                    int dbcutOfftime = Convert.ToInt32(dtcuttime.Rows[0]["CutOffTime"]);
                    DateTime FromDate = Convert.ToDateTime(seconddate);

                    if (DateTime.Now.Hour < dbcutOfftime)
                    {
                        FromDate = Convert.ToDateTime(seconddate);
                    }
                    else
                        FromDate = FromDate.AddDays(1);

                    DateTime ToDate = FromDate;

                    if (!string.IsNullOrEmpty(customerid))
                    { obj.CustomerId = Convert.ToInt32(customerid); }
                    else { obj.CustomerId = 0; }

                    var timming = objproduct.CheckProductOrderTimimg(Convert.ToInt32(productid));
                    if (timming.IsTime == false)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = timming.message;
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }
                    DataTable dtDupliAssign = objcust.DuplicateStaffCustomer(null, obj.CustomerId);
                    if (dtDupliAssign.Rows.Count == 0)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Deliveryboy not assign...";
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }
                    int societyid = 0;

                    ToDate = FromDate.AddMonths(2);

                    DataTable dtBuildingId = objcust.BindCustomer(obj.CustomerId);

                    if (dtBuildingId.Rows.Count > 0)
                    {
                        //if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["BuildingId"].ToString()))
                        //    societyid = Convert.ToInt32(dtBuildingId.Rows[0]["BuildingId"]);

                        societyid = 0;
                        if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["IsActive"].ToString()))
                        {
                            if (dtBuildingId.Rows[0]["IsActive"].ToString() == "False")
                            {
                                dr["status"] = "Fail";
                                dr["error_msg"] = "Account Not Active...";
                                dtNew.Rows.Add(dr);
                                return Ok(dtNew);
                            }
                        }
                    }



                    if (!string.IsNullOrEmpty(societyid.ToString()))
                    { obj.BuildingId = Convert.ToInt32(societyid); }
                    else { obj.BuildingId = 0; }

                    obj.Qty = Convert.ToInt32(qty);


                    obj.ProductId = Convert.ToInt32(productid);
                    //get Product detail
                    DataTable dtProduct = objproduct.BindProuct(obj.ProductId);

                    if (dtProduct.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Id"].ToString()))
                            productid = dtProduct.Rows[0]["Id"].ToString();
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SalePrice"].ToString()))
                        {
                            obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]) * obj.Qty;
                            obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
                        }
                        else
                        {
                            obj.Amount = 0;
                            obj.SalePrice = 0;
                        }
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountAmount"].ToString()))
                            obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountAmount"]) * obj.Qty;
                        else
                            obj.Discount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                            obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                        else
                            obj.CGSTPerct = 0;
                        //count cgst Amount
                        if (obj.CGSTPerct > 0)
                            obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                        else
                            obj.CGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                            obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                        else
                            obj.IGSTPerct = 0;
                        //count igst Amount
                        if (obj.IGSTPerct > 0)
                            obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                        else
                            obj.IGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                            obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                        else
                            obj.SGSTPerct = 0;
                        //count sgst Amount
                        if (obj.SGSTPerct > 0)
                            obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                        else
                            obj.SGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                            obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                        else
                            obj.RewardPoint = 0;
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                            obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                        else
                            obj.Profit = 0;

                        //Final Amount
                        obj.TotalFinalAmount = obj.Amount;
                    }

                    if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                    { obj.TotalAmount = obj.TotalFinalAmount; }
                    else { obj.TotalAmount = 0; }

                    obj.TotalGSTAmt = obj.CGSTAmount + obj.SGSTAmount;

                    obj.Status = "Pending";
                    obj.StateCode = null;
                    obj.ProductId = Convert.ToInt32(productid);

                    obj.OrderFlag = "WeekDay";
                    var checkAmount = objproduct.CheckProductOrderAmount(Convert.ToInt32(productid), obj.TotalAmount);
                    if (checkAmount.IsOrderAmount == false)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = checkAmount.message;
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }

                    DateTime _fromDate = FromDate;
                    for (int idate = 0; FromDate <= ToDate; idate++)
                    {
                        //Generate OrderNo
                        con.Open();
                        SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                        com1.CommandType = CommandType.StoredProcedure;
                        var Formno = com1.ExecuteScalar();
                        con.Close();

                        obj.OrderNo = Convert.ToInt32(Formno);
                        obj.OrderDate = FromDate;

                        //get Subscription Id
                        DataTable dtCustSubscription = obj.getCustomerSubscriptionOrderdate(obj);
                        if (dtCustSubscription.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                                obj.CustSubscriptionId = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                            else
                                obj.CustSubscriptionId = 0;
                        }
                        else
                            obj.CustSubscriptionId = 0;

                        AddProductOrder = obj.InsertCustomerOrder(obj);

                        if (AddProductOrder > 0)
                        {
                            obj.OrderId = Convert.ToInt32(AddProductOrder);
                            obj.ProductId = Convert.ToInt32(productid);
                            obj.Qty = Convert.ToInt32(qty);
                            obj.OrderItemDate = FromDate;

                            AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                        }
                        FromDate = FromDate.AddDays(7);


                    }
                    ToDate = FromDate.AddDays(-7);

                    if (AddProductDetail > 0)
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Placed Successfully";
                        string OrderFlag = "WeekDay";
                        Helper dHelper = new Helper();
                        dHelper.InsertCustomerOrderTrackNew(obj.CustomerId, obj.ProductId, obj.Qty, _fromDate, ToDate,OrderFlag);
                    }
                    else
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Order Not Inserted.";
                    }
                }

                if (!string.IsNullOrEmpty(thirddate.ToString()) && thirddate.ToString() != "0")
                {
                    DataTable dtcuttime = objcust.GetSchedularTime(null);
                    int dbcutOfftime = Convert.ToInt32(dtcuttime.Rows[0]["CutOffTime"]);
                    DateTime FromDate = Convert.ToDateTime(thirddate);

                    if (DateTime.Now.Hour < dbcutOfftime)
                    {
                        FromDate = Convert.ToDateTime(thirddate);
                    }
                    else
                        FromDate = FromDate.AddDays(1);

                    DateTime ToDate = FromDate;

                    if (!string.IsNullOrEmpty(customerid))
                    { obj.CustomerId = Convert.ToInt32(customerid); }
                    else { obj.CustomerId = 0; }

                    var timming = objproduct.CheckProductOrderTimimg(Convert.ToInt32(productid));
                    if (timming.IsTime == false)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = timming.message;
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }
                    DataTable dtDupliAssign = objcust.DuplicateStaffCustomer(null, obj.CustomerId);
                    if (dtDupliAssign.Rows.Count == 0)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Deliveryboy not assign...";
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }
                    int societyid = 0;

                    ToDate = FromDate.AddMonths(2);

                    DataTable dtBuildingId = objcust.BindCustomer(obj.CustomerId);

                    if (dtBuildingId.Rows.Count > 0)
                    {
                        //if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["BuildingId"].ToString()))
                        //    societyid = Convert.ToInt32(dtBuildingId.Rows[0]["BuildingId"]);

                        societyid = 0;
                        if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["IsActive"].ToString()))
                        {
                            if (dtBuildingId.Rows[0]["IsActive"].ToString() == "False")
                            {
                                dr["status"] = "Fail";
                                dr["error_msg"] = "Account Not Active...";
                                dtNew.Rows.Add(dr);
                                return Ok(dtNew);
                            }
                        }
                    }



                    if (!string.IsNullOrEmpty(societyid.ToString()))
                    { obj.BuildingId = Convert.ToInt32(societyid); }
                    else { obj.BuildingId = 0; }

                    obj.Qty = Convert.ToInt32(qty);


                    obj.ProductId = Convert.ToInt32(productid);
                    //get Product detail
                    DataTable dtProduct = objproduct.BindProuct(obj.ProductId);

                    if (dtProduct.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Id"].ToString()))
                            productid = dtProduct.Rows[0]["Id"].ToString();
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SalePrice"].ToString()))
                        {
                            obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]) * obj.Qty;
                            obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
                        }
                        else
                        {
                            obj.Amount = 0;
                            obj.SalePrice = 0;
                        }
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountAmount"].ToString()))
                            obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountAmount"]) * obj.Qty;
                        else
                            obj.Discount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                            obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                        else
                            obj.CGSTPerct = 0;
                        //count cgst Amount
                        if (obj.CGSTPerct > 0)
                            obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                        else
                            obj.CGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                            obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                        else
                            obj.IGSTPerct = 0;
                        //count igst Amount
                        if (obj.IGSTPerct > 0)
                            obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                        else
                            obj.IGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                            obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                        else
                            obj.SGSTPerct = 0;
                        //count sgst Amount
                        if (obj.SGSTPerct > 0)
                            obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                        else
                            obj.SGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                            obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                        else
                            obj.RewardPoint = 0;
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                            obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                        else
                            obj.Profit = 0;

                        //Final Amount
                        obj.TotalFinalAmount = obj.Amount;
                    }

                    if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                    { obj.TotalAmount = obj.TotalFinalAmount; }
                    else { obj.TotalAmount = 0; }

                    obj.TotalGSTAmt = obj.CGSTAmount + obj.SGSTAmount;

                    obj.Status = "Pending";
                    obj.StateCode = null;
                    obj.ProductId = Convert.ToInt32(productid);

                    obj.OrderFlag = "WeekDay";
                    var checkAmount = objproduct.CheckProductOrderAmount(Convert.ToInt32(productid), obj.TotalAmount);
                    if (checkAmount.IsOrderAmount == false)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = checkAmount.message;
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }

                    DateTime _fromDate = FromDate;
                    for (int idate = 0; FromDate <= ToDate; idate++)
                    {
                        //Generate OrderNo
                        con.Open();
                        SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                        com1.CommandType = CommandType.StoredProcedure;
                        var Formno = com1.ExecuteScalar();
                        con.Close();

                        obj.OrderNo = Convert.ToInt32(Formno);
                        obj.OrderDate = FromDate;

                        //get Subscription Id
                        DataTable dtCustSubscription = obj.getCustomerSubscriptionOrderdate(obj);
                        if (dtCustSubscription.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                                obj.CustSubscriptionId = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                            else
                                obj.CustSubscriptionId = 0;
                        }
                        else
                            obj.CustSubscriptionId = 0;

                        AddProductOrder = obj.InsertCustomerOrder(obj);

                        if (AddProductOrder > 0)
                        {
                            obj.OrderId = Convert.ToInt32(AddProductOrder);
                            obj.ProductId = Convert.ToInt32(productid);
                            obj.Qty = Convert.ToInt32(qty);
                            obj.OrderItemDate = FromDate;

                            AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                        }
                        FromDate = FromDate.AddDays(7);


                    }

                    ToDate = FromDate.AddDays(-7);
                    if (AddProductDetail > 0)
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Placed Successfully";
                        string OrderFlag = "WeekDay";
                        Helper dHelper = new Helper();
                        dHelper.InsertCustomerOrderTrackNew(obj.CustomerId, obj.ProductId, obj.Qty, _fromDate, ToDate,OrderFlag);
                    }
                    else
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Order Not Inserted.";
                    }
                }

                if (!string.IsNullOrEmpty(fourthdate.ToString()) && fourthdate.ToString() != "0")
                {
                    DataTable dtcuttime = objcust.GetSchedularTime(null);
                    int dbcutOfftime = Convert.ToInt32(dtcuttime.Rows[0]["CutOffTime"]);
                    DateTime FromDate = Convert.ToDateTime(fourthdate);

                    if (DateTime.Now.Hour < dbcutOfftime)
                    {
                        FromDate = Convert.ToDateTime(fourthdate);
                    }
                    else
                        FromDate = FromDate.AddDays(1);

                    DateTime ToDate = FromDate;

                    if (!string.IsNullOrEmpty(customerid))
                    { obj.CustomerId = Convert.ToInt32(customerid); }
                    else { obj.CustomerId = 0; }

                    var timming = objproduct.CheckProductOrderTimimg(Convert.ToInt32(productid));
                    if (timming.IsTime == false)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = timming.message;
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }
                    DataTable dtDupliAssign = objcust.DuplicateStaffCustomer(null, obj.CustomerId);
                    if (dtDupliAssign.Rows.Count == 0)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Deliveryboy not assign...";
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }
                    int societyid = 0;

                    ToDate = FromDate.AddMonths(2);

                    DataTable dtBuildingId = objcust.BindCustomer(obj.CustomerId);

                    if (dtBuildingId.Rows.Count > 0)
                    {
                        //if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["BuildingId"].ToString()))
                        //    societyid = Convert.ToInt32(dtBuildingId.Rows[0]["BuildingId"]);

                        societyid = 0;
                        if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["IsActive"].ToString()))
                        {
                            if (dtBuildingId.Rows[0]["IsActive"].ToString() == "False")
                            {
                                dr["status"] = "Fail";
                                dr["error_msg"] = "Account Not Active...";
                                dtNew.Rows.Add(dr);
                                return Ok(dtNew);
                            }
                        }
                    }



                    if (!string.IsNullOrEmpty(societyid.ToString()))
                    { obj.BuildingId = Convert.ToInt32(societyid); }
                    else { obj.BuildingId = 0; }

                    obj.Qty = Convert.ToInt32(qty);


                    obj.ProductId = Convert.ToInt32(productid);
                    //get Product detail
                    DataTable dtProduct = objproduct.BindProuct(obj.ProductId);

                    if (dtProduct.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Id"].ToString()))
                            productid = dtProduct.Rows[0]["Id"].ToString();
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SalePrice"].ToString()))
                        {
                            obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]) * obj.Qty;
                            obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
                        }
                        else
                        {
                            obj.Amount = 0;
                            obj.SalePrice = 0;
                        }
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountAmount"].ToString()))
                            obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountAmount"]) * obj.Qty;
                        else
                            obj.Discount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                            obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                        else
                            obj.CGSTPerct = 0;
                        //count cgst Amount
                        if (obj.CGSTPerct > 0)
                            obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                        else
                            obj.CGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                            obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                        else
                            obj.IGSTPerct = 0;
                        //count igst Amount
                        if (obj.IGSTPerct > 0)
                            obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                        else
                            obj.IGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                            obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                        else
                            obj.SGSTPerct = 0;
                        //count sgst Amount
                        if (obj.SGSTPerct > 0)
                            obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                        else
                            obj.SGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                            obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                        else
                            obj.RewardPoint = 0;
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                            obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                        else
                            obj.Profit = 0;

                        //Final Amount
                        obj.TotalFinalAmount = obj.Amount;
                    }

                    if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                    { obj.TotalAmount = obj.TotalFinalAmount; }
                    else { obj.TotalAmount = 0; }

                    obj.TotalGSTAmt = obj.CGSTAmount + obj.SGSTAmount;

                    obj.Status = "Pending";
                    obj.StateCode = null;
                    obj.ProductId = Convert.ToInt32(productid);

                    obj.OrderFlag = "WeekDay";
                    var checkAmount = objproduct.CheckProductOrderAmount(Convert.ToInt32(productid), obj.TotalAmount);
                    if (checkAmount.IsOrderAmount == false)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = checkAmount.message;
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }

                    DateTime _fromDate = FromDate;
                    for (int idate = 0; FromDate <= ToDate; idate++)
                    {
                        //Generate OrderNo
                        con.Open();
                        SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                        com1.CommandType = CommandType.StoredProcedure;
                        var Formno = com1.ExecuteScalar();
                        con.Close();

                        obj.OrderNo = Convert.ToInt32(Formno);
                        obj.OrderDate = FromDate;

                        //get Subscription Id
                        DataTable dtCustSubscription = obj.getCustomerSubscriptionOrderdate(obj);
                        if (dtCustSubscription.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                                obj.CustSubscriptionId = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                            else
                                obj.CustSubscriptionId = 0;
                        }
                        else
                            obj.CustSubscriptionId = 0;

                        AddProductOrder = obj.InsertCustomerOrder(obj);

                        if (AddProductOrder > 0)
                        {
                            obj.OrderId = Convert.ToInt32(AddProductOrder);
                            obj.ProductId = Convert.ToInt32(productid);
                            obj.Qty = Convert.ToInt32(qty);
                            obj.OrderItemDate = FromDate;

                            AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                        }
                        FromDate = FromDate.AddDays(7);


                    }
                    ToDate = FromDate.AddDays(-7);

                    if (AddProductDetail > 0)
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Placed Successfully";
                        string OrderFlag = "WeekDay";
                        Helper dHelper = new Helper();
                        dHelper.InsertCustomerOrderTrackNew(obj.CustomerId, obj.ProductId, obj.Qty, _fromDate, ToDate,OrderFlag);
                    }
                    else
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Order Not Inserted.";
                    }
                }

                if (!string.IsNullOrEmpty(fifthdate.ToString()) && fifthdate.ToString() != "0")
                {
                    DataTable dtcuttime = objcust.GetSchedularTime(null);
                    int dbcutOfftime = Convert.ToInt32(dtcuttime.Rows[0]["CutOffTime"]);
                    DateTime FromDate = Convert.ToDateTime(fifthdate);

                    if (DateTime.Now.Hour < dbcutOfftime)
                    {
                        FromDate = Convert.ToDateTime(fifthdate);
                    }
                    else
                        FromDate = FromDate.AddDays(1);

                    DateTime ToDate = FromDate;

                    if (!string.IsNullOrEmpty(customerid))
                    { obj.CustomerId = Convert.ToInt32(customerid); }
                    else { obj.CustomerId = 0; }

                    var timming = objproduct.CheckProductOrderTimimg(Convert.ToInt32(productid));
                    if (timming.IsTime == false)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = timming.message;
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }
                    DataTable dtDupliAssign = objcust.DuplicateStaffCustomer(null, obj.CustomerId);
                    if (dtDupliAssign.Rows.Count == 0)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Deliveryboy not assign...";
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }
                    int societyid = 0;

                    ToDate = FromDate.AddMonths(2);

                    DataTable dtBuildingId = objcust.BindCustomer(obj.CustomerId);

                    if (dtBuildingId.Rows.Count > 0)
                    {
                        //if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["BuildingId"].ToString()))
                        //    societyid = Convert.ToInt32(dtBuildingId.Rows[0]["BuildingId"]);

                        societyid = 0;
                        if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["IsActive"].ToString()))
                        {
                            if (dtBuildingId.Rows[0]["IsActive"].ToString() == "False")
                            {
                                dr["status"] = "Fail";
                                dr["error_msg"] = "Account Not Active...";
                                dtNew.Rows.Add(dr);
                                return Ok(dtNew);
                            }
                        }
                    }



                    if (!string.IsNullOrEmpty(societyid.ToString()))
                    { obj.BuildingId = Convert.ToInt32(societyid); }
                    else { obj.BuildingId = 0; }

                    obj.Qty = Convert.ToInt32(qty);


                    obj.ProductId = Convert.ToInt32(productid);
                    //get Product detail
                    DataTable dtProduct = objproduct.BindProuct(obj.ProductId);

                    if (dtProduct.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Id"].ToString()))
                            productid = dtProduct.Rows[0]["Id"].ToString();
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SalePrice"].ToString()))
                        {
                            obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]) * obj.Qty;
                            obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
                        }
                        else
                        {
                            obj.Amount = 0;
                            obj.SalePrice = 0;
                        }
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountAmount"].ToString()))
                            obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountAmount"]) * obj.Qty;
                        else
                            obj.Discount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                            obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                        else
                            obj.CGSTPerct = 0;
                        //count cgst Amount
                        if (obj.CGSTPerct > 0)
                            obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                        else
                            obj.CGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                            obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                        else
                            obj.IGSTPerct = 0;
                        //count igst Amount
                        if (obj.IGSTPerct > 0)
                            obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                        else
                            obj.IGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                            obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                        else
                            obj.SGSTPerct = 0;
                        //count sgst Amount
                        if (obj.SGSTPerct > 0)
                            obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                        else
                            obj.SGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                            obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                        else
                            obj.RewardPoint = 0;
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                            obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                        else
                            obj.Profit = 0;

                        //Final Amount
                        obj.TotalFinalAmount = obj.Amount;
                    }

                    if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                    { obj.TotalAmount = obj.TotalFinalAmount; }
                    else { obj.TotalAmount = 0; }

                    obj.TotalGSTAmt = obj.CGSTAmount + obj.SGSTAmount;

                    obj.Status = "Pending";
                    obj.StateCode = null;
                    obj.ProductId = Convert.ToInt32(productid);

                    obj.OrderFlag = "WeekDay";
                    var checkAmount = objproduct.CheckProductOrderAmount(Convert.ToInt32(productid), obj.TotalAmount);
                    if (checkAmount.IsOrderAmount == false)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = checkAmount.message;
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }

                    DateTime _fromDate = FromDate;
                    for (int idate = 0; FromDate <= ToDate; idate++)
                    {
                        //Generate OrderNo
                        con.Open();
                        SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                        com1.CommandType = CommandType.StoredProcedure;
                        var Formno = com1.ExecuteScalar();
                        con.Close();

                        obj.OrderNo = Convert.ToInt32(Formno);
                        obj.OrderDate = FromDate;

                        //get Subscription Id
                        DataTable dtCustSubscription = obj.getCustomerSubscriptionOrderdate(obj);
                        if (dtCustSubscription.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                                obj.CustSubscriptionId = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                            else
                                obj.CustSubscriptionId = 0;
                        }
                        else
                            obj.CustSubscriptionId = 0;

                        AddProductOrder = obj.InsertCustomerOrder(obj);

                        if (AddProductOrder > 0)
                        {
                            obj.OrderId = Convert.ToInt32(AddProductOrder);
                            obj.ProductId = Convert.ToInt32(productid);
                            obj.Qty = Convert.ToInt32(qty);
                            obj.OrderItemDate = FromDate;

                            AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                        }
                        FromDate = FromDate.AddDays(7);


                    }

                    ToDate = FromDate.AddDays(-7);
                    if (AddProductDetail > 0)
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Placed Successfully";
                        string OrderFlag = "WeekDay";
                        Helper dHelper = new Helper();
                        dHelper.InsertCustomerOrderTrackNew(obj.CustomerId, obj.ProductId, obj.Qty, _fromDate, ToDate,OrderFlag);
                    }
                    else
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Order Not Inserted.";
                    }
                }

                if (!string.IsNullOrEmpty(sixthdate.ToString()) && sixthdate.ToString() != "0")
                {
                    DataTable dtcuttime = objcust.GetSchedularTime(null);
                    int dbcutOfftime = Convert.ToInt32(dtcuttime.Rows[0]["CutOffTime"]);
                    DateTime FromDate = Convert.ToDateTime(sixthdate);

                    if (DateTime.Now.Hour < dbcutOfftime)
                    {
                        FromDate = Convert.ToDateTime(sixthdate);
                    }
                    else
                        FromDate = FromDate.AddDays(1);

                    DateTime ToDate = FromDate;

                    if (!string.IsNullOrEmpty(customerid))
                    { obj.CustomerId = Convert.ToInt32(customerid); }
                    else { obj.CustomerId = 0; }

                    var timming = objproduct.CheckProductOrderTimimg(Convert.ToInt32(productid));
                    if (timming.IsTime == false)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = timming.message;
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }
                    DataTable dtDupliAssign = objcust.DuplicateStaffCustomer(null, obj.CustomerId);
                    if (dtDupliAssign.Rows.Count == 0)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Deliveryboy not assign...";
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }
                    int societyid = 0;

                    ToDate = FromDate.AddMonths(2);

                    DataTable dtBuildingId = objcust.BindCustomer(obj.CustomerId);

                    if (dtBuildingId.Rows.Count > 0)
                    {
                        //if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["BuildingId"].ToString()))
                        //    societyid = Convert.ToInt32(dtBuildingId.Rows[0]["BuildingId"]);

                        societyid = 0;
                        if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["IsActive"].ToString()))
                        {
                            if (dtBuildingId.Rows[0]["IsActive"].ToString() == "False")
                            {
                                dr["status"] = "Fail";
                                dr["error_msg"] = "Account Not Active...";
                                dtNew.Rows.Add(dr);
                                return Ok(dtNew);
                            }
                        }
                    }



                    if (!string.IsNullOrEmpty(societyid.ToString()))
                    { obj.BuildingId = Convert.ToInt32(societyid); }
                    else { obj.BuildingId = 0; }

                    obj.Qty = Convert.ToInt32(qty);


                    obj.ProductId = Convert.ToInt32(productid);
                    //get Product detail
                    DataTable dtProduct = objproduct.BindProuct(obj.ProductId);

                    if (dtProduct.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Id"].ToString()))
                            productid = dtProduct.Rows[0]["Id"].ToString();
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SalePrice"].ToString()))
                        {
                            obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]) * obj.Qty;
                            obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
                        }
                        else
                        {
                            obj.Amount = 0;
                            obj.SalePrice = 0;
                        }
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountAmount"].ToString()))
                            obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountAmount"]) * obj.Qty;
                        else
                            obj.Discount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                            obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                        else
                            obj.CGSTPerct = 0;
                        //count cgst Amount
                        if (obj.CGSTPerct > 0)
                            obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                        else
                            obj.CGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                            obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                        else
                            obj.IGSTPerct = 0;
                        //count igst Amount
                        if (obj.IGSTPerct > 0)
                            obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                        else
                            obj.IGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                            obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                        else
                            obj.SGSTPerct = 0;
                        //count sgst Amount
                        if (obj.SGSTPerct > 0)
                            obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                        else
                            obj.SGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                            obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                        else
                            obj.RewardPoint = 0;
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                            obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                        else
                            obj.Profit = 0;

                        //Final Amount
                        obj.TotalFinalAmount = obj.Amount;
                    }

                    if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                    { obj.TotalAmount = obj.TotalFinalAmount; }
                    else { obj.TotalAmount = 0; }

                    obj.TotalGSTAmt = obj.CGSTAmount + obj.SGSTAmount;

                    obj.Status = "Pending";
                    obj.StateCode = null;
                    obj.ProductId = Convert.ToInt32(productid);

                    obj.OrderFlag = "WeekDay";
                    var checkAmount = objproduct.CheckProductOrderAmount(Convert.ToInt32(productid), obj.TotalAmount);
                    if (checkAmount.IsOrderAmount == false)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = checkAmount.message;
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }

                    DateTime _fromDate = FromDate;
                    for (int idate = 0; FromDate <= ToDate; idate++)
                    {
                        //Generate OrderNo
                        con.Open();
                        SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                        com1.CommandType = CommandType.StoredProcedure;
                        var Formno = com1.ExecuteScalar();
                        con.Close();

                        obj.OrderNo = Convert.ToInt32(Formno);
                        obj.OrderDate = FromDate;

                        //get Subscription Id
                        DataTable dtCustSubscription = obj.getCustomerSubscriptionOrderdate(obj);
                        if (dtCustSubscription.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                                obj.CustSubscriptionId = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                            else
                                obj.CustSubscriptionId = 0;
                        }
                        else
                            obj.CustSubscriptionId = 0;

                        AddProductOrder = obj.InsertCustomerOrder(obj);

                        if (AddProductOrder > 0)
                        {
                            obj.OrderId = Convert.ToInt32(AddProductOrder);
                            obj.ProductId = Convert.ToInt32(productid);
                            obj.Qty = Convert.ToInt32(qty);
                            obj.OrderItemDate = FromDate;

                            AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                        }
                        FromDate = FromDate.AddDays(7);


                    }

                    ToDate = FromDate.AddDays(-7);
                    if (AddProductDetail > 0)
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Placed Successfully";
                        string OrderFlag = "WeekDay";
                        Helper dHelper = new Helper();
                        dHelper.InsertCustomerOrderTrackNew(obj.CustomerId, obj.ProductId, obj.Qty, _fromDate, ToDate,OrderFlag);
                    }
                    else
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Order Not Inserted.";
                    }
                }

                if (!string.IsNullOrEmpty(seventhdate.ToString()) && seventhdate.ToString() != "0")
                {
                    DataTable dtcuttime = objcust.GetSchedularTime(null);
                    int dbcutOfftime = Convert.ToInt32(dtcuttime.Rows[0]["CutOffTime"]);
                    DateTime FromDate = Convert.ToDateTime(seventhdate);

                    if (DateTime.Now.Hour < dbcutOfftime)
                    {
                        FromDate = Convert.ToDateTime(seventhdate);
                    }
                    else
                        FromDate = FromDate.AddDays(1);

                    DateTime ToDate = FromDate;

                    if (!string.IsNullOrEmpty(customerid))
                    { obj.CustomerId = Convert.ToInt32(customerid); }
                    else { obj.CustomerId = 0; }

                    var timming = objproduct.CheckProductOrderTimimg(Convert.ToInt32(productid));
                    if (timming.IsTime == false)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = timming.message;
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }
                    DataTable dtDupliAssign = objcust.DuplicateStaffCustomer(null, obj.CustomerId);
                    if (dtDupliAssign.Rows.Count == 0)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Deliveryboy not assign...";
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }
                    int societyid = 0;

                    ToDate = FromDate.AddMonths(2);

                    DataTable dtBuildingId = objcust.BindCustomer(obj.CustomerId);

                    if (dtBuildingId.Rows.Count > 0)
                    {
                        //if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["BuildingId"].ToString()))
                        //    societyid = Convert.ToInt32(dtBuildingId.Rows[0]["BuildingId"]);

                        societyid = 0;
                        if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["IsActive"].ToString()))
                        {
                            if (dtBuildingId.Rows[0]["IsActive"].ToString() == "False")
                            {
                                dr["status"] = "Fail";
                                dr["error_msg"] = "Account Not Active...";
                                dtNew.Rows.Add(dr);
                                return Ok(dtNew);
                            }
                        }
                    }



                    if (!string.IsNullOrEmpty(societyid.ToString()))
                    { obj.BuildingId = Convert.ToInt32(societyid); }
                    else { obj.BuildingId = 0; }

                    obj.Qty = Convert.ToInt32(qty);


                    obj.ProductId = Convert.ToInt32(productid);
                    //get Product detail
                    DataTable dtProduct = objproduct.BindProuct(obj.ProductId);

                    if (dtProduct.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Id"].ToString()))
                            productid = dtProduct.Rows[0]["Id"].ToString();
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SalePrice"].ToString()))
                        {
                            obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]) * obj.Qty;
                            obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
                        }
                        else
                        {
                            obj.Amount = 0;
                            obj.SalePrice = 0;
                        }
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountAmount"].ToString()))
                            obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountAmount"]) * obj.Qty;
                        else
                            obj.Discount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                            obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                        else
                            obj.CGSTPerct = 0;
                        //count cgst Amount
                        if (obj.CGSTPerct > 0)
                            obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                        else
                            obj.CGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                            obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                        else
                            obj.IGSTPerct = 0;
                        //count igst Amount
                        if (obj.IGSTPerct > 0)
                            obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                        else
                            obj.IGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                            obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                        else
                            obj.SGSTPerct = 0;
                        //count sgst Amount
                        if (obj.SGSTPerct > 0)
                            obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                        else
                            obj.SGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                            obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                        else
                            obj.RewardPoint = 0;
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                            obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                        else
                            obj.Profit = 0;

                        //Final Amount
                        obj.TotalFinalAmount = obj.Amount;
                    }

                    if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                    { obj.TotalAmount = obj.TotalFinalAmount; }
                    else { obj.TotalAmount = 0; }

                    obj.TotalGSTAmt = obj.CGSTAmount + obj.SGSTAmount;

                    obj.Status = "Pending";
                    obj.StateCode = null;
                    obj.ProductId = Convert.ToInt32(productid);

                    obj.OrderFlag = "WeekDay";
                    var checkAmount = objproduct.CheckProductOrderAmount(Convert.ToInt32(productid), obj.TotalAmount);
                    if (checkAmount.IsOrderAmount == false)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = checkAmount.message;
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }

                    DateTime _fromDate = FromDate;
                    for (int idate = 0; FromDate <= ToDate; idate++)
                    {
                        //Generate OrderNo
                        con.Open();
                        SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                        com1.CommandType = CommandType.StoredProcedure;
                        var Formno = com1.ExecuteScalar();
                        con.Close();

                        obj.OrderNo = Convert.ToInt32(Formno);
                        obj.OrderDate = FromDate;

                        //get Subscription Id
                        DataTable dtCustSubscription = obj.getCustomerSubscriptionOrderdate(obj);
                        if (dtCustSubscription.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                                obj.CustSubscriptionId = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                            else
                                obj.CustSubscriptionId = 0;
                        }
                        else
                            obj.CustSubscriptionId = 0;

                        AddProductOrder = obj.InsertCustomerOrder(obj);

                        if (AddProductOrder > 0)
                        {
                            obj.OrderId = Convert.ToInt32(AddProductOrder);
                            obj.ProductId = Convert.ToInt32(productid);
                            obj.Qty = Convert.ToInt32(qty);
                            obj.OrderItemDate = FromDate;

                            AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                        }
                        FromDate = FromDate.AddDays(7);


                    }

                    ToDate = FromDate.AddDays(-7);
                    if (AddProductDetail > 0)
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Placed Successfully";
                        string OrderFlag = "WeekDay";
                        Helper dHelper = new Helper();
                        dHelper.InsertCustomerOrderTrackNew(obj.CustomerId, obj.ProductId, obj.Qty, _fromDate, ToDate,OrderFlag);
                    }
                    else
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Order Not Inserted.";
                    }
                }

               
            }



            if(AddProductDetail>0)
            {
                dr["status"] = "Success";
                dr["error_msg"] = "Order Placed Successfully";
                dtNew.Rows.Add(dr);
                return Ok(dtNew);
                //return Ok("Order Added Successfully");
            }
            else
            {
                dr["status"] = "Fail";
                dr["error_msg"] = "Please Fill Correct Details";
                dtNew.Rows.Add(dr);
                return Ok(dtNew);
            }
                

           // return Ok(dtNew);
        }

        //Work Week Day Order End


        //Work Multiple Day Order Start

        [Route("api/OrderNewApi/AddCustomerOrdermultipledate/{customerid?}/{productid?}/{qty?}/{dates}/{AttributeId?}/{VendorId?}/{VendorcatId?}/{SectorId?}/{DmId?}/{CusType?}")]
        [HttpGet]
        public IHttpActionResult AddCustomerOrdermultipledate(string customerid, string productid, string qty, string dates, string AttributeId, string VendorId, string VendorcatId, string SectorId,string DmId,string CusType)//string strjson
        {
            //Get CurrentTime

            DateTime centuryBegin = new DateTime(2001, 1, 1);
            var currentDate = Helper.indianTime;
            long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
            TimeSpan elapsedSpan = TimeSpan.Parse(currentDate.ToString("HH:mm"));
            string curhour = elapsedSpan.ToString();
            curhour = curhour.Substring(0, 2);

            //

            decimal minorderamount = 0, dailyorderamount = 0;
            decimal credit = 0;
            decimal Walletbal = 0, TotalCredit = 0, TotalDebit = 0;

            Vendor objvendor = new Vendor();
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();
            DataTable dtNew = new DataTable();
            DataTable dtVendor = new DataTable();

            dtVendor = objvendor.getVendorid(Convert.ToInt32(VendorcatId));
            if (dtVendor.Rows.Count > 0)
            {
                minorderamount = Convert.ToDecimal(dtVendor.Rows[0]["MinAmount"]);
            }

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            long AddProductOrder = 0; int AddProductDetail = 0;
            int r = 0;
            string msg = "Order Placed Successfully";
            string msg1 = "";
            string notinsert = "";
            if (!string.IsNullOrEmpty(customerid) && Convert.ToInt32(customerid) != 0 && !string.IsNullOrEmpty(productid) && Convert.ToInt32(productid) != 0 && !string.IsNullOrEmpty(qty) && Convert.ToInt32(qty) != 0 && !string.IsNullOrEmpty(dates.ToString()))
            {

                int c = dates.Length;

                string delimStr = ",";
                char[] delimiter = delimStr.ToCharArray();
                string a = "";
                foreach (string s in dates.Split(delimiter))
                {
                    // a = a + "-" + s;

                    DataTable dtcuttime = objcust.GetSchedularTime(null);
                    int dbcutOfftime = Convert.ToInt32(dtcuttime.Rows[0]["CutOffTime"]);
                    DateTime FromDate = Convert.ToDateTime(s);

                    DateTime FromDate1 = DateTime.Now;
                    FromDate1 = FromDate1.AddDays(1);

                    r = FromDate.Date.CompareTo(FromDate1.Date);
                    if(r==0)
                    {
                        //if (DateTime.Now.Hour < dbcutOfftime)
                        if (Convert.ToInt32(curhour) < dbcutOfftime)
                        {
                            FromDate = Convert.ToDateTime(s);
                            //Find daily order total
                            DataTable dtprodRecord = new DataTable();
                            dtprodRecord = obj.getCustomerOrderFuture(Convert.ToInt32(customerid), FromDate, FromDate);
                            if (dtprodRecord.Rows.Count > 0)
                            {
                                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                                {
                                    dailyorderamount = dailyorderamount + Convert.ToDecimal(dtprodRecord.Rows[i]["Amount"]);
                                }
                            }

                            //


                            // var timming = objproduct.CheckProductOrderTimimg(Convert.ToInt32(productid));
                            var timming = objproduct.CheckProductOrderTimimgNew(Convert.ToInt32(productid), Convert.ToInt32(VendorcatId));
                            if (timming.IsTime == false)
                                {

                                    notinsert = "Invalid";
                                    msg = timming.message + "So your order successfully placed on " + FromDate.Date;
                                }
                            


                        }
                        else
                        {
                            notinsert = "Invalid";
                        }
                    }
                  
                    else
                    {

                        FromDate = Convert.ToDateTime(s);
                    }
                        

                    if (!string.IsNullOrEmpty(customerid))
                    { obj.CustomerId = Convert.ToInt32(customerid); }
                    else { obj.CustomerId = 0; }

                   
                    DataTable dtDupliAssign = objcust.DuplicateStaffCustomer(null, obj.CustomerId);
                    if (dtDupliAssign.Rows.Count == 0)
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Deliveryboy not assign...";
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }
                    else
                    { obj.DeliveryBoyId = dtDupliAssign.Rows[0]["StaffId"].ToString(); }
                    int societyid = 0;


                    DataTable dtBuildingId = objcust.BindCustomer(obj.CustomerId);

                    if (dtBuildingId.Rows.Count > 0)
                    {
                        //if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["BuildingId"].ToString()))
                        //    societyid = Convert.ToInt32(dtBuildingId.Rows[0]["BuildingId"]);

                        societyid = 0;
                        if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["IsActive"].ToString()))
                        {
                            if (dtBuildingId.Rows[0]["IsActive"].ToString() == "False")
                            {
                                dr["status"] = "Fail";
                                dr["error_msg"] = "Account Not Active...";
                                dtNew.Rows.Add(dr);
                                return Ok(dtNew);
                            }
                        }
                    }


                    if (!string.IsNullOrEmpty(societyid.ToString()))
                    { obj.BuildingId = Convert.ToInt32(societyid); }
                    else { obj.BuildingId = 0; }

                    obj.Qty = Convert.ToInt32(qty);


                    obj.ProductId = Convert.ToInt32(productid);
                    //get Product detail
                    DataTable dtProduct = objproduct.BindProuctOrder(obj.ProductId, Convert.ToInt32(AttributeId), Convert.ToInt32(VendorId), Convert.ToInt32(VendorcatId));
                    if (dtProduct.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["ProductID"].ToString()))
                            productid = dtProduct.Rows[0]["ProductID"].ToString();


                        if (string.IsNullOrEmpty(CusType) || CusType == "General")
                        {
                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SellPrice"].ToString()))
                            {
                                obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]) * obj.Qty;
                                obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]);
                            }
                            else
                            {
                                obj.Amount = 0;
                                obj.SalePrice = 0;
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["B2BSellPrice"].ToString()))
                            {
                                obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["B2BSellPrice"]) * obj.Qty;
                                obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["B2BSellPrice"]);
                            }
                            else
                            {
                                obj.Amount = 0;
                                obj.SalePrice = 0;
                            }
                        }

                        

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["PurchasePrice"].ToString()))
                        {
                            obj.PurchasePrice = Convert.ToDecimal(dtProduct.Rows[0]["PurchasePrice"]) * obj.Qty;

                        }
                        else
                        {
                            obj.PurchasePrice = 0;

                        }

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["MRPPrice"].ToString()))
                        {
                            obj.MRPPrice = Convert.ToDecimal(dtProduct.Rows[0]["MRPPrice"]) * obj.Qty;

                        }
                        else
                        {
                            obj.MRPPrice = 0;

                        }
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountPrice"].ToString()))
                            obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountPrice"]) * obj.Qty;
                        else
                            obj.Discount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                            obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                        else
                            obj.CGSTPerct = 0;
                        //count cgst Amount
                        if (obj.CGSTPerct > 0)
                            obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                        else
                            obj.CGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                            obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                        else
                            obj.IGSTPerct = 0;
                        //count igst Amount
                        if (obj.IGSTPerct > 0)
                            obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                        else
                            obj.IGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                            obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                        else
                            obj.SGSTPerct = 0;
                        //count sgst Amount
                        if (obj.SGSTPerct > 0)
                            obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                        else
                            obj.SGSTAmount = 0;

                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                            obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                        else
                            obj.RewardPoint = 0;
                        if (string.IsNullOrEmpty(CusType) || CusType == "General")
                        {
                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                                obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                            else
                                obj.Profit = 0;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["B2BProfit"].ToString()))
                                obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["B2BProfit"]) * obj.Qty;
                            else
                                obj.Profit = 0;
                        }


                        //Final Amount
                        obj.TotalFinalAmount = obj.Amount;
                    }


                    if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                    { obj.TotalAmount = obj.TotalFinalAmount; }
                    else { obj.TotalAmount = 0; }

                    obj.TotalGSTAmt = obj.CGSTAmount + obj.SGSTAmount;

                    obj.Status = "Pending";
                    obj.StateCode = null;
                    obj.ProductId = Convert.ToInt32(productid);

                    obj.OrderFlag = "MultipleDt";
                    //var checkAmount = objproduct.CheckProductOrderAmount(Convert.ToInt32(productid), obj.TotalAmount);
                    //if (checkAmount.IsOrderAmount == false)
                    //{
                    //    dr["status"] = "Fail";
                    //    dr["error_msg"] = checkAmount.message;
                    //    dtNew.Rows.Add(dr);
                    //    return Ok(dtNew);
                    //}

                    //Calculate wallet with Order Amount 02-01-2022
                    DataTable dt = new DataTable();
                    dt = obj.GetCustomerCredit(obj.CustomerId);

                    if (dt.Rows.Count > 0)
                    {
                        credit = Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString());
                    }
                    DataTable dtprodRecord1 = obj.getCustomerWallet(obj.CustomerId);
                    int userRecords = dtprodRecord1.Rows.Count;
                    if (userRecords > 0)
                    {
                        if (!string.IsNullOrEmpty(dtprodRecord1.Rows[0]["Amt"].ToString()))
                            TotalCredit = Convert.ToDecimal(dtprodRecord1.Rows[0]["Amt"]);
                        if (userRecords > 1)
                        {
                            if (!string.IsNullOrEmpty(dtprodRecord1.Rows[1]["Amt"].ToString()))
                                TotalDebit = Convert.ToDecimal(dtprodRecord1.Rows[1]["Amt"]);
                        }
                        Walletbal = TotalCredit - TotalDebit;
                    }

                    decimal sumamount = dailyorderamount + obj.TotalAmount;
                    if ((dailyorderamount + obj.TotalAmount) < minorderamount)
                    {
                        if ((dailyorderamount + obj.TotalAmount) < (Walletbal + credit))
                        {
                            msg1 = "Minimum Order Value for this Category is " + minorderamount;
                        }

                        if ((dailyorderamount + obj.TotalAmount) > (Walletbal + credit))
                        {
                            msg1 += ".wallet balance is lower than order value please upload balance before 10:00 PM to process this order ";
                        }
                    }

                    if ((dailyorderamount + obj.TotalAmount) > minorderamount)
                    {
                        if ((dailyorderamount + obj.TotalAmount) > (Walletbal + credit))
                        {
                            msg1 = "wallet balance is lower than order value please upload balance before 10:00 PM to process this order ";
                        }
                    }
                  


                    DateTime _fromDate = FromDate;

                    con.Open();
                    SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                    com1.CommandType = CommandType.StoredProcedure;
                    var Formno = com1.ExecuteScalar();
                    con.Close();

                    obj.OrderNo = Convert.ToInt32(Formno);
                    obj.OrderDate = FromDate;

                    //get Subscription Id
                    DataTable dtCustSubscription = obj.getCustomerSubscriptionOrderdate(obj);
                    if (dtCustSubscription.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                            obj.CustSubscriptionId = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                        else
                            obj.CustSubscriptionId = 0;
                    }
                    else
                        obj.CustSubscriptionId = 0;


                    if(notinsert == "")
                    {
                        AddProductOrder = obj.InsertCustomerOrder(obj);
                    }
                    obj.AttributeId = Convert.ToInt32(AttributeId);
                    obj.VendorId = Convert.ToInt32(VendorId);
                    obj.VendorCatId = Convert.ToInt32(VendorcatId);
                    obj.SectorId = Convert.ToInt32(SectorId);
                    if (AddProductOrder > 0)
                    {
                        obj.OrderId = Convert.ToInt32(AddProductOrder);
                        obj.ProductId = Convert.ToInt32(productid);
                        obj.Qty = Convert.ToInt32(qty);
                        obj.OrderItemDate = FromDate;

                        AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                    }


                    //if (AddProductDetail > 0)
                    //{
                    //    dr["status"] = "Success";
                    //    dr["error_msg"] = "Order Placed Successfully";
                    //    string OrderFlag = "MultipleDt";
                    //    Helper dHelper = new Helper();
                    //    dHelper.InsertCustomerOrderTrackNew(obj.CustomerId, obj.ProductId, obj.Qty, _fromDate, _fromDate, OrderFlag);
                    //}
                    //else
                    //{
                    //    dr["status"] = "Fail";
                    //    dr["error_msg"] = "Order Not Inserted.";
                    //}



                }

                dr["status"] = "Success";
                dr["error_msg"] = msg;


                //send notification
                if (msg1 != "")
                {
                    Customer objcustomer = new Customer();
                    Customer objLogin = new Customer();
                    DataTable dtToken = new DataTable();
                    string customerId = obj.CustomerId.ToString();
                    dtToken = objLogin.getCustomerSector(Convert.ToInt32(customerId));
                    if (dtToken.Rows.Count > 0)
                    {
                        objcustomer.UserName = dtToken.Rows[0].ItemArray[1].ToString();
                        objcustomer.SectorId = Convert.ToInt32(dtToken.Rows[0].ItemArray[0]);
                    }

                    string type1 = "Customer";

                    objcustomer.ntitle = "Related Order";
                    objcustomer.ntext = msg1;
                    objcustomer.Photo = "";
                    objcustomer.nlink = "";
                    var addresult = objcustomer.InsertNotification(objcustomer, type1);


                    //DataTable dtToken = new DataTable();

                    int cusnot = 0;

                    objcustomer.nid = addresult.ToString();

                    cusnot = objcustomer.InsertCustomerNotification(objcustomer);

                }
                //
            }
            else
            {
                dr["status"] = "Failed";
                dr["error_msg"] = "Please Fill Correct Details";
            }
            
            if(AddProductDetail>0)
            {
                dr["status"] = "Success";
                dr["error_msg"] = msg+msg1;
                dtNew.Rows.Add(dr);
                return Ok(dtNew);
                //return Ok("Product Added Successfully");
            }
            else
            {
                dr["status"] = "Failed";
                dr["error_msg"] = "Please Fill Correct Details";


                return Ok(dtNew);
            }
            //return Ok(dtNew);

        }


        [Route("api/OrderNewApi/CustomerAlternateOrderDelete/{customerid?}/{productid?}/{startdate}/{status}")]
        [HttpGet]
        public IHttpActionResult CustomerAlternateOrderDelete(int customerid,int productid, DateTime? startdate,string status)//string strjson
        {
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            DataTable dtprodRecord = new DataTable();

            if (!string.IsNullOrEmpty(customerid.ToString()) && customerid != 0 && !string.IsNullOrEmpty(productid.ToString()) && productid != 0)
            {


                if (status == "Particular")
                {
                    DateTime Currentdate = Helper.indianTime;
                    DateTime FromDate = Convert.ToDateTime(startdate);
                    DateTime ToDate = DateTime.Now;
                    ToDate = FromDate;

                    if (!string.IsNullOrEmpty(customerid.ToString()))
                    { obj.CustomerId = customerid; }
                    else { obj.CustomerId = 0; }

                    if (!string.IsNullOrEmpty(productid.ToString()))
                    { obj.ProductId = productid; }
                    else { obj.ProductId = 0; }

                    dtprodRecord = obj.getCustomerOrderListalternate(obj.CustomerId, obj.ProductId, FromDate, ToDate);
                    int userRecords = dtprodRecord.Rows.Count;
                    obj.OrderId = 0;
                    int DelProductOrder = 0; int DelAddProductDetail = 0;

                    if (userRecords > 0)
                    {
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["OrderDate"].ToString()))
                        {
                            DateTime CurrentDate = Helper.indianTime.AddDays(1);
                            DateTime OrderDate = Convert.ToDateTime(dtprodRecord.Rows[0]["OrderDate"]);
                            if (CurrentDate.Day == OrderDate.Day && CurrentDate.Month == OrderDate.Month && CurrentDate.Year == OrderDate.Year)
                            {
                                var timming = objproduct.CheckProductOrderTimimg(obj.ProductId);
                                if (timming.IsTime == false)
                                {
                                    dr["status"] = "Fail";
                                    dr["error_msg"] = "Unable to delete... - " + timming.message;
                                    dtNew.Rows.Add(dr);
                                    return Ok(dtNew);
                                }
                            }
                        }

                        for (int i = 0; i < userRecords; i++)
                        {
                            if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Id"].ToString()))
                            {
                                obj.OrderId = Convert.ToInt32(dtprodRecord.Rows[i]["Id"]);
                                obj.OrderDate = FromDate;
                                DelProductOrder = obj.DeleteCustomerOrder(obj.OrderId);
                                FromDate = FromDate.AddDays(2);
                            }
                            else
                            {
                                dr["status"] = "Success";
                                dr["error_msg"] = "Order Not Found";
                            }
                        }


                        if (DelProductOrder > 0)
                        {
                            obj.DeleteCustomerOrderTrack(obj.CustomerId, obj.ProductId);
                            dr["status"] = "Success";
                            dr["error_msg"] = "Order Deleted Successfully";
                        }
                        else
                        {
                            dr["status"] = "Failed";
                            dr["error_msg"] = "Order Not Deleted";
                        }

                    }
                    else
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Not Found";
                    }

                    dtNew.Rows.Add(dr);
                    return Ok(dtNew);

                }
                if(status=="Complete")
                {
                    DateTime Currentdate = Helper.indianTime;
                    DateTime FromDate = Convert.ToDateTime(startdate);
                    DateTime ToDate = DateTime.Now;
                    ToDate = FromDate.AddMonths(3);

                    if (!string.IsNullOrEmpty(customerid.ToString()))
                    { obj.CustomerId = customerid; }
                    else { obj.CustomerId = 0; }

                    if (!string.IsNullOrEmpty(productid.ToString()))
                    { obj.ProductId = productid; }
                    else { obj.ProductId = 0; }

                    dtprodRecord = obj.getCustomerOrderListalternate(obj.CustomerId, obj.ProductId, FromDate, ToDate);
                    int userRecords = dtprodRecord.Rows.Count;
                    obj.OrderId = 0;
                    int DelProductOrder = 0; int DelAddProductDetail = 0;

                    if (userRecords > 0)
                    {
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["OrderDate"].ToString()))
                        {
                            DateTime CurrentDate = Helper.indianTime.AddDays(1);
                            DateTime OrderDate = Convert.ToDateTime(dtprodRecord.Rows[0]["OrderDate"]);
                            if (CurrentDate.Day == OrderDate.Day && CurrentDate.Month == OrderDate.Month && CurrentDate.Year == OrderDate.Year)
                            {
                                var timming = objproduct.CheckProductOrderTimimg(obj.ProductId);
                                if (timming.IsTime == false)
                                {
                                    dr["status"] = "Fail";
                                    dr["error_msg"] = "Unable to delete... - " + timming.message;
                                    dtNew.Rows.Add(dr);
                                    return Ok(dtNew);
                                }
                            }
                        }

                        for (int i = 0; i < userRecords; i++)
                        {
                            if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Id"].ToString()))
                            {
                                obj.OrderId = Convert.ToInt32(dtprodRecord.Rows[i]["Id"]);
                                obj.OrderDate = FromDate;
                                DelProductOrder = obj.DeleteCustomerOrder(obj.OrderId);
                                FromDate = FromDate.AddDays(2);
                            }
                            else
                            {
                                dr["status"] = "Success";
                                dr["error_msg"] = "Order Not Found";
                            }
                        }


                        if (DelProductOrder > 0)
                        {
                            obj.DeleteCustomerOrderTrack(obj.CustomerId, obj.ProductId);
                            dr["status"] = "Success";
                            dr["error_msg"] = "Order Deleted Successfully";
                        }
                        else
                        {
                            dr["status"] = "Failed";
                            dr["error_msg"] = "Order Not Deleted";
                        }

                    }
                    else
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Not Found";
                    }

                    dtNew.Rows.Add(dr);
                    return Ok(dtNew);




                }



            }


                return Ok(dtprodRecord);

        }




        [Route("api/OrderNewApi/CustomerMultipledtOrderDelete/{customerid?}/{productid?}/{selecteddate}")]
        [HttpGet]

        public IHttpActionResult CustomerMultipledtOrderDelete(int customerid, int productid, DateTime? selecteddate)//string strjson
        {

            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();


            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            long AddProductOrder = 0; int AddProductDetail = 0;

            DateTime Currentdate = Helper.indianTime;
            DateTime FromDate = Convert.ToDateTime(selecteddate);
            DateTime ToDate = DateTime.Now;
            ToDate = FromDate;

            if (!string.IsNullOrEmpty(customerid.ToString()))
            { obj.CustomerId = customerid; }
            else { obj.CustomerId = 0; }

            if (!string.IsNullOrEmpty(productid.ToString()))
            { obj.ProductId = productid; }
            else { obj.ProductId = 0; }
            DataTable dtprodRecord = new DataTable();

            if (!string.IsNullOrEmpty(customerid.ToString()) && customerid != 0 && !string.IsNullOrEmpty(productid.ToString()) && productid != 0)
            {
                dtprodRecord = obj.getCustomerOrderListMultidt(obj.CustomerId, obj.ProductId, FromDate, ToDate);
                int userRecords = dtprodRecord.Rows.Count;
                obj.OrderId = 0;
                int DelProductOrder = 0; int DelAddProductDetail = 0;

                if (userRecords > 0)
                {
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["OrderDate"].ToString()))
                    {
                        DateTime CurrentDate = Helper.indianTime.AddDays(1);
                        DateTime OrderDate = Convert.ToDateTime(dtprodRecord.Rows[0]["OrderDate"]);
                        if (CurrentDate.Day == OrderDate.Day && CurrentDate.Month == OrderDate.Month && CurrentDate.Year == OrderDate.Year)
                        {
                            var timming = objproduct.CheckProductOrderTimimg(obj.ProductId);
                            if (timming.IsTime == false)
                            {
                                dr["status"] = "Fail";
                                dr["error_msg"] = "Unable to delete... - " + timming.message;
                                dtNew.Rows.Add(dr);
                                return Ok(dtNew);
                            }
                        }
                    }

                    for (int i = 0; i < userRecords; i++)
                    {
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Id"].ToString()))
                        {
                            obj.OrderId = Convert.ToInt32(dtprodRecord.Rows[i]["Id"]);
                            obj.OrderDate = FromDate;
                            DelProductOrder = obj.DeleteCustomerOrder(obj.OrderId);
                            //FromDate = FromDate.AddDays(2);
                        }
                        else
                        {
                            dr["status"] = "Success";
                            dr["error_msg"] = "Order Not Found";
                        }
                    }


                    if (DelProductOrder > 0)
                    {
                       // obj.DeleteCustomerOrderTrack(obj.CustomerId, obj.ProductId);
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Deleted Successfully";
                    }
                    else
                    {
                        dr["status"] = "Failed";
                        dr["error_msg"] = "Order Not Deleted";
                    }

                }
                else
                {
                    dr["status"] = "Success";
                    dr["error_msg"] = "Order Not Found";
                }

                dtNew.Rows.Add(dr);
                return Ok(dtNew);
            }

            return Ok(dtprodRecord);
        }


        //Work Week Day Order Start New
        [Route("api/OrderNewApi/AddCustomerOrderweeklydaynew/{customerid?}/{productid?}/{qty?}/{selecteddays?}/{AttributeId?}/{VendorId?}/{VendorcatId?}/{SectorId?}/{DmId}/{CusType?}")]
        [HttpGet]

        public IHttpActionResult AddCustomerOrderweeklydaynew(string customerid, string productid, string qty, string selecteddays, string AttributeId, string VendorId, string VendorcatId, string SectorId,string DmId,string CusType)//string strjson
        {

            //Get CurrentTime

            DateTime centuryBegin = new DateTime(2001, 1, 1);
            var currentDate = Helper.indianTime;
            long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
            TimeSpan elapsedSpan = TimeSpan.Parse(currentDate.ToString("HH:mm"));
            string curhour = elapsedSpan.ToString();
            curhour = curhour.Substring(0, 2);

            //
            decimal minorderamount = 0, dailyorderamount = 0;
            decimal credit = 0;
            decimal Walletbal = 0, TotalCredit = 0, TotalDebit = 0;

            Vendor objvendor = new Vendor();
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();
            DataTable dtNew = new DataTable();
            DataTable dtVendor = new DataTable();

            dtVendor = objvendor.getVendorid(Convert.ToInt32(VendorcatId));
            if (dtVendor.Rows.Count > 0)
            {
                minorderamount = Convert.ToDecimal(dtVendor.Rows[0]["MinAmount"]);
            }

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            long AddProductOrder = 0; int AddProductDetail = 0;

            if (!string.IsNullOrEmpty(customerid) && Convert.ToInt32(customerid) != 0 && !string.IsNullOrEmpty(productid) && Convert.ToInt32(productid) != 0 && !string.IsNullOrEmpty(qty) && Convert.ToInt32(qty) != 0 && !string.IsNullOrEmpty(selecteddays.ToString()))
            {
                string msg1 = "",msg="Order Placed Successfully.";
                int c = selecteddays.Length;

                string delimStr = ",";
                char[] delimiter = delimStr.ToCharArray();
                string choosenday = "";
                int currentdayno = 0,daydiff=0;
                

               // DayOfWeek currentday = DateTime.Today.DayOfWeek;
                DayOfWeek currentday = Helper.indianTime.DayOfWeek;
                if (currentday.ToString() == "Monday") currentdayno = 1;
                if (currentday.ToString() == "Tuesday") currentdayno = 2;
                if (currentday.ToString() == "Wednesday") currentdayno = 3;
                if (currentday.ToString() == "Thursday") currentdayno = 4;
                if (currentday.ToString() == "Friday") currentdayno = 5;
                if (currentday.ToString() == "Saturday") currentdayno = 6;
                if (currentday.ToString() == "Sunday") currentdayno = 7;
                foreach (string s in selecteddays.Split(delimiter))
                {
                    daydiff = 0;
                    choosenday = s.ToString();
                    if(Convert.ToInt32(choosenday)<=currentdayno)
                    {
                        DateTime FromDate = Helper.indianTime;
                        int dayadd = 0;
                        daydiff = currentdayno - Convert.ToInt32(choosenday);
                        dayadd = 7 - daydiff;
                        FromDate = FromDate.AddDays(dayadd);


                        DataTable dtcuttime = objcust.GetSchedularTime(null);
                        int dbcutOfftime = Convert.ToInt32(dtcuttime.Rows[0]["CutOffTime"]);

                        if (FromDate == Helper.indianTime.AddDays(1))
                        {
                            //if (DateTime.Now.Hour < dbcutOfftime)
                             if (Convert.ToInt32(curhour) < dbcutOfftime)
                            {
                                FromDate = Convert.ToDateTime(FromDate);
                                //Find daily order total
                                DataTable dtprodRecord = new DataTable();
                                dtprodRecord = obj.getCustomerOrderFuture(Convert.ToInt32(customerid), FromDate, FromDate);
                                if (dtprodRecord.Rows.Count > 0)
                                {
                                    for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                                    {
                                        dailyorderamount = dailyorderamount + Convert.ToDecimal(dtprodRecord.Rows[i]["Amount"]);
                                    }
                                }

                                //
                                //var timming = objproduct.CheckProductOrderTimimg(Convert.ToInt32(productid));
                                var timming = objproduct.CheckProductOrderTimimgNew(Convert.ToInt32(productid), Convert.ToInt32(VendorcatId));
                                if (timming.IsTime == false)
                                {
                                    
                                    FromDate = FromDate.AddDays(7);
                                    
                                }
                            }
                            else
                                FromDate = FromDate.AddDays(7);
                        }
                        DateTime ToDate = FromDate;

                        if (!string.IsNullOrEmpty(customerid))
                        { obj.CustomerId = Convert.ToInt32(customerid); }
                        else { obj.CustomerId = 0; }

                        //var timming = objproduct.CheckProductOrderTimimg(Convert.ToInt32(productid));
                        //if (timming.IsTime == false)
                        //{
                        //    dr["status"] = "Fail";
                        //    dr["error_msg"] = timming.message;
                        //    dtNew.Rows.Add(dr);
                        //    return Ok(dtNew);
                        //}
                        DataTable dtDupliAssign = objcust.DuplicateStaffCustomer(null, obj.CustomerId);
                        if (dtDupliAssign.Rows.Count == 0)
                        {
                            dr["status"] = "Fail";
                            dr["error_msg"] = "Deliveryboy not assign...";
                            dtNew.Rows.Add(dr);
                            return Ok(dtNew);
                        }

                        else
                        { obj.DeliveryBoyId = dtDupliAssign.Rows[0]["StaffId"].ToString(); }
                        int societyid = 0;

                        ToDate = FromDate.AddMonths(2);

                        DataTable dtBuildingId = objcust.BindCustomer(obj.CustomerId);

                        if (dtBuildingId.Rows.Count > 0)
                        {
                            //if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["BuildingId"].ToString()))
                            //    societyid = Convert.ToInt32(dtBuildingId.Rows[0]["BuildingId"]);

                            societyid = 0;
                            if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["IsActive"].ToString()))
                            {
                                if (dtBuildingId.Rows[0]["IsActive"].ToString() == "False")
                                {
                                    dr["status"] = "Fail";
                                    dr["error_msg"] = "Account Not Active...";
                                    dtNew.Rows.Add(dr);
                                    return Ok(dtNew);
                                }
                            }
                        }



                        if (!string.IsNullOrEmpty(societyid.ToString()))
                        { obj.BuildingId = Convert.ToInt32(societyid); }
                        else { obj.BuildingId = 0; }

                        obj.Qty = Convert.ToInt32(qty);


                        obj.ProductId = Convert.ToInt32(productid);
                        //get Product detail
                        DataTable dtProduct = objproduct.BindProuctOrder(obj.ProductId, Convert.ToInt32(AttributeId), Convert.ToInt32(VendorId), Convert.ToInt32(VendorcatId));
                        if (dtProduct.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["ProductID"].ToString()))
                                productid = dtProduct.Rows[0]["ProductID"].ToString();
                            if (string.IsNullOrEmpty(CusType) || CusType == "General")
                            {
                                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SellPrice"].ToString()))
                                {
                                    obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]) * obj.Qty;
                                    obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]);
                                }
                                else
                                {
                                    obj.Amount = 0;
                                    obj.SalePrice = 0;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["B2BSellPrice"].ToString()))
                                {
                                    obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["B2BSellPrice"]) * obj.Qty;
                                    obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["B2BSellPrice"]);
                                }
                                else
                                {
                                    obj.Amount = 0;
                                    obj.SalePrice = 0;
                                }
                            }

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["PurchasePrice"].ToString()))
                            {
                                obj.PurchasePrice = Convert.ToDecimal(dtProduct.Rows[0]["PurchasePrice"]) * obj.Qty;

                            }
                            else
                            {
                                obj.PurchasePrice = 0;

                            }

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["MRPPrice"].ToString()))
                            {
                                obj.MRPPrice = Convert.ToDecimal(dtProduct.Rows[0]["MRPPrice"]) * obj.Qty;

                            }
                            else
                            {
                                obj.MRPPrice = 0;

                            }
                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountPrice"].ToString()))
                                obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountPrice"]) * obj.Qty;
                            else
                                obj.Discount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                                obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                            else
                                obj.CGSTPerct = 0;
                            //count cgst Amount
                            if (obj.CGSTPerct > 0)
                                obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                            else
                                obj.CGSTAmount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                                obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                            else
                                obj.IGSTPerct = 0;
                            //count igst Amount
                            if (obj.IGSTPerct > 0)
                                obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                            else
                                obj.IGSTAmount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                                obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                            else
                                obj.SGSTPerct = 0;
                            //count sgst Amount
                            if (obj.SGSTPerct > 0)
                                obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                            else
                                obj.SGSTAmount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                                obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                            else
                                obj.RewardPoint = 0;
                            if (string.IsNullOrEmpty(CusType) || CusType == "General")
                            {
                                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                                    obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                                else
                                    obj.Profit = 0;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["B2BProfit"].ToString()))
                                    obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["B2BProfit"]) * obj.Qty;
                                else
                                    obj.Profit = 0;
                            }


                            //Final Amount
                            obj.TotalFinalAmount = obj.Amount;
                        }

                        if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                        { obj.TotalAmount = obj.TotalFinalAmount; }
                        else { obj.TotalAmount = 0; }

                        obj.TotalGSTAmt = obj.CGSTAmount + obj.SGSTAmount;

                        obj.Status = "Pending";
                        obj.StateCode = null;
                        obj.ProductId = Convert.ToInt32(productid);

                        obj.OrderFlag = "WeekDay";
                        //var checkAmount = objproduct.CheckProductOrderAmount(Convert.ToInt32(productid), obj.TotalAmount);
                        //if (checkAmount.IsOrderAmount == false)
                        //{
                        //    dr["status"] = "Fail";
                        //    dr["error_msg"] = checkAmount.message;
                        //    dtNew.Rows.Add(dr);
                        //    return Ok(dtNew);
                        //}

                        //Calculate wallet with Order Amount 02-01-2022
                        DataTable dt = new DataTable();
                        dt = obj.GetCustomerCredit(obj.CustomerId);

                        if (dt.Rows.Count > 0)
                        {
                            credit = Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString());
                        }
                        DataTable dtprodRecord1 = obj.getCustomerWallet(obj.CustomerId);
                        int userRecords = dtprodRecord1.Rows.Count;
                        if (userRecords > 0)
                        {
                            if (!string.IsNullOrEmpty(dtprodRecord1.Rows[0]["Amt"].ToString()))
                                TotalCredit = Convert.ToDecimal(dtprodRecord1.Rows[0]["Amt"]);
                            if (userRecords > 1)
                            {
                                if (!string.IsNullOrEmpty(dtprodRecord1.Rows[1]["Amt"].ToString()))
                                    TotalDebit = Convert.ToDecimal(dtprodRecord1.Rows[1]["Amt"]);
                            }
                            Walletbal = TotalCredit - TotalDebit;
                        }

                        decimal sumamount = dailyorderamount + obj.TotalAmount;
                        if ((dailyorderamount + obj.TotalAmount) < minorderamount)
                        {
                            if ((dailyorderamount + obj.TotalAmount) < (Walletbal + credit))
                            {
                                msg1 = "Minimum Order Value for this Category is " + minorderamount;
                            }

                            if ((dailyorderamount + obj.TotalAmount) > (Walletbal + credit))
                            {
                                msg1 += ".wallet balance is lower than order value please upload balance before 10:00 PM to process this order ";
                            }
                        }

                        if ((dailyorderamount + obj.TotalAmount) > minorderamount)
                        {
                            if ((dailyorderamount + obj.TotalAmount) > (Walletbal + credit))
                            {
                                msg1 = "wallet balance is lower than order value please upload balance before 10:00 PM to process this order ";
                            }
                        }


                        //send notification
                        if (msg1 != "")
                        {
                            Customer objcustomer = new Customer();
                            Customer objLogin = new Customer();
                            DataTable dtToken = new DataTable();
                            string customerId = obj.CustomerId.ToString();
                            dtToken = objLogin.getCustomerSector(Convert.ToInt32(customerId));
                            if (dtToken.Rows.Count > 0)
                            {
                                objcustomer.UserName = dtToken.Rows[0].ItemArray[1].ToString();
                                objcustomer.SectorId = Convert.ToInt32(dtToken.Rows[0].ItemArray[0]);
                            }

                            string type1 = "Customer";

                            objcustomer.ntitle = "Related Order";
                            objcustomer.ntext = msg1;
                            objcustomer.Photo = "";
                            objcustomer.nlink = "";
                            var addresult = objcustomer.InsertNotification(objcustomer, type1);


                            //DataTable dtToken = new DataTable();

                            int cusnot = 0;

                            objcustomer.nid = addresult.ToString();

                            cusnot = objcustomer.InsertCustomerNotification(objcustomer);

                        }
                        //

                        obj.AttributeId = Convert.ToInt32(AttributeId);
                        obj.VendorId = Convert.ToInt32(VendorId);
                        obj.VendorCatId = Convert.ToInt32(VendorcatId);
                        obj.SectorId = Convert.ToInt32(SectorId);
                        DateTime _fromDate = FromDate;
                        for (int idate = 0; FromDate <= ToDate; idate++)
                        {
                            //Generate OrderNo
                            con.Open();
                            SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                            com1.CommandType = CommandType.StoredProcedure;
                            var Formno = com1.ExecuteScalar();
                            con.Close();

                            obj.OrderNo = Convert.ToInt32(Formno);
                            obj.OrderDate = FromDate;

                            //get Subscription Id
                            DataTable dtCustSubscription = obj.getCustomerSubscriptionOrderdate(obj);
                            if (dtCustSubscription.Rows.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                                    obj.CustSubscriptionId = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                                else
                                    obj.CustSubscriptionId = 0;
                            }
                            else
                                obj.CustSubscriptionId = 0;

                            AddProductOrder = obj.InsertCustomerOrder(obj);

                            if (AddProductOrder > 0)
                            {
                                obj.OrderId = Convert.ToInt32(AddProductOrder);
                                obj.ProductId = Convert.ToInt32(productid);
                                obj.Qty = Convert.ToInt32(qty);
                                obj.OrderItemDate = FromDate;

                                AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                            }
                            FromDate = FromDate.AddDays(7);


                        }
                        ToDate = FromDate.AddDays(-7);

                        if (AddProductDetail > 0)
                        {
                           // dr["status"] = "Success";
                          //  dr["error_msg"] = "Order Placed Successfully";
                            string OrderFlag = "WeekDay";
                            Helper dHelper = new Helper();
                            //dHelper.InsertCustomerOrderTrackNew(obj.CustomerId, obj.ProductId, obj.Qty, _fromDate, ToDate, OrderFlag);

                            dHelper.InsertCustomerOrderTrackNeworder(obj.CustomerId, obj.ProductId, obj.Qty, _fromDate, ToDate, OrderFlag, Convert.ToInt32(AttributeId));
                        }
                        else
                        {
                          //  dr["status"] = "Fail";
                           // dr["error_msg"] = "Order Not Inserted.";
                        }

                    }

                    if (Convert.ToInt32(choosenday) > currentdayno)
                    {
                        DateTime FromDate =Helper.indianTime;
                        int dayadd = 0;
                        daydiff =  Convert.ToInt32(choosenday)- currentdayno;
                        FromDate = FromDate.AddDays(daydiff);

                        DateTime fdate =Helper.indianTime;

                        DataTable dtcuttime = objcust.GetSchedularTime(null);
                        int dbcutOfftime = Convert.ToInt32(dtcuttime.Rows[0]["CutOffTime"]);


                        if (FromDate == Helper.indianTime.AddDays(1))
                        {
                            if (Convert.ToInt32(curhour) < dbcutOfftime)
                            {
                                FromDate = Convert.ToDateTime(FromDate);
                                //Find daily order total
                                DataTable dtprodRecord = new DataTable();
                                dtprodRecord = obj.getCustomerOrderFuture(Convert.ToInt32(customerid), FromDate, FromDate);
                                if (dtprodRecord.Rows.Count > 0)
                                {
                                    for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                                    {
                                        dailyorderamount = dailyorderamount + Convert.ToDecimal(dtprodRecord.Rows[i]["Amount"]);
                                    }
                                }

                                //
                                //var timming = objproduct.CheckProductOrderTimimg(Convert.ToInt32(productid));
                                var timming = objproduct.CheckProductOrderTimimgNew(Convert.ToInt32(productid), Convert.ToInt32(VendorcatId));
                                if (timming.IsTime == false)
                                {

                                    FromDate = FromDate.AddDays(7);

                                }
                            }
                            else
                                FromDate = FromDate.AddDays(7);
                        }

                        //if (FromDate==DateTime.Now.AddDays(1))
                        //{ 
                        //if (DateTime.Now.Hour < dbcutOfftime)
                        //{
                        //    FromDate = Convert.ToDateTime(FromDate);
                        //}
                        //else
                        //    FromDate = FromDate.AddDays(7);
                        //}
                        DateTime ToDate = FromDate;

                        if (!string.IsNullOrEmpty(customerid))
                        { obj.CustomerId = Convert.ToInt32(customerid); }
                        else { obj.CustomerId = 0; }

                        //var timming = objproduct.CheckProductOrderTimimg(Convert.ToInt32(productid));
                        //if (timming.IsTime == false)
                        //{
                        //    dr["status"] = "Fail";
                        //    dr["error_msg"] = timming.message;
                        //    dtNew.Rows.Add(dr);
                        //    return Ok(dtNew);
                        //}
                        DataTable dtDupliAssign = objcust.DuplicateStaffCustomer(null, obj.CustomerId);
                        if (dtDupliAssign.Rows.Count == 0)
                        {
                            dr["status"] = "Fail";
                            dr["error_msg"] = "Deliveryboy not assign...";
                            dtNew.Rows.Add(dr);
                            return Ok(dtNew);
                        }

                        else
                        { obj.DeliveryBoyId = dtDupliAssign.Rows[0]["StaffId"].ToString(); }
                        int societyid = 0;

                        ToDate = FromDate.AddMonths(2);

                        DataTable dtBuildingId = objcust.BindCustomer(obj.CustomerId);

                        if (dtBuildingId.Rows.Count > 0)
                        {
                            //if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["BuildingId"].ToString()))
                            //    societyid = Convert.ToInt32(dtBuildingId.Rows[0]["BuildingId"]);

                            societyid = 0;
                            if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["IsActive"].ToString()))
                            {
                                if (dtBuildingId.Rows[0]["IsActive"].ToString() == "False")
                                {
                                    dr["status"] = "Fail";
                                    dr["error_msg"] = "Account Not Active...";
                                    dtNew.Rows.Add(dr);
                                    return Ok(dtNew);
                                }
                            }
                        }



                        if (!string.IsNullOrEmpty(societyid.ToString()))
                        { obj.BuildingId = Convert.ToInt32(societyid); }
                        else { obj.BuildingId = 0; }

                        obj.Qty = Convert.ToInt32(qty);


                        obj.ProductId = Convert.ToInt32(productid);
                        //get Product detail
                        DataTable dtProduct = objproduct.BindProuctOrder(obj.ProductId, Convert.ToInt32(AttributeId), Convert.ToInt32(VendorId), Convert.ToInt32(VendorcatId));
                        if (dtProduct.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["ProductID"].ToString()))
                                productid = dtProduct.Rows[0]["ProductID"].ToString();
                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SellPrice"].ToString()))
                            {
                                obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]) * obj.Qty;
                                obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]);
                            }
                            else
                            {
                                obj.Amount = 0;
                                obj.SalePrice = 0;
                            }

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["PurchasePrice"].ToString()))
                            {
                                obj.PurchasePrice = Convert.ToDecimal(dtProduct.Rows[0]["PurchasePrice"]) * obj.Qty;

                            }
                            else
                            {
                                obj.PurchasePrice = 0;

                            }

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["MRPPrice"].ToString()))
                            {
                                obj.MRPPrice = Convert.ToDecimal(dtProduct.Rows[0]["MRPPrice"]) * obj.Qty;

                            }
                            else
                            {
                                obj.MRPPrice = 0;

                            }
                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountPrice"].ToString()))
                                obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountPrice"]) * obj.Qty;
                            else
                                obj.Discount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                                obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                            else
                                obj.CGSTPerct = 0;
                            //count cgst Amount
                            if (obj.CGSTPerct > 0)
                                obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                            else
                                obj.CGSTAmount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                                obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                            else
                                obj.IGSTPerct = 0;
                            //count igst Amount
                            if (obj.IGSTPerct > 0)
                                obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                            else
                                obj.IGSTAmount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                                obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                            else
                                obj.SGSTPerct = 0;
                            //count sgst Amount
                            if (obj.SGSTPerct > 0)
                                obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                            else
                                obj.SGSTAmount = 0;

                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                                obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                            else
                                obj.RewardPoint = 0;
                            if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                                obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                            else
                                obj.Profit = 0;

                            //Final Amount
                            obj.TotalFinalAmount = obj.Amount;
                        }

                        if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                        { obj.TotalAmount = obj.TotalFinalAmount; }
                        else { obj.TotalAmount = 0; }

                        obj.TotalGSTAmt = obj.CGSTAmount + obj.SGSTAmount;

                        obj.Status = "Pending";
                        obj.StateCode = null;
                        obj.ProductId = Convert.ToInt32(productid);

                        obj.OrderFlag = "WeekDay";

                        //Calculate wallet with Order Amount 02-01-2022
                        DataTable dt = new DataTable();
                        dt = obj.GetCustomerCredit(obj.CustomerId);

                        if (dt.Rows.Count > 0)
                        {
                            credit = Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString());
                        }
                        DataTable dtprodRecord1 = obj.getCustomerWallet(obj.CustomerId);
                        int userRecords = dtprodRecord1.Rows.Count;
                        if (userRecords > 0)
                        {
                            if (!string.IsNullOrEmpty(dtprodRecord1.Rows[0]["Amt"].ToString()))
                                TotalCredit = Convert.ToDecimal(dtprodRecord1.Rows[0]["Amt"]);
                            if (userRecords > 1)
                            {
                                if (!string.IsNullOrEmpty(dtprodRecord1.Rows[1]["Amt"].ToString()))
                                    TotalDebit = Convert.ToDecimal(dtprodRecord1.Rows[1]["Amt"]);
                            }
                            Walletbal = TotalCredit - TotalDebit;
                        }

                        decimal sumamount = dailyorderamount + obj.TotalAmount;
                        if ((dailyorderamount + obj.TotalAmount) < minorderamount)
                        {
                            if ((dailyorderamount + obj.TotalAmount) < (Walletbal + credit))
                            {
                                msg1 = "Minimum Order Value for this Category is " + minorderamount;
                            }

                            if ((dailyorderamount + obj.TotalAmount) > (Walletbal + credit))
                            {
                                msg1 += ".wallet balance is lower than order value please upload balance before 10:00 PM to process this order ";
                            }
                        }

                        if ((dailyorderamount + obj.TotalAmount) > minorderamount)
                        {
                            if ((dailyorderamount + obj.TotalAmount) > (Walletbal + credit))
                            {
                                msg1 = "wallet balance is lower than order value please upload balance before 10:00 PM to process this order ";
                            }
                        }


                        //send notification
                        if (msg1 != "")
                        {
                            Customer objcustomer = new Customer();
                            Customer objLogin = new Customer();
                            DataTable dtToken = new DataTable();
                            string customerId = obj.CustomerId.ToString();
                            dtToken = objLogin.getCustomerSector(Convert.ToInt32(customerId));
                            if (dtToken.Rows.Count > 0)
                            {
                                objcustomer.UserName = dtToken.Rows[0].ItemArray[1].ToString();
                                objcustomer.SectorId = Convert.ToInt32(dtToken.Rows[0].ItemArray[0]);
                            }

                            string type1 = "Customer";

                            objcustomer.ntitle = "Related Order";
                            objcustomer.ntext = msg1;
                            objcustomer.Photo = "";
                            objcustomer.nlink = "";
                            var addresult = objcustomer.InsertNotification(objcustomer, type1);


                            //DataTable dtToken = new DataTable();

                            int cusnot = 0;

                            objcustomer.nid = addresult.ToString();

                            cusnot = objcustomer.InsertCustomerNotification(objcustomer);

                        }
                        //

                        //var checkAmount = objproduct.CheckProductOrderAmount(Convert.ToInt32(productid), obj.TotalAmount);
                        //if (checkAmount.IsOrderAmount == false)
                        //{
                        //    dr["status"] = "Fail";
                        //    dr["error_msg"] = checkAmount.message;
                        //    dtNew.Rows.Add(dr);
                        //    return Ok(dtNew);
                        //}
                        obj.AttributeId = Convert.ToInt32(AttributeId);
                        obj.VendorId = Convert.ToInt32(VendorId);
                        obj.VendorCatId = Convert.ToInt32(VendorcatId);
                        DateTime _fromDate = FromDate;
                        for (int idate = 0; FromDate <= ToDate; idate++)
                        {
                            //Generate OrderNo
                            con.Open();
                            SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                            com1.CommandType = CommandType.StoredProcedure;
                            var Formno = com1.ExecuteScalar();
                            con.Close();

                            obj.OrderNo = Convert.ToInt32(Formno);
                            obj.OrderDate = FromDate;

                            //get Subscription Id
                            DataTable dtCustSubscription = obj.getCustomerSubscriptionOrderdate(obj);
                            if (dtCustSubscription.Rows.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                                    obj.CustSubscriptionId = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                                else
                                    obj.CustSubscriptionId = 0;
                            }
                            else
                                obj.CustSubscriptionId = 0;

                            AddProductOrder = obj.InsertCustomerOrder(obj);

                            if (AddProductOrder > 0)
                            {
                                obj.OrderId = Convert.ToInt32(AddProductOrder);
                                obj.ProductId = Convert.ToInt32(productid);
                                obj.Qty = Convert.ToInt32(qty);
                                obj.OrderItemDate = FromDate;

                                AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                            }
                            FromDate = FromDate.AddDays(7);


                        }
                        ToDate = FromDate.AddDays(-7);

                        if (AddProductDetail > 0)
                        {
                           // dr["status"] = "Success";
                           // dr["error_msg"] = "Order Placed Successfully";
                            string OrderFlag = "WeekDay";
                            Helper dHelper = new Helper();
                            // dHelper.InsertCustomerOrderTrackNew(obj.CustomerId, obj.ProductId, obj.Qty, _fromDate, ToDate, OrderFlag);
                            //dtNew.Rows.Add(dr);
                            dHelper.InsertCustomerOrderTrackNeworder(obj.CustomerId, obj.ProductId, obj.Qty, _fromDate, ToDate, OrderFlag, Convert.ToInt32(AttributeId));
                        }
                        else
                        {
                           //dr["status"] = "Fail";
                            //dr["error_msg"] = "Order Not Inserted.";
                            //dtNew.Rows.Add(dr);
                        }

                    }

                   
                    }

                if (AddProductDetail > 0)
                {
                    dr["status"] = "Success";
                    dr["error_msg"] = msg.ToString() + msg1.ToString();
                    dtNew.Rows.Add(dr);
                }
                else
                {
                    dr["status"] = "Fail";
                    dr["error_msg"] = "Order Not Inserted.";
                    dtNew.Rows.Add(dr);
                }


            }

                return Ok(dtNew);

        }



        [Route("api/OrderNewApi/CustomerWeekdayOrderDelete/{customerid?}/{productid?}/{startdate}/{status}")]
        [HttpGet]
        public IHttpActionResult CustomerWeekdayOrderDelete(int customerid, int productid, DateTime? startdate, string status)//string strjson
        {



            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            DataTable dtprodRecord = new DataTable();

            if (!string.IsNullOrEmpty(customerid.ToString()) && customerid != 0 && !string.IsNullOrEmpty(productid.ToString()) && productid != 0)
            {


                if (status == "Particular")
                {
                    DateTime Currentdate = Helper.indianTime;
                    DateTime FromDate = Convert.ToDateTime(startdate);
                    DateTime ToDate = DateTime.Now;
                    ToDate = FromDate;

                    if (!string.IsNullOrEmpty(customerid.ToString()))
                    { obj.CustomerId = customerid; }
                    else { obj.CustomerId = 0; }

                    if (!string.IsNullOrEmpty(productid.ToString()))
                    { obj.ProductId = productid; }
                    else { obj.ProductId = 0; }

                    dtprodRecord = obj.getCustomerOrderListweeklyday(obj.CustomerId, obj.ProductId, FromDate, ToDate);
                    int userRecords = dtprodRecord.Rows.Count;
                    obj.OrderId = 0;
                    int DelProductOrder = 0; int DelAddProductDetail = 0;

                    if (userRecords > 0)
                    {
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["OrderDate"].ToString()))
                        {
                            DateTime CurrentDate = Helper.indianTime.AddDays(1);
                            DateTime OrderDate = Convert.ToDateTime(dtprodRecord.Rows[0]["OrderDate"]);
                            if (CurrentDate.Day == OrderDate.Day && CurrentDate.Month == OrderDate.Month && CurrentDate.Year == OrderDate.Year)
                            {
                                var timming = objproduct.CheckProductOrderTimimg(obj.ProductId);
                                if (timming.IsTime == false)
                                {
                                    dr["status"] = "Fail";
                                    dr["error_msg"] = "Unable to delete... - " + timming.message;
                                    dtNew.Rows.Add(dr);
                                    return Ok(dtNew);
                                }
                            }
                        }

                        for (int i = 0; i < userRecords; i++)
                        {
                            if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Id"].ToString()))
                            {
                                obj.OrderId = Convert.ToInt32(dtprodRecord.Rows[i]["Id"]);
                                obj.OrderDate = FromDate;
                                DelProductOrder = obj.DeleteCustomerOrder(obj.OrderId);
                                FromDate = FromDate.AddDays(7);
                            }
                            else
                            {
                                dr["status"] = "Success";
                                dr["error_msg"] = "Order Not Found";
                            }
                        }


                        if (DelProductOrder > 0)
                        {
                            obj.DeleteCustomerOrderTrack(obj.CustomerId, obj.ProductId);
                            dr["status"] = "Success";
                            dr["error_msg"] = "Order Deleted Successfully";
                        }
                        else
                        {
                            dr["status"] = "Failed";
                            dr["error_msg"] = "Order Not Deleted";
                        }

                    }
                    else
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Not Found";
                    }

                    dtNew.Rows.Add(dr);
                    return Ok(dtNew);

                }
                if (status == "Complete")
                {
                    DateTime Currentdate = Helper.indianTime;
                    DateTime FromDate = Convert.ToDateTime(startdate);
                    DateTime ToDate = DateTime.Now;
                    ToDate = FromDate.AddMonths(3);

                    if (!string.IsNullOrEmpty(customerid.ToString()))
                    { obj.CustomerId = customerid; }
                    else { obj.CustomerId = 0; }

                    if (!string.IsNullOrEmpty(productid.ToString()))
                    { obj.ProductId = productid; }
                    else { obj.ProductId = 0; }

                    dtprodRecord = obj.getCustomerOrderListweeklyday(obj.CustomerId, obj.ProductId, FromDate, ToDate);
                    int userRecords = dtprodRecord.Rows.Count;
                    obj.OrderId = 0;
                    int DelProductOrder = 0; int DelAddProductDetail = 0;

                    if (userRecords > 0)
                    {
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["OrderDate"].ToString()))
                        {
                            DateTime CurrentDate = Helper.indianTime.AddDays(1);
                            DateTime OrderDate = Convert.ToDateTime(dtprodRecord.Rows[0]["OrderDate"]);
                            if (CurrentDate.Day == OrderDate.Day && CurrentDate.Month == OrderDate.Month && CurrentDate.Year == OrderDate.Year)
                            {
                                var timming = objproduct.CheckProductOrderTimimg(obj.ProductId);
                                if (timming.IsTime == false)
                                {
                                    dr["status"] = "Fail";
                                    dr["error_msg"] = "Unable to delete... - " + timming.message;
                                    dtNew.Rows.Add(dr);
                                    return Ok(dtNew);
                                }
                            }
                        }

                        for (int i = 0; i < userRecords; i++)
                        {
                            if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Id"].ToString()))
                            {
                                obj.OrderId = Convert.ToInt32(dtprodRecord.Rows[i]["Id"]);
                                obj.OrderDate = FromDate;
                                
                                DelProductOrder = obj.DeleteCustomerOrderw(obj.OrderId,FromDate);
                                FromDate = FromDate.AddDays(7);
                            }
                            else
                            {
                                dr["status"] = "Success";
                                dr["error_msg"] = "Order Not Found";
                            }
                        }

                        FromDate = FromDate.AddDays(-7);
                        if (DelProductOrder > 0)
                        {
                            obj.DeleteCustomerOrderTrackw(obj.CustomerId, obj.ProductId,FromDate);
                            dr["status"] = "Success";
                            dr["error_msg"] = "Order Deleted Successfully";
                        }
                        else
                        {
                            dr["status"] = "Failed";
                            dr["error_msg"] = "Order Not Deleted";
                        }

                    }
                    else
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Order Not Found";
                    }

                    dtNew.Rows.Add(dr);
                    return Ok(dtNew);




                }



            }


            return Ok(dtprodRecord);


           // return Ok("");
        }


        [Route("api/OrderNewApi/CustomerFlpAmzOrderAdd/{customerid?}/{Orderid?}/{OrderDate}/{OrderFrom}")]
        [HttpGet]

        public IHttpActionResult CustomerFlpAmzOrderAdd(int customerid, string Orderid, DateTime? OrderDate, string OrderFrom)//string strjson
        {

            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();


            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            int AddProductOrder = 0; int AddProductDetail = 0;

            if (!string.IsNullOrEmpty(customerid.ToString()) && Convert.ToInt32(customerid) != 0 && !string.IsNullOrEmpty(Orderid))
            {


                int societyid = 0;
                obj.CustomerId = customerid;


                DataTable dtBuildingId = objcust.BindCustomerNew(obj.CustomerId);

                if (dtBuildingId.Rows.Count > 0)
                {
                    //if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["BuildingId"].ToString()))
                    //    societyid = Convert.ToInt32(dtBuildingId.Rows[0]["BuildingId"]);

                    societyid = 0;
                    if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["IsActive"].ToString()))
                    {
                        if (dtBuildingId.Rows[0]["IsActive"].ToString() == "False")
                        {
                            dr["status"] = "Fail";
                            dr["error_msg"] = "Account Not Active...";
                            dtNew.Rows.Add(dr);
                            return Ok(dtNew);
                        }

                        objcust.CustomerId = customerid;
                        objcust.FirstName = dtBuildingId.Rows[0]["FirstName"].ToString();
                        objcust.LastName = dtBuildingId.Rows[0]["LastName"].ToString();
                        objcust.MobileNo = dtBuildingId.Rows[0]["MobileNo"].ToString();
                        objcust.SectorId =Convert.ToInt16(dtBuildingId.Rows[0]["SectorId"]);
                        objcust.SectorName = dtBuildingId.Rows[0]["SectorName"].ToString();


                        DateTime OrderDate1 = Convert.ToDateTime(OrderDate);
                        string orderfrom1 = OrderFrom;

                        string delimStr = ",";
                        char[] delimiter = delimStr.ToCharArray();

                        foreach (string s in Orderid.Split(delimiter))
                        {


                            DataTable dtOrderId = objcust.getCustomerFlpAmzOrderViewparticular(objcust, s, OrderDate1, orderfrom1);

                            if(dtOrderId.Rows.Count>0)

                            {
                                dr["status"] = "Error";
                                dr["error_msg"] = "You Already Applied Cashback For This Order Id , From " +orderfrom1;


                                dtNew.Rows.Add(dr);
                                return Ok(dtNew);
                            }
                            AddProductOrder = objcust.InsertCustomerOrder(objcust, s, OrderDate1, orderfrom1);
                        }
                            if (AddProductOrder>0)
                        {
                            dr["status"] = "Success";
                            dr["error_msg"] = "Your Amazon/flipkart Order Placed Successfully";
                           

                            dtNew.Rows.Add(dr);
                            return Ok(dtNew);
                        }

                        else
                        {

                            dr["status"] = "Fail";
                            dr["error_msg"] = "Choose Properly";


                            dtNew.Rows.Add(dr);
                            return Ok(dtNew);

                        }

                       // return Ok(dtBuildingId);
                    }
                }
            }


                return Ok("");
        }



       


        [Route("api/OrderNewApi/CustomerFlpAmzOrderView")]
        [HttpGet]

        public IHttpActionResult CustomerFlpAmzOrderView()//string strjson
        {

            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();


            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            //obj.StaffId = Convert.ToInt32(Deliveryboyid);

            Subscription order = new Subscription();
            //ViewBag.FromDate = _fdate.ToString("dd-MMM-yyyy");
            //ViewBag.FromDate = FDate;
            //ViewBag.ToDate = TDate;
            var dtList = objcust.getCustomerFlpAmzOrderView();


            return Ok(dtList);
        }


        [Route("api/OrderNewApi/AddCustomerOrderCart/{customerid?}/{productid?}/{qty?}/{startdate?}/{AttributeId?}/{VendorId?}/{VendorcatId?}/{SectorId?}/{DmId?}/{CusType?}")]
        [HttpGet]
        public IHttpActionResult AddCustomerOrderCart(string customerid, string productid, string qty, DateTime? startdate, string AttributeId, string VendorId, string VendorcatId, string SectorId, string DmId, string CusType)
        {
            Vendor objvendor = new Vendor();
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();


            DataTable dtNew = new DataTable();

            DataTable dtVendor = new DataTable();

            dtVendor = objvendor.getVendorid(Convert.ToInt32(VendorcatId));

           

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            long AddProductOrder = 0; int AddProductDetail = 0;
            if (!string.IsNullOrEmpty(customerid) && Convert.ToInt32(customerid) != 0 && !string.IsNullOrEmpty(productid) && Convert.ToInt32(productid) != 0 && !string.IsNullOrEmpty(qty) && Convert.ToInt32(qty) != 0 && !string.IsNullOrEmpty(startdate.ToString()))
            {
                int r = 0;
                string msg = "Order Cart Successfully";
                string msg1 = "";
               
                DateTime FromDate = Convert.ToDateTime(startdate);
               
                //if (DateTime.Now.Hour < dbcutOfftime)
              

                DateTime ToDate = FromDate;


                if (!string.IsNullOrEmpty(customerid))
                { obj.CustomerId = Convert.ToInt32(customerid); }
                else { obj.CustomerId = 0; }



                DataTable dtDupliAssign = objcust.DuplicateStaffCustomer(null, obj.CustomerId);
                if (dtDupliAssign.Rows.Count == 0)
                {
                    dr["status"] = "Fail";
                    dr["error_msg"] = "Deliveryboy not assign...";
                    dtNew.Rows.Add(dr);
                    return Ok(dtNew);
                }
                else
                { obj.DeliveryBoyId = dtDupliAssign.Rows[0]["StaffId"].ToString(); }


                int societyid = 0;

                

                DataTable dtBuildingId = objcust.BindCustomer(obj.CustomerId);

                if (dtBuildingId.Rows.Count > 0)
                {
                    //if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["BuildingId"].ToString()))
                    //    societyid = Convert.ToInt32(dtBuildingId.Rows[0]["BuildingId"]);

                    societyid = 0;
                    if (!string.IsNullOrEmpty(dtBuildingId.Rows[0]["IsActive"].ToString()))
                    {
                        if (dtBuildingId.Rows[0]["IsActive"].ToString() == "False")
                        {
                            dr["status"] = "Fail";
                            dr["error_msg"] = "Account Not Active...";
                            dtNew.Rows.Add(dr);
                            return Ok(dtNew);
                        }
                    }
                }



                if (!string.IsNullOrEmpty(societyid.ToString()))
                { obj.BuildingId = Convert.ToInt32(societyid); }
                else { obj.BuildingId = 0; }

                obj.Qty = Convert.ToInt32(qty);


                obj.ProductId = Convert.ToInt32(productid);

                //get Product detail
                DataTable dtProduct = objproduct.BindProuctOrder(obj.ProductId, Convert.ToInt32(AttributeId), Convert.ToInt32(VendorId), Convert.ToInt32(VendorcatId));
                if (dtProduct.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["ProductID"].ToString()))
                        productid = dtProduct.Rows[0]["ProductID"].ToString();

                    if (string.IsNullOrEmpty(CusType) || CusType == "General")
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SellPrice"].ToString()))
                        {
                            obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]) * obj.Qty;
                            obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]);
                        }
                        else
                        {
                            obj.Amount = 0;
                            obj.SalePrice = 0;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["B2BSellPrice"].ToString()))
                        {
                            obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["B2BSellPrice"]) * obj.Qty;
                            obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["B2BSellPrice"]);
                        }
                        else
                        {
                            obj.Amount = 0;
                            obj.SalePrice = 0;
                        }
                    }


                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["PurchasePrice"].ToString()))
                    {
                        obj.PurchasePrice = Convert.ToDecimal(dtProduct.Rows[0]["PurchasePrice"]) * obj.Qty;

                    }
                    else
                    {
                        obj.PurchasePrice = 0;

                    }

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["MRPPrice"].ToString()))
                    {
                        obj.MRPPrice = Convert.ToDecimal(dtProduct.Rows[0]["MRPPrice"]) * obj.Qty;

                    }
                    else
                    {
                        obj.MRPPrice = 0;

                    }
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountPrice"].ToString()))
                        obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountPrice"]) * obj.Qty;
                    else
                        obj.Discount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                        obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                    else
                        obj.CGSTPerct = 0;
                    //count cgst Amount
                    if (obj.CGSTPerct > 0)
                        obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                    else
                        obj.CGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                        obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                    else
                        obj.IGSTPerct = 0;
                    //count igst Amount
                    if (obj.IGSTPerct > 0)
                        obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                    else
                        obj.IGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                        obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                    else
                        obj.SGSTPerct = 0;
                    //count sgst Amount
                    if (obj.SGSTPerct > 0)
                        obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                    else
                        obj.SGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                        obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                    else
                        obj.RewardPoint = 0;
                    if (string.IsNullOrEmpty(CusType) || CusType == "General")
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                            obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                        else
                            obj.Profit = 0;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["B2BProfit"].ToString()))
                            obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["B2BProfit"]) * obj.Qty;
                        else
                            obj.Profit = 0;
                    }


                    //Final Amount
                    obj.TotalFinalAmount = obj.Amount;
                }

                if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                { obj.TotalAmount = obj.TotalFinalAmount; }
                else { obj.TotalAmount = 0; }

                obj.TotalGSTAmt = obj.CGSTAmount + obj.SGSTAmount;

                obj.Status = "Cart";
                obj.StateCode = null;
                obj.ProductId = Convert.ToInt32(productid);
                   

          

                //send notification
               
                //

                DateTime _fromDate = FromDate;
                for (int idate = 0; FromDate <= ToDate; idate++)
                {
                    
                    obj.AttributeId = Convert.ToInt32(AttributeId);
                    obj.VendorId = Convert.ToInt32(VendorId);
                    obj.VendorCatId = Convert.ToInt32(VendorcatId);
                    obj.SectorId = Convert.ToInt32(SectorId);
                   // AddProductOrder = obj.InsertCustomerOrder(obj);

                  
                        obj.OrderId = Convert.ToInt32(AddProductOrder);
                        obj.ProductId = Convert.ToInt32(productid);
                        obj.Qty = Convert.ToInt32(qty);
                        obj.OrderItemDate = FromDate;

                        AddProductDetail = obj.InsertCustomerOrderCart(obj);

                    FromDate = FromDate.AddDays(1);


                }
               // ToDate = FromDate.AddDays(-2);

                if (AddProductDetail > 0)
                {
                    dr["status"] = "Success";
                    dr["error_msg"] = msg.ToString() + msg1.ToString();
                   
                    dtNew.Rows.Add(dr);
                    return Ok(dtNew);
                }
                else
                {
                    dr["status"] = "Fail";
                    dr["error_msg"] = "Order Not Cart.";
                }
            }
            else
            {
                dr["status"] = "Failed";
                dr["error_msg"] = "Please Fill Correct Details";
            }


            return Ok(dtNew);
        }

        [Route("api/OrderNewApi/UpdateOrderCart/{customerid?}/{productid?}/{qty?}/{CartId?}/{AttributeId?}/{VendorId?}/{VendorcatId?}/{CusType?}")]
        [HttpGet]
        public IHttpActionResult UpdateOrderCart(string customerid, string productid, string qty,string CartId, string AttributeId, string VendorId, string VendorcatId,string CusType)
        {
            Vendor objvendor = new Vendor();
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();
            DataTable dtNew = new DataTable();
            DataTable dtVendor = new DataTable();

            long AddProductOrder = 0; int AddProductDetail = 0;
            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();
       
            if (!string.IsNullOrEmpty(customerid) && Convert.ToInt32(customerid) != 0 && !string.IsNullOrEmpty(productid) && Convert.ToInt32(productid) != 0 && !string.IsNullOrEmpty(qty) && Convert.ToInt32(qty) != 0)
            {
                int r = 0;
                string msg = "Cart Updated Successfully";
                string msg1 = "";

                obj.Qty = Convert.ToInt32(qty);
                obj.ProductId = Convert.ToInt32(productid);

                //get Product detail
                DataTable dtProduct = objproduct.BindProuctOrder(obj.ProductId, Convert.ToInt32(AttributeId), Convert.ToInt32(VendorId), Convert.ToInt32(VendorcatId));
                if (dtProduct.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["ProductID"].ToString()))
                        productid = dtProduct.Rows[0]["ProductID"].ToString();

                    if (string.IsNullOrEmpty(CusType) || CusType == "General")
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SellPrice"].ToString()))
                        {
                            obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]) * obj.Qty;
                            obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["SellPrice"]);
                        }
                        else
                        {
                            obj.Amount = 0;
                            obj.SalePrice = 0;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["B2BSellPrice"].ToString()))
                        {
                            obj.Amount = Convert.ToDecimal(dtProduct.Rows[0]["B2BSellPrice"]) * obj.Qty;
                            obj.SalePrice = Convert.ToDecimal(dtProduct.Rows[0]["B2BSellPrice"]);
                        }
                        else
                        {
                            obj.Amount = 0;
                            obj.SalePrice = 0;
                        }
                    }


                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["PurchasePrice"].ToString()))
                    {
                        obj.PurchasePrice = Convert.ToDecimal(dtProduct.Rows[0]["PurchasePrice"]) * obj.Qty;

                    }
                    else
                    {
                        obj.PurchasePrice = 0;

                    }

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["MRPPrice"].ToString()))
                    {
                        obj.MRPPrice = Convert.ToDecimal(dtProduct.Rows[0]["MRPPrice"]) * obj.Qty;

                    }
                    else
                    {
                        obj.MRPPrice = 0;

                    }
                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["DiscountPrice"].ToString()))
                        obj.Discount = Convert.ToDecimal(dtProduct.Rows[0]["DiscountPrice"]) * obj.Qty;
                    else
                        obj.Discount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["CGST"].ToString()))
                        obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["CGST"]) * obj.Qty;
                    else
                        obj.CGSTPerct = 0;
                    //count cgst Amount
                    if (obj.CGSTPerct > 0)
                        obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                    else
                        obj.CGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["IGST"].ToString()))
                        obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["IGST"]) * obj.Qty;
                    else
                        obj.IGSTPerct = 0;
                    //count igst Amount
                    if (obj.IGSTPerct > 0)
                        obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                    else
                        obj.IGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["SGST"].ToString()))
                        obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SGST"]) * obj.Qty;
                    else
                        obj.SGSTPerct = 0;
                    //count sgst Amount
                    if (obj.SGSTPerct > 0)
                        obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                    else
                        obj.SGSTAmount = 0;

                    if (!string.IsNullOrEmpty(dtProduct.Rows[0]["RewardPoint"].ToString()))
                        obj.RewardPoint = Convert.ToInt64(dtProduct.Rows[0]["RewardPoint"]) * obj.Qty;
                    else
                        obj.RewardPoint = 0;
                    if (string.IsNullOrEmpty(CusType) || CusType == "General")
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["Profit"].ToString()))
                            obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["Profit"]) * obj.Qty;
                        else
                            obj.Profit = 0;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dtProduct.Rows[0]["B2BProfit"].ToString()))
                            obj.Profit = Convert.ToDecimal(dtProduct.Rows[0]["B2BProfit"]) * obj.Qty;
                        else
                            obj.Profit = 0;
                    }


                    //Final Amount
                    obj.TotalFinalAmount = obj.Amount;
                }

                if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
                { obj.TotalAmount = obj.TotalFinalAmount; }
                else { obj.TotalAmount = 0; }

                obj.TotalGSTAmt = obj.CGSTAmount + obj.SGSTAmount;

                obj.AttributeId = Convert.ToInt32(AttributeId);
                obj.VendorId = Convert.ToInt32(VendorId);
                obj.VendorCatId = Convert.ToInt32(VendorcatId);

                // AddProductOrder = obj.InsertCustomerOrder(obj);


                obj.CustomerId =Convert.ToInt32(customerid);
                obj.ProductId = Convert.ToInt32(productid);
                obj.Qty = Convert.ToInt32(qty);
                obj.Id =Convert.ToInt32(CartId);

                AddProductDetail = obj.UpdateCustomerOrderCart(obj);

                if (AddProductDetail > 0)
                {
                    dr["status"] = "Success";
                    dr["error_msg"] = msg.ToString() ;

                    dtNew.Rows.Add(dr);
                    return Ok(dtNew);
                }
                else
                {
                    dr["status"] = "Fail";
                    dr["error_msg"] = "Cart Not Updated.";
                    dtNew.Rows.Add(dr);
                }

            }
            return Ok(dtNew);
        }

        [Route("api/OrderNewApi/CartOrderPlace/{CustomerId?}/{startdate?}/{PayType?}/{PaidAmount?}/{TransactionId?}")]
        [HttpGet]
        public IHttpActionResult CartOrderPlace(string CustomerId,string CartId, DateTime? startdate,string PayType,string PaidAmount,string TransactionId)
        {
            DateTime centuryBegin = new DateTime(2001, 1, 1);
            var currentDate = Helper.indianTime;
            long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
            TimeSpan elapsedSpan = TimeSpan.Parse(currentDate.ToString("HH:mm"));
            string curhour = elapsedSpan.ToString();
            curhour = curhour.Substring(0, 2);
            DataTable dtNew = new DataTable();
            decimal minorderamount = 0, dailyorderamount = 0;
            decimal credit = 0;
            decimal Walletbal = 0, TotalCredit = 0, TotalDebit = 0;
            Vendor objvendor = new Vendor();
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();
            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();
            DataTable dt = new DataTable();
            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            string a = "";
            DateTime FromDate = Convert.ToDateTime(startdate);
            obj.Status = "Complete";
            obj.StateCode = null;
            obj.OrderFlag = "SingleDay";
            string msg = "Order Placed Succesfully";
            long AddProductOrder = 0; int AddProductDetail = 0;int UpdateCart =0;
            int AddWallet = 0; int Addpayment=0;
            foreach (string s in CartId.Split(delimiter))
            {
                AddProductOrder = 0; AddProductDetail = 0; Addpayment=0;
                dt = objproduct.getCartdetail(Convert.ToInt32(s));
                if (dt.Rows.Count > 0)
                {
                    con.Open();
                    SqlCommand com1 = new SqlCommand("Generate_OrderNo", con);
                    com1.CommandType = CommandType.StoredProcedure;
                    var Formno = com1.ExecuteScalar();
                    con.Close();

                    obj.OrderNo = Convert.ToInt32(Formno);
                    obj.OrderDate = FromDate;

                    //get Subscription Id
                    DataTable dtCustSubscription = obj.getCustomerSubscriptionOrderdate(obj);
                    if (dtCustSubscription.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dtCustSubscription.Rows[0]["Id"].ToString()))
                            obj.CustSubscriptionId = Convert.ToInt32(dtCustSubscription.Rows[0]["Id"]);
                        else
                            obj.CustSubscriptionId = 0;
                    }
                    else
                        obj.CustSubscriptionId = 0;
                    //


                    obj.CustomerId = Convert.ToInt32(CustomerId);
                    if (!string.IsNullOrEmpty(dt.Rows[0]["ProductId"].ToString()))
                        obj.ProductId =Convert.ToInt32(dt.Rows[0]["ProductId"].ToString());


                    obj.Amount = Convert.ToDecimal(dt.Rows[0]["Amount"]);
                    obj.SalePrice = Convert.ToDecimal(dt.Rows[0]["SalePrice"]);


                    if (!string.IsNullOrEmpty(dt.Rows[0]["PurchasePrice"].ToString()))
                    {
                        obj.PurchasePrice = Convert.ToDecimal(dt.Rows[0]["PurchasePrice"]);

                    }
                    else
                    {
                        obj.PurchasePrice = 0;

                    }

                    if (!string.IsNullOrEmpty(dt.Rows[0]["Mrp"].ToString()))
                    {
                        obj.MRPPrice = Convert.ToDecimal(dt.Rows[0]["Mrp"]);

                    }
                    else
                    {
                        obj.MRPPrice = 0;

                    }
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Discount"].ToString()))
                        obj.Discount = Convert.ToDecimal(dt.Rows[0]["Discount"]);
                    else
                        obj.Discount = 0;

                    if (!string.IsNullOrEmpty(dt.Rows[0]["CGSTAmount"].ToString()))
                        obj.CGSTAmount = Convert.ToDecimal(dt.Rows[0]["CGSTAmount"]);
                    else
                        obj.CGSTAmount = 0;
                    //count cgst Amount
              

                    if (!string.IsNullOrEmpty(dt.Rows[0]["IGSTAmount"].ToString()))
                        obj.IGSTAmount = Convert.ToDecimal(dt.Rows[0]["IGSTAmount"]);
                    else
                        obj.IGSTAmount = 0;
                    //count igst Amount
                 

                    if (!string.IsNullOrEmpty(dt.Rows[0]["SGSTAmount"].ToString()))
                        obj.SGSTAmount = Convert.ToDecimal(dt.Rows[0]["SGSTAmount"]);
                    else
                        obj.SGSTAmount = 0;
                    //count sgst Amount
                    

                    if (!string.IsNullOrEmpty(dt.Rows[0]["RewardPoint"].ToString()))
                        obj.RewardPoint = Convert.ToInt64(dt.Rows[0]["RewardPoint"]);
                    else
                        obj.RewardPoint = 0;

                    if (!string.IsNullOrEmpty(dt.Rows[0]["Profit"].ToString()))
                        obj.Profit = Convert.ToDecimal(dt.Rows[0]["Profit"]);
                    else
                        obj.Profit = 0;



                    //Final Amount
                    obj.TotalFinalAmount = Convert.ToDecimal(dt.Rows[0]["TotalFinalAmount"]);
                    obj.Qty = Convert.ToInt32(dt.Rows[0]["Qty"]);

                    obj.AttributeId = Convert.ToInt32(dt.Rows[0]["AttributeId"]);
                    obj.VendorId = Convert.ToInt32(dt.Rows[0]["VendorId"]);
                    obj.VendorCatId = Convert.ToInt32(dt.Rows[0]["VendorCatId"]);
                    obj.SectorId = Convert.ToInt32(dt.Rows[0]["SectorId"]);
                    obj.DeliveryBoyId =dt.Rows[0]["DmId"].ToString();
                    AddProductOrder = obj.InsertCustomerOrder(obj);

                    if (AddProductOrder > 0)
                    {
                        obj.OrderId = Convert.ToInt32(AddProductOrder);
                        
                        obj.OrderItemDate = FromDate;

                        AddProductDetail = obj.InsertCustomerOrderDetail(obj);

                        obj.Id = Convert.ToInt32(s);
                        UpdateCart = obj.UpdateCustomerOrderCartStatus(obj);
                    }

                    if (PayType == "Wallet")
                    {
                       DataTable dtDateOrderSchel = objproduct.getCustomerOrderIdwiseList(obj.CustomerId, obj.OrderItemDate, obj.OrderId);

                        if (dtDateOrderSchel.Rows.Count > 0)
                        {
                            //chcek wallet balance
                           // decimal Walletbal = 0, TotalCredit = 0, TotalDebit = 0, Last2days = 0; bool AllowOrderWallet = false;
                            DataTable dtprodRecord = new DataTable();
                            //wallet balance

                         

                           
                            dt = obj.GetCustomerCredit(obj.CustomerId);

                            

                            int TotalRewardPoint = 0;
                            int UpdateOrderStatus = 0, UpdateOrderStatusCancle = 0;
                            //find order amount deduct from wallet
                            for (int j = 0; j < dtDateOrderSchel.Rows.Count; j++)
                            {
                                string pname = "", proqty = "0", AttributeName = "";
                                obj.CustomerId = Convert.ToInt32(dtDateOrderSchel.Rows[j]["CustomerId"]);
                                //  obj.TransactionDate = DateTime.Now.AddDays(1);
                                obj.TransactionDate = Convert.ToDateTime(dtDateOrderSchel.Rows[j]["OrderDate"]);
                                obj.Amount = Convert.ToDecimal(dtDateOrderSchel.Rows[j]["Amount"]);
                                obj.OrderId = Convert.ToInt32(dtDateOrderSchel.Rows[j]["OId"]);
                                pname = dtDateOrderSchel.Rows[j]["ProductName"].ToString();
                                AttributeName = dtDateOrderSchel.Rows[j]["AttributeName"].ToString();
                                obj.proqty = dtDateOrderSchel.Rows[j]["Qty"].ToString();
                                obj.Description = pname + "-" + AttributeName;
                                obj.Status = "Purchase";
                                obj.Type = "Debit";
                                obj.CustSubscriptionId = 0;
                                obj.TransactionType = Convert.ToInt32(Helper.TransactionType.Purchase);
                                obj.RewardPoint = Convert.ToInt64(dtDateOrderSchel.Rows[j]["RewardPoint"]);
                                //Dibakar 31-12-2022 chk wallet balance+ credit < or > of Amount
                                //wallet balance

                                AddWallet = obj.InsertWalletScheduleOrder(obj);
                            }
                          
                        }

                        
                    }
                   
                  

                }
            }

            if (PayType == "General")
            {
                obj.CustomerId = Convert.ToInt32(CustomerId);
                obj.GeneralPayAmount = Convert.ToDecimal(PaidAmount);
                obj.GeneralPayTransactionId = TransactionId;
                obj.OrderDate = FromDate;
                Addpayment = obj.InsertGeneralPayment(obj);
                    
           }

            if (AddProductDetail > 0)
            {
                dr["status"] = "Success";
                dr["error_msg"] = msg.ToString();
                
                dtNew.Rows.Add(dr);
                return Ok(dtNew);
            }
            else
            {
                dr["status"] = "Fail";
                dr["error_msg"] = "Order Not Inserted.";
            }
            return Ok(dtNew);

        }



        [Route("api/OrderNewApi/DeleteOrderCart/{CartId?}")]
        [HttpGet]
        public IHttpActionResult DeleteOrderCart(string CartId)
        {
            Vendor objvendor = new Vendor();
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();
            DataTable dtNew = new DataTable();
            DataTable dtVendor = new DataTable();

            long AddProductOrder = 0; int AddProductDetail = 0;
            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();
            obj.Id = Convert.ToInt32(CartId);
          int  UpdateCart = obj.DeleteCart(obj);


            if (UpdateCart > 0)
            {
                dr["status"] = "Success";
                dr["error_msg"] = "Cart Item Deleted";

                dtNew.Rows.Add(dr);
                return Ok(dtNew);
            }
            else
            {
                dr["status"] = "Fail";
                dr["error_msg"] = "Not Deleted.";
            }
            return Ok(dtNew);
        }
    }
}

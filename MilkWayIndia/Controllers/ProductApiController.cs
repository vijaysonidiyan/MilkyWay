using MilkWayIndia.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace MilkWayIndia.Controllers
{
    public class ProductApiController : ApiController
    {
        Models.Helper dHelper = new Models.Helper();

        [Route("api/ProductApi/ProductCategoryList")]
        [HttpGet]
        public HttpResponseMessage ProductCategoryList() //JsonResult
        {
            Product obj = new Product();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.BindCategory(null);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("categoryname", typeof(string));
                dtNew.Columns.Add("image", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["id"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    dr["categoryname"] = dtprodRecord.Rows[i]["CategoryName"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Image"].ToString()))
                    {
                        var encoded = Uri.EscapeUriString(dtprodRecord.Rows[i]["Image"].ToString().Trim());
                        dr["image"] = Helper.PhotoFolderPath + "/image/productcategory/" + encoded;
                    }
                    else
                        dr["image"] = "";
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["categories"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""categories"":" + dict["categories"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["categories"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""categories"":" + dict["categories"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }

        [Route("api/ProductApi/ProductList")]
        [HttpGet]
        public HttpResponseMessage ProductList() //JsonResult
        {
            Product obj = new Product();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.BindProuct(null);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("categoryid", typeof(Int32));
                dtNew.Columns.Add("product", typeof(string));
                dtNew.Columns.Add("code", typeof(string));
                dtNew.Columns.Add("price", typeof(decimal));
                dtNew.Columns.Add("discountamt", typeof(decimal));
                dtNew.Columns.Add("cgst", typeof(decimal));
                dtNew.Columns.Add("sgst", typeof(decimal));
                dtNew.Columns.Add("igst", typeof(decimal));
                dtNew.Columns.Add("rewardpoint", typeof(Int64));
                dtNew.Columns.Add("detail", typeof(string));
                dtNew.Columns.Add("image", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["id"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    dr["categoryid"] = dtprodRecord.Rows[i]["CategoryId"].ToString().Trim();
                    dr["product"] = dtprodRecord.Rows[i]["ProductName"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Code"].ToString()))
                        dr["code"] = dtprodRecord.Rows[i]["Code"].ToString().Trim();
                    else
                        dr["code"] = "";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Price"].ToString()))
                        dr["price"] = Convert.ToDecimal(dtprodRecord.Rows[i]["Price"]);
                    else
                        dr["price"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["DiscountAmount"].ToString()))
                        dr["discountamt"] = Convert.ToDecimal(dtprodRecord.Rows[i]["DiscountAmount"]);
                    else
                        dr["discountamt"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["CGST"].ToString()))
                        dr["cgst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["CGST"]);
                    else
                        dr["cgst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["SGST"].ToString()))
                        dr["sgst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["SGST"]);
                    else
                        dr["sgst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["IGST"].ToString()))
                        dr["igst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["IGST"]);
                    else
                        dr["igst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["RewardPoint"].ToString()))
                        dr["rewardpoint"] = Convert.ToInt64(dtprodRecord.Rows[i]["RewardPoint"]);
                    else
                        dr["rewardpoint"] = "0";
                    dr["detail"] = dtprodRecord.Rows[i]["Detail"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Image"].ToString()))
                    {
                        var encoded = Uri.EscapeUriString(dtprodRecord.Rows[i]["Image"].ToString().Trim());
                        dr["image"] = Helper.PhotoFolderPath + "/image/product/" + encoded;
                    }
                    else
                        dr["image"] = "";
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["products"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""products"":" + dict["products"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }

            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["products"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""products"":" + dict["products"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }



        }

        [Route("api/ProductApi/CategoryWiseProductList/{CategoryId?}")]
        [HttpGet]
        public HttpResponseMessage CategoryWiseProductList(Int32? CategoryId) //JsonResult
        {
            Product obj = new Product();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.BindCategProuct(CategoryId);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("categoryid", typeof(Int32));
                dtNew.Columns.Add("product", typeof(string));
                dtNew.Columns.Add("code", typeof(string));
                dtNew.Columns.Add("price", typeof(decimal));
                dtNew.Columns.Add("discountamt", typeof(decimal));
                dtNew.Columns.Add("cgst", typeof(decimal));
                dtNew.Columns.Add("sgst", typeof(decimal));
                dtNew.Columns.Add("igst", typeof(decimal));
                dtNew.Columns.Add("rewardpoint", typeof(Int64));
                dtNew.Columns.Add("detail", typeof(string));
                dtNew.Columns.Add("image", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["id"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    dr["categoryid"] = dtprodRecord.Rows[i]["CategoryId"].ToString().Trim();
                    dr["product"] = dtprodRecord.Rows[i]["ProductName"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Code"].ToString()))
                        dr["code"] = dtprodRecord.Rows[i]["Code"].ToString().Trim();
                    else
                        dr["code"] = "";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Price"].ToString()))
                        dr["price"] = Convert.ToDecimal(dtprodRecord.Rows[i]["Price"]);
                    else
                        dr["price"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["DiscountAmount"].ToString()))
                        dr["discountamt"] = Convert.ToDecimal(dtprodRecord.Rows[i]["DiscountAmount"]);
                    else
                        dr["discountamt"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["CGST"].ToString()))
                        dr["cgst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["CGST"]);
                    else
                        dr["cgst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["SGST"].ToString()))
                        dr["sgst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["SGST"]);
                    else
                        dr["sgst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["IGST"].ToString()))
                        dr["igst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["IGST"]);
                    else
                        dr["igst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["RewardPoint"].ToString()))
                        dr["rewardpoint"] = Convert.ToInt64(dtprodRecord.Rows[i]["RewardPoint"]);
                    else
                        dr["rewardpoint"] = "0";
                    dr["detail"] = dtprodRecord.Rows[i]["Detail"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Image"].ToString()))
                    {
                        var encoded = Uri.EscapeUriString(dtprodRecord.Rows[i]["Image"].ToString().Trim());
                        dr["image"] = Helper.PhotoFolderPath + "/image/product/" + encoded;
                    }
                    else
                        dr["image"] = "";

                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["products"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""products"":" + dict["products"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["products"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""products"":" + dict["products"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }



        }

        [Route("api/ProductApi/SearchCategoryProductList/{SectorId?}/{Search?}")]
        [HttpGet]
        public HttpResponseMessage SearchCategoryProductList(string SectorId, string Search) //JsonResult
        {
            Product obj = new Product();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            // dtprodRecord = obj.SearchCategProuct(Search); 
            dtprodRecord = obj.SearchSectorCategProuct(SectorId, Search);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("categoryid", typeof(Int32));
                dtNew.Columns.Add("product", typeof(string));
                dtNew.Columns.Add("code", typeof(string));
                dtNew.Columns.Add("price", typeof(decimal));
                dtNew.Columns.Add("discountamt", typeof(decimal));
                dtNew.Columns.Add("cgst", typeof(decimal));
                dtNew.Columns.Add("sgst", typeof(decimal));
                dtNew.Columns.Add("igst", typeof(decimal));
                dtNew.Columns.Add("rewardpoint", typeof(Int64));
                dtNew.Columns.Add("detail", typeof(string));
                dtNew.Columns.Add("image", typeof(string));
                dtNew.Columns.Add("categoryname", typeof(string));
                dtNew.Columns.Add("isfavourite", typeof(Boolean));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["id"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    int productID = Convert.ToInt32(dtprodRecord.Rows[i]["Id"].ToString().Trim());
                    dr["categoryid"] = dtprodRecord.Rows[i]["CategoryId"].ToString().Trim();
                    dr["product"] = dtprodRecord.Rows[i]["ProductName"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Code"].ToString()))
                        dr["code"] = dtprodRecord.Rows[i]["Code"].ToString().Trim();
                    else
                        dr["code"] = "";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Price"].ToString()))
                        dr["price"] = Convert.ToDecimal(dtprodRecord.Rows[i]["Price"]);
                    else
                        dr["price"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["DiscountAmount"].ToString()))
                        dr["discountamt"] = Convert.ToDecimal(dtprodRecord.Rows[i]["DiscountAmount"]);
                    else
                        dr["discountamt"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["CGST"].ToString()))
                        dr["cgst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["CGST"]);
                    else
                        dr["cgst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["SGST"].ToString()))
                        dr["sgst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["SGST"]);
                    else
                        dr["sgst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["IGST"].ToString()))
                        dr["igst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["IGST"]);
                    else
                        dr["igst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["RewardPoint"].ToString()))
                        dr["rewardpoint"] = Convert.ToInt64(dtprodRecord.Rows[i]["RewardPoint"]);
                    else
                        dr["rewardpoint"] = "0";
                    dr["detail"] = dtprodRecord.Rows[i]["Detail"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Image"].ToString()))
                    {
                        var encoded = Uri.EscapeUriString(dtprodRecord.Rows[i]["Image"].ToString().Trim());
                        dr["image"] = Helper.PhotoFolderPath + "/image/product/" + encoded;
                    }
                    else
                        dr["image"] = "";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["CategoryName"].ToString()))
                        dr["categoryname"] = dtprodRecord.Rows[i]["CategoryName"].ToString();
                    else
                        dr["categoryname"] = "0";
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["product"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""product"":" + dict["product"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["product"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""product"":" + dict["product"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }

        #region Sectorwise Search Product
        //Add Is favorite in Search 
        [Route("api/ProductApi/SearchCategoryProductListNew/{SectorId?}/{Search?}/{customerId?}")]
        [HttpGet]
        public HttpResponseMessage SearchCategoryProductListNew(string SectorId, string Search, string customerId) //JsonResult
        {
            Product obj = new Product();
            Customer _customer = new Customer();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            // dtprodRecord = obj.SearchCategProuct(Search); 
            dtprodRecord = obj.SearchSectorCategProuct(SectorId, Search);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("categoryid", typeof(Int32));
                dtNew.Columns.Add("product", typeof(string));
                dtNew.Columns.Add("code", typeof(string));
                dtNew.Columns.Add("price", typeof(decimal));
                dtNew.Columns.Add("discountamt", typeof(decimal));
                dtNew.Columns.Add("cgst", typeof(decimal));
                dtNew.Columns.Add("sgst", typeof(decimal));
                dtNew.Columns.Add("igst", typeof(decimal));
                dtNew.Columns.Add("rewardpoint", typeof(Int64));
                dtNew.Columns.Add("detail", typeof(string));
                dtNew.Columns.Add("image", typeof(string));
                dtNew.Columns.Add("categoryname", typeof(string));
                dtNew.Columns.Add("isfavourite", typeof(Boolean));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["id"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    int productID = Convert.ToInt32(dtprodRecord.Rows[i]["Id"].ToString().Trim());
                    dr["categoryid"] = dtprodRecord.Rows[i]["CategoryId"].ToString().Trim();
                    dr["product"] = dtprodRecord.Rows[i]["ProductName"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Code"].ToString()))
                        dr["code"] = dtprodRecord.Rows[i]["Code"].ToString().Trim();
                    else
                        dr["code"] = "";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Price"].ToString()))
                        dr["price"] = Convert.ToDecimal(dtprodRecord.Rows[i]["Price"]);
                    else
                        dr["price"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["DiscountAmount"].ToString()))
                        dr["discountamt"] = Convert.ToDecimal(dtprodRecord.Rows[i]["DiscountAmount"]);
                    else
                        dr["discountamt"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["CGST"].ToString()))
                        dr["cgst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["CGST"]);
                    else
                        dr["cgst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["SGST"].ToString()))
                        dr["sgst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["SGST"]);
                    else
                        dr["sgst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["IGST"].ToString()))
                        dr["igst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["IGST"]);
                    else
                        dr["igst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["RewardPoint"].ToString()))
                        dr["rewardpoint"] = Convert.ToInt64(dtprodRecord.Rows[i]["RewardPoint"]);
                    else
                        dr["rewardpoint"] = "0";
                    dr["detail"] = dtprodRecord.Rows[i]["Detail"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Image"].ToString()))
                    {
                        var encoded = Uri.EscapeUriString(dtprodRecord.Rows[i]["Image"].ToString().Trim());
                        dr["image"] = Helper.PhotoFolderPath + "/image/product/" + encoded;
                    }
                    else
                        dr["image"] = "";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["CategoryName"].ToString()))
                        dr["categoryname"] = dtprodRecord.Rows[i]["CategoryName"].ToString();
                    else
                        dr["categoryname"] = "0";

                    var favourite = _customer.CheckCustomerFavourite(Convert.ToInt32(customerId), productID);
                    if (favourite.Rows.Count > 0)
                        dr["isfavourite"] = true;
                    else
                        dr["isfavourite"] = false;
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["product"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""product"":" + dict["product"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["product"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""product"":" + dict["product"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }
        #endregion

        [Route("api/ProductApi/BuildingList")]
        [HttpGet]
        public HttpResponseMessage BuildingList() //JsonResult
        {
            Sector obj = new Sector();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.getBuildingList(null);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("buildingname", typeof(string));
                dtNew.Columns.Add("blockno", typeof(string));
                //  dtNew.Columns.Add("flatno", typeof(string));
                dtNew.Columns.Add("sectorname", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["id"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    dr["buildingname"] = dtprodRecord.Rows[i]["BuildingName"].ToString().Trim();
                    dr["blockno"] = dtprodRecord.Rows[i]["BlockNo"].ToString().Trim();
                    // dr["flatno"] = dtprodRecord.Rows[i]["FlatNo"].ToString().Trim();
                    dr["sectorname"] = dtprodRecord.Rows[i]["SectorName"].ToString().Trim();
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["building"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""building"":" + dict["building"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["building"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""building"":" + dict["building"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }



        }

        [Route("api/ProductApi/SectorList")]
        [HttpGet]
        public HttpResponseMessage SectorList() //JsonResult
        {
            Sector obj = new Sector();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.getSectorList(null);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("sectorname", typeof(string));
                dtNew.Columns.Add("landmark", typeof(string));
                dtNew.Columns.Add("pincode", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["id"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    dr["sectorname"] = dtprodRecord.Rows[i]["SectorName"].ToString().Trim();
                    dr["landmark"] = dtprodRecord.Rows[i]["LandMark"].ToString().Trim();
                    dr["pincode"] = dtprodRecord.Rows[i]["PinCode"].ToString().Trim();
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["sector"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""sector"":" + dict["sector"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["sector"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""sector"":" + dict["sector"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;

            }


        }

        [Route("api/ProductApi/SectorwiseBuildingList/{SectroId?}")]
        [HttpGet]
        public HttpResponseMessage SectorwiseBuildingList(Int32? SectroId) //JsonResult
        {
            Sector obj = new Sector();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.geSectorwisetBuildingList(SectroId);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("buildingname", typeof(string));
                dtNew.Columns.Add("blockno", typeof(string));
                //  dtNew.Columns.Add("flatno", typeof(string));
                dtNew.Columns.Add("sectorname", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["id"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    dr["buildingname"] = dtprodRecord.Rows[i]["BuildingName"].ToString().Trim();
                    dr["blockno"] = dtprodRecord.Rows[i]["BlockNo"].ToString().Trim();
                    //    dr["flatno"] = dtprodRecord.Rows[i]["FlatNo"].ToString().Trim();
                    dr["sectorname"] = dtprodRecord.Rows[i]["SectorName"].ToString().Trim();
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["building"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""building"":" + dict["building"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["building"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""building"":" + dict["building"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;

            }



        }

        [Route("api/ProductApi/BuildingwiseFlatNoList/{BuildingId?}")]
        [HttpGet]
        public HttpResponseMessage BuildingwiseFlatNoList(Int32? BuildingId) //JsonResult
        {
            Sector obj = new Sector();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.getBuildingwiseFlatNoList(BuildingId);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("buildingname", typeof(string));
                dtNew.Columns.Add("blockno", typeof(string));
                dtNew.Columns.Add("flatno", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["id"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    dr["buildingname"] = dtprodRecord.Rows[i]["BuildingName"].ToString().Trim();
                    dr["blockno"] = dtprodRecord.Rows[i]["BlockNo"].ToString().Trim();
                    dr["flatno"] = dtprodRecord.Rows[i]["FlatNo"].ToString().Trim();
                    dtNew.Rows.Add(dr);
                }

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["flatno"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""flatno"":" + dict["flatno"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["flatno"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""flatno"":" + dict["flatno"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }

        [Route("api/ProductApi/FlatNoList")]
        [HttpGet]
        public HttpResponseMessage FlatNoList() //JsonResult
        {
            Sector obj = new Sector();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.getFlatNoList(null);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("buildingname", typeof(string));
                dtNew.Columns.Add("blockno", typeof(string));
                dtNew.Columns.Add("flatno", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["id"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    dr["buildingname"] = dtprodRecord.Rows[i]["BuildingName"].ToString().Trim();
                    dr["blockno"] = dtprodRecord.Rows[i]["BlockNo"].ToString().Trim();
                    dr["flatno"] = dtprodRecord.Rows[i]["FlatNo"].ToString().Trim();
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["flatno"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""flatno"":" + dict["flatno"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["flatno"] = jsonString;


                string one = @"{""Fail"":""success""";
                string two = @",""flatno"":" + dict["flatno"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }



        }

        #region Sector vendor Wise Product and category List
        [Route("api/ProductApi/SectorProductCategoryList/{SectorId?}")]
        [HttpGet]
        public HttpResponseMessage SectorProductCategoryList(Int32? SectorId) //JsonResult
        {
            Product obj = new Product();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.BindSectorCategory(SectorId);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("categoryname", typeof(string));
                dtNew.Columns.Add("image", typeof(string));

                dtNew.Columns.Add("MinAmount", typeof(string));
                dtNew.Columns.Add("MaxAmount", typeof(string));

                dtNew.Columns.Add("FromTime", typeof(string));
                dtNew.Columns.Add("ToTime", typeof(string));

                dtNew.Columns.Add("DeliveryFrom", typeof(string));
                dtNew.Columns.Add("DeliveryTo", typeof(string));
                dtNew.Columns.Add("IsActive", typeof(string));
                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Id"].ToString()))
                    {
                        var categoryId = Convert.ToInt32(dtprodRecord.Rows[i]["Id"].ToString());
                        var Maindt = obj.CheckMainCategorySector(categoryId, SectorId);
                        var Subdt = obj.CheckSubCategorySector(categoryId, SectorId);
                        if (Maindt.Rows.Count > 0 || Subdt.Rows.Count > 0)
                        {
                            DataRow dr = dtNew.NewRow();
                            dr["id"] = categoryId;
                            dr["categoryname"] = dtprodRecord.Rows[i]["CategoryName"].ToString().Trim();
                            if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Image"].ToString()))
                            {
                                var encoded = Uri.EscapeUriString(dtprodRecord.Rows[i]["Image"].ToString().Trim());
                                dr["image"] = Helper.PhotoFolderPath + "/image/productcategory/" + encoded;
                            }
                            else
                                dr["image"] = "";

                            dr["MinAmount"] = dtprodRecord.Rows[i]["MinAmount"].ToString().Trim();
                            dr["MaxAmount"] = dtprodRecord.Rows[i]["MaxAmount"].ToString().Trim();

                            dr["FromTime"] = dtprodRecord.Rows[i]["FromTime"].ToString().Trim();
                            dr["ToTime"] = dtprodRecord.Rows[i]["ToTime"].ToString().Trim();

                            dr["DeliveryFrom"] = dtprodRecord.Rows[i]["DeliveryFrom"].ToString().Trim();
                            dr["DeliveryTo"] = dtprodRecord.Rows[i]["DeliveryTo"].ToString().Trim();
                            dr["IsActive"] = dtprodRecord.Rows[i]["IsActive"].ToString().Trim();
                            dtNew.Rows.Add(dr);
                        }
                    }
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["categories"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""categories"":" + dict["categories"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["categories"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""categories"":" + dict["categories"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }
        #endregion

        #region Sub Category List
        [Route("api/ProductApi/SubCategoryList/{CategoryId?}")]
        [HttpGet]
        public HttpResponseMessage SubCategoryList(Int32? CategoryId) //JsonResult
        {
            Product obj = new Product();
            DataTable dtNew = new DataTable();
            CategoryResponse response = new CategoryResponse();
            response.status = "Failed";
            try
            {
                var categoryList = obj.GetSubMaincategory(CategoryId);
                var rowCount = categoryList.Rows.Count;
                if (rowCount > 0)
                {
                    List<CategoryModel> list = new List<CategoryModel>();
                    for (int i = 0; i < categoryList.Rows.Count; i++)
                    {
                        CategoryModel category = new CategoryModel();
                        category.id = Convert.ToInt32(categoryList.Rows[i]["Id"].ToString().Trim());
                        category.categoryname = categoryList.Rows[i]["CategoryName"].ToString().Trim();

                        if (!string.IsNullOrEmpty(categoryList.Rows[i]["Image"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(categoryList.Rows[i]["Image"].ToString().Trim());
                            category.image = Helper.PhotoFolderPath + "/image/productcategory/" + encoded;
                        }
                        else
                            category.image = "";
                        list.Add(category);
                    }
                    response.status = "Success";
                    response.error_msg = "Get All Sub Category...";
                    response.categories = list;
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.msg = "No Record Found...";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch (Exception ex)
            {
                response.error_msg = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }
        #endregion

        [Route("api/ProductApi/SectorProductList/{SectorId?}")]
        [HttpGet]
        public HttpResponseMessage SectorProductList(Int32? SectorId) //JsonResult
        {
            Product obj = new Product();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.BindSectorProuct(SectorId);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("categoryid", typeof(Int32));
                dtNew.Columns.Add("product", typeof(string));
                dtNew.Columns.Add("code", typeof(string));
                dtNew.Columns.Add("price", typeof(decimal));
                dtNew.Columns.Add("discountamt", typeof(decimal));
                dtNew.Columns.Add("cgst", typeof(decimal));
                dtNew.Columns.Add("sgst", typeof(decimal));
                dtNew.Columns.Add("igst", typeof(decimal));
                dtNew.Columns.Add("rewardpoint", typeof(Int64));
                dtNew.Columns.Add("detail", typeof(string));
                dtNew.Columns.Add("image", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["id"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    dr["categoryid"] = dtprodRecord.Rows[i]["CategoryId"].ToString().Trim();
                    dr["product"] = dtprodRecord.Rows[i]["ProductName"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Code"].ToString()))
                        dr["code"] = dtprodRecord.Rows[i]["Code"].ToString().Trim();
                    else
                        dr["code"] = "";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Price"].ToString()))
                        dr["price"] = Convert.ToDecimal(dtprodRecord.Rows[i]["Price"]);
                    else
                        dr["price"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["DiscountAmount"].ToString()))
                        dr["discountamt"] = Convert.ToDecimal(dtprodRecord.Rows[i]["DiscountAmount"]);
                    else
                        dr["discountamt"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["CGST"].ToString()))
                        dr["cgst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["CGST"]);
                    else
                        dr["cgst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["SGST"].ToString()))
                        dr["sgst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["SGST"]);
                    else
                        dr["sgst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["IGST"].ToString()))
                        dr["igst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["IGST"]);
                    else
                        dr["igst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["RewardPoint"].ToString()))
                        dr["rewardpoint"] = Convert.ToInt64(dtprodRecord.Rows[i]["RewardPoint"]);
                    else
                        dr["rewardpoint"] = "0";
                    dr["detail"] = dtprodRecord.Rows[i]["Detail"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Image"].ToString()))
                    {
                        var encoded = Uri.EscapeUriString(dtprodRecord.Rows[i]["Image"].ToString().Trim());
                        dr["image"] = Helper.PhotoFolderPath + "/image/product/" + encoded;
                    }
                    else
                        dr["image"] = "";
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["products"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""products"":" + dict["products"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }

            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["products"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""products"":" + dict["products"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }

        #region Get Sector Category Product
        [Route("api/ProductApi/SectorCategoryWiseProductList/{SectorId?}/{CategoryId?}")]
        [HttpGet]
        public HttpResponseMessage SectorCategoryWiseProductList(Int32? SectorId, Int32? CategoryId) //JsonResult
        {
            Product obj = new Product();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.BindSectorCategProuct(SectorId, CategoryId);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("categoryid", typeof(Int32));
                dtNew.Columns.Add("product", typeof(string));
                dtNew.Columns.Add("code", typeof(string));
                dtNew.Columns.Add("price", typeof(decimal));
                dtNew.Columns.Add("discountamt", typeof(decimal));
                dtNew.Columns.Add("cgst", typeof(decimal));
                dtNew.Columns.Add("sgst", typeof(decimal));
                dtNew.Columns.Add("igst", typeof(decimal));
                dtNew.Columns.Add("rewardpoint", typeof(Int64));
                dtNew.Columns.Add("detail", typeof(string));
                dtNew.Columns.Add("image", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["id"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    dr["categoryid"] = dtprodRecord.Rows[i]["CategoryId"].ToString().Trim();
                    dr["product"] = dtprodRecord.Rows[i]["ProductName"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Code"].ToString()))
                        dr["code"] = dtprodRecord.Rows[i]["Code"].ToString().Trim();
                    else
                        dr["code"] = "";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Price"].ToString()))
                        dr["price"] = Convert.ToDecimal(dtprodRecord.Rows[i]["Price"]);
                    else
                        dr["price"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["DiscountAmount"].ToString()))
                        dr["discountamt"] = Convert.ToDecimal(dtprodRecord.Rows[i]["DiscountAmount"]);
                    else
                        dr["discountamt"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["CGST"].ToString()))
                        dr["cgst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["CGST"]);
                    else
                        dr["cgst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["SGST"].ToString()))
                        dr["sgst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["SGST"]);
                    else
                        dr["sgst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["IGST"].ToString()))
                        dr["igst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["IGST"]);
                    else
                        dr["igst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["RewardPoint"].ToString()))
                        dr["rewardpoint"] = Convert.ToInt64(dtprodRecord.Rows[i]["RewardPoint"]);
                    else
                        dr["rewardpoint"] = "0";
                    dr["detail"] = dtprodRecord.Rows[i]["Detail"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Image"].ToString()))
                    {
                        var encoded = Uri.EscapeUriString(dtprodRecord.Rows[i]["Image"].ToString().Trim());
                        dr["image"] = Helper.PhotoFolderPath + "/image/product/" + encoded;
                    }
                    else
                        dr["image"] = "";

                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["products"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""products"":" + dict["products"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["products"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""products"":" + dict["products"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }



        }
        #endregion

        #region Get Sector Category Product Page No
        [Route("api/ProductApi/SectorCategoryWiseProductListNew/{SectorId?}/{CategoryId?}/{CustomerId?}/{psize?}/{pno?}")]
        [HttpGet]
        public HttpResponseMessage SectorCategoryWiseProductListNew(string SectorId, string CategoryId, int psize, int pno, string CustomerId) //JsonResult
        {
            Product obj = new Product();
            Customer _customer = new Customer();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.BindSectorCategProuctNew(SectorId, CategoryId, psize, pno);
            int userRecords = dtprodRecord.Rows.Count;
            if (string.IsNullOrEmpty(CustomerId))
                CustomerId = "0";
            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("categoryid", typeof(Int32));
                dtNew.Columns.Add("productid", typeof(Int32));
                dtNew.Columns.Add("producttotal", typeof(Int32));
                dtNew.Columns.Add("product", typeof(string));
                dtNew.Columns.Add("code", typeof(string));
                dtNew.Columns.Add("price", typeof(decimal));
                dtNew.Columns.Add("discountamt", typeof(decimal));
                dtNew.Columns.Add("cgst", typeof(decimal));
                dtNew.Columns.Add("sgst", typeof(decimal));
                dtNew.Columns.Add("igst", typeof(decimal));
                //dtNew.Columns.Add("rewardpoint", typeof(Int64));
                dtNew.Columns.Add("detail", typeof(string));
                dtNew.Columns.Add("image", typeof(string));
                dtNew.Columns.Add("isdaily", typeof(Boolean));
                dtNew.Columns.Add("isfavourite", typeof(Boolean));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    try
                    {
                        //Status = "0";
                        DataRow dr = dtNew.NewRow();
                        // dr["status"] = "Success";
                        dr["id"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                        dr["productid"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                        int productID = Convert.ToInt32(dtprodRecord.Rows[i]["Id"].ToString().Trim());
                        dr["categoryid"] = dtprodRecord.Rows[i]["CategoryId"].ToString().Trim();
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["TotalRecords"].ToString()))
                            dr["producttotal"] = dtprodRecord.Rows[i]["TotalRecords"].ToString().Trim();
                        else
                            dr["producttotal"] = "0";
                        dr["product"] = dtprodRecord.Rows[i]["ProductName"].ToString().Trim();
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Code"].ToString()))
                            dr["code"] = dtprodRecord.Rows[i]["Code"].ToString().Trim();
                        else
                            dr["code"] = "";
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Price"].ToString()))
                            dr["price"] = Convert.ToDecimal(dtprodRecord.Rows[i]["Price"]);
                        else
                            dr["price"] = "0";
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["DiscountAmount"].ToString()))
                            dr["discountamt"] = Convert.ToDecimal(dtprodRecord.Rows[i]["DiscountAmount"]);
                        else
                            dr["discountamt"] = "0";
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["CGST"].ToString()))
                            dr["cgst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["CGST"]);
                        else
                            dr["cgst"] = "0";
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["SGST"].ToString()))
                            dr["sgst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["SGST"]);
                        else
                            dr["sgst"] = "0";
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["IGST"].ToString()))
                            dr["igst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["IGST"]);
                        else
                            dr["igst"] = "0";
                        //if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["RewardPoint"].ToString()))
                        //    dr["rewardpoint"] = Convert.ToInt64(dtprodRecord.Rows[i]["RewardPoint"]);
                        //else
                        //    dr["rewardpoint"] = "0";
                        dr["detail"] = dtprodRecord.Rows[i]["Detail"].ToString().Trim();
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Image"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtprodRecord.Rows[i]["Image"].ToString().Trim());
                            dr["image"] = Helper.PhotoFolderPath + "/image/product/" + encoded;
                        }
                        else
                            dr["image"] = "";
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["IsDaily"].ToString()))
                            dr["isdaily"] = Convert.ToBoolean(dtprodRecord.Rows[i]["IsDaily"]);
                        else
                            dr["isdaily"] = false;
                        var favourite = _customer.CheckCustomerFavourite(Convert.ToInt32(CustomerId), productID);
                        if (favourite.Rows.Count > 0)
                            dr["isfavourite"] = true;
                        else
                            dr["isfavourite"] = false;

                        dtNew.Rows.Add(dr);
                    }
                    catch { }
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["products"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""products"":" + dict["products"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["products"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""products"":" + dict["products"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }
        #endregion

        #region Get Product Details
        [Route("api/ProductApi/getproductdetail/{ProductId?}"), HttpGet]
        public HttpResponseMessage getproductdetail(int ProductId)
        {
            Product obj = new Product();
            ProductResponse response = new ProductResponse();
            response.status = "Failed";
            try
            {
                var favourite = obj.BindProuct(ProductId);
                var rowCount = favourite.Rows.Count;
                if (rowCount > 0)
                {
                    ProductViewModel product = new ProductViewModel();
                    product.id = Convert.ToInt32(favourite.Rows[0]["Id"].ToString().Trim());
                    product.productid = product.id;
                    product.producttotal = 0;
                    product.categoryid = Convert.ToInt32(favourite.Rows[0]["CategoryId"].ToString().Trim());
                    product.product = favourite.Rows[0]["ProductName"].ToString().Trim();
                    product.categoryname = favourite.Rows[0]["CategoryName"].ToString().Trim();

                    if (!string.IsNullOrEmpty(favourite.Rows[0]["Code"].ToString()))
                        product.code = favourite.Rows[0]["Code"].ToString().Trim();
                    else
                        product.code = "";
                    if (!string.IsNullOrEmpty(favourite.Rows[0]["SalePrice"].ToString()))
                        product.price = Convert.ToDecimal(favourite.Rows[0]["SalePrice"]);
                    else
                        product.price = 0;
                    if (!string.IsNullOrEmpty(favourite.Rows[0]["Price"].ToString()))
                        product.mrp = Convert.ToDecimal(favourite.Rows[0]["Price"]);
                    else
                        product.mrp = 0;
                    if (!string.IsNullOrEmpty(favourite.Rows[0]["DiscountAmount"].ToString()))
                        product.discountamt = Convert.ToDecimal(favourite.Rows[0]["DiscountAmount"]);
                    else
                        product.discountamt = 0;
                    if (!string.IsNullOrEmpty(favourite.Rows[0]["CGST"].ToString()))
                        product.cgst = Convert.ToDecimal(favourite.Rows[0]["CGST"]);
                    else
                        product.cgst = 0;
                    if (!string.IsNullOrEmpty(favourite.Rows[0]["SGST"].ToString()))
                        product.sgst = Convert.ToDecimal(favourite.Rows[0]["SGST"]);
                    else
                        product.sgst = 0;
                    if (!string.IsNullOrEmpty(favourite.Rows[0]["IGST"].ToString()))
                        product.igst = Convert.ToDecimal(favourite.Rows[0]["IGST"]);
                    else
                        product.igst = 0;
                    if (!string.IsNullOrEmpty(favourite.Rows[0]["RewardPoint"].ToString()))
                        product.rewardpoint = Convert.ToInt64(favourite.Rows[0]["RewardPoint"]);
                    else
                        product.rewardpoint = 0;
                    product.detail = favourite.Rows[0]["Detail"].ToString().Trim();
                    if (!string.IsNullOrEmpty(favourite.Rows[0]["Image"].ToString()))
                    {
                        var encoded = Uri.EscapeUriString(favourite.Rows[0]["Image"].ToString().Trim());
                        product.image = Helper.PhotoFolderPath + "/image/product/" + encoded;
                    }
                    else
                        product.image = "";
                    if (!string.IsNullOrEmpty(favourite.Rows[0]["YoutubeTitle"].ToString()))
                        product.youtubetitle = favourite.Rows[0]["YoutubeTitle"].ToString();
                    else
                        product.youtubetitle = "";
                    if (!string.IsNullOrEmpty(favourite.Rows[0]["YoutubeURL"].ToString()))
                        product.youtubeurl = favourite.Rows[0]["YoutubeURL"].ToString();
                    else
                        product.youtubetitle = "";
                    if (!string.IsNullOrEmpty(favourite.Rows[0]["IsDaily"].ToString()))
                        product.isdaily = Convert.ToBoolean(favourite.Rows[0]["IsDaily"].ToString());
                    else
                        product.isdaily = false;

                    if (!string.IsNullOrEmpty(favourite.Rows[0]["IsAlternate"].ToString()))
                        product.IsAlternate = Convert.ToBoolean(favourite.Rows[0]["IsAlternate"].ToString());
                    else
                        product.IsAlternate = false;


                    if (!string.IsNullOrEmpty(favourite.Rows[0]["IsMultiple"].ToString()))
                        product.IsMultipleDay = Convert.ToBoolean(favourite.Rows[0]["IsMultiple"].ToString());
                    else
                        product.IsMultipleDay = false;


                    if (!string.IsNullOrEmpty(favourite.Rows[0]["IsWeeklyday"].ToString()))
                        product.IsWeeklyDay = Convert.ToBoolean(favourite.Rows[0]["IsWeeklyday"].ToString());
                    else
                        product.IsWeeklyDay = false;

                    product.OrderTime = dHelper.GetProductOrderTime(ProductId);
                    product.DeliveryTime = dHelper.GetProductDeliveryTime(ProductId);
                    product.minAmount = obj.GetProductOrderAmount(ProductId);
                    var photos = obj.BindProuctPhotos(ProductId);
                    var photoRow = photos.Rows.Count;
                    if (photoRow > 0)
                    {
                        List<ProductPhotoModel> lstPhoto = new List<ProductPhotoModel>();
                        for (int i = 0; i < photoRow; i++)
                        {
                            ProductPhotoModel p = new ProductPhotoModel();
                            if (!string.IsNullOrEmpty(photos.Rows[i]["Image"].ToString()))
                                p.photopath = Helper.PhotoFolderPath + "/image/product/" + photos.Rows[i]["Image"].ToString();
                            lstPhoto.Add(p);
                        }
                        product.photos = lstPhoto;
                    }

                    response.status = "Success";
                    response.msg = "Get Product Details...";
                    response.products = product;
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.msg = "No Record Found...";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch (Exception ex)
            {
                response.error_msg = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }



        //New Apis

        [Route("api/ProductApi/ProductParentCategoryList")]
        [HttpGet]
        public HttpResponseMessage ProductParentCategoryList() //JsonResult
        {
            Product obj = new Product();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.BindParentCategory(null);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("categoryname", typeof(string));
                dtNew.Columns.Add("Isactive", typeof(string));
                dtNew.Columns.Add("image", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["id"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    dr["categoryname"] = dtprodRecord.Rows[i]["ParentCategory"].ToString().Trim();
                    dr["Isactive"] = dtprodRecord.Rows[i]["IsActive"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Image"].ToString()))
                    {
                        var encoded = Uri.EscapeUriString(dtprodRecord.Rows[i]["Image"].ToString().Trim());
                        dr["image"] = Helper.PhotoFolderPath + "/image/productcategory/" + encoded;
                    }
                    else
                        dr["image"] = "";
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["categories"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""categories"":" + dict["categories"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["categories"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""categories"":" + dict["categories"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }




        [Route("api/ProductApi/VendorCategoryList/{SectorId}/{ParentCatId}")]
        [HttpGet]
        public HttpResponseMessage VendorCategoryList(int? SectorId, int? ParentCatId) //JsonResult
        {
            Product obj = new Product();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.BindVendorCategory(SectorId, ParentCatId);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {

                dtNew.Columns.Add("VendorId", typeof(string));
                dtNew.Columns.Add("VendorName", typeof(string));
                dtNew.Columns.Add("VendorCatId", typeof(string));
                dtNew.Columns.Add("VendorCatName", typeof(string));
                dtNew.Columns.Add("CatId", typeof(string));
                dtNew.Columns.Add("categoryname", typeof(string));
                dtNew.Columns.Add("SectorId", typeof(string));
                dtNew.Columns.Add("SectorName", typeof(string));
                dtNew.Columns.Add("Isactive", typeof(string));

             
                
                dtNew.Columns.Add("image", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";

                    dr["VendorId"] = dtprodRecord.Rows[i]["VendorId"].ToString().Trim();
                    dr["VendorName"] = dtprodRecord.Rows[i]["Name"].ToString().Trim();
                    dr["VendorCatId"] = dtprodRecord.Rows[i]["VendorCatId"].ToString().Trim();
                    dr["VendorCatName"] = dtprodRecord.Rows[i]["VendorCatname"].ToString().Trim();
                    dr["CatId"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    dr["categoryname"] = dtprodRecord.Rows[i]["ParentCategory"].ToString().Trim();
                    dr["SectorId"] = dtprodRecord.Rows[i]["SectorId"].ToString().Trim();
                    dr["SectorName"] = dtprodRecord.Rows[i]["SectorName"].ToString().Trim();

                    dr["Isactive"] = dtprodRecord.Rows[i]["IsActive"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Catimg"].ToString()))
                    {
                        var encoded = Uri.EscapeUriString(dtprodRecord.Rows[i]["Catimg"].ToString().Trim());
                        dr["image"] = Helper.PhotoFolderPath + "/image/VendorCatImage/" + encoded;
                    }
                    else
                        dr["image"] = "";
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["categories"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""Vendorcategories"":" + dict["categories"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["categories"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""Vendorcategories"":" + dict["categories"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }




        [Route("api/ProductApi/VendorSubCategoryList/{SectorId}/{ParentCatId}/{VendorId}/{VendorCatId}")]
        [HttpGet]
        public HttpResponseMessage VendorSubCategoryList(int? SectorId, int? ParentCatId, int? VendorId, int? VendorCatId) //JsonResult
        {
            Product obj = new Product();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.BindVendorSubCategory(SectorId, ParentCatId, VendorId, VendorCatId);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {

                dtNew.Columns.Add("VendorId", typeof(string));
                dtNew.Columns.Add("VendorName", typeof(string));
                dtNew.Columns.Add("VendorCatId", typeof(string));
                dtNew.Columns.Add("VendorCatName", typeof(string));

                dtNew.Columns.Add("MinAmount", typeof(string));
                dtNew.Columns.Add("MaxAmount", typeof(string));
                dtNew.Columns.Add("FromTime", typeof(string));
                dtNew.Columns.Add("ToTime", typeof(string));
                dtNew.Columns.Add("DeliveryFrom", typeof(string));
                dtNew.Columns.Add("DeliveryTo", typeof(string));
                dtNew.Columns.Add("CatId", typeof(string));
                dtNew.Columns.Add("categoryname", typeof(string));
                dtNew.Columns.Add("SubCatId", typeof(string));
                dtNew.Columns.Add("SubcatName", typeof(string));
                dtNew.Columns.Add("SectorId", typeof(string));
                dtNew.Columns.Add("SectorName", typeof(string));
                dtNew.Columns.Add("Isactive", typeof(string));
                dtNew.Columns.Add("image", typeof(string));
                dtNew.Columns.Add("Subcatimage", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";

                    dr["VendorId"] = dtprodRecord.Rows[i]["VendorId"].ToString().Trim();
                    dr["VendorName"] = dtprodRecord.Rows[i]["Name"].ToString().Trim();
                    dr["VendorCatId"] = dtprodRecord.Rows[i]["VendorCatId"].ToString().Trim();
                    dr["VendorCatName"] = dtprodRecord.Rows[i]["VendorCatname"].ToString().Trim();


                    dr["MinAmount"] = dtprodRecord.Rows[i]["MinAmount"].ToString().Trim();
                    dr["MaxAmount"] = dtprodRecord.Rows[i]["MaxAmount"].ToString().Trim();
                    dr["FromTime"] = dtprodRecord.Rows[i]["FromTime"].ToString().Trim();
                    dr["ToTime"] = dtprodRecord.Rows[i]["ToTime"].ToString().Trim();
                    dr["DeliveryFrom"] = dtprodRecord.Rows[i]["DeliveryFrom"].ToString().Trim();
                    dr["DeliveryTo"] = dtprodRecord.Rows[i]["DeliveryTo"].ToString().Trim();

                    dr["CatId"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    dr["categoryname"] = dtprodRecord.Rows[i]["ParentCategory"].ToString().Trim();
                    dr["SubCatId"] = dtprodRecord.Rows[i]["subcatId"].ToString().Trim();
                    dr["SubcatName"] = dtprodRecord.Rows[i]["SubCatName"].ToString().Trim();
                    dr["SectorId"] = dtprodRecord.Rows[i]["SectorId"].ToString().Trim();
                    dr["SectorName"] = dtprodRecord.Rows[i]["SectorName"].ToString().Trim();

                    dr["Isactive"] = dtprodRecord.Rows[i]["IsActive"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Catimg"].ToString()))
                    {
                        var encoded = Uri.EscapeUriString(dtprodRecord.Rows[i]["Catimg"].ToString().Trim());
                        dr["image"] = Helper.PhotoFolderPath + "/image/VendorCatImage/" + encoded;
                    }
                    else
                        dr["image"] = "";

                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["SubcatImage"].ToString()))
                    {
                        var encoded = Uri.EscapeUriString(dtprodRecord.Rows[i]["SubcatImage"].ToString().Trim());
                        dr["Subcatimage"] = Helper.PhotoFolderPath + "/image/subcategory/" + encoded;
                    }
                    else
                        dr["Subcatimage"] = "";
                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["categories"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""categories"":" + dict["categories"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["categories"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""categories"":" + dict["categories"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }





        
        [Route("api/ProductApi/SectorVendorCategoryWiseProductListNew/{SectorId?}/{ParentCat?}/{Subcat}/{VendorId?}/{VendorCatId?}/{psize?}/{pno?}/{CustomerId?}/{CusType?}")]
        [HttpGet]
        public HttpResponseMessage SectorVendorCategoryWiseProductListNew(string SectorId, string ParentCat,string Subcat,string VendorId,string VendorCatId, int psize, int pno,string CustomerId,string CusType) //JsonResult
        {
            Product obj = new Product();
            Customer _customer = new Customer();
            DataTable dtNew = new DataTable();
            DataTable dtCart = new DataTable();
            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.BindSectorVendorCategProuctNew(SectorId, ParentCat, Subcat,  VendorId,  VendorCatId, psize, pno);
            int userRecords = dtprodRecord.Rows.Count;
            //if (string.IsNullOrEmpty(CustomerId))
            //    CustomerId = "0";
            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("categoryid", typeof(Int32));
                dtNew.Columns.Add("subcategoryid", typeof(Int32));
                dtNew.Columns.Add("productid", typeof(Int32));
                dtNew.Columns.Add("producttotal", typeof(Int32));
                dtNew.Columns.Add("product", typeof(string));
                dtNew.Columns.Add("code", typeof(string));
                dtNew.Columns.Add("Mrpprice", typeof(decimal));
                dtNew.Columns.Add("discountamt", typeof(decimal));
                dtNew.Columns.Add("cgst", typeof(decimal));
                dtNew.Columns.Add("sgst", typeof(decimal));
                dtNew.Columns.Add("igst", typeof(decimal));

                dtNew.Columns.Add("PurchasePrice", typeof(decimal));
                dtNew.Columns.Add("SellPrice", typeof(decimal));
                dtNew.Columns.Add("AttributeId", typeof(string));

                dtNew.Columns.Add("VendorId", typeof(string));
                dtNew.Columns.Add("VendorCatId", typeof(string));

                dtNew.Columns.Add("rewardpoint", typeof(Int64));
                dtNew.Columns.Add("detail", typeof(string));
                dtNew.Columns.Add("image", typeof(string));
                dtNew.Columns.Add("isdaily", typeof(Boolean));
                 dtNew.Columns.Add("isfavourite", typeof(Boolean));


                //cart
                dtNew.Columns.Add("Cart Id", typeof(Int32));
                dtNew.Columns.Add("Cart Qty", typeof(Int32));
                dtNew.Columns.Add("Cart Amount", typeof(decimal));
                dtNew.Columns.Add("Cart Date", typeof(string));
                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //try
                    //{
                        //Status = "0";
                        DataRow dr = dtNew.NewRow();
                        // dr["status"] = "Success";
                        dr["id"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                        dr["productid"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                        int productID = Convert.ToInt32(dtprodRecord.Rows[i]["Id"].ToString().Trim());
                        dr["categoryid"] = dtprodRecord.Rows[i]["CategoryId"].ToString().Trim();
                        dr["subcategoryid"] = dtprodRecord.Rows[i]["SubcatId"].ToString().Trim();
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["TotalRecords"].ToString()))
                            dr["producttotal"] = dtprodRecord.Rows[i]["TotalRecords"].ToString().Trim();
                        else
                            dr["producttotal"] = "0";
                        dr["product"] = dtprodRecord.Rows[i]["ProductName"].ToString().Trim();
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Code"].ToString()))
                            dr["code"] = dtprodRecord.Rows[i]["Code"].ToString().Trim();
                        else
                            dr["code"] = "";


                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["MRPPrice"].ToString()))
                            dr["Mrpprice"] = Convert.ToDecimal(dtprodRecord.Rows[i]["MRPPrice"]);
                        else
                            dr["Mrpprice"] = "0";

                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["PurchasePrice"].ToString()))
                            dr["PurchasePrice"] = Convert.ToDecimal(dtprodRecord.Rows[i]["PurchasePrice"]);
                        else
                            dr["PurchasePrice"] = "0";

                        if (string.IsNullOrEmpty(CusType) || CusType == "General")
                        {
                            if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["SellPrice"].ToString()))
                                dr["SellPrice"] = Convert.ToDecimal(dtprodRecord.Rows[i]["SellPrice"]);
                            else
                                dr["SellPrice"] = "0";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["B2BSellPrice"].ToString()))
                                dr["SellPrice"] = Convert.ToDecimal(dtprodRecord.Rows[i]["B2BSellPrice"]);
                            else
                                dr["SellPrice"] = "0";
                        }
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["AttributeID"].ToString()))
                            dr["AttributeId"] = Convert.ToDecimal(dtprodRecord.Rows[i]["AttributeID"]);
                        else
                            dr["AttributeId"] = "0";

                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["VendorId"].ToString()))
                            dr["VendorId"] = Convert.ToDecimal(dtprodRecord.Rows[i]["VendorId"]);
                        else
                            dr["VendorId"] = "0";

                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["VendorCatId"].ToString()))
                            dr["VendorCatId"] = Convert.ToDecimal(dtprodRecord.Rows[i]["VendorCatId"]);
                        else
                            dr["VendorCatId"] = "0";

                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["DiscountPrice"].ToString()))
                            dr["discountamt"] = Convert.ToDecimal(dtprodRecord.Rows[i]["DiscountPrice"]);
                        else
                            dr["discountamt"] = "0";
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["CGST"].ToString()))
                            dr["cgst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["CGST"]);
                        else
                            dr["cgst"] = "0";
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["SGST"].ToString()))
                            dr["sgst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["SGST"]);
                        else
                            dr["sgst"] = "0";
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["IGST"].ToString()))
                            dr["igst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["IGST"]);
                        else
                            dr["igst"] = "0";
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["RewardPoint"].ToString()))
                            dr["rewardpoint"] = Convert.ToInt64(dtprodRecord.Rows[i]["RewardPoint"]);
                        else
                            dr["rewardpoint"] = "0";
                        dr["detail"] = dtprodRecord.Rows[i]["Detail"].ToString().Trim();
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Image"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtprodRecord.Rows[i]["Image"].ToString().Trim());
                            dr["image"] = Helper.PhotoFolderPath + "/image/product/" + encoded;
                        }
                        else
                            dr["image"] = "";
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["IsDaily"].ToString()))
                            dr["isdaily"] = Convert.ToBoolean(dtprodRecord.Rows[i]["IsDaily"]);
                        else
                            dr["isdaily"] = false;

                        //Check Cart
                        dtCart = obj.getItemwiseCartdetail(Convert.ToInt32(CustomerId),Convert.ToInt32(VendorId),Convert.ToInt32(VendorCatId),productID,Convert.ToInt32(dr["AttributeId"]));
                        if(dtCart.Rows.Count>0)
                        {
                        if (!string.IsNullOrEmpty(dtCart.Rows[0]["Id"].ToString()))
                            dr["Cart Id"] = Convert.ToInt32(dtCart.Rows[0]["Id"]);
                        else
                            dr["Cart Id"] = 0;


                        if (!string.IsNullOrEmpty(dtCart.Rows[0]["Qty"].ToString()))
                                dr["Cart Qty"] = Convert.ToInt32(dtCart.Rows[0]["Qty"]);
                            else
                                dr["Cart Qty"] = 0;

                            if (!string.IsNullOrEmpty(dtCart.Rows[0]["TotalFinalAmount"].ToString()))
                                dr["Cart Amount"] = Convert.ToInt32(dtCart.Rows[0]["TotalFinalAmount"]);
                            else
                                dr["Cart Amount"] = 0;

                            if (!string.IsNullOrEmpty(dtCart.Rows[0]["CartDate"].ToString()))
                                dr["Cart Date"] = dtCart.Rows[0]["CartDate"].ToString();
                            else
                                dr["Cart Date"] = "";
                        }

                        var favourite = _customer.CheckCustomerFavourite(Convert.ToInt32(CustomerId), productID);
                        if (favourite.Rows.Count > 0)
                            dr["isfavourite"] = true;
                        else
                            dr["isfavourite"] = false;

                        dtNew.Rows.Add(dr);
                    //}
                    //catch { }
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["products"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""products"":" + dict["products"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["products"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""products"":" + dict["products"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }

        [Route("api/ProductApi/AttributeWiseProductListNew/{SectorId?}/{ProductId?}/{AttributeId}/{VendorId?}/{VendorCatId?}/{CustomerId?}/{CusType?}")]
        [HttpGet]
        public HttpResponseMessage AttributeWiseProductListNew(string SectorId, string ProductId, string AttributeId, string VendorId, string VendorCatId,string CustomerId, string CusType) //JsonResult
        {
            Product obj = new Product();
            Customer _customer = new Customer();
            DataTable dtNew = new DataTable();
            DataTable dtCart = new DataTable();
            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.BindAttributewiseProuctNew(SectorId, ProductId, AttributeId, VendorId, VendorCatId);
            int userRecords = dtprodRecord.Rows.Count;
            //if (string.IsNullOrEmpty(CustomerId))
            //    CustomerId = "0";
            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("categoryid", typeof(Int32));
                dtNew.Columns.Add("subcategoryid", typeof(Int32));
                dtNew.Columns.Add("productid", typeof(Int32));
                dtNew.Columns.Add("producttotal", typeof(Int32));
                dtNew.Columns.Add("product", typeof(string));
                dtNew.Columns.Add("code", typeof(string));
                dtNew.Columns.Add("Mrpprice", typeof(decimal));
                dtNew.Columns.Add("discountamt", typeof(decimal));
                dtNew.Columns.Add("cgst", typeof(decimal));
                dtNew.Columns.Add("sgst", typeof(decimal));
                dtNew.Columns.Add("igst", typeof(decimal));

                dtNew.Columns.Add("PurchasePrice", typeof(decimal));
                dtNew.Columns.Add("SellPrice", typeof(decimal));
                dtNew.Columns.Add("AttributeId", typeof(string));

                dtNew.Columns.Add("VendorId", typeof(string));
                dtNew.Columns.Add("VendorCatId", typeof(string));

                dtNew.Columns.Add("rewardpoint", typeof(Int64));
                dtNew.Columns.Add("detail", typeof(string));
                dtNew.Columns.Add("image", typeof(string));
                dtNew.Columns.Add("isdaily", typeof(Boolean));
                dtNew.Columns.Add("isfavourite", typeof(Boolean));


                //cart
                dtNew.Columns.Add("Cart Id", typeof(Int32));
                dtNew.Columns.Add("Cart Qty", typeof(Int32));
                dtNew.Columns.Add("Cart Amount", typeof(decimal));
                dtNew.Columns.Add("Cart Date", typeof(string));
                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //try
                    //{
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["id"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    dr["productid"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    int productID = Convert.ToInt32(dtprodRecord.Rows[i]["Id"].ToString().Trim());
                    dr["categoryid"] = dtprodRecord.Rows[i]["CategoryId"].ToString().Trim();
                    dr["subcategoryid"] = dtprodRecord.Rows[i]["SubcatId"].ToString().Trim();
                    //if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["TotalRecords"].ToString()))
                    //    dr["producttotal"] = dtprodRecord.Rows[i]["TotalRecords"].ToString().Trim();
                    //else
                    //    dr["producttotal"] = "0";
                    dr["product"] = dtprodRecord.Rows[i]["ProductName"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Code"].ToString()))
                        dr["code"] = dtprodRecord.Rows[i]["Code"].ToString().Trim();
                    else
                        dr["code"] = "";


                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["MRPPrice"].ToString()))
                        dr["Mrpprice"] = Convert.ToDecimal(dtprodRecord.Rows[i]["MRPPrice"]);
                    else
                        dr["Mrpprice"] = "0";

                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["PurchasePrice"].ToString()))
                        dr["PurchasePrice"] = Convert.ToDecimal(dtprodRecord.Rows[i]["PurchasePrice"]);
                    else
                        dr["PurchasePrice"] = "0";

                    if (string.IsNullOrEmpty(CusType) || CusType == "General")
                    {
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["SellPrice"].ToString()))
                            dr["SellPrice"] = Convert.ToDecimal(dtprodRecord.Rows[i]["SellPrice"]);
                        else
                            dr["SellPrice"] = "0";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["B2BSellPrice"].ToString()))
                            dr["SellPrice"] = Convert.ToDecimal(dtprodRecord.Rows[i]["B2BSellPrice"]);
                        else
                            dr["SellPrice"] = "0";
                    }
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["AttributeID"].ToString()))
                        dr["AttributeId"] = Convert.ToDecimal(dtprodRecord.Rows[i]["AttributeID"]);
                    else
                        dr["AttributeId"] = "0";

                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["VendorId"].ToString()))
                        dr["VendorId"] = Convert.ToDecimal(dtprodRecord.Rows[i]["VendorId"]);
                    else
                        dr["VendorId"] = "0";

                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["VendorCatId"].ToString()))
                        dr["VendorCatId"] = Convert.ToDecimal(dtprodRecord.Rows[i]["VendorCatId"]);
                    else
                        dr["VendorCatId"] = "0";

                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["DiscountPrice"].ToString()))
                        dr["discountamt"] = Convert.ToDecimal(dtprodRecord.Rows[i]["DiscountPrice"]);
                    else
                        dr["discountamt"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["CGST"].ToString()))
                        dr["cgst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["CGST"]);
                    else
                        dr["cgst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["SGST"].ToString()))
                        dr["sgst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["SGST"]);
                    else
                        dr["sgst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["IGST"].ToString()))
                        dr["igst"] = Convert.ToDecimal(dtprodRecord.Rows[i]["IGST"]);
                    else
                        dr["igst"] = "0";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["RewardPoint"].ToString()))
                        dr["rewardpoint"] = Convert.ToInt64(dtprodRecord.Rows[i]["RewardPoint"]);
                    else
                        dr["rewardpoint"] = "0";
                    dr["detail"] = dtprodRecord.Rows[i]["Detail"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["Image"].ToString()))
                    {
                        var encoded = Uri.EscapeUriString(dtprodRecord.Rows[i]["Image"].ToString().Trim());
                        dr["image"] = Helper.PhotoFolderPath + "/image/product/" + encoded;
                    }
                    else
                        dr["image"] = "";
                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[i]["IsDaily"].ToString()))
                        dr["isdaily"] = Convert.ToBoolean(dtprodRecord.Rows[i]["IsDaily"]);
                    else
                        dr["isdaily"] = false;

                    //Check Cart
                    dtCart = obj.getItemwiseCartdetail(Convert.ToInt32(CustomerId), Convert.ToInt32(VendorId), Convert.ToInt32(VendorCatId), productID, Convert.ToInt32(dr["AttributeId"]));
                    if (dtCart.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dtCart.Rows[0]["Id"].ToString()))
                            dr["Cart Id"] = Convert.ToInt32(dtCart.Rows[0]["Id"]);
                        else
                            dr["Cart Id"] = 0;


                        if (!string.IsNullOrEmpty(dtCart.Rows[0]["Qty"].ToString()))
                            dr["Cart Qty"] = Convert.ToInt32(dtCart.Rows[0]["Qty"]);
                        else
                            dr["Cart Qty"] = 0;

                        if (!string.IsNullOrEmpty(dtCart.Rows[0]["TotalFinalAmount"].ToString()))
                            dr["Cart Amount"] = Convert.ToInt32(dtCart.Rows[0]["TotalFinalAmount"]);
                        else
                            dr["Cart Amount"] = 0;

                        if (!string.IsNullOrEmpty(dtCart.Rows[0]["CartDate"].ToString()))
                            dr["Cart Date"] = dtCart.Rows[0]["CartDate"].ToString();
                        else
                            dr["Cart Date"] = "";
                    }

                    var favourite = _customer.CheckCustomerFavourite(Convert.ToInt32(CustomerId), productID);
                    if (favourite.Rows.Count > 0)
                        dr["isfavourite"] = true;
                    else
                        dr["isfavourite"] = false;

                    dtNew.Rows.Add(dr);
                    //}
                    //catch { }
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["products"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""products"":" + dict["products"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();
                dr["status"] = "Fail";
                dr["msg"] = "No Record Found";
                dtNew.Rows.Add(dr);

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";
                dict["products"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""products"":" + dict["products"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }


        #endregion
    }
}

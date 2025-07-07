using MilkWayIndia.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace MilkWayIndia.Controllers
{
    public class VendorApiController : ApiController
    {

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        [Route("api/VendorApi/Login/{username?}/{password?}")]
        [HttpGet]

        public HttpResponseMessage Login(string username, string password)
        {



            Vendornew objlogin = new Vendornew();

            DataTable dtUser = new DataTable();
            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("msg", typeof(string));
            //   dtNew.Columns.Add("error_msg", typeof(string));
            dtNew.Columns.Add("userid", typeof(Int64));
            dtNew.Columns.Add("firstname", typeof(string));
            dtNew.Columns.Add("lastname", typeof(string));
            dtNew.Columns.Add("mobileno", typeof(string));
            dtNew.Columns.Add("email", typeof(string));
            dtNew.Columns.Add("address", typeof(string));
            dtNew.Columns.Add("photo", typeof(string));


            dtNew.Columns.Add("SectorId", typeof(int));
            dtNew.Columns.Add("companyname", typeof(string));
            dtNew.Columns.Add("pancardno", typeof(string));
            dtNew.Columns.Add("gstno", typeof(string));
            dtNew.Columns.Add("isactive", typeof(int));
            dtNew.Columns.Add("VendorType", typeof(string));
            DataRow dr = dtNew.NewRow();




            if (username != null)
            {
                DataTable dtUsername = new DataTable();
                dtUsername = objlogin.CheckVendorUserName(username);

                DataTable dtpassword = new DataTable();
                dtpassword = objlogin.Vendorlogin(username, password);

                if (dtUsername.Rows.Count == 0)
                {
                    dr["status"] = "Failed";
                    dr["msg"] = "Username Not Found";
                }
                else if (dtpassword.Rows.Count == 0)
                {
                    dr["status"] = "Failed";
                    dr["msg"] = "Wrong Password";
                }

                if (dtpassword.Rows.Count > 0)
                {
                    dr["status"] = "Success";
                    dr["msg"] = "Find User";
                    int vendorId = Convert.ToInt32(dtpassword.Rows[0]["Id"]);

                    // var _deliverboy = objlogin.BindDeliverboy(deliverboyId);

                    dr["userid"] = Convert.ToInt32(dtpassword.Rows[0]["Id"]);
                    dr["firstname"] = dtpassword.Rows[0]["FirstName"].ToString().Trim();
                    dr["lastname"] = dtpassword.Rows[0]["LastName"].ToString().Trim();
                    dr["mobileno"] = dtpassword.Rows[0]["MobileNo"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dtpassword.Rows[0]["Email"].ToString()))
                        dr["email"] = dtpassword.Rows[0]["Email"].ToString().Trim();
                    else
                        dr["email"] = "";
                    if (!string.IsNullOrEmpty(dtpassword.Rows[0]["Photo"].ToString()))
                    {
                        var encoded = Uri.EscapeUriString(dtpassword.Rows[0]["Photo"].ToString().Trim());
                        dr["photo"] = Helper.PhotoFolderPath + "/image/staffphoto/" + encoded;
                    }
                    else
                        dr["photo"] = "";
                    if (!string.IsNullOrEmpty(dtpassword.Rows[0]["SectorId"].ToString()))
                        dr["SectorId"] = Convert.ToInt32(dtpassword.Rows[0]["SectorId"]);
                    else
                        dr["SectorId"] = 0;

                    if (!string.IsNullOrEmpty(dtpassword.Rows[0]["Address"].ToString()))
                        dr["address"] = dtpassword.Rows[0]["Address"];
                    else
                        dr["address"] = "";


                    if (!string.IsNullOrEmpty(dtpassword.Rows[0]["CompanyName"].ToString()))
                        dr["companyname"] = dtpassword.Rows[0]["CompanyName"];
                    else
                        dr["companyname"] = "";
                    if (!string.IsNullOrEmpty(dtpassword.Rows[0]["PanCardNo"].ToString()))
                        dr["pancardno"] = dtpassword.Rows[0]["PanCardNo"];
                    else
                        dr["pancardno"] = "";
                    if (!string.IsNullOrEmpty(dtpassword.Rows[0]["GSTNo"].ToString()))
                        dr["gstno"] = dtpassword.Rows[0]["GSTNo"];
                    else
                        dr["gstno"] = "";


                    if (!string.IsNullOrEmpty(dtpassword.Rows[0]["IsActive"].ToString()))
                        dr["isactive"] = Convert.ToInt32(dtpassword.Rows[0]["IsActive"]);
                    else
                        dr["isactive"] = 0;

                    if (!string.IsNullOrEmpty(dtpassword.Rows[0]["VendorType"].ToString()))
                        dr["VendorType"] = dtpassword.Rows[0]["VendorType"];
                    else
                        dr["VendorType"] = "";
                }

            }





            dtNew.Rows.Add(dr);
            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dtNew);
            //jsonString = jsonString.Replace(@"[", "");
            // jsonString = jsonString.Replace(@"]", "");

            // ViewBag.Status = jsonString;
            // return jsonString;
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return response;
        }

        [Route("api/VendorApi/DailyOrderVendor/{Vendorid?}")]
        [HttpGet]
        public HttpResponseMessage DailyOrderVendor(string Vendorid) //JsonResult
        {

            Vendornew obj = new Vendornew();

            obj.VendorId = Convert.ToInt32(Vendorid);



            Vendornew order = new Vendornew();
            //ViewBag.FromDate = _fdate.ToString("dd-MMM-yyyy");
            //ViewBag.FromDate = FDate;
            //ViewBag.ToDate = TDate;
            DataTable dtNew = new DataTable();

            DataTable dtList = new DataTable();
            dtList = order.getVendorCustomerOrder(obj.VendorId);
            int userRecords = dtList.Rows.Count;
            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("Vendor", typeof(string));


                dtNew.Columns.Add("OrderNo", typeof(Int64));
                dtNew.Columns.Add("OrderDate", typeof(string));
                dtNew.Columns.Add("ProductName", typeof(string));
                dtNew.Columns.Add("image", typeof(string));
                dtNew.Columns.Add("SectorName", typeof(string));
                dtNew.Columns.Add("Qty", typeof(Int64));
                dtNew.Columns.Add("PurchasePrice", typeof(decimal));
                dtNew.Columns.Add("status", typeof(string));
                //dtNew.Columns.Add("orderby", typeof(Int64));
                //dtNew.Columns.Add("sid", typeof(Int64));

                dtNew.Columns.Add("DeliveryStatus", typeof(string));

                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    try
                    {
                        //Status = "0";
                        DataRow dr = dtNew.NewRow();
                        // dr["status"] = "Success";
                        dr["id"] = dtList.Rows[i]["Id"].ToString().Trim();
                        dr["Vendor"] = dtList.Rows[i]["Vendor"].ToString();
                        //int productID = Convert.ToInt32(dtList.Rows[i]["Id"].ToString().Trim());
                        dr["OrderNo"] = dtList.Rows[i]["OrderNo"].ToString().Trim();

                        dr["OrderDate"] = dtList.Rows[i]["OrderDate"].ToString().Trim();
                        dr["ProductName"] = dtList.Rows[i]["ProductName"].ToString();


                        if (!string.IsNullOrEmpty(dtList.Rows[i]["image"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtList.Rows[i]["image"].ToString().Trim());
                            dr["image"] = Helper.PhotoFolderPath + "/image/product/" + encoded;
                        }
                        else
                            dr["image"] = "";
                        dr["SectorName"] = dtList.Rows[i]["Sector"].ToString().Trim();








                        if (!string.IsNullOrEmpty(dtList.Rows[i]["Qty"].ToString()))
                            dr["Qty"] = Convert.ToInt32(dtList.Rows[i]["Qty"]);
                        else
                            dr["Qty"] = "0";
                        if (!string.IsNullOrEmpty(dtList.Rows[i]["PurchasePrice"].ToString()))
                            dr["PurchasePrice"] = Convert.ToDecimal(dtList.Rows[i]["PurchasePrice"]);
                        else
                            dr["PurchasePrice"] = "0";

                        dr["status"] = dtList.Rows[i]["status"].ToString();

                        //if (!string.IsNullOrEmpty(dtList.Rows[i]["orderby"].ToString()))
                        //    dr["orderby"] = Convert.ToInt32(dtList.Rows[i]["orderby"]);
                        //else
                        //    dr["orderby"] = "0";
                        //if (!string.IsNullOrEmpty(dtList.Rows[i]["sid"].ToString()))
                        //    dr["sid"] = Convert.ToInt64(dtList.Rows[i]["sid"]);
                        //else
                        //    dr["sid"] = "0";


                        dr["DeliveryStatus"] = dtList.Rows[i]["DeliveryStatus"].ToString();




                        dtNew.Rows.Add(dr);
                    }
                    catch { }
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["OrderDetail"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""OrderDetail"":" + dict["OrderDetail"];
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
                dict["OrderDetail"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""OrderDetail"":" + dict["OrderDetail"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }




        }

        [Route("api/VendorApi/DailyTomOrderVendor/{Vendorid?}")]
        [HttpGet]
        public HttpResponseMessage DailyTomOrderVendor(string Vendorid) //JsonResult
        {

            Vendornew obj = new Vendornew();

            obj.VendorId = Convert.ToInt32(Vendorid);



            Vendornew order = new Vendornew();
            //ViewBag.FromDate = _fdate.ToString("dd-MMM-yyyy");
            //ViewBag.FromDate = FDate;
            //ViewBag.ToDate = TDate;
            DataTable dtNew = new DataTable();

            DataTable dtList = new DataTable();
            dtList = order.getVendorTomCustomerOrder(obj.VendorId);
            int userRecords = dtList.Rows.Count;
            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("Vendor", typeof(string));


                dtNew.Columns.Add("OrderNo", typeof(Int64));
                dtNew.Columns.Add("OrderDate", typeof(string));
                dtNew.Columns.Add("ProductName", typeof(string));
                dtNew.Columns.Add("image", typeof(string));
                dtNew.Columns.Add("SectorName", typeof(string));
                dtNew.Columns.Add("Qty", typeof(Int64));
                dtNew.Columns.Add("PurchasePrice", typeof(decimal));
                dtNew.Columns.Add("status", typeof(string));
                //dtNew.Columns.Add("orderby", typeof(Int64));
                //dtNew.Columns.Add("sid", typeof(Int64));

                dtNew.Columns.Add("DeliveryStatus", typeof(string));

                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    try
                    {
                        //Status = "0";
                        DataRow dr = dtNew.NewRow();
                        // dr["status"] = "Success";
                        dr["id"] = dtList.Rows[i]["Id"].ToString().Trim();
                        dr["Vendor"] = dtList.Rows[i]["Vendor"].ToString();
                        //int productID = Convert.ToInt32(dtList.Rows[i]["Id"].ToString().Trim());
                        dr["OrderNo"] = dtList.Rows[i]["OrderNo"].ToString().Trim();

                        dr["OrderDate"] = dtList.Rows[i]["OrderDate"].ToString().Trim();
                        dr["ProductName"] = dtList.Rows[i]["ProductName"].ToString();


                        if (!string.IsNullOrEmpty(dtList.Rows[i]["image"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtList.Rows[i]["image"].ToString().Trim());
                            dr["image"] = Helper.PhotoFolderPath + "/image/product/" + encoded;
                        }
                        else
                            dr["image"] = "";
                        dr["SectorName"] = dtList.Rows[i]["Sector"].ToString().Trim();








                        if (!string.IsNullOrEmpty(dtList.Rows[i]["Qty"].ToString()))
                            dr["Qty"] = Convert.ToInt32(dtList.Rows[i]["Qty"]);
                        else
                            dr["Qty"] = "0";
                        if (!string.IsNullOrEmpty(dtList.Rows[i]["PurchasePrice"].ToString()))
                            dr["PurchasePrice"] = Convert.ToDecimal(dtList.Rows[i]["PurchasePrice"]);
                        else
                            dr["PurchasePrice"] = "0";

                        dr["status"] = dtList.Rows[i]["status"].ToString();

                        //if (!string.IsNullOrEmpty(dtList.Rows[i]["orderby"].ToString()))
                        //    dr["orderby"] = Convert.ToInt32(dtList.Rows[i]["orderby"]);
                        //else
                        //    dr["orderby"] = "0";
                        //if (!string.IsNullOrEmpty(dtList.Rows[i]["sid"].ToString()))
                        //    dr["sid"] = Convert.ToInt64(dtList.Rows[i]["sid"]);
                        //else
                        //    dr["sid"] = "0";


                        dr["DeliveryStatus"] = dtList.Rows[i]["DeliveryStatus"].ToString();




                        dtNew.Rows.Add(dr);
                    }
                    catch { }
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["OrderDetail"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""OrderDetail"":" + dict["OrderDetail"];
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
                dict["OrderDetail"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""OrderDetail"":" + dict["OrderDetail"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }




        }



        [Route("api/VendorApi/VendorOwnProduct/{Vendorid?}")]
        [HttpGet]
        public HttpResponseMessage VendorOwnProduct(string Vendorid)
        {



            Vendornew obj = new Vendornew();

            obj.VendorId = Convert.ToInt32(Vendorid);



            Vendornew order = new Vendornew();
            //ViewBag.FromDate = _fdate.ToString("dd-MMM-yyyy");
            //ViewBag.FromDate = FDate;
            //ViewBag.ToDate = TDate;
            DataTable dtNew = new DataTable();

            DataTable dtList = new DataTable();
            dtList = order.getSectorProductList(obj.VendorId);
            int userRecords = dtList.Rows.Count;
            if (userRecords > 0)
            {
                dtNew.Columns.Add("Id", typeof(Int64));
                dtNew.Columns.Add("Sector", typeof(Int64));


                dtNew.Columns.Add("VendorId", typeof(Int64));
                dtNew.Columns.Add("CategoryId", typeof(Int64));
                dtNew.Columns.Add("ProductId", typeof(Int64));
                dtNew.Columns.Add("OrderBy", typeof(string));
                dtNew.Columns.Add("IsActive", typeof(string));
                dtNew.Columns.Add("SectorName", typeof(string));
                dtNew.Columns.Add("FirstName", typeof(string));
                dtNew.Columns.Add("LastName", typeof(string));
                dtNew.Columns.Add("CategoryName", typeof(string));
                dtNew.Columns.Add("ProductName", typeof(string));
                dtNew.Columns.Add("Image", typeof(string));




                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    try
                    {
                        //Status = "0";
                        DataRow dr = dtNew.NewRow();
                        // dr["status"] = "Success";
                        dr["Id"] = dtList.Rows[i]["Id"].ToString().Trim();
                        dr["Sector"] = dtList.Rows[i]["SectorId"].ToString();
                        //int productID = Convert.ToInt32(dtList.Rows[i]["Id"].ToString().Trim());
                        dr["VendorId"] = dtList.Rows[i]["VendorId"].ToString();

                        dr["CategoryId"] = dtList.Rows[i]["CategoryId"].ToString();
                        dr["ProductId"] = dtList.Rows[i]["ProductId"].ToString();

                        dr["OrderBy"] = dtList.Rows[i]["OrderBy"].ToString();
                        dr["IsActive"] = dtList.Rows[i]["IsActive"].ToString();
                        dr["SectorName"] = dtList.Rows[i]["SectorName"].ToString();
                        dr["FirstName"] = dtList.Rows[i]["FirstName"].ToString();
                        dr["LastName"] = dtList.Rows[i]["FirstName"].ToString();
                        dr["CategoryName"] = dtList.Rows[i]["CategoryName"].ToString();
                        dr["ProductName"] = dtList.Rows[i]["ProductName"].ToString();
                        if (!string.IsNullOrEmpty(dtList.Rows[i]["Image"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtList.Rows[i]["Image"].ToString().Trim());
                            dr["Image"] = Helper.PhotoFolderPath + "/image/product/" + encoded;
                        }
                        else
                            dr["Image"] = "";




                        dtNew.Rows.Add(dr);
                    }
                    catch { }
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["ProductDetail"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""ProductDetail"":" + dict["ProductDetail"];
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
                dict["ProductDetail"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""ProductDetail"":" + dict["ProductDetail"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }




        public IHttpActionResult UpdateProductPhoto(Vendornew item)
        {
            string jsonstring = null;

            DataTable dt = new DataTable();
            DataTable dtNew = new DataTable();

            int result = 0, updatepoint = 0;
            dtNew.Columns.Add("imagepath", typeof(string));
            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("message", typeof(string));
            DataRow dr = dtNew.NewRow();
            dr["status"] = "failed";
            dr["message"] = "";

            if ((!string.IsNullOrEmpty(item.VendorId.ToString()) || item.VendorId != 0) && (!string.IsNullOrEmpty(item.Description.ToString())))
            {

                Vendornew obj = new Vendornew();
                string filepath = "~/image/VendorImage/";

                //



                //

                if (!string.IsNullOrEmpty(item.base64Image.ToString()))
                {
                    filepath = AppDomain.CurrentDomain.BaseDirectory + ("image\\VendorImage\\" + item.Photo);//active
                    string filename = Path.GetFileName(filepath);

                    var bytess = Convert.FromBase64String(item.base64Image);
                    using (var imageFile = new FileStream(filepath, FileMode.Create))
                    {
                        imageFile.Write(bytess, 0, bytess.Length);
                        imageFile.Flush();
                    }
                }
                obj.Photo = item.Photo;
                obj.VendorId = item.VendorId;
                obj.Description = item.Description;

                result = obj.InsertProductPhoto(obj);

                if (result > 0)
                {

                    dr["imagepath"] = Helper.PhotoFolderPath + "/image/VendorImage/" + obj.Photo;
                    dr["status"] = "success";
                    dr["message"] = "Invoice/ Photo Updated!!!";
                }
                else
                {
                    dr["imagepath"] = "";
                    dr["status"] = "failed";
                    dr["message"] = "Invoice/ Photo not Updated!!!";
                }

            }
            else
            {
                dr["status"] = "failed";
                dr["message"] = "Please Enter Valid Data";
            }


            dtNew.Rows.Add(dr);
            jsonstring = string.Empty;
            jsonstring = JsonConvert.SerializeObject(dtNew);
            jsonstring = jsonstring.Remove(jsonstring.Length - 1);
            jsonstring = jsonstring.Remove(0, 1);
            return Ok(dtNew);
        }


        public IHttpActionResult VendordocUpload(Vendornew item)
        {

            string jsonstring = null;

            DataTable dt = new DataTable();
            DataTable dtNew = new DataTable();

            int result = 0, updatepoint = 0;
            dtNew.Columns.Add("imagepath", typeof(string));
            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("message", typeof(string));
            DataRow dr = dtNew.NewRow();
            dr["status"] = "failed";
            dr["message"] = "";

            if ((!string.IsNullOrEmpty(item.VendorId.ToString()) || item.VendorId != 0))
            {

                Vendornew obj = new Vendornew();
                string filepath = "~/image/VendorDoc/";
                string filename = "";


                if (!string.IsNullOrEmpty(item.base64Aadhar.ToString()))
                {
                    filepath = AppDomain.CurrentDomain.BaseDirectory + ("image\\VendorDoc\\" + item.Aadhar);//active
                    filename = Path.GetFileName(filepath);

                    var bytess = Convert.FromBase64String(item.base64Aadhar);
                    using (var imageFileAadhar = new FileStream(filepath, FileMode.Create))
                    {
                        imageFileAadhar.Write(bytess, 0, bytess.Length);
                        imageFileAadhar.Flush();
                    }

                    item.Aadharstatus = "Pending";
                }

                if (!string.IsNullOrEmpty(item.base64Pan.ToString()))
                {
                    filepath = AppDomain.CurrentDomain.BaseDirectory + ("image\\VendorDoc\\" + item.Pan);//active
                    filename = Path.GetFileName(filepath);

                    var bytess = Convert.FromBase64String(item.base64Pan);
                    using (var imageFilePan = new FileStream(filepath, FileMode.Create))
                    {
                        imageFilePan.Write(bytess, 0, bytess.Length);
                        imageFilePan.Flush();
                    }
                    item.Panstatus = "Pending";
                }

                if (!string.IsNullOrEmpty(item.base64License.ToString()))
                {
                    filepath = AppDomain.CurrentDomain.BaseDirectory + ("image\\VendorDoc\\" + item.License);//active
                    filename = Path.GetFileName(filepath);

                    var bytess = Convert.FromBase64String(item.base64License);
                    using (var imageFileLicense = new FileStream(filepath, FileMode.Create))
                    {
                        imageFileLicense.Write(bytess, 0, bytess.Length);
                        imageFileLicense.Flush();
                    }
                    item.Licensestatus = "Pending";
                }


                obj.Aadhar = item.Aadhar;
                obj.Pan = item.Pan;
                obj.License = item.License;
                obj.VendorId = item.VendorId;
                obj.bankaccount = item.bankaccount;
                obj.ifsc = item.ifsc;
                obj.bankname = item.bankname;
                obj.Accholdername = item.Accholdername;
                obj.GSTnewfield1 = item.GSTnewfield1;
                obj.GSTnewfield2 = item.GSTnewfield2;

                DataTable dtDMStatusList = new DataTable();
                dtDMStatusList = obj.getVendorwiseDoc(item.VendorId);

                if (dtDMStatusList.Rows.Count > 0)
                {
                    obj.Bankstatus = dtDMStatusList.Rows[0].ItemArray[14].ToString();

                    if (dtDMStatusList.Rows[0].ItemArray[5].ToString() != obj.bankaccount || dtDMStatusList.Rows[0].ItemArray[6].ToString() != obj.ifsc || dtDMStatusList.Rows[0].ItemArray[7].ToString() != obj.bankname || dtDMStatusList.Rows[0].ItemArray[8].ToString() != obj.Accholdername)
                    {
                        obj.Bankstatus = "Pending";
                    }
                    obj.newstatus = dtDMStatusList.Rows[0].ItemArray[18].ToString();
                    if (dtDMStatusList.Rows[0].ItemArray[16].ToString() != obj.GSTnewfield1 || dtDMStatusList.Rows[0].ItemArray[17].ToString() != obj.GSTnewfield2)
                    {
                        obj.newstatus = "Pending";
                    }



                    result = obj.UpdateDoc(obj);
                }
                else
                {
                    obj.Aadharstatus = "Pending";
                    obj.Panstatus = "Pending";
                    obj.Licensestatus = "Pending";

                    obj.Bankstatus = "Pending";
                    obj.newstatus = "Pending";
                    result = obj.InsertDoc(obj);
                }


                if (result > 0)
                {

                    dr["imagepath"] = Helper.PhotoFolderPath + "/image/VendorDoc/";
                    dr["status"] = "success";
                    dr["message"] = "Photo Updated!!!";
                }
                else
                {
                    dr["imagepath"] = "";
                    dr["status"] = "failed";
                    dr["message"] = "Photo not Updated!!!";
                }

            }
            else
            {
                dr["status"] = "failed";
                dr["message"] = "Please Enter Valid Data";
            }


            dtNew.Rows.Add(dr);
            jsonstring = string.Empty;
            jsonstring = JsonConvert.SerializeObject(dtNew);
            jsonstring = jsonstring.Remove(jsonstring.Length - 1);
            jsonstring = jsonstring.Remove(0, 1);
            return Ok(dtNew);
        }



        [Route("api/VendorApi/VendorOrderDateWise/{Vendorid?}/{FDate?}/{TDate?}")]
        [HttpGet]

        public HttpResponseMessage VendorOrderDateWise(string Vendorid, DateTime? FDate, DateTime? TDate) //JsonResult
        {
            DateTime Fromdate = DateTime.Now, Todate = DateTime.Now;
            //if (!string.IsNullOrEmpty(FDate.ToString()))
            //    Fromdate = Convert.ToDateTime(DateTime.ParseExact(FDate, @"dd-MM-yyyy", null));
            //if (!string.IsNullOrEmpty(TDate.ToString()))
            //    Todate = Convert.ToDateTime(DateTime.ParseExact(TDate, @"dd-MM-yyyy", null));
            Fromdate = Convert.ToDateTime(FDate);
            Todate = Convert.ToDateTime(TDate);

            Vendornew obj = new Vendornew();

            obj.VendorId = Convert.ToInt32(Vendorid);

            int SectorId = 2;

            Vendornew order = new Vendornew();
            //ViewBag.FromDate = _fdate.ToString("dd-MMM-yyyy");
            //ViewBag.FromDate = FDate;
            //ViewBag.ToDate = TDate;
            DataTable dtNew = new DataTable();

            DataTable dtList = new DataTable();
            CustomerOrder order1 = new CustomerOrder();

            dtList = order1.getSectorVendorOrderapi(obj.VendorId, Fromdate, Todate);

            // dtList = order.getVendorCustomerOrderDatewise(obj.VendorId, Fromdate, Todate);
            int userRecords = dtList.Rows.Count;
            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("Vendor", typeof(string));
                dtNew.Columns.Add("Mobile", typeof(string));

                dtNew.Columns.Add("OrderNo", typeof(Int64));
                dtNew.Columns.Add("OrderDate", typeof(string));
                dtNew.Columns.Add("ProductName", typeof(string));
                dtNew.Columns.Add("image", typeof(string));
                dtNew.Columns.Add("SectorName", typeof(string));
                dtNew.Columns.Add("Qty", typeof(Int64));
                dtNew.Columns.Add("PurchasePrice", typeof(decimal));
                dtNew.Columns.Add("status", typeof(string));
                //dtNew.Columns.Add("orderby", typeof(Int64));
                //dtNew.Columns.Add("sid", typeof(Int64));

                dtNew.Columns.Add("DeliveryStatus", typeof(string));
                dtNew.Columns.Add("AttributeName", typeof(string));
                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    try
                    {
                        //Status = "0";
                        DataRow dr = dtNew.NewRow();
                        // dr["status"] = "Success";
                        dr["id"] = dtList.Rows[i]["Id"].ToString().Trim();
                        dr["Vendor"] = dtList.Rows[i]["Vendor"].ToString();
                        dr["Mobile"] = dtList.Rows[i]["MobileNo"].ToString();
                        //int productID = Convert.ToInt32(dtList.Rows[i]["Id"].ToString().Trim());
                        dr["OrderNo"] = dtList.Rows[i]["OrderNo"].ToString().Trim();

                        dr["OrderDate"] = dtList.Rows[i]["OrderDate"].ToString().Trim();
                        dr["ProductName"] = dtList.Rows[i]["ProductName"].ToString();


                        if (!string.IsNullOrEmpty(dtList.Rows[i]["image"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtList.Rows[i]["image"].ToString().Trim());
                            dr["image"] = Helper.PhotoFolderPath + "/image/product/" + encoded;
                        }
                        else
                            dr["image"] = "";
                        dr["SectorName"] = dtList.Rows[i]["Sector"].ToString().Trim();


                        if (!string.IsNullOrEmpty(dtList.Rows[i]["Qty"].ToString()))
                            dr["Qty"] = Convert.ToInt32(dtList.Rows[i]["Qty"]);
                        else
                            dr["Qty"] = "0";
                        if (!string.IsNullOrEmpty(dtList.Rows[i]["PurchasePrice"].ToString()))
                            dr["PurchasePrice"] = Convert.ToDecimal(dtList.Rows[i]["PurchasePrice"]);
                        else
                            dr["PurchasePrice"] = "0";

                        dr["status"] = dtList.Rows[i]["status"].ToString();

                        //if (!string.IsNullOrEmpty(dtList.Rows[i]["orderby"].ToString()))
                        //    dr["orderby"] = Convert.ToInt32(dtList.Rows[i]["orderby"]);
                        //else
                        //    dr["orderby"] = "0";
                        //if (!string.IsNullOrEmpty(dtList.Rows[i]["sid"].ToString()))
                        //    dr["sid"] = Convert.ToInt64(dtList.Rows[i]["sid"]);
                        //else
                        //    dr["sid"] = "0";


                        dr["DeliveryStatus"] = dtList.Rows[i]["DeliveryStatus"].ToString();
                        dr["AttributeName"] = dtList.Rows[i]["Name"].ToString();



                        dtNew.Rows.Add(dr);
                    }
                    catch { }
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["OrderDetail"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""OrderDetail"":" + dict["OrderDetail"];
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
                dict["OrderDetail"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""OrderDetail"":" + dict["OrderDetail"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }



        [Route("api/VendorApi/VendorPayment/{Vendorid?}")]
        [HttpGet]
        public HttpResponseMessage VendorPayment(string Vendorid)
        {



            Vendornew obj = new Vendornew();

            obj.VendorId = Convert.ToInt32(Vendorid);



            Vendornew order = new Vendornew();
            //ViewBag.FromDate = _fdate.ToString("dd-MMM-yyyy");
            //ViewBag.FromDate = FDate;
            //ViewBag.ToDate = TDate;
            DataTable dtNew = new DataTable();

            DataTable dtList = new DataTable();
            dtList = order.getVendorPaymentList(obj.VendorId);
            int userRecords = dtList.Rows.Count;
            if (userRecords > 0)
            {
                dtNew.Columns.Add("Id", typeof(Int64));



                dtNew.Columns.Add("VendorId", typeof(Int64));

                dtNew.Columns.Add("RefferenceNo", typeof(string));
                dtNew.Columns.Add("Amount", typeof(string));
                dtNew.Columns.Add("UpdatedOn", typeof(string));





                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    try
                    {
                        //Status = "0";
                        DataRow dr = dtNew.NewRow();
                        // dr["status"] = "Success";
                        dr["Id"] = dtList.Rows[i]["Id"].ToString().Trim();

                        dr["VendorId"] = dtList.Rows[i]["VendorId"].ToString();

                        dr["RefferenceNo"] = dtList.Rows[i]["RefferenceNo"].ToString();
                        dr["Amount"] = dtList.Rows[i]["Amount"].ToString();

                        dr["UpdatedOn"] = dtList.Rows[i]["UpdatedOn"].ToString();





                        dtNew.Rows.Add(dr);
                    }
                    catch { }
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["PaymentList"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""PaymentList"":" + dict["PaymentList"];
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
                dict["PaymentList"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""PaymentList"":" + dict["PaymentList"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }





        [Route("api/VendorApi/VendorOrderDateWise1/{Vendorid?}/{FDate?}/{TDate?}")]
        [HttpGet]

        public HttpResponseMessage VendorOrderDateWise1(string Vendorid, DateTime? FDate, DateTime? TDate) //JsonResult
        {
            DateTime Fromdate = DateTime.Now, Todate = DateTime.Now;

            Fromdate = Convert.ToDateTime(FDate);
            Todate = Convert.ToDateTime(TDate);

            Vendornew obj = new Vendornew();

            obj.VendorId = Convert.ToInt32(Vendorid);



            Vendornew order = new Vendornew();

            DataTable dtNew = new DataTable();
            DataTable dtNewItem = new DataTable();
            DataTable dtList = new DataTable();
            dtList = order.getVendorCustomerOrderDatewisenew(obj.VendorId, Fromdate, Todate);
            int userRecords = dtList.Rows.Count;
            if (userRecords > 0)
            {
                //dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("Vendor", typeof(string));
                dtNew.Columns.Add("OrderDate", typeof(DateTime));
                dtNew.Columns.Add("itemdetail", typeof(string));

                dtNewItem.Columns.Add("OrderNo", typeof(Int64));
                dtNewItem.Columns.Add("ProductName", typeof(string));
                dtNewItem.Columns.Add("image", typeof(string));
                dtNewItem.Columns.Add("SectorName", typeof(string));
                dtNewItem.Columns.Add("Qty", typeof(Int64));
                dtNewItem.Columns.Add("PurchasePrice", typeof(decimal));
                dtNewItem.Columns.Add("status", typeof(string));
                //dtNew.Columns.Add("orderby", typeof(Int64));
                //dtNew.Columns.Add("sid", typeof(Int64));

                dtNewItem.Columns.Add("DeliveryStatus", typeof(string));

                for (int i = 0; i < dtList.Rows.Count; i++)
                {
                    DataRow dr = dtNew.NewRow();
                    try
                    {

                        // dr["status"] = "Success";
                        //dr["id"] = Convert.ToInt32(dtList.Rows[i]["Id"]);
                        dr["Vendor"] = dtList.Rows[i]["Vendor"].ToString();
                        dr["OrderDate"] = Convert.ToDateTime(dtList.Rows[i]["OrderDate"]);
                        // int oid = Convert.ToInt32(dtList.Rows[i]["Id"]);
                        DataTable dtitemRecord = new DataTable();
                        dtitemRecord = order.getVendorCustomerOrderSingleDate(obj.VendorId, Convert.ToDateTime(dr["OrderDate"]));
                        int orderRecords = dtitemRecord.Rows.Count;
                        //Status = "0";

                        if (orderRecords > 0)
                        {
                            for (int j = 0; j < dtitemRecord.Rows.Count; j++)
                            {
                                DataRow dritem = dtNewItem.NewRow();
                                // dr["status"] = "Success";

                                //int productID = Convert.ToInt32(dtList.Rows[i]["Id"].ToString().Trim());
                                dritem["OrderNo"] = dtitemRecord.Rows[j]["OrderNo"].ToString().Trim();


                                dritem["ProductName"] = dtitemRecord.Rows[j]["ProductName"].ToString();


                                if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["image"].ToString()))
                                {
                                    var encoded = Uri.EscapeUriString(dtitemRecord.Rows[j]["image"].ToString().Trim());
                                    dritem["image"] = Helper.PhotoFolderPath + "/image/product/" + encoded;
                                }
                                else
                                    dritem["image"] = "";
                                dritem["SectorName"] = dtitemRecord.Rows[j]["Sector"].ToString().Trim();


                                if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["Qty"].ToString()))
                                    dritem["Qty"] = Convert.ToInt32(dtitemRecord.Rows[j]["Qty"]);
                                else
                                    dritem["Qty"] = "0";
                                if (!string.IsNullOrEmpty(dtitemRecord.Rows[j]["PurchasePrice"].ToString()))
                                    dritem["PurchasePrice"] = Convert.ToDecimal(dtitemRecord.Rows[j]["PurchasePrice"]);
                                else
                                    dritem["PurchasePrice"] = "0";

                                dritem["status"] = dtitemRecord.Rows[j]["status"].ToString();


                                dritem["DeliveryStatus"] = dtitemRecord.Rows[j]["DeliveryStatus"].ToString();


                                dtNewItem.Rows.Add(dritem);


                            }


                            string jsonString1 = string.Empty;
                            jsonString1 = JsonConvert.SerializeObject(dtNewItem); //new Newtonsoft.Json.Formatting()

                            var dict1 = new Dictionary<string, string>();
                            dict1["walletitem"] = jsonString1;

                            dr["itemdetail"] = dict1["walletitem"];
                        }

                        dtNewItem.Clear();
                        dtNew.Rows.Add(dr);

                    }
                    catch { }
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                //  dict["status"] = "success";
                dict["Transaction"] = jsonString;

                string one = @"{""status"":""success""";
                string two = @",""Transaction"":" + dict["Transaction"];
                string three = one + two + "}";
                three = three.Replace("\"[", "[");
                three = three.Replace("]\"", "]");
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
                dict["Transaction"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""Transaction"":" + dict["Transaction"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }



        [Route("api/VendorApi/VendorContactUpdate/{VendorId?}/{ContactNo?}")]
        [HttpGet]

        public IHttpActionResult VendorContactUpdate(int VendorId, string ContactNo)//JsonResult
        {
            Vendornew obj = new Vendornew();

            obj.VendorId = VendorId;
            obj.MobileNo = ContactNo.ToString();
            int updatecontact = 0;

            if (!string.IsNullOrEmpty(obj.VendorId.ToString()) && !string.IsNullOrEmpty(obj.MobileNo.ToString()))
            {
                updatecontact = obj.UpdateContact(VendorId, ContactNo);
            }

            if (updatecontact > 0)
            {
                return Ok("Contact Updated");
            }
            else
            {
                return Ok("Contact Not Updated");
            }
        }


        [Route("api/VendorApi/VendorGetDocStatus/{VendorId?}")]

        [HttpGet]

        public HttpResponseMessage VendorGetDocStatus(String VendorId)
        {

            Vendornew obj = new Vendornew();

            Customer objcust = new Customer();
            string jsonString1 = string.Empty;


            obj.VendorId = Convert.ToInt32(VendorId);

            DeliveryBoy order = new DeliveryBoy();

            DataTable dtNew = new DataTable();
            DataTable dtNew1 = new DataTable();
            DataTable dtList = new DataTable();
            DataTable dtList1 = new DataTable();

            dtList1 = obj.getVendorDocStaus(obj.VendorId);
            int userRecords1 = dtList1.Rows.Count;
            if (dtList1.Rows.Count > 0)
            {

                dtNew1.Columns.Add("Aadhar", typeof(string));
                dtNew1.Columns.Add("Pan", typeof(string));
                dtNew1.Columns.Add("License", typeof(string));
                dtNew1.Columns.Add("BankAccount", typeof(string));
                dtNew1.Columns.Add("Ifsc", typeof(string));
                dtNew1.Columns.Add("Bankname", typeof(string));
                dtNew1.Columns.Add("AccholderName", typeof(string));
                dtNew1.Columns.Add("Status", typeof(string));
                dtNew1.Columns.Add("Decsiption", typeof(string));
                dtNew1.Columns.Add("Gstnewfield1", typeof(string));
                dtNew1.Columns.Add("Gstnewfield2", typeof(string));

                dtNew1.Columns.Add("Aadharstatus", typeof(string));
                dtNew1.Columns.Add("Panstatus", typeof(string));
                dtNew1.Columns.Add("Licensestatus", typeof(string));
                dtNew1.Columns.Add("Bankstatus", typeof(string));
                dtNew1.Columns.Add("GstStatus", typeof(string));
                for (int i = 0; i < dtList1.Rows.Count; i++)
                {
                    try
                    {
                        DataRow dr1 = dtNew1.NewRow();

                        if (!string.IsNullOrEmpty(dtList1.Rows[0]["Aadhar"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtList1.Rows[0]["Aadhar"].ToString().Trim());
                            dr1["Aadhar"] = Helper.PhotoFolderPath + "/image/VendorDoc/" + encoded;
                        }
                        else
                            dr1["Aadhar"] = "";

                        if (!string.IsNullOrEmpty(dtList1.Rows[0]["Pan"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtList1.Rows[0]["Pan"].ToString().Trim());
                            dr1["Pan"] = Helper.PhotoFolderPath + "/image/VendorDoc/" + encoded;
                        }
                        else
                            dr1["Pan"] = "";

                        if (!string.IsNullOrEmpty(dtList1.Rows[0]["License"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtList1.Rows[0]["License"].ToString().Trim());
                            dr1["License"] = Helper.PhotoFolderPath + "/image/VendorDoc/" + encoded;
                        }
                        else
                            dr1["License"] = "";


                        dr1["BankAccount"] = dtList1.Rows[i]["BankAccount"].ToString();
                        dr1["Ifsc"] = dtList1.Rows[i]["Ifsc"].ToString();
                        dr1["Bankname"] = dtList1.Rows[i]["Bankname"].ToString();
                        dr1["AccholderName"] = dtList1.Rows[i]["AccholderName"].ToString();
                        dr1["Status"] = dtList1.Rows[i]["Status"].ToString();
                        dr1["Decsiption"] = dtList1.Rows[i]["Decsiption"].ToString();
                        dr1["Gstnewfield1"] = dtList1.Rows[i]["Gstnewfield1"].ToString();
                        dr1["Gstnewfield2"] = dtList1.Rows[i]["Gstnewfield2"].ToString();

                        dr1["Aadharstatus"] = dtList1.Rows[i]["Aadharstatus"].ToString();
                        dr1["Panstatus"] = dtList1.Rows[i]["Panstatus"].ToString();
                        dr1["Licensestatus"] = dtList1.Rows[i]["Licensestatus"].ToString();
                        dr1["Bankstatus"] = dtList1.Rows[i]["Bankstatus"].ToString();
                        dr1["GstStatus"] = dtList1.Rows[i]["newstatus"].ToString();


                        dtNew1.Rows.Add(dr1);
                    }

                    catch { }


                }



                jsonString1 = JsonConvert.SerializeObject(dtNew1);
            }


            if (userRecords1 > 0)
            {

                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();



                dict["status"] = "success";

                dict["DmStatus"] = jsonString1;

                string one = @"{""status"":""success""";

                string three = @",""VendorStatus"":" + dict["DmStatus"];
                string four = one + three + "}";

                var str = four.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {



                dtNew1.Columns.Add("status", typeof(string));
                dtNew1.Columns.Add("msg", typeof(string));
                DataRow dr1 = dtNew1.NewRow();
                dr1["status"] = "Fail";
                dr1["msg"] = "No Record Found";
                dtNew1.Rows.Add(dr1);

                //new Newtonsoft.Json.Formatting()
                jsonString1 = string.Empty;
                jsonString1 = JsonConvert.SerializeObject(dtNew1);
                var dict = new Dictionary<string, string>();
                dict["status"] = "Fail";

                dict["DmStatus"] = jsonString1;

                string one = @"{""status"":""Fail""";

                string three = @",""VendorStatus"":" + dict["DmStatus"];
                string four = one + three + "}";

                var str = four.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }

            //return Ok(dtList);
        }

        [Route("api/VendorApi/UpdatePassword/{VendorId}/{Oldpass}/{NewPass}")]
        [HttpGet]
        public HttpResponseMessage UpdatePassword(int VendorId, string Oldpass, string NewPass)
        {
            string Status = ""; string jsonString = null; string str1 = null; string str2 = null;

            Vendornew obj = new Vendornew();

            DataTable dtNew = new DataTable();

            Vendornew objlogin = new Vendornew();

            DataTable dtUser = new DataTable();



            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("msg", typeof(string));

            DataRow dr = dtNew.NewRow();
            dr["status"] = "failed";
            dr["msg"] = "";


            if (VendorId.ToString() != null)
            {


                DataTable dtpassword = new DataTable();
                dtpassword = objlogin.Vendorlogin2(VendorId, Oldpass);


                if (dtpassword.Rows.Count > 0)
                {
                    //dr["status"] = "Success";
                    //dr["msg"] = "Find User";
                    //int customerId = Convert.ToInt32(dtpassword.Rows[0]["Id"]);
                    //var _customer = objlogin.BindCustomer(customerId);

                    int updPwd = obj.UpdateVendorPwd(VendorId, NewPass);

                    if (updPwd > 0)
                    {
                        dr["status"] = "success";
                        dr["msg"] = "Password Updated Successfully";
                    }

                }

                else
                {
                    dr["status"] = "Failed";
                    dr["msg"] = "Old Password Mismatched";
                }
            }







            dtNew.Rows.Add(dr);
            jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dtNew);
            str1 = jsonString.ToString().Replace(@"[", "");
            str2 = str1.ToString().Replace(@"]", "");
            jsonString = str2;
            //  return jsonString;
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return response;
        }



        [Route("api/VendorApi/UpdateAddress/{VendorId}/{Lat}/{Lon}")]
        [HttpGet]
        //public IHttpActionResult UpdateAddress(Vendornew item)
        public IHttpActionResult UpdateAddress(int? VendorId, string Lat, string Lon)
        {
            string Status = ""; string jsonString = null; string str1 = null; string str2 = null;

            Vendornew obj = new Vendornew();

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();
            Lat = Lat.Replace("-", ".");
            Lon = Lon.Replace("-", ".");
            //obj.VendorId = item.VendorId;
            //obj.lat = item.lat;
            //obj.lon = item.lon;
            obj.VendorId = Convert.ToInt32(VendorId);
            obj.lat = Lat;
            obj.lon = Lon;

            //Customer response = new Customer();
            //response.status = "Failed";
            try
            {


                int favourite = obj.UpdateAddress(obj.VendorId, obj.lat, obj.lon);
                if (favourite > 0)
                {
                    dr["status"] = "Success";
                    dr["error_msg"] = "Location Updated...";

                }
                else
                {
                    dr["status"] = "Fail";
                    dr["error_msg"] = "Location Not Updated...";
                }
                //return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                dr["status"] = "Fail";
                dr["error_msg"] = "Location Not Updated...";
            }
            dtNew.Rows.Add(dr);
            // jsonString = string.Empty;
            //jsonString = JsonConvert.SerializeObject(dtNew);
            //jsonString = jsonString.Replace(@"[", "");
            // jsonString = jsonString.Replace(@"]", "");

            // ViewBag.Status = jsonString;
            // return jsonString;

            return Ok(dtNew);


        }
    }
}

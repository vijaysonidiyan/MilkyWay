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
    public class CustomerApiController : ApiController
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
        ///{FirstName?}/{LastName?}/{Mobile?}/{Email?}/{BuildingId?}/{FlatId?}/{Password?}

        [Route("api/CustomerApi/RegisterCustomerTest")]
        [HttpGet]
        public HttpResponseMessage RegisterCustomerTest(string FirstName, string LastName, string Mobile, string Email, int BuildingId, int FlatId, string Password)
        {
            string Status = ""; string jsonString = null; string str1 = null; string str2 = null;

            Customer obj = new Customer();

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            //dtNew.Columns.Add("FirstName", typeof(string));
            //dtNew.Columns.Add("LastName", typeof(string));
            //dtNew.Columns.Add("Mobile", typeof(string));
            //dtNew.Columns.Add("Email", typeof(string));
            //dtNew.Columns.Add("BuildingId", typeof(string));
            //dtNew.Columns.Add("UserName", typeof(string));
            //dtNew.Columns.Add("Password", typeof(string));

            DataRow dr = dtNew.NewRow();

            if ((FirstName != "" && FirstName != null) && (LastName != "" && LastName != null) && (Mobile != "" && Mobile != null) && (BuildingId != 0 && BuildingId.ToString() != null) && (Password != "" && Password != null))
            {
                //check mobileno
                DataTable dtuserRecord = new DataTable();
                dtuserRecord = obj.CheckCustomerMobile(Mobile);
                int userRecords = dtuserRecord.Rows.Count;

                //check username
                //DataTable dtuserRecord1 = new DataTable();
                //dtuserRecord1 = obj.CheckCustomerUserName(UserName);
                //int userRecords1 = dtuserRecord1.Rows.Count;

                if (userRecords > 0)
                {
                    Status = "0";
                    dr["status"] = "Failed";
                    dr["error_msg"] = "Mobile No Already Exist";
                }
                //else if (userRecords1 > 0)
                //{
                //    Status = "0";
                //    dr["status"] = "Failed";
                //    dr["error_msg"] = "UserName already exist";
                //}
                else
                {
                    //check emailid validation
                    //if (!IsValidEmail(Email))
                    //{
                    //    dr["status"] = "Failed";
                    //    dr["error_msg"] = "Invalid Email Address";

                    //    dtNew.Rows.Add(dr);
                    //    jsonString = string.Empty;
                    //    jsonString = JsonConvert.SerializeObject(dtNew);
                    //    str1 = jsonString.ToString().Replace(@"[", "");
                    //    str2 = str1.ToString().Replace(@"]", "");
                    //    jsonString = str2;
                    //    var response1 = Request.CreateResponse(HttpStatusCode.OK);
                    //    response1.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                    //    return response1;
                    //}

                    //check mobileno validation
                    if (!IsValidMobileNo(Mobile))
                    {
                        dr["status"] = "Failed";
                        dr["error_msg"] = "Invalid MobileNo";

                        dtNew.Rows.Add(dr);
                        jsonString = string.Empty;
                        jsonString = JsonConvert.SerializeObject(dtNew);
                        str1 = jsonString.ToString().Replace(@"[", "");
                        str2 = str1.ToString().Replace(@"]", "");
                        jsonString = str2;
                        var response2 = Request.CreateResponse(HttpStatusCode.OK);
                        response2.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                        return response2;
                    }

                    //check password must be 8 digit
                    if (!CheckPassword(Password))
                    {
                        dr["status"] = "Failed";
                        dr["error_msg"] = "Password Should be 8 digit";

                        dtNew.Rows.Add(dr);
                        jsonString = string.Empty;
                        jsonString = JsonConvert.SerializeObject(dtNew);
                        str1 = jsonString.ToString().Replace(@"[", "");
                        str2 = str1.ToString().Replace(@"]", "");
                        jsonString = str2;
                        var response3 = Request.CreateResponse(HttpStatusCode.OK);
                        response3.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                        return response3;
                    }

                    int _min = 1000;
                    int _max = 9999;
                    Random _rdm = new Random();
                    int otp = _rdm.Next(_min, _max);

                    //string Msg = "%3C%23%3E Your OTP is:" + otp + " esWOXq8cybG";
                    string Msg = "Welcome to Milkyway India Family!!Your Milkyway India OTP is " + otp + " You can now order Milk, Dairy, Grocery at your doorstep Or Bill Payment with Cash back.";

                    string strUrl = "";
                    //india sms
                    ////strUrl = "https://apps.vibgyortel.in/client/api/sendmessage?apikey=dca6c57e6c6f4638&mobiles=" + Mobile + "&sms=" + Msg + "&senderid=Aruhat";
                    strUrl = "http://trans.magicsms.co.in/api/v4/?api_key=" + Helper.MagicSMSKey + "&method=sms&message=" + Msg + "&to=" + Mobile + "&sender=MLKYwy&template_id=" + Helper.MagicOTPTemplateID;

                    // Create a request object  
                    WebRequest request = HttpWebRequest.Create(strUrl);
                    // Get the response back  
                    try
                    {
                        HttpWebResponse responsesms = (HttpWebResponse)request.GetResponse();
                        Stream s = (Stream)responsesms.GetResponseStream();
                        StreamReader readStream = new StreamReader(s);
                        string dataString = readStream.ReadToEnd();
                        responsesms.Close();
                        s.Close();
                        readStream.Close();

                        dr["status"] = "success";
                        dr["error_msg"] = "OTP Send Successfully";

                        //check otp available or not by mobile
                        DataTable dtotp = obj.getOtpCustomerList(Mobile);
                        obj.Count = 0;
                        if (dtotp.Rows.Count > 0)
                        {
                            obj.Count = Convert.ToInt32(dtotp.Rows[0]["Count"]);
                            //update
                            int UpdateOtp = obj.CustomerupdateOtp(otp.ToString(), Mobile, obj.Count);
                        }
                        else
                        {
                            //Insert
                            int InsertOtp = obj.CustomerInsertOtp(otp.ToString(), Mobile);
                        }

                        dr["FirstName"] = FirstName;
                        dr["LastName"] = LastName;
                        dr["Mobile"] = Mobile;
                        dr["Email"] = Email;
                        // dr["UserName"] = UserName;
                        dr["Password"] = Password;
                    }
                    catch (Exception ex)
                    {

                    }


                }
            }
            else
            {
                dr["status"] = "Failed";
                dr["error_msg"] = "Please Fill Correct Details";
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

        [Route("api/CustomerApi/RegisterVerifiedCustomerTest/{FirstName?}/{LastName?}/{Mobile?}/{Email?}/{BuildingId?}/{FlatId?}/{Password?}/{otp?}/{Address}/{gpickloc}/{lat}/{lon}")]
        [HttpGet]
        public HttpResponseMessage RegisterVerifiedCustomerTest(string FirstName, string LastName, string Mobile, string Email, int BuildingId, int FlatId, string Password, string otp,string Address,string gpickloc,string lat,string lon)
        {   //

            Helper dHelper = new Helper();
            Customer obj = new Customer();

            string jsonString = null; string str1 = null; string str2 = null;

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            dtNew.Columns.Add("msg", typeof(string));
            dtNew.Columns.Add("Customerid", typeof(Int64));
            dtNew.Columns.Add("FirstName", typeof(string));
            dtNew.Columns.Add("LastName", typeof(string));
            dtNew.Columns.Add("Mobile", typeof(string));
            dtNew.Columns.Add("Email", typeof(string));
            dtNew.Columns.Add("BuildingId", typeof(string));
            //dtNew.Columns.Add("BlockNo", typeof(string));
            //dtNew.Columns.Add("FlatNo", typeof(string));
            dtNew.Columns.Add("SectorName", typeof(string));
            dtNew.Columns.Add("UserName", typeof(string));
            dtNew.Columns.Add("Password", typeof(string));

            DataRow dr = dtNew.NewRow();
            if ((FirstName != "" && FirstName != null) && (Mobile != "" && Mobile != null) && (Password != "" && Password != null) && (otp != "" && otp != null))
            {
                //check mobileno
                DataTable dtuserRecord = new DataTable();
                dtuserRecord = obj.CheckCustomerMobile(Mobile);
                int userRecords = dtuserRecord.Rows.Count;

                //check username
                //DataTable dtuserRecord1 = new DataTable();
                //dtuserRecord1 = obj.CheckCustomerUserName(UserName);
                //int userRecords1 = dtuserRecord1.Rows.Count;

                //check Flatno
                DataTable dtflatno = new DataTable();
                dtflatno = obj.CheckCustomerFlatNo(FlatId);
                int flatno = dtflatno.Rows.Count;

                if (userRecords > 0)
                {
                    dr["status"] = "Failed";
                    dr["error_msg"] = "Mobile No Already Exist";
                }
                //else if (userRecords1 > 0)
                //{
                //    dr["status"] = "Failed";
                //    dr["error_msg"] = "UserName already exist";
                //}
                else if (flatno > 0)
                {
                    dr["status"] = "Failed";
                    dr["error_msg"] = "Flat No already exist";
                }
                else
                {
                    //check emailid validation
                    //if (!IsValidEmail(Email))
                    //{
                    //    dr["status"] = "Failed";
                    //    dr["error_msg"] = "Invalid Email Address";

                    //    dtNew.Rows.Add(dr);
                    //    jsonString = string.Empty;
                    //    jsonString = JsonConvert.SerializeObject(dtNew);
                    //    str1 = jsonString.ToString().Replace(@"[", "");
                    //    str2 = str1.ToString().Replace(@"]", "");
                    //    jsonString = str2;
                    //    var response1 = Request.CreateResponse(HttpStatusCode.OK);
                    //    response1.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                    //    return response1;
                    //}

                    //check mobileno validation
                    if (!IsValidMobileNo(Mobile))
                    {
                        dr["status"] = "Failed";
                        dr["error_msg"] = "Invalid MobileNo";

                        dtNew.Rows.Add(dr);
                        jsonString = string.Empty;
                        jsonString = JsonConvert.SerializeObject(dtNew);
                        str1 = jsonString.ToString().Replace(@"[", "");
                        str2 = str1.ToString().Replace(@"]", "");
                        jsonString = str2;
                        var response2 = Request.CreateResponse(HttpStatusCode.OK);
                        response2.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                        return response2;
                    }

                    //check password must be 8 digit
                    if (!CheckPassword(Password))
                    {
                        dr["status"] = "Failed";
                        dr["error_msg"] = "Password Should be 8 digit";

                        dtNew.Rows.Add(dr);
                        jsonString = string.Empty;
                        jsonString = JsonConvert.SerializeObject(dtNew);
                        str1 = jsonString.ToString().Replace(@"[", "");
                        str2 = str1.ToString().Replace(@"]", "");
                        jsonString = str2;
                        var response3 = Request.CreateResponse(HttpStatusCode.OK);
                        response3.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                        return response3;
                    }



                    //check mobile store or not
                    DataTable CheckMobile = obj.getOtpCustomerList(Mobile);
                    if (CheckMobile.Rows.Count == 0)
                    {
                        dr["status"] = "Failed";
                        dr["error_msg"] = "Unauthorized Access";

                        dtNew.Rows.Add(dr);
                        jsonString = string.Empty;
                        jsonString = JsonConvert.SerializeObject(dtNew);
                        str1 = jsonString.ToString().Replace(@"[", "");
                        str2 = str1.ToString().Replace(@"]", "");
                        jsonString = str2;
                        var response4 = Request.CreateResponse(HttpStatusCode.OK);
                        response4.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                        return response4;
                    }

                    int result = 0;

                    //check otp and mobile
                    DataTable CheckuserOtp = obj.CheckCustomerOtp(Mobile, otp);
                    if (CheckuserOtp.Rows.Count == 0)
                    {
                        //insert
                        obj.Id = 0;
                        obj.FirstName = FirstName;
                        obj.LastName = LastName;
                        obj.MobileNo = Mobile;
                        obj.Email = Email;
                        obj.BuildingId = BuildingId;
                        obj.UserName = Mobile;
                        obj.Password = Password;
                        obj.FlatId = FlatId;
                        obj.SectorId = 0;
                        obj.Address = Address.ToString();
                        obj.gpickloc = gpickloc.ToString();
                        obj.lat = lat.ToString();
                        obj.lon = lon.ToString();


                        obj.ReferralCode = dHelper.GenerateCustomerReferalCode();
                        var referralID = 123;

                        
                     
                        if (referralID > 0)
                            obj.ReferralID = referralID;
                        result = obj.InsertCustomer(obj);
                    }
                    else
                    {
                        dr["status"] = "Failed";
                        dr["error_msg"] = "OTP not match";

                        dtNew.Rows.Add(dr);
                        jsonString = string.Empty;
                        jsonString = JsonConvert.SerializeObject(dtNew);
                        str1 = jsonString.ToString().Replace(@"[", "");
                        str2 = str1.ToString().Replace(@"]", "");
                        jsonString = str2;
                        var response5 = Request.CreateResponse(HttpStatusCode.OK);
                        response5.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                        return response5;
                    }

                    if (result > 0)
                    {

                        //notification
                        string title = "Registration";
                        string content = "Congratulations!!You are now part of Milkyway Family To activate our service kindly update your wallet balance and choose desired subscription and enjoy our daily services.";
                        string type = "Notification";//PRODUCT   NEWS_INFO  ORDER
                        string obj_id = "1";
                        string image = "";
                        int appnotification = AppNotification(result, title, content, type, obj_id, image);

                        dr["status"] = "Success";
                        dr["error_msg"] = "Registered Successfully";

                        DataTable dtUser = new DataTable();
                        dtUser = obj.NewCustomer(Mobile, Password, Mobile);
                        if (dtUser.Rows.Count > 0)
                        {
                            dr["msg"] = "Find User";
                            dr["Customerid"] = Convert.ToInt64(dtUser.Rows[0]["Id"]);
                            dr["FirstName"] = dtUser.Rows[0]["FirstName"].ToString().Trim();
                            dr["LastName"] = dtUser.Rows[0]["LastName"].ToString().Trim();
                            dr["Mobile"] = dtUser.Rows[0]["MobileNo"].ToString().Trim();
                            dr["Email"] = dtUser.Rows[0]["Email"].ToString().Trim();
                            dr["SectorName"] = dtUser.Rows[0]["SectorName"].ToString().Trim();
                            dr["BuildingId"] = dtUser.Rows[0]["BuildingId"].ToString().Trim();
                            //dr["BlockNo"] = dtUser.Rows[0]["BlockNo"].ToString().Trim();
                            //dr["FlatNo"] = dtUser.Rows[0]["FlatNo"].ToString().Trim();
                            dr["UserName"] = dtUser.Rows[0]["UserName"].ToString().Trim();
                            dr["Password"] = dtUser.Rows[0]["Password"].ToString().Trim();
                        }
                    }
                    else
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Registration Failed";
                    }
                }
            }
            else
            {
                dr["status"] = "Failed";
                dr["error_msg"] = "Please Fill Correct Details";
            }

            dtNew.Rows.Add(dr);
            jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dtNew);
            str1 = jsonString.ToString().Replace(@"[", "");
            str2 = str1.ToString().Replace(@"]", "");
            jsonString = str2;
            var response6 = Request.CreateResponse(HttpStatusCode.OK);
            response6.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return response6;
        }

        public bool IsValidEmail(string Emailid)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(Emailid);
                return addr.Address == Emailid;
            }
            catch
            {
                return false;
            }
        }

        public bool IsValidMobileNo(string MobileNo)
        {
            try
            {
                Regex rx = new Regex("^[0-9]{10}");
                return rx.Match(MobileNo).Success;
            }
            catch
            {
                return false;
            }

        }

        public bool CheckPassword(string password)
        {
            if (password.Length < 1) return false;
            else return true;
        }

        [Route("api/CustomerApi/Login/{username?}/{password?}/{fcm_token?}")]
        [HttpGet]
        public HttpResponseMessage Login(string username, string password, string fcm_token = "")
        {
            Customer objlogin = new Customer();

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
            dtNew.Columns.Add("referralcode", typeof(string));
            dtNew.Columns.Add("photo", typeof(string));
            dtNew.Columns.Add("buildingname", typeof(string));
            dtNew.Columns.Add("address", typeof(string));
            dtNew.Columns.Add("flatno", typeof(string));
            dtNew.Columns.Add("SectorName", typeof(string));
            dtNew.Columns.Add("SectorId", typeof(int));
            dtNew.Columns.Add("BuildingId", typeof(int));
            dtNew.Columns.Add("FlatId", typeof(int));
            DataRow dr = dtNew.NewRow();

            if (username != null)
            {
                DataTable dtUsername = new DataTable();
                dtUsername = objlogin.CheckCustomerUserName(username);

                DataTable dtpassword = new DataTable();
                dtpassword = objlogin.Customerlogin(username, password);

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
                    int customerId = Convert.ToInt32(dtpassword.Rows[0]["Id"]);
                    var _customer = objlogin.BindCustomer(customerId);

                    if (_customer.Rows.Count > 0)
                    {
                        dr["userid"] = Convert.ToInt32(dtpassword.Rows[0]["Id"]);
                        dr["firstname"] = _customer.Rows[0]["FirstName"].ToString().Trim();
                        dr["lastname"] = _customer.Rows[0]["LastName"].ToString().Trim();
                        dr["mobileno"] = _customer.Rows[0]["MobileNo"].ToString().Trim();
                        if (!string.IsNullOrEmpty(_customer.Rows[0]["Email"].ToString()))
                            dr["email"] = _customer.Rows[0]["Email"].ToString().Trim();
                        else
                            dr["email"] = "";
                        if (!string.IsNullOrEmpty(_customer.Rows[0]["Photo"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(_customer.Rows[0]["Photo"].ToString().Trim());
                            dr["photo"] = Helper.PhotoFolderPath + "/image/customer/" + encoded;
                        }
                        else
                            dr["photo"] = "";
                        if (!string.IsNullOrEmpty(_customer.Rows[0]["SectorId"].ToString()))
                            dr["SectorId"] = Convert.ToInt32(_customer.Rows[0]["SectorId"]);
                        else
                            dr["SectorId"] = 0;
                        if (!string.IsNullOrEmpty(_customer.Rows[0]["ReferralCode"].ToString()))
                            dr["referralcode"] = _customer.Rows[0]["ReferralCode"];
                        else
                            dr["referralcode"] = 0;
                        if (!string.IsNullOrEmpty(_customer.Rows[0]["Address"].ToString()))
                            dr["address"] = _customer.Rows[0]["address"];
                        else
                            dr["address"] = 0;

                        if (!string.IsNullOrEmpty(_customer.Rows[0]["SectorId"].ToString()))
                        {
                            Sector _sector = new Sector();
                            var sector = _sector.getSectorList(Convert.ToInt32(_customer.Rows[0]["SectorId"]));
                            if (sector.Rows.Count > 0)
                                dr["SectorName"] = sector.Rows[0]["SectorName"];
                        }

                        dr["buildingname"] = "";
                        dr["flatno"] = "";

                        if (fcm_token != null && fcm_token != "")
                        {
                            con.Open();
                            SqlCommand cmd51 = new SqlCommand("update tbl_Customer_master set fcm_token='" + fcm_token + "' where id=" + Convert.ToInt64(dtpassword.Rows[0]["Id"]), con);
                            cmd51.ExecuteNonQuery();
                            con.Close();
                        }
                    }
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

        //public IHttpActionResult RegisterCustomerTest(string FirstName, string LastName, string Mobile, string Email, int BuildingId, int FlatId, string Password)
        // [System.Web.Http.HttpPost]
        // [Route("api/CustomerApi/RegisterCustomer")]
        public IHttpActionResult RegisterCustomer(Customer item)
        {
            string Status = ""; string jsonString = null; string str1 = null; string str2 = null;

            Customer obj = new Customer();

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            //dtNew.Columns.Add("FirstName", typeof(string));
            //dtNew.Columns.Add("LastName", typeof(string));
            //dtNew.Columns.Add("Mobile", typeof(string));
            //dtNew.Columns.Add("Email", typeof(string));
            //dtNew.Columns.Add("BuildingId", typeof(string));
            //dtNew.Columns.Add("UserName", typeof(string));
            //dtNew.Columns.Add("Password", typeof(string));

            DataRow dr = dtNew.NewRow();

            //if ((item.FirstName != "" && item.FirstName != null) && (item.LastName != "" && item.LastName != null) && (item.MobileNo != "" && item.MobileNo != null)
            //    && (item.BuildingId != 0 && item.BuildingId.ToString() != null) && (item.Password != "" && item.Password != null))
            if ((item.FirstName != "" && item.FirstName != null) && (item.LastName != "" && item.LastName != null) && (item.MobileNo != "" && item.MobileNo != null)
            && (item.Password != "" && item.Password != null))
            {
                //check mobileno
                DataTable dtuserRecord = new DataTable();
                dtuserRecord = obj.CheckCustomerMobile(item.MobileNo);
                int userRecords = dtuserRecord.Rows.Count;

                //check Flatno
                DataTable dtflatno = new DataTable();
                dtflatno = obj.CheckCustomerFlatNo(item.FlatId);
                int flatno = dtflatno.Rows.Count;

                //check username
                //DataTable dtuserRecord1 = new DataTable();
                //dtuserRecord1 = obj.CheckCustomerUserName(UserName);
                //int userRecords1 = dtuserRecord1.Rows.Count;

                if (userRecords > 0)
                {
                    Status = "0";
                    dr["status"] = "Failed";
                    dr["error_msg"] = "Mobile No Already Exist";
                }
                //else if (flatno > 0)
                //{
                //    Status = "0";
                //    dr["status"] = "Failed";
                //    dr["error_msg"] = "Flat No already exist";
                //}
                else
                {
                    //check emailid validation
                    //if (!IsValidEmail(item.Email))
                    //{
                    //    dr["status"] = "Failed";
                    //    dr["error_msg"] = "Invalid Email Address";

                    //    dtNew.Rows.Add(dr);
                     
                    //    return Ok(dtNew);
                    //}

                    //check mobileno validation
                    if (!IsValidMobileNo(item.MobileNo))
                    {
                        dr["status"] = "Failed";
                        dr["error_msg"] = "Invalid MobileNo";

                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }

                    //check password must be 8 digit
                    if (!CheckPassword(item.Password))
                    {
                        dr["status"] = "Failed";
                        dr["error_msg"] = "Password Should be 8 digit";

                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }

                    int _min = 1000;
                    int _max = 9999;
                    Random _rdm = new Random();
                    int otp = _rdm.Next(_min, _max);

                    //string Msg = "%3C%23%3E Your OTP is:" + otp ;
                    string Msg = "Welcome to Milkyway India Family!!Your Milkyway India OTP is " + otp + " You can now order Milk, Dairy, Grocery at your doorstep Or Bill Payment with Cash back.";
                    // Get the response back  
                    try
                    {
                      Helper.SendOTPSMS(Msg, item.MobileNo);
                        dr["status"] = "success";
                        dr["error_msg"] = "Otp Send Successfully";

                        //check otp available or not by mobile
                        DataTable dtotp = obj.getOtpCustomerList(item.MobileNo);
                        obj.Count = 0;
                        if (dtotp.Rows.Count > 0)
                        {
                            obj.Count = Convert.ToInt32(dtotp.Rows[0]["Count"]) + 1;
                            //update
                            int UpdateOtp = obj.CustomerupdateOtp(otp.ToString(), item.MobileNo, obj.Count);
                        }
                        else
                        {
                            //Insert
                            int InsertOtp = obj.CustomerInsertOtp(otp.ToString(), item.MobileNo);
                        }

                        dr["FirstName"] = item.FirstName;
                        dr["LastName"] = item.LastName;
                        dr["Mobile"] = item.MobileNo;
                        dr["Email"] = item.Email;
                        dr["Address"] = item.Address;
                        // dr["UserName"] = UserName;
                        dr["Password"] = item.Password;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            else
            {
                dr["status"] = "Failed";
                dr["error_msg"] = "Please Fill Correct Details";
            }
            dtNew.Rows.Add(dr);
            jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dtNew);
           // str1 = jsonString.ToString().Replace(@"[", "");
           // str2 = str1.ToString().Replace(@"]", "");
            //jsonString = str2;
            //  return jsonString;
            // var response = Request.CreateResponse(HttpStatusCode.OK);
            // response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return Ok(dtNew);
        }

        public IHttpActionResult RegisterVerifiedCustomer(Customer item)
        {   //
            Customer obj = new Customer();

            string jsonString = null; string str1 = null; string str2 = null;

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            dtNew.Columns.Add("msg", typeof(string));
            dtNew.Columns.Add("Customerid", typeof(Int64));
            dtNew.Columns.Add("FirstName", typeof(string));
            dtNew.Columns.Add("LastName", typeof(string));
            dtNew.Columns.Add("Mobile", typeof(string));
            dtNew.Columns.Add("Email", typeof(string));
            //dtNew.Columns.Add("BuildingId", typeof(string));
           // dtNew.Columns.Add("BlockNo", typeof(string));
           // dtNew.Columns.Add("FlatNo", typeof(string));
            dtNew.Columns.Add("SectorName", typeof(string));

            //dtNew.Columns.Add("UserName", typeof(string));
            dtNew.Columns.Add("Password", typeof(string));
            dtNew.Columns.Add("Address", typeof(string));


            dtNew.Columns.Add("gpickloc", typeof(string));
            dtNew.Columns.Add("lat", typeof(string));
            dtNew.Columns.Add("lon", typeof(string));

            // dtNew.Columns.Add("fcmtoken", typeof(string));

            DataRow dr = dtNew.NewRow();
            if ((item.FirstName != "" && item.FirstName != null) && (item.MobileNo != "" && item.MobileNo != null) && (item.Password != "" && item.Password != null) && (item.OTP != "" && item.OTP != null))
            {

                //check mobileno
                DataTable dtuserRecord = new DataTable();
                dtuserRecord = obj.CheckCustomerMobile(item.MobileNo);
                int userRecords = dtuserRecord.Rows.Count;

                //check Flatno
                DataTable dtflatno = new DataTable();
                dtflatno = obj.CheckCustomerFlatNo(item.FlatId);
                int flatno = dtflatno.Rows.Count;

                if (userRecords > 0)
                {
                    dr["status"] = "Failed";
                    dr["error_msg"] = "Mobile No Already Exist";
                }
                //else if (string.IsNullOrEmpty(item.SectorId.ToString()))
                //{
                //    dr["status"] = "Failed";
                //    dr["error_msg"] = "Please select sector...";
                //}
                //else if (flatno > 0)
                //{
                //    dr["status"] = "Failed";
                //    dr["error_msg"] = "Flat No already exist";
                //}
                else
                {
                    //check emailid validation
                    //if (!IsValidEmail(item.Email))
                    //{
                    //    dr["status"] = "Failed";
                    //    dr["error_msg"] = "Invalid Email Address";
                    //    dtNew.Rows.Add(dr);
                    //    return Ok(dtNew);
                    //}

                    //check mobileno validation
                    if (!IsValidMobileNo(item.MobileNo))
                    {
                        dr["status"] = "Failed";
                        dr["error_msg"] = "Invalid MobileNo";

                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }

                    //check password must be 8 digit
                    if (!CheckPassword(item.Password))
                    {
                        dr["status"] = "Failed";
                        dr["error_msg"] = "Password Should be 8 digit";

                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }

                    //check mobile store or not
                    DataTable CheckMobile = obj.getOtpCustomerList(item.MobileNo);
                    if (CheckMobile.Rows.Count == 0)
                    {
                        dr["status"] = "Failed";
                        dr["error_msg"] = "Unauthorized Access";
                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }

                    int result = 0;

                    //check otp and mobile
                    DataTable CheckuserOtp = obj.CheckCustomerOtp(item.MobileNo, item.OTP);
                    if (CheckuserOtp.Rows.Count > 0)
                    {
                        Helper dHelper = new Helper();
                        //insert
                        obj.Id = 0;
                        obj.FirstName = item.FirstName;
                        obj.LastName = item.LastName;
                        obj.MobileNo = item.MobileNo;
                        obj.Email = item.Email;
                        obj.BuildingId = item.BuildingId;
                        obj.UserName = item.MobileNo;
                        obj.Password = item.Password;
                        obj.FlatId = item.FlatId;
                        obj.SectorId = 0;
                        obj.fcm_token = item.fcm_token;
                        obj.Address = item.Address;
                        obj.ReferralCode = dHelper.GenerateCustomerReferalCode();
                        var referralID = dHelper.GetCustomerIDByReferralCode(item.ReferralCode);

                        obj.gpickloc = item.gpickloc;
                        obj.lat = item.lat;
                        obj.lon = item.lon;
                        if (referralID > 0)
                            obj.ReferralID = referralID;
                        result = obj.InsertCustomer(obj);
                        //result = 193;
                    }
                    else
                    {
                        dr["status"] = "Failed";
                        dr["error_msg"] = "OTP not match";

                        dtNew.Rows.Add(dr);
                        return Ok(dtNew);
                    }

                    if (result > 0)
                    {
                        //notification
                        string title = "Registration";
                        string content = "Congratulations!!You are now part of Milkyway Family To activate our service kindly update your wallet balance and choose desired subscription and enjoy our daily services.";
                        string type = "Notification";//PRODUCT   NEWS_INFO  ORDER
                        string obj_id = "1";
                        string image = "";
                        int appnotification = AppNotification(result, title, content, type, obj_id, image);
                        //send sms
                        SendSMS(item.MobileNo);
                        dr["status"] = "Success";
                        dr["error_msg"] = "Registered Successfully";

                        DataTable dtUser = new DataTable();
                        dtUser = obj.NewCustomer(item.MobileNo, item.Password, item.MobileNo);
                        if (dtUser.Rows.Count > 0)
                        {
                            dr["msg"] = "Find User";
                            dr["Customerid"] = Convert.ToInt64(dtUser.Rows[0]["Id"]);
                            dr["FirstName"] = dtUser.Rows[0]["FirstName"].ToString().Trim();
                            dr["LastName"] = dtUser.Rows[0]["LastName"].ToString().Trim();
                            dr["Mobile"] = dtUser.Rows[0]["MobileNo"].ToString().Trim();
                            if (!string.IsNullOrEmpty(dtUser.Rows[0]["Email"].ToString()))
                                dr["Email"] = dtUser.Rows[0]["Email"].ToString().Trim();
                            else
                                dr["Email"] = "";
                            dr["SectorName"] = dtUser.Rows[0]["SectorName"].ToString().Trim();
                            //dr["BuildingName"] = dtUser.Rows[0]["BuildingName"].ToString().Trim();
                            //dr["BlockNo"] = dtUser.Rows[0]["BlockNo"].ToString().Trim();
                           // dr["FlatNo"] = dtUser.Rows[0]["FlatNo"].ToString().Trim();
                            //dr["UserName"] = dtUser.Rows[0]["UserName"].ToString().Trim();
                            dr["Password"] = dtUser.Rows[0]["Password"].ToString().Trim();
                            dr["Address"] = dtUser.Rows[0]["Address"].ToString().Trim();
                        }
                    }
                    else
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Registration Failed";
                    }
                }
            }
            else
            {
                dr["status"] = "Failed";
                dr["error_msg"] = "Please Fill Correct Details";
            }

            dtNew.Rows.Add(dr);
            //jsonString = string.Empty;
            //jsonString = JsonConvert.SerializeObject(dtNew);
            //str1 = jsonString.ToString().Replace(@"[", "");
            //str2 = str1.ToString().Replace(@"]", "");
            //jsonString = str2;
            //var response6 = Request.CreateResponse(HttpStatusCode.OK);
            //response6.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return Ok(dtNew);
        }

        private void SendSMS(string mobileNo)
        {
            if ((mobileNo != "" && mobileNo != null))
            {
                //string Msg = "Hello, " + Name + ". Your OTP is:" + otp + " For Registartion.";
                string Msg = "Congratulations!!You are now part of Milkyway Family To activate our service kindly update your wallet balance and choose desired subscription and enjoy our daily services.";

                string strUrl = "";
                //india sms
                ////strUrl = "https://apps.vibgyortel.in/client/api/sendmessage?apikey=dca6c57e6c6f4638&mobiles=" + mobileNo + "&sms=" + Msg + "&senderid=Aruhat";
                //new sms link
                strUrl = "http://trans.magicsms.co.in/api/v4/?api_key=" + Helper.MagicSMSKey + "&method=sms&message=" + Msg + "&to=" + mobileNo + "&sender=MLKYwy";
                // Create a request object  
                WebRequest request = HttpWebRequest.Create(strUrl);
                // Get the response back  
                try
                {
                    HttpWebResponse responsesms = (HttpWebResponse)request.GetResponse();
                    Stream s = (Stream)responsesms.GetResponseStream();
                    StreamReader readStream = new StreamReader(s);
                    string dataString = readStream.ReadToEnd();
                    responsesms.Close();
                    s.Close();
                    readStream.Close();
                }
                catch (Exception ex)
                {
                }
            }
        }

        [Route("api/CustomerApi/CustomerList")]
        [HttpGet]
        public HttpResponseMessage CustomerList() //JsonResult
        {
            Customer obj = new Customer();
            DataTable dtNew = new DataTable();

            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.BindCustomer(null);
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                dtNew.Columns.Add("id", typeof(Int64));
                dtNew.Columns.Add("firstname", typeof(string));
                dtNew.Columns.Add("lastname", typeof(string));
                dtNew.Columns.Add("mobile", typeof(string));
                dtNew.Columns.Add("email", typeof(string));
                dtNew.Columns.Add("password", typeof(string));

                for (int i = 0; i < dtprodRecord.Rows.Count; i++)
                {
                    //Status = "0";
                    DataRow dr = dtNew.NewRow();
                    // dr["status"] = "Success";
                    dr["id"] = dtprodRecord.Rows[i]["Id"].ToString().Trim();
                    dr["firstname"] = dtprodRecord.Rows[i]["FirstName"].ToString().Trim();
                    dr["lastname"] = dtprodRecord.Rows[i]["LastName"].ToString().Trim();
                    dr["mobile"] = dtprodRecord.Rows[i]["MobileNo"].ToString().Trim();
                    dr["email"] = dtprodRecord.Rows[i]["Email"].ToString().Trim();
                    dr["password"] = dtprodRecord.Rows[i]["Password"].ToString().Trim();

                    dtNew.Rows.Add(dr);
                }
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["customer"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""customer"":" + dict["customer"];
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
                dict["customer"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""customer"":" + dict["customer"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }

        public IHttpActionResult ReSendOTP(Customer item)
        {
            string jsonString = null; string str1 = null; string str2 = null;
            Customer obj = new Customer();
            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("msg", typeof(string));
            DataRow dr = dtNew.NewRow();
            if ((item.MobileNo != "" && item.MobileNo != null))
            {
                int _min = 1000;
                int _max = 9999;
                Random _rdm = new Random();
                int otp = _rdm.Next(_min, _max);

                //string Msg = "Hello, " + Name + ". Your OTP is:" + otp + " For Registartion.";
                //string Msg = "%3C%23%3E Your OTP is:" + otp;
                string Msg = "Welcome to Milkyway India Family!!Your Milkyway India OTP is " + otp + "You can now order Milk, Dairy, Grocery at your doorstep Or Bill Payment with Cash back.";

                //string strUrl = "";
                ////india sms
                ////// strUrl = "https://apps.vibgyortel.in/client/api/sendmessage?apikey=dca6c57e6c6f4638&mobiles=" + item.MobileNo + "&sms=" + Msg + "&senderid=Aruhat";
                //strUrl = "http://trans.magicsms.co.in/api/v4/?api_key=A26ab1931dd8a93d90165ae7abd912d41&method=sms&message=" + Msg + "&to=" + item.MobileNo + "&sender=MLKYwy";
                //// Create a request object  
                //WebRequest request = HttpWebRequest.Create(strUrl);
                // Get the response back  
                try
                {
                    Helper.SendOTPSMS(Msg, item.MobileNo);

                    //check otp available or not by mobile
                    DataTable dtotp = obj.getOtpCustomerList(item.MobileNo);
                    obj.Count = 0;
                    if (dtotp.Rows.Count > 0)
                    {
                        obj.Count = Convert.ToInt32(dtotp.Rows[0]["Count"]) + 1;
                        //update
                        int UpdateOtp = obj.CustomerupdateOtp(otp.ToString(), item.MobileNo, obj.Count);
                    }
                    else
                    {
                        //Insert
                        int InsertOtp = obj.CustomerInsertOtp(otp.ToString(), item.MobileNo);
                    }

                    dr["status"] = "success";
                    dr["msg"] = "OTP Send Successfully";
                }
                catch (Exception ex)
                {
                    dr["status"] = "Failed";
                    dr["msg"] = "OTP Sending Failure";
                }
            }
            else
            {
                dr["status"] = "Failed";
                dr["msg"] = "OTP Sending Failure";
            }
            dtNew.Rows.Add(dr);
            jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dtNew);
            str1 = jsonString.ToString().Replace(@"[", "");
            str2 = str1.ToString().Replace(@"]", "");
            jsonString = str2;
            return Ok(dtNew);
        }

        [Route("api/CustomerApi/ForgotPassword/{username?}/{DeviceId?}")]
        [System.Web.Http.HttpGet]

        public IHttpActionResult ForgotPassword(Customer item)
        {
            Customer obj = new Customer();

            DataTable dtNew = new DataTable();
            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("message", typeof(string));

            DataRow dr = dtNew.NewRow();
            dr["status"] = "failed";
            dr["message"] = "message failed";

            if (!string.IsNullOrEmpty(item.MobileNo))
            {
                DataTable dt = new DataTable();

                dt = obj.CheckPwdFromUserName(item.MobileNo);

                if (dt.Rows.Count > 0)
                {
                    string uid = dt.Rows[0]["Id"].ToString();
                    //string Name = dt.Rows[0]["Name"].ToString();
                    string MobileNo = dt.Rows[0]["MobileNo"].ToString();
                    if ((item.MobileNo != "" && item.MobileNo != null))
                    {
                        int _min = 1000;
                        int _max = 9999;
                        Random _rdm = new Random();
                        int otp = _rdm.Next(_min, _max);

                        //string Msg = "Hello, " + Name + ". Your OTP is:" + otp + " For Registartion.";
                        //string Msg = "%3C%23%3E Your OTP is:" + otp;
                        string Msg = "Welcome to Milkyway India Family!!Your Milkyway India OTP is " + otp + " You can now order Milk, Dairy, Grocery at your doorstep Or Bill Payment with Cash back.";

                        //string strUrl = "";
                        ////india sms
                        //////strUrl = "https://apps.vibgyortel.in/client/api/sendmessage?apikey=dca6c57e6c6f4638&mobiles=" + item.MobileNo + "&sms=" + Msg + "&senderid=Aruhat";
                        //strUrl = "http://trans.magicsms.co.in/api/v4/?api_key=A26ab1931dd8a93d90165ae7abd912d41&method=sms&message=" + Msg + "&to=" + item.MobileNo + "&sender=MLKYwy";
                        //// Create a request object  
                        //WebRequest request = HttpWebRequest.Create(strUrl);
                        // Get the response back  
                        try
                        {
                            Helper.SendOTPSMS(Msg, item.MobileNo);
                            //check otp available or not by mobile
                            DataTable dtotp = obj.getOtpCustomerList(item.MobileNo);
                            obj.Count = 0;
                            if (dtotp.Rows.Count > 0)
                            {
                                obj.Count = Convert.ToInt32(dtotp.Rows[0]["Count"]) + 1;
                                //update
                                int UpdateOtp = obj.CustomerupdateOtp(otp.ToString(), item.MobileNo, obj.Count);
                            }
                            else
                            {
                                //Insert
                                int InsertOtp = obj.CustomerInsertOtp(otp.ToString(), item.MobileNo);
                            }

                            dr["status"] = "success";
                            dr["message"] = "OTP Send Successfully";
                        }
                        catch (Exception ex)
                        {
                            dr["status"] = "Failed";
                            dr["message"] = "OTP Sending Failure";
                        }
                    }

                }
                else
                {
                    dr["status"] = "failed";
                    dr["message"] = "User Not Exist";
                }
            }
            else
            {
                dr["status"] = "failed";
                dr["message"] = "Please Enter Valid Data";
            }

            dtNew.Rows.Add(dr);
            string jsonstring = string.Empty;
            jsonstring = JsonConvert.SerializeObject(dtNew);
            jsonstring = jsonstring.Remove(jsonstring.Length - 1);
            jsonstring = jsonstring.Remove(0, 1);
            return Ok(dtNew);
        }





        [Route("api/CustomerApi/UpdatePassword/{UserName}/{OTP}/{Password}")]
        [HttpGet]
        public IHttpActionResult UpdatePassword(Customer item)
        {
            Customer obj = new Customer();

            string jsonstring = null;

            DataTable dt = new DataTable();
            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("message", typeof(string));

            DataRow dr = dtNew.NewRow();
            dr["status"] = "failed";
            dr["message"] = "";

            if (!string.IsNullOrEmpty(item.UserName) && !string.IsNullOrEmpty(item.OTP) && !string.IsNullOrEmpty(item.Password))
            {
                //check otp and mobile
                DataTable CheckuserOtp = obj.CheckCustomerOtp(item.UserName, item.OTP);
                if (CheckuserOtp.Rows.Count > 0)
                {
                    //username ckeck
                    dt = obj.CheckPwdFromUserName(item.UserName);

                    if (dt.Rows.Count > 0)
                    {
                        string uid = dt.Rows[0]["Id"].ToString();

                        //update password
                        int updPwd = obj.UpdateCustomerPwd(Convert.ToInt32(uid), item.Password);

                        if (updPwd > 0)
                        {
                            dr["status"] = "success";
                            dr["message"] = "Password Updated Successfully";
                        }
                    }
                    else
                    {
                        dr["status"] = "Fail";
                        dr["message"] = "UserName/MobileNo not exist";
                    }
                }
                else
                {
                    dr["status"] = "failed";
                    dr["message"] = "Not Valid OTP";
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



        public IHttpActionResult EditCustomer(Customer item)

        {   //

            Customer obj = new Customer();
            int result = 0;
            string jsonString = null; string str1 = null; string str2 = null;

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            dtNew.Columns.Add("msg", typeof(string));
            dtNew.Columns.Add("Customerid", typeof(Int64));
            dtNew.Columns.Add("FirstName", typeof(string));
            dtNew.Columns.Add("LastName", typeof(string));
            dtNew.Columns.Add("Mobile", typeof(string));
            dtNew.Columns.Add("Email", typeof(string));
            dtNew.Columns.Add("BuildingName", typeof(string));
            dtNew.Columns.Add("BlockNo", typeof(string));
            dtNew.Columns.Add("FlatNo", typeof(string));
            dtNew.Columns.Add("SectorName", typeof(string));
            dtNew.Columns.Add("Address", typeof(string));
            // dtNew.Columns.Add("UserName", typeof(string));
            dtNew.Columns.Add("Password", typeof(string));

            DataRow dr = dtNew.NewRow();
            if ((item.FirstName != "" && item.FirstName != null) && (item.MobileNo != "" && item.MobileNo != null))
            {
                //check emailid validation
                if (!IsValidEmail(item.Email))
                {
                    dr["status"] = "Failed";
                    dr["error_msg"] = "Invalid Email Address";
                    dtNew.Rows.Add(dr);
                    return Ok(dtNew);
                }

                //check mobileno validation
                if (!IsValidMobileNo(item.MobileNo))
                {
                    dr["status"] = "Failed";
                    dr["error_msg"] = "Invalid MobileNo";
                    dtNew.Rows.Add(dr);
                    return Ok(dtNew);
                }

                //check mobileno
                DataTable dtuserRecord = new DataTable();
                dtuserRecord = obj.CheckCustomerMobile(item.MobileNo);
                int userRecords = dtuserRecord.Rows.Count;

                //check Flatno
                //DataTable dtflatno = new DataTable();
                //dtflatno = obj.CheckCustomerFlatNo(item.FlatId);
                //int flatno = dtflatno.Rows.Count;
                if (userRecords > 0)
                {
                    if (item.Id != Convert.ToInt32(dtuserRecord.Rows[0]["Id"]))
                    {
                        dr["status"] = "Failed";
                        dr["error_msg"] = "Mobile No Already Exist";
                    }
                    else
                    {
                        DataTable dtCheckMobile = new DataTable();
                        dtCheckMobile = obj.BindCustomer(item.Id);
                        if (dtCheckMobile.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtCheckMobile.Rows[0]["MobileNo"].ToString()))
                            {
                                if (item.MobileNo == dtCheckMobile.Rows[0]["MobileNo"].ToString())
                                {
                                    //Update
                                    obj.Id = item.Id;
                                    obj.FirstName = item.FirstName;
                                    obj.LastName = item.LastName;
                                    obj.MobileNo = item.MobileNo;
                                    obj.Email = item.Email;
                                    obj.BuildingId = item.BuildingId;
                                    obj.UserName = item.MobileNo;
                                    obj.Address = item.Address;
                                    //  obj.Password = item.Password;
                                    obj.FlatId = item.FlatId;
                                    result = obj.MobileUpdateCustomer(obj);
                                }
                                else
                                {
                                    int _min = 1000;
                                    int _max = 9999;
                                    Random _rdm = new Random();
                                    int otp = _rdm.Next(_min, _max);
                                    string Msg = "Welcome to Milkyway India Family!!Your Milkyway India OTP is " + otp + " You can now order Milk, Dairy, Grocery at your doorstep Or Bill Payment with Cash back.";

                                    try
                                    {
                                        Helper.SendOTPSMS(Msg, item.MobileNo);
                                        //check otp available or not by mobile
                                        DataTable dtotp = obj.getOtpCustomerList(item.MobileNo);
                                        obj.Count = 0;
                                        if (dtotp.Rows.Count > 0)
                                        {
                                            obj.Count = Convert.ToInt32(dtotp.Rows[0]["Count"]) + 1;
                                            int UpdateOtp = obj.CustomerupdateOtp(otp.ToString(), item.MobileNo, obj.Count);//update
                                        }
                                        else
                                        {
                                            int InsertOtp = obj.CustomerInsertOtp(otp.ToString(), item.MobileNo);//Insert
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    result = 2;
                                }
                            }

                        }

                        if (result == 2)
                        {
                            dr["status"] = "success";
                            dr["error_msg"] = "OTP Send Successfully";
                        }
                        else if (result > 0)
                        {
                            dr["status"] = "Success";
                            dr["error_msg"] = "Update data Successfully";

                            DataTable dtUser = new DataTable();
                            dtUser = obj.NewCustomer(item.MobileNo, item.Password, item.MobileNo);
                            if (dtUser.Rows.Count > 0)
                            {
                                dr["msg"] = "Find User";
                                dr["Customerid"] = Convert.ToInt64(dtUser.Rows[0]["Id"]);
                                dr["FirstName"] = dtUser.Rows[0]["FirstName"].ToString().Trim();
                                dr["LastName"] = dtUser.Rows[0]["LastName"].ToString().Trim();
                                dr["Mobile"] = dtUser.Rows[0]["MobileNo"].ToString().Trim();
                                if (!string.IsNullOrEmpty(dtUser.Rows[0]["Email"].ToString()))
                                    dr["Email"] = dtUser.Rows[0]["Email"].ToString().Trim();
                                else
                                    dr["Email"] = "";
                                dr["SectorName"] = dtUser.Rows[0]["SectorName"].ToString().Trim();
                                //dr["BuildingName"] = dtUser.Rows[0]["BuildingName"].ToString().Trim();
                                //dr["BlockNo"] = dtUser.Rows[0]["BlockNo"].ToString().Trim();
                                //dr["FlatNo"] = dtUser.Rows[0]["FlatNo"].ToString().Trim();
                                //dr["UserName"] = dtUser.Rows[0]["UserName"].ToString().Trim();
                                dr["Password"] = dtUser.Rows[0]["Password"].ToString().Trim();
                                dr["Address"] = dtUser.Rows[0]["Address"].ToString().Trim();
                            }
                        }
                        else
                        {
                            dr["status"] = "Fail";
                            dr["error_msg"] = "Update Fail";
                        }
                    }
                }
                else
                {
                    //check mobile 
                    string usermobile = "";
                    DataTable dtCheckMobile = new DataTable();
                    dtCheckMobile = obj.BindCustomer(item.Id);
                    if (dtCheckMobile.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dtCheckMobile.Rows[0]["MobileNo"].ToString()))
                        {
                            if (item.MobileNo == dtCheckMobile.Rows[0]["MobileNo"].ToString())
                            {
                                //Update
                                obj.Id = item.Id;
                                obj.FirstName = item.FirstName;
                                obj.LastName = item.LastName;
                                obj.MobileNo = item.MobileNo;
                                obj.Email = item.Email;
                                obj.BuildingId = item.BuildingId;
                                obj.UserName = item.MobileNo;
                                obj.Address = item.Address;
                                //  obj.Password = item.Password;
                                obj.FlatId = item.FlatId;
                                result = obj.MobileUpdateCustomer(obj);
                            }
                            else
                            {
                                int _min = 1000;
                                int _max = 9999;
                                Random _rdm = new Random();
                                int otp = _rdm.Next(_min, _max);
                                //string Msg = "%3C%23%3E Your OTP is:" + otp;
                                string Msg = "Welcome to Milkyway India Family!!Your Milkyway India OTP is " + otp + " You can now order Milk, Dairy, Grocery at your doorstep Or Bill Payment with Cash back.";
                              
                                try
                                {
                                    Helper.SendOTPSMS(Msg, item.MobileNo);
                                    //check otp available or not by mobile
                                    DataTable dtotp = obj.getOtpCustomerList(item.MobileNo);
                                    obj.Count = 0;
                                    if (dtotp.Rows.Count > 0)
                                    {
                                        obj.Count = Convert.ToInt32(dtotp.Rows[0]["Count"]) + 1;                                        
                                        int UpdateOtp = obj.CustomerupdateOtp(otp.ToString(), item.MobileNo, obj.Count);//update
                                    }
                                    else
                                    {                                        
                                        int InsertOtp = obj.CustomerInsertOtp(otp.ToString(), item.MobileNo);//Insert
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                                result = 2;
                            }
                        }
                    }

                    if (result == 2)
                    {
                        dr["status"] = "success";
                        dr["error_msg"] = "OTP Send Successfully";
                    }
                    else if (result > 0)
                    {
                        dr["status"] = "Success";
                        dr["error_msg"] = "Update data Successfully";

                        DataTable dtUser = new DataTable();
                        dtUser = obj.NewCustomer(item.MobileNo, item.Password, item.MobileNo);
                        if (dtUser.Rows.Count > 0)
                        {
                            dr["msg"] = "Find User";
                            dr["Customerid"] = Convert.ToInt64(dtUser.Rows[0]["Id"]);
                            dr["FirstName"] = dtUser.Rows[0]["FirstName"].ToString().Trim();
                            dr["LastName"] = dtUser.Rows[0]["LastName"].ToString().Trim();
                            dr["Mobile"] = dtUser.Rows[0]["MobileNo"].ToString().Trim();
                            if (!string.IsNullOrEmpty(dtUser.Rows[0]["Email"].ToString()))
                                dr["Email"] = dtUser.Rows[0]["Email"].ToString().Trim();
                            else
                                dr["Email"] = "";
                            dr["SectorName"] = dtUser.Rows[0]["SectorName"].ToString().Trim();
                           // dr["BuildingName"] = dtUser.Rows[0]["BuildingName"].ToString().Trim();
                            //dr["BlockNo"] = dtUser.Rows[0]["BlockNo"].ToString().Trim();
                            //dr["FlatNo"] = dtUser.Rows[0]["FlatNo"].ToString().Trim();
                            dr["Address"] = dtUser.Rows[0]["Address"].ToString().Trim();
                            //dr["UserName"] = dtUser.Rows[0]["UserName"].ToString().Trim();
                            dr["Password"] = dtUser.Rows[0]["Password"].ToString().Trim();
                        }
                    }
                    else
                    {
                        dr["status"] = "Fail";
                        dr["error_msg"] = "Update Fail";
                    }
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

        public IHttpActionResult VerifyOTPEditCustomer(Customer item)
        {   //
            Customer obj = new Customer();
            int result = 0;
            string jsonString = null; string str1 = null; string str2 = null;

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            dtNew.Columns.Add("msg", typeof(string));
            dtNew.Columns.Add("Customerid", typeof(Int64));
            dtNew.Columns.Add("FirstName", typeof(string));
            dtNew.Columns.Add("LastName", typeof(string));
            dtNew.Columns.Add("Mobile", typeof(string));
            dtNew.Columns.Add("Email", typeof(string));
            dtNew.Columns.Add("BuildingName", typeof(string));
            dtNew.Columns.Add("BlockNo", typeof(string));
            dtNew.Columns.Add("FlatNo", typeof(string));
            dtNew.Columns.Add("Address", typeof(string));
            // dtNew.Columns.Add("UserName", typeof(string));
            dtNew.Columns.Add("Password", typeof(string));

            dtNew.Columns.Add("gpickloc", typeof(string));
            dtNew.Columns.Add("lat", typeof(string));
            dtNew.Columns.Add("lon", typeof(string));

            DataRow dr = dtNew.NewRow();
            if ((item.FirstName != "" && item.FirstName != null) && (item.MobileNo != "" && item.MobileNo != null))
            {
                //check emailid validation
                //if (!IsValidEmail(item.Email))
                //{
                //    dr["status"] = "Failed";
                //    dr["error_msg"] = "Invalid Email Address";
                //    dtNew.Rows.Add(dr);
                //    return Ok(dtNew);
                //}

                //check mobileno validation
                if (!IsValidMobileNo(item.MobileNo))
                {
                    dr["status"] = "Failed";
                    dr["error_msg"] = "Invalid MobileNo";

                    dtNew.Rows.Add(dr);
                    return Ok(dtNew);
                }

                //check otp match
                DataTable CheckuserOtp = obj.CheckCustomerOtp(item.MobileNo, item.OTP);
                if (CheckuserOtp.Rows.Count > 0)
                {
                    //check mobileno
                    DataTable dtuserRecord = new DataTable();
                    dtuserRecord = obj.CheckCustomerMobile(item.MobileNo);
                    int userRecords = dtuserRecord.Rows.Count;

                    //check Flatno
                    DataTable dtflatno = new DataTable();
                    dtflatno = obj.CheckCustomerFlatNo(item.FlatId);
                    int flatno = dtflatno.Rows.Count;
                    if (userRecords > 0)
                    {
                        if (item.Id != Convert.ToInt32(dtuserRecord.Rows[0]["Id"]))
                        {
                            dr["status"] = "Failed";
                            dr["error_msg"] = "Mobile No Already Exist";
                            dtNew.Rows.Add(dr);
                            return Ok(dtNew);
                        }
                        else
                        {
                            //check mobile 
                            string usermobile = "";
                            DataTable dtCheckMobile = new DataTable();
                            dtCheckMobile = obj.BindCustomer(item.Id);
                            if (dtCheckMobile.Rows.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(dtCheckMobile.Rows[0]["MobileNo"].ToString()))
                                {
                                    if (item.MobileNo == dtCheckMobile.Rows[0]["MobileNo"].ToString())
                                    {
                                        //Update
                                        obj.Id = item.Id;
                                        obj.FirstName = item.FirstName;
                                        obj.LastName = item.LastName;
                                        obj.MobileNo = item.MobileNo;
                                        obj.Email = item.Email;
                                        obj.BuildingId = item.BuildingId;
                                        obj.UserName = item.MobileNo;
                                        //  obj.Password = item.Password;
                                        obj.Address = item.Address;
                                        obj.FlatId = item.FlatId;
                                        obj.gpickloc = item.gpickloc;
                                        obj.lat = item.lat;
                                        obj.lon = item.lon;
                                        result = obj.MobileUpdateCustomer(obj);
                                    }
                                    else
                                    {
                                        int _min = 1000;
                                        int _max = 9999;
                                        Random _rdm = new Random();
                                        int otp = _rdm.Next(_min, _max);

                                        //string Msg = "%3C%23%3E Your OTP is:" + otp;
                                        string Msg = "Welcome to Milkyway India Family!!Your Milkyway India OTP is " + otp + " You can now order Milk, Dairy, Grocery at your doorstep Or Bill Payment with Cash back.";
                                        try
                                        {
                                            Helper.SendOTPSMS(Msg, item.MobileNo);
                                            //check otp available or not by mobile
                                            DataTable dtotp = obj.getOtpCustomerList(item.MobileNo);
                                            obj.Count = 0;
                                            if (dtotp.Rows.Count > 0)
                                            {
                                                obj.Count = Convert.ToInt32(dtotp.Rows[0]["Count"]) + 1;
                                                //update
                                                int UpdateOtp = obj.CustomerupdateOtp(otp.ToString(), item.MobileNo, obj.Count);
                                            }
                                            else
                                            {
                                                //Insert
                                                int InsertOtp = obj.CustomerInsertOtp(otp.ToString(), item.MobileNo);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                        result = 2;
                                    }
                                }

                            }

                            if (result == 2)
                            {
                                dr["status"] = "success";
                                dr["error_msg"] = "OTP Send Successfully";
                            }
                            else if (result > 0)
                            {
                                dr["status"] = "Success";
                                dr["error_msg"] = "Update data Successfully";

                                DataTable dtUser = new DataTable();
                                dtUser = obj.NewCustomer(item.MobileNo, item.Password, item.MobileNo);
                                if (dtUser.Rows.Count > 0)
                                {
                                    dr["msg"] = "Find User";
                                    dr["Customerid"] = Convert.ToInt64(dtUser.Rows[0]["Id"]);
                                    dr["FirstName"] = dtUser.Rows[0]["FirstName"].ToString().Trim();
                                    dr["LastName"] = dtUser.Rows[0]["LastName"].ToString().Trim();
                                    dr["Mobile"] = dtUser.Rows[0]["MobileNo"].ToString().Trim();
                                    if (!string.IsNullOrEmpty(dtUser.Rows[0]["Email"].ToString()))
                                        dr["Email"] = dtUser.Rows[0]["Email"].ToString().Trim();
                                    else
                                        dr["Email"] = "";
                                    dr["Address"] = dtUser.Rows[0]["Address"].ToString().Trim();
                                    //dr["BuildingName"] = dtUser.Rows[0]["BuildingName"].ToString().Trim();
                                    //dr["BlockNo"] = dtUser.Rows[0]["BlockNo"].ToString().Trim();
                                    //dr["FlatNo"] = dtUser.Rows[0]["FlatNo"].ToString().Trim();
                                    //dr["UserName"] = dtUser.Rows[0]["UserName"].ToString().Trim();

                                    dr["Password"] = dtUser.Rows[0]["Password"].ToString().Trim();
                                }
                            }
                            else
                            {
                                dr["status"] = "Fail";
                                dr["error_msg"] = "Update Fail";
                            }
                        }
                    }
                    else
                    {
                        //Update
                        obj.Id = item.Id;
                        obj.FirstName = item.FirstName;
                        obj.LastName = item.LastName;
                        obj.MobileNo = item.MobileNo;
                        obj.Email = item.Email;
                        obj.BuildingId = item.BuildingId;
                        obj.UserName = item.MobileNo;
                        //  obj.Password = item.Password;
                        obj.Address = item.Address;
                        obj.FlatId = item.FlatId;
                        obj.gpickloc = item.gpickloc;
                        obj.lat = item.lat;
                        obj.lon = item.lon;
                        result = obj.MobileUpdateCustomer(obj);

                        if (result > 0)
                        {
                            dr["status"] = "Success";
                            dr["error_msg"] = "Update data Successfully";

                            DataTable dtUser = new DataTable();
                            dtUser = obj.NewCustomer(item.MobileNo, item.Password, item.MobileNo);
                            if (dtUser.Rows.Count > 0)
                            {
                                dr["msg"] = "Find User";
                                dr["Customerid"] = Convert.ToInt64(dtUser.Rows[0]["Id"]);
                                dr["FirstName"] = dtUser.Rows[0]["FirstName"].ToString().Trim();
                                dr["LastName"] = dtUser.Rows[0]["LastName"].ToString().Trim();
                                dr["Mobile"] = dtUser.Rows[0]["MobileNo"].ToString().Trim();
                                if (!string.IsNullOrEmpty(dtUser.Rows[0]["Email"].ToString()))
                                    dr["Email"] = dtUser.Rows[0]["Email"].ToString().Trim();
                                else
                                    dr["Email"] = "";
                                dr["Address"] = dtUser.Rows[0]["Address"].ToString().Trim();
                                //dr["BuildingName"] = dtUser.Rows[0]["BuildingName"].ToString().Trim();
                                //dr["BlockNo"] = dtUser.Rows[0]["BlockNo"].ToString().Trim();
                                //dr["FlatNo"] = dtUser.Rows[0]["FlatNo"].ToString().Trim();
                                //dr["UserName"] = dtUser.Rows[0]["UserName"].ToString().Trim();
                                dr["Password"] = dtUser.Rows[0]["Password"].ToString().Trim();
                            }
                        }
                        else
                        {
                            dr["status"] = "Fail";
                            dr["error_msg"] = "Update Fail";
                        }
                    }
                }
                else
                {
                    dr["status"] = "Failed";
                    dr["error_msg"] = "OTP not match";

                    dtNew.Rows.Add(dr);
                    return Ok(dtNew);
                }

                if (result > 0)
                {

                    dr["status"] = "Success";
                    dr["error_msg"] = "Update data Successfully";

                    DataTable dtUser = new DataTable();
                    dtUser = obj.NewCustomer(item.MobileNo, item.Password, item.MobileNo);
                    if (dtUser.Rows.Count > 0)
                    {
                        dr["msg"] = "Find User";
                        dr["Customerid"] = Convert.ToInt64(dtUser.Rows[0]["Id"]);
                        dr["FirstName"] = dtUser.Rows[0]["FirstName"].ToString().Trim();
                        dr["LastName"] = dtUser.Rows[0]["LastName"].ToString().Trim();
                        dr["Mobile"] = dtUser.Rows[0]["MobileNo"].ToString().Trim();
                        if (!string.IsNullOrEmpty(dtUser.Rows[0]["Email"].ToString()))
                            dr["Email"] = dtUser.Rows[0]["Email"].ToString().Trim();
                        else
                            dr["Email"] = "";
                        dr["Address"] = dtUser.Rows[0]["Address"].ToString().Trim();
                        //dr["BuildingName"] = dtUser.Rows[0]["BuildingName"].ToString().Trim();
                        //dr["BlockNo"] = dtUser.Rows[0]["BlockNo"].ToString().Trim();
                        //dr["FlatNo"] = dtUser.Rows[0]["FlatNo"].ToString().Trim();
                        //dr["UserName"] = dtUser.Rows[0]["UserName"].ToString().Trim();
                        dr["Password"] = dtUser.Rows[0]["Password"].ToString().Trim();
                    }
                }
                else
                {
                    dr["status"] = "Fail";
                    dr["error_msg"] = "Update Fail";
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

        //get Total Reward Point
        [Route("api/CustomerApi/CustomerRewardPoint/{CustomerId?}")]
        [HttpGet]
        public HttpResponseMessage CustomerRewardPoint(string CustomerId) //JsonResult
        {
            Customer obj = new Customer();
            DataTable dtNew = new DataTable();
            int RewardPoint = 0;
            DataTable dtprodRecord = new DataTable();
            dtprodRecord = obj.BindCustomer(Convert.ToInt32(CustomerId));
            int userRecords = dtprodRecord.Rows.Count;

            if (userRecords > 0)
            {
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                //  dict["status"] = "success";
                if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["RewardPoint"].ToString()))
                    RewardPoint = Convert.ToInt32(dtprodRecord.Rows[0]["RewardPoint"]);

                dict["pointbalance"] = RewardPoint.ToString();

                string one = @"{""status"":""success""";
                string two = @",""rewardpoint"":" + dict["pointbalance"];
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
                dict["pointbalance"] = "0";


                string one = @"{""status"":""Fail""";
                string two = @",""rewardpoint"":" + dict["pointbalance"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }

        //customer redeem reward point
        public IHttpActionResult ReedemRewardPoint(Customer item)
        {
            Customer obj = new Customer();
            Subscription objsub = new Subscription();
            string jsonstring = null;
            double RewardRs = 0;

            DataTable dt = new DataTable();
            DataTable dtNew = new DataTable();

            int result = 0, updatepoint = 0;
            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("message", typeof(string));

            DataRow dr = dtNew.NewRow();
            dr["status"] = "failed";
            dr["message"] = "";

            if ((!string.IsNullOrEmpty(item.CustomerId.ToString()) || item.CustomerId != 0) && (!string.IsNullOrEmpty(item.RewardPoint.ToString()) || item.RewardPoint != 0))
            {
                //convert point to Rs 1 point = 1(0.01) paisa
                RewardRs = (Convert.ToDouble(item.RewardPoint) * 1) / 100;
                if (RewardRs > 0)
                {
                    //add rewards Rs to wallet and minus point from customer table
                    objsub.CustomerId = item.CustomerId;
                    objsub.TransactionDate = DateTime.Now;
                    objsub.Amount = Convert.ToDecimal(RewardRs);
                    objsub.OrderId = 0;
                    objsub.BillNo = null;
                    objsub.Description = "Reedem Reward Point";
                    objsub.Type = "Credit";
                    objsub.CustSubscriptionId = 0;

                    result = objsub.InsertWalletMobile(objsub);
                    if (result > 0)
                    {
                        //update reward point 0 in customer
                        updatepoint = obj.UpdateCustomerPoint(item.CustomerId, 0);
                    }
                    if (updatepoint > 0)
                    {
                        dr["status"] = "success";
                        dr["message"] = "Reward Point Redeemed";
                    }
                    else
                    {
                        dr["status"] = "failed";
                        dr["message"] = "Reward Point Not Reedemed";
                    }
                }
                else
                {
                    dr["status"] = "failed";
                    dr["message"] = "Cant Redeem If Points Balance Less Than 1000";
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

        [HttpGet]
        public int AppNotification(Int64 UserId, string notificationtitle, string notificationcontent, string notificationtype, string notificationobj_id, string notificationimage)
        {
            Customer objLogin = new Customer();
            DataTable dtToken = new DataTable();
            if (UserId == 0)
            {

            }
            else
            {
                dtToken = objLogin.getDeviceInstanceId(UserId);
            }
            if (dtToken.Rows.Count > 0)
            {
                string str = "";

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                //serverKey - Key from Firebase cloud messaging server  
                tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAP1kVwxg:APA91bEGnOhfhaWJqwwBtW0uzn3nF-mSbJxDtjggcMRts5mith8ArpqpnW57HEhO3yKpohZZZs7PPF1LCsYMlioCFXzyFt5nRxeTCgPk-zlrX-YfQps6yCn1Z9bdVAFK7HnCja_S3Nsp"));
                //Sender Id - From firebase project setting  
                tRequest.Headers.Add(string.Format("Sender: id={0}", "272077538072"));
                tRequest.ContentType = "application/json";
                var payload = new
                {
                    //to = dtToken.Rows[0]["fcm_token"].ToString(),
                    to = "fihh7RG_8UAEuT8-_ppQXg:APA91bFiPTHhcIa28d3CtVIDhwazv0AG3xFPgDzi5y7w6U715-JF9BeXgIOYZoldYbmN8fKPd-h6FLZjka4k2fQBKsF7DuCu5Hq_Detwy1dyJF8JsiNvZ8bacrZkYsc9amE0T8cGfLNX",
                    //// to = "cM2O8S-69e0:APA91bEpBJZhPu9amYDGY2ZBqVA0ubB9D-TYVmsSHkxiJetthLzHzvfToVbDz53aGS_w5qiXsd-g6C3wCFSQefxhISf-DX3HhL4XyIrMrG7lfCT1uQdxhTSOEG5DSSgeOKQP0bwhnJJS",
                    priority = "high",
                    content_available = true,
                    notification = new
                    {
                        body = notificationcontent,
                        title = notificationtitle,
                        badge = 1
                    },
                    data = new
                    {
                        click_action = "FLUTTER_NOTIFICATION_CLICK",
                        body = notificationcontent,
                        title = notificationtitle
                    }
                };

                string postbody = JsonConvert.SerializeObject(payload).ToString();
                Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
                                    //result.Response = sResponseFromServer;
                                }
                        }
                    }
                }
            }
            return 1;
        }

        [HttpGet]
        public int AppNotification1()
        {
            Customer objLogin = new Customer();
            DataTable dtToken = new DataTable();
            //if (UserId == 0)
            //{

            //}
            //else
            //{
            //    dtToken = objLogin.getDeviceInstanceId(UserId);
            //}
            //if (dtToken.Rows.Count > 0)
            //{
            string str = "";

            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            //serverKey - Key from Firebase cloud messaging server  
            tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAP1kVwxg:APA91bEGnOhfhaWJqwwBtW0uzn3nF-mSbJxDtjggcMRts5mith8ArpqpnW57HEhO3yKpohZZZs7PPF1LCsYMlioCFXzyFt5nRxeTCgPk-zlrX-YfQps6yCn1Z9bdVAFK7HnCja_S3Nsp"));
            //Sender Id - From firebase project setting  
            tRequest.Headers.Add(string.Format("Sender: id={0}", "272077538072"));
            tRequest.ContentType = "application/json";
            var payload = new
            {
                //to = dtToken.Rows[0]["fcm_token"].ToString(),
                to = "fn_EFLwORhaBdnAap3iROp:APA91bG8shui7_wgj4s0zZUHJ466m-tibh6ciTOT31wQ9DlRbwdhcORu-g5PFPUwtngVAh_nwZqPZDDZpdSMd6b36_myNsrXskh1vrBp6oMumb7heR5GKpE_b-2KBe21u4iZokeDbq4i",
                //// to = "cM2O8S-69e0:APA91bEpBJZhPu9amYDGY2ZBqVA0ubB9D-TYVmsSHkxiJetthLzHzvfToVbDz53aGS_w5qiXsd-g6C3wCFSQefxhISf-DX3HhL4XyIrMrG7lfCT1uQdxhTSOEG5DSSgeOKQP0bwhnJJS",
                priority = "high",
                content_available = true,
                notification = new
                {
                    body = "Test",
                    title = "Test",
                    badge = 1
                },
                data = new
                {
                    click_action = "FLUTTER_NOTIFICATION_CLICK",
                    body = "Test",
                    title = "Test"
                }
            };

            string postbody = JsonConvert.SerializeObject(payload).ToString();
            Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
            tRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = tRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    {
                        if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                //result.Response = sResponseFromServer;
                            }
                    }
                }
            }
            // }
            return 1;
        }

        public IHttpActionResult UpdateProfilePhoto(Customer item)
        {
            Customer obj = new Customer();
            Subscription objsub = new Subscription();
            string jsonstring = null;
            double RewardRs = 0;

            DataTable dt = new DataTable();
            DataTable dtNew = new DataTable();

            int result = 0, updatepoint = 0;
            dtNew.Columns.Add("imagepath", typeof(string));
            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("message", typeof(string));

            DataRow dr = dtNew.NewRow();
            dr["status"] = "failed";
            dr["message"] = "";

            if ((!string.IsNullOrEmpty(item.CustomerId.ToString()) || item.CustomerId != 0) && (!string.IsNullOrEmpty(item.base64Image.ToString())))
            {


                string filepath = "~/image/customer/";
                //fname = item.filename.ToString();
                //byte[] bytes = Convert.FromBase64String(item.);

                //string strm = "R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7";
                //Generate unique filename
                //string filepath = "D:/vijyeta/MilkWayIndia/MilkWayIndia/image/customer/" + item.Photo;
                //  string filepath = "https://admin.milkywayindia.com/image/customer/" + item.Photo;
                // filepath = Path.Combine(Server.MapPath("~/image/customer/"), item.Photo);
                filepath = AppDomain.CurrentDomain.BaseDirectory + ("image\\customer\\" + item.Photo);//active
                string filename = Path.GetFileName(filepath);

                var bytess = Convert.FromBase64String(item.base64Image);
                using (var imageFile = new FileStream(filepath, FileMode.Create))
                {
                    imageFile.Write(bytess, 0, bytess.Length);
                    imageFile.Flush();
                }
                obj.Photo = item.Photo;
                obj.Id = item.CustomerId;

                result = obj.UpdateCustomerPhoto(obj);

                if (result > 0)
                {

                    dr["imagepath"] = Helper.PhotoFolderPath + "/image/customer/" + obj.Photo;
                    dr["status"] = "success";
                    dr["message"] = "Profile Photo Updated!!!";
                }
                else
                {
                    dr["imagepath"] = "";
                    dr["status"] = "failed";
                    dr["message"] = "Profile Photo not Updated!!!";
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

        #region customer Favourite Product
        [Route("api/customerapi/insertcustomerfavourite/{CustomerId?}/{ProductId?}"), HttpGet]
        public HttpResponseMessage InsertCustomerFavourite(int CustomerId, int ProductId)
        {
            Customer _customer = new Customer();
            CustomerOrder obj = new CustomerOrder();
            FavouriteResponse response = new FavouriteResponse();
            response.status = "Failed";
            try
            {
                var checkFavourite = _customer.CheckCustomerFavourite(CustomerId, ProductId);
                if (checkFavourite.Rows.Count > 0)
                    return Request.CreateResponse(HttpStatusCode.OK);

                var favourite = obj.insertCustomerFavourite(CustomerId, ProductId);
                if (favourite > 0)
                {
                    response.status = "Success";
                    response.error_msg = "Favourite Successfully Add...";
                }
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                response.error_msg = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }

        [Route("api/customerapi/getcustomerfavourite/{CustomerId?}/{psize?}/{pno?}"), HttpGet]
        public HttpResponseMessage GetCustomerFavourite(int CustomerId, int psize, int pno)
        {
            CustomerOrder obj = new CustomerOrder();
            FavouriteProductResponse response = new FavouriteProductResponse();
            response.status = "Failed";
            try
            {
                var favourite = obj.getCustomerFavourite(CustomerId, psize, pno);
                var rowCount = favourite.Rows.Count;
                if (rowCount > 0)
                {
                    List<ProductViewModel> list = new List<ProductViewModel>();
                    for (int i = 0; i < favourite.Rows.Count; i++)
                    {
                        ProductViewModel product = new ProductViewModel();
                        product.id = Convert.ToInt32(favourite.Rows[i]["Id"].ToString().Trim());
                        product.productid = Convert.ToInt32(favourite.Rows[i]["ProductId"].ToString().Trim());
                        product.categoryid = Convert.ToInt32(favourite.Rows[i]["CategoryId"].ToString().Trim());
                        product.product = favourite.Rows[i]["ProductName"].ToString().Trim();
                        if (!string.IsNullOrEmpty(favourite.Rows[i]["TotalRecords"].ToString()))
                            product.producttotal = Convert.ToInt32(favourite.Rows[i]["TotalRecords"].ToString().Trim());
                        else
                            product.producttotal = 0;
                        if (!string.IsNullOrEmpty(favourite.Rows[i]["Code"].ToString()))
                            product.code = favourite.Rows[i]["Code"].ToString().Trim();
                        else
                            product.code = "";
                        if (!string.IsNullOrEmpty(favourite.Rows[i]["Price"].ToString()))
                            product.price = Convert.ToDecimal(favourite.Rows[i]["Price"]);
                        else
                            product.price = 0;
                        if (!string.IsNullOrEmpty(favourite.Rows[i]["DiscountAmount"].ToString()))
                            product.discountamt = Convert.ToDecimal(favourite.Rows[i]["DiscountAmount"]);
                        else
                            product.discountamt = 0;
                        if (!string.IsNullOrEmpty(favourite.Rows[i]["CGST"].ToString()))
                            product.cgst = Convert.ToDecimal(favourite.Rows[i]["CGST"]);
                        else
                            product.cgst = 0;
                        if (!string.IsNullOrEmpty(favourite.Rows[i]["SGST"].ToString()))
                            product.sgst = Convert.ToDecimal(favourite.Rows[i]["SGST"]);
                        else
                            product.sgst = 0;
                        if (!string.IsNullOrEmpty(favourite.Rows[i]["IGST"].ToString()))
                            product.igst = Convert.ToDecimal(favourite.Rows[i]["IGST"]);
                        else
                            product.igst = 0;
                        if (!string.IsNullOrEmpty(favourite.Rows[i]["RewardPoint"].ToString()))
                            product.rewardpoint = Convert.ToInt64(favourite.Rows[i]["RewardPoint"]);
                        else
                            product.rewardpoint = 0;
                        product.detail = favourite.Rows[i]["Detail"].ToString().Trim();
                        if (!string.IsNullOrEmpty(favourite.Rows[i]["Image"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(favourite.Rows[i]["Image"].ToString().Trim());
                            product.image = Helper.PhotoFolderPath + "/image/product/" + encoded;
                        }
                        else
                            product.image = "";

                        if (!string.IsNullOrEmpty(favourite.Rows[i]["IsDaily"].ToString()))
                            product.isdaily = Convert.ToBoolean(favourite.Rows[i]["IsDaily"]);
                        else
                            product.isdaily = false;
                        list.Add(product);
                    }
                    response.status = "Success";
                    response.error_msg = "Get Customer Favourite Products...";
                    response.products = list;
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

        [Route("api/customerapi/CheckCustomerFavourite/{CustomerId?}/{ProductId?}"), HttpGet]
        public HttpResponseMessage CheckCustomerFavourite(int CustomerId, int ProductId)
        {
            Customer obj = new Customer();
            try
            {
                var favourite = obj.CheckCustomerFavourite(CustomerId, ProductId);
                if (favourite.Rows.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, true);
                }
            }
            catch (Exception ex)
            {

            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, false);
        }

        [Route("api/customerapi/deletecustomerfavourite/{CustomerId?}/{ProductId?}"), HttpGet]
        public HttpResponseMessage DeleteCustomerFavourite(int CustomerId, int ProductId)
        {
            CustomerOrder obj = new CustomerOrder();
            FavouriteResponse response = new FavouriteResponse();
            response.status = "Failed";
            try
            {
                var favourite = obj.DeleteCustomerFavorite(CustomerId, ProductId);
                if (favourite == 1)
                {
                    response.status = "Success";
                    response.error_msg = "Favourite Successfully Delete...";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.error_msg = "Invalid customerid or productid...";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception ex)
            {
                response.error_msg = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }
        #endregion

        #region Customer Inquiry
        [Route("api/CustomerApi/InsertInquiry/{FirstName?}/{LastName?}/{Mobile?}/{Email?}/{Address1?}/{Address2?}/{Address3?}")]
        [HttpGet]
        public HttpResponseMessage InsertInquiry(string FirstName, string LastName, string Mobile, string Email, string Address1, string Address2, string Address3)
        {
            CustomerOrder obj = new CustomerOrder();
            InquiryResponse response = new InquiryResponse();
            response.status = "Failed";
            try
            {
                var favourite = obj.insertInquiry(FirstName, LastName, Mobile, Email, Address1, Address2, Address3);
                if (favourite > 0)
                {
                    response.status = "Success";
                    response.error_msg = "Inquiry Successfully Add...";
                }
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                response.error_msg = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }





        [Route("api/CustomerApi/LoginViaOtp/{MobileNo}")]
        [HttpGet]
        public HttpResponseMessage LoginViaOtp(string MobileNo)
        {
            string Status = ""; string jsonString = null; string str1 = null; string str2 = null;

            Customer obj = new Customer();

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
           DataRow dr = dtNew.NewRow();



            if ((MobileNo != "" && MobileNo != null))
            {


                if (!IsValidMobileNo(MobileNo))
                {
                    dr["status"] = "Failed";
                    dr["error_msg"] = "Invalid MobileNo";

                    dtNew.Rows.Add(dr);
                    jsonString = string.Empty;
                    jsonString = JsonConvert.SerializeObject(dtNew);
                 //   str1 = jsonString.ToString().Replace(@"[", "");
                  //  str2 = str1.ToString().Replace(@"]", "");
                   // jsonString = str2;
                    var response2 = Request.CreateResponse(HttpStatusCode.OK);
                    response2.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                    return response2;
                }


                //check mobileno
                DataTable dtuserRecord = new DataTable();
                dtuserRecord = obj.CheckCustomerMobile(MobileNo);
                int userRecords = dtuserRecord.Rows.Count;

                //check username
                //DataTable dtuserRecord1 = new DataTable();
                //dtuserRecord1 = obj.CheckCustomerUserName(UserName);
                //int userRecords1 = dtuserRecord1.Rows.Count;

                if (userRecords > 0)
                {
                    //Status = "0";
                    //dr["status"] = "Failed";
                    //dr["error_msg"] = "Mobile No Already Exist";


                    int _min = 1000;
                    int _max = 9999;
                    Random _rdm = new Random();
                    int otp = _rdm.Next(_min, _max);

                    //string Msg = "%3C%23%3E Your OTP is:" + otp + " esWOXq8cybG";
                    string Msg = "Welcome to Milkyway India Family!!Your Milkyway India OTP is " + otp + " You can now order Milk, Dairy, Grocery at your doorstep Or Bill Payment with Cash back.";

                    string strUrl = "";
                    //india sms
                    ////strUrl = "https://apps.vibgyortel.in/client/api/sendmessage?apikey=dca6c57e6c6f4638&mobiles=" + Mobile + "&sms=" + Msg + "&senderid=Aruhat";
                  // strUrl = "http://trans.magicsms.co.in/api/v4/?api_key=" + Helper.MagicSMSKey + "&method=sms&message=" + Msg + "&to=" + MobileNo + "&sender="+Helper.MagicSender+"&template_id=" + Helper.MagicOTPTemplateID;

                    // Create a request object  
                    //WebRequest request = HttpWebRequest.Create(strUrl);
                    

                    // Get the response back  
                    try
                    {
                        //HttpWebResponse responsesms = (HttpWebResponse)request.GetResponse();
                       // Stream s = (Stream)responsesms.GetResponseStream();
                       // StreamReader readStream = new StreamReader(s);
                       // string dataString = readStream.ReadToEnd();
                       // responsesms.Close();
                       // s.Close();
                       // readStream.Close();
                   Helper.SendOTPSMS(Msg, MobileNo);
                        dr["status"] = "success";
                        dr["error_msg"] = "Otp Send Successfully";

                        //check otp available or not by mobile
                        DataTable dtotp = obj.getOtpCustomerList(MobileNo);
                        obj.Count = 0;
                        if (dtotp.Rows.Count > 0)
                        {
                            obj.Count = Convert.ToInt32(dtotp.Rows[0]["Count"]);
                        //update
                        int UpdateOtp = obj.CustomerupdateOtp(otp.ToString(), MobileNo, obj.Count);
                    }
                        else
                        {
                        //Insert
                        int InsertOtp = obj.CustomerInsertOtp(otp.ToString(), MobileNo);
                    }

                      
                    }
                    catch (Exception ex)
                    {

                    }


                }

                else
                {
                    dr["status"] = "Failed";
                    dr["error_msg"] = "Mobile No Does Not Exist";
                }

            }








                dtNew.Rows.Add(dr);
            jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dtNew);
           // str1 = jsonString.ToString().Replace(@"[", "");
           // str2 = str1.ToString().Replace(@"]", "");
           // jsonString = str2;
            //  return jsonString;
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return response;
        }



        [Route("api/CustomerApi/LoginViaOtpValidation/{MobileNo}/{OTP}")]
        [HttpGet]
        public HttpResponseMessage LoginViaOtpValidation(string MobileNo, string OTP)
        {
            string Status = ""; string jsonString = null; string str1 = null; string str2 = null;

            Customer obj = new Customer();

            DataTable dtNew = new DataTable();

            Customer objlogin = new Customer();

            DataTable dtUser = new DataTable();

         

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("msg", typeof(string));
            //   dtNew.Columns.Add("error_msg", typeof(string));
            dtNew.Columns.Add("userid", typeof(Int64));
            dtNew.Columns.Add("firstname", typeof(string));
            dtNew.Columns.Add("lastname", typeof(string));
            dtNew.Columns.Add("mobileno", typeof(string));
            dtNew.Columns.Add("email", typeof(string));
            dtNew.Columns.Add("referralcode", typeof(string));
            dtNew.Columns.Add("photo", typeof(string));
            dtNew.Columns.Add("buildingname", typeof(string));
            dtNew.Columns.Add("address", typeof(string));
            dtNew.Columns.Add("flatno", typeof(string));
            dtNew.Columns.Add("SectorName", typeof(string));
            dtNew.Columns.Add("SectorId", typeof(int));
            dtNew.Columns.Add("BuildingId", typeof(int));
            dtNew.Columns.Add("FlatId", typeof(int));
            DataRow dr = dtNew.NewRow();


            int result = 0;
            
            //check otp and mobile
            DataTable CheckuserOtp = obj.CheckCustomerOtp(MobileNo, OTP);
            if (CheckuserOtp.Rows.Count > 0)
            {
                //insert
                obj.Id = 0;


                DataTable dtpassword1 = new DataTable();
                dtpassword1 = objlogin.Customerlogin1(MobileNo);

                if (dtpassword1.Rows.Count > 0)
                {
                    dr["status"] = "Success";
                    dr["msg"] = "Find User";
                    int customerId = Convert.ToInt32(dtpassword1.Rows[0]["Id"]);
                    var _customer = objlogin.BindCustomer(customerId);

                    if (_customer.Rows.Count > 0)
                    {
                        dr["userid"] = Convert.ToInt32(dtpassword1.Rows[0]["Id"]);
                        dr["firstname"] = _customer.Rows[0]["FirstName"].ToString().Trim();
                        dr["lastname"] = _customer.Rows[0]["LastName"].ToString().Trim();
                        dr["mobileno"] = _customer.Rows[0]["MobileNo"].ToString().Trim();
                        if (!string.IsNullOrEmpty(_customer.Rows[0]["Email"].ToString()))
                            dr["email"] = _customer.Rows[0]["Email"].ToString().Trim();
                        else
                            dr["email"] = "";
                        if (!string.IsNullOrEmpty(_customer.Rows[0]["Photo"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(_customer.Rows[0]["Photo"].ToString().Trim());
                            dr["photo"] = Helper.PhotoFolderPath + "/image/customer/" + encoded;
                        }
                        else
                            dr["photo"] = "";
                        if (!string.IsNullOrEmpty(_customer.Rows[0]["SectorId"].ToString()))
                            dr["SectorId"] = Convert.ToInt32(_customer.Rows[0]["SectorId"]);
                        else
                            dr["SectorId"] = 0;
                        if (!string.IsNullOrEmpty(_customer.Rows[0]["ReferralCode"].ToString()))
                            dr["referralcode"] = _customer.Rows[0]["ReferralCode"];
                        else
                            dr["referralcode"] = 0;
                        if (!string.IsNullOrEmpty(_customer.Rows[0]["Address"].ToString()))
                            dr["address"] = _customer.Rows[0]["address"];
                        else
                            dr["address"] = 0;

                        if (!string.IsNullOrEmpty(_customer.Rows[0]["SectorId"].ToString()))
                        {
                            Sector _sector = new Sector();
                            var sector = _sector.getSectorList(Convert.ToInt32(_customer.Rows[0]["SectorId"]));
                            if (sector.Rows.Count > 0)
                                dr["SectorName"] = sector.Rows[0]["SectorName"];
                        }

                        dr["buildingname"] = "";
                        dr["flatno"] = "";
                    }


                    //result = obj.InsertCustomer(obj);
                }
       


            
            }


            else
            {
                dr["status"] = "Failed";
                dr["msg"] = "OTP not match";

                dtNew.Rows.Add(dr);
                jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew);
                str1 = jsonString.ToString().Replace(@"[", "");
                str2 = str1.ToString().Replace(@"]", "");
                jsonString = str2;
                var response5 = Request.CreateResponse(HttpStatusCode.OK);
                response5.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                return response5;
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


        [Route("api/CustomerApi/UpdatePassword/{CustomerId}/{Oldpass}/{NewPass}")]
        [HttpGet]
        public HttpResponseMessage UpdatePassword(int CustomerId,string Oldpass, string NewPass)
        {
            string Status = ""; string jsonString = null; string str1 = null; string str2 = null;

            Customer obj = new Customer();

            DataTable dtNew = new DataTable();

            Customer objlogin = new Customer();

            DataTable dtUser = new DataTable();



            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("msg", typeof(string));

            DataRow dr = dtNew.NewRow();
            dr["status"] = "failed";
            dr["msg"] = "";


            if (CustomerId.ToString() != null)
            {
               

                DataTable dtpassword = new DataTable();
                dtpassword = objlogin.Customerlogin2(CustomerId, Oldpass);


                if (dtpassword.Rows.Count > 0)
                {
                    //dr["status"] = "Success";
                    //dr["msg"] = "Find User";
                    //int customerId = Convert.ToInt32(dtpassword.Rows[0]["Id"]);
                    //var _customer = objlogin.BindCustomer(customerId);

                    int updPwd = obj.UpdateCustomerPwd(CustomerId, NewPass);

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


        [Route("api/CustomerApi/getcustomerMobileRecharge/{CustomerId?}/{Type}"), HttpGet]

        public HttpResponseMessage getcustomerMobileRecharge(int CustomerId,string Type)
        {
            Customer obj = new Customer();
            string jsonString = null; string str1 = null; string str2 = null;

            DataTable dtNew = new DataTable();

           // dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            //dtNew.Columns.Add("msg", typeof(string));
            dtNew.Columns.Add("Customerid", typeof(Int64));
            dtNew.Columns.Add("RechargeNo", typeof(string));
            dtNew.Columns.Add("OperatorId", typeof(string));
            //

            dtNew.Columns.Add("CirclCode", typeof(string));
            dtNew.Columns.Add("IsPartial", typeof(string));
            
            dtNew.Columns.Add("Type", typeof(string));
            dtNew.Columns.Add("Lat", typeof(string));
            dtNew.Columns.Add("Long", typeof(string));
            dtNew.Columns.Add("AgentId", typeof(string));
            dtNew.Columns.Add("PaymentMode", typeof(string));
            dtNew.Columns.Add("CustomerMobile", typeof(string));

            dtNew.Columns.Add("Fieldtag1", typeof(string));
            dtNew.Columns.Add("Fieldtag2", typeof(string));
            dtNew.Columns.Add("FieldTag3", typeof(string));

            //

            dtNew.Columns.Add("Status", typeof(string));
            dtNew.Columns.Add("RechargeAmount", typeof(string));
            dtNew.Columns.Add("rechargedate", typeof(string));
            dtNew.Columns.Add("Name", typeof(string));


            DataRow dr = dtNew.NewRow();
           // dr["status"] = "Success";
            

            DataTable dtUser = new DataTable();
            dtUser = obj.getMobileRecharge(CustomerId,Type);
            if (dtUser.Rows.Count > 0)
            {
               
                

              

                for (int i=0;i< dtUser.Rows.Count;i++)
                {

                    DataRow dr1 = dtNew.NewRow();
                    dr1["error_msg"] = "Recent Recharge";
                    dr1["Customerid"] = Convert.ToInt64(dtUser.Rows[0]["CustomerId"]);
                    dr1["RechargeNo"] = dtUser.Rows[i]["Rechargeno"].ToString().Trim();
                    dr1["OperatorId"] = dtUser.Rows[i]["OpeartorId"].ToString().Trim();


                    //
                      dr1["CirclCode"] = dtUser.Rows[i]["CircleCode"].ToString().Trim();
                    if(Type!="Mobile" && Type!="DTH")
                    {

                        if (dtUser.Rows[i]["IsPartial"].ToString().Trim() != null) dr1["IsPartial"] = dtUser.Rows[i]["IsPartial"].ToString().Trim();

                        if (dtUser.Rows[i]["Type"].ToString().Trim() != null) dr1["Type"] = dtUser.Rows[i]["Type"].ToString().Trim();

                        if (dtUser.Rows[i]["Lat"].ToString().Trim() != null) dr1["Lat"] = dtUser.Rows[i]["Lat"].ToString().Trim();
                        if (dtUser.Rows[i]["Long"].ToString().Trim() != null) dr1["Long"] = dtUser.Rows[i]["Long"].ToString().Trim();
                        if (dtUser.Rows[i]["AgentId"].ToString().Trim() != null) dr1["AgentId"] = dtUser.Rows[i]["AgentId"].ToString().Trim();
                        if (dtUser.Rows[i]["PaymentMode"].ToString().Trim() != null) dr1["PaymentMode"] = dtUser.Rows[i]["PaymentMode"].ToString().Trim();
                        if (dtUser.Rows[i]["CustomerMobile"].ToString().Trim() != null) dr1["CustomerMobile"] = dtUser.Rows[i]["CustomerMobile"].ToString().Trim();
                        if (dtUser.Rows[i]["Fieldtag1"].ToString().Trim() != null) dr1["Fieldtag1"] = dtUser.Rows[i]["Fieldtag1"].ToString().Trim();
                        if (dtUser.Rows[i]["Fieldtag2"].ToString().Trim() != null) dr1["Fieldtag2"] = dtUser.Rows[i]["Fieldtag2"].ToString().Trim();
                        if (dtUser.Rows[i]["Fieldtag3"].ToString().Trim() != null) dr1["FieldTag3"] = dtUser.Rows[i]["Fieldtag3"].ToString().Trim();
                    }



                    //
                    dr1["Status"] = dtUser.Rows[i]["RechargeStatus"].ToString().Trim();
                    dr1["RechargeAmount"] = dtUser.Rows[i]["RechargeAmount"].ToString().Trim();
                    dr1["rechargedate"] = dtUser.Rows[i]["rechargedate"].ToString().Trim();
                    dr1["name"] = dtUser.Rows[i]["Name"].ToString().Trim();
                    dtNew.Rows.Add(dr1);

                }
                
               
            }
           
            jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dtNew);
           // str1 = jsonString.ToString().Replace(@"[", "");
           // str2 = str1.ToString().Replace(@"]", "");
           //jsonString = str2;
            var response6 = Request.CreateResponse(HttpStatusCode.OK);
            response6.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return response6;

        }






        [Route("api/CustomerApi/getcustomerfavouriteshow/{CustomerId?}/{psize?}/{pno?}"), HttpGet]
        public HttpResponseMessage GetCustomerFavouriteshow(int CustomerId, int psize, int pno)
        {
            CustomerOrder obj = new CustomerOrder();
            FavouriteProductResponse response = new FavouriteProductResponse();
            response.status = "Failed";
            try
            {
                var favourite = obj.getCustomerFavouriteshow(CustomerId, psize, pno);
                var rowCount = favourite.Rows.Count;
                if (rowCount > 0)
                {
                    List<ProductViewModel> list = new List<ProductViewModel>();
                    for (int i = 0; i < favourite.Rows.Count; i++)
                    {
                        ProductViewModel product = new ProductViewModel();
                        //product.id = Convert.ToInt32(favourite.Rows[i]["Id"].ToString().Trim());
                        product.productid = Convert.ToInt32(favourite.Rows[i]["ProductId"].ToString().Trim());
                        product.categoryid = Convert.ToInt32(favourite.Rows[i]["CategoryId"].ToString().Trim());
                        product.product = favourite.Rows[i]["ProductName"].ToString().Trim();
                        if (!string.IsNullOrEmpty(favourite.Rows[i]["TotalRecords"].ToString()))
                            product.producttotal = Convert.ToInt32(favourite.Rows[i]["TotalRecords"].ToString().Trim());
                        else
                            product.producttotal = 0;
                        if (!string.IsNullOrEmpty(favourite.Rows[i]["Code"].ToString()))
                            product.code = favourite.Rows[i]["Code"].ToString().Trim();
                        else
                            product.code = "";
                        if (!string.IsNullOrEmpty(favourite.Rows[i]["Price"].ToString()))
                            product.price = Convert.ToDecimal(favourite.Rows[i]["Price"]);
                        else
                            product.price = 0;
                        if (!string.IsNullOrEmpty(favourite.Rows[i]["DiscountAmount"].ToString()))
                            product.discountamt = Convert.ToDecimal(favourite.Rows[i]["DiscountAmount"]);
                        else
                            product.discountamt = 0;
                        if (!string.IsNullOrEmpty(favourite.Rows[i]["CGST"].ToString()))
                            product.cgst = Convert.ToDecimal(favourite.Rows[i]["CGST"]);
                        else
                            product.cgst = 0;
                        if (!string.IsNullOrEmpty(favourite.Rows[i]["SGST"].ToString()))
                            product.sgst = Convert.ToDecimal(favourite.Rows[i]["SGST"]);
                        else
                            product.sgst = 0;
                        if (!string.IsNullOrEmpty(favourite.Rows[i]["IGST"].ToString()))
                            product.igst = Convert.ToDecimal(favourite.Rows[i]["IGST"]);
                        else
                            product.igst = 0;
                        if (!string.IsNullOrEmpty(favourite.Rows[i]["RewardPoint"].ToString()))
                            product.rewardpoint = Convert.ToInt64(favourite.Rows[i]["RewardPoint"]);
                        else
                            product.rewardpoint = 0;
                        product.detail = favourite.Rows[i]["Detail"].ToString().Trim();
                        if (!string.IsNullOrEmpty(favourite.Rows[i]["Image"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(favourite.Rows[i]["Image"].ToString().Trim());
                            product.image = Helper.PhotoFolderPath + "/image/product/" + encoded;
                        }
                        else
                            product.image = "";

                        if (!string.IsNullOrEmpty(favourite.Rows[i]["IsDaily"].ToString()))
                            product.isdaily = Convert.ToBoolean(favourite.Rows[i]["IsDaily"]);
                        else
                            product.isdaily = false;
                        list.Add(product);
                    }
                    response.status = "Success";
                    response.error_msg = "Get Customer Favourite Products...";
                    response.products = list;
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



        [Route("api/CustomerApi/getTotalCashback/{CustomerId?}"), HttpGet]

        public HttpResponseMessage getTotalCashback(int CustomerId)
        {

            Customer obj = new Customer();
            string jsonString = null; string str1 = null; string str2 = null;

            DataTable dtNew = new DataTable();

            // dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            //dtNew.Columns.Add("msg", typeof(string));
            dtNew.Columns.Add("Customerid", typeof(Int64));
            dtNew.Columns.Add("CashbackType", typeof(string));
            dtNew.Columns.Add("Description", typeof(string));
            dtNew.Columns.Add("CashbackAmount", typeof(string));
            dtNew.Columns.Add("Date", typeof(string));
           


            DataRow dr = dtNew.NewRow();
            // dr["status"] = "Success";


            DataTable dtUser = new DataTable();
            dtUser = obj.getCustomerCashback(CustomerId);
            if (dtUser.Rows.Count > 0)
            {





                for (int i = 0; i < dtUser.Rows.Count; i++)
                {

                    DataRow dr1 = dtNew.NewRow();
                    dr1["error_msg"] = "CashBack Detail";
                    dr1["Customerid"] = Convert.ToInt64(dtUser.Rows[0]["CustomerId"]);
                    dr1["CashbackType"] = dtUser.Rows[i]["Cashbacktype"].ToString().Trim();
                    dr1["Description"] = dtUser.Rows[i]["Description"].ToString().Trim();
                    dr1["CashbackAmount"] = dtUser.Rows[i]["Amount"].ToString().Trim();
                    dr1["Date"] = dtUser.Rows[i]["TransactionDate"].ToString().Trim();
                    
                    dtNew.Rows.Add(dr1);

                }


            }

            jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dtNew);
            //str1 = jsonString.ToString().Replace(@"[", "");
            //str2 = str1.ToString().Replace(@"]", "");
            //jsonString = str2;
            var response6 = Request.CreateResponse(HttpStatusCode.OK);
            response6.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return response6;

        }



        [Route("api/CustomerApi/CashbackSeen/{CustomerId?}"), HttpGet]

        public HttpResponseMessage CashbackSeen(int CustomerId)
        {

            Customer obj = new Customer();
            string jsonString = null; string str1 = null; string str2 = null;

            DataTable dtNew = new DataTable();

            // dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            //dtNew.Columns.Add("msg", typeof(string));
            dtNew.Columns.Add("Id", typeof(Int64));
            dtNew.Columns.Add("Customerid", typeof(Int64));
            dtNew.Columns.Add("CashbackType", typeof(string));
            dtNew.Columns.Add("Description", typeof(string));
            dtNew.Columns.Add("CashbackAmount", typeof(string));
            dtNew.Columns.Add("Date", typeof(string));



            DataRow dr = dtNew.NewRow();
            // dr["status"] = "Success";


            DataTable dtUser = new DataTable();
            dtUser = obj.getCustomerCashbackSeen(CustomerId);
            if (dtUser.Rows.Count > 0)
            {





                for (int i = 0; i < dtUser.Rows.Count; i++)
                {

                    DataRow dr1 = dtNew.NewRow();
                    dr1["error_msg"] = "CashBack Detail";
                    dr1["Id"] = Convert.ToInt64(dtUser.Rows[i]["Id"]);
                    dr1["Customerid"] = Convert.ToInt64(dtUser.Rows[0]["CustomerId"]);
                    dr1["CashbackType"] = dtUser.Rows[i]["Cashbacktype"].ToString().Trim();
                    dr1["Description"] = dtUser.Rows[i]["Description"].ToString().Trim();
                    dr1["CashbackAmount"] = dtUser.Rows[i]["Amount"].ToString().Trim();
                    dr1["Date"] = dtUser.Rows[i]["TransactionDate"].ToString().Trim();

                    dtNew.Rows.Add(dr1);

                }


            }
            else
            {
                DataRow dr1 = dtNew.NewRow();
                dr1["error_msg"] = "No Data Found";
                dtNew.Rows.Add(dr1);
            }
            jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dtNew);
            //str1 = jsonString.ToString().Replace(@"[", "");
            //str2 = str1.ToString().Replace(@"]", "");
            //jsonString = str2;
            var response6 = Request.CreateResponse(HttpStatusCode.OK);
            response6.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return response6;

        }




        [Route("api/CustomerApi/UpdateCashBackSeen/{CustomerId}/{Id}")]
        [HttpGet]
        public HttpResponseMessage UpdateCashBackSeen(int CustomerId,int Id)
        {
            string Status = ""; string jsonString = null; string str1 = null; string str2 = null;

            Customer obj = new Customer();

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();


            CashbackSeenResponse response = new CashbackSeenResponse();
            response.status = "Failed";
            try
            {
              

                var favourite = obj.UpdateCashbackseen(CustomerId, Id);
                if (favourite > 0)
                {
                    response.status = "Success";
                    response.error_msg = "Enjoy Your Cashback with Milkyway...";
                }
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                response.error_msg = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, response);

           
        }



        [Route("api/CustomerApi/VisitorList")]
        [HttpGet]
        public IHttpActionResult VisitorList() //JsonResult
        {
            Customer visitor = new Customer();
            var dtList = visitor.GetVisitor();


            return Ok(dtList);
        }



        [Route("api/CustomerApi/LoginRefresh/{CustomerId?}")]
        [HttpGet]
        public HttpResponseMessage LoginRefresh(int? CustomerId)
        {
            Customer objlogin = new Customer();

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
            dtNew.Columns.Add("referralcode", typeof(string));
            dtNew.Columns.Add("photo", typeof(string));
            dtNew.Columns.Add("buildingname", typeof(string));
            dtNew.Columns.Add("address", typeof(string));
            dtNew.Columns.Add("flatno", typeof(string));
            dtNew.Columns.Add("SectorName", typeof(string));
            dtNew.Columns.Add("SectorId", typeof(int));
            dtNew.Columns.Add("BuildingId", typeof(int));
            dtNew.Columns.Add("FlatId", typeof(int));
            DataRow dr = dtNew.NewRow();

            if (!string.IsNullOrEmpty(CustomerId.ToString()) || CustomerId!=0)
            {
                DataTable dtUsername = new DataTable();
                dtUsername = objlogin.CustomerLoginRefresh(Convert.ToInt32(CustomerId));

                

                if (dtUsername.Rows.Count == 0)
                {
                    dr["status"] = "Failed";
                    dr["msg"] = "Username Not Found";
                }
               
                if (dtUsername.Rows.Count > 0)
                {
                    dr["status"] = "Success";
                    dr["msg"] = "Find User";
                    int customerId = Convert.ToInt32(dtUsername.Rows[0]["Id"]);
                    var _customer = objlogin.BindCustomer(CustomerId);

                    if (_customer.Rows.Count > 0)
                    {
                        dr["userid"] = Convert.ToInt32(dtUsername.Rows[0]["Id"]);
                        dr["firstname"] = _customer.Rows[0]["FirstName"].ToString().Trim();
                        dr["lastname"] = _customer.Rows[0]["LastName"].ToString().Trim();
                        dr["mobileno"] = _customer.Rows[0]["MobileNo"].ToString().Trim();
                        if (!string.IsNullOrEmpty(_customer.Rows[0]["Email"].ToString()))
                            dr["email"] = _customer.Rows[0]["Email"].ToString().Trim();
                        else
                            dr["email"] = "";
                        if (!string.IsNullOrEmpty(_customer.Rows[0]["Photo"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(_customer.Rows[0]["Photo"].ToString().Trim());
                            dr["photo"] = Helper.PhotoFolderPath + "/image/customer/" + encoded;
                        }
                        else
                            dr["photo"] = "";
                        if (!string.IsNullOrEmpty(_customer.Rows[0]["SectorId"].ToString()))
                            dr["SectorId"] = Convert.ToInt32(_customer.Rows[0]["SectorId"]);
                        else
                            dr["SectorId"] = 0;
                        if (!string.IsNullOrEmpty(_customer.Rows[0]["ReferralCode"].ToString()))
                            dr["referralcode"] = _customer.Rows[0]["ReferralCode"];
                        else
                            dr["referralcode"] = 0;
                        if (!string.IsNullOrEmpty(_customer.Rows[0]["Address"].ToString()))
                            dr["address"] = _customer.Rows[0]["address"];
                        else
                            dr["address"] = 0;

                        if (!string.IsNullOrEmpty(_customer.Rows[0]["SectorId"].ToString()))
                        {
                            Sector _sector = new Sector();
                            var sector = _sector.getSectorList(Convert.ToInt32(_customer.Rows[0]["SectorId"]));
                            if (sector.Rows.Count > 0)
                                dr["SectorName"] = sector.Rows[0]["SectorName"];
                        }

                        dr["buildingname"] = "";
                        dr["flatno"] = "";

                      
                    }
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



        //[Route("api/CustomerApi/UpdateAddress/{CustomerId}/{Lat}/{Lon}")]
        //[HttpGet]
        public IHttpActionResult UpdateAddress(Customer item)
        {
            string Status = ""; string jsonString = null; string str1 = null; string str2 = null;

            Customer obj = new Customer();

            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

            obj.CustomerId = item.CustomerId;
            obj.lat = item.lat;
            obj.lon = item.lon;
           
            //Customer response = new Customer();
            //response.status = "Failed";
            try
            {


                int favourite = obj.UpdateAddress(obj.CustomerId,obj.lat,obj.lon);
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




        [Route("api/CustomerApi/NewCustomerMsg/{StateId?}")]
        [HttpGet]
        public HttpResponseMessage NewCustomerMsg(int? StateId)
        {
            Customer objlogin = new Customer();

            DataTable dtUser = new DataTable();
            DataTable dtNew = new DataTable();

          
           

            if (!string.IsNullOrEmpty(StateId.ToString()) )
            {
                DataTable dtUsername = new DataTable();
                dtUsername = objlogin.NewCustomerMsg(Convert.ToInt32(StateId));
             

                if (dtUsername.Rows.Count > 0)
                {
                    
                    
                    //dtNew.Columns.Add("status", typeof(string));
                    //dtNew.Columns.Add("msg", typeof(string));

                    dtNew.Columns.Add("StateId", typeof(Int64));
                    dtNew.Columns.Add("Message", typeof(string));
                    dtNew.Columns.Add("ContactNo", typeof(string));
                    dtNew.Columns.Add("WhatsAppNo", typeof(string));
                    dtNew.Columns.Add("Type", typeof(string));
                    dtNew.Columns.Add("Status", typeof(string));


                    for (int i=0;i<dtUsername.Rows.Count;i++)
                    {
                        DataRow dr = dtNew.NewRow();
                        dr["StateId"] = Convert.ToInt32(dtUsername.Rows[i]["StateId"]);
                        dr["Message"] = dtUsername.Rows[i]["Message"].ToString().Trim();

                        dr["ContactNo"] = dtUsername.Rows[i]["MobileNo"].ToString().Trim();
                        dr["WhatsAppNo"] = dtUsername.Rows[i]["Whatsappno"].ToString().Trim();
                        dr["Type"] = dtUsername.Rows[i]["Type"].ToString().Trim();
                        dr["Status"] = dtUsername.Rows[i]["IsActive"].ToString().Trim();

                        dtNew.Rows.Add(dr);
                    }




                    string jsonString = string.Empty;
                    jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                    var dict = new Dictionary<string, string>();
                    dict["status"] = "success";
                    dict["MsgDetail"] = jsonString;


                    string one = @"{""status"":""success""";
                    string two = @",""MsgDetail"":" + dict["MsgDetail"];
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
                    dict["MsgDetail"] = jsonString;


                    string one = @"{""status"":""Fail""";
                    string two = @",""MsgDetail"":" + dict["MsgDetail"];
                    string three = one + two + "}";

                    var str = three.ToString().Replace(@"\", "");
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                    return response;
                }
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
                dict["MsgDetail"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""MsgDetail"":" + dict["MsgDetail"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            //dtNew.Rows.Add(dr);
            //string jsonString = string.Empty;
            //jsonString = JsonConvert.SerializeObject(dtNew);

            //var response = Request.CreateResponse(HttpStatusCode.OK);
            //response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            //return response;
        }



        [Route("api/CustomerApi/SectorMessage/{SectorId?}")]
        [HttpGet]
        public HttpResponseMessage SectorMessage(int? SectorId)
        {
            Customer objlogin = new Customer();
            Offer objoffer = new Offer();
            DataTable dtUser = new DataTable();
            DataTable dtNew = new DataTable();

            //dtNew.Columns.Add("status", typeof(string));
            //dtNew.Columns.Add("msg", typeof(string));
            ////   dtNew.Columns.Add("error_msg", typeof(string));
            //dtNew.Columns.Add("SectorId", typeof(Int64));
            //dtNew.Columns.Add("SectorName", typeof(string));
            //dtNew.Columns.Add("Message", typeof(string));
           
            //dtNew.Columns.Add("Status", typeof(string));

            //DataRow dr = dtNew.NewRow();

            if (!string.IsNullOrEmpty(SectorId.ToString()))
            {
                DataTable dtUsername = new DataTable();
                dtUsername = objoffer.getSectorMessageList(Convert.ToInt32(SectorId));

                int userRecords = dtUsername.Rows.Count;
                if (userRecords > 0)
                {
                    dtNew.Columns.Add("SectorId", typeof(Int64));
                    dtNew.Columns.Add("SectorName", typeof(string));
                    dtNew.Columns.Add("Message", typeof(string));
                    dtNew.Columns.Add("Status", typeof(string));
                   

                    for (int i = 0; i < dtUsername.Rows.Count; i++)
                    {
                        //Status = "0";
                        DataRow dr = dtNew.NewRow();
                        // dr["status"] = "Success";
                        dr["SectorId"] = dtUsername.Rows[i]["SectorId"].ToString().Trim();
                        dr["SectorName"] = dtUsername.Rows[i]["SectorName"].ToString().Trim();
                        dr["Message"] = dtUsername.Rows[i]["Message"].ToString().Trim();
                        dr["Status"] = dtUsername.Rows[i]["IsActive"].ToString().Trim();
                        

                        dtNew.Rows.Add(dr);
                    }
                    string jsonString = string.Empty;
                    jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                    var dict = new Dictionary<string, string>();
                    dict["status"] = "success";
                    dict["MsgDetail"] = jsonString;


                    string one = @"{""status"":""success""";
                    string two = @",""MsgDetail"":" + dict["MsgDetail"];
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

                   string  jsonString = string.Empty;
                    jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                    var dict = new Dictionary<string, string>();
                    dict["status"] = "Fail";
                    dict["MsgDetail"] = jsonString;


                    string one = @"{""status"":""Fail""";
                    string two = @",""MsgDetail"":" + dict["MsgDetail"];
                    string three = one + two + "}";

                    var str = three.ToString().Replace(@"\", "");
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                    return response;
                }



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
                dict["MsgDetail"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""MsgDetail"":" + dict["MsgDetail"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }
        //dtNew.Rows.Add(dr);
        //string jsonString = string.Empty;
        //jsonString = JsonConvert.SerializeObject(dtNew);
        ////jsonString = jsonString.Replace(@"[", "");
        //// jsonString = jsonString.Replace(@"]", "");

        //// ViewBag.Status = jsonString;
        //// return jsonString;
        //var response = Request.CreateResponse(HttpStatusCode.OK);
        //response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
        //return response;



        [Route("api/CustomerApi/GetNotification/{Username}/{FDate?}/{TDate?}")]
        [HttpGet]
        public HttpResponseMessage GetNotification(string Username, DateTime? FDate, DateTime? TDate)
        {
            DateTime Fromdate = DateTime.Now, Todate = DateTime.Now;

            Fromdate = Convert.ToDateTime(FDate);
            Todate = Convert.ToDateTime(TDate);
            Customer objlogin = new Customer();

            DataTable dtUser = new DataTable();
            DataTable dtNew = new DataTable();




            if (!string.IsNullOrEmpty(Username.ToString()))
            {
                DataTable dtUsername = new DataTable();
                dtUsername = objlogin.CustomerNotification(Username, Fromdate, Todate);


                if (dtUsername.Rows.Count > 0)
                {


                    //dtNew.Columns.Add("status", typeof(string));
                    //dtNew.Columns.Add("msg", typeof(string));

                    dtNew.Columns.Add("Id", typeof(Int64));
                    dtNew.Columns.Add("Title", typeof(string));
                    dtNew.Columns.Add("Notification", typeof(string));
                    dtNew.Columns.Add("NewLink", typeof(string));
                    dtNew.Columns.Add("Image", typeof(string));
                    dtNew.Columns.Add("UpdatedOn", typeof(string));
                    dtNew.Columns.Add("UserId", typeof(string));
                    dtNew.Columns.Add("Status", typeof(string));
                    dtNew.Columns.Add("NotificationId", typeof(string));
                    for (int i = 0; i < dtUsername.Rows.Count; i++)
                    {
                        DataRow dr = dtNew.NewRow();
                        dr["Id"] = Convert.ToInt32(dtUsername.Rows[i]["Id"]);
                        dr["Title"] = dtUsername.Rows[i]["Title"].ToString().Trim();

                        dr["Notification"] = dtUsername.Rows[i]["Notification"].ToString().Trim();
                        dr["NewLink"] = dtUsername.Rows[i]["NewLink"].ToString().Trim();

                        if (!string.IsNullOrEmpty(dtUsername.Rows[i]["Image"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtUsername.Rows[i]["Image"].ToString().Trim());
                            dr["Image"] = Helper.PhotoFolderPath + "/image/Notification/" + encoded;
                        }
                        else
                            dr["Image"] = "";
                        // dr["Image"] = dtUsername.Rows[i]["Image"].ToString().Trim();
                        dr["UpdatedOn"] = dtUsername.Rows[i]["UpdatedOn"].ToString().Trim();
                        dr["UserId"] = dtUsername.Rows[i]["UserId"].ToString().Trim();
                        dr["Status"] = dtUsername.Rows[i]["Status"].ToString().Trim();
                        dr["NotificationId"] = dtUsername.Rows[i]["NotificationId"].ToString().Trim();
                        dtNew.Rows.Add(dr);
                    }




                    string jsonString = string.Empty;
                    jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                    var dict = new Dictionary<string, string>();
                    dict["status"] = "success";
                    dict["MsgDetail"] = jsonString;


                    string one = @"{""status"":""success""";
                    string two = @",""MsgDetail"":" + dict["MsgDetail"];
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
                    dict["MsgDetail"] = jsonString;


                    string one = @"{""status"":""Fail""";
                    string two = @",""MsgDetail"":" + dict["MsgDetail"];
                    string three = one + two + "}";

                    var str = three.ToString().Replace(@"\", "");
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                    return response;
                }
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
                dict["MsgDetail"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""MsgDetail"":" + dict["MsgDetail"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
            //dtNew.Rows.Add(dr);
            //string jsonString = string.Empty;
            //jsonString = JsonConvert.SerializeObject(dtNew);

            //var response = Request.CreateResponse(HttpStatusCode.OK);
            //response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            //return response;
        }


        [Route("api/CustomerApi/UpdateNotificationSeen/{Username}/{NotId?}")]
        [HttpGet]
        public HttpResponseMessage UpdateNotificationSeen(string Username, string NotId)
        {
            string Status = ""; string jsonString = null; string str1 = null; string str2 = null;
            int favourite = 0;
            Customer obj = new Customer();
            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            DataTable dtNew = new DataTable();

            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();


            CashbackSeenResponse response = new CashbackSeenResponse();
            response.status = "Failed";
            try
            {
                foreach (string s in NotId.Split(delimiter))
                {
                    favourite = obj.UpdateCustomerNotificationseen(Username, Convert.ToInt32(s));
                }


                if (favourite > 0)
                {
                    response.status = "Success";
                    response.error_msg = "Notification Seen...";
                }
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                response.error_msg = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, response);


        }


        [Route("api/CustomerApi/GetTerms")]
        [HttpGet]
        public HttpResponseMessage GetTerms()
        {
            Customer objlogin = new Customer();

            DataTable dtUser = new DataTable();
            DataTable dtNew = new DataTable();





            DataTable dtUsername = new DataTable();
            dtUsername = objlogin.GetTerms();


            if (dtUsername.Rows.Count > 0)
            {


                //dtNew.Columns.Add("status", typeof(string));
                //dtNew.Columns.Add("msg", typeof(string));

                dtNew.Columns.Add("Id", typeof(Int64));
                dtNew.Columns.Add("Pos", typeof(string));
                dtNew.Columns.Add("Terms", typeof(string));


                for (int i = 0; i < dtUsername.Rows.Count; i++)
                {
                    DataRow dr = dtNew.NewRow();
                    dr["Id"] = Convert.ToInt32(dtUsername.Rows[i]["Id"]);
                    dr["Pos"] = dtUsername.Rows[i]["Pos"].ToString().Trim();

                    dr["Terms"] = dtUsername.Rows[i]["terms"].ToString().Trim();

                    dtNew.Rows.Add(dr);
                }




                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                var dict = new Dictionary<string, string>();
                dict["status"] = "success";
                dict["MsgDetail"] = jsonString;


                string one = @"{""status"":""success""";
                string two = @",""MsgDetail"":" + dict["MsgDetail"];
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
                dict["MsgDetail"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""MsgDetail"":" + dict["MsgDetail"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }

            //dtNew.Rows.Add(dr);
            //string jsonString = string.Empty;
            //jsonString = JsonConvert.SerializeObject(dtNew);

            //var response = Request.CreateResponse(HttpStatusCode.OK);
            //response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            //return response;
        }




        [Route("api/CustomerApi/GetInvoice/{CustomerId?}/{FromDate?}/{ToDate?}")]
        [HttpGet]
        public HttpResponseMessage GetInvoice(string CustomerId, DateTime? FromDate, DateTime? ToDate)
        {
            Customer objlogin = new Customer();
            Subscription obj = new Subscription();
            DataTable dtNew = new DataTable();
            DataTable dtNewItem = new DataTable();
            DataTable dtprodRecord = new DataTable();
            DataTable dtUser = new DataTable();
            DateTime FDate = DateTime.Now; DateTime TDate = DateTime.Now;
            string date1 = "", date2 = "";
            // DateTime displaydate1= Convert.ToDateTime(DateTime.ParseExact(FromDate.ToString(), @"dd/MM/yyyy", null));
            //DateTime displaydate2 = Convert.ToDateTime(DateTime.ParseExact(ToDate.ToString(), @"dd/MM/yyyy", null));
            if (!string.IsNullOrEmpty(FromDate.ToString()))
            {
                FDate = Convert.ToDateTime(FromDate);
                //var date = FDate.Date;
                //FromDate = date.Date.AddHours(00).AddMinutes(00).AddSeconds(00);
                var formattedDate = $"{FromDate:dd-MMM-yyyy}";
                date1 = formattedDate.ToString();
            }
            if (!string.IsNullOrEmpty(ToDate.ToString()))
            {
                TDate = Convert.ToDateTime(ToDate);
                var formattedDate = $"{ToDate:dd-MMM-yyyy}";

                date2 = formattedDate.ToString();
                //var date1 = TDate.Date;
                // ToDate = date1.Date.AddHours(00).AddMinutes(00).AddSeconds(00);
            }



            DataTable dtUsername = new DataTable();
            dtUsername = objlogin.GetCustomerbyId(Convert.ToInt32(CustomerId));

            string str = "";

            if (dtUsername.Rows.Count > 0)
            {
                decimal subtotal = 0;
                decimal totaltax = 0;
                DataTable dtitemRecord = new DataTable();
                dtitemRecord = objlogin.getCustomerOrderDetail(Convert.ToInt32(CustomerId), FromDate, ToDate);

                str += "<html><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"> <title></title>";
                str += "<style>";
                str += "@@page {size: A4; margin: 10mm 10mm 10mm 10mm;}";
                str += "body {font-family: 'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;text-align: center; color: #777;}";
                str += "body h1{ margin-bottom: 0px;padding-bottom: 0px;color: #000;}";
                str += "body h3{ font-weight: 300;margin-top: 10px;margin-bottom: 20px;color: #555;}";
                str += ".main-box{max-width: 800px;margin: auto;padding: 30px;font-size: 15px;line-height: 24px;font-family: 'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;color: #000;background-image: url(https://milkywayindia.com/img/cow-pattern.png);background-size: contain;}";
                str += ".main-box table td{ padding: 3px;}";
                str += "</style>";
                str += "</head>";
                str += "<body>";
                str += "<div id='printarea' class=\"main-box\"> <div style=\"background-color:#fff;\">";
                str += "<table style=\"width:100%; background-color:#ffffff\">";
                str += "<td style=\"width:50%;\"><img src=\"https://milkywayindia.com/img/logo.png\" style=\"width:100px;height:100px;\" /> </td>";
                str += " <td style=\"width:50%; text-align:right; \">";
                str += "<h1>REPORT</h1><br />";
                str += "<span><b>DATE:</b>" + date1.ToString() + " to " + date2.ToString() + "</span><br />";
                str += "</td> </tr>";

                str += " <tr>";
                str += "<td style=\"width:50%\"><h3>BILL TO</h3> <hr /><span><b>" + dtUsername.Rows[0]["FirstName"].ToString() + " " + dtUsername.Rows[0]["LastName"].ToString() + "</b></span> <br />";
                str += "<span><b>" + dtUsername.Rows[0]["Address"].ToString() + "</b></span><br />";
                str += "<span><b>+91 " + dtUsername.Rows[0]["MobileNo"].ToString() + "</b></span><br />";
                str += "<span><b></b></span><br /> </td></tr></table>";
                str += "<table style=\"width:100%; background-color:#ffffff\">";
                str += " <tr style=\"background-color:#777;color:#fff\"><td colspan=\"6\" style=\"padding:5px;border-radius:5px;\">Product Section</td></tr><br/>";
                str += " <tr style=\"color:#777\">";
                str += " <td style=\"width:10%;font-weight:bold;\"> Sl.</td><td style=\"width:30%;font-weight:bold;\">DESCRIPTION</td><td style=\"width:15%;font-weight:bold;\">PRICE</td><td style=\"width:15%;font-weight:bold;\">GST</td><td style=\"width:15%;font-weight:bold;\">QTY</td><td style=\"width:15%;font-weight:bold;\">TOTAL</td> </tr>";

                for (int i = 0; i < dtitemRecord.Rows.Count; i++)
                {
                    int k = i + 1;
                    //string am = dtitemRecord.Rows[i]["Amount"].ToString();
                    //string qty = dtitemRecord.Rows[i]["qty"].ToString();
                    decimal price = Convert.ToDecimal(dtitemRecord.Rows[i]["Amount"].ToString()) / Convert.ToDecimal(dtitemRecord.Rows[i]["qty"].ToString());
                    decimal gst = Convert.ToDecimal(dtitemRecord.Rows[i]["CGSTAmount"].ToString()) + Convert.ToDecimal(dtitemRecord.Rows[i]["SGSTAmount"].ToString());

                    str += "<tr>";
                    str += "<td style=\"width:10%\">" + k.ToString() + "</td>";
                    str += "<td style=\"width:30%\">" + dtitemRecord.Rows[i]["Description"].ToString() + " </td>";
                    str += "<td style=\"width:15%\">" + price.ToString() + " </td>";
                    str += "<td style=\"width:15%\">" + gst.ToString() + " </td>";
                    str += "<td style=\"width:15%\">" + dtitemRecord.Rows[i]["qty"].ToString() + " </td>";
                    str += "<td style=\"width:15%\">" + dtitemRecord.Rows[i]["Amount"].ToString() + " </td>";
                    str += "</tr>";

                    subtotal = subtotal + Convert.ToDecimal(dtitemRecord.Rows[i]["Amount"].ToString());
                    totaltax = totaltax + Convert.ToDecimal(gst.ToString());
                }
                str += "</table>";
                str += " <br />";


                DataTable dtitemBillpay = new DataTable();
                dtitemBillpay = objlogin.getCustomerBillPayDetail(Convert.ToInt32(CustomerId), FromDate, ToDate);
                if (dtitemBillpay.Rows.Count > 0)
                {
                    str += "<table style=\"width:100%; background-color:#ffffff\">";
                    str += " <tr style=\"background-color:#777;color:#fff\"><td colspan=\"6\" style=\"padding:5px;border-radius:5px;\">Recharge & Bill Pay Section</td></tr><br/>";
                    str += " <tr style=\"color:#777\">";
                    str += " <td style=\"width:10%;font-weight:bold;\"> Sl.</td><td style=\"width:75%;font-weight:bold;\">DESCRIPTION</td><td style=\"width:15%;font-weight:bold;\">Amount</td> </tr>";


                    for (int i = 0; i < dtitemBillpay.Rows.Count; i++)
                    {
                        int k = i + 1;

                        str += "<tr>";
                        str += "<td style=\"width:10%\">" + k.ToString() + "</td>";
                        str += "<td style=\"width:75%\">" + dtitemBillpay.Rows[i]["Description"].ToString() + " </td>";


                        str += "<td style=\"width:15%\">" + dtitemBillpay.Rows[i]["Amount"].ToString() + " </td>";
                        str += "</tr>";
                    }

                    str += "</table>";
                    str += "<br/>";
                }

                DataTable dtitemAddWallet = new DataTable();
                dtitemAddWallet = objlogin.getCustomerAddWallet(Convert.ToInt32(CustomerId), FromDate, ToDate);
                if (dtitemAddWallet.Rows.Count > 0)
                {
                    str += "<table style=\"width:100%; background-color:#ffffff\">";
                    str += " <tr style=\"background-color:#777;color:#fff\"><td colspan=\"6\" style=\"padding:5px;border-radius:5px;\">Wallet Balance Update Detail Section</td></tr><br/>";
                    str += " <tr style=\"color:#777\">";
                    str += " <td style=\"width:10%;font-weight:bold;\"> Sl.</td><td style=\"width:75%;font-weight:bold;\">Date</td><td style=\"width:15%;font-weight:bold;\">Amount</td> </tr>";


                    for (int i = 0; i < dtitemAddWallet.Rows.Count; i++)
                    {
                        int k = i + 1;

                        str += "<tr>";
                        str += "<td style=\"width:10%\">" + k.ToString() + "</td>";
                        str += "<td style=\"width:75%\">" + dtitemAddWallet.Rows[i]["TransactionDate"].ToString() + " </td>";


                        str += "<td style=\"width:15%\">" + dtitemAddWallet.Rows[i]["Amount"].ToString() + " </td>";
                        str += "</tr>";
                    }

                    str += "</table>";
                    str += "<br/>";
                }



                str += "<table style=\"width:100%; background-color:#fff\">";
                str += "<tr>";
                str += "<td style=\"text-align:right; width:80%;\">";
                str += "<span class=\"fontstyle\"><b>SUBTOTAL</b></span>";
                str += " </td>";
                str += "<td style=\"text-align:right; width:20%;\">";
                str += " <span>" + subtotal.ToString() + "</span>";
                str += "</td>";
                str += "</tr>";
                //
                str += "<tr>";
                str += "<td style=\"text-align:right; width:80%;\">";
                str += "<span class=\"fontstyle\"><b>DISCOUNT</b></span>";
                str += " </td>";
                str += "<td style=\"text-align:right; width:20%;\">";
                str += " <span>0.0</span>";
                str += "</td>";
                str += "</tr>";

                str += "<tr>";
                str += "<td style=\"text-align:right; width:80%;\">";
                str += "<span class=\"fontstyle\"><b>SUBTOTAL LESS DISCOUNT</b></span>";
                str += " </td>";
                str += "<td style=\"text-align:right; width:20%;\">";
                str += " <span>0.0</span>";
                str += "</td>";
                str += "</tr>";


                str += "<tr>";
                str += "<td style=\"text-align:right; width:80%;\">";
                str += "<span class=\"fontstyle\"><b>TAX RATE</b></span>";
                str += " </td>";
                str += "<td style=\"text-align:right; width:20%;\">";
                str += " <span>0.0</span>";
                str += "</td>";
                str += "</tr>";

                str += "<tr>";
                str += "<td style=\"text-align:right; width:80%;\">";
                str += "<span class=\"fontstyle\"><b>TOTAL TAX</b></span>";
                str += " </td>";
                str += "<td style=\"text-align:right; width:20%;\">";
                str += " <span>" + totaltax.ToString() + "</span>";
                str += "</td>";
                str += "</tr>";

                str += "<tr>";
                str += "<td style=\"text-align:right; width:80%;\">";
                str += "<span class=\"fontstyle\"><b>SHIPPING HANDLING</b></span>";
                str += " </td>";
                str += "<td style=\"text-align:right; width:20%;\">";
                str += " <span>0.0</span>";
                str += "</td>";
                str += "</tr>";
                str += "</table><br/>";
                //



                str += " <table style=\"width:100%; background-color:#fff;\">";
                str += "<tr>";
                str += "<td style=\"text-align:left;\">";
                str += "<span class=\"fontstyle\"><b>THANK YOU !!!</b></span> <br />";
                str += "<span class=\"fontstyle\">For your Purchase & being part of 'Milkyway India family'. </span>";
                str += "</td>";
                str += "</tr>";
                str += " </table>";
                str += " </div></div>";
                str += "</body>";
                str += "</html>";
                //var response = Request.CreateResponse(HttpStatusCode.OK);
                //response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                //string response = str1;
                //return Ok(response);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }

            else
            {
                dtNew.Columns.Add("status", typeof(string));
                dtNew.Columns.Add("msg", typeof(string));
                DataRow dr = dtNew.NewRow();


                str = "";

                // var response = str;
                //  return Ok(response);

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }

            //dtNew.Rows.Add(dr);
            //string jsonString = string.Empty;
            //jsonString = JsonConvert.SerializeObject(dtNew);

            //var response = Request.CreateResponse(HttpStatusCode.OK);
            //response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            //return response;
        }


        [Route("api/CustomerApi/OfferVendorList/{SectorId?}/{Type}")]
        [HttpGet]
        public HttpResponseMessage OfferVendorList(int? SectorId,string Type)
        {
            Customer objlogin = new Customer();
            Offer objoffer = new Offer();
            DataTable dtUser = new DataTable();
            DataTable dtNew = new DataTable();

            if (!string.IsNullOrEmpty(SectorId.ToString()))
            {
                DataTable dtUsername = new DataTable();
                dtUsername = objlogin.getOfferVendor(Convert.ToInt32(SectorId),Type);

                int userRecords = dtUsername.Rows.Count;
                if (userRecords > 0)
                {
                    dtNew.Columns.Add("SectorId", typeof(Int64));
                    dtNew.Columns.Add("SectorName", typeof(string));
                    dtNew.Columns.Add("VendorId", typeof(string));
                    dtNew.Columns.Add("VendorName", typeof(string));
                    dtNew.Columns.Add("Logo", typeof(string));
                    dtNew.Columns.Add("MobileNo", typeof(string));
                    dtNew.Columns.Add("Vendorstore", typeof(string));
                    dtNew.Columns.Add("Slider1", typeof(string));
                    dtNew.Columns.Add("Slider2", typeof(string));
                    dtNew.Columns.Add("DiscountPer", typeof(string));
                    dtNew.Columns.Add("Vendorterms", typeof(string));
                    
                    dtNew.Columns.Add("Status", typeof(string));


                    for (int i = 0; i < dtUsername.Rows.Count; i++)
                    {
                        //Status = "0";
                        DataRow dr = dtNew.NewRow();
                        // dr["status"] = "Success";
                        dr["SectorId"] = dtUsername.Rows[i]["SectorId"].ToString().Trim();
                        dr["SectorName"] = dtUsername.Rows[i]["SectorName"].ToString().Trim();
                        dr["VendorId"] = dtUsername.Rows[i]["Id"].ToString().Trim();
                        dr["VendorName"] = dtUsername.Rows[i]["FirstName"].ToString().Trim() +" "+ dtUsername.Rows[i]["LastName"].ToString().Trim();

                        //

                        if (!string.IsNullOrEmpty(dtUsername.Rows[i]["Logo"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtUsername.Rows[i]["Logo"].ToString().Trim());
                            dr["Logo"] = Helper.PhotoFolderPath + "/image/Vendor/" + encoded;
                        }
                        else
                            dr["Logo"] = "";
                        //
                        
                        dr["MobileNo"] = dtUsername.Rows[i]["MobileNo"].ToString().Trim();
                        dr["Vendorstore"] = dtUsername.Rows[i]["Vendorstore"].ToString().Trim();

                        if (!string.IsNullOrEmpty(dtUsername.Rows[i]["Slider1"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtUsername.Rows[i]["Slider1"].ToString().Trim());
                            dr["Slider1"] = Helper.PhotoFolderPath + "/image/Vendor/" + encoded;
                        }
                        else
                            dr["Slider1"] = "";

                        if (!string.IsNullOrEmpty(dtUsername.Rows[i]["Slider2"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtUsername.Rows[i]["Slider2"].ToString().Trim());
                            dr["Slider2"] = Helper.PhotoFolderPath + "/image/Vendor/" + encoded;
                        }
                        else
                            dr["Slider2"] = "";
                       
                        dr["DiscountPer"] = dtUsername.Rows[i]["DiscountPer"].ToString().Trim();
                        dr["Vendorterms"] = dtUsername.Rows[i]["Vendorterms"].ToString().Trim();
                       
                        dr["Status"] = dtUsername.Rows[i]["IsActive"].ToString().Trim();


                        dtNew.Rows.Add(dr);
                    }
                    string jsonString = string.Empty;
                    jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                    var dict = new Dictionary<string, string>();
                    dict["status"] = "success";
                    dict["MsgDetail"] = jsonString;


                    string one = @"{""status"":""success""";
                    string two = @",""MsgDetail"":" + dict["MsgDetail"];
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
                    dict["MsgDetail"] = jsonString;


                    string one = @"{""status"":""Fail""";
                    string two = @",""MsgDetail"":" + dict["MsgDetail"];
                    string three = one + two + "}";

                    var str = three.ToString().Replace(@"\", "");
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                    return response;
                }



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
                dict["MsgDetail"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""MsgDetail"":" + dict["MsgDetail"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }


        [Route("api/CustomerApi/OfferProductVendorWise/{SectorId?}/{VendorId?}")]
        [HttpGet]
        public HttpResponseMessage OfferProductVendorWise(int? SectorId,int? VendorId)
        {
            Customer objlogin = new Customer();
            Offer objoffer = new Offer();
            DataTable dtUser = new DataTable();
            DataTable dtNew = new DataTable();

        
            if (!string.IsNullOrEmpty(SectorId.ToString()))
            {
                DataTable dtUsername = new DataTable();
                dtUsername = objlogin.getVendorOfferList(Convert.ToInt32(SectorId),VendorId);

                int userRecords = dtUsername.Rows.Count;
                if (userRecords > 0)
                {
                    
                    
                   
                   
                    dtNew.Columns.Add("ProductId", typeof(string));
                    
                    dtNew.Columns.Add("ValidFrom", typeof(string));
                    dtNew.Columns.Add("ValidTo", typeof(string));
                    dtNew.Columns.Add("OfferType", typeof(string));
                    dtNew.Columns.Add("OfferCat", typeof(string));
                    dtNew.Columns.Add("OfferDates", typeof(string));
                    dtNew.Columns.Add("OfferValue", typeof(string));
                   
                    dtNew.Columns.Add("Status", typeof(string));


                    for (int i = 0; i < dtUsername.Rows.Count; i++)
                    {
                        //Status = "0";
                        DataRow dr = dtNew.NewRow();
                        // dr["status"] = "Success";
                        dr["ProductId"] = dtUsername.Rows[i]["ProductId"].ToString().Trim();
                        dr["ValidFrom"] = dtUsername.Rows[i]["Validfrom"].ToString().Trim();
                        dr["ValidTo"] = dtUsername.Rows[i]["ValidTo"].ToString().Trim();
                        dr["OfferType"] = dtUsername.Rows[i]["OfferType"].ToString().Trim();
                        dr["OfferCat"] = dtUsername.Rows[i]["OfferCat"].ToString().Trim();
                        dr["OfferDates"] = dtUsername.Rows[i]["offerdates"].ToString().Trim();
                        dr["OfferValue"] = dtUsername.Rows[i]["OfferValue"].ToString().Trim();
                        dr["Status"] = dtUsername.Rows[i]["IsActive"].ToString().Trim();


                        dtNew.Rows.Add(dr);
                    }
                    string jsonString = string.Empty;
                    jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                    var dict = new Dictionary<string, string>();
                    dict["status"] = "success";
                    dict["MsgDetail"] = jsonString;


                    string one = @"{""status"":""success""";
                    string two = @",""MsgDetail"":" + dict["MsgDetail"];
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
                    dict["MsgDetail"] = jsonString;


                    string one = @"{""status"":""Fail""";
                    string two = @",""MsgDetail"":" + dict["MsgDetail"];
                    string three = one + two + "}";

                    var str = three.ToString().Replace(@"\", "");
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                    return response;
                }



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
                dict["MsgDetail"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""MsgDetail"":" + dict["MsgDetail"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }


        [Route("api/CustomerApi/OfferVendorProductList/{SectorId?}/{VendorId?}")]
        [HttpGet]
        public HttpResponseMessage OfferVendorProductList(int? SectorId, int? VendorId)
        {
            Customer objlogin = new Customer();
            Offer objoffer = new Offer();
            DataTable dtUser = new DataTable();
            DataTable dtNew = new DataTable();


            if (!string.IsNullOrEmpty(SectorId.ToString()))
            {
                DataTable dtUsername = new DataTable();
                dtUsername = objlogin.getOfferVendorProductList(Convert.ToInt32(SectorId), VendorId);

                int userRecords = dtUsername.Rows.Count;
                if (userRecords > 0)
                {




                    dtNew.Columns.Add("ProductId", typeof(string));
                    dtNew.Columns.Add("ProductName", typeof(string));
                    dtNew.Columns.Add("SalePrice", typeof(string));
                    dtNew.Columns.Add("Image", typeof(string));
                    dtNew.Columns.Add("Detail", typeof(string));
                    

                    dtNew.Columns.Add("Status", typeof(string));


                    for (int i = 0; i < dtUsername.Rows.Count; i++)
                    {
                        //Status = "0";
                        DataRow dr = dtNew.NewRow();
                        // dr["status"] = "Success";
                        dr["ProductId"] = dtUsername.Rows[i]["Id"].ToString().Trim();
                        dr["ProductName"] = dtUsername.Rows[i]["ProductName"].ToString().Trim();
                        dr["SalePrice"] = dtUsername.Rows[i]["SalePrice"].ToString().Trim();

                        if (!string.IsNullOrEmpty(dtUsername.Rows[i]["Image"].ToString()))
                        {
                            var encoded = Uri.EscapeUriString(dtUsername.Rows[i]["Image"].ToString().Trim());
                            dr["Image"] = Helper.PhotoFolderPath + "/image/Product/" + encoded;
                        }
                        else
                            dr["Image"] = "";
                        
                        dr["Detail"] = dtUsername.Rows[i]["Detail"].ToString().Trim();
                       
                        dr["Status"] = dtUsername.Rows[i]["IsActive"].ToString().Trim();


                        dtNew.Rows.Add(dr);
                    }
                    string jsonString = string.Empty;
                    jsonString = JsonConvert.SerializeObject(dtNew); //new Newtonsoft.Json.Formatting()

                    var dict = new Dictionary<string, string>();
                    dict["status"] = "success";
                    dict["MsgDetail"] = jsonString;


                    string one = @"{""status"":""success""";
                    string two = @",""MsgDetail"":" + dict["MsgDetail"];
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
                    dict["MsgDetail"] = jsonString;


                    string one = @"{""status"":""Fail""";
                    string two = @",""MsgDetail"":" + dict["MsgDetail"];
                    string three = one + two + "}";

                    var str = three.ToString().Replace(@"\", "");
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                    return response;
                }



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
                dict["MsgDetail"] = jsonString;


                string one = @"{""status"":""Fail""";
                string two = @",""MsgDetail"":" + dict["MsgDetail"];
                string three = one + two + "}";

                var str = three.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }
        }


        [Route("api/CustomerApi/OfferVendorProductOrder/{CustomerId?}/{VendorId?}/{ProductId?}/{Qty?}/{OfferId?}")]
        [HttpGet]
        public IHttpActionResult OfferVendorProductOrder(int? CustomerId, int? VendorId,int? ProductId,int? Qty,int? OfferId)
        {
            Customer objlogin = new Customer();
            Offer objoffer = new Offer();
            DataTable dt = new DataTable();
            DataTable dtUser = new DataTable();
            DataTable dtNew = new DataTable();
            Vendor objvendor = new Vendor();
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();
            int r = 0;
            string msg = "Order Placed Successfully";
            string msg1 = "";
            obj.CustomerId =Convert.ToInt32(CustomerId);
            dtNew.Columns.Add("status", typeof(string));
            dtNew.Columns.Add("error_msg", typeof(string));
            DataRow dr = dtNew.NewRow();

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



            DataTable dtProduct = objproduct.BindOfferProuctOrder(ProductId,  VendorId,OfferId);
            if(dtProduct.Rows.Count>0)
            {
                obj.ProductId =Convert.ToInt32(ProductId);
                obj.SalePrice =Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
                obj.CGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
                obj.SGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);
                obj.IGSTPerct = Convert.ToDecimal(dtProduct.Rows[0]["SalePrice"]);

                if (obj.CGSTPerct > 0)
                    obj.CGSTAmount = (obj.Amount * obj.CGSTPerct) / 100;
                else
                    obj.CGSTAmount = 0;

                if (obj.SGSTPerct > 0)
                    obj.SGSTAmount = (obj.Amount * obj.SGSTPerct) / 100;
                else
                    obj.SGSTAmount = 0;

                if (obj.IGSTPerct > 0)
                    obj.IGSTAmount = (obj.Amount * obj.IGSTPerct) / 100;
                else
                    obj.IGSTAmount = 0;

                dt = objlogin.getVendorOfferDetail(OfferId);
                decimal amount = 0;
                if(dt.Rows.Count>0)
                {
                    if(dt.Rows[0]["OfferType"].ToString() =="Offer")
                    {
                        amount = obj.SalePrice - ((obj.SalePrice * Convert.ToDecimal(dt.Rows[0]["OfferValue"]))/100);
                    }

                    if (dt.Rows[0]["OfferType"].ToString() == "Voucher")
                    {
                        amount = obj.SalePrice - Convert.ToDecimal(dt.Rows[0]["OfferValue"]);
                    }
                    obj.Amount = amount;
                }

            }

            return Ok(dtNew);

        }
    }





    #endregion
}


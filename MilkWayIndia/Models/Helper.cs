using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using MilkWayIndia.Abstract;
using MilkWayIndia.Concrete;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MilkWayIndia.Entity;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Script.Serialization;


namespace MilkWayIndia.Models
{
    public static class Extension
    {
        public static string FetchUniquePath(this string directoryPath, string imageName)
        {
            string extension = Path.GetExtension(imageName);
            string fileName = DateTime.UtcNow.Ticks.ToString();

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            int i = 0;
            while (System.IO.File.Exists(directoryPath + "/" + fileName + i + extension))
                i++;

            return (fileName + i + extension);
        }
    }

    public class Helper
    {
     //  public static string MagicSMSKey = "A26ab1931dd8a93d90165ae7abd912d41";
       public static string MagicSMSKey = "Ae8927e386ffd02badbcd8a22de75440e";
       public static string MagicOTPTemplateID = "1207162555527225948";
       //public static string MagicOTPTemplateID = "120716255552722594";
        public static string MagicSender = "MILKWY";
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public static DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        public static string PhotoFolderPath = "https://gatimaan.milkywaysoftware.com";
        public static string PhotoFolderPathtest = "https://gatimaan.milkywaysoftware.com";
        //Test
        //public static string PaytmMerchantID = "NXEpnY32055934299372";
        //public static string PaytmMerchantKey = "j_Vnb_oN6XqIPVI5";
        //public static string PaytmAPI = "https://securegw-stage.paytm.in";

        //Live
        public static string PaytmMerchantID = "RhFYmH27735725379698";
        public static string PaytmMerchantKey = "24SyZ9n@qqoor5jD";
        public static string PaytmAPI = "https://securegw.paytm.in";
        private readonly Random _random = new Random();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
        Subscription _subscription = new Subscription();

        public enum TransactionType
        {
            Deposit = 1,
            Purchase = 2,
            Subscription = 3,
            Cashback=4
        }

        public string PaytmCallback()
        {
            var rawURL = HttpContext.Current.Request.Url.ToString();
            if (rawURL.Contains("admin"))
                return "https://admin.milkywaysoftware.com";
			else if (rawURL.Contains("gatimaan"))
				return "https://gatimaan.milkywaysoftware.com";
			else
                return "http://localhost:4937";
        }
        EFDbContext db = new EFDbContext();
        CustomerOrder _cOrder = new CustomerOrder();
        private ISecPaytm _SecPaytmRepo;
        private ICustomer _CustomerRepo;
        public Helper()
        {
            this._SecPaytmRepo = new SecPaytmRepository();
            this._CustomerRepo = new CustomerRepository();
        }

        public static string RoleName()
        {
            string str = "";
            try
            {
                if (HttpContext.Current.Session["RoleName"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["RoleName"] as string))
                {
                    str = HttpContext.Current.Session["RoleName"].ToString();
                }
            }
            catch { }
            return str;
        }

        private string RemoveExtraHyphen(string text)
        {
            if (text.Contains("__"))
            {
                text = text.Replace("__", "_");
                return RemoveExtraHyphen(text);
            }
            return text;
        }

        private string RemoveDiacritics(string text)
        {
            string Normalize = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= Normalize.Length - 1; i++)
            {
                char c = Normalize[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public string RemoveIllegalCharacters(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            text = text.Replace(":", string.Empty);
            text = text.Replace("/", string.Empty);
            text = text.Replace("?", string.Empty);
            text = text.Replace("#", string.Empty);
            text = text.Replace("[", string.Empty);
            text = text.Replace("]", string.Empty);
            text = text.Replace("@", string.Empty);
            text = text.Replace(",", string.Empty);
            text = text.Replace("\"", string.Empty);
            text = text.Replace("&", string.Empty);
            text = text.Replace(".", string.Empty);
            text = text.Replace("'", string.Empty);
            text = text.Replace("_", string.Empty);
            text = text.Replace("(", string.Empty);
            text = text.Replace(")", string.Empty);
            text = text.Replace(" ", "-");
            text = text.Replace("--", "-");
            text = RemoveDiacritics(text);
            text = RemoveExtraHyphen(text);

            return HttpUtility.UrlEncode(text.ToLower()).Replace("%", string.Empty);
        }

        public static string CurrentLoginUser()
        {
            string str = "";
            try
            {
                if (HttpContext.Current.Session["UserId"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["UserId"] as string))
                {
                    str = HttpContext.Current.Session["UserId"].ToString();
                }
            }
            catch { }
            return str;
        }

        public static void SendOTPSMS(string Msg, string MobileNo)
        {
            SendSMS(Msg, MobileNo, MagicOTPTemplateID);
        }

        public static void SendSMS(string Msg, string MobileNo, string TemplateID)
        {
            try
            {
                string strUrl = "";
                strUrl = string.Format("http://trans.magicsms.co.in/api/v4/?api_key={0}&method=sms&message={1}&to={2}&sender={3}&template_id={4}", MagicSMSKey, Msg, MobileNo,
                           MagicSender, TemplateID);
                WebClient wc = new WebClient();
                var s = wc.DownloadString(strUrl);
            }
            catch { }
        }

        public static DateTime GetMonthFirstDate(DateTime date)
        {
            DateTime firstday = new DateTime(date.Year, date.Month, 1);
            return firstday;
        }

        public static DateTime GetMonthLastDate(DateTime date)
        {
            DateTime lastDate = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
            return lastDate;
        }

        public static string GenerateUniqueCode(int length)
        {
            string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string numbers = "1234567890";

            string characters = numbers;
            characters += alphabets + numbers;
            string id = string.Empty;
            for (int i = 0; i < length; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (id.IndexOf(character) != -1);
                id += character;
            }
            return id;
        }

        public static string GenerateReferenceCode(int length)
        {
            string alphabets = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string numbers = "1234567890";

            string characters = numbers;
            characters += alphabets + numbers;
            string id = string.Empty;
            for (int i = 0; i < length; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (id.IndexOf(character) != -1);
                id += character;
            }
            return id;
        }

        public static string DownloadImageFromUrl(string imageUrl)
        {
            try
            {
                string file = Path.GetFileNameWithoutExtension(imageUrl);
                string extension = Path.GetExtension(imageUrl);
                imageUrl = "http://portal.milkywayindia.com/Uploads/Product/" + imageUrl;
                WebClient wc = new WebClient();
                //wc.Headers.Add("Content-Type", "Image/jpeg");
                wc.Headers.Add("User-Agent: Other");
                string rootPath = HttpContext.Current.Server.MapPath("~/image/product/");
                string fname = file + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
                string fileName = System.IO.Path.Combine(rootPath, fname);
                wc.DownloadFile(imageUrl, fileName);
                return fname;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public List<SelectListItem> GetCategoryList()
        {
            Product objProduct = new Product();
            List<SelectListItem> lstCategory = new List<SelectListItem>();
            var category = objProduct.GetAllMaincategory();
            if (category.Rows.Count > 0)
            {
                for (int i = 0; i < category.Rows.Count; i++)
                {
                    var cName = category.Rows[i]["CategoryName"].ToString();
                    lstCategory.Add(new SelectListItem { Text = cName, Value = category.Rows[i]["Id"].ToString() });
                    var parentId = category.Rows[i]["Id"];
                    if (!string.IsNullOrEmpty(parentId.ToString()))
                    {
                        var sub = objProduct.GetSubMaincategory(Convert.ToInt32(parentId.ToString()));
                        if (sub.Rows.Count > 0)
                        {
                            for (int j = 0; j < sub.Rows.Count; j++)
                            {
                                var subCName = cName + " >> " + sub.Rows[j]["CategoryName"].ToString();
                                lstCategory.Add(new SelectListItem { Text = subCName, Value = sub.Rows[j]["Id"].ToString() });
                            }
                        }
                    }
                }
            }
            return lstCategory;
        }

        public string GetProductOrderTime(int? ProductID)
        {
            string str = " {0} To {1}";
            Product objProduct = new Product();
            var product = objProduct.BindProuct(ProductID);
            if (product.Rows.Count > 0)
            {
                var cID = Convert.ToInt32(product.Rows[0]["CategoryId"].ToString());
                var category = objProduct.BindCategory(cID);
                if (category.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(category.Rows[0]["ParentCategoryId"].ToString()))
                    {
                        cID = Convert.ToInt32(category.Rows[0]["ParentCategoryId"].ToString());
                        var parCategory = objProduct.BindCategory(cID);
                        if (parCategory.Rows.Count > 0)
                        {
                            str = string.Format(str, parCategory.Rows[0]["FromTime"], parCategory.Rows[0]["ToTime"]);
                        }
                    }
                    else
                        str = string.Format(str, category.Rows[0]["FromTime"], category.Rows[0]["ToTime"]);
                }
            }
            return str;
        }

        public string GetProductDeliveryTime(int? ProductID)
        {
            string str = " {0} To {1}";
            Product objProduct = new Product();
            var product = objProduct.BindProuct(ProductID);
            if (product.Rows.Count > 0)
            {
                var cID = Convert.ToInt32(product.Rows[0]["CategoryId"].ToString());
                var category = objProduct.BindCategory(cID);
                if (category.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(category.Rows[0]["ParentCategoryId"].ToString()))
                    {
                        cID = Convert.ToInt32(category.Rows[0]["ParentCategoryId"].ToString());
                        var parCategory = objProduct.BindCategory(cID);
                        if (parCategory.Rows.Count > 0)
                        {
                            str = string.Format(str, parCategory.Rows[0]["DeliveryFrom"], parCategory.Rows[0]["DeliveryTo"]);
                        }
                    }
                    else
                        str = string.Format(str, category.Rows[0]["DeliveryFrom"], category.Rows[0]["DeliveryTo"]);
                }
            }
            return str;
        }

        public void UpdateSectorID()
        {
            clsCommon _clsCommon = new clsCommon();
            var customer = _clsCommon.select("*", "tbl_Customer_Master");
            if (customer.Rows.Count > 0)
            {
                for (int i = 0; i < customer.Rows.Count; i++)
                {
                    int custID = Convert.ToInt32(customer.Rows[i]["Id"].ToString());
                    if (!string.IsNullOrEmpty(customer.Rows[i]["BuildingId"].ToString()))
                    {
                        Sector objsector = new Sector();
                        var bID = Convert.ToInt32(customer.Rows[i]["BuildingId"].ToString());
                        var building = objsector.getBuildingList(bID);
                        if (building.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(building.Rows[0]["SectorId"].ToString()))
                            {
                                var sectorId = Convert.ToInt32(building.Rows[0]["SectorId"]);
                                string s = "update tbl_Customer_Master set SectorId='" + sectorId + "' where id='" + custID + "'";
                                _clsCommon.update(s);
                            }
                        }
                    }
                }
            }
        }

        public void UpdateCustomerReferalCode()
        {
            clsCommon _clsCommon = new clsCommon();
            var customer = _clsCommon.selectwhere("*", "tbl_Customer_Master", "ReferralCode Is null");
            if (customer.Rows.Count > 0)
            {
                for (int i = 0; i < customer.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(customer.Rows[i]["ReferralCode"].ToString()))
                    {
                        string refe = customer.Rows[i]["ReferralCode"].ToString();
                        int custID = Convert.ToInt32(customer.Rows[i]["Id"].ToString());
                        var referralCode = GenerateUniqueCode(8);
                        var checkCode = _clsCommon.selectwhere("*", "tbl_Customer_Master", "ReferralCode='" + referralCode + "'");
                        if (checkCode.Rows.Count > 0)
                        {
                            referralCode = GenerateUniqueCode(8);
                        }
                        else
                        {
                            string s = "update tbl_Customer_Master set ReferralCode='" + referralCode + "' where id='" + custID + "'";
                            _clsCommon.update(s);
                        }
                    }
                }
            }
        }

        public void UpdateCustomerAddress()
        {
            clsCommon _clsCommon = new clsCommon();
            var customer = _clsCommon.selectwhere("*", "tbl_Customer_Master", "address is not null");
            if (customer.Rows.Count > 0)
            {
                for (int i = 0; i < customer.Rows.Count; i++)
                {
                    try
                    {
                        var Address = "";
                        if (!string.IsNullOrEmpty(customer.Rows[i]["FlatId"].ToString()))
                        {
                            int flatId = Convert.ToInt32(customer.Rows[i]["FlatId"].ToString());
                            var flat = _clsCommon.selectwhere("*", "tbl_Building_Flat_Master", "Id='" + flatId + "'");
                            for (int j = 0; j < flat.Rows.Count; j++)
                            {
                                Address = flat.Rows[0]["FlatNo"].ToString();
                            }
                        }
                        if (!string.IsNullOrEmpty(customer.Rows[i]["BuildingId"].ToString()))
                        {
                            int builId = Convert.ToInt32(customer.Rows[i]["BuildingId"].ToString());
                            var building = _clsCommon.selectwhere("*", "tbl_Building_Master", "Id='" + builId + "'");
                            for (int j = 0; j < building.Rows.Count; j++)
                            {
                                Address += building.Rows[0]["BlockNo"] == null ? "" : "-" + building.Rows[0]["BlockNo"];
                                Address += building.Rows[0]["BuildingName"] == null ? "" : ", " + building.Rows[0]["BuildingName"];
                                Address += ", ";
                            }

                            int custID = Convert.ToInt32(customer.Rows[i]["Id"].ToString());
                            string s = "update tbl_Customer_Master set Address='" + Address + "' where id='" + custID + "'";
                            _clsCommon.update(s);
                        }
                    }
                    catch { }
                }
            }
        }

        public string GenerateCustomerReferalCode()
        {
            var referralCode = GenerateUniqueCode(8);
            clsCommon _clsCommon = new clsCommon();
            var checkCode = _clsCommon.selectwhere("*", "tbl_Customer_Master", "ReferralCode='" + referralCode + "'");
            if (checkCode.Rows.Count > 0)
            {
                referralCode = GenerateUniqueCode(8);
            }
            return referralCode;
        }

        public int GetCustomerIDByReferralCode(string referralCode)
        {
            int customerID = 0;
            clsCommon _clsCommon = new clsCommon();
            var checkCode = _clsCommon.selectwhere("*", "tbl_Customer_Master", "ReferralCode='" + referralCode + "'");
            if (checkCode.Rows.Count > 0)
            {
                customerID = Convert.ToInt32(checkCode.Rows[0]["Id"].ToString());
            }
            return customerID;
        }

        public class UserControl
        {
            public Boolean IsView { get; set; }
            public Boolean IsAdd { get; set; }
            public Boolean IsDeleted { get; set; }
            public Boolean IsEdit { get; set; }
            public Boolean IsAdmin { get; set; }
            public Boolean IsLogin { get; set; }
        }

        public static UserControl CheckPermission(string url)
        {
            url = url.ToLower();
            UserControl control = new UserControl();
            control.IsAdd = false;
            control.IsDeleted = false;
            control.IsView = false;
            control.IsEdit = false;
            control.IsAdmin = false;
            control.IsLogin = false;


            //HttpContext.Response.Cookies["gstusr"].Values["key"] = "10";
            
            if (HttpContext.Current.Session["UserId"] != null && HttpContext.Current.Session["RoleName"].ToString()=="Admin")
            {
                control.IsAdd = true;
                control.IsDeleted = true;
                control.IsView = true;
                control.IsEdit = true;
                control.IsAdmin = true;
                control.IsLogin = true;
                //return control;
            }
            
            //string b = HttpContext.Current.Request.Cookies["gstusr"].Values["key"].ToString();
                if (HttpContext.Current.Request.Cookies["gstusr"] == null)
                return control;
			else
			{
				try
				{
					var roleName = "";

					if (HttpContext.Current.Request.Cookies["gstusr"].Values["key"] != null)
					{
						var id = HttpContext.Current.Request.Cookies["gstusr"].Values["key"];

						// Try from Staff table
						Staff _staff = new Staff();
						var staff = _staff.getStaffList(Convert.ToInt32(id));
						if (staff != null && staff.Rows.Count > 0)
						{
							roleName = staff.Rows[0]["Role"] == null ? "" : staff.Rows[0]["Role"].ToString();
						}
						if (string.IsNullOrEmpty(roleName))
						{
							Vendor _vendor = new Vendor(); 
							var vendor = _vendor.getVendorList(Convert.ToInt32(id));
							if (vendor != null && vendor.Rows.Count > 0)
							{
								roleName = vendor.Rows[0]["VendorStatus"] == null ? "" : vendor.Rows[0]["VendorStatus"].ToString();
							}
						}
					}
					if (string.IsNullOrEmpty(roleName))
						return control;

					if (roleName.ToLower() == "admin" || roleName.ToLower() == "approved")
					{
						control.IsAdmin = true;
						control.IsAdd = true;
						control.IsDeleted = true;
						control.IsView = true;
						control.IsEdit = true;
					}

					return control;
				}
				catch
				{
					return control;
				}
			}


			return control;
        }

        public static string RenderRazorViewToString(Controller controller, string viewName, object model)
        {
            controller.ViewBag.Model = model;
            controller.ViewData.Model = model;
            var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
            // checking the view inside the controller  
            if (viewResult.View != null)
            {
                using (var sw = new StringWriter())
                {
                    var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                    return sw.GetStringBuilder().ToString();
                }
            }
            else
                return "View cannot be found.";
        }

        public static byte[] GetPDF(string pHTML, string filename)
        {
            byte[] bPDF = null;

            MemoryStream ms = new MemoryStream();
            TextReader txtReader = new StringReader(pHTML);
            // 1: create object of a itextsharp document class  
            Document doc = new Document(PageSize.A4, 25, 25, 25, 25);
            // 2: we create a itextsharp pdfwriter that listens to the document and directs a XML-stream to a file  
            PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);
            // 3: we create a worker parse the document  
            HTMLWorker htmlWorker = new HTMLWorker(doc);
            // 4: we open document and start the worker on the document  
            doc.Open();
            htmlWorker.StartDocument();
            // 5: parse the html into the document  
            htmlWorker.Parse(txtReader);
            // 6: close the document and the worker  
            htmlWorker.EndDocument();
            htmlWorker.Close();
            doc.Close();
            bPDF = ms.ToArray();
            return bPDF;
        }

        public string ScheduleOrder(DateTime? orderDate)
        {
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            int AddWallet = 0;
            DataTable dtCustomerOrderGroup = new DataTable();
            DataTable dtDateOrderSchel = new DataTable();
            DataTable dtGetCustomerPoint = new DataTable();
            dtCustomerOrderGroup = obj.getCustomerOrderScheduleGroupList(orderDate);
            if (dtCustomerOrderGroup.Rows.Count > 0)
            {
                for (int i = 0; i < dtCustomerOrderGroup.Rows.Count; i++)
                {
                    obj.CustomerId = Convert.ToInt32(dtCustomerOrderGroup.Rows[i]["CustomerId"]);
                    obj.Amount = Convert.ToDecimal(dtCustomerOrderGroup.Rows[i]["Amount"]);
                    obj.TransactionDate = orderDate.Value;

                    dtDateOrderSchel = obj.getCustomerOrderScheduleList(obj.CustomerId, orderDate);
                    if (dtDateOrderSchel.Rows.Count > 0)
                    {
                        //chcek wallet balance
                        decimal Walletbal = 0, TotalCredit = 0, TotalDebit = 0, Last2days = 0; bool AllowOrderWallet = false;
                        DataTable dtprodRecord = new DataTable();
                       //wallet balance

                        //New Dibakar
                        decimal credit = 0;

                        DataTable dt = new DataTable();
                        dt = obj.GetCustomerCredit(obj.CustomerId);

                        if (dt.Rows.Count > 0)
                        {
                            credit = Convert.ToDecimal(dt.Rows[0].ItemArray[0].ToString());
                        }

                        //


                        Last2days = obj.Amount + obj.Amount;
                        if (Walletbal < Last2days)
                        {
                            //notification for low balance
                            //string title = "Low balance";
                            //string content = "Dear Milkyway Family member, your wallet balance is low kindly upload balance for seamless delivery service- Milkyway accounts dept.";
                            //string type = "Notification";//PRODUCT   NEWS_INFO  ORDER
                            //string obj_id = "1";
                            //string image = "";
                            //int appnotification = AppNotification(obj.CustomerId, title, content, type, obj_id, image);
                        }
                        if (Walletbal < obj.Amount)
                        {
                            AllowOrderWallet = true;
                        }
                        else { AllowOrderWallet = true; }
                        //New Dibakar
                        //if (credit != 0)
                        //{
                        //    credit = 0 - credit;
                        //    if (Walletbal <= credit)
                        //    {
                        //        AllowOrderWallet = false;
                        //    }
                        //}
                        //

                        int TotalRewardPoint = 0;
                        int UpdateOrderStatus = 0, UpdateOrderStatusCancle = 0;
                        //find order amount deduct from wallet
                        for (int j = 0; j < dtDateOrderSchel.Rows.Count; j++)
                        {
                            string pname = "",proqty="0",AttributeName="";
                            obj.CustomerId = Convert.ToInt32(dtDateOrderSchel.Rows[j]["CustomerId"]);
                            //  obj.TransactionDate = DateTime.Now.AddDays(1);
                            obj.TransactionDate = Convert.ToDateTime(dtDateOrderSchel.Rows[j]["OrderDate"]);
                            obj.Amount = Convert.ToDecimal(dtDateOrderSchel.Rows[j]["Amount"]);
                            obj.OrderId = Convert.ToInt32(dtDateOrderSchel.Rows[j]["OId"]);
                             pname = dtDateOrderSchel.Rows[j]["ProductName"].ToString();
                            AttributeName = dtDateOrderSchel.Rows[j]["AttributeName"].ToString();
                            obj.proqty= dtDateOrderSchel.Rows[j]["Qty"].ToString();
                            obj.Description = pname+"-"+AttributeName;
                            obj.Status = "Purchase";
                            obj.Type = "Debit";
                            obj.CustSubscriptionId = 0;
                            obj.TransactionType = Convert.ToInt32(Helper.TransactionType.Purchase);
                            obj.RewardPoint = Convert.ToInt64(dtDateOrderSchel.Rows[j]["RewardPoint"]);
                            //Dibakar 31-12-2022 chk wallet balance+ credit < or > of Amount
                            //wallet balance
                            dtprodRecord = obj.getCustomerWallet(obj.CustomerId);
                            int userRecords = dtprodRecord.Rows.Count;
                            if (userRecords > 0)
                            {
                                if (!string.IsNullOrEmpty(dtprodRecord.Rows[0]["Amt"].ToString()))
                                    TotalCredit = Convert.ToDecimal(dtprodRecord.Rows[0]["Amt"]);
                                if (userRecords > 1)
                                {
                                    if (!string.IsNullOrEmpty(dtprodRecord.Rows[1]["Amt"].ToString()))
                                        TotalDebit = Convert.ToDecimal(dtprodRecord.Rows[1]["Amt"]);
                                }
                                Walletbal = TotalCredit - TotalDebit;
                            }

                            if(Walletbal+credit< obj.Amount)
                            {
                                AllowOrderWallet = false;
                            }
                            else
                                AllowOrderWallet = true;
                            //


                            //


                            if (AllowOrderWallet == false)
                            {
                                var date = obj.TransactionDate.Date;
                                //order status 
                                UpdateOrderStatusCancle = obj.UpdateCustomerOrderCancle(obj.OrderId, obj.CustomerId, Convert.ToDateTime(date), "InComplete");
                            }
                            else
                            {
                                //check dupliacte records
                                //debit from wallet
                                AddWallet = obj.InsertWalletScheduleOrder(obj);
                                // AddWallet = 1;
                                if (AddWallet > 0)
                                {
                                    var date = obj.TransactionDate.Date;
                                    //order status 
                                    UpdateOrderStatus = obj.UpdateCustomerOrderCancle(obj.OrderId, obj.CustomerId, Convert.ToDateTime(date), "Complete");
                                    // UpdateOrderStatus = 1;
                                    //add Rewards Point to Customer table
                                    dtGetCustomerPoint = objcust.BindCustomer(obj.CustomerId);
                                    if (dtGetCustomerPoint.Rows.Count > 0)
                                    {
                                        if (!string.IsNullOrEmpty(dtGetCustomerPoint.Rows[0]["RewardPoint"].ToString()))
                                            TotalRewardPoint = Convert.ToInt32(dtGetCustomerPoint.Rows[0]["RewardPoint"]);
                                        obj.RewardPoint = obj.RewardPoint + TotalRewardPoint;
                                        int UpdateCustomer = objcust.UpdateCustomerPoint(obj.CustomerId, Convert.ToInt64(obj.RewardPoint));
                                    }

                                }
                            }
                        }
                        if (UpdateOrderStatus > 0)
                        {
                            //notification
                            //string title = "Order Confirmed";
                            //string content = "Dear Customer, Your Order is confirmed.Thank you for Purchase Order with MilkyWay India! ";
                            //string type = "Notification";//PRODUCT   NEWS_INFO  ORDER
                            //string obj_id = "1";
                            //string image = "";
                            //int appnotification = AppNotification(obj.CustomerId, title, content, type, obj_id, image);
                        }
                    }
                }
            }
            if (AddWallet > 0)
            {
                return "Schedular Inserted Successfully!!!";
            }
            else
            { return "Schedular Not Inserted !!!"; }
        }

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
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAP1kVwxg:APA91bEGnOhfhaWJqwwBtW0uzn3nF-mSbJxDtjggcMRts5mith8ArpqpnW57HEhO3yKpohZZZs7PPF1LCsYMlioCFXzyFt5nRxeTCgPk-zlrX-YfQps6yCn1Z9bdVAFK7HnCja_S3Nsp"));
                tRequest.Headers.Add(string.Format("Sender: id={0}", "272077538072"));
                tRequest.ContentType = "application/json";
                var payload = new
                {
                    to = dtToken.Rows[0]["fcm_token"].ToString(),
                    priority = "high",
                    content_available = true,
                    notification = new
                    {
                        body = notificationcontent,
                        title = notificationtitle,
                        badge = 1,
                        image = "https://5.imimg.com/data5/IW/DC/RP/SELLER-107655683/coconut-water-wholesaler-in-ahmedabad-500x500.jpg",
                    },
                    data = new
                    {
                        click_action = "FLUTTER_NOTIFICATION_CLICK",
                        body = notificationcontent,
                        title = notificationtitle,
                        link = "https://milkywayindia.com"
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

        public List<AutoPayiewModel> GetAutoPayPlan()
        {
            List<AutoPayiewModel> list = new List<AutoPayiewModel>();
            list.Add(new AutoPayiewModel { id = 1, balance = 50, deposit = 500 });
            list.Add(new AutoPayiewModel { id = 2, balance = 150, deposit = 1000 });
            list.Add(new AutoPayiewModel { id = 3, balance = 200, deposit = 1500 });
            list.Add(new AutoPayiewModel { id = 4, balance = 250, deposit = 2000 });
            list.Add(new AutoPayiewModel { id = 5, balance = 300, deposit = 3000 });
            list.Add(new AutoPayiewModel { id = 6, balance = 350, deposit = 4000 });
            list.Add(new AutoPayiewModel { id = 7, balance = 400, deposit = 5000 });
            return list;
        }

        public InitiateSubscriptionResponse InitiateSubscription(string CustomerId, int PlanID)
        {
            //"{\"head\":{\"requestId\":null,\"responseTimestamp\":\"1638972326743\",\"version\":\"v1\"},\"body\":{\"extraParamsMap\":null,\"resultInfo\":{\"resultStatus\":\"TXN_FAILURE\",\"resultMsg\":\"OnDemand Subscriptions are not allowed on merchant\"}}}"
            InitiateSubscriptionResponse response = new InitiateSubscriptionResponse();
            response.status = "400";
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Dictionary<string, object> body = new Dictionary<string, object>();
                Dictionary<string, string> head = new Dictionary<string, string>();
                Dictionary<string, object> requestBody = new Dictionary<string, object>();
                Dictionary<string, string> txnAmount = new Dictionary<string, string>();

                var orderid = _SecPaytmRepo.GenerateOrderNo();
                txnAmount.Add("value", "1.00");
                txnAmount.Add("currency", "INR");

                decimal Amount = 1;
                var fillAmount = GetAutoPayPlan().FirstOrDefault(s => s.id == PlanID);
                if (fillAmount != null)
                    Amount = fillAmount.deposit.Value;

                Dictionary<string, string> userInfo = new Dictionary<string, string>();
                userInfo.Add("custId", CustomerId);
                body.Add("requestType", "NATIVE_SUBSCRIPTION");
                body.Add("mid", Helper.PaytmMerchantID);
                body.Add("websiteName", "DEFAULT");
                body.Add("orderId", orderid);
                body.Add("subscriptionAmountType", "VARIABLE");
                body.Add("subscriptionMaxAmount", Amount.ToString());
                body.Add("subscriptionFrequency", "1");
                body.Add("subscriptionFrequencyUnit", "ONDEMAND");
                //body.Add("subscriptionFrequencyUnit", "DAY");
                body.Add("subscriptionStartDate", Helper.indianTime.ToString("yyyy-MM-dd"));
                body.Add("subscriptionExpiryDate", Helper.indianTime.AddYears(1).ToString("yyyy-MM-dd"));
                body.Add("subscriptionGraceDays", "0");
                body.Add("subscriptionPaymentMode", "UPI");
                body.Add("subscriptionEnableRetry", "1");
                body.Add("txnAmount", txnAmount);
                body.Add("userInfo", userInfo);
                body.Add("callbackUrl", PaytmCallback() + "/home/callback");
                string paytmChecksum = Paytm.Checksum.generateSignature(JsonConvert.SerializeObject(body), Helper.PaytmMerchantKey);
                head.Add("signature", paytmChecksum);
                head.Add("channelId", "WAP");

                requestBody.Add("body", body);
                requestBody.Add("head", head);

                string post_data = JsonConvert.SerializeObject(requestBody);
                string url = string.Format("{2}/subscription/create?mid={0}&orderId={1}", Helper.PaytmMerchantID, orderid, Helper.PaytmAPI);
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.ContentLength = post_data.Length;

                using (StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter.Write(post_data);
                }

                string responseData = string.Empty;
                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();
                    PaytmResponse _paytm = JsonConvert.DeserializeObject<PaytmResponse>(responseData);
                    if (_paytm.body.resultInfo.resultStatus == "S")
                    {
                        tbl_Paytm_Request model = new tbl_Paytm_Request();
                        model.CustomerID = Convert.ToInt32(CustomerId);
                        model.PlanID = Convert.ToInt32(PlanID);
                        model.OrderNo = orderid;
                        model.StartDate = Helper.indianTime;
                        model.EndDate = Helper.indianTime.AddYears(1);
                        model.SubscriptionId = _paytm.body.subscriptionId;
                        model.Signature = _paytm.head.signature;
                        model.TxnToken = _paytm.body.txnToken;
                        model.Authenticated = Convert.ToBoolean(_paytm.body.authenticated);
                        model.CreatedDate = indianTime;
                        model.Authenticated = false;
                        model.PreNofifyCall = false;
                        model.Response = responseData;
                        var res = _SecPaytmRepo.SavePaytmRequest(model);
                        response.status = "200";
                        response.msg = _paytm.body.resultInfo.resultMsg;
                        response.token = _paytm.body.txnToken;
                    }
                    else
                    {
                        response.error_msg = _paytm.body.resultInfo.resultMsg;
                        response.status = _paytm.body.resultInfo.resultCode;
                    }
                }
            }
            catch (Exception ex)
            {
                response.error_msg = ex.Message;
            }

            return response;
        }

        public void PaytmPreNotify(int? CustomerID, decimal? Amount)
        {
            try
            {
                var detail = _SecPaytmRepo.GetDetailByCustomerID(CustomerID);
                if (detail != null)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    Dictionary<string, string> body = new Dictionary<string, string>();
                    Dictionary<string, string> head = new Dictionary<string, string>();
                    Dictionary<string, Dictionary<string, string>> requestBody = new Dictionary<string, Dictionary<string, string>>();

                    DateTime DeductDate = Helper.indianTime.AddDays(1);
                    var referenceId = GenerateReferenceCode(28);
                    body.Add("mid", Helper.PaytmMerchantID);
                    body.Add("subsId", detail.SubscriptionId);
                    body.Add("txnAmount", Amount.ToString());
                    body.Add("txnDate", DeductDate.ToString("dd-MM-yyyy"));
                    body.Add("txnMessage", "Add Money To Milkyway India");
                    body.Add("referenceId", referenceId);
                    string paytmChecksum = Paytm.Checksum.generateSignature(JsonConvert.SerializeObject(body), Helper.PaytmMerchantKey);
                    head.Add("tokenType", "AES");
                    head.Add("signature", paytmChecksum);
                    requestBody.Add("body", body);
                    requestBody.Add("head", head);
                    string post_data = JsonConvert.SerializeObject(requestBody);

                    string url = Helper.PaytmAPI + "/subscription/preNotify";
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                    webRequest.Method = "POST";
                    webRequest.ContentType = "application/json";
                    webRequest.ContentLength = post_data.Length;
                    using (StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                    {
                        requestWriter.Write(post_data);
                    }
                    string responseData = string.Empty;
                    using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                    {
                        responseData = responseReader.ReadToEnd();
                        dynamic response = JsonConvert.DeserializeObject(responseData);
                        var status = response.body.resultInfo.code;
                        if (status == "3006")
                        {
                            tbl_Paytm_Request_Details model1 = new tbl_Paytm_Request_Details();
                            model1.PaytmRequestID = detail.ID;
                            model1.CustomerID = detail.CustomerID;
                            model1.PaytmReferenceID = response.body.paytmReferenceId;
                            model1.ReferenceID = referenceId;
                            model1.NotifyDate = Helper.indianTime;
                            model1.Amount = Amount;
                            model1.PrenotifyRequest = post_data;
                            model1.PrenotifyResponse = responseData;
                            model1.RenewalDate = DeductDate;
                            model1.IsConfirm = false;
                            var res = _SecPaytmRepo.InsertPaytmPreNotify(model1);
                            if (res == 1)
                            {
                                var request = db.tbl_Paytm_Request.FirstOrDefault(s => s.ID == detail.ID);
                                if (request != null)
                                    request.PreNofifyCall = true;
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch { }
        }

        public void PaytmPreNotifyStatus(int? CustomerID)
        {
            var detail = _SecPaytmRepo.GetDetailByCustomerID(CustomerID);
            if (detail != null)
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Dictionary<string, string> body = new Dictionary<string, string>();
                Dictionary<string, string> head = new Dictionary<string, string>();
                Dictionary<string, Dictionary<string, string>> requestBody = new Dictionary<string, Dictionary<string, string>>();

                body.Add("mid", Helper.PaytmMerchantID);
                body.Add("subsId", detail.SubscriptionId);
                body.Add("referenceId", detail.ReferebceId);
                string paytmChecksum = Paytm.Checksum.generateSignature(JsonConvert.SerializeObject(body), Helper.PaytmMerchantKey);
                head.Add("version", "v1");
                head.Add("timestamp", "");
                head.Add("tokenType", "AES");
                head.Add("signature", paytmChecksum);
                head.Add("clientId", "1234");

                requestBody.Add("body", body);
                requestBody.Add("head", head);

                string post_data = JsonConvert.SerializeObject(requestBody);
                string url = Helper.PaytmAPI + "/subscription/preNotify/status";
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.ContentLength = post_data.Length;

                using (StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter.Write(post_data);
                }

                string responseData = string.Empty;

                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();
                    Console.WriteLine(responseData);
                }
            }
        }

        public void PaytmRequestPayment(int? PaymentRequestID, int? CustomerID, decimal? Amount)
        {
            try
            {
                var detail = _SecPaytmRepo.GetDetailByCustomerID(CustomerID);
                if (detail != null)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    Dictionary<string, object> body = new Dictionary<string, object>();
                    Dictionary<string, string> head = new Dictionary<string, string>();
                    Dictionary<string, object> requestBody = new Dictionary<string, object>();

                    var orderid = _SecPaytmRepo.GenerateOrderNo();
                    Dictionary<string, string> txnAmount = new Dictionary<string, string>();
                    txnAmount.Add("value", Amount.ToString());
                    txnAmount.Add("currency", "INR");
                    body.Add("mid", Helper.PaytmMerchantID);
                    body.Add("orderId", orderid.ToString());
                    body.Add("subscriptionId", detail.SubscriptionId);
                    body.Add("txnAmount", txnAmount);

                    string paytmChecksum = Paytm.Checksum.generateSignature(JsonConvert.SerializeObject(body), Helper.PaytmMerchantKey);
                    head.Add("signature", paytmChecksum);
                    requestBody.Add("body", body);
                    requestBody.Add("head", head);
                    string post_data = JsonConvert.SerializeObject(requestBody);

                    string url = string.Format("{0}/subscription/renew?mid={1}&orderId={2}", Helper.PaytmAPI, Helper.PaytmMerchantID, orderid);
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                    webRequest.Method = "POST";
                    webRequest.ContentType = "application/json";
                    webRequest.ContentLength = post_data.Length;

                    using (StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                    {
                        requestWriter.Write(post_data);
                    }

                    string responseData = string.Empty;
                    using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                    {
                        responseData = responseReader.ReadToEnd();
                        dynamic response = JsonConvert.DeserializeObject(responseData);
                        var request = db.tbl_Paytm_Request_Details.FirstOrDefault(s => s.ID == PaymentRequestID);
                        if (request != null)
                        {
                            request.RenewalOrderID = orderid;
                            request.RenewalRequest = post_data;
                            request.RenewalResponse = responseData;
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch { }
        }

        public void PaytmTransactoinStatus(int? PaymentRequestID, string Orderid)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Dictionary<string, object> body = new Dictionary<string, object>();
                Dictionary<string, string> head = new Dictionary<string, string>();
                Dictionary<string, object> requestBody = new Dictionary<string, object>();

                body.Add("mid", Helper.PaytmMerchantID);
                body.Add("orderId", Orderid);

                string paytmChecksum = Paytm.Checksum.generateSignature(JsonConvert.SerializeObject(body), Helper.PaytmMerchantKey);
                head.Add("tokenType", "AES");
                head.Add("signature", paytmChecksum);
                requestBody.Add("body", body);
                requestBody.Add("head", head);
                string post_data = JsonConvert.SerializeObject(requestBody);

                string url = PaytmAPI + "/v3/order/status";
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.ContentLength = post_data.Length;

                using (StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter.Write(post_data);
                }

                string responseData = string.Empty;
                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();
                    dynamic response = JsonConvert.DeserializeObject(responseData);
                    var status = response.body.resultInfo.resultCode;
                    if (status == "01")
                    {
                        var txnId = response.body.txnId;
                        var bankTxnId = response.body.bankTxnId;
                        var txnDate = response.body.txnDate;
                        var request = db.tbl_Paytm_Request_Details.FirstOrDefault(s => s.ID == PaymentRequestID);
                        if (request != null)
                        {
                            request.RenewalTxnID = txnId;
                            request.RenewalResponse = responseData;
                            request.BankTxnID = bankTxnId;
                            request.TxnDate = txnDate;
                            request.IsConfirm = true;
                            request.ConfirmDate = Helper.indianTime;

                            var paytmRequest = db.tbl_Paytm_Request.FirstOrDefault(s => s.ID == request.PaytmRequestID);
                            if (paytmRequest != null)
                                paytmRequest.PreNofifyCall = false;
                            db.SaveChanges();

                            _subscription.OrderId = 0;
                            //_subscription.BillNo = request.RenewalOrderID;
                            _subscription.BillNo = null;
                            _subscription.CustomerId = request.CustomerID.Value;
                            _subscription.TransactionDate = Helper.indianTime;
                            _subscription.Amount = request.Amount.Value;
                            _subscription.Type = "Credit";
                            _subscription.TransactionType = Convert.ToInt32(Helper.TransactionType.Deposit);
                            _subscription.CustSubscriptionId = 0;
                            _subscription.Description = "Add To Wallet AutoPay OrderNo - " + request.RenewalOrderID;
                            _subscription.InsertWallet(_subscription);
                        }
                    }
                    else
                    {
                        var request = db.tbl_Paytm_Request_Details.FirstOrDefault(s => s.ID == PaymentRequestID);
                        if (request != null)
                        {
                            var paytmRequest = db.tbl_Paytm_Request.FirstOrDefault(s => s.ID == request.PaytmRequestID);
                            if (paytmRequest != null)
                            {
                                paytmRequest.PreNofifyCall = false;
                                paytmRequest.ErrorMessage = response.body.resultInfo.resultMsg;
                            }
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex) { string s = ex.Message; }
        }

        public PaytmInitiateTransaction InitiateTransaction(string CustomerId, Decimal Amount)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Dictionary<string, object> body = new Dictionary<string, object>();
                Dictionary<string, string> head = new Dictionary<string, string>();
                Dictionary<string, object> requestBody = new Dictionary<string, object>();

                var orderid = _SecPaytmRepo.GenerateOrderNo();
                Dictionary<string, string> txnAmount = new Dictionary<string, string>();
                txnAmount.Add("value", Amount.ToString("0.00"));
                txnAmount.Add("currency", "INR");
                Dictionary<string, string> userInfo = new Dictionary<string, string>();
                userInfo.Add("custId", CustomerId);
                body.Add("requestType", "Payment");
                body.Add("mid", PaytmMerchantID);
                body.Add("websiteName", "DEFAULT");
                body.Add("orderId", orderid);
                body.Add("txnAmount", txnAmount);
                body.Add("userInfo", userInfo);

                List<Dictionary<string, string>> enablePaymentMode = new List<Dictionary<string, string>>();
                Dictionary<string, string> mode = new Dictionary<string, string>();
                mode.Add("mode", "UPI");
                enablePaymentMode.Add(mode);

                body.Add("enablePaymentMode", enablePaymentMode);
                body.Add("callbackUrl", PaytmAPI + "/theia/paytmCallback?ORDER_ID=" + orderid);
                string paytmChecksum = Paytm.Checksum.generateSignature(JsonConvert.SerializeObject(body), PaytmMerchantKey);
                head.Add("signature", paytmChecksum);
                requestBody.Add("body", body);
                requestBody.Add("head", head);
                string post_data = JsonConvert.SerializeObject(requestBody);

                string url = string.Format("{0}/theia/api/v1/initiateTransaction?mid={1}&orderId={2}", PaytmAPI, PaytmMerchantID, orderid);
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.ContentLength = post_data.Length;

                using (StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter.Write(post_data);
                }

                string responseData = string.Empty;
                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();
                    Console.WriteLine(responseData);
                    PaytmResponse _paytm = JsonConvert.DeserializeObject<PaytmResponse>(responseData);
                    if (_paytm.body.resultInfo.resultStatus == "S")
                    {
                        PaytmInitiateTransaction rs = new PaytmInitiateTransaction();
                        rs.token = _paytm.body.txnToken;
                        rs.orderid = orderid.ToString();
                        return rs;
                    }
                }
            }
            catch (Exception ex) { string s = ex.Message; }
            return null;
        }

        //Dibakar 07-12-2022


        public PaytmInitiateTransaction InitiateTransactionnew(string CustomerId, Decimal Amount,int paymentSourceId)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Dictionary<string, object> body = new Dictionary<string, object>();
                Dictionary<string, string> head = new Dictionary<string, string>();
                Dictionary<string, object> requestBody = new Dictionary<string, object>();

                var orderid = _SecPaytmRepo.GenerateOrderNo();
                Dictionary<string, string> txnAmount = new Dictionary<string, string>();
                txnAmount.Add("value", Amount.ToString("0.00"));
                txnAmount.Add("currency", "INR");
                Dictionary<string, string> userInfo = new Dictionary<string, string>();
                userInfo.Add("custId", CustomerId);
                body.Add("requestType", "Payment");
                body.Add("mid", PaytmMerchantID);
                body.Add("websiteName", "WEBSTAGING");
                body.Add("orderId", orderid);
                body.Add("txnAmount", txnAmount);
                body.Add("userInfo", userInfo);

                List<Dictionary<string, string>> enablePaymentMode = new List<Dictionary<string, string>>();
                Dictionary<string, string> mode = new Dictionary<string, string>();
                //mode.Add("mode", "UPI");
                //enablePaymentMode.Add(mode);

                //body.Add("enablePaymentMode", enablePaymentMode);
                body.Add("callbackUrl", PaytmAPI + "/theia/paytmCallback?ORDER_ID=" + orderid);
                string paytmChecksum = Paytm.Checksum.generateSignature(JsonConvert.SerializeObject(body), PaytmMerchantKey);
                head.Add("signature", paytmChecksum);
                requestBody.Add("body", body);
                requestBody.Add("head", head);
                string post_data = JsonConvert.SerializeObject(requestBody);

                string url = string.Format("{0}/theia/api/v1/initiateTransaction?mid={1}&orderId={2}", PaytmAPI, PaytmMerchantID, orderid);
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.ContentLength = post_data.Length;

                using (StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter.Write(post_data);
                }

                string responseData = string.Empty;
                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();
                    Console.WriteLine(responseData);
                    PaytmResponse _paytm = JsonConvert.DeserializeObject<PaytmResponse>(responseData);
                    if (_paytm.body.resultInfo.resultStatus == "S")
                    {
                        PaytmInitiateTransaction rs = new PaytmInitiateTransaction();
                        rs.token = _paytm.body.txnToken;
                        rs.orderid = orderid.ToString();
                        return rs;
                    }
                }
            }
            catch (Exception ex) { string s = ex.Message; }
            return null;
        }
        public int AddCustomerDailyOrder(string customerid, string productid, string qty, DateTime FromDate,string AttributeId,string NextOrderDate)
        {
            long AddProductOrder = 0; int AddProductDetail = 0;
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();

            if (!string.IsNullOrEmpty(customerid))
            { obj.CustomerId = Convert.ToInt32(customerid); }
            else { obj.CustomerId = 0; }

            int societyid = 0;
            DateTime ToDate = FromDate.AddMonths(1);
            if (!string.IsNullOrEmpty(societyid.ToString()))
            { obj.BuildingId = Convert.ToInt32(societyid); }
            else { }
            obj.BuildingId = 0;
            obj.Qty = Convert.ToInt32(qty);

            obj.ProductId = Convert.ToInt32(productid);
            obj.AttributeId = Convert.ToInt32(AttributeId);
            //get Product detail
            DataTable dtProduct = objproduct.BindProuctForwardnewversion(obj.ProductId,obj.AttributeId);
            if (dtProduct.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["ProductID"].ToString()))
                    productid = dtProduct.Rows[0]["ProductID"].ToString();

                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["AttributeID"].ToString()))
                    AttributeId = dtProduct.Rows[0]["AttributeID"].ToString();


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
            obj.AttributeId = Convert.ToInt32(AttributeId);

            obj.OrderFlag = "Daily";

            DataTable getcustomerorder = obj.getCustomerOrderDetailnewversion(Convert.ToInt32(customerid), Convert.ToInt32(productid), Convert.ToInt32(AttributeId),NextOrderDate);
            if(getcustomerorder.Rows.Count>0)
            {
                if (!string.IsNullOrEmpty(getcustomerorder.Rows[0]["DeliveryBoyId"].ToString()))
                    obj.DeliveryBoyId = getcustomerorder.Rows[0]["DeliveryBoyId"].ToString();
                else
                    obj.DeliveryBoyId = "0";
                if (!string.IsNullOrEmpty(getcustomerorder.Rows[0]["VendorId"].ToString()))
                    obj.VendorId = Convert.ToInt32(getcustomerorder.Rows[0]["VendorId"].ToString());
                else
                    obj.VendorId = 0;
                if (!string.IsNullOrEmpty(getcustomerorder.Rows[0]["VendorCatId"].ToString()))
                    obj.VendorCatId = Convert.ToInt32(getcustomerorder.Rows[0]["VendorCatId"].ToString());
                else
                    obj.VendorCatId = 0;
                if (!string.IsNullOrEmpty(getcustomerorder.Rows[0]["SectorId"].ToString()))
                    obj.SectorId = Convert.ToInt32(getcustomerorder.Rows[0]["SectorId"].ToString());
                else
                    obj.SectorId = 0;
            }
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

                string OrderDate = FromDate.ToString("yyyy-MM-dd");
                var checkOrder = _cOrder.CheckCustomerOrderByQtyDate(obj.CustomerId, Convert.ToInt32(productid), Convert.ToInt32(qty), OrderDate,Convert.ToInt32(AttributeId));
                if (checkOrder.Rows.Count == 0)
                {
                    AddProductOrder = obj.InsertCustomerOrder(obj);

                    if (AddProductOrder > 0)
                    {
                        obj.OrderId = Convert.ToInt32(AddProductOrder);
                        obj.ProductId = Convert.ToInt32(productid);
                        obj.Qty = Convert.ToInt32(qty);
                        obj.OrderItemDate = FromDate;

                        AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                    }
                }

                FromDate = FromDate.AddDays(1);
            }
            return AddProductDetail;
        }



        public int AddCustomerAlternateOrder(string customerid, string productid, string qty, DateTime FromDate, string AttributeId, string NextOrderDate)
        {
            int Attributeid = 0;
            long AddProductOrder = 0; int AddProductDetail = 0;
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();

            if (!string.IsNullOrEmpty(customerid))
            { obj.CustomerId = Convert.ToInt32(customerid); }
            else { obj.CustomerId = 0; }

            int societyid = 0;
            DateTime ToDate = FromDate.AddMonths(1);
            if (!string.IsNullOrEmpty(societyid.ToString()))
            { obj.BuildingId = Convert.ToInt32(societyid); }
            else { }
            obj.BuildingId = 0;
            obj.Qty = Convert.ToInt32(qty);

            obj.ProductId = Convert.ToInt32(productid);
            obj.AttributeId = Convert.ToInt32(AttributeId);
            //get Product detail
            DataTable dtProduct = objproduct.BindProuctForwardnewversion(obj.ProductId, obj.AttributeId);
            if (dtProduct.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["ProductID"].ToString()))
                    productid = dtProduct.Rows[0]["ProductID"].ToString();

                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["AttributeID"].ToString()))
                    AttributeId = dtProduct.Rows[0]["AttributeID"].ToString();


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

            obj.OrderFlag = "Alternate";

            DataTable getcustomerorder = obj.getCustomerOrderDetailnewversion(Convert.ToInt32(customerid), Convert.ToInt32(productid), Convert.ToInt32(AttributeId), NextOrderDate);
            if (getcustomerorder.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(getcustomerorder.Rows[0]["DeliveryBoyId"].ToString()))
                    obj.DeliveryBoyId = getcustomerorder.Rows[0]["DeliveryBoyId"].ToString();
                else
                    obj.DeliveryBoyId = "0";
                if (!string.IsNullOrEmpty(getcustomerorder.Rows[0]["VendorId"].ToString()))
                    obj.VendorId = Convert.ToInt32(getcustomerorder.Rows[0]["VendorId"].ToString());
                else
                    obj.VendorId = 0;
                if (!string.IsNullOrEmpty(getcustomerorder.Rows[0]["VendorCatId"].ToString()))
                    obj.VendorCatId = Convert.ToInt32(getcustomerorder.Rows[0]["VendorCatId"].ToString());
                else
                    obj.VendorCatId = 0;
                if (!string.IsNullOrEmpty(getcustomerorder.Rows[0]["SectorId"].ToString()))
                    obj.SectorId = Convert.ToInt32(getcustomerorder.Rows[0]["SectorId"].ToString());
                else
                    obj.SectorId = 0;
            }

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

                string OrderDate = FromDate.ToString("yyyy-MM-dd");
                var checkOrder = _cOrder.CheckCustomerOrderByQtyDate(obj.CustomerId, Convert.ToInt32(productid), Convert.ToInt32(qty), OrderDate, Attributeid);
                if (checkOrder.Rows.Count == 0)
                {
                    AddProductOrder = obj.InsertCustomerOrder(obj);

                    if (AddProductOrder > 0)
                    {
                        obj.OrderId = Convert.ToInt32(AddProductOrder);
                        obj.ProductId = Convert.ToInt32(productid);
                        obj.Qty = Convert.ToInt32(qty);
                        obj.OrderItemDate = FromDate;

                        AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                    }
                }

                FromDate = FromDate.AddDays(2);
            }

           
            


            return AddProductDetail;
        }


        public int AddCustomerWeekOrder(string customerid, string productid, string qty, DateTime FromDate, string AttributeId, string NextOrderDate)
        {
            int Attributeid = 0;
            long AddProductOrder = 0; int AddProductDetail = 0;
            Subscription obj = new Subscription();
            Customer objcust = new Customer();
            Product objproduct = new Product();

            if (!string.IsNullOrEmpty(customerid))
            { obj.CustomerId = Convert.ToInt32(customerid); }
            else { obj.CustomerId = 0; }

            int societyid = 0;
            DateTime ToDate = FromDate.AddMonths(1);
            if (!string.IsNullOrEmpty(societyid.ToString()))
            { obj.BuildingId = Convert.ToInt32(societyid); }
            else { }
            obj.BuildingId = 0;
            obj.Qty = Convert.ToInt32(qty);

            obj.ProductId = Convert.ToInt32(productid);
            obj.AttributeId = Convert.ToInt32(AttributeId);
            //get Product detail
            DataTable dtProduct = objproduct.BindProuctForwardnewversion(obj.ProductId, obj.AttributeId);
            if (dtProduct.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["ProductID"].ToString()))
                    productid = dtProduct.Rows[0]["ProductID"].ToString();

                if (!string.IsNullOrEmpty(dtProduct.Rows[0]["AttributeID"].ToString()))
                    AttributeId = dtProduct.Rows[0]["AttributeID"].ToString();


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


            DataTable getcustomerorder = obj.getCustomerOrderDetailnewversion(Convert.ToInt32(customerid), Convert.ToInt32(productid), Convert.ToInt32(AttributeId), NextOrderDate);
            if (getcustomerorder.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(getcustomerorder.Rows[0]["DeliveryBoyId"].ToString()))
                    obj.DeliveryBoyId = getcustomerorder.Rows[0]["DeliveryBoyId"].ToString();
                else
                    obj.DeliveryBoyId = "0";
                if (!string.IsNullOrEmpty(getcustomerorder.Rows[0]["VendorId"].ToString()))
                    obj.VendorId = Convert.ToInt32(getcustomerorder.Rows[0]["VendorId"].ToString());
                else
                    obj.VendorId = 0;
                if (!string.IsNullOrEmpty(getcustomerorder.Rows[0]["VendorCatId"].ToString()))
                    obj.VendorCatId = Convert.ToInt32(getcustomerorder.Rows[0]["VendorCatId"].ToString());
                else
                    obj.VendorCatId = 0;
                if (!string.IsNullOrEmpty(getcustomerorder.Rows[0]["SectorId"].ToString()))
                    obj.SectorId = Convert.ToInt32(getcustomerorder.Rows[0]["SectorId"].ToString());
                else
                    obj.SectorId = 0;
            }

            if (!string.IsNullOrEmpty(obj.TotalFinalAmount.ToString()))
            { obj.TotalAmount = obj.TotalFinalAmount; }
            else { obj.TotalAmount = 0; }

            obj.TotalGSTAmt = obj.CGSTAmount + obj.SGSTAmount;

            obj.Status = "Pending";
            obj.StateCode = null;
            obj.ProductId = Convert.ToInt32(productid);

            obj.OrderFlag = "WeekDay";
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

                string OrderDate = FromDate.ToString("yyyy-MM-dd");
                var checkOrder = _cOrder.CheckCustomerOrderByQtyDate(obj.CustomerId, Convert.ToInt32(productid), Convert.ToInt32(qty), OrderDate, Attributeid);
                if (checkOrder.Rows.Count == 0)
                {
                    AddProductOrder = obj.InsertCustomerOrder(obj);

                    if (AddProductOrder > 0)
                    {
                        obj.OrderId = Convert.ToInt32(AddProductOrder);
                        obj.ProductId = Convert.ToInt32(productid);
                        obj.Qty = Convert.ToInt32(qty);
                        obj.OrderItemDate = FromDate;

                        AddProductDetail = obj.InsertCustomerOrderDetail(obj);
                    }
                }

                FromDate = FromDate.AddDays(7);
            }





            return AddProductDetail;
        }

        public void InsertCustomerOrderTrack(int customerid, int productid, int qty, DateTime fromdate, DateTime todate)
        {
            try
            {
                //tblad

                tbl_Cusomter_Order_Track model = new tbl_Cusomter_Order_Track();
                model.CustomerID = customerid;
                model.ProductID = productid;
                model.Qty = qty;
                model.OrderDate = fromdate;
                model.NextOrderDate = todate;
                model.IsActive = true;
                
                _CustomerRepo.InsertCustomerOrderTrack(model);
            }
            catch { }
        }


        public void InsertCustomerOrderTrackNew(int customerid, int productid, int qty, DateTime fromdate, DateTime todate,string OrderFlag)
        {
            try
            {
                tbl_Cusomter_Order_Track model = new tbl_Cusomter_Order_Track();
                model.CustomerID = customerid;
                model.ProductID = productid;
                model.Qty = qty;
                model.OrderDate = fromdate;
                model.NextOrderDate = todate;

                model.IsActive = true;
                model.OrderFlag = OrderFlag;
                _CustomerRepo.InsertCustomerOrderTrack(model);
            }
            catch { }
        }


        public void InsertCustomerOrderTrackNeworder(int customerid, int productid, int qty, DateTime fromdate, DateTime todate, string OrderFlag,int AttributeId)
        {
            try
            {
                tbl_Cusomter_Order_Track model = new tbl_Cusomter_Order_Track();
                model.CustomerID = customerid;
                model.ProductID = productid;
                
                model.Qty = qty;
                model.OrderDate = fromdate;
                model.NextOrderDate = todate;

                model.IsActive = true;
                model.OrderFlag = OrderFlag;
                model.AttributeId = AttributeId;
                _CustomerRepo.InsertCustomerOrderTracknew(model);
            }
            catch { }
        }
    }
}
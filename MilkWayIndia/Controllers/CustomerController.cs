using MilkWayIndia.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using MilkWayIndia.Entity;
using MilkWayIndia.Abstract;
using MilkWayIndia.Concrete;
using PagedList;
using System.Web.Helpers;

namespace MilkWayIndia.Controllers
{
	public class CustomerController : Controller
	{
		Customer objcust = new Customer();
		Subscription sobjSub = new Subscription();
		SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
		Helper dHelper = new Helper();
		private ICustomer _CustomerRepo;
		Dictionary<string, object> res = new Dictionary<string, object>();
		public CustomerController()
		{
			this._CustomerRepo = new CustomerRepository();
		}
		// GET: Customer
		[HttpGet]
		public ActionResult AddCustomer()
		{
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{
				var control = Helper.CheckPermission(Request.RawUrl.ToString());
				if (control.IsView == false)
					return Redirect("/notaccess/index");

				ViewBag.IsAdmin = control.IsAdmin;
				ViewBag.IsView = control.IsView;
				ViewBag.IsAdd = control.IsAdd;

				Sector objsec = new Sector();
				DataTable dt = new DataTable();
				dt = objsec.getBuildingList(null);
				ViewBag.Building = dt;

				DataTable dtsec = new DataTable();
				dtsec = objsec.getSectorList(null);
				ViewBag.Sector = dtsec;

				DataTable dtflatno = new DataTable();
				dtflatno = objsec.getFlatNoList(null);
				ViewBag.FlatNo = dtflatno;

				//SendNotification(null, null, null);
				//int result = 1;
				//string title = "Registration";
				//string content = "Congratulations!!You are now part of Milkyway Family To activate our service kindly update your wallet balance and choose desired subscription and enjoy our daily services.";
				//string type = "Notification";//PRODUCT   NEWS_INFO  ORDER
				//string obj_id = "1";
				//string image = "";
				//int appnotification = AppNotification(result, title, content, type, obj_id, image);
				return View();
			}
			else
			{
				return RedirectToAction("Login", "Home");
			}
		}
		[HttpPost]
		public ActionResult AddCustomer(Customer objcust, FormCollection form, HttpPostedFileBase Photo)
		{
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{
				Sector objsec = new Sector();
				DataTable dt = new DataTable();
				DataTable dtsec = new DataTable();
				DataTable dtflatno1 = new DataTable();

				string building = Request["ddlBuilding"];
				if (!string.IsNullOrEmpty(building))
				{
					objcust.BuildingId = Convert.ToInt32(building);
				}
				string flatId = Request["ddlFlatNo"];
				if (!string.IsNullOrEmpty(flatId))
				{
					objcust.FlatId = Convert.ToInt32(flatId);
				}
				//check username
				DataTable dtuserRecord1 = new DataTable();
				dtuserRecord1 = objcust.CheckCustomerUserName(objcust.MobileNo);
				int userRecords1 = dtuserRecord1.Rows.Count;
				if (userRecords1 > 0)
				{
					ViewBag.SuccessMsg = "MobileNo Already Exits!!!";
					dt = objsec.getBuildingList(null);
					ViewBag.Building = dt;

					dtsec = objsec.getSectorList(null);
					ViewBag.Sector = dtsec;

					dtflatno1 = objsec.getFlatNoList(null);
					ViewBag.FlatNo = dtflatno1;
					return View();
				}
				//check Flatno
				DataTable dtflatno = new DataTable();
				dtflatno = objcust.CheckCustomerFlatNo(objcust.FlatId);
				int flatno = dtflatno.Rows.Count;
				if (flatno > 0)
				{
					ViewBag.SuccessMsg = "FlatNo Already Exits!!!";

					dt = objsec.getBuildingList(null);
					ViewBag.Building = dt;

					dtsec = objsec.getSectorList(null);
					ViewBag.Sector = dtsec;

					dtflatno1 = objsec.getFlatNoList(null);
					ViewBag.FlatNo = dtflatno1;
					return View();
				}
				//check data duplicate
				//DataTable dtDupliStaff = new DataTable();
				//dtDupliStaff = objcust.CheckDuplicateCustomer(objcust.FirstName, objcust.LastName, objcust.MobileNo);
				//if (dtDupliStaff.Rows.Count > 0)
				//{
				//    //ViewBag.SuccessMsg = "Data Already Exits!!!";
				//}
				//else
				//{
				string fname = null, path = null;
				HttpPostedFileBase document1 = Request.Files["Photo"];
				string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png" };
				if (document1 != null)
				{
					if (document1.ContentLength > 0)
					{
						try
						{
							HttpFileCollectionBase files = Request.Files;
							HttpPostedFileBase file = Photo;
							string fileName = Path.GetFileNameWithoutExtension(file.FileName);
							string extension = Path.GetExtension(file.FileName);
							if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
							{
								string[] testfiles = file.FileName.Split(new char[] { '\\' });
								fname = testfiles[testfiles.Length - 1];
							}
							else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
							{
								ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
							}
							else
							{
								fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
							}
							path = Path.Combine(Server.MapPath("~/image/customer/"), fname);
							file.SaveAs(path);
							objcust.Photo = fname;
						}

						catch (Exception ex)
						{
							ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
						}
					}
				}
				objcust.ReferralCode = dHelper.GenerateCustomerReferalCode();
				// int addresult = 1;
				int addresult = objcust.InsertCustomer(objcust);
				if (addresult > 0)
				{

					ViewBag.SuccessMsg = "Customer Inserted Successfully!!!";
				}
				else
				{ ViewBag.SuccessMsg = "Customer Not Inserted!!!"; }
				// }

				ModelState.Clear();
				dt = objsec.getBuildingList(null);
				ViewBag.Building = dt;

				dtsec = objsec.getSectorList(null);
				ViewBag.Sector = dtsec;

				dtflatno1 = objsec.getFlatNoList(null);
				ViewBag.FlatNo = dtflatno1;
				return View();
			}
			else
			{
				return RedirectToAction("Login", "Home");
			}
		}

		#region Customer Create & Update - Hitesh

		public void Populatedrp()
		{
			Sector objsec = new Sector();
			DataTable dtsec = new DataTable();
			dtsec = objsec.getSectorList(null);
			List<SelectListItem> lstSector = new List<SelectListItem>();
			for (int i = 0; i < dtsec.Rows.Count; i++)
			{
				lstSector.Add(new SelectListItem { Text = dtsec.Rows[i]["SectorName"].ToString(), Value = dtsec.Rows[i]["Id"].ToString() });
			}
			ViewBag.Sector = new SelectList(lstSector, "Value", "Text");
		}

		public ActionResult Create()
		{
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{
				var control = Helper.CheckPermission(Request.RawUrl.ToString());
				if (control.IsView == false)
					return Redirect("/notaccess/index");
				Populatedrp();
				return View();
			}
			else
			{
				return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
			}
		}

		public ActionResult Edit(int ID)
		{
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{
				var control = Helper.CheckPermission(Request.RawUrl.ToString());
				if (control.IsView == false)
					return Redirect("/notaccess/index");
				Populatedrp();
				Customer obj = new Customer();
				try
				{
					var customer = _CustomerRepo.GetCustomerByID(ID);
					if (customer != null)
					{
						obj.Id = customer.ID.Value;
						obj.FirstName = customer.FirstName;
						obj.LastName = customer.LastName;
						obj.Email = customer.Email;
						obj.Address = customer.Address;
						obj.MobileNo = customer.MobileNo;
						obj.SectorId = customer.SectorId == null ? 0 : customer.SectorId.Value;
						obj.Password = customer.Password;
						obj.Photo = customer.Photo;
						obj.OrderBy = customer.OrderBy == null ? 0 : customer.OrderBy.Value;
						obj.lat = customer.lat + "," + customer.lon;
						obj.lon = customer.lon;

						if (customer.Credit != null)
							obj.Credit = customer.Credit.Value;
						else
							obj.Credit = 0;
						obj.CustomerType = customer.CustomerType;
						ViewBag.CustomerType = obj.CustomerType;
					}
				}
				catch { }
				return View(obj);
			}
			else
			{
				return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
			}
		}

		[HttpPost]
		public ActionResult Create(Customer model, HttpPostedFileBase Photo, FormCollection from)
		{
			ViewBag.SuccessMsg = "";
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{
				Populatedrp();

				DataTable dtuserRecord1 = new DataTable();
				dtuserRecord1 = objcust.CheckCustomerUserName(model.MobileNo);
				int userRecords1 = dtuserRecord1.Rows.Count;
				if (userRecords1 > 0)
				{
					ViewBag.SuccessMsg = "MobileNo Already Exits!!!";
					return View();
				}
				var referralCode = Helper.GenerateUniqueCode(8);
				model.ReferralCode = referralCode;
				model.CustomerType = Request["ddlcustype"];
				var response = InsertUpdate(model, Photo);
				//objcust.CustomerSortOrderInsert(response.ID, model.SectorId);
				if (response.ID > 0)
					ViewBag.SuccessMsg = "Customer Inserted Successfully!!!";
				else
					ViewBag.SuccessMsg = "Customer Not Inserted!!!";
			}
			else
				return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
			return View();
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult Edit(Customer model, HttpPostedFileBase photo, FormCollection frm)
		{
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{
				var customer = _CustomerRepo.GetCustomerByID(model.Id);
				int oldOrder = 0;
				if (customer.OrderBy != null)
					oldOrder = customer.OrderBy.Value;

				Populatedrp();
				model.CustomerType = Request["ddlcustype"];
				var response = InsertUpdate(model, photo);
				//if (oldOrder != model.OrderBy)
				//    objcust.CustomerSortOrderUpdate(oldOrder, model.OrderBy, model.Id, model.SectorId);
				if (response.ID > 0)
					ViewBag.SuccessMsg = "Customer Updated Successfully!!!";
				else
					ViewBag.SuccessMsg = "Customer Not Inserted!!!";
			}
			else
				return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
			return View();
		}

		public tbl_Customer_Master InsertUpdate(Customer model, HttpPostedFileBase Photo)
		{
			tbl_Customer_Master customer = new tbl_Customer_Master();

			if (Photo != null)
			{
				string path = Server.MapPath("~/Image/customer/");
				string extension = Path.GetExtension(Photo.FileName);
				string filename = Guid.NewGuid() + extension;
				var filePath = path + filename;
				Photo.SaveAs(filePath);
				customer.Photo = filename;
			}
			else
				customer.Photo = model.Photo;
			customer.ID = model.Id;
			customer.FirstName = model.FirstName;
			customer.LastName = model.LastName;
			customer.MobileNo = model.MobileNo;
			customer.UserName = model.MobileNo;
			customer.Email = model.Email;
			customer.SectorId = model.SectorId;
			customer.Password = model.Password;
			customer.Address = model.Address;
			customer.Credit = model.Credit;
			customer.OrderBy = model.OrderBy;

			customer.IsActive = true;
			customer.IsDeleted = false;

			string stringBeforeChar = "";
			string stringAfterChar = "";

			if (!string.IsNullOrEmpty(model.lat))
			{
				string a = model.lat;

				stringBeforeChar = a.Substring(0, a.IndexOf(","));
				stringAfterChar = a.Substring(a.IndexOf(",") + 1);
				model.lat = stringBeforeChar;
				model.lon = stringAfterChar;
			}


			customer.lat = model.lat;
			customer.lon = model.lon;

			customer.CustomerType = model.CustomerType;
			var response = _CustomerRepo.SaveCustomer(customer);
			return response;
		}
		#endregion


		private string serverKey = "AAAAHhkm8Z8:APA91bHnaCxvovFdO-ML4LLKxNc_qhdK0FjQtgUhSbW3L9awmVcHyQbrFUMtAAl5LkRlhNRmZ9svlzoTo24BbFQNnH1klFcnioEkTxKU_Nu34tt2ypP3kKXr04g2KLELKilrLzGZDJsE";
		private string senderId = "129271001503";
		// string applicationID = "AAAAHhkm8Z8:APA91bHnaCxvovFdO-ML4LLKxNc_qhdK0FjQtgUhSbW3L9awmVcHyQbrFUMtAAl5LkRlhNRmZ9svlzoTo24BbFQNnH1klFcnioEkTxKU_Nu34tt2ypP3kKXr04g2KLELKilrLzGZDJsE"; // server key
		// string senderId = "129271001503"; // senderid
		private string webAddr = "https://fcm.googleapis.com/fcm/send";

		public string SendNotification(string DeviceToken, string title, string msg)
		{
			string str = "";

			WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
			tRequest.Method = "post";
			//serverKey - Key from Firebase cloud messaging server  
			//// tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAHhkm8Z8:APA91bHnaCxvovFdO-ML4LLKxNc_qhdK0FjQtgUhSbW3L9awmVcHyQbrFUMtAAl5LkRlhNRmZ9svlzoTo24BbFQNnH1klFcnioEkTxKU_Nu34tt2ypP3kKXr04g2KLELKilrLzGZDJsE"));
			tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAP1kVwxg:APA91bEGnOhfhaWJqwwBtW0uzn3nF-mSbJxDtjggcMRts5mith8ArpqpnW57HEhO3yKpohZZZs7PPF1LCsYMlioCFXzyFt5nRxeTCgPk-zlrX-YfQps6yCn1Z9bdVAFK7HnCja_S3Nsp"));
			//Sender Id - From firebase project setting  
			////  tRequest.Headers.Add(string.Format("Sender: id={0}", "129271001503"));
			tRequest.Headers.Add(string.Format("Sender: id={0}", "272077538072"));
			tRequest.ContentType = "application/json";
			var payload = new
			{
				to = "erb_TLUFL0tKg2z-hGUUzc:APA91bGfihDriLJbcJhRTaBUwVFXlK82oEdh7FmvBHwfki2QsJOFdU8FsgiWL3-GpbPahO3Ya_8nYaU-l3yg6iD1tCeHP0Dugu91ckDOKn9gcIT06-u-ENiNXQn0evDnid_ht_La-uTU",
				//to = "e2NVzx5yykLusYFG0Wt4vj:APA91bFi8lLpXrHNrWAXE_PtXXP2scJTU0u_YKtY6TPhxmpSXfO3zoeJyNknzKDkORokpryBo8qD6DJMe7TcCw96SRkV9l68a8r6sEEvrhi7X - vZSLbKsWtR2y1SxGQ8pwZwH2YdSNBS",
				//to = "cAecajbK-E_bo7M8y08tn1:APA91bF3sQA_a3EAai5FnVAJxB8dke3zkUXh6Q9hxTQkueTNQqaCfVbUYoqhRQbjxFxRzwr-LbYYFm-BVF6_1j_RulrzyZf7WqZfQwW0Uouin4zhyGmxU-xcBMq7GLWjw4s8LAWBOnBr",
				//to = "fYYyP3xOVwQ:APA91bErdDOfftO5HHzWawPuA5T5NYo3BrI3rrF2M0X4bbQlbb3Gfnuwn-yGs71TjnPs_Zjn8AR0WHGsxtiRVWh32sKr_MJoQBtUlQbMwaptSMF9PI8tgKnlTvTIq518FED8Lt-N7USz",
				//to= "fYYyP3xOVwQ:APA91bErdDOfftO5HHzWawPuA5T5NYo3BrI3rrF2M0X4bbQlbb3Gfnuwn-yGs71TjnPs_Zjn8AR0WHGsxtiRVWh32sKr_MJoQBtUlQbMwaptSMF9PI8tgKnlTvTIq518FED8Lt-N7USz",
				//to = "cuwTCkfhYtM:APA91bEmY8UzUT9mogmi9N59eIyh1ny_MgPv5DGmgfcFyWKvNt0tJieF0OBOFL04uvgRpp_yxESyJ8kqIgUW5p0QYB1l4mLFTGOTpE-eK_rkine675rtAJlYA3Zmyab7m9pJvFcifiax", //my
				//to = "fw-0lcZ2dqQ:APA91bEL5AC30W7kXc6h3I1vEZtq_nJdyNCWueVtkMm_2WLWCKJ3n35089vhZjsV-OcyK1yasKTsbrHF1NifeNOxX-fQmPZGSSUtqKUXwFEXFkTWtQhXzGTAl8Z-ny6FsstNa9qg8sRY",
				// to = "eRhRbil6EQo:APA91bG1PNi_lScKJ7g - j0ToZk7xsH7U8x02lmB973xcOcyt3jny5dhLtWaTm5TjarIIiTx7u9KSM24qlSJuCU8GqyV8oyVn7mC0XfyBh7DDwa5ZA - QDgOT3SKAUMj753P1czERCQbpg", //Android
				//  to = "caMqaVHNrDQ:APA91bGCO5RKdaJYZNyCqSlcgRaQr4_K5PZ2UlRWTDsHV25u2y_Wwvq6J11IaVUgrVVcoFOSsuEfiHic5mpa9W2hcbN7NeinWmD5IbZY5qKG6stQN6Y1QSC2eMCZySokEln-s-iXy50D", //sir
				//to = "APA91bHg1jFrKMyaO_h_BYYfm5fhJgd7UjzoYLj5RdfDWMulunvGM6QZV6ScBTB-l7O2dgynHiYpQwALoRLHB-8FkcIyY-LTZiUE7roeIafqkA0_t4cyg2DiHXSllbbkohhyjQ2m8YVa",
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
					body = "Test",
					title = "Test"
					//key1 = "value1",
					//key2 = "value2"
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

			return str;
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
			string str = "";

			WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
			tRequest.Method = "post";
			//serverKey - Key from Firebase cloud messaging server  
			//// tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAHhkm8Z8:APA91bHnaCxvovFdO-ML4LLKxNc_qhdK0FjQtgUhSbW3L9awmVcHyQbrFUMtAAl5LkRlhNRmZ9svlzoTo24BbFQNnH1klFcnioEkTxKU_Nu34tt2ypP3kKXr04g2KLELKilrLzGZDJsE"));
			tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAP1kVwxg:APA91bEGnOhfhaWJqwwBtW0uzn3nF-mSbJxDtjggcMRts5mith8ArpqpnW57HEhO3yKpohZZZs7PPF1LCsYMlioCFXzyFt5nRxeTCgPk-zlrX-YfQps6yCn1Z9bdVAFK7HnCja_S3Nsp"));
			//Sender Id - From firebase project setting  
			////  tRequest.Headers.Add(string.Format("Sender: id={0}", "129271001503"));
			tRequest.Headers.Add(string.Format("Sender: id={0}", "272077538072"));
			tRequest.ContentType = "application/json";
			var payload = new
			{
				to = "e8gpiUZ-RCCieVwE3DXNOF:APA91bFdKgTvXK74nF_MJcT4PRICrkJUuCuDl8RAwwdKBraMPajGgl0OftJ32cq-oG9V5228ajvSP9emLwuEXCMvOss-6h5hLMzeY_q5pqopc6yx_GI8PV0boVECdjZv7mgvnwqlliCk",
				//to = "erb_TLUFL0tKg2z-hGUUzc:APA91bGfihDriLJbcJhRTaBUwVFXlK82oEdh7FmvBHwfki2QsJOFdU8FsgiWL3-GpbPahO3Ya_8nYaU-l3yg6iD1tCeHP0Dugu91ckDOKn9gcIT06-u-ENiNXQn0evDnid_ht_La-uTU",
				//to = "e2NVzx5yykLusYFG0Wt4vj:APA91bFi8lLpXrHNrWAXE_PtXXP2scJTU0u_YKtY6TPhxmpSXfO3zoeJyNknzKDkORokpryBo8qD6DJMe7TcCw96SRkV9l68a8r6sEEvrhi7X - vZSLbKsWtR2y1SxGQ8pwZwH2YdSNBS",
				//to = "cl50VeDHokHDgljEqmVfOw:APA91bF6Hj5fM8gtJPT1a-nq0WJCJZLgjgXTontbuMlHOzwSqAixTSAJACrnBPdeiaWOWgbII07Sw-rGPR4Z4HC0tweZaiIPFxSnV0IXfFld8J83AIUeo7oeuZH8d5i_IR7XVB7Kq5Q6",
				//to= "d2i2ezulrEIdmdfDYhvvkO:APA91bH_PD3ZTILxCFDFtVFXRyfLfn3AvoJ1fM4e8aSVS7vtx_4aqUuRAOgrudpjmUKaRZ0U7tX5dyCYdRfP1HyExi0bAWt4Envuz6oIRZkpMULR0fuKUDR8LuHhkRGqRjL17nmAbWAm",
				//  to = "cukSp-4SWBTAbjjdbSQVJf:APA91bGZLSmTX9zkVrJCfDCsujOxkLWakqNB6zmyp8nsjpdElCz26pUvC-aIu-ul5ogVIH0CofMarzKCXw97uJv3XTXrOpRQswfUTncXaiwJ0V_vLk6U4216t58VBY61rN1Se6N4benD",
				//to = "caMqaVHNrDQ:APA91bGCO5RKdaJYZNyCqSlcgRaQr4_K5PZ2UlRWTDsHV25u2y_Wwvq6J11IaVUgrVVcoFOSsuEfiHic5mpa9W2hcbN7NeinWmD5IbZY5qKG6stQN6Y1QSC2eMCZySokEln-s-iXy50D",
				// to = "fYYyP3xOVwQ:APA91bErdDOfftO5HHzWawPuA5T5NYo3BrI3rrF2M0X4bbQlbb3Gfnuwn-yGs71TjnPs_Zjn8AR0WHGsxtiRVWh32sKr_MJoQBtUlQbMwaptSMF9PI8tgKnlTvTIq518FED8Lt-N7USz",
				priority = "high",
				content_available = true,

				notification = new
				{
					body = notificationcontent,
					title = notificationtitle
					//  badge = 1
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
			// }
			return 1;
		}

		[HttpGet]
		public ActionResult SendSMS(string mobileNo)
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
			return View();
		}

		[HttpGet]
		public ActionResult EditCustomer(int id)
		{
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{
				Sector objsec = new Sector();
				DataTable dtbuild = new DataTable();
				dtbuild = objsec.getBuildingList(null);
				ViewBag.Building = dtbuild;

				DataTable dtsec = new DataTable();
				dtsec = objsec.getSectorList(null);
				ViewBag.Sector = dtsec;

				DataTable dtflatno = new DataTable();
				dtflatno = objsec.getFlatNoList(null);
				ViewBag.FlatNo = dtflatno;

				DataTable dt = new DataTable();
				dt = objcust.BindCustomer(id);
				if (dt.Rows.Count > 0)
				{
					if (!string.IsNullOrEmpty(dt.Rows[0]["FirstName"].ToString()))
						ViewBag.FirstName = dt.Rows[0]["FirstName"].ToString();
					else
						ViewBag.FirstName = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["LastName"].ToString()))
						ViewBag.LastName = dt.Rows[0]["LastName"].ToString();
					else
						ViewBag.LastName = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["MobileNo"].ToString()))
						ViewBag.MobileNo = dt.Rows[0]["MobileNo"].ToString();
					else
						ViewBag.MobileNo = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["Email"].ToString()))
						ViewBag.Email = dt.Rows[0]["Email"].ToString();
					else
						ViewBag.Email = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["Address"].ToString()))
						ViewBag.Address = dt.Rows[0]["Address"].ToString();
					else
						ViewBag.Address = "";

					if (!string.IsNullOrEmpty(dt.Rows[0]["Photo"].ToString()))
						ViewBag.Photo = dt.Rows[0]["Photo"].ToString();
					else
						ViewBag.Photo = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["UserName"].ToString()))
						ViewBag.UserName = dt.Rows[0]["UserName"].ToString();
					else
						ViewBag.UserName = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["Password"].ToString()))
						ViewBag.Password = dt.Rows[0]["Password"].ToString();
					else
						ViewBag.Password = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["BuildingId"].ToString()))
					{
						ViewBag.BuildingId = dt.Rows[0]["BuildingId"].ToString();
					}
					else
						ViewBag.BuildingId = "0";


					//get sectorId
					DataTable dtSecId = objsec.getBuildingList(Convert.ToInt32(ViewBag.BuildingId));
					if (dtSecId.Rows.Count > 0)
					{
						if (!string.IsNullOrEmpty(dtSecId.Rows[0]["SectorId"].ToString()))
							ViewBag.SectorId = dtSecId.Rows[0]["SectorId"].ToString();
						else
							ViewBag.SectorId = "0";
						dtbuild = objsec.geSectorwisetBuildingList(Convert.ToInt32(dtSecId.Rows[0]["SectorId"]));
						ViewBag.Building = dtbuild;


					}
					if (!string.IsNullOrEmpty(dt.Rows[0]["BuildingId"].ToString()))
					{
						//  ViewBag.BuildingId = dt.Rows[0]["BuildingId"].ToString();
						dtflatno = objsec.getBuildingwiseFlatNoList(Convert.ToInt32(dt.Rows[0]["BuildingId"]));
						ViewBag.FlatNo = dtflatno;
					}
					//else
					//    ViewBag.BuildingId = "0";
					if (!string.IsNullOrEmpty(dt.Rows[0]["FlatId"].ToString()))
						ViewBag.FlatNoId = dt.Rows[0]["FlatId"].ToString();
					else
						ViewBag.FlatNoId = "0";
				}

				return View();
			}
			else
			{
				return RedirectToAction("Login", "Home");
			}
		}

		[HttpPost]
		public ActionResult EditCustomer(Customer objcust, FormCollection form, HttpPostedFileBase Photo)
		{
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{
				Sector objsec = new Sector();
				DataTable dtbuild = new DataTable();
				DataTable dtsec = new DataTable();
				DataTable dtflatno = new DataTable();
				DataTable dt = new DataTable();

				string building = Request["ddlBuilding"];
				if (!string.IsNullOrEmpty(building))
				{
					objcust.BuildingId = Convert.ToInt32(building);
				}

				string flat = Request["ddlFlatNo"];
				if (!string.IsNullOrEmpty(flat))
				{
					objcust.FlatId = Convert.ToInt32(flat);
				}
				//check data duplicate
				//DataTable dtDupliStaff = new DataTable();
				//dtDupliStaff = objcust.CheckDuplicateCustomer(objcust.FirstName, objcust.LastName, objcust.MobileNo);
				//if (dtDupliStaff.Rows.Count > 0)
				//{
				//int SId = Convert.ToInt32(dtDupliStaff.Rows[0]["Id"]);
				//if (SId == objcust.Id)
				//{
				//check username
				DataTable dtuserRecord1 = new DataTable();
				dtuserRecord1 = objcust.CheckCustomerUserName(objcust.MobileNo);
				int userRecords1 = dtuserRecord1.Rows.Count;
				if (userRecords1 > 0)
				{
					int SId = Convert.ToInt32(dtuserRecord1.Rows[0]["Id"]);
					if (SId == objcust.Id)
					{ }
					else
					{
						ViewBag.SuccessMsg = "MobileNo Already Exits!!!";
						dtbuild = objsec.getBuildingList(null);
						ViewBag.Building = dtbuild;


						dtsec = objsec.getSectorList(null);
						ViewBag.Sector = dtsec;


						dtflatno = objsec.getFlatNoList(null);
						ViewBag.FlatNo = dtflatno;


						dt = objcust.BindCustomer(objcust.Id);
						if (dt.Rows.Count > 0)
						{
							if (!string.IsNullOrEmpty(dt.Rows[0]["FirstName"].ToString()))
								ViewBag.FirstName = dt.Rows[0]["FirstName"].ToString();
							else
								ViewBag.FirstName = "";
							if (!string.IsNullOrEmpty(dt.Rows[0]["LastName"].ToString()))
								ViewBag.LastName = dt.Rows[0]["LastName"].ToString();
							else
								ViewBag.LastName = "";
							if (!string.IsNullOrEmpty(dt.Rows[0]["MobileNo"].ToString()))
								ViewBag.MobileNo = dt.Rows[0]["MobileNo"].ToString();
							else
								ViewBag.MobileNo = "";
							if (!string.IsNullOrEmpty(dt.Rows[0]["Email"].ToString()))
								ViewBag.Email = dt.Rows[0]["Email"].ToString();
							else
								ViewBag.Email = "";
							if (!string.IsNullOrEmpty(dt.Rows[0]["Address"].ToString()))
								ViewBag.Address = dt.Rows[0]["Address"].ToString();
							else
								ViewBag.Address = "";

							if (!string.IsNullOrEmpty(dt.Rows[0]["Photo"].ToString()))
								ViewBag.Photo = dt.Rows[0]["Photo"].ToString();
							else
								ViewBag.Photo = "";
							if (!string.IsNullOrEmpty(dt.Rows[0]["UserName"].ToString()))
								ViewBag.UserName = dt.Rows[0]["UserName"].ToString();
							else
								ViewBag.UserName = "";
							if (!string.IsNullOrEmpty(dt.Rows[0]["Password"].ToString()))
								ViewBag.Password = dt.Rows[0]["Password"].ToString();
							else
								ViewBag.Password = "";

							if (!string.IsNullOrEmpty(dt.Rows[0]["BuildingId"].ToString()))
								ViewBag.BuildingId = dt.Rows[0]["BuildingId"].ToString();
							else
								ViewBag.BuildingId = "0";
							if (!string.IsNullOrEmpty(dt.Rows[0]["FlatId"].ToString()))
								ViewBag.FlatNoId = dt.Rows[0]["FlatId"].ToString();
							else
								ViewBag.FlatNoId = "0";

							//get sectorId
							//Sector objsec = new Sector();
							DataTable dtSecId = objsec.getBuildingList(Convert.ToInt32(ViewBag.BuildingId));
							if (dtSecId.Rows.Count > 0)
							{
								if (!string.IsNullOrEmpty(dtSecId.Rows[0]["SectorId"].ToString()))
									ViewBag.SectorId = dtSecId.Rows[0]["SectorId"].ToString();
								else
									ViewBag.SectorId = "0";
							}
						}
						return View();
					}
				}
				//check Flatno
				DataTable dtflatno1 = new DataTable();
				dtflatno1 = objcust.CheckCustomerFlatNo(objcust.FlatId);
				int flatno = dtflatno1.Rows.Count;
				if (flatno > 0)
				{
					int SId = Convert.ToInt32(dtflatno1.Rows[0]["Id"]);
					if (SId == objcust.Id)
					{ }
					else
					{
						ViewBag.SuccessMsg = "FlatNo Already Exits!!!";
						dtbuild = objsec.getBuildingList(null);
						ViewBag.Building = dtbuild;


						dtsec = objsec.getSectorList(null);
						ViewBag.Sector = dtsec;


						dtflatno = objsec.getFlatNoList(null);
						ViewBag.FlatNo = dtflatno;


						dt = objcust.BindCustomer(objcust.Id);
						if (dt.Rows.Count > 0)
						{
							if (!string.IsNullOrEmpty(dt.Rows[0]["FirstName"].ToString()))
								ViewBag.FirstName = dt.Rows[0]["FirstName"].ToString();
							else
								ViewBag.FirstName = "";
							if (!string.IsNullOrEmpty(dt.Rows[0]["LastName"].ToString()))
								ViewBag.LastName = dt.Rows[0]["LastName"].ToString();
							else
								ViewBag.LastName = "";
							if (!string.IsNullOrEmpty(dt.Rows[0]["MobileNo"].ToString()))
								ViewBag.MobileNo = dt.Rows[0]["MobileNo"].ToString();
							else
								ViewBag.MobileNo = "";
							if (!string.IsNullOrEmpty(dt.Rows[0]["Email"].ToString()))
								ViewBag.Email = dt.Rows[0]["Email"].ToString();
							else
								ViewBag.Email = "";
							if (!string.IsNullOrEmpty(dt.Rows[0]["Address"].ToString()))
								ViewBag.Address = dt.Rows[0]["Address"].ToString();
							else
								ViewBag.Address = "";

							if (!string.IsNullOrEmpty(dt.Rows[0]["Photo"].ToString()))
								ViewBag.Photo = dt.Rows[0]["Photo"].ToString();
							else
								ViewBag.Photo = "";
							if (!string.IsNullOrEmpty(dt.Rows[0]["UserName"].ToString()))
								ViewBag.UserName = dt.Rows[0]["UserName"].ToString();
							else
								ViewBag.UserName = "";
							if (!string.IsNullOrEmpty(dt.Rows[0]["Password"].ToString()))
								ViewBag.Password = dt.Rows[0]["Password"].ToString();
							else
								ViewBag.Password = "";

							if (!string.IsNullOrEmpty(dt.Rows[0]["BuildingId"].ToString()))
								ViewBag.BuildingId = dt.Rows[0]["BuildingId"].ToString();
							else
								ViewBag.BuildingId = "0";
							if (!string.IsNullOrEmpty(dt.Rows[0]["FlatId"].ToString()))
								ViewBag.FlatNoId = dt.Rows[0]["FlatId"].ToString();
							else
								ViewBag.FlatNoId = "0";

							//get sectorId
							//Sector objsec = new Sector();
							DataTable dtSecId = objsec.getBuildingList(Convert.ToInt32(ViewBag.BuildingId));
							if (dtSecId.Rows.Count > 0)
							{
								if (!string.IsNullOrEmpty(dtSecId.Rows[0]["SectorId"].ToString()))
									ViewBag.SectorId = dtSecId.Rows[0]["SectorId"].ToString();
								else
									ViewBag.SectorId = "0";
							}
						}
						return View();
					}
				}
				string fname = null, path = null;
				HttpPostedFileBase document1 = Request.Files["Photo"];
				string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png" };
				if (document1 != null)
				{
					if (document1.ContentLength > 0)
					{
						try
						{
							HttpFileCollectionBase files = Request.Files;
							HttpPostedFileBase file = Photo;
							string fileName = Path.GetFileNameWithoutExtension(file.FileName);
							string extension = Path.GetExtension(file.FileName);
							if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
							{
								string[] testfiles = file.FileName.Split(new char[] { '\\' });
								fname = testfiles[testfiles.Length - 1];
							}
							else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
							{
								ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
							}
							else
							{
								fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
							}
							path = Path.Combine(Server.MapPath("~/image/customer/"), fname);
							file.SaveAs(path);
							objcust.Photo = fname;
						}

						catch (Exception ex)
						{
							ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
						}
					}
					else
					{
						var existfile = "";
						DataTable dt1 = objcust.BindCustomer(objcust.Id);
						ViewBag.Photo = dt1.Rows[0]["Photo"].ToString();
						existfile = ViewBag.Photo;
						objcust.Photo = existfile;
					}
				}
				else
				{
					var existfile = "";
					DataTable dt1 = objcust.BindCustomer(objcust.Id);
					ViewBag.Photo = dt1.Rows[0]["Photo"].ToString();
					existfile = ViewBag.Photo;
					objcust.Photo = existfile;
				}
				int addresult = objcust.UpdateCustomer(objcust);
				if (addresult > 0)
				{
					ViewBag.SuccessMsg = "Customer Updated Successfully!!!";
				}
				else
				{ ViewBag.SuccessMsg = "Customer Not Updated!!!"; }
				// }
				//else
				//{
				//    ViewBag.SuccessMsg = "Customer Already Exits!!!";
				//}
				// }
				dtbuild = objsec.getBuildingList(null);
				ViewBag.Building = dtbuild;


				dtsec = objsec.getSectorList(null);
				ViewBag.Sector = dtsec;


				dtflatno = objsec.getFlatNoList(null);
				ViewBag.FlatNo = dtflatno;


				dt = objcust.BindCustomer(objcust.Id);
				if (dt.Rows.Count > 0)
				{
					if (!string.IsNullOrEmpty(dt.Rows[0]["FirstName"].ToString()))
						ViewBag.FirstName = dt.Rows[0]["FirstName"].ToString();
					else
						ViewBag.FirstName = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["LastName"].ToString()))
						ViewBag.LastName = dt.Rows[0]["LastName"].ToString();
					else
						ViewBag.LastName = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["MobileNo"].ToString()))
						ViewBag.MobileNo = dt.Rows[0]["MobileNo"].ToString();
					else
						ViewBag.MobileNo = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["Email"].ToString()))
						ViewBag.Email = dt.Rows[0]["Email"].ToString();
					else
						ViewBag.Email = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["Address"].ToString()))
						ViewBag.Address = dt.Rows[0]["Address"].ToString();
					else
						ViewBag.Address = "";

					if (!string.IsNullOrEmpty(dt.Rows[0]["Photo"].ToString()))
						ViewBag.Photo = dt.Rows[0]["Photo"].ToString();
					else
						ViewBag.Photo = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["UserName"].ToString()))
						ViewBag.UserName = dt.Rows[0]["UserName"].ToString();
					else
						ViewBag.UserName = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["Password"].ToString()))
						ViewBag.Password = dt.Rows[0]["Password"].ToString();
					else
						ViewBag.Password = "";

					if (!string.IsNullOrEmpty(dt.Rows[0]["BuildingId"].ToString()))
						ViewBag.BuildingId = dt.Rows[0]["BuildingId"].ToString();
					else
						ViewBag.BuildingId = "0";
					if (!string.IsNullOrEmpty(dt.Rows[0]["FlatId"].ToString()))
						ViewBag.FlatNoId = dt.Rows[0]["FlatId"].ToString();
					else
						ViewBag.FlatNoId = "0";

					//get sectorId
					//Sector objsec = new Sector();
					DataTable dtSecId = objsec.getBuildingList(Convert.ToInt32(ViewBag.BuildingId));
					if (dtSecId.Rows.Count > 0)
					{
						if (!string.IsNullOrEmpty(dtSecId.Rows[0]["SectorId"].ToString()))
							ViewBag.SectorId = dtSecId.Rows[0]["SectorId"].ToString();
						else
							ViewBag.SectorId = "0";
					}
				}
				return View();
			}
			else
			{
				return RedirectToAction("Login", "Home");
			}
		}

		[HttpGet]
		public ActionResult CustomerList()
		{
			if (Request.Cookies["gstusr"] == null)
				return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

			var control = Helper.CheckPermission(Request.RawUrl.ToString());
			if (control.IsView == false)
				return Redirect("/notaccess/index");

			ViewBag.IsAdmin = control.IsAdmin;
			ViewBag.IsView = control.IsView;
			ViewBag.IsAdd = control.IsAdd;

			Customer objsec = new Customer();
			DataTable dt = new DataTable();
			dt = objsec.GetAllCustomer1(null);
			ViewBag.CustomerList = dt;



			return View();
		}

		public JsonResult Status(int ID)
		{
			var userInfo = _CustomerRepo.UpdateCustomerStatus(ID);
			if (userInfo != null)
			{
				if (userInfo.IsActive == false)
				{
					userInfo.IsActive = false;
					res["class"] = "label-danger";
					res["name"] = "InActive";
				}
				else
				{
					userInfo.IsActive = true;
					res["class"] = "label-success";
					res["name"] = "Active";
				}
				res["id"] = ID.ToString();
				res["status"] = "1";
			}
			return Json(res, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public ActionResult DeleteCustomer(int id)
		{
			try
			{
				int sectorId = 0;
				var customer = objcust.BindCustomer(id);
				if (customer.Rows.Count > 0)
				{
					if (!string.IsNullOrEmpty(customer.Rows[0]["SectorId"].ToString()))
						sectorId = Convert.ToInt32(customer.Rows[0]["SectorId"].ToString());

					if (!string.IsNullOrEmpty(customer.Rows[0]["UserName"].ToString()))
						sobjSub.DeleteCustomerOTPByCustomer(customer.Rows[0]["UserName"].ToString());
				}
				objcust.DeleteCustomerFavourite(id);
				DataTable dt = sobjSub.getCustomerOrderFutureAdmin(id, DateTime.Now.AddYears(-10), DateTime.Now.AddYears(10));
				if (dt.Rows.Count > 0)
				{
					for (int i = 0; i < dt.Rows.Count; i++)
					{
						int orderId = Convert.ToInt32(dt.Rows[i]["Id"]);
						sobjSub.DeleteCustomerOrder(orderId);
					}
				}
				sobjSub.DeleteCustomerWalletByCustomer(id);
				sobjSub.DeleteSubscriptionByCustomer(id);
				sobjSub.DeleteDeliveryBoyAssignByCustomer(id);
				sobjSub.DeletePaytmRequestByCustomer(id);

				int delresult = objcust.DeleteCustomer(id);
				_CustomerRepo.DeleteCustomer(id);
				//objcust.CustomerSortOrderDelete(id, sectorId);
				return RedirectToAction("CustomerList");
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				if (ex.Message.ToLower().Contains("fk_customer_staffcustassign") || ex.Message.ToLower().Contains("fk_customer_custsubscription") || ex.Message.ToLower().Contains("fk_order_customer") || ex.Message.ToLower().Contains("fk_wallet_customer"))
				{
					TempData["error"] = String.Format("You can not deleted. Child record found.");
				}
				else
					throw ex;
			}
			catch (Exception ex)
			{
				return View();
			}
			return RedirectToAction("CustomerList");
		}

		public ActionResult CustomerSortOrder(string SectorId, string StaffId)
		{
			if (Request.Cookies["gstusr"] == null)
				return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

			int? sectorId = new int();
			int? staffId = new int();

			Sector _sector = new Sector();
			DataTable sectorList = _sector.getSectorList(null);
			ViewBag.lstSector = sectorList;

			Staff objStaff = new Staff();
			DataTable dtStaff = objStaff.getDeliveryBoyList(null);
			ViewBag.lstStaff = dtStaff;

			if (!String.IsNullOrEmpty(SectorId))
			{
				sectorId = SectorId == "0" ? new Nullable<int>() : Convert.ToInt32(SectorId);
				ViewBag.SectorId = sectorId;
			}
			if (!String.IsNullOrEmpty(StaffId))
			{
				staffId = StaffId == "0" ? new Nullable<int>() : Convert.ToInt32(StaffId);
				ViewBag.StaffId = staffId;
			}

			//DataTable dtcustomer = _customer.GetCustomerBySectorDeliveryBoy(sectorId, staffId);
			//ViewBag.lstCustomer = dtcustomer;
			return View();
		}

		public PartialViewResult GetCustomerSortOrder(string sectorid, string staffId)
		{
			Customer _customer = new Customer();
			int? _sectorId = new int();
			int? _staffId = new int();
			if (!String.IsNullOrEmpty(sectorid))
			{
				_sectorId = sectorid == "0" ? new Nullable<int>() : Convert.ToInt32(sectorid);
				ViewBag.SectorId = _sectorId;
			}
			if (!String.IsNullOrEmpty(staffId))
			{
				_staffId = staffId == "0" ? new Nullable<int>() : Convert.ToInt32(staffId);
				ViewBag.StaffId = _staffId;
			}

			DataTable dtcustomer = _customer.GetCustomerBySectorDeliveryBoy(_sectorId, _staffId);
			ViewBag.lstCustomer = dtcustomer;
			return PartialView("_CustomerSortList");
		}

		public JsonResult UpdateCustomerSortOrder(string id, string order, string sectorid, string staffId)
		{
			if (string.IsNullOrEmpty(id))
				return Json("0");
			if (string.IsNullOrEmpty(order))
				return Json("0");
			if (string.IsNullOrEmpty(sectorid))
				return Json("0");
			if (string.IsNullOrEmpty(staffId))
				return Json("0");

			int customerId = Convert.ToInt32(id);
			Customer _customer = new Customer();
			DataTable cust = _customer.GetAllCustomer(customerId);
			if (cust.Rows.Count > 0)
			{
				int orderBy = 1000;
				if (!string.IsNullOrEmpty(cust.Rows[0]["OrderBy"].ToString()))
					orderBy = Convert.ToInt32(cust.Rows[0]["OrderBy"].ToString());
				//var dtSort = _customer.CheckCustomerSortOrder(Convert.ToInt32(sectorid), Convert.ToInt32(staffId), Convert.ToInt32(order));
				//_CustomerRepo.UpdateCustomerSortOrder(Convert.ToInt32(id), Convert.ToInt32(order));
				// if (dtSort.Rows.Count > 0)
				_customer.CustomerSortOrderUpdateBySector(orderBy, Convert.ToInt32(order), customerId, Convert.ToInt32(sectorid), Convert.ToInt32(staffId));
			}
			return Json("1");
		}

		[HttpGet]
		public ActionResult AddStaffCustomerAssign()
		{
			//if (Request.Cookies["gstusr"] == null)
			//    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
			if (HttpContext.Session["UserId"] == null)
				return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

			var control = Helper.CheckPermission(Request.RawUrl.ToString());
			if (control.IsView == false)
				return Redirect("/notaccess/index");

			ViewBag.IsAdmin = control.IsAdmin;
			ViewBag.IsView = control.IsView;
			ViewBag.IsAdd = control.IsAdd;

			DataTable dt = new DataTable();
			dt = objcust.GetAllCustomer(null);
			ViewBag.Customer = dt;

			Staff objStaff = new Staff();
			DataTable dtStaff = new DataTable();
			dtStaff = objStaff.getDeliveryBoyList(null);
			ViewBag.Staff = dtStaff;

			return View();
		}

		[HttpPost]
		public ActionResult AddStaffCustomerAssign(FormCollection form)
		{
			//if (Request.Cookies["gstusr"] == null)
			//    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
			if (HttpContext.Session["UserId"] == null)
				return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

			string StaffId = Request["ddlStaff"];
			if (!string.IsNullOrEmpty(StaffId))
			{
				objcust.StaffId = Convert.ToInt32(StaffId);
			}
			string CustId = Request["ddlCustomer"];
			if (!string.IsNullOrEmpty(CustId))
			{
				objcust.CustomerId = Convert.ToInt32(CustId);
			}
			//check duplicate record
			DataTable dtDupliAssign = objcust.DuplicateStaffCustomer(objcust.StaffId, objcust.CustomerId);
			if (dtDupliAssign.Rows.Count > 0)
			{ ViewBag.SuccessMsg = "Customer Already Assign!!!"; }
			else
			{
				//int addresult = 0;
				int addresult = objcust.InsertStaffCustAssign(objcust);
				if (addresult > 0)
				{ ViewBag.SuccessMsg = "Inserted Successfully!!!"; }
				else
				{ ViewBag.SuccessMsg = "Not Inserted!!!"; }
			}

			ModelState.Clear();
			DataTable dt = new DataTable();
			dt = objcust.GetAllCustomer(null);
			ViewBag.Customer = dt;

			Staff objStaff = new Staff();
			DataTable dtStaff = new DataTable();
			dtStaff = objStaff.getStaffList(null);
			return View();
		}

		[HttpGet]
		public ActionResult StaffCustomerAssignList()
		{
			//if (Request.Cookies["gstusr"] == null)
			//    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

			if (HttpContext.Session["UserId"] == null)
				return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
			var control = Helper.CheckPermission(Request.RawUrl.ToString());
			if (control.IsView == false)
				return Redirect("/notaccess/index");

			ViewBag.IsAdmin = control.IsAdmin;
			ViewBag.IsView = control.IsView;
			ViewBag.IsAdd = control.IsAdd;

			Customer objsec = new Customer();
			DataTable dt = new DataTable();
			dt = objsec.StaffCustomerList(null);
			ViewBag.StaffCustomerList = dt;

			return View();
		}

		[HttpGet]
		public ActionResult EditStaffCustomerAssign(int id = 0)
		{
			//if (Request.Cookies["gstusr"] == null)
			//    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
			if (HttpContext.Session["UserId"] == null)
				return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

			DataTable dt = new DataTable();
			dt = objcust.BindCustomer(null);
			ViewBag.Customer = dt;

			Staff objStaff = new Staff();
			DataTable dtStaff = new DataTable();
			dtStaff = objStaff.getDeliveryBoyList(null);
			ViewBag.Staff = dtStaff;


			DataTable dtEdit = objcust.StaffCustomerList(id);
			if (dtEdit.Rows.Count > 0)
			{
				if (!string.IsNullOrEmpty(dtEdit.Rows[0]["StaffId"].ToString()))
					ViewBag.StaffId = dtEdit.Rows[0]["StaffId"].ToString();
				else
					ViewBag.StaffId = "0";
				if (!string.IsNullOrEmpty(dtEdit.Rows[0]["CustomerId"].ToString()))
					ViewBag.CustomerId = dtEdit.Rows[0]["CustomerId"].ToString();
				else
					ViewBag.CustomerId = "0";
			}

			return View();
		}

		[HttpPost]
		public ActionResult EditStaffCustomerAssign(Customer objcust, FormCollection form)
		{
			//if (Request.Cookies["gstusr"] == null)
			//    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
			if (HttpContext.Session["UserId"] == null)
				return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

			string StaffId = Request["ddlStaff"];
			if (!string.IsNullOrEmpty(StaffId))
			{
				objcust.StaffId = Convert.ToInt32(StaffId);
			}
			string CustId = Request["ddlCustomer"];
			if (!string.IsNullOrEmpty(CustId))
			{
				objcust.CustomerId = Convert.ToInt32(CustId);
			}
			//check duplicate record
			DataTable dtDupliAssign = objcust.DuplicateStaffCustomer(objcust.StaffId, objcust.CustomerId);
			if (dtDupliAssign.Rows.Count > 0)
			{
				int SId = Convert.ToInt32(dtDupliAssign.Rows[0]["Id"]);
				if (SId == objcust.Id)
				{
					//int addresult = 0;
					int addresult = objcust.UpdateStaffCustAssign(objcust);
					if (addresult > 0)
					{
						ViewBag.SuccessMsg = "Updated Successfully!!!";

					}
					else
					{ ViewBag.SuccessMsg = "Not Updated!!!"; }
				}
				else
				{ ViewBag.SuccessMsg = "Customer Already Assign!!!"; }
			}
			else
			{
				//int addresult = 0;
				int addresult = objcust.UpdateStaffCustAssign(objcust);
				if (addresult > 0)
				{
					ViewBag.SuccessMsg = "Updated Successfully!!!";

				}
				else
				{ ViewBag.SuccessMsg = "Not Updated!!!"; }
			}


			DataTable dt = new DataTable();
			dt = objcust.BindCustomer(null);
			ViewBag.Customer = dt;

			Staff objStaff = new Staff();
			DataTable dtStaff = new DataTable();
			dtStaff = objStaff.getDeliveryBoyList(null);
			ViewBag.Staff = dtStaff;

			DataTable dtEdit = objcust.StaffCustomerList(objcust.Id);
			if (dtEdit.Rows.Count > 0)
			{
				if (!string.IsNullOrEmpty(dtEdit.Rows[0]["StaffId"].ToString()))
					ViewBag.StaffId = dtEdit.Rows[0]["StaffId"].ToString();
				else
					ViewBag.StaffId = "0";
				if (!string.IsNullOrEmpty(dtEdit.Rows[0]["CustomerId"].ToString()))
					ViewBag.CustomerId = dtEdit.Rows[0]["CustomerId"].ToString();
				else
					ViewBag.CustomerId = "0";
			}
			return View();
		}

		[HttpGet]
		public ActionResult DeleteStaffCustomerAssign(int id)
		{
			try
			{
				int delresult = objcust.DeleteStaffCustAssign(id);
				return RedirectToAction("StaffCustomerAssignList");
			}
			//catch (System.Data.SqlClient.SqlException ex)
			//{
			//    if (ex.Message.ToLower().Contains("fk_customer_staffcustassign") || ex.Message.ToLower().Contains("fk_customer_custsubscription") || ex.Message.ToLower().Contains("fk_order_customer") || ex.Message.ToLower().Contains("fk_wallet_customer"))
			//    {
			//        TempData["error"] = String.Format("You can not deleted. Child record found.");
			//    }
			//    else
			//        throw ex;
			//}
			catch (Exception ex)
			{
				return View();
			}
			return RedirectToAction("StaffCustomerAssignList");
		}

		[HttpGet]
		public ActionResult CustomerWalletList(int? page, string FromDate, string ToDate, string ddlCustomer)
		{
			if (Request.Cookies["gstusr"] == null)
				return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

			int sectorId = 0;
			if (Session["isVendorLogin"] != null && Convert.ToBoolean(Session["isVendorLogin"]) == true)
			{
				sectorId = Convert.ToInt32(Session["VendorSectorId"]);
			}

			//var control = Helper.CheckPermission(Request.RawUrl.ToString());
			//if (control.IsView == false)
			//    return Redirect("/notaccess/index");

			//ViewBag.IsAdmin = control.IsAdmin;
			//ViewBag.IsView = control.IsView;
			//ViewBag.IsAdd = control.IsAdd;

			int pageSize = 25;
			int pageIndex = 1;
			pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
			Customer objorder = new Customer();
			Subscription objsec = new Subscription();
			DataTable dt = new DataTable();

			DataTable dtc = new DataTable();
			dtc = objorder.BindCustomer(null);
			ViewBag.Customer = dtc;

			DateTime? _fromDate = new Nullable<DateTime>();
			DateTime? _toDate = new Nullable<DateTime>();
			int? customerId = new int();

			if (!String.IsNullOrEmpty(FromDate))
			{
				_fromDate = Convert.ToDateTime(DateTime.ParseExact(FromDate, @"dd/MM/yyyy", null));
				ViewBag.FromDate = FromDate;
			}
			if (!String.IsNullOrEmpty(ToDate))
			{
				_toDate = Convert.ToDateTime(DateTime.ParseExact(ToDate, @"dd/MM/yyyy", null));
				ViewBag.ToDate = ToDate;
			}
			if (!String.IsNullOrEmpty(ddlCustomer))
			{
				customerId = ddlCustomer == "0" ? new Nullable<int>() : Convert.ToInt32(ddlCustomer);
				ViewBag.ct = ddlCustomer;
				ViewBag.ddlCustomer = ddlCustomer;
			}

			dt = objsec.getDateCustomerWalletTransAll(customerId, _fromDate, _toDate, sectorId);
			var customerList = (from DataRow dr in dt.Rows
								select new CustomerWalletModel()
								{
									Id = dr["Id"] == null ? "" : dr["Id"].ToString(),
									Tdate = dr["Tdate"] == null ? "" : dr["Tdate"].ToString(),
									Customer = dr["Customer"] == null ? "" : dr["Customer"].ToString(),
									BillNo = dr["BillNo"] == null ? "" : dr["BillNo"].ToString(),
									OrderNo = dr["OrderNo"] == null ? "" : dr["OrderNo"].ToString(),
									OrderDate = dr["OrderDate"] == null ? "" : dr["OrderDate"].ToString(),
									Subscription = dr["Subscription"] == null ? "" : dr["Subscription"].ToString(),
									Amount = dr["Amount"] == null ? "" : dr["Amount"].ToString(),
									Type = dr["Type"] == null ? "" : dr["Type"].ToString(),
									Description = dr["Description"] == null ? "" : dr["Description"].ToString()
								}).ToList();

			ViewBag.RowCount = (pageSize * (pageIndex - 1));
			IPagedList<CustomerWalletModel> list = customerList.ToPagedList(pageIndex, pageSize);
			return View(list);
		}

		[HttpPost]
		public ActionResult CustomerWalletList(FormCollection form)
		{
			if (Request.Cookies["gstusr"] == null)
				return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

			Subscription objsec = new Subscription();
			CustomerOrder objorder = new CustomerOrder();
			var fdate = Request["datepicker"];
			if (!string.IsNullOrEmpty(fdate.ToString()))
			{
				objorder.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
			}
			var tdate = Request["datepicker1"];
			if (!string.IsNullOrEmpty(tdate.ToString()))
			{
				objorder.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
			}
			string customer = Request["ddlCustomer"];
			if ((!string.IsNullOrEmpty(customer)) && customer != "0")
			{
				objorder.CustomerId = Convert.ToInt32(customer);
			}

			DataTable dt = new DataTable();
			dt = objsec.getDateCustomerWalletTransAll(objorder.CustomerId, objorder.FromDate, objorder.ToDate);
			ViewBag.CustomerWalletList = dt;
			if (!string.IsNullOrEmpty(fdate.ToString()))
			{
				ViewBag.FromDate = fdate;
			}
			if (!string.IsNullOrEmpty(tdate.ToString()))
			{
				ViewBag.ToDate = tdate;
			}
			if (!string.IsNullOrEmpty(customer.ToString()))
			{
				ViewBag.ct = customer;
			}
			Customer objorderc = new Customer();
			DataTable dtc = new DataTable();
			dtc = objorderc.BindCustomer(null);
			ViewBag.Customer = dtc;
			return View();
		}

		[HttpGet]
		public ActionResult AddCustomerWallet()
		{
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{
				var control = Helper.CheckPermission(Request.RawUrl.ToString());
				if (control.IsView == false)
					return Redirect("/notaccess/index");

				ViewBag.IsAdmin = control.IsAdmin;
				ViewBag.IsView = control.IsView;
				ViewBag.IsAdd = control.IsAdd;

				Customer objorder = new Customer();
				Subscription objsub = new Subscription();

				DataTable dt = new DataTable();
				dt = objorder.BindCustomer(null);
				ViewBag.Customer = dt;

				DataTable dtOrder = objorder.GetCustomerwiseOrder(null);
				ViewBag.OrderNo = dtOrder;

				DataTable dtsubscri = new DataTable();
				dtsubscri = objsub.getSubscriptionList(null);
				ViewBag.Subscription = dtsubscri;

				con.Open();
				SqlCommand com1 = new SqlCommand("Wallet_GetBillNo", con);
				com1.CommandType = CommandType.StoredProcedure;
				var Formno = com1.ExecuteScalar();
				con.Close();
				ViewBag.billno = "MWI" + Formno;
				return View();
			}
			else
			{
				return RedirectToAction("Login", "Home");
			}
		}

		[HttpPost]
		public ActionResult AddCustomerWallet(Subscription objsec, FormCollection form)
		{
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{
				int result = 0; decimal Walletbal = 0, TotalCredit = 0, TotalDebit = 0;
				// Subscription objsec = new Subscription();
				DataTable dt = new DataTable();
				Customer objorder = new Customer();
				string type = Request["ddlType"];
				var fdate = Request["TransactionDate"];
				if (!string.IsNullOrEmpty(fdate.ToString()))
				{
					objsec.TransactionDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
				}

				if (type == "1")
				{
					objsec.OrderId = 0;
					objsec.Type = "Credit";
					objsec.TransactionType = Convert.ToInt32(Helper.TransactionType.Deposit);
					objsec.CustSubscriptionId = 0;
					objsec.Description = "Add To Wallet " + objsec.Description;
				}
				else if (type == "2")
				{
					objsec.BillNo = null;
					objsec.Type = "Debit";
					objsec.CustSubscriptionId = 0;
					objsec.TransactionType = Convert.ToInt32(Helper.TransactionType.Purchase);
					objsec.Description = "Place Order " + objsec.Description;
					string orderid = Request["ddlOrderNo"];
					if (!string.IsNullOrEmpty(orderid))
					{
						objsec.OrderId = Convert.ToInt32(orderid);
					}
					//chcek wallet balance

					DataTable dtprodRecord = new DataTable();
					var balance = objsec.GetCustomerBalace(objsec.CustomerId);
					Walletbal = balance;
				}
				else if (type == "3")
				{
					objsec.BillNo = null;
					objsec.Type = "Debit";
					objsec.OrderId = 0;
					objsec.TransactionType = Convert.ToInt32(Helper.TransactionType.Subscription);
					objsec.Description = "Purchase Subscription " + objsec.Description;
					string subscription = Request["ddlSubscription"];
					if (!string.IsNullOrEmpty(subscription))
					{
						objsec.CustSubscriptionId = Convert.ToInt32(subscription);
					}
				}
				else
				{
					objsec.BillNo = null;
					objsec.OrderId = 0;
					objsec.CustSubscriptionId = 0;
				}

				string customer = Request["ddlCustomer"];
				if (!string.IsNullOrEmpty(customer))
				{
					objsec.CustomerId = Convert.ToInt32(customer);
				}
				//check wallet balance for order 
				if (type == "2" && Walletbal > objsec.Amount)
					result = objsec.InsertWallet(objsec);
				else
					result = objsec.InsertWallet(objsec);
				// result = 0;
				if (result > 0)
				{
					ViewBag.SuccessMsg = "Wallet Insert Successfully";
					//update in order amount is debit status change
					if (type == "2")
					{
						int TotalRewardPoint = 0;
						int UpdateOrderStatus = 0;
						var date = objsec.TransactionDate.Date;
						//order status 
						UpdateOrderStatus = objsec.UpdateCustomerOrderCancle(objsec.OrderId, objsec.CustomerId, Convert.ToDateTime(date), "Complete");
						//add Rewards Point to Customer table
						DataTable dtGetCustomerPoint = objcust.BindCustomer(objsec.CustomerId);
						if (dtGetCustomerPoint.Rows.Count > 0)
						{
							if (!string.IsNullOrEmpty(dtGetCustomerPoint.Rows[0]["RewardPoint"].ToString()))
								TotalRewardPoint = Convert.ToInt32(dtGetCustomerPoint.Rows[0]["RewardPoint"]);
							objsec.RewardPoint = objsec.RewardPoint + TotalRewardPoint;
							int UpdateCustomer = objcust.UpdateCustomerPoint(objsec.CustomerId, Convert.ToInt64(objsec.RewardPoint));
						}
					}
				}
				else if (type == "2" && Walletbal > objsec.Amount)
				{
					ViewBag.SuccessMsg = "Low Wallet balance";
				}
				else
				{
					ViewBag.SuccessMsg = "Not Inserted";
				}

				ModelState.Clear();

				dt = objorder.BindCustomer(null);
				ViewBag.Customer = dt;

				DataTable dtOrder = objorder.GetCustomerwiseOrder(null);
				ViewBag.OrderNo = dtOrder;

				DataTable dtsubscri = new DataTable();
				dtsubscri = objsec.getSubscriptionList(null);
				ViewBag.Subscription = dtsubscri;

				con.Open();
				SqlCommand com1 = new SqlCommand("Wallet_GetBillNo", con);
				com1.CommandType = CommandType.StoredProcedure;
				var Formno = com1.ExecuteScalar();
				con.Close();
				ViewBag.billno = "MWI" + Formno;
				return View();
			}
			else
			{
				return RedirectToAction("Login", "Home");
			}
		}

		[HttpGet]
		public ActionResult EditCustomerWallet(int id)
		{
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{
				Subscription objsub = new Subscription();
				Customer objorder = new Customer();
				DataTable dt = new DataTable();
				dt = objorder.BindCustomer(null);
				ViewBag.Customer = dt;

				DataTable dtData = new DataTable();
				dtData = objsub.getWalletSelect(id);
				if (dtData.Rows.Count > 0)
				{
					if (!string.IsNullOrEmpty(dtData.Rows[0]["CustomerId"].ToString()))
						ViewBag.CustomerId = dtData.Rows[0]["CustomerId"].ToString();
					else
						ViewBag.CustomerId = "0";
					if (!string.IsNullOrEmpty(dtData.Rows[0]["StaffId"].ToString()))
						ViewBag.StaffId = dtData.Rows[0]["StaffId"].ToString();
					else
						ViewBag.StaffId = "0";
					if (!string.IsNullOrEmpty(dtData.Rows[0]["StaffId"].ToString()))
						ViewBag.StaffId = dtData.Rows[0]["StaffId"].ToString();
					else
						ViewBag.StaffId = "0";
					if (!string.IsNullOrEmpty(dtData.Rows[0]["StaffId"].ToString()))
						ViewBag.StaffId = dtData.Rows[0]["StaffId"].ToString();
					else
						ViewBag.StaffId = "0";
					if (!string.IsNullOrEmpty(dtData.Rows[0]["StaffId"].ToString()))
						ViewBag.StaffId = dtData.Rows[0]["StaffId"].ToString();
					else
						ViewBag.StaffId = "0";
					if (!string.IsNullOrEmpty(dtData.Rows[0]["StaffId"].ToString()))
						ViewBag.StaffId = dtData.Rows[0]["StaffId"].ToString();
					else
						ViewBag.StaffId = "0";
				}

				return View();
			}
			else
			{
				return RedirectToAction("Login", "Home");
			}
		}

		[HttpPost]
		public ActionResult EditCustomerWallet(Subscription objsec, FormCollection form)
		{
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{
				int result = 0;
				// Subscription objsec = new Subscription();
				DataTable dt = new DataTable();
				string type = Request["ddlType"];
				objsec.TransactionType = Convert.ToInt32(type);
				string customer = Request["ddlCustomer"];
				if (!string.IsNullOrEmpty(customer))
				{
					objsec.CustomerId = Convert.ToInt32(customer);
				}
				objsec.OrderId = 0;
				//generate bill no
				objsec.Type = "Credit";
				objsec.CustSubscriptionId = 0;

				result = objsec.InsertWallet(objsec);
				// result = 0;
				if (result > 0)
				{
					ViewBag.SuccessMsg = "Amount Added into Wallet Successfully";
				}
				else
				{
					ViewBag.SuccessMsg = "Not Inserted";
				}

				ModelState.Clear();

				return View();
			}
			else
			{
				return RedirectToAction("Login", "Home");
			}
		}

		[HttpPost]
		public ActionResult GetCustomerOrder(string id)
		{
			DataTable dt = new DataTable();
			dt = objcust.GetCustomerwiseOrder(id);

			System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
			List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
			Dictionary<string, object> row;
			foreach (DataRow dr in dt.Rows)
			{
				row = new Dictionary<string, object>();
				foreach (DataColumn col in dt.Columns)
				{
					row.Add(col.ColumnName, dr[col]);
				}
				rows.Add(row);
			}
			string jsonString = string.Empty;
			jsonString = JsonConvert.SerializeObject(rows);
			return Json(jsonString, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult GetOrderAmount(string id)
		{
			Subscription objsub = new Subscription();
			DataTable dt = new DataTable();
			dt = objsub.getCustomerOrderList(id);

			System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
			List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
			Dictionary<string, object> row;
			foreach (DataRow dr in dt.Rows)
			{
				row = new Dictionary<string, object>();
				foreach (DataColumn col in dt.Columns)
				{
					row.Add(col.ColumnName, dr[col]);
				}
				rows.Add(row);
			}
			string jsonString = string.Empty;
			jsonString = JsonConvert.SerializeObject(rows);
			return Json(jsonString, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult GetSubscriptionAmount(string id)
		{
			Subscription objsub = new Subscription();
			DataTable dt = new DataTable();
			dt = objsub.getSubscriptionList(Convert.ToInt32(id));

			System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
			List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
			Dictionary<string, object> row;
			foreach (DataRow dr in dt.Rows)
			{
				row = new Dictionary<string, object>();
				foreach (DataColumn col in dt.Columns)
				{
					row.Add(col.ColumnName, dr[col]);
				}
				rows.Add(row);
			}
			string jsonString = string.Empty;
			jsonString = JsonConvert.SerializeObject(rows);
			return Json(jsonString, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public ActionResult DeleteWallet(int id)
		{
			Subscription objsub = new Subscription();
			try
			{
				//get customer id

				//check order data in transaction with Pending


				int delresult = objsub.DeleteCustomerWallet(id);
				return RedirectToAction("CustomerWalletList");
			}
			//catch (System.Data.SqlClient.SqlException ex)
			//{
			//    if (ex.Message.ToLower().Contains("fk_customer_staffcustassign") || ex.Message.ToLower().Contains("fk_customer_custsubscription") || ex.Message.ToLower().Contains("fk_order_customer") || ex.Message.ToLower().Contains("fk_wallet_customer"))
			//    {
			//        TempData["error"] = String.Format("You can not deleted. Child record found.");
			//    }
			//    else
			//        throw ex;
			//}
			catch (Exception ex)
			{
				return View();
			}
			return RedirectToAction("CustomerWalletList");
		}

		[HttpPost]
		public ActionResult GetCustomerDetail(string customerid)
		{
			Subscription objsub = new Subscription();
			Customer objcust = new Customer();

			DataTable dt = new DataTable();
			dt = objcust.BindsubDateCustomer(Convert.ToInt32(customerid));

			System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
			List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
			Dictionary<string, object> row;
			foreach (DataRow dr in dt.Rows)
			{
				row = new Dictionary<string, object>();
				foreach (DataColumn col in dt.Columns)
				{
					row.Add(col.ColumnName, dr[col]);
				}
				rows.Add(row);
			}
			string jsonString = string.Empty;
			jsonString = JsonConvert.SerializeObject(rows);

			//chcek wallet amount
			decimal Walletbal = 0;
			DataTable dtprodRecord = new DataTable();
			var balance = objsub.GetCustomerBalace(Convert.ToInt32(customerid));
			Walletbal = balance;

			string jsonString1 = string.Empty;
			jsonString1 = JsonConvert.SerializeObject(Walletbal);

			int RewardPoint = 0;
			DataTable dtreward = new DataTable();
			dtreward = objcust.BindCustomer(Convert.ToInt32(customerid));
			int userRecords1 = dtreward.Rows.Count;

			if (userRecords1 > 0)
			{
				if (!string.IsNullOrEmpty(dtreward.Rows[0]["RewardPoint"].ToString()))
					RewardPoint = Convert.ToInt32(dtreward.Rows[0]["RewardPoint"]);
			}
			string jsonString2 = string.Empty;
			jsonString2 = JsonConvert.SerializeObject(RewardPoint);
			return Json(new { jsonString, jsonString1, jsonString2 }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public ActionResult CustomerSubReport()
		{
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{

				Customer objcust = new Customer();
				CustomerOrder objorder = new CustomerOrder();
				Sector objsector = new Sector();

				DataTable dt = new DataTable();
				dt = objsector.getSectorList(null);
				ViewBag.Sector = dt;

				Sector objsec = new Sector();
				DataTable dt1 = new DataTable();
				dt1 = objsec.getBuildingList(null);
				ViewBag.Building = dt1;

				DataTable dtList = new DataTable();
				dtList = objorder.getSectorSubscriptiondays(null, null, null);
				ViewBag.ProductorderList = dtList;

				return View();
			}
			else
			{
				return RedirectToAction("Login", "Home");
			}
		}

		[HttpPost]
		public ActionResult CustomerSubReport(FormCollection form, CustomerOrder objorder)
		{
			Staff objstaff = new Staff();
			Subscription objsub = new Subscription();
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{
				string SectorId = Request["ddlSector"];
				if (!string.IsNullOrEmpty(SectorId) && Convert.ToInt32(SectorId) != 0)
				{
					objorder.SectorId = Convert.ToInt32(SectorId);
				}
				string BuildingId = Request["ddlBuilding"];
				if (!string.IsNullOrEmpty(BuildingId) && Convert.ToInt32(BuildingId) != 0)
				{
					objorder.BuildingId = Convert.ToInt32(BuildingId);
				}
				string DaysId = Request["ddlDays"];
				if (!string.IsNullOrEmpty(DaysId) && Convert.ToInt32(DaysId) != 0)
				{
					objorder.DaysId = Convert.ToInt32(DaysId);
				}

				DataTable dtList = new DataTable();
				dtList = objorder.getSectorSubscriptiondays(objorder.SectorId, objorder.BuildingId, objorder.DaysId);
				ViewBag.ProductorderList = dtList;

				Sector objsec = new Sector();
				DataTable dt = new DataTable();
				dt = objsec.getSectorList(null);
				ViewBag.Sector = dt;


				DataTable dt1 = new DataTable();
				dt1 = objsec.getBuildingList(null);
				ViewBag.Building = dt1;


				ViewBag.SectorId = objorder.SectorId;
				ViewBag.BuildingId = objorder.BuildingId;
				ViewBag.DaysId = objorder.DaysId;

				return View();
			}
			else
			{
				// return PartialView("DeliveryBoyCustomerPartial");
				return RedirectToAction("Login", "Home");
			}
		}

		[HttpGet]
		public ActionResult CustomerWalletReport()
		{
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{
				CustomerOrder objorder = new CustomerOrder();
				DataTable dt = new DataTable();
				dt = objcust.BindCustomer(null);
				ViewBag.Customer = dt;

				Staff objStaff = new Staff();
				DataTable dtStaff = new DataTable();
				dtStaff = objStaff.getDeliveryBoyList(null);
				ViewBag.Staff = dtStaff;

				DataTable dtList = new DataTable();
				dtList = objorder.getDeliveryBoyCustomerOrder(null, null, null, null, null);
				ViewBag.ProductorderList = dtList;

				return View();
			}
			else
			{
				return RedirectToAction("Login", "Home");
			}
		}

		[HttpPost]
		public ActionResult CustomerWalletReport(FormCollection form, CustomerOrder objorder)
		{
			Staff objstaff = new Staff();
			Subscription objsub = new Subscription();
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{
				string StaffId = Request["ddlStaff"];
				if (!string.IsNullOrEmpty(StaffId) && Convert.ToInt32(StaffId) != 0)
				{
					objorder.StaffId = Convert.ToInt32(StaffId);
				}
				string CustomerId = Request["ddlCustomer"];
				if (!string.IsNullOrEmpty(CustomerId) && Convert.ToInt32(CustomerId) != 0)
				{
					objorder.CustomerId = Convert.ToInt32(CustomerId);
				}
				var fdate = Request["datepicker"];
				if (!string.IsNullOrEmpty(fdate.ToString()))
				{
					objorder.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
				}
				var tdate = Request["datepicker1"];
				if (!string.IsNullOrEmpty(tdate.ToString()))
				{
					objorder.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
				}
				DataTable dtList = new DataTable();
				dtList = objorder.getDeliveryBoyCustomerOrder(objorder.StaffId, objorder.CustomerId, objorder.FromDate, objorder.ToDate, null);
				ViewBag.ProductorderList = dtList;

				DataTable dt = new DataTable();
				dt = objcust.BindCustomer(null);
				ViewBag.Customer = dt;

				Staff objStaff = new Staff();
				DataTable dtStaff = new DataTable();
				dtStaff = objStaff.getDeliveryBoyList(null);
				ViewBag.Staff = dtStaff;

				ViewBag.DeliveryBoyId = objorder.StaffId;
				ViewBag.CustomerId = objorder.CustomerId;

				return View();
			}
			else
			{
				// return PartialView("DeliveryBoyCustomerPartial");
				return RedirectToAction("Login", "Home");
			}
		}

		[HttpGet]
		public ActionResult CustomerNotificationList()
		{
			if (Request.Cookies["gstusr"] == null)
				return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

			var control = Helper.CheckPermission(Request.RawUrl.ToString());
			if (control.IsView == false)
				return Redirect("/notaccess/index");

			ViewBag.IsAdmin = control.IsAdmin;
			ViewBag.IsView = control.IsView;
			ViewBag.IsAdd = control.IsAdd;

			Customer objsec = new Customer();
			DataTable dt = new DataTable();
			dt = objsec.GetAllCustomerNotification(null);
			ViewBag.CustomerNotList = dt;



			dt = objsec.BindCustomer(null);
			ViewBag.Customer = dt;

			Sector objsector = new Sector();
			DataTable dtList = new DataTable();
			dtList = objsector.getStateList(null);
			ViewBag.StateList = dtList;
			return View();
		}

		[HttpPost]
		public ActionResult CustomerNotificationList(Customer obj, FormCollection frm)
		{
			DataTable dt = new DataTable();
			if (Request.Cookies["gstusr"] == null)
				return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

			var control = Helper.CheckPermission(Request.RawUrl.ToString());
			if (control.IsView == false)
				return Redirect("/notaccess/index");

			ViewBag.IsAdmin = control.IsAdmin;
			ViewBag.IsView = control.IsView;
			ViewBag.IsAdd = control.IsAdd;
			Customer objsec = new Customer();
			string submit = Request["submit"];
			string CustomerId = Request["ddlCustomer"];
			string Sectorid = Request["ddlSector"];
			int sector = 0;
			string type = "";
			if (!string.IsNullOrEmpty(CustomerId) && Convert.ToInt32(CustomerId) != 0)
			{
				objsec.CustomerId = Convert.ToInt32(CustomerId);
			}

			var fdate = Request["datepicker"];
			if (!string.IsNullOrEmpty(fdate.ToString()))
			{
				objsec.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
			}
			var tdate = Request["datepicker1"];
			if (!string.IsNullOrEmpty(tdate.ToString()))
			{
				objsec.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
			}

			var _fdate = objsec.FromDate.Value.ToString("dd-MM-yyyy");
			var _tdate = objsec.ToDate.Value.ToString("dd-MM-yyyy");
			if (!string.IsNullOrEmpty(fdate.ToString()))
			{
				ViewBag.FromDate = fdate;
			}
			if (!string.IsNullOrEmpty(tdate.ToString()))
			{
				ViewBag.ToDate = tdate;
			}
			ViewBag.CustomerId = objsec.CustomerId;

			if (submit == "submit")
			{
				if (string.IsNullOrEmpty(Sectorid) || Sectorid == "0")
				{
					Sectorid = null;
					type = null;

				}
				else
				{
					type = "Sector";
				}
				dt = objsec.GetCustomerNotification(objsec.CustomerId.ToString(), objsec.FromDate, objsec.ToDate, Sectorid, type);
				ViewBag.CustomerNotList = dt;
			}

			else
			{
				if (submit == "Search")
				{
					dt = objsec.GetAllCustomerNotification(null);
					ViewBag.CustomerNotList = dt;
				}

				if (submit == "Delete")
				{
					string msg = "";
					string proid = Request["txtproid"];

					string delimStr = ",";
					char[] delimiter = delimStr.ToCharArray();
					int id = 0;

					foreach (string s in proid.Split(delimiter))
					{

						id = Convert.ToInt16(s);

						int delresult = objcust.DeleteCustomerNotification(id);
						if (delresult > 0)
						{
							msg = "Notification Deleted Successfully";
							Session["Msg"] = msg;
						}
					}

					return RedirectToAction("CustomerNotificationList");
				}


			}


			dt = objsec.BindCustomer(null);
			ViewBag.Customer = dt;
			Sector objsector = new Sector();
			DataTable dtList = new DataTable();
			dtList = objsector.getStateList(null);
			ViewBag.StateList = dtList;
			return View();
		}

		[HttpGet]
		public ActionResult CustomerNotification()
		{
			if (Request.Cookies["gstusr"] == null)
				return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
			//if (HttpContext.Session["UserId"] == null)
			//    return Redirect("/home/login?ReturnURL=" + Request.RawUrl);

			var control = Helper.CheckPermission(Request.RawUrl.ToString());
			if (control.IsView == false)
				return Redirect("/notaccess/index");


			ViewBag.IsAdmin = control.IsAdmin;
			ViewBag.IsView = control.IsView;
			ViewBag.IsAdd = control.IsAdd;


			Sector objsector = new Sector();
			DataTable dtList = new DataTable();
			dtList = objsector.getStateList(null);
			ViewBag.StateList = dtList;
			Customer objcust = new Customer();

			//DataTable dt = new DataTable();
			//dt = objsector.getSectorList(null);
			//ViewBag.Sector = dt;

			// dtList = new DataTable();
			//dtList = objcust.getCustomerList(null);
			//ViewBag.CustomerList = dtList;


			Customer objorder = new Customer();
			Subscription objsub = new Subscription();

			DataTable dt = new DataTable();
			dt = objorder.BindCustomer(null);
			ViewBag.Customer = dt;
			return View();

		}

		[HttpPost]
		public ActionResult CustomerNotification(FormCollection form, CustomerOrder objorder, HttpPostedFileBase Document1, HttpPostedFileBase Document2, string[] chkSector)
		{
			Staff objstaff = new Staff();
			Sector objsector = new Sector();
			Subscription obj = new Subscription();
			Customer objcustomer = new Customer();
			Customer objLogin = new Customer();
			string SectorId = "";
			string fname = null, path = null;
			string submit = Request["submit"];
			decimal Walletbal = 0;
			if (submit == "Sectornot")
			{
				HttpPostedFileBase document1 = Request.Files["Document1"];
				string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png", ".PNG" };

				if (document1 != null)
				{
					if (document1.ContentLength > 0)
					{
						try
						{
							HttpFileCollectionBase files = Request.Files;
							HttpPostedFileBase file = Document1;
							//Resize image 500*300 coding
							WebImage img = new WebImage(file.InputStream);
							img.Resize(300, 300, false, false);
							string fileName = Path.GetFileNameWithoutExtension(file.FileName);
							string extension = Path.GetExtension(file.FileName);
							if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
							{
								string[] testfiles = file.FileName.Split(new char[] { '\\' });
								fname = testfiles[testfiles.Length - 1];
							}
							else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.ToLower().LastIndexOf('.'))))
							{
								ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
							}
							else
							{
								fileName = dHelper.RemoveIllegalCharacters(fileName);
								fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
							}
							path = Path.Combine(Server.MapPath("~/image/Notification/"), fname);
							img.Save(path);
							//file.SaveAs(path);
							//objProdt.Image = fname;
							objcustomer.Photo = fname;
							objcustomer.ntext = Request["TextNotification"];
							objcustomer.ntitle = Request["txttitle"];
							objcustomer.nlink = Request["txtlink"].ToString();

						}

						catch (Exception ex)
						{
							ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
						}
					}


					else
					{
						objcustomer.Photo = "";
						objcustomer.ntext = Request["TextNotification"];
						objcustomer.ntitle = Request["txttitle"];
						objcustomer.nlink = Request["txtlink"].ToString();
					}
				}


				string type = "Sector";

				var addresult = objcustomer.InsertNotification(objcustomer, type);

				DataTable dtToken = new DataTable();

				int cusnot = 0;
				if (chkSector != null)
				{
					foreach (var item in chkSector)
					{
						if (!string.IsNullOrEmpty(item))
						{
							SectorId = item;

							dtToken = objLogin.getCustomerList(Convert.ToInt32(SectorId));



							//
							bool add;
							string active = Request["IsLowbalance"];
							if (active == "on")
							{
								add = Convert.ToBoolean(true);
								for (int i = 0; i < dtToken.Rows.Count; i++)
								{
									objcustomer.CustomerId = Convert.ToInt32(dtToken.Rows[i]["Id"]);
									objcustomer.UserName = dtToken.Rows[i].ItemArray[5].ToString();
									objcustomer.SectorId = Convert.ToInt32(SectorId);
									objcustomer.nid = addresult.ToString();
									var balance = obj.GetCustomerBalace(obj.CustomerId);
									Walletbal = balance;
									if (Walletbal < 0)
										cusnot = objcustomer.InsertCustomerNotification(objcustomer);
								}


							}
							else
							{
								for (int i = 0; i < dtToken.Rows.Count; i++)
								{
									objcustomer.UserName = dtToken.Rows[i].ItemArray[5].ToString();
									objcustomer.SectorId = Convert.ToInt32(SectorId);
									objcustomer.nid = addresult.ToString();

									cusnot = objcustomer.InsertCustomerNotification(objcustomer);
								}
							}

						}
					}
				}
				try
				{
					if (cusnot > 0)
					{
						//dtToken = objLogin.getCustomerNotiList(Convert.ToInt32(addresult));



						//if (dtToken.Rows.Count > 0)
						//{
						//    string title = dtToken.Rows[0].ItemArray[1].ToString();
						//    string text = dtToken.Rows[0].ItemArray[2].ToString();
						//    string image = "";
						//    if (!string.IsNullOrEmpty(dtToken.Rows[0]["Image"].ToString()))
						//    {
						//        var encoded = Uri.EscapeUriString(dtToken.Rows[0]["Image"].ToString().Trim());
						//        image = Helper.PhotoFolderPath + "/image/Notification/" + encoded;
						//    }
						//    else
						//        image = "";
						//    string link = dtToken.Rows[0]["NewLink"].ToString();
						//    string[] arrray = dtToken.Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToArray();
						//    string notificationtitle = title, notificationcontent = text;

						//    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
						//    tRequest.Method = "post";
						//    //serverKey - Key from Firebase cloud messaging server  
						//    tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAP1kVwxg:APA91bEGnOhfhaWJqwwBtW0uzn3nF-mSbJxDtjggcMRts5mith8ArpqpnW57HEhO3yKpohZZZs7PPF1LCsYMlioCFXzyFt5nRxeTCgPk-zlrX-YfQps6yCn1Z9bdVAFK7HnCja_S3Nsp"));
						//    //Sender Id - From firebase project setting  
						//    tRequest.Headers.Add(string.Format("Sender: id={0}", "272077538072"));
						//    tRequest.ContentType = "application/json";
						//    string postbody = "";
						//    if (image != "")
						//    {
						//        var payload = new
						//        {
						//            priority = "high",
						//            content_available = true,
						//            registration_ids = arrray,                    //for send multiple user fcm_id
						//                                                          //  registration_ids = "dr-2mwpDRH2oryNy9fRq_U:APA91bHNELlIzgpKNrVcSBmtJWRq-kHaVEQ7KKbhUyrfg_mFBNqRlO0p5V_QWwjU0_GXiK5E6SZrVwA7sNPeSBueIa1IkNOnpvelM3_F_49Ki82b_Q9n6XepNrt_mVfosyEymfoZdniq",
						//            notification = new
						//            {

						//                body = notificationcontent,
						//                title = notificationtitle,
						//                image = image,
						//                badge = 1
						//            },
						//            data = new
						//            {
						//                click_action = "FLUTTER_NOTIFICATION_CLICK",
						//                body = notificationcontent,
						//                title = notificationtitle,
						//                link = link
						//            }
						//        };

						//        postbody = JsonConvert.SerializeObject(payload).ToString();
						//    }

						//    else
						//    {
						//        var payload = new
						//        {
						//            priority = "high",
						//            content_available = true,
						//            registration_ids = arrray,                    //for send multiple user fcm_id
						//                                                          //  registration_ids = "dr-2mwpDRH2oryNy9fRq_U:APA91bHNELlIzgpKNrVcSBmtJWRq-kHaVEQ7KKbhUyrfg_mFBNqRlO0p5V_QWwjU0_GXiK5E6SZrVwA7sNPeSBueIa1IkNOnpvelM3_F_49Ki82b_Q9n6XepNrt_mVfosyEymfoZdniq",
						//            notification = new
						//            {

						//                body = notificationcontent,
						//                title = notificationtitle,

						//                badge = 1
						//            },
						//            data = new
						//            {
						//                click_action = "FLUTTER_NOTIFICATION_CLICK",
						//                body = notificationcontent,
						//                title = notificationtitle,
						//                link = link
						//            }
						//        };

						//        postbody = JsonConvert.SerializeObject(payload).ToString();
						//    }


						//    Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
						//    tRequest.ContentLength = byteArray.Length;
						//    using (Stream dataStream = tRequest.GetRequestStream())
						//    {
						//        dataStream.Write(byteArray, 0, byteArray.Length);
						//        using (WebResponse tResponse = tRequest.GetResponse())
						//        {
						//            using (Stream dataStreamResponse = tResponse.GetResponseStream())
						//            {
						//                if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
						//                    {
						//                        String sResponseFromServer = tReader.ReadToEnd();
						//                        //result.Response = sResponseFromServer;
						//                    }
						//            }
						//        }
						//    }
						//    //jsonString1 = JsonConvert.SerializeObject(1);

						//    ViewBag.Successmsg = "Notification Sent";
						//}
						//else
						//    //jsonString1 = JsonConvert.SerializeObject(3);
						//    ViewBag.Successmsg = "Notification Not Sent";

						ViewBag.Successmsg = "Notification Sent";
					}
					else
						ViewBag.Successmsg = "Notification Not Sent";
				}

				catch (Exception ex)
				{
					ViewBag.Successmsg = "Notification Not Sent";
				}

			}

			if (submit == "Cusnot")
			{
				DataTable dtToken = new DataTable();
				string customerId = Request["ddlCustomer"];
				dtToken = objLogin.getCustomerSector(Convert.ToInt32(customerId));
				if (dtToken.Rows.Count > 0)
				{
					objcustomer.UserName = dtToken.Rows[0].ItemArray[1].ToString();
					objcustomer.SectorId = Convert.ToInt32(dtToken.Rows[0].ItemArray[0]);
				}

				HttpPostedFileBase document2 = Request.Files["Document2"];
				string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".png", ".PNG" };

				if (document2 != null)
				{
					if (document2.ContentLength > 0)
					{
						try
						{
							HttpFileCollectionBase files = Request.Files;
							HttpPostedFileBase file = Document2;
							//Resize image 500*300 coding
							WebImage img = new WebImage(file.InputStream);
							img.Resize(300, 300, false, false);
							string fileName = Path.GetFileNameWithoutExtension(file.FileName);
							string extension = Path.GetExtension(file.FileName);
							if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
							{
								string[] testfiles = file.FileName.Split(new char[] { '\\' });
								fname = testfiles[testfiles.Length - 1];
							}
							else if (!sAllowedExt.Contains(file.FileName.Substring(file.FileName.ToLower().LastIndexOf('.'))))
							{
								ViewBag.Message = string.Format("Please upload Your Notification File of type" + string.Join(",", sAllowedExt));
							}
							else
							{
								fileName = dHelper.RemoveIllegalCharacters(fileName);
								fname = fileName + "_" + DateTime.Now.ToString("yyyyMMdd-hhmmssfff") + extension;
							}
							path = Path.Combine(Server.MapPath("~/image/Notification/"), fname);
							img.Save(path);
							//file.SaveAs(path);
							//objProdt.Image = fname;
							objcustomer.Photo = fname;
							objcustomer.ntext = Request["TextNotification1"];
							objcustomer.ntitle = Request["txttitle1"];
							objcustomer.nlink = Request["txtlink1"].ToString();

						}

						catch (Exception ex)
						{
							ViewBag.SuccessMsg = "Error occurred. Error details: " + ex.Message;
						}
					}

					else
					{
						objcustomer.Photo = "";
						objcustomer.ntext = Request["TextNotification1"];
						objcustomer.ntitle = Request["txttitle1"];
						objcustomer.nlink = Request["txtlink1"].ToString();
					}
				}


				string type = "Customer";

				var addresult = objcustomer.InsertNotification(objcustomer, type);


				//DataTable dtToken = new DataTable();

				int cusnot = 0;

				objcustomer.nid = addresult.ToString();

				cusnot = objcustomer.InsertCustomerNotification(objcustomer);

				try
				{
					if (cusnot > 0)
					{


						ViewBag.Successmsg1 = "Notification Sent";
					}
					else
						ViewBag.Successmsg1 = "Notification Not Sent";
				}

				catch (Exception ex)
				{
					ViewBag.Successmsg1 = "Notification Not Sent";
				}

			}


			if (submit == "Delnot")
			{
				int delresult = objcustomer.DeleteNotification();

				if (delresult > 0)
				{


					ViewBag.Successmsg1 = "Notification Deleted";
				}
				else
					ViewBag.Successmsg1 = "Notification Not Deleted";
			}
			DataTable dtList = new DataTable();
			dtList = objsector.getStateList(null);
			ViewBag.StateList = dtList;
			Customer objcust = new Customer();

			DataTable dt = new DataTable();
			dt = objsector.getSectorList(null);
			ViewBag.Sector = dt;


			dt = objcustomer.BindCustomer(null);
			ViewBag.Customer = dt;
			return View();

		}

		[HttpGet]
		public ActionResult DeleteCustomerNotification(int id)
		{
			try
			{
				int delresult = objcust.DeleteCustomerNotification(id);
				return RedirectToAction("CustomerNotificationList");
			}
			//catch (System.Data.SqlClient.SqlException ex)
			//{
			//    if (ex.Message.ToLower().Contains("fk_customer_staffcustassign") || ex.Message.ToLower().Contains("fk_customer_custsubscription") || ex.Message.ToLower().Contains("fk_order_customer") || ex.Message.ToLower().Contains("fk_wallet_customer"))
			//    {
			//        TempData["error"] = String.Format("You can not deleted. Child record found.");
			//    }
			//    else
			//        throw ex;
			//}
			catch (Exception ex)
			{
				return View();
			}
			return RedirectToAction("StaffCustomerAssignList");
		}
		[HttpPost]
		public ActionResult SendCustomerNotification(FormCollection form, string sectorId, string text)
		{
			Customer objLogin = new Customer();
			DataTable dtToken = new DataTable();
			string SectorId = Request["hdnSectorId"];
			//sectorId = "2";

			string jsonString1 = string.Empty;
			try
			{
				if (!string.IsNullOrEmpty(sectorId))
				{
					string delimStr = ",";
					char[] delimiter = delimStr.ToCharArray();
					string a = "";
					foreach (string s in sectorId.Split(delimiter))
					{
						dtToken = objLogin.getCustomerList(Convert.ToInt32(s));

						if (dtToken.Rows.Count > 0)
						{
							string[] arrray = dtToken.Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToArray();
							string notificationtitle = "Coming Soon", notificationcontent = text;

							WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
							tRequest.Method = "post";
							//serverKey - Key from Firebase cloud messaging server  
							tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAP1kVwxg:APA91bEGnOhfhaWJqwwBtW0uzn3nF-mSbJxDtjggcMRts5mith8ArpqpnW57HEhO3yKpohZZZs7PPF1LCsYMlioCFXzyFt5nRxeTCgPk-zlrX-YfQps6yCn1Z9bdVAFK7HnCja_S3Nsp"));
							//Sender Id - From firebase project setting  
							tRequest.Headers.Add(string.Format("Sender: id={0}", "272077538072"));
							tRequest.ContentType = "application/json";


							var payload = new
							{
								priority = "high",
								content_available = true,
								registration_ids = arrray,                    //for send multiple user fcm_id
																			  //  registration_ids = "dr-2mwpDRH2oryNy9fRq_U:APA91bHNELlIzgpKNrVcSBmtJWRq-kHaVEQ7KKbhUyrfg_mFBNqRlO0p5V_QWwjU0_GXiK5E6SZrVwA7sNPeSBueIa1IkNOnpvelM3_F_49Ki82b_Q9n6XepNrt_mVfosyEymfoZdniq",
								notification = new
								{

									body = notificationcontent,
									title = notificationtitle,
									image = "https://5.imimg.com/data5/IW/DC/RP/SELLER-107655683/coconut-water-wholesaler-in-ahmedabad-500x500.jpg",
									badge = 1
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
							jsonString1 = JsonConvert.SerializeObject(1);
						}
						else
							jsonString1 = JsonConvert.SerializeObject(3);
					}


				}


			}
			catch (Exception ex)
			{
				jsonString1 = JsonConvert.SerializeObject(2);
			}
			// return View();

			//if (tablecount == notaddcount)
			//{
			//    jsonString1 = JsonConvert.SerializeObject(2);
			//}
			//else
			//{
			//    if (AddSectorProduct > 0)

			//    else
			//        jsonString1 = JsonConvert.SerializeObject(3);
			//}
			return Json(jsonString1, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public ActionResult SendNotification()
		{
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{
				Customer objorder = new Customer();
				Subscription objsub = new Subscription();

				DataTable dt = new DataTable();
				dt = objorder.BindCustomer(null);
				ViewBag.Customer = dt;

				DataTable dtList = new DataTable();
				dtList = objcust.getCustomerList(null);
				ViewBag.CustomerList = dtList;

				return View();
			}
			else
			{
				return RedirectToAction("Login", "Home");
			}
		}

		[HttpPost]
		public ActionResult SendNotification(FormCollection form, CustomerOrder objorder)
		{
			Staff objstaff = new Staff();
			Sector objsector = new Sector();
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{
				string SectorId = Request["ddlSector"];
				if (!string.IsNullOrEmpty(SectorId) && Convert.ToInt32(SectorId) != 0)
				{
					objorder.SectorId = Convert.ToInt32(SectorId);
				}

				string text = Request["TextNotification"];
				ViewBag.SectorId = objorder.SectorId;
				ViewBag.NotificationText = text;

				DataTable dt = new DataTable();
				dt = objcust.BindCustomer(null);
				ViewBag.Customer = dt;

				DataTable dtList = new DataTable();
				dtList = objcust.getCustomerList(null);
				ViewBag.CustomerList = dtList;

				return View();
			}
			else
			{
				// return PartialView("DeliveryBoyCustomerPartial");
				return RedirectToAction("Login", "Home");
			}
		}

		[HttpPost]
		public ActionResult SendSingleCustomerNotification(FormCollection form, string customerId, string text)
		{
			Customer objLogin = new Customer();
			DataTable dtToken = new DataTable();
			string SectorId = Request["hdnCustomerId"];
			if (!string.IsNullOrEmpty(customerId) && Convert.ToInt32(customerId) != 0)
			{
				dtToken = objLogin.BindCustomer(Convert.ToInt32(customerId));
			}
			string jsonString1 = string.Empty;
			try
			{
				if (dtToken.Rows.Count > 0)
				{
					string notificationtitle = "MilkywayIndia", notificationcontent = text;
					var response = dHelper.AppNotification(Convert.ToInt32(customerId), notificationtitle, notificationcontent, "", "", "");
					if (response == 1)
					{
						jsonString1 = JsonConvert.SerializeObject(1);
					}
				}
				else
					jsonString1 = JsonConvert.SerializeObject(3);
			}
			catch (Exception ex)
			{
				jsonString1 = JsonConvert.SerializeObject(2);
			}

			return Json(jsonString1, JsonRequestBehavior.AllowGet);
		}


		public ActionResult CustomerWalletTransactionPrint(string cId, string fd, string td)
		{
			try
			{

				CustomerOrder objorder = new CustomerOrder();
				if (cId == "0") cId = null;

				var fdate = fd;
				if (!string.IsNullOrEmpty(fdate.ToString()))
				{
					objorder.FromDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
				}
				var tdate = td;
				if (!string.IsNullOrEmpty(tdate.ToString()))
				{
					objorder.ToDate = Convert.ToDateTime(DateTime.ParseExact(tdate, @"dd/MM/yyyy", null));
				}
				DataTable dt = new DataTable();
				Report.StaffOrder obj = new Report.StaffOrder();
				ReportDocument rd = new ReportDocument();
				rd.Load(Server.MapPath("~/Report/CustomerWalletTransaction.rpt"));
				SqlDataAdapter da = new SqlDataAdapter("rpt_Customer_Wallet_DateWise_SelectAll", con);
				da.SelectCommand.CommandType = CommandType.StoredProcedure;
				if (!string.IsNullOrEmpty(cId))
					da.SelectCommand.Parameters.AddWithValue("@CustomerId", cId);
				else
					da.SelectCommand.Parameters.AddWithValue("@CustomerId", DBNull.Value);
				if (!string.IsNullOrEmpty(fd))
					da.SelectCommand.Parameters.AddWithValue("@FromDate", objorder.FromDate);
				else
					da.SelectCommand.Parameters.AddWithValue("@FromDate", DBNull.Value);
				if (!string.IsNullOrEmpty(td))
					da.SelectCommand.Parameters.AddWithValue("@ToDate", objorder.ToDate);
				else
					da.SelectCommand.Parameters.AddWithValue("@ToDate", DBNull.Value);


				da.Fill(dt);

				rd.Database.Tables[0].SetDataSource(dt);
				rd.SetParameterValue("@CustomerId", cId);
				rd.SetParameterValue("@FromDate", objorder.FromDate);
				rd.SetParameterValue("@ToDate", objorder.ToDate);


				Response.Buffer = false;
				Response.ClearContent();
				Response.ClearHeaders();
				//CrystalReportViewer1.RefreshReport();
				try
				{
					Stream str = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
					str.Seek(0, SeekOrigin.Begin);
					return File(str, "application/pdf", "CustomerWalletTransaction.pdf");
				}
				catch (Exception e)
				{
					throw;
				}
			}
			catch (Exception e)
			{
				throw;
			}

		}

		[HttpGet]
		public ActionResult TestSms()
		{
			return View();
		}

		[HttpPost]
		public ActionResult TestSms(Customer objcust, FormCollection form)
		{
			string mobile = objcust.testsms;

			string Msg = "Welcome to Milkyway India Family!!Your Milkyway India OTP is 1234 You can now order Milk, Dairy, Grocery at your doorstep Or Bill Payment with Cash back.";

			string strUrl = "";
			//india sms
			////strUrl = "https://apps.vibgyortel.in/client/api/sendmessage?apikey=dca6c57e6c6f4638&mobiles=" + Mobile + "&sms=" + Msg + "&senderid=Aruhat";
			strUrl = "http://trans.magicsms.co.in/api/v4/?api_key=" + Helper.MagicSMSKey + "&method=sms&message=" + Msg + "&to=" + mobile + "&sender=" + Helper.MagicSender + "&template_id=" + Helper.MagicOTPTemplateID;

			// Create a request object  
			// WebRequest request = HttpWebRequest.Create(strUrl);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
			request.Credentials = CredentialCache.DefaultCredentials;
			// Get the response back  

			HttpWebResponse responsesms = (HttpWebResponse)request.GetResponse();
			Stream s = responsesms.GetResponseStream();
			StreamReader readStream = new StreamReader(s, Encoding.UTF8);
			string dataString = readStream.ReadToEnd();
			responsesms.Close();
			s.Close();
			readStream.Close();


			return View();
		}



		[HttpGet]
		public ActionResult CashRequest(int id)
		{
			if (Request.Cookies["gstusr"] == null)
				return Redirect("/home/login?ReturnURL=" + Request.RawUrl);
			var control = Helper.CheckPermission(Request.RawUrl.ToString());
			if (control.IsView == false)
				return Redirect("/notaccess/index");

			ViewBag.IsAdmin = control.IsAdmin;
			ViewBag.IsView = control.IsView;

			Sector objsec = new Sector();





			DataTable dt = new DataTable();
			dt = objcust.BindCustomer(id);
			if (dt.Rows.Count > 0)
			{
				if (!string.IsNullOrEmpty(dt.Rows[0]["FirstName"].ToString()))
					ViewBag.FirstName = dt.Rows[0]["FirstName"].ToString();
				else
					ViewBag.FirstName = "";
				if (!string.IsNullOrEmpty(dt.Rows[0]["LastName"].ToString()))
					ViewBag.LastName = dt.Rows[0]["LastName"].ToString();
				else
					ViewBag.LastName = "";
				if (!string.IsNullOrEmpty(dt.Rows[0]["MobileNo"].ToString()))
					ViewBag.MobileNo = dt.Rows[0]["MobileNo"].ToString();
				else
					ViewBag.MobileNo = "";

			}


			dt = objcust.BindCustomerCash(id);
			ViewBag.CustomerCashList = dt;
			return View();

		}

		[HttpPost]
		public ActionResult CashRequest(Customer objcust, FormCollection form)
		{
			DataTable dt = new DataTable();
			var fdate = Request["datepicker"];
			objcust.CashReqDate = Convert.ToDateTime(DateTime.ParseExact(fdate, @"dd/MM/yyyy", null));
			int customerid = objcust.Id;

			dt = objcust.cashdupl(customerid, objcust.CashReqDate);

			if (dt.Rows.Count > 0)
			{
				ViewBag.SuccessMsg = "Duplicate Entry!!!";
			}
			else
			{
				int addresult = objcust.InsertCashRequest(objcust);
				if (addresult > 0)
				{
					ViewBag.SuccessMsg = "Cash Amount Updated Successfully!!!";
				}
				else
				{ ViewBag.SuccessMsg = "Cash Amount Not Updated!!!"; }
			}


			dt = objcust.BindCustomer(objcust.Id);
			if (dt.Rows.Count > 0)
			{
				if (!string.IsNullOrEmpty(dt.Rows[0]["FirstName"].ToString()))
					ViewBag.FirstName = dt.Rows[0]["FirstName"].ToString();
				else
					ViewBag.FirstName = "";
				if (!string.IsNullOrEmpty(dt.Rows[0]["LastName"].ToString()))
					ViewBag.LastName = dt.Rows[0]["LastName"].ToString();
				else
					ViewBag.LastName = "";
				if (!string.IsNullOrEmpty(dt.Rows[0]["MobileNo"].ToString()))
					ViewBag.MobileNo = dt.Rows[0]["MobileNo"].ToString();
				else
					ViewBag.MobileNo = "";

			}


			dt = objcust.BindCustomerCash(objcust.Id);
			ViewBag.CustomerCashList = dt;
			return View();
		}


		public ActionResult DeleteCash(string Id, string cid)
		{
			try
			{
				//CustomerOrder objorder = new CustomerOrder();
				//if (StaffId == "0") StaffId = null;
				//if (Status == "0") Status = null;
				//string Fdate = System.DateTime.Now.Date.AddDays(1).ToString("dd-MM-yyyy");
				//string Tdate = System.DateTime.Now.Date.AddDays(1).ToString("dd-MM-yyyy");
				//string query = string.Format("DeliveryboyId={0}&CustomerId={1}&FDate={2}&TDate={3}&status={4}",
				//       StaffId, null, Fdate, Tdate, Status);
				//return Redirect("/customerorder/DeliveryBoyDailyReport?" + query);
				//return Redirect("/Report/DeliveryBoyDailyReport?" + query);
				Customer obj = new Customer();
				int delresult = obj.DeleteCash(Id, cid);



				return Redirect("/Customer/CashRequest/" + cid);
			}
			catch (Exception e)
			{
				throw;
			}

		}



		[HttpGet]
		public ActionResult CustomerAttributes(int id)
		{
			if (Session["Username"] != null && !string.IsNullOrEmpty(Session["Username"] as string))
			{
				Sector objsec = new Sector();
				DataTable dtbuild = new DataTable();
				dtbuild = objsec.getBuildingList(null);
				ViewBag.Building = dtbuild;

				DataTable dtsec = new DataTable();
				dtsec = objsec.getSectorList(null);
				ViewBag.Sector = dtsec;

				DataTable dtflatno = new DataTable();
				dtflatno = objsec.getFlatNoList(null);
				ViewBag.FlatNo = dtflatno;

				DataTable dt = new DataTable();
				dt = objcust.BindCustomer(id);
				if (dt.Rows.Count > 0)
				{
					if (!string.IsNullOrEmpty(dt.Rows[0]["FirstName"].ToString()))
						ViewBag.FirstName = dt.Rows[0]["FirstName"].ToString();
					else
						ViewBag.FirstName = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["LastName"].ToString()))
						ViewBag.LastName = dt.Rows[0]["LastName"].ToString();
					else
						ViewBag.LastName = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["MobileNo"].ToString()))
						ViewBag.MobileNo = dt.Rows[0]["MobileNo"].ToString();
					else
						ViewBag.MobileNo = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["Email"].ToString()))
						ViewBag.Email = dt.Rows[0]["Email"].ToString();
					else
						ViewBag.Email = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["Address"].ToString()))
						ViewBag.Address = dt.Rows[0]["Address"].ToString();
					else
						ViewBag.Address = "";

					if (!string.IsNullOrEmpty(dt.Rows[0]["Photo"].ToString()))
						ViewBag.Photo = dt.Rows[0]["Photo"].ToString();
					else
						ViewBag.Photo = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["UserName"].ToString()))
						ViewBag.UserName = dt.Rows[0]["UserName"].ToString();
					else
						ViewBag.UserName = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["Password"].ToString()))
						ViewBag.Password = dt.Rows[0]["Password"].ToString();
					else
						ViewBag.Password = "";
					if (!string.IsNullOrEmpty(dt.Rows[0]["BuildingId"].ToString()))
					{
						ViewBag.BuildingId = dt.Rows[0]["BuildingId"].ToString();
					}
					else
						ViewBag.BuildingId = "0";


					//get sectorId
					DataTable dtSecId = objsec.getBuildingList(Convert.ToInt32(ViewBag.BuildingId));
					if (dtSecId.Rows.Count > 0)
					{
						if (!string.IsNullOrEmpty(dtSecId.Rows[0]["SectorId"].ToString()))
							ViewBag.SectorId = dtSecId.Rows[0]["SectorId"].ToString();
						else
							ViewBag.SectorId = "0";
						dtbuild = objsec.geSectorwisetBuildingList(Convert.ToInt32(dtSecId.Rows[0]["SectorId"]));
						ViewBag.Building = dtbuild;


					}
					if (!string.IsNullOrEmpty(dt.Rows[0]["BuildingId"].ToString()))
					{
						//  ViewBag.BuildingId = dt.Rows[0]["BuildingId"].ToString();
						dtflatno = objsec.getBuildingwiseFlatNoList(Convert.ToInt32(dt.Rows[0]["BuildingId"]));
						ViewBag.FlatNo = dtflatno;
					}
					//else
					//    ViewBag.BuildingId = "0";
					if (!string.IsNullOrEmpty(dt.Rows[0]["FlatId"].ToString()))
						ViewBag.FlatNoId = dt.Rows[0]["FlatId"].ToString();
					else
						ViewBag.FlatNoId = "0";
				}

				return View();
			}
			else
			{
				return RedirectToAction("Login", "Home");
			}
		}
	}
}
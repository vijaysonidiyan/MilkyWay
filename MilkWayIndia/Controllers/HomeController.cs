using MilkWayIndia.Abstract;
using MilkWayIndia.Concrete;
using MilkWayIndia.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MilkWayIndia.Entity;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Configuration;

namespace MilkWayIndia.Controllers
{
	public class HomeController : Controller
	{
		Customer objCustomer = new Customer();
		private ISecPaytm _SecPaytmRepo;
		Helper dHelper = new Helper();

		public HomeController()
		{
			this._SecPaytmRepo = new SecPaytmRepository();
		}

		[HttpGet]
		public ActionResult Login(string ReturnURL)
		{
			if (!string.IsNullOrEmpty(ReturnURL))
				ViewBag.ReturnURL = ReturnURL;
			ClearAllCookies();
			return View();
		}

		[HttpPost]
		public ActionResult Login(Staff staff)
		{
			var ReturnURL = Request.Form["ReturnURL"];
			if (staff.UserName != null && staff.Password != null)
			{
				DataTable dt = staff.Adminlogin(staff.UserName, staff.Password);
				if (dt.Rows.Count > 0)
				{
					HttpCookie cookie = new HttpCookie("gstusr");
					cookie.Values.Add("key", dt.Rows[0]["Id"].ToString());
					cookie.Expires = DateTime.Now.AddHours(3);
					Response.Cookies.Add(cookie);

					Session["Msg"] = "";
					Session["UserId"] = dt.Rows[0]["Id"].ToString();
					Session["Username"] = staff.UserName;
					Session["RoleName"] = dt.Rows[0]["Role"].ToString();
					Session["isVendorLogin"] = false;
					Session["VendorSectorId"] = "0";
					Session["ProfilePic"] = dt.Rows[0]["Photo"].ToString();
					return RedirectToAction("Index", "Admin");
				}
				else
				{
					DataTable dtVendor = staff.VendorLogin(staff.UserName, staff.Password);
					if (dtVendor.Rows.Count > 0)
					{
						HttpCookie cookie = new HttpCookie("gstusr");
						cookie.Values.Add("key", dtVendor.Rows[0]["Id"].ToString());
						cookie.Expires = DateTime.Now.AddHours(3);
						Response.Cookies.Add(cookie);

						Session["Msg"] = "";
						Session["UserId"] = dtVendor.Rows[0]["Id"].ToString();
						Session["Username"] = dtVendor.Rows[0]["FirstName"] + " " + dtVendor.Rows[0]["LastName"];
						Session["RoleName"] = "Vendor";
						Session["isVendorLogin"] = true;
						Session["VendorSectorId"] = dtVendor.Rows[0]["SectorId"].ToString();
						Session["ProfilePic"] = dtVendor.Rows[0]["Photo"].ToString();
						return RedirectToAction("Index", "Admin");
					}
					else
					{
						ViewBag.SuccessMsg = "Incorrect Username or Password.";
						ModelState.Clear();
					}
				}
			}
			else
			{
				ViewBag.SuccessMsg = "Enter Username or Password.";
			}
			return View();
		}

		[HttpPost]
		public JsonResult ForgotPassword(string usernameOrEmail)
		{
			string email = "";
			string name = "";
			bool userFound = false;
			Staff staffModel = new Staff();

			// Check only in Vendor table
			DataTable dtVendor = staffModel.GetVendorByUsernameOrEmail(usernameOrEmail);
			if (dtVendor.Rows.Count > 0)
			{
				email = dtVendor.Rows[0]["Email"].ToString();
				name = dtVendor.Rows[0]["FirstName"] + " " + dtVendor.Rows[0]["LastName"];
				userFound = true;
			}

			if (userFound && !string.IsNullOrEmpty(email))
			{
				try
				{
					string subject = "Forgot Password Request";
					string encodedUser = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(usernameOrEmail));
					string resetLink = Url.Action("ResetPassword", "Home", new { user = encodedUser }, protocol: Request.Url.Scheme);
					string body = $"Hello {name},<br/><br/>Click the link below to reset your password:<br/><br/><a href='{resetLink}'>Reset Password</a><br/><br/>If you did not request this, please ignore this email.<br/><br/>Regards,<br/>MilkyWayIndia Team";

					SendEmail(email, subject, body);
					return Json(new { success = true });
				}
				catch
				{
					return Json(new { success = false, message = "Failed to send email. Try again later." });
				}
			}

			return Json(new { success = false, message = "No matching user found." });
		}


		public void SendEmail(string toEmail, string subject, string body)
		{
			string smtpServer = ConfigurationManager.AppSettings["SmtpServer"];
			int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
			string smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
			string smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
			bool enableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);

			using (MailMessage mail = new MailMessage())
			{
				mail.From = new MailAddress(smtpUsername, "MilkyWayIndia");
				mail.To.Add(toEmail);
				mail.Subject = subject;
				mail.Body = body;
				mail.IsBodyHtml = true;

				using (SmtpClient smtp = new SmtpClient(smtpServer, smtpPort))
				{
					smtp.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
					smtp.EnableSsl = enableSsl;
					smtp.Send(mail);
				}
			}
		}


		[HttpGet]
		public ActionResult ResetPassword(string user)
		{
			if (string.IsNullOrEmpty(user))
				return RedirectToAction("Login");

			string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(user));
			var model = new Staff { UsernameOrEmail = decoded };
			return View(model);
		}



		[HttpPost]
		public ActionResult ResetPassword(Staff model)
		{
			if (ModelState.IsValid)
			{
				if (model.NewPassword != model.ConfirmPassword)
				{
					ModelState.AddModelError("", "Passwords do not match.");
					return View(model);
				}

				bool updated = model.UpdateVendorPassword(model.UsernameOrEmail, model.NewPassword);

				if (updated)
				{
					TempData["SuccessMsg"] = "Password reset successful.";
					return RedirectToAction("Login");
				}
				else
				{
					ModelState.AddModelError("", "Vendor not found.");
				}
			}

			return View(model);
		}




		public void ClearAllCookies()
		{
			string[] myCookies = Request.Cookies.AllKeys;
			foreach (string cookie in myCookies)
			{
				Response.Cookies.Remove(cookie);
				Response.Cookies[cookie].Expires = DateTime.MinValue;
				Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
			}
		}

		public ActionResult LogOut()
		{
			ClearAllCookies();
			Session.Clear();
			Session.Abandon();
			Response.Cookies.Clear();
			Response.Cookies.Remove("gstusr");
			Response.Cookies["gstusr"].Expires = DateTime.Now.AddDays(-30);
			return RedirectToAction("Login", "Home");
		}

		public PartialViewResult GetLeftMenu()
		{
			var control = Helper.CheckPermission("");
			ViewBag.IsAdmin = control.IsAdmin;
			return PartialView("_LeftMenu");
		}

		public ActionResult Callback()
		{
			try
			{
				var status = Request.Form["STATUS"];
				var responseCode = Request.Form["RESPCODE"];
				var orderID = Request.Form["ORDERID"];
				tbl_Paytm_Request model = new tbl_Paytm_Request { OrderNo = orderID ?? "0" };
				if (responseCode == "01")
				{
					model.Authenticated = true;
					model.TransactionID = Request.Form["TXNID"];
					ViewBag.Status = "Confirm";
				}
				model.UpdatedDate = Helper.indianTime;
				var paytm = _SecPaytmRepo.UpdatePaytmResponseByOrderID(model);
				if (paytm != null)
				{
					var customer = objCustomer.BindCustomer(paytm.CustomerID);
					if (customer.Rows.Count > 0)
					{
						ViewBag.CustomerName = customer.Rows[0]["FirstName"] + " " + customer.Rows[0]["LastName"];
						ViewBag.MobileNo = customer.Rows[0]["MobileNo"];
					}
				}
				ViewBag.OrderNo = orderID;
				if (responseCode == "01")
					return Redirect("/home/success");
			}
			catch { }
			return Redirect("/home/failed");
		}

		public ActionResult Success() => View();

		public ActionResult Failed() => View();
	}
}
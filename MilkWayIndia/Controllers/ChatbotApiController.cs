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
    public class ChatbotApiController : ApiController
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");


        [Route("api/ChatbotApi/getChatbootmsg/")]

        [HttpGet]
        public HttpResponseMessage getChatbootmsg()
        {

            Chatbot obj = new Chatbot();

            Customer objcust = new Customer();
            string jsonString1 = string.Empty;


            

            DeliveryBoy order = new DeliveryBoy();

            DataTable dtNew = new DataTable();
            DataTable dtNew1 = new DataTable();
            DataTable dtList = new DataTable();
            DataTable dtList1 = new DataTable();

            dtList1 = obj.getChatbotquery(1008);
            int userRecords1 = dtList1.Rows.Count;
            if (dtList1.Rows.Count > 0)
            {
                dtNew1.Columns.Add("Id", typeof(string));
                dtNew1.Columns.Add("ChatbotQue", typeof(string));
                dtNew1.Columns.Add("generalreply", typeof(string));
                dtNew1.Columns.Add("displayyesno", typeof(string));
                dtNew1.Columns.Add("Category", typeof(string));
                dtNew1.Columns.Add("Chatbotyesreply", typeof(string));
                dtNew1.Columns.Add("Chatbotnoreply", typeof(string));
                dtNew1.Columns.Add("SortNo", typeof(string));


                dtNew1.Columns.Add("ChatbotQue2Yes", typeof(string));
                dtNew1.Columns.Add("Chatbot2GeneralReply", typeof(string));
                dtNew1.Columns.Add("Chatbot2DisplayYesNo", typeof(string));
                dtNew1.Columns.Add("ChatbotCategory", typeof(string));
                dtNew1.Columns.Add("Chatbot2YesReply", typeof(string));
                dtNew1.Columns.Add("Chatbot2NoReply", typeof(string));


                dtNew1.Columns.Add("ChatbotQue3No", typeof(string));
                dtNew1.Columns.Add("Chatbot3GeneralReply", typeof(string));
                dtNew1.Columns.Add("Chatbot3DisplayYesNo", typeof(string));
                dtNew1.Columns.Add("Chatbot3Category", typeof(string));
                dtNew1.Columns.Add("Chatbot3YesReply", typeof(string));
                dtNew1.Columns.Add("Chatbot3NoReply", typeof(string));

                for (int i = 0; i < dtList1.Rows.Count; i++)
                {
                    try
                    {
                        DataRow dr1 = dtNew1.NewRow();
                        dr1["Id"] = dtList1.Rows[i]["Id"].ToString().Trim();
                        dr1["ChatbotQue"] = dtList1.Rows[i]["ChatbotQue"].ToString().Trim();
                        dr1["generalreply"] = dtList1.Rows[i]["generalreply"].ToString();
                        dr1["displayyesno"] = dtList1.Rows[i]["displayyesno"].ToString();
                        dr1["Category"] = dtList1.Rows[i]["Category"].ToString();
                        dr1["Chatbotyesreply"] = dtList1.Rows[i]["Chatbotyesreply"].ToString();
                        dr1["Chatbotnoreply"] = dtList1.Rows[i]["Chatbotnoreply"].ToString();
                        dr1["SortNo"] = dtList1.Rows[i]["SortOrder"].ToString();


                        dr1["ChatbotQue2Yes"] = dtList1.Rows[i]["ChatbotQue2Yes"].ToString().Trim();
                        dr1["Chatbot2GeneralReply"] = dtList1.Rows[i]["Chatbot2GeneralReply"].ToString();
                        dr1["Chatbot2DisplayYesNo"] = dtList1.Rows[i]["Chatbot2DisplayYesNo"].ToString();
                        dr1["ChatbotCategory"] = dtList1.Rows[i]["ChatbotCategory"].ToString();
                        dr1["Chatbot2YesReply"] = dtList1.Rows[i]["Chatbot2YesReply"].ToString();
                        dr1["Chatbot2NoReply"] = dtList1.Rows[i]["Chatbot2NoReply"].ToString();

                        dr1["ChatbotQue3No"] = dtList1.Rows[i]["ChatbotQue3No"].ToString().Trim();
                        dr1["Chatbot3GeneralReply"] = dtList1.Rows[i]["Chatbot3GeneralReply"].ToString();
                        dr1["Chatbot3DisplayYesNo"] = dtList1.Rows[i]["Chatbot3DisplayYesNo"].ToString();
                        dr1["Chatbot3Category"] = dtList1.Rows[i]["Chatbot3Category"].ToString();
                        dr1["Chatbot3YesReply"] = dtList1.Rows[i]["Chatbot3YesReply"].ToString();
                        dr1["Chatbot3NoReply"] = dtList1.Rows[i]["Chatbot3NoReply"].ToString();

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

                dict["Chatboot"] = jsonString1;

                string one = @"{""status"":""success""";

                string three = @",""Chatboot"":" + dict["Chatboot"];
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

                dict["Chatboot"] = jsonString1;

                string one = @"{""status"":""Fail""";

                string three = @",""Chatboot"":" + dict["Chatboot"];
                string four = one + three + "}";

                var str = four.ToString().Replace(@"\", "");
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(str, Encoding.UTF8, "application/json");
                return response;
            }

        }

    }
}

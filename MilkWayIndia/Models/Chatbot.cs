using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
namespace MilkWayIndia.Models
{
    public class Chatbot
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
        public int Id { get; set; }
        public string Chatbotquery { get; set; }
        public string ChatbotYesReply { get; set; }
        public string ChatbotNoReply { get; set; }
        public string ChatbotGeneralReply { get; set; }
        public string ChatbotCategory { get; set; }
        public string Displayyesno { get; set; }
        public string MobileNo { get; set; }

        public int SortOrder { get; set; }



        public string ChatbotQue2Yes { get; set; }
        public string Chatbot2YesReply { get; set; }
        public string Chatbot2NoReply { get; set; }
        public string Chatbot2Category { get; set; }
        public string Chatbot2GeneralReply { get; set; }
        public string Chatbot2DisplayYesNo { get; set; }



        public string ChatbotQue3No { get; set; }
        public string Chatbot3YesReply { get; set; }
        public string Chatbot3NoReply { get; set; }
        public string Chatbot3Category { get; set; }
        public string Chatbot3GeneralReply { get; set; }
        public string Chatbot3DisplayYesNo { get; set; }

        public string Query { get; set; }
        public string Youtubelink { get; set; }
        public string Image { get; set; }
        public string steps { get; set; }
        public bool IsActive { get; set; }
        public string BackOfficeId { get; set; }
        public string videolink { get; set; }
        public DataTable getChatbotquery(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_Master_Chatbot_settings WHERE (@Id IS NULL OR Id = @Id) Order By SortOrder", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public int Insertchatbot(Chatbot obj)
        {
            int i = 0;
            //try
            //{
                if(obj.SortOrder.ToString()=="" || string.IsNullOrEmpty(obj.SortOrder.ToString()))
                {
                    obj.SortOrder =0;
                }
            //

            //
            con.Open();


           

            SqlCommand com = new SqlCommand("Insert Into tbl_Master_Chatbot_settings(ChatbotQue,Chatbotyesreply,Chatbotnoreply,SortOrder,Category,generalreply,displayyesno,ChatbotQue2Yes,Chatbot2YesReply,Chatbot2NoReply,ChatbotCategory,Chatbot2GeneralReply,Chatbot2DisplayYesNo,ChatbotQue3No,Chatbot3YesReply,Chatbot3NoReply,Chatbot3Category,Chatbot3GeneralReply,Chatbot3DisplayYesNo)Values(@ChatbotQue,@Chatbotyesreply,@Chatbotnoreply,@SortOrder,@Category,@generalreply,@displayyesno,@ChatbotQue2Yes,@Chatbot2YesReply,@Chatbot2NoReply,@ChatbotCategory,@Chatbot2GeneralReply,@Chatbot2DisplayYesNo,@Chnew,@Chnew2,@Chnew3,@Chnew4,@Chnew5,@Chnew6)", con);


            //SqlCommand com = new SqlCommand("Insert Into tbl_Master_Chatbot_settings(ChatbotQue3No,Chatbot3YesReply,Chatbot3NoReply,Chatbot3Category,Chatbot3GeneralReply,Chatbot3DisplayYesNo)Values(@ChatbotQue3No,@Chatbot3YesReply,@Chatbot3NoReply,@Chatbot3Category,@Chatbot3GeneralReply,@Chatbot3DisplayYesNo)", con);
            com.CommandType = CommandType.Text;

            if(!string.IsNullOrEmpty(obj.Chatbotquery))
            {
                com.Parameters.AddWithValue("@ChatbotQue", obj.Chatbotquery);
            }
            else
                com.Parameters.AddWithValue("@ChatbotQue", DBNull.Value);


            if (!string.IsNullOrEmpty(obj.ChatbotYesReply))
              com.Parameters.AddWithValue("@Chatbotyesreply", obj.ChatbotYesReply);
            
            else
                com.Parameters.AddWithValue("@Chatbotyesreply", DBNull.Value);

            if (!string.IsNullOrEmpty(obj.ChatbotNoReply))
                com.Parameters.AddWithValue("@Chatbotnoreply", obj.ChatbotNoReply);

            else
                com.Parameters.AddWithValue("@Chatbotnoreply", DBNull.Value);

           
            com.Parameters.AddWithValue("@SortOrder", obj.SortOrder);

            com.Parameters.AddWithValue("@Category", obj.ChatbotCategory);
            if (!string.IsNullOrEmpty(obj.ChatbotGeneralReply))
                com.Parameters.AddWithValue("@generalreply", obj.ChatbotGeneralReply);

            else
                com.Parameters.AddWithValue("@generalreply", DBNull.Value);
           
            com.Parameters.AddWithValue("@displayyesno", obj.Displayyesno);


            if (!string.IsNullOrEmpty(obj.ChatbotQue2Yes))
                com.Parameters.AddWithValue("@ChatbotQue2Yes", obj.ChatbotQue2Yes);

            else
                com.Parameters.AddWithValue("@ChatbotQue2Yes", DBNull.Value);

            if (!string.IsNullOrEmpty(obj.ChatbotQue2Yes))
                com.Parameters.AddWithValue("@Chatbot2YesReply", obj.Chatbot2YesReply);

            else
                com.Parameters.AddWithValue("@Chatbot2YesReply", DBNull.Value);

            if (!string.IsNullOrEmpty(obj.ChatbotQue2Yes))
                com.Parameters.AddWithValue("@Chatbot2NoReply", obj.Chatbot2YesReply);

            else
                com.Parameters.AddWithValue("@Chatbot2NoReply", DBNull.Value);
           
            com.Parameters.AddWithValue("@ChatbotCategory", obj.Chatbot2Category);
            if (!string.IsNullOrEmpty(obj.Chatbot2GeneralReply))
                com.Parameters.AddWithValue("@Chatbot2GeneralReply", obj.Chatbot2GeneralReply);

            else
                com.Parameters.AddWithValue("@Chatbot2GeneralReply", DBNull.Value);
           
            com.Parameters.AddWithValue("@Chatbot2DisplayYesNo", obj.Chatbot2DisplayYesNo);



            if (!string.IsNullOrEmpty(obj.ChatbotQue3No))
                com.Parameters.AddWithValue("@Chnew", obj.ChatbotQue3No);

            else
                com.Parameters.AddWithValue("@Chnew", DBNull.Value);


            if (!string.IsNullOrEmpty(obj.Chatbot3YesReply))
                com.Parameters.AddWithValue("@Chnew2", obj.Chatbot3YesReply);

            else
                com.Parameters.AddWithValue("@Chnew2", DBNull.Value);


            if (!string.IsNullOrEmpty(obj.Chatbot3YesReply))
                com.Parameters.AddWithValue("@Chnew3", obj.Chatbot3NoReply);

            else
                com.Parameters.AddWithValue("@Chnew3", DBNull.Value);



         
            com.Parameters.AddWithValue("@Chnew4", obj.Chatbot3Category);
            if (!string.IsNullOrEmpty(obj.Chatbot3GeneralReply))
                com.Parameters.AddWithValue("@Chnew5", obj.Chatbot3GeneralReply);

            else
                com.Parameters.AddWithValue("@Chnew5", DBNull.Value);
          
            com.Parameters.AddWithValue("@Chnew6", obj.Chatbot3DisplayYesNo);


            //  com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
            i = com.ExecuteNonQuery();
                con.Close();
            //}
            //catch (Exception ex)
            //{ }
            return i;

        }


        public int UpdateChatbot(Chatbot obj)
        {
            int i = 0;
            try
            {
                con.Open();
                                                                                                                                                                                                                                                                                                                                                                                                                                                
                SqlCommand com = new SqlCommand("Update tbl_Master_Chatbot_settings set ChatbotQue=@ChatbotQue,Chatbotyesreply=@Chatbotyesreply,Chatbotnoreply=@Chatbotnoreply,SortOrder=@SortOrder,Category=@Category,generalreply=@generalreply,displayyesno=@displayyesno,ChatbotQue2Yes=@ChatbotQue2Yes,Chatbot2YesReply=@Chatbot2YesReply,Chatbot2NoReply=@Chatbot2NoReply,ChatbotCategory=@ChatbotCategory,Chatbot2GeneralReply=@Chatbot2GeneralReply,Chatbot2DisplayYesNo=@Chatbot2DisplayYesNo,ChatbotQue3No=@Chnew,Chatbot3YesReply=@Chnew2,Chatbot3NoReply=@Chnew3,Chatbot3Category=@Chnew4,Chatbot3GeneralReply=@Chnew5,Chatbot3DisplayYesNo=@Chnew6 WHERE (@Id IS NULL OR [Id] = @Id)", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", obj.Id);

                if (!string.IsNullOrEmpty(obj.Chatbotquery))
                    com.Parameters.AddWithValue("@ChatbotQue", obj.Chatbotquery);
                else
                    com.Parameters.AddWithValue("@ChatbotQue", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.ChatbotYesReply))
                    com.Parameters.AddWithValue("@Chatbotyesreply", obj.ChatbotYesReply);
                else
                    com.Parameters.AddWithValue("@Chatbotyesreply", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.ChatbotNoReply))
                    com.Parameters.AddWithValue("@Chatbotnoreply", obj.ChatbotNoReply);
                else
                    com.Parameters.AddWithValue("@Chatbotnoreply", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.SortOrder.ToString()))
                    com.Parameters.AddWithValue("@SortOrder", obj.SortOrder);
                else
                    com.Parameters.AddWithValue("@SortOrder", DBNull.Value);




                com.Parameters.AddWithValue("@Category", obj.ChatbotCategory);

                if (!string.IsNullOrEmpty(obj.ChatbotGeneralReply))
                    com.Parameters.AddWithValue("@generalreply", obj.ChatbotGeneralReply);

                else
                    com.Parameters.AddWithValue("@generalreply", DBNull.Value);

                com.Parameters.AddWithValue("@displayyesno", obj.Displayyesno);


                if (!string.IsNullOrEmpty(obj.ChatbotQue2Yes))
                    com.Parameters.AddWithValue("@ChatbotQue2Yes", obj.ChatbotQue2Yes);

                else
                    com.Parameters.AddWithValue("@ChatbotQue2Yes", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.ChatbotQue2Yes))
                    com.Parameters.AddWithValue("@Chatbot2YesReply", obj.Chatbot2YesReply);

                else
                    com.Parameters.AddWithValue("@Chatbot2YesReply", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.ChatbotQue2Yes))
                    com.Parameters.AddWithValue("@Chatbot2NoReply", obj.Chatbot2YesReply);

                else
                    com.Parameters.AddWithValue("@Chatbot2NoReply", DBNull.Value);

                com.Parameters.AddWithValue("@ChatbotCategory", obj.Chatbot2Category);
                if (!string.IsNullOrEmpty(obj.Chatbot2GeneralReply))
                    com.Parameters.AddWithValue("@Chatbot2GeneralReply", obj.Chatbot2GeneralReply);

                else
                    com.Parameters.AddWithValue("@Chatbot2GeneralReply", DBNull.Value);

                com.Parameters.AddWithValue("@Chatbot2DisplayYesNo", obj.Chatbot2DisplayYesNo);



                if (!string.IsNullOrEmpty(obj.ChatbotQue3No))
                    com.Parameters.AddWithValue("@Chnew", obj.ChatbotQue3No);

                else
                    com.Parameters.AddWithValue("@Chnew", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.Chatbot3YesReply))
                    com.Parameters.AddWithValue("@Chnew2", obj.Chatbot3YesReply);

                else
                    com.Parameters.AddWithValue("@Chnew2", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.Chatbot3YesReply))
                    com.Parameters.AddWithValue("@Chnew3", obj.Chatbot3NoReply);

                else
                    com.Parameters.AddWithValue("@Chnew3", DBNull.Value);




                com.Parameters.AddWithValue("@Chnew4", obj.Chatbot3Category);
                if (!string.IsNullOrEmpty(obj.Chatbot3GeneralReply))
                    com.Parameters.AddWithValue("@Chnew5", obj.Chatbot3GeneralReply);

                else
                    com.Parameters.AddWithValue("@Chnew5", DBNull.Value);

                com.Parameters.AddWithValue("@Chnew6", obj.Chatbot3DisplayYesNo);

                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public int DeleteChatbot(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from [tbl_Master_Chatbot_settings] where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int insertBackofficeSteps(Chatbot obj)
        {
            int i = 0;
            string active = "1";
            obj.IsActive = true;
            try { 
            con.Open();




            SqlCommand com = new SqlCommand("Insert Into tbl_backoffice_Steps(Stepname,youtubelink,image,stepfollow,IsActive,VideoLink)Values(@Stepname,@youtubelink,@image,@stepfollow,@IsActive,@VideoLink)", con);


            //SqlCommand com = new SqlCommand("Insert Into tbl_Master_Chatbot_settings(ChatbotQue3No,Chatbot3YesReply,Chatbot3NoReply,Chatbot3Category,Chatbot3GeneralReply,Chatbot3DisplayYesNo)Values(@ChatbotQue3No,@Chatbot3YesReply,@Chatbot3NoReply,@Chatbot3Category,@Chatbot3GeneralReply,@Chatbot3DisplayYesNo)", con);
            com.CommandType = CommandType.Text;

            if (!string.IsNullOrEmpty(obj.Query))
            {
                com.Parameters.AddWithValue("@Stepname", obj.Query);
            }
            else
                com.Parameters.AddWithValue("@Stepname", DBNull.Value);


            if (!string.IsNullOrEmpty(obj.Youtubelink))
                com.Parameters.AddWithValue("@youtubelink", obj.Youtubelink);

            else
                com.Parameters.AddWithValue("@youtubelink", DBNull.Value);

            if (!string.IsNullOrEmpty(obj.Image))
                com.Parameters.AddWithValue("@image", obj.Image);

            else
                com.Parameters.AddWithValue("@image", DBNull.Value);


            if (!string.IsNullOrEmpty(obj.steps))
                com.Parameters.AddWithValue("@stepfollow", obj.steps);

            else
                com.Parameters.AddWithValue("@stepfollow", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.IsActive.ToString()))
                    com.Parameters.AddWithValue("@IsActive", obj.IsActive);

                else
                    com.Parameters.AddWithValue("@IsActive", DBNull.Value);



                if (!string.IsNullOrEmpty(obj.videolink))
                    com.Parameters.AddWithValue("@VideoLink", obj.videolink);

                else
                    com.Parameters.AddWithValue("@VideoLink", DBNull.Value);
                //  com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
            con.Close();
            }
            catch (Exception ex)
            { }

            return i;
        }

        public DataTable getBackOfficeSteps(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_backoffice_Steps WHERE (@Id IS NULL OR Id = @Id) Order By Id", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public int UpdateBackofficeSteps(Chatbot obj)
        {
            int i = 0;
            string active = "1";
            obj.IsActive = true;
            try
            {
                con.Open();

                SqlCommand com = new SqlCommand("Update tbl_backoffice_Steps set  Stepname=@Stepname,youtubelink=@youtubelink,image=@image,stepfollow=@stepfollow,VideoLink=@VideoLink where Id=@Id", con);


                //SqlCommand com = new SqlCommand("Insert Into tbl_Master_Chatbot_settings(ChatbotQue3No,Chatbot3YesReply,Chatbot3NoReply,Chatbot3Category,Chatbot3GeneralReply,Chatbot3DisplayYesNo)Values(@ChatbotQue3No,@Chatbot3YesReply,@Chatbot3NoReply,@Chatbot3Category,@Chatbot3GeneralReply,@Chatbot3DisplayYesNo)", con);
                com.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(obj.Query))
                {
                    com.Parameters.AddWithValue("@Stepname", obj.Query);
                }
                else
                    com.Parameters.AddWithValue("@Stepname", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.Youtubelink))
                    com.Parameters.AddWithValue("@youtubelink", obj.Youtubelink);

                else
                    com.Parameters.AddWithValue("@youtubelink", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.Image))
                    com.Parameters.AddWithValue("@image", obj.Image);

                else
                    com.Parameters.AddWithValue("@image", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.steps))
                    com.Parameters.AddWithValue("@stepfollow", obj.steps);

                else
                    com.Parameters.AddWithValue("@stepfollow", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.BackOfficeId.ToString()))
                    com.Parameters.AddWithValue("@Id", obj.BackOfficeId);

                else
                    com.Parameters.AddWithValue("@Id", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.videolink))
                    com.Parameters.AddWithValue("@VideoLink", obj.videolink);

                else
                    com.Parameters.AddWithValue("@VideoLink", DBNull.Value);
                //  com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }

            return i;
        }

        public int DeleteBackOfficeSet(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from [tbl_backoffice_Steps] where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MilkWayIndia.Models
{
    public class Offer
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);
        public int Id { get; set; }
        
        public string NewCustomerMsg { get; set; }
        public string NewCustomerMsg1 { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public int SectorId { get; set; }
        public string MobileNo { get; set; }
        public string Whatsappno { get; set; }
        public string Type { get; set; }

        public string StateName { get; set; }
        public string CityName { get; set; }
        public DateTime Updatedon { get; set; }
        public bool IsActive { get; set; }
        public int InsertNewCustomerMsg(Offer obj)
        {
            int i = 0;
            string IsActive = "1";
            try
            {
                if (obj.NewCustomerMsg.ToString() == "" || string.IsNullOrEmpty(obj.NewCustomerMsg.ToString()))
                {
                    obj.NewCustomerMsg = "";
                }

                

                con.Open();
                SqlCommand com = new SqlCommand("Insert Into tbl_New_Customermsg(Message,IsActive,StateId,MobileNo,Whatsappno,CityId,Type)Values(@Message,@IsActive,@StateId,@MobileNo,@Whatsappno,@CityId,@Type)", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Message", obj.NewCustomerMsg);
                com.Parameters.AddWithValue("@IsActive", IsActive);
                com.Parameters.AddWithValue("@StateId", obj.StateId);
                com.Parameters.AddWithValue("@MobileNo", obj.MobileNo);
                com.Parameters.AddWithValue("@Whatsappno", obj.Whatsappno);
                com.Parameters.AddWithValue("@CityId", obj.CityId);
                com.Parameters.AddWithValue("@Type", obj.Type);

                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
        }
            catch (Exception ex)
            { }
            return i;

        }


        public DataTable getMessageList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Select Cm.*,sm.statename,Cm1.Cityname from tbl_New_Customermsg Cm inner join tblcitymaster Cm1 on Cm1.id=Cm.CityId Inner join tblstatemaster Sm on Sm.id=Cm1.StateName", con);
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


        public int InsertSectorOffer(Offer obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Insert Into tbl_SectorWiseMsg(StateId,CityId,SectorId,Message,Updatedon,IsActive)Values(@StateId,@CityId,@SectorId,@Message,@Updatedon,@IsActive)", con);
                com.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(obj.StateId.ToString()))
                    com.Parameters.AddWithValue("@StateId", obj.StateId);
                else
                    com.Parameters.AddWithValue("@StateId", 0);
                if (!string.IsNullOrEmpty(obj.CityId.ToString()))
                    com.Parameters.AddWithValue("@CityId", obj.CityId);
                else
                    com.Parameters.AddWithValue("@CityId", 0);
                if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    com.Parameters.AddWithValue("@SectorId", obj.SectorId);
                else
                    com.Parameters.AddWithValue("@SectorId", 0);
                if (!string.IsNullOrEmpty(obj.NewCustomerMsg.ToString()))
                    com.Parameters.AddWithValue("@Message", obj.NewCustomerMsg);
                else
                    com.Parameters.AddWithValue("@Message", DBNull.Value);
                com.Parameters.AddWithValue("@Updatedon", obj.Updatedon);

                com.Parameters.AddWithValue("@IsActive", true);
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                try
                {
                    con.Close();
                    int id = Convert.ToInt32(com.Parameters["@Id"].Value);
                    return id;
                }
                catch { }
            }
            catch (Exception ex)
            { }
            return i;
        }


        public DataTable getSectorMessageList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Select Cm.*,Sm1.statename,Cm1.Cityname,sm.SectorName from tbl_SectorWiseMsg Cm inner join tbl_Sector_Master sm on sm.id=Cm.SectorId Inner Join tblstatemaster Sm1 On sm.StateId=Sm1.id Inner Join tblcitymaster Cm1 on sm.CityId=Cm1.id where (@SectorId is Null Or (Cm.SectorId=@SectorId)) AND Cm.IsActive='True'", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", Id);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getSectorMessageListById(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Select Cm.*,Sm1.statename,Cm1.Cityname,sm.SectorName from tbl_SectorWiseMsg Cm inner join tbl_Sector_Master sm on sm.id=Cm.SectorId Inner Join tblstatemaster Sm1 On sm.StateId=Sm1.id Inner Join tblcitymaster Cm1 on sm.CityId=Cm1.id where (@Id is Null Or (Cm.Id=@Id)) AND Cm.IsActive='True'", con);
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


        public DataTable GetSectorAssignedMsg(int? cId, int? sId, string msg)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select Cm.*,Sm.SectorName from tbl_SectorWiseMsg Cm inner join tbl_Sector_Master Sm on Sm.Id=Cm.SectorId  where Cm.StateId=@StateId AND Cm.CityId=@CityId AND Cm.Message=@Message AND IsActive='True'", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(cId.ToString()))
                cmd.Parameters.AddWithValue("@CityId", cId);
            else
                cmd.Parameters.AddWithValue("@CityId", DBNull.Value);
            if (!string.IsNullOrEmpty(sId.ToString()))
                cmd.Parameters.AddWithValue("@StateId", sId);
            else
                cmd.Parameters.AddWithValue("@StateId", DBNull.Value);
            if (!string.IsNullOrEmpty(msg.ToString()))
                cmd.Parameters.AddWithValue("@Message", msg);
            else
                cmd.Parameters.AddWithValue("@Message", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public int DeleteSectorMessageUnAssigned(int id,string msg)
        {
            try
            {
                con.Open();
                string q = "Update tbl_SectorWiseMsg set IsActive='0' where SectorId=" + id + " and Message='"+msg.ToString()+"'";
                SqlCommand cd = new SqlCommand(q, con);
                int i = cd.ExecuteNonQuery();
                return i;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                con.Close();
            }

        }


        public int UpdateSectorMessage(int id, string oldmsg,string newmsg)
        {
            try
            {
                con.Open();
                string q = "Update tbl_SectorWiseMsg set Message='"+newmsg+"' where SectorId=" + id + " and Message='" + oldmsg.ToString() + "'";
                SqlCommand cd = new SqlCommand(q, con);
                int i = cd.ExecuteNonQuery();
                return i;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                con.Close();
            }

        }


        public DataTable getNewMessageListById(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Select * from tbl_New_Customermsg where (@Id is Null Or (Id=@Id))", con);
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


        public int UpdateNewCustomerMsg(Offer obj)
        {
            int i = 0;
            string IsActive = "1";
            try
            {
                if (obj.NewCustomerMsg.ToString() == "" || string.IsNullOrEmpty(obj.NewCustomerMsg.ToString()))
                {
                    obj.NewCustomerMsg = "";
                }



                con.Open();
                SqlCommand com = new SqlCommand("UPDATE tbl_New_Customermsg SET Message=@Message,StateId=@StateId,MobileNo=@MobileNo,Whatsappno=@Whatsappno,CityId=@CityId,Type=@Type where (@Id is Null or Id=@Id)", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", obj.Id);
                com.Parameters.AddWithValue("@Message", obj.NewCustomerMsg);
                
                com.Parameters.AddWithValue("@StateId", obj.StateId);
                com.Parameters.AddWithValue("@MobileNo", obj.MobileNo);
                com.Parameters.AddWithValue("@Whatsappno", obj.Whatsappno);
                com.Parameters.AddWithValue("@CityId", obj.CityId);
                com.Parameters.AddWithValue("@Type", obj.Type);

               
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }
    }
}
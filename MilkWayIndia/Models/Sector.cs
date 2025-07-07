using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Models
{
    public class Sector
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);

        //Sector
        public int Id { get; set; }
        public string SectorName { get; set; }
        public string Statename { get; set; }
        public string Cityname { get; set; }
        public string LandMark { get; set; }
        public string PinCode { get; set; }

        //Building
        public string BuildingName { get; set; }
        public int SectorId { get; set; }


        public int StateId { get; set; }
        public int CityId { get; set; }
        public string BlockNo { get; set; }
        public string FlatNo { get; set; }

        public int BuildingId { get; set; }
        public int orderBy { get; set; }

        public DataTable getSectorList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Sector_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if(!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


    

        //duplicate sector
        public DataTable getCheckDuplSector(string secname)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Sector_Master where SectorName='" + secname + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public int InsertSector(Sector obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Sector_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@SectorName", obj.SectorName);
                if (!string.IsNullOrEmpty(obj.LandMark))
                    com.Parameters.AddWithValue("@LandMark", obj.LandMark);
                else
                    com.Parameters.AddWithValue("@LandMark", DBNull.Value);
                com.Parameters.AddWithValue("@PinCode", obj.PinCode);
                com.Parameters.AddWithValue("@StateId", obj.StateId);
                com.Parameters.AddWithValue("@CityId", obj.CityId);
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;
            
        }

        public int InsertSector1(Sector obj,string sec, string pin)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Sector_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@SectorName",sec);
                if (!string.IsNullOrEmpty(obj.LandMark))
                    com.Parameters.AddWithValue("@LandMark", obj.LandMark);
                else
                    com.Parameters.AddWithValue("@LandMark", DBNull.Value);
                com.Parameters.AddWithValue("@PinCode", pin);
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int UpdateSector(Sector obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Sector_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.Id);
                com.Parameters.AddWithValue("@SectorName", obj.SectorName);
                if (!string.IsNullOrEmpty(obj.LandMark))
                    com.Parameters.AddWithValue("@LandMark", obj.LandMark);
                else
                    com.Parameters.AddWithValue("@LandMark", DBNull.Value);
                com.Parameters.AddWithValue("@PinCode", obj.PinCode);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        //delete
        public int DeleteSector(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Sector_Master where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public DataTable getBuildingList(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Building_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        //duplicate building
        public DataTable getCheckDuplBuilding(int sid,string bname,string block)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Building_Master where SectorId='"+sid+ "' and BuildingName='" + bname + "' and (BlockNo is NULL or BlockNo='" + block+"')", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public int InsertBuilding(Sector obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Building_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@SectorId", obj.SectorId);
                com.Parameters.AddWithValue("@BuildingName", obj.BuildingName);
                if (!string.IsNullOrEmpty(obj.BlockNo))
                    com.Parameters.AddWithValue("@BlockNo", obj.BlockNo);
                else
                    com.Parameters.AddWithValue("@BlockNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.FlatNo))
                    com.Parameters.AddWithValue("@FlatNo", obj.FlatNo);
                else
                    com.Parameters.AddWithValue("@FlatNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.orderBy.ToString()))
                    com.Parameters.AddWithValue("@orderBy", obj.orderBy);
                else
                    com.Parameters.AddWithValue("@orderBy", DBNull.Value);
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int UpdateBuilding(Sector obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Building_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.Id);
                com.Parameters.AddWithValue("@SectorId", obj.SectorId);
                com.Parameters.AddWithValue("@BuildingName", obj.BuildingName);
                if (!string.IsNullOrEmpty(obj.BlockNo))
                    com.Parameters.AddWithValue("@BlockNo", obj.BlockNo);
                else
                    com.Parameters.AddWithValue("@BlockNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.orderBy.ToString()) && obj.orderBy!= 0)
                    com.Parameters.AddWithValue("@orderBy", obj.orderBy);
                else
                    com.Parameters.AddWithValue("@orderBy", DBNull.Value);
                //if (!string.IsNullOrEmpty(obj.FlatNo))
                //    com.Parameters.AddWithValue("@FlatNo", obj.FlatNo);
                //else
                //    com.Parameters.AddWithValue("@FlatNo", obj.FlatNo);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public DataTable geSectorwisetBuildingList(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Sector_Building_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", Id);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
        public DataTable getBuildingWiseCustomerList(int? BuildingId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Building_Customer_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@BuildingId", BuildingId);
            else
                cmd.Parameters.AddWithValue("@BuildingId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable getBuildingwiseFlatNoList(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Building_FlatNo_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@BuildingId", Id);
            else
                cmd.Parameters.AddWithValue("@BuildingId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable getFlatNoList(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("FlatNo_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        //delete
        public int DeleteBuilding(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Building_Master where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        //duplicate flatno
        public DataTable getCheckDupliFlatNo(int bid, string flatno)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Building_Flat_Master where BuildingId='" + bid + "' and FlatNo='" + flatno + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public int InsertFlatNo(Sector obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("FlatNo_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@BuildingId", obj.BuildingId);
                if (!string.IsNullOrEmpty(obj.FlatNo))
                    com.Parameters.AddWithValue("@FlatNo", obj.FlatNo);
                else
                    com.Parameters.AddWithValue("@FlatNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.orderBy.ToString()))
                    com.Parameters.AddWithValue("@orderBy", obj.orderBy);
                else
                    com.Parameters.AddWithValue("@orderBy", DBNull.Value);
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int UpdateFlatNo(Sector obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("FlatNo_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.Id);
                com.Parameters.AddWithValue("@BuildingId", obj.BuildingId);
                if (!string.IsNullOrEmpty(obj.FlatNo))
                    com.Parameters.AddWithValue("@FlatNo", obj.FlatNo);
                else
                    com.Parameters.AddWithValue("@FlatNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.orderBy.ToString()) && obj.orderBy != 0)
                    com.Parameters.AddWithValue("@orderBy", obj.orderBy);
                else
                    com.Parameters.AddWithValue("@orderBy", DBNull.Value);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        //delete
        public int DeleteFlatNo(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Building_Flat_Master where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }


        public DataTable getStateList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * from[dbo].[tblstatemaster] WHERE (@Id IS NULL OR[Id] = @Id) Order by statename ASC", con);
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


        public DataTable getallCityList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * from[dbo].[tblcitymaster] WHERE (@Id IS NULL OR[Id] = @Id) Order by Cityname ASC", con);
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
        public DataTable getCheckDuplState(string statename)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tblstatemaster where statename='" + statename + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public int InsertState(Sector obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("INSERT [dbo].[tblstatemaster](statename)Values(@statename)", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@statename", obj.Statename);
                
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public int UpdateState(Sector obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Update tblstatemaster set statename=@StateName WHERE id = @Id", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", obj.Id);
                com.Parameters.AddWithValue("@StateName", obj.Statename);
                
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int DeleteState(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tblstatemaster where id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }




        public DataTable getCityList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT cm.*,sm.statename AS state from[dbo].[tblcitymaster] cm Inner join tblstatemaster sm on sm.id=cm.StateName WHERE (@Id IS NULL OR cm.id = @Id) Order by sm.statename ASC", con);
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
        public DataTable getCheckDuplCity(string state,string city)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tblcitymaster where StateName='" + state + "' and Cityname='"+city+"'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public int InsertCity(Sector obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("INSERT [dbo].[tblcitymaster](Cityname,StateName)Values(@cityname,@statename)", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@cityname", obj.Cityname);
                com.Parameters.AddWithValue("@statename", obj.Statename);

                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public int UpdateCity(Sector obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Update tblcitymaster set Cityname=@Cityname WHERE id = @Id", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", obj.Id);
                com.Parameters.AddWithValue("@Cityname", obj.Cityname);

                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public int DeleteCity(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tblcitymaster where id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }
    }
}
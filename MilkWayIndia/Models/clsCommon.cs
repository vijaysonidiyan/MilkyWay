using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Models
{
    public class clsCommon
    {
        SqlCommand cm;
        SqlDataAdapter da;
        DataTable dt;
        string Condition;        
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);

        public DataTable select(string columnname, string tablename)
        {
            try
            {
                cn.Open();
                dt = new DataTable();
                cm = new SqlCommand("Select " + columnname + " from " + tablename, cn);
                da = new SqlDataAdapter(cm);
                da.Fill(dt);
            }

            catch (Exception ex)
            {

            }
            finally
            {
                cn.Close();
            }
            return dt;

        }

        public int insertbyquery(string qry)
        {
            int result = 0;
            try
            {
                cn.Open();
                cm = new SqlCommand(qry, cn);
                result = cm.ExecuteNonQuery();
                cm.Dispose();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                cn.Close();
            }
            return result;
        }

        public int insertdata(string tablename, string columnname, string values)
        {
            int val = 0;
            try
            {
                cn.Open();
                cm = new SqlCommand("insert into " + tablename + " " + columnname + " " + "values  (" + values + ")", cn);
                val = cm.ExecuteNonQuery();
                cm.Dispose();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                cn.Close();
            }
            return val;
        }
      
        public int updatedata(string tablename, string values, string condition)
        {


            int val = 0;
            try
            {
                cn.Open();
                Condition = "";
                if (condition != "")
                {
                    Condition = "where " + condition;
                }

                cm = new SqlCommand("update " + tablename + " set " + values + " " + Condition, cn);
                val = cm.ExecuteNonQuery();
                cm.Dispose();
            }
            catch (Exception)
            {

            }
            finally
            {
                cn.Close();
            }
            return val;
        }

        public int deletedata(string tablename, string condition)
        {


            int val = 0;
            try
            {
                cn.Open();
                Condition = "";
                if (condition != "")
                {
                    Condition = "where " + condition;
                }

                cm = new SqlCommand("delete from " + tablename + " " + Condition, cn);
                val = cm.ExecuteNonQuery();
                cm.Dispose();
            }
            catch (Exception)
            {

            }
            finally
            {
                cn.Close();
            }
            return val;
        }

        public DataTable selectwhere(string columnname, string tablename, string condition)
        {

            try
            {
                cn.Open();
                Condition = "";
                if (condition != "")
                    Condition = "where " + condition;

                dt = new DataTable();
                cm = new SqlCommand("Select " + columnname + " from " + tablename + " " + Condition, cn);
                da = new SqlDataAdapter(cm);
                da.Fill(dt);
            }

            catch (Exception ex)
            {

            }
            finally
            {
                cn.Close();
            }
            return dt;
        }
       
        public DataTable showdata(string qry)
        {
            try
            {
                cn.Open();
                cn.Close();
                dt = new DataTable();
                da = new SqlDataAdapter(qry, cn);
                da.Fill(dt);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                cn.Close();
            }
            return dt;
        }

        public int insert(string qry)
        {
            int res = 0;
            try
            {
                cn.Open();
                cm = new SqlCommand(qry, cn);
                res = cm.ExecuteNonQuery();
                cm.Dispose();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                cn.Close();
            }
            return res;
        }

        public int update(string qry)
        {
            int res = 0;
            try
            {
                cn.Open();
                cm = new SqlCommand(qry, cn);
                res = cm.ExecuteNonQuery();
                cm.Dispose();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                cn.Close();
            }
            return res;
        }

        public string Get_Entity(string columnname, string tablename, string condition)
        {
            string abcd = "";

            try
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }

                cn.Open();
                Condition = "";
                if (condition != "")
                {
                    Condition = "where " + condition;
                }

                cm = new SqlCommand("Select " + columnname + " from " + tablename + " " + Condition, cn);
                abcd = cm.ExecuteScalar().ToString();
            }

            catch (Exception)
            {

            }
            finally
            {
                cn.Close();
            }
            return abcd;
        }

        public string Get_Entity(string query)
        {
            string abcd = "";

            try
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
                cn.Open();
                cm = new SqlCommand(query, cn);
                abcd = cm.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                cn.Close();
            }
            return abcd;
        }
    }
}
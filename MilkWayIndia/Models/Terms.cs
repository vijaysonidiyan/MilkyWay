using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Models
{
    public class Terms
    {

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);


        public int Id { get; set; }
        public int Pos { get; set; }
        public string terms { get; set; }




        public int Insertterms(Terms obj)
        {
            int i = 0;
            //try
            //{
               
                con.Open();
                SqlCommand com = new SqlCommand("Insert Into tbl_terms(Pos,terms)Values(@Pos,@terms)", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Pos", obj.Pos);
                com.Parameters.AddWithValue("@terms", obj.terms);
                
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            //}
            //catch (Exception ex)
            //{ }
            return i;

        }

        public DataTable getTermsList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_terms where @Id is Null or(Id=@Id)  Order By Pos ", con);
            cmd.CommandType = CommandType.Text;
            
            if(!string.IsNullOrEmpty(Id.ToString()))
            {
                cmd.Parameters.AddWithValue("@Id", Id);
            }

            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public int Updateterms(Terms obj)
        {
            int i = 0;
            //try
            //{

            con.Open();
            SqlCommand com = new SqlCommand("UPDATE tbl_terms  SEt Pos=@Pos,terms=@terms where Id=@id", con);
            com.CommandType = CommandType.Text;
            com.Parameters.AddWithValue("@Id", obj.Id);
            com.Parameters.AddWithValue("@Pos", obj.Pos);
            com.Parameters.AddWithValue("@terms", obj.terms);

           
            i = com.ExecuteNonQuery();
            con.Close();
            //}
            //catch (Exception ex)
            //{ }
            return i;

        }


        public int Deleteterms(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from [tbl_terms] where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

    }
}
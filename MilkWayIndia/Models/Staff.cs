using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Models
{
    public class Staff
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);

        public int Id { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string StaffName { get; set; }
        public string LastName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime? DOB { get; set; }
        public bool IsActive { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }

        public string PlatformId { get; set; }
        public string PlatformName { get; set; }

        public string ModuleName { get; set; }
        public int ModuleId { get; set; }

        public int RoleId { get; set; }
        public int UservalidationId { get; set; }
        public bool IsView { get; set; }
        public bool IsAdd{ get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsmanageFund { get; set; }
        public DataTable Adminlogin(string UserName, string Password)
        {
            con.Open();
            SqlCommand cmdLoginUser = new SqlCommand("select * from tbl_Staff_Master WHERE UserName=@UserName and Password=@Password", con);
            cmdLoginUser.Parameters.AddWithValue("@UserName", UserName);
            cmdLoginUser.Parameters.AddWithValue("@Password", Password);
            SqlDataAdapter daLoginUser = new SqlDataAdapter(cmdLoginUser);
            DataTable dtLoginUser = new DataTable();
            daLoginUser.Fill(dtLoginUser);
            con.Close();
            return dtLoginUser;
        }

        public int InsertStaff(Staff obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Staff_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Role", obj.Role);
                if (!string.IsNullOrEmpty(obj.FirstName))
                    com.Parameters.AddWithValue("@FirstName", obj.FirstName);
                else
                    com.Parameters.AddWithValue("@FirstName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.LastName))
                    com.Parameters.AddWithValue("@LastName", obj.LastName);
                else
                    com.Parameters.AddWithValue("@LastName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.MobileNo))
                    com.Parameters.AddWithValue("@MobileNo", obj.MobileNo);
                else
                    com.Parameters.AddWithValue("@MobileNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Email))
                    com.Parameters.AddWithValue("@Email", obj.Email);
                else
                    com.Parameters.AddWithValue("@Email", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Address))
                    com.Parameters.AddWithValue("@Address", obj.Address);
                else
                    com.Parameters.AddWithValue("@Address", DBNull.Value);
                com.Parameters.AddWithValue("@UserName", obj.UserName);
                com.Parameters.AddWithValue("@Password", obj.Password);
                if (!string.IsNullOrEmpty(obj.Photo))
                    com.Parameters.AddWithValue("@Photo", obj.Photo);
                else
                    com.Parameters.AddWithValue("@Photo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.DOB.ToString()))
                    com.Parameters.AddWithValue("@DOB", obj.DOB);
                else
                    com.Parameters.AddWithValue("@DOB", DBNull.Value);
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int UpdateStaff(Staff obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Staff_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.Id);
                com.Parameters.AddWithValue("@Role", obj.Role);
                if (!string.IsNullOrEmpty(obj.FirstName))
                    com.Parameters.AddWithValue("@FirstName", obj.FirstName);
                else
                    com.Parameters.AddWithValue("@FirstName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.LastName))
                    com.Parameters.AddWithValue("@LastName", obj.LastName);
                else
                    com.Parameters.AddWithValue("@LastName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.MobileNo))
                    com.Parameters.AddWithValue("@MobileNo", obj.MobileNo);
                else
                    com.Parameters.AddWithValue("@MobileNo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Email))
                    com.Parameters.AddWithValue("@Email", obj.Email);
                else
                    com.Parameters.AddWithValue("@Email", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Address))
                    com.Parameters.AddWithValue("@Address", obj.Address);
                else
                    com.Parameters.AddWithValue("@Address", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.DOB.ToString()))
                    com.Parameters.AddWithValue("@DOB", obj.DOB);
                else
                    com.Parameters.AddWithValue("@DOB", DBNull.Value);
                com.Parameters.AddWithValue("@UserName", obj.UserName);
                com.Parameters.AddWithValue("@Password", obj.Password);
                if (!string.IsNullOrEmpty(obj.Photo))
                    com.Parameters.AddWithValue("@Photo", obj.Photo);
                else
                    com.Parameters.AddWithValue("@Photo", DBNull.Value);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public DataTable getStaffList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Staff_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        //bind only delivery boy
        public DataTable getDeliveryBoyList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Staff_DeliveryBoy_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable CheckStaffUserName(string username)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Staff_Master where UserName='" + username + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable CheckDuplicateStaff(string role,string fname,string lname,string mobile)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Staff_Master where Role='" + role + "' and FirstName='"+fname+"' and LastName='"+lname+"' and MobileNo='"+mobile+"'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        //delete
        public int DeleteStaff(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from [tbl_Staff_Master] where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }


        public DataTable getPlatformList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Select * From tbldesignation where (@Id Is Null Or Id=@Id)", con);
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



        public int InsertPlatform(Staff obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("InsertRole", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@DesignationName", obj.PlatformName);
             
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;

               
                i = com.ExecuteNonQuery();

                try
                {
                    con.Close();
                    int id = Convert.ToInt32(com.Parameters["@Id"].Value);
                    return id;
                }
                catch { }
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int DeleteRole(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from [tbldesignation] where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }


        public DataTable getModuleList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Select * From tblModule WHERE (@Id IS NULL OR [Id] = @Id)", con);
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


        public int InsertModule(Staff obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Insert Into tblModule(PlatformName,Modulename)Values(@Platform,@Module)", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Platform", obj.PlatformName);
                com.Parameters.AddWithValue("@Module", obj.ModuleName);
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int UpdateModule(Staff obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Update tblModule set PlatformName=@PlatformName,Modulename=@Modulename WHERE (@Id IS NULL OR [Id] = @Id)", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", obj.Id);
                
                if (!string.IsNullOrEmpty(obj.PlatformName))
                    com.Parameters.AddWithValue("@PlatformName", obj.PlatformName);
                else
                    com.Parameters.AddWithValue("@PlatformName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.ModuleName))
                    com.Parameters.AddWithValue("@Modulename", obj.ModuleName);
                else
                    com.Parameters.AddWithValue("@Modulename", DBNull.Value);
                
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int DeleteModule(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from [tblModule] where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }



        public DataTable BindPlatformWiseModule(String Platform)
        {
            con.Open();


            string query = "SELECT * from tblModule where (@Platform IS NULL OR [PlatformName] = @Platform)";
           



            // SqlCommand cmd = new SqlCommand("Sector_Category_Product_SelectAll", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Platform.ToString()))
                cmd.Parameters.AddWithValue("@Platform", Platform);
            else
                cmd.Parameters.AddWithValue("@Platform", DBNull.Value);
            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public DataTable getStaffList()
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_Staff_Master", con);
            cmd.CommandType = CommandType.Text;
            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getStaffUserValidation()
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_Staff_Master", con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getStaffUserRole(int? Id)
        {
            //con.Open();

            string query = "Select suv.*,Ds.DesignationName AS RoleName,sm.FirstName,sm.LastName,sm.Role As Category from  tbl_StaffUserValidation suv ";
            query += " inner join tbldesignation Ds ON suv.RoleId=Ds.Id Left join tbl_Staff_Master sm ON suv.StaffId=sm.Id WHERE (@Id Is Null Or suv.Id=@Id)";
            //  SqlCommand cmd = new SqlCommand("Select suv.*,m.Modulename AS Name,sm.FirstName,sm.LastName,sm.Role from  tbl_StaffUserValidation suv inner join tblModule m ON suv.ModuleId=m.Id Inner join tbl_Staff_Master sm ON suv.StaffId=sm.Id", con);

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if(!string.IsNullOrEmpty(Id.ToString()))
            cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public int InsertUserValidation(Staff obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Insert Into tbl_StaffUserValidation(StaffId,IsView,IsAdd,IsEdit,IsDelete,IsmanageFund,RoleId)Values(@StaffId,@IsView,@IsAdd,@IsEdit,@IsDelete,@IsmanageFund,@RoleId)", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@StaffId", obj.Id);
                //com.Parameters.AddWithValue("@PlatformName", obj.PlatformName);

                //com.Parameters.AddWithValue("@ModuleId", obj.ModuleId);
                com.Parameters.AddWithValue("@IsView", obj.IsView);
                com.Parameters.AddWithValue("@IsAdd", obj.IsAdd);
                com.Parameters.AddWithValue("@IsEdit", obj.IsEdit);
                com.Parameters.AddWithValue("@IsDelete", obj.IsDelete);
                com.Parameters.AddWithValue("@IsmanageFund", obj.IsmanageFund);
                com.Parameters.AddWithValue("@RoleId", obj.RoleId);

                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;
        }


        public DataTable GetModuleList()
        {
            
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT Id,PlatformName,Modulename FROM tblModule  ORDER BY PlatformName", con);
            cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(CityId.ToString()))
            //    cmd.Parameters.AddWithValue("@CityId", CityId);
            //else
            //    cmd.Parameters.AddWithValue("@CityId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable ChkDuplModule(string moduleid, int? roleId)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Role_Module where RoleId=" + roleId + " and ModuleId=" + moduleid + "", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public DataTable ChkDuplRole(string RoleName)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbldesignation where DesignationName='" + RoleName + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public int InsertRoleModule(string item, string RoleId)
        {
            string active = "1";
            con.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO tbl_Role_Module(RoleId,ModuleId,IsActive)VALUES(@RoleId,@ModuleId,@IsActive)", con);


           
            if (RoleId != "")
                cmd.Parameters.AddWithValue("@RoleId", RoleId);
            else
                cmd.Parameters.AddWithValue("@RoleId", DBNull.Value);
            if (item != "")
                cmd.Parameters.AddWithValue("@ModuleId", item);
            else
                cmd.Parameters.AddWithValue("@ModuleId", DBNull.Value);

            if (active != "")
                cmd.Parameters.AddWithValue("@IsActive", active);
            else
                cmd.Parameters.AddWithValue("@IsActive", 0);


           



            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;


        }

        public DataTable getRoleList()
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Select * from  tbldesignation", con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public DataTable getModuleListByrole(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Select Rm.*,Mo.Id As Id1,Mo.Modulename,Mo.PlatformName From tbl_Role_Module Rm Right Join tblModule Mo On Rm.ModuleId=Mo.Id AND (@Id IS NULL OR Rm.RoleId = @Id) And Rm.IsActive='true'", con);
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


        public int UpdateRoleModulecommon(string PlatformId)
        {




            string active = "0";
            con.Open();




            SqlCommand cmd = new SqlCommand("UPDATE tbl_Role_Module set IsActive=@IsActive where RoleId=@RoleId", con);


            if (PlatformId != "")
                cmd.Parameters.AddWithValue("@RoleId", PlatformId);
            else
                cmd.Parameters.AddWithValue("@RoleId", DBNull.Value);


          



            if (active != "")
                cmd.Parameters.AddWithValue("@IsActive", active);
            else
                cmd.Parameters.AddWithValue("@IsActive", 0);







            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;


        }


        public DataTable ChkDuplRoleModule(string roleid, string Item)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Role_Module where RoleId=" + roleid + " and ModuleId=" + Item + "", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }



        public int UpdateRoleModule(string PlatformId,string Item)
        {




            string active = "1";
            con.Open();




            SqlCommand cmd = new SqlCommand("UPDATE tbl_Role_Module set IsActive=@IsActive where RoleId=@RoleId and ModuleId=@ModuleId", con);


            if (PlatformId != "")
                cmd.Parameters.AddWithValue("@RoleId", PlatformId);
            else
                cmd.Parameters.AddWithValue("@RoleId", DBNull.Value);


            if (Item != "")
                cmd.Parameters.AddWithValue("@ModuleId", Item);
            else
                cmd.Parameters.AddWithValue("@ModuleId", DBNull.Value);

            if (active != "")
                cmd.Parameters.AddWithValue("@IsActive", active);
            else
                cmd.Parameters.AddWithValue("@IsActive", 0);


            int result = cmd.ExecuteNonQuery();
            con.Close();
            return result;


        }


        public int UpdateUserValidation(Staff obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Update tbl_StaffUserValidation Set StaffId=@StaffId,IsView=@IsView,IsAdd=@IsAdd,IsEdit=@IsEdit,IsDelete=@IsDelete,IsmanageFund=@IsmanageFund,RoleId=@RoleId where (@Id Is Null Or Id=@Id)", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@StaffId", obj.Id);
                //com.Parameters.AddWithValue("@PlatformName", obj.PlatformName);

                //com.Parameters.AddWithValue("@ModuleId", obj.ModuleId);
                com.Parameters.AddWithValue("@IsView", obj.IsView);
                com.Parameters.AddWithValue("@IsAdd", obj.IsAdd);
                com.Parameters.AddWithValue("@IsEdit", obj.IsEdit);
                com.Parameters.AddWithValue("@IsDelete", obj.IsDelete);
                com.Parameters.AddWithValue("@IsmanageFund", obj.IsmanageFund);
                com.Parameters.AddWithValue("@RoleId", obj.RoleId);

                com.Parameters.AddWithValue("@Id", obj.UservalidationId);

                //com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Models
{
    public class Customer
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string SectorName { get; set; }
        public int SectorId { get; set; }
        public int BuildingId { get; set; }
        public string Photo { get; set; }
        public string base64Image { get; set; }
        public int FlatId { get; set; }
        public DateTime SubnFromDate { get; set; }
        public DateTime SubnToDate { get; set; }
        public string fcm_token { get; set; }
        public long RewardPoint { get; set; }
        public string ReferralCode { get; set; }
        public int ReferralID { get; set; }
        public Decimal Credit { get; set; }
        public int OrderBy { get; set; }
        public Decimal CashAmount { get; set; }
        public DateTime CashReqDate { get; set; }
        //assign delivery boy
        public int CustomerId { get; set; }
        public int StaffId { get; set; }

        //otp 
        public string OTP { get; set; }
        public DateTime LastUpdateOtpDate { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Count { get; set; }
        clsCommon _clsCommon = new clsCommon();
        Helper dHelper = new Helper();


        public string gpickloc { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }


        public string testsms { get; set; }


        public string ntitle { get; set; }
        public string ntext { get; set; }
        public string nlink { get; set; }
        public string nid { get; set; }
        public string CustomerType { get; set; }
        public DataTable GetAllCustomer(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SP_Customer_SelectAll", con);
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

        public DataTable GetAllCustomer1(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SP_Customer_SelectAll1", con);
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

        public DataTable BindCustomer(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Customer_SelectAll", con);
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


        public DataTable BindCustomerNew(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT cm.*,bm.BuildingName,fm.FlatNo,bm.blockno,sm.[SectorName] from [dbo].[tbl_Customer_Master] cm left join [dbo].[tbl_Building_Master] bm on bm.Id = cm.BuildingId left join [dbo].[tbl_Building_Flat_Master] fm on fm.Id = cm.FlatId left join [dbo].[tbl_Sector_Master] sm on sm.Id = cm.SectorId where (@Id IS NULL OR cm.[Id] = @Id)", con);
            cmd.CommandType = CommandType.Text;
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

        public DataTable GetCustomerBySectorDeliveryBoy(int? SectorId, int? StaffId)
        {
            if (SectorId == 0) SectorId = null;
            if (StaffId == 0) StaffId = null;
            con.Open();
            SqlCommand cmd = new SqlCommand("SP_Customer_Sector_Staff", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(StaffId.ToString()))
                cmd.Parameters.AddWithValue("@StaffID", StaffId);
            else
                cmd.Parameters.AddWithValue("@StaffID", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable CheckCustomerSortOrder(int? SectorId, int? StaffId, int? OrderBy)
        {
            //SectorId = " + SectorId + " and
            var dt = _clsCommon.showdata("select * from tbl_Customer_Master c join tbl_DeliveryBoy_Customer_Assign d on c.id = d.CustomerId where  c.OrderBy = " + OrderBy + " and d.StaffId = " + StaffId + "");
            return dt;
        }

        public DataTable CustomerSortOrderUpdateBySector(int? oldOrder, int? newOrder, int? customerId, int? SectorId, int? StaffId)
        {
            int so = 0, so1 = 0, currentsortorder, nextsortorder;
            Customer obj = new Customer();

            obj.CustomerId = Convert.ToInt32(customerId);
            obj.SectorId = Convert.ToInt32(SectorId);
            obj.StaffId = Convert.ToInt32(StaffId);
            con.Open();
            //SqlCommand cmd = new SqlCommand("SP_Customer_Sector_SortOrder_Update", con);
            //cmd.CommandType = CommandType.StoredProcedure;
            //if (!string.IsNullOrEmpty(oldOrder.ToString()))
            //    cmd.Parameters.AddWithValue("@OldSortOrder", oldOrder);
            //else
            //    cmd.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
            //if (!string.IsNullOrEmpty(newOrder.ToString()))
            //    cmd.Parameters.AddWithValue("@NewSortOrder", newOrder);
            //else
            //    cmd.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
            //if (!string.IsNullOrEmpty(customerId.ToString()))
            //    cmd.Parameters.AddWithValue("@CustomerId", customerId);
            //else
            //    cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            //if (!string.IsNullOrEmpty(SectorId.ToString()))
            //    cmd.Parameters.AddWithValue("@SectorID", SectorId);
            //else
            //    cmd.Parameters.AddWithValue("@SectorID", DBNull.Value);
            //if (!string.IsNullOrEmpty(StaffId.ToString()))
            //    cmd.Parameters.AddWithValue("@StaffId", StaffId);
            //else
            //    cmd.Parameters.AddWithValue("@StaffId", DBNull.Value);
            string aold = oldOrder.ToString();
            string anew = newOrder.ToString();
            currentsortorder = Convert.ToInt32(aold);
            nextsortorder = Convert.ToInt32(anew);
            SqlCommand cmd1, cmd, cmd2, cmd3, cmd4;


            if (currentsortorder < nextsortorder)
            {

                //OrderBy != currentsortorder;


                string j = currentsortorder.ToString();

                int k = Convert.ToInt32(j);

                if (currentsortorder == 0)
                {
                    cmd2 = new SqlCommand("UPDATE c SET OrderBy = OrderBy + 1 FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id = d.CustomerId WHERE OrderBy!=@OldSortOrder AND OrderBy>=@NewSortOrder AND c.Id != @CustomerId AND IsDeleted='False'  AND d.StaffId=@StaffId", con);


                    cmd2.CommandType = CommandType.Text;


                    if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                        cmd2.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                    else
                        cmd2.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                    if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                        cmd2.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                    else
                        cmd2.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                    if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                        cmd2.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                    else
                        cmd2.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                    //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    //    cmd2.Parameters.AddWithValue("@SectorID", obj.SectorId);
                    //else
                    //    cmd2.Parameters.AddWithValue("@SectorID", DBNull.Value);
                    if (!string.IsNullOrEmpty(StaffId.ToString()))
                        cmd2.Parameters.AddWithValue("@StaffId", StaffId);
                    else
                        cmd2.Parameters.AddWithValue("@StaffId", DBNull.Value);

                    so = cmd2.ExecuteNonQuery();


                    cmd3 = new SqlCommand("UPDATE c SET OrderBy = @NewSortOrder FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id = d.CustomerId WHERE OrderBy = @OldSortOrder AND c.Id = @CustomerId AND IsDeleted='False' AND d.StaffId=@StaffId", con);


                    cmd3.CommandType = CommandType.Text;


                    if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                        cmd3.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                    else
                        cmd3.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                    if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                        cmd3.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                    else
                        cmd3.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                    if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                        cmd3.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                    else
                        cmd3.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                    //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    //    cmd3.Parameters.AddWithValue("@SectorID", obj.SectorId);
                    //else
                    //    cmd3.Parameters.AddWithValue("@SectorID", DBNull.Value);
                    if (!string.IsNullOrEmpty(StaffId.ToString()))
                        cmd3.Parameters.AddWithValue("@StaffId", StaffId);
                    else
                        cmd3.Parameters.AddWithValue("@StaffId", DBNull.Value);
                    so1 = cmd3.ExecuteNonQuery();
                    con.Close();
                }
                else
                {

                    //AND SectorId = @SectorID
                    SqlCommand cmdnew = new SqlCommand("SELECT c.* From tbl_Customer_Master c join tbl_DeliveryBoy_Customer_Assign d on c.id = d.CustomerId where d.StaffId = @StatffId AND c.OrderBy=@i1  AND c.IsDeleted='False' ", con);
                    cmdnew.CommandType = CommandType.Text;

                    if (!string.IsNullOrEmpty(k.ToString()))
                        cmdnew.Parameters.AddWithValue("@i1", k);
                    else
                        cmdnew.Parameters.AddWithValue("@i1", DBNull.Value);


                    if (!string.IsNullOrEmpty(obj.StaffId.ToString()))
                        cmdnew.Parameters.AddWithValue("@StatffId", obj.StaffId);
                    else
                        cmdnew.Parameters.AddWithValue("@StatffId", DBNull.Value);
                    SqlDataAdapter da = new SqlDataAdapter(cmdnew);
                    DataTable dt1 = new DataTable();
                    da.Fill(dt1);
                    if (dt1.Rows.Count > 1)
                    {

                        cmd2 = new SqlCommand("UPDATE c SET OrderBy = OrderBy + 1 FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id != d.CustomerId WHERE OrderBy!=@OldSortOrder AND OrderBy>=@NewSortOrder AND c.Id != @CustomerId AND IsDeleted='False'  AND d.StaffId=@StaffId", con);


                        cmd2.CommandType = CommandType.Text;


                        if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                            cmd2.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                        else
                            cmd2.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                        if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                            cmd2.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                        else
                            cmd2.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                        if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                            cmd2.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                        else
                            cmd2.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                        //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                        //    cmd2.Parameters.AddWithValue("@SectorID", obj.SectorId);
                        //else
                        //    cmd2.Parameters.AddWithValue("@SectorID", DBNull.Value);
                        if (!string.IsNullOrEmpty(StaffId.ToString()))
                            cmd2.Parameters.AddWithValue("@StaffId", StaffId);
                        else
                            cmd2.Parameters.AddWithValue("@StaffId", DBNull.Value);

                        so = cmd2.ExecuteNonQuery();



                        cmd1 = new SqlCommand("UPDATE c SET OrderBy = @NewSortOrder FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id = d.CustomerId WHERE OrderBy = @OldSortOrder AND c.Id = @CustomerId AND IsDeleted='False'  AND d.StaffId=@StaffId", con);


                        cmd1.CommandType = CommandType.Text;


                        if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                            cmd1.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                        else
                            cmd1.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                        if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                            cmd1.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                        else
                            cmd1.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                        if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                            cmd1.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                        else
                            cmd1.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                        //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                        //    cmd1.Parameters.AddWithValue("@SectorID", obj.SectorId);
                        //else
                        //    cmd1.Parameters.AddWithValue("@SectorID", DBNull.Value);
                        if (!string.IsNullOrEmpty(StaffId.ToString()))
                            cmd1.Parameters.AddWithValue("@StaffId", StaffId);
                        else
                            cmd1.Parameters.AddWithValue("@StaffId", DBNull.Value);
                        so1 = cmd1.ExecuteNonQuery();
                        //for (int i=0;i<dt1.Rows.Count-1;i++)
                        //{

                        //}





                    }
                    else


                    {
                        for (int i = k; i <= nextsortorder; i++)
                        {
                            cmd = new SqlCommand("UPDATE c SET OrderBy = OrderBy - 1 FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id = d.CustomerId WHERE OrderBy=@i1 AND c.Id != @CustomerId AND IsDeleted='False'  AND d.StaffId=@StaffId", con);


                            cmd.CommandType = CommandType.Text;

                            if (!string.IsNullOrEmpty(i.ToString()))
                                cmd.Parameters.AddWithValue("@i1", i);
                            else
                                cmd.Parameters.AddWithValue("@i1", DBNull.Value);
                            if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                                cmd.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                            else
                                cmd.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                            if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                                cmd.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                            else
                                cmd.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                            if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                                cmd.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                            else
                                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                            //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                            //    cmd.Parameters.AddWithValue("@SectorID", obj.SectorId);
                            //else
                            //    cmd.Parameters.AddWithValue("@SectorID", DBNull.Value);
                            if (!string.IsNullOrEmpty(StaffId.ToString()))
                                cmd.Parameters.AddWithValue("@StaffId", StaffId);
                            else
                                cmd.Parameters.AddWithValue("@StaffId", DBNull.Value);

                            so = cmd.ExecuteNonQuery();




                        }

                        cmd1 = new SqlCommand("UPDATE c SET OrderBy = @NewSortOrder FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id = d.CustomerId WHERE OrderBy = @OldSortOrder AND c.Id = @CustomerId AND IsDeleted='False'  AND d.StaffId=@StaffId", con);


                        cmd1.CommandType = CommandType.Text;


                        if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                            cmd1.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                        else
                            cmd1.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                        if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                            cmd1.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                        else
                            cmd1.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                        if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                            cmd1.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                        else
                            cmd1.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                        //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                        //    cmd1.Parameters.AddWithValue("@SectorID", obj.SectorId);
                        //else
                        //    cmd1.Parameters.AddWithValue("@SectorID", DBNull.Value);
                        if (!string.IsNullOrEmpty(StaffId.ToString()))
                            cmd1.Parameters.AddWithValue("@StaffId", StaffId);
                        else
                            cmd1.Parameters.AddWithValue("@StaffId", DBNull.Value);
                        so1 = cmd1.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }


            if (currentsortorder > nextsortorder)
            {
                string j = currentsortorder.ToString();

                int k = Convert.ToInt32(j);

                if (currentsortorder >= 1000)
                {

                    cmd2 = new SqlCommand("UPDATE c SET OrderBy = OrderBy + 1 FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id != d.CustomerId WHERE OrderBy!=@OldSortOrder AND OrderBy>=@NewSortOrder AND c.Id != @CustomerId AND IsDeleted='False'  AND d.StaffId=@StaffId", con);


                    cmd2.CommandType = CommandType.Text;


                    if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                        cmd2.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                    else
                        cmd2.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                    if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                        cmd2.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                    else
                        cmd2.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                    if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                        cmd2.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                    else
                        cmd2.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                    //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    //    cmd2.Parameters.AddWithValue("@SectorID", obj.SectorId);
                    //else
                    //    cmd2.Parameters.AddWithValue("@SectorID", DBNull.Value);
                    if (!string.IsNullOrEmpty(StaffId.ToString()))
                        cmd2.Parameters.AddWithValue("@StaffId", StaffId);
                    else
                        cmd2.Parameters.AddWithValue("@StaffId", DBNull.Value);

                    so = cmd2.ExecuteNonQuery();




                    cmd1 = new SqlCommand("UPDATE c SET OrderBy = @NewSortOrder FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id = d.CustomerId WHERE OrderBy = @OldSortOrder AND c.Id = @CustomerId AND IsDeleted='False'  AND d.StaffId=@StaffId", con);


                    cmd1.CommandType = CommandType.Text;


                    if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                        cmd1.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                    else
                        cmd1.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                    if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                        cmd1.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                    else
                        cmd1.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                    if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                        cmd1.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                    else
                        cmd1.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                    //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    //    cmd1.Parameters.AddWithValue("@SectorID", obj.SectorId);
                    //else
                    //    cmd1.Parameters.AddWithValue("@SectorID", DBNull.Value);
                    if (!string.IsNullOrEmpty(StaffId.ToString()))
                        cmd1.Parameters.AddWithValue("@StaffId", StaffId);
                    else
                        cmd1.Parameters.AddWithValue("@StaffId", DBNull.Value);
                    so1 = cmd1.ExecuteNonQuery();
                }

                else
                {

                    SqlCommand cmdnew1 = new SqlCommand("SELECT c.* From tbl_Customer_Master c join tbl_DeliveryBoy_Customer_Assign d on c.id = d.CustomerId where d.StaffId = @StatffId AND c.OrderBy=@i1  AND c.IsDeleted='False' ", con);
                    cmdnew1.CommandType = CommandType.Text;

                    if (!string.IsNullOrEmpty(k.ToString()))
                        cmdnew1.Parameters.AddWithValue("@i1", k - 1);
                    else
                        cmdnew1.Parameters.AddWithValue("@i1", DBNull.Value);


                    if (!string.IsNullOrEmpty(obj.StaffId.ToString()))
                        cmdnew1.Parameters.AddWithValue("@StatffId", obj.StaffId);
                    else
                        cmdnew1.Parameters.AddWithValue("@StatffId", DBNull.Value);
                    SqlDataAdapter da1 = new SqlDataAdapter(cmdnew1);
                    DataTable dt2 = new DataTable();
                    da1.Fill(dt2);

                    if (dt2.Rows.Count > 0)
                    {
                        SqlCommand cmdnew = new SqlCommand("SELECT c.* From tbl_Customer_Master c join tbl_DeliveryBoy_Customer_Assign d on c.id = d.CustomerId where c.OrderBy=@i1 AND d.StaffId=@StatffId  AND c.IsDeleted='False'", con);
                        cmdnew.CommandType = CommandType.Text;

                        if (!string.IsNullOrEmpty(k.ToString()))
                            cmdnew.Parameters.AddWithValue("@i1", k);
                        else
                            cmdnew.Parameters.AddWithValue("@i1", DBNull.Value);
                        //AND d.StaffId = @StatffId

                        if (!string.IsNullOrEmpty(obj.StaffId.ToString()))
                            cmdnew.Parameters.AddWithValue("@StatffId", obj.StaffId);
                        else
                            cmdnew.Parameters.AddWithValue("@StatffId", DBNull.Value);
                        SqlDataAdapter da = new SqlDataAdapter(cmdnew);
                        DataTable dt1 = new DataTable();
                        da.Fill(dt1);
                        if (dt1.Rows.Count > 1 && currentsortorder < 1000)
                        {




                            cmd1 = new SqlCommand("UPDATE c SET OrderBy = @NewSortOrder FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id = d.CustomerId WHERE OrderBy = @OldSortOrder AND c.Id = @CustomerId AND IsDeleted='False'  AND d.StaffId=@StaffId", con);


                            cmd1.CommandType = CommandType.Text;


                            if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                                cmd1.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                            else
                                cmd1.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                            if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                                cmd1.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                            else
                                cmd1.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                            if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                                cmd1.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                            else
                                cmd1.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                            //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                            //    cmd1.Parameters.AddWithValue("@SectorID", obj.SectorId);
                            //else
                            //    cmd1.Parameters.AddWithValue("@SectorID", DBNull.Value);
                            if (!string.IsNullOrEmpty(StaffId.ToString()))
                                cmd1.Parameters.AddWithValue("@StaffId", StaffId);
                            else
                                cmd1.Parameters.AddWithValue("@StaffId", DBNull.Value);
                            so1 = cmd1.ExecuteNonQuery();
                            //for (int i=0;i<dt1.Rows.Count-1;i++)
                            //{

                            //}





                        }
                        else

                        {
                            for (int i = k; i >= nextsortorder; i--)
                            {
                                cmd = new SqlCommand("UPDATE c SET OrderBy = OrderBy + 1 FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id = d.CustomerId WHERE OrderBy=@i1 AND c.Id != @CustomerId AND IsDeleted='False'  AND d.StaffId=@StaffId", con);
                                //AND SectorId = @SectorID

                                cmd.CommandType = CommandType.Text;

                                if (!string.IsNullOrEmpty(i.ToString()))
                                    cmd.Parameters.AddWithValue("@i1", i);
                                else
                                    cmd.Parameters.AddWithValue("@i1", DBNull.Value);
                                if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                                    cmd.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                                else
                                    cmd.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                                if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                                    cmd.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                                else
                                    cmd.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                                if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                                    cmd.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                                else
                                    cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                                //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                                //    cmd.Parameters.AddWithValue("@SectorID", obj.SectorId);
                                //else
                                //    cmd.Parameters.AddWithValue("@SectorID", DBNull.Value);
                                if (!string.IsNullOrEmpty(StaffId.ToString()))
                                    cmd.Parameters.AddWithValue("@StaffId", StaffId);
                                else
                                    cmd.Parameters.AddWithValue("@StaffId", DBNull.Value);

                                so = cmd.ExecuteNonQuery();




                            }


                            cmd1 = new SqlCommand("UPDATE c SET OrderBy = @NewSortOrder FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id = d.CustomerId WHERE OrderBy = @OldSortOrder AND c.Id = @CustomerId AND IsDeleted='False'  AND d.StaffId=@StaffId", con);


                            cmd1.CommandType = CommandType.Text;


                            if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                                cmd1.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                            else
                                cmd1.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                            if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                                cmd1.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                            else
                                cmd1.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                            if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                                cmd1.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                            else
                                cmd1.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                            //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                            //    cmd1.Parameters.AddWithValue("@SectorID", obj.SectorId);
                            //else
                            //    cmd1.Parameters.AddWithValue("@SectorID", DBNull.Value);
                            if (!string.IsNullOrEmpty(StaffId.ToString()))
                                cmd1.Parameters.AddWithValue("@StaffId", StaffId);
                            else
                                cmd1.Parameters.AddWithValue("@StaffId", DBNull.Value);
                            so1 = cmd1.ExecuteNonQuery();

                            con.Close();
                        }
                    }


                    else
                    {


                        cmd = new SqlCommand("UPDATE c SET OrderBy = OrderBy -1 FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id = d.CustomerId WHERE OrderBy!=@OldSortOrder AND OrderBy>=@NewSortOrder AND c.Id != @CustomerId AND IsDeleted='False'  AND d.StaffId=@StaffId", con);


                        cmd.CommandType = CommandType.Text;


                        if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                            cmd.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                        else
                            cmd.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                        if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                            cmd.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                        else
                            cmd.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                        if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                            cmd.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                        else
                            cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                        //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                        //    cmd.Parameters.AddWithValue("@SectorID", obj.SectorId);
                        //else
                        //    cmd.Parameters.AddWithValue("@SectorID", DBNull.Value);
                        if (!string.IsNullOrEmpty(StaffId.ToString()))
                            cmd.Parameters.AddWithValue("@StaffId", StaffId);
                        else
                            cmd.Parameters.AddWithValue("@StaffId", DBNull.Value);

                        so = cmd.ExecuteNonQuery();




                        cmd1 = new SqlCommand("UPDATE c SET OrderBy = @NewSortOrder FROM tbl_Customer_Master c JOIN tbl_DeliveryBoy_Customer_Assign d ON c.Id = d.CustomerId WHERE OrderBy = @OldSortOrder AND c.Id = @CustomerId AND IsDeleted='False'  AND d.StaffId=@StaffId", con);


                        cmd1.CommandType = CommandType.Text;


                        if (!string.IsNullOrEmpty(currentsortorder.ToString()))
                            cmd1.Parameters.AddWithValue("@OldSortOrder", currentsortorder);
                        else
                            cmd1.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
                        if (!string.IsNullOrEmpty(nextsortorder.ToString()))
                            cmd1.Parameters.AddWithValue("@NewSortOrder", nextsortorder);
                        else
                            cmd1.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
                        if (!string.IsNullOrEmpty(obj.CustomerId.ToString()))
                            cmd1.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                        else
                            cmd1.Parameters.AddWithValue("@CustomerId", DBNull.Value);
                        //if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                        //    cmd1.Parameters.AddWithValue("@SectorID", obj.SectorId);
                        //else
                        //    cmd1.Parameters.AddWithValue("@SectorID", DBNull.Value);
                        if (!string.IsNullOrEmpty(StaffId.ToString()))
                            cmd1.Parameters.AddWithValue("@StaffId", StaffId);
                        else
                            cmd1.Parameters.AddWithValue("@StaffId", DBNull.Value);
                        so1 = cmd1.ExecuteNonQuery();
                    }
                }

        }


            //return so;
            //SectorId = " + SectorId + " and
            var dt = _clsCommon.showdata("select * from tbl_Customer_Master c join tbl_DeliveryBoy_Customer_Assign d on c.id = d.CustomerId where  c.OrderBy = " + OrderBy + " and d.StaffId = " + StaffId + "");
            con.Close();
            return dt;

            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataTable dt = new DataTable();
            //da.Fill(dt);
            //con.Close();
            //return dt;
        }

        public DataTable CustomerSortOrderInsert(int? customerId, int? SectorId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SP_CustomerSortOrderInsert", con);
            cmd.CommandType = CommandType.StoredProcedure;

            if (!string.IsNullOrEmpty(customerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorID", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorID", DBNull.Value);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable CustomerSortOrderUpdate(int? oldOrder, int? newOrder, int? customerId, int? SectorId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SP_CustomerSortOrderUpdate", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(oldOrder.ToString()))
                cmd.Parameters.AddWithValue("@OldSortOrder", oldOrder);
            else
                cmd.Parameters.AddWithValue("@OldSortOrder", DBNull.Value);
            if (!string.IsNullOrEmpty(newOrder.ToString()))
                cmd.Parameters.AddWithValue("@NewSortOrder", newOrder);
            else
                cmd.Parameters.AddWithValue("@NewSortOrder", DBNull.Value);
            if (!string.IsNullOrEmpty(customerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorID", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorID", DBNull.Value);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable CustomerSortOrderDelete(int? customerId, int? SectorId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SP_CustomerSortOrderDelete", con);
            cmd.CommandType = CommandType.StoredProcedure;

            if (!string.IsNullOrEmpty(customerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorID", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorID", DBNull.Value);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
        //bind
        //bind customer whom purchase subscription
        public DataTable BindCustomerPurchaseSubn(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Customer_Purchase_Subscription_SelectAll", con);
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

        public DataTable BindsubDateCustomer(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Customer_SubDate_SelectAll", con);
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
        public DataTable BindsubDateAllSubsnCustomer(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Customer_SubDate_Subn_SelectAll", con);
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

        public int InsertCustomer(Customer obj)
        {
            int i = 0;
            //try
            //{
                con.Open();
                SqlCommand com = new SqlCommand("SP_Customer_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
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
                com.Parameters.AddWithValue("@ReferralCode", obj.ReferralCode);
                com.Parameters.AddWithValue("@UserName", obj.MobileNo);
                com.Parameters.AddWithValue("@Password", obj.Password);
                com.Parameters.AddWithValue("@SectorId", obj.SectorId);
            com.Parameters.AddWithValue("@ReferralID", obj.ReferralID);
            //com.Parameters.AddWithValue("@FlatId", obj.FlatId);
            if (!string.IsNullOrEmpty(obj.Photo))
                    com.Parameters.AddWithValue("@Photo", obj.Photo);
                else
                    com.Parameters.AddWithValue("@Photo", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.fcm_token))
                    com.Parameters.AddWithValue("@fcm_token", obj.fcm_token);
                else
                    com.Parameters.AddWithValue("@fcm_token", DBNull.Value);

                com.Parameters.AddWithValue("@CreatedOn", Helper.indianTime);


             

                if (!string.IsNullOrEmpty(obj.gpickloc))
                    com.Parameters.AddWithValue("@gpickloc", obj.gpickloc);
                else
                    com.Parameters.AddWithValue("@gpickloc", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.lat))
                    com.Parameters.AddWithValue("@lat", obj.lat);
                else
                    com.Parameters.AddWithValue("@lat", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.lon))
                    com.Parameters.AddWithValue("@lon", obj.lon);
                else
                    com.Parameters.AddWithValue("@lon", DBNull.Value);

                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                //object scalarId = com.ExecuteScalar();
                //if (scalarId != null)
                //{
                //    i = Convert.ToInt32(scalarId);
                //}
                con.Close();
            //}
            //catch (Exception ex)
            //{ }
            return i;

        }

        public int UpdateCustomer(Customer obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Customer_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.Id);
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
                com.Parameters.AddWithValue("@UserName", obj.MobileNo);
                com.Parameters.AddWithValue("@Password", obj.Password);
                com.Parameters.AddWithValue("@BuildingId", obj.BuildingId);
                com.Parameters.AddWithValue("@FlatId", obj.FlatId);
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

        public int MobileUpdateCustomer(Customer obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Customer_Update_mobile", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.Id);
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



                com.Parameters.AddWithValue("@UserName", obj.MobileNo);




               
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int UpdateCustomerFromToDate(Customer obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("update tbl_Customer_Master set SubnFromDate=@SubnFromDate,SubnToDate=@SubnToDate where Id=@Id", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", obj.Id);
                if (!string.IsNullOrEmpty(obj.SubnFromDate.ToString()))
                    com.Parameters.AddWithValue("@SubnFromDate", obj.SubnFromDate);
                else
                    com.Parameters.AddWithValue("@SubnFromDate", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.SubnToDate.ToString()))
                    com.Parameters.AddWithValue("@SubnToDate", obj.SubnToDate);
                else
                    com.Parameters.AddWithValue("@SubnToDate", DBNull.Value);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int UpdateCustomerToDate(Customer obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("update tbl_Customer_Master set SubnToDate=@SubnToDate where Id=@Id", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", obj.Id);
                if (!string.IsNullOrEmpty(obj.SubnToDate.ToString()))
                    com.Parameters.AddWithValue("@SubnToDate", obj.SubnToDate);
                else
                    com.Parameters.AddWithValue("@SubnToDate", DBNull.Value);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int UpdateCustomerPoint(int CustomerId, Int64 RewardPoint)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("update tbl_Customer_Master set RewardPoint=@RewardPoint where Id=@Id", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", CustomerId);
                if (!string.IsNullOrEmpty(RewardPoint.ToString()))
                    com.Parameters.AddWithValue("@RewardPoint", RewardPoint);
                else
                    com.Parameters.AddWithValue("@RewardPoint", DBNull.Value);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        //update pwd
        public int UpdateCustomerPwd(int Id, string Password)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Customer_Password_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", Id);
                com.Parameters.AddWithValue("@Password", Password);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        //delete
        public int DeleteCustomer(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Customer_Master where Id=" + id, con);
            //SqlCommand cmd = new SqlCommand("update tbl_Customer_Master Set IsDeleted='True' where id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int DeleteCustomerFavourite(int customerId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_Customer_Favourite where CustomerId=" + customerId, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int InsertStaffCustAssign(Customer obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Staff_Customer_Assign_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@StaffId", obj.StaffId);
                com.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();

                SqlCommand cmd1 = new SqlCommand("UPDATE tbl_Customer_Master set OrderBy=0 where ID=@CustomerId", con);
                cmd1.CommandType = CommandType.Text;
               // cmd1.Parameters.AddWithValue("@StaffId", obj.StaffId);
                cmd1.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                //cmd1.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = cmd1.ExecuteNonQuery();

                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int UpdateStaffCustAssign(Customer obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("Staff_Customer_Assign_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", obj.Id);
                com.Parameters.AddWithValue("@StaffId", obj.StaffId);
                com.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        //delete
        public int DeleteStaffCustAssign(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_DeliveryBoy_Customer_Assign where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public DataTable StaffCustomerList(int? Id)
        {
            con.Open();
            string query = "  SELECT scm.*,Concat(sm.FirstName,' ',sm.LastName) as StaffName,Concat(cm.FirstName,' ',cm.LastName) as Customer";

            query += " from[dbo].[tbl_DeliveryBoy_Customer_Assign] scm";
            query += " left join[dbo].[tbl_Staff_Master] sm on sm.Id = scm.StaffId";

            query += " left join[dbo].[tbl_Customer_Master] cm on cm.Id = scm.CustomerId";

            query += " where(@Id IS NULL OR scm.[Id] = @Id)";



            //SqlCommand cmd = new SqlCommand("Staff_Customer_Assign_SelectAll", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
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

        public DataTable DuplicateStaffCustomer(int? StaffId, int? CustomerId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Check_Staff_Customer_Assign", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(StaffId.ToString()))
                cmd.Parameters.AddWithValue("@StaffId", StaffId);
            else
                cmd.Parameters.AddWithValue("@StaffId", DBNull.Value);
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        //check customer name duplicate
        public DataTable CheckCustomerUserName(string UserName)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Check_Customer_UserName", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(UserName))
                cmd.Parameters.AddWithValue("@UserName", UserName);
            else
                cmd.Parameters.AddWithValue("@UserName", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public DataTable CheckCustomerMobile(string Mobile)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Check_Customer_MobileNo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Mobile))
                cmd.Parameters.AddWithValue("@MobileNo", Mobile);
            else
                cmd.Parameters.AddWithValue("@MobileNo", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable CheckCustomerFlatNo(int flatno)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Check_Customer_FlatNo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(flatno.ToString()))
                cmd.Parameters.AddWithValue("@FlatNoID", flatno);
            else
                cmd.Parameters.AddWithValue("@FlatNoID", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable Customerlogin_Old(string UserName, string Password)
        {
            con.Open();
            SqlCommand cmdLoginUser = new SqlCommand("select tbl_Customer_Master.*,tbl_Building_Master.Id As BID,tbl_Building_Master.BuildingName,tbl_Building_Flat_Master.FlatNo, " +
                " tbl_Sector_Master.SectorName from tbl_Customer_Master left join tbl_Building_Master on tbl_Building_Master.ID = tbl_Customer_Master.BuildingId " +
                " left join tbl_Sector_Master on tbl_Sector_Master.ID = tbl_Building_Master.SectorId  left join [dbo].[tbl_Building_Flat_Master] on tbl_Building_Flat_Master.ID = tbl_Customer_Master.FlatId  " +
                " WHERE UserName='" + UserName + "' and Password='" + Password + "'", con);
            // cmdLoginUser.Parameters.AddWithValue("@UserName", UserName);
            // cmdLoginUser.Parameters.AddWithValue("@Password", Password);
            SqlDataAdapter daLoginUser = new SqlDataAdapter(cmdLoginUser);
            DataTable dtLoginUser = new DataTable();
            daLoginUser.Fill(dtLoginUser);
            con.Close();
            return dtLoginUser;
        }

        public DataTable Customerlogin(string UserName, string Password)
        {
            con.Open();
            SqlCommand cmdLoginUser = new SqlCommand("select * from tbl_Customer_Master WHERE UserName='" + UserName + "' and Password='" + Password + "'", con);
            SqlDataAdapter daLoginUser = new SqlDataAdapter(cmdLoginUser);
            DataTable dtLoginUser = new DataTable();
            daLoginUser.Fill(dtLoginUser);
            con.Close();
            return dtLoginUser;
        }


        public DataTable Customerlogin1(string UserName)
        {
            con.Open();
            SqlCommand cmdLoginUser = new SqlCommand("select * from tbl_Customer_Master WHERE UserName='" + UserName + "'", con);
            SqlDataAdapter daLoginUser = new SqlDataAdapter(cmdLoginUser);
            DataTable dtLoginUser = new DataTable();
            daLoginUser.Fill(dtLoginUser);
            con.Close();
            return dtLoginUser;
        }



        public DataTable Customerlogin2(int CustomerId, string Password)
        {
            con.Open();
            SqlCommand cmdLoginUser = new SqlCommand("select * from tbl_Customer_Master WHERE Id=" + CustomerId + " and Password='" + Password + "'", con);
            SqlDataAdapter daLoginUser = new SqlDataAdapter(cmdLoginUser);
            DataTable dtLoginUser = new DataTable();
            daLoginUser.Fill(dtLoginUser);
            con.Close();
            return dtLoginUser;
        }
        public DataTable NewCustomer(string UserName, string Password, string Mobile)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SP_Check_Customer", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(UserName))
                cmd.Parameters.AddWithValue("@UserName", UserName);
            else
                cmd.Parameters.AddWithValue("@UserName", DBNull.Value);
            if (!string.IsNullOrEmpty(Password))
                cmd.Parameters.AddWithValue("@Password", Password);
            else
                cmd.Parameters.AddWithValue("@Password", DBNull.Value);
            if (!string.IsNullOrEmpty(Mobile))
                cmd.Parameters.AddWithValue("@Mobile", Mobile);
            else
                cmd.Parameters.AddWithValue("@Mobile", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable CheckDuplicateCustomer(string fname, string lname, string mobile)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Customer_Master where FirstName='" + fname + "' and LastName='" + lname + "' and MobileNo='" + mobile + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable CheckPwdFromUserName(string mobile)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbl_Customer_Master where UserName='" + mobile + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public DataTable getCustomerList(int? SectorId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Sector_Customer_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable getCustomerSector(int? CustomerId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SElect SectorId,UserName from tbl_Customer_Master where Id=@CustomerId", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
        //otp
        public DataTable getOtpCustomerList(string Mobile)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("OTP_SelectAll", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Mobile))
                cmd.Parameters.AddWithValue("@MobileNo", Mobile);
            else
                cmd.Parameters.AddWithValue("@MobileNo", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public int CustomerInsertOtp(string otp, string MobileNo)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("OTP_Insert", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@MobileNo", MobileNo);
                if (!string.IsNullOrEmpty(otp))
                    com.Parameters.AddWithValue("@OTP", otp);
                else
                    com.Parameters.AddWithValue("@OTP", DBNull.Value);
                com.Parameters.AddWithValue("@LastUpdateOtpDate", DateTime.Now);
                com.Parameters.AddWithValue("@Count", 0);
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int CustomerupdateOtp(string otp, string MobileNo, int count)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("OTP_Update", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@MobileNo", MobileNo);
                if (!string.IsNullOrEmpty(otp))
                    com.Parameters.AddWithValue("@OTP", otp);
                else
                    com.Parameters.AddWithValue("@OTP", DBNull.Value);
                com.Parameters.AddWithValue("@LastUpdateOtpDate", DateTime.Now);
                com.Parameters.AddWithValue("@Count", count);
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public DataTable CheckCustomerOtp(string Mobile, string OTP)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Chcek_Mobile_OTP_Select", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(Mobile))
                cmd.Parameters.AddWithValue("@MobileNo", Mobile);
            else
                cmd.Parameters.AddWithValue("@MobileNo", DBNull.Value);
            if (!string.IsNullOrEmpty(OTP))
                cmd.Parameters.AddWithValue("@OTP", OTP);
            else
                cmd.Parameters.AddWithValue("@OTP", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable GetCustomerwiseOrder(string customerId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("CustomerWise_Order_List", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(customerId))
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable getDeviceInstanceId(Int64 userid)
        {
            SqlDataAdapter da = new SqlDataAdapter(@"select fcm_token from tbl_Customer_Master where id=" + userid, con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable CheckCustomerMobile(Int64 userid, string Mobile)
        {
            SqlDataAdapter da = new SqlDataAdapter(@"select * from tbl_Customer_Master where id='" + userid + "' and MobileNo='" + Mobile + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        //update profile photo
        public int UpdateCustomerPhoto(Customer obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("update tbl_Customer_Master set Photo=@Photo where Id=@Id", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", obj.Id);
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

        //for latest app version
        public DataTable GetAppVersion(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("App_Version_SelectAll", con);
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

        //for latest cut off time
        public DataTable GetSchedularTime(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select * from tbl_Setting where Id=1", con);
            cmd.CommandType = CommandType.Text;
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

        public DashboadModel getDashboard()
        {
            string todayDate = Helper.indianTime.ToString("yyyy-MM-dd");
            string yesterdayDate = Helper.indianTime.AddDays(-1).ToString("yyyy-MM-dd");
            DashboadModel dashboard = new DashboadModel();
            var customer = _clsCommon.Get_Entity("SELECT count(*) FROM tbl_Customer_Master");
            dashboard.TotalCustomer = customer == "" ? "0" : customer;

            //var activeCust = _clsCommon.Get_Entity("SELECT count(c.id) FROM tbl_Customer_Master c join tbl_Customer_Subscription cs on c.id = cs.CustomerId where Getdate() between cs.FromDate AND cs.ToDate");
            //dashboard.ActiveCustomer = activeCust == "" ? "0" : activeCust;



          //  DataTable dtactcus = new DataTable();

            //var activeCust = _clsCommon.Get_Entity("SELECT Distinct ot.CustomerId FROM tbl_Customer_Order_Transaction ot inner join tbl_Customer_Master c on c.id = ot.CustomerId where (convert(varchar,ot.Orderdate,23) >= CONVERT(date, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'))) and ot.Status='Pending'");
            //dashboard.ActiveCustomer = activeCust.

            SqlCommand cmd = new SqlCommand("SELECT Distinct ot.CustomerId FROM tbl_Customer_Order_Transaction ot inner join tbl_Customer_Master c on c.id = ot.CustomerId where (convert(varchar,ot.Orderdate,23) >= CONVERT(date, SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'))) and ot.Status='Pending'", con);
            cmd.CommandType = CommandType.Text;
           
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dashboard.ActiveCustomer = dt.Rows.Count.ToString();

           var todayOrder = _clsCommon.Get_Entity(@"SELECT SUM(TotalAmount) from tbl_Customer_Order_Transaction Where Status='Complete' AND convert(varchar,OrderDate,23)='" + todayDate + "'");
            dashboard.TodayOrder = todayOrder == "" ? "0" : todayOrder;

            var yesterdayOrder = _clsCommon.Get_Entity(@"SELECT SUM(TotalAmount) from tbl_Customer_Order_Transaction Where Status='Complete' AND convert(varchar,OrderDate,23)='" + yesterdayDate + "'");
            dashboard.YesterdayOrder = yesterdayOrder == "" ? "0" : yesterdayOrder;


            //Pending Order
            cmd = new SqlCommand("SELECT Distinct ot.Id FROM tbl_Customer_Order_Transaction ot inner join tbl_Customer_Order_Detail c on c.OrderId = ot.Id where (convert(varchar,ot.Orderdate,23)='" + todayDate + "') and ot.Status='Pending'", con);
            cmd.CommandType = CommandType.Text;

            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            dashboard.TodayPendingOrder = dt.Rows.Count.ToString();
            //

            //

            cmd = new SqlCommand("SELECT * FROM tbl_Customer_Mobile_Recharge", con);
            cmd.CommandType = CommandType.Text;

            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            dashboard.Recentrecharge = dt.Rows.Count.ToString();

            cmd = new SqlCommand("SELECT * FROM tbl_Customer_Mobile_Recharge where Status='1'", con);
            cmd.CommandType = CommandType.Text;

            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            dashboard.Recentrecharges = dt.Rows.Count.ToString();

            cmd = new SqlCommand("SELECT * FROM tbl_Customer_Mobile_Recharge where Status='0'", con);
            cmd.CommandType = CommandType.Text;

            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            dashboard.Recentrechargep = dt.Rows.Count.ToString();
            cmd = new SqlCommand("SELECT * FROM tbl_Customer_Mobile_Recharge where Status<>'1' AND Status<>'0'", con);
            cmd.CommandType = CommandType.Text;

            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            dashboard.Recentrechargef = dt.Rows.Count.ToString();


            cmd = new SqlCommand("SELECT * FROM tbl_Customer_bill_Pay", con);
            cmd.CommandType = CommandType.Text;

            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            dashboard.RecentBillPay = dt.Rows.Count.ToString();

            cmd = new SqlCommand("SELECT * FROM tbl_Customer_bill_Pay where Status='1'", con);
            cmd.CommandType = CommandType.Text;

            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            dashboard.RecentBillPays = dt.Rows.Count.ToString();

            cmd = new SqlCommand("SELECT * FROM tbl_Customer_bill_Pay where Status='0'", con);
            cmd.CommandType = CommandType.Text;

            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            dashboard.RecentBillPayp = dt.Rows.Count.ToString();

            cmd = new SqlCommand("SELECT * FROM tbl_Customer_bill_Pay where Status<>'1' AND Status<>'0'", con);
            cmd.CommandType = CommandType.Text;

            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            dashboard.RecentBillPayf = dt.Rows.Count.ToString();
            //

            return dashboard;
        }

        public DataTable CheckCustomerFavourite(int? CustomerID, int? ProductID)
        {
            var favourite = _clsCommon.selectwhere("*", "tbl_Customer_Favourite", "ProductId='" + ProductID + "' and CustomerId='" + CustomerID + "'");
            return favourite;
        }




        public int InsertCustomerOrder(Customer obj,string OrderId,DateTime OrderDate1,string orderfrom1)
        {
            int i = 0;
            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();
                SqlCommand com = new SqlCommand("Insert Into tbl_flip_amz_Order([CustomerId],[FirstName],[LastName],[MobileNo],[SectorId],[SectorName],[OrderId],[OrderDate],[OrderFrom])Values(@CustomerId,@FirstName,@LastName,@MobileNo,@SectorId,@SectorName,@OrderId,@OrderDate,@OrderFrom)", con);
                com.CommandType = CommandType.Text;

               // @CustomerId,@FirstName,@LastName,@MobileNo,@SectorId,@SectorName,@OrderId,@OrderDate,@OrderFrom


                com.Parameters.AddWithValue("@CustomerId", obj.CustomerId);

                if (!string.IsNullOrEmpty(obj.FirstName.ToString()))
                    com.Parameters.AddWithValue("@FirstName", obj.FirstName);
                else
                    com.Parameters.AddWithValue("@FirstName", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.LastName.ToString()))
                    com.Parameters.AddWithValue("@LastName", obj.LastName);
                else
                    com.Parameters.AddWithValue("@LastName", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.MobileNo.ToString()))
                    com.Parameters.AddWithValue("@MobileNo", obj.MobileNo);
                else
                    com.Parameters.AddWithValue("@MobileNo", DBNull.Value);

                if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    com.Parameters.AddWithValue("@SectorId", obj.SectorId);
                else
                    com.Parameters.AddWithValue("@SectorId", 0);

                if (!string.IsNullOrEmpty(obj.SectorName.ToString()))
                    com.Parameters.AddWithValue("@SectorName", obj.SectorName);
                else
                    com.Parameters.AddWithValue("@SectorName", DBNull.Value);
                if (!string.IsNullOrEmpty(OrderId.ToString()))
                    com.Parameters.AddWithValue("@OrderId", OrderId);
                else
                    com.Parameters.AddWithValue("@OrderId", 0);
                if (!string.IsNullOrEmpty(OrderDate1.ToString()))
                    com.Parameters.AddWithValue("@OrderDate", OrderDate1);
                else
                    com.Parameters.AddWithValue("@OrderDate", DBNull.Value);

                if (!string.IsNullOrEmpty(orderfrom1.ToString()))
                    com.Parameters.AddWithValue("@OrderFrom", orderfrom1);
                else
                    com.Parameters.AddWithValue("@OrderFrom", DBNull.Value);
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public DataTable getCustomerFlpAmzOrderViewparticular(Customer obj, string OrderId, DateTime OrderDate1, string orderfrom1)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select * from [tbl_flip_amz_Order] where CustomerId=@CustomerId AND OrderId=@OrderId AND OrderFrom=@OrderFrom ", con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@CustomerId", obj.CustomerId);
            cmd.Parameters.AddWithValue("@OrderId", OrderId);
            cmd.Parameters.AddWithValue("@OrderFrom", orderfrom1);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable getCustomerFlpAmzOrderView()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select * from [tbl_flip_amz_Order] ", con);
            cmd.CommandType = CommandType.Text;
           
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }



        public DataTable getMobileRecharge( int CustomerId,string type)
        {
            con.Open();
            DataTable dt = new DataTable();
           
            if (type=="Mobile" || type=="DTH")
            {
                SqlCommand cmd = new SqlCommand("Select cmr.*,bo.Name from [tbl_Customer_Mobile_Recharge] cmr Inner join tblBillPayOperators bo ON bo.OperatorCode=cmr.OpeartorId where cmr.CustomerId=" + CustomerId + " AND  cmr.rechargetype='"+type+"' Order By cmr.Id DESC", con);
                cmd.CommandType = CommandType.Text;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            //if (type == "Electricity" || type == "Gas")
            else
            {
                string query = "Select bp.CustomerId AS CustomerId,bp.numberCustomer AS Rechargeno,bp.OperatorId AS OpeartorId,bp.Amount AS RechargeAmount,";
                query += "bp.billdate AS rechargedate,bo.Name,bpp.CircleID,bpp.IsPartial AS IsPartial,bc.CircleCode As CircleCode,bp.type As Type,bp.Lat As Lat,bp.Lon As Long,bp.AgentId As AgentId,";
                query += "bp.paymentmode As PaymentMode,bp.customermobile As CustomerMobile,bp.field1 As Fieldtag1,bp.field2 As Fieldtag2,bp.field3 As Fieldtag3,bp.billstatus As RechargeStatus ";
                query += "from [tbl_Customer_bill_Pay] bp ";
                query += "Inner join tblBillPayOperators bo ON bo.OperatorCode=bp.OperatorId ";
                query += "inner join tblBillPayProviders bpp ON bpp.OperatorCode=bo.OperatorCode ";
                query += "inner join tblBillPayCircles bc ON bpp.CircleID=bc.ID ";
                query += "where bp.CustomerId=" + CustomerId + " AND  bp.type='" + type + "' Order By bp.Id DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }


                con.Close();
            return dt;
        }


        public DataTable getCustomerCashback(int CustomerId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select * From tbl_Customer_Wallet where CustomerId=" + CustomerId + " AND status='Cashback' Order By Id ASC", con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public DataTable getCustomerCashbackSeen(int CustomerId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select * From tbl_Customer_Wallet where CustomerId=" + CustomerId + " AND status='Cashback' AND (CashbackSeen is null or CashbackSeen<>'1') Order By Id ASC", con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }







        public int UpdateCashbackseen(int customerid,int Id)
        {
            int i = 0;
            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();
                SqlCommand com = new SqlCommand("UPDATE tbl_Customer_Wallet set CashbackSeen='1' where Id="+Id+ " And CustomerId="+customerid+"", con);
                com.CommandType = CommandType.Text;

                // @CustomerId,@FirstName,@LastName,@MobileNo,@SectorId,@SectorName,@OrderId,@OrderDate,@OrderFrom


                
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }



        public DataTable GetVisitor()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select * From tbl_Total_Visitor", con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable cashdupl(int? Customerid,DateTime? reqdate)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select * From tbl_Customer_Cash_Request where (@FromDate IS NULL  OR CONVERT(VARCHAR,CashRequestDate,23) BETWEEN @FromDate AND @FromDate) AND CustomerId=@CustomerId", con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@CustomerId", Customerid);
           
            if (!string.IsNullOrEmpty(reqdate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", reqdate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public int InsertCashRequest(Customer obj)
        {
            int i = 0;
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("INSERT INTO tbl_Customer_Cash_Request([CustomerId],[CashAmount],[CashRequestDate])VALUES(@CustomerId,@CashAmount,@CashReqDate)", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@CustomerId", obj.Id);
                if (!string.IsNullOrEmpty(obj.CashAmount.ToString()))
                    com.Parameters.AddWithValue("@CashAmount", obj.CashAmount);
                else
                    com.Parameters.AddWithValue("@CashAmount", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.CashReqDate.ToString()))
                    com.Parameters.AddWithValue("@CashReqDate", obj.CashReqDate);
                else
                    com.Parameters.AddWithValue("@CashReqDate", DBNull.Value);
               
                
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public DataTable BindCustomerCash(int? Id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT cr.Id ,cr.CustomerId,cr.CashAmount,cr.CashRequestDate,cm.FirstName,cm.LastName FROM tbl_Customer_Cash_Request cr INNer join tbl_Customer_Master cm on cm.Id=cr.CustomerId where (@Id IS NULL OR cm.Id = @Id)", con);
            cmd.CommandType = CommandType.Text;
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
        public DataTable BindCustomerCashOrder(int? Id, DateTime? Fromdate, DateTime? Todate)
        {
           
            ToDate = Todate;
             FromDate = Fromdate;
            con.Open();
            string query = "SELECT cr.Id As cashId,cr.CustomerId,cr.CashAmount,cr.CashRequestDate,cm.FirstName +' '+ cm.LastName as Customer,cm.lat,cm.lon,cm.OrderBy,cm.MobileNo,cm.Address ";
            query += " FROM  [tbl_Staff_Master] sm";
            query += " left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id";
            query += " left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId ";
            query += " left join tbl_Customer_Cash_Request cr  on cm.Id=cr.CustomerId";
            query += " where (@Id IS NULL OR  sm.Id=@Id) AND (CONVERT(VARCHAR,cr.CashRequestDate,23) BETWEEN @FromDate AND @ToDate) ORDER BY cm.OrderBy";


            //SqlCommand cmd = new SqlCommand("SELECT cr.Id ,cr.CustomerId,cr.CashAmount,cr.CashRequestDate,cm.FirstName,cm.LastName,cm.lat,cm.lon,cm.OrderBy,cm.MobileNo,cm.Address FROM tbl_Customer_Cash_Request cr INNer join tbl_Customer_Master cm on cm.Id=cr.CustomerId where (@Id IS NULL OR cm.Id = @Id) AND (@ToDate IS NULL OR CONVERT(VARCHAR,cr.CashRequestDate,23) BETWEEN @ToDate AND @Todate)", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);

            if (!string.IsNullOrEmpty(FromDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FromDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);

            if (!string.IsNullOrEmpty(ToDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", ToDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public DataTable BindCustomerCashOrderTom(int? Id)
        {
            DateTime ToDate= DateTime.Today.AddDays(1);
            con.Open();
            string query = "SELECT cr.Id As cashId,cr.CustomerId,cr.CashAmount,cr.CashRequestDate,cm.FirstName +' '+ cm.LastName as Customer,cm.lat,cm.lon,cm.OrderBy,cm.MobileNo,cm.Address ";
            query += " FROM  [tbl_Staff_Master] sm";
            query += " left join [tbl_DeliveryBoy_Customer_Assign] dca on dca.StaffId = sm.Id";
            query += " left join [dbo].[tbl_Customer_Master] cm on cm.Id = dca.CustomerId ";
            query += " left join tbl_Customer_Cash_Request cr  on cm.Id=cr.CustomerId";
            query += " where (@Id IS NULL OR  sm.Id=@Id) AND (CONVERT(VARCHAR,cr.CashRequestDate,23) BETWEEN @ToDate AND @ToDate) ORDER BY cm.OrderBy";


            //SqlCommand cmd = new SqlCommand("SELECT cr.Id ,cr.CustomerId,cr.CashAmount,cr.CashRequestDate,cm.FirstName,cm.LastName,cm.lat,cm.lon,cm.OrderBy,cm.MobileNo,cm.Address FROM tbl_Customer_Cash_Request cr INNer join tbl_Customer_Master cm on cm.Id=cr.CustomerId where (@Id IS NULL OR cm.Id = @Id) AND (@ToDate IS NULL OR CONVERT(VARCHAR,cr.CashRequestDate,23) BETWEEN @ToDate AND @Todate)", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);

            if (!string.IsNullOrEmpty(ToDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", ToDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
        public int DeleteCash(string id,string cid)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from [tbl_Customer_Cash_Request] where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }


        public DataTable CustomerLoginRefresh(int CustomerId)
        {
            con.Open();

            DateTime today = Helper.indianTime;

            SqlCommand cmd1 = new SqlCommand("UPDATE tbl_Customer_Master set LastLogin=@LastLogin where Id=@Id1", con);


            cmd1.Parameters.AddWithValue("@LastLogin", today);
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd1.Parameters.AddWithValue("@Id1", CustomerId);
            else
                cmd1.Parameters.AddWithValue("@Id1", DBNull.Value);

            int i = cmd1.ExecuteNonQuery();


            SqlCommand cmd = new SqlCommand("select * from tbl_Customer_Master WHERE Id=@Id", con);

            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@Id", CustomerId);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public int UpdateAddress(int customerid, string Lat,string Lon)
        {
            int i = 0;
            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();
                SqlCommand com = new SqlCommand("UPDATE tbl_Customer_Master set lat=@Lat,lon=@Lon where Id=@Id", con);
                com.CommandType = CommandType.Text;

                // @CustomerId,@FirstName,@LastName,@MobileNo,@SectorId,@SectorName,@OrderId,@OrderDate,@OrderFrom
                if (!string.IsNullOrEmpty(customerid.ToString()))
                    com.Parameters.AddWithValue("@Id", customerid);
                else
                    com.Parameters.AddWithValue("@Id", DBNull.Value);
                if (!string.IsNullOrEmpty(Lat.ToString()))
                    com.Parameters.AddWithValue("@Lat", Lat);
                else
                    com.Parameters.AddWithValue("@Lat", DBNull.Value);
                if (!string.IsNullOrEmpty(Lon.ToString()))
                    com.Parameters.AddWithValue("@Lon", Lon);
                else
                    com.Parameters.AddWithValue("@Lon", DBNull.Value);


                //com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }


        public DataTable NewCustomerMsg(int StateId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from tbl_New_Customermsg WHERE IsActive='True'", con);

            cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(CustomerId.ToString()))
            //    cmd.Parameters.AddWithValue("@Id", CustomerId);
            //else
            //    cmd.Parameters.AddWithValue("@Id", DBNull.Value);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        
        //catch (Exception ex)
        //{ }




        public int InsertCustomerNotification(Customer obj)
        {
            string Status = "0";
            int i = 0;
            DateTime today = Helper.indianTime;
            //try
            //{
                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();
                SqlCommand com = new SqlCommand("Insert Into tbl_CustomerNotification1(UserId,UpdatedOn,SectorId,NotificationId,Status)Values(@CustomerId,@UpdatedOn,@SectorId,@NotificationId,@Status)", con);
                com.CommandType = CommandType.Text;

                // @CustomerId,@FirstName,@LastName,@MobileNo,@SectorId,@SectorName,@OrderId,@OrderDate,@OrderFrom


                com.Parameters.AddWithValue("@CustomerId", obj.UserName);

                if (!string.IsNullOrEmpty(today.ToString()))
                    com.Parameters.AddWithValue("@UpdatedOn", today);
                else
                    com.Parameters.AddWithValue("@UpdatedOn", DBNull.Value);


                if (!string.IsNullOrEmpty(obj.SectorId.ToString()))
                    com.Parameters.AddWithValue("@SectorId", obj.SectorId);
                else
                    com.Parameters.AddWithValue("@SectorId", 0);

                if (!string.IsNullOrEmpty(obj.nid.ToString()))
                    com.Parameters.AddWithValue("@NotificationId", obj.nid);
                else
                    com.Parameters.AddWithValue("@NotificationId", 0);

            if (!string.IsNullOrEmpty(Status))
                com.Parameters.AddWithValue("@Status", Status);
            else
                com.Parameters.AddWithValue("@Status", DBNull.Value);
            // com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
            i = com.ExecuteNonQuery();
                con.Close();
            //}
            //catch (Exception ex)
            //{ }
            return i;

        }


        public DataTable getCustomerNotiList(int? Notid)
        {
            con.Open();

            string query = "SELECT Cm.fcm_token,Nt.Title,Nt.Notification,Nt.Image,Nt.NewLink From tbl_Notification Nt";
            query += " Inner Join tbl_CustomerNotification Cn On Nt.Id=Cn.NotificationId";
            query += " Inner Join tbl_Customer_Master Cm On Cm.UserName=Cn.UserId";
            query += " where Nt.Id=@Notid and Cm.fcm_token is not null";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@Notid", Notid);
            else
                cmd.Parameters.AddWithValue("@Notid", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable GetAllCustomerNotification(int? Notid)
        {
            con.Open();

            string query = "SELECT Nt.Id,Nt.Title,Nt.Notification,Nt.Image,Nt.NewLink,Convert(varchar,Nt.UpdatedOn,23) As UpdatedOn,Nt.Type,Concat(Cm.FirstName,'',Cm.LastName) As Name,Cm.MobileNo,Cn.Status,Cn.Id as CnId From tbl_Notification1 Nt";
            query += "  Inner Join tbl_CustomerNotification1 Cn On Nt.Id=Cn.NotificationId";
            query += " Inner Join tbl_Customer_Master Cm On Cm.UserName=Cn.UserId";
            //query += " where Nt.Id=@Notid and Cm.fcm_token is not null";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(SectorId.ToString()))
            //    cmd.Parameters.AddWithValue("@Notid", Notid);
            //else
            //    cmd.Parameters.AddWithValue("@Notid", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
        public DataTable GetCustomerNotification(string CustomerId, DateTime? Fromdate, DateTime? Todate, string sector, string type)
        {
            con.Open();
            if (CustomerId == "0")
                CustomerId = null;

            string query = "SELECT Nt.Id,Nt.Title,Nt.Notification,Nt.Image,Nt.NewLink,Convert(varchar,Nt.UpdatedOn,23) As UpdatedOn,Nt.Type,Concat(Cm.FirstName,'',Cm.LastName) As Name,Cm.MobileNo,Cn.Status,Cn.Id as CnId From tbl_Notification1 Nt";
            query += "  Inner Join tbl_CustomerNotification1 Cn On Nt.Id=Cn.NotificationId";
            query += " Inner Join tbl_Customer_Master Cm On Cm.UserName=Cn.UserId";
            query += " where (@CustomerId Is Null Or Cm.Id=@CustomerId) AND (@FromDate Is Null Or @ToDate Is Null Or CONVERT(VARCHAR,Cn.UpdatedOn,23) BETWEEN @FromDate AND @ToDate)";
            query += " And (@Type Is Null Or Nt.Type=@Type) And (@Sector Is Null Or Cn.SectorId=@Sector)";
            //query += " where Nt.Id=@Notid and Cm.fcm_token is not null";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(CustomerId))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);


            if (!string.IsNullOrEmpty(Fromdate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", Fromdate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);

            if (!string.IsNullOrEmpty(Todate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", Todate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);


            if (!string.IsNullOrEmpty(type))
                cmd.Parameters.AddWithValue("@Type", type);
            else
                cmd.Parameters.AddWithValue("@Type", DBNull.Value);

            if (!string.IsNullOrEmpty(sector))
                cmd.Parameters.AddWithValue("@Sector", sector);
            else
                cmd.Parameters.AddWithValue("@Sector", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable CustomerNotification(string Uname)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select Nt.Id, Nt.Title,Nt.Notification,Nt.NewLink,Nt.Image,Nt.UpdatedOn,Cn.UserId From tbl_Notification1 Nt Inner Join tbl_CustomerNotification1 Cn On Cn.NotificationId=Nt.Id Where Cn.UserId=@Uname order by Nt.Id Desc", con);

            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Uname.ToString()))
                cmd.Parameters.AddWithValue("@Uname", Uname);
            else
                cmd.Parameters.AddWithValue("@Uname", DBNull.Value);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public int DeleteCustomerNotification(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_CustomerNotification1 where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }
        public int DeleteNotification()
        {
            int i1 = 0;
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from tbl_CustomerNotification1 where convert(varchar,[UpdatedOn],23)<GETDATE()-7", con);
            int i = cmd.ExecuteNonQuery();

           
            return i;
        }


        public DataTable DailyDeleteOrder(DateTime? FDate, DateTime? TDate)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select Concat(Cm.FirstName,' ',Cm.LastName) As Name,Pm.ProductName,Dl.UpdatedOn,Cm.MobileNo FRom tbl_dailyDelete Dl INNER Join tbl_Customer_Master Cm on Dl.CustomerId=Cm.Id Inner Join tbl_Product_Master Pm On Dl.Proid=Pm.Id where (@FromDate is null or @ToDate is null or convert(varchar,Dl.UpdatedOn,23) between @FromDate and @Todate)", con);

            cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(Uname.ToString()))
            //    cmd.Parameters.AddWithValue("@Uname", Uname);
            //else
            //    cmd.Parameters.AddWithValue("@Uname", DBNull.Value);
            if (!string.IsNullOrEmpty(FDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(TDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", TDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public DataTable GetCustomerPoductWise(int? Id)
        {
            con.Open();
            string query = "SELECT Distinct cm.*,sm.Id As sectorId,sm.[SectorName],Ot.Status,Da.StaffId,Pm.ProductName,Pm.Id As ProductId	FROM [dbo].[tbl_Customer_Master] cm ";
            query += " Inner JOIN [dbo].[tbl_Sector_Master] sm ON sm.Id = cm.SectorId ";
            query += " Inner Join tbl_DeliveryBoy_Customer_Assign Da on Da.CustomerId=cm.Id ";
            query += " Inner join tbl_Customer_Order_Transaction Ot On Ot.CustomerId=cm.Id and Ot.Status='Pending' ";
            query += " Inner Join tbl_Customer_Order_Detail Od on Ot.Id=Od.OrderId ";
            query += " Inner Join tbl_Product_Master Pm On Od.ProductId=Pm.Id";
            query += " AND (convert(varchar,Ot.Orderdate,23) < CONVERT(date,SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'))) ";
            query += " And Od.ProductId=@Id And Od.AttributeId Is Null";
            //SqlCommand cmd = new SqlCommand("SP_Customer_SelectAll1", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
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


        public DataTable GetCustomerOrderPoductWise(int? customerid,int? Id)
        {
            con.Open();
            string query = "SELECT Distinct Ot.Status,Ot.Id As OrderId	";
           
            query += " FROM tbl_Customer_Order_Transaction  Ot ";
            query += " Inner Join tbl_Customer_Order_Detail Od on Ot.Id=Od.OrderId and Ot.Status='Pending' ";
            
            query += " AND (convert(varchar,Ot.Orderdate,23) < CONVERT(date,SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'))) ";
            query += " And Od.ProductId=@Id And Od.AttributeId Is Null";
            query += " where Ot.CustomerId=@CustomerId";
            //SqlCommand cmd = new SqlCommand("SP_Customer_SelectAll1", con);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@Id", Id);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);

            if (!string.IsNullOrEmpty(customerid.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", customerid);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public int InsertNotification(Customer obj, string type)
        {
            int i = 0;
            //try
            //{
            DateTime today = DateTime.Now;
            con.Open();
            //SqlCommand com = new SqlCommand("Vendor_Product_Assign_Insert", con);
            // SqlCommand com = new SqlCommand("INSERT INTO tbl_Notification1([Title],[Notification],[Image],[NewLink],[UpdatedOn])VALUES(@Title,@Notification,@Image,@NewLink,@UpdatedOn)", con);
            //com.CommandType = CommandType.Text;
            SqlCommand com = new SqlCommand("InsertNotification1", con);
            com.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(obj.ntitle.ToString()))
                com.Parameters.AddWithValue("@Title", obj.ntitle);
            else
                com.Parameters.AddWithValue("@Title", DBNull.Value);

            if (!string.IsNullOrEmpty(obj.ntext))
                com.Parameters.AddWithValue("@Notification", obj.ntext);
            else
                com.Parameters.AddWithValue("@Notification", DBNull.Value);

            if (!string.IsNullOrEmpty(obj.Photo))
                com.Parameters.AddWithValue("@Image", obj.Photo);
            else
                com.Parameters.AddWithValue("@Image", DBNull.Value);


            if (!string.IsNullOrEmpty(obj.nlink))
                com.Parameters.AddWithValue("@NewLink", obj.nlink);
            else
                com.Parameters.AddWithValue("@NewLink", DBNull.Value);

            com.Parameters.AddWithValue("@UpdatedOn", Helper.indianTime);

            if (!string.IsNullOrEmpty(type))
                com.Parameters.AddWithValue("@Type", type);
            else
                com.Parameters.AddWithValue("@Type", DBNull.Value);
            com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
            i = com.ExecuteNonQuery();
            try
            {
                con.Close();
                int id = Convert.ToInt32(com.Parameters["@Id"].Value);
                return id;
            }
            catch { }

            return i;

        }

        public DataTable CustomerNotification(string Uname, DateTime? Fromdate, DateTime? Todate)
        {
            ToDate = Todate;
            FromDate = Fromdate;
            con.Open();
            SqlCommand cmd = new SqlCommand("Select Nt.Id, Nt.Title,Nt.Notification,Nt.NewLink,Nt.Image,Nt.UpdatedOn,Cn.UserId,Cn.Status,Cn.NotificationId From tbl_Notification1 Nt Inner Join tbl_CustomerNotification1 Cn On Cn.NotificationId=Nt.Id Where Cn.UserId=@Uname AND (CONVERT(VARCHAR,Cn.UpdatedOn,23) BETWEEN @FromDate AND @ToDate) order by Nt.Id Desc", con);

            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(Uname.ToString()))
                cmd.Parameters.AddWithValue("@Uname", Uname);
            else
                cmd.Parameters.AddWithValue("@Uname", DBNull.Value);

            if (!string.IsNullOrEmpty(FromDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FromDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);

            if (!string.IsNullOrEmpty(ToDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", ToDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public int UpdateCustomerNotificationseen(string Username, int? NotId)
        {
            int i = 0;
            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();
                SqlCommand com = new SqlCommand("UPDATE tbl_CustomerNotification1 set Status='1' where UserId='" + Username + "' And NotificationId=" + NotId + "", con);
                com.CommandType = CommandType.Text;

                // @CustomerId,@FirstName,@LastName,@MobileNo,@SectorId,@SectorName,@OrderId,@OrderDate,@OrderFrom



                // com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public DataTable GetTerms()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select * From tbl_terms  order by Pos", con);

            cmd.CommandType = CommandType.Text;


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public DataTable GetCustomerbyId(int CustomerId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from tbl_Customer_Master WHERE Id=@Id", con);

            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@Id", CustomerId);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable getCustomerOrderDetail(int CustomerId, DateTime? FromDate, DateTime? ToDate)
        {
            con.Open();
            string query = "select sum(Od.Qty) As qty,max(Od.ProductId) AS ProductId,Max(Pm.ProductName) As Description,sum(Od.Amount) As Amount,sum(Od.CGSTAmount) As CGSTAmount,sum(Od.SGSTAmount) As SGSTAmount";
            query += " from [dbo].[tbl_Customer_Order_Transaction] ot ";
            query += " inner join tbl_Customer_Order_Detail Od on ot.Id=Od.OrderId";
            query += " Inner Join tbl_Product_Master Pm On Od.ProductId=Pm.Id";
            query += " where ot.CustomerId=@CustomerId and Od.Qty<>0";
            query += " and (@FromDate is null or @ToDate is null or convert(varchar,ot.OrderDate,23) between @FromDate and @ToDate)";
            query += "  Group By Od.ProductId";

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.CommandType = CommandType.Text;

            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(FromDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FromDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(ToDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", ToDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable getCustomerBillPayDetail(int CustomerId, DateTime? FromDate, DateTime? ToDate)
        {
            con.Open();
            string query = "select *";
            query += " from [dbo].[tbl_Customer_Wallet]  ";

            query += " where CustomerId=@CustomerId and (status='Recharge-Mobile' or status='Recharge-DTH' or status='BillPay-Gas' or status='BillPay-Electricity')";
            query += " and (@FromDate is null or @ToDate is null or convert(varchar,TransactionDate,23) between @FromDate and @ToDate)";


            SqlCommand cmd = new SqlCommand(query, con);

            cmd.CommandType = CommandType.Text;

            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(FromDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FromDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(ToDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", ToDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable getCustomerAddWallet(int CustomerId, DateTime? FromDate, DateTime? ToDate)
        {
            con.Open();
            string query = "select Amount,convert(varchar,TransactionDate,23) As TransactionDate";
            query += " from [dbo].[tbl_Customer_Wallet]  ";

            query += " where CustomerId=@CustomerId and (Description='Add To Wallet' or Description='Add To Wallet Add To Wallet')";
            query += " and (@FromDate is null or @ToDate is null or TransactionDate between @FromDate and @ToDate)";


            SqlCommand cmd = new SqlCommand(query, con);

            cmd.CommandType = CommandType.Text;

            if (!string.IsNullOrEmpty(CustomerId.ToString()))
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
            else
                cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);
            if (!string.IsNullOrEmpty(FromDate.ToString()))
                cmd.Parameters.AddWithValue("@FromDate", FromDate);
            else
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            if (!string.IsNullOrEmpty(ToDate.ToString()))
                cmd.Parameters.AddWithValue("@ToDate", ToDate);
            else
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public DataTable getOfferVendor(int? SectorId,string Type)
        {
            con.Open();
            string query = "select Vm.Id,Vm.FirstName,Vm.LastName,Vm.MobileNo,Vm.Vendorstore,Vm.Photo As Logo,Vm.Slider1,Vm.Slider2,Vm.DiscountPer,Vm.Vendorterms,Vs.SectorId,Sm.SectorName,Vm.IsActive";
            query += " from [dbo].[tbl_Vendor_Master] Vm ";
            query += " Inner Join tbl_Vendor_Sector_Assign Vs On Vm.Id=Vs.VendorId";
            query += " Inner Join tbl_Sector_Master Sm On Sm.Id=Vs.SectorId";
            query += " where Vs.SectorId=@SectorId And Vm.MasterCat=@Type";
            


            SqlCommand cmd = new SqlCommand(query, con);

            cmd.CommandType = CommandType.Text;

            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(Type))
                cmd.Parameters.AddWithValue("@Type", Type);
            else
                cmd.Parameters.AddWithValue("@Type", DBNull.Value);
            

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public DataTable getVendorOfferList(int? SectorId, int? VendorId)
        {
            con.Open();
            string query = "select Vo.*";
            query += " from [dbo].[tbl_Offer_VoucherDetail] Vo ";
            query += " Inner Join tbl_OfferVoucherSector Vs On Vo.VendorId=Vs.VendorId";
            //query += " Inner Join tbl_Sector_Master Sm On Sm.Id=Vs.SectorId";
            query += " where Vs.SectorId=@SectorId And Vo.VendorId=@VendorId";



            SqlCommand cmd = new SqlCommand(query, con);

            cmd.CommandType = CommandType.Text;

            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@Type", DBNull.Value);


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public DataTable getOfferVendorProductList(int? SectorId, int? VendorId)
        {
            con.Open();
            string query = "select Vp.*";
            query += " from [dbo].[tbl_Vendor_ProductDetail] Vp ";
            query += " Inner Join tbl_Vendor_Sector_Assign Vs On Vp.VendorId=Vs.VendorId";
            //query += " Inner Join tbl_Sector_Master Sm On Sm.Id=Vs.SectorId";
            query += " where Vs.SectorId=@SectorId And Vp.VendorId=@VendorId";



            SqlCommand cmd = new SqlCommand(query, con);

            cmd.CommandType = CommandType.Text;

            if (!string.IsNullOrEmpty(SectorId.ToString()))
                cmd.Parameters.AddWithValue("@SectorId", SectorId);
            else
                cmd.Parameters.AddWithValue("@SectorId", DBNull.Value);
            if (!string.IsNullOrEmpty(VendorId.ToString()))
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
            else
                cmd.Parameters.AddWithValue("@Type", DBNull.Value);


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public DataTable getVendorOfferDetail(int? OfferId)
        {
            con.Open();
            string query = "select Vo.*";
            query += " from [dbo].[tbl_Offer_VoucherDetail] Vo ";
            
            //query += " Inner Join tbl_Sector_Master Sm On Sm.Id=Vs.SectorId";
            query += " where Vo.Id=@Id";



            SqlCommand cmd = new SqlCommand(query, con);

            cmd.CommandType = CommandType.Text;

            
            if (!string.IsNullOrEmpty(OfferId.ToString()))
                cmd.Parameters.AddWithValue("@Id", OfferId);
            else
                cmd.Parameters.AddWithValue("@Id", DBNull.Value);


            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
    }
}
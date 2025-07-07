using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
namespace MilkWayIndia.Models
{
    public class MobileRecharge
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
        public int SectorId { get; set; }
        public int BuildingId { get; set; }
        public string Photo { get; set; }
        public string base64Image { get; set; }
        public int FlatId { get; set; }


        public long CompanyName { get; set; }
        public string PanCardNo { get; set; }
        public int GSTNo { get; set; }
        public Decimal Credit { get; set; }
        public int OrderBy { get; set; }

        public int VendorId { get; set; }

        //assign delivery boy
        public int CustomerId { get; set; }
        public int StaffId { get; set; }

        //otp 
        public string OTP { get; set; }
        public DateTime LastUpdateOtpDate { get; set; }
        public int Count { get; set; }



        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        clsCommon _clsCommon = new clsCommon();
        Helper dHelper = new Helper();



        public string Rechargeno { get; set; }
        public string UtransactionId { get; set; }
        public string UtransactionId1 { get; set; }
        public string CircleCode { get; set; }
        public string OperatorId { get; set; }
        public int Status { get; set; }
        public string Responsemsg { get; set; }
        public string Marginper { get; set; }
        public Decimal Marginamnt { get; set; }
        public string TransactionId { get; set; }
        public Decimal RechargeAmount { get; set; }
        public string RechargeType { get; set; }
        public string CashbackStatus { get; set; }
        public string CashbackAmount { get; set; }
        public DateTime? CashbackDate { get; set; }


        public string RechargeStatus { get; set; }
        public DataTable getCircles()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[tblBillPayCircles] WHERE IsActive=1 Order By SortOrder Asc ", con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable getOperator(string type)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[tblBillPayOperators] WHERE IsActive=1 and Type='" + type + "'", con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        public DataTable getUtransactionId()
        {

            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT Id,UtransactionId FROM [dbo].[tbl_Utransaction] Order By Id ASC", con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }



        public int insertrecharge(MobileRecharge objrecharge)
        {
            int i = 0;
            try
            {
                //if (CustomerId == 0) CustomerId = null;
                //if (ProductId == 0) ProductId = null;

                con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO tbl_Customer_Mobile_Recharge(CustomerId,Rechargeno,Utransactionid,CircleCode,OpeartorId,Status,Responsemsg,Marginper,Marginamnt,TransactionID1,RechargeAmount,rechargedate,rechargetype)VALUES(@CustomerId,@Rechargeno,@Utransactionid,@CircleCode,@OpeartorId,@Status,@Responsemsg,@Marginper,@Marginamnt,@TransactionID1,@RechargeAmount,@rechargedate,@rechargetype)", con);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(CustomerId.ToString()))
                    cmd.Parameters.AddWithValue("@CustomerId", objrecharge.CustomerId);
                else
                    cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);

                cmd.Parameters.AddWithValue("@Rechargeno", objrecharge.Rechargeno);
                cmd.Parameters.AddWithValue("@Utransactionid", objrecharge.UtransactionId);



                cmd.Parameters.AddWithValue("@CircleCode", objrecharge.CircleCode);
                cmd.Parameters.AddWithValue("@OpeartorId", objrecharge.OperatorId);
                cmd.Parameters.AddWithValue("@Status", objrecharge.Status);
                cmd.Parameters.AddWithValue("@Responsemsg", objrecharge.Responsemsg);
                cmd.Parameters.AddWithValue("@Marginper", objrecharge.Marginper);
                cmd.Parameters.AddWithValue("@Marginamnt", objrecharge.Marginamnt);
                cmd.Parameters.AddWithValue("@TransactionID1", objrecharge.TransactionId);
                cmd.Parameters.AddWithValue("@RechargeAmount", objrecharge.RechargeAmount);
                cmd.Parameters.AddWithValue("@rechargedate", objrecharge.FromDate);
                cmd.Parameters.AddWithValue("@rechargetype", objrecharge.RechargeType);
               
                i = cmd.ExecuteNonQuery();


                SqlCommand cmd1 = new SqlCommand("INSERT INTO tbl_Utransaction(UtransactionId,UtransactionIdnew)VALUES(@UtransactionId,@UtransactionIdnew)", con);
                cmd1.CommandType = CommandType.Text;
               
                cmd1.Parameters.AddWithValue("@UtransactionId", objrecharge.UtransactionId1);
                cmd1.Parameters.AddWithValue("@UtransactionIdnew", objrecharge.UtransactionId);





                i=cmd1.ExecuteNonQuery();



                con.Close();
            }
            catch (Exception ex) { string s = ex.Message; }
            return i;
        }

        public int Updaterecharge(MobileRecharge objrecharge)
        {


           

            int i = 0;
           
                //if (CustomerId == 0) CustomerId = null;
                //if (ProductId == 0) ProductId = null;

                con.Open();
                SqlCommand cmd = new SqlCommand("UPDATE tbl_Customer_Mobile_Recharge SET CashBackstatus=@CashBackstatus,CashBackAmount=@CashBackAmount,cashbackdate=@cashbackdate,RechargeStatus=@RechargeStatus where Utransactionid=@Utransactionid", con);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@CashBackstatus", objrecharge.CashbackStatus);
                cmd.Parameters.AddWithValue("@CashBackAmount", objrecharge.CashbackAmount);



                cmd.Parameters.AddWithValue("@cashbackdate", objrecharge.CashbackDate);
            cmd.Parameters.AddWithValue("@RechargeStatus", objrecharge.RechargeStatus);
            cmd.Parameters.AddWithValue("@Utransactionid", objrecharge.UtransactionId);
                i = cmd.ExecuteNonQuery();
                con.Close();
           

            
            return i;

        }



        public int Updaterecharge1(MobileRecharge objrecharge)
        {




            int i = 0;

            //if (CustomerId == 0) CustomerId = null;
            //if (ProductId == 0) ProductId = null;

            con.Open();
            SqlCommand cmd = new SqlCommand("UPDATE tbl_Customer_Mobile_Recharge SET Status=@Status,Responsemsg=@Responsemsg,TransactionID1=@TransactionID1,CashBackstatus=@CashBackstatus,CashBackAmount=@CashBackAmount,cashbackdate=@cashbackdate,Marginper=@Marginper,Marginamnt=@Marginamnt,RechargeStatus=@RechargeStatus where Utransactionid=@Utransactionid", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@Status", objrecharge.Status);
            cmd.Parameters.AddWithValue("@Responsemsg", objrecharge.Responsemsg);

            cmd.Parameters.AddWithValue("@TransactionID1", objrecharge.TransactionId);
           



            cmd.Parameters.AddWithValue("@CashBackstatus", objrecharge.CashbackStatus);
            cmd.Parameters.AddWithValue("@CashBackAmount", objrecharge.CashbackAmount);



            cmd.Parameters.AddWithValue("@cashbackdate", objrecharge.CashbackDate);


            cmd.Parameters.AddWithValue("@Marginper", objrecharge.Marginper);
            cmd.Parameters.AddWithValue("@Marginamnt", objrecharge.Marginamnt);

            cmd.Parameters.AddWithValue("@RechargeStatus", objrecharge.RechargeStatus);
         
            cmd.Parameters.AddWithValue("@Utransactionid", objrecharge.UtransactionId);
            i = cmd.ExecuteNonQuery();

            
            con.Close();



            return i;

        }


        public int Updaterecharge2(MobileRecharge objrecharge)
        {




            int i = 0;
            objrecharge.CashbackAmount = "0";
            objrecharge.CashbackStatus = "Fail";
            //if (CustomerId == 0) CustomerId = null;
            //if (ProductId == 0) ProductId = null;

            con.Open();
            SqlCommand cmd = new SqlCommand("UPDATE tbl_Customer_Mobile_Recharge SET Status=@Status,Responsemsg=@Responsemsg,TransactionID1=@TransactionID1,CashBackstatus=@CashBackstatus,CashBackAmount=@CashBackAmount,cashbackdate=@cashbackdate,Marginper=@Marginper,Marginamnt=@Marginamnt,RechargeStatus=@RechargeStatus where Utransactionid=@Utransactionid", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@Status", objrecharge.Status);
            cmd.Parameters.AddWithValue("@Responsemsg", objrecharge.Responsemsg);

            cmd.Parameters.AddWithValue("@TransactionID1", objrecharge.TransactionId);




            cmd.Parameters.AddWithValue("@CashBackstatus", objrecharge.CashbackStatus);
            cmd.Parameters.AddWithValue("@CashBackAmount", objrecharge.CashbackAmount);



            cmd.Parameters.AddWithValue("@cashbackdate", objrecharge.CashbackDate);


            cmd.Parameters.AddWithValue("@Marginper", objrecharge.Marginper);
            cmd.Parameters.AddWithValue("@Marginamnt", objrecharge.Marginamnt);

            cmd.Parameters.AddWithValue("@RechargeStatus", objrecharge.RechargeStatus);

            cmd.Parameters.AddWithValue("@Utransactionid", objrecharge.UtransactionId);





            i = cmd.ExecuteNonQuery();


            SqlCommand cmd1 = new SqlCommand("DELETE from tbl_Customer_Wallet where UtransactionId=@Utransactionid", con);


            cmd1.Parameters.AddWithValue("@Utransactionid", objrecharge.UtransactionId);





            i = cmd1.ExecuteNonQuery();
            con.Close();



            return i;

        }

        public DataTable getCashbackper(string service, string operatorid)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            
          
                cmd = new SqlCommand("Select Amount1 from tbl_cashback_settings where (@Service IS NULL OR Service = @Service) AND (@OperatorId IS NULL OR OperatorId = @OperatorId)", con);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(service.ToString()))
                    cmd.Parameters.AddWithValue("@Service", service);
                else
                    cmd.Parameters.AddWithValue("@Service", DBNull.Value);

            if (!string.IsNullOrEmpty(operatorid.ToString()))
                cmd.Parameters.AddWithValue("@OperatorId", operatorid);
            else
                cmd.Parameters.AddWithValue("@OperatorId", DBNull.Value);
            da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            
        }


        public DataTable getmobilebalance()
        {

            con.Open();
            SqlCommand cmd = new SqlCommand("Select SUM(RechargeAmount) AS RechargeAmount from [tbl_Customer_Mobile_Recharge] where Status=4", con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }


        public DataTable getgasbalance()
        {

            con.Open();
            SqlCommand cmd = new SqlCommand("Select SUM(Amount) AS RechargeAmount from [tbl_Customer_bill_Pay] where Status=5", con);
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
    }
}
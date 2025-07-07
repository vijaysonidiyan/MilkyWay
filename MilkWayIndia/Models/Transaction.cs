using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Models
{
    public class Transaction
    {


        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString);

        //Sector
        public int Id { get; set; }
        public string Service { get; set; }
        public string ProviderName { get; set; }
        public int OperatorCode { get; set; }

        //Building
        public string Type { get; set; }
        public decimal Amount { get; set; }

        public int statuscode { get; set; }
        public DataTable getTransactionMobileDTHList(string service)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT mr.Utransactionid,mr.CustomerId,cm.FirstName,cm.LastName,mr.Rechargeno,mr.Status,mr.Responsemsg,mr.RechargeAmount,mr.rechargedate,mr.rechargetype,mr.RechargeStatus From tbl_Customer_Mobile_Recharge mr INNER Join tbl_Customer_Master CM on mr.CustomerId=cm.Id", con);
            cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(service.ToString()))
            //    cmd.Parameters.AddWithValue("@service", service);
            //else
            //    cmd.Parameters.AddWithValue("@service", "");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);


            return dt;
        }


        public DataTable getTransactionElectricityGasList(string service)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT bp.Utransactionid,bp.OperatorId,bp.CustomerId,cm.FirstName,cm.LastName,bp.numberCustomer,bp.status,bp.ResMsg,bp.Amount,bp.billdate,bp.type,bp.billstatus From tbl_Customer_bill_Pay bp INNER Join tbl_Customer_Master CM on bp.CustomerId=cm.Id", con);
            cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(service.ToString()))
            //    cmd.Parameters.AddWithValue("@service", service);
            //else
            //    cmd.Parameters.AddWithValue("@service", "");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);


            return dt;
        }



        public DataTable getTransactionMobileDTHListFilter(string service,int? statuscode, DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT mr.Utransactionid,mr.CustomerId,cm.FirstName,cm.LastName,mr.Rechargeno,mr.Status,mr.Responsemsg,mr.RechargeAmount,mr.rechargedate,mr.rechargetype,mr.RechargeStatus From tbl_Customer_Mobile_Recharge mr INNER Join tbl_Customer_Master CM on mr.CustomerId=cm.Id where mr.rechargetype=@service AND mr.Status=@StatusCode and (@FromDate is null or @ToDate is null or convert(varchar,mr.rechargedate,23) between @FromDate and @Todate)", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(service.ToString()))
                cmd.Parameters.AddWithValue("@service", service);
            else
                cmd.Parameters.AddWithValue("@service", "");


            if (!string.IsNullOrEmpty(statuscode.ToString()))
                cmd.Parameters.AddWithValue("@StatusCode", statuscode);
            else
                cmd.Parameters.AddWithValue("@StatusCode", DBNull.Value);
          


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


            return dt;
        }




        public DataTable getTransactionElectricityGasListFilter(string service, int? statuscode, DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT bp.Utransactionid,bp.OperatorId,bp.CustomerId,cm.FirstName,cm.LastName,bp.numberCustomer,bp.status,bp.ResMsg,bp.Amount,bp.billdate,bp.type,bp.billstatus From tbl_Customer_bill_Pay bp INNER Join tbl_Customer_Master CM on bp.CustomerId=cm.Id where bp.type=@service AND bp.status=@StatusCode and (@FromDate is null or @ToDate is null or convert(varchar,bp.billdate,23) between @FromDate and @Todate)", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(service.ToString()))
                cmd.Parameters.AddWithValue("@service", service);
            else
                cmd.Parameters.AddWithValue("@service", "");


            if (!string.IsNullOrEmpty(statuscode.ToString()))
                cmd.Parameters.AddWithValue("@StatusCode", statuscode);
            else
                cmd.Parameters.AddWithValue("@StatusCode", DBNull.Value);



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


            return dt;
        }



        public int Updaterecharge(string utransactionid, string itemstatus,string itemmsg,string rechargestatus)
        {




            int i = 0;

            //if (CustomerId == 0) CustomerId = null;
            //if (ProductId == 0) ProductId = null;

            con.Open();
            SqlCommand cmd = new SqlCommand("UPDATE tbl_Customer_Mobile_Recharge SET Status=@Status,Responsemsg=@Responsemsg,RechargeStatus=@RechargeStatus where Utransactionid=@Utransactionid", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@Status", itemstatus);
            cmd.Parameters.AddWithValue("@Responsemsg", itemmsg);



            
            cmd.Parameters.AddWithValue("@RechargeStatus", rechargestatus);
            cmd.Parameters.AddWithValue("@Utransactionid", utransactionid);
            i = cmd.ExecuteNonQuery();
            con.Close();



            return i;

        }



        public int Updaterechargeelectricity(string utransactionid, string itemstatus, string itemmsg, string rechargestatus)
        {




            int i = 0;

            //if (CustomerId == 0) CustomerId = null;
            //if (ProductId == 0) ProductId = null;

            con.Open();
            SqlCommand cmd = new SqlCommand("UPDATE tbl_Customer_bill_Pay SET status=@Status,ResMsg=@Responsemsg,billstatus=@RechargeStatus where UtransactionId=@Utransactionid", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@Status", itemstatus);
            cmd.Parameters.AddWithValue("@Responsemsg", itemmsg);




            cmd.Parameters.AddWithValue("@RechargeStatus", rechargestatus);
            cmd.Parameters.AddWithValue("@Utransactionid", utransactionid);
            i = cmd.ExecuteNonQuery();
            con.Close();



            return i;

        }



        public DataTable getparticularUtransaction(string Utransaction,int? CustomerId)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FRom tbl_Customer_bill_Pay where UtransactionId='"+Utransaction+ "' and CustomerId="+CustomerId+"", con);
            cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(service.ToString()))
            //    cmd.Parameters.AddWithValue("@service", service);
            //else
            //    cmd.Parameters.AddWithValue("@service", "");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);


            return dt;
        }


        public DataTable getparticularMobileUtransaction(string Utransaction)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT CircleCode,OpeartorId,Rechargeno,RechargeAmount,CashBackstatus,CashBackAmount,cashbackdate FRom tbl_Customer_Mobile_Recharge where Utransactionid='" + Utransaction + "'", con);
           
           
            cmd.CommandType = CommandType.Text;
           
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);


            return dt;
        }

        public DataTable getparticularBillUtransaction(string Utransaction)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("SELECT OperatorId, numberCustomer, Amount, Lat, Lon, AgentId, paymentmode, customermobile, field1, field2, field3,CashBackstatus,CashBackAmount,cashbackdate FRom tbl_Customer_bill_Pay where UtransactionId='" + Utransaction + "'", con);


            cmd.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);


            return dt;
        }
        public int UpdateRerecharge(int id)
        {
            con.Open();

           
             
           
            SqlCommand cmd = new SqlCommand("Delete from [tbl_Staff_Master] where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }



        public int Refund(int id)
        {
            con.Open();


            int i = 0;
            string cashbackstatus = "Refund",CashbackAmount="0", RechargeStatus="Refund";

            SqlCommand cmd = new SqlCommand("UPDATE tbl_Customer_Mobile_Recharge SET CashBackstatus=@CashBackstatus,CashBackAmount=@CashBackAmount,RechargeStatus=@RechargeStatus where Utransactionid=@Utransactionid", con);
            cmd.CommandType = CommandType.Text;

            




            cmd.Parameters.AddWithValue("@CashBackstatus", cashbackstatus);
            cmd.Parameters.AddWithValue("@CashBackAmount", CashbackAmount);



           


            //cmd.Parameters.AddWithValue("@Marginper", objrecharge.Marginper);
           // cmd.Parameters.AddWithValue("@Marginamnt", objrecharge.Marginamnt);

            cmd.Parameters.AddWithValue("@RechargeStatus", RechargeStatus);

            cmd.Parameters.AddWithValue("@Utransactionid", id.ToString());





            i = cmd.ExecuteNonQuery();

            SqlCommand cmd1 = new SqlCommand("DELETE from tbl_Customer_Wallet where UtransactionId=@Utransactionid", con);


            cmd1.Parameters.AddWithValue("@Utransactionid", id.ToString());

            i = cmd1.ExecuteNonQuery();
            con.Close();
            return i;
        }


        public int Refund1(int id)
        {
            con.Open();


            int i = 0;
            string cashbackstatus = "Refund", CashbackAmount = "0", RechargeStatus = "Refund";

            SqlCommand cmd = new SqlCommand("UPDATE tbl_Customer_bill_Pay SET CashBackstatus=@CashBackstatus,CashBackAmount=@CashBackAmount,billstatus=@RechargeStatus where UtransactionId=@Utransactionid", con);
            cmd.CommandType = CommandType.Text;






            cmd.Parameters.AddWithValue("@CashBackstatus", cashbackstatus);
            cmd.Parameters.AddWithValue("@CashBackAmount", CashbackAmount);






            //cmd.Parameters.AddWithValue("@Marginper", objrecharge.Marginper);
            // cmd.Parameters.AddWithValue("@Marginamnt", objrecharge.Marginamnt);

            cmd.Parameters.AddWithValue("@RechargeStatus", RechargeStatus);

            cmd.Parameters.AddWithValue("@Utransactionid", id.ToString());





            i = cmd.ExecuteNonQuery();

            SqlCommand cmd1 = new SqlCommand("DELETE from tbl_Customer_Wallet where UtransactionId=@Utransactionid", con);


            cmd1.Parameters.AddWithValue("@Utransactionid", id.ToString());

            i = cmd1.ExecuteNonQuery();
            con.Close();
            return i;
        }
    }
}
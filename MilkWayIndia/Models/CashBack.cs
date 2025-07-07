using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Models
{
    public class CashBack
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


        public DataTable getCashbackList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Select * From tbl_cashback_settings where (@Id IS NULL OR [Id] = @Id)", con);
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



        public DataTable getServiceList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Select * From tblBillPayServices where (@Id IS NULL OR [Id] = @Id)Order By SortOrder ASC", con);
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


        public DataTable getproviderList(string service)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Select * From tblBillPayOperators where (@Type IS NULL OR [Type] = @Type)", con);
            cmd.CommandType = CommandType.Text;
            if (!string.IsNullOrEmpty(service))
                cmd.Parameters.AddWithValue("@Type", service);
            else
                cmd.Parameters.AddWithValue("@Type", DBNull.Value);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }


        public int InsertCashbackSetting(CashBack obj)
        {
            int i = 0;
            try
            {
                con.Open();
                
	
                SqlCommand com = new SqlCommand("Insert Into tbl_cashback_settings([Service],[ProviderName],[OperatorId],[Type],[Amount1])VALUES(@Service,@ProviderName,@OperatorId,@Type,@Amount)", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Service", obj.Service);
                com.Parameters.AddWithValue("@ProviderName", obj.ProviderName);
                if (!string.IsNullOrEmpty(obj.OperatorCode.ToString()))
                    com.Parameters.AddWithValue("@OperatorId", obj.OperatorCode);
                else
                    com.Parameters.AddWithValue("@OperatorId", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Type))
                    com.Parameters.AddWithValue("@Type", obj.Type);
                else
                    com.Parameters.AddWithValue("@Type", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Amount.ToString()))
                    com.Parameters.AddWithValue("@Amount", Convert.ToDecimal(obj.Amount));
                else
                    com.Parameters.AddWithValue("@Amount", DBNull.Value);
                com.Parameters.AddWithValue("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }

        public int DeleteCashbackSetting(int id)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Delete from [tbl_cashback_settings] where Id=" + id, con);
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public DataTable getCashbackflipamzList(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Select * From tbl_flip_amz_Order where (@Id IS NULL OR [Id] = @Id) AND (CashBackstatus IS NULL OR CashBackstatus<>'Complete')", con);
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

        public int AddCashBackAmount(CashBack objcashback, int id,DateTime cdate)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Update tbl_flip_amz_Order set CashBackAmount=@CashBackAmount,CashBackstatus='Complete',cashbackdate=@Cdate where Id=" + id, con);
            if (!string.IsNullOrEmpty(Id.ToString()))
                cmd.Parameters.AddWithValue("@CashBackAmount", objcashback.Amount);
            else
                cmd.Parameters.AddWithValue("@CashBackAmount", DBNull.Value);


            if (!string.IsNullOrEmpty(cdate.ToString()))
                cmd.Parameters.AddWithValue("@Cdate", cdate);
            else
                cmd.Parameters.AddWithValue("@Cdate", DBNull.Value);

            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }


        public DataTable getService(int? Id)
        {
            //con.Open();
            SqlCommand cmd = new SqlCommand("Select * From tblBillPayServices where (@Id IS NULL OR [Id] = @Id)", con);
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



        public DataTable getCashbackBillList(string service)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            if(service==null  || service=="")
                {
                service = "Mobile";
            }
            if (service == "Mobile")
            {
                cmd = new SqlCommand("Select mr.Id AS Id,mr.CustomerId as CustomerId,cm.FirstName AS FirstName,cm.LastName As LastName,mr.rechargetype As RechargeType,bps.Name As OperatorName,mr.RechargeAmount As Amount,mr.rechargedate AS RechargeDate,mr.TransactionID1 AS TransactionId,mr.Rechargeno AS RechargeNo,cs.Amount1 AS Amount1 From tbl_Customer_Mobile_Recharge mr Inner join tbl_Customer_Master cm ON cm.Id=mr.CustomerId Inner Join tbl_cashback_settings cs ON mr.OpeartorId=cs.OperatorId Inner Join tblBillPayOperators bps ON mr.OpeartorId=bps.OperatorCode where (@Service IS NULL OR mr.rechargetype = @Service) AND (mr.CashBackstatus IS NULL OR mr.CashBackstatus<>'Complete') AND mr.Status=1", con);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@Service", service);
                else
                    cmd.Parameters.AddWithValue("@Service", DBNull.Value);
                 da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }

            if (service == "DTH")
            {
                cmd = new SqlCommand("Select mr.Id AS Id,mr.CustomerId as CustomerId,cm.FirstName AS FirstName,cm.LastName As LastName,mr.rechargetype As RechargeType,bps.Name As OperatorName,mr.RechargeAmount As Amount,mr.rechargedate AS RechargeDate,mr.TransactionID1 AS TransactionId,mr.Rechargeno AS RechargeNo,cs.Amount1 AS Amount1 From tbl_Customer_Mobile_Recharge mr Inner join tbl_Customer_Master cm ON cm.Id=mr.CustomerId Inner Join tbl_cashback_settings cs ON mr.OpeartorId=cs.OperatorId Inner Join tblBillPayOperators bps ON mr.OpeartorId=bps.OperatorCode where (@Service IS NULL OR mr.rechargetype = @Service) AND (mr.CashBackstatus IS NULL OR mr.CashBackstatus<>'Complete') AND mr.Status=1", con);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@Service", service);
                else
                    cmd.Parameters.AddWithValue("@Service", DBNull.Value);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }



            if (service == "Electricity")
            {
                cmd = new SqlCommand("Select bp.Id AS Id,bp.CustomerId as CustomerId,cm.FirstName AS FirstName,cm.LastName As LastName,bp.type As RechargeType,bps.Name As OperatorName,bp.Amount As Amount,bp.billdate AS RechargeDate,bp.TransactionId As TransactionId,bp.numberCustomer AS RechargeNo,cs.Amount1 AS Amount1 From tbl_Customer_bill_Pay bp Inner join tbl_Customer_Master cm ON cm.Id=bp.CustomerId Inner Join tbl_cashback_settings cs ON bp.type=cs.Service Inner Join tblBillPayOperators bps ON bp.OperatorId=bps.OperatorCode  where (@Service IS NULL OR bp.type = @Service) AND (bp.CashBackstatus IS NULL OR bp.CashBackstatus<>'Complete' bp.status=1)", con);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@Service", service);
                else
                    cmd.Parameters.AddWithValue("@Service", DBNull.Value);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }


            if (service == "Gas")
            {
                cmd = new SqlCommand("Select bp.Id AS Id,bp.CustomerId as CustomerId,cm.FirstName AS FirstName,cm.LastName As LastName,bp.type As RechargeType,bps.Name As OperatorName,bp.Amount As Amount,bp.billdate AS RechargeDate,bp.TransactionId As TransactionId,bp.numberCustomer AS RechargeNo,cs.Amount1 AS Amount1 From tbl_Customer_bill_Pay bp Inner join tbl_Customer_Master cm ON cm.Id=bp.CustomerId Inner Join tbl_cashback_settings cs ON bp.type=cs.Service Inner Join tblBillPayOperators bps ON bp.OperatorId=bps.OperatorCode  where (@Service IS NULL OR bp.type = @Service) AND (bp.CashBackstatus IS NULL OR bp.CashBackstatus<>'Complete' and bp.status=1)", con);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@Service", service);
                else
                    cmd.Parameters.AddWithValue("@Service", DBNull.Value);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            // SqlCommand 


            //cmd = new SqlCommand("Select * From tbl_Customer_Mobile_Recharge where (@Service IS NULL OR [rechargetype] = @Service) AND (CashBackstatus IS NULL OR CashBackstatus<>'Complete')", con);
            //cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(Id.ToString()))
            //    cmd.Parameters.AddWithValue("@Service", service);
            //else
            //    cmd.Parameters.AddWithValue("@Service", DBNull.Value);
            //da = new SqlDataAdapter(cmd);
            //da.Fill(dt);
            //return dt;
            return dt;

        }


        public int AddCashBackAmount1(CashBack objcashback, int id, DateTime cdate)
        {
            con.Open();
            int i=0;
            if (objcashback.Service == "Mobile")
            {

                SqlCommand cmd = new SqlCommand("Update tbl_Customer_Mobile_Recharge set CashBackAmount=@CashBackAmount,CashBackstatus='Complete',cashbackdate=@Cdate where Id=" + id, con);
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@CashBackAmount", objcashback.Amount);
                else
                    cmd.Parameters.AddWithValue("@CashBackAmount", DBNull.Value);


                if (!string.IsNullOrEmpty(cdate.ToString()))
                    cmd.Parameters.AddWithValue("@Cdate", cdate);
                else
                    cmd.Parameters.AddWithValue("@Cdate", DBNull.Value);

                i = cmd.ExecuteNonQuery();
                con.Close();
            }


            if (objcashback.Service == "DTH")
            {

                SqlCommand cmd = new SqlCommand("Update tbl_Customer_Mobile_Recharge set CashBackAmount=@CashBackAmount,CashBackstatus='Complete',cashbackdate=@Cdate where Id=" + id, con);
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@CashBackAmount", objcashback.Amount);
                else
                    cmd.Parameters.AddWithValue("@CashBackAmount", DBNull.Value);


                if (!string.IsNullOrEmpty(cdate.ToString()))
                    cmd.Parameters.AddWithValue("@Cdate", cdate);
                else
                    cmd.Parameters.AddWithValue("@Cdate", DBNull.Value);

                i = cmd.ExecuteNonQuery();
                con.Close();
            }


            if (objcashback.Service == "Electricity")
            {

                SqlCommand cmd = new SqlCommand("Update tbl_Customer_bill_Pay set CashBackAmount=@CashBackAmount,CashBackstatus='Complete',cashbackdate=@Cdate where Id=" + id, con);
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@CashBackAmount", objcashback.Amount);
                else
                    cmd.Parameters.AddWithValue("@CashBackAmount", DBNull.Value);


                if (!string.IsNullOrEmpty(cdate.ToString()))
                    cmd.Parameters.AddWithValue("@Cdate", cdate);
                else
                    cmd.Parameters.AddWithValue("@Cdate", DBNull.Value);

                i = cmd.ExecuteNonQuery();
                con.Close();
            }

            if (objcashback.Service == "Gas")
            {

                SqlCommand cmd = new SqlCommand("Update tbl_Customer_bill_Pay set CashBackAmount=@CashBackAmount,CashBackstatus='Complete',cashbackdate=@Cdate where Id=" + id, con);
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@CashBackAmount", objcashback.Amount);
                else
                    cmd.Parameters.AddWithValue("@CashBackAmount", DBNull.Value);


                if (!string.IsNullOrEmpty(cdate.ToString()))
                    cmd.Parameters.AddWithValue("@Cdate", cdate);
                else
                    cmd.Parameters.AddWithValue("@Cdate", DBNull.Value);

                i = cmd.ExecuteNonQuery();
                con.Close();
            }
            return i;
        }











        public DataTable getCashbackBillListReport(string service, DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();

            
            if (service == null || service == "")
            {
                service = "Mobile/DTH";
            }
            if (service == "Mobile")
            {
                cmd = new SqlCommand("Select mr.Id AS Id,mr.CustomerId as CustomerId,cm.FirstName AS FirstName,cm.LastName As LastName,mr.rechargetype As RechargeType,bps.Name As OperatorName,mr.RechargeAmount As Amount,mr.rechargedate AS RechargeDate,mr.TransactionID1 AS TransactionId,mr.Rechargeno AS RechargeNo,mr.Responsemsg AS Responsemsg,mr.Status As Status From tbl_Customer_Mobile_Recharge mr Inner join tbl_Customer_Master cm ON cm.Id=mr.CustomerId Inner Join tbl_cashback_settings cs ON mr.OpeartorId=cs.OperatorId Inner Join tblBillPayOperators bps ON mr.OpeartorId=bps.OperatorCode where (@Service IS NULL OR mr.rechargetype = @Service) and (@FromDate is null or @ToDate is null or convert(varchar,mr.rechargedate,23) between @FromDate and @Todate) ORDER BY Status DESC", con);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@Service", service);
                else
                    cmd.Parameters.AddWithValue("@Service", DBNull.Value);


                if (!string.IsNullOrEmpty(FDate.ToString()))
                    cmd.Parameters.AddWithValue("@FromDate", FDate);
                else
                    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
                if (!string.IsNullOrEmpty(TDate.ToString()))
                    cmd.Parameters.AddWithValue("@ToDate", TDate);
                else
                    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }


            if (service == "Mobile/DTH")
            {
                cmd = new SqlCommand("Select mr.Id AS Id,mr.CustomerId as CustomerId,cm.FirstName AS FirstName,cm.LastName As LastName,mr.rechargetype As RechargeType,bps.Name As OperatorName,mr.RechargeAmount As Amount,mr.rechargedate AS RechargeDate,mr.TransactionID1 AS TransactionId,mr.Rechargeno AS RechargeNo,mr.Responsemsg AS Responsemsg,mr.Status As Status From tbl_Customer_Mobile_Recharge mr Inner join tbl_Customer_Master cm ON cm.Id=mr.CustomerId Inner Join tbl_cashback_settings cs ON mr.OpeartorId=cs.OperatorId Inner Join tblBillPayOperators bps ON mr.OpeartorId=bps.OperatorCode where  (@FromDate is null or @ToDate is null or convert(varchar,mr.rechargedate,23) between @FromDate and @Todate) ORDER BY Status DESC", con);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@Service", service);
                else
                    cmd.Parameters.AddWithValue("@Service", DBNull.Value);


                if (!string.IsNullOrEmpty(FDate.ToString()))
                    cmd.Parameters.AddWithValue("@FromDate", FDate);
                else
                    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
                if (!string.IsNullOrEmpty(TDate.ToString()))
                    cmd.Parameters.AddWithValue("@ToDate", TDate);
                else
                    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }

            if (service == "DTH")
            {
                cmd = new SqlCommand("Select mr.Id AS Id,mr.CustomerId as CustomerId,cm.FirstName AS FirstName,cm.LastName As LastName,mr.rechargetype As RechargeType,bps.Name As OperatorName,mr.RechargeAmount As Amount,mr.rechargedate AS RechargeDate,mr.TransactionID1 AS TransactionId,mr.Rechargeno AS RechargeNo,mr.Responsemsg AS Responsemsg,mr.Status As Status From tbl_Customer_Mobile_Recharge mr Inner join tbl_Customer_Master cm ON cm.Id=mr.CustomerId Inner Join tbl_cashback_settings cs ON mr.OpeartorId=cs.OperatorId Inner Join tblBillPayOperators bps ON mr.OpeartorId=bps.OperatorCode where (@Service IS NULL OR mr.rechargetype = @Service) and (@FromDate is null or @ToDate is null or convert(varchar,mr.rechargedate,23) between @FromDate and @Todate) ORDER BY Status DESC", con);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@Service", service);
                else
                    cmd.Parameters.AddWithValue("@Service", DBNull.Value);

                if (!string.IsNullOrEmpty(FDate.ToString()))
                    cmd.Parameters.AddWithValue("@FromDate", FDate);
                else
                    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
                if (!string.IsNullOrEmpty(TDate.ToString()))
                    cmd.Parameters.AddWithValue("@ToDate", TDate);
                else
                    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }



            if (service == "Electricity")
            {
                cmd = new SqlCommand("Select bp.Id AS Id,bp.CustomerId as CustomerId,cm.FirstName AS FirstName,cm.LastName As LastName,bp.type As RechargeType,bps.Name As OperatorName,bp.Amount As Amount,bp.billdate AS RechargeDate,bp.TransactionId As TransactionId,bp.numberCustomer AS RechargeNo,bp.ResMsg AS Responsemsg,bp.status AS Status From tbl_Customer_bill_Pay bp Inner join tbl_Customer_Master cm ON cm.Id=bp.CustomerId Inner Join tbl_cashback_settings cs ON bp.type=cs.Service Inner Join tblBillPayOperators bps ON bp.OperatorId=bps.OperatorCode  where (@Service IS NULL OR bp.type = @Service) and (@FromDate is null or @ToDate is null or convert(varchar,bp.billdate,23) between @FromDate and @Todate) ORDER BY bp.status DESC", con);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@Service", service);
                else
                    cmd.Parameters.AddWithValue("@Service", DBNull.Value);

                if (!string.IsNullOrEmpty(FDate.ToString()))
                    cmd.Parameters.AddWithValue("@FromDate", FDate);
                else
                    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
                if (!string.IsNullOrEmpty(TDate.ToString()))
                    cmd.Parameters.AddWithValue("@ToDate", TDate);
                else
                    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }


            if (service == "Gas")
            {
                cmd = new SqlCommand("Select bp.Id AS Id,bp.CustomerId as CustomerId,cm.FirstName AS FirstName,cm.LastName As LastName,bp.type As RechargeType,bps.Name As OperatorName,bp.Amount As Amount,bp.billdate AS RechargeDate,bp.TransactionId As TransactionId,bp.numberCustomer AS RechargeNo,bp.ResMsg AS Responsemsg,bp.status AS Status From tbl_Customer_bill_Pay bp Inner join tbl_Customer_Master cm ON cm.Id=bp.CustomerId Inner Join tbl_cashback_settings cs ON bp.type=cs.Service Inner Join tblBillPayOperators bps ON bp.OperatorId=bps.OperatorCode  where (@Service IS NULL OR bp.type = @Service) and (@FromDate is null or @ToDate is null or convert(varchar,bp.billdate,23) between @FromDate and @Todate) ORDER BY bp.status DESC", con);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@Service", service);
                else
                    cmd.Parameters.AddWithValue("@Service", DBNull.Value);

                if (!string.IsNullOrEmpty(FDate.ToString()))
                    cmd.Parameters.AddWithValue("@FromDate", FDate);
                else
                    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
                if (!string.IsNullOrEmpty(TDate.ToString()))
                    cmd.Parameters.AddWithValue("@ToDate", TDate);
                else
                    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;




            }


            if (service == "Electricty/Gas")
            {
                cmd = new SqlCommand("Select bp.Id AS Id,bp.CustomerId as CustomerId,cm.FirstName AS FirstName,cm.LastName As LastName,bp.type As RechargeType,bps.Name As OperatorName,bp.Amount As Amount,bp.billdate AS RechargeDate,bp.TransactionId As TransactionId,bp.numberCustomer AS RechargeNo,bp.ResMsg AS Responsemsg,bp.status AS Status From tbl_Customer_bill_Pay bp Inner join tbl_Customer_Master cm ON cm.Id=bp.CustomerId Inner Join tbl_cashback_settings cs ON bp.type=cs.Service Inner Join tblBillPayOperators bps ON bp.OperatorId=bps.OperatorCode  where  (@FromDate is null or @ToDate is null or convert(varchar,bp.billdate,23) between @FromDate and @Todate) ORDER BY bp.status DESC", con);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@Service", service);
                else
                    cmd.Parameters.AddWithValue("@Service", DBNull.Value);


                if (!string.IsNullOrEmpty(FDate.ToString()))
                    cmd.Parameters.AddWithValue("@FromDate", FDate);
                else
                    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
                if (!string.IsNullOrEmpty(TDate.ToString()))
                    cmd.Parameters.AddWithValue("@ToDate", TDate);
                else
                    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            // SqlCommand 


            //cmd = new SqlCommand("Select * From tbl_Customer_Mobile_Recharge where (@Service IS NULL OR [rechargetype] = @Service) AND (CashBackstatus IS NULL OR CashBackstatus<>'Complete')", con);
            //cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(Id.ToString()))
            //    cmd.Parameters.AddWithValue("@Service", service);
            //else
            //    cmd.Parameters.AddWithValue("@Service", DBNull.Value);
            //da = new SqlDataAdapter(cmd);
            //da.Fill(dt);
            //return dt;
            return dt;

        }




        public DataTable getCashbackListReport(string service, DateTime? FDate, DateTime? TDate)
        {
            //con.Open();
            SqlCommand cmd;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            if (service == null || service == "")
            {
                service = "Flipkart/Amazon";
            }

            if(service== "Flipkart/Amazon")
            {
                cmd = new SqlCommand("Select CustomerId As CustomerId,FirstName As FirstName,LastName AS LastName,OrderFrom AS RechargeType,OrderFrom AS OperatorName,OrderDate As RechargeDate,OrderId AS TransactionId,CashBackAmount AS CashBackAmount,cashbackdate AS cashbackdate,CashBackstatus As CashBackstatus From tbl_flip_amz_Order where CashBackstatus='Complete' and (@FromDate is null or @ToDate is null or convert(varchar,cashbackdate,23) between @FromDate and @Todate) ORDER BY Id ASC ", con);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@Service", service);
                else
                    cmd.Parameters.AddWithValue("@Service", DBNull.Value);


                if (!string.IsNullOrEmpty(FDate.ToString()))
                    cmd.Parameters.AddWithValue("@FromDate", FDate);
                else
                    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
                if (!string.IsNullOrEmpty(TDate.ToString()))
                    cmd.Parameters.AddWithValue("@ToDate", TDate);
                else
                    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);

                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }


            if (service == "Mobile")
            {
                string query = "SELECT mr.CustomerId AS CustomerId,cm.FirstName AS FirstName, cm.LastName As LastName,mr.rechargetype AS RechargeType,";
                query += " bp.Name AS OperatorName,mr.RechargeAmount As RechargeAmount,mr.rechargedate AS RechargeDate,mr.TransactionID1 AS TransactionId,mr.Rechargeno As RechargeNo,mr.CashBackAmount As CashBackAmount,";
                query += " mr.cashbackdate AS cashbackdate,mr.CashBackstatus AS CashBackstatus";
                query += " FROM tbl_Customer_Mobile_Recharge mr inner join tbl_Customer_Master cm on mr.CustomerId=cm.Id";
                query += " Inner Join tblBillPayOperators bp ON mr.OpeartorId=bp.OperatorCode";
                query += " where (@Service IS NULL OR mr.rechargetype = @Service) AND (mr.CashBackstatus IS NULL OR mr.CashBackstatus='Complete') AND mr.Status=1 and (@FromDate is null or @ToDate is null or convert(varchar,mr.cashbackdate,23) between @FromDate and @Todate)";


                cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@Service", service);
                else
                    cmd.Parameters.AddWithValue("@Service", DBNull.Value);
                if (!string.IsNullOrEmpty(FDate.ToString()))
                    cmd.Parameters.AddWithValue("@FromDate", FDate);
                else
                    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
                if (!string.IsNullOrEmpty(TDate.ToString()))
                    cmd.Parameters.AddWithValue("@ToDate", TDate);
                else
                    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }

            if (service == "DTH")
            {
                string query = "SELECT mr.CustomerId AS CustomerId,cm.FirstName AS FirstName, cm.LastName As LastName,mr.rechargetype AS RechargeType,";
                query += " bp.Name AS OperatorName,mr.RechargeAmount As RechargeAmount,mr.rechargedate AS RechargeDate,mr.TransactionID1 AS TransactionId,mr.Rechargeno As RechargeNo,mr.CashBackAmount As CashBackAmount,";
                query += " mr.cashbackdate AS cashbackdate,mr.CashBackstatus AS CashBackstatus";
                query += " FROM tbl_Customer_Mobile_Recharge mr inner join tbl_Customer_Master cm on mr.CustomerId=cm.Id";
                query += " Inner Join tblBillPayOperators bp ON mr.OpeartorId=bp.OperatorCode";
                query += " where (@Service IS NULL OR mr.rechargetype = @Service) AND (mr.CashBackstatus IS NULL OR mr.CashBackstatus='Complete') AND mr.Status=1 and (@FromDate is null or @ToDate is null or convert(varchar,mr.cashbackdate,23) between @FromDate and @Todate)";


                cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@Service", service);
                else
                    cmd.Parameters.AddWithValue("@Service", DBNull.Value);

                if (!string.IsNullOrEmpty(FDate.ToString()))
                    cmd.Parameters.AddWithValue("@FromDate", FDate);
                else
                    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
                if (!string.IsNullOrEmpty(TDate.ToString()))
                    cmd.Parameters.AddWithValue("@ToDate", TDate);
                else
                    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }



            if (service == "Electricity")
            {

                string query = "SELECT bp.CustomerId AS CustomerId,cm.FirstName AS FirstName, cm.LastName As LastName,bp.type AS RechargeType,";
                query += " bo.Name AS OperatorName,bp.Amount As RechargeAmount,bp.billdate AS RechargeDate,bp.TransactionID AS TransactionId,bp.numberCustomer As RechargeNo,bp.CashBackAmount As CashBackAmount,";
                query += " bp.cashbackdate AS cashbackdate,bp.CashBackstatus AS CashBackstatus";
                query += " FROM tbl_Customer_bill_Pay bp inner join tbl_Customer_Master cm on bp.CustomerId=cm.Id";
                query += " Inner Join tblBillPayOperators bo ON bp.OperatorId=bo.OperatorCode ";
                query += " where (@Service IS NULL OR bp.type = @Service) AND (bp.CashBackstatus IS NULL OR bp.CashBackstatus='Complete') and (@FromDate is null or @ToDate is null or convert(varchar,bp.cashbackdate,23) between @FromDate and @Todate)";


                cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@Service", service);
                else
                    cmd.Parameters.AddWithValue("@Service", DBNull.Value);

                if (!string.IsNullOrEmpty(FDate.ToString()))
                    cmd.Parameters.AddWithValue("@FromDate", FDate);
                else
                    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
                if (!string.IsNullOrEmpty(TDate.ToString()))
                    cmd.Parameters.AddWithValue("@ToDate", TDate);
                else
                    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }


            if (service == "Gas")
            {
                string query = "SELECT bp.CustomerId AS CustomerId,cm.FirstName AS FirstName, cm.LastName As LastName,bp.type AS RechargeType,";
                query += " bo.Name AS OperatorName,bp.Amount As RechargeAmount,bp.billdate AS RechargeDate,bp.TransactionID AS TransactionId,bp.numberCustomer As RechargeNo,bp.CashBackAmount As CashBackAmount,";
                query += " bp.cashbackdate AS cashbackdate,bp.CashBackstatus AS CashBackstatus";
                query += " FROM tbl_Customer_bill_Pay bp inner join tbl_Customer_Master cm on bp.CustomerId=cm.Id";
                query += " Inner Join tblBillPayOperators bo ON bp.OperatorId=bo.OperatorCode ";
                query += " where (@Service IS NULL OR bp.type = @Service) AND (bp.CashBackstatus IS NULL OR bp.CashBackstatus='Complete')and (@FromDate is null or @ToDate is null or convert(varchar,bp.cashbackdate,23) between @FromDate and @Todate)";


                cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(Id.ToString()))
                    cmd.Parameters.AddWithValue("@Service", service);
                else
                    cmd.Parameters.AddWithValue("@Service", DBNull.Value);

                if (!string.IsNullOrEmpty(FDate.ToString()))
                    cmd.Parameters.AddWithValue("@FromDate", FDate);
                else
                    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
                if (!string.IsNullOrEmpty(TDate.ToString()))
                    cmd.Parameters.AddWithValue("@ToDate", TDate);
                else
                    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
            // SqlCommand 


            //cmd = new SqlCommand("Select * From tbl_Customer_Mobile_Recharge where (@Service IS NULL OR [rechargetype] = @Service) AND (CashBackstatus IS NULL OR CashBackstatus<>'Complete')", con);
            //cmd.CommandType = CommandType.Text;
            //if (!string.IsNullOrEmpty(Id.ToString()))
            //    cmd.Parameters.AddWithValue("@Service", service);
            //else
            //    cmd.Parameters.AddWithValue("@Service", DBNull.Value);
            //da = new SqlDataAdapter(cmd);
            //da.Fill(dt);
            //return dt;
            return dt;

        }


        public int UpdateCashbackSetting(CashBack obj)
        {
            int i = 0;
            try
            {
                con.Open();


                SqlCommand com = new SqlCommand("Update tbl_cashback_settings set [Service]=@Service,[ProviderName]=@ProviderName,[OperatorId]=@OperatorId,[Type]=@Type,[Amount1]=@Amount WHERE (@Id IS NULL OR [Id] = @Id)", con);
                com.CommandType = CommandType.Text;
                com.Parameters.AddWithValue("@Id", obj.Id);
                com.Parameters.AddWithValue("@Service", obj.Service);
                com.Parameters.AddWithValue("@ProviderName", obj.ProviderName);
                if (!string.IsNullOrEmpty(obj.OperatorCode.ToString()))
                    com.Parameters.AddWithValue("@OperatorId", obj.OperatorCode);
                else
                    com.Parameters.AddWithValue("@OperatorId", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Type))
                    com.Parameters.AddWithValue("@Type", obj.Type);
                else
                    com.Parameters.AddWithValue("@Type", DBNull.Value);
                if (!string.IsNullOrEmpty(obj.Amount.ToString()))
                    com.Parameters.AddWithValue("@Amount", Convert.ToDecimal(obj.Amount));
                else
                    com.Parameters.AddWithValue("@Amount", DBNull.Value);
                
                i = com.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            { }
            return i;

        }
    }
}
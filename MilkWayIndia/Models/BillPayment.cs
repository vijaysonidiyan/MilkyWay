using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MilkWayIndia.Models
{
	public class PaymentSourcePlatformFeesModel
	{
		public int PaymentSourceId { get; set; }
		public string PaymentSource { get; set; }
		public bool IsPriceRangeApplicable { get; set; }
		public List<PaymentSourcePlatformFeesDetailModel> detail { get; set; }
	}

	public class PaymentSourcePlatformFeesDetailModel
	{
		public decimal FromPrice { get; set; }
		public decimal ToPrice { get; set; }		
		public decimal Percentage { get; set; }
		public decimal LumsumAmount { get; set; }
		public decimal PlatformChargesPercentage { get; set; }
		public decimal PlatformChargesLumsumAmount { get; set; }
	}
	public class BillPayment
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
		public string CircleCode { get; set; }
		public string OperatorId { get; set; }
		public int Status { get; set; }
		public string Responsemsg { get; set; }
		public string Marginper { get; set; }
		public Decimal Marginamnt { get; set; }
		public string TransactionId { get; set; }
		public Decimal RechargeAmount { get; set; }
		public string RechargeType { get; set; }

		public string NumberCustomer { get; set; }
		public string Lat { get; set; }
		public string Lon { get; set; }
		public string PaymentMode { get; set; }
		public string CustomerMobile { get; set; }

		public string FieldTag1 { get; set; }
		public string FieldTag2 { get; set; }
		public string FieldTag3 { get; set; }
		public string Billstatus { get; set; }

		public string errorcode { get; set; }
		public string Agentid { get; set; }




		public string CashbackStatus { get; set; }
		public string CashbackAmount { get; set; }
		public DateTime? CashbackDate { get; set; }

		public string UtransactionId1 { get; set; }
		public DataTable GetProvider(string type, int? circlecode)
		{
			con.Open();
			DataTable dt = new DataTable();
			if (circlecode == 51)
			{

				SqlCommand cmd = new SqlCommand("SELECT bo.Name,bs.Name as sname,bp.*,bc.Name As statename,bc.CircleCode As Circlecode from tblBillPayOperators bo Inner join tblBillPayProviders bp ON bo.ID=bp.OperatorID INNER Join tblBillPayServices bs ON bp.ServiceID=bs.ID INNER Join tblBillPayCircles bc ON bp.CircleID=bc.ID where bo.Type=@Type and bc.CircleCode=@CircleCode", con);
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@Type", type);
				cmd.Parameters.AddWithValue("@CircleCode", circlecode);
				SqlDataAdapter da = new SqlDataAdapter(cmd);

				da.Fill(dt);
			}

			else
			{
				SqlCommand cmd = new SqlCommand("SELECT bo.Name,bs.Name as sname,bp.*,bc.Name As statename,bc.CircleCode As Circlecode from tblBillPayOperators bo Inner join tblBillPayProviders bp ON bo.ID=bp.OperatorID INNER Join tblBillPayServices bs ON bp.ServiceID=bs.ID INNER Join tblBillPayCircles bc ON bp.CircleID=bc.ID where bo.Type=@Type and (bc.CircleCode=@CircleCode or bc.CircleCode=51)", con);
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@Type", type);
				cmd.Parameters.AddWithValue("@CircleCode", circlecode);
				SqlDataAdapter da = new SqlDataAdapter(cmd);

				da.Fill(dt);
			}

			con.Close();
			return dt;
		}


		public DataTable GetPaymentSourcePlatformFeesdt()
		{
			con.Open();
			DataTable dt = new DataTable();

			SqlCommand cmd = new SqlCommand("Select tp.PaymentSource,tps.PaymentSourceId,tps.FromPrice,tps.ToPrice,tps.IsPriceRangeApplicable,tps.percentage,tps.LumsumAmount ,tps.PlatformChargespercentage,tps.PlatformChargesLumsumAmount from tbl_PaymentSourceWisePlatformFees tps inner join tbl_PaymentSourceMaster tp on tps.PaymentSourceId = tp.Id", con);
			cmd.CommandType = CommandType.Text;
			SqlDataAdapter da = new SqlDataAdapter(cmd);
			da.Fill(dt);

			con.Close();
			return dt;
		}

		public List<PaymentSourcePlatformFeesModel> GetPaymentSourcePlatformFees()
		{
			var result = new List<PaymentSourcePlatformFeesModel>();

			using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MilkWayIndia"].ConnectionString))
			{
				con.Open();

				string query = @"SELECT tp.PaymentSource, tps.PaymentSourceId, tps.FromPrice, tps.ToPrice, 
		                        tps.IsPriceRangeApplicable, tps.Percentage, tps.LumsumAmount, 
		                        tps.PlatformChargesPercentage, tps.PlatformChargesLumsumAmount
		                FROM tbl_PaymentSourceWisePlatformFees tps
		                INNER JOIN tbl_PaymentSourceMaster tp ON tps.PaymentSourceId = tp.Id";

				using (SqlCommand cmd = new SqlCommand(query, con))
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					var dict = new Dictionary<int, PaymentSourcePlatformFeesModel>();

					while (reader.Read())
					{
						int sourceId = reader.GetInt32(reader.GetOrdinal("PaymentSourceId"));

						if (!dict.ContainsKey(sourceId))
						{
							dict[sourceId] = new PaymentSourcePlatformFeesModel
							{
								PaymentSourceId = sourceId,
								PaymentSource = reader.GetString(reader.GetOrdinal("PaymentSource")),
								detail = new List<PaymentSourcePlatformFeesDetailModel>()
							};
						}

						var detail = new PaymentSourcePlatformFeesDetailModel
						{
							FromPrice = reader["FromPrice"] != DBNull.Value ? Convert.ToDecimal(reader["FromPrice"]) : 0,
							ToPrice = reader["ToPrice"] != DBNull.Value ? Convert.ToDecimal(reader["ToPrice"]) : 0,
							//IsPriceRangeApplicable = reader["IsPriceRangeApplicable"] != DBNull.Value ? Convert.ToBoolean(reader["IsPriceRangeApplicable"]) : false,
							Percentage = reader["Percentage"] != DBNull.Value ? Convert.ToDecimal(reader["Percentage"]) : 0,
							LumsumAmount = reader["LumsumAmount"] != DBNull.Value ? Convert.ToDecimal(reader["LumsumAmount"]) : 0,
							PlatformChargesPercentage = reader["PlatformChargesPercentage"] != DBNull.Value ? Convert.ToDecimal(reader["PlatformChargesPercentage"]) : 0,
							PlatformChargesLumsumAmount = reader["PlatformChargesLumsumAmount"] != DBNull.Value ? Convert.ToDecimal(reader["PlatformChargesLumsumAmount"]) : 0
						};

						dict[sourceId].detail.Add(detail);
					}

					result = dict.Values.ToList();
				}
			}

			return result;
		}

		public DataTable GetCircleProvider(string Operatorcode)
		{
			con.Open();
			DataTable dt = new DataTable();

			SqlCommand cmd = new SqlCommand("SELECT bo.Name,bs.Name as sname,bp.*,bc.Name As statename,bc.CircleCode As Circlecode from tblBillPayOperators bo Inner join tblBillPayProviders bp ON bo.ID=bp.OperatorID INNER Join tblBillPayServices bs ON bp.ServiceID=bs.ID INNER Join tblBillPayCircles bc ON bp.CircleID=bc.ID where bp.OperatorCode=@OpeartorCode", con);
			cmd.CommandType = CommandType.Text;
			cmd.Parameters.AddWithValue("@OpeartorCode", Operatorcode);

			SqlDataAdapter da = new SqlDataAdapter(cmd);

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


		public int insertrecharge(BillPayment objrecharge)
		{
			int i = 0;
			try
			{
				//if (CustomerId == 0) CustomerId = null;
				//if (ProductId == 0) ProductId = null;

				con.Open();
				SqlCommand cmd = new SqlCommand("INSERT INTO tbl_Customer_bill_Pay([CustomerId],[UtransactionId],[numberCustomer],[Amount],[Lat],[Lon],[OperatorId],[paymentmode],[customermobile],[field1],[field2],[field3],[errorcode],[TransactionId],[status],[ResMsg],[AgentId],type,billdate,billstatus)VALUES(@CustomerId,@Utransactionid,@numberCustomer,@Amount,@Lat,@Lon,@OperatorId,@paymentmode,@customermobile,@field1,@field2,@field3,@errorcode,@TransactionId,@status,@ResMsg,@AgentId,@type,@billdate,@billstatus)", con);
				cmd.CommandType = CommandType.Text;
				if (!string.IsNullOrEmpty(CustomerId.ToString()))
					cmd.Parameters.AddWithValue("@CustomerId", objrecharge.CustomerId);
				else
					cmd.Parameters.AddWithValue("@CustomerId", DBNull.Value);


				cmd.Parameters.AddWithValue("@Utransactionid", objrecharge.UtransactionId);



				cmd.Parameters.AddWithValue("@numberCustomer", objrecharge.NumberCustomer);
				cmd.Parameters.AddWithValue("@Amount", objrecharge.RechargeAmount);
				cmd.Parameters.AddWithValue("@Lat", objrecharge.Lat);
				cmd.Parameters.AddWithValue("@Lon", objrecharge.Lon);
				cmd.Parameters.AddWithValue("@OperatorId", objrecharge.OperatorId);
				cmd.Parameters.AddWithValue("@paymentmode", objrecharge.PaymentMode);
				cmd.Parameters.AddWithValue("@customermobile", objrecharge.CustomerMobile);
				cmd.Parameters.AddWithValue("@field1", objrecharge.FieldTag1);
				cmd.Parameters.AddWithValue("@field2", objrecharge.FieldTag2);
				cmd.Parameters.AddWithValue("@field3", objrecharge.FieldTag3);


				cmd.Parameters.AddWithValue("@errorcode", objrecharge.errorcode);
				cmd.Parameters.AddWithValue("@TransactionId", objrecharge.TransactionId);
				cmd.Parameters.AddWithValue("@status", objrecharge.Status);
				cmd.Parameters.AddWithValue("@ResMsg", objrecharge.Responsemsg);
				cmd.Parameters.AddWithValue("@AgentId", objrecharge.Agentid);
				cmd.Parameters.AddWithValue("@type", objrecharge.RechargeType);
				cmd.Parameters.AddWithValue("@billdate", objrecharge.FromDate);
				cmd.Parameters.AddWithValue("@billstatus", objrecharge.Billstatus);



				SqlCommand cmd1 = new SqlCommand("INSERT INTO tbl_Utransaction(UtransactionId,UtransactionIdnew)VALUES(@UtransactionId,@UtransactionIdnew)", con);
				cmd1.CommandType = CommandType.Text;

				cmd1.Parameters.AddWithValue("@UtransactionId", objrecharge.UtransactionId1);
				cmd1.Parameters.AddWithValue("@UtransactionIdnew", objrecharge.UtransactionId);





				i = cmd1.ExecuteNonQuery();


				i = cmd.ExecuteNonQuery();
				con.Close();
			}
			catch (Exception ex) { string s = ex.Message; }
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


		public int Updaterecharge(BillPayment objrecharge)
		{




			int i = 0;

			//if (CustomerId == 0) CustomerId = null;
			//if (ProductId == 0) ProductId = null;

			con.Open();
			SqlCommand cmd = new SqlCommand("UPDATE tbl_Customer_bill_Pay SET CashBackstatus=@CashBackstatus,CashBackAmount=@CashBackAmount,cashbackdate=@cashbackdate,billstatus=@billstatus where Utransactionid=@Utransactionid", con);
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.AddWithValue("@CashBackstatus", objrecharge.CashbackStatus);
			cmd.Parameters.AddWithValue("@CashBackAmount", objrecharge.CashbackAmount);



			cmd.Parameters.AddWithValue("@cashbackdate", objrecharge.CashbackDate);
			cmd.Parameters.AddWithValue("@billstatus", objrecharge.Billstatus);
			cmd.Parameters.AddWithValue("@Utransactionid", objrecharge.UtransactionId);
			i = cmd.ExecuteNonQuery();
			con.Close();



			return i;

		}


		public int Updaterecharge1(BillPayment objrecharge)
		{




			int i = 0;

			//if (CustomerId == 0) CustomerId = null;
			//if (ProductId == 0) ProductId = null;

			con.Open();
			SqlCommand cmd = new SqlCommand("UPDATE tbl_Customer_bill_Pay SET status=@Status,ResMsg=@Responsemsg,TransactionId=@TransactionID1,CashBackstatus=@CashBackstatus,CashBackAmount=@CashBackAmount,cashbackdate=@cashbackdate,billstatus=@billstatus where UtransactionId=@Utransactionid", con);
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.AddWithValue("@Status", objrecharge.Status);
			cmd.Parameters.AddWithValue("@Responsemsg", objrecharge.Responsemsg);

			cmd.Parameters.AddWithValue("@TransactionID1", objrecharge.TransactionId);




			cmd.Parameters.AddWithValue("@CashBackstatus", objrecharge.CashbackStatus);
			cmd.Parameters.AddWithValue("@CashBackAmount", objrecharge.CashbackAmount);



			cmd.Parameters.AddWithValue("@cashbackdate", objrecharge.CashbackDate);



			cmd.Parameters.AddWithValue("@billstatus", objrecharge.Billstatus);
			cmd.Parameters.AddWithValue("@Utransactionid", objrecharge.UtransactionId);
			i = cmd.ExecuteNonQuery();













			//SqlCommand cmd1 = new SqlCommand("UPDATE tbl_Customer_Wallet SET Description=@Description where Utransactionid=@Utransactionid", con);
			//cmd1.CommandType = CommandType.Text;

			//cmd.Parameters.AddWithValue("@Status", objrecharge.Status);

			//cmd.Parameters.AddWithValue("@Utransactionid", objrecharge.UtransactionId);
			//i = cmd1.ExecuteNonQuery();





			con.Close();



			return i;

		}




		public int Updaterecharge2(BillPayment objrecharge)
		{




			int i = 0;

			//if (CustomerId == 0) CustomerId = null;
			//if (ProductId == 0) ProductId = null;
			objrecharge.CashbackAmount = "0";
			objrecharge.CashbackStatus = "Fail";
			con.Open();
			SqlCommand cmd = new SqlCommand("UPDATE tbl_Customer_bill_Pay SET status=@Status,ResMsg=@Responsemsg,TransactionId=@TransactionID1,CashBackstatus=@CashBackstatus,CashBackAmount=@CashBackAmount,cashbackdate=@cashbackdate,billstatus=@billstatus where UtransactionId=@Utransactionid", con);
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.AddWithValue("@Status", objrecharge.Status);
			cmd.Parameters.AddWithValue("@Responsemsg", objrecharge.Responsemsg);

			cmd.Parameters.AddWithValue("@TransactionID1", objrecharge.TransactionId);




			cmd.Parameters.AddWithValue("@CashBackstatus", objrecharge.CashbackStatus);
			cmd.Parameters.AddWithValue("@CashBackAmount", objrecharge.CashbackAmount);



			cmd.Parameters.AddWithValue("@cashbackdate", objrecharge.CashbackDate);



			cmd.Parameters.AddWithValue("@billstatus", objrecharge.Billstatus);
			cmd.Parameters.AddWithValue("@Utransactionid", objrecharge.UtransactionId);
			i = cmd.ExecuteNonQuery();



			SqlCommand cmd1 = new SqlCommand("DELETE from tbl_Customer_Wallet where UtransactionId=@Utransactionid", con);


			cmd1.Parameters.AddWithValue("@Utransactionid", objrecharge.UtransactionId);





			i = cmd1.ExecuteNonQuery();









			//SqlCommand cmd1 = new SqlCommand("UPDATE tbl_Customer_Wallet SET Description=@Description where Utransactionid=@Utransactionid", con);
			//cmd1.CommandType = CommandType.Text;

			//cmd.Parameters.AddWithValue("@Status", objrecharge.Status);

			//cmd.Parameters.AddWithValue("@Utransactionid", objrecharge.UtransactionId);
			//i = cmd1.ExecuteNonQuery();





			con.Close();



			return i;

		}
	}
}
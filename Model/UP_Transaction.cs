using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using Microsoft.VisualBasic.CompilerServices;
using Register.ModClasses;
namespace Register
{

    public partial class UP_Transaction
    {
        public string Trans_ID;
        public string Name, Email, PaymentReference, Trans_Status, Trans_Remark, TransProcState, MerchantReference;
        public string Phone = string.Empty;
        public string CustomerID;
        public UPConfigParams.UP_Currency CurrencyCode;
        public DateTime Trans_Date;
        public decimal Trans_Amt;
        public decimal Trans_Charge;
        private string con;
        public int ServiceId, Unit;
        public int PaymentSourceId;
        public string ServiceName;
        public decimal Cost;
        public decimal ServiceCost;
        public string PaymentType;

        public string ResponseCode;
        public string RRR, SessionID;
        public string Currency;
        public string ResponseDesc;
        public string CentreCode;
        public byte[] PasswordSerial;
        private DataTable TransTable = new DataTable();

        public UP_Transaction()
        {
            // con = Utility.DbConnectionString
            Trans_Status = "Pending";
            Trans_Date = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(TimeZone.CurrentTimeZone.ToUniversalTime(DateTime.Now), "W. Central Africa Standard Time");

        }


        public UP_Transaction(string TransactionID) : this()
        {

            try
            {
                TransTable = SelectUPTransaction(TransactionID);
                if (TransTable is null)
                {
                    throw new Exception("Error initializing data");
                }
                else if (TransTable.Rows.Count > 0)
                {

                    var row = TransTable.Rows[0];
                    Trans_ID = TransactionID;

                    Trans_Charge = Conversions.ToDecimal(row["Trans_Charge"]);
                    TransProcState = Conversions.ToString(row["TransProcState"]);
                    ServiceId = Conversions.ToInteger(row["ServiceId"]); 
                    if (!(row["CentreCode"] is DBNull))
                    {
                        CentreCode = Conversions.ToString(row["CentreCode"]);
                    }
                    if (!(row["CustomerID"] is DBNull))
                    {
                        CustomerID = Conversions.ToString(row["CustomerID"]);
                    }
                    if (!(row["RRR"] is DBNull))
                    {
                        RRR = Conversions.ToString(row["RRR"]);
                    }

                    if (!(row["SessionID"] is DBNull))
                    {
                        SessionID = Conversions.ToString(row["SessionID"]);
                    }

                    if (!(row["Email"] is DBNull))
                    {
                        Email = Conversions.ToString(row["Email"]);
                    }

                    if (!(row["Name"] is DBNull))
                    {
                        Name = Conversions.ToString(row["Name"]);
                    }

                    if (!(row["PaymentReference"] is DBNull))
                    {
                        PaymentReference = Conversions.ToString(row["PaymentReference"]);
                        //PaymentReference = Register.ModClasses.Payments.GetOrders();
                    }

                    if (!(row["MarchantReference"] is DBNull))
                    {
                        MerchantReference = Conversions.ToString(row["MarchantReference"]);
                    }

                    if (!(row["Phone"] is DBNull))
                    {
                        Phone = Conversions.ToString(row["Phone"]);
                    }

                    if (!(row["Trans_Amt"] is DBNull))
                    {
                        Trans_Amt = Conversions.ToDecimal(row["Trans_Amt"]);
                    }

                    if (!(row["Trans_Date"] is DBNull))
                    {
                        Trans_Date = Conversions.ToDate(row["Trans_Date"]);
                    }

                    if (!(row["Trans_Remark"] is DBNull))
                    {
                        Trans_Remark = Conversions.ToString(row["Trans_Remark"]);
                    }
                    if (!(row["Unit"] is DBNull))
                    {
                        Unit = Conversions.ToInteger(row["Unit"]);
                    }

                    if (!(row["Trans_Status"] is DBNull))
                    {
                        Trans_Status = Conversions.ToString(row["Trans_Status"]);
                    }

                    if (!(row["ResponseCode"] is DBNull))
                    {
                        ResponseCode = Conversions.ToString(row["ResponseCode"]);
                    }

                    if (!(row["Cost"] is DBNull))
                    {
                        Cost = Conversions.ToDecimal(row["Cost"]);
                    } 

                    if (!(row["Currency"] is DBNull))
                    {
                        Currency = Conversions.ToString(row["Currency"]);
                    }

                    if (!(row["PaymentTypeId"] is DBNull))
                    {
                        PaymentType = Conversions.ToString(row["serviceName"]);
                        ServiceName = Conversions.ToString(row["serviceName"]);
                    }

                }
            }
            catch (Exception ex)
            {

            }


        } 
        private static string DbConnectionString = ConfigurationSettings.AppSettings["ConnectionString"];
        public DataTable SelectUPTransaction(string TransactionID)
        {
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TransactionIDoreMail", TransactionID) };
            var dt = new DataTable();
            try
            {
                dt = SqlHelper.ExecuteDataset(DbConnectionString, CommandType.StoredProcedure, "Payments_SelectpaymentdTransactionbyTransID", param).Tables[0];
            }
            catch (Exception ex)
            {
                throw new Exception("Unable To Connect To Server");
                // Throw ex
            }
            return dt;
        }
        public DataTable SelectUPTransaction_Brief(string TransactionID)
        {
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TransactionID", TransactionID) };
            var dt = new DataTable();
            try
            {
                dt = SqlHelper.ExecuteDataset(DbConnectionString, CommandType.StoredProcedure, "SelectInterswichTransactionbyTransID_UP", param).Tables[0];
            }
            catch (Exception ex)
            {
                throw new Exception("Unable To Connect To Server");
                // Throw ex
            }
            return dt;
        }
        public DataTable SelectUPTransaction_BriefBulk(string IDSets)
        {
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@IDSets", IDSets) };
            var dt = new DataTable();
            try
            {
                dt = SqlHelper.ExecuteDataset(DbConnectionString, CommandType.StoredProcedure, "SelectInterswichTransactionbyTransID_UPSets", param).Tables[0];
            }
            catch (Exception ex)
            {
                throw new Exception("Unable To Connect To Server");
                // Throw ex
            }
            return dt;
        }
        public static DataTable SelectUnusedUPTransaction(string Email, int PaymentTypeId, int GatewayID = 2)
        {
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@Email", Email), new SqlParameter("@PaymentTypeId", PaymentTypeId), new SqlParameter("@GatewayID", GatewayID) };
            var dt = new DataTable();
            try
            {
                dt = SqlHelper.ExecuteDataset(DbConnectionString, CommandType.StoredProcedure, "GetUnUsedTransactionViaRR", param).Tables[0];
            }
            catch (Exception ex)
            {
                throw new Exception("Unable To Connect To Server");
                // Throw ex
            }
            return dt;
        }

        public static UP_Transaction IntializeTransaction(UP_PaymentParameters Param)
        {
            try
            {

                var trans = new UP_Transaction();
                trans.CurrencyCode = Param.currencyCode;
                trans.CustomerID = Param.CustomerID;
                trans.Name = Param.Fullname;
                trans.Phone = Param.ContactPhoneNo is null ? "" : Param.ContactPhoneNo;
                trans.Email = Param.ContactEmail;
                trans.ServiceName = Param.ServiceName;
                trans.Unit = Param.Unit; 
                trans.Cost = Param.Cost;
                trans.CentreCode = Param.CentreCode;
                trans.ServiceId = Param.ServiceId;
                trans.PasswordSerial = Param.Password;
                trans.PaymentSourceId = Param.PaymentSourceId;
                trans.Trans_Charge = Param.ServiceCharge; // 0m; // ' TrxChargeNYSC + TrxChargeSidmach
                trans.Trans_Amt = Param.Cost * Param.Unit; // ' + TrxChargeNYSC + TrxChargeSidmach 



                //string transidret = trans.SaveTransaction(Param.PaymentGatewayId);
                string transidret = Payments.SaveTransaction(ref trans, Param.PaymentGatewayId);

                if (string.IsNullOrEmpty(transidret))
                {

                    return null;
                }
                else
                {
                    return trans;
                }
            }
            catch (Exception ex)
            {
                General.logger.WriteLog(ex);
                throw ex;
            }

        }

        public string SaveTransaction(int GateWayID = 1)
        {
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@Name", Name), new SqlParameter("@CustomerID", CustomerID), 
                new SqlParameter("@Email", Email), new SqlParameter("@Phone", Phone), new SqlParameter("@Trans_Status", Trans_Status),
                new SqlParameter("@Trans_Date", Trans_Date), new SqlParameter("@Trans_Amt", Trans_Amt),
                new SqlParameter("@PasswordSerial", PasswordSerial), new SqlParameter("@Trans_Remark", Trans_Remark), 
                new SqlParameter("@ServiceCharge", Trans_Charge), new SqlParameter("@Cost", Cost), new SqlParameter("@CentreCode", CentreCode),
                new SqlParameter("@ServiceName", ServiceName),new SqlParameter("@Unit", Unit), new SqlParameter("@PaymentSourceId", PaymentSourceId),
                new SqlParameter("@bankPaymentPrefix", WebConfigurationManager.AppSettings["BankPaymentMerchantID"].ToString()), 
                new SqlParameter("@PaymentTypeId", ServiceId), new SqlParameter("@GateWayID", GateWayID)};



            var cn = new SqlConnection(DbConnectionString);
            var cmd = new SqlCommand("Payments_InsertTransaction", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(param);
            var returnvalue = cmd.Parameters.Add("@Trans_ID", SqlDbType.VarChar, 20);
            // Dim returnvalue_Amt As SqlParameter = cmd.Parameters.Add("@Trans_Amt", SqlDbType.Money)
            returnvalue.Direction = ParameterDirection.Output;
            // returnvalue_Amt.Direction = ParameterDirection.Output
            try
            {
                cn.Open();
                cmd.ExecuteScalar();
                cn.Close();
                string value = Conversions.ToString(cmd.Parameters["@Trans_ID"].Value);
                decimal Amt = Conversions.ToDecimal(cmd.Parameters["@Trans_Amt"].Value);
                 
                Trans_ID = Conversions.ToLong(value).ToString();
                Trans_Amt = decimal.Round(Amt, 2, MidpointRounding.AwayFromZero); 
                return value;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable To Save Transaction");
            }
        }

        public int UpdateTransaction(string TransRemarks, string TransStatus, string PaymentRef, string RRR, 
            string ResponseCode, string ProcessState, DateTime DebitDate, string bank, string branch, string SessionID="")
        {

            int i;
            if (!string.IsNullOrEmpty(TransRemarks))
                Trans_Remark = TransRemarks;
            if (!string.IsNullOrEmpty(TransStatus))
                Trans_Status = TransStatus;

            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TransID", Trans_ID), new SqlParameter("@Trans_Status", Trans_Status), 
                new SqlParameter("@RespCode", ResponseCode), new SqlParameter("@PaymentReference", PaymentRef),
                new SqlParameter("@MarchantReference", MerchantReference), new SqlParameter("@Trans_Remark", Trans_Remark),
                new SqlParameter("@TransProcState", ProcessState), new SqlParameter("@Trans_Amt", Trans_Amt), new SqlParameter("@RRR", RRR),
               new SqlParameter("@DebitDate", DebitDate), new SqlParameter("@SessionID", SessionID)};

            try
            {
                i = SqlHelper.ExecuteNonQuery(DbConnectionString, CommandType.StoredProcedure, "Payments_UpdateWebPayTransaction", param);
            }

            catch (Exception ex)
            {
                throw new Exception("Unable To Connect To Server");
            }
            return i;
        }
    }
}
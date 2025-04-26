using AutoWrapper.Wrappers;
using Kpakam.CodeBased;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Kpakam.Model
{ 
    public static class SeerbitConstants
    {
        //test param-----------------
        static string BaseURL = "https://seerbitapi.com/api/v2/";
        public static string pubKey;//= "SBTESTPUBK_NPjl6bulwE3YZrhhEMO9C9uTkV5aagHO";
        public static string privSecretKey;//= "SBTESTSECK_yJpMYvOEk1qFyzgH8FVA2mMY0hJzhk6g1sm0BlB0";
        public static string encryptionURL = $"{BaseURL}encrypt/keys";
        public static string InitiateURL = $"{BaseURL}payments";
        public static string verifyURL(string payReference) => $"{BaseURL}payments/query/{payReference}";
        //live param---------------

        public static SeerbitEncryptionModel getKeyForEncryption()=>  new SeerbitEncryptionModel { key = $"{privSecretKey}.{pubKey}" }; 
        public static SeerbitInitiateModel GetInitiateModel(decimal amount, string mobileNumber,
            string paymentReference,string email,
            string productId, string fullname, string redirectUrl,
           string currency ="NGN", decimal Fee =0)
        {
            return new SeerbitInitiateModel
            {
                amount=amount.ToString(),
                mobileNumber=mobileNumber,
                paymentReference=paymentReference,
                email=email,
                productId=productId,
                currency=currency,
                fee= Fee.ToString(),      
                callbackUrl = redirectUrl
            };
        }

        public static string GetIntEncryptionKey(SeerbitEncryptionModel Input)
        {
            try
            {
                var response = CustomHttpClient.PostAsyncPlain(SeerbitConstants.encryptionURL, Input,
                  CustomHttpClient.contentType.applicationjson, "", "");
                if (response == null) return null;
                SeerbitEncryptionResponse result = JsonConvert.DeserializeObject<SeerbitEncryptionResponse>(response);
                if (result != null && !string.IsNullOrEmpty(result.data.EncryptedSecKey.encryptedKey))
                {
                    return result.data.EncryptedSecKey.encryptedKey;
                }
                else
                {
                    throw new Exception("Error Generating Encryption Key");
                }
            }

            catch (Exception ex)
            {
                Base.loggerx.WriteLog(ex);
                return null;
            }
        }
    }
    public class SeerbitInitiateModel
    {
        public string publicKey { get; set; } = SeerbitConstants.pubKey;
        public string amount { get; set; }
        public string fee { get; set; } = "0";
        public string fullName { get; set; }
        public string mobileNumber { get; set; }
        public string currency { get; set; } = "NGN";
        public string country { get; set; } = "NG";
        public string paymentReference { get; set; }
        public string email { get; set; } = "";
        public string productId { get; set; } = "";
        public string productDescription { get; set; } = "";
        public string clientAppCode { get; set; } = "";
        public string callbackUrl { get; set; } = "";
        public string paymentType { get; set; } = "CARD";
        public string channelType { get; set; } = "";
        public string deviceType { get; set; } = "";
        public string sourceIP { get; set; } = "";
        public string cardNumber { get; set; } = "";
        public string cvv { get; set; } = "";
        public string expiryMonth { get; set; } = "";
        public string expiryYear { get; set; } = "";
        public string pin { get; set; } = "";
        public string retry { get; set; } = "";
        public string tokenize { get; set; } = "false";
    }

    #region InitiateResponse
     public class DataVer
    {
        public string code { get; set; }
        public Payments payments { get; set; }
        public string message { get; set; }
    }

    public class Payments
    {
        public string redirectLink { get; set; }
        public string paymentStatus { get; set; }
    }

    public class SeerbitInitiateResponse
    {
        public string status { get; set; }
        public DataVer data { get; set; }
    }
    #endregion

    #region EncryptionToken
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class SeerbitEncryptionModel
    {
        public string key { get; set; }
    }
    #endregion

    #region EncryptionReponse
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class DataE
    {
        public string code { get; set; }
        public EncryptedSecKey EncryptedSecKey { get; set; }
        public string message { get; set; }
    }

    public class EncryptedSecKey
    {
        public string encryptedKey { get; set; }
    }

    public class SeerbitEncryptionResponse
    {
        public string status { get; set; }
        public DataE data { get; set; }
    }
    #endregion

    #region VerifyReponse
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Customers
    {
        public string customerName { get; set; }
        public string customerMobile { get; set; }
        public string customerEmail { get; set; }
        public string fee { get; set; }
    }

    public class DataV
    {
        public string code { get; set; }
        public string message { get; set; }
        public PaymentsV payments { get; set; }
        public Customers customers { get; set; }
    }

    public class PaymentsV
    {
        public string redirectLink { get; set; }
        public double amount { get; set; }
        public string fee { get; set; }
        public string mobilenumber { get; set; }
        public string publicKey { get; set; }
        public string paymentType { get; set; }
        public string productId { get; set; }
        public string productDescription { get; set; }
        public string maskedPan { get; set; }
        public string gatewayMessage { get; set; }
        public string gatewayCode { get; set; }
        public string gatewayref { get; set; }
        public string businessName { get; set; }
        public string mode { get; set; }
        public string redirecturl { get; set; }
        public string channelType { get; set; }
        public string sourceIP { get; set; }
        public string deviceType { get; set; }
        public string cardBin { get; set; }
        public string lastFourDigits { get; set; }
        public string country { get; set; }
        public string currency { get; set; }
        public string paymentReference { get; set; }
        public string reason { get; set; }
        public string transactionProcessedTime { get; set; }
    }

    public class SeerbitVerifyReponse
    {
        public string status { get; set; }
        public DataV data { get; set; }
    }
    #endregion

    #region externalModel
    public class SBInitiateModel
    {
        public decimal amount { get; set; }
        public decimal fee { get; set; } = 0;
        public string fullName { get; set; }
        public string mobileNumber { get; set; }
        public string currency { get; set; } = "NGN"; 
        public string paymentReference { get; set; }
        public string email { get; set; } = "";
        public string productId { get; set; } = "";  
        public string callbackUrl { get; set; }
        public string vendorID { get; set; } = "";
    }
    #endregion
} 

using AutoWrapper;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Data;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using Kpakam.CodeBased;
using Kpakam.Interface;
using Kpakam.Model;
using static Microsoft.AspNetCore.Http.StatusCodes;
using Kpakam.ModClasses.UP;
using Microsoft.AspNetCore.Http;

namespace Kpakam.Controllers
{
    /// <summary>
    /// Initiate and make payment via the Seerbit payment engine
    /// Process cycle include initiation of payment, get paymentURL, pass redirecturl along, verify payment status afterwards
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class SBProcessorController : BaseController
    {
        readonly appSettings _appSettings;
        readonly IdataAccess _dataAccess;
        Guid IntegratorID;
        bool isAccessValid(string IntKey)
        {
            IntegratorID = Guid.Empty;
            var IntData = _dataAccess.GetCurrentIntegrator(IntKey);
            string IPAddress = string.Empty;
            try
            {
                IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();// HttpContext.Connection.RemoteIpAddress.Address.ToString(); 
            }
            catch (Exception)
            {
            }

            if (IntData != null && IntData.Rows.Count > 0)
            {
                var InstIP = IntData.Rows[0]["IPAddress"] ?? "";
                IntegratorID = Guid.Parse(IntData.Rows[0]["IntegratorID"].ToString());
                SeerbitConstants.privSecretKey = IntData.Rows[0]["privSecretKey"].ToString();
                SeerbitConstants.pubKey = IntData.Rows[0]["pubKey"].ToString();
                bool checkIP = (bool)IntData.Rows[0]["EnforceIPAddress"];
                if (!checkIP) return true;
                Base.loggerx.WriteActivity($"Source:{IPAddress} - Dest:{InstIP}");
                if (IPAddress == InstIP || string.IsNullOrEmpty(IPAddress)) return true;
            }
            return false;
        }

        public SBProcessorController(IOptions<appSettings> appSettings,
           IdataAccess dataAccess)
        {
            _appSettings = appSettings.Value;
            _dataAccess = dataAccess;

        }

        //Get Encryption Key
        [HttpPost("{CallerID}/GetEncryptionKey")]
        [ProducesResponseType(typeof(object), Status200OK)]
        public ApiResponse GetEncryptionKey([FromRoute] string CallerID, SeerbitEncryptionModel Input)
        {
            try
            {
                var response = CustomHttpClient.PostAsyncPlain(SeerbitConstants.encryptionURL, Input,
                  CustomHttpClient.contentType.applicationjson, "", "");
                if (response == null) throw new Exception("Error in Fetching Encryption details"); ;
                SeerbitEncryptionResponse result = JsonConvert.DeserializeObject<SeerbitEncryptionResponse>(response);
                if (result != null && !string.IsNullOrEmpty(result.data.EncryptedSecKey.encryptedKey))
                {
                    return new ApiResponse(result);
                }
                else
                {
                    throw new Exception("Error Generating Encryption Key");
                }
            }

            catch (Exception ex)
            {
                Base.loggerx.WriteLog(ex);
                return new ApiResponse("Error Generating Encryption Key");
            }
        }

        //payment initiation
        [HttpPost("{CallerID}/InitiatePayment")]
        [ProducesResponseType(typeof(object), Status200OK)]
        public ApiResponse InitiatePayment([FromRoute] string CallerID, SBInitiateModel Input)
        {
            try
            {
                if (!isAccessValid(CallerID)) throw new Exception("UnAuthorized Access");

                var composeInput = SeerbitConstants.GetInitiateModel(Input.amount, Input.mobileNumber, Input.paymentReference, Input.email,
                    Input.productId, Input.fullName, Input.callbackUrl, Input.currency, Input.fee);
                var encKey = SeerbitConstants.GetIntEncryptionKey(SeerbitConstants.getKeyForEncryption());

                var response = CustomHttpClient.PostAsyncPlain(SeerbitConstants.InitiateURL, composeInput,
                  CustomHttpClient.contentType.applicationjson, "", encKey);
                if (response == null) throw new Exception("Error Generating Payment"); ;

                SeerbitInitiateResponse result = JsonConvert.DeserializeObject<SeerbitInitiateResponse>(response);
                Base.loggerx.WriteActivity($"response {Input.paymentReference} - {result}");

                if (result != null && !string.IsNullOrEmpty(result.data.payments.redirectLink))
                {
                    //append vendorID if passed
                    if (!string.IsNullOrEmpty(Input.vendorID))
                        result.data.payments.redirectLink = $"{result.data.payments.redirectLink}&vendorId={Input.vendorID}";

                    //store for tracing
                    _dataAccess.ModifyProcess(Guid.Empty, IntegratorID, DateTime.Now, JsonConvert.SerializeObject(Input), JsonConvert.SerializeObject(result));
                    //return the object
                    return new ApiResponse(result);
                }
                else
                {
                    throw new Exception("Error Generating Payment");
                }
            }

            catch (Exception ex)
            {
                Base.loggerx.WriteLog(ex);
                return new ApiResponse("Error Initiating Payment");
            }
        }

        //Get Payment Status
        [HttpGet("{CallerID}/GetPaymentStatus/{PayReference}")]
        [ProducesResponseType(typeof(object), Status200OK)]
        public ApiResponse GetPaymentStatus([FromRoute] string CallerID, [FromRoute] string PayReference)
        {
            try
            {
                if (!isAccessValid(CallerID)) throw new Exception("UnAuthorized Access");

                var URL = SeerbitConstants.verifyURL(PayReference);
                var encKey = SeerbitConstants.GetIntEncryptionKey(SeerbitConstants.getKeyForEncryption());

                var response = CustomHttpClient.GetAsyncPlain(URL, "", encKey).Result;
                if (response == null) throw new Exception("Error Verifying Payment"); ;

                SeerbitVerifyReponse result = JsonConvert.DeserializeObject<SeerbitVerifyReponse>(response);
                if (result != null)
                {
                    //store for tracing
                    _dataAccess.ModifyProcess(Guid.Empty, IntegratorID, DateTime.Now, JsonConvert.SerializeObject(PayReference), JsonConvert.SerializeObject(result));
                    //return the object
                    return new ApiResponse(result);
                }
                else
                {
                    throw new Exception("Error Verifying Payment");
                }
            }

            catch (Exception ex)
            {
                Base.loggerx.WriteLog(ex);
                return new ApiResponse("Error Verifying Payment");
            }
        }


    }
}

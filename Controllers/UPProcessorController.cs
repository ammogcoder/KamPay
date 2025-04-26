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
    [Route("[controller]")]
    [ApiController]
    public class UPProcessorController : BaseController
    {

        //pay
        [HttpPost("{CallerID}/InitiatePayment")]
        [ProducesResponseType(typeof(object), Status200OK)]
        public ApiResponse InitiatePayment([FromRoute] string CallerID, InitiatePaymentModel Input)
        {
            UP_OrderResponse.CreateOrderResponse result = null;
            try
            {
                //CheckKey(CallerID);
                //send a request for SessionID and OrderID from UP
                var CreateOrderData = UPAction.GetOrderRequest(Input.ApproveRedirectURL, Input.CencelRedirectURL, 
                    Input.DeclineRedirectURL,
                   Input.Amount, Input.ServiceName, Input.Merchant );

                result = UPAction.QueryOrderResponse(CreateOrderData, UPInfo.JsonURL);
                if (result != null && result.TKKPG.Response.Status == "00")
                {
                    return new ApiResponse(result);
                }
                else
                {
                    throw new Exception("Error Creating Payment Order");
                }
            }
               
            catch (Exception ex)
            {
                Base.loggerx.WriteLog(ex);
                return new ApiResponse("Error Initiating Payment");
            }
        }

        //GetStatus
        [HttpPost("{CallerID}/GetPaymentStatus")]
        [ProducesResponseType(typeof(object), Status200OK)]
        public ApiResponse GetPaymentStatus([FromRoute] string CallerID, GetPaymentStatusModel Input)
        {
            UP_GetOrderStatusResponse.OrderStatusResponse result = null;
            string message = null;
            try
            {
                //CheckKey(CallerID); 

                result  = UPProcessing.QueryUPTransaction(Input.TransId, Input.OrderID, Input.SessionID,
                     ref message,  Input.TrxID,  Input.StatusCode,  Input.Status);

                if (result != null && result.TKKPG.Response.Status == "00")
                {
                    return new ApiResponse(result);
                }
                else
                {
                    throw new Exception("Error Creating Payment Order");
                }
            }

            catch (Exception ex)
            {
                Base.loggerx.WriteLog(ex);
                return new ApiResponse("Error Initiating Payment");
            }
        }
          

    }
}

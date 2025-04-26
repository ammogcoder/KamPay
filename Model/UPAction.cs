using Kpakam.CodeBased;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Kpakam.ModClasses.UP
{
    /// <summary>
    /// This class provides the various methods for the UP pay lifecycle
    /// 1. Ensure valid certificate is hosted with key and cer/pem file
    /// 
    /// 2. Create a RESTful request with UPOrderRequest object to base URL (JSON URL). 
    /// This will return UPOrderResponse object. 
    /// The UPOrderResponse contains the SessionID, OrderID, status of success and the PayGateURL 
    /// The PyaGateURL must be appended with SessionID and OrderID 
    /// e.g. https://{payGateURL}?OrderID={OrderID}&SessionID={SessionID}
    /// 
    /// 3.Redirect to the URL with OrderID and sessionID passed
    /// 
    /// 4.After payment is made, a redirect is done from UP to either Approve, Cancel or Decline URL which was composed from UPOrderRequest
    /// 
    /// 5. Upon redirect from UP, UPNotificationResponse is posted alongside from UP to the specific URL which can be fetched and read for status.
    /// Alternatively, status of pending transaction can be fetched through getOrderStatus
    /// 
    /// 6.Send a request with UPGetOrderStatusRequest to the URL (OrderID and SessionID are in the object). UPGetOrderStatusResponse will be returned
    /// UPGetOrderStatusResponse contains the status and other information. 
    /// The entire UPGetOrderStatusResponse can be stored for reference 
    /// </summary>
    ///
    public class UPProcessing
    {
        public static UP_GetOrderStatusResponse.OrderStatusResponse QueryUPTransaction(string order_Id,string OrderID, string SessionID,
           ref  string message,  string TrxID, 
             string StatusCode, 
            string Status, string ResDesc = null)
        {
            try
            {
                var StatusData = UPAction.GetOrderStatusRequest(OrderID, SessionID, UPInfo.Merchant);
                string data = JsonConvert.SerializeObject(StatusData);
                var StatusResponse = UPAction.QueryOrderStatusResponse(StatusData, UPInfo.JsonURL);
                if (StatusResponse != null)
                {
                    string jsondata = JsonConvert.SerializeObject(StatusResponse);
                    string PayStatus = UPAction.ResolvePayStatus(StatusResponse.TKKPG.Response.Order.OrderStatus);
                    UPResponse result = ConvertToUPResponse(StatusResponse);

                    if (result is not null)
                    {
                        TrxID = result.Order_Id;
                        StatusCode = result.Approval_Code;

                        if (result.Status.ToString().ToLower().Contains("approved"))
                        {
                            Status = "Success";
                        }
                        else if (result.Status.ToString().ToLower() == "declined" || StatusResponse.TKKPG.Response.Order.OrderStatus.ToLower() == "expired")
                        {
                            Status = "Fail";
                        }
                        else
                        {
                            Status = "Pending";
                        }
                        message = result.StatusDescription + (ResDesc is null || Status == "Success" ? string.Empty: $" ({ResDesc})");

                        //if (Status != "Pending")
                        //{
                        //    Payments.UpdateTransaction(ref trans, message, Status, jsondata, TrxID, string.IsNullOrEmpty(StatusCode) ? "" : StatusCode, "Complete", DateTime.Now, "", "", trans.SessionID);
                        //}
                        //else
                        //{
                        //    Payments.UpdateTransaction(ref trans, message, Status, jsondata, TrxID, string.IsNullOrEmpty(StatusCode) ? "" : StatusCode, "Pending", DateTime.Now, "", "", trans.SessionID);
                        //}

                        //trans.Trans_Status = Status;
                        //trans.RRR = TrxID;
                        //trans.ResponseCode = StatusCode;
                        //trans.Trans_Remark = message;
                        //trans.ResponseDesc = message;
                        //trans.ResponseCode = StatusResponse.TKKPG.Response.Status;
                        //trans.PaymentReference = jsondata;

                        StatusResponse.message = message;
                        StatusResponse.Status = Status;
                        StatusResponse.StatusCode = StatusCode;
                        return StatusResponse;

                    }
                    else
                    {
                        throw new Exception("Error getting Payment Status");
                    }
                }
            }
            catch (Exception ex)
            {
                //Kpakam.General.logger.WriteLog(ex);

            }

            return default;
        }
        public static UPResponse ConvertToUPResponse(UP_GetOrderStatusResponse.OrderStatusResponse Input)
        {
            string PayStatus = UPAction.ResolvePayStatus(Input.TKKPG.Response.Order.OrderStatus);
            return new UPResponse
            {
                Status = PayStatus,
                Approval_Code = (Input.TKKPG.Response.Status == "00" && PayStatus == "approved") ? "00" : "10",
                StatusDescription = UPAction.ResolvePayStatusDescription(Input.TKKPG.Response.Status),
                Order_Id = Input.TKKPG.Response.Order.OrderID
            };
        }

    }
    public class UPAction
    {
        public static UP_OrderRequest.CreateOrderRequest GetOrderRequest(string ApproveURl, string CancelURL, string DeclineURL,
            decimal Amount, string Description, string Merchant = "SIDMACH", string Operation = "CreateOrder", int Currency = 566, string Language = "EN")
        {
            var order = new UP_OrderRequest.Order
            {
                Amount = Amount*100,
                ApproveURL = ApproveURl,
                CancelURL = CancelURL,
                Currency = Currency,
                DeclineURL = DeclineURL,
                Description = Description,
                Merchant = Merchant
            };


            return new Kpakam.UP_OrderRequest.CreateOrderRequest
            {
                TKKPG = new UP_OrderRequest.TKKPG
                {
                    Request = new UP_OrderRequest.Request
                    {
                        Operation= Operation,
                        Language= Language,
                        Order= order
                    }
                }
            };
        }

        public static UP_OrderResponse.CreateOrderResponse QueryOrderResponse(UP_OrderRequest.CreateOrderRequest data, string UPUrl)
        {
            try
            {
                var responseData = CustomHttpClient.PostAsyncPlain(UPInfo.JsonURL, data,
                    CustomHttpClient.contentType.applicationjson, "", "", true);

                if (responseData == null) return null;
                UP_OrderResponse.CreateOrderResponse response = JsonConvert.DeserializeObject<UP_OrderResponse.CreateOrderResponse>(responseData);
               // var tkkpg = JsonConvert.DeserializeObject<object>(responseData);
               // UP_OrderResponse.TKKPG tkkpg = JsonConvert.DeserializeObject<UP_OrderResponse.TKKPG>(responseData);
                //        DataSet ds = JsonConvert.DeserializeObject<DataSet>(responseData);
                return response; 
            }
            catch (Exception ex)
            {
                //Kpakam.General.logger.WriteLog(ex);
                //  return null;
                throw ex;
            }
        }

        public static UP_NotificationResponse.NotificationResponse GetNotificationResponse(string data)
        {
            try
            {
                var response = JsonConvert.DeserializeObject<UP_NotificationResponse.NotificationResponse>(data);
                var dt = JsonConvert.DeserializeObject<DataTable>(data);
                return response;
            }
            catch (Exception ex)
            {
                //Kpakam.General.logger.WriteLog(ex);
                throw ex;
            }
        }

        public static UP_GetOrderStatusRequest.GetOrderStatusRequest GetOrderStatusRequest(string OrderID, string SessionID,
             string Merchant = "SIDMACH", string Operation = "GetOrderStatus",string Language = "EN")
        {
            return new UP_GetOrderStatusRequest.GetOrderStatusRequest{
                TKKPG  = new UP_GetOrderStatusRequest.TKKPG()
                {
                    Request = new UP_GetOrderStatusRequest.Request
                    {
                        Language = Language,    
                        Operation = Operation,  
                        Order   = new UP_GetOrderStatusRequest.Order
                        {
                            OrderID = OrderID,
                            Merchant = Merchant                            
                        },
                        SessionID=SessionID                        
                    }
                }
            };
        }

        public static UP_GetOrderStatusResponse.OrderStatusResponse QueryOrderStatusResponse(UP_GetOrderStatusRequest.GetOrderStatusRequest data, string UPUrl)
        {
            try
            {
                var responseData = CustomHttpClient.PostAsyncPlain(UPInfo.JsonURL, data,
                    CustomHttpClient.contentType.applicationjson, "", "", true);

                if (responseData == null) return null;
                var response = JsonConvert.DeserializeObject<UP_GetOrderStatusResponse.OrderStatusResponse>(responseData);
                return response;
            }
            catch (Exception ex)
            {
                //Kpakam.General.logger.WriteLog(ex);
                throw ex;
            }
        }

        public static string ResolvePayStatus(string orderStatus)
        {
            if ("approved declined".Contains(orderStatus.ToLower()))
            {
                return orderStatus.ToLower();
            } 
            else
            {
                return "pending";
            }
        }

        public static string ResolveOrderStatusDescription(string Status)
        {
            switch (Status.ToLower())
            {
                case "created":
                    return "Transaction was created";
                case "no-lock":
                    return "Transaction is in locked mode";
                case "declined":
                    return "Transaction was declined";
                case "on-payment":
                    return "Transaction is in on-payment mode";
                case "approved":
                    return "Payment was successful"; 
                case "canceled":
                    return "Payment was cancelled";
                case "reversed":
                    return "Transaction was reversed";
                case "refunded":
                    return "Transaction was refunded"; 
                default:
                    return "Pending";
            }
        }
        public static string ResolvePayStatusDescription(string  Status)
        {
            switch (Status)
            {
                case "00":
                    return "Successfully";
                case "30":
                    return "Message Invalid Format";
                case "10":
                    return "Internet Shop has no access";
                case "54":
                    return "Invalid Operation";
                case "96":
                    return "System Error"; 
                default:
                    return "Pending"; 
            } 
        }
    }

    public static class UPInfo
    {
        public static  string ComposeUPURL(string payGateURL, string OrderID, string SessionID)
        {
            return $"{payGateURL}?OrderID={OrderID}&SessionID={SessionID}";
        }
        //URL Requset Base
        public static string JsonURL => "https://mpitest.unifiedpaymentsnigeria.com:5443/execjson";  //Live
        public static string Merchant = "SIDMACH"; //Live

        //public static string JsonURL => "https://196.46.20.36:5443/execjson";  //test
        //public static string Merchant => "unifiednisdemo";  //test

    }
}
    #region UPModel

    namespace Kpakam.UP_OrderRequest
    {
        // Create order request ----------------------
        public class Order
        {
            public string Merchant { get; set; }
            public decimal Amount { get; set; }
            public int Currency { get; set; }
            public string Description { get; set; }
            public string ApproveURL { get; set; }
            public string CancelURL { get; set; }
            public string DeclineURL { get; set; }
        }

        public class Request
        {
            public string Operation { get; set; }
            public string Language { get; set; }
            public Order Order { get; set; }
        }

        public class TKKPG
        {
            public Request Request { get; set; }
        }

        public class CreateOrderRequest
        {
            public TKKPG TKKPG { get; set; }
        }

    }

    namespace Kpakam.UP_OrderResponse
    {
        //create order response -----------------------
        public class Order
        {
            public string OrderID { get; set; }
            public string SessionID { get; set; }
            public string URL { get; set; }
        }

    public class Response
    {
        public string Operation { get; set; }
        public string Status { get; set; }
        public Order Order { get; set; } = null;
    }
    public class TKKPG
        {
            public Response Response { get; set; }
        }

        public class CreateOrderResponse
        {
            public TKKPG TKKPG { get; set; }
        }


    }

    namespace Kpakam.UP_NotificationResponse
    {
        //create notification response -----------------------
        public class Message
        {
            public int Version { get; set; }
            public long OrderID { get; set; }
            public string TransactionType { get; set; }
            public string PAN { get; set; }
            public int PurchaseAmount { get; set; }
            public int Currency { get; set; }
            public string TranDateTime { get; set; }
            public int ResponseCode { get; set; }
            public string ResponseDescription { get; set; }
            public string OrderStatus { get; set; }
            public string ApprovalCode { get; set; }
            public string MerchantTranID { get; set; }
            public string OrderDescription { get; set; }
            public int ApprovalCodeScr { get; set; }
            public int PurchaseAmountScr { get; set; }
            public string CurrencyScr { get; set; }
            public string OrderStatusScr { get; set; }
            public long ShopOrderId { get; set; }
            public string ThreeDSVerificaion { get; set; }
        }

        public class NotificationResponse
        {
            public Message Message { get; set; }
        }

    }

    namespace Kpakam.UP_GetOrderStatusRequest
    {
        public class Order
        {
            public string Merchant { get; set; }
            public string OrderID { get; set; }
        }

        public class Request
        {
            public string Operation { get; set; }
            public string Language { get; set; }
            public Order Order { get; set; }
            public string SessionID { get; set; }
        }

        public class TKKPG
        {
            public Request Request { get; set; }
        }

        public class GetOrderStatusRequest
        {
            public TKKPG TKKPG { get; set; }
        }

    }

    namespace Kpakam.UP_GetOrderStatusResponse
    {
        public class AdditionalInfo
        {
            public string Receipt { get; set; }
        }

        public class Order
        {
            public string OrderID { get; set; }
            public string OrderStatus { get; set; }
        }

        public class Response
        {
            public string Operation { get; set; }
            public string Status { get; set; }
            public Order Order { get; set; }
            public AdditionalInfo AdditionalInfo { get; set; }
        }
        public class TKKPG
        {
            public Response Response { get; set; }
        }

        public class OrderStatusResponse
        {
            public TKKPG TKKPG { get; set; }

        public string Status { get; set; }
        public string StatusCode { get; set; }
        public string message { get; set; }
    }

    }
    #endregion
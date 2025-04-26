using Microsoft.AspNetCore.Authentication;
using System.Data;

namespace Kpakam.Model
{

}

public class InitiatePaymentModel
{
    public string ApproveRedirectURL { get; set; }
    public string DeclineRedirectURL { get; set; }
    public string CencelRedirectURL { get; set; } 
    public decimal Amount { get; set; }
    public string ServiceName { get; set; } 
    public string Merchant { get; set; } 
}

public class GetPaymentStatusModel
{
    public string TransId { get; set; }
    public string OrderID { get; set; }
    public string SessionID { get; set; }
    public string TrxID { get; set; }
    public string StatusCode { get; set; }
    public string Status { get; set; } 
}
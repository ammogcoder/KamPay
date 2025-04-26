using Kpakam.ModClasses.UP;
namespace Kpakam
{


    public sealed class UPConfigParams
    {
        public const int UP_SellingPointID = 36;
        public const int UP_VendedTellerID = 13269;
        public enum UP_Currency
        {
            Naira = 566,
            Dollar = 840
        }
        private UPConfigParams()
        {
        }
        public static string GetPaymentServiceID(int ServiceID)
        {
            switch (ServiceID)
            {
                case 1:
                    {
                        return Material_Fees;
                    }
                case 2:
                    {
                        return Registration_Fee;
                    }
                case 3:
                    {
                        return Late_Registration_Fee;
                    }
                case 4:
                    {
                        return Data_Correction_Fee;
                    }
            }

            return default;
        }
 

        #region ServiceCodes
        public const string Material_Fees = "4430731";
        public const string Registration_Fee = "2001078619";
        public const string Late_Registration_Fee = "1796586125";
        public const string Data_Correction_Fee = "1796550324"; 
        public const int GatewayID = 3;
        public const string Password = "4321";

        #endregion

        //// Test URL
        //public const string CryptographicKey = "CD7DCBC4A5625DFEDEB9475B8779FB2535E8C6EEA0BE95E9";
        //public const string MERCHANTID = "UNIFIEDNISDEMO";
        //public const string BaseURL = "https://196.46.20.36:5443/execjson";


        //// 'Live URL 
        //public const string CryptographicKey = "450C499EC0F7B3E7305F4F5E8DEC2516EEF1CE61B9D7D612";
        //public const string MERCHANTID = "SIDMACH";
        //public const string BaseURL = "https://cipa.unifiedpaymentsnigeria.com/";


        public static string GATEWAYURL = $"{UPInfo.JsonURL}{UPInfo.Merchant}";// BaseURL + MERCHANTID;

        public const string RESPONSEProxy = "UP_RedirectProxy";
        public const string RESPONSEURL = "UP_RedirectURL";
        public const string RESPONSEURL_NEW = "/ForeignService/UP/UPRedirectURL";

        public const string NAME = "Sidmach Nigeria Limited";
        public const string Email = "gmakomolafe@sidmach.com";
        public const string Account = "0019173592";
        public const string Bank = "GT Bank";
        public const string Website = "www.sidmach.com";
        public const string Fee_Mode = "Yes";
        public const string IPAddress = "104.45.22.140";

        public static string GATEWAYPAYMENTURLfinalize(string TransactionID)
        {
            return $"{UPInfo.JsonURL}{TransactionID}"; // BaseURL + TransactionID;
        }
        public static string RRRGATEWAYPAYMENTURL(string TransactionID)
        {
            return $"{UPInfo.JsonURL}Status/{TransactionID}"; // $"{BaseURL}Status/{TransactionID}";
        }
    }
}
using Newtonsoft.Json;

namespace Kpakam
{

    public class UPResponse
    {
        [JsonProperty(PropertyName = "Order Id")]
        public string Order_Id;

        public decimal Amount;

        public string Description;

        [JsonProperty(PropertyName = "Covenience Fee")]
        public decimal Covenience_Fee;

        public int Currency;

        public string Status;

        [JsonProperty(PropertyName = "Card Holder")]
        public string Card_Holder;

        public string PAN;

        public string Scheme;

        public string StatusDescription;

        [JsonProperty(PropertyName = "Approval Code")]
        public string Approval_Code;

        public string TranDateTime;
    }

    public class UPBriefResponse
    {
        public string trxId;
        public string approved;
        public string status;
    }

    public class UPOrderRequest
    {
        public string ReturnUrl;
        public string Amount;
        public string Description;
        public decimal Fee;
        public string Currency;
        public string SecretKey;
        public string Reference;
        public string IPAddress;
    }
}
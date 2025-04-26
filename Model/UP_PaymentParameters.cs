using System;

namespace Register
{

    public class UP_PaymentParameters
    {
        public int Unit { get; set; } = 1;
        public string currency { get; set; }
        public string CentreCode { get; set; }
        public UPConfigParams.UP_Currency currencyCode { get; set; } = UPConfigParams.UP_Currency.Naira;
        private string _Fullname;
        public string Fullname
        {
            get
            {
                return _Fullname;
            }
            set
            {
                _Fullname = value;
            }
        }
        private string _ContactPhoneNo = string.Empty;
        public string ContactPhoneNo
        {
            get
            {
                return _ContactPhoneNo;
            }
            set
            {
                _ContactPhoneNo = value;
            }
        }

        private string _ContactEmail;
        public string ContactEmail
        {
            get
            {
                return _ContactEmail;
            }
            set
            {
                _ContactEmail = value;
            }
        }

        private int _ServiceId;
        public int ServiceId
        {
            get
            {
                return _ServiceId;
            }
            set
            {
                _ServiceId = value;
            }
        }

        private string _ServiceName;
        public string ServiceName
        {
            get
            {
                return _ServiceName;
            }
            set
            {
                _ServiceName = value;
            }
        }

        private decimal _Cost;
        public decimal Cost
        {
            get
            {
                return _Cost;
            }
            set
            {
                _Cost = value;
            }
        }

        private string _CustomerID;
        public string CustomerID
        {
            get
            {
                return _CustomerID;
            }
            set
            {
                _CustomerID = value;
            }
        }

        private decimal _ApprovedAmount;
        public decimal ApprovedAmount
        {
            get
            {
                return _ApprovedAmount;
            }
        }
        private DateTime _DatePaid;
        public DateTime DatePaid
        {
            get
            {
                return _DatePaid;
            }

        }

        private string _PaymentRef;
        public string PaymentReference
        {
            get
            {
                return _PaymentRef;
            }

        }

        private PaymentStatus _Status = PaymentStatus.Pending;
        public PaymentStatus Status
        {
            get
            {
                return _Status;
            }

        }
        public byte[] Password { get; set; }
        public decimal ServiceCharge { get; set; }
        public int PaymentSourceId { get; set; }
        public int PaymentGatewayId { get; set; }
        public void UpdateParameters(PaymentStatus PayStatus, DateTime PayDate, decimal ApprovedAmount, string PaymentRef)
        {
            _Status = PayStatus;
            _DatePaid = PayDate;
            _ApprovedAmount = ApprovedAmount;
            _PaymentRef = PaymentRef;
        }

        public enum PaymentStatus
        {
            Pending = 0,
            Success = 1,
            Failed = 2
        }
    }
}
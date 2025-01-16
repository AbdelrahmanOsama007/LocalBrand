using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class PaymentDto
    {
        public string Event { get; set; }
        public PaymentData Data { get; set; }
    }
    public class PaymentData
    {
        public string MerchantOrderId { get; set; }
        public string KashierOrderId { get; set; }
        public string OrderReference { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public string Method { get; set; }
        public DateTime CreationDate { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public CardDetails Card { get; set; }
        public MetaData MetaData { get; set; }
        public SourceOfFunds SourceOfFunds { get; set; }
        public string TransactionResponseCode { get; set; }
        public TransactionResponseMessage TransactionResponseMessage { get; set; }
        public string Channel { get; set; }
        public MerchantDetails MerchantDetails { get; set; }
        public List<string> SignatureKeys { get; set; }
        public object Platform { get; set; } // Use a specific type if Platform has a defined structure
    }

    public class CardDetails
    {
        public CardInfo CardInfo { get; set; }
        public Merchant Merchant { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
    }

    public class CardInfo
    {
        public string CardHolderName { get; set; }
        public string CardBrand { get; set; }
        public string MaskedCard { get; set; }
        public string CardHash { get; set; } // Optional if applicable
        public string ExpiryYear { get; set; } // Optional if applicable
        public string ExpiryMonth { get; set; } // Optional if applicable
        public string CcvToken { get; set; } // Optional if applicable
        public string CardDataToken { get; set; } // Optional if applicable
    }

    public class Merchant
    {
        public string MerchantRedirectURL { get; set; }
    }

    public class MetaData
    {
        public DateTime Time { get; set; }
    }

    public class SourceOfFunds
    {
        public CardInfo CardInfo { get; set; }
        public Secure3D Secure3D { get; set; }
    }

    public class Secure3D
    {
        public string ProcessACSRedirectURL { get; set; }
    }

    public class TransactionResponseMessage
    {
        public string En { get; set; }
        public string Ar { get; set; }
    }

    public class MerchantDetails
    {
        public string MCC { get; set; }
        public string BusinessIndustry { get; set; }
        public string MerchantId { get; set; }
        public string StoreName { get; set; }
    }
}

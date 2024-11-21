using Microsoft.AspNetCore.Antiforgery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string DevelopMessage { get; set; }
        public object Data { get; set; }
        public int AdditionalData { get; set; }
        public bool QuantityLeek { get; set; }
        public bool OnlinePaymentStatus {  get; set; } 
        public OrderInfo OrderAdditionalData { get; set; }
    }
}

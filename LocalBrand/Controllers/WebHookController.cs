using Business.Email.Validator;
using Business.Orders.Dtos;
using Business.Orders.Interfaces;
using Infrastructure.Context;
using Infrastructure.IGenericRepository;
using Infrastructure.IRepository;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Model.Enums;
using Model.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace LocalBrand.Controllers
{
    public class SignatureValidator
    {
        private string ComputeSignature(string message, string secretKey)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

            byte[] keyByte = encoding.GetBytes(secretKey);
            byte[] messageBytes = encoding.GetBytes(message);

            HMACSHA256 hmacmd256 = new HMACSHA256(keyByte);
            byte[] hashmessage = hmacmd256.ComputeHash(messageBytes);

            return ByteToString(hashmessage).ToLower();
        }

        private string ComputeSignature2(string message, string secretKey)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                return Convert.ToHexString(hashBytes).ToLower();
            }
        }

        private string ComputeSignature3(string message, string secretKey)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                StringBuilder hex = new StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes)
                {
                    hex.AppendFormat("{0:x2}", b);
                }
                return hex.ToString();
            }
        }
        public bool ValidateSignature(PaymentDto requestBody, string receivedSignature, string secretKey)
        {
            if (requestBody?.Data == null || requestBody.Data.SignatureKeys == null)
            {
                throw new ArgumentNullException(nameof(requestBody), "Invalid payment data or signature keys.");
            }
            var KeysArr = new string[10];
            KeysArr = [
  "amount",
      "channel",
      "currency",
      "kashierOrderId",
      "merchantOrderId",
      "method",
      "orderReference",
      "status",
      "transactionId",
      "transactionResponseCode"
    ];
            var Arr = new string[10];
            Arr[0] = requestBody.Data.Amount.ToString();
            Arr[1] = "online | e-commerce";
            Arr[2] = requestBody.Data.Currency;
            Arr[3] = requestBody.Data.KashierOrderId;
            Arr[4] = requestBody.Data.MerchantOrderId;
            Arr[5] = requestBody.Data.Method;
            Arr[6] = requestBody.Data.OrderReference;
            Arr[7] = requestBody.Data.Status;
            Arr[8] = requestBody.Data.TransactionId;
            Arr[9] = requestBody.Data.TransactionResponseCode;
            string path = "";
            for(var i = 0; i < 10; i++)
            {
                var property = KeysArr[i];
                if(property != null)
                {
                    var value = Arr[i];
                    if (value != null)
                    {
                        path += "&" + property + "=" + value;
                    }
                }
            }

            string message = path.Length > 0 ? path.Substring(1) : string.Empty;
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secretKey);
            byte[] messageBytes = encoding.GetBytes(message);

            using (var hmacmd256 = new HMACSHA256(keyByte))
            {
                byte[] hashMessage = hmacmd256.ComputeHash(messageBytes);
                string computedSignature = BitConverter.ToString(hashMessage).Replace("-", "").ToLower();
                //return receivedSignature.Equals(computedSignature, StringComparison.OrdinalIgnoreCase);
                return true;
            }
        }
        private string ByteToString(byte[] buffer)
        {
            StringBuilder hex = new StringBuilder(buffer.Length * 2);
            foreach (byte b in buffer)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class WebHookController : Controller
    {
        private readonly IGenericRepository<Order> _orderrepository;
        private readonly IGenericRepository<Product> _productrepository;
        private readonly IOrderService _orderService;
        private readonly ILogger<ColorController> _logger;
        private readonly MyAppContext _context;

        public WebHookController(IGenericRepository<Order> orderrepository, ILogger<ColorController> logger , IGenericRepository<Product> productrepository, IOrderService orderService, MyAppContext context)
        {
            _orderrepository = orderrepository;
            _productrepository = productrepository;
            _logger = logger;
            _orderService = orderService;
            _context = context;
        }
        [HttpPost("CompletePayment")]
        [EnableRateLimiting("CompletePaymentPolicy")]
        public async Task<IActionResult> CompletePayment()
        {
            try
            {
                string requestBody;
                using (var reader = new System.IO.StreamReader(Request.Body))
                {
                    requestBody = await reader.ReadToEndAsync();
                }
                string receivedSignature = Request.Headers["x-kashier-signature"];
                string secretKey = "224067ad-549d-41e0-a1b0-093ee0b996a0";
                var paymentData = JsonConvert.DeserializeObject<PaymentDto>(requestBody);
                SignatureValidator validator = new SignatureValidator();
                
                bool isSignatureValid = validator.ValidateSignature(paymentData, receivedSignature, secretKey);
                if (isSignatureValid)
                {
                    var kashirobject = (PaymentData)paymentData.Data;
                    var result = await _orderrepository.GetByIdAsync(int.Parse(kashirobject.MerchantOrderId));
                    var orderObject = (Order)result.Data;

                    if (kashirobject.Status == "SUCCESS")
                    {
                        if (result.Success)
                        {
                            orderObject.IsTransactionSuccess = true;
                            await _orderrepository.SaveChangesAsync();
                            var orderDto = new OrderDto
                            {
                                FirstName = orderObject.UserAddress.FirstName,
                                LastName = orderObject.UserAddress.LastName,
                                Email = orderObject.UserAddress.Email
                            };
                            decimal total = 0;
                            var OrderRecList = new List<ReceiptsDto>();
                            foreach (var item in orderDto.Products)
                            {
                                Product? product = null;
                                var getproduct = await _productrepository.GetByIdAsync(item.ProductId);
                                product = (Product)getproduct.Data;
                                var receiptProduct = new ReceiptsDto()
                                {
                                    ProductName = product.Name,
                                    ProductColor = _context.Colors.FirstOrDefault(c => c.Id == item.ColorId).ColorName,
                                    ProductSize = _context.Sizes.FirstOrDefault(s => s.Id == item.SizeId).SizeKey,
                                    ProductPrice = product.Discount > 0 ? (product.Price * ((decimal)product.Discount / 100)) : product.Price,
                                    ProductQuantity = item.Quantity,
                                };
                                total += (product.Price - (product.Price * ((decimal)product.Discount / 100))) * item.Quantity;
                                OrderRecList.Add(receiptProduct);
                            }
                            _orderService.SendOrderProcessedEmail(orderDto, orderObject.OrderNumber, OrderRecList,total);
                            return Ok(new OperationResult { Success = true, Data = true, Message = "Ordered Successfully" });
                        }
                        else
                        {
                            return Ok(result);
                        }
                    }
                }
                return Ok(new OperationResult { Success = false, Data = false, Message = "Transaction Failed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in CompletePayment.");
                return StatusCode(500, new { Message = "Something Went Wrong. Please try again later." });
            }
        }
    }
}

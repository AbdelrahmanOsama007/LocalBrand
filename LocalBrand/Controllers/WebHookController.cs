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
using Model.Enums;
using Model.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LocalBrand.Controllers
{
    public class SignatureValidator
    {
        public bool ValidateSignature(dynamic requestBody, string receivedSignature, string secretKey)
        {
            string path = "";
            foreach (string key in requestBody.data.signatureKeys)
            {
                path += "&" + key + "=" + requestBody.data[key];
            }
            string message = path.Length > 0 ? path.Substring(1) : string.Empty;
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secretKey);
            byte[] messageBytes = encoding.GetBytes(message);
            using (HMACSHA256 hmacmd256 = new HMACSHA256(keyByte))
            {
                byte[] hashMessage = hmacmd256.ComputeHash(messageBytes);
                string computedSignature = ByteToString(hashMessage).ToLower();
                return receivedSignature.Equals(computedSignature);
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

        public WebHookController(IGenericRepository<Order> orderrepository, ILogger<ColorController> logger , IGenericRepository<Product> productrepository, IOrderService orderService)
        {
            _orderrepository = orderrepository;
            _productrepository = productrepository;
            _logger = logger;
            _orderService = orderService;
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
                bool isSignatureValid = validator.ValidateSignature(paymentData.Data, receivedSignature, secretKey);
                if (isSignatureValid)
                {
                    var result = await _orderrepository.GetByIdAsync(int.Parse(paymentData.Data.MerchantOrderId));
                    var orderObject = (Order)result.Data;

                    if (paymentData.Data.Status == "SUCCESS")
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
                            _orderService.SendOrderProcessedEmail(orderDto, orderObject.OrderNumber);
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

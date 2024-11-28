using Business.Email.Validator;
using Infrastructure.Context;
using Infrastructure.IGenericRepository;
using Infrastructure.IRepository;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;

namespace LocalBrand.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebHookController : Controller
    {
        private readonly IGenericRepository<Order> _orderrepository;
        private readonly IGenericRepository<Product> _productrepository;
        private readonly ILogger<ColorController> _logger;

        public WebHookController(IGenericRepository<Order> orderrepository, ILogger<ColorController> logger , IGenericRepository<Product> productrepository)
        {
            _orderrepository = orderrepository;
            _productrepository = productrepository;
            _logger = logger;
        }
        [HttpPost("CompletePayment")]
        public async Task<IActionResult> CompletePayment([FromForm] string paymentStatus, [FromForm] string merchantOrderId)
        {
            try
            {
                var result = await _orderrepository.GetByIdAsync(int.Parse(merchantOrderId));
                if (paymentStatus == "SUCCESS")
                {
                    if (result.Success)
                    {
                       return Ok(new OperationResult { Success = true, Data = true, Message = "Ordered Successfully" }) ;
                    }
                    else
                    {
                        return Ok(result);
                    }
                }
                else
                {
                    var orderobject = (Order)result.Data;
                    foreach (var item in orderobject.OrderDetails)
                    {
                        var productresult = await _productrepository.GetByIdAsync(item.Product.Id);
                        Product product;
                        if (productresult.Success)
                        {
                            product = (Product)productresult.Data;
                            var resulttt = product.Stock.FirstOrDefault(s => s.SizeId == item.SizeId && s.ColorId == item.ColorId);
                            if (resulttt != null)
                            {
                                resulttt.Quantity = resulttt.Quantity + item.Quantity;
                                await _productrepository.SaveChangesAsync();
                            }
                        }

                    }
                    await _orderrepository.DeleteAsync(int.Parse(merchantOrderId));
                    return Ok(new OperationResult { Success = false, Data = false, Message = "Transaction Failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { Message = "Something Went Wrong. Please try again later." });
            }
        }
    }
}

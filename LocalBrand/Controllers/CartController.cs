using Business.Cart.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Model.Models;

namespace LocalBrand.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : Controller
    {
        private readonly ILogger<WishlistController> _logger;
        private readonly ICartService _cartService;
        public CartController(ILogger<WishlistController> logger, ICartService cartService)
        {
            _logger = logger;
            _cartService = cartService;
        }
        [HttpPost("CheckStockQuantity")]
        public async Task<IActionResult> CheckStockQuantity(CartInfo productinfo)
        {
            try
            {
                var result = await _cartService.CheckStockQuantity(productinfo);
                if (string.IsNullOrEmpty(result.DevelopMessage))
                {
                    return Ok(result);
                }
                else
                {
                    _logger.LogError(result.DevelopMessage);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { Message = "Something Went Wrong. Please try again later." });
            }
        }
        [HttpPost("GetCartProducts")]
        public async Task<IActionResult> GetCartProducts(CartInfo[] productsinfo)
        {
            try
            {
                var result = await _cartService.GetCartProducts(productsinfo);
                if (string.IsNullOrEmpty(result.DevelopMessage))
                {
                    return Ok(result);
                }
                else
                {
                    _logger.LogError(result.DevelopMessage);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { Message = "Something Went Wrong. Please try again later."});
            }
        }
    }
}

using Business.Wishlist.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LocalBrand.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : Controller
    {
        private readonly ILogger<WishlistController> _logger;
        private readonly IWishlistService _wishlistService;
        public WishlistController(IWishlistService wishlistService, ILogger<WishlistController> logger)
        {
            _logger = logger;
            _wishlistService = wishlistService;
        }
        [HttpPost("GetWishListProducts")]
        [EnableRateLimiting("GetWishListProductsPolicy")]
        public async Task<IActionResult> GetWishlistProducts([FromBody] int[] Ids)
        {
            try
            {
                var result = await _wishlistService.GetWishlistProducts(Ids);
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
    }
}

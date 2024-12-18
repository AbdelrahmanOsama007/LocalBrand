using Business.Cart.Interfaces;
using Business.Colors.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LocalBrand.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorController : Controller
    {
        private readonly ILogger<ColorController> _logger;
        private readonly IColorService _colorService;
        public ColorController(ILogger<ColorController> logger, IColorService cartService)
        {
            _logger = logger;
            _colorService = cartService;
        }
        [HttpPost("GetAllColors")]
        [EnableRateLimiting("GetAllColorsPolicy")]
        public async Task<IActionResult> GetAllColors()
        {
            try
            {
                var result = await _colorService.GetAllColors();
                if (result.Success)
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

using Business.Colors.Interfaces;
using Business.Sizes.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LocalBrand.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizeController : Controller
    {
        private readonly ILogger<SizeController> _logger;
        private readonly ISizeService _sizeService;
        public SizeController(ILogger<SizeController> logger, ISizeService sizeService)
        {
            _logger = logger;
            _sizeService = sizeService;
        }
        [HttpPost("GetAllSizes")]
        [EnableRateLimiting("GetAllSizesPolicy")]
        public async Task<IActionResult> GetAllSizes()
        {
            try
            {
                var result = await _sizeService.GetAllSizes();
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

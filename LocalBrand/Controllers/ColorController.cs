using Business.Cart.Interfaces;
using Business.Colors.Dtos;
using Business.Colors.Interfaces;
using Infrastructure.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Model.Models;

namespace LocalBrand.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
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
        [HttpPost("GetColorGrid")]
        [EnableRateLimiting("GetColorGridPolicy")]
        public async Task<IActionResult> GetColorGrid(ColorPagination colormodel)
        {
            try
            {
                var result = await _colorService.GetColorGrid(colormodel);
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
        [HttpPost("AddColor")]
        [EnableRateLimiting("AddColorPolicy")]
        public async Task<IActionResult> AddColor(ColorDto color)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _colorService.AddColor(color);
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
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { Message = "Something Went Wrong. Please try again later." });
            }
        }
        [HttpPost("UpdateColor")]
        [EnableRateLimiting("UpdateColorPolicy")]
        public async Task<IActionResult> UpdateColor(ColorDto color)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _colorService.UpdateColor(color);
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
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return StatusCode(500, new { Message = "Something Went Wrong. Please try again later." });
            }
        }
        [HttpPost("GetColorById")]
        [EnableRateLimiting("GetColorByIdPolicy")]
        public async Task<IActionResult> GetColorById([FromBody]int id)
        {
            try
            {
                var result = await _colorService.GetColorById(id);
                if (!string.IsNullOrEmpty(result.DevelopMessage))
                {
                    _logger.LogError(result.DevelopMessage);
                    return Ok(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { Message = "Something Went Wrong. Please try again later." });
            }
        }
        [HttpPost("DeleteColor")]
        [EnableRateLimiting("DeleteColorPolicy")]
        public async Task<IActionResult> DeleteColor([FromBody]int id)
        {
            try
            {
                var result = await _colorService.DeleteColor(id);
                if (!string.IsNullOrEmpty(result.DevelopMessage))
                {
                    _logger.LogError(result.DevelopMessage);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { Message = "Something Went Wrong. Please try again later." });
            }
        }
    }
}

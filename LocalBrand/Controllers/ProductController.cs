using Business;
using Business.Products.Dtos;
using Business.Products.Interfaces;
using System.Collections.Generic;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.IdentityModel.Tokens;
using Model.Models;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.RateLimiting;

namespace LocalBrand.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        private readonly IFileCloudService _fileCloudService;
        private readonly ImageService imageService;
        public ProductController(IProductService productService, ILogger<ProductController> logger, IFileCloudService fileCloudService, ImageService imageService)
        {
            _productService = productService;
            _logger = logger;
            _fileCloudService = fileCloudService;
            this.imageService = imageService;
        }
        [HttpPost("GetAllProducts")]
        [EnableRateLimiting("GetAllProductsPolicy")]
        public async Task<IActionResult> GetAllProducts([FromBody] string? searchQuery = null)
        {
            try
            {
                var result = await _productService.GetAllProductsAsync(searchQuery);
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
        [HttpPost("GetProductById")]
        [EnableRateLimiting("GetProductByIdPolicy")]
        public async Task<IActionResult> GetProductById([FromBody]int id)
        {
            try
            {
                var result = await _productService.GetProductByIdAsync(id);
                if (!string.IsNullOrEmpty(result.DevelopMessage))
                {
                    _logger.LogError(result.DevelopMessage);
                    return Ok(result);
                }
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { Message = "Something Went Wrong. Please try again later." });
            }
        }
        [HttpPost("AddProduct")]
        [EnableRateLimiting("AddProductPolicy")]
        public async Task<IActionResult> AddProduct(AdminProductDto product)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var result = await _productService.AddProductAsync(product);
                    // new file 
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
        [HttpPost("UpdateProduct")]
        [EnableRateLimiting("UpdateProductPolicy")]
        public async Task<IActionResult> UpdateProduct(int id, AdminProductDto product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _productService.UpdateProductAsync(id, product);
                    if (!string.IsNullOrEmpty(result.DevelopMessage))
                    {
                        _logger.LogError(result.DevelopMessage);
                    }
                    return Ok(result);
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
        [HttpPost("DeleteProduct")]
        [EnableRateLimiting("DeleteProductPolicy")]
        public async Task<IActionResult> DeleteProduct([FromBody]int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
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
        [HttpPost("GetProductBySubCategory")]
        [EnableRateLimiting("GetProductBySubCategoryPolicy")]
        public async Task<IActionResult> GetProductBySubCategory([FromBody]int id)
        {
            try
            {
                var result = await _productService.GetProductsBySubCategoryAsync(id);
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
        [HttpPost("GetBestSeller")]
        [EnableRateLimiting("GetBestSellerPolicy")]
        public async Task<IActionResult> GetBestSellerProducts()
        {
            try
            {
                var result = await _productService.GetBestSellers();
                if(result.Success)
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
        [HttpPost("GetProductsByCatId")]
        [EnableRateLimiting("GetProductsByCatIdPolicy")]
        public async Task<IActionResult> GetProductsByCatId([FromBody]int id)
        {
            try
            {
                var result = await _productService.GetProductsByCategoryAsync(id);
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
        [HttpPost("GetSaleProducts")]
        [EnableRateLimiting("GetSaleProductsPolicy")]
        public async Task<IActionResult> GetSaleProducts()
        {
            try
            {
                var result = await _productService.GetSaleProducts();
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
        [HttpPost("uploadImage")]
        [EnableRateLimiting("uploadImagePolicy")]
        public Task< List<string> >uploadImage( [FromBody] List <string> image)
        {
          List<string> imageUrl =   imageService.UploadBase64Images(image);
          return Task.FromResult(imageUrl);
        }
    }
}
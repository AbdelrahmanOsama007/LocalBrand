using Business.Products.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.Models;
using System.Runtime.CompilerServices;

namespace LocalBrand.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult GetAll()
        {
            return Ok();
        }
    }
}

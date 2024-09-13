using Microsoft.AspNetCore.Mvc;

namespace LocalBrand.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult GetAll()
        {
            return Ok();
        }
    }
}

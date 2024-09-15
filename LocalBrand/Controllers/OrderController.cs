using Business.Orders.Validator;
using Microsoft.AspNetCore.Mvc;

namespace LocalBrand.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost("AddNewOrder")]
        public IActionResult AddOrder()
        {
            return View();
        }
    }
}
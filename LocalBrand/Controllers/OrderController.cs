using Business.Orders.Dtos;
using Business.Orders.Interfaces;
using Business.Orders.Validator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Models;

namespace LocalBrand.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<ProductController> _logger;
        public OrderController(IOrderService orderService, ILogger<ProductController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }
        [HttpPost("AddNewOrder")]
        public async Task<IActionResult> AddOrder(OrderDto order)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var result = await _orderService.AddOrderAsync(order);
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
        [HttpPost("UpdateOrder")]
        [Authorize]
        public async Task<IActionResult> UpdateOrder(AdminOrderDto updatedOrder)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _orderService.UpdateOrderAsync(updatedOrder);
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
        [HttpPost("DeleteOrder")]
        [Authorize]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var result = await _orderService.DeleteOrderAsync(id);
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
        [HttpPost("GetAllOrders")]
        [Authorize]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var result = await _orderService.GetAllOrdersAsync();
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
        [HttpPost("GetOrderById")]
        public async Task<IActionResult> GetOrderById(int orderid)
        {
            try
            {
                var result = await _orderService.GetOrderById(orderid);
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
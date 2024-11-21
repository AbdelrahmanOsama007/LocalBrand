using Business.Orders.Dtos;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Orders.Interfaces
{
    public interface IOrderService
    {
        Task<OperationResult> AddOrderAsync(OrderDto order);
        Task<OperationResult> DeleteOrderAsync(int id);
        Task<OperationResult> GetAllOrdersAsync();
        Task<OperationResult> UpdateOrderAsync(AdminOrderDto updatedOrder);
        Task<OperationResult> GetOrderById(int orderid);
    }
}

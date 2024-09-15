using Business.Orders.Dtos;
using Business.Orders.Interfaces;
using Infrastructure.Context;
using Infrastructure.IGenericRepository;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Orders.Validator
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _orderrepository;
        private readonly IGenericRepository<OrderDetails> _orderdetailsrepository;
        private readonly MyAppContext _context;
        public OrderService(IGenericRepository<Order> orderrepository, MyAppContext context, IGenericRepository<OrderDetails> orderdetailsrepository)
        {
            _orderrepository = orderrepository;
            _context = context;
            _orderdetailsrepository = orderdetailsrepository;
        }
        public async Task<OperationResult> AddOrderAsync(OrderDto order)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Order neworder = new Order()
                {
                    OrderNumber = await GenerateUniqueOrderNumber(),
                    OrderDate = order.OrderDate,
                    UserName = order.UserName,
                    UserEmail = order.UserEmail,
                    UserPhone = order.UserPhone,
                    OrderStatus = Model.Enums.OrderStatusEnum.Processing,
                };
                await _orderrepository.AddAsync(neworder);
                foreach (var item in order.Products)
                {
                    var orderitem = new OrderDetails()
                    {
                        ProductId = item.Id,
                        Price = item.Price,
                        Size = item.Size,
                        Color = item.Color,
                        OrderId = neworder.Id,
                    };
                    await _orderdetailsrepository.AddAsync(orderitem);
                }
                await transaction.CommitAsync();
                return new OperationResult() { Success = true, Message = "Ordered Successfully" };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message};
            }

        }
        public Task<OperationResult> DeleteOrderAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<OperationResult> GetAllOrdersAsync()
        {
            throw new NotImplementedException();
        }
        public Task<OperationResult> GetOrderByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<OperationResult> UpdateOrderAsync(int id, Order updatedOrder)
        {
            throw new NotImplementedException();
        }
        private async Task<string> GenerateUniqueOrderNumber()
        {
            string orderNumber;
            Order isUnique;

            do
            {
                // Generate a 7-digit random number
                Random random = new Random();
                orderNumber = random.Next(1000000, 9999999).ToString();

                // Check if it is unique by querying the database
                isUnique = await _context.Orders.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            } while (isUnique != null); // Repeat until a unique number is found

            return orderNumber;
        }
    }
}

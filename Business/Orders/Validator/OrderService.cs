using Business.Orders.Dtos;
using Business.Orders.Interfaces;
using Business.Products.Dtos;
using Infrastructure.Context;
using Infrastructure.IGenericRepository;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Orders.Validator
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _orderrepository;
        private readonly IGenericRepository<OrderDetails> _orderdetailsrepository;
        private readonly IGenericRepository<Product> _productrepository;
        private readonly MyAppContext _context;
        public OrderService(IGenericRepository<Order> orderrepository, MyAppContext context, IGenericRepository<OrderDetails> orderdetailsrepository, IGenericRepository<Product> productrepository)
        {
            _orderrepository = orderrepository;
            _context = context;
            _orderdetailsrepository = orderdetailsrepository;
            _productrepository = productrepository;
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
                    OrderStatus = Model.Enums.OrderStatusEnum.Processing,
                    OrderDetails = new List<OrderDetails>(),
                    UserAddress = new UserAddress() { FirstName = order.FirstName, LastName = order.LastName, City = order.City, StreetAddress = order.StreetAddress, Appartment = order.Appartment,
                                                      PhoneNumber = order.PhoneNumber, Email = order.Email, PaymentMethod = order.PaymentMethod.ToString() }
                };
                decimal subtotal = 0;
                decimal total = 0;

                foreach (var item in order.Products)
                {

                    Product? product = null;
                    var getproduct = await _productrepository.GetByIdAsync(item.ProductId);
                    if (!getproduct.Success)
                    {
                        return getproduct;
                    }

                    product = (Product)getproduct.Data;
                    subtotal += product.Price;
                    total += product.Price - (product.Price * (product.Discount / 100));

                    var orderitem = new OrderDetails()
                    {
                        ProductId = item.ProductId,
                        SubTotalPrice = product.Price,
                        TotalPrice = product.Price - (product.Price * (product.Discount / 100)),
                        SizeId = item.SizeId,
                        ColorId = item.ColorId,
                        Quantity = item.Quantity,
                    };
                    neworder.OrderDetails.Add(orderitem);

                    var result = product.Stock.FirstOrDefault(s => s.SizeId == item.SizeId && s.ColorId == item.ColorId);
                    if (result != null)
                        {
                            result.Quantity = result.Quantity - item.Quantity;
                            await _productrepository.SaveChangesAsync();
                    }
                }

                neworder.SubTotalPrice = subtotal;
                neworder.TotalPrice = total;
                await _orderrepository.AddAsync(neworder);
                await transaction.CommitAsync();
                return new OperationResult() { Success = true, Message = "Ordered Successfully" };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
        public async Task<OperationResult> DeleteOrderAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var orderresult = await _orderrepository.GetByIdAsync(id);
                if (orderresult.Success)
                {
                    var order = (Order)orderresult.Data;
                    foreach(var item in order.OrderDetails)
                    {
                        var productstockresult = await _productrepository.GetByIdAsync(item.ProductId);
                        if (productstockresult.Success)
                        {
                            var productstock = (Product)productstockresult.Data;
                            var result = productstock.Stock.FirstOrDefault(s => s.SizeId == item.SizeId && s.ColorId == item.ColorId);
                            if (result != null)
                            {
                                result.Quantity = result.Quantity + item.Quantity;
                                await _productrepository.SaveChangesAsync();
                            }
                        }
                    }
                }
                var deletionresult = await _orderrepository.DeleteAsync(id);
                await transaction.CommitAsync();
                return deletionresult;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
        public async Task<OperationResult> GetAllOrdersAsync()
        {
            try
            {
                var GetOrdersResult = await _orderrepository.GetAllAsync();
                if (GetOrdersResult.Success)
                {
                    var orders = (List<Order>)GetOrdersResult.Data;
                    var ordersDto = new List<AdminOrderDto>();
                    foreach (var order in orders)
                    {
                        var orderDto = new AdminOrderDto()
                        {
                            OrderDate = order.OrderDate,
                            OrderNumber = order.OrderNumber,
                            FirstName = order.UserAddress.FirstName,
                            LastName = order.UserAddress.LastName,
                            City = order.UserAddress.City,
                            StreetAddress = order.UserAddress.StreetAddress,
                            Appartment = order.UserAddress.Appartment,
                            PhoneNumber = order.UserAddress.PhoneNumber,
                            PaymentMethod = order.UserAddress.PaymentMethod,
                            Email = order.UserAddress.Email,
                            SubTotal = order.SubTotalPrice,
                            Total = order.TotalPrice,
                            Products = new List<UserProductDto>()
                        };
                        foreach (var orderdetail in order.OrderDetails)
                        {
                            var orderdetaildto = new UserProductDto()
                            {
                                ProductId = orderdetail.ProductId,
                                Total = orderdetail.TotalPrice,
                                SubTotal = orderdetail.SubTotalPrice,
                                SizeId = orderdetail.SizeId,
                                ColorId = orderdetail.ColorId,
                                Quantity = orderdetail.Quantity,
                            };
                            orderDto.Products.Add(orderdetaildto);
                        }
                        ordersDto.Add(orderDto);
                    }
                    return new OperationResult() { Success = true, Message = GetOrdersResult.Message, Data = ordersDto };
                }
                return GetOrdersResult;
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
        public async Task<OperationResult> UpdateOrderAsync(int id, OrderDto updatedOrder)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var orderresult = await _orderrepository.GetByIdAsync(id);
                if (!orderresult.Success)
                {
                    return orderresult;
                }
                var order = (Order)orderresult.Data;

                order.OrderDate = updatedOrder.OrderDate;
                order.UserAddress.FirstName = updatedOrder.FirstName;
                order.UserAddress.LastName = updatedOrder.LastName;
                order.UserAddress.City = updatedOrder.City;
                order.UserAddress.Appartment = updatedOrder.Appartment;
                order.UserAddress.StreetAddress = updatedOrder.StreetAddress;
                order.UserAddress.PhoneNumber = updatedOrder.PhoneNumber;
                order.UserAddress.Email = updatedOrder.Email;

                decimal subtotal = 0;
                decimal total = 0;



                var OrderDetailsList = new List<OrderDetails>();
                    foreach (var orderdetail in updatedOrder.Products)
                    {

                        Product? product = null;
                        var getproduct = await _productrepository.GetByIdAsync(orderdetail.ProductId);
                        if (!getproduct.Success)
                        {
                            return getproduct;
                        }

                    product = (Product)getproduct.Data;
                    subtotal += product.Price;
                    total += product.Price - (product.Price * (product.Discount / 100));

                    var orderdetailsobject = new OrderDetails()
                        {
                            SubTotalPrice = product.Price,
                            TotalPrice = product.Price - (product.Price * (product.Discount / 100)),
                            SizeId = orderdetail.SizeId,
                            ColorId = orderdetail.ColorId,
                            Quantity = orderdetail.Quantity,
                            ProductId = orderdetail.ProductId,
                        };
                        OrderDetailsList.Add(orderdetailsobject);

                    var productstockresult = await _productrepository.GetByIdAsync(orderdetail.ProductId);
                    if (productstockresult.Success)
                    {
                        var productstock = (Product)productstockresult.Data;
                        var result = productstock.Stock.FirstOrDefault(s => s.SizeId == orderdetail.SizeId && s.ColorId == orderdetail.ColorId);
                        if (result != null)
                        {
                            var oldorderdetail = order.OrderDetails.FirstOrDefault(d => d.ProductId == result.ProductId && d.SizeId == result.SizeId && d.ColorId == result.ColorId);
                            if (oldorderdetail != null)
                            {
                                if (oldorderdetail.Quantity > orderdetail.Quantity)
                                {
                                    result.Quantity = result.Quantity + (oldorderdetail.Quantity - orderdetail.Quantity);
                                    await _productrepository.SaveChangesAsync();
                                }
                                else if (oldorderdetail.Quantity < orderdetail.Quantity)
                                {
                                    result.Quantity = result.Quantity - (orderdetail.Quantity - oldorderdetail.Quantity);
                                    await _productrepository.SaveChangesAsync();
                                }
                            }
                        }
                    }
                }
                await _orderdetailsrepository.DeleteRangeAsync(order.OrderDetails);
                order.OrderDetails = OrderDetailsList;
                var updateresult = await _orderrepository.UpdateAsync(order);
                await transaction.CommitAsync();
                return updateresult;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
        private async Task<string> GenerateUniqueOrderNumber()
        {
            string orderNumber;
            Order isUnique;

            do
            {
                Random random = new Random();
                orderNumber = random.Next(1000000, 9999999).ToString();

                isUnique = await _context.Orders.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            } while (isUnique != null);

            return orderNumber;
        }
    }
}

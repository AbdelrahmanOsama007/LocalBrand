using Business.Email.Validator;
using Business.Orders.Dtos;
using Business.Orders.Interfaces;
using Business.Products.Dtos;
using Infrastructure.Context;
using Infrastructure.IGenericRepository;
using Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Business.Orders.Validator
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _orderrepository;
        private readonly IGenericRepository<OrderDetails> _orderdetailsrepository;
        private readonly IGenericRepository<Product> _productrepository;
        private readonly IProductRepository _ProductRepository;
        private readonly EmailService _emailService;
        private readonly MyAppContext _context;
        public OrderService(IGenericRepository<Order> orderrepository, MyAppContext context, IGenericRepository<OrderDetails> orderdetailsrepository, IGenericRepository<Product> productrepository , IProductRepository ProductRepository, EmailService emailService)
        {
            _orderrepository = orderrepository;
            _context = context;
            _orderdetailsrepository = orderdetailsrepository;
            _productrepository = productrepository;
            _ProductRepository = ProductRepository;
            _emailService = emailService;
        }
        public async Task<OperationResult> AddOrderAsync(OrderDto order)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var product in order.Products)
                {
                    var productinfo = new CartInfo() { ProductId = product.ProductId, ColorId = product.ColorId, SizeId = product.SizeId, Quantity = product.Quantity };
                    var result = await _ProductRepository.CheckStockQuantity(productinfo);
                    if (result.Success)
                    {
                        var stockproduct = (Stock)result.Data;
                        if (stockproduct.Quantity < productinfo.Quantity)
                        {
                            return new OperationResult() { Success = true, Data = false, QuantityLeek = true, Message = $"Sorry there is one or more product's quantity of your order are not available"};
                        }
                    }
                }
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
                    subtotal += product.Price * item.Quantity;
                    total += (product.Price - (product.Price * ((decimal)product.Discount / 100))) * item.Quantity;

                    var orderitem = new OrderDetails()
                    {
                        ProductId = item.ProductId,
                        PriceAfterDiscount = product.Price - (product.Price * ((decimal)product.Discount / 100)),
                        PriceBeforeDiscount = product.Price,
                        SizeId = item.SizeId,
                        ColorId = item.ColorId,
                        Quantity = item.Quantity,
                    };
                    orderitem.TotalPrice = orderitem.PriceAfterDiscount * orderitem.Quantity;
                    orderitem.SubTotalPrice = orderitem.PriceBeforeDiscount * orderitem.Quantity;
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
                await _orderrepository.SaveChangesAsync();
                await transaction.CommitAsync();
                if(order.PaymentMethod == PaymentMethodEnum.PayOnDelivery)
                {
                    SendOrderProcessedEmail(order);
                    return new OperationResult() { Success = true, Data = true, Message = "Ordered Successfully" };
                }
                var orderinfoobject = new OrderInfo { Id = neworder.Id, TotalPrice = neworder.TotalPrice, Hash = Kashier.create_hash(neworder.Id, neworder.TotalPrice) };
                return new OperationResult() { Success = true, Data = true, OrderAdditionalData = orderinfoobject, Message = "Ordered Successfully" };
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
                            OrderId = order.Id,
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
                            OrderStatus = order.OrderStatus,
                            Products = new List<UserProductDto>()
                        };
                        foreach (var orderdetail in order.OrderDetails)
                        {
                            var orderdetaildto = new UserProductDto()
                            {
                                ProductId = orderdetail.ProductId,
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
        public async Task<OperationResult> GetOrderById(int orderid)
        {
            var result = await _orderrepository.GetByIdAsync(orderid);
            if (!result.Success)
            {
                return new OperationResult() { Success = result.Success, Message = result.Message, OnlinePaymentStatus = false };
            }
            return new OperationResult() { Success = result.Success, Message = result.Message, OnlinePaymentStatus = true };
        }
        public async Task<OperationResult> UpdateOrderAsync(AdminOrderDto updatedOrder)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var orderresult = await _orderrepository.GetByIdAsync(updatedOrder.OrderId);
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
                    total += product.Price - (product.Price * ((decimal)product.Discount / 100));

                    var orderdetailsobject = new OrderDetails()
                        {
                            TotalPrice = product.Price - (product.Price * ((decimal)product.Discount / 100)),
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
                if (updatedOrder.OrderStatus == OrderStatusEnum.Delivered) {
                    order.OrderStatus = updatedOrder.OrderStatus;
                    var OrderDto = new OrderDto() { FirstName = updatedOrder.FirstName, LastName = updatedOrder.LastName, Email = updatedOrder.Email };
                    SendOrderDeliveredEmail(OrderDto);
                }
                else if(updatedOrder.OrderStatus == OrderStatusEnum.Cancelled)
                {
                    order.OrderStatus = updatedOrder.OrderStatus;
                    var OrderDto = new OrderDto() { FirstName = updatedOrder.FirstName, LastName = updatedOrder.LastName, Email = updatedOrder.Email };
                    SendOrderCanceledEmail(OrderDto);
                }
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
        public void SendOrderProcessedEmail(OrderDto order)
        {
            var SentEmail = _emailService.SendEmail(new EmailModel()
            {
                FromName = "Eleve Store",
                ToName = $"{order.FirstName} {order.LastName}",
                ToEmail = order.Email,
                Subject = "Order Confirmation",
                Body = $@"<div style='width: 100%; max-width: 600px; margin: 20px auto; background-color: #fff; padding: 20px; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);'>
                        <div style='text-align: center; padding: 10px 0;'>
                            <img src='https://orca-app-sw4g7.ondigitalocean.app/images/logo.png' alt='Eleve Store Logo' style='max-width: 200px; height: auto;' />
                        </div>
                        <div style='font-size: 16px; line-height: 1.5;'>
                            <h2 style='font-size: 24px; color: #333; text-align:center'>Order Confirmation</h2>
                            <p>Dear <strong>{order.FirstName} {order.LastName}</strong>,</p>
                            <p>Thank you for shopping with <strong>Eleve Store</strong>! Your order has been successfully placed.</p>
                            <p>We will deliver it for you as soon as possible.</p>
                            <p>If you have any questions or need assistance, feel free to contact our support team.</p>
                        </div>
                        <div style='text-align: center; font-size: 12px; color: #888; padding-top: 20px;'>
                            <p>&copy; 2024 Eleve Store | All rights reserved</p>
                        </div>
                    </div>"
            });
        }
        public void SendOrderDeliveredEmail(OrderDto order) {
            var SentEmail = _emailService.SendEmail(new EmailModel()
            {
                FromName = "Eleve Store",
                ToName = $"{order.FirstName} {order.LastName}",
                ToEmail = order.Email,
                Subject = "Order Delevired",
                Body = $@"<div style='width: 100%; max-width: 600px; margin: 20px auto; background-color: #fff; padding: 20px; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);'>
                        <div style='text-align: center; padding: 10px 0;'>
                            <img src='https://orca-app-sw4g7.ondigitalocean.app/images/logo.png' alt='Eleve Store Logo' style='max-width: 200px; height: auto;' />
                        </div>
                        <div style='font-size: 16px; line-height: 1.5;'>
                            <h2 style='font-size: 24px; color: #333; text-align:center'>Order Delevired</h2>
                            <p>Dear <strong>{order.FirstName} {order.LastName}</strong>,</p>
                            <p>Thank you for shopping with <strong>Eleve Store</strong>! Your order has been successfully delevired.</p>
                            <p>If you have any feedback , questions or need assistance, feel free to contact our support team.</p>
                        </div>
                        <div style='text-align: center; font-size: 12px; color: #888; padding-top: 20px;'>
                            <p>&copy; 2024 Eleve Store | All rights reserved</p>
                        </div>
                    </div>"
            });
        }
        public void SendOrderCanceledEmail(OrderDto order)
        {
            var SentEmail = _emailService.SendEmail(new EmailModel()
            {
                FromName = "Eleve Store",
                ToName = $"{order.FirstName} {order.LastName}",
                ToEmail = order.Email,
                Subject = "Order Canceled",
                Body = $@"<div style='width: 100%; max-width: 600px; margin: 20px auto; background-color: #fff; padding: 20px; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);'>
                        <div style='text-align: center; padding: 10px 0;'>
                            <img src='https://orca-app-sw4g7.ondigitalocean.app/images/logo.png' alt='Eleve Store Logo' style='max-width: 200px; height: auto;' />
                        </div>
                        <div style='font-size: 16px; line-height: 1.5;'>
                            <h2 style='font-size: 24px; color: #333; text-align:center'>Order Canceled</h2>
                            <p>Dear <strong>{order.FirstName} {order.LastName}</strong>,</p>
                            <p>Thank you for shopping with <strong>Eleve Store</strong>! We are sorry to tell you that your order has been successfully canceled.</p>
                            <p>If you have any feedback , questions or need assistance, feel free to contact our support team.</p>
                        </div>
                        <div style='text-align: center; font-size: 12px; color: #888; padding-top: 20px;'>
                            <p>&copy; 2024 Eleve Store | All rights reserved</p>
                        </div>
                    </div>"
            });
        }
    }
    public class Kashier
    {
        public static string create_hash(int orderId, decimal amount)
        {
            string mid = "MID-29963-501";
            string currency = "EGP";
            string secret = "224067ad-549d-41e0-a1b0-093ee0b996a0";
            string ORDERID = $"{orderId}";
            string AMOUNT = $"{(int)amount}";
            string path = "/?payment=" + mid + "." + ORDERID + "." + AMOUNT + "." + currency;
            Console.WriteLine(path);
            string message;
            string key;
            key = secret;
            message = path;
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(key);
            byte[] messageBytes = encoding.GetBytes(message);
            HMACSHA256 hmacmd256 = new HMACSHA256(keyByte);
            byte[] hashmessage = hmacmd256.ComputeHash(messageBytes);
            return ByteToString(hashmessage).ToLower();
        }
        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2");
            }
            return (sbinary);
        }
    }
}

using Infrastructure.IGenericRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Model.Enums;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.OrderCleanUp.Validator
{
    public class OrderCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderCleanupService> _logger;

        public OrderCleanupService(IServiceProvider serviceProvider, ILogger<OrderCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var orderRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Order>>();
                    var productRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Product>>();
                    var staleOrdersResult = await orderRepository.GetAllAsync();
                    if (staleOrdersResult.Success && staleOrdersResult.Data is List<Order> staleOrders)
                    {
                        var filteredOrders = staleOrders
                            .Where(o => !o.IsTransactionSuccess && o.PaymentMethod == PaymentMethodEnum.OnlinePayment && o.OrderDate.AddMinutes(5) <= DateTime.UtcNow && !o.IsDeleted).ToList();

                        foreach (var order in filteredOrders)
                        {
                            foreach (var item in order.OrderDetails)
                            {
                                var productResult = await productRepository.GetByIdAsync(item.ProductId);
                                if (productResult.Success && productResult.Data is Product product)
                                {
                                    var resulttt = product.Stock.FirstOrDefault(s => s.SizeId == item.SizeId && s.ColorId == item.ColorId);
                                    if (resulttt != null)
                                    {
                                        resulttt.Quantity = resulttt.Quantity + item.Quantity;
                                        await productRepository.SaveChangesAsync();
                                    }
                                }
                            }
                            order.IsDeleted = true;
                            await orderRepository.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No stale orders found or an error occurred while retrieving orders.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while cleaning up stale orders.");
                }
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}

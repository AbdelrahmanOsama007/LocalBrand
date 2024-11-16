using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Cart.Interfaces
{
    public interface ICartService
    {
        Task<OperationResult> CheckStockQuantity(CartInfo productinfo);
        Task<OperationResult> GetCartProducts(CartInfo[] productsinfo);
    }
}

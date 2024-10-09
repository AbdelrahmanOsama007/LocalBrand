using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.IRepository
{
    public interface IProductRepository
    {
        Task<OperationResult> GetProductsBySubCaregory(int id);
        Task<OperationResult> GetProductsByCategoryId(int categoryid);
        Task<OperationResult> GetBestSellers();
        Task<OperationResult> GetWishlistProducts(int[] Ids);
        Task<OperationResult> CheckStockQuantity(CartInfo productinfo);
    }
}

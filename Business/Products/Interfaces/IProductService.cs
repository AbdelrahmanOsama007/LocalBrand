using Business.Products.Dtos;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Products.Interfaces
{
    public interface IProductService
    {
        Task<OperationResult> AddProductAsync(Product product);
        Task<OperationResult> DeleteProductAsync(int id);
        Task<OperationResult> GetAllProductsAsync();
        Task<OperationResult> GetProductByIdAsync(int id);
        Task<OperationResult> UpdateProductAsync(int id, Product updatedProduct);
    }
}

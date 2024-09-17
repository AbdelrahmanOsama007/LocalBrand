using Business.Products.Dtos;
using Business.Products.Interfaces;
using Infrastructure.IGenericRepository;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Products.Validator
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _repository;
        public ProductService(IGenericRepository<Product> repository)
        {
            _repository = repository;
        }
        public async Task<OperationResult> AddProductAsync(Product product)
        {
            return await _repository.AddAsync(product);
        }

        public async Task<OperationResult> DeleteProductAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<OperationResult> GetAllProductsAsync()
        {
            var result = await _repository.GetAllAsync();
            if (result.Success)
            {
                var productlist = (List<Product>)result.Data;
                var productdtolist = new List<ProductDto>();
                foreach (var product in productlist)
                {
                    var productdto = new ProductDto()
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        Discount = product.Discount,
                        CategoryId = product.CategoryId,
                    };
                    productdtolist.Add(productdto);
                }
                return new OperationResult() { Success = true , Message = result.Message, Data = productdtolist};
            }
            else
            {
                return result;
            }
        }

        public async Task<OperationResult> GetProductByIdAsync(int id)
        {
            var result = await _repository.GetByIdAsync(id);
            if(result.Success)
            {
                var productobject = (Product)result.Data;
                ProductDto productdto = new ProductDto()
                {
                    Id = productobject.Id,
                    Name = productobject.Name,
                    Discount = productobject.Discount,
                    Price = productobject.Price,
                    Description = productobject.Description,
                    CategoryId= productobject.CategoryId,
                };
                return new OperationResult() { Success = true , Message = result.Message, Data = productdto};
            }
            else
            {
                return result;
            }
        }

        public async Task<OperationResult> UpdateProductAsync(int id, Product updatedProduct)
        {
            return await _repository.UpdateAsync(id, updatedProduct);
        }
    }
}

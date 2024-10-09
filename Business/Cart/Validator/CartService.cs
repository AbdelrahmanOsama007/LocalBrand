using Business.Cart.Interfaces;
using Infrastructure.IRepository;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Cart.Validator
{
    public class CartService: ICartService
    {
        private readonly IProductRepository _productRepository;
        public CartService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<OperationResult> CheckStockQuantity(CartInfo productinfo)
        {
            try
            {
                var result = await _productRepository.CheckStockQuantity(productinfo);
                if (result.Success)
                {
                    var product = (Stock)result.Data;
                    if (product.Quantity >= productinfo.Quantity + 1)
                    {
                        return new OperationResult() { Success = true, Message = "Quantity is available", Data = true };
                    }
                    else
                    {
                        return new OperationResult() { Success = true, Message = "Quantity is not available", Data = false };
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
    }
}

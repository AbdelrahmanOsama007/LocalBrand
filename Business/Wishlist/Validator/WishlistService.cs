using Business.Wishlist.Dtos;
using Business.Wishlist.Interfaces;
using Infrastructure.IRepository;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Wishlist.Validator
{
    public class WishlistService : IWishlistService
    {
        private readonly IProductRepository _productRepository;
        public WishlistService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<OperationResult> GetWishlistProducts(int[] Ids)
        {
            try
            {
                var result = await _productRepository.GetWishlistProducts(Ids);
                if (!result.Success)
                {
                    return result;
                }

                var products = (List<Product>)result.Data;
                var wishlistproducts = new List<WishlistDto>();

                foreach(var product in products)
                {
                    var productdto = new WishlistDto()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Image = product.ProductImages.ToList()[0].Name
                    };
                    wishlistproducts.Add(productdto);
                }
                return new OperationResult() { Success = true, Data = wishlistproducts, Message = result.Message };
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
    }
}
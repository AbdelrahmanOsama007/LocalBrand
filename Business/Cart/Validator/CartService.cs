using Business.Cart.Dtos;
using Business.Cart.Interfaces;
using Business.Products.Dtos;
using Business.Wishlist.Dtos;
using Infrastructure.IGenericRepository;
using Infrastructure.IRepository;
using Infrastructure.Repository;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Business.Cart.Validator
{
    public class CartService: ICartService
    {
        private readonly IProductRepository _productRepository;
        private readonly IGenericRepository<Product> _productrepository;
        private readonly IGenericRepository<Color> _colorrepository;
        private readonly IGenericRepository<Size> _sizerepository;
        public CartService(IProductRepository productRepository , IGenericRepository<Product> productrepository, IGenericRepository<Color> colorrepository, IGenericRepository<Size> sizerepository)
        {
            _productRepository = productRepository;
            _productrepository = productrepository;
            _colorrepository = colorrepository;
            _sizerepository = sizerepository;
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
                        return new OperationResult() { Success = true, Message = "Quantity is not available", Data = false, AdditionalData = product.Quantity };
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
        public async Task<OperationResult> GetCartProducts(CartInfo[] productsinfo)
        {
            try
            {
                var cartproductsdto = new List<CartlistDto>();
                foreach (var productinfo in productsinfo)
                {
                    var productresult = await _productrepository.GetByIdAsync(productinfo.ProductId);
                    var colorresult = await _colorrepository.GetByIdAsync(productinfo.ColorId);
                    var sizeresult = await _sizerepository.GetByIdAsync(productinfo.SizeId);
                    if (productresult.Success)
                    {
                        var productobject = (Product)productresult.Data;
                        var productdto = new CartlistDto()
                        {
                            ProductId = productobject.Id,
                            ProductName = productobject.Name,
                            Image = productobject.ProductImages.FirstOrDefault(i => i.Id == (productobject.ProductColorImages.FirstOrDefault(i => i.ProductId == productinfo.ProductId && i.ColorId == productinfo.ColorId).ImageId)).Name,
                            Quantity = productinfo.Quantity,
                            PriceBeforeDiscount = productobject.Price,
                            Discount = productobject.Discount,
                            ColorId = productinfo.ColorId,
                            SizeId = productinfo.SizeId,
                        };
                        if (productobject.Discount > 0) {
                            productdto.PriceAfterDiscount = productdto.PriceBeforeDiscount - (productdto.PriceBeforeDiscount * productobject.Discount / 100);
                        }
                        else
                        {
                            productdto.PriceAfterDiscount = productdto.PriceBeforeDiscount;
                        }
                        var result = await CheckStockQuantity(productinfo);
                        if ((bool)result.Data == false)
                        {
                            productdto.Quantity = result.AdditionalData;
                        }
                        if (colorresult.Success)
                        {
                            var colorobject = (Color)colorresult.Data;
                            productdto.ColorCode = colorobject.ColorCode;
                        }
                        if (sizeresult.Success)
                        {
                            var sizeobject = (Size)sizeresult.Data;
                            productdto.Size = sizeobject.SizeKey;
                        }
                        cartproductsdto.Add(productdto);
                    }
                }
                return new OperationResult() { Success = true, Data = cartproductsdto };
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
    }
}

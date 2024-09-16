using Business.Images.Dtos;
using Business.Products.Dtos;
using Business.Products.Interfaces;
using Infrastructure.Context;
using Infrastructure.IGenericRepository;
using Model.Enums;
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
        private readonly IGenericRepository<Product> _productrepository;
        private readonly IGenericRepository<Stock> _stockrepository;
        private readonly IGenericRepository<ProductImage> _productimagerepository;
        private readonly IGenericRepository<ProductColorImage> _productcolorimagerepository;
        public ProductService(IGenericRepository<Product> productrepository, IGenericRepository<Stock> stockrepository, IGenericRepository<ProductImage> productimagerepository, IGenericRepository<ProductColorImage> productcolorimagerepository)
        {
            _productrepository = productrepository;
            _stockrepository = stockrepository;
            _productimagerepository = productimagerepository;
            _productcolorimagerepository = productcolorimagerepository;
        }
        public async Task<OperationResult> AddProductAsync(AdminProductDto product)
        {
            try
            {
                var productobject = new Product()
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Discount = product.Discount,
                    SubCategoryId = product.SubCategoryId,
                    ProductImages = new List<ProductImage>(),
                    Stock = new List<Stock>(),
                    ProductColorImages = new List<ProductColorImage>()
                };

                foreach(var item in product.Stocks)
                {
                    productobject.Stock.Add(new Stock()
                    {
                        ProductId = productobject.Id,
                        SizeId = item.SizeId,
                        ColorId = item.ColorId,
                        Quantity = item.Quantity
                    });
                }
                
                foreach(var item in product.Images)
                {
                    foreach(var Url in item.ImageUrls)
                    {
                        var newimageobject = new ProductImage()
                        {
                            Name = Url,
                            ProductId = productobject.Id,
                            ProductColorImages = new List<ProductColorImage>()
                        };
                        productobject.ProductImages.Add(newimageobject);

                        var newproductcolorimage = new ProductColorImage()
                        {
                            ProductId = productobject.Id,
                            ColorId = item.ColorId,
                            ImageId = newimageobject.Id
                        };

                        productobject.ProductColorImages.Add(newproductcolorimage);
                        newimageobject.ProductColorImages.Add(newproductcolorimage);
                    }
                }

                return await _productrepository.AddAsync(productobject);
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }

        }

        public async Task<OperationResult> DeleteProductAsync(int id)
        {
            return await _productrepository.DeleteAsync(id);
        }

        public async Task<OperationResult> GetAllProductsAsync()
        {
            var result = await _productrepository.GetAllAsync();
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
                        Price = product.Price,
                        Discount = product.Discount,
                        CategoryId = product.SubCategoryId,
                        ImageUrl = product.ProductImages.ToList()[0].Name,
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
            var result = await _productrepository.GetByIdAsync(id);
            if(result.Success)
            {
                var productobject = (Product)result.Data;
                var ProductInfo = new List<Tuple<string, string, int>>();

                foreach (var stock in productobject.Stock)
                {
                    ProductInfo.Add(new Tuple<string, string, int>(((SizeEnum)stock.SizeId).ToString(),((ColorEnum)stock.ColorId).ToString(),stock.Quantity));
                }

                var ColorImagesList = new List<ColorImagesDto>();
                foreach(var colorimages in productobject.ProductColorImages)
                {
                    var colorEntry = ColorImagesList.FirstOrDefault(c => c.ColorId == colorimages.ColorId);
                    if(colorEntry == null)
                    {
                        colorEntry = new ColorImagesDto()
                        {
                            ColorId = colorimages.ColorId,
                            ImageUrls = new List<string>()
                        };
                        ColorImagesList.Add(colorEntry);
                    }

                    var getImage = await _productimagerepository.GetByIdAsync(colorimages.ImageId);
                    if (getImage.Success)
                    {
                        var ImageUrl = ((ProductImage)getImage.Data).Name;
                        colorEntry.ImageUrls.Add(ImageUrl);
                    }
                }

                ProductDetailsDto productdto = new ProductDetailsDto()
                {
                    Id = productobject.Id,
                    Name = productobject.Name,
                    Discount = productobject.Discount,
                    Price = productobject.Price,
                    CategoryId= productobject.SubCategoryId,
                    SizesAndColorsQuantity = ProductInfo,
                    ColorImages = ColorImagesList
                };
                return new OperationResult() { Success = true , Message = result.Message, Data = productdto};
            }
            else
            {
                return result;
            }
        }

        public async Task<OperationResult> UpdateProductAsync(int id, AdminProductDto updatedProduct)
        {
            using var transaction = await _productrepository.BeginTransactionAsync();
            try
            {
                var existingProductResult = await _productrepository.GetByIdAsync(id);

                if (!existingProductResult.Success)
                {
                    return existingProductResult;
                }

                var existingProduct = (Product)existingProductResult.Data;

                    existingProduct.Name = updatedProduct.Name;
                    existingProduct.Description = updatedProduct.Description;
                    existingProduct.Price = updatedProduct.Price;
                    existingProduct.Discount = updatedProduct.Discount;
                    existingProduct.SubCategoryId = updatedProduct.SubCategoryId;

                    await _productcolorimagerepository.DeleteRangeAsync(existingProduct.ProductColorImages);
                    await _productimagerepository.DeleteRangeAsync(existingProduct.ProductImages);
                    await _stockrepository.DeleteRangeAsync(existingProduct.Stock);

                    foreach (var item in updatedProduct.Stocks)
                    {
                        existingProduct.Stock.Add(new Stock()
                        {
                            ProductId = existingProduct.Id,
                            SizeId = item.SizeId,
                            ColorId = item.ColorId,
                            Quantity = item.Quantity
                        });
                    }

                    foreach (var item in updatedProduct.Images)
                    {
                        foreach(var Url in item.ImageUrls)
                        {
                            var newimageobject = new ProductImage()
                            {
                                Name = Url,
                                ProductId = existingProduct.Id,
                                ProductColorImages = new List<ProductColorImage>()
                            };
                            existingProduct.ProductImages.Add(newimageobject);

                            var newproductcolorimage = new ProductColorImage()
                            {
                                ProductId = existingProduct.Id,
                                ColorId = item.ColorId,
                                ImageId = newimageobject.Id,
                            };

                            existingProduct.ProductColorImages.Add(newproductcolorimage);
                            newimageobject.ProductColorImages.Add(newproductcolorimage);
                        }

                    }

                    var updateresult = await _productrepository.UpdateAsync(existingProduct);
                    await transaction.CommitAsync();
                    return updateresult;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
    }
}

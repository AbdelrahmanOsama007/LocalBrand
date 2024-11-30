using Business.Images.Dtos;
using Business.Products.Dtos;
using Business.Products.Interfaces;
using Infrastructure.Context;
using Infrastructure.IGenericRepository;
using Infrastructure.IRepository;
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
        private readonly IProductRepository _productRepository;
        public ProductService(IGenericRepository<Product> productrepository, IGenericRepository<Stock> stockrepository, IGenericRepository<ProductImage> productimagerepository, IGenericRepository<ProductColorImage> productcolorimagerepository, IProductRepository productRepository)
        {
            _productrepository = productrepository;
            _stockrepository = stockrepository;
            _productimagerepository = productimagerepository;
            _productcolorimagerepository = productcolorimagerepository;
            _productRepository = productRepository;
        }
        public async Task<OperationResult> AddProductAsync(AdminProductDto product)
        {
            try
            {
                var productobject = new Product()
                {
                    Name = product.Name,
                    FullDescription = product.FullDescription,
                    Summary = product.Summary,
                    Price = product.Price,
                    Discount = product.Discount,
                    SubCategoryId = product.SubCategoryId,
                    BestSeller = product.BestSeller,
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
        public async Task<OperationResult> GetAllProductsAsync(string? searchQuery = null)
        {
            try
            {
                var result = await _productrepository.GetAllAsync(searchQuery);
                if (result.Success)
                {
                    var productlist = (List<Product>)result.Data;
                    var productdtolist = new List<ProductDto>();
                    if(productlist.Count > 0)
                    {
                        foreach (var product in productlist)
                        {
                            var IsOutOfStock = true;
                            IsOutOfStock = CheckStock(product);

                            var ActualPrice = GetActualPrice(product.Price, product.Discount);

                            var productdto = new ProductDto()
                            {
                                Id = product.Id,
                                Name = product.Name,
                                PriceBeforeDiscount = product.Price,
                                PriceAfterDiscount = ActualPrice,
                                Discount = product.Discount,
                                SubCategoryId = product.SubCategoryId,
                                SubCategoryName = product.SubCategory.Name,
                                Images = new List<string>() { product.ProductImages.ToList()[0].Name, product.ProductImages.ToList()[1].Name },
                                IsOutOfStock = IsOutOfStock
                            };
                            productdtolist.Add(productdto);
                        }
                    }
                    return new OperationResult() { Success = true, Message = result.Message, Data = productdtolist };
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
        public async Task<OperationResult> GetBestSellers()
        {
            try
            {
                var productlistresult = await _productRepository.GetBestSellers();

                if (!productlistresult.Success)
                {
                    return productlistresult;
                }

                var productlist = (List<Product>)productlistresult.Data;
                var productsdto = new List<ProductDto>();
                foreach (var product in productlist)
                {
                    var IsOutOfStock = true;
                    IsOutOfStock = CheckStock(product);

                    var ActualPrice = GetActualPrice(product.Price, product.Discount);

                    var peoductdto = new ProductDto()
                    {
                        Id = product.Id,
                        Name = product.Name,
                        PriceBeforeDiscount = product.Price,
                        PriceAfterDiscount = ActualPrice,
                        Discount = product.Discount,
                        SubCategoryId = product.SubCategoryId,
                        Images = new List<string>() { product.ProductImages.ToList()[0].Name , product.ProductImages.ToList()[1].Name },
                        IsOutOfStock = IsOutOfStock
                    };
                    productsdto.Add(peoductdto);
                }
                return new OperationResult() { Success = true, Message = productlistresult.Message, Data = productsdto };
            }
            catch(Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
        public async Task<OperationResult> GetProductByIdAsync(int id)
        {
            var result = await _productrepository.GetByIdAsync(id);
            if(result.Success)
            {
                var productobject = (Product)result.Data;
                var ProductInfo = new List<ProductInfoDto>();

                foreach (var stock in productobject.Stock)
                {
                    if(stock.Quantity > 0)
                    {
                        ProductInfo.Add(new ProductInfoDto() { SizeId = stock.SizeId, ColorId = stock.ColorId, SizeName = stock.Size.SizeKey, Quantity = stock.Quantity});
                    }
                    else
                    {
                        ProductInfo.Add(new ProductInfoDto() {ColorId = stock.ColorId, Quantity = stock.Quantity });
                    }
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
                            ColorCode = colorimages.Color.ColorCode,
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

                var IsOutOfStock = true;
                IsOutOfStock = CheckStock(productobject);
                var ActualPrice = GetActualPrice(productobject.Price, productobject.Discount);

                ProductDetailsDto productdto = new ProductDetailsDto()
                {
                    Id = productobject.Id,
                    Name = productobject.Name,
                    Discount = productobject.Discount,
                    Summary = productobject.Summary,
                    FullDescription = productobject.FullDescription,
                    PriceBeforeDiscount = productobject.Price,
                    PriceAfterDiscount = ActualPrice,
                    SubCategoryId = productobject.SubCategoryId,
                    SubCategoryName = productobject.SubCategory.Name,
                    CategoryId = productobject.SubCategory.Category.Id,
                    CategoryName = productobject.SubCategory.Category.Name,
                    SizesAndColorsQuantity = ProductInfo,
                    ColorImages = ColorImagesList,
                    IsOutOfStock= IsOutOfStock,
                };
                return new OperationResult() { Success = true , Message = result.Message, Data = productdto};
            }
            else
            {
                return result;
            }
        }
        public async Task<OperationResult> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                var result = await _productRepository.GetProductsByCategoryId(categoryId);
                if (!result.Success)
                {
                    return result;
                }

                var productlist = (List<Product>)result.Data;
                var productdtolist = new List<ProductDto>();

                foreach (var product in productlist)
                {
                    var IsOutOfStock = true;
                    IsOutOfStock = CheckStock(product);

                    var ActualPrice = GetActualPrice(product.Price, product.Discount);

                    var productdto = new ProductDto()
                    {
                        Id = product.Id,
                        Name = product.Name,
                        PriceBeforeDiscount = product.Price,
                        PriceAfterDiscount = ActualPrice,
                        Discount = product.Discount,
                        SubCategoryId = product.SubCategoryId,
                        Images = new List<string>() { product.ProductImages.ToList()[0].Name, product.ProductImages.ToList()[1].Name },
                        IsOutOfStock= IsOutOfStock
                    };
                    productdtolist.Add(productdto);
                }
                return new OperationResult() { Success = true, Message = result.Message, Data = productdtolist };
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
        public async Task<OperationResult> GetProductsBySubCategoryAsync(int id)
        {
            try
            {
                var result = await _productRepository.GetProductsBySubCaregory(id);
                if (!result.Success)
                {
                    return result;
                }

                var productlist = (List<Product>)result.Data;
                var productdtolist = new List<ProductDto>();

                foreach (var product in productlist)
                {
                    var IsOutOfStock = true;
                    IsOutOfStock = CheckStock(product);

                    var ActualPrice = GetActualPrice(product.Price, product.Discount);

                    var productdto = new ProductDto()
                    {
                        Id = product.Id,
                        Name = product.Name,
                        PriceBeforeDiscount = product.Price,
                        PriceAfterDiscount = ActualPrice,
                        Discount = product.Discount,
                        SubCategoryId = product.SubCategoryId,
                        Images = new List<string>() { product.ProductImages.ToList()[0].Name, product.ProductImages.ToList()[1].Name },
                        IsOutOfStock = IsOutOfStock
                    };
                    productdtolist.Add(productdto);
                }
                return new OperationResult() { Success = true, Message = result.Message, Data = productdtolist };
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
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
                    existingProduct.FullDescription = updatedProduct.FullDescription;
                    existingProduct.Summary = updatedProduct.Summary;
                    existingProduct.Price = updatedProduct.Price;
                    existingProduct.Discount = updatedProduct.Discount;
                    existingProduct.SubCategoryId = updatedProduct.SubCategoryId;
                    existingProduct.BestSeller = updatedProduct.BestSeller;

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
        private bool CheckStock (Product product)
        {
            foreach (var stock in product.Stock)
            {
                if (stock.Quantity != 0)
                {
                    return false;
                }
            }
            return true;
        }
        private decimal GetActualPrice(decimal price, int discount)
        {
            decimal ActualPrice;
            if(discount != 0)
            {
                ActualPrice = price - (price * discount / 100);
                return ActualPrice;
            }
            ActualPrice = price;
            return ActualPrice;
        }
    }
}
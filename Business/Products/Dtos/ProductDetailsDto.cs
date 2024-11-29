using Business.Images.Dtos;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Products.Dtos
{
    public class ProductDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal PriceBeforeDiscount { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public string Summary { get; set; }
        public string FullDescription { get; set; }
        public int Discount { get; set; } = 0;
        public int SubCategoryId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsOutOfStock { get; set; }
        public List<ColorImagesDto> ColorImages { get; set; }
        public List<ProductInfoDto> SizesAndColorsQuantity { get; set; }
    }
}

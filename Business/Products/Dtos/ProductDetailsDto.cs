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
        public decimal Price { get; set; }
        public int Discount { get; set; } = 0;
        public int SubCategoryId { get; set; }
        public bool IsOutOfStock { get; set; }
        public List<ColorImagesDto> ColorImages { get; set; }
        public List<Tuple<string, string, int>> SizesAndColorsQuantity { get; set; } = new List<Tuple<string, string, int>>();
    }
}

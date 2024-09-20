using Business.Images.Dtos;
using Business.Stocks.Dtos;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Products.Dtos
{
    public class AdminProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public bool BestSeller { get; set; }
        public int SubCategoryId { get; set; }
        public List<StockDto> Stocks { get; set; }
        public List<ColorImagesDto> Images { get; set; }
    }
}

    using Business.Images.Dtos;
    using Business.Stocks.Dtos;
    using Model.Enums;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Business.Products.Dtos
    {
        public class AdminProductDto
        {
            [Required]
            [MinLength(5)]
            [MaxLength(150)]
            [RegularExpression(@"^[a-zA-Z\s\-]+$", ErrorMessage = "Product Name can only contain letters, spaces, and hyphens.")]
            public string Name { get; set; }
            [Required]
            [MinLength(20)]
            [MaxLength(800)]
            public string Description { get; set; }
            [Required]
            [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
            public decimal Price { get; set; }
            [Required]
            [Range(0, 100, ErrorMessage = "Discount must be 0 or a positive number.")]
            public int Discount { get; set; }
            [Required]
            public bool BestSeller { get; set; }
            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "SubCategoryId must be a positive number.")]
            public int SubCategoryId { get; set; }
            [Required]
            public List<StockDto> Stocks { get; set; }
            [Required]
            public List<ColorImagesDto> Images { get; set; }
        }
    }

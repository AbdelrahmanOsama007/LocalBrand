using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Products.Dtos
{
    public class UserProductDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ProductId must be a positive number.")]
        public int ProductId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "SizeId must be a positive number.")]
        public int SizeId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ColorId must be a positive number.")]
        public int ColorId { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "SubTotal must be a positive number.")]
        public decimal SubTotal { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total must be a positive number.")]
        public decimal Total { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
        public int Quantity { get; set; }
    }
}
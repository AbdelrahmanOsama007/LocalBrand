using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Stocks.Dtos
{
    public class StockDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "SizeId must be a positive number.")]
        public int SizeId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ColorId must be a positive number.")]
        public int ColorId { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be 0 or a positive number.")]
        public int Quantity { get; set; }
    }
}

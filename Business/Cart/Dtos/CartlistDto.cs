using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Cart.Dtos
{
    public class CartlistDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ColorCode { get; set; }
        public int ColorId { get; set; }
        public string Size { get; set; }
        public int SizeId { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public decimal PriceBeforeDiscount { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public int Discount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Products.Dtos
{
    public class UserProductDto
    {
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int ColorId { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public int Quantity { get; set; }
    }
}
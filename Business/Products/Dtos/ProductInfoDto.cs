using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Products.Dtos
{
    public class ProductInfoDto
    {
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        public int ColorId { get; set; }
        public int Quantity { get; set; }
    }
}

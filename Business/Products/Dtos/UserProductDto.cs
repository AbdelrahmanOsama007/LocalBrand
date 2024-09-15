using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Products.Dtos
{
    public class UserProductDto
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
    }
}
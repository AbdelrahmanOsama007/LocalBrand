using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Wishlist.Dtos
{
    public class WishlistDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image {  get; set; }
        public decimal Price { get; set; }
    }
}

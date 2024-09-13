using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public List<ProductImage> Images { get; set; }
        public List<ProductSize> Sizes { get; set; }
        public List<ProductColor> Colors { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
        public Category Category { get; set; }
    }
}

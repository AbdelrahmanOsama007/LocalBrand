using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class Product: ISoftDelete
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; } = 0;
        public bool BestSeller { get; set; }
        [ForeignKey("SubCategory")]
        public int SubCategoryId { get; set; }
        public virtual List<OrderDetails> OrderDetails { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public virtual ICollection<Stock> Stock { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<ProductColorImage> ProductColorImages { get; set; }
        public bool IsDeleted { get; set; }
    }
}

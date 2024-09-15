using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [ForeignKey("Size")]
        public int SizeId { get; set; }
        public virtual Size Size { get; set; }

        [ForeignKey("Color")]
        public int ColorId { get; set; }
        public virtual Color Color { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class ProductColorImage
    {
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [ForeignKey("Color")]
        public int ColorId { get; set; }
        public virtual Color Color { get; set; }

        [ForeignKey("Image")]
        public int ImageId { get; set; }
        public virtual ProductImage Image { get; set; }
    }
}

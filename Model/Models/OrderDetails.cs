using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class OrderDetails: ISoftDelete
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public virtual Product Product { get; set; }
        public virtual Order Order { get; set; }
        public bool IsDeleted { get; set; }
    }
}

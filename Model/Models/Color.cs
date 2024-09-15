using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class Color
    {
        public int Id { get; set; }
        public string ColorName { get; set; }
        public virtual ICollection<Stock> Stocks { get; set; }
        public virtual ICollection<ProductColorImage> ProductColorImages { get; set; }
    }
}

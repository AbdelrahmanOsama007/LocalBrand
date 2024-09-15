using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class Size
    {
        public int Id { get; set; }
        public string SizeName { get; set; }
        public int Indicator { get; set; }
        public virtual ICollection<Stock> Stock { get; set; }
    }
}
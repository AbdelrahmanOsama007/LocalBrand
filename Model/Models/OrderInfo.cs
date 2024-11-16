using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class OrderInfo
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public string Hash { get; set; }
    }
}

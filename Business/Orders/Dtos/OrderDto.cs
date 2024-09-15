using Business.Products.Dtos;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Orders.Dtos
{
    public class OrderDto
    {
        public DateTime OrderDate { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public List<UserProductDto> Products { get; set; }
    }
}

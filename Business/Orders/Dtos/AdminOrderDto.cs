using Business.Products.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Orders.Dtos
{
    public class AdminOrderDto
    {
        public DateTime OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public List<UserProductDto> Products { get; set; }
    }
}

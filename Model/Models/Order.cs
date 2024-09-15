using Model.Enums;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class Order: ISoftDelete
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public OrderStatusEnum OrderStatus { get; set; }
        public virtual List<OrderDetails> OrderDetails { get; set; }
        public bool IsDeleted { get; set; }
    }
}

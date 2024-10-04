using Model.Enums;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public decimal TotalPrice { get; set; }
        public decimal SubTotalPrice { get; set; }
        public OrderStatusEnum OrderStatus { get; set; }
        [ForeignKey("UserAddress")]
        public int AddressId { get; set; }
        public virtual UserAddress UserAddress { get; set; }
        public virtual List<OrderDetails> OrderDetails { get; set; }
        public bool IsDeleted { get; set; }
    }
}

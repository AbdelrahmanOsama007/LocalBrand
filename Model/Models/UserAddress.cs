using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class UserAddress
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public string? Appartment { get; set; }
        public string PhoneNumber {  get; set; }
        public string Email { get; set; }
        public string PaymentMethod { get; set; }
        public virtual Order Order { get; set; }
    }
}
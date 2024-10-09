using Business.Products.Dtos;
using Model.Enums;
using Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Orders.Dtos
{
    public class OrderDto
    {
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF]+$", ErrorMessage = "First name can only contain letters.")]
        public string FirstName { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF]+$", ErrorMessage = "Last name can only contain letters.")]
        public string LastName { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^(011|010|015|012)\d{8}$", ErrorMessage = "Phone number must start with 011, 010, 015, or 012 followed by 8 digits.")]
        public string PhoneNumber { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF]+$", ErrorMessage = "City name can only contain letters.")]
        public string City { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF]+$", ErrorMessage = "Street Address can only contain letters.")]
        public string StreetAddress { get; set; }
        public string? Appartment { get; set; }
        [Required]
        public PaymentMethodEnum PaymentMethod { get; set; }
        [Required]
        public List<UserProductDto> Products { get; set; }
    }
}

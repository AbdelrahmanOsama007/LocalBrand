﻿using Business.Products.Dtos;
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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public string? Appartment { get; set; }
        public string PaymentMethod { get; set; }
        public List<UserProductDto> Products { get; set; }
    }
}

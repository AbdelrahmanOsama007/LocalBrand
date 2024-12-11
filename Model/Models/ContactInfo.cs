using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class ContactInfo
    {
        public int Id { get; set; }
        public ReceviedEmailEnum EmailStatus { get; set; }
    }
}

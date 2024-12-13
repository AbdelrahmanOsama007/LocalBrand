using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class EmailPagination
    {
        public string SearchParam { get; set; }
        public int EmailStatus { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public EmailPagination()
        {
            SearchParam = string.Empty;
            EmailStatus = 0;
            PageNumber = 1;
            PageSize = 10;
        }
    }
}

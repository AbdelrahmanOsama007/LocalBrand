using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class ColorPagination
    {
        public string SearchParam { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public ColorPagination()
        {
            SearchParam = string.Empty;
            PageNumber = 1;
            PageSize = 10;
        }
    }
}

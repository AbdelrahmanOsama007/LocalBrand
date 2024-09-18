using Business.SubCategories.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Categories.Dtos
{
    public class NewCategoryDto
    {
        public string CategoryName { get; set; }
        public List<NewSubCategoryDto> SubCategories { get; set; }
    }
}

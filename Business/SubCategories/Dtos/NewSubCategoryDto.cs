using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.SubCategories.Dtos
{
    public class NewSubCategoryDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(10)]
        public string SubCategoryName { get; set; }
    }
}

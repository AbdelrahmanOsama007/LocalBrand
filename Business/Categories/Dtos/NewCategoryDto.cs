using Business.SubCategories.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Categories.Dtos
{
    public class NewCategoryDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(10)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "SubCategoryName can only contain letters.")]
        public string CategoryName { get; set; }
        [Required]
        public List<NewSubCategoryDto> SubCategories { get; set; }
    }
}

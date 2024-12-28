using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Colors.Dtos
{
    public class ColorAdminDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "ColorName can only contain letters.")]
        public string ColorName { get; set; }
        [Required]
        public string ColorCode { get; set; }
    }
}

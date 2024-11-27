using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Images.Dtos
{
    public class ColorImagesDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ColorId must be a positive number.")]
        public int ColorId { get; set; }
        [Required(ErrorMessage = "Color code is required.")]
        //[StringLength(7, MinimumLength = 7, ErrorMessage = "Color code must be exactly 7 characters.")]
            //[RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "Color code must be a valid hex code (e.g., #FFFFFF).")]
        public string ColorCode { get; set; }
        [Required]
        public List<string> ImageUrls { get; set; }
    }
}

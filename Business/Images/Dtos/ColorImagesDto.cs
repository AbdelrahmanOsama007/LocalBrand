using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Images.Dtos
{
    public class ColorImagesDto
    {
        public int ColorId { get; set; }
        public List<string> ImageUrls { get; set; }
    }
}

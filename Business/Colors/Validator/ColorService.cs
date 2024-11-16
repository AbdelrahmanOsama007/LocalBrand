using Business.Colors.Dtos;
using Business.Colors.Interfaces;
using Business.Wishlist.Dtos;
using Infrastructure.IGenericRepository;
using Model.Models;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Colors.Validator
{
    public class ColorService : IColorService
    {
        private readonly IGenericRepository<Color> _colorrepository;
        public ColorService(IGenericRepository<Color> colorrepository)
        {
            _colorrepository = colorrepository;
        }
        public async Task<OperationResult> GetAllColors()
        {
            try
            {
                var result = await _colorrepository.GetAllAsync();
                if (!result.Success)
                {
                    return result;
                }

                var colors = (List<Color>)result.Data;
                var colorsdtos = new List<ColorDto>();

                foreach (var color in colors)
                {
                    var colordto = new ColorDto()
                    {
                        Id = color.Id,
                        ColorCode = color.ColorCode,
                        ColorName = color.ColorName,
                    };
                    colorsdtos.Add(colordto);
                }
                return new OperationResult() { Success = true, Data = colorsdtos, Message = result.Message };
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
    }
}

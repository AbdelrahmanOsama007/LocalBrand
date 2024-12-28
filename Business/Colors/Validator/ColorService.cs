using Business.Colors.Dtos;
using Business.Colors.Interfaces;
using Business.Wishlist.Dtos;
using Infrastructure.IGenericRepository;
using Infrastructure.IRepository;
using Infrastructure.Repository;
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
        private readonly IColorRepository _colorrepository2;
        public ColorService(IGenericRepository<Color> colorrepository, IColorRepository colorRepository)
        {
            _colorrepository = colorrepository;
            _colorrepository2 = colorRepository;
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
        public async Task<GridOperationResult> GetColorGrid(ColorPagination colormodel)
        {
            try
            {
                var result = await _colorrepository2.GetColorGrid(colormodel);
                if (!result.Success)
                {
                    return result;
                }

                var colors = (List<Color>)result.Data.GridList;
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
                return new GridOperationResult() { Success = true, Message = result.Message, Data = new GridData() { GridList = colorsdtos, TotalRecords = result.Data.TotalRecords, PageNumber = result.Data.PageNumber, PageSize = result.Data.PageSize, TotalPages = result.Data.TotalPages }};
            }
            catch (Exception ex)
            {
                return new GridOperationResult() { Success = false, Message = "Something went wrong. Please try again later.", DevelopMessage = ex.Message };
            }
        }
        public async Task<OperationResult> AddColor(ColorDto color)
        {
            try
            {
                var colorobject = new Color()
                {
                    ColorName = color.ColorName,
                    ColorCode = color.ColorCode
                };
                return await _colorrepository.AddAsync(colorobject);
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
        public async Task<OperationResult> GetColorById(int id)
        {
            try
            {
                var result = await _colorrepository.GetByIdAsync(id);
                if (result.Success)
                {
                    var colorobject = (Color)result.Data;
                    var colordto = new ColorDto()
                    {
                        Id = colorobject.Id,
                        ColorCode = colorobject.ColorCode,
                        ColorName = colorobject.ColorName
                    };
                    return new OperationResult() { Success = true, Data = colordto, Message = result.Message };
                }
                return result;
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
        public async Task<OperationResult> UpdateColor(ColorDto color)
        {
            try
            {
                var result = await _colorrepository.GetByIdAsync(color.Id);
                if (result.Success)
                {
                    var colorobject = (Color)result.Data;
                    colorobject.ColorName = color.ColorName;
                    colorobject.ColorCode = color.ColorCode;
                    var updateresult = await _colorrepository.UpdateAsync(colorobject);
                    return updateresult;
                }
                return result;
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
        public async Task<OperationResult> DeleteColor(int id)
        {
            return await _colorrepository.DeleteAsync(id);
        }
    }
}

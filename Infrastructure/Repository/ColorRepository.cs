using Infrastructure.Context;
using Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class ColorRepository:IColorRepository
    {
        private readonly MyAppContext _context;
        public ColorRepository(MyAppContext context)
        {
            _context = context;
        }
        public async Task<GridOperationResult> GetColorGrid(ColorPagination colormodel)
        {
            try
            {
                var query = _context.Colors.AsQueryable();
                if (!string.IsNullOrWhiteSpace(colormodel.SearchParam))
                {
                    query = query.Where(e => e.ColorName.Contains(colormodel.SearchParam) || e.ColorCode.Contains(colormodel.SearchParam));
                }
                var totalRecords = await query.CountAsync();
                var colors = await query.Skip((colormodel.PageNumber - 1) * colormodel.PageSize).Take(colormodel.PageSize).ToListAsync();
                if (colors.Any())
                {
                    return new GridOperationResult
                    {
                        Success = true,
                        Message = "Data retrieved successfully.",
                        Data = new GridData { GridList = colors, TotalRecords = totalRecords, PageNumber = colormodel.PageNumber, PageSize = colormodel.PageSize, TotalPages = (int)Math.Ceiling((double)totalRecords / colormodel.PageSize) }
                    };
                }
                else
                {
                    return new GridOperationResult
                    {
                        Success = true,
                        Message = "No data found!",
                        Data = new GridData { GridList = new List<Color>(), TotalRecords = 0, PageNumber = colormodel.PageNumber, PageSize = colormodel.PageSize, TotalPages = 0 }
                    };
                }
            }
            catch (Exception ex)
            {
                return new GridOperationResult
                {
                    Success = false,
                    Message = "Something went wrong. Please try again later.",
                    DevelopMessage = ex.Message
                };
            }
        }
    }
}

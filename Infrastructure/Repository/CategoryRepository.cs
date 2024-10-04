using Infrastructure.Context;
using Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MyAppContext _context;
        public CategoryRepository(MyAppContext context)
        {
            _context = context;
        }
        public async Task<OperationResult> GetSubCatByCatId(int catId)
        {
            try
            {
                var result = await _context.SubCategories.Where(s => s.CategoryId == catId).ToListAsync();
                return new OperationResult() { Success = true, Message = "Data retrieved successfully", Data = result };
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
    }
}

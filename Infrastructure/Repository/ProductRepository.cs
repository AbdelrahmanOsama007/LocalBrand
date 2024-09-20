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
    public class ProductRepository : IProductRepository
    {
        private readonly MyAppContext _context;
        public ProductRepository(MyAppContext context)
        {
            _context = context;
        }
        public async Task<OperationResult> GetProductsBySubCaregory(int id)
        {
            try
            {
                var productlist = await _context.Products.Where(p => p.SubCategoryId == id).ToListAsync();
                return new OperationResult() { Success = true, Message = "Products retrieved successfully", Data = productlist };
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
        public async Task<OperationResult> GetBestSellers()
        {
            try
            {
                var productlist = await _context.Products.Where(p => p.BestSeller == true).ToListAsync();
                return new OperationResult() { Success = true, Message = "Products retrieved successfully", Data = productlist };
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
    }
}

using Business.Categories.Dtos;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Categories.Interfaces
{
    public interface ICategoryService
    {
        Task<OperationResult> AddCategoryAsync(NewCategoryDto category);
        Task<OperationResult> DeleteCategoryAsync(int id);
        Task<OperationResult> UpdateCategoryAsync(int id, NewCategoryDto updatedcategory);
        Task<OperationResult> GetAllCategoriesAsync();
        Task<OperationResult> GetSubCatsByCatId(int catId);
    }
}
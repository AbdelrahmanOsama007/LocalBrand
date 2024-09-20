using Business.Categories.Dtos;
using Business.Categories.Interfaces;
using Business.SubCategories.Dtos;
using Infrastructure.IGenericRepository;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Categories.Validator
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _categoryrepository;
        private readonly IGenericRepository<SubCategory> _subcategoryrepository;
        public CategoryService(IGenericRepository<Category> categoryRepository, IGenericRepository<SubCategory> subcategoryrepository)
        {
            _categoryrepository = categoryRepository;
            _subcategoryrepository = subcategoryrepository;
        }
        public async Task<OperationResult> AddCategoryAsync(NewCategoryDto category)
        {
            try
            {
                var newcategory = new Category()
                {
                    Name = category.CategoryName,
                    SubCategories = new List<SubCategory>()
                };

                foreach (var subcategory in category.SubCategories)
                {
                    var newsubcategory = new SubCategory()
                    {
                        Name = subcategory.SubCategoryName,
                    };
                    newcategory.SubCategories.Add(newsubcategory);
                }
                return await _categoryrepository.AddAsync(newcategory);
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
        public async Task<OperationResult> DeleteCategoryAsync(int id)
        {
            return await _categoryrepository.DeleteAsync(id);
        }
        public async Task<OperationResult> GetAllCategoriesAsync()
        {
            try
            {
                var result = await _categoryrepository.GetAllAsync();
                if (result.Success)
                {
                    var AllCategories = (List<Category>)result.Data;
                    var CategoriesList = new List<CategoryDto>();
                    foreach (var category in AllCategories)
                    {
                        var categoryDto = new CategoryDto()
                        {
                            CategoryId = category.Id,
                            CategoryName = category.Name,
                            SubCategories = new List<SubCategoryDto>()
                        };
                        foreach (var subcategory in category.SubCategories)
                        {
                            var subcategorydto = new SubCategoryDto()
                            {
                                SubCategoryId = subcategory.Id,
                                CategoryId = category.Id,
                                SubCategoryName = subcategory.Name,
                            };
                            categoryDto.SubCategories.Add(subcategorydto);
                        }
                        CategoriesList.Add(categoryDto);
                    }
                    return new OperationResult() { Success = true, Message = result.Message, Data = CategoriesList };
                }
                return result;
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
        public async Task<OperationResult> UpdateCategoryAsync(int id, NewCategoryDto updatedcategory)
        {
            using var transaction = await _categoryrepository.BeginTransactionAsync();
            try
            {
                var existingCategoryResult = await _categoryrepository.GetByIdAsync(id);
                if (!existingCategoryResult.Success)
                {
                    return existingCategoryResult;
                }

                var existingCategory = (Category)existingCategoryResult.Data;
                existingCategory.Name = updatedcategory.CategoryName;

                await _subcategoryrepository.DeleteRangeAsync(existingCategory.SubCategories);

                foreach (var subcategory in updatedcategory.SubCategories)
                {
                    var newsubcategory = new SubCategory()
                    {
                        Name = subcategory.SubCategoryName,
                        CategoryId = id,
                    };
                    existingCategory.SubCategories.Add(newsubcategory);
                }

                var updateresult = await _categoryrepository.UpdateAsync(existingCategory);
                await transaction.CommitAsync();
                return updateresult;
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
    }
}
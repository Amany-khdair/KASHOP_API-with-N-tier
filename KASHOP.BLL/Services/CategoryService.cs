using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }  
        public async Task<Result<List<CategoryResponse>>> GetAllCategories()
        {
           
            var categories = await _categoryRepository.GetAllAsync(
            new string[] {nameof(Category.Translations), "CreatedBy"}
            );
            return Result<List<CategoryResponse>>.Ok(categories.Adapt<List<CategoryResponse>>(), "Categories retrieved successfully");
            //return new Result<List<CategoryResponse>>
            //{
            //    Success = true,
            //    Message = "Categories retrieved successfully.",
            //    Data = categories.Adapt<List<CategoryResponse>>()
            //};                                 
        }
        
        public async Task<Result<CategoryResponse>> GetCategory(Expression<Func<Category, bool>> filter)
        {
            
            var category = await _categoryRepository.GetOne(filter, new string[] {nameof(Category.Translations), "CreatedBy" });
            
            if (category is null)
            {
                return Result<CategoryResponse>.Fail("Category not found");
                //return new Result<CategoryResponse>
                //{
                //    Success = false,
                //    Message = "Category not found."
                //};
            }
            return Result<CategoryResponse>.Ok(category.Adapt<CategoryResponse>(), "Category retrieved successfully");
            //return new Result<CategoryResponse>
            //{
            //    Success = true,
            //    Message = "Category retrieved successfully.",
            //    Data = category.Adapt<CategoryResponse>()
            //};
                        
        }

        public async Task<Result<CategoryResponse>> CreateCategory(CategoryRequest request)
        {            
            var category = request.Adapt<Category>();
            await _categoryRepository.CreateAsync(category);
            return Result<CategoryResponse>.Ok(category.Adapt<CategoryResponse>(), "Category created successfully.");
            //return new Result<CategoryResponse>
            //{
            //    Success = true,
            //    Message = "Category created successfully.",
            //};            
            
        }
        public async Task<Result<bool>> DeleteCategory(int id)
        {
        
            var category = await _categoryRepository.GetOne(c => c.Id == id);
            if (category == null) 
                return Result<bool>.Fail("Category not found.");
            //return new Result<bool>
            //    {
            //        Success = false,
            //        Message = "Category not found.",
            //        Data = false
            //    };
            var deleted = await _categoryRepository.DeleteAsync(category);
            return Result<bool>.Ok(deleted, deleted ? "Category deleted successfully." : "Failed to delete category.");
            //return new Result<bool>
            //{
            //    Success = deleted,
            //    Message = deleted ? "Category deleted successfully." : "Failed to delete category.",
            //    Data = deleted
            //};           
            
        }

        public async Task<Result<CategoryResponse>> UpdateCategory(int id, CategoryRequest request)
        {
            
            var category = await _categoryRepository.GetOne(c => c.Id == id, new string[] { nameof(Category.Translations), "CreatedBy" });
            if (category == null)
                return Result<CategoryResponse>.Fail("Category not found");
            //return new Result<CategoryResponse>
            //    {
            //        Success = false,
            //        Message = "Category not found."
            //    };
            category = request.Adapt(category);
            var result = await _categoryRepository.UpdateAsync(category);
            if (result == null)
                return Result<CategoryResponse>.Fail("Failed to update category.");
            //return new Result<CategoryResponse>
            //    {
            //        Success = false,
            //        Message = "Failed to update category."
            //    };
            return Result<CategoryResponse>.Ok(result.Adapt<CategoryResponse>(), "Category updated successfully.");
            //return new Result<CategoryResponse>
            //{
            //    Success = true,
            //    Message = "Category updated successfully.",
            //    Data = result.Adapt<CategoryResponse>()
            //};
            
        }
    }
}

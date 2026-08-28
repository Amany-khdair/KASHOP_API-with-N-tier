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
            try
            {
                var categories = await _categoryRepository.GetAllAsync(
                new string[] {nameof(Category.Translations), "CreatedBy"}
                );
                return new Result<List<CategoryResponse>>
                {
                    Success = true,
                    Message = "Categories retrieved successfully.",
                    Data = categories.Adapt<List<CategoryResponse>>()
                };
            }
            catch (Exception ex)
            {
                return new Result<List<CategoryResponse>>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving categories: {ex.InnerException.Message}",
                };
            }                      
        }
        
        public async Task<Result<CategoryResponse>> GetCategory(Expression<Func<Category, bool>> filter)
        {
            try
            {
                var category = await _categoryRepository.GetOne(filter, new string[] {nameof(Category.Translations), "CreatedBy" });
            
                if (category is null)
                {
                    return new Result<CategoryResponse>
                    {
                        Success = false,
                        Message = "Category not found."
                    };
                }
                return new Result<CategoryResponse>
                {
                    Success = true,
                    Message = "Category retrieved successfully.",
                    Data = category.Adapt<CategoryResponse>()
                };
            }
            catch(Exception ex)
            {
                return new Result<CategoryResponse>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving category: {ex.InnerException.Message}",
                };
            }
            
        }

        public async Task<Result<CategoryResponse>> CreateCategory(CategoryRequest request)
        {
            try
            {
                var category = request.Adapt<Category>();
                await _categoryRepository.CreateAsync(category);
                return new Result<CategoryResponse>
                {
                    Success = true,
                    Message = "Category created successfully.",
                };
            }
            catch(Exception ex)
            {
                return new Result<CategoryResponse>
                {
                    Success = false,
                    Message = $"An error occurred while creating category: {ex.InnerException.Message}",
                };                
            }
            
        }
        public async Task<Result<bool>> DeleteCategory(int id)
        {
            try
            {
                var category = await _categoryRepository.GetOne(c => c.Id == id);
                if (category == null) 
                    return new Result<bool>
                    {
                        Success = false,
                        Message = "Category not found.",
                        Data = false
                    };
                var deleted = await _categoryRepository.DeleteAsync(category);
                return new Result<bool>
                {
                    Success = deleted,
                    Message = deleted ? "Category deleted successfully." : "Failed to delete category.",
                    Data = deleted
                };
            }
            catch(Exception ex)
            {
                return new Result<bool>
                {
                    Success = false,
                    Message = $"An error occurred while deleting category: {ex.InnerException.Message}",
                    Data = false
                };                
            }
            
        }

        public async Task<Result<CategoryResponse>> UpdateCategory(int id, CategoryRequest request)
        {
            try
            {
                var category = await _categoryRepository.GetOne(c => c.Id == id, new string[] { nameof(Category.Translations), "CreatedBy" });
                if (category == null)
                    return new Result<CategoryResponse>
                    {
                        Success = false,
                        Message = "Category not found."
                    };
                category = request.Adapt(category);
                var result = await _categoryRepository.UpdateAsync(category);
                if (result == null)
                    return new Result<CategoryResponse>
                    {
                        Success = false,
                        Message = "Failed to update category."
                    };
                return new Result<CategoryResponse>
                {
                    Success = true,
                    Message = "Category updated successfully.",
                    Data = result.Adapt<CategoryResponse>()
                };
            }
            catch (Exception ex)
            {
                return new Result<CategoryResponse>
                {
                    Success = false,
                    Message = $"An error occurred while updating category: {ex.InnerException.Message}",
                };

            }
        }
    }
}

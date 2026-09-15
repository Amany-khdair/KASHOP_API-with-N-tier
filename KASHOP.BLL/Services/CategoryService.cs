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
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }  
        public async Task<Result<List<CategoryResponse>>> GetAllCategories()
        {
           
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync(
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
            
            var category = await _unitOfWork.CategoryRepository.GetOne(filter, new string[] {nameof(Category.Translations), "CreatedBy" });
            
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
            await _unitOfWork.CategoryRepository.CreateAsync(category);
            await _unitOfWork.CompleteAsync();
            return Result<CategoryResponse>.Ok(category.Adapt<CategoryResponse>(), "Category created successfully.");
            //return new Result<CategoryResponse>
            //{
            //    Success = true,
            //    Message = "Category created successfully.",
            //};            
            
        }
        public async Task<Result<bool>> DeleteCategory(int id)
        {
        
            var category = await _unitOfWork.CategoryRepository.GetOne(c => c.Id == id);
            if (category == null) 
                return Result<bool>.Fail("Category not found.");
            //return new Result<bool>
            //    {
            //        Success = false,
            //        Message = "Category not found.",
            //        Data = false
            //    };
            _unitOfWork.CategoryRepository.Delete(category);
            var affectedRows = await _unitOfWork.CompleteAsync();
            return new Result<bool>
            {
                Success = affectedRows > 0,
                Message = affectedRows > 0 ? "Category deleted successfully." : "Failed to delete category.",            
            };

        }

        public async Task<Result<CategoryResponse>> UpdateCategory(int id, CategoryRequest request)
        {
            
            var category = await _unitOfWork.CategoryRepository.GetOne(c => c.Id == id, new string[] { nameof(Category.Translations), "CreatedBy" });
            if (category == null)
                return Result<CategoryResponse>.Fail("Category not found");
            //return new Result<CategoryResponse>
            //    {
            //        Success = false,
            //        Message = "Category not found."
            //    };
            category = request.Adapt(category);
            _unitOfWork.CategoryRepository.Update(category);
            var affectedRows = await _unitOfWork.CompleteAsync();
            
            if (affectedRows <= 0)
                return Result<CategoryResponse>.Fail("Failed to update category.");
            //return new Result<CategoryResponse>
            //    {
            //        Success = false,
            //        Message = "Failed to update category."
            //    };
            return Result<CategoryResponse>.Ok(category.Adapt<CategoryResponse>(), "Category updated successfully.");
            //return new Result<CategoryResponse>
            //{
            //    Success = true,
            //    Message = "Category updated successfully.",
            //    Data = result.Adapt<CategoryResponse>()
            //};
            
        }
    }
}

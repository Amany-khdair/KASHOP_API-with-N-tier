using KASHOP.BLL.Extensions;
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
    public class ProductService : IProductService
    {
        private readonly IFileService _fileService;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }
        public async Task<Result<ProductResponse>> CreateProduct(ProductRequest request)
        {            
            if (request.MainImage is null)
            {
                return Result<ProductResponse>.Fail("Main image is required");                      
            }

            var uploadResult = await _fileService.UploadAsync(request.MainImage);
            if (!uploadResult.Success)
            {
                return Result<ProductResponse>.Fail(uploadResult.Message);                   
            }

            var product = request.Adapt<Product>();
            product.MainImage = uploadResult.Data.Url;
            product.MainImagePublicId = uploadResult.Data.PublicId;
            await _unitOfWork.ProductRepository.CreateAsync(product);
            await _unitOfWork.CompleteAsync();
            return Result<ProductResponse>.Ok(product.Adapt<ProductResponse>(), "Product created successfully");                
                                 
        }       

        public async Task<Result<PaginationResponse<ProductResponse>>> GetAllProducts(ProductQueryRequest request)
        {           
            var query = _unitOfWork.ProductRepository.GetQueryable(
                includes: new string[] { nameof(Product.Translations), nameof(Product.Category) });

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(p => p.Translations.Any(t => t.Name.Contains(request.Search)));
            }
            
            if (request.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == request.CategoryId);
            }

            if (request.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= request.MinPrice);
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= request.MaxPrice);
            }

            var sortBy = request.sortBy?.ToLower();
            var language = CultureInfo.CurrentCulture.Name;

            if(sortBy == "price")
            {
                query = request.Descending ? query.OrderByDescending(p=>p.Price)
                    : query.OrderBy(p => p.Price);
            }else if(sortBy == "rate")
            {
                query = request.Descending ? query.OrderByDescending(p=>p.Rate)
                    : query.OrderBy(p => p.Rate);            
            } else if(sortBy == "name")
            {
                query = request.Descending ? query.OrderByDescending(p=>p.Translations.Where(t=>t.Language == language).Select(t=>t.Name).FirstOrDefault())
                    : query.OrderBy(p => p.Translations.Where(t=>t.Language == language).Select(t=>t.Name).FirstOrDefault());
            }
            else
            {
                query = request.Descending ? query.OrderByDescending(p => p.Id)
                    : query.OrderBy(p => p.Id);
            }

            var result = await query.ToPaginationAsync(request.Page, request.Limit);
            return Result<PaginationResponse<ProductResponse>>.Ok(result.Adapt<PaginationResponse<ProductResponse>>(), "Products retrieved successfully");                           
        }

        public async Task<Result<ProductResponse>> GetProduct(Expression<Func<Product, bool>> filter)
        {            
            var product = await _unitOfWork.ProductRepository.GetOne(filter, 
                new string[] { nameof(Product.Translations), nameof(Product.Category) });
            if (product is null)
            {
                return Result<ProductResponse>.Fail("Product not found");                    
            }

            return Result<ProductResponse>.Ok(product.Adapt<ProductResponse>(), "Product retrieved successfully");                          
        }

        public async Task<Result<bool>> DeleteProduct(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetOne(p => p.Id == id);
            if (product is null)
            {
                return Result<bool>.Fail("Product not found");
            }

            var deletedImageResult = await _fileService.Delete(product.MainImagePublicId);
            if(!deletedImageResult.Success)
            {
                return Result<bool>.Fail($"Failed to delete product image: {deletedImageResult.Message}");
            }

            _unitOfWork.ProductRepository.Delete(product);
            var affectedRows = await _unitOfWork.CompleteAsync();
            return affectedRows > 0
                ? Result<bool>.Ok(true, "Product deleted successfully")
                : Result<bool>.Fail("Failed to delete product");
        }
    }
}

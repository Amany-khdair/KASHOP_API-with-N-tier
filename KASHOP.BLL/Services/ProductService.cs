using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileService _fileService;

        public ProductService(IProductRepository productRepository, IFileService fileService)
        {
            _productRepository = productRepository;
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
            product.MainImage = uploadResult.Data;
            await _productRepository.CreateAsync(product);

            return Result<ProductResponse>.Ok(product.Adapt<ProductResponse>(), "Product created successfully");                
                                 
        }

        public async Task<Result<List<ProductResponse>>> GetAllProducts()
        {           
            var products = await _productRepository.GetAllAsync(
                new string[] { nameof(Product.Translations), nameof(Product.Category) });

            return Result<List<ProductResponse>>.Ok(products.Adapt<List<ProductResponse>>(), "Products retrieved successfully");                           
        }

        public async Task<Result<ProductResponse>> GetProduct(Expression<Func<Product, bool>> filter)
        {            
            var product = await _productRepository.GetOne(filter, 
                new string[] { nameof(Product.Translations), nameof(Product.Category) });
            if (product is null)
            {
                return Result<ProductResponse>.Fail("Product not found");                    
            }

            return Result<ProductResponse>.Ok(product.Adapt<ProductResponse>(), "Product retrieved successfully");                          
        }
    }
}

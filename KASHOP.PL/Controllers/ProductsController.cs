using KASHOP.BLL.Services;
using KASHOP.DAL.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProduct([FromForm] ProductRequest request)
        {
            var result = await _productService.CreateProduct(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

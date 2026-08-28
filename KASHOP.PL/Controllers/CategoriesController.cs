using KASHOP.BLL.Services;
using KASHOP.DAL.Data;
using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using KASHOP.PL.Resources;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {        
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ICategoryService _categoryService;

        public CategoriesController(ApplicationDbContext context, IStringLocalizer<SharedResource> localizer, ICategoryService categoryService)
        {
            
            _localizer = localizer;
            _categoryService = categoryService;
        }

        [HttpGet("")]
        public async Task< IActionResult> Index()
        {
            //var lang = Request.Headers["Accept-Language"].ToString();
            var result = await _categoryService.GetAllCategories();
            return result.Success ?Ok(result) : BadRequest(result);
            //return Ok(new { _localizer["success"].Value, categories });
        }

        [HttpPost("")]
        public async Task<IActionResult> Create(CategoryRequest request)
        {
            //var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _categoryService.CreateCategory(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoryService.GetCategory(c => c.Id == id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteCategory(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CategoryRequest request)
        {
            var result = await _categoryService.UpdateCategory(id, request);
            return result.Success ? Ok(result) : BadRequest(result);           
        }
    }
}


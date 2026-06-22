using KASHOP.DAL.Data;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private ApplicationDbContext _context;
        private IStringLocalizer<SharedResource> _localizer;

        public CategoriesController(ApplicationDbContext context, IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            //try
            //{
            //    if (_context.Database.CanConnect())
            //    {
            //        return Ok("Done");
            //    }
            //    else
            //    {
            //        return StatusCode(500, "Cannot connect to the database");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return StatusCode(500, $"An error occurred: {ex.Message}");
            //}
            return Ok(_localizer["Success"].Value);
        }
    }
}


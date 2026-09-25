using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        //to get the current user id from the token
        protected string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}

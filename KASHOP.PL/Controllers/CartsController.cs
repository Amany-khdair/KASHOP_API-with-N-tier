using KASHOP.BLL.Services;
using KASHOP.DAL.Dto;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : BaseApiController
    {
        private readonly ICartService _cartService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public CartsController(ICartService cartService, IStringLocalizer<SharedResource> localizer)
        {
            _cartService = cartService;
            _localizer = localizer;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(CartItemRequest request) {
            var result = await _cartService.AddToCart(CurrentUserId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetCart()
        {
            var result = await _cartService.GetCart(CurrentUserId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem([FromRoute] int id)
        {
            var result = await _cartService.RemoveFromCart(CurrentUserId, id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuantity([FromRoute] int id, [FromBody] int count)
        {
            var result = await _cartService.UpdateQuantity(CurrentUserId, id, count);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var result = await _cartService.ClearCart(CurrentUserId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

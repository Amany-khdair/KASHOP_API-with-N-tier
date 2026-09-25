using KASHOP.DAL.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public interface ICartService
    {
        Task<Result<bool>> AddToCart(string userId, CartItemRequest request);
        Task<Result<List<CartItemResponse>>> GetCart(string userId);
        Task<Result<bool>> UpdateQuantity(string userId, int productId, int count);
        Task<Result<bool>> RemoveFromCart(string userId, int productId);        
        Task<Result<bool>> ClearCart(string userId);
    }
}

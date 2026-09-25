using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> AddToCart(string userId, CartItemRequest request)
        {
            var product =await _unitOfWork.ProductRepository.GetOne(p => p.Id == request.ProductId);
            if (product is null)            
                return Result<bool>.Fail("Product not found");
            
            if(request.Count <= 0)
                return Result<bool>.Fail("Count must be greater than 0");
            
            if(product.Quantity < request.Count)
                return Result<bool>.Fail("Not enough quantity in stock");

            var existingItem = await _unitOfWork.CartRepository.GetOne(
                 c => c.ProductId == request.ProductId && c.UserId == userId);

            if(existingItem is not null)
            {
                existingItem.Count += request.Count;
            }
            else
            {
                var newItem = new CartItem
                {
                    UserId = userId,
                    ProductId = request.ProductId,
                    Count = request.Count
                };
                await _unitOfWork.CartRepository.CreateAsync(newItem);
            }
            await _unitOfWork.CompleteAsync();
            return Result<bool>.Ok(true);
        }

        public async Task<Result<List<CartItemResponse>>> GetCart(string userId)
        {
            var userCartItems = await _unitOfWork.CartRepository.GetAllAsync(
                filter:c => c.UserId == userId, 
                includes: new string[] { nameof(CartItem.Product),$"{nameof(CartItem.Product)}.{nameof(Product.Translations)}" });
            
            var response = userCartItems.Adapt<List<CartItemResponse>>();
            return Result<List<CartItemResponse>>.Ok(response);
        }

        public async Task<Result<bool>> RemoveFromCart(string userId, int productId)
        {
            var cartItem = await _unitOfWork.CartRepository.GetOne(
                c => c.UserId == userId && c.ProductId == productId);
            if (cartItem is null)
                return Result<bool>.Fail("Item not found in cart");

            _unitOfWork.CartRepository.Delete(cartItem);
            var affectedRows = await _unitOfWork.CompleteAsync();
            return affectedRows > 0
                ? Result<bool>.Ok(true, "Item removed from cart successfully")
                : Result<bool>.Fail("Failed to remove item from cart");
        }

        public async Task<Result<bool>> UpdateQuantity(string userId, int productId, int count)
        {
            if (count < 0)
                return Result<bool>.Fail("Count must be greater than 0");

            var cartItem = await _unitOfWork.CartRepository.GetOne(
                filter:c => c.UserId == userId && c.ProductId == productId,
                includes: new string[] { nameof(CartItem.Product) });

            if(cartItem.Product.Quantity < count)
                return Result<bool>.Fail("Not enough quantity in stock");

            cartItem.Count = count;
            var affectedRows = await _unitOfWork.CompleteAsync();

            return affectedRows > 0
                ? Result<bool>.Ok(true, "Cart item quantity updated successfully")
                : Result<bool>.Fail("Failed to update cart item quantity");
        }

        public async Task<Result<bool>> ClearCart(string userId)
        {
            var userCartItems = await _unitOfWork.CartRepository.GetAllAsync(
               filter: c => c.UserId == userId);

            if (!userCartItems.Any())
                return Result<bool>.Fail("Cart is already empty");

            foreach (var item in userCartItems)
            {
                _unitOfWork.CartRepository.Delete(item);
            }

            var affectedRows = await _unitOfWork.CompleteAsync();

            return affectedRows > 0
                ? Result<bool>.Ok(true, "Cart cleared successfully")
                : Result<bool>.Fail("Failed to clear cart");
        }

    }
}

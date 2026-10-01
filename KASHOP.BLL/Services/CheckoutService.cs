using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public CheckoutService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<Result<OrderResponse>> CheckOut(string userId, CheckoutRequest request)
        {
            var cartItems = await _unitOfWork.CartRepository.GetAllAsync(
                filter: c => c.UserId == userId,
                includes: new string[] { nameof(CartItem.Product), $"{nameof(CartItem.Product)}.{nameof(Product.Translations)}" }
                );
           
            if(!cartItems.Any())
            {
                return Result<OrderResponse>.Fail("Cart is empty");
            }

            foreach(var item in cartItems)
            {
                if(item.Product.Quantity < item.Count)
                {
                    return Result<OrderResponse>.Fail($"Product {item.ProductId} is out of stock");
                }
            }
            var user = await _userManager.FindByIdAsync(userId);
            if(user is null)
            {
                return Result<OrderResponse>.Fail("User not found");
            }

            var city = request.City ?? user.City;
            var street = request.Street ?? user.Street;
            var phoneNumber = request.PhoneNumber ?? user.PhoneNumber;

            if(string.IsNullOrEmpty(city) || string.IsNullOrEmpty(street) || string.IsNullOrEmpty(phoneNumber))            
                return Result<OrderResponse>.Fail("Shipping info is required");

            var order = new Order
            {
                UserId = userId,
                PaymentMethod = request.PaymentMethod,
                Status = DAL.Enums.OrderStatusEnum.Pending,
                PaymentStatus = DAL.Enums.PaymentStatusEnum.Pending,
                City = city,
                Street = street,
                PhoneNumber = phoneNumber,
                OrderItems = new List<OrderItem>()
            };
            decimal totalAmount = 0;
            foreach (var item in cartItems)
            {
                var OrderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Count = item.Count,
                    PriceAtPurchase = item.Product.Price
                };
                order.OrderItems.Add(OrderItem);
                totalAmount += OrderItem.PriceAtPurchase * OrderItem.Count;
            }
            order.TotalAmount = totalAmount;
            await _unitOfWork.OrderRepository.CreateAsync(order);
            foreach(var item in cartItems)
            {
                _unitOfWork.CartRepository.Delete(item);
            }
            await _unitOfWork.CompleteAsync();
            return Result<OrderResponse>.Ok(order.Adapt<OrderResponse>(), "Order placed successfully");
        }
    }
}

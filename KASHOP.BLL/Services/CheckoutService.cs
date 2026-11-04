using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Stripe.Checkout;

namespace KASHOP.BLL.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CheckoutService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
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
            

            return request.PaymentMethod == DAL.Enums.PaymentMethodEnum.Cash ? 
                await PlaceCashOrderAsync(order, cartItems) 
                :
                await PlaceVisaOrderAsync(order, cartItems);
        }
        public async Task<Result<OrderResponse>> HandleSuccess(string sessionId)
        {
            var order = await _unitOfWork.OrderRepository.GetOne(
                filter: o => o.StripeSessionId == sessionId,
                includes: new[]
                {
                    $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}",
                    $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}.{nameof(Product.Translations)}"
                });
            if(order.PaymentStatus == DAL.Enums.PaymentStatusEnum.Paid)
            {
                return Result<OrderResponse>.Fail("Order already paid");
            }
            var session = await new SessionService().GetAsync(sessionId);
            if(session.PaymentStatus != "paid")
            {
                return Result<OrderResponse>.Fail("Payment not completed");
            }
            order.PaymentStatus = DAL.Enums.PaymentStatusEnum.Paid;
            order.Status = DAL.Enums.OrderStatusEnum.Confirmed;
            order.AmountPaid = session.AmountTotal;
            order.PaidAt = DateTime.UtcNow;
            order.PaymentIntentId = session.PaymentIntentId;

            foreach (var item in order.OrderItems)
            {
                item.Product.Quantity -= item.Count;
            }

            var cartItems = await _unitOfWork.CartRepository.GetAllAsync(
                filter: c => c.UserId == order.UserId
                );

            foreach (var item in cartItems)
            {
                _unitOfWork.CartRepository.Delete(item);
            }

            await _unitOfWork.CompleteAsync();

            foreach (var item in order.OrderItems)
            {
                var product = item.Product;

                var translations = product?.Translations;

                var test = item.Adapt<OrderItemResponse>();
            }

            return Result<OrderResponse>.Ok(order.Adapt<OrderResponse>(), "Payment successful and order confirmed");


        }
        private async Task<Result<OrderResponse>> PlaceVisaOrderAsync(Order order, List<CartItem> cartItems)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            var options = new SessionCreateOptions
            {
                Mode = "payment",
                LineItems = cartItems.Select(item => new SessionLineItemOptions
                {
                    Quantity = item.Count,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "USD",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Translations.FirstOrDefault(t => t.Language == "en")?.Name,
                            Description = item.Product.Translations.FirstOrDefault(t => t.Language == "en")?.Description
                        },                    
                        UnitAmount = (long)item.Product.Price * 100, // Stripe expects the amount in cents
                    },
                }).ToList(),
                SuccessUrl = $"{baseUrl}/api/orders/payment/success?sessionId={{CHECKOUT_SESSION_ID}}",
                ExpiresAt = DateTime.UtcNow.AddHours(1)
                //CancelUrl = $"{baseUrl}/checkout/cancel",
            }; 
            var service = new SessionService();
            var session = service.Create(options);
            order.StripeSessionId = session.Id;
            await _unitOfWork.OrderRepository.CreateAsync(order);
            await _unitOfWork.CompleteAsync();

            var response = order.Adapt<OrderResponse>();
            response.PaymentUrl = session.Url;
            return Result<OrderResponse>.Ok(response, "Redirect to payment page");
        }

        private async Task<Result<OrderResponse>> PlaceCashOrderAsync(Order order, List<CartItem> cartItems)
        {
            foreach(var item in cartItems)
            {
                item.Product.Quantity -= item.Count;
                _unitOfWork.CartRepository.Delete(item);
            }
            await _unitOfWork.OrderRepository.CreateAsync(order);
            await _unitOfWork.CompleteAsync();
            return Result<OrderResponse>.Ok(order.Adapt<OrderResponse>(), "Order placed successfully");
        }

        
    }
}

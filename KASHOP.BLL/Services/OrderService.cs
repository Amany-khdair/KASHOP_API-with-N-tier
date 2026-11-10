using KASHOP.DAL.Dto;
using KASHOP.DAL.Enums;
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
    public class OrderService  : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<List<OrderSummaryResponse>>> GetUserOrdersAsync(string userId)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync(
                filter: o => o.UserId == userId,
                includes: new[] { nameof(Order.OrderItems)}
            );
            var response = orders.OrderByDescending(o => o.OrderDate).Adapt<List<OrderSummaryResponse>>();
            return Result<List<OrderSummaryResponse>>.Ok(response);
        }

        public async Task<Result<OrderDetailsResponse>> GetUserOrderDetailsAsync(int orderId, string userId)
        {
            var order = await _unitOfWork.OrderRepository.GetOne(
                filter: o => o.Id == orderId && o.UserId == userId
            );
            return Result<OrderDetailsResponse>.Ok(order.Adapt<OrderDetailsResponse>());
        } 
        
        public async Task<Result<List<OrderSummaryResponse>>> GetAllOrdersAsync(OrderStatusEnum? status)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync(
                filter: o => status == null || o.Status == status                
            );
            var response = orders.OrderByDescending(o => o.OrderDate).Adapt<List<OrderSummaryResponse>>();
            return Result<List<OrderSummaryResponse>>.Ok(response);
        }
    }
}

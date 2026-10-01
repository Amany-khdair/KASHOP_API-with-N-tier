using KASHOP.DAL.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public interface ICheckoutService
    {
        Task<Result<OrderResponse>> CheckOut(string userId, CheckoutRequest request);
    }
}

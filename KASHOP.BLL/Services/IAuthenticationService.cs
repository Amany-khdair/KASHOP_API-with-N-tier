using KASHOP.DAL.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public interface IAuthenticationService
    {
        Task<Result<bool>> RegisterAsync(RegisterRequest request);
        Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
        Task<Result<bool>> ConfirmEmail(ConfirmEmailRequest request);

    }
}

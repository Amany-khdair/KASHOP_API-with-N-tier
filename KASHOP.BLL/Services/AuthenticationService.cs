using KASHOP.BLL.Common;
using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }        

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request) 
        {
            var user = request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return new RegisterResponse()
                {
                    Message = "User registration failed"
                };

            var emailUrl = $"https://localhost:7214/api/Account/ConfirmEmail?email={request.Email}";
            await _emailSender.SendEmailAsync(
                request.Email,
                "Welcome to KASHOP",
                $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 10px;'>
        
                    <h2 style='color: #DB4444; text-align: center;'>
                        Welcome to KASHOP 🎉
                    </h2>

                    <p style='font-size: 16px; color: #555;'>
                        Thank you for registering with <strong>KASHOP</strong>.
                    </p>

                    <p style='font-size: 16px; color: #555;'>
                        Please confirm your email address by clicking the button below:
                    </p>

                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{emailUrl}'
                           style='background-color: #DB4444;
                                  color: white;
                                  text-decoration: none;
                                  padding: 12px 24px;
                                  border-radius: 6px;
                                  display: inline-block;
                                  font-weight: bold;'>
                            Confirm Email
                        </a>
                    </div>

                    <p style='font-size: 14px; color: #888;'>
                        If you didn't create this account, you can safely ignore this email.
                    </p>

                    <hr />

                    <p style='font-size: 12px; color: #999; text-align: center;'>
                        © 2026 KASHOP. All rights reserved.
                    </p>

                </div>"
            );

            return new RegisterResponse()
            {
                Message = "User registration successful"
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if(user is null)
            {
                return new LoginResponse()
                {
                    Message = "User not found"
                };
            }
            
            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if(!result)
            {
                return new LoginResponse()
                {
                    Message = "Invalid password"
                };
            }
            
            return new LoginResponse()
            {
                Message = "Login successful"
            };
        }
    }
}

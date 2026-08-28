using KASHOP.BLL.Common;
using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration config)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _config = config;
        }        

        public async Task<Result<bool>> RegisterAsync(RegisterRequest request) 
        {
            try
            {
                var user = request.Adapt<ApplicationUser>();
                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                    return new Result<bool>
                    {
                        Success = false,
                        Message = "User registration failed",
                        Data = false,
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //this will convert each character in the token to its ASCII value and then convert it to a string representation of that value, separated by dashes.
                token = Uri.EscapeDataString(token);
                var emailUrl = $"https://localhost:7214/api/Account/ConfirmEmail?token={token}&userId={user.Id}";
                await _emailSender.SendEmailAsync(
                    request.Email,
                    "Welcome to KASHOP",
                    $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 10px;'>
        
                        <h2 style='color: #DB4444; text-align: center;'>
                            Welcome to KASHOP 
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

                return new Result<bool>
                {
                    Success = true,
                    Message = "User registered successfully. Please check your email to confirm your account.",
                    Data = true,
                    Errors = null
                };
            }
            catch (Exception ex)
            {
                return new Result<bool>
                {
                    Success = false,
                    Message = $"An error occurred during registration: {ex.InnerException.Message}",
                    Data = false,                    
                };
            }
        }

        public async Task<Result<bool>> ConfirmEmail(ConfirmEmailRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user is null)
                {
                    return new Result<bool>
                    {
                        Success = false,
                        Message = "User not found",
                        Data = false
                    };
                }
                    
                // Decode the token from the query string(return to its original shape)
                request.Token = Uri.UnescapeDataString(request.Token);
                var result = await _userManager.ConfirmEmailAsync(user, request.Token);               
                return new Result<bool>
                {
                    Success = result.Succeeded,
                    Message = result.Succeeded ? "Email confirmed successfully" : "Email confirmation failed",
                    Data = result.Succeeded,
                };
            }
            catch(Exception ex)
            {
                return new Result<bool>
                {
                    Success = false,
                    Message = $"An error occurred during email confirmation: {ex.InnerException.Message}",
                    Data = false,
                };
            }
           
            
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user is null)
                {
                    return new Result<LoginResponse>
                    {
                        Success = false,
                        Message = "Invalid Email"
                    };
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    return new Result<LoginResponse>
                    {
                        Success = false,
                        Message = "Email not confirmed. Please check your email to confirm your account."
                    };
                }

                var PasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!PasswordValid)
                {
                    return new Result<LoginResponse>
                    {
                        Success = false,
                        Message = "Invalid Password"
                    };
                }

                return new Result<LoginResponse>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = new LoginResponse
                    {
                        AccessToken = await GenerateJwt(user)
                    }
                };
            }
            catch(Exception ex)
            {
                return new Result<LoginResponse>
                {
                    Success = false,
                    Message = ex.InnerException.Message
                };
            }          
            
        }
        private async Task<string> GenerateJwt(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, string.Join(",",roles))
            };
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["ApiSettings:SecretKey"]));
            
            var creds = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);
            
            var token = new JwtSecurityToken(
                issuer: _config["ApiSettings:issuer"],
                audience: _config["ApiSettings:audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(20),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }
}

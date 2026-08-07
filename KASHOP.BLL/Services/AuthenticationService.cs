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

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request) 
        {
            var user = request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(user, request.Password);

            foreach(var item in result.Errors)
            {
                Console.WriteLine(item.Description);
            }

            if (!result.Succeeded)
                return new RegisterResponse()
                {
                    Message = "User registration failed",
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

            return new RegisterResponse()
            {
                Message = "User registration successful"
            };
        }

        public async Task<bool> ConfirmEmail(ConfirmEmailRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null) return false;
            // Decode the token from the query string(return to its original shape)
            request.Token = Uri.UnescapeDataString(request.Token);
            var result = await _userManager.ConfirmEmailAsync(user, request.Token);
            if (!result.Succeeded) return false;
            return true;
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

            if(!await _userManager.IsEmailConfirmedAsync(user))
            {
                return new LoginResponse()
                {
                    Message = "Email not confirmed"
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
                Message = "Login successful",
                AccessToken = await GenerateJwt(user)
            };
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

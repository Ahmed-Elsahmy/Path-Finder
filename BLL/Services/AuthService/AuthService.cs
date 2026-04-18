using BLL.Dtos.AuthDtos;
using BLL.Dtos.UserProfileDtos;
using BLL.Services.EmailService;
using BLL.Services.UserProfileServices;
using DAL.Helper;
using DAL.Models;
using Google;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BLL.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IUserProfileService _userProfileService;
        private readonly AppDbContext _context;

        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt, IConfiguration configuration, IEmailService emailService,IUserProfileService userProfileService,AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _configuration = configuration;
            _emailService = emailService;
            _userProfileService = userProfileService;
            _context = context;
        }
        public async Task<AuthModel> RegisterAsync(RegisterRQ model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (await _userManager.FindByEmailAsync(model.Email) is not null)
                    return new AuthModel { Message = "Email is already registered!" };

                if (await _userManager.FindByNameAsync(model.Username) is not null)
                    return new AuthModel { Message = "Username is already registered!" };

                var user = new User
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    CreatedAt = DateTime.UtcNow,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(",", result.Errors.Select(e => e.Description));
                    return new AuthModel { Message = errors };
                }

                await _userManager.AddToRoleAsync(user, "User");
                await _userManager.UpdateSecurityStampAsync(user);

                var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                var emailBody = $"<h1>Welcome to Path Finder!</h1><p>Your code: <strong>{otp}</strong></p>";

                await _emailService.SendEmailAsync(user.Email, "Confirm Email", emailBody);

                var jwtSecurityToken = await CreateJwtToken(user);

                await _userProfileService.AddUserProfileAsync(user.Id, new UserProfileRQ
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    UpdatedAt = DateTime.UtcNow
                });
                await transaction.CommitAsync();

                return new AuthModel
                {
                    Email = user.Email,
                    ExpiresOn = jwtSecurityToken.ValidTo,
                    IsAuthenticated = true,
                    Roles = new List<string> { "User" },
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    Username = user.UserName,
                    Message = "User registered successfully!"
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new AuthModel { Message = ex.Message };
            }
        }
        public async Task<AuthModel> LoginAsync(LoginRQ model)
        {
            try
            {
                var authModel = new AuthModel();

                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    authModel.Message = "Email or Password is incorrect!";
                    return authModel;
                }
                if (!user.EmailConfirmed)
                {
                    authModel.Message = "Please confirm your email address before logging in!";
                    return authModel;
                }
                var jwtSecurityToken = await CreateJwtToken(user);
                user.LastLogin = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                var rolesList = await _userManager.GetRolesAsync(user);

                authModel.IsAuthenticated = true;
                authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                authModel.Email = user.Email;
                authModel.Username = user.UserName;
                authModel.ExpiresOn = jwtSecurityToken.ValidTo;
                authModel.Roles = rolesList.ToList();
                authModel.Message = "User logged in successfully!";

                return authModel;
            }
            catch (Exception ex)
            {
                return new AuthModel { Message = ex.Message };
            }
        }
        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            try
            {
                var userClaims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);
                var roleClaims = new List<Claim>();

                foreach (var role in roles)
                    roleClaims.Add(new Claim("roles", role));

                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
                new Claim("ss", user.SecurityStamp ?? "")
            }
                .Union(userClaims)
                .Union(roleClaims);

                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    issuer: _jwt.Issuer,
                    audience: _jwt.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                    signingCredentials: signingCredentials);

                return jwtSecurityToken;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AuthModel> GoogleSignInAsync(GoogleAuthRQ model)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _configuration["Google:ClientId"] }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken, settings);

                var user = await _userManager.FindByEmailAsync(payload.Email);

                if (user == null)
                {
                    user = new User
                    {
                        UserName = payload.Email.Split('@')[0], //save all chars before @ as username
                        Email = payload.Email,
                        FirstName = payload.GivenName ?? "",
                        LastName = payload.FamilyName ?? "",
                        CreatedAt = DateTime.UtcNow,
                        LastLogin = DateTime.UtcNow,
                        EmailConfirmed = true // Since it's Google auth, we can consider the email as confirmed
                    };
                    // there's no password since it's Google auth, so we create the user without a password
                    var result = await _userManager.CreateAsync(user);

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(",", result.Errors.Select(e => e.Description));
                        return new AuthModel { Message = errors };
                    }

                    await _userManager.AddToRoleAsync(user, "User");
                }
                else
                {
                    // Update Last Login if user already exists
                    user.LastLogin = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);
                }

                var jwtSecurityToken = await CreateJwtToken(user);
                var rolesList = await _userManager.GetRolesAsync(user);
                var profileResult = await _userProfileService.GetUserProfileAsync(user.Id);
                if (!profileResult.IsSuccess) 
                {
                    await _userProfileService.AddUserProfileAsync(user.Id, new UserProfileRQ
                    {
                        UserName = user.UserName,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
                return new AuthModel
                {
                    Email = user.Email,
                    ExpiresOn = jwtSecurityToken.ValidTo,
                    IsAuthenticated = true,
                    Roles = rolesList.ToList(),
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    Username = user.UserName,
                    Message = "User authenticated successfully via Google!"
                };
            }
            catch (InvalidJwtException ex)
            {
                return new AuthModel { Message = $"Invalid Google Authentication Token" + ex.Message};
            }
            catch (Exception ex)
            {
                return new AuthModel { Message = ex.Message };
            }
        }

        public async Task<AuthModel> ForgotPasswordAsync(ForgotPasswordRQ model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);  // get user data

            if (user == null)
            {
                return new AuthModel { Message = "If an account with that email exists, a code has been sent." };
            }
            try
            {
                await _userManager.UpdateSecurityStampAsync(user);
                var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "Email"); // genterate Uniqe Code
                // send confrimation mail to email address
                var emailBody = $"<h1> Hello From Path Finder </h1> <h1>Reset Your Password</h1><p>Your 6-digit password reset code is: <strong>{otp}</strong></p><p>This code will expire shortly , Path Finder (Team).</p>";

                await _emailService.SendEmailAsync(user.Email, "Path Finder : Your Password Reset Code", emailBody);
                await _userManager.UpdateSecurityStampAsync(user);

                return new AuthModel
                {
                    IsAuthenticated = false,
                    Message = "A 6-digit code has been sent to your email address.",
                    Username = user.UserName,
                    Email = user.Email,

                };
            }
            catch (Exception ex)
            {
                return new AuthModel { Message = "An error occurred while Send OTP Code To Your Email : " + ex.Message };

            }
        }

        public async Task<AuthModel> ResetPasswordAsync(ResetPasswordRQ model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new AuthModel { Message = "Invalid Email or OTP code." };

            try
            {
                // Check otp code
                var isValidOtp = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", model.Otp);

                if (!isValidOtp)
                    return new AuthModel { Message = "Invalid or expired OTP code." };

                //  Since the OTP is valid, generate the long Identity token internally
                // (We do this entirely in the backend so the user never has to see the massive string)
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new AuthModel { Message = errors };
                }

                return new AuthModel { Message = "Password has been reset successfully!", Username = user.UserName, Email = user.Email };
            }
            catch (Exception ex)
            {
                return new AuthModel { Message = "An error occurred while resetting the password: " + ex.Message };
            }
        }

        public async Task<AuthModel> ConfirmEmailAsync(ConfirmEmailRQ model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new AuthModel { Message = "User not found." };

            if (user.EmailConfirmed)
                return new AuthModel { Message = "Email is already confirmed. You can log in directly." };
            try
            {
                var isValidOtp = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", model.Otp);

                if (!isValidOtp)
                    return new AuthModel { Message = "Invalid or expired OTP code." };

                // If OTP is valid, mark email as confirmed
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);

                return new AuthModel
                {
                    Message = "Email confirmed successfully! You can now log in.",
                    IsAuthenticated = true,
                    Email = user.Email,
                    Username = user.UserName
                };
            }
            catch (Exception ex)
            {
                return new AuthModel { Message = "An error occurred while confirming the email: " + ex.Message };

            }
        }

        public async Task<AuthModel> ResendOtpAsync(ResendOTPRQ model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return new AuthModel { Message = "User not found." };

            if (user.EmailConfirmed)
                return new AuthModel { Message = "Email already confirmed." };

            try
            {
                await _userManager.UpdateSecurityStampAsync(user);
                var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                var emailBody = $"<h1>Path Finder</h1><p>Your new OTP code: <strong>{otp}</strong></p>";

                await _emailService.SendEmailAsync(user.Email, "Resend OTP", emailBody);

                return new AuthModel
                {
                    Message = "OTP has been resent successfully.",
                    Email = user.Email,
                    Username = user.UserName
                };
            }
            catch (Exception ex)
            {
                return new AuthModel
                {
                    Message = "Error while resending OTP: " + ex.Message
                };
            }
        }
        public async Task<AuthModel> LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return new AuthModel { Message = "User not found" };

            await _userManager.UpdateSecurityStampAsync(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            return new AuthModel
            {
                Message = "Logged out successfully",
                Username=user.UserName,
                Email=user.Email,
                Roles=rolesList.ToList(),
                ExpiresOn=DateTime.Now
            };
        }
    }
}
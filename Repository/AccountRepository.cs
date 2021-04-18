using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookAppStore.Areas.Admin.Models;
using BookAppStore.Models;
using BookAppStore.Service;

namespace BookAppStore.Repository
{
    public class AccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signManager;
        private readonly UserService _userService;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;

        public AccountRepository(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            UserService userService, EmailService emailService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signManager = signInManager;
            _userService = userService;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<ApplicationUser> GetUserId(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> CreateUserAsync(UserSignUpModel userSignUpModel)
        {
            var user = new ApplicationUser()
            {
                FirstName = userSignUpModel.FirstName,
                LastName = userSignUpModel.LastName,
                Email = userSignUpModel.Email,
                UserName = userSignUpModel.Email
            };
            
            var result = await _userManager.CreateAsync(user, userSignUpModel.Password);
            var userId = _userService.GetUserId();
            if (result.Succeeded)
            {
                // send confirmation email
                await GenerateEmailConfirmationToken(user);
            }

            return result;
        }

        public async Task GenerateEmailConfirmationToken(ApplicationUser user)
        {
            // Generate user email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                await SendUserConfirmationEmail(user, token);
            }
        }

        public async Task GenerateForgotPaswordToken(ApplicationUser user)
        {
            // Generate user forgot password token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                await SendForgotPasswordEmail(user, token);
            }
        }

        public async Task<SignInResult> SignInUserAsync(UserSignInModel userSignInModel)
        {
            // sign in user
            return await _signManager.PasswordSignInAsync(
                userSignInModel.Email, userSignInModel.Password, userSignInModel.RememberMe, true);
        }

        public async Task<IdentityResult> ActivateUserEmail(string uid, string token)
        {
            // confirm / activate user email
            return await _userManager.ConfirmEmailAsync(
                await _userManager.FindByIdAsync(uid), token);
        }

        public async Task UserLogoutAsync()
        {
            await _signManager.SignOutAsync();
        }

        public async Task<IdentityResult> UpdateProfileAsync(EditUserProfile editUserProfile)
        {
            // implement update profile
            var userId = _userService.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.FirstName = editUserProfile.FirstName;
                user.LastName = editUserProfile.LastName;
                user.Email = editUserProfile.Email;
            }
            var result = await _userManager.UpdateAsync(user);

            return result;
        }

        public async Task<IdentityResult> ChangeUserPasswordAsync(ChangePasswordModel model)
        {
            // change password
            var userId = _userService.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);
            return await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        }
        
        public async Task<IdentityResult> UserResetPasswordAsync(ResetPasswordModel resetPasswordModel)
        {
            // reset user password
            return await _userManager.ResetPasswordAsync(
                await _userManager.FindByIdAsync(resetPasswordModel.UserId), resetPasswordModel.Token, 
                resetPasswordModel.NewPassword
            );
        }

        private async Task SendUserConfirmationEmail(ApplicationUser user, string token)
        {
            // confirmations email and placeholder
            string appDomain = _configuration.GetSection("Application:AppDomain").Value;
            string confirmationLink = _configuration.GetSection("Application:EmailConfirmation").Value;

            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string>() { user.Email },
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.FirstName),
                    new KeyValuePair<string, string>("{{Link}}",
                        string.Format(appDomain + confirmationLink, user.Id, token))
                }
            };

            await _emailService.SendConfirmationEmail(options);
        }

        private async Task SendForgotPasswordEmail(ApplicationUser user, string token)
        {
            // Forgot password email and placeholder
            string appDomain = _configuration.GetSection("Application:AppDomain").Value;
            string confirmationLink = _configuration.GetSection("Application:ResetPassword").Value;

            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string>() { user.Email },
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.FirstName),
                    new KeyValuePair<string, string>("{{Link}}",
                        string.Format(appDomain + confirmationLink, user.Id, token))
                }
            };

            await _emailService.SendForgotPasswordEmail(options);
        }
    }
}
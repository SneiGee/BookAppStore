using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookAppStore.Areas.Admin.Models;
using BookAppStore.Repository;
using BookAppStore.Models;
using BookAppStore.Service;

namespace BookAppStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AccountRepository _accountRepository;
        private readonly UserService _userService;

        public AccountController(UserManager<ApplicationUser> userManager, 
            AccountRepository accountRepository, UserService userService)
        {
            _userManager = userManager;
            _accountRepository = accountRepository;
            _userService = userService;
        }

        [Route("create-account")]
        public IActionResult UserSignUp()
        {
            return View();
        }

        [HttpPost("create-account")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserSignUp(UserSignUpModel userSignUpModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.CreateUserAsync(userSignUpModel);

                if (result.Succeeded)
                {
                    ViewBag.IsSuccess = true;
                    ModelState.Clear();
                    return View();
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(userSignUpModel);
        }

        [Route("login")]
        public IActionResult UserLogin()
        {
            // login form view
            return View();
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserLogin(UserSignInModel userSignInModel, string returnUrl)
        {
            // valid the login form and login user if success
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.SignInUserAsync(userSignInModel);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Not allowed to login");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "You account is locked, Please try again");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid credentials!");
                }
            }

            return View(userSignInModel);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ActivateEmail(string uid, string token)
        {
            //  activate user email
            if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(token))
            {
                var userId = await _accountRepository.GetUserId(uid);

                token = token.Replace(' ', '+');
                var result = await _accountRepository.ActivateUserEmail(uid, token);
                if (result.Succeeded)
                {
                    // add User role to confirm user
                    await _userManager.AddToRoleAsync(userId, "User");
                    ViewBag.IsSuccess = true;
                }
            }

            return View();
        }

        [Route("logout")]
        public async Task<IActionResult> UserLogout()
        {
            // User Logout
            await _accountRepository.UserLogoutAsync();
            return RedirectToAction("UserLogin", "Account");
        }

        [HttpGet("account")]
        public async Task<IActionResult> UserProfile()
        {
            // display user data
            var userId = _userService.GetUserId();
            var getUser = await _userManager.FindByIdAsync(userId);
            if (getUser == null)
            {
                return NotFound($"Unable to load user data");
            }

            var model = new EditUserProfile()
            {
                FirstName = getUser.FirstName,
                LastName = getUser.LastName,
                Email = getUser.Email
            };

            return View(model);
        }

        [HttpPost("account")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProfile(EditUserProfile editUserProfile)
        {
            // validate and update profile form
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.UpdateProfileAsync(editUserProfile);
                if (result.Succeeded)
                {
                    ViewBag.IsSuccess = true;
                    return View(editUserProfile);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(editUserProfile);
        }

        [Route("change-password")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost("change-password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            // change user password
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.ChangeUserPasswordAsync(model);
                if (result.Succeeded)
                {
                    ViewBag.IsSuccess = true;
                    ModelState.Clear();
                    return View();
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [AllowAnonymous, HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            // view for forgotpassword
            return View();
        }

        [AllowAnonymous, HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(UserForgotPasswordModel forgotPassword)
        {
            // validate and sent a reset password
            if (ModelState.IsValid)
            {
                var user = await _accountRepository.GetUserByEmail(forgotPassword.Email);

                if (user != null)
                {
                    // sent reset password email to request user email
                    await _accountRepository.GenerateForgotPaswordToken(user);
                }

                ModelState.Clear();
                forgotPassword.EmailSent = true;
            }
            
            return View(forgotPassword);
        }

        [AllowAnonymous, HttpGet("reset-password")]
        public IActionResult ResetPassword(string uid, string token)
        {
            // view for reset user password
            ResetPasswordModel resetPasswordModel = new ResetPasswordModel
            {
                UserId = uid,
                Token = token
            };

            return View(resetPasswordModel);
        }

        [AllowAnonymous, HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            // validate and reset user password
            if (ModelState.IsValid)
            {
                resetPasswordModel.Token = resetPasswordModel.Token.Replace(' ', '+');
                var result = await _accountRepository.UserResetPasswordAsync(resetPasswordModel);
                if (result.Succeeded)
                {
                    ModelState.Clear();
                    resetPasswordModel.IsSuccess = true;

                    return View(resetPasswordModel);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(resetPasswordModel);
        }
    }
}
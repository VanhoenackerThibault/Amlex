using Amlex.Models.Core;
using Amlex.Models.Models.Authentication;
using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Amlex.Mvc.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> RegisterRequest(RegisterRequest registerRequest, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Register));
            }

            returnUrl ??= Url.Content("~/");

            ApplicationUser applicationUser = new ApplicationUser
            {
                Email = registerRequest.Email,
                UserName = registerRequest.Email,
            };

            IdentityResult result = await userManager.CreateAsync(applicationUser, registerRequest.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(nameof(Register), registerRequest);
            }

            await signInManager.SignInAsync(applicationUser, false);

            return LocalRedirect(returnUrl);
        }
    
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> SignInRequest(SignInRequest signInRequest, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(SignIn), signInRequest);
            }

            returnUrl ??= Url.Content("~/");

            var result = await signInManager.PasswordSignInAsync(signInRequest.Email, signInRequest.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid username/email or password.");
                return View(nameof(SignIn), signInRequest);
            }

            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> SignOut(string? returnUrl)
        {
            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            return LocalRedirect(returnUrl);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

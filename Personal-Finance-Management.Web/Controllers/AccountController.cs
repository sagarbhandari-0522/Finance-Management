using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Personal_Finance_Management.Domain.Entities;
using Personal_Finance_Management.Web.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Personal_Finance_Management.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await AddFirstNameClaimsAsync(user);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _signInManager.PasswordSignInAsync(
                userName: model.Email,
                password: model.Password,
                isPersistent: model.RememberMe,
                lockoutOnFailure: false
                );
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                await AddFirstNameClaimsAsync(user);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Invalid Login attempt");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        [HttpGet("external-signin")]
        public IActionResult ExternalSignIn(string provider)
        {
            if (string.IsNullOrEmpty(provider))
                return BadRequest("Provider is required");
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction("Login", "Account");
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user != null)
            {
                await AddFirstNameClaimsAsync(user);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");

            }
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            var newUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "User",
                LastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "N/A",
            };
            var identityresult = await _userManager.CreateAsync(newUser);
            if (identityresult.Succeeded)
            {
                identityresult = await _userManager.AddLoginAsync(newUser, info);
                if (identityresult.Succeeded)
                {
                    await AddFirstNameClaimsAsync(newUser);
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Login", "Account");
        }
        public async Task<IActionResult> Setting(string userId)
        {
            if (userId == null)
                return RedirectToAction("Login");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return RedirectToAction("Login");
            var hasPassword = await _userManager.HasPasswordAsync(user);
            var updatePassword = new UpdatePasswordVM()
            {
                UserId = user.Id,
                IsExternalUser = !hasPassword
            };
            var updateDetails = new UpdateUserDetailsVM
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                UserId = user.Id,
                IsExternalUser = !hasPassword
            };
            var settingVM = new SettingVM
            {
                userDetails = updateDetails,
                updatePassword = updatePassword,
                ActiveTab = "Details",
            };
            return View(settingVM);
        }
        public async Task<IActionResult> UpdateDetails(UpdateUserDetailsVM model)
        {
            if (!ModelState.IsValid)
            {
                var settingModel = new SettingVM
                {
                    userDetails = model,
                    updatePassword = new UpdatePasswordVM() { UserId = "", IsExternalUser = model?.IsExternalUser ?? true },
                    ActiveTab = "Details",
                };
                return View("Setting", settingModel);
            }
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return RedirectToAction("Login");
            var hasPassword = await _userManager.HasPasswordAsync(user);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                var settingModel = new SettingVM
                {
                    userDetails = model,
                    updatePassword = new UpdatePasswordVM { UserId = model.UserId, IsExternalUser = !hasPassword },
                    ActiveTab = "Details"

                };
                return View("Setting", settingModel);
            }
            await AddFirstNameClaimsAsync(user);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> UpdatePassword(UpdatePasswordVM model)
        {
            if (model.IsExternalUser) return RedirectToAction("Login");
            if (!ModelState.IsValid)
            {
                var newsettingModel = new SettingVM
                {
                    userDetails = new UpdateUserDetailsVM { IsExternalUser = model?.IsExternalUser ?? true },
                    updatePassword = model,
                    ActiveTab = "Password"
                };
                return View("Setting", newsettingModel);
            }
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return RedirectToAction("Login");
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["Success"] = "Password Updated Successfully";
                return RedirectToAction("Logout");
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            var settingModel = new SettingVM
            {
                userDetails = new UpdateUserDetailsVM
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    IsExternalUser = model.IsExternalUser
                },
                updatePassword = model,
                ActiveTab = "Password"
            };
            return View("Setting", settingModel);
        }
        private async Task<bool> AddFirstNameClaimsAsync(ApplicationUser user)
        {
            var existingClaims = await _userManager.GetClaimsAsync(user);
            var existingFirstNameClaim = existingClaims.FirstOrDefault(c => c.Type == "FirstName");
            if (existingFirstNameClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, existingFirstNameClaim);
            }
            var result = await _userManager.AddClaimAsync(user, new Claim("FirstName", user.FirstName));

            return result.Succeeded;
        }


    }
}

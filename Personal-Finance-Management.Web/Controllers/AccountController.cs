using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Personal_Finance_Management.Domain.Entities;
using Personal_Finance_Management.Web.ViewModels;
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
                LastName=model.LastName,
                Email = model.Email,
                UserName = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }
        [HttpGet]
        public IActionResult Login() =>  View();
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var result=await _signInManager.PasswordSignInAsync(
                userName:model.Email,
                password:model.Password,
                isPersistent:model.RememberMe,
                lockoutOnFailure:false
                );
            if(result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Invalid Login attempt");
            return View(model);
        }
      
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

    }
}

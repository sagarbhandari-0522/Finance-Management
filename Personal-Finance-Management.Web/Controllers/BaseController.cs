using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Personal_Finance_Management.Domain.Entities;

namespace Personal_Finance_Management.Web.Controllers
{
    public class BaseController : Controller
    {
        protected  string? CurrentUserId { get; private set; }
        private UserManager<ApplicationUser> _userManager;
        public BaseController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            CurrentUserId = _userManager.GetUserId(User);
            base.OnActionExecuting(context);
        }
    }
}

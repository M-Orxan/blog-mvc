using AspNetCoreHero.ToastNotification.Abstractions;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INotyfService _notification;

        public UsersController(UserManager<ApplicationUser> userManager,
                               SignInManager<ApplicationUser> signInManager,
                               INotyfService notification)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notification = notification;
        }
        public IActionResult Index()
        {
           
                return View();

            
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (!User.Identity.IsAuthenticated)
                return View(new LoginVM());

            return RedirectToAction("Index", "Users", new { area = "Admin" });
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginVM vm)
        {


            if (!ModelState.IsValid)
                return View(vm);

            var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == vm.Username);

            if (existingUser == null)
            {
                _notification.Error("Username or Password is wrong");
                return View(vm);
            }

            var verifyPassword = await _userManager.CheckPasswordAsync(existingUser, vm.Password);
            if (!verifyPassword)
            {
                _notification.Error("Username or Password is wrong");
                return View(vm);
            }



            await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, vm.RememberMe, true);
            _notification.Success("Login succesfull");

            return RedirectToAction("Index", "Users", new { area = "Admin" });

        }

        [HttpPost]

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _notification.Success("Logout succesfull");
            return RedirectToAction("Index", "Home", new { area=""});
        }


    }
}

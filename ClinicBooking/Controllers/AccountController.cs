using Microsoft.AspNetCore.Mvc;
using ClinicBooking.Models;
using ClinicBooking.Repositories;
using ClinicBooking.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace ClinicBooking.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository userRepo;
        private readonly PasswordService passwordService;

        public AccountController(
            IUserRepository userRepo,
            PasswordService passwordService)
        {
            this.userRepo = userRepo;
            this.passwordService = passwordService;
        }


        // =========================================
        // ACCESS DENIED
        // =========================================

        public IActionResult AccessDenied()
        {
            return StatusCode(403);
        }


        // =========================================
        // LOGOUT
        // =========================================

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }


        // =========================================
        // LOGIN - GET
        // =========================================

        public IActionResult Login()
        {
            return View();
        }


        // =========================================
        // LOGIN - POST
        // =========================================

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }


            var user = userRepo.GetByEmail(vm.Email);


            if (user == null ||
                !passwordService.VerifyPassword(
                    vm.Password,
                    user.PasswordHash))
            {
                ModelState.AddModelError(
                    "",
                    "Invalid email or password.");

                return View(vm);
            }


            List<Claim> claims =
                new List<Claim>
                {
                    new Claim(
                        ClaimTypes.Name,
                        user.Email),

                    new Claim(
                        ClaimTypes.Role,
                        user.Role)
                };


            ClaimsIdentity identity =
                new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);


            ClaimsPrincipal principal =
                new ClaimsPrincipal(identity);


            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);


            return RedirectToAction(
                "Index",
                "Home");
        }


        // =========================================
        // REGISTER - GET
        // =========================================

        public IActionResult Register()
        {
            return View();
        }


        // =========================================
        // REGISTER - POST
        // =========================================

        [HttpPost]
        public IActionResult Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }


            bool emailExists =
                userRepo.EmailExists(vm.Email);


            if (emailExists)
            {
                ModelState.AddModelError(
                    "Email",
                    "This email is already registered.");

                return View(vm);
            }


            User user =
                new User
                {
                    Email = vm.Email,

                    PasswordHash =
                        passwordService.HashPassword(
                            vm.Password),

                    Role = "Patient"
                };


            userRepo.Add(user);


            return RedirectToAction(
                "Login",
                "Account");
        }
    }
}
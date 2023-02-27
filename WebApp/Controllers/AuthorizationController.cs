using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.Context;
using WebApp.Entities;
using WebApp.Models.Auth;
using WebApp.Tools;

namespace WebApp.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly WebAppContext _context;
        public AuthorizationController(WebAppContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User? user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Login == model.Login && u.Password == model.Password);

                if (user is null)
                {
                    return Content("Не верно введен пароль или логин");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return Redirect("/Home/Index");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Login == model.Login);

                if (user != null)
                {
                    return Content("Такой пользователь уже существует");
                }

                await _context.Users.AddAsync(new User
                {
                    Id = Guid.NewGuid(),
                    Login = model.Login,
                    Password = model.Password,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    IsBlocked = false,
                    UserName = model.NickName,
                    Role = _context.Roles.First(r => r.Id == ConstansCS.Roles.User)
                });

                await _context.SaveChangesAsync();
                return Redirect("/Authorization/Login");
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Authorization/Login");
        }
    }
}

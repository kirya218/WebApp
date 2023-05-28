using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.Context;
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
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);

                if (user == null)
                {
                    ModelState.AddModelError("Login", "Не верно введен пароль или логин");
                    return View();
                }

                if (user.IsBlocked)
                {
                    ModelState.AddModelError("Login", "Данный пользователь заблокирован");
                    return View();
                }

                var claims = new List<Claim>
                {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                    new Claim("RoleId", user.Role.Id.ToString()),
                    new Claim("RoleName", user.Role.Name)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return Redirect("/Home/Index");
            }

            return View();
        }

        /// <summary>
        /// Не используется.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Login == model.Login);

                if (user != null)
                {
                    return Content("Такой пользователь уже существует");
                }

                await _context.Users.AddAsync(new Entities.User
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

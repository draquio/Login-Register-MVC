using LoginProject.Data;
using LoginProject.Models;
using LoginProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace LoginProject.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserDbContext _dbContext;

        public AuthController(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserVM user)
        {
            if (user.Password != user.RepeatPassword) {
                ViewData["Message"] = "Las contraseñas deben coincidir";
                return View();
            }
            User createdUser = new User()
            {
                Username = user.Username,
                Email = user.Email,
                Password = user.Password,
            };
            await _dbContext.Users.AddAsync(createdUser);
            await _dbContext.SaveChangesAsync();
            if(createdUser.Id != 0) return RedirectToAction("Login", "Auth");

            ViewData["Message"] = "No se pudo registrar su cuenta";
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if(User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login)
        {
            User? userSearched = await _dbContext.Users
                .Where(u => u.Username == login.Username && u.Password == login.Password)
                .FirstOrDefaultAsync();
            if (userSearched == null) {
                ViewData["Message"] = "No se encontró coincidencias";
                return View();
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userSearched.Username)
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
                );
            return RedirectToAction("Index", "Home");
        }
    }
}

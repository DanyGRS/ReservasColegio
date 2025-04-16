using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasColegio.Context;
using ReservasColegio.Models;
using ReservasColegio.Services;

namespace ReservasColegio.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;

        public LoginController(AppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public IActionResult Default()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var login = await _context.Logins
                .Include(l => l.Usuario)
                .FirstOrDefaultAsync(l => l.Username == email && l.Senha == password);

            if (login != null)
            {
                var token = _authService.GerarToken(login.Usuario);
                var claimsPrincipal = _authService.GerarClaimsPrincipal(login.Usuario);
                await HttpContext.SignInAsync("Cookies", claimsPrincipal);

                return RedirectToAction("Index", "Home");
            }

            TempData["Erro"] = "Login inválido!";
            return View("Default");
        }

        public IActionResult EsqueciSenha()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Default", "Login");
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasColegio.Context;
using ReservasColegio.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace ReservasColegio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Authorize]
        public IActionResult Perfil()
        {
            var nome = User.Identity?.Name;
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            var permissoes = User.Claims.Where(c => c.Type == "permissao").Select(c => c.Value).ToList();

            ViewBag.Nome = nome;
            ViewBag.Email = email;
            ViewBag.Roles = roles;
            ViewBag.Permissoes = permissoes;

            return View();
        }

        [Authorize]
        public async Task<IActionResult> EditarPerfil()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var usuario = await _context.Usuarios.FindAsync(userId);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPerfil(Usuario model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var usuario = await _context.Usuarios.FindAsync(userId);

            if (usuario == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            usuario.Nome = model.Nome;
            usuario.Email = model.Email;
            usuario.Departamento = model.Departamento;

            await _context.SaveChangesAsync();
            TempData["Sucesso"] = "Perfil atualizado com sucesso!";
            return RedirectToAction("Perfil");
        }

    }
}

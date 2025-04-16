using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservasColegio.Context;
using ReservasColegio.Models;

namespace ReservasColegio.Controllers
{
    public class ReservasController : Controller
    {
        private readonly AppDbContext _context;

        public ReservasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Reservas
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(DateTime? dataInicio, DateTime? dataFim)
        {
            var reservas = _context.Reservas
                .Include(r => r.Equipamento)
                .Include(r => r.Usuario)
                .AsQueryable();

            if (dataInicio.HasValue)
            {
                reservas = reservas.Where(r => r.DataReserva >= dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                reservas = reservas.Where(r => r.DataReserva <= dataFim.Value);
            }

            ViewData["DataInicio"] = dataInicio?.ToString("yyyy-MM-dd");
            ViewData["DataFim"] = dataFim?.ToString("yyyy-MM-dd");

            return View(await reservas.ToListAsync());
        }

        // GET: Reservas/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var reserva = await _context.Reservas
                .Include(r => r.Equipamento)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null)
                return NotFound();

            return View(reserva);
        }

        // GET: Reservas/Create
        public IActionResult Create()
        {
            ViewData["EquipamentoId"] = new SelectList(_context.Equipamentos, "Id", "Nome");

            if (User.IsInRole("Admin"))
            {
                ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email");
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UsuarioId,EquipamentoId,DataReserva,HoraInicio,HoraFim,Status")] Reserva reserva)
        {
            if (!User.IsInRole("Admin"))
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                reserva.UsuarioId = userId;
            }

            if (!ModelState.IsValid)
            {
                ViewData["EquipamentoId"] = new SelectList(_context.Equipamentos, "Id", "Nome", reserva.EquipamentoId);
                if (User.IsInRole("Admin"))
                {
                    ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", reserva.UsuarioId);
                }
                return View(reserva);
            }

            if (reserva.HoraFim <= reserva.HoraInicio)
                ModelState.AddModelError("", "A hora de término deve ser posterior à hora de início.");

            if (User.IsInRole("Funcionario"))
            {
                var inicio = reserva.HoraInicio.TimeOfDay;
                var fim = reserva.HoraFim.TimeOfDay;
                var horarioMin = new TimeSpan(7, 0, 0);
                var horarioMax = new TimeSpan(18, 0, 0);

                if (inicio < horarioMin || fim > horarioMax)
                {
                    ModelState.AddModelError("", "Funcionários só podem reservar entre 07h e 18h.");
                }
            }

            var conflito = await _context.Reservas
                .Where(r => r.EquipamentoId == reserva.EquipamentoId
                    && r.DataReserva == reserva.DataReserva
                    && r.Status != StatusReserva.Cancelada
                    && ((reserva.HoraInicio >= r.HoraInicio && reserva.HoraInicio < r.HoraFim)
                        || (reserva.HoraFim > r.HoraInicio && reserva.HoraFim <= r.HoraFim)
                        || (reserva.HoraInicio <= r.HoraInicio && reserva.HoraFim >= r.HoraFim)))
                .AnyAsync();

            if (conflito)
                ModelState.AddModelError("", "Já existe uma reserva para este equipamento neste horário.");

            if (!ModelState.IsValid)
            {
                ViewData["EquipamentoId"] = new SelectList(_context.Equipamentos, "Id", "Nome", reserva.EquipamentoId);
                if (User.IsInRole("Admin"))
                {
                    ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", reserva.UsuarioId);
                }
                return View(reserva);
            }

            reserva.Status = StatusReserva.Pendente;

            _context.Add(reserva);
            await _context.SaveChangesAsync();

            if (User.IsInRole("Admin"))
                return RedirectToAction(nameof(Index));
            else
                return RedirectToAction(nameof(Minhas));
        }

        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var reserva = await _context.Reservas
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
                return NotFound();

            if (reserva.Status != StatusReserva.Pendente)
            {
                TempData["Erro"] = "Reservas aprovadas ou em uso não podem ser editadas.";
                return RedirectToAction(nameof(Index));
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role == "Funcionario" && reserva.UsuarioId != userId)
            {
                TempData["Erro"] = "Você não pode editar reservas de outros usuários.";
                return RedirectToAction("Minhas");
            }

            ViewBag.UsuarioLogadoId = userId;
            ViewBag.Role = role;

            ViewData["EquipamentoId"] = new SelectList(_context.Equipamentos, "Id", "Nome", reserva.EquipamentoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", reserva.UsuarioId);

            return View(reserva);
        }

        // POST: Reservas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UsuarioId,EquipamentoId,DataReserva,HoraInicio,HoraFim,Status")] Reserva reserva)
        {
            if (id != reserva.Id)
                return NotFound();

            var reservaAtual = await _context.Reservas.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            if (reservaAtual == null)
                return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role == "Funcionario")
            {
                if (reservaAtual.UsuarioId != userId || reservaAtual.DataReserva.Date < DateTime.Today)
                {
                    TempData["Erro"] = "Você não pode editar essa reserva.";
                    return RedirectToAction("Minhas");
                }

                reserva.UsuarioId = userId;
                reserva.Status = reservaAtual.Status;
            }

            if (role != "Admin")
            {
                reserva.UsuarioId = reservaAtual.UsuarioId;
                reserva.Status = reservaAtual.Status;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Reservas.Any(e => e.Id == reserva.Id))
                        return NotFound();
                    else
                        throw;
                }

                return role == "Admin" ? RedirectToAction("Index") : RedirectToAction("Minhas");
            }

            ViewBag.UsuarioLogadoId = userId;
            ViewBag.Role = role;

            ViewData["EquipamentoId"] = new SelectList(_context.Equipamentos, "Id", "Nome", reserva.EquipamentoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", reserva.UsuarioId);

            return View(reserva);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Cancelar(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);

            if (reserva == null)
                return NotFound();

            // Apenas o dono da reserva ou Admin pode cancelar
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (User.IsInRole("Funcionario") && reserva.UsuarioId != userId)
            {
                return Forbid();
            }

            if (reserva.DataReserva < DateTime.Today || reserva.Status != StatusReserva.Pendente)
            {
                TempData["Erro"] = "Só é possível cancelar reservas futuras que estejam pendentes.";
                return RedirectToAction("Minhas");
            }

            reserva.Status = StatusReserva.Cancelada;
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Reserva cancelada com sucesso.";
            return RedirectToAction("Minhas");
        }


        // GET: Reservas/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var reserva = await _context.Reservas
                .Include(r => r.Equipamento)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null)
                return NotFound();

            if (reserva.Status != StatusReserva.Pendente)
            {
                TempData["Erro"] = "Somente reservas pendentes podem ser canceladas.";
                return RedirectToAction(nameof(Index));
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);

            if (reserva == null)
                return NotFound();

            if (reserva.Status != StatusReserva.Pendente)
            {
                TempData["Erro"] = "Somente reservas pendentes podem ser canceladas.";
                return RedirectToAction(nameof(Index));
            }

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }

        [Authorize]
        public async Task<IActionResult> Minhas()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                return Unauthorized();

            var reservas = await _context.Reservas
                .Include(r => r.Equipamento)
                .Include(r => r.Usuario)
                .Where(r => r.UsuarioId == userId)
                .ToListAsync();

            return View(reservas);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Aprovar(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null || reserva.Status != StatusReserva.Pendente)
            {
                TempData["Erro"] = "Apenas reservas pendentes podem ser aprovadas.";
                return RedirectToAction(nameof(Index));
            }

            reserva.Status = StatusReserva.Aprovada;
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = "Reserva aprovada com sucesso.";
            return RedirectToAction(nameof(Index));
        }

    }
}

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasColegio.Context;
using ReservasColegio.Models;

namespace ReservasColegio.Controllers
{
    public class EquipamentosController : Controller
    {
        private readonly AppDbContext _context;

        public EquipamentosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Equipamentos.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var equipamento = await _context.Equipamentos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipamento == null) return NotFound();

            return View(equipamento);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Tipo,Descricao,IdentificacaoPatrimonio,Disponivel")] Equipamento equipamento)
        {
            if (_context.Equipamentos.Any(e => e.IdentificacaoPatrimonio == equipamento.IdentificacaoPatrimonio))
            {
                ModelState.AddModelError("IdentificacaoPatrimonio", "Já existe um equipamento com esta identificação patrimonial.");
                return View(equipamento);
            }

            equipamento.Nome = equipamento.Nome?.Trim();
            equipamento.IdentificacaoPatrimonio = equipamento.IdentificacaoPatrimonio?.Trim().ToUpper();

            if (ModelState.IsValid)
            {
                _context.Add(equipamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(equipamento);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var equipamento = await _context.Equipamentos.FindAsync(id);
            if (equipamento == null) return NotFound();

            return View(equipamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Tipo,Descricao,IdentificacaoPatrimonio,Disponivel")] Equipamento equipamento)
        {
            if (id != equipamento.Id) return NotFound();

            if (_context.Equipamentos.Any(e => e.IdentificacaoPatrimonio == equipamento.IdentificacaoPatrimonio && e.Id != equipamento.Id))
            {
                ModelState.AddModelError("IdentificacaoPatrimonio", "Outro equipamento já possui esta identificação patrimonial.");
                return View(equipamento);
            }

            equipamento.Nome = equipamento.Nome?.Trim();
            equipamento.IdentificacaoPatrimonio = equipamento.IdentificacaoPatrimonio?.Trim().ToUpper();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipamentoExists(equipamento.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(equipamento);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var equipamento = await _context.Equipamentos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipamento == null) return NotFound();

            return View(equipamento);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipamento = await _context.Equipamentos.FindAsync(id);
            if (equipamento != null)
                _context.Equipamentos.Remove(equipamento);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EquipamentoExists(int id)
        {
            return _context.Equipamentos.Any(e => e.Id == id);
        }
    }
}

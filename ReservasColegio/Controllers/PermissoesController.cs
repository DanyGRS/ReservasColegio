using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservasColegio.Context;
using ReservasColegio.Models;

namespace ReservasColegio.Controllers
{
    public class PermissoesController : Controller
    {
        private readonly AppDbContext _context;

        public PermissoesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Permissaos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Permissoes.ToListAsync());
        }

        // GET: Permissaos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permissao = await _context.Permissoes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (permissao == null)
            {
                return NotFound();
            }

            return View(permissao);
        }

        // GET: Permissaos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Permissaos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome")] Permissao permissao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(permissao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(permissao);
        }

        // GET: Permissaos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permissao = await _context.Permissoes.FindAsync(id);
            if (permissao == null)
            {
                return NotFound();
            }
            return View(permissao);
        }

        // POST: Permissaos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome")] Permissao permissao)
        {
            if (id != permissao.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(permissao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PermissaoExists(permissao.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(permissao);
        }

        // GET: Permissaos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permissao = await _context.Permissoes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (permissao == null)
            {
                return NotFound();
            }

            return View(permissao);
        }

        // POST: Permissaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var permissao = await _context.Permissoes.FindAsync(id);
            if (permissao != null)
            {
                _context.Permissoes.Remove(permissao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PermissaoExists(int id)
        {
            return _context.Permissoes.Any(e => e.Id == id);
        }
    }
}

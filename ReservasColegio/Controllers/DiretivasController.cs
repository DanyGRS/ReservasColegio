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
    public class DiretivasController : Controller
    {
        private readonly AppDbContext _context;

        public DiretivasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Diretivas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Diretivas.ToListAsync());
        }

        // GET: Diretivas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diretiva = await _context.Diretivas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diretiva == null)
            {
                return NotFound();
            }

            return View(diretiva);
        }

        // GET: Diretivas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Diretivas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome")] Diretiva diretiva)
        {
            if (ModelState.IsValid)
            {
                _context.Add(diretiva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(diretiva);
        }

        // GET: Diretivas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diretiva = await _context.Diretivas.FindAsync(id);
            if (diretiva == null)
            {
                return NotFound();
            }
            return View(diretiva);
        }

        // POST: Diretivas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome")] Diretiva diretiva)
        {
            if (id != diretiva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diretiva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiretivaExists(diretiva.Id))
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
            return View(diretiva);
        }

        // GET: Diretivas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diretiva = await _context.Diretivas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diretiva == null)
            {
                return NotFound();
            }

            return View(diretiva);
        }

        // POST: Diretivas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diretiva = await _context.Diretivas.FindAsync(id);
            if (diretiva != null)
            {
                _context.Diretivas.Remove(diretiva);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiretivaExists(int id)
        {
            return _context.Diretivas.Any(e => e.Id == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPTBook.Data;
using FPTBook.Models;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;

namespace FPTBook.Controllers
{
    [Authorize]
    public class PublishersController : Controller
    {
        private readonly AppDbContext _context;

        public PublishersController(AppDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: Publishers
        public async Task<IActionResult> Index(string filter, int page = 1, string sortExpression = "FullName")
        {
            var query = _context.Publishers.AsNoTracking()
                .OrderBy(p => p.Id)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(p => p.FullName.Contains(filter));
            }
            var model = await PagingList.CreateAsync(query, 4, page, sortExpression, "FullName");
            model.RouteValue = new RouteValueDictionary {
            { "filter", filter}
            };
            return View(model);
        }
        [AllowAnonymous]
        // GET: Publishers/Details/?id
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publishers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }


        // GET: Authors/Add
        // GET:Authors/Edit/?id
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Publisher());
            else
            {
                var publisher = await _context.Publishers.FindAsync(id);
                if (publisher == null)
                {
                    return NotFound();
                }
                return View(publisher);
            }
        }

        // POST: Publishers/Add
        // POST: Publishers/Edit/?id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("Id,ProfilePictureURL,FullName,Description")]Publisher publisher)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    _context.Update(publisher);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    try
                    {
                        _context.Update(publisher);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PublisherExists(publisher.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.Publishers.ToList()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", publisher) });
        }


        // POST: Publishers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();
            return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.Publishers.ToList()) });
        }

        private bool PublisherExists(int id)
        {
            return _context.Publishers.Any(e => e.Id == id);
        }
    }
}

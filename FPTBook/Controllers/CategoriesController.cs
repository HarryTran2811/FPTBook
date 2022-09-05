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

namespace FPTBook.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index(string filter, int page = 1, string sortExpression = "Name")
        {
            var query = _context.Categories.AsNoTracking()
                .OrderBy(p => p.Id)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(p => p.Name.Contains(filter));
            }
            var model = await PagingList.CreateAsync(query, 4, page, sortExpression, "Name");
            model.RouteValue = new RouteValueDictionary {
            { "filter", filter}
            };
            return View(model);
        }

        //GET: Categories/ProductCategory
        public ActionResult BookCategory (int Id)
        {
            var listProducts = _context.Books.Where(n => n.CategoryId == Id).ToList();
            return View(listProducts);
        }

        // GET: Authors/Add
        // GET:Authors/Edit/?id
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Category());
            else
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }
                return View(category);
            }
        }

        // POST: Categories/Add
        // POST:Authors/Edit/?id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    try
                    {
                        _context.Update(category);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CategoryExists(category.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.Authors.ToList()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", category) });
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.Categories.ToList()) });
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}

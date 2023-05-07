using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPTBook.Data;
using FPTBook.Models;
using FPTBook.Data.Services;
using Microsoft.AspNetCore.Authorization;
using FPTBook.Data.ViewModel;
using ReflectionIT.Mvc.Paging;

namespace FPTBook.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly IBooksService _service;
        private readonly AppDbContext _context;
        public BooksController(IBooksService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        [AllowAnonymous]
        //GET: Books/Index
        public async Task<IActionResult> Index(int page = 1, string sortExpression = "Title")
        {
            var lsCategories = _context.Categories
         .AsNoTracking()
         .ToList();
            ViewData["Categories"] = lsCategories;

            var query = _context.Books.AsNoTracking()
                .OrderBy(p => p.Id)
                .AsQueryable();
            var model = await PagingList.CreateAsync(query, 4, page, sortExpression, "Title");
            return View(model);
        }

        //GET: Books/Create
        public async Task<IActionResult> Create()
        {
            var bookDropdownsData = await _service.GetNewBookDropdownsValues();

            ViewBag.CategoryId = new SelectList(bookDropdownsData.Categories, "Id", "Name");
            ViewBag.PublisherId = new SelectList(bookDropdownsData.Publishers, "Id", "FullName");
            ViewBag.AuthorId = new SelectList(bookDropdownsData.Authors, "Id", "FullName");
            return View();
        }
        //POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewBookVM book)
        {
            if (!ModelState.IsValid)
            {
                var bookDropdownsData = await _service.GetNewBookDropdownsValues();

                ViewBag.CategoryId = new SelectList(bookDropdownsData.Categories, "Id", "Name");
                ViewBag.PublisherId = new SelectList(bookDropdownsData.Publishers, "Id", "FullName");
                ViewBag.AuthorId = new SelectList(bookDropdownsData.Authors, "Id", "FullName");

                return View(book);
            }

            await _service.AddNewBookAsync(book);
            return RedirectToAction(nameof(Index));
        }

        //GET: Books/Details/?id
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var bookDetail = await _service.GetBookByIdAsync(id);
            return View(bookDetail);
        }
        //GET: Books/Edit/?id
        public async Task<IActionResult> Edit(int id)
        {
            var bookDetails = await _service.GetBookByIdAsync(id);
            if (bookDetails == null) return View("NotFound");
            var response = new NewBookVM()
            {
                Title = bookDetails.Title,
                Description = bookDetails.Description,
                ISBN = bookDetails.ISBN,
                Price = bookDetails.Price,
                ProfilePictureURL = bookDetails.ProfilePictureURL,
                CategoryId = bookDetails.CategoryId,
                page_num = bookDetails.page_num,
                publication_date = bookDetails.publication_date,
                PublisherId = bookDetails.PublisherId,
                AuthorId = bookDetails.Author_Books.Select(n => n.AuthorId).ToList(),

            };

            var bookDropdownsData = await _service.GetNewBookDropdownsValues();
            ViewBag.CategoryId = new SelectList(bookDropdownsData.Categories, "Id", "Name");
            ViewBag.PublisherId = new SelectList(bookDropdownsData.Publishers, "Id", "FullName");
            ViewBag.AuthorId = new SelectList(bookDropdownsData.Authors, "Id", "FullName");

            return View(response);
        }
        //POST: Books/Edit/?id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NewBookVM book)
        {
            if (id != book.Id) return View("NotFound");

            if (!ModelState.IsValid)
            {
                var bookDropdownsData = await _service.GetNewBookDropdownsValues();
                ViewBag.CategoryId = new SelectList(bookDropdownsData.Categories, "Id", "Name");
                ViewBag.PublisherId = new SelectList(bookDropdownsData.Publishers, "Id", "FullName");
                ViewBag.AuthorId = new SelectList(bookDropdownsData.Authors, "Id", "FullName");

                return View(book);
            }

            await _service.UpdateBookAsync(book);
            return RedirectToAction(nameof(Index));
        }
        //POST: Books/Delete/?id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookDetail = await _service.GetBookByIdAsync(id);
            if (bookDetail == null) return View("NotFound");
            else await _service.DeleteBookByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [AllowAnonymous]
        //GET: Books/CatePro
        public async Task<IActionResult> CatePro()
        {
            var allBooks = await _service.GetAllAsync(n => n.Publisher, m => m.Category);
            var lsCategories = _context.Categories
      .AsNoTracking()
      .ToList();
            ViewData["Categories"] = lsCategories;
            return View(allBooks);
        }

        //POST: Books/BookCategory/?CategoryString
        [AllowAnonymous]
        public async Task<IActionResult> BookCategory(int id)
        {
            var allBooks = await _service.GetAllAsync(n => n.Publisher, m => m.Category);
            var lsCategories = _context.Categories
       .AsNoTracking()
       .ToList();
            ViewData["Categories"] = lsCategories;

            var Result = allBooks.Where(n => n.CategoryId == id);

            //var filteredResultNew = allBooks.Where(n => string.Equals(n.Title, searchString, StringComparison.CurrentCultureIgnoreCase) || string.Equals(n.Description, searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();

            return View("CatePro", Result);
        }
        //POST: Books/Filter/?filterString
        [AllowAnonymous]
        public async Task<IActionResult> Filter(string searchString)
        {
            var allBooks = await _service.GetAllAsync(n => n.Publisher, m => m.Category);

            if (!string.IsNullOrEmpty(searchString))
            {
                var filteredResult = allBooks.Where(n => n.Title.ToLower().Contains(searchString.ToLower()) || n.Description.ToLower().Contains(searchString.ToLower())).ToList();

                //var filteredResultNew = allBooks.Where(n => string.Equals(n.Title, searchString, StringComparison.CurrentCultureIgnoreCase) || string.Equals(n.Description, searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();

                return View("Index", filteredResult);
            }

            return View("Index", allBooks);
        }
    }
}
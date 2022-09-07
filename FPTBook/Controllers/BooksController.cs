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

namespace FPTBook.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBooksService _service;

        public BooksController(IBooksService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        //GET: Books/Index
        public async Task<IActionResult> Index()
        {
            var allBooks = await _service.GetAllAsync(n => n.Publisher,m=> m.Category);
            return View(allBooks);
        }

        //GET: Books/Create
        public async Task<IActionResult> Create()
        {
            var bookDropdownsData = await _service.GetNewMovieDropdownsValues();

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
                var bookDropdownsData = await _service.GetNewMovieDropdownsValues();

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

            var bookDropdownsData = await _service.GetNewMovieDropdownsValues();
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
                var bookDropdownsData = await _service.GetNewMovieDropdownsValues();
                ViewBag.CategoryId = new SelectList(bookDropdownsData.Categories, "Id", "Name");
                ViewBag.PublisherId = new SelectList(bookDropdownsData.Publishers, "Id", "FullName");
                ViewBag.AuthorId = new SelectList(bookDropdownsData.Authors, "Id", "FullName");

                return View(book);
            }

            await _service.UpdateBookAsync(book);
            return RedirectToAction(nameof(Index));
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookDetail = await _service.GetBookByIdAsync(id);
            if (bookDetail == null) return View("NotFound");
            else await _service.DeleteBookByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
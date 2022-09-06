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
            var movieDropdownsData = await _service.GetNewMovieDropdownsValues();

            ViewBag.CategoryId = new SelectList(movieDropdownsData.Categories, "Id", "Name");
            ViewBag.PublisherId = new SelectList(movieDropdownsData.Publishers, "Id", "FullName");
            ViewBag.AuthorId = new SelectList(movieDropdownsData.Authors, "Id", "FullName");
            return View();
        }
        //POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewBookVM book)
        {
            if (!ModelState.IsValid)
            {
                var movieDropdownsData = await _service.GetNewMovieDropdownsValues();

                ViewBag.CategoryId = new SelectList(movieDropdownsData.Categories, "Id", "Name");
                ViewBag.PublisherId = new SelectList(movieDropdownsData.Publishers, "Id", "FullName");
                ViewBag.AuthorId = new SelectList(movieDropdownsData.Authors, "Id", "FullName");

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
    }
}
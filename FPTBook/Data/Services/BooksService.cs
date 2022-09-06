using FPTBook.Data.Base;
using FPTBook.Data.ViewModel;
using FPTBook.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBook.Data.Services
{
    public class BooksService : EntityBaseRepository<Book>, IBooksService
    {
        private readonly AppDbContext _context;
        public BooksService(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<NewBookDropdownsVM> GetNewMovieDropdownsValues()
        {
            var response = new NewBookDropdownsVM()
            {
                Authors = await _context.Authors.OrderBy(n => n.FullName).ToListAsync(),
                Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync(),
                Publishers = await _context.Publishers.OrderBy(n => n.FullName).ToListAsync()
            };

            return response;
        }
        public async Task AddNewBookAsync(NewBookVM data)
        {
            var newBook = new Book()
            {
                Title = data.Title,
                Description = data.Description,
                ISBN = data.ISBN,
                Price = data.Price,
                ProfilePictureURL = data.ProfilePictureURL,
                CategoryId = data.CategoryId,
                page_num = data.page_num,
                publication_date = data.publication_date,
                PublisherId = data.PublisherId
            };
            await _context.Books.AddAsync(newBook);
            await _context.SaveChangesAsync();

            //Add Book Authors
            foreach (var authorId in data.AuthorId)
            {
                var newAuthorBook = new Author_Book()
                {
                    BookId = newBook.Id,
                    AuthorId = authorId
                };
                await _context.Author_Books.AddAsync(newAuthorBook);
            }
            await _context.SaveChangesAsync();
        }
        public async Task<Book> GetBookByIdAsync(int id)
        {
            var movieDetails = await _context.Books
                .Include(c => c.Category)
                .Include(p => p.Publisher)
                .Include(ab => ab.Author_Books).ThenInclude(a => a.Author)
                .FirstOrDefaultAsync(n => n.Id == id);

            return movieDetails;
        }
    }
}

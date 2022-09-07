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

        public async Task UpdateBookAsync(NewBookVM data)
        {
            var dbBook = await _context.Books.FirstOrDefaultAsync(n => n.Id == data.Id);

            if (dbBook != null)
            {
                dbBook.Title = data.Title;
                dbBook.Description = data.Description;
                dbBook.ISBN = data.ISBN;
                dbBook.Price = data.Price;
                dbBook.ProfilePictureURL = data.ProfilePictureURL;
                dbBook.CategoryId = data.CategoryId;
                dbBook.page_num = data.page_num;
                dbBook.publication_date = data.publication_date;
                dbBook.PublisherId = data.PublisherId;
                await _context.SaveChangesAsync();
            }

            //Remove existing actors
            var existingAuthorsDb = _context.Author_Books.Where(n => n.BookId == data.Id).ToList();
            _context.Author_Books.RemoveRange(existingAuthorsDb);
            await _context.SaveChangesAsync();

            //Add Movie Actors
            foreach (var authorId in data.AuthorId)
            {
                var newAuhtorBook = new Author_Book()
                {
                    BookId = data.Id,
                    AuthorId = authorId
                };
                await _context.Author_Books.AddAsync(newAuhtorBook);
            }
            await _context.SaveChangesAsync();
        }
        public async Task<Book> DeleteBookByIdAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return book;
        }
     }
}
using FPTBook.Data.Base;
using FPTBook.Data.ViewModel;
using FPTBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBook.Data.Services
{
    public interface IBooksService : IEntityBaseRepository<Book>
    {
        Task<NewBookDropdownsVM> GetNewMovieDropdownsValues();
        Task AddNewBookAsync(NewBookVM data);
        Task<Book> GetBookByIdAsync(int id);
        Task<Book> DeleteBookByIdAsync(int id);
        Task UpdateBookAsync(NewBookVM data);
    }
}

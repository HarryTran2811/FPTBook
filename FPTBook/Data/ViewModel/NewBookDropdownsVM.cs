using FPTBook.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBook.Data.ViewModel
{
    public class NewBookDropdownsVM
    {
        public NewBookDropdownsVM()
        {
            Publishers = new List<Publisher>();
            Categories = new List<Category>();
            Authors = new List<Author>();
        }

        public List<Publisher> Publishers { get; set; }
        public List<Category> Categories { get; set; }
        public List<Author> Authors { get; set; }
    }

}
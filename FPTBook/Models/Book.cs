using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBook.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Profile Picture")]
        [Required(ErrorMessage = "Profile Picture is required")]
        public string ProfilePictureURL { get; set; }

        [Display(Name = "ISBN")]
        [Required(ErrorMessage = "ISBN is required")]
        public int ISBN { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 chars")]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Display(Name = "Publication Date")]
        [DisplayFormat(DataFormatString = "{0:dddd, MMMM d, yyyy}")]
        public DateTime publication_date { get; set; }

        [Display(Name = "Number of Pages")]
        [Required(ErrorMessage = "This field is required")]
        public int page_num { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "Price is required")]
        public double Price { get; set; }

        //Relationships
        public List<Author_Book> Author_Books { get; set; }
        //Cinema
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        //Producer
        public int PublisherId { get; set; }
        [ForeignKey("PublisherId")]
        public Publisher Publisher { get; set; }
    }
}

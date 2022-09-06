using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBook.Data.ViewModel
{
    public class NewBookVM
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Profile Picture")]
        [Required(ErrorMessage = "Profile Picture is required")]
        public string ProfilePictureURL { get; set; }

        [Display(Name = "ISBN")]
        [Required(ErrorMessage = "ISBN is required")]
        [StringLength(17, MinimumLength = 3, ErrorMessage = "ISBN is invalid")]
        public string ISBN { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 chars")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Display(Name = "Publication Date")]
        [DisplayFormat(DataFormatString = "{0:dddd, MMMM d, yyyy}")]
        public DateTime publication_date { get; set; }

        [Display(Name = "Number of pages")]
        [Required(ErrorMessage = "This field is required")]
        public int page_num { get; set; }

        [Display(Name = "Price in $")]
        [Required(ErrorMessage = "Price is required")]
        public double Price { get; set; }

        [Display(Name = "Select author(s)")]
        [Required(ErrorMessage = "This field is required")]
        public List<int> AuthorId { get; set; }

        [Display(Name = "Select a category")]
        [Required(ErrorMessage = "This field is required")]
        public int CategoryId { get; set; }

        [Display(Name = "Select a publisher")]
        [Required(ErrorMessage = "This field is required")]
        public int PublisherId { get; set; }

    }
}

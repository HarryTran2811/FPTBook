using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBook.Models
{

    public class ApplicationUser : IdentityUser
    {
        [Column(TypeName = "text")]
        [Display(Name = "Full name")]
        [MaxLength(100)]
        public string FullName { set; get; }

        [Column(TypeName = "text")]
        [Display(Name = "Address")]
        [MaxLength(255)]
        public string? Address { set; get; }

        [DataType(DataType.Date)]
        public DateTime? Birthday { set; get; }

    }
}
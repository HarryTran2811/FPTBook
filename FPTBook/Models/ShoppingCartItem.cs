﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBook.Models
{
    public class ShoppingCartItem
    {
        [Key]
        public int Id { get; set; }

        public Book? Book { get; set; }
        public int Quantity { get; set; }


        public string? ShoppingCartId { get; set; }
    }
}

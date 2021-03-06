﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eLibraryShop.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int BookId { get; set; }

        public string BookTitle { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal Price { get; set; }

        public decimal Total { get { return Quantity * Price;} }

        public string Image { get; set; }

        public CartItem()
        {

        }

        public CartItem(Book book)
        {
            BookId = book.Id;
            BookTitle = book.Title;
            Price = book.Price;
            Quantity = 1;
            Image = book.Image;
        }
    }
}

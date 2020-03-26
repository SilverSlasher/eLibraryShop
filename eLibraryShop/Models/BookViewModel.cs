using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using eLibraryShop.Infrastructure;

namespace eLibraryShop.Models
{
    public class BookViewModel
    {
        public int Id { get; set; }


        [Display(Name = "Tytuł")]
        public string Title { get; set; }

        [Display(Name = "Autor")]
        public string Author { get; set; }

        [Display(Name = "Liczba stron")]
        public int Pages { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Display(Name = "Gatunek")]
        public string Genre { get; set; }

        [Display(Name = "Cena")]
        public decimal Price { get; set; }

        public string ReturnUrl { get; set; }

        public BookViewModel()
        {
            
        }

        public BookViewModel(Book book, string returnUrl)
        {
            Id = book.Id;
            Title = book.Title;
            Author = book.Author;
            Pages = book.Pages;
            Description = book.Description;
            Price = book.Price;
            Genre = book.Genre.Name;
            ReturnUrl = returnUrl;
        }
    }
}

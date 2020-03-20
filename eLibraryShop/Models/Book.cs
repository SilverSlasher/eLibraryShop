using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eLibraryShop.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace eLibraryShop.Models
{
    [Display(Name = "Książka")]
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pole jest obowiązkowe"), MinLength(2, ErrorMessage = "Tytuł książki musi zawierać minimum 2 znaki")]
        [Display(Name = "Tytuł")]
        public string Title { get; set; }

        public string Slug { get; set; }

        [Required(ErrorMessage = "Pole jest obowiązkowe")]
        [Display(Name = "Autor")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Pole jest obowiązkowe")]
        [PagesNumber]
        [Display(Name = "Liczba stron")]
        public int Pages { get; set; }

        [Required(ErrorMessage = "Pole jest obowiązkowe")]
        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Musisz wybrać kategorię")]
        [Display(Name = "Gatunek")]
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Pole jest obowiązkowe")]
        //Check if wrote number is correct for price
        [RegularExpression("(\\d+\\.\\d{2})",ErrorMessage = "Podaj poprawną cenę")]
        [Column(TypeName = "decimal(6,2)")]
        [Display(Name = "Cena")]
        public decimal Price { get; set; }

        [Display(Name = "Zdjęcie")]
        public string Image { get; set; }

        [ForeignKey("GenreId")]
        public virtual Genre Genre { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile ImageUpload { get; set; }

    }
}

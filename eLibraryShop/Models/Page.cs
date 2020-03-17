using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eLibraryShop.Models
{
    [Display(Name = "Karta")]
    public class Page
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pole jest obowiązkowe"),MinLength(2, ErrorMessage = "Tytuł musi zawierać minimum 2 znaki")] 
        [Display(Name = "Tytuł")]
        public string Title { get; set; }

        public string Slug { get; set; }

        [Required(ErrorMessage = "Pole jest obowiązkowe"), MinLength(5, ErrorMessage = "Opis musi zawierać minimum 5 znaków")] 
        [Display(Name = "Opis")]
        public string Content { get; set; }

        [Display(Name = "Pozycja")]
        public int Sorting { get; set; }
    }
}

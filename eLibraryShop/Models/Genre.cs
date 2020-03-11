using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eLibraryShop.Models
{
    [Display(Name = "Gatunek")]
    public class Genre
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pole jest obowiązkowe"), MinLength(2, ErrorMessage = "Nazwa gatunku musi zawierać minimum 2 znaki")]
        [RegularExpression(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$", ErrorMessage = "Nazwa gatunku może zawierać jedynie litery")]
        [Display(Name = "Gatunek")]
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
    }
}

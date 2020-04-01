using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eLibraryShop.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Pole jest obowiązkowe"), MinLength(5, ErrorMessage = "Nazwa użytkownika musi zawierać minimum 5 znaków")]
        [Display(Name = "Nazwa użytkownika")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Pole jest obowiązkowe")]
        [DataType(DataType.Password, ErrorMessage = "Wprowadź poprawne hasło")]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}

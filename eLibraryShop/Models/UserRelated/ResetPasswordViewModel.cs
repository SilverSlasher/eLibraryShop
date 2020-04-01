using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eLibraryShop.Models.UserRelated
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Pole jest obowiązkowe"), EmailAddress(ErrorMessage = "Wprowadź poprawny adres email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Pole jest obowiązkowe"), MinLength(6, ErrorMessage = "Hasło musi zawierać minimum 6 znaków")]
        [DataType(DataType.Password, ErrorMessage = "Wprowadź poprawne hasło")]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Pole jest obowiązkowe")]
        [DataType(DataType.Password, ErrorMessage = "Wprowadź poprawne hasło")]
        [Compare("Password", ErrorMessage = "Hasła muszą być identyczne")]
        [Display(Name = "Powtórz hasło")]
        public string RepeatPassword { get; set; }

        public string Token { get; set; }
    }
}

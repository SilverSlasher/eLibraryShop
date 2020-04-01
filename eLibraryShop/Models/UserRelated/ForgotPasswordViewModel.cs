using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eLibraryShop.Models.UserRelated
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Pole jest obowiązkowe"), EmailAddress(ErrorMessage = "Wprowadź poprawny adres email")]
        public string Email { get; set; }

        public bool IsExisting { get; set; }
    }
}

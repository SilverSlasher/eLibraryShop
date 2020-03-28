using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eLibraryShop.Models
{
    public class DeliveryAddress
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [Required(ErrorMessage = "Pole jest obowiązkowe")]
        [RegularExpression(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$", ErrorMessage = "Nazwa miasta może zawierać jedynie litery")]
        [Display(Name = "Miasto")]
        public string City { get; set; }

        [Required(ErrorMessage = "Pole jest obowiązkowe")]
        [Display(Name = "Ulica")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Pole jest obowiązkowe")]
        [RegularExpression("(\\d{2}\\-\\d{3})", ErrorMessage = "Podaj poprawny kod pocztowy w formacie XX-XXX")]
        [Display(Name = "Kod pocztowy")]
        public string ZIPCode { get; set; }

        public DeliveryAddress()
        {
            
        }

        public DeliveryAddress(AppUser appUser)
        {
            UserId = appUser.Id;
        }
    }
}

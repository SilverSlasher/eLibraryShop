using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eLibraryShop.Models.UserVariants
{
    public class UserViewModel
    {
        [Display(Name = "Nazwa użytkownika")]
        public string UserName { get; set; }

        public string Email { get; set; }

        public DeliveryAddress Address { get; set; }

        public UserViewModel(AppUser appUser)
        {
            UserName = appUser.UserName;
            Email = appUser.Email;
        }
    }
}

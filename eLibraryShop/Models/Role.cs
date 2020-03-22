using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eLibraryShop.Models
{
    [Display(Name = "Rola")]
    public class Role
    {
        [Required(ErrorMessage = "Pole jest obowiązkowe")]
        [MinLength(2, ErrorMessage = "Nazwa funkcji musi zawierać minimum 2 znaki")]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }
    }
}

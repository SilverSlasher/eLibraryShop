using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace eLibraryShop.Models
{
    public class AppUser : IdentityUser
    {
        public string Occupation { get; set; }
    }
}

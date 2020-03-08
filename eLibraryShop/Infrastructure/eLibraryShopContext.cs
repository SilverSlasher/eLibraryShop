using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eLibraryShop.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Page = eLibraryShop.Models.Page;


namespace eLibraryShop.Infrastructure
{
    public class eLibraryShopContext : DbContext
    {
        public eLibraryShopContext(DbContextOptions<eLibraryShopContext> options)
            :base(options)
        {
            
        }

        public DbSet<Page> Pages { get; set; }

        public DbSet<Genre> Genres { get; set; }

    }
}

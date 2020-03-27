using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eLibraryShop.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Page = eLibraryShop.Models.Page;


namespace eLibraryShop.Infrastructure
{
    public class eLibraryShopContext : IdentityDbContext<AppUser>
    {
        public eLibraryShopContext(DbContextOptions<eLibraryShopContext> options)
            :base(options)
        {
            
        }

        public DbSet<Page> Pages { get; set; }

        public DbSet<Genre> Genres { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<OrderItem> OrderItem { get; set; }

        public DbSet<Order> Order { get; set; }

    }
}

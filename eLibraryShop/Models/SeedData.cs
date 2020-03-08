﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eLibraryShop.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace eLibraryShop.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context =
                new eLibraryShopContext(serviceProvider.GetRequiredService<DbContextOptions<eLibraryShopContext>>()))
            {
                if (context.Pages.Any())
                {
                    return;
                }

                context.Pages.AddRange(
                    new Page
                    {
                        Title = "Home",
                        Slug = "home",
                        Content = "home page",
                        Sorting = 0
                    },
                    new Page
                    {
                        Title = "About us",
                        Slug = "about-us",
                        Content = "about us page",
                        Sorting = 100
                    },
                    new Page
                    {
                        Title = "Services",
                        Slug = "services",
                        Content = "services page",
                        Sorting = 100
                    },
                    new Page
                    {
                        Title = "Contact",
                        Slug = "contact",
                        Content = "contact page",
                        Sorting = 100
                    }
                    );

                context.SaveChanges();
            }
        }
    }
}
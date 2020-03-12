using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eLibraryShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eLibraryShop.Infrastructure
{
    public class GenresViewComponent : ViewComponent
    {
        private readonly eLibraryShopContext context;

        public GenresViewComponent(eLibraryShopContext context)
        {
            this.context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var genres = await GetGenresAsync();

            return View(genres);
        }

        private Task<List<Genre>> GetGenresAsync()
        {
            return context.Genres.OrderBy(x => x.Sorting).ToListAsync();
        }
    }
}
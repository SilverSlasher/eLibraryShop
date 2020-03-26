using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eLibraryShop.Infrastructure;
using eLibraryShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eLibraryShop.Controllers
{
    public class PagesController : Controller
    {

        private readonly eLibraryShopContext context;

        public PagesController(eLibraryShopContext context)
        {
            this.context = context;
        }

        //GET / or /slug
        public async Task<IActionResult> Page(string slug)
        {
            if (slug == null)
            {
                return View(await context.Pages.FirstOrDefaultAsync(x => x.Slug == "home"));
            }

            Page page = await context.Pages.FirstOrDefaultAsync(x => x.Slug == slug);

            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }
    }


}
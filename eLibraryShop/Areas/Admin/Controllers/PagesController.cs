using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eLibraryShop.Infrastructure;
using eLibraryShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eLibraryShop.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin,redaktor")]
    [Area("Admin")]
    public class PagesController : Controller
    {
        private readonly eLibraryShopContext context;

        public PagesController(eLibraryShopContext context)
        {
            this.context = context;
        }

        //GET /admin/pages
        public async Task<IActionResult> Index()
        {
            return View(await context.Pages.OrderBy(x => x.Sorting).ToListAsync());
        }

        //GET /admin/pages/details/id
        public async Task<IActionResult> Details(int id)
        {
            Page page = await context.Pages.FirstOrDefaultAsync(x => x.Id == id);

            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        //GET /admin/pages/create
        public IActionResult Create() => View();

        //Post /admin/pages/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Page page)
        {
            if (ModelState.IsValid)
            {
                page.Slug = page.Title.ToLower().Replace(" ", "-");
                page.Sorting = 100;

                var slug = await context.Pages.FirstOrDefaultAsync(x => x.Slug == page.Slug);

                if (slug != null)
                {
                    ModelState.AddModelError("", "Karta o podanym tytule już istnieje"); //Page already exists
                    return View(page);
                }

                context.Add(page);
                await context.SaveChangesAsync();

                TempData["Success"] = "Karta została utworzona"; //Page has been created

                return RedirectToAction("Index");
            }

            return View(page);
        }

        //GET /admin/pages/edit/id
        public async Task<IActionResult> Edit(int id)
        {
            Page page = await context.Pages.FirstOrDefaultAsync(x => x.Id == id);

            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        //Post /admin/pages/edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Page page)
        {
            if (ModelState.IsValid)
            {
                //Home page has to has "home" slug regardless of the display name
                page.Slug = page.Id == 1 ? "home" : page.Title.ToLower().Replace(" ", "-");

                var slug = await context.Pages.Where(x => x.Id != page.Id).FirstOrDefaultAsync(x => x.Slug == page.Slug);

                if (slug != null)
                {
                    ModelState.AddModelError("", "Karta o podanym tytule już istnieje"); //Page already exists
                    return View(page);
                }

                context.Update(page);
                await context.SaveChangesAsync();

                TempData["Success"] = "Karta została zedytowana pomyślnie"; //Page has been edited

                return RedirectToAction("Edit",new {id = page.Id});
            }

            return View(page);
        }


        //GET /admin/pages/delete/id
        public async Task<IActionResult> Delete(int id)
        {
            Page page = await context.Pages.FirstOrDefaultAsync(x => x.Id == id);

            if (page == null)
            {
                TempData["Error"] = "Karta nie istnieje"; //Page does not exist
            }
            else
            {
                context.Pages.Remove(page);
                await context.SaveChangesAsync();
                TempData["Success"] = "Karta została usunięta"; //Page has been deleted
            }

            return RedirectToAction("Index");
        }

        //Post /admin/pages/reorder
        [HttpPost]
        public async Task<IActionResult> Reorder(int[] id)
        {
            int count = 1;

            foreach (var pageId in id)
            {
                Page page = await context.Pages.FirstOrDefaultAsync(x => x.Id == pageId);
                page.Sorting = count;
                context.Update(page);
                await context.SaveChangesAsync();
                count++;
            }

            return Ok();
        }
    }
}
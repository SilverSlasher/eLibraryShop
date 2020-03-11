using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eLibraryShop.Infrastructure;
using eLibraryShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eLibraryShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GenresController : Controller
    {

        private readonly eLibraryShopContext context;

        public GenresController(eLibraryShopContext context)
        {
            this.context = context;
        }

        //GET /admin/genres
        public async Task<IActionResult> Index()
        {
            return View(await context.Genres.OrderBy(x => x.Sorting).ToListAsync());
        }

        //GET /admin/genres/create
        public IActionResult Create() => View();


        //Post /admin/genres/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Genre genre)
        {
            if (ModelState.IsValid)
            {
                genre.Slug = genre.Name.ToLower().Replace(" ", "-");
                genre.Sorting = 100;

                var slug = await context.Genres.FirstOrDefaultAsync(x => x.Slug == genre.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Gatunek o podanej nazwie już istnieje");
                    return View(genre);
                }

                context.Add(genre);
                await context.SaveChangesAsync();

                TempData["Success"] = "Gatunek został dodany";

                return RedirectToAction("Index");
            }

            return View(genre);
        }


        //GET /admin/genres/edit/id
        public async Task<IActionResult> Edit(int id)
        {
            Genre genre = await context.Genres.FirstOrDefaultAsync(x => x.Id == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        //Post /admin/genres/edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Genre genre)
        {
            if (ModelState.IsValid)
            {
                genre.Slug = genre.Name.ToLower().Replace(" ", "-");

                var slug = await context.Genres.Where(x => x.Id != genre.Id).FirstOrDefaultAsync(x => x.Slug == genre.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Gatunek o podanej nazwie już istnieje");
                    return View(genre);
                }

                context.Update(genre);
                await context.SaveChangesAsync();

                TempData["Success"] = "Gatunek został zedytowany pomyślnie";

                return RedirectToAction("Edit", new { id = genre.Id });
            }

            return View(genre);
        }


        //GET /admin/genres/delete/id
        public async Task<IActionResult> Delete(int id)
        {
            Genre genre = await context.Genres.FirstOrDefaultAsync(x => x.Id == id);
            if (genre == null)
            {
                TempData["Error"] = "Gatunek nie istnieje";
            }
            else
            {
                context.Genres.Remove(genre);
                await context.SaveChangesAsync();
                TempData["Success"] = "Gatunek został usunięty pomyślnie";
            }

            return RedirectToAction("Index");
        }


        //Post /admin/genres/reorder
        [HttpPost]
        public async Task<IActionResult> Reorder(int[] id)
        {
            int count = 1;

            foreach (var genreId in id)
            {
                Genre genre = await context.Genres.FirstOrDefaultAsync(x => x.Id == genreId);
                genre.Sorting = count;
                context.Update(genre);
                await context.SaveChangesAsync();
                count++;
            }

            return Ok();
        }
    }
}
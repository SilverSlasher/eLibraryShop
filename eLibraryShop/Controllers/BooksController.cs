using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eLibraryShop.Infrastructure;
using eLibraryShop.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eLibraryShop.Controllers
{
    public class BooksController : Controller
    {
        private readonly eLibraryShopContext context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public BooksController(eLibraryShopContext context)
        {
            this.context = context;
        }

        //GET /books
        public async Task<IActionResult> Index(int p = 1)
        {
            int pageSize = 6;
            var books = context.Books.OrderByDescending(x => x.Id).Skip((p - 1) * pageSize).Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)context.Books.Count() / pageSize);

            return View(await books.ToListAsync());
        }

        //GET /books/genre
        public async Task<IActionResult> BooksByGenre(string genreSlug, int p = 1)
        {
            Genre genre = await context.Genres.Where(x => x.Slug == genreSlug).FirstOrDefaultAsync();

            if (genre == null)
            {
              return  RedirectToAction("Index");
            }

            int pageSize = 6;
            var books = context.Books.OrderByDescending(x => x.Id).Where(x => x.GenreId == genre.Id).Skip((p - 1) * pageSize).Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)context.Books.Where(x => x.GenreId == genre.Id).Count() / pageSize);
            ViewBag.GenreName = genre.Name;
            ViewBag.CategorySlug = genreSlug;

            return View(await books.ToListAsync());
        }
    }
}
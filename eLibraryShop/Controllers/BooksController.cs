using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eLibraryShop.Infrastructure;
using eLibraryShop.Models;
using Microsoft.AspNetCore.Authorization;
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

            BooksGroupViewModel booksGroupVM = new BooksGroupViewModel(await books.ToListAsync(),
                                                                        p,
                                                                        pageSize, 
                                                                        (int)Math.Ceiling((decimal)context.Books.Count() / pageSize)
                                                                        );

            return View(booksGroupVM);
        }

        //GET /books/genre
        public async Task<IActionResult> BooksByGenre(string genreSlug, int p = 1)
        {
            Genre genre = await context.Genres.Where(x => x.Slug == genreSlug).FirstOrDefaultAsync();

            if (genre == null)
            {
                return RedirectToAction("Index");
            }

            int pageSize = 6;
            var books = context.Books.OrderByDescending(x => x.Id).Where(x => x.GenreId == genre.Id)
                .Skip((p - 1) * pageSize).Take(pageSize);

            BooksGroupViewModel booksGroupVM = new BooksGroupViewModel(await books.ToListAsync(),
                                                                        p,
                                                                        pageSize,
                                                                        (int)Math.Ceiling((decimal)context.Books.Count(x => x.GenreId == genre.Id) / pageSize),
                                                                        genre.Name,
                                                                        genreSlug
                                                                        );

            return View(booksGroupVM);
        }

        //GET /books/BookDescription/id
        public async Task<IActionResult> BookDescription(int id, string returnUrl, int pageNumber)
        {
            Book book = await context.Books.Include(x => x.Genre).FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            //Books are displayed in groups of 6, so it's important to get back to te right page number
            string _returnUrl = returnUrl + "?p=" + pageNumber;

            BookViewModel bookVM = new BookViewModel(book, _returnUrl);

            return View(bookVM);
        }
    }
}
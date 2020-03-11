using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eLibraryShop.Infrastructure;
using eLibraryShop.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eLibraryShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BooksController : Controller
    {
        private readonly eLibraryShopContext context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public BooksController(eLibraryShopContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        //GET /admin/books
        public async Task<IActionResult> Index()
        {
            return View(await context.Books.OrderByDescending(x => x.Id).Include(x => x.Genre).ToListAsync());
        }


        //GET /admin/books/create
        public IActionResult Create()
        {
            ViewBag.GenreId = new SelectList(context.Genres.OrderBy(x => x.Sorting), "Id", "Name");

            return View();
        }


        //Post /admin/books/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {

            ViewBag.GenreId = new SelectList(context.Genres.OrderBy(x => x.Sorting), "Id", "Name");

            if (ModelState.IsValid)
            {
                book.Slug = book.Title.ToLower().Replace(" ", "-");

                var slug = await context.Books.FirstOrDefaultAsync(x => x.Slug == book.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Książka o podanej nazwie już istnieje");
                    return View(book);
                }

                string imageName = "noimage.png";
                if (book.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "media/books");
                    imageName = Guid.NewGuid().ToString() + "_" + book.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await book.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                }

                book.Image = imageName;

                context.Add(book);
                await context.SaveChangesAsync();

                TempData["Success"] = "Książka została dodana";

                return RedirectToAction("Index");
            }

            return View(book);
        }
    }
}